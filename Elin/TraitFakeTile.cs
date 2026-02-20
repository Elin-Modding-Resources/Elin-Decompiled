using System.Collections.Generic;

public class TraitFakeTile : Trait
{
	public override TileMode tileMode => TileMode.FakeBlock;

	public override RenderData GetRenderData()
	{
		return EClass.sources.blocks.map[owner.refVal].renderData;
	}

	public override void OnCrafted(Recipe recipe, List<Thing> ings)
	{
		owner.refVal = 0;
		if (this is TraitFakeBlock && ings != null && ings.Count > 0)
		{
			TraitBlock traitBlock = ings[0].trait as TraitBlock;
			owner.refVal = traitBlock.source.id;
		}
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		if (this is TraitFakeBlock)
		{
			if (p.pos.cell._block == 0)
			{
				return;
			}
		}
		else if (p.pos.cell.obj == 0)
		{
			return;
		}
		TileRow source = ((this is TraitFakeBlock) ? ((TileRow)p.pos.sourceBlock) : ((TileRow)p.pos.sourceObj));
		SourceMaterial.Row mat = ((this is TraitFakeBlock) ? p.pos.matBlock : p.pos.cell.matObj);
		if (!source.ContainsTag("noFake"))
		{
			p.TrySetAct("actCopyBlock", delegate
			{
				owner.Dye(mat);
				owner.refVal = source.id;
				SE.Play("offering");
				owner._CreateRenderer();
				HotItemHeld.recipe = GetRecipe();
				LayerInventory.SetDirty(owner.Thing);
				return false;
			});
		}
	}
}
