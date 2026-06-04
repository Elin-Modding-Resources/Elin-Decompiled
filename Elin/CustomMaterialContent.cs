public class CustomMaterialContent : CustomSourceContent
{
	public override string SourceType => "SourceMaterial";

	public static CustomMaterialContent CreateFromRow(SourceMaterial.Row r, ModPackage owner = null)
	{
		if (owner == null)
		{
			owner = ModUtil.FindSourceRowPackage(r);
		}
		return new CustomMaterialContent
		{
			ContentId = "Material/" + r.alias,
			SourceId = r.alias,
			Owner = owner
		};
	}
}
