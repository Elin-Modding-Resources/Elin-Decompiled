using System;
using Newtonsoft.Json;

public abstract class CustomSourceContent : CustomContent
{
	[JsonIgnore]
	public virtual string SourceType { get; }

	[JsonIgnore]
	public string SourceId { get; set; }

	public static (string action, string spec, string[] kv) GetParams(string tag)
	{
		if (tag.IsEmpty())
		{
			return (action: string.Empty, spec: string.Empty, kv: Array.Empty<string>());
		}
		int num = tag.IndexOf('(');
		string text;
		string text2;
		if (num >= 0)
		{
			text = tag[..num];
			text2 = tag.ExtractInBetween('(', ')');
		}
		else
		{
			text = tag.Split('_')[0];
			text2 = tag[text.Length..].TrimStart('_');
		}
		string[] item = text2.Split('=');
		return (action: text, spec: text2, kv: item);
	}
}
