using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptOut)]
public class CustomStatContent : CustomSourceContent
{
	public bool addOnLoad;

	public override string SourceType => "SourceStat";

	public static CustomStatContent CreateFromRow(SourceStat.Row r, ModPackage owner = null)
	{
		if (owner == null)
		{
			owner = ModUtil.FindSourceRowPackage(r);
		}
		CustomStatContent result = new CustomStatContent
		{
			ContentId = "Stat/" + r.alias,
			SourceId = r.alias,
			Owner = owner
		};
		ModUtil.AppendSpriteSheet(r.alias, 32, 32);
		return result;
	}
}
