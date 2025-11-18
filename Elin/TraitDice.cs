public class TraitDice : Trait
{
	public override bool IsThrowMainAction => true;

	public override ThrowType ThrowType => ThrowType.Dice;
}
