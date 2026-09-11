public class CustomCellEffectContent : CustomSourceContent
{
	public override string SourceType => "SourceCellEffect";

	public static CustomCellEffectContent CreateFromRow(SourceCellEffect.Row r, ModPackage owner = null)
	{
		if (owner == null)
		{
			owner = ModUtil.FindSourceRowPackage(r);
		}
		return new CustomCellEffectContent
		{
			ContentId = "CellEffect/" + r.alias,
			SourceId = r.alias,
			Owner = owner
		};
	}
}
