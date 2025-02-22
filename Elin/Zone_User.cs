using System;
using System.IO;
using Newtonsoft.Json;

public class Zone_User : Zone
{
	[JsonProperty]
	public string path;

	public override bool IsUserZone => true;

	public override string idExport => Path.GetFileNameWithoutExtension(path);

	public override string pathExport => path;

	public override bool HasLaw => true;

	public override bool MakeTownProperties => true;

	public override int BaseElectricity => 1000;

	public override bool RevealRoom => true;

	public override void OnActivate()
	{
		base.OnActivate();
		if (EClass._map.exportSetting == null || EClass._map.exportSetting.textWelcome.IsEmpty())
		{
			return;
		}
		WidgetMainText.Instance.NewLine();
		string[] array = EClass._map.exportSetting.textWelcome.Replace("\\n", "").Split(Environment.NewLine.ToCharArray());
		int num = 0;
		string[] array2 = array;
		foreach (string text in array2)
		{
			Msg.SetColor("save");
			Msg.SayRaw("<i>" + text.Replace(Environment.NewLine, "") + "</i>");
			WidgetMainText.Instance.NewLine();
			num++;
			if (num >= 5)
			{
				break;
			}
		}
	}
}
