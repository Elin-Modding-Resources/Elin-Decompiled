using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SourceAsset : EScriptable
{
	public class PrefData
	{
		public Dictionary<string, SourcePref> dict = new Dictionary<string, SourcePref>();
	}

	public class Prefs
	{
		public int version;

		public PrefData things = new PrefData();

		public PrefData charas = new PrefData();
	}

	public string idLoad = "prefs";

	public UD_String_String renames;

	public static string PrefPath => Application.dataPath + "/Resources/Data/Export/";

	public void DoFix()
	{
	}

	public void SavePrefs(string id = "prefs")
	{
		_SavePrefs(id);
	}

	public static void _SavePrefs(string id = "prefs")
	{
		if (File.Exists(PrefPath + id))
		{
			IO.CopyAs(PrefPath + id, PrefPath + id + "_bk");
		}
		Prefs prefs = new Prefs();
		prefs.version = 2;
		Debug.Log(EClass.sources.things.rows.Count);
		Debug.Log(EClass.sources.charas.rows.Count);
		foreach (SourceThing.Row row in EClass.sources.things.rows)
		{
			if (prefs.things.dict.ContainsKey(row.id))
			{
				Debug.LogError("exception: duplicate id:" + row.id + "/" + row.name);
			}
			else
			{
				prefs.things.dict.Add(row.id, row.pref);
			}
		}
		foreach (SourceChara.Row row2 in EClass.sources.charas.rows)
		{
			if (prefs.charas.dict.ContainsKey(row2.id))
			{
				Debug.LogError("exception: duplicate id:" + row2.id + "/" + row2.name);
			}
			else
			{
				prefs.charas.dict.Add(row2.id, row2.pref);
			}
		}
		IO.SaveFile(PrefPath + id, prefs);
		Debug.Log("Exported Prefs:" + id);
	}

	public void LoadPrefs()
	{
		_LoadPrefs(idLoad);
	}

	public void LoadPrefs_bk()
	{
		_LoadPrefs(idLoad);
	}

	public static void _LoadPrefs(string id = "prefs")
	{
		IO.CopyAs(PrefPath + id, PrefPath + id + "_loadbk");
		Prefs prefs = IO.LoadFile<Prefs>(PrefPath + id);
		Debug.Log(prefs);
		foreach (SourceThing.Row row in EClass.sources.things.rows)
		{
			if (prefs.things.dict.ContainsKey(row.id))
			{
				row.pref = prefs.things.dict[row.id];
			}
			if (prefs.version == 0)
			{
				row.pref.y = 0f;
			}
		}
		foreach (SourceChara.Row row2 in EClass.sources.charas.rows)
		{
			if (prefs.charas.dict.ContainsKey(row2.id))
			{
				row2.pref = prefs.charas.dict[row2.id];
			}
		}
		Debug.Log("Imported Prefs:" + id);
	}
}
