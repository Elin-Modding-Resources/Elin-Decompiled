public class TraitThrownConsume : TraitThrown
{
	public override bool ShowAsTool => true;

	public override int DefaultStock => 10 + EClass.rnd(50);

	public override int CraftNum => 20;
}
