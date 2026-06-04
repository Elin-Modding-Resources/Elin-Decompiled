using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptOut)]
public class CustomElementContent : CustomSourceContent
{
	public bool addOnLoad;

	public override string SourceType => "SourceElement";

	public static CustomElementContent CreateFromRow(SourceElement.Row r, ModPackage owner = null)
	{
		if (owner == null)
		{
			owner = ModUtil.FindSourceRowPackage(r);
		}
		CustomElementContent customElementContent = new CustomElementContent
		{
			ContentId = "Element/" + r.alias,
			SourceId = r.alias,
			Owner = owner
		};
		string[] tag = r.tag;
		int i;
		for (i = 0; i < tag.Length; i++)
		{
			string item = CustomSourceContent.GetParams(tag[i]).action;
			if (item == "addEleOnLoad" || item == "addOnLoad")
			{
				customElementContent.addOnLoad = true;
			}
		}
		i = r.id;
		if (i > 10000 || i < 0)
		{
			switch (r.group)
			{
			case "FEAT":
			case "ABILITY":
			case "SPELL":
			{
				i = ((!(r.group == "FEAT")) ? 48 : 32);
				int num = i;
				ModUtil.AppendSpriteSheet(r.alias, num, num);
				break;
			}
			}
		}
		return customElementContent;
	}

	public override void OnGameLoad(GameIOContext context)
	{
		SourceElement.Row row = EClass.sources.elements.alias[base.SourceId];
		if (addOnLoad && !EClass.pc.HasElement(base.SourceId))
		{
			switch (row.group)
			{
			case "FEAT":
				EClass.pc.SetFeat(row.id, 1, msg: true);
				break;
			case "ABILITY":
			case "SPELL":
				EClass.pc.GainAbility(row.id);
				break;
			}
		}
	}

	public override string ToString()
	{
		return $"{base.ContentId}/addOnLoad={addOnLoad}";
	}
}
