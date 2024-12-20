using System;
using System.Collections.Generic;
using UnityEngine;

public class ZonePreEnterEncounter : ZonePreEnterEvent
{
	public int enemies;

	public int roadDist;

	public Chara mob;

	public override void Execute()
	{
		bool flag = EClass.pc.HasCondition<ConDrawMetal>();
		int lv = Mathf.Max(EClass._zone.DangerLv, EClass.pc.FameLv * Math.Min(roadDist * 20, 100) / 100);
		if (mob != null)
		{
			List<Chara> list = new List<Chara>();
			Chara leader = null;
			float num = Mathf.Clamp(EClass.pc.FameLv + 8, 8f, 24f + Mathf.Sqrt(EClass.pc.FameLv));
			for (int i = 0; i < EClass.rndHalf((int)num); i++)
			{
				Point randomPointInRadius = EClass.pc.pos.GetRandomPointInRadius(2, 5, requireLos: false, allowChara: false);
				if (randomPointInRadius != null)
				{
					Chara chara = EClass._zone.SpawnMob(randomPointInRadius, SpawnSetting.Mob(mob.id, (mob.MainElement == Element.Void) ? null : mob.MainElement.source.alias.Substring(3)));
					Hostility hostility2 = (chara.c_originalHostility = Hostility.Enemy);
					chara.hostility = hostility2;
					chara.enemy = EClass.pc.party.members.RandomItem();
					leader = chara;
					list.Add(chara);
				}
			}
			if (leader != null)
			{
				List<Thing> list2 = EClass.pc.things.List(delegate(Thing t)
				{
					if (t.Num >= 10)
					{
						return false;
					}
					return t.trait.CanBeDestroyed && t.things.Count == 0 && t.invY != 1 && t.trait.CanBeStolen && !t.trait.CanOnlyCarry && !t.IsUnique && !t.isEquipped;
				}, onlyAccessible: true);
				Thing t2 = ((list2.Count > 0) ? list2.RandomItem() : null);
				if (t2 == null)
				{
					GameLang.refDrama1 = (GameLang.refDrama2 = "mobPity".lang());
				}
				else
				{
					GameLang.refDrama1 = t2.NameSimple;
					GameLang.refDrama2 = t2.Name;
				}
				LayerDrama.refAction1 = delegate
				{
					foreach (Chara item in list)
					{
						item.ShowEmo(Emo.angry);
						if (EClass.rnd(6) == 0)
						{
							item.Talk((EClass.rnd(5) == 0) ? "rumor_bad" : ((EClass.rnd(5) == 0) ? "callGuards" : "disgust"));
						}
					}
				};
				LayerDrama.refAction2 = delegate
				{
					if (t2 != null)
					{
						leader.AddCard(t2);
					}
					foreach (Chara item2 in list)
					{
						if (EClass.rnd(6) == 0)
						{
							item2.Talk((EClass.rnd(5) == 0) ? "rumor_good" : ((EClass.rnd(3) == 0) ? "thanks3" : "thanks"));
						}
						item2.ShowEmo(Emo.happy);
						Hostility hostility5 = (item2.c_originalHostility = Hostility.Neutral);
						item2.hostility = hostility5;
						item2.enemy = null;
					}
					EClass.player.ModKarma(1);
				};
				leader.ShowDialog("_chara", "encounter_mob");
			}
		}
		else
		{
			for (int j = 0; j < enemies; j++)
			{
				Point nearestPoint = (EClass.pc.pos.GetRandomPoint(4) ?? EClass.pc.pos).GetNearestPoint(allowBlock: false, allowChara: false);
				Chara chara2 = EClass._zone.SpawnMob(nearestPoint, SpawnSetting.Encounter(lv));
				Hostility hostility2 = (chara2.c_originalHostility = Hostility.Enemy);
				chara2.hostility = hostility2;
				chara2.enemy = EClass.pc.party.members.RandomItem();
			}
		}
		if (flag && EClass.rnd(EClass.debug.enable ? 1 : 3) == 0)
		{
			Point nearestPoint2 = (EClass.pc.pos.GetRandomPoint(4) ?? EClass.pc.pos).GetNearestPoint(allowBlock: false, allowChara: false);
			SpawnList list3 = SpawnListChara.Get("c_metal", (SourceChara.Row s) => s.race == "metal");
			EClass._zone.AddCard(CharaGen.CreateFromFilter(list3, EClass._zone.DangerLv), nearestPoint2);
		}
		if ((EClass._zone.Tile.isRoad || EClass._zone.Tile.IsNeighborRoad) && EClass.rnd(2) == 0)
		{
			Point nearestPoint3 = (EClass.pc.pos.GetRandomPoint(4) ?? EClass.pc.pos).GetNearestPoint(allowBlock: false, allowChara: false);
			EClass._zone.AddCard(CharaGen.Create("guard"), nearestPoint3);
		}
	}
}
