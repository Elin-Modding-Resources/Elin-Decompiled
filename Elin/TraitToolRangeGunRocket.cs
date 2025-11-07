public class TraitToolRangeGunRocket : TraitToolRangeGun
{
	public override HitEffect GroundHitEffect => HitEffect.Rocket;

	public override int ChanceMissAim => 3;

	public override bool IsAmmo(Thing t)
	{
		return t.trait is TraitAmmoRocket;
	}
}
