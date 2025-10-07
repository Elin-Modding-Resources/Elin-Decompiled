public class TraitToolRangeGunRocket : TraitToolRangeGun
{
	public override bool IsAmmo(Thing t)
	{
		return t.trait is TraitAmmoRocket;
	}
}
