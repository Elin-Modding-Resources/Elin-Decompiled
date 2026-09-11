public class CustomDecoContent : CustomSourceContent
{
	public override string SourceType => "SourceDeco";

	public static CustomDecoContent CreateFromRow(SourceDeco.Row r, ModPackage owner = null)
	{
		if (owner == null)
		{
			owner = ModUtil.FindSourceRowPackage(r);
		}
		return new CustomDecoContent
		{
			ContentId = "Deco/" + r.alias,
			SourceId = r.alias,
			Owner = owner
		};
	}
}
