using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

public class SurvivalManager : EClass
{
	public class Flags : EClass
	{
		[JsonProperty]
		public int[] ints = new int[50];

		public BitArray32 bits;

		public int spawnedFloor
		{
			get
			{
				return ints[3];
			}
			set
			{
				ints[3] = value;
			}
		}

		public int floors
		{
			get
			{
				return ints[4];
			}
			set
			{
				ints[4] = value;
			}
		}

		public int searchWreck
		{
			get
			{
				return ints[5];
			}
			set
			{
				ints[5] = value;
			}
		}

		public int dateNextRaid
		{
			get
			{
				return ints[6];
			}
			set
			{
				ints[6] = value;
			}
		}

		public int raidRound
		{
			get
			{
				return ints[7];
			}
			set
			{
				ints[7] = value;
			}
		}

		public int raidLv
		{
			get
			{
				return ints[8];
			}
			set
			{
				ints[8] = value;
			}
		}

		public bool raid
		{
			get
			{
				return bits[0];
			}
			set
			{
				bits[0] = value;
			}
		}

		public bool gotTaxChest
		{
			get
			{
				return bits[1];
			}
			set
			{
				bits[1] = value;
			}
		}

		public bool gotGaragara
		{
			get
			{
				return bits[2];
			}
			set
			{
				bits[2] = value;
			}
		}

		[OnSerializing]
		private void _OnSerializing(StreamingContext context)
		{
			ints[0] = (int)bits.Bits;
		}

		[OnDeserialized]
		private void _OnDeserialized(StreamingContext context)
		{
			bits.Bits = (uint)ints[0];
		}
	}

	[JsonProperty]
	public Flags flags = new Flags();

	[JsonProperty]
	public List<string> listReward = new List<string>();

	public bool IsInRaid => GetRaidEvent() != null;

	public ZoneEventRaid GetRaidEvent()
	{
		return EClass._zone.events.GetEvent<ZoneEventRaid>();
	}

	public void Meteor(Point pos, Action action)
	{
		EffectMeteor.Create(pos, 0, 1, delegate
		{
			action();
		});
	}

	public void MeteorThing(Point pos, string id, bool install = false)
	{
		Meteor(pos, delegate
		{
			Card card = EClass._zone.AddCard(ThingGen.Create(id), pos);
			if (card.id == "rod_wish")
			{
				card.SetCharge(1);
			}
			if (install)
			{
				card.Install();
			}
		});
	}

	public void MeteorThing(Point pos, Thing t, bool install = false)
	{
		Meteor(pos, delegate
		{
			Card card = EClass._zone.AddCard(t, pos);
			if (install)
			{
				card.Install();
			}
		});
	}

	public void RefreshRewards()
	{
		if (listReward.Count <= 0)
		{
			Add(new string[7] { "659", "758", "759", "806", "828", "1190", "1191" });
			if (flags.raidLv < 50)
			{
				Add(new string[7] { "pillow_jure", "pillow_ehekatl", "pillow_opatos", "pillow_kumiromi", "pillow_lulwy", "pillow_mani", "pillow_itzpalt" });
			}
			else
			{
				Add(new string[1] { "rod_wish" });
			}
		}
		void Add(string[] ids)
		{
			foreach (string item in ids)
			{
				listReward.Add(item);
			}
		}
	}

