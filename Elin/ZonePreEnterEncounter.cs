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
			if (mob.trait is TraitMerchantTravel)
			{
				lv = Mathf.Max(20, EClass.pc.FameLv * 2);
				Point randomPointInRadius = EClass.pc.pos.GetRandomPointInRadius(2, 5, requireLos: false, allowChara: false);
				for (int i = 0; i < EClass.rndHalf(12); i++)
				{
					Point randomPointInRadius2 = randomPointInRadius.GetRandomPointInRadius(1, 4, requireLos: false, allowChara: false);
					if (randomPointInRadius2 != null)
					{
						Chara chara = null;
						if (i == 0)
						{
							chara = (leader = EClass._zone.SpawnMob(randomPointInRadius2, SpawnSetting.TravelMerchant(mob.id, lv)));
						}
						else
						{
							string[] source = new string[5] { "merc", "merc_archer", "merc_mage", "merc_warrior", "dog_hound" };
							chara = EClass._zone.SpawnMob(randomPointInRadius2, SpawnSetting.Mob(source.RandomItem(), null, lv * 2 / 3));
							Chara chara2 = chara;
							Hostility hostility = (chara.c_originalHostility = Hostility.Neutral);
							chara2.hostility = hostility;
							chara.MakeMinion(leader);
						}
						list.Add(chara);
					}
				}
				leader.ShowDialog();
			}
			else
			{
				float num = Mathf.Clamp(EClass.pc.FameLv + 8, 8f, Mathf.Min(24f + Mathf.Sqrt(EClass.pc.FameLv), 100f));
				for (int j = 0; j < EClass.rndHalf((int)num); j++)
				{
					Point randomPointInRadius3 = EClass.pc.pos.GetRandomPointInRadius(2, 5, requireLos: false, allowChara: false);
					if (randomPointInRadius3 != null)
					{
						bool flag2 = EClass.rnd(5) == 0;
						Chara chara3 = EClass._zone.SpawnMob(randomPointInRadius3, SpawnSetting.Mob(mob.id, (mob.MainElement == Element.Void) ? null : mob.MainElement.source.alias.Substring(3), -1, (!flag2 && EClass.player.HasKeyItem("license_adv")) ? EClass.rndHalf(EClass.pc.FameLv) : 0));
						Hostility hostility = (chara3.c_originalHostility = Hostility.Enemy);
						chara3.hostility = hostility;
						chara3.enemy = EClass.pc.party.members.RandomItem();
						leader = chara3;
						if (flag2)
						{
							TraitFoodEggFertilized.MakeBaby(chara3, 1);
						}
						list.Add(chara3);
					}
				}
				if (leader != null)
				{
					List<Thing> list2 = EClass.pc.things.List(delegate(Thing thing)
					{
						if (thing.Num >= 10)
						{
							return false;
						}
						return thing.trait.CanBeDestroyed && thing.things.Count == 0 && thing.invY != 1 && thing.trait.CanBeStolen && !thing.trait.CanOnlyCarry && !thing.IsUnique && !thing.isEquipped;
					}, onlyAccessible: true);
					Thing t = ((list2.Count > 0) ? list2.RandomItem() : null);
					if (t == null)
					{
						GameLang.refDrama1 = (GameLang.refDrama2 = "mobPity".lang());
					}
					else
					{
						GameLang.refDrama1 = t.NameSimple;
						GameLang.refDrama2 = t.Name;
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
						if (t != null)
						{
							leader.AddCard(t);
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
		}
		else
		{
			for (int num2 = 0; num2 < enemies; num2++)
			{
				Point nearestPoint = (EClass.pc.pos.GetRandomPoint(4) ?? EClass.pc.pos).GetNearestPoint(allowBlock: false, allowChara: false);
				Chara chara4 = EClass._zone.SpawnMob(nearestPoint, SpawnSetting.Encounter(lv));
				Hostility hostility = (chara4.c_originalHostility = Hostility.Enemy);
				chara4.hostility = hostility;
				chara4.enemy = EClass.pc.party.members.RandomItem();
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
