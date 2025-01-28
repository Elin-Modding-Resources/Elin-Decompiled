using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Algorithms;
using FloodSpill;
using Ionic.Zip;
using Newtonsoft.Json;
using UnityEngine;

public class Map : MapBounds, IPathfindGrid
{
	public static HashSet<int> sunMap = new HashSet<int>();

	public static bool isDirtySunMap;

	[JsonProperty]
	public int seed;

	[JsonProperty]
	public int _bits;

	[JsonProperty]
	public IO.Compression compression;

	[JsonProperty]
	public Version version;

	[JsonProperty]
	public RoomManager rooms = new RoomManager();

	[JsonProperty]
	public TaskManager tasks = new TaskManager();

	[JsonProperty]
	public MapConfig config = new MapConfig();

	[JsonProperty]
	public CustomData custom;

	[JsonProperty]
	public List<Chara> serializedCharas = new List<Chara>();

	[JsonProperty]
	public List<Chara> deadCharas = new List<Chara>();

	[JsonProperty]
	public List<Thing> things = new List<Thing>();

	[JsonProperty]
	public MapBounds bounds = new MapBounds();

	[JsonProperty]
	public List<int> _plDay = new List<int>();

	[JsonProperty]
	public List<int> _plNight = new List<int>();

	[JsonProperty]
	public Dictionary<int, int> gatherCounts = new Dictionary<int, int>();

	[JsonProperty]
	public Dictionary<int, CellEffect> cellEffects = new Dictionary<int, CellEffect>();

	[JsonProperty]
	public Dictionary<int, int> backerObjs = new Dictionary<int, int>();

	[JsonProperty]
	public Dictionary<int, PlantData> plants = new Dictionary<int, PlantData>();

	public BitArray32 bits;

	public Playlist plDay;

	public Playlist plNight;

	public List<Chara> charas = new List<Chara>();

	public List<TransAnime> pointAnimes = new List<TransAnime>();

	public Cell[,] cells;

	public Zone zone;

	public CellEffectManager effectManager = new CellEffectManager();

	public PropsManager props = new PropsManager();

	public FloodSpiller flood = new FloodSpiller();

	public BiomeProfile[,] biomes;

	public POIMap poiMap;

	public List<Footmark> footmarks = new List<Footmark>();

	public FowProfile fowProfile;

	public bool revealed;

	private HashSet<int> roomHash = new HashSet<int>();

	private List<Thing> _things = new List<Thing>();

	public bool isBreakerDown
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

	public PropsStocked Stocked => props.stocked;

	public PropsInstalled Installed => props.installed;

	public PropsRoaming Roaming => props.roaming;

	public float sizeModifier => 1f / (16384f / (float)SizeXZ);

	public bool isGenerated => Size != 0;

	public bool IsIndoor => config.indoor;

	public int SizeXZ => Size * Size;

	public IEnumerable<Card> Cards => ((IEnumerable<Card>)things).Concat((IEnumerable<Card>)charas);

	[OnSerializing]
	private void OnSerializing(StreamingContext context)
	{
		_bits = (int)bits.Bits;
	}

	protected virtual void OnSerializing()
	{
	}

	[OnDeserialized]
	private void OnDeserialized(StreamingContext context)
	{
		bits.Bits = (uint)_bits;
	}

	public void CreateNew(int size, bool setReference = true)
	{
		Debug.Log("Map CreateNew:");
		Size = size;
		cells = new Cell[Size, Size];
		bounds = new MapBounds
		{
			x = 0,
			z = 0,
			maxX = Size - 1,
			maxZ = Size - 1,
			Size = Size
		};
		SetBounds(0, 0, Size - 1, Size - 1);
		ForeachXYZ(delegate(int x, int z)
		{
			cells[x, z] = new Cell
			{
				x = (byte)x,
				z = (byte)z
			};
		});
		if (setReference)
		{
			SetReference();
		}
	}

	public void SetZone(Zone _zone)
	{
		zone = _zone;
		zone.bounds = bounds;
		bounds.Size = Size;
		SetReference();
		props.Init();
		EClass.scene.profile = SceneProfile.Load(config.idSceneProfile.IsEmpty("default"));
		if (!config.idFowProfile.IsEmpty())
		{
			fowProfile = FowProfile.Load(config.idFowProfile);
		}
	}

	public void SetReference()
	{
		Fov.map = (Cell.map = (Wall.map = (Point.map = (CellDetail.map = this))));
		Cell.Size = Size;
		Cell.cells = cells;
		IPathfinder pathfinder = PathManager.Instance.pathfinder;
		WeightCell[,] weightMap = cells;
		pathfinder.Init(this, weightMap, Size);
	}

	public void OnDeactivate()
	{
		charas.ForeachReverse(delegate(Chara c)
		{
			c.ai = new NoGoal();
			if (c.IsGlobal)
			{
				zone.RemoveCard(c);
				c.currentZone = zone;
			}
		});
		foreach (Thing thing in things)
		{
			if (thing.renderer.hasActor)
			{
				thing.renderer.KillActor();
			}
		}
		EClass.player.ClearMapHighlights();
	}

