using System.Collections.Generic;
using UnityEngine;

public class MapGen : BaseMapGen
{
	private static MapGen _Instance;

	public static MapGen Instance => _Instance ?? (_Instance = new MapGen());

	protected override void GenerateTerrain()
	{
		SetSize((zone is Zone_Gathering) ? 60 : ((!zp.useRootSize) ? zp.size : (zone.GetTopZone().bounds?.Size ?? zp.size)), 10);
		if (biomes == null || Size != biomes.GetLength(0))
		{
			biomes = new BiomeProfile[Size, Size];
			subBiomes = new bool[Size, Size];
			heights1 = new float[Size, Size];
			heights2 = new float[Size, Size];
			heights3 = new float[Size, Size];
			waters = new float[Size, Size];
			heights3d = new float[Size, Size];
			lastSize = Size;
		}
		map.biomes = biomes;
		layerHeight.SaveSettings();
		layerStratum.SaveSettings();
		layerBiome.SaveSettings();
		layerRiver.SaveSettings();
		layerBiome.SaveSettings();
		skipWater = zp.noWater || (bp.tileCenter != null && (bp.tileCenter.IsNeighborRoad || bp.tileCenter.isRoad));
		for (int i = 0; i < 100; i++)
		{
			seed = (map.seed = bp.genSetting.seed + i);
			Rand.SetSeed(seed);
			if (OnGenerateTerrain())
			{
				break;
			}
			Debug.Log("Failed map generation:" + i + " / " + BaseMapGen.err);
			skipWater = true;
		}
		map.SetZone(EClass._zone);
		for (int j = 0; j < Size; j++)
		{
			for (int k = 0; k < Size; k++)
			{
				map.QuickRefreshTile(j, k);
			}
		}
		Rand.SetSeed();
	}

