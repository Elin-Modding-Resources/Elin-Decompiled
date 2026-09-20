using Newtonsoft.Json;

public class ConTimeStop : Condition
{
	[JsonProperty]
	public int uidMaster;

	public Chara Master => EClass._map.FindChara(uidMaster);

	public override bool ConsumeTurn
	{
		get
		{
			if (Master != owner)
			{
				return !owner.HasElement(433);
			}
			return false;
		}
	}

	public override int GetPhase()
	{
		return 0;
	}

	public override bool ShouldOverride(Condition c)
	{
		return true;
	}

	public override void Tick()
	{
		if (Master == owner)
		{
			Mod(-1);
		}
		else if (Master == null || !Master.HasCondition<ConTimeStop>())
		{
			Kill();
		}
	}
}
