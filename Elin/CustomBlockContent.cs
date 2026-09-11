public class CustomBlockContent : CustomSourceContent
{
	public override string SourceType => "SourceBlock";

	public static CustomBlockContent CreateFromRow(SourceBlock.Row r, ModPackage owner = null)
	{
		if (owner == null)
		{
			owner = ModUtil.FindSourceRowPackage(r);
		}
		return new CustomBlockContent
		{
			ContentId = "Block/" + r.alias,
			SourceId = r.alias,
			Owner = owner
		};
	}
}
