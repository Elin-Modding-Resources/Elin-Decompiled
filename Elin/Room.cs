using System.Collections.Generic;

public class Room : BaseArea
{
	public int x;

	public int z;

	public int mx;

	public int mz;

	public int mh;

	public bool dirty;

	public bool roof;

	public bool fullblock;

	public Lot lot;

	public Point pointMinX;

	public Point pointMaxX;

	public Dictionary<long, int> roofCount = new Dictionary<long, int>();

	public bool HasRoof => lot.idRoofStyle != 0;

	public override string ToString()
	{
		return uid.ToString() ?? "";
	}

	public void OnLoad()
	{
		foreach (Point point in points)
		{
			point.cell.room = this;
		}
		EClass._map.rooms.mapIDs[uid] = this;
		SetDirty();
	}

	public void AddPoint(Point p)
	{
		points.Add(p);
		p.cell.room = this;
		lot = Lot.Void;
		SetDirty();
	}

	public void OnRemove()
	{
		Clean();
		for (int num = points.Count - 1; num >= 0; num--)
		{
			points[num].cell.room = null;
		}
		points.Clear();
		EClass._map.rooms.dirtyLots = true;
	}

	public void Clean()
	{
		plate = null;
	}

	public void Refresh()
	{
		Clean();
		roofCount.Clear();
		x = (mx = points[0].x);
		z = (mz = points[0].z);
		mh = 0;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		pointMinX = points[0];
		pointMaxX = points[0];
		foreach (Point point in points)
		{
			Cell cell = point.cell;
			if (point.x < x)
			{
				x = point.x;
			}
			else if (point.x > mx)
			{
				mx = point.x;
			}
			if (point.z < z)
			{
				z = point.z;
			}
			else if (point.z > mz)
			{
				mz = point.z;
			}
			if (point.x + point.z < pointMinX.x + pointMinX.z)
			{
				pointMinX = point;
			}
			if (point.x + point.z > pointMaxX.x + pointMaxX.z)
			{
				pointMaxX = point;
			}
			bool hasFullBlockOrWallOrFence = cell.HasFullBlockOrWallOrFence;
			if (cell.TopHeight > mh && hasFullBlockOrWallOrFence)
			{
				mh = cell.TopHeight;
			}
			if (hasFullBlockOrWallOrFence)
			{
				TileType tileType = point.sourceBlock.tileType;
				if (tileType.IsWall)
				{
					num3++;
				}
				else if (tileType.IsFence)
				{
					num2++;
				}
				else
				{
					num++;
				}
				long key = ((long)point.sourceBlock.id << 32) | (uint)point.matBlock.id;
				roofCount.TryAdd(key, 0);
				roofCount[key]++;
			}
			if (plate == null)
			{
				plate = point.GetInstalled<TraitRoomPlate>();
			}
			num4 += point.x + point.z * 256;
			cell.fullWall = false;
			cell.lotWall = false;
		}
		int num5 = (mx - x + pointMinX.z - z) / 2;
		int num6 = pointMaxX.x - pointMinX.x + pointMaxX.z - pointMinX.z + 1;
		int num7 = (mx - x + mz - z) / 2 + 2;
		int num8 = pointMinX.x + num5 - num7;
		int num9 = pointMinX.z - num5 + num7;
		int num10 = 0;
		for (int i = 0; i < num6; i++)
		{
			int num11 = num8;
			int num12 = num9;
			bool flag = false;
			for (int j = 0; j < num7 + 1; j++)
			{
				if (num12 < num10 - 1 || num11 < 0 || num12 < 0 || num11 >= EClass._map.Size || num12 >= EClass._map.Size)
				{
					num11++;
					num12--;
					continue;
				}
				Cell cell2 = EClass._map.cells[num11, num12];
				if (flag && (cell2.room != this || (!cell2.HasFullBlock && !cell2.HasWall)))
				{
					break;
				}
				if (cell2.room == this)
				{
					cell2.fullWall = true;
					if (!flag && !cell2.HasFullBlock && !cell2.HasWall)
					{
						break;
					}
					flag = true;
					num10 = num12;
				}
				num11++;
				num12--;
			}
			if (i % 2 == 0)
			{
				num8++;
			}
			else
			{
				num9++;
			}
		}
		fullblock = num > num3;
		roof = (mx - x + (mz - z)) / 2 > num2;
		dirty = false;
		EClass._map.rooms.dirtyLots = true;
		type = new AreaTypeRoom
		{
			owner = this
		};
		SetRandomName(num4);
		if (plate != null)
		{
			data.name = plate.areaData.name;
			if (plate.areaData.type != null)
			{
				type = plate.areaData.type;
			}
			data.group = plate.areaData.group;
			data.atrium = plate.areaData.atrium;
			data.accessType = plate.areaData.accessType;
		}
	}

	public void SetDirty()
	{
		dirty = true;
		EClass._map.rooms.dirtyRooms = true;
	}
}
