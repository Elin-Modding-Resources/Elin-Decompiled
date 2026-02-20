public class TraitFakeBlock : TraitFakeTile
{
	public SourceBlock.Row block => EClass.sources.blocks.map.TryGetValue(owner.refVal, 1);

	public override SourcePref GetPref()
	{
		TileType tileType = block.tileType;
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

	public override string GetName()
	{
		if (owner.refVal != 0)
		{
			return "_fakeblock".lang(block.GetName().ToLower());
		}
		return base.GetName();
	}
}