	protected override bool OnGenerateTerrain()
	{
		int idMat = 66;
		if (map.isGenerated)
		{
			if (map.Size != Size)
			{
				map.Resize(Size);
			}
			map.Reset();
		}
		else
		{
			map.CreateNew(Size);
		}
		map.poiMap.Reset();
		if (zone is Zone_Gathering)
		{
			map.SetBounds(Size - 20);
		}
		else if (bp.zoneProfile.useRootSize && zone.GetTopZone().bounds != null)
		{
			map.SetBounds(zone.GetTopZone().bounds);
		}
		else
		{
			map.SetBounds((bp.zoneProfile.sizeBounds == 0) ? EClass.setting.defaultMapSize : bp.zoneProfile.sizeBounds);
		}
		switch (zp.genType)
		{
		case ZoneProfile.GenType.Underground:
			map.config.idBiome = "Underground";
			zone._biome = null;
			break;
		case ZoneProfile.GenType.Sky:
		{
			map.config.idBiome = "Sky";
			zone._biome = null;
			for (int i = 0; i < Size; i++)
			{
				for (int j = 0; j < Size; j++)
				{
					SetFloor(i, j, 0, 90);
				}
			}
			return true;
		}
		}
		waterCount = 0f;
		if (!skipWater)
		{
			float num = (float)(Size * Size) * variation.maxWaterRatio;
			layerRiver.FillHeightMap(waters, OX, 0, OZ, Size, Size, seed);
			for (int k = 0; k < Size; k++)
			{
				for (int l = 0; l < Size; l++)
				{
					waters[k, l] += zp.water;
					if (waters[k, l] > 0f)
					{
						waterCount += 1f;
					}
				}
				if (waterCount > num)
				{
					BaseMapGen.err = "Too many water";
					return false;
				}
			}
		}
		layerBiome.FillHeightMap(heights1, OX, 0, OZ, Size, Size, zp.seeds.biome);
		layerBiome.FillHeightMap(heights2, OX, 0, OZ, Size, Size, zp.seeds.biome + 1);
		if (base.extraBiome)
		{
			layerBiome.FillHeightMap(heights3, OX, 0, OZ, Size, Size, zp.seeds.biome + 2);
		}
		for (int m = 0; m < Size; m++)
		{
			for (int n = 0; n < Size; n++)
			{
				if (heights1[m, n] != 0f)
				{
					biomes[m, n] = biomeProfiles[1];
				}
				else if (heights2[m, n] != 0f)
				{
					biomes[m, n] = biomeProfiles[2];
				}
				else if (base.extraBiome && heights3[m, n] != 0f)
				{
					biomes[m, n] = biomeProfiles[3];
				}
				else
				{
					biomes[m, n] = biomeProfiles[0];
				}
			}
		}
		layerBiome.FillHeightMap(heights1, OX, 0, OZ, Size, Size, zp.seeds.biomeSub, zp.biomeSubScale);
		for (int num2 = 0; num2 < Size; num2++)
		{
			for (int num3 = 0; num3 < Size; num3++)
			{
				subBiomes[num2, num3] = heights1[num2, num3] != 0f;
			}
		}
		layerBiome.FillHeightMap(heights1, OX, 0, OZ, Size, Size, zp.seeds.biome);
		for (int num4 = 0; num4 < biomeProfiles.Length; num4++)
		{
			if (num4 != 0 && biomeProfiles[num4 - 1] == biomeProfiles[num4])
			{
				continue;
			}
			biomeProfiles[num4].layerBlock.FillHeightMap(heights1, OX, 0, OZ, Size, Size, zp.seeds.biome);
			bool flag = EClass.sources.floors.rows[biomeProfiles[num4].exterior.floor.id].tag.Contains("soil");
			for (int num5 = 0; num5 < Size; num5++)
			{
				if (zp.clearEdge && ((num5 > map.bounds.x - 4 && num5 <= map.bounds.x + 4) || (num5 > map.bounds.maxX - 4 && num5 <= map.bounds.maxX + 4)))
				{
					continue;
				}
				for (int num6 = 0; num6 < Size; num6++)
				{
					if ((!zp.clearEdge || ((num6 <= map.bounds.z - 4 || num6 > map.bounds.z + 4) && (num6 <= map.bounds.maxZ - 4 || num6 > map.bounds.maxZ + 4))) && !(biomes[num5, num6] != biomeProfiles[num4]) && heights1[num5, num6] > (float)blockHeight)
					{
						SourceBlock.Row row = EClass.sources.blocks.rows[biomeProfiles[num4].exterior.block.id];
						if (!flag || row.alias == "block_ice")
						{
							SourceFloor.Row row2 = EClass.sources.floors.alias[row.autoFloor];
							SetFloor(num5, num6, row2.DefaultMaterial.id, row2.id);
						}
						SetBlock(num5, num6, biomeProfiles[num4].exterior.block.mat, biomeProfiles[num4].exterior.block.id);
					}
				}
			}
		}
		bool flag2 = false;
		for (int num7 = Size / 2 - 4; num7 < Size / 2 + 4; num7++)
		{
			for (int num8 = Size / 2 - 4; num8 < Size / 2 + 4; num8++)
			{
				if (num7 >= 0 && num8 >= 0 && num7 < Size && num8 < Size && map.cells[num7, num8]._block != 0)
				{
					flag2 = true;
					break;
				}
			}
		}
		if (flag2)
		{
			for (int num9 = 0; num9 < Size; num9++)
			{
				for (int num10 = 0; num10 < Size; num10++)
				{
					SetBlock(num9, num10, 0, 0);
				}
			}
		}
		layerHeight.FillHeightMap(heights1, OX, 0, OZ, Size, Size, seed);
		layerHeight.FillHeightMap(heights2, OX, 0, OZ, Size, Size, zp.seeds.bush);
		layerHeight.FillHeightMap(heights3, OX, 0, OZ, Size, Size, seed + 1);
		BiomeProfile biomeProfile = biomeWater;
		byte b = (byte)EClass.setting.maxGenHeight;
		for (int num11 = 0; num11 < Size; num11++)
		{
			for (int num12 = 0; num12 < Size; num12++)
			{
				int num13 = (int)heights1[num11, num12];
				Cell cell = map.cells[num11, num12];
				if (cell._block != 0 && cell._floor != 0)
				{
					continue;
				}
				BiomeProfile biomeProfile2 = biomes[num11, num12];
				SourceMaterial.Row row3 = (subBiomes[num11, num12] ? biomeProfile2.MatSub : biomeProfile2.MatFloor);
				int dir = biomeProfile2.exterior.floor.GetDir();
				byte b2 = (byte)(heights1[num11, num12] * (float)hSetting.heightMod + (float)hSetting.baseHeight);
				if (hSetting.step > 0)
				{
					b2 = (byte)(b2 / hSetting.step * hSetting.step);
				}
				if (b2 > b)
				{
					b2 = b;
				}
				cell.height = b2;
				if (!skipWater && waters[num11, num12] > 0f && cell._block == 0)
				{
					cell.height = 0;
					if ((bool)biomeProfile)
					{
						bool flag3 = false;
						for (int num14 = num12 - 2; num14 < num12 + 3; num14++)
						{
							for (int num15 = num11 - 2; num15 < num11 + 3; num15++)
							{
								if (num14 >= 0 && num15 >= 0 && num15 < Size && num14 < Size && waters[num15, num14] <= 0f)
								{
									flag3 = true;
									break;
								}
							}
						}
						SetFloor(num11, num12, biomeWater.MatFloor.id, flag3 ? 44 : 43);
					}
					else
					{
						SetFloor(num11, num12, idMat, (waters[num11, num12] < 5f) ? 44 : 43);
					}
				}
				else if (zp.shoreHeight > 0f && (float)num13 < zp.shoreHeight)
				{
					SetFloor(num11, num12, biomeShore.MatFloor.id, biomeShore.MatFloor.defFloor);
					biomes[num11, num12] = biomeShore;
				}
				else if (biomeProfile2.floor_height != 0 && (int)heights2[num11, num12] + zp.bushMod >= 4)
				{
					SetFloor(num11, num12, (byte)row3.id, biomeProfile2.floor_height, dir);
				}
				else
				{
					SetFloor(num11, num12, (byte)row3.id, (biomeProfile2.exterior.floor.id != 0) ? biomeProfile2.exterior.floor.id : row3.defFloor, dir);
				}
			}
		}
		if (hSetting.heightMod > 0)
		{
			ModifyHeight(map);
		}
		MakeNeighbor();
		for (int num16 = 0; num16 < Size; num16++)
		{
			for (int num17 = 0; num17 < Size; num17++)
			{
				Cell cell2 = map.cells[num16, num17];
				if (!zp.setShore || cell2.HasBlock || cell2.IsTopWater || cell2.height > 20)
				{
					continue;
				}
				bool flag4 = false;
				for (int num18 = num16 - 1; num18 < num16 + 2; num18++)
				{
					if (num18 < 0 || num18 >= Size)
					{
						continue;
					}
					for (int num19 = num17 - 1; num19 < num17 + 2; num19++)
					{
						if (num19 >= 0 && num19 < Size && (num18 != num16 || num19 != num17) && map.cells[num18, num19].IsTopWater)
						{
							flag4 = true;
							break;
						}
					}
					if (flag4)
					{
						break;
					}
				}
				if (!flag4)
				{
					continue;
				}
				SetFloor(num16, num17, biomeShore.MatFloor.id, biomeShore.MatFloor.defFloor);
				biomes[num16, num17] = biomeShore;
				if (zp.extraShores <= 0)
				{
					continue;
				}
				int extraShores = zp.extraShores;
				for (int num20 = num16 - extraShores; num20 < num16 + extraShores + 1; num20++)
				{
					if (num20 < 0 || num20 >= Size)
					{
						continue;
					}
					for (int num21 = num17 - extraShores; num21 < num17 + extraShores + 1; num21++)
					{
						if (num21 >= 0 && num21 < Size && !(biomes[num20, num21] == biomeShore) && !map.cells[num20, num21].IsTopWater && !map.cells[num20, num21].HasBlock)
						{
							SetFloor(num20, num21, biomeShore.MatFloor.id, biomeShore.MatFloor.defFloor);
							biomes[num20, num21] = biomeShore;
						}
					}
				}
			}
		}
		return true;
	}

