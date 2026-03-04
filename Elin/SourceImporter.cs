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
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
			if (!sourceData.ImportData(sheet, fileNameWithoutExtension, overwrite: true))
			{
				throw new SourceParseException("#source failed to import data " + sourceData.GetType().Name + ":" + fileNameWithoutExtension + "/" + sheetName);
			}
			SourceData.BaseRow[] item = Array.Empty<SourceData.BaseRow>();
			int num = ERROR.msg.Split('/')[^1].ToInt();
			if (num > 0)
			{
				item = list2.OfType<SourceData.BaseRow>().Skip(count).Take(num)
					.ToArray();
			}
			return (sourceData, item);
		}
		catch (Exception arg)
		{
			Debug.LogError($"#source failed to load sheet {sheetName}\n{arg}");
		}
		return (null, null);
	}

	public IEnumerable<SourceData> ImportFilesCached(IEnumerable<string> imports, bool resetData = true)
	{
		SourceCache[] array = imports.Select(SourceCache.GetOrCreate).Distinct().ToArray();
		Dictionary<SourceCache, (string, ISheet[], ISheet)> dictionary = (from c in array
			where c.IsDirtyOrEmpty
			select PrefetchWorkbook(c.SheetFile.FullName)).ToArray().ToDictionary(((string file, ISheet[] sheets, ISheet element) p) => SourceCache.GetOrCreate(p.file));
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
			SourceData.BaseRow[] rows;
			if (sourceCache.IsDirtyOrEmpty)
			{
				if (dictionary.TryGetValue(sourceCache, out var value2) && value2.Item3 != null)
				{
					SourceData.BaseRow[] item = LoadBySheetName(value2.Item3, value2.Item1).Item2;
					sourceCache.EmplaceCache("Element", item);
					value?.sourceRows.UnionWith(item);
					Debug.Log(string.Format("#source workbook {0}:{1}:{2}", arg, "Element", item.Length));
				}
			}
			else if (sourceCache.TryGetCache("Element", out rows))
			{
				int num = elements.ImportRows(rows);
				value?.sourceRows.UnionWith(rows);
				Debug.Log(string.Format("#source workbook-cache {0}:{1}:{2}", arg, "Element", num));
			}
		}
		array2 = array;
		foreach (SourceCache sourceCache2 in array2)
		{
			string text = sourceCache2.SheetFile.ShortPath();
			if (sourceCache2.IsDirtyOrEmpty)
			{
				if (!dictionary.TryGetValue(sourceCache2, out var value3) || value3.Item2.Length == 0)
				{
					continue;
				}
				Debug.Log("#source workbook " + text);
				ISheet[] item2 = value3.Item2;
				foreach (ISheet sheet in item2)
				{
					if (sheet.SheetName == "Element")
					{
						continue;
					}
					var (sourceData, array3) = LoadBySheetName(sheet, value3.Item1);
					if ((object)sourceData != null)
					{
						int? num2 = array3?.Length;
						if (num2.HasValue && num2.GetValueOrDefault() > 0)
						{
							sourceCache2.EmplaceCache(sheet.SheetName, array3);
							sourceCache2.Mod?.sourceRows.UnionWith(array3);
							hashSet.Add(sourceData);
						}
					}
				}
				continue;
			}
			foreach (KeyValuePair<string, SourceData.BaseRow[]> item3 in sourceCache2.Source)
			{
				item3.Deconstruct(out var key, out var value4);
				string text2 = key;
				SourceData.BaseRow[] array4 = value4;
				SourceData sourceData2 = FindSourceByName(text2);
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
				if (array4 == null)
				{
					Debug.Log("#source cached rows are empty " + text + ":" + text2);
					continue;
				}
				int num3 = sourceData2.ImportRows(array4);
				sourceCache2.Mod?.sourceRows.UnionWith(array4);
				Debug.Log($"#source workbook-cache {text}:{text2}:{num3}");
				hashSet.Add(sourceData2);
			}
		}
		SourceCache.FinalizeCache();
		SourceCache.ClearDetail();
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

	private (string file, ISheet[] sheets, ISheet element) PrefetchWorkbook(string file)
	{
		using FileStream @is = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		XSSFWorkbook xSSFWorkbook = new XSSFWorkbook((Stream)@is);
		List<ISheet> list = new List<ISheet>();
		ISheet item = null;
		for (int i = 0; i < xSSFWorkbook.NumberOfSheets; i++)
		{
			ISheet sheetAt = xSSFWorkbook.GetSheetAt(i);
			SourceData sourceData = FindSourceByName(sheetAt.SheetName);
			if (!(sourceData is SourceElement))
			{
				if ((object)sourceData != null)
				{
					list.Add(sheetAt);
				}
			}
			else
			{
				item = sheetAt;
			}
		}
		Debug.Log("#source workbook-prefetch " + file.ShortPath());
		return (file: file, sheets: list.ToArray(), element: item);
	}
}
