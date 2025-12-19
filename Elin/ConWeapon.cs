using Newtonsoft.Json;

public class ConWeapon : BaseBuff
{
	[JsonProperty]
	public int cha;

	public override bool IsElemental => true;

	public override int P2 => cha;

	public override bool IsOverrideConditionMet(Condition c, int turn)
	{
		return true;
	}

	public override void Tick()
	{
	}

	public override bool CanStack(Condition c)
	{
		return true;
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		if (!onDeserialize)
		{
			cha = _owner.CHA;
		}
		base.SetOwner(_owner, onDeserialize);
	}
}