	public void MakeNeighbor()
	{
		EloMap.TileInfo thisInfo;
		int seaDir;
		if (bp.surrounding != null && !zone.IsUnderwater)
		{
			thisInfo = bp.surrounding[1, 1];
			seaDir = (thisInfo.sea ? (1 + EClass.rnd(4)) : 0);
			_MakeNeighbor(bp.surrounding[1, 2], Size / 3 + 7, new Point(0, Size - 1), 0, -1, 3, 1);
			_MakeNeighbor(bp.surrounding[1, 0], Size / 3 + 7, new Point(0, 0), 0, 1, 1, 3);
			_MakeNeighbor(bp.surrounding[2, 1], Size / 3 + 7, new Point(Size - 1, 0), -1, 0, 2, 4);
			_MakeNeighbor(bp.surrounding[0, 1], Size / 3 + 7, new Point(0, 0), 1, 0, 4, 2);
			EClass._map.config.seaDir = seaDir;
		}
		void _MakeNeighbor(EloMap.TileInfo info, int _s, Point p, int vx, int vz, int _seaDir1, int _seaDir2)
		{
			int num = _s;
			int num2 = 4;
			Point point = new Point();
			while (p.IsValid)
			{
				point.Set(p);
				num2--;
				if (num2 < 0)
				{
					num += ((EClass.rnd(2) == 0) ? 1 : (-1));
					num = Mathf.Clamp(num, _s - 3, _s + 3);
					num2 = 2 + EClass.rnd(4);
				}
				for (int i = 0; i < num; i++)
				{
					if (vx != 0)
					{
						point.x = p.x + i * vx;
					}
					else
					{
						point.z = p.z + i * vz;
					}
					if (point.IsValid)
					{
						if (info.sea)
						{
							if (!thisInfo.sea)
							{
								seaDir = _seaDir1;
							}
							SetFloor(point.x, point.z, 66, (i >= num - 3) ? 44 : 43);
						}
						else if (info.rock)
						{
							SetBlock(point.x, point.z, 45, 1);
						}
						else if (info.shore)
						{
							if (!thisInfo.shore)
							{
								seaDir = _seaDir2;
							}
							SetFloor(point.x, point.z, 97, 33);
						}
					}
				}
				if (vx == 0)
				{
					p.x++;
				}
				else
				{
					p.z++;
				}
			}
		}
	}

