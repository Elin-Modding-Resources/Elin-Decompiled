using System.Collections.Generic;
using UnityEngine;

public class Lot : EClass
{
	public static Lot Void = new Lot();

	public static Dictionary<int, int> roofCount = new Dictionary<int, int>();

	public int id;

	public int height = 2;

	public int idRoofStyle;

	public int idBGM;

	public int heightFix;

	public int idRoofTile;

	public int idBlock;

	public int idRamp;

	public int idDeco;

	public int idDeco2;

	public int colRoof;

	public int colBlock;

	public int colDeco;

	public int colDeco2;

	public int decoFix;

	public int decoFix2;

	public bool reverse;

	public bool snow;

	public bool altRoof;

	public bool fullblock;

	public int x;

	public int z;

	public int mx;

	public int mz;

	public int mh;

	public bool sync;

	public bool dirty;

	public float realHeight;

	public float light;

	public Point pointMinX;

	public Point pointMaxX;

	private static int minX;

	private static int minZ;

	private static int maxX;

	private static int maxZ;

	public TraitHouseBoard board;

	public void SetBaseRoom(Room r)
	{
		board = null;
		fullblock = false;
		minX = r.x;
		maxX = r.mx;
		minZ = r.z;
		maxZ = r.mz;
		mh = r.mh;
		pointMinX = r.pointMinX;
		pointMaxX = r.pointMaxX;
		AddConnectedRooms(r);
		x = minX;
		mx = maxX;
		z = minZ;
		mz = maxZ;
		bool flag = false;
		roofCount.Clear();
		foreach (Room item in EClass._map.rooms.listRoom)
		{
			if (item.lot != this)
			{
				continue;
			}
			if (item.roof)
			{
				flag = true;
			}
			if (item.fullblock)
			{
				fullblock = true;
			}
			foreach (KeyValuePair<int, int> item2 in item.roofCount)
			{
				if (!roofCount.ContainsKey(item2.Key))
				{
					roofCount[item2.Key] = 0;
				}
				roofCount[item2.Key] += item2.Value;
			}
		}
		int num = 0;
		int num2 = 0;
		foreach (KeyValuePair<int, int> item3 in roofCount)
		{
			if (item3.Value > num)
			{
				num = item3.Value;
				num2 = item3.Key;
			}
		}
		if (board != null)
		{
			if (board.data == null)
			{
				TraitHouseBoard.Data data = (board.data = new TraitHouseBoard.Data());
				CoreRef.DefaultRoof defaultRoof = EClass.core.refs.defaultRoofs[EClass.sources.blocks[num2 / 1000].roof];
				data.height = (fullblock ? 1 : 2);
				data.heightFix = (fullblock ? 40 : 0);
				data.reverse = EClass.rnd(2) == 0;
				data.idRoofStyle = ((!EClass._map.IsIndoor && flag) ? 2 : 0);
				data.idBlock = defaultRoof.idBlock;
				data.idRamp = defaultRoof.idRamp;
				SourceMaterial.Row row = EClass.sources.materials[num2 % 1000];
				data.colRoof = IntColor.ToInt(row.matColor);
			}
			RefreshData(board.data);
		}
		else
		{
			CoreRef.DefaultRoof defaultRoof2 = EClass.core.refs.defaultRoofs[EClass.sources.blocks[num2 / 1000].roof];
			height = (fullblock ? 1 : 2);
			heightFix = (fullblock ? 40 : 0);
			idRoofStyle = ((!EClass._map.IsIndoor && flag) ? 2 : 0);
			idBlock = defaultRoof2.idBlock;
			idRamp = defaultRoof2.idRamp;
			realHeight = (float)height * EClass.setting.render.roomHeightMod + 0.01f * (float)heightFix;
			Color matColor = EClass.sources.materials.alias["oak"].matColor.SetAlpha(1f);
			colRoof = (colBlock = (colDeco = BaseTileMap.GetColorInt(ref matColor, 100)));
			SourceMaterial.Row row2 = EClass.sources.materials[num2 % 1000];
			colRoof = BaseTileMap.GetColorInt(ref row2.matColor, 100);
		}
		int num3 = (mx - x + pointMinX.z - z) / 2;
		int num4 = pointMaxX.x - pointMinX.x + pointMaxX.z - pointMinX.z + 1;
		int num5 = (mx - x + mz - z) / 2 + 4;
		int num6 = pointMinX.x + num3 - num5;
		int num7 = pointMinX.z - num3 + num5;
		int num8 = 0;
		for (int i = 0; i < num4; i++)
		{
			int num9 = num6;
			int num10 = num7;
			bool flag2 = false;
			for (int j = 0; j < num5 + 1; j++)
			{
				if (num10 < num8 - 1 || num9 < 0 || num10 < 0 || num9 >= EClass._map.Size || num10 >= EClass._map.Size)
				{
					num9++;
					num10--;
					continue;
				}
				Cell cell = EClass._map.cells[num9, num10];
				if (flag2 && (cell.room?.lot != this || (!cell.HasFullBlock && !cell.HasWall)))
				{
					break;
				}
				if (cell.room?.lot == this)
				{
					cell.lotWall = true;
					if (!flag2 && !cell.HasFullBlock && !cell.HasWall)
					{
						break;
					}
					flag2 = true;
					num8 = num10;
				}
				num9++;
				num10--;
			}
			if (i % 2 == 0)
			{
				num6++;
			}
			else
			{
				num7++;
			}
		}
	}

