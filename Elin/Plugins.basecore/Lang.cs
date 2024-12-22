using System;
using System.Collections.Generic;
using System.Text;

public class Lang
{
	public enum LangCode
	{
		None = 0,
		JP = 10,
		EN = 20,
		CN = 30
	}

	public class Words
	{
		public char comma;

		public char period;
	}

	public static NaturalStringComparer comparer = new NaturalStringComparer();

	public static bool runUpdate;

	public static Words words = new Words();

	public static string langCode = "";

	public static string suffix = "";

	public static string space = "";

	public static List<Dictionary<string, object>> listName;

	public static List<Dictionary<string, object>> listAlias;

	public static bool isJP;

	public static bool isEN;

	public static bool isBuiltin;

	public static List<char[]> articlesToRemove;

	public static LangGeneral General;

	public static LangGame Game;

	public static LangNote Note;

	public static SourceData List;

	public static LangSetting setting;

	public static ExcelData alias;

	public static ExcelData names;

	public static ExcelData excelDialog;

	public static bool IsBuiltin(string id)
	{
		if (!(id == "JP"))
		{
			return id == "EN";
		}
		return true;
	}

	public static void Init(string lang)
	{
		setting = MOD.langs.TryGetValue(lang) ?? MOD.langs["EN"];
		langCode = lang;
		isJP = lang == "JP";
		isEN = lang == "EN";
		isBuiltin = lang == "JP" || lang == "EN";
		suffix = ((!isBuiltin) ? "_L" : (isJP ? "_JP" : ""));
		space = (setting.useSpace ? " " : "");
		char.TryParse("_comma".lang(), out words.comma);
		char.TryParse("_period".lang(), out words.period);
		SourceData.LangSuffix = suffix;
		articlesToRemove = new List<char[]>();
		string[] list = GetList("_articlesToRemove");
		foreach (string text in list)
		{
			articlesToRemove.Add(text.ToCharArray());
		}
	}

	public static string Get(string id)
	{
		if (!(General != null))
		{
			return id;
		}
		return General.Get(id);
	}

	public static bool Has(string id)
	{
		return General.map.ContainsKey(id);
	}

	public static string TryGet(string id)
	{
		if (!General.map.ContainsKey(id))
		{
			return null;
		}
		return Get(id);
	}

	public static string[] GetList(string id)
	{
		return List.GetList(id);
	}

	public static string ParseRaw(string text, string val1, string val2 = null, string val3 = null, string val4 = null, string val5 = null)
	{
		StringBuilder stringBuilder = new StringBuilder(text);
		stringBuilder.Replace("#1", val1 ?? "");
		if (val2 != null)
		{
			stringBuilder.Replace("#(s2)", (val2.Replace(",", "").ToInt() <= 1) ? "" : "_s".lang());
		}
		stringBuilder.Replace("#(s)", (val1.Replace(",", "").ToInt() <= 1) ? "" : "_s".lang());
		if (val2 != null)
		{
			stringBuilder.Replace("#2", val2);
		}
		if (val3 != null)
		{
			stringBuilder.Replace("#3", val3);
		}
		if (val4 != null)
		{
			stringBuilder.Replace("#4", val4);
		}
		if (val5 != null)
		{
			stringBuilder.Replace("#5", val5);
		}
		return stringBuilder.ToString();
	}

	public static string Parse(string idLang, string val1, string val2 = null, string val3 = null, string val4 = null, string val5 = null)
	{
		return ParseRaw(Get(idLang), val1, val2, val3, val4, val5);
	}

	public static string LoadText(string id)
	{
		return null;
	}

	public static string _Number(int a)
	{
		return $"{a:#,0}";
	}

	public static string _currency(int a, string IDCurrency)
	{
		return ("u_currency_" + IDCurrency).lang($"{a:#,0}");
	}

	public static string _currency(int a, bool showUnit = false, int unitSize = 14)
	{
		return $"{a:#,0}" + ((!showUnit) ? "" : ((unitSize == 0) ? "u_money".lang(a.ToString() ?? "") : ("<size=" + unitSize + "> " + "u_money".lang(a.ToString() ?? "") + "</size>")));
	}

	public static string _weight(int a, int b, bool showUnit = true, int unitSize = 0)
	{
		return (0.001f * (float)a).ToString("#0.0") + "/" + (0.001f * (float)b).ToString("#0.0") + ((!showUnit) ? "" : ((unitSize == 0) ? "s" : ("<size=" + unitSize + "> s</size>")));
	}

	public static string _gender(int id)
	{
		return GetList("genders")[id];
	}

	public static string _weight(int a, bool showUnit = true, int unitSize = 0)
	{
		return (0.001f * (float)a).ToString("#0.0") + ((!showUnit) ? "" : ((unitSize == 0) ? "s" : ("<size=" + unitSize + "> s</size>")));
	}

	public static string _rarity(int a)
	{
		return a switch
		{
			4 => "SSR", 
			3 => "SR", 
			2 => "R", 
			1 => "C", 
			0 => "K", 
			_ => "LE", 
		};
	}

	public static string _ChangeNum(int prev, int now)
	{
		return prev + " ⇒ " + now;
	}

	public static string[] GetDialog(string idSheet, string idTopic)
	{
		if (excelDialog == null)
		{
			string path = CorePath.CorePackage.TextDialog + "dialog.xlsx";
			excelDialog = new ExcelData();
			excelDialog.path = path;
		}
		excelDialog.BuildMap(idSheet);
		ExcelData.Sheet sheet = excelDialog.sheets[idSheet];
		string key = "text" + (isBuiltin ? ("_" + langCode) : "");
		Dictionary<string, string> dictionary = sheet.map.TryGetValue(idTopic);
		if (dictionary == null || !dictionary.ContainsKey(key))
		{
			return new string[1] { idTopic };
		}
		string text = dictionary[key];
		if (text.IsEmpty())
		{
			text = dictionary["text_EN"];
		}
		return text.Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
	}
}
