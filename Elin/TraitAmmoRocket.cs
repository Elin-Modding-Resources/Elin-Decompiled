public class TraitAmmoRocket : TraitAmmo
{
	public override int DefaultStock => 10 + EClass.rnd(50);

	public override int CraftNum => 20;
}
