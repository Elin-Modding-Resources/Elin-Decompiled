using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class QuestManager : EClass
{
	public const int MaxRandomQuest = 5;

	[JsonProperty]
	public List<Quest> list = new List<Quest>();

	[JsonProperty]
	public List<Quest> globalList = new List<Quest>();

	[JsonProperty]
	public HashSet<string> completedIDs = new HashSet<string>();

	[JsonProperty]
	public HashSet<string> completedTypes = new HashSet<string>();

	[JsonProperty]
	public int uid;

	public QuestMain Main => Get<QuestMain>();

	public new QuestHome Home => Get<QuestHome>();

	public Quest Add(string id, string idGlobalChara = null)
	{
		Quest quest = Quest.Create(id);
		if (idGlobalChara.IsEmpty())
		{
			idGlobalChara = quest.source.drama[0];
		}
		quest.SetClient(EClass.game.cards.globalCharas.Find(idGlobalChara), assignQuest: false);
		EClass.game.quests.globalList.Add(quest);
		return quest;
	}

	public Quest Start(string id, string idGlobalChara)
	{
		Quest q = Quest.Create(id).SetClient(EClass.game.cards.globalCharas.Find(idGlobalChara), assignQuest: false);
		return Start(q);
	}

	public Quest Start(string id, Chara c = null, bool assignQuest = true)
	{
		return Start(Quest.Create(id).SetClient(c, assignQuest));
	}

	public Quest Start(Quest q)
	{
		list.Insert(0, q);
		q.Start();
		if (EClass.core.IsGameStarted)
		{
			q.UpdateJournal();
			if (EClass.player.questTracker)
			{
				WidgetQuestTracker.Show();
			}
		}
		return q;
	}

	public void Remove(Quest q)
	{
		if (!list.Remove(q))
		{
			Debug.Log("exception: Failed to remove quest:" + q?.ToString() + "/" + q.uid + "/" + q.source.id + "/" + q.deadline + "/" + q.Hours);
			q = Get(q.uid);
			Debug.Log(q);
			if (q != null && q.IsRandomQuest)
			{
				list.Remove(q);
			}
		}
	}

	public void RemoveGlobal(Quest q)
	{
		globalList.Remove(q);
	}

	public void RemoveAll(Chara c)
	{
		foreach (Quest item in list.Where((Quest a) => a.chara == c).ToList())
		{
			Remove(item);
		}
		foreach (Quest item2 in globalList.Where((Quest a) => a.chara == c).ToList())
		{
			RemoveGlobal(item2);
		}
	}

	public void Complete(Quest q)
	{
		q.Complete();
	}

	public bool OnShowDialog(Chara c)
	{
		foreach (Quest item in list)
		{
			if (item.person.chara == c && (item.CanUpdateOnTalk(c) || (item.CanAutoAdvance && EClass.debug.autoAdvanceQuest)))
			{
				return item.UpdateOnTalk();
			}
		}
		return false;
	}

	public void OnAdvanceHour()
	{
		list.ForeachReverse(delegate(Quest q)
		{
			if (q.IsExpired)
			{
				Msg.Say("questExpired", q.GetTitle());
				q.Fail();
			}
		});
	}

	public bool IsAdded<T>() where T : Quest
	{
		if (IsStarted<T>() || GetPhase<T>() == 999)
		{
			return true;
		}
		foreach (Quest global in globalList)
		{
			if (global is T)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsStarted<T>() where T : Quest
	{
		return GetPhase<T>() != -1;
	}

	public int GetPhase<T>() where T : Quest
	{
		if (completedTypes.Contains(typeof(T).ToString()))
		{
			return 999;
		}
		foreach (Quest item in list)
		{
			if (item is T)
			{
				return item.phase;
			}
		}
		return -1;
	}

	public T Get<T>() where T : Quest
	{
		foreach (Quest item in list)
		{
			if (item is T)
			{
				return item as T;
			}
		}
		return null;
	}

	public Quest Get(string id)
	{
		foreach (Quest item in list)
		{
			if (item.GetType().ToString() == id || item.id == id)
			{
				return item;
			}
		}
		return null;
	}

	public Quest Get(int uid)
	{
		foreach (Quest item in list)
		{
			if (item.uid == uid)
			{
				return item;
			}
		}
		return null;
	}

	public Quest GetGlobal(string id)
	{
		foreach (Quest global in globalList)
		{
			if (global.GetType().ToString() == id || global.id == id)
			{
				return global;
			}
		}
		return null;
	}

	public bool IsCompleted(string id)
	{
		return completedIDs.Contains(id);
	}

	public int CountNew()
	{
		int num = 0;
		foreach (Quest item in list)
		{
			if (item.isNew)
			{
				num++;
			}
		}
		return num;
	}

	public int CountRandomQuest()
	{
		int num = 0;
		foreach (Quest item in list)
		{
			if (item is QuestRandom)
			{
				num++;
			}
		}
		return num;
	}

	public void UpdateJournal()
	{
		SE.WriteJournal();
		Msg.Say("journalUpdate");
	}

	public bool IsDeliverTarget(Chara c)
	{
		foreach (Quest item in list)
		{
			if (item.IsDeliverTarget(c))
			{
				return true;
			}
		}
		return false;
	}

	public void AddQuestAsh()
	{
		Chara c = EClass.game.cards.globalCharas.Find("ashland");
		QuestCraft obj = Quest.Create("ash1") as QuestCraft;
		obj.SetClient(c);
		obj.req1.Add(new QuestCraft.Req("rock", 1));
		obj.req1.Add(new QuestCraft.Req("branch", 1));
		obj.req2.Add(new QuestCraft.Req("axe", 1));
	}

	public void AddQuestFiama()
	{
		Chara c = EClass.game.cards.globalCharas.Find("fiama");
		QuestCraft obj = Quest.Create("fiama1") as QuestCraft;
		obj.SetClient(c);
		obj.req1.Add(new QuestCraft.Req("crim", 3));
		obj.req2.Add(new QuestCraft.Req("hotcrim", 1));
	}

	public bool HasFarAwayEscort(bool execute = false)
	{
		bool has = false;
		if (EClass._zone.IsTown || EClass._zone.IsPCFaction || EClass._zone.HasLaw || EClass._zone.IsInstance)
		{
			return false;
		}
		EClass._map.charas.ForeachReverse(delegate(Chara m)
		{
			if (m.IsEscorted() && EClass.pc.Dist(m) >= 5)
			{
				has = true;
				if (execute)
				{
					m.Destroy();
				}
			}
		});
		if (has && execute)
		{
			OnEnterZone();
		}
		return has;
	}

	public void OnEnterZone()
	{
		list.ForeachReverse(delegate(Quest q)
		{
			q.OnEnterZone();
		});
	}
}
