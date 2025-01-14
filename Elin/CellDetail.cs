using System.Collections.Generic;

public class CellDetail
{
	public static int count;

	public static Map map;

	public static Stack<CellDetail> cache = new Stack<CellDetail>();

	public List<Thing> things = new List<Thing>(1);

	public List<Chara> charas = new List<Chara>(1);

	public Area area;

	public Footmark footmark;

	public TaskDesignation designation;

	public TransAnime anime;

	public static CellDetail Empty => new CellDetail();

	public static CellDetail Spawn()
	{
		if (cache.Count > 0)
		{
			return cache.Pop();
		}
		count++;
		return new CellDetail();
	}

	public bool TryDespawn()
	{
		if (things.Count > 0 || charas.Count > 0 || area != null || footmark != null || designation != null || anime != null)
		{
			return false;
		}
		cache.Push(this);
		return true;
	}

	public void MoveThingToBottom(Thing t)
	{
		if (things.Count != 1)
		{
			things.Remove(t);
			things.Insert(0, t);
			for (int i = 0; i < things.Count && things[i].IsInstalled; i++)
			{
				things[i].stackOrder = i;
			}
		}
	}

	public void MoveThingToTop(Thing t)
	{
		if (things.Count != 1)
		{
			int num = 0;
			for (int i = 0; i < things.Count && things[i].IsInstalled; i++)
			{
				num = i + 1;
			}
			if (num >= things.Count)
			{
				num--;
			}
			things.Remove(t);
			things.Insert(num, t);
			t.stackOrder = num;
		}
	}
}