	public void OnExpandFloor(Point pos)
	{
		int i = 0;
		bool done = false;
		EClass._map.ForeachCell(delegate(Cell c)
		{
			if (!c.sourceFloor.tileType.IsSkipFloor)
			{
				i++;
			}
		});
		Check(6, delegate
		{
			EClass._zone.ClaimZone(debug: false, pos);
		});
		Check(9, delegate
		{
			EClass.pc.homeBranch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("shojo"), pos.x, pos.z).Chara);
		});
		Check(20, delegate
		{
			EClass.pc.homeBranch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("fiama"), pos.x, pos.z).Chara);
		});
		Check(40, delegate
		{
			EClass.pc.homeBranch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("nino"), pos.x, pos.z).Chara);
		});
		Check(60, delegate
		{
			EClass.pc.homeBranch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("loytel"), pos.x, pos.z).Chara);
			CheckLoytelDebt();
		});
		Check(80, delegate
		{
			EClass._zone.AddCard(ThingGen.Create("core_defense"), pos).Install();
		});
		void Check(int a, Action action)
		{
			if (!done && flags.floors < a && i >= a)
			{
				Meteor(pos, action);
				EClass.game.survival.flags.floors = a;
				done = true;
			}
		}
	}

	public void CheckLoytelDebt()
	{
		if (EClass.game.cards.globalCharas.Find("loytel") != null && !EClass.game.quests.IsAdded<QuestDebt>())
		{
			Quest quest = EClass.game.quests.Add("debt", "loytel");
			EClass.game.quests.globalList.Remove(quest);
			EClass.game.quests.Start(quest);
		}
	}

	public bool OnMineWreck(Point point)
	{
		if (EClass._zone.events.GetEvent<ZoneEventSurvival>() == null)
		{
			EClass._zone.events.Add(new ZoneEventSurvival());
		}
		CheckLoytelDebt();
		SourceObj.Row sourceObj = point.cell.sourceObj;
		int searchWreck = EClass.game.survival.flags.searchWreck;
		string[] array = new string[6] { "log", "rock", "branch", "bone", "grass", "vine" };
		int chanceChange = 25;
		int num = searchWreck / 50 + 1;
		if (searchWreck == 0)
		{
			Thing t2 = ThingGen.CreateParcel(null, ThingGen.Create("log").SetNum(6), ThingGen.Create("rock").SetNum(4), ThingGen.Create("resin").SetNum(2), ThingGen.Create("money2").SetNum(10), ThingGen.Create("1267"), ThingGen.CreateRod(50311, 8));
			Pop(t2);
		}
		switch (sourceObj.alias)
		{
		case "nest_bird":
			chanceChange = 65;
			return Pop(ThingGen.Create((EClass.rnd(10) == 0) ? "egg_fertilized" : "_egg").TryMakeRandomItem(num));
		case "wreck_wood":
			array = new string[8] { "log", "log", "branch", "grass", "vine", "resin", "leaf", "chunk" };
			break;
		case "wreck_junk":
			chanceChange = 100;
			return Pop(ThingGen.CreateFromFilter("shop_junk", num));
		case "wreck_stone":
			chanceChange = 30;
			array = new string[4] { "rock", "rock", "stone", "bone" };
			break;
		case "wreck_scrap":
			chanceChange = 75;
			array = new string[1] { "scrap" };
			break;
		case "wreck_cloth":
			chanceChange = 75;
			array = new string[1] { "fiber" };
			break;
		case "wreck_precious":
			chanceChange = 100;
			return Pop(ThingGen.CreateFromFilter("shop_magic", num));
		default:
			return false;
		}
		if (EClass.rnd(3 + EClass.game.survival.flags.spawnedFloor * 2) == 0 && EClass.game.survival.flags.spawnedFloor < 8)
		{
			EClass.game.survival.flags.spawnedFloor++;
			switch (EClass.game.survival.flags.spawnedFloor)
			{
			case 5:
			case 6:
				return Pop(ThingGen.CreateFloor(33, 97).SetNum(3));
			case 7:
			case 8:
				return Pop(ThingGen.CreateFloor(43, 66).SetNum(3));
			default:
				return Pop(ThingGen.CreateFloor(40, 45).SetNum(3));
			}
		}
		if (EClass.rnd(20) == 0)
		{
			return Pop(TraitSeed.MakeRandomSeed());
		}
		if (EClass.rnd(12 + num / 5) == 0)
		{
			return Pop(ThingGen.Create("money2"));
		}
		if (searchWreck > 10 && EClass.rnd(40 + num) == 0)
		{
			TreasureType treasureType = ((EClass.rnd(10) == 0) ? TreasureType.BossNefia : ((EClass.rnd(10) == 0) ? TreasureType.Map : TreasureType.RandomChest));
			Thing t3 = ThingGen.Create(treasureType switch
			{
				TreasureType.Map => "chest_treasure", 
				TreasureType.BossNefia => "chest_boss", 
				_ => "chest3", 
			});
			ThingGen.CreateTreasureContent(t3, num, treasureType, clearContent: true);
			MeteorThing(GetRandomPoint(), t3, install: true);
		}
		if (EClass.rnd(12) == 0)
		{
			Point pos = point.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false, ignoreCenter: true) ?? point;
			if (searchWreck < 100 || EClass.rnd(3) != 0)
			{
				EClass._zone.SpawnMob(pos, SpawnSetting.HomeWild(num));
			}
			else
			{
				EClass._zone.SpawnMob(pos, SpawnSetting.HomeEnemy(Mathf.Max(num - 5, 1)));
			}
		}
		return Pop(ThingGen.Create(array.RandomItem()).SetNum(1 + EClass.rnd(3)));
		bool Next()
		{
			EClass.game.survival.flags.searchWreck++;
			int searchWreck2 = EClass.game.survival.flags.searchWreck;
			Point pos2 = point.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false, ignoreCenter: true) ?? point;
			if (searchWreck2 == 20)
			{
				Thing container = ThingGen.CreateParcel(null, ThingGen.CreateRecipe("container_shipping"), ThingGen.CreateRecipe("stonecutter"), ThingGen.CreateRecipe("workbench2"));
				Meteor(pos2, delegate
				{
					EClass._zone.AddCard(container, pos2);
				});
			}
			if (searchWreck2 > (EClass.debug.enable ? 5 : 100) && !flags.gotTaxChest)
			{
				MeteorThing(pos2, "chest_tax");
				flags.gotTaxChest = true;
			}
			if (searchWreck2 > (EClass.debug.enable ? 10 : 200) && !flags.gotGaragara)
			{
				MeteorThing(pos2, "rolling_fortune");
				flags.gotGaragara = true;
			}
			NextObj();
			return true;
		}
		void NextObj()
		{
			if (EClass.rnd(100) < chanceChange)
			{
				string[] source = new string[12]
				{
					"nest_bird", "wreck_wood", "wreck_wood", "wreck_wood", "wreck_wood", "wreck_wood", "wreck_stone", "wreck_stone", "wreck_scrap", "wreck_junk",
					"wreck_cloth", "wreck_precious"
				};
				EClass._map.SetObj(point.x, point.z, EClass.sources.objs.alias[source.RandomItem()].id);
			}
		}
		bool Pop(Thing t)
		{
			EClass._map.TrySmoothPick(point, t, EClass.pc);
			Next();
			return true;
		}
	}

	public Point GetRandomPoint()
	{
		TraitCoreZone traitCoreZone = EClass._map.FindThing<TraitCoreZone>();
		Point point = ((traitCoreZone != null) ? traitCoreZone.owner.pos : EClass._map.GetCenterPos());
		List<Cell> list = new List<Cell>();
		EClass._map.ForeachSphere(point.x, point.z, 50f, delegate(Point p)
		{
			if (!p.IsSky && !p.IsBlocked && !p.HasObj && !p.HasThing)
			{
				list.Add(p.cell);
			}
		});
		if (list.Count == 0)
		{
			return null;
		}
		return list.RandomItem().GetPoint();
	}

	public List<SourceChara.Row> ListUnrecruitedUniques()
	{
		return EClass.sources.charas.rows.Where(delegate(SourceChara.Row r)
		{
			if (r.quality != 4 || r.race == "god" || r.size.Length != 0)
			{
				return false;
			}
			switch (r.id)
			{
			case "fiama":
			case "loytel":
			case "nino":
			case "big_daddy":
			case "littleOne":
				return false;
			default:
				if (EClass.game.cards.globalCharas.Find(r.id) == null)
				{
					return r.LV < flags.raidLv + 10;
				}
				return false;
			}
		}).ToList();
	}

	public void OnUpdateRecruit(FactionBranch branch)
	{
		List<SourceChara.Row> list = ListUnrecruitedUniques();
		for (int i = 0; i < (EClass.debug.enable ? 10 : 2); i++)
		{
			SourceChara.Row row = list.RandomItem();
			if (row != null)
			{
				Chara chara = CharaGen.Create(row.id);
				chara.RemoveEditorTag(EditorTag.AINoMove);
				branch.AddRecruit(chara);
				list.Remove(row);
			}
		}
	}

	public void StartRaid()
	{
		flags.raid = true;
		Point pos = EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false, ignoreCenter: true) ?? EClass.pc.pos;
		Meteor(pos, delegate
		{
			EClass._zone.AddCard(ThingGen.Create("teleporter_demon"), pos).Install();
		});
		flags.dateNextRaid = EClass.world.date.GetRaw(72);
	}
}
