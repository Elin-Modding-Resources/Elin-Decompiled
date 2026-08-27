using System.Collections.Generic;
using System.Linq;
using Dungen;
using UnityEngine;

public class MapGenDungen : BaseMapGen
{
	private static MapGenDungen _Instance;

	public static MapGenDungen Instance => _Instance ?? (_Instance = new MapGenDungen());

	protected override bool OnGenerateTerrain()
	{
		DunGen.Init();
		MapGenerator generator = zone.Generator;
		MapData mapData = DunGen.Generate(generator);
		width = mapData.size_X + 2;
		height = mapData.size_Y + 2;
		SetSize(Mathf.Max(width, height), 10);
		map.CreateNew(Size);
		map.poiMap.Reset();
		map.SetZone(zone);
		map.config.blockHeight = EClass.core.gameSetting.gen.defaultBlockHeight;
		BiomeProfile biome = zone.biome;
		if (zone is Zone_Void)
		{
			biome = biome.Instantiate();
			biome.interior.block.mat = MATERIAL.GetRandomMaterialFromCategory(zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
			biome.interior.block.matSub = MATERIAL.GetRandomMaterialFromCategory(zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
			biome.interior.floor.mat = MATERIAL.GetRandomMaterialFromCategory(zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
			biome.interior.floor.matSub = MATERIAL.GetRandomMaterialFromCategory(zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
			biome.exterior.block.mat = MATERIAL.GetRandomMaterialFromCategory(zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
			biome.exterior.block.matSub = MATERIAL.GetRandomMaterialFromCategory(zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
			biome.exterior.floor.mat = MATERIAL.GetRandomMaterialFromCategory(zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
			biome.exterior.floor.matSub = MATERIAL.GetRandomMaterialFromCategory(zone.lv % 50 + 10, "rock", EClass.sources.materials.alias["granite"]).id;
		}
		if (biome.name == "Dungeon_Water")
		{
			int num = ((EClass.rnd(3) == 0) ? 187 : ((EClass.rnd(2) == 0) ? 188 : 189));
			biome.exterior.block.id = (biome.interior.block.id = num);
			if (num == 189)
			{
				biome.exterior.block.mat = (biome.interior.block.mat = (byte)EClass.sources.materials.rows.Where((SourceMaterial.Row r) => r.tag.Contains("coral")).RandomItem().id);
			}
			else
			{
				biome.exterior.block.mat = (biome.interior.block.mat = 3);
			}
		}
		BiomeProfile.TileFloor floor = biome.exterior.floor;
		BiomeProfile.TileBlock block = biome.exterior.block;
		int idMat = -1;
		if ((zone is Zone_RandomDungeonPlain || zone is Zone_RandomDungeonForest) && EClass.rndSeed(3, zone.uid) != 0)
		{
			block = EClass.core.refs.biomes.dict["Dungeon_Forest"].exterior.block;
			if (zone is Zone_RandomDungeonPlain)
			{
				idMat = 5;
			}
		}
		bool flag = zone.lv <= 0;
		bool flag2 = false;
		bool flag3 = false;
		Point point = new Point();
		Thing thing = null;
		Thing thing2 = null;
		for (int num2 = 0; num2 < mapData.size_X + 2; num2++)
		{
			for (int num3 = 0; num3 < mapData.size_Y + 2; num3++)
			{
				if (num2 == 0 || num3 == 0 || num2 >= mapData.size_X || num3 >= mapData.size_Y)
				{
					SetFloor(floor, num2, num3);
					SetBlock(block, num2, num3, idMat);
					continue;
				}
				Dungen.Cell cell = mapData.cellsOnMap[num2 - 1, num3 - 1];
				CellType type = cell.type;
				point.Set(num2, num3);
				SetFloor(floor, num2, num3);
				switch (type.name)
				{
				case "Entrance":
				{
					if (flag2)
					{
						Debug.LogError("exception: already created entrance");
						break;
					}
					flag2 = true;
					Thing thing4 = ThingGen.Create(biome.style.GetIdStairs(flag), biome.style.matStairs);
					if (!flag)
					{
						thing = thing4;
					}
					else
					{
						thing2 = thing4;
					}
					zone.AddCard(thing4, num2, num3).Install();
					break;
				}
				case "Exit":
				{
					if (!zone.ShouldMakeExit)
					{
						break;
					}
					if (flag3)
					{
						Debug.LogError("exception: already created exit");
						break;
					}
					flag3 = true;
					Thing thing3 = ThingGen.Create(EClass._zone.LockExit ? "stairs_locked" : biome.style.GetIdStairs(!flag), zone.LockExit ? (-1) : biome.style.matStairs);
					if (flag)
					{
						thing = thing3;
					}
					else
					{
						thing2 = thing3;
					}
					zone.AddCard(thing3, num2, num3).Install();
					break;
				}
				case "Door":
					if (!(biome.style.doorChance < Rand.Range(0f, 1f)))
					{
						SetBlock(block, num2, num3, idMat);
						Thing t = ThingGen.Create(biome.style.GetIdDoor(), biome.style.matDoor);
						zone.AddCard(t, num2, num3).Install();
					}
					break;
				case "Abyss":
					SetBlock(block, num2, num3, idMat);
					break;
				default:
					if (type.passable == generator.reversePassage)
					{
						SetBlock(block, num2, num3, idMat);
					}
					break;
				}
				if (!cell.isRoomCell)
				{
					biome.Populate(point);
				}
			}
		}
		if (!flag2)
		{
			Debug.LogError("exception: Failed to create entrance:");
			return false;
		}
		if (zone.ShouldMakeExit && !flag3)
		{
			Debug.LogError("exception: Failed to create exist:");
			return false;
		}
		if (thing2 != null)
		{
			thing2.pos.cell._block = 0;
			thing2.pos.cell.obj = 0;
		}
		if (thing != null)
		{
			thing.pos.cell._block = 0;
			thing.pos.cell.obj = 0;
		}
		Dictionary<int, GenRoom> rooms = new Dictionary<int, GenRoom>();
		int count = 0;
		int num4 = 0;
		foreach (Dungen.Room room in mapData.rooms)
		{
			if (room.width != 0 && room.height != 0)
			{
				num4++;
			}
		}
		if (num4 == 0)
		{
			mapData.rooms.Clear();
		}
		if (mapData.rooms.Count == 0)
		{
			GenRoomBig genRoomBig = new GenRoomBig();
			genRoomBig.Init(1, 1, width - 1, height - 1);
			SetRoom(genRoomBig);
		}
		else
		{
			foreach (Dungen.Room room2 in mapData.rooms)
			{
				GenRoom genRoom = ChooseRoom();
				genRoom.Init(room2);
				SetRoom(genRoom);
			}
		}
		map.RefreshAllTiles();
		foreach (GenRoom value in rooms.Values)
		{
			value.Populate();
		}
		zone.OnGenerateRooms(this);
		map.ReloadRoom();
		Debug.Log("Dungen: room:" + rooms.Count + "/" + mapData.rooms.Count + " width:" + width + " height:" + height);
		int num5 = EClass.rnd(Size * Size / 50 + EClass.rnd(20)) + 5;
		num5 = num5 * Mathf.Min(20 + zone.DangerLv * 5, 100) / 100;
		if (zone is Zone_RandomDungeonNature)
		{
			num5 /= 5;
		}
		for (int num6 = 0; num6 < num5; num6++)
		{
			point = EClass._map.GetRandomPoint();
			if (!point.cell.isModified && !point.HasThing && !point.HasBlock && !point.HasObj)
			{
				Thing t2 = ThingGen.CreateFromCategory("trap", zone.DangerLv);
				EClass._zone.AddCard(t2, point).Install();
			}
		}
		bool forest = zone is Zone_RandomDungeonForest;
		if (zone is Zone_RandomDungeonPlain || (forest && EClass.rnd(3) == 0))
		{
			Crawler.Create("pasture").CrawlUntil(tries: EClass.debug.enable ? 3 : EClass.rnd(EClass.rnd(3) + 1), map: EClass._map, onStart: () => EClass._map.GetRandomPoint(), canComplete: delegate(Crawler.Result r)
			{
				int num7 = ((forest || EClass.rnd(5) == 0) ? EClass.sources.objs.rows.Where((SourceObj.Row a) => a.ContainsTag("wild")).RandomItem().id : ((EClass.rnd(3) == 0) ? 108 : 105));
				Thing thing5 = null;
				int num8 = Mathf.Min(EClass._zone.DangerLv, EClass.pc.Evalue(286) * 2 / 3);
				if (num8 > 0)
				{
					thing5 = TraitSeed.MakeSeed(EClass.sources.objs.map[num7]);
					Rand.SetSeed(EClass._zone.uid * 10 + num8);
					TraitSeed.LevelSeed(thing5, (thing5.trait as TraitSeed).row, num8);
					Rand.SetSeed();
					thing5.elements.SetBase(2, EClass.curve(thing5.encLV, 50, 10, 80));
				}
				foreach (Point point2 in r.points)
				{
					if (!point2.cell.isModified && !point2.HasThing && !point2.HasBlock && !point2.HasObj)
					{
						map.SetObj(point2.x, point2.z, num7);
						int num9 = 3;
						if (EClass.rnd(6) == 0)
						{
							num9++;
						}
						point2.growth.SetStage(num9);
						if (thing5 != null)
						{
							EClass._map.AddPlant(point2, thing5);
						}
					}
				}
				return false;
			});
		}
		map.things.ForeachReverse(delegate(Thing thing5)
		{
			if (thing5.trait is TraitDoor traitDoor && !traitDoor.IsValid())
			{
				Debug.Log("Purging Door:" + thing5.Name + "/" + thing5.pos);
				thing5.Destroy();
			}
		});
		if (thing != null)
		{
			ClearPos(thing);
		}
		if (thing2 != null)
		{
			ClearPos(thing2);
		}
		return true;
		static void ClearPos(Thing thing5)
		{
			foreach (Card item in thing5.pos.ListCards())
			{
				if (item != thing5 && item.isThing)
				{
					item.Destroy();
				}
			}
			thing5.pos.SetObj();
			thing5.pos.SetBlock();
			thing5.pos.cell.height = 0;
		}
		void SetRoom(GenRoom room)
		{
			room.map = map;
			room.zone = zone;
			room.gen = this;
			room.group = ((zone is Zone_RandomDungeonNature) ? biome.exterior : biome.interior);
			rooms[room.Index] = room;
			room.Fill();
			Debug.Log("Room" + count + " " + room.width + "*" + room.height + " " + room);
			count++;
		}
	}

	public GenRoom ChooseRoom()
	{
		if (EClass.rnd(100) < 5)
		{
			return new GenRoomMonsterHouse();
		}
		return new GenRoom();
	}
}