	public void MakeNeighbor_old()
	{
		if (bp.surrounding != null)
		{
			_MakeNeighbor(bp.surrounding[1, 2], new Point(Size / 2, Size / 3 * 2 - 5), 0, 1);
			_MakeNeighbor(bp.surrounding[1, 0], new Point(Size / 2, Size / 3 + 6), 0, -1);
			_MakeNeighbor(bp.surrounding[2, 1], new Point(Size / 3 * 2 - 5, Size / 2), 1, 0);
			_MakeNeighbor(bp.surrounding[0, 1], new Point(Size / 3 + 6, Size / 2), -1, 0);
		}
		void _MakeNeighbor(EloMap.TileInfo info, Point p, int vx, int vz)
		{
			Point point = new Point();
			while (p.IsValid)
			{
				point.Set(p);
				int num = 0;
				num = EClass.rnd(2) - EClass.rnd(2);
				if (vx != 0)
				{
					p.x += num;
				}
				else
				{
					p.z += num;
				}
				for (int i = -Size / 2; i < Size / 2; i++)
				{
					if (vx != 0)
					{
						point.z = p.z + i;
					}
					else
					{
						point.x = p.x + i;
					}
					if (point.IsValid)
					{
						if (info.sea)
						{
							SetFloor(point.x, point.z, 66, 43);
						}
						else if (info.rock)
						{
							SetBlock(point.x, point.z, 45, 1);
						}
						else if (info.shore)
						{
							SetFloor(point.x, point.z, 97, 33);
						}
					}
				}
				p.x += vx;
				p.z += vz;
			}
		}
	}

