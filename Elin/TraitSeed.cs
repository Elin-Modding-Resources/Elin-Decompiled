using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TraitSeed : Trait
{
	public static List<SourceObj.Row> listSeeds;

	public SourceObj.Row row => EClass.sources.objs.map[owner.refVal];

	public override int InstallBottomPriority => 10000;

	public override bool CanExtendBuild => true;

	public override bool CanChangeHeight => false;

	public override bool CanName => true;

	public override int DefaultStock => 3 + EClass.rnd(10);

	public override void OnCreate(int lv)
	{
		ApplySeed(owner.Thing, GetRandomSeedObj().id);
		owner.c_seed = EClass.rnd(10000);
	}

	public override void SetName(ref string s)
	{
		s = "_of".lang(row.GetName(), s);
	}

	public override void WriteNote(UINote n, bool identified)
	{
		base.WriteNote(n, identified);
		int num = 1;
		if (!row._growth.IsEmpty() && row._growth.Length >= 4)
		{
			if (row._growth.Length >= 5)
			{
				num = row._growth[4].ToInt();
			}
			n.AddText("isHarvestCrop".lang(num.ToString() ?? ""));
		}
		n.AddText("isConsumeFertility".lang((0.1f * (float)row.costSoil).ToString() ?? ""));
		if (row.tag.Contains("flood"))
		{
			n.AddText("isWaterCrop");
		}
		if (row.growth != null)
		{
			if (row.growth.GrowOnLand)
			{
				n.AddText("isGrowOnLand");
			}
			if (row.growth.GrowUndersea)
			{
				n.AddText("isGrowUndersea");
			}
			if (row.growth.NeedSunlight)
			{
				n.AddText("isNeedSun");
			}
		}
		if (row.growth == null || !row.growth.CanLevelSeed)
		{
			n.AddText("isDecoSeed", FontColor.Warning);
		}
	}

	public void TrySprout(bool force = false, bool sucker = false, VirtualDate date = null)
	{
		Point pos = owner.pos;
		if (!pos.HasObj && pos.cell.CanGrow(row, date ?? new VirtualDate()))
		{
			pos.SetObj(row.id, 1, owner.dir);
			EClass._map.AddPlant(pos, owner.Thing);
			if (sucker)
			{
				Zone.Suckers.Add(owner.Thing);
			}
			else
			{
				owner.Destroy();
			}
		}
	}

	public static Thing MakeSeed(SourceObj.Row obj, PlantData plant = null)
	{
		Thing thing = plant?.seed;
		if (EClass._zone.IsUserZone)
		{
			thing = null;
		}
		Thing thing2 = ThingGen.Create("seed");
		ApplySeed(thing2, obj.id);
		if (thing != null && obj.growth != null && obj.growth.CanLevelSeed)
		{
			foreach (Element value in thing.elements.dict.Values)
			{
				if (value.IsFoodTrait)
				{
					thing2.elements.SetTo(value.id, value.Value);
				}
			}
			thing2.SetEncLv(thing.encLV);
			thing2.elements.SetBase(2, EClass.curve(thing2.encLV, 50, 10, 80));
			thing2.c_refText = thing.c_refText;
			thing2.c_seed = thing.c_seed;
			int num = plant?.water ?? 0;
			int num2 = plant?.fert ?? 0;
			int num3 = 220 / (Mathf.Clamp(EClass.pc.Evalue(286) - thing.LV, 0, 50) * 2 + 10 + num * 2 + ((num2 > 0) ? 20 : 0) + (EClass.pc.HasElement(1325) ? 25 : 0));
			if (EClass.player.isAutoFarming)
			{
				num3 = 2 + num3 * 2;
			}
			if (EClass.rnd(num3) == 0 && EClass._zone.IsPCFactionOrTent)
			{
				int num4 = Mathf.Max(5, EClass.pc.Evalue(286)) - thing2.encLV;
				if (num4 <= 0)
				{
					if (!EClass.player.isAutoFarming)
					{
						Msg.Say("seedLvLimit", thing2);
					}
				}
				else
				{
					int num5 = Mathf.Clamp(EClass.rnd(num4) - 5, 1, EClass.player.isAutoFarming ? 3 : 10);
					LevelSeed(thing2, obj, num5);
					EClass.pc.PlaySound("seed_level");
				}
			}
			Rand.SetSeed();
		}
		thing2.SetBlessedState(BlessedState.Normal);
		return thing2;
	}

	public static void LevelSeed(Thing t, SourceObj.Row obj, int num)
	{
		if (obj == null || obj.growth == null || !obj.growth.CanLevelSeed)
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			if (obj == null || obj.objType == "crop")
			{
				if (t.encLV == 0)
				{
					CraftUtil.AddRandomFoodEnc(t);
				}
				else
				{
					Rand.SetSeed(t.c_seed);
					CraftUtil.ModRandomFoodEnc(t);
				}
			}
			t.ModEncLv(1);
		}
	}

	public static Thing MakeSeed(string idSource)
	{
		return MakeSeed(EClass.sources.objs.alias[idSource]);
	}

	public static Thing ApplySeed(Thing t, int refval)
	{
		t.refVal = refval;
		SourceObj.Row row = EClass.sources.objs.map.TryGetValue(refval);
		if (row != null && row.vals.Length != 0)
		{
			t.idSkin = row.vals[0].ToInt();
		}
		return t;
	}

	public static Thing MakeSeed(SourceObj.Row obj)
	{
		Thing thing = ThingGen.Create("seed");
		ApplySeed(thing, obj.id);
		return thing;
	}

	public static Thing MakeRandomSeed(bool enc = false)
	{
		Thing thing = ThingGen.Create("seed", null);
		SourceObj.Row randomSeedObj = GetRandomSeedObj();
		ApplySeed(thing, randomSeedObj.id);
		return thing;
	}

	public static SourceObj.Row GetRandomSeedObj()
	{
		if (listSeeds == null)
		{
			listSeeds = EClass.sources.objs.rows.Where((SourceObj.Row s) => s.HasTag(CTAG.seed) && !s.HasTag(CTAG.rareSeed)).ToList();
		}
		return listSeeds.RandomItemWeighted((SourceObj.Row a) => a.chance);
	}
}
