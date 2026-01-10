using Newtonsoft.Json;
using UnityEngine;

public class ConPeaky : Condition
{
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
		if (EClass.rnd(2) == 0)
		{
			return;
		}
		int num = ec.Value(79);
		int num2 = owner.Evalue(1423) * 10;
		if (num < num2)
		{
			ec.ModBase(79, Mathf.Clamp(num2 / 10, 1, num2 - num));
			if (ec.Value(79) == num2)
			{
				owner.PlaySound("bike_kick");
			}
		}
	}

	public void OnHit()
	{
		ec.SetBase(79, ec.Value(79) / 2);
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
