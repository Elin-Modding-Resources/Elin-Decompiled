using System;
using UnityEngine;

public class TraitItemProc : TraitItem
{
	public EffectId IdEffect => GetParam(1).ToEnum<EffectId>();

	public string n1 => GetParam(2);

	public override int CraftNum => GetCraftNum();

	public override bool CanChangeHeight => false;

	public int GetCraftNum()
	{
		if (owner.id == "bandage")
		{
			return 3;
		}
		return 0;
	}

	public override bool OnUse(Chara c)
	{
		int num = owner.Power;
		if (IdEffect == EffectId.Buff && n1 == "ConBandage")
		{
			num += owner.Evalue(750) * 5;
			num = num * (100 + (int)Mathf.Sqrt(c.Evalue(300)) * 5) / 100;
		}
		ActEffect.Proc(IdEffect, GetParamInt(3, num), owner.blessedState, c, null, new ActRef
		{
			n1 = n1,
			refThing = owner.Thing
		});
		if (c.ExistsOnMap)
		{
			FoodEffect.ProcTrait(c, owner);
		}
		if (IdEffect == EffectId.RemedyJure)
		{
			c.Say("destory_feather", owner);
		}
		owner.ModNum(-1);
		return true;
	}

	public override Action GetHealAction(Chara c)
	{
		if (IdEffect == EffectId.Buff && n1 == "ConBandage" && !c.HasCondition<ConBandage>())
		{
			return delegate
			{
				OnUse(c);
			};
		}
		return null;
	}
}
