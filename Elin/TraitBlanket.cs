using System.Collections.Generic;
using UnityEngine;

public class TraitBlanket : Trait
{
	public override bool CanStack => false;

	public override bool HasCharges => true;

	public override void OnCreate(int lv)
	{
		owner.c_charges = EClass.rndHalf(8 + Mathf.Clamp(owner.QualityLv * 2, -2, 30));
	}

	public override void OnCrafted(Recipe recipe, List<Thing> ings)
	{
		OnCreate(owner.LV);
	}
}
