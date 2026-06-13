using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

public class SourceImporter : EClass
{
	private readonly IReadOnlyDictionary<string, SourceData> _sourceMapping;

	public Dictionary<string, EMod> fileProviders = new Dictionary<string, EMod>(PathComparer.Default);

	public SourceImporter(IReadOnlyDictionary<string, SourceData> sourceMapping)
	{
		_sourceMapping = sourceMapping;
	}

	public SourceData FindSourceByName(string name)
	{
		string[] array = new string[3]
		{
			"Source" + name,
			"Lang" + name,
			name
		};
		foreach (string key in array)
		{
			if (_sourceMapping.TryGetValue(key, out var value))
			{
				return value;
			}
		}
		return null;
	}

	private (SourceData, SourceData.BaseRow[]) LoadBySheetName(ISheet sheet, string file)
	{
		string sheetName = sheet.SheetName;
		try
		{
			SourceData sourceData = FindSourceByName(sheetName);
			if ((object)sourceData == null)
			{
				Debug.Log("#source skipping sheet " + sheetName);
				return (null, null);
			}
			IList list;
			if (!(sourceData is SourceThingV))
			{
				list = sourceData.GetField<IList>("rows");
			}
			else
			{
				IList rows = EClass.sources.things.rows;
				list = rows;
			}
			IList list2 = list;
			int count = list2.Count;
			Debug.Log("#source loading sheet " + sheetName);
			ExcelParser.path = file;
			ERROR.lastImported = 0;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
			if (!sourceData.ImportData(sheet, fileNameWithoutExtension, overwrite: true))
			{
				throw new SourceParseException("#source failed to import data " + sourceData.GetType().Name + ":" + fileNameWithoutExtension + "/" + sheetName);
			}
			if (ERROR.lastImported > 0)
			{
				SourceData.BaseRow[] item = list2.OfType<SourceData.BaseRow>().Skip(count).Take(ERROR.lastImported)
					.ToArray();
				return (sourceData, item);
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"#source failed to load sheet {sheetName}\n{arg}");
		}
		return (null, Array.Empty<SourceData.BaseRow>());
	}