	public void MakeRoad()
	{
		if (bp.tileCenter != null && !bp.ignoreRoad)
		{
			EloMap.TileInfo tileCenter = bp.tileCenter;
			if (tileCenter.roadLeft)
			{
				_MakeRoad(-1, 0);
			}
			if (tileCenter.roadRight)
			{
				_MakeRoad(1, 0);
			}
			if (tileCenter.roadUp)
			{
				_MakeRoad(0, 1);
			}
			if (tileCenter.roadDown)
			{
				_MakeRoad(0, -1);
			}
		}
		void _MakeRoad(int vx, int vz)
		{
			Point point = new Point(Size / 2, Size / 2);
			Point point2 = new Point();
			while (point.IsValid)
			{
				point2.Set(point);
				for (int i = -1; i < 2; i++)
				{
					if (vx != 0)
					{
						point2.z = point.z + i;
					}
					else
					{
						point2.x = point.x + i;
					}
					if (point2.IsValid)
					{
						SetFloor(point2.x, point2.z, 45, 40);
						SetBlock(point2.x, point2.z, 0, 0);
						point2.cell.obj = 0;
					}
				}
				point.x += vx;
				point.z += vz;
				if (EClass.rnd(30) == 0)
				{
					point.x += ((vx == 0) ? (EClass.rnd(3) - 1) : 0);
					point.z += ((vz == 0) ? (EClass.rnd(3) - 1) : 0);
				}
			}
		}
	}

	public void ModifyHeight(Map _map)
	{
		map = _map;
		Cell[,] cells = map.cells;
		bool flag = false;
		for (int num = Size - 1; num > 0; num--)
		{
			for (int i = 0; i < Size - 1; i++)
			{
				Cell cell = cells[num, i];
				byte b = cell.height;
				Cell cell2 = cells[num - 1, i];
				Cell cell3 = cells[num, i + 1];
				if (cell2.IsTopWater || cell3.IsTopWater)
				{
					continue;
				}
				if (EClass.rnd(500) == 0)
				{
					flag = !flag;
				}
				if (EClass.rnd(3) == 0)
				{
					if (flag)
					{
						cell2.height = b;
					}
					else
					{
						cell3.height = b;
					}
				}
				if (hSetting.smoothDownhill && EClass.rnd(3) == 0)
				{
					if (cell2.height < b - 2)
					{
						cell2.height = (byte)(b - 2);
					}
					if (cell3.height < b - 2)
					{
						cell3.height = (byte)(b - 2);
					}
					continue;
				}
				if (hSetting.mod1 && (cell2.height >= b - 1 || cell3.height >= b - 1))
				{
					if (EClass.rnd(4) == 0 && b != 0)
					{
						cell2.height = (cell3.height = b);
					}
					else
					{
						cell.height = (cell2.height = cell3.height);
					}
				}
				if (hSetting.mod2 && (cell2.height < b - 1 || cell3.height < b - 1))
				{
					if (EClass.rnd(2) == 0)
					{
						cell2.height = (cell3.height = b);
					}
					else
					{
						cells[num, i].height = (cell2.height = cell3.height);
					}
				}
				if (hSetting.mod3 && (cell2.height >= b - 1 || cell3.height >= b - 1))
				{
					if (EClass.rnd(2) == 0)
					{
						cell2.height = (cell3.height = b);
					}
					else
					{
						cells[num, i].height = (cell2.height = cell3.height);
					}
				}
			}
		}
		if (!hSetting.smoothDownhill)
		{
			return;
		}
		for (int num2 = Size - 1; num2 > 0; num2--)
		{
			for (int j = 0; j < Size - 1; j++)
			{
				byte b2 = cells[num2, j].height;
				Cell cell4 = cells[num2 - 1, j];
				Cell cell5 = cells[num2, j + 1];
				if (b2 != 0)
				{
					if (cell4.height < b2 - 2 && !cell4.IsTopWater)
					{
						cell4.height = (byte)(b2 - 2);
					}
					if (cell5.height < b2 - 2 && !cell5.IsTopWater)
					{
						cell5.height = (byte)(b2 - 2);
					}
				}
			}
		}
	}

