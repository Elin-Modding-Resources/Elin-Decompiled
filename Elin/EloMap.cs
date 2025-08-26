using System.Collections.Generic;
using System.Runtime.Serialization;
using CreativeSpore.SuperTilemapEditor;
using Newtonsoft.Json;
using UnityEngine;

public class EloMap : EClass
{
	public class Cell
	{
		public Zone zone;

		public int obj;

		public Cell()
		{
		}

		public Cell(int i)
		{
			obj = i / 10;
		}

		public int GetInt()
		{
			return obj * 10;
		}
	}

	public class TileInfo
	{
		public Sprite sprite;

		public bool blocked;

		public bool isRoad;

		public bool roadLeft;

		public bool roadRight;

		public bool roadUp;

		public bool roadDown;

		public bool rock;

		public bool sea;

		public bool shore;

		public SourceGlobalTile.Row source;

		public bool IsSnow
		{
			get
			{
				if (!(idSurface == "snow_edge"))
				{
					return idSurface == "snow";
				}
				return true;
			}
		}

		public string idSurface => source?.alias ?? "";

		public string idZoneProfile => source.zoneProfile;

		public string name => source.GetName();

		public bool CanEmbark => idZoneProfile != null;

		public bool IsBridge => idSurface == "bridge";

		public bool IsNeighborRoad
		{
			get
			{
				if (!roadLeft && !roadRight && !roadUp)
				{
					return roadDown;
				}
				return true;
			}
		}
	}

	[JsonProperty]
	public int[,] _ints;

	public Cell[,] cells;

	public TilemapGroup group;

	public STETilemap fogmap;

	public STETilemap seaMap;

	public STETilemap objScatterMap;

	public STETilemap objmap;

	public STETilemap extramap;

	public STETilemap cloudmap;

	public int w;

	public int h;

	public int minX;

	public int minY;

	public bool initialized;

	public EloMapActor actor;

	public Region region => EClass.world.region;

	public string idMap => "map_ntyris";

