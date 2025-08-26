using System.Collections.Generic;

public class TraitSpotBiome : TraitSpot
{
	public override int radius => 6;

	public override bool HaveUpdate => true;

	public override void Update()
	{
		if (EClass.rnd(10) != 0 || !EClass._zone.IsPCFactionOrTent || !owner.IsInstalled)
		{
			return;
		}
		Point randomPoint = GetRandomPoint();
		if (randomPoint.IsSky || randomPoint.HasObj || randomPoint.HasBlock || randomPoint.HasThing || randomPoint.cell.room != null)
		{
			return;
		}
		if (EClass.game.IsSurvival)
		{
			int num = 0;
			foreach (Thing thing2 in EClass._map.props.installed.things)
			{
				if (thing2.trait is TraitSpotBiome)
				{
					num++;
				}
			}
			int num2 = CountNotableThing();
			if (EClass.rnd(num * 10) == 0)
			{
				if (num2 < 2)
				{
					string text = ((EClass.rnd(10) == 0) ? "statue_god" : ((EClass.rnd(2) == 0) ? "statue_power" : "altar"));
					if (text == "altar" && EClass.rnd(20) == 0)
					{
						text = ((EClass.rnd(5) == 0) ? "altar_machine" : ((EClass.rnd(3) == 0) ? "altar_fox" : ((EClass.rnd(2) == 0) ? "altar_fox2" : "altar_strife")));
					}
					Thing thing = ThingGen.Create(text);
					thing.SetPriceFix(-100);
					EClass._zone.AddCard(thing, randomPoint).Install();
				}
				if (EClass.rnd(40) == 0 && EClass._map.FindChara("big_daddy") == null)
				{
					CardBlueprint.Set(new CardBlueprint
					{
						lv = EClass.game.survival.flags.raidLv + 10
					});
					Chara t = CharaGen.Create("big_daddy");
					EClass._zone.AddCard(t, randomPoint);
					Msg.Say("sign_bigdaddy");
				}
				return;
			}
		}
		if (EClass.rnd(5) == 0)
		{
			if (EClass.game.IsSurvival && EClass.game.survival.flags.raidLv < 20)
			{
				EClass._zone.SpawnMob(randomPoint, SpawnSetting.HomeWild(1));
			}
			else
			{
				EClass._zone.SpawnMob(randomPoint);
			}
		}
		else
		{
			randomPoint.cell.biome.Populate(randomPoint, interior: false, 100f);
		}
	}

	public int CountNotableThing(bool tryRemove = true)
	{
		int num = 0;
		List<Thing> listRemove = new List<Thing>();
		foreach (Point item in ListPoints(null, onlyPassable: false))
		{
			if (!item.HasThing)
			{
				continue;
			}
			foreach (Thing thing in item.Things)
			{
				if (!thing.IsInstalled)
				{
					continue;
				}
				if (thing.trait is TraitAltar)
				{
					num++;
					if (tryRemove && thing.c_priceFix == -100)
					{
						TryRemove(thing, 4);
					}
				}
				else if (thing.trait is TraitPowerStatue)
				{
					num++;
					if (tryRemove && !thing.isOn && thing.c_priceFix == -100)
					{
						TryRemove(thing, 2);
					}
				}
			}
		}
		if (tryRemove)
		{
			foreach (Thing item2 in listRemove)
			{
				item2.Destroy();
			}
		}
		return num;
		void TryRemove(Thing t, int h)
		{
			if (t.c_dateStockExpire == 0)
			{
				t.c_dateStockExpire = EClass.world.date.GetRaw(h);
			}
			else if (EClass.world.date.IsExpired(t.c_dateStockExpire))
			{
				listRemove.Add(t);
			}
		}
	}
}
