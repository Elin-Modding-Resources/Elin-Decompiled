using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class QuestDeliver : QuestDestZone
{
	[JsonProperty]
	public string idThing;

	[JsonProperty]
	public int num;

	private static List<SourceCategory.Row> _listDeliver = new List<SourceCategory.Row>();

	public virtual bool ConsumeGoods => true;

	public SourceThing.Row sourceThing => EClass.sources.things.map[idThing.IsEmpty("generator_snowman")];

	public override string NameDeliver => sourceThing.GetName();

	public override string RefDrama2 => NameDeliver;

	public override int KarmaOnFail
	{
		get
		{
			if (!IsDeliver)
			{
				return base.KarmaOnFail;
			}
			return -15;
		}
	}

	public override DifficultyType difficultyType => DifficultyType.Deliver;

	public override bool ForbidTeleport => true;

	public override int GetExtraMoney()
	{
		if (base.DestZone == null)
		{
			return 0;
		}
		return base.DestZone.Dist(base.ClientZone) * 6;
	}

	public override void OnInit()
	{
		num = GetDestNum();
		SetIdThing();
	}

	public virtual int GetDestNum()
	{
		return 1;
	}

	public virtual void SetIdThing()
	{
		CardRow cardRow;
		do
		{
			SourceCategory.Row r = GetDeliverCat();
			cardRow = SpawnListThing.Get("cat_" + r.id, (SourceThing.Row s) => EClass.sources.categories.map[s.category].IsChildOf(r.id)).Select();
		}
		while (cardRow == null);
		idThing = cardRow.id;
	}

	private SourceCategory.Row GetDeliverCat()
	{
		if (_listDeliver.Count == 0)
		{
			foreach (SourceCategory.Row row in EClass.sources.categories.rows)
			{
				if (row.deliver > 0)
				{
					_listDeliver.Add(row);
				}
			}
		}
		return _listDeliver.RandomItem();
	}

	public override void OnStart()
	{
		if (IsDeliver)
		{
			Thing thing = ThingGen.Create(idThing);
			thing.Identify(show: false, IDTSource.SuperiorIdentify);
			Msg.Say("get_quest_item");
			EClass.pc.Pick(thing);
		}
	}

	public virtual bool IsDestThing(Thing t)
	{
		if (t.parentCard != null && !t.parentCard.trait.CanUseContent)
		{
			return false;
		}
		if (!t.c_isImportant && t.Num >= num && t.IsIdentified && (!t.IsContainer || t.things.Count == 0) && !t.isEquipped)
		{
			if (t.id == idThing)
			{
				return true;
			}
			if (t.c_altName.IsEmpty())
			{
				string name = sourceThing.GetName();
				if (t.source.GetName() == name || t.GetName(NameStyle.Simple, 1) == name)
				{
					return true;
				}
			}
		}
		return false;
	}

	public List<Thing> ListDestThing(bool onlyFirst = false)
	{
		List<Thing> list = EClass.pc.things.List((Thing t) => IsDestThing(t), onlyAccessible: true);
		if (onlyFirst && list.Count > 0)
		{
			return list;
		}
		if (!IsDeliver && EClass._zone.IsPCFaction)
		{
			foreach (Thing thing in EClass._map.props.stocked.things)
			{
				if (IsDestThing(thing))
				{
					list.Add(thing);
					if (onlyFirst)
					{
						return list;
					}
				}
			}
		}
		return list;
	}

	public override Thing GetDestThing()
	{
		List<Thing> list = ListDestThing(onlyFirst: true);
		if (list.Count <= 0)
		{
			return null;
		}
		return list[0];
	}

	public override bool IsDeliverTarget(Chara c)
	{
		if (IsDeliver)
		{
			if (EClass._zone == base.DestZone)
			{
				return c.uid == uidTarget;
			}
			return false;
		}
		if (c.quest != null)
		{
			return c.quest.uid == uid;
		}
		return false;
	}

	public override bool CanDeliverToClient(Chara c)
	{
		if (GetDestThing() == null && !EClass.debug.autoAdvanceQuest)
		{
			return false;
		}
		if (IsDeliver)
		{
			if (EClass._zone == base.DestZone)
			{
				return c.uid == uidTarget;
			}
			return false;
		}
		if (person.chara?.uid == c.uid)
		{
			return true;
		}
		if (c.quest != null)
		{
			return c.quest.uid == uid;
		}
		return false;
	}

	public override bool CanDeliverToBox(Thing t)
	{
		return false;
	}

	public override bool Deliver(Chara c, Thing t = null)
	{
		if (t == null)
		{
			t = GetDestThing();
			if (t == null && EClass.debug.autoAdvanceQuest)
			{
				Debug.Log("[error] creating " + idThing);
				t = ThingGen.Create(idThing);
			}
		}
		if (t != null)
		{
			Thing thing = t.Split(num);
			bonusMoney += GetBonus(thing);
			Msg.Say("deliverItem", thing);
			if (ConsumeGoods)
			{
				thing.Destroy();
			}
			else
			{
				c.Pick(thing);
			}
			EClass.game.quests.Complete(this);
			c.quest = null;
			return true;
		}
		return false;
	}

	public virtual int GetBonus(Thing t)
	{
		return 0;
	}

	public override string GetTextProgress()
	{
		string text = ((GetDestThing() != null) ? "supplyInInv".lang().TagColor(FontColor.Good) : "supplyNotInInv".lang());
		if (IsDeliver)
		{
			string @ref = (base.DestZone.dictCitizen.TryGetValue(uidTarget) ?? "???") + " (" + base.DestZone.Name + ")";
			return "progressDeliver".lang(sourceThing.GetName(num), @ref, text);
		}
		return "progressSupply".lang(sourceThing.GetName(num) + Lang.space + TextExtra2.IsEmpty(""), text);
	}

	public override void OnEnterZone()
	{
		if (!IsDeliver || EClass._zone != base.DestZone)
		{
			return;
		}
		Chara chara = EClass._map.charas.Find((Chara c) => c.uid == uidTarget);
		if (chara == null)
		{
			chara = EClass._map.deadCharas.Find((Chara c) => c.uid == uidTarget);
		}
		if (chara == null)
		{
			Msg.Say("deliver_funny");
			Complete();
		}
	}
}
