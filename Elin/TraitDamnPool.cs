public class TraitDamnPool : Trait
{
	public override bool CanBeHeld => false;

	public override bool CanBeDestroyed => false;

	public override bool CanOnlyCarry => true;

	public override bool CanPutAway => false;
}