	public void MakeRiver(Map _map)
	{
		int num = 0;
		for (int i = 0; i < 100; i++)
		{
			Point point = null;
			Point point2 = null;
			for (int j = 0; j < 1000; j++)
			{
				point = _map.GetRandomEdge(1);
				if (!point.cell.blocked)
				{
					break;
				}
			}
			for (int k = 0; k < 1000; k++)
			{
				point2 = _map.GetRandomEdge(1);
				if (point.x != point2.x && point.z != point2.z && !point2.cell.blocked && point.Distance(point2) >= _map.Size / 2)
				{
					break;
				}
			}
			if (TryMakeRiver(point, point2))
			{
				num += 1 + EClass.rnd(2);
				if (num > 10)
				{
					break;
				}
			}
		}
	}

	public bool TryMakeRiver(Point p1, Point p2)
	{
		List<Point> list = new List<Point>();
		int num = 30;
		for (int i = 0; i < num; i++)
		{
			Point point = new Point();
			point.x = p1.x + (p2.x - p1.x) * i / num;
			point.z = p1.z + (p2.z - p1.z) * i / num;
			list.Add(point);
		}
		for (int j = 0; j < num; j++)
		{
			Point point2 = list[j];
			if (p1.Equals(point2))
			{
				continue;
			}
			while (!p1.IsWater)
			{
				SetBlock(p1.x, p1.z, 0, 0);
				SetFloor(p1.x, p1.z, 67, 44);
				if (EClass.rnd(2) == 0)
				{
					if (p1.x != point2.x)
					{
						p1.x += ((p1.x <= point2.x) ? 1 : (-1));
					}
					else if (p1.z != point2.z)
					{
						p1.z += ((p1.z <= point2.z) ? 1 : (-1));
					}
				}
				else if (p1.z != point2.z)
				{
					p1.z += ((p1.z <= point2.z) ? 1 : (-1));
				}
				else if (p1.x != point2.x)
				{
					p1.x += ((p1.x <= point2.x) ? 1 : (-1));
				}
				if (p1.Equals(point2))
				{
					break;
				}
			}
			if (p1.IsWater || p1.Equals(p2))
			{
				return true;
			}
		}
		return true;
	}

	public void Populate(Map _map)
	{
		map = _map;
		EClass._zone.isShore = bp.zoneProfile.isShore;
		Rand.SetSeed(zp.seeds.poi);
		if (zp.river)
		{
			MakeRiver(_map);
		}
		map.RefreshAllTiles();
		layerStratum.FillHeightMap(heights1, OX, 0, OZ, Size, Size, seed);
		layerStratum.FillHeightMap(heights2, OX, 0, OZ, Size, Size, seed + 1);
		layerStratum.FillHeightMap(heights3, OX, 0, OZ, Size, Size, seed + 2);
		layerStratum.FillHeightMap(heights3d, OX, 0, OZ, Size, Size, seed);
		Point point = new Point();
		for (int i = 0; i < Size; i++)
		{
			for (int j = 0; j < Size; j++)
			{
				point.Set(i, j);
				Cell cell = point.cell;
				if (cell.IsTopWater && (bool)biomeWater)
				{
					if (cell.Left.IsTopWater && cell.Right.IsTopWater && cell.Front.IsTopWater && cell.Back.IsTopWater)
					{
						biomeWater.Populate(point);
					}
					continue;
				}
				BiomeProfile biome = cell.biome;
				if (!cell.HasBlock)
				{
					biome.Populate(point);
				}
			}
		}
		if (zp.name == "R_Plain")
		{
			Crawler crawler = Crawler.Create("pasture");
			int tries = 10;
			crawler.CrawlUntil(_map, () => _map.GetRandomPoint(), tries, delegate(Crawler.Result r)
			{
				int id = ((EClass.rnd(3) == 0) ? 108 : 105);
				foreach (Point point2 in r.points)
				{
					map.SetObj(point2.x, point2.z, id);
					int num = 3;
					if (EClass.rnd(6) == 0)
					{
						num++;
					}
					point2.growth.SetStage(num);
				}
				return false;
			});
		}
		if (crawlers != null)
		{
			Crawler[] array = crawlers;
			for (int k = 0; k < array.Length; k++)
			{
				array[k].Crawl(map);
			}
		}
		MakeRoad();
		Rand.SetSeed();
	}

	public void Output()
	{
		Debug.Log(zp.name + "/" + variation.name);
		Debug.Log("seed:" + EClass._map.seed + "  offset: " + OX + "/" + OZ);
	}
}
