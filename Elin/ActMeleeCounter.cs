public class ActMeleeCounter : ActMelee
{
	public float bonus;

	public override bool AllowCounter => false;

	public override bool AllowParry => false;

	public override bool ShouldRollMax => true;

	public override float BaseDmgMTP => 1f + bonus;
}
