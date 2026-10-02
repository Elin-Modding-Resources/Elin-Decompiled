public class TraitBlackNote : TraitBaseSpellbook
{
	public override Type BookType => Type.BlackNote;

	public override bool CanStack => owner.isOn;

	public override bool HasCharges => false;

	public override int eleParent => 74;

	public override int Difficulty => 10000;

	public override int GetActDuration(Chara c)
	{
		return 100;
	}
}
