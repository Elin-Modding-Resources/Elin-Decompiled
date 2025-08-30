public class ActMeleeParry : ActMelee
{
	public override bool ShouldRollMax => true;

	public override float BaseDmgMTP => 1.5f;
}
