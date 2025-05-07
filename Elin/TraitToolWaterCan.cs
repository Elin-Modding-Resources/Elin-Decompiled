using UnityEngine;

public class TraitToolWaterCan : TraitTool
{
	public int MaxCharge => Mathf.Max(1, owner.material.hardness / 4 + Mathf.Clamp(owner.QualityLv * 2, 0, 40) + 3);

	public override bool HasCharges => true;

	public override void TrySetHeldAct(ActPlan p)
	{
		if (!p.TrySetAct(new TaskWater
		{
			dest = p.pos
		}, owner) && !p.TrySetAct(new ActDrawWater
		{
			waterCan = this
		}, owner))
		{
			p.TrySetAct(new ActWater
			{
				waterCan = this
			}, owner);
		}
	}
}
