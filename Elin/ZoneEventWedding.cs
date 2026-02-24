using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class ZoneEventWedding : ZoneEventQuest
{
	[JsonProperty]
	public int attendees;

	[JsonProperty]
	public CharaMassTransfer transfer = new CharaMassTransfer();

	public QuestWedding questWedding => base.quest as QuestWedding;

	public override string TextWidgetDate => "eventWedding".lang((TimeLimit - minElapsed <= 30) ? "end_soon".lang() : "", attendees.ToString() ?? "");

	public override int TimeLimit => 180;

	public int MaxAttendee => 50;

	public override void OnVisit()
	{
		if (EClass.game.isLoading || (EClass.debug.enable && Input.GetKey(KeyCode.LeftShift)))
		{
			return;
		}
		foreach (Thing thing2 in EClass._map.things)
		{
			if (!thing2.isHidden)
			{
				thing2.isNPCProperty = true;
			}
		}
		EClass._zone.SetFieldEffect(10001, 1);
		List<Chara> list = new List<Chara>();
		foreach (Chara value in EClass.game.cards.globalCharas.Values)
		{
			if (!value.isDead && value != EClass.pc && value != questWedding.chara && value.IsPCFaction && value.currentZone != null && value.currentZone.IsPCFaction && value.conSuspend == null && !value.isRestrained && value.pos != null)
			{
				list.Add(value);
			}
		}
		list.Sort((Chara a, Chara b) => Mathf.Abs(b.affinity.value) - Mathf.Abs(a.affinity.value));
		if (list.Count > MaxAttendee)
		{
			list.RemoveRange(MaxAttendee, list.Count - MaxAttendee);
		}
		List<Thing> list2 = new List<Thing>();
		bool flag = EClass.pc.HasElement(1250);
		foreach (Thing thing3 in EClass._map.things)
		{
			if (thing3.HasElement(10))
			{
				thing3.isNPCProperty = false;
				thing3.c_priceFix = -90;
				thing3.SetBool(128, enable: true);
				if (flag)
				{
					thing3.elements.ModBase(710, 2);
				}
			}
			if (thing3.id == "1321")
			{
				list2.Add(thing3);
			}
		}
		list2.Sort((Thing a, Thing b) => a.pos.index - b.pos.index);
		foreach (Chara item in list)
		{
			if (list2.Count == 0)
			{
				break;
			}
			transfer.Add(item);
			attendees++;
			item.MoveZone(EClass._zone);
			Thing thing = list2.Last();
			list2.Remove(thing);
			item.noMove = false;
			item.turnLastSeen = -100;
			item.MoveImmediate(thing.pos, focus: false);
		}
	}

	public override void OnLeaveZone()
	{
		transfer.Restore();
	}

	public override ZoneInstance.Status OnReachTimeLimit()
	{
		Msg.Say("party_end3");
		return ZoneInstance.Status.Success;
	}
}
