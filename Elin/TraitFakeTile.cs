using System.Collections.Generic;

public class TraitFakeTile : Trait
{
	public override TileMode tileMode => TileMode.FakeBlock;

	public override RenderData GetRenderData()
	{
		return EClass.sources.blocks.map[owner.refVal].renderData;
	}

	public override SourcePref GetPref()
	{
		TileType tileType = EClass.sources.blocks.map[owner.refVal].tileType;
		if (!(tileType is TileTypePillar))
		{
			if (!(tileType is TileTypeFence))
			{
				if (!(tileType is TileTypeStairs))
				{
					if (!(tileType is TileTypeWall))
					{
						if (tileType is TileTypeSlope)
						{
							return EClass.core.refs.prefs.blockStairs;
						}
						return null;
					}
					return EClass.core.refs.prefs.blockWall;
				}
				return EClass.core.refs.prefs.blockStairs;
			}
			return EClass.core.refs.prefs.blockFence;
		}
		return EClass.core.refs.prefs.blockPillar;
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
