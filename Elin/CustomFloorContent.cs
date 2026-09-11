public class CustomFloorContent : CustomSourceContent
{
	public override string SourceType => "SourceFloor";

	public static CustomFloorContent CreateFromRow(SourceFloor.Row r, ModPackage owner = null)
	{
		if (owner == null)
		{
			owner = ModUtil.FindSourceRowPackage(r);
		}
		return new CustomFloorContent
		{
			ContentId = "Floor/" + r.alias,
			SourceId = r.alias,
			Owner = owner
		};
	}
}
