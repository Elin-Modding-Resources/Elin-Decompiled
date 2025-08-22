using System.Collections.Generic;

public class TraitFakeBlock : Trait
{
	public override TileMode tileMode => TileMode.FakeBlock;

	public override void OnCreate(int lv)
	{
		if (owner.refVal == 0)
		{
			owner.refVal = 1;
		}
	}

	public override void OnCrafted(Recipe recipe, List<Thing> ings)
	{
		if (ings == null || ings.Count == 0)
		{
			owner.refVal = 1;
			return;
		}
		TraitBlock traitBlock = ings[0].trait as TraitBlock;
		owner.refVal = traitBlock.source.id;
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		if (p.pos.cell._block != 0 && !p.pos.sourceBlock.ContainsTag("noFake"))
		{
			p.TrySetAct("actCopyBlock", delegate
			{
				owner.Dye(p.pos.matBlock);
				owner.refVal = p.pos.sourceBlock.id;
				SE.Play("offering");
				owner._CreateRenderer();
				HotItemHeld.recipe = GetRecipe();
				return false;
			});
		}
	}
}
