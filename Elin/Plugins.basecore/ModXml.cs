using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;

public static class ModXml
{
	private static readonly string[] _validXml = new string[14]
	{
		"title", "author", "id", "description", "version", "builtin", "tag", "tags", "loadPriority", "visibility",
		"dependency", "incompatible", "loadAfter", "loadBefore"
	};

	public static string ValidXml(string name)
	{
		return _validXml.FirstOrDefault((string known) => string.Equals(name, known, StringComparison.OrdinalIgnoreCase));
	}

	public static string Text(XElement e)
	{
		return e.Value.Trim();
	}

	public static string[] SplitTags(string value)
	{
		if (value.IsEmpty())
		{
			return null;
		}
		List<string> list = new List<string>();
		string[] array = value.Split(',');
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i].Trim();
			if (!text.IsEmpty() && !list.Contains(text))
			{
				list.Add(text);
			}
		}
		if (list.Count != 0)
		{
			return list.ToArray();
		}
		return null;
	}

	public static void ReadIdRow(XElement e, ref List<string[]> rows, string path)
	{
		try
		{
			string text = Attribute(e, "id");
			if (text.IsEmpty())
			{
				Debug.LogWarning("#mod package.xml <" + e.Name.LocalName + "> missing id, ignored: " + path);
				return;
			}
			List<string> list = null;
			string[] array = text.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = BaseModPackage.NormalizeId(array[i]);
				if (!text2.IsEmpty())
				{
					if (list == null)
					{
						list = new List<string>();
					}
					if (!list.Contains(text2))
					{
						list.Add(text2);
					}
				}
			}
			if (list != null)
			{
				if (rows == null)
				{
					rows = new List<string[]>();
				}
				rows.Add(list.ToArray());
			}
		}
		catch (Exception arg)
		{
			Debug.LogWarning($"#mod package.xml <{e.Name.LocalName}> is ill-formatted, ignored: {path}\n{arg}");
		}
	}

	private static string Attribute(XElement e, string name)
	{
		return e.Attributes().FirstOrDefault((XAttribute a) => string.Equals(a.Name.LocalName, name, StringComparison.OrdinalIgnoreCase))?.Value;
	}

	public static XDocument Load(byte[] bytes, out string error)
	{
		error = null;
		XmlReaderSettings settings = new XmlReaderSettings
		{
			IgnoreComments = true,
			IgnoreWhitespace = true,
			IgnoreProcessingInstructions = true,
			DtdProcessing = DtdProcessing.Prohibit,
			XmlResolver = null
		};
		try
		{
			using MemoryStream input = new MemoryStream(bytes, writable: false);
			using XmlReader reader = XmlReader.Create(input, settings);
			return XDocument.Load(reader);
		}
		catch (Exception ex)
		{
			error = ex.Message;
			return null;
		}
	}
}
