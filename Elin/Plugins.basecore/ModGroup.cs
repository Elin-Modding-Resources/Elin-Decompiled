using System;
using UnityEngine;

public class ModGroup
{
	public string name;

	public bool collapsed;

	public Color? color;

	public ModGroup Clone()
	{
		return (ModGroup)MemberwiseClone();
	}

	public override string ToString()
	{
		string text = $"#group,{name},{(collapsed ? 1 : 0)}";
		Color? color = this.color;
		object obj;
		if (color.HasValue)
		{
			Color valueOrDefault = color.GetValueOrDefault();
			obj = "," + ColorUtility.ToHtmlStringRGB(valueOrDefault);
		}
		else
		{
			obj = "";
		}
		return text + (string)obj;
	}

	public static bool TryParse(string line, out ModGroup group)
	{
		group = null;
		line = line?.Trim() ?? "";
		if (line != "#group" && !line.StartsWith("#group,", StringComparison.Ordinal))
		{
			return false;
		}
		int num = line.LastIndexOf(',');
		Color? color = null;
		if (line.Length - num == 7 && ColorUtility.TryParseHtmlString("#" + line[(num + 1)..], out var value))
		{
			color = value;
			line = line[..num];
			num = line.LastIndexOf(',');
		}
		if (num > 6)
		{
			group = new ModGroup
			{
				name = line[7..num].Trim(),
				collapsed = (line[(num + 1)..].Trim() == "1"),
				color = color
			};
		}
		return true;
	}
}
