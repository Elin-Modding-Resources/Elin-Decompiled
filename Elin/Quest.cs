using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class Quest : EClass
{
	public enum DifficultyType
	{
		Default,
		Deliver,
		Supply,
		Escort,
		Bulk,
		Meal,
		Farm,
		Music
	}

	public enum SubReward
	{
		plat,
		gacha_coin,
		money2,
		ticket_furniture
	}

	public const int PhaseComplete = 999;

	[JsonProperty]
	public string id;

	[JsonProperty]
	public int uid;

	[JsonProperty]
	public int uidClientZone;

	[JsonProperty]
	public int phase;

	[JsonProperty]
	public int lv;

	[JsonProperty]
	public int deadline;

	[JsonProperty]
	public int difficulty;

	[JsonProperty]
	public int rewardMoney;

	[JsonProperty]
	public int bonusMoney;

	[JsonProperty]
	public int startDate;

	[JsonProperty]
	public int dangerLv;

	[JsonProperty]
	public bool isNew;

	[JsonProperty]
	public bool isComplete;

	[JsonProperty]
	public bool track;

	[JsonProperty]
	public Person person;

	[JsonProperty]
	public QuestTask task;

	public RefChara refChara = new RefChara();

	public Zone ClientZone
	{
		get
		{
			Zone zone;
			if (chara == null || !chara.IsGlobal || chara.currentZone == null)
			{
				zone = RefZone.Get(uidClientZone);
				if (zone == null)
				{
					if (source.idZone.IsEmpty())
					{
						return EClass.game.StartZone;
					}
					return EClass.game.spatials.Find(source.idZone);
				}
			}
			else
			{
				zone = chara.currentZone;
			}
			return zone;
		}
	}

	public Chara chara => person.chara;

	public virtual SourceQuest.Row source => EClass.sources.quests.map[idSource];

	public bool Confetti => false;

	public bool IsExpired
	{
		get
		{
			if (deadline <= 0 || Hours >= 0)
			{
				return isComplete;
			}
			return true;
		}
	}

	public int Hours
	{
		get
		{
			if (deadline != 0)
			{
				return EClass.world.date.GetRemainingHours(deadline);
			}
			return -1;
		}
	}

	public string TextDeadline
	{
		get
		{
			if (deadline != 0)
			{
				return Date.GetText((Hours >= 0) ? Hours : 0);
			}
			return "dateDayVoid".lang();
		}
	}

	public virtual bool HasDLC => true;

	public virtual string idSource => id;

	public virtual string RewardSuffix => "";

	public virtual bool FameContent => false;

	public virtual string TextExtra => null;

	public virtual string TextExtra2
	{
		get
		{
			if (!FameContent)
			{
				return null;
			}
			return "dangerLv".lang(DangerLv.ToString() ?? "");
		}
	}

	public virtual int DangerLv => dangerLv;

	public virtual int AffinityGain => 20;

	public virtual int BaseMoney => source.money;

	public virtual int KarmaOnFail => 0;

	public virtual bool CanAbandon => IsRandomQuest;

	public virtual int FameOnComplete => 0;

	public virtual int RangeDeadLine => 0;

	public virtual bool UseInstanceZone => false;

	public virtual bool ForbidTeleport => false;

	public virtual bool TrackOnStart => true;

	public virtual bool RequireClientInSameZone => true;

	public virtual DifficultyType difficultyType => DifficultyType.Default;

	public virtual Chara DestChara => chara;

	public virtual string RefDrama1
	{
		get
		{
			if (task == null)
			{
				return "";
			}
			return task.RefDrama1;
		}
	}

	public virtual string RefDrama2
	{
		get
		{
			if (task == null)
			{
				return "";
			}
			return task.RefDrama2;
		}
	}

	public virtual string RefDrama3
	{
		get
		{
			if (task == null)
			{
				return "";
			}
			return task.RefDrama3;
		}
	}

	public virtual string RefDrama4 => "";

	public virtual string TitlePrefix => "";

	public virtual bool CanAutoAdvance => true;

	public virtual bool IsRandomQuest => false;

	public virtual string NameDeliver => "";

	public static Quest Create(string _id, string _idPerson = null, Chara c = null)
	{
		Quest quest = ClassCache.Create<Quest>(EClass.sources.quests.map[_id].type.IsEmpty("Quest"), "Elin");
		quest.id = _id;
		quest.person = new Person(_idPerson);
		if (quest is QuestDestZone { IsDeliver: not false } questDestZone)
		{
			Zone zone = ListDeliver().RandomItem();
			questDestZone.SetDest(zone, zone.dictCitizen.Keys.RandomItem());
		}
		if (c != null)
		{
			quest.SetClient(c);
		}
		quest.Init();
		return quest;
	}

	public static List<Zone> ListDeliver()
	{
		List<Zone> list = new List<Zone>();
		foreach (Zone zone in EClass.game.spatials.Zones)
		{
			if (zone != EClass._zone && zone.IsTown && !zone.IsClosed && !zone.IsPCFaction && zone.dictCitizen.Count > 0 && zone.CanBeDeliverDestination)
			{
				list.Add(zone);
			}
		}
		return list;
	}

	public virtual Thing GetDestThing()
	{
		return null;
	}

	public virtual int GetExtraMoney()
	{
		return 0;
	}

	public virtual int GetRewardPlat(int money)
	{
		return 1 + EClass.rnd(2) + EClass.rndHalf((int)Mathf.Sqrt(money / 200));
	}

	public override bool Equals(object obj)
	{
		return uid == (obj as Quest)?.uid;
	}

	public virtual bool IsVisibleOnQuestBoard()
	{
		if (RequireClientInSameZone && (chara == null || !chara.IsAliveInCurrentZone || chara.conSuspend != null))
		{
			return false;
		}
		if (EClass.world.date.GetRaw() < startDate || !CanStartQuest())
		{
			return false;
		}
		if ((chara.IsPCFaction || !(this is QuestRandom)) && !EClass._zone.IsPCFaction)
		{
			return false;
		}
		if (chara != null && chara.trait is TraitLoytel && chara.currentZone != null && !chara.currentZone.IsPCFaction)
		{
			return false;
		}
		return true;
	}

	public virtual bool CanStartQuest()
	{
		return true;
	}

	public virtual bool CanUpdateOnTalk(Chara c)
	{
		return false;
	}

	public virtual Zone CreateInstanceZone(Chara c)
	{
		return null;
	}

	public virtual bool UpdateOnTalk()
	{
		NextPhase();
		OnClickQuest();
		return true;
	}

	public void Init()
	{
		if (RangeDeadLine != 0)
		{
			deadline = EClass.world.date.GetRaw() + (36 + EClass.rnd(12)) * 60 + EClass.rnd(RangeDeadLine) * 1440;
		}
		uid = EClass.game.quests.uid;
		EClass.game.quests.uid++;
		isNew = true;
		track = TrackOnStart;
		switch (difficultyType)
		{
		case DifficultyType.Deliver:
		case DifficultyType.Supply:
		case DifficultyType.Escort:
			difficulty = Mathf.Clamp(6 - Hours / 48, 1, 6);
			break;
		case DifficultyType.Bulk:
			difficulty = Mathf.Clamp(EClass.rnd(EClass.rnd(((EClass._zone.branch == null) ? 1 : EClass._zone.branch.lv) + 3)) + 1, 1, 6);
			break;
		case DifficultyType.Farm:
			difficulty = 1 + EClass.rnd(3 + EClass.pc.Evalue(250) / 10);
			break;
		case DifficultyType.Music:
			difficulty = 1 + EClass.rnd(3 + EClass.pc.Evalue(241) / 10);
			break;
		default:
			difficulty = 3 + EClass.rnd(EClass.rnd(4)) - EClass.rnd(4);
			break;
		}
		difficulty = Mathf.Clamp(difficulty, 1, 7);
		OnInit();
		if (task != null)
		{
			task.OnInit();
		}
		int fameLv = EClass.pc.FameLv;
		dangerLv = Mathf.Max(fameLv + EClass.rnd(fameLv / 10 + 5) - EClass.rnd(fameLv / 10 + 5), 1);
		rewardMoney = (BaseMoney + EClass.rnd(BaseMoney / 4) + GetExtraMoney()) * (55 + difficulty * 15) / 100;
	}

	public virtual void OnInit()
	{
	}

	public Quest SetClient(Chara c, bool assignQuest = true)
	{
		if (c == null)
		{
			person = new Person();
			return this;
		}
		if (assignQuest)
		{
			c.quest = this;
		}
		if (c.currentZone != null)
		{
			uidClientZone = c.currentZone.uid;
		}
		person = new Person(c);
		return this;
	}

	public void SetTask(QuestTask _task)
	{
		task = _task;
		task.SetOwner(this);
	}

	public void Start()
	{
		OnStart();
		if (task != null)
		{
			task.OnStart();
		}
	}

	public virtual void OnStart()
	{
	}

	public virtual void OnEnterZone()
	{
	}

	public virtual void OnClickQuest()
	{
		if (!source.drama.IsEmpty())
		{
			LayerDrama.currentQuest = this;
			SoundManager.ForceBGM();
			chara.ShowDialog(source.drama[0], (source.drama.Length > 1) ? source.drama[1] : ("quest_" + source.id));
		}
		else
		{
			LayerDrama.currentQuest = this;
			LayerDrama.forceJump = "_quest";
			chara.ShowDialog();
		}
	}

	public void Fail()
	{
		EClass.Sound.Play("questFail");
		if (!(this is QuestTrackCraft))
		{
			EClass.player.ModFame(-10 - (int)((float)EClass.player.fame * 0.05f));
			EClass.player.ModKarma(KarmaOnFail);
		}
		EClass.game.quests.Remove(this);
		if (chara != null && chara.quest != null && chara.quest.uid == uid)
		{
			chara.quest = null;
		}
		if (ClientZone != null)
		{
			ClientZone.completedQuests.Add(uid);
		}
		OnFail();
	}

	public virtual void OnFail()
	{
	}

	public virtual void ShowCompleteText()
	{
		EClass.Sound.Play("questComplete");
		Msg.Say("completeQuest", GetTitle());
	}

	public void Complete()
	{
		OnBeforeComplete();
		EClass.game.quests.Remove(this);
		EClass.game.quests.completedIDs.Add(id);
		EClass.game.quests.completedTypes.Add(GetType().ToString());
		ShowCompleteText();
		OnDropReward();
		if (Confetti)
		{
			EClass.Sound.Play("confetti");
			ScreenEffect.Play("Firework");
		}
		if (chara != null && chara.quest != null && chara.quest.uid == uid)
		{
			chara.quest = null;
		}
		if (DestChara != null && DestChara.IsAliveInCurrentZone && AffinityGain != 0)
		{
			DestChara.ModAffinity(EClass.pc, AffinityGain);
		}
		if (ClientZone != null)
		{
			ClientZone.completedQuests.Add(uid);
		}
		OnComplete();
		EClass.player.ModKarma(1);
		isComplete = true;
	}

	public virtual void OnBeforeComplete()
	{
	}

	public virtual void OnDropReward()
	{
	}

	public Thing DropReward(string id)
	{
		return DropReward(ThingGen.Create(id));
	}

	public Thing DropReward(Thing t)
	{
		return EClass.player.DropReward(t);
	}

	public virtual void OnComplete()
	{
	}

	public void CompleteTask()
	{
		OnCompleteTask();
		task = null;
	}

	public virtual void OnCompleteTask()
	{
		EClass.game.quests.Complete(this);
	}

	public virtual void OnGiveItem(Chara c, Thing t)
	{
		if (task != null)
		{
			task.OnGiveItem(c, t);
			if (task.IsComplete())
			{
				CompleteTask();
			}
		}
	}

	public virtual void OnKillChara(Chara c)
	{
		if (task != null)
		{
			task.OnKillChara(c);
			if (task.IsComplete())
			{
				CompleteTask();
			}
		}
	}

	public virtual void OnModKarma(int a)
	{
		if (task != null)
		{
			task.OnModKarma(a);
			if (task.IsComplete())
			{
				CompleteTask();
			}
		}
	}

	public void NextPhase()
	{
		ChangePhase(phase + 1);
	}

	public void ChangePhase(int a)
	{
		phase = a;
		UpdateJournal();
		OnChangePhase(a);
	}

	public virtual void OnChangePhase(int a)
	{
	}

	public string Parse(string s)
	{
		if (s.IsEmpty())
		{
			return s;
		}
		GameLang.refDrama1 = RefDrama1.TagColor(FontColor.QuestObjective);
		GameLang.refDrama2 = RefDrama2.TagColor(FontColor.QuestObjective);
		GameLang.refDrama3 = RefDrama3.TagColor(FontColor.QuestObjective);
		GameLang.refDrama4 = RefDrama4.TagColor(FontColor.QuestObjective);
		Rand.UseSeed(uid, delegate
		{
			s = GameLang.ConvertDrama(s, chara);
			if (chara != null)
			{
				s = chara.ApplyTone(s);
			}
			else
			{
				s = Card.ApplyTone(chara, ref s, person.tones, person.gender);
			}
		});
		return s;
	}

	public virtual string GetTitle()
	{
		string s = "";
		Rand.UseSeed(uid, delegate
		{
			s = source.GetText().Split(Environment.NewLine.ToCharArray()).RandomItem();
		});
		return s;
	}

	public virtual string GetTrackerText()
	{
		string text = GetTextProgress();
		if (text.IsEmpty() && !IsRandomQuest)
		{
			text = GetDetail(onJournal: true);
		}
		return text;
	}

	public virtual string GetDetail(bool onJournal = false)
	{
		string detail = Parse(GetDetailText(onJournal));
		if (onJournal)
		{
			string textProgress = GetTextProgress();
			if (!textProgress.IsEmpty())
			{
				detail = detail + "\n\n" + textProgress;
			}
		}
		if (task != null)
		{
			task.OnGetDetail(ref detail, onJournal);
		}
		return GameLang.Convert(detail);
	}

	public virtual string GetDetailText(bool onJournal = false)
	{
		string s = "";
		Rand.UseSeed(uid, delegate
		{
			s = source.GetText("detail").Split(Environment.NewLine.ToCharArray()).RandomItem();
		});
		return s.ToTitleCase();
	}

	public virtual string GetTextProgress()
	{
		if (task != null)
		{
			return task.GetTextProgress();
		}
		return "";
	}

	public string GetRewardText()
	{
		string result = "-";
		if (rewardMoney != 0 || this is QuestRandom)
		{
			result = ("qReward" + RewardSuffix).lang(rewardMoney.ToString() ?? "");
		}
		return result;
	}

	public virtual string GetTalkProgress()
	{
		return Parse(source.GetText("talkProgress", returnNull: true));
	}

	public virtual string GetTalkComplete()
	{
		return Parse(source.GetText("talkComplete", returnNull: true));
	}

	public void UpdateJournal()
	{
		SE.WriteJournal();
		Msg.Say("journalUpdate2", GetTitle());
	}

	public virtual bool IsDeliverTarget(Chara c)
	{
		return false;
	}

	public virtual bool CanDeliverToClient(Chara c)
	{
		return false;
	}

	public virtual bool CanDeliverToBox(Thing t)
	{
		return false;
	}

	public virtual bool Deliver(Chara c, Thing t = null)
	{
		return false;
	}
}