	public IEnumerable<SourceData> ImportFilesCached(IEnumerable<string> imports, bool resetData = true)
	{
		string[] prefetchSheetNames = new string[2] { "Element", "Material" };
		SourceCache[] array = imports.Select(SourceCache.GetOrCreate).Distinct().ToArray();
		Dictionary<SourceCache, (string, ISheet[], ISheet[])> dictionary = (from c in array
			where c.IsDirtyOrEmpty
			select PrefetchWorkbook(c.SheetFile.FullName, prefetchSheetNames)).ToArray().ToDictionary(((string file, ISheet[] sheets, ISheet[] fetched) p) => SourceCache.GetOrCreate(p.file));
		SourceElement elements = EClass.sources.elements;
		HashSet<SourceData> hashSet = new HashSet<SourceData> { elements };
		SourceCache[] array2 = array;
		foreach (SourceCache sourceCache in array2)
		{
			string arg = sourceCache.SheetFile.ShortPath();
			if (fileProviders.TryGetValue(sourceCache.SheetFile.FullName, out var value))
			{
				sourceCache.SetMod(value);
			}
			if (sourceCache.IsDirtyOrEmpty)
			{
				if (!dictionary.TryGetValue(sourceCache, out var value2) || value2.Item3.Length == 0)
				{
					continue;
				}
				ISheet[] item = value2.Item3;
				foreach (ISheet sheet in item)
				{
					ExcelParser.allowTrimming = true;
					try
					{
						SourceData.BaseRow[] item2 = LoadBySheetName(sheet, value2.Item1).Item2;
						if (!item2.IsEmpty())
						{
							sourceCache.EmplaceCache(sheet.SheetName, item2);
							value?.sourceRows.UnionWith(item2);
							Debug.Log($"#source workbook {arg}:{sheet.SheetName}:{item2.Length}");
						}
					}
					finally
					{
						ExcelParser.allowTrimming = false;
					}
				}
				continue;
			}
			string[] array3 = prefetchSheetNames;
			foreach (string text in array3)
			{
				if (sourceCache.TryGetCache(text, out var rows))
				{
					int num = elements.ImportRows(rows);
					value?.sourceRows.UnionWith(rows);
					Debug.Log($"#source workbook-cache {arg}:{text}:{num}");
				}
			}
		}
		array2 = array;
		foreach (SourceCache sourceCache2 in array2)
		{
			string text2 = sourceCache2.SheetFile.ShortPath();
			string key;
			if (sourceCache2.IsDirtyOrEmpty)
			{
				if (!dictionary.TryGetValue(sourceCache2, out var value3) || value3.Item2.Length == 0)
				{
					continue;
				}
				Debug.Log("#source workbook " + text2);
				ISheet[] item = value3.Item2;
				foreach (ISheet sheet2 in item)
				{
					if (prefetchSheetNames.Contains(sheet2.SheetName))
					{
						continue;
					}
					key = sheet2.SheetName;
					int allowTrimming;
					switch (key)
					{
					default:
						allowTrimming = ((!(key == "Word")) ? 1 : 0);
						break;
					case "General":
					case "Game":
					case "Note":
					case "List":
						allowTrimming = 0;
						break;
					}
					ExcelParser.allowTrimming = (byte)allowTrimming != 0;
					try
					{
						var (sourceData, array4) = LoadBySheetName(sheet2, value3.Item1);
						if ((object)sourceData != null)
						{
							int? num2 = array4?.Length;
							if (num2.HasValue && num2.GetValueOrDefault() > 0)
							{
								sourceCache2.EmplaceCache(sheet2.SheetName, array4);
								sourceCache2.Mod?.sourceRows.UnionWith(array4);
								hashSet.Add(sourceData);
							}
						}
					}
					finally
					{
						ExcelParser.allowTrimming = false;
					}
				}
				continue;
			}
			foreach (KeyValuePair<string, SourceData.BaseRow[]> item3 in sourceCache2.Source)
			{
				item3.Deconstruct(out key, out var value4);
				string text3 = key;
				SourceData.BaseRow[] array5 = value4;
				SourceData sourceData2 = FindSourceByName(text3);
				if (!(sourceData2 is SourceThingV))
				{
					if (sourceData2 is SourceElement || (object)sourceData2 == null)
					{
						continue;
					}
				}
				else
				{
					sourceData2 = EClass.sources.things;
				}
				if (array5 == null)
				{
					Debug.Log("#source cached rows are empty " + text2 + ":" + text3);
					continue;
				}
				int num3 = sourceData2.ImportRows(array5);
				sourceCache2.Mod?.sourceRows.UnionWith(array5);
				Debug.Log($"#source workbook-cache {text2}:{text3}:{num3}");
				hashSet.Add(sourceData2);
			}
		}
		if (resetData)
		{
			HotInit(hashSet);
		}
		return hashSet;
	}

	public static void HotInit(IEnumerable<SourceData> sourceData)
	{
		Debug.Log("#source resetting data...");
		foreach (SourceData sourceDatum in sourceData)
		{
			try
			{
				sourceDatum.Reset();
				sourceDatum.Init();
			}
			catch (Exception arg)
			{
				Debug.LogError($"#source failed to reset dirty data {sourceDatum.GetType().Name}\n{arg}");
			}
		}
		Debug.Log("#source initialized data");
	}

	private (string file, ISheet[] sheets, ISheet[] fetched) PrefetchWorkbook(string file, string[] prefetchNames)
	{
		using FileStream @is = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		XSSFWorkbook xSSFWorkbook = new XSSFWorkbook((Stream)@is);
		List<ISheet> list = new List<ISheet>();
		List<ISheet> list2 = new List<ISheet>();
		for (int i = 0; i < xSSFWorkbook.NumberOfSheets; i++)
		{
			ISheet sheetAt = xSSFWorkbook.GetSheetAt(i);
			if (FindSourceByName(sheetAt.SheetName) != null && prefetchNames.Contains(sheetAt.SheetName))
			{
				list2.Add(sheetAt);
			}
			else
			{
				list.Add(sheetAt);
			}
		}
		Debug.Log("#source workbook-prefetch " + file.ShortPath());
		return (file: file, sheets: list.ToArray(), fetched: list2.ToArray());
	}
}
