public class ConBerserk : BadCondition
{
	public override bool CancelAI => true;

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		owner.isBerserk = true;
	}

	public override void OnRemoved()
	{
		owner.isBerserk = false;
		owner.ai.Cancel();
	}
}
