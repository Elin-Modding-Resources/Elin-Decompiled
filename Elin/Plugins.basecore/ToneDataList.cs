using System;
using System.Collections.Generic;
using System.Text;

public class ToneDataList : ExcelDataList
{
	public override void OnInitialize()
	{
	}

	public StringBuilder ApplyTone(string id, ref string text, int gender)
	{
		if (!initialized)
		{
			Initialize();
		}
		Dictionary<string, string> dictionary = all[id];
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		string text2 = "";
		for (int i = 0; i < text.Length; i++)
		{
			if (flag)
			{
				if (text[i] == '}')
				{
					flag = false;
					if (dictionary.ContainsKey(text2))
					{
						string text3 = dictionary[text2].Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None).TryGet((gender != 2) ? 1 : 0, 0);
						stringBuilder.Append(text3.Split(',').RandomItem());
					}
					else
					{
						stringBuilder.Append(text2);
					}
				}
				else
				{
					text2 += text[i];
				}
			}
			else if (text[i] == '{')
			{
				text2 = "";
				flag = true;
			}
			else
			{
				stringBuilder.Append(text[i]);
			}
		}
		stringBuilder.Replace("…～", "～…");
		stringBuilder.Replace("…ー", "ー…");
		return stringBuilder;
	}

	public string GetToneID(string id, int gender)
	{
		if (!initialized)
		{
			Initialize();
		}
		if (!Lang.isJP)
		{
			return id + "|I|YOU";
		}
		Dictionary<string, string> dictionary = all[id];
		string text = dictionary["I"].Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None).TryGet((gender != 2) ? 1 : 0, 0);
		string text2 = dictionary["YOU"].Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None).TryGet((gender != 2) ? 1 : 0, 0);
		string[] array = text.Split(',');
		string[] array2 = text2.Split(',');
		return id + "|" + array[rnd(rnd(array.Length) + 1)] + "|" + array2[rnd(rnd(array2.Length) + 1)];
		static int rnd(int a)
		{
			return Rand.rnd(a);
		}
	}
}
