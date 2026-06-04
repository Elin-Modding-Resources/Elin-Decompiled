using System;
using System.Collections.Generic;
using UnityEngine;

public class TalkDataList : ExcelDataList
{
	public List<string> IDs;

	public override int StartIndex => 2;

	public override void OnInitialize()
	{
		IDs = new List<string>();
		foreach (string key in list[0].Keys)
		{
			if (key != "id" && key != "path")
			{
				IDs.Add(key);
			}
		}
	}

	public string GetTalk(string id, string idTopic, bool useDefault = false, bool human = true)
	{
		bool flag = false;
		Dictionary<string, string> row = GetRow(idTopic);
		if (row == null)
		{
			return "";
		}
		string text = row.TryGetValue(id);
		if (text.IsEmpty())
		{
			if (!useDefault)
			{
				Debug.LogError("id not found:" + id);
				return "";
			}
			text = row.TryGetValue("default");
			flag = true;
		}
		text = text.Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None).RandomItem();
		if (!human)
		{
			if (flag && !text.IsEmpty() && !text.StartsWith("(") && !text.StartsWith("*"))
			{
				text = "(" + text + ")";
			}
			text = text.Replace("。)", ")");
		}
		return text;
	}

	public string GetRandomID(string tag)
	{
		return IDs.RandomItem();
	}
}
