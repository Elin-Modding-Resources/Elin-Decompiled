using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZoneEventMusic : ZoneEventQuest
{
	public QuestMusic questMusic => base.quest as QuestMusic;

	public override string TextWidgetDate => "eventMusic".lang((TimeLimit - minElapsed <= 30) ? "end_soon".lang() : "", questMusic.score.ToString() ?? "", questMusic.destScore.ToString() ?? "");

	public override int TimeLimit => 180;

	public override void OnVisit()
	{
		if (EClass.game.isLoading || (EClass.debug.enable && Input.GetKey(KeyCode.LeftShift)))
		{
			return;
		}
		int difficulty = questMusic.difficulty;
		int maxLv = difficulty * 5 + 2;
		if (difficulty > 2)
		{
			maxLv += (difficulty - 2) * 10;
		}
		GenBounds genBounds = GenBounds.Create(EClass._zone);
		genBounds.marginPartial = 1;
		genBounds.FuncCheckEmpty = (Cell cell) => cell.sourceFloor.id == 76;
		int i;
		for (i = 0; i < 25; i++)
		{
			genBounds.TryAddMapPiece(MapPiece.Type.Concert, 0f, "", delegate(PartialMap p, GenBounds b)
			{
				int num = 1 + i * 3;
				if (num > maxLv)
				{
					num = EClass.rnd(maxLv);
				}
				List<Point> list = b.ListEmptyPoint();
				for (int k = 0; k < EClass.rndHalf(list.Count); k++)
				{
					if (list.Count == 0)
					{
						break;
					}
					Point point = list.RandomItem();
					Chara c4 = CharaGen.CreateFromFilter("c_neutral", num, 5);
					Spawn(c4, point);
					list.Remove(point);
				}
			});
		}
		foreach (SourceChara.Row item in EClass.sources.charas.rows.Where((SourceChara.Row c) => c.tag.Contains("party")))
		{
			if (item.LV >= maxLv * 2)
			{
				continue;
			}
			Point randomPoint = EClass._map.bounds.GetRandomPoint();
			if (randomPoint.HasChara || randomPoint.HasBlock)
			{
				continue;
			}
			bool flag = true;
			foreach (Chara member in EClass.pc.party.members)
			{
				if (member.id == item.id)
				{
					flag = false;
				}
			}
			if (flag)
			{
				Chara c2 = CharaGen.Create(item.id);
				Spawn(c2, randomPoint);
			}
		}
		for (int j = 0; j < 50; j++)
		{
			Point randomPoint2 = EClass._map.bounds.GetRandomPoint();
			if (!randomPoint2.HasChara && !randomPoint2.HasBlock)
			{
				Chara c3 = CharaGen.CreateFromFilter("c_neutral", maxLv / 2);
				Spawn(c3, randomPoint2);
			}
		}
		foreach (Thing thing in EClass._map.things)
		{
			thing.isNPCProperty = true;
		}
	}

	public void Spawn(Chara c, Point p)
	{
		EClass._zone.AddCard(c, p);
		if (c.hostility < Hostility.Neutral)
		{
			Hostility c_originalHostility = (c.hostility = Hostility.Neutral);
			c.c_originalHostility = c_originalHostility;
		}
	}

	public override ZoneInstance.Status OnReachTimeLimit()
	{
		Msg.Say("party_end", questMusic.score.ToString() ?? "");
		Msg.Say("party_end2", questMusic.sumMoney.ToString() ?? "");
		if (questMusic.score < questMusic.destScore)
		{
			return ZoneInstance.Status.Fail;
		}
		return ZoneInstance.Status.Success;
	}
}
