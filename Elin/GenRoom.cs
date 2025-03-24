using System;
using Dungen;
using UnityEngine;

public class GenRoom : GenBounds
{
	public BiomeProfile.TileGroup group;

	public MapGenDungen gen;

	public static int[] ListWrecks = new int[6] { 34, 46, 83, 85, 86, 87 };

	private Point _p = new Point();

	private Cell _c;

	public int Index => x + y * Size;

	public virtual bool PopulateCluster => true;

	public virtual bool IsBigRoom => false;

	public void Init(Dungen.Room room)
	{
		x = room.x + 1;
		y = room.y + 1;
		width = room.width;
		height = room.height;
		Size = width * height;
	}

	public void Init(int _x, int _y, int w, int h)
	{
		x = _x;
		y = _y;
		width = w;
		height = h;
		Size = width * height;
	}

	public void Populate()
	{
		PartialMap partialMap = TryAddMapPiece();
		OnPopulate();
		BiomeProfile biome = zone.biome;
		if (biome.style.lights.Count > 0 && (partialMap == null || !partialMap.result.hasLight))
		{
			foreach (BiomeProfile.Cluster.ItemThing light in biome.style.lights)
			{
				if (Rand.Range(0f, 1f) > light.chance)
				{
					continue;
				}
				Thing thing = ThingGen.Create(light.id);
				Point point = new Point();
				for (int i = 0; i < 100; i++)
				{
					if (thing.TileType.UseMountHeight)
					{
						point.Set(x + EClass.rnd(width), y + EClass.rnd(height));
						Cell cell = point.cell;
						if (cell.Left.hasDoor || cell.Right.hasDoor || cell.Front.hasDoor || cell.Back.hasDoor || (!cell.Left.HasBlock && !cell.Right.HasBlock && !cell.Front.HasBlock && !cell.Back.HasBlock))
						{
							continue;
						}
					}
					else
					{
						int a = Mathf.Min(i / 3 + 2, width / 2);
						int a2 = Mathf.Min(i / 3 + 2, height / 2);
						point.Set(x + width / 2 + EClass.rnd(a) - EClass.rnd(a), y + height / 2 + EClass.rnd(a2) - EClass.rnd(a2));
					}
					if (!point.cell.blocked && !point.HasBlock && !point.HasObj && point.Installed == null)
					{
						int desiredDir = thing.TileType.GetDesiredDir(point, 0);
						if (desiredDir != -1)
						{
							thing.dir = desiredDir;
						}
						zone.AddCard(thing, point).Install();
						break;
					}
				}
				break;
			}
		}
		if (!PopulateCluster)
		{
			return;
		}
		Point point2 = new Point();
		for (int j = x; j < x + width; j++)
		{
			for (int k = y; k < y + height; k++)
			{
				point2.Set(j, k);
				if (!point2.cell.isModified && !point2.cell.blocked)
				{
					biome.Populate(point2, interior: true);
				}
			}
		}
	}

	public void SpawnMob(Point p)
	{
		if (EClass.rnd(100) <= 55 + zone.DangerLv * 3)
		{
			zone.SpawnMob(p);
		}
	}

	public virtual void OnPopulate()
	{
		for (int i = 0; i < EClass.rnd(3 + Size / 8) + 1; i++)
		{
			SetRandomPoint(delegate(Point p)
			{
				switch (EClass.rnd(7))
				{
				case 0:
				case 1:
					if (zone.biome.spawn.thing.Count > 0)
					{
						Thing thing2 = ThingGen.CreateFromFilter(zone.biome.spawn.GetRandomThingId(), EClass._zone.DangerLv);
						if (thing2 != null)
						{
							zone.AddCard(thing2, p);
						}
					}
					break;
				case 2:
					if (EClass.rnd(5) == 0)
					{
						map.SetObj(p.x, p.z, 82);
						if (EClass.rnd(2) == 0)
						{
							map.ApplyBackerObj(p);
						}
					}
					else
					{
						map.SetObj(p.x, p.z, ListWrecks.RandomItem());
					}
					break;
				default:
					SpawnMob(p);
					break;
				}
			});
		}
		if (Size <= 25)
		{
			return;
		}
		for (int j = 0; j < Size / 25 + EClass.rnd(Size / 25); j++)
		{
			SetRandomPoint(delegate(Point p)
			{
				if (EClass.rnd(3) == 0)
				{
					Thing thing = ThingGen.CreateFromFilter(zone.biome.spawn.GetRandomThingId(), EClass._zone.DangerLv);
					if (thing != null)
					{
						zone.AddCard(thing, p);
					}
				}
				else
				{
					SpawnMob(p);
				}
			});
		}
	}

	public void Fill()
	{
		BiomeProfile.TileFloor floor = group.floor;
		BiomeProfile.TileBlock block = group.block;
		int idMat = block.mat;
		if (zone is Zone_RandomDungeonNature && EClass.rndSeed(3, zone.uid) != 0)
		{
			block = EClass.core.refs.biomes.dict["Dungeon_Forest"].exterior.block;
			if (zone is Zone_RandomDungeonPlain)
			{
				idMat = 5;
			}
		}
		for (int i = x; i < x + width; i++)
		{
			for (int j = y; j < y + height; j++)
			{
				if (map.cells[i, j]._block != 0)
				{
					SetBlock(i, j, idMat, block.id, EClass.rnd(EClass.rnd(4) + 1));
				}
				SetFloor(i, j, floor.mat, floor.id, EClass.rnd(EClass.rnd(8) + 1));
			}
		}
	}

	public void SetRandomPoint(Action<Point> action)
	{
		int a = Mathf.Max(width - 2, 1);
		int a2 = Mathf.Max(height - 2, 1);
		for (int i = 0; i < 100; i++)
		{
			_p.x = x + 1 + EClass.rnd(a);
			_p.z = y + 1 + EClass.rnd(a2);
			_c = map.cells[_p.x, _p.z];
			if (!_c.blocked && !_c.HasBlock && !_p.HasThing && !_p.HasChara)
			{
				action(_p);
				break;
			}
		}
	}

	public void SetRandomPointCentered(Action<Point> action)
	{
		int a = Mathf.Max(width / 2 - 1, 1);
		int a2 = Mathf.Max(height / 2 - 1, 1);
		for (int i = 0; i < 100; i++)
		{
			_p.x = x + width / 2 + EClass.rnd(EClass.rnd(a) + 1) - EClass.rnd(EClass.rnd(a) + 1);
			_p.z = y + height / 2 + EClass.rnd(EClass.rnd(a2) + 1) - EClass.rnd(EClass.rnd(a2) + 1);
			_c = map.cells[_p.x, _p.z];
			if (!_c.blocked && !_c.HasBlock && !_p.HasThing && !_p.HasChara)
			{
				action(_p);
				break;
			}
		}
	}
}
