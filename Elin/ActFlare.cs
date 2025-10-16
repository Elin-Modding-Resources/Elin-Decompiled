public class ActFlare : Spell
{
	public override bool CanAutofire => true;

	public override bool CanPressRepeat => true;

	public override bool CanRapidFire => true;

	public override float RapidDelay => 0.3f;
}
