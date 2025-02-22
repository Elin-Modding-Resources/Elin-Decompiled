public class ConWeapon : BaseBuff
{
	public override bool IsElemental => true;

	public override int P2 => owner.CHA;

	public override void Tick()
	{
	}

	public override bool CanStack(Condition c)
	{
		return true;
	}
}