	public void Resize(int newSize)
	{
		Point p = new Point(0, 0);
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.pos.x >= newSize || thing.pos.z >= newSize)
			{
				MoveCard(p, thing);
			}
		}
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.pos.x >= newSize || chara.pos.z >= newSize)
			{
				MoveCard(p, chara);
			}
		}
		Size = (bounds.Size = newSize);
		maxX = (maxZ = Size);
		cells = Util.ResizeArray(EClass._map.cells, newSize, newSize, (int x, int y) => new Cell
		{
			x = (byte)x,
			z = (byte)y,
			isSeen = true
		});
		Reload();
	}

	public void Shift(Vector2Int offset)
	{
		TweenUtil.Tween(0.1f, null, delegate
		{
			foreach (Card item in ((IEnumerable<Card>)EClass._map.things).Concat((IEnumerable<Card>)EClass._map.charas))
			{
				item.pos.x += offset.x;
				item.pos.z += offset.y;
				item.pos.Clamp();
			}
			Cell[,] array = new Cell[Size, Size];
			for (int i = 0; i < Size; i++)
			{
				int num = i - offset.y;
				for (int j = 0; j < Size; j++)
				{
					int num2 = j - offset.x;
					if (num2 >= 0 && num2 < Size && num >= 0 && num < Size)
					{
						array[j, i] = cells[num2, num];
					}
					else
					{
						array[j, i] = new Cell
						{
							x = (byte)j,
							z = (byte)i
						};
					}
				}
			}
			cells = array;
			bounds.x += offset.x;
			bounds.z += offset.y;
			bounds.maxX += offset.x;
			bounds.maxZ += offset.y;
			if (bounds.x < 0)
			{
				bounds.x = 0;
			}
			if (bounds.z < 0)
			{
				bounds.z = 0;
			}
			if (bounds.maxX > Size)
			{
				bounds.maxX = Size;
			}
			if (bounds.maxZ > Size)
			{
				bounds.maxZ = Size;
			}
			Reload();
		});
	}

	public void Reload()
	{
		rooms = new RoomManager();
		SetReference();
		string id = Game.id;
		EClass.game.Save();
		EClass.scene.Init(Scene.Mode.None);
		Game.Load(id, EClass.game.isCloud);
		RevealAll();
		TweenUtil.Tween(0.1f, null, delegate
		{
			ReloadRoom();
		});
	}

	public void ReloadRoom()
	{
		List<Thing> list = new List<Thing>();
		foreach (Thing thing in things)
		{
			if (thing.trait.IsDoor)
			{
				list.Add(thing);
			}
		}
		foreach (Thing item in list)
		{
			Point pos = item.pos;
			EClass._zone.RemoveCard(item);
			EClass._zone.AddCard(item, pos);
			item.Install();
		}
		rooms.RefreshAll();
	}

	public void Reset()
	{
		for (int i = 0; i < Size; i++)
		{
			for (int j = 0; j < Size; j++)
			{
				cells[i, j].Reset();
			}
		}
		SetReference();
	}

	public void ResetEditorPos()
	{
		EClass._zone.Revive();
		foreach (Chara chara in charas)
		{
			if (chara.isPlayerCreation && chara.orgPos != null)
			{
				MoveCard(chara.orgPos, chara);
			}
		}
		foreach (Thing thing in things)
		{
			if (thing.trait is TraitDoor)
			{
				(thing.trait as TraitDoor).ForceClose();
			}
		}
	}

	public void Save(string path, ZoneExportData export = null, PartialMap partial = null)
	{
		version = EClass.core.version;
		Debug.Log("#io saving map:" + path);
		IO.CreateDirectory(path);
		int num = 0;
		int num2 = 0;
		int num3 = Size;
		int num4 = Size;
		if (partial != null)
		{
			num = partial.offsetX;
			num2 = partial.offsetZ;
			num3 = partial.w;
			num4 = partial.h;
		}
		int num5 = num3 * num4;
		byte[] array = new byte[num5];
		byte[] array2 = new byte[num5];
		byte[] array3 = new byte[num5];
		byte[] array4 = new byte[num5];
		byte[] array5 = new byte[num5];
		byte[] array6 = new byte[num5];
		byte[] array7 = new byte[num5];
		byte[] array8 = new byte[num5];
		byte[] array9 = new byte[num5];
		byte[] array10 = new byte[num5];
		byte[] array11 = new byte[num5];
		byte[] array12 = new byte[num5];
		byte[] array13 = new byte[num5];
		byte[] array14 = new byte[num5];
		byte[] array15 = new byte[num5];
		byte[] array16 = new byte[num5];
		byte[] array17 = new byte[num5];
		byte[] array18 = new byte[num5];
		byte[] array19 = new byte[num5];
		cellEffects.Clear();
		int num6 = 0;
		for (int i = num; i < num + num3; i++)
		{
			for (int j = num2; j < num2 + num4; j++)
			{
				Cell cell = cells[i, j];
				array[num6] = cell.objVal;
				array3[num6] = cell._blockMat;
				array2[num6] = cell._block;
				array5[num6] = cell._floorMat;
				array4[num6] = cell._floor;
				array6[num6] = cell.obj;
				array7[num6] = cell.objMat;
				array8[num6] = cell.decal;
				array9[num6] = cell._dirs;
				array12[num6] = cell.height;
				array13[num6] = cell._bridge;
				array14[num6] = cell._bridgeMat;
				array15[num6] = cell.bridgeHeight;
				array17[num6] = cell._roofBlockDir;
				array18[num6] = cell._roofBlock;
				array19[num6] = cell._roofBlockMat;
				array16[num6] = cell.bridgePillar;
				array10[num6] = array10[num6].SetBit(1, cell.isSeen);
				array10[num6] = array10[num6].SetBit(2, cell.isHarvested);
				array10[num6] = array10[num6].SetBit(3, cell.impassable);
				array10[num6] = array10[num6].SetBit(4, cell.isModified);
				array10[num6] = array10[num6].SetBit(5, cell.isClearSnow);
				array10[num6] = array10[num6].SetBit(6, cell.isForceFloat);
				array10[num6] = array10[num6].SetBit(7, cell.isToggleWallPillar);
				array11[num6] = array11[num6].SetBit(0, cell.isWatered);
				array11[num6] = array11[num6].SetBit(1, cell.isObjDyed);
				array11[num6] = array11[num6].SetBit(2, cell.crossWall);
				if (cell.effect != null)
				{
					cellEffects[j * num4 + i] = cell.effect;
				}
				num6++;
			}
		}
		compression = IO.Compression.None;
		IO.WriteLZ4(path + "objVals", array);
		IO.WriteLZ4(path + "blocks", array2);
		IO.WriteLZ4(path + "blockMats", array3);
		IO.WriteLZ4(path + "floors", array4);
		IO.WriteLZ4(path + "floorMats", array5);
		IO.WriteLZ4(path + "objs", array6);
		IO.WriteLZ4(path + "objMats", array7);
		IO.WriteLZ4(path + "decal", array8);
		IO.WriteLZ4(path + "flags", array10);
		IO.WriteLZ4(path + "flags2", array11);
		IO.WriteLZ4(path + "dirs", array9);
		IO.WriteLZ4(path + "heights", array12);
		IO.WriteLZ4(path + "bridges", array13);
		IO.WriteLZ4(path + "bridgeMats", array14);
		IO.WriteLZ4(path + "bridgeHeights", array15);
		IO.WriteLZ4(path + "bridgePillars", array16);
		IO.WriteLZ4(path + "roofBlocks", array18);
		IO.WriteLZ4(path + "roofBlockMats", array19);
		IO.WriteLZ4(path + "roofBlockDirs", array17);
		things.Sort((Thing a, Thing b) => a.stackOrder - b.stackOrder);
		if (export == null)
		{
			foreach (Chara chara in charas)
			{
				if (!chara.IsGlobal)
				{
					serializedCharas.Add(chara);
				}
			}
			GameIO.SaveFile(path + "map", this);
		}
		else
		{
			export.serializedCards.cards.Clear();
			if (partial == null)
			{
				foreach (Chara chara2 in charas)
				{
					if (export.usermap)
					{
						if (!chara2.trait.IsUnique && !chara2.IsPC)
						{
							export.serializedCards.Add(chara2);
						}
					}
					else if (!chara2.IsGlobal && chara2.isPlayerCreation)
					{
						export.serializedCards.Add(chara2);
					}
				}
				foreach (Thing thing in things)
				{
					if (thing.isPlayerCreation && thing.c_altName != "DebugContainer")
					{
						export.serializedCards.Add(thing);
					}
				}
			}
			else
			{
				foreach (Thing thing2 in things)
				{
					if ((ActionMode.Copy.IsActive || thing2.trait.CanCopyInBlueprint) && thing2.pos.x >= num && thing2.pos.z >= num2 && thing2.pos.x < num + num3 && thing2.pos.z < num2 + num4)
					{
						export.serializedCards.Add(thing2);
					}
				}
			}
			List<Thing> list = things;
			things = new List<Thing>();
			GameIO.SaveFile(path + "map", this);
			things = list;
		}
		serializedCharas.Clear();
	}

	public byte[] TryLoadFile(string path, string s, int size)
	{
		byte[] array = IO.ReadLZ4(path + s, size, compression);
		if (array == null)
		{
			Debug.Log("Couldn't load:" + s);
			return new byte[size];
		}
		return array;
	}

	public void Load(string path, bool import = false, PartialMap partial = null)
	{
		if (partial == null)
		{
			Debug.Log("Map Load:" + compression.ToString() + ": " + path);
		}
		int num = Size;
		int num2 = Size;
		if (partial != null)
		{
			num = partial.w;
			num2 = partial.h;
			Debug.Log(compression.ToString() + ": " + num + "/" + num2);
		}
		int size = num * num2;
		cells = new Cell[num, num2];
		if (bounds.maxX == 0)
		{
			bounds.SetBounds(0, 0, num - 1, num2 - 1);
		}
		SetBounds(0, 0, num - 1, num2 - 1);
		byte[] bytes2 = TryLoad("objVals");
		byte[] bytes3 = TryLoad("blockMats");
		byte[] bytes4 = TryLoad("blocks");
		byte[] bytes5 = TryLoad("floorMats");
		byte[] bytes6 = TryLoad("floors");
		byte[] bytes7 = TryLoad("objs");
		byte[] bytes8 = TryLoad("objMats");
		byte[] bytes9 = TryLoad("decal");
		byte[] bytes10 = TryLoad("dirs");
		byte[] bytes11 = TryLoad("flags");
		byte[] bytes12 = TryLoad("flags2");
		byte[] bytes13 = TryLoad("heights");
		byte[] bytes14 = TryLoad("bridges");
		byte[] bytes15 = TryLoad("bridgeMats");
		byte[] bytes16 = TryLoad("bridgeHeights");
		byte[] bytes17 = TryLoad("bridgePillars");
		byte[] bytes18 = TryLoad("roofBlocks");
		byte[] bytes19 = TryLoad("roofBlockMats");
		byte[] bytes20 = TryLoad("roofBlockDirs");
		if (bytes17.Length < size)
		{
			bytes17 = new byte[size];
		}
		if (bytes2.Length < size)
		{
			bytes2 = new byte[size];
		}
		if (bytes12.Length < size)
		{
			bytes12 = new byte[size];
		}
		Validate(ref bytes2, "objVals");
		Validate(ref bytes3, "blockMats");
		Validate(ref bytes4, "blocks");
		Validate(ref bytes5, "floorMats");
		Validate(ref bytes6, "floors");
		Validate(ref bytes7, "objs");
		Validate(ref bytes8, "objMats");
		Validate(ref bytes9, "decal");
		Validate(ref bytes10, "dirs");
		Validate(ref bytes11, "flags");
		Validate(ref bytes12, "flags2");
		Validate(ref bytes13, "heights");
		Validate(ref bytes14, "bridges");
		Validate(ref bytes15, "bridgeMats");
		Validate(ref bytes16, "bridgeHeights");
		Validate(ref bytes17, "bridgePillars");
		Validate(ref bytes18, "roofBlocks");
		Validate(ref bytes19, "roofBlockMats");
		Validate(ref bytes20, "roofBlockDirs");
		int num3 = 0;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				cells[i, j] = new Cell
				{
					x = (byte)i,
					z = (byte)j,
					objVal = bytes2[num3],
					_blockMat = bytes3[num3],
					_block = bytes4[num3],
					_floorMat = bytes5[num3],
					_floor = bytes6[num3],
					obj = bytes7[num3],
					objMat = bytes8[num3],
					decal = bytes9[num3],
					_dirs = bytes10[num3],
					height = bytes13[num3],
					_bridge = bytes14[num3],
					_bridgeMat = bytes15[num3],
					bridgeHeight = bytes16[num3],
					bridgePillar = bytes17[num3],
					_roofBlock = bytes18[num3],
					_roofBlockMat = bytes19[num3],
					_roofBlockDir = bytes20[num3],
					isSeen = bytes11[num3].GetBit(1),
					isHarvested = bytes11[num3].GetBit(2),
					impassable = bytes11[num3].GetBit(3),
					isModified = bytes11[num3].GetBit(4),
					isClearSnow = bytes11[num3].GetBit(5),
					isForceFloat = bytes11[num3].GetBit(6),
					isToggleWallPillar = bytes11[num3].GetBit(7),
					isWatered = bytes12[num3].GetBit(0),
					isObjDyed = bytes12[num3].GetBit(1),
					crossWall = bytes12[num3].GetBit(2)
				};
				Critter.RebuildCritter(cells[i, j]);
				num3++;
			}
		}
		things.ForeachReverse(delegate(Thing t)
		{
			if (t.Num <= 0 || t.isDestroyed)
			{
				Debug.Log("[bug] Removing bugged thing:" + t.Num + "/" + t.isDestroyed + "/" + t);
				things.Remove(t);
			}
		});
		foreach (KeyValuePair<int, CellEffect> cellEffect in cellEffects)
		{
			int key = cellEffect.Key;
			int num4 = key % Size;
			int num5 = key / Size;
			cells[num4, num5].effect = cellEffect.Value;
			if (cellEffect.Value.IsFire)
			{
				effectManager.GetOrCreate(new Point(num4, num5));
			}
		}
		cellEffects.Clear();
		ValidateVersion();
		byte[] TryLoad(string s)
		{
			return TryLoadFile(path, s, size);
		}
		void Validate(ref byte[] bytes, string id)
		{
			if (bytes.Length < size)
			{
				Debug.LogError("expection: size invalid:" + id + " " + bytes.Length + "/" + size);
				bytes = new byte[size];
			}
		}
	}

	public void ValidateVersion()
	{
		version = EClass.core.version;
	}

	public void OnLoad()
	{
		rooms.OnLoad();
		tasks.OnLoad();
	}

	public void OnImport(ZoneExportData data)
	{
		tasks = new TaskManager();
		data.serializedCards.Restore(this, data.orgMap, addToZone: false);
	}

	public void ExportMetaData(string _path, string id, PartialMap partial = null)
	{
		if (custom == null)
		{
			custom = new CustomData();
		}
		MapMetaData mapMetaData = new MapMetaData
		{
			name = EClass._zone.Name,
			version = BaseCore.Instance.version.GetInt(),
			partial = partial
		};
		custom.id = (mapMetaData.id = id);
		IO.SaveFile(_path + "meta", mapMetaData);
	}

	public static MapMetaData GetMetaData(string pathZip)
	{
		try
		{
			using ZipFile zipFile = ZipFile.Read(pathZip);
			ZipEntry zipEntry = zipFile["meta"];
			if (zipEntry != null)
			{
				Debug.Log(zipEntry);
				using MemoryStream stream = new MemoryStream();
				zipEntry.Extract(stream);
				MapMetaData mapMetaData = IO.LoadStreamJson<MapMetaData>(stream);
				Debug.Log(mapMetaData);
				mapMetaData.path = pathZip;
				return mapMetaData;
			}
		}
		catch (Exception message)
		{
			Debug.Log(message);
		}
		return null;
	}

	public static void UpdateMetaData(string pathZip, PartialMap partial = null)
	{
		IO.CreateTempDirectory();
		ZipFile zipFile = ZipFile.Read(pathZip);
		zipFile.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
		zipFile.ExtractAll(IO.TempPath);
		zipFile.Dispose();
		EClass._map.ExportMetaData(IO.TempPath + "/", Path.GetFileNameWithoutExtension(pathZip), partial);
		using (zipFile = new ZipFile())
		{
			zipFile.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
			zipFile.AddDirectory(IO.TempPath);
			zipFile.Save(pathZip);
			zipFile.Dispose();
		}
		IO.DeleteTempDirectory();
	}

	public void AddCardOnActivate(Card c)
	{
		c.parent = zone;
		props.OnCardAddedToZone(c);
		Chara chara = c.Chara;
		if (chara != null)
		{
			chara.currentZone = EClass._zone;
		}
		if (c.isChara)
		{
			if (!c.pos.IsInBounds)
			{
				c.pos.Set(EClass._map.GetCenterPos());
			}
		}
		else if (!c.pos.IsValid)
		{
			c.pos.Set(EClass._map.GetCenterPos());
		}
		_AddCard(c.pos.x, c.pos.z, c, onAddToZone: true);
	}

	public void OnCardAddedToZone(Card t, int x, int z)
	{
		if (t.isChara)
		{
			charas.Add(t.Chara);
		}
		else
		{
			things.Add(t.Thing);
		}
		props.OnCardAddedToZone(t);
		if (t.isChara && new Point(x, z).cell.HasFullBlock)
		{
			DestroyBlock(x, z);
		}
		_AddCard(x, z, t, onAddToZone: true);
		t.trait.OnAddedToZone();
	}

	public void OnCardRemovedFromZone(Card t)
	{
		t.trait.OnRemovedFromZone();
		t.SetPlaceState(PlaceState.none);
		_RemoveCard(t);
		t.parent = null;
		if (t.isChara)
		{
			charas.Remove(t.Chara);
		}
		else
		{
			things.Remove(t.Thing);
		}
	}

	public void MoveCard(Point p, Card t)
	{
		_AddCard(p.x, p.z, t, onAddToZone: false);
	}

	public void _AddCard(int x, int z, Card t, bool onAddToZone)
	{
		if (!onAddToZone)
		{
			_RemoveCard(t);
		}
		t.pos.Set(x, z);
		if (t.IsMultisize)
		{
			t.ForeachPoint(delegate(Point p, bool main)
			{
				p.cell.AddCard(t);
			});
		}
		else
		{
			t.Cell.AddCard(t);
		}
		t.CalculateFOV();
		if (t.isThing && !t.trait.IDActorEx.IsEmpty())
		{
			EClass.scene.AddActorEx(t);
		}
	}

	public void _RemoveCard(Card t)
	{
		if (t.IsMultisize)
		{
			t.ForeachPoint(delegate(Point p, bool main)
			{
				p.cell.RemoveCard(t);
			});
		}
		else
		{
			t.Cell.RemoveCard(t);
		}
		t.ClearFOV();
	}

	public Cell GetCell(int index)
	{
		return cells[index % Size, index % SizeXZ / Size];
	}

	public void SetSeen(int x, int z, bool seen = true, bool refresh = true)
	{
		if (cells[x, z].isSeen != seen)
		{
			cells[x, z].isSeen = seen;
			if (refresh)
			{
				WidgetMinimap.UpdateMap(x, z);
			}
			EClass._map.RefreshNeighborTiles(x, z);
		}
	}

	public void RevealAll(bool reveal = true)
	{
		for (int i = 0; i < Size; i++)
		{
			for (int j = 0; j < Size; j++)
			{
				EClass._map.SetSeen(i, j, reveal, refresh: false);
			}
		}
		revealed = !revealed;
		WidgetMinimap.UpdateMap();
	}

	public void Reveal(Point center, int power = 100)
	{
		ForeachSphere(center.x, center.z, 10 + power / 5, delegate(Point p)
		{
			if (EClass.rnd(power) >= Mathf.Min(p.Distance(center) * 10, power - 10))
			{
				EClass._map.SetSeen(p.x, p.z);
			}
		});
	}

	public void RefreshFOV(int x, int z, int radius = 6, bool recalculate = false)
	{
		ForeachSphere(x, z, radius, delegate(Point p)
		{
			List<Card> list = p.ListCards();
			if (recalculate)
			{
				foreach (Card item in list)
				{
					item.RecalculateFOV();
				}
				return;
			}
			foreach (Card item2 in list)
			{
				item2.CalculateFOV();
			}
		});
	}

	public void RefreshFOVAll()
	{
		foreach (Card item in ((IEnumerable<Card>)EClass._map.things).Concat((IEnumerable<Card>)EClass._map.charas))
		{
			item.RecalculateFOV();
		}
	}

	public void SetFloor(int x, int z, int idMat = 0, int idFloor = 0)
	{
		SetFloor(x, z, idMat, idFloor, 0);
	}

	public void SetFloor(int x, int z, int idMat, int idFloor, int dir)
	{
		Cell cell = cells[x, z];
		cell._floorMat = (byte)idMat;
		cell._floor = (byte)idFloor;
		cell.floorDir = dir;
		Critter.RebuildCritter(cell);
		RefreshNeighborTiles(x, z);
	}

	public void SetBridge(int x, int z, int height = 0, int idMat = 0, int idBridge = 0, int dir = 0)
	{
		Cell cell = cells[x, z];
		cell.bridgeHeight = (byte)height;
		cell._bridgeMat = (byte)idMat;
		cell._bridge = (byte)idBridge;
		cell.bridgePillar = 0;
		cell.floorDir = dir;
		if (cell.room != null)
		{
			cell.room.SetDirty();
		}
		RefreshNeighborTiles(x, z);
	}

	public void SetRoofBlock(int x, int z, int idMat, int idBlock, int dir, int height)
	{
		Cell cell = cells[x, z];
		cell._roofBlockMat = (byte)idMat;
		cell._roofBlock = (byte)idBlock;
		cell._roofBlockDir = (byte)(dir + height * 4);
		RefreshSingleTile(x, z);
	}

	public void SetBlock(int x, int z, int idMat = 0, int idBlock = 0)
	{
		SetBlock(x, z, idMat, idBlock, 0);
	}

	public void SetBlock(int x, int z, int idMat, int idBlock, int dir)
	{
		Cell cell = cells[x, z];
		bool hasFloodBlock = cell.HasFloodBlock;
		cell._blockMat = (byte)idMat;
		cell._block = (byte)idBlock;
		cell.blockDir = dir;
		if (cell.effect == null || !cell.effect.IsFire)
		{
			cell.effect = null;
		}
		cell.isToggleWallPillar = false;
		if (cell.room != null)
		{
			cell.room.SetDirty();
		}
		if (hasFloodBlock || cell.HasFloodBlock)
		{
			OnSetBlockOrDoor(x, z);
		}
		RefreshNeighborTiles(x, z);
	}

	public void OnSetBlockOrDoor(int x, int z)
	{
		new Point(x, z);
		TryRemoveRoom(x, z);
		if (x > 0)
		{
			TryRemoveRoom(x - 1, z);
		}
		if (x < Size - 1)
		{
			TryRemoveRoom(x + 1, z);
		}
		if (z > 0)
		{
			TryRemoveRoom(x, z - 1);
		}
		if (z < Size - 1)
		{
			TryRemoveRoom(x, z + 1);
		}
		if (x > 0 && z < Size - 1)
		{
			TryRemoveRoom(x - 1, z + 1);
		}
		roomHash.Clear();
		TryAddRoom(x, z);
		if (x > 0)
		{
			TryAddRoom(x - 1, z);
		}
		if (x < Size - 1)
		{
			TryAddRoom(x + 1, z);
		}
		if (z > 0)
		{
			TryAddRoom(x, z - 1);
		}
		if (z < Size - 1)
		{
			TryAddRoom(x, z + 1);
		}
		if (x > 0 && z < Size - 1)
		{
			TryAddRoom(x - 1, z + 1);
		}
	}

	public void TryRemoveRoom(int x, int z)
	{
		if (!cells[x, z].HasFloodBlock)
		{
			Room room = cells[x, z].room;
			if (room != null)
			{
				rooms.RemoveRoom(room);
			}
		}
	}

	public void TryAddRoom(int x, int z)
	{
		if (EClass._zone.DisableRooms || roomHash.Contains(x + z * Size) || cells[x, z].HasFloodBlock)
		{
			return;
		}
		FloodSpiller floodSpiller = flood;
		IFloodCell[,] array = cells;
		FloodSpiller.Result result = floodSpiller.SpillFlood(array, x, z);
		if (!result.IsValid)
		{
			return;
		}
		bool flag = false;
		foreach (IFloodCell item in result.visited)
		{
			if (item.hasDoor)
			{
				flag = true;
				break;
			}
			Cell cell = item as Cell;
			if (cell.sourceBlock.tileType.IsFloodDoor || cell.Front.hasDoor || cell.Right.hasDoor || cell.FrontRight.hasDoor || cell.Back.hasDoor || cell.Left.hasDoor || cell.BackLeft.hasDoor)
			{
				flag = true;
				break;
			}
		}
		if (!flag && IsIndoor)
		{
			foreach (IFloodCell item2 in result.visited)
			{
				Cell cell2 = item2 as Cell;
				if (cell2.detail == null || cell2.detail.things.Count == 0)
				{
					continue;
				}
				foreach (Thing thing in cell2.detail.things)
				{
					if (thing.trait is TraitRoomPlate || thing.trait is TraitHouseBoard)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		if (!flag)
		{
			return;
		}
		Room room = rooms.AddRoom(new Room());
		foreach (IFloodCell item3 in result.visited)
		{
			Cell cell3 = item3 as Cell;
			byte b = cell3.x;
			byte b2 = cell3.z;
			room.AddPoint(new Point(b, b2));
			roomHash.Add(b + b2 * Size);
			if (b2 > 0 && cell3.Front.HasFloodBlock && cell3.Front.room == null)
			{
				room.AddPoint(new Point(b, b2 - 1));
				roomHash.Add(b + (b2 - 1) * Size);
			}
			if (b < Size - 1 && cell3.Right.HasFloodBlock && cell3.Right.room == null)
			{
				room.AddPoint(new Point(b + 1, b2));
				roomHash.Add(b + 1 + b2 * Size);
			}
			if (b2 > 0 && b < Size - 1 && cell3.FrontRight.HasFloodBlock && cell3.FrontRight.room == null)
			{
				room.AddPoint(new Point(b + 1, b2 - 1));
				roomHash.Add(b + 1 + (b2 - 1) * Size);
			}
		}
	}

	public void SetBlockDir(int x, int z, int dir)
	{
		Cell cell = cells[x, z];
		cell._block = (byte)cell.sourceBlock.id;
		cell.blockDir = dir;
	}

	public void ModFire(int x, int z, int amount)
	{
		Cell cell = cells[x, z];
		if (amount <= 0 || (!cell.IsTopWaterAndNoSnow && !cell.IsSnowTile))
		{
			if (cell.effect == null && amount > 0)
			{
				SE.Play("fire");
			}
			int num = amount + (cell.effect?.FireAmount ?? 0);
			if (num > 20)
			{
				num = 20;
			}
			if (num <= 0)
			{
				cell.effect = null;
				return;
			}
			cell.effect = new CellEffect
			{
				id = 3,
				amount = num
			};
			effectManager.GetOrCreate(new Point(x, z));
		}
	}

	public void TryShatter(Point pos, int ele, int power)
	{
		Element element = Element.Create(ele);
		List<Card> list = new List<Card>();
		bool fire = ele == 910;
		_ = fire;
		List<Card> list2 = pos.ListCards();
		if (fire && (pos.cell.IsSnowTile || pos.cell.IsTopWater))
		{
			return;
		}
		foreach (Card item in list2)
		{
			if (item.ResistLvFrom(ele) >= 3 || item.trait is TraitBlanket || (EClass.rnd(3) == 0 && !CanCook(item)) || (item.IsPCFaction && EClass.rnd(3) == 0))
			{
				continue;
			}
			if (EClass._zone.IsPCFaction && EClass.Branch.HasItemProtection)
			{
				Card rootCard = item.GetRootCard();
				if (!rootCard.isChara || rootCard.Chara.IsPCFaction)
				{
					if (pos.IsSync)
					{
						Msg.Say("damage_protectCore", item);
					}
					continue;
				}
			}
			if (item.isThing && item.things.Count == 0)
			{
				list.Add(item);
			}
			else
			{
				if (ele == 911 && item.HasElement(1236))
				{
					continue;
				}
				Thing thing = ((ele == 910) ? item.things.FindBest<TraitBlanketFireproof>((Thing t) => -t.c_charges) : item.things.FindBest<TraitBlanketColdproof>((Thing t) => -t.c_charges));
				if (thing != null)
				{
					thing.ModCharge(-1);
					Card rootCard2 = item.GetRootCard();
					if (pos.IsSync)
					{
						Msg.Say((item.isChara ? "blanketInv_" : "blanketGround_") + element.source.alias, thing, item);
					}
					if (thing.c_charges <= 0)
					{
						thing.Die(element);
						if (rootCard2.IsPCParty)
						{
							ActionMode.Adv.itemLost++;
						}
					}
					continue;
				}
				foreach (Thing item2 in item.things.List((Thing a) => a.things.Count == 0))
				{
					Card parentCard = item2.parentCard;
					if (parentCard == null || (!(parentCard.trait is TraitChestMerchant) && !parentCard.HasEditorTag(EditorTag.PreciousContainer)))
					{
						list.Add(item2);
					}
				}
			}
		}
		list.Shuffle();
		int num = 0;
		foreach (Card item3 in list)
		{
			if (!item3.trait.CanBeDestroyed || item3.category.IsChildOf("currency") || item3.trait is TraitDoor || item3.trait is TraitFigure || item3.trait is TraitTrainingDummy || item3.rarity >= Rarity.Legendary)
			{
				continue;
			}
			Card rootCard3 = item3.GetRootCard();
			if (item3.IsEquipmentOrRanged && (EClass.rnd(4) != 0 || ((item3.IsRangedWeapon || item3.Thing.isEquipped) && rootCard3.IsPCFaction && EClass.rnd(4) != 0)))
			{
				continue;
			}
			switch (ele)
			{
			case 910:
				if (item3.isFireproof || (!item3.category.IsChildOf("book") && EClass.rnd(2) == 0))
				{
					continue;
				}
				break;
			case 911:
				if (!item3.category.IsChildOf("drink") && EClass.rnd(5) == 0)
				{
					continue;
				}
				break;
			}
			if (EClass.rnd(num * num) != 0)
			{
				continue;
			}
			bool flag = CanCook(item3);
			string text = "";
			if (flag)
			{
				List<SourceThing.Row> list3 = new List<SourceThing.Row>();
				foreach (RecipeSource item4 in RecipeManager.list)
				{
					if (!(item4.row is SourceThing.Row { isOrigin: false } row) || row.components.IsEmpty() || (row.components.Length >= 3 && !row.components[2].StartsWith('+')) || !row.Category.IsChildOf("meal"))
					{
						continue;
					}
					if (!row.factory.IsEmpty())
					{
						switch (row.factory[0])
						{
						case "chopper":
						case "mixer":
						case "camppot":
						case "cauldron":
							continue;
						}
					}
					if (row.components[0] == item3.id || row.components[0] == item3.sourceCard._origin)
					{
						list3.Add(row);
					}
				}
				if (list3.Count > 0)
				{
					text = list3.RandomItem().id;
				}
			}
			if (flag && !text.IsEmpty())
			{
				item3.GetRoot();
				Thing thing2 = item3.Split(1);
				List<Thing> list4 = new List<Thing>();
				list4.Add(thing2);
				Thing thing3 = ThingGen.Create(text);
				CraftUtil.MakeDish(thing3, list4, 999);
				thing3.elements.ModBase(2, EClass.curve(power / 10, 50, 10));
				if (pos.IsSync)
				{
					Msg.Say((rootCard3 == item3) ? "cook_groundItem" : "cook_invItem", thing2, rootCard3, thing3.Name);
				}
				if (rootCard3 == item3)
				{
					EClass._zone.AddCard(thing3, item3.pos);
				}
				else if (rootCard3.isChara)
				{
					rootCard3.Chara.Pick(thing3, msg: false);
				}
				else
				{
					rootCard3.AddThing(thing3);
				}
				thing2.Destroy();
			}
			else
			{
				int num2 = EClass.rnd(item3.Num) / 2 + 1;
				if (item3.Num > num2)
				{
					Thing thing4 = item3.Split(num2);
					if (pos.IsSync)
					{
						Msg.Say((rootCard3 == item3) ? "damage_groundItem" : "damage_invItem", thing4, rootCard3);
					}
					thing4.Destroy();
					if (rootCard3.IsPCFaction)
					{
						WidgetPopText.Say("popDestroy".lang(thing4.Name, rootCard3.Name));
					}
				}
				else
				{
					item3.Die(element);
					if (rootCard3.IsPCFaction)
					{
						WidgetPopText.Say("popDestroy".lang(item3.Name, rootCard3.Name));
					}
				}
			}
			if (rootCard3.IsPCParty)
			{
				ActionMode.Adv.itemLost++;
			}
			num++;
		}
		_ValidateInstalled(pos.x, pos.z);
		bool CanCook(Card c)
		{
			if (fire && c.IsFood)
			{
				return c.category.IsChildOf("foodstuff");
			}
			return false;
		}
	}

	public void Burn(int x, int z, bool instant = false)
	{
		Cell cell = cells[x, z];
		Point sharedPoint = cell.GetSharedPoint();
		if ((instant || EClass.rnd(10) == 0) && cell.HasObj)
		{
			if (cell.sourceObj.tileType is TileTypeTree)
			{
				SetObj(x, z, cell.matObj_fixed.id, 59, 0, EClass.rnd(4));
			}
			else
			{
				SetObj(x, z);
				if (EClass.rnd(2) == 0)
				{
					EClass._zone.AddCard(ThingGen.Create((EClass.rnd(2) == 0) ? "ash" : "ash2"), sharedPoint);
				}
			}
		}
		if ((instant || EClass.rnd(5) == 0) && cell._block != 0 && cell._block != 96)
		{
			if (EClass.rnd(10) == 0 || !cell.sourceBlock.tileType.IsWall)
			{
				cell._block = 0;
			}
			else
			{
				cell._block = 96;
			}
			SetObj(x, z);
			if (cell.room != null)
			{
				cell.room.SetDirty();
			}
			RefreshNeighborTiles(x, z);
		}
		if (instant || EClass.rnd(10) == 0)
		{
			if (EClass.rnd(4) != 0)
			{
				cell._floor = 49;
			}
			if (cell._bridge != 0 && EClass.rnd(5) != 0)
			{
				cell._bridge = 49;
			}
		}
		foreach (Card item in sharedPoint.ListCards())
		{
			if (item.trait.CanBeDestroyed && !item.trait.IsDoor && !item.isFireproof && !item.category.IsChildOf("currency") && item.rarity < Rarity.Legendary && !(item.trait is TraitFigure) && item.isThing)
			{
				if (instant)
				{
					item.Destroy();
					EClass._zone.AddCard(ThingGen.Create((EClass.rnd(2) == 0) ? "ash" : "ash2"), sharedPoint);
				}
				else
				{
					item.DamageHP(30, 910);
				}
			}
		}
		if (instant)
		{
			cell.effect = null;
		}
	}

	public void SetLiquid(int x, int z, CellEffect effect = null)
	{
		Cell cell = cells[x, z];
		if (!cell.IsTopWaterAndNoSnow || effect == null)
		{
			cell.effect = effect;
		}
	}

	public void SetLiquid(int x, int z, int id, int value = 1)
	{
		Cell cell = cells[x, z];
		if (!cell.IsTopWaterAndNoSnow || value == 0)
		{
			if (value > 3)
			{
				value = 3;
			}
			if (value <= 0)
			{
				cell.effect = null;
				return;
			}
			cell.effect = new CellEffect
			{
				id = id,
				amount = value
			};
		}
	}

	public void SetEffect(int x, int z, CellEffect effect = null)
	{
		cells[x, z].effect = effect;
	}

	public void ModLiquid(int x, int z, int amount)
	{
		Cell cell = cells[x, z];
		if (!cell.IsTopWaterAndNoSnow && cell.effect != null)
		{
			cell.effect.amount += amount;
			if (cell.effect.amount <= 0)
			{
				cell.effect = null;
			}
		}
	}

	public void ClearRainAndDecal()
	{
		ForeachCell(delegate(Cell c)
		{
			c.effect = null;
			c.decal = 0;
		});
	}

	public void SetObj(int x, int z, int id = 0, int value = 1, int dir = 0)
	{
		SetObj(x, z, (byte)EClass.sources.objs.rows[id].DefaultMaterial.id, id, value, dir);
	}

	public void SetObj(int x, int z, int idMat, int idObj, int value, int dir)
	{
		Cell cell = cells[x, z];
		if (cell.sourceObj.id == 118 || idObj == 118)
		{
			EClass._zone.dirtyElectricity = true;
		}
		cell.obj = (byte)idObj;
		cell.objVal = (byte)value;
		cell.objMat = (byte)idMat;
		cell.objDir = dir;
		cell.isHarvested = false;
		cell.isObjDyed = false;
		SourceObj.Row sourceObj = cell.sourceObj;
		if (!sourceObj.matCategory.IsEmpty())
		{
			int num = EClass._zone.DangerLv;
			if (sourceObj.tag.Contains("spot"))
			{
				num += EClass.pc.Evalue(1656) * 5;
			}
			cell.objMat = (byte)MATERIAL.GetRandomMaterialFromCategory(num, sourceObj.matCategory.Split(','), cell.matObj).id;
		}
		if (backerObjs.ContainsKey(cell.index))
		{
			backerObjs.Remove(cell.index);
		}
		if (plants.ContainsKey(cell.index))
		{
			plants.Remove(cell.index);
		}
		cell.growth?.OnSetObj();
		Critter.RebuildCritter(cell);
		RefreshNeighborTiles(x, z);
	}

	public void AddBackerTree(bool draw)
	{
		SourceBacker.Row item = EClass.sources.backers.listTree.NextItem(ref BackerContent.indexTree);
		int num = ((!draw) ? 1 : 2);
		EClass._map.bounds.ForeachCell(delegate(Cell c)
		{
			if (num > 0 && c.growth != null && c.growth.IsTree && c.growth.IsMature && (!EClass.player.doneBackers.Contains(item.id) || EClass.core.config.test.ignoreBackerDestoryFlag) && (c.sourceObj.alias == item.tree || item.tree == "random"))
			{
				backerObjs[c.index] = item.id;
				Debug.Log(c.index + "/" + c.x + "/" + c.z + "/" + item.id + "/" + item.Name + "/" + item.tree);
				num--;
				item = EClass.sources.backers.listTree.NextItem(ref BackerContent.indexTree);
			}
		});
	}

	public SourceBacker.Row GetBackerObj(Point p)
	{
		if (!backerObjs.ContainsKey(p.index))
		{
			return null;
		}
		return EClass.sources.backers.map.TryGetValue(backerObjs[p.index]);
	}

	public void ApplyBackerObj(Point p, int id = -1)
	{
		if (!p.HasObj)
		{
			return;
		}
		bool flag = p.sourceObj.id == 82;
		SourceBacker.Row row = ((id != -1) ? EClass.sources.backers.map.TryGetValue(id) : (flag ? EClass.sources.backers.listRemain.NextItem(ref BackerContent.indexRemain) : EClass.sources.backers.listTree.NextItem(ref BackerContent.indexTree)));
		if (row == null)
		{
			return;
		}
		if (!EClass.player.doneBackers.Contains(row.id) || EClass.core.config.test.ignoreBackerDestoryFlag)
		{
			backerObjs[p.index] = row.id;
			if (flag)
			{
				p.cell.objDir = row.skin;
			}
		}
		else
		{
			backerObjs.Remove(p.index);
		}
	}

	public void DropBlockComponent(Point point, TileRow r, SourceMaterial.Row mat, bool recoverBlock, bool isPlatform = false, Chara c = null)
	{
		Thing thing = null;
		if (r.components.Length == 0)
		{
			return;
		}
		RecipeManager.BuildList();
		if (recoverBlock)
		{
			thing = ((!(r is SourceFloor.Row)) ? ThingGen.CreateBlock(r.id, mat.id) : ThingGen.CreateFloor(r.id, mat.id, isPlatform));
		}
		else
		{
			RecipeSource recipeSource = RecipeManager.Get(r.RecipeID + (isPlatform ? "-b" : ""));
			if (recipeSource == null)
			{
				return;
			}
			string iDIngredient = recipeSource.GetIDIngredient();
			if (iDIngredient == null)
			{
				return;
			}
			thing = ThingGen.Create(iDIngredient);
			thing.ChangeMaterial(mat.alias);
		}
		if (EClass.scene.actionMode.IsBuildMode && EClass.debug.godBuild)
		{
			PutAway(thing);
		}
		else
		{
			TrySmoothPick(point, thing, c);
		}
	}

	public void MineBlock(Point point, bool recoverBlock = false, Chara c = null)
	{
		bool flag = ActionMode.Mine.IsRoofEditMode() && point.cell._roofBlock != 0;
		if (!point.IsValid || (!flag && !point.cell.HasBlock))
		{
			return;
		}
		SourceMaterial.Row row = (flag ? point.matRoofBlock : point.matBlock);
		byte b = (flag ? point.cell._roofBlock : point.cell._block);
		SourceBlock.Row row2 = EClass.sources.blocks.rows[b];
		Effect.Get("smoke").Play(point);
		Effect.Get("mine").Play(point).SetParticleColor(row.GetColor())
			.Emit(10 + EClass.rnd(10));
		point.PlaySound(row.GetSoundDead(row2));
		row.AddBlood(point, 8);
		bool flag2 = c == null || c.IsAgent || c.IsPCFactionOrMinion;
		if (flag)
		{
			point.cell._roofBlock = 0;
			RefreshSingleTile(point.x, point.z);
		}
		else
		{
			if (point.cell.HasFullBlock)
			{
				RemoveLonelyRamps(point.cell);
			}
			point.SetBlock();
			if (flag2 && point.sourceObj.tileType.IsBlockMount)
			{
				MineObj(point, null, c);
			}
		}
		if (flag2)
		{
			DropBlockComponent(point, row2, row, recoverBlock, isPlatform: false, c);
		}
		RefreshShadow(point.x, point.z);
		RefreshShadow(point.x, point.z - 1);
		ValidateInstalled(point);
		RefreshFOV(point.x, point.z);
		if (flag2 && !point.cell.isModified && !flag)
		{
			if (b == 17 || EClass.rnd(100) == 0)
			{
				zone.AddCard(ThingGen.Create("money2"), point);
			}
			if (EClass._zone.DangerLv >= 10 && EClass.rnd(200) == 0)
			{
				zone.AddCard(ThingGen.Create("crystal_earth"), point);
			}
			if (EClass._zone.DangerLv >= 25 && EClass.rnd(200) == 0)
			{
				zone.AddCard(ThingGen.Create("crystal_sun"), point);
			}
			if (EClass._zone.DangerLv >= 40 && EClass.rnd(200) == 0)
			{
				zone.AddCard(ThingGen.Create("crystal_mana"), point);
			}
			point.cell.isModified = true;
		}
	}

	public void MineRamp(Point point, int ramp, bool recoverBlock = false)
	{
		if (point.IsValid && point.cell.HasFullBlock)
		{
			SourceMaterial.Row matBlock = point.matBlock;
			byte block = point.cell._block;
			Effect.Get("smoke").Play(point);
			Effect.Get("mine").Play(point).SetParticleColor(point.matBlock.GetColor())
				.Emit(10 + EClass.rnd(10));
			MineObj(point);
			int rampDir = EClass._map.GetRampDir(point.x, point.z, EClass.sources.blocks.rows[ramp].tileType);
			RemoveLonelyRamps(point.cell);
			SetBlock(point.x, point.z, point.cell._blockMat, ramp, rampDir);
			DropBlockComponent(point, EClass.sources.blocks.rows[block], matBlock, recoverBlock);
		}
	}

	public void MineFloor(Point point, Chara c = null, bool recoverBlock = false, bool removePlatform = true)
	{
		if (!point.IsValid || (!point.HasFloor && !point.HasBridge))
		{
			return;
		}
		SourceMaterial.Row row = (point.cell.HasBridge ? point.matBridge : point.matFloor);
		SourceFloor.Row c2 = (point.cell.HasBridge ? point.sourceBridge : point.sourceFloor);
		Effect.Get("mine").Play(point).SetParticleColor(row.GetColor())
			.Emit(10 + EClass.rnd(10));
		point.PlaySound(row.GetSoundDead(c2));
		MineObj(point, null, c);
		if (point.cell.HasBridge && removePlatform)
		{
			DropBlockComponent(EClass.pc.pos, point.sourceBridge, point.matBridge, recoverBlock, isPlatform: true, c);
			EClass._map.SetBridge(point.x, point.z);
			if (point.IsSky)
			{
				EClass.pc.Kick(point, ignoreSelf: true);
			}
			return;
		}
		if (EClass._zone.IsSkyLevel)
		{
			DropBlockComponent(EClass.pc.pos, point.sourceFloor, row, recoverBlock, isPlatform: false, c);
			SetFloor(point.x, point.z, 0, 90);
			if (point.IsSky)
			{
				EClass.pc.Kick(point, ignoreSelf: true);
			}
			return;
		}
		if (zone.IsRegion || point.cell._floor == 40)
		{
			Thing thing = ThingGen.CreateRawMaterial(row);
			thing.ChangeMaterial(row.alias);
			TrySmoothPick(point, thing, c);
		}
		else
		{
			DropBlockComponent(point, point.sourceFloor, row, recoverBlock, isPlatform: false, c);
		}
		if (!EClass._zone.IsRegion && !point.sourceFloor.components[0].Contains("chunk@soil"))
		{
			point.SetFloor(EClass.sources.floors.rows[1].DefaultMaterial.id, 40);
		}
	}

	public void RefreshShadow(int x, int z)
	{
	}

	public void TrySmoothPick(Cell cell, Thing t, Chara c)
	{
		TrySmoothPick(cell.GetPoint(), t, c);
	}

	public void TrySmoothPick(Point p, Thing t, Chara c)
	{
		if (c != null && c.IsAgent)
		{
			EClass.pc.PickOrDrop(p, t);
		}
		else if (c != null && (c.pos.Equals(p) || EClass.core.config.game.smoothPick || EClass._zone.IsRegion))
		{
			c.PickOrDrop(p, t);
		}
		else
		{
			EClass._zone.AddCard(t, p);
		}
	}

	public void DestroyObj(Point point)
	{
		Cell cell = point.cell;
		SourceObj.Row sourceObj = cell.sourceObj;
		SourceMaterial.Row matObj = cell.matObj;
		if (sourceObj.tileType.IsBlockPass)
		{
			Effect.Get("smoke").Play(point);
		}
		Effect.Get("mine").Play(point).SetParticleColor(cell.matObj.GetColor())
			.Emit(10 + EClass.rnd(10));
		point.PlaySound(matObj.GetSoundDead());
		matObj.AddBlood(point, 3);
	}

	public void MineObj(Point point, Task task = null, Chara c = null)
	{
		if (!point.IsValid || !point.HasObj)
		{
			return;
		}
		Cell cell = point.cell;
		SourceObj.Row sourceObj = cell.sourceObj;
		bool flag = false;
		if (c == null && task != null)
		{
			c = task.owner;
		}
		bool num = c == null || c.IsAgent || c.IsPCFactionOrMinion;
		DestroyObj(point);
		if (num)
		{
			SourceMaterial.Row matObj_fixed = cell.matObj_fixed;
			if (task is TaskHarvest { IsReapSeed: not false })
			{
				int num2 = 1 + EClass.rnd(2) + ((EClass.rnd(3) == 0) ? 1 : 0);
				int soilCost = EClass._zone.GetSoilCost();
				int maxSoil = EClass._zone.MaxSoil;
				if (soilCost > maxSoil)
				{
					num2 -= EClass.rnd(2 + (soilCost - maxSoil) / 10);
				}
				if (num2 <= 0)
				{
					Msg.Say("seedSpoiled", cell.GetObjName());
				}
				else if (!EClass._zone.IsUserZone)
				{
					Thing t2 = TraitSeed.MakeSeed(sourceObj, TryGetPlant(cell)).SetNum(num2);
					EClass.pc.PickOrDrop(point, t2);
				}
				if (cell.growth.IsTree)
				{
					cell.isHarvested = true;
					return;
				}
			}
			else if (sourceObj.HasGrowth)
			{
				cell.growth.PopMineObj(c);
			}
			else
			{
				if (cell.HasBlock && (sourceObj.id == 18 || sourceObj.id == 19))
				{
					MineBlock(point, recoverBlock: false, c);
				}
				switch (sourceObj.alias)
				{
				case "nest_bird":
					if (EClass.rnd(5) <= 1)
					{
						Pop(ThingGen.Create((EClass.rnd(10) == 0) ? "egg_fertilized" : "_egg").TryMakeRandomItem());
					}
					break;
				}
				int num3 = EClass.rnd(EClass.rnd(sourceObj.components.Length) + 1);
				string[] array = sourceObj.components[num3].Split('/');
				Thing thing = ThingGen.Create(array[0], matObj_fixed.alias);
				if (array.Length > 1)
				{
					thing.SetNum(EClass.rnd(array[1].ToInt()) + 1);
				}
				Pop(thing);
			}
		}
		SetObj(point.x, point.z);
		cell.gatherCount = 0;
		if (flag)
		{
			RefreshFOV(point.x, point.z);
		}
		void Pop(Thing t)
		{
			if (EClass.scene.actionMode.IsBuildMode && EClass.debug.godBuild)
			{
				EClass._map.PutAway(t);
			}
			else
			{
				TrySmoothPick(point, t, c);
			}
		}
	}

	public void MineObjSound(Point point)
	{
		point.PlaySound(point.cell.matObj.GetSoundDead(point.cell.sourceObj));
	}

	public PlantData TryGetPlant(Point p)
	{
		return plants.TryGetValue(p.index);
	}

	public PlantData TryGetPlant(Cell c)
	{
		return plants.TryGetValue(c.index);
	}

	public PlantData AddPlant(Point pos, Thing seed)
	{
		PlantData plantData = new PlantData
		{
			seed = seed
		};
		plants[pos.index] = plantData;
		return plantData;
	}

	public void RemovePlant(Point pos)
	{
		plants.Remove(pos.index);
	}

	public void ValidateInstalled(Point p)
	{
		_ValidateInstalled(p.x, p.z);
		_ValidateInstalled(p.x + 1, p.z);
		_ValidateInstalled(p.x - 1, p.z);
		_ValidateInstalled(p.x, p.z + 1);
		_ValidateInstalled(p.x, p.z - 1);
	}

	public void _ValidateInstalled(int x, int y)
	{
		Point point = Point.shared.Set(x, y);
		if (!point.IsValid)
		{
			return;
		}
		List<Card> list = point.ListCards();
		CellDetail detail = point.cell.detail;
		if (detail == null)
		{
			return;
		}
		foreach (Card item in list)
		{
			if (!item.isThing || !item.trait.CanBeDestroyed || !item.IsInstalled)
			{
				continue;
			}
			HitResult hitResult = item.TileType._HitTest(point, item.Thing, canIgnore: false);
			if (item.Thing.stackOrder != detail.things.IndexOf(item.Thing) || (hitResult != HitResult.Valid && hitResult != HitResult.Warning))
			{
				if (EClass._zone.IsPCFaction)
				{
					item.SetPlaceState(PlaceState.roaming);
				}
				else
				{
					item.Die();
				}
			}
		}
	}

	public void RemoveLonelyRamps(Cell cell)
	{
		for (int i = 0; i < 4; i++)
		{
			Cell dependedRamp = GetDependedRamp(cell);
			if (dependedRamp != null)
			{
				MineBlock(dependedRamp.GetPoint());
				continue;
			}
			break;
		}
	}

	public void DestroyBlock(int x, int z)
	{
		SetBlock(x, z);
	}

	public void AddDecal(int x, int z, int id, int amount = 1, bool refresh = true)
	{
		if (x < 0 || z < 0 || x >= Size || z >= Size)
		{
			return;
		}
		Cell cell = cells[x, z];
		if (cell.sourceFloor.tileType.AllowBlood && (cell.decal / 8 == id || cell.decal % 8 <= amount))
		{
			if (cell.decal / 8 != id && cell.decal % 8 == 0)
			{
				amount--;
			}
			int num = Mathf.Clamp(((cell.decal / 8 == id) ? (cell.decal % 8) : 0) + amount, 0, 7);
			cell.decal = (byte)(id * 8 + num);
			if (refresh)
			{
				RefreshNeighborTiles(x, z);
			}
		}
	}

	public void SetDecal(int x, int z, int id = 0, int amount = 1, bool refresh = true)
	{
		cells[x, z].decal = (byte)((id != 0 && amount != 0) ? ((uint)(id * 8 + amount)) : 0u);
		if (refresh)
		{
			RefreshNeighborTiles(x, z);
		}
	}

	public void SetFoormark(Point pos, int id, int angle, int offset = 0)
	{
		Cell cell = pos.cell;
		int tile = AngleToIndex(angle) + offset;
		Footmark footmark = new Footmark
		{
			tile = tile,
			remaining = 10
		};
		footmark.pos.Set(pos);
		footmarks.Add(footmark);
		cell.GetOrCreateDetail().footmark = footmark;
	}

	public int AngleToIndex(int a)
	{
		if (EClass._zone.IsRegion)
		{
			return a switch
			{
				135 => 7, 
				180 => 0, 
				225 => 1, 
				-90 => 2, 
				-45 => 3, 
				0 => 4, 
				45 => 5, 
				_ => 6, 
			};
		}
		return a switch
		{
			135 => 0, 
			180 => 1, 
			225 => 2, 
			-90 => 3, 
			-45 => 4, 
			0 => 5, 
			45 => 6, 
			_ => 7, 
		};
	}

	public void RefreshSingleTile(int x, int z)
	{
		cells[x, z].Refresh();
	}

	public void RefreshAllTiles()
	{
		for (int i = 0; i < Size; i++)
		{
			for (int j = 0; j < Size; j++)
			{
				cells[i, j].Refresh();
			}
		}
	}

	public void RefreshNeighborTiles(int x, int z)
	{
		cells[x, z].Refresh();
		for (int i = x - 2; i < x + 3; i++)
		{
			if (i < 0 || i >= Size)
			{
				continue;
			}
			for (int j = z - 2; j < z + 3; j++)
			{
				if (j >= 0 && j < Size && (x != i || z != j))
				{
					cells[i, j].Refresh();
				}
			}
		}
	}

	public void QuickRefreshTile(int x, int z)
	{
		Cell cell = cells[x, z];
		Cell cell2 = ((x > 0) ? cells[x - 1, z] : Cell.Void);
		Cell cell3 = ((x + 1 < Size) ? cells[x + 1, z] : Cell.Void);
		Cell cell4 = ((z > 0) ? cells[x, z - 1] : Cell.Void);
		Cell cell5 = ((z + 1 < Size) ? cells[x, z + 1] : Cell.Void);
		Cell cell6 = ((x > 0 && z > 0) ? cells[x - 1, z - 1] : Cell.Void);
		Cell cell7 = ((x + 1 < Size && z > 0) ? cells[x + 1, z - 1] : Cell.Void);
		Cell cell8 = ((x > 0 && z + 1 < Size) ? cells[x - 1, z + 1] : Cell.Void);
		Cell cell9 = ((x + 1 < Size && z + 1 < Size) ? cells[x + 1, z + 1] : Cell.Void);
		cell.isSurrounded4d = cell2.HasFullBlock && cell3.HasFullBlock && cell4.HasFullBlock && cell5.HasFullBlock;
		cell.isSurrounded = cell.isSurrounded4d && cell6.HasFullBlock && cell7.HasFullBlock && cell8.HasFullBlock && cell9.HasFullBlock;
	}

	public int GetRampDir(int x, int z, TileType blockType = null)
	{
		Cell cell = cells[x, z];
		if (cell.HasFullBlock)
		{
			if (blockType == null)
			{
				blockType = cell.sourceBlock.tileType;
			}
			Cell right = cell.Right;
			Cell front = cell.Front;
			Cell left = cell.Left;
			Cell back = cell.Back;
			if (!right.HasBlock && !right.IsVoid && left.HasFullBlock && front.CanBuildRamp(1) && back.CanBuildRamp(1))
			{
				return 1;
			}
			if (!front.HasBlock && !front.IsVoid && back.HasFullBlock && left.CanBuildRamp(0) && right.CanBuildRamp(0))
			{
				return 0;
			}
			if (!left.HasBlock && !left.IsVoid && right.HasFullBlock && front.CanBuildRamp(3) && back.CanBuildRamp(3))
			{
				return 3;
			}
			if (!back.HasBlock && !back.IsVoid && front.HasFullBlock && left.CanBuildRamp(2) && right.CanBuildRamp(2))
			{
				return 2;
			}
			if (!blockType.IsRamp)
			{
				return 0;
			}
		}
		return -1;
	}

	public Cell GetDependedRamp(Cell cell)
	{
		Cell right = cell.Right;
		if (right.HasRamp && !right.HasStairs && right.blockDir == 1)
		{
			return right;
		}
		Cell front = cell.Front;
		if (front.HasRamp && !front.HasStairs && front.blockDir == 0)
		{
			return front;
		}
		Cell left = cell.Left;
		if (left.HasRamp && !left.HasStairs && left.blockDir == 3)
		{
			return left;
		}
		Cell back = cell.Back;
		if (back.HasRamp && !back.HasStairs && back.blockDir == 2)
		{
			return back;
		}
		return null;
	}

	public Point GetRandomPoint(Point center, int radius, int tries = 100, bool mustBeWalkable = true, bool requireLos = true)
	{
		Point point = new Point();
		for (int i = 0; i < tries; i++)
		{
			point.x = center.x + EClass.rnd(radius * 2 + 1) - radius;
			point.z = center.z + EClass.rnd(radius * 2 + 1) - radius;
			point.Clamp();
			if ((!mustBeWalkable || !point.cell.blocked) && (!requireLos || Los.IsVisible(center, point)))
			{
				return point;
			}
		}
		Debug.Log("GetRandomPoint failed center:" + center?.ToString() + " rad:" + radius);
		point.IsValid = false;
		return point;
	}

	public new Point GetRandomEdge(int r = 3)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 10000; i++)
		{
			if (EClass.rnd(2) == 0)
			{
				num = ((EClass.rnd(2) == 0) ? EClass.rnd(r) : (Size - 1 - EClass.rnd(r)));
				num2 = EClass.rnd(Size);
			}
			else
			{
				num2 = ((EClass.rnd(2) == 0) ? EClass.rnd(r) : (Size - 1 - EClass.rnd(r)));
				num = EClass.rnd(Size);
			}
			Point surface = GetSurface(num, num2, walkable: false);
			if (surface.IsValid)
			{
				return surface;
			}
		}
		return GetSurface(Size / 2, Size / 2, walkable: false);
	}

	public Point GetNearbyResourcePoint(Point center)
	{
		Point point = new Point();
		int num = ((EClass.rnd(2) == 0) ? 1 : (-1));
		int num2 = ((EClass.rnd(2) == 0) ? 1 : (-1));
		for (int i = 0; i < 3; i++)
		{
			point.x = center.x - num + i * num;
			for (int j = 0; j < 3; j++)
			{
				point.z = center.z - num2 + j * num2;
				if (point.IsValid && point.area == null && point.cell.CanHarvest())
				{
					return point;
				}
			}
		}
		return Point.Invalid;
	}

	public List<Point> ListPointsInCircle(Point center, float radius, bool mustBeWalkable = true, bool los = true)
	{
		List<Point> list = new List<Point>();
		ForeachSphere(center.x, center.z, radius, delegate(Point p)
		{
			if ((!mustBeWalkable || !p.cell.blocked) && (!los || Los.IsVisible(center, p)))
			{
				list.Add(p.Copy());
			}
		});
		return list;
	}

	public List<Chara> ListCharasInCircle(Point center, float radius, bool los = true)
	{
		List<Chara> list = new List<Chara>();
		foreach (Point item in ListPointsInCircle(center, radius, mustBeWalkable: false, los))
		{
			CellDetail detail = item.detail;
			if (detail == null || detail.charas.Count <= 0)
			{
				continue;
			}
			foreach (Chara chara in item.detail.charas)
			{
				list.Add(chara);
			}
		}
		return list;
	}

	public List<Point> ListPointsInArc(Point center, Point to, int radius, float angle)
	{
		Point to2 = new Point((to.x > center.x) ? 1 : ((to.x < center.x) ? (-1) : 0), (to.z > center.z) ? 1 : ((to.z < center.z) ? (-1) : 0));
		Point point = new Point(0, 0);
		List<Point> list = new List<Point>();
		float diff = point.GetAngle2(to2);
		ForeachSphere(center.x, center.z, radius, delegate(Point p)
		{
			float angle2 = center.GetAngle2(p);
			if ((Mathf.Abs(diff - angle2) < angle || Mathf.Abs(diff - angle2 + 360f) < angle || Mathf.Abs(360f - diff + angle2) < angle) && Los.IsVisible(center, p) && !p.IsBlocked)
			{
				list.Add(p.Copy());
			}
		});
		return list;
	}

	public List<Point> ListPointsInLine(Point center, Point to, int radius)
	{
		return Los.ListVisible(center, to, radius);
	}

	public void SetBounds(int size)
	{
		if (size > Size / 2 + 1)
		{
			size = Size / 2 - 1;
		}
		bounds.SetBounds(Size / 2 - size, Size / 2 - size, Size / 2 + size, Size / 2 + size);
	}

	public void SetBounds(MapBounds b)
	{
		bounds.SetBounds(b.x, b.z, b.maxX, b.maxZ);
		bounds.Size = b.Size;
	}

	public new void ForeachCell(Action<Cell> action)
	{
		for (int i = 0; i < Size; i++)
		{
			for (int j = 0; j < Size; j++)
			{
				action(cells[i, j]);
			}
		}
	}

	public new void ForeachPoint(Action<Point> action)
	{
		Point point = new Point();
		for (int i = 0; i < Size; i++)
		{
			for (int j = 0; j < Size; j++)
			{
				action(point.Set(i, j));
			}
		}
	}

	public new void ForeachXYZ(Action<int, int> action)
	{
		for (int i = 0; i < Size; i++)
		{
			for (int j = 0; j < Size; j++)
			{
				action(i, j);
			}
		}
	}

	public void ForeachSphere(int _x, int _z, float r, Action<Point> action)
	{
		Point point = new Point();
		int num = (int)Mathf.Ceil(r);
		for (int i = _x - num; i < _x + num + 1; i++)
		{
			if (i < 0 || i >= Size)
			{
				continue;
			}
			for (int j = _z - num; j < _z + num + 1; j++)
			{
				if (j >= 0 && j < Size && (float)((i - _x) * (i - _x) + (j - _z) * (j - _z)) < r * r)
				{
					point.Set(i, j);
					action(point);
				}
			}
		}
	}

	public void ForeachNeighbor(Point center, Action<Point> action)
	{
		int num = center.x;
		int num2 = center.z;
		Point point = new Point();
		for (int i = num - 1; i < num + 2; i++)
		{
			if (i < 0 || i >= Size)
			{
				continue;
			}
			for (int j = num2 - 1; j < num2 + 2; j++)
			{
				if (j >= 0 && j < Size)
				{
					point.Set(i, j);
					action(point);
				}
			}
		}
	}

	public void Quake()
	{
		Point point = new Point();
		int num;
		for (num = 0; num < Size; num++)
		{
			int num2;
			for (num2 = 0; num2 < Size; num2++)
			{
				point.x = num;
				point.z = num2;
				point.Copy().Animate(AnimeID.Quake, animeBlock: true);
				num2 += EClass.rnd(2);
			}
			num += EClass.rnd(2);
		}
	}

	public int CountChara(Faction faction)
	{
		int num = 0;
		foreach (Chara chara in charas)
		{
			if (chara.faction == faction)
			{
				num++;
			}
		}
		return num;
	}

	public int CountGuest()
	{
		int num = 0;
		foreach (Chara chara in charas)
		{
			if (chara.IsGuest())
			{
				num++;
			}
		}
		return num;
	}

	public int CountHostile()
	{
		int num = 0;
		foreach (Chara chara in charas)
		{
			if (!chara.IsPCFaction && chara.IsHostile())
			{
				num++;
			}
		}
		return num;
	}

	public int CountWildAnimal()
	{
		int num = 0;
		foreach (Chara chara in charas)
		{
			if (!chara.IsPCFaction && chara.race.IsAnimal)
			{
				num++;
			}
		}
		return num;
	}

	public int CountNonHostile()
	{
		int num = 0;
		foreach (Chara chara in charas)
		{
			if (!chara.IsPCFaction && !chara.IsHostile())
			{
				num++;
			}
		}
		return num;
	}

	public List<Chara> ListChara(Faction faction)
	{
		List<Chara> list = new List<Chara>();
		foreach (Chara chara in charas)
		{
			if (chara.faction == faction)
			{
				list.Add(chara);
			}
		}
		return list;
	}

	public List<Thing> ListThing<T>() where T : Trait
	{
		List<Thing> list = new List<Thing>();
		foreach (Thing thing in things)
		{
			if (thing.IsInstalled && thing.trait is T)
			{
				list.Add(thing);
			}
		}
		return list;
	}

	public bool PutAway(Card c)
	{
		if (c.IsPC || (c.trait.CanOnlyCarry && !EClass.debug.ignoreBuildRule))
		{
			return false;
		}
		if (c.isChara)
		{
			if (!EClass.debug.ignoreBuildRule)
			{
				return false;
			}
			c.Destroy();
			return true;
		}
		Thing thing = c.Thing;
		if (thing.parent != null)
		{
			thing.parent.RemoveCard(thing);
		}
		thing.isMasked = false;
		thing.isRoofItem = false;
		if (EClass._zone.IsPCFaction && EClass._map.props.installed.traits.GetRandomThing<TraitSpotStockpile>() != null)
		{
			EClass._zone.TryAddThingInSpot<TraitSpotStockpile>(thing);
			return true;
		}
		if (EClass.debug.enable)
		{
			EClass.debug.GetOrCreateDebugContainer().AddThing(thing);
			return true;
		}
		EClass.pc.Pick(thing, msg: false);
		return true;
	}

	public Chara FindChara(string id)
	{
		foreach (Chara chara in charas)
		{
			if (chara.id == id)
			{
				return chara;
			}
		}
		return null;
	}

	public Chara FindChara(int uid)
	{
		foreach (Chara chara in charas)
		{
			if (chara.uid == uid)
			{
				return chara;
			}
		}
		return null;
	}

	public Thing FindThing(Func<Thing, bool> func)
	{
		foreach (Thing thing in things)
		{
			if (func(thing))
			{
				return thing;
			}
		}
		return null;
	}

	public Thing FindThing(int uid)
	{
		foreach (Thing thing in things)
		{
			if (thing.uid == uid)
			{
				return thing;
			}
		}
		return null;
	}

	public T FindThing<T>() where T : Trait
	{
		foreach (Thing thing in things)
		{
			if (thing.trait is T)
			{
				return thing.trait as T;
			}
		}
		return null;
	}

	public Thing FindThing(Type type, Chara c = null)
	{
		_things.Clear();
		foreach (Thing thing in EClass._map.props.installed.things)
		{
			if (type.IsAssignableFrom(thing.trait.GetType()) && thing.pos.IsPublicSpace())
			{
				_things.Add(thing);
			}
		}
		if (_things.Count <= 0)
		{
			return null;
		}
		return _things.RandomItem();
	}

	public Thing FindThing(Type type, BaseArea area1, BaseArea area2 = null)
	{
		if (area1 == null && area2 == null)
		{
			return null;
		}
		Thing thing = Find(area1);
		if (thing == null && area2 != null)
		{
			thing = Find(area2);
		}
		return thing;
		Thing Find(BaseArea area)
		{
			_things.Clear();
			foreach (Thing thing2 in EClass._map.props.installed.things)
			{
				if (type.IsAssignableFrom(thing2.trait.GetType()) && thing2.pos.HasRoomOrArea(area))
				{
					_things.Add(thing2);
				}
			}
			if (_things.Count <= 0)
			{
				return null;
			}
			return _things.RandomItem();
		}
	}

	public Thing FindThing(string workTag, BaseArea area1 = null, BaseArea area2 = null)
	{
		if (area1 == null && area2 == null)
		{
			return null;
		}
		Thing thing = null;
		PropSet orCreate = EClass._map.Installed.workMap.GetOrCreate(workTag);
		if (area1 != null)
		{
			IEnumerable<Card> enumerable = orCreate.Where((Card a) => a.pos.HasRoomOrArea(area1));
			if (enumerable.Count() > 0)
			{
				thing = enumerable.RandomItem() as Thing;
			}
		}
		if (thing == null && area2 != null)
		{
			IEnumerable<Card> enumerable2 = orCreate.Where((Card a) => a.pos.HasRoomOrArea(area2));
			if (enumerable2.Count() > 0)
			{
				thing = enumerable2.RandomItem() as Thing;
			}
		}
		return thing;
	}

	public Thing FindThing(string workTag, Chara c)
	{
		Thing result = null;
		IEnumerable<Card> enumerable = from a in EClass._map.Installed.workMap.GetOrCreate(workTag)
			where a.pos.IsPublicSpace()
			select a;
		if (enumerable.Count() > 0)
		{
			result = enumerable.RandomItem() as Thing;
		}
		return result;
	}

	public BaseArea FindPublicArea()
	{
		return (from a in ((IEnumerable<BaseArea>)rooms.listArea).Concat((IEnumerable<BaseArea>)rooms.listRoom)
			where a.type.IsPublicArea
			select a).RandomItem();
	}

	public void RefreshSunMap()
	{
		if (!isDirtySunMap)
		{
			return;
		}
		sunMap.Clear();
		foreach (Card sun in EClass._map.props.installed.traits.suns)
		{
			foreach (Point item in sun.trait.ListPoints(null, onlyPassable: false))
			{
				sunMap.Add(item.index);
			}
		}
		isDirtySunMap = false;
	}
}
