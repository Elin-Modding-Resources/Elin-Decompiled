public class TraitVoidgate : Trait
{
	public override bool CanBeHeld => !owner.isOn;

	public override bool CanBeDestroyed => false;

	public override bool CanOnlyCarry => true;

	public override bool CanPutAway => false;

	public override bool IsAnimeOn => owner.isOn;
}
