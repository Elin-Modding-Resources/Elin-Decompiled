public class TraitToolWaterPot : TraitTool
{
	public int MaxCharge => 20 + owner.material.hardness * 2;

	public override bool HasCharges => true;

	public override void OnCreate(int lv)
	{
		owner.Dye("void");
	}

	public override void SetName(ref string s)
	{
		if (owner.c_dyeMat != 0)
		{
			s = "_of".lang(owner.DyeMat.GetName(), s);
		}
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		p.TrySetAct(new ActClearWater
		{
			waterPot = this
		}, owner);
	}
}