	[OnSerializing]
	internal void OnSerializing(StreamingContext context)
	{
		if (cells == null)
		{
			return;
		}
		_ints = new int[w, h];
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				_ints[j, i] = cells[j, i].GetInt();
			}
		}
	}

	[OnDeserialized]
	internal void OnDeserialized(StreamingContext context)
	{
		if (_ints == null || _ints.GetLength(0) <= 0)
		{
			return;
		}
		w = _ints.GetLength(0);
		h = _ints.GetLength(1);
		cells = new Cell[w, h];
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				cells[j, i] = new Cell(_ints[j, i]);
			}
		}
	}

	public void Init(EloMapActor _actor)
	{
		if (initialized)
		{
			return;
		}
		actor = _actor;
		initialized = true;
		group = actor.transMap.GetComponentInChildren<TilemapGroup>();
		seaMap = group.Tilemaps[0];
		cloudmap = group.Tilemaps[7];
		extramap = group.Tilemaps[6];
		fogmap = group.Tilemaps[5];
		objmap = group.Tilemaps[4];
		objScatterMap = group.Tilemaps[3];
		w = fogmap.GridWidth;
		h = fogmap.GridHeight;
		minX = fogmap.MinGridX;
		minY = fogmap.MinGridY;
		if (cells == null)
		{
			cells = new Cell[w, h];
			for (int i = 0; i < h; i++)
			{
				for (int j = 0; j < w; j++)
				{
					cells[j, i] = new Cell();
				}
			}
		}
		foreach (Spatial child in region.children)
		{
			int x = child.x;
			int y = child.y;
			Zone zone = child as Zone;
			Cell cell = GetCell(x, y);
			if (cell == null)
			{
				Debug.Log("cell is null:" + x + "/" + y);
			}
			else if (!zone.IsInstance)
			{
				cell.zone = zone;
				cell.obj = zone.icon;
			}
		}
		for (int k = 0; k < h; k++)
		{
			for (int l = 0; l < w; l++)
			{
				Cell cell2 = cells[l, k];
				int num = minX + l;
				int num2 = minY + k;
				if (cell2.obj != 0)
				{
					objmap.SetTile(num, num2, cell2.obj);
				}
				if (cell2.zone != null)
				{
					if (cell2.zone.UseLight)
					{
						AddLight(num, num2);
					}
					if (cell2.zone.IsClosed)
					{
						extramap.SetTile(num, num2, 333);
					}
				}
			}
		}
		extramap.UpdateMeshImmediate();
		objmap.UpdateMeshImmediate();
	}

	public void SetZone(int gx, int gy, Zone z, bool updateMesh = false)
	{
		Cell cell = GetCell(gx, gy);
		if (cell == null)
		{
			Debug.Log("cell is null:" + gx + "/" + gy);
		}
		else if (z == null || cell.obj != z.icon)
		{
			cell.obj = z?.icon ?? 0;
			if (z != null && z.source.tag.Contains("iconFlag"))
			{
				cell.obj = 306;
			}
			if (cell.zone != null && cell.zone.UseLight)
			{
				RemoveLight(gx, gy);
			}
			cell.zone = z;
			if (cell.obj == 0)
			{
				objmap.Erase(gx, gy);
			}
			else
			{
				objmap.SetTile(gx, gy, cell.obj);
			}
			if (z != null && z.UseLight)
			{
				AddLight(gx, gy);
			}
			if (updateMesh)
			{
				objmap.UpdateMeshImmediate();
			}
		}
	}

	public Cell GetCell(Point pos)
	{
		return GetCell(pos.x + minX, pos.z + minY);
	}

	public Cell GetCell(int gx, int gy)
	{
		if (gx < minX || gy < minY || gx >= minX + w || gy >= minY + h)
		{
			return null;
		}
		return cells[gx - minX, gy - minY];
	}

	public TileInfo GetTileInfo(int gx, int gy)
	{
		TileInfo t = new TileInfo();
		bool skip = false;
		group.Tilemaps.ForeachReverse(delegate(STETilemap m)
		{
			if (!(m == fogmap || skip))
			{
				int tileIdFromTileData = Tileset.GetTileIdFromTileData(m.GetTileData(gx, gy));
				TileData tileData = new TileData(m.GetTileData(gx, gy));
				int tileId = tileData.tileId;
				if (tileId >= 22 && tileId <= 25)
				{
					bool flipHorizontal = tileData.flipHorizontal;
					bool flipVertical = tileData.flipVertical;
					bool rot = tileData.rot90;
					int num = (flipHorizontal ? 1 : 0) + (flipVertical ? 1 : 0) * 2 + (rot ? 1 : 0) * 4;
					t.isRoad = true;
					t.roadLeft = tileId == 23 || (tileId == 24 && (num == 4 || num == 0)) || (tileId == 25 && num != 6);
					t.roadRight = tileId == 23 || (tileId == 24 && (num == 6 || num == 1)) || (tileId == 25 && num != 4);
					t.roadUp = tileId == 22 || (tileId == 24 && (num == 4 || num == 6)) || (tileId == 25 && num != 0);
					t.roadDown = tileId == 22 || (tileId == 24 && (num == 0 || num == 1)) || (tileId == 25 && num != 2);
				}
				SourceGlobalTile.Row row = EClass.sources.globalTiles.tileAlias.TryGetValue(tileIdFromTileData);
				if (row != null)
				{
					t.source = row;
					t.sprite = TilemapUtils.GetOrCreateTileSprite(actor.tileset, row.tiles[0]);
					switch (row.alias)
					{
					case "wall":
					case "rock":
						t.rock = true;
						break;
					case "sea":
						t.sea = true;
						break;
					case "beach":
						t.shore = true;
						break;
					}
					if (!row.zoneProfile.IsEmpty())
					{
						skip = true;
					}
					else if (row.attribs[0] == 0)
					{
						t.blocked = true;
					}
				}
			}
		});
		return t;
	}

	public List<SourceGlobalTile.Row> GetSources(int gx, int gy)
	{
		List<SourceGlobalTile.Row> list = new List<SourceGlobalTile.Row>();
		foreach (STETilemap tilemap in group.Tilemaps)
		{
			if (!(tilemap == fogmap))
			{
				int tileIdFromTileData = Tileset.GetTileIdFromTileData(tilemap.GetTileData(gx, gy));
				if (EClass.sources.globalTiles.tileAlias.ContainsKey(tileIdFromTileData))
				{
					list.Add(EClass.sources.globalTiles.tileAlias.TryGetValue(tileIdFromTileData));
				}
			}
		}
		return list;
	}

	public bool CanBuildSite(int gx, int gy, int radius = 0, ElomapSiteType type = ElomapSiteType.Nefia)
	{
		if (radius != 0)
		{
			for (int i = gy - radius; i < gy + radius + 1; i++)
			{
				for (int j = gx - radius; j < gx + radius + 1; j++)
				{
					if (!CanBuildSite(j, i, 0, type))
					{
						return false;
					}
				}
			}
			return true;
		}
		Cell cell = GetCell(gx, gy);
		if (cell == null || cell.zone != null || cloudmap.GetTileData(gx, gy) != uint.MaxValue)
		{
			return false;
		}
		SourceGlobalTile.Row row = GetSources(gx, gy).LastItem();
		if (row == null)
		{
			return false;
		}
		switch (type)
		{
		case ElomapSiteType.NefiaWater:
			if (row.id != 4)
			{
				return false;
			}
			break;
		case ElomapSiteType.Mob:
			if (row.id == 4 && EClass.rnd(5) == 0)
			{
				return false;
			}
			if (row.id == 7 && EClass.rnd(2) == 0)
			{
				return false;
			}
			break;
		default:
			if (row == null || !row.tag.Contains("site"))
			{
				return false;
			}
			if (row.id == 7 && EClass.rnd(5) == 0)
			{
				return false;
			}
			break;
		}
		return true;
	}

	public bool IsWater(int gx, int gy)
	{
		if (GetCell(gx, gy) == null)
		{
			return false;
		}
		SourceGlobalTile.Row row = GetSources(gx, gy).LastItem();
		if (row != null)
		{
			if (row.id != 4)
			{
				return row.id == 15;
			}
			return true;
		}
		return false;
	}

	public bool IsSnow(int gx, int gy)
	{
		if (GetCell(gx, gy) == null)
		{
			return false;
		}
		SourceGlobalTile.Row row = GetSources(gx, gy).LastItem();
		if (row != null)
		{
			return row.id == 7;
		}
		return false;
	}

	public Zone GetZone(Point p)
	{
		return GetZone(p.x + minX, p.z + minY);
	}

	public Zone GetZone(int gx, int gy)
	{
		Zone zone = null;
		foreach (Spatial child in region.children)
		{
			if (child.x == gx && child.y == gy && (child as Zone)?.instance == null && (zone == null || zone is Zone_Field))
			{
				zone = child as Zone;
			}
		}
		return zone;
	}

	public int GetRoadDist(int gx, int gy)
	{
		if (!initialized)
		{
			EClass.scene.elomapActor.Initialize(EClass.world.region.elomap);
		}
		for (int i = 0; i < 100; i++)
		{
			for (int j = gy - i; j < gy + i + 1; j++)
			{
				for (int k = gx - i; k < gx + i + 1; k++)
				{
					if (j == gy - i || j == gy + i || k == gx - i || k == gx + i)
					{
						uint tileData = objScatterMap.GetTileData(k, j);
						if (((tileData != uint.MaxValue) ? ((tileData & 0xFFF0000) >> 16) : 0) == 3)
						{
							return i;
						}
						tileData = seaMap.GetTileData(k, j);
						if (((tileData != uint.MaxValue) ? ((tileData & 0xFFF0000) >> 16) : 0) == 3)
						{
							return i;
						}
					}
				}
			}
		}
		return 100;
	}

	public void AddLight(int gx, int gy, string id = "elolight")
	{
		SpriteRenderer spriteRenderer = Util.Instantiate<SpriteRenderer>(id, actor.transLight);
		EloMapLight item = new EloMapLight
		{
			sr = spriteRenderer,
			gx = gx,
			gy = gy
		};
		actor.lights.Add(item);
		spriteRenderer.transform.position = TilemapUtils.GetGridWorldPos(fogmap, gx, gy);
	}

	public void RemoveLight(int gx, int gy)
	{
		foreach (EloMapLight light in actor.lights)
		{
			if (light.gx == gx && light.gy == gy)
			{
				Object.DestroyImmediate(light.sr.gameObject);
				actor.lights.Remove(light);
				break;
			}
		}
	}
}
