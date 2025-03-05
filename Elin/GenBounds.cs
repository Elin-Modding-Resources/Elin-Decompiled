using System;
using System.Collections.Generic;
using UnityEngine;

public class GenBounds : EClass
{
	public Map map;

	public Zone zone;

	public int Size;

	public int x;

	public int y;

	public int width;

	public int height;

	public int marginPartial;

	public Func<Cell, bool> FuncCheckEmpty;

	private bool IsSub(BiomeProfile.Tile g, int x, int y)
	{
		return g.subType switch
		{
			BiomeProfile.BaseTile.SubType.Rnd5 => EClass.rnd(5) == 0, 
			BiomeProfile.BaseTile.SubType.Rnd10 => EClass.rnd(10) == 0, 
			BiomeProfile.BaseTile.SubType.Rnd20 => EClass.rnd(20) == 0, 
			BiomeProfile.BaseTile.SubType.Pattern => (x + y % 2) % 2 == 0, 
			_ => false, 
		};
	}

	public void SetFloor(BiomeProfile.Tile t, int x, int z)
	{
		bool flag = IsSub(t, x, z);
		SetFloor(x, z, flag ? t.matSub : t.mat, flag ? t.idSub : t.id, EClass.rnd(EClass.rnd(8) + 1));
	}

	public void SetBlock(BiomeProfile.Tile t, int x, int z, int idMat = -1)
	{
		bool flag = IsSub(t, x, z);
		SetBlock(x, z, (idMat != -1) ? idMat : (flag ? t.matSub : t.mat), flag ? t.idSub : t.id);
	}

	public void SetFloor(int x, int z, int idMat, int idFloor, int direction = 0)
	{
		Cell cell = map.cells[x, z];
		cell._floorMat = (byte)idMat;
		cell._floor = (byte)idFloor;
		cell.floorDir = direction;
	}

	public void SetBlock(int x, int z, int idMat, int idBlock, int direction = 0)
	{
		Cell cell = map.cells[x, z];
		cell._blockMat = (byte)idMat;
		cell._block = (byte)idBlock;
		cell.blockDir = direction;
		cell.effect = null;
	}

	public bool IsEmpty()
	{
		for (int i = y - marginPartial; i < y + height + marginPartial; i++)
		{
			if (i < 0 || i >= EClass._map.Size)
			{
				return false;
			}
			for (int j = x - marginPartial; j < x + width + marginPartial; j++)
			{
				if (j < 0 || j >= EClass._map.Size)
				{
					return false;
				}
				Cell cell = map.cells[j, i];
				if (FuncCheckEmpty != null && !FuncCheckEmpty(cell))
				{
					return false;
				}
				if (cell.blocked || cell.HasBlock || cell.Installed != null)
				{
					return false;
				}
			}
		}
		return true;
	}

	public List<Point> ListEmptyPoint()
	{
		List<Point> list = new List<Point>();
		for (int i = y; i < y + height; i++)
		{
			if (i < 0 || i >= EClass._map.Size)
			{
				continue;
			}
			for (int j = x; j < x + width; j++)
			{
				if (j < 0 || j >= EClass._map.Size)
				{
					continue;
				}
				Cell cell = map.cells[j, i];
				if (!cell.blocked && !cell.HasBlock && (cell.Installed == null || !cell.Installed.trait.IsBlockPath))
				{
					Point point = new Point(j, i);
					if (!point.HasChara)
					{
						list.Add(point);
					}
				}
			}
		}
		return list;
	}

	public static GenBounds Create(Zone z)
	{
		MapBounds bounds = z.map.bounds;
		return new GenBounds
		{
			zone = z,
			map = z.map,
			x = bounds.x,
			y = bounds.z,
			width = bounds.Width,
			height = bounds.Height
		};
	}

	public GenBounds GetBounds(int w, int h, bool ignoreBlock)
	{
		return GetBounds(map, zone, x, y, width, height, w, h, ignoreBlock);
	}

	public GenBounds GetBounds(Map map, Zone zone, int x, int y, int width, int height, int dw, int dh, bool ignoreBlock)
	{
		if (dw >= width || dh >= height)
		{
			return null;
		}
		GenBounds genBounds = new GenBounds
		{
			x = x,
			y = y,
			width = dw,
			height = dh,
			map = map,
			zone = zone,
			marginPartial = marginPartial,
			FuncCheckEmpty = FuncCheckEmpty
		};
		for (int i = 0; i < 200; i++)
		{
			if (ignoreBlock)
			{
				genBounds.x = EClass.rnd(width - dw) + x - dw / 2;
				genBounds.y = EClass.rnd(height - dh) + y - dh / 2;
				if (genBounds.x > 0 && genBounds.y > 0)
				{
					return genBounds;
				}
			}
			else
			{
				genBounds.x = EClass.rnd(width - dw) + x;
				genBounds.y = EClass.rnd(height - dh) + y;
				if (genBounds.IsEmpty())
				{
					return genBounds;
				}
			}
		}
		return null;
	}

	public PartialMap TryAddMapPiece(MapPiece.Type type = MapPiece.Type.Any, float ruin = -1f, string tags = null, Action<PartialMap, GenBounds> onCreate = null)
	{
		if (ruin == -1f)
		{
			ruin = zone.RuinChance;
		}
		PartialMap partialMap = MapPiece.Instance.GetMap(type, tags.IsEmpty(zone.biome.tags), ruin);
		if (partialMap == null)
		{
			Debug.Log("TryAddMap Piece: no map");
			return null;
		}
		bool flag = partialMap.dir == 1 || partialMap.dir == 3;
		GenBounds bounds = GetBounds(flag ? partialMap.h : partialMap.w, flag ? partialMap.w : partialMap.h, partialMap.ignoreBlock);
		if (bounds == null)
		{
			return null;
		}
		partialMap.Apply(new Point(bounds.x, bounds.y), PartialMap.ApplyMode.Apply);
		onCreate?.Invoke(partialMap, bounds);
		Debug.Log(partialMap.path);
		return partialMap;
	}
}