	public void RefreshData(TraitHouseBoard.Data data)
	{
		if (data.colRoof == 0)
		{
			data.colRoof = IntColor.ToInt(EClass.sources.materials.alias["oak"].matColor.SetAlpha(1f));
		}
		if (data.colBlock == 0)
		{
			data.colBlock = IntColor.ToInt(EClass.sources.materials.alias["oak"].matColor.SetAlpha(1f));
		}
		if (data.colDeco == 0)
		{
			data.colDeco = IntColor.ToInt(EClass.sources.materials.alias["oak"].matColor.SetAlpha(1f));
		}
		if (data.colDeco2 == 0)
		{
			data.colDeco2 = IntColor.ToInt(EClass.sources.materials.alias["oak"].matColor.SetAlpha(1f));
		}
		height = data.height;
		reverse = data.reverse;
		altRoof = data.altRoof;
		snow = data.snow;
		idRoofStyle = data.idRoofStyle;
		idRoofTile = data.idRoofTile;
		idBlock = data.idBlock;
		idRamp = data.idRamp;
		idBGM = data.idBGM;
		idDeco = data.idDeco;
		idDeco2 = data.idDeco2;
		heightFix = data.heightFix;
		decoFix = data.decoFix;
		decoFix2 = data.decoFix2;
		realHeight = (float)height * EClass.setting.render.roomHeightMod + 0.01f * (float)heightFix;
		Color matColor = IntColor.FromInt(data.colRoof);
		Color matColor2 = IntColor.FromInt(data.colBlock);
		Color matColor3 = IntColor.FromInt(data.colDeco);
		Color matColor4 = IntColor.FromInt(data.colDeco2);
		colRoof = BaseTileMap.GetColorInt(ref matColor, 100);
		colBlock = BaseTileMap.GetColorInt(ref matColor2, 100);
		colDeco = BaseTileMap.GetColorInt(ref matColor3, 100);
		colDeco2 = BaseTileMap.GetColorInt(ref matColor4, 100);
	}

	public void AddConnectedRooms(Room r2)
	{
		r2.lot = this;
		if (r2.x < minX)
		{
			minX = r2.x;
		}
		if (r2.mx > maxX)
		{
			maxX = r2.mx;
		}
		if (r2.z < minZ)
		{
			minZ = r2.z;
		}
		if (r2.mz > maxZ)
		{
			maxZ = r2.mz;
		}
		if (r2.mh > mh)
		{
			mh = r2.mh;
		}
		if (r2.pointMinX.x + r2.pointMinX.z < pointMinX.x + pointMinX.z)
		{
			pointMinX = r2.pointMinX;
		}
		if (r2.pointMaxX.x + r2.pointMaxX.z > pointMaxX.x + pointMaxX.z)
		{
			pointMaxX = r2.pointMaxX;
		}
		if (board == null)
		{
			foreach (Point point in r2.points)
			{
				TraitHouseBoard installed = point.GetInstalled<TraitHouseBoard>();
				if (installed != null)
				{
					board = installed;
					break;
				}
			}
		}
		foreach (Room item in EClass._map.rooms.listRoom)
		{
			if (item.lot != null)
			{
				continue;
			}
			bool flag = false;
			foreach (Point point2 in item.points)
			{
				foreach (Point point3 in r2.points)
				{
					if (point3.Distance(point2) == 1 && r2.data.group == item.data.group)
					{
						AddConnectedRooms(item);
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
	}
}
