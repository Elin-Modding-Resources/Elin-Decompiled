using System;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : EScriptable
{
	public class Member
	{
		public Point pos = new Point();

		public int life;
	}

	public class Result
	{
		public List<Point> points = new List<Point>();

		public Point startPos;
	}

	public enum MoveType
	{
		Surface,
		Block
	}

	public enum StartType
	{
		Surface,
		Block
	}

	public static int[,] mapping = new int[0, 0];

	public static int sync;

	public int life;

	public int member;

	public int radius;

	public int repeat;

	public bool stickToStartBiome;

	public bool skipBorder;

	public StartType startType;

	public MoveType moveType;

	[NonSerialized]
	public int Size;

	[NonSerialized]
	public List<Member> members = new List<Member>();

	public bool CrawlUntil(Map map, Func<Point> onStart, int tries, Func<Result, bool> canComplete, Action onFail = null)
	{
		if (tries <= 0)
		{
			return false;
		}
		for (int i = 0; i < tries; i++)
		{
			bool flag = false;
			for (int j = 0; j < 100; j++)
			{
				Point point = onStart();
				switch (startType)
				{
				case StartType.Surface:
					if (!point.HasBlock)
					{
						flag = true;
					}
					break;
				case StartType.Block:
					if (point.sourceBlock.tileType.IsFullBlock && point.Installed == null)
					{
						flag = true;
					}
					break;
				}
			}
			if (!flag)
			{
				continue;
			}
			Result arg = Crawl(map, onStart());
			if (canComplete(arg))
			{
				if (i > 0)
				{
					Debug.Log("CrawlUntil Complete:" + i);
				}
				return true;
			}
		}
		if (onFail != null)
		{
			Debug.Log("CrawlUntil Fail" + tries);
			onFail?.Invoke();
		}
		return false;
	}

	public void Crawl(Map map)
	{
		for (int i = 0; (float)i < map.sizeModifier * (float)repeat + 1f; i++)
		{
			Crawl(map, map.GetRandomSurface(), delegate(Point p)
			{
				p.cell.biome.Populate(p);
			});
		}
	}

	public Result Crawl(Map map, Point _start, Action<Point> onNewVisit = null)
	{
		if (members.Count == 0)
		{
			for (int i = 0; i < this.member; i++)
			{
				members.Add(new Member());
			}
		}
		Point point = _start.Copy();
		point.Clamp();
		Result result = new Result();
		BiomeProfile biome = point.cell.biome;
		result.startPos = _start;
		sync++;
		Size = map.Size;
		int num = (skipBorder ? 2 : 0);
		int num2 = (skipBorder ? (Size - 2) : Size);
		if (mapping.Length != Size)
		{
			mapping = new int[map.Size, map.Size];
		}
		foreach (Member member2 in members)
		{
			member2.pos.Set(point);
			member2.life = life;
		}
		for (int j = 0; j < life; j++)
		{
			for (int k = 0; k < members.Count; k++)
			{
				Member member = members[k];
				int num3 = EScriptable.rnd(3) - 1;
				int num4 = EScriptable.rnd(3) - 1;
				int num5 = member.pos.x + num3;
				int num6 = member.pos.z + num4;
				if (point.Distance(num5, num6) > radius || num5 < num || num6 < num || num5 >= num2 || num6 >= num2)
				{
					continue;
				}
				Cell cell = map.cells[num5, num6];
				if (stickToStartBiome && cell.biome != biome)
				{
					continue;
				}
				switch (moveType)
				{
				case MoveType.Surface:
					if (cell._block != 0 || cell.IsTopWater)
					{
						continue;
					}
					break;
				case MoveType.Block:
					if (cell._block == 0 || !cell.sourceBlock.tileType.IsFullBlock || cell.Installed != null)
					{
						continue;
					}
					break;
				}
				member.pos.Set(num5, num6);
				if (mapping[num5, num6] != sync)
				{
					mapping[num5, num6] = sync;
					Point point2 = member.pos.Copy();
					result.points.Add(point2);
					onNewVisit?.Invoke(point2);
				}
			}
		}
		return result;
	}

	public static Crawler Create(string id)
	{
		return ResourceCache.Load<Crawler>("World/Map/Crawler/crawler " + id);
	}
}
