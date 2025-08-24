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
		Check(9, delegate
		{
			EClass._zone.ClaimZone(debug: false, pos);
		});
		Check(15, delegate
		{
			EClass.pc.homeBranch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("fiama"), pos.x, pos.z).Chara);
		});
		Check(25, delegate
		{
			EClass.pc.homeBranch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("nino"), pos.x, pos.z).Chara);
		});
		Check(40, delegate
		{
			EClass.pc.homeBranch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("loytel"), pos.x, pos.z).Chara);
		});
		Check(50, delegate
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

	public bool OnMineWreck(Point point)
	{
		if (EClass._zone.events.GetEvent<ZoneEventSurvival>() == null)
		{
			EClass._zone.events.Add(new ZoneEventSurvival());
		}
		SourceObj.Row sourceObj = point.cell.sourceObj;
		int searchWreck = EClass.game.survival.flags.searchWreck;
		string[] array = new string[6] { "log", "rock", "branch", "bone", "grass", "vine" };
		int chanceChange = 25;
		int num = searchWreck / 50 + 3;
		if (searchWreck == 0)
		{
			Pop(ThingGen.Create("log").SetNum(6));
			Pop(ThingGen.Create("rock").SetNum(4));
		}
		switch (sourceObj.alias)
		{
		case "nest_bird":
			chanceChange = 100;
			return Pop(ThingGen.Create((EClass.rnd(10) == 0) ? "egg_fertilized" : "_egg").TryMakeRandomItem(num));
		case "wreck_wood":
			array = new string[6] { "log", "log", "branch", "grass", "vine", "resin" };
			break;
		case "wreck_junk":
			chanceChange = 50;
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
		if (EClass.rnd(3) == 0 && EClass.game.survival.flags.spawnedFloor < 4)
		{
			EClass.game.survival.flags.spawnedFloor++;
			return Pop(ThingGen.CreateFloor(40, 45).SetNum(3));
		}
		if (EClass.rnd(20) == 0)
		{
			return Pop(TraitSeed.MakeRandomSeed());
		}
		if (EClass.rnd(12) == 0)
		{
			return Pop(ThingGen.Create("money2"));
		}
		if (EClass.rnd(12) == 0)
		{
			Point pos = point.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false, ignoreCenter: true) ?? point;
			if (searchWreck < 50 || EClass.rnd(3) != 0)
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
			Point pos2 = point.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false, ignoreCenter: true) ?? point;
			if (EClass.game.survival.flags.searchWreck == 20)
			{
				Meteor(pos2, delegate
				{
					EClass._zone.AddCard(ThingGen.CreateRecipe("container_shipping"), pos2);
				});
			}
			NextObj();
			return true;
		}
		void NextObj()
		{
			if (EClass.rnd(100) < chanceChange)
			{
				string[] source = new string[11]
				{
					"nest_bird", "wreck_wood", "wreck_wood", "wreck_wood", "wreck_wood", "wreck_stone", "wreck_stone", "wreck_scrap", "wreck_junk", "wreck_cloth",
					"wreck_precious"
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
				return EClass.game.cards.globalCharas.Find(r.id) == null;
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
		SE.Play("warhorn");
		Msg.Say("warhorn");
		flags.raid = true;
		Point pos = EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false, ignoreCenter: true) ?? EClass.pc.pos;
		Meteor(pos, delegate
		{
			EClass._zone.AddCard(ThingGen.Create("teleporter_demon"), pos).Install();
		});
		flags.dateNextRaid = EClass.world.date.GetRaw(72);
	}
}
