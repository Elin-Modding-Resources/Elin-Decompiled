using System.Collections.Generic;
using Newtonsoft.Json;

public class WorkSummary : EClass
{
	[JsonProperty]
	public int[] ints = new int[10];

	[JsonProperty]
	public WorkSession work;

	[JsonProperty]
	public List<WorkSession> hobbies = new List<WorkSession>();

	[JsonProperty]
	public List<Thing> things = new List<Thing>();

	public int progress
	{
		get
		{
			return ints[0];
		}
		set
		{
			ints[0] = value;
		}
	}

	public int money
	{
		get
		{
			return ints[5];
		}
		set
		{
			ints[5] = value;
		}
	}

	public int food
	{
		get
		{
			return ints[6];
		}
		set
		{
			ints[6] = value;
		}
	}

	public int knowledge
	{
		get
		{
			return ints[7];
		}
		set
		{
			ints[7] = value;
		}
	}

	public int material
	{
		get
		{
			return ints[8];
		}
		set
		{
			ints[8] = value;
		}
	}

	public void Reset()
	{
		int num = (material = 0);
		int num3 = (knowledge = num);
		int num5 = (food = num3);
		int num7 = (money = num5);
		progress = num7;
	}

	public void AddThing(Thing t)
	{
		things.Add(t);
	}
}
