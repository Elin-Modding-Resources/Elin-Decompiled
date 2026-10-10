public class TraitBlackNote : TraitBaseSpellbook
{
	public override Type BookType => Type.BlackNote;

	public override bool CanStack => true;

	public override bool HasCharges => false;

	public override int eleParent => 74;

	public override int Difficulty => 10000;

	public override int GetActDuration(Chara c)
	{
		if (!EClass.debug.enable)
		{
			return 100;
		}
		return 1;
	}

	public override bool CanStackTo(Thing to)
	{
		if (to.isOn != owner.isOn)
		{
			return false;
		}
		if (to.c_idRefName != owner.c_idRefName)
		{
			return false;
		}
		return base.CanStackTo(to);
	}
}
