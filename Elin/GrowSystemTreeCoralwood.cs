using System.Linq;

public class GrowSystemTreeCoralwood : GrowSystemTreeSingle
{
	public override int GetShadow(int index)
	{
		return 34;
	}

	public override bool BlockPass(Cell cell)
	{
		return true;
	}

	public override bool BlockSight(Cell cell)
	{
		return true;
	}

	public override void OnSetObj()
	{
		GrowSystem.cell.isObjDyed = true;
		GrowSystem.cell.objMat = (byte)EClass.sources.materials.rows.Where((SourceMaterial.Row r) => r.tag.Contains("coral")).RandomItem().id;
	}
}
