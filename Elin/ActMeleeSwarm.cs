public class ActMeleeSwarm : ActMelee
{
	public override bool UseWeaponDist => false;

	public override int PerformDistance => 99;

	public override float BaseDmgMTP => 0.9f + 0.1f * (float)Act.CC.Evalue(1750);
}
