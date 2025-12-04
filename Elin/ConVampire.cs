using Newtonsoft.Json;
using UnityEngine;

public class ConVampire : Condition
{
	public static int[] List_Vampire = new int[13]
	{
		70, 72, 71, 77, 74, 75, 76, 73, 60, 61,
		79, 300, 301
	};

	[JsonProperty]
	private ElementContainer ec = new ElementContainer();

	public override int GetPhase()
	{
		return 0;
	}

	public override ElementContainer GetElementContainer()
	{
		return ec;
	}

	public override void Tick()
	{
		if (EClass.rnd(2) != 0)
		{
			return;
		}
		int num = List_Vampire.RandomItem();
		int num2 = ec.Value(num);
		int num3 = owner.elements.Base(num) / 7 + 5;
		if (EClass.world.date.IsNight)
		{
			switch (num)
			{
			case 60:
			case 61:
				num3 = 20;
				break;
			case 79:
				num3 = 20;
				break;
			}
			if (num2 < num3 && (float)EClass.rnd(100) >= 99f * (float)num2 / (float)num3)
			{
				ec.ModBase(num, Mathf.Clamp(num3 / 10, 1, num3 - num2));
			}
		}
		else if (num2 > 0)
		{
			ec.ModBase(num, -Mathf.Clamp(num3 / 4, 1, num2));
		}
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		ec.SetParent(owner);
	}

	public override void OnRemoved()
	{
		ec.SetParent();
	}
}
