public class BackerContent : EClass
{
	public static int indexTree;

	public static int indexRemain;

	public static int indexLantern;

	public static int indexSnail;

	public static int indexFollower;

	public static int indexSister;

	public static string ConvertName(string s)
	{
		return s.Replace("『", "「");
	}

	public static void GakiConvert(ref string text, string idLang = "zako")
	{
		if (text.IsEmpty())
		{
			return;
		}
		if (!text.StartsWith("("))
		{
			text = idLang.lang().Split(',').RandomItem() + " (" + text + ")";
		}
		else
		{
			switch (idLang)
			{
			case "mokyu":
			case "babu":
			case "mimu":
				text = idLang.lang().Split(',').RandomItem() + " " + text;
				break;
			}
		}
		text = text.Replace("。)", ")");
		if (text.EndsWith("」"))
		{
			text = text.Substring(0, text.Length - 1);
		}
	}
}
