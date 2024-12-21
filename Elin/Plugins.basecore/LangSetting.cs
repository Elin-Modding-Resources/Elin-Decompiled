using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IniParser;
using IniParser.Model;
using UnityEngine;

public class LangSetting
{
	public class FontSetting
	{
		public float lineSpace = 1f;

		public int index;

		public int importSize = 16;

		public string id;
	}

	public string id;

	public string name;

	public string name_en;

	public string dir;

	public bool addArticle;

	public bool pluralize;

	public bool capitalize;

	public bool useSpace;

	public bool hyphenation;

	public bool useTone;

	public bool thirdperson;

	public bool stripPuns;

	public int nameStyle;

	public int bracket;

	public int combatTextStyle;

	public List<FontSetting> listFont = new List<FontSetting>();

	public string pathVersion => dir + "/version.ini";

	public LangSetting(string path)
	{
		try
		{
			IniData iniData = new FileIniDataParser().ReadFile(path, Encoding.UTF8);
			name = iniData.GetKey("name");
			name_en = iniData.GetKey("name_en");
			addArticle = iniData.GetKey("add_article") == "1";
			pluralize = iniData.GetKey("pluralize") == "1";
			capitalize = iniData.GetKey("capitalize") == "1";
			useSpace = iniData.GetKey("use_space") == "1";
			useTone = iniData.GetKey("use_tone") == "1";
			nameStyle = iniData.GetKey("name_style").ToInt();
			combatTextStyle = iniData.GetKey("combat_text_style").ToInt();
			bracket = iniData.GetKey("bracket").ToInt();
			hyphenation = iniData.GetKey("hyphenation") == "1";
			thirdperson = iniData.GetKey("thirdperson") == "1";
			stripPuns = iniData.GetKey("strip_puns") == "1";
			foreach (KeyData item in iniData["LoadFont"])
			{
				string[] array = item.Value.Split(',');
				FontSetting fontSetting = new FontSetting
				{
					index = array[0].ToInt(),
					id = array[1]
				};
				if (array.Length >= 3)
				{
					fontSetting.importSize = array[2].ToInt();
				}
				if (array.Length >= 4)
				{
					fontSetting.lineSpace = array[3].ToFloat();
				}
				listFont.Add(fontSetting);
			}
		}
		catch (Exception message)
		{
			Debug.Log(message);
			Debug.Log("exception: Failed to parse:" + path);
		}
	}

	public int GetVersion()
	{
		try
		{
			return new FileIniDataParser().ReadFile(pathVersion, Encoding.UTF8).Global["version"].ToInt();
		}
		catch (Exception message)
		{
			Debug.Log(message);
			Debug.Log("exception: Failed to parse:" + pathVersion);
			return 0;
		}
	}

	public void SetVersion()
	{
		FileIniDataParser fileIniDataParser = new FileIniDataParser();
		if (!File.Exists(pathVersion))
		{
			StreamWriter streamWriter = File.CreateText(pathVersion);
			streamWriter.WriteLine("version=" + BaseCore.Instance.version.GetInt());
			streamWriter.Close();
		}
		else
		{
			IniData iniData = fileIniDataParser.ReadFile(pathVersion, Encoding.UTF8);
			iniData.Global["version"] = BaseCore.Instance.version.GetInt().ToString() ?? "";
			fileIniDataParser.WriteFile(pathVersion, iniData);
		}
	}
}
