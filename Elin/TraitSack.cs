public class TraitSack : Trait
{
	public override bool CanBeSmashedToDeath => true;

	public override bool CanBeAttacked => !EClass._zone.IsPCFactionOrTent;
}
