public class CustomObjContent : CustomSourceContent
{
	public override string SourceType => "SourceObj";

	public static CustomObjContent CreateFromRow(SourceObj.Row r, ModPackage owner = null)
	{
		if (owner == null)
		{
			owner = ModUtil.FindSourceRowPackage(r);
		}
		return new CustomObjContent
		{
			ContentId = "Obj/" + r.alias,
			SourceId = r.alias,
			Owner = owner
		};
	}
}
