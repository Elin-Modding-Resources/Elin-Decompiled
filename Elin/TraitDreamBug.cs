public class TraitDreamBug : Trait
{
	public override bool CanStack => false;

	public override bool IsBlendBase => true;

	public override void OnCreate(int lv)
	{
		owner.c_charges = 3 + EClass.rnd(5);
	}

	public override bool CanBlend(Thing t)
	{
		return t.IsFood;
	}

	public override void OnBlend(Thing t, Chara c)
	{
		TraitDrink.BlendLove(EClass.pc, t, owner, dream: true);
		owner.ModNum(-1);
	}
}
