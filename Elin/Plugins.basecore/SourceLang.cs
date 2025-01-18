using System.Text;

public class SourceLang<T> : SourceData<T, string> where T : LangRow
{
	public override bool AllowHotInitialization => true;

	public override T GetRow(string id)
	{
		return map.TryGetValue(id);
	}

	public string Get(string id)
	{
		T val = map.TryGetValue(id);
		if (val == null)
		{
			return id;
		}
		if (!Lang.isBuiltin)
		{
			if (val.text_L.IsEmpty() && !val.text.IsEmpty())
			{
				return val.text;
			}
			return val.text_L;
		}
		if (!Lang.isJP)
		{
			return val.text;
		}
		return val.text_JP;
	}

	public string TryGetId(string id, string id2)
	{
		if (map.TryGetValue(id) == null)
		{
			return id2;
		}
		return id;
	}

	public string Parse(string idLang, string val1, string val2 = null, string val3 = null, string val4 = null)
	{
		StringBuilder stringBuilder = new StringBuilder(Get(idLang));
		stringBuilder.Replace("#1", val1 ?? "");
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
		return stringBuilder.ToString();
	}
}
