using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneEventHarvest : ZoneEventQuest
{
	public QuestHarvest questHarvest => base.quest as QuestHarvest;

	public override string TextWidgetDate => "eventHarvest".lang((TimeLimit - minElapsed <= 30) ? "end_soon".lang() : "", Lang._weight(questHarvest.weightDelivered), Lang._weight(questHarvest.destWeight));

	public override int TimeLimit => 180;

	public override void OnVisit()
	{
		if (EClass.game.isLoading)
		{
			return;
		}
		EClass._zone.SetBGM(17);
		Point centerPos = EClass._map.GetCenterPos();
		PartialMap.Apply("Special/farm_chest.mp", centerPos);
		GenBounds genBounds = GenBounds.Create(EClass._zone);
		genBounds.marginPartial = 2;
		genBounds.FuncCheckEmpty = (Cell cell) => cell.sourceFloor.id == 42;
		List<SourceObj.Row> crops = EClass.sources.objs.rows.Where((SourceObj.Row o) => o.tag.Contains("harvest")).ToList();
		for (int num = 0; num < 50; num++)
		{
			genBounds.TryAddMapPiece(MapPiece.Type.Farm, 0f, "", delegate(PartialMap p, GenBounds b)
			{
				List<Point> list = b.ListEmptyPoint();
				SourceObj.Row row = crops.RandomItemWeighted((SourceObj.Row o) => o.chance);
				int num4 = 1 + EClass.rnd(5 + base.quest.difficulty * 2);
				foreach (Point item in list)
				{
					if (item.sourceFloor.id == 4 && EClass.rnd(4) != 0)
					{
						item.SetObj(row.id);
						int num5 = item.growth.HarvestStage - EClass.rnd(4);
						item.growth.SetStage(num5);
						if (num5 == item.growth.HarvestStage)
						{
							EClass._map.AddPlant(item, null).size = Mathf.Clamp(num4 + EClass.rnd(2) - EClass.rnd(2), 0, 9) + 1;
						}
						item.cell.isClearSnow = true;
					}
				}
			});
		}
		foreach (Thing thing in EClass._map.things)
		{
			thing.isNPCProperty = true;
		}
		for (int num2 = 0; num2 < 12; num2++)
		{
			EClass._zone.SpawnMob();
		}
		for (int num3 = 0; num3 < 30; num3++)
		{
			EClass._zone.SpawnMob(null, SpawnSetting.HomeWild(1));
		}
	}

	public override ZoneInstance.Status OnReachTimeLimit()
	{
		Msg.Say("harvest_end", Lang._weight(questHarvest.weightDelivered));
		if (questHarvest.weightDelivered < questHarvest.destWeight)
		{
			return ZoneInstance.Status.Fail;
		}
		return ZoneInstance.Status.Success;
	}

	public override void OnLeaveZone()
	{
		if (EClass._zone.instance.status == ZoneInstance.Status.Running)
		{
			EClass._zone.instance.status = OnReachTimeLimit();
		}
		List<Thing> list = new List<Thing>();
		foreach (Chara member in EClass.pc.party.members)
		{
			member.things.Foreach(delegate(Thing t)
			{
				if (t.GetBool(115) && EClass.rnd(2) != 0)
				{
					list.Add(t);
				}
			});
		}
		if (list.Count <= 0)
		{
			return;
		}
		Msg.Say("harvest_confiscate", list.Count.ToString() ?? "");
		foreach (Thing item in list)
		{
			item.Destroy();
		}
		EClass.player.ModKarma(-1);
	}
}
