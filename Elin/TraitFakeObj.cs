public class TraitFakeObj : TraitFakeTile
{
	public Cell cell = new Cell();

	public GrowSystem growth
	{
		get
		{
			if (!obj.HasGrowth)
			{
				return null;
			}
			GrowSystem.cell = cell;
			cell.objVal = (byte)(owner.dir % obj.growth.StageLength * 30);
			cell.objDir = owner.dir;
			cell.obj = (byte)owner.refVal;
			return obj.growth;
		}
	}

	public SourceObj.Row obj => EClass.sources.objs.map[owner.refVal];

	public override TileMode tileMode => TileMode.FakeObj;

	public override RenderData GetRenderData()
	{
		return obj.renderData;
	}

	public override SourcePref GetPref()
	{
		return obj.pref;
	}

	public int GetMaxDir()
	{
		if (growth != null && !(growth is GrowSystemTreeCoralwood))
		{
			return growth.StageLength;
		}
		return obj._tiles.Length;
	}
}
