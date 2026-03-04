using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

public class ModPackage : EMod
{
	public struct SheetIndex
	{
		public int dest;

		public int old;
	}

	public IReadOnlyList<FileInfo> ParseTalkText(DirectoryInfo dir)
	{
		List<FileInfo> list = new List<FileInfo>();
		FileInfo[] files = dir.GetFiles("*.xlsx", SearchOption.TopDirectoryOnly);
		foreach (FileInfo fileInfo in files)
		{
			ExcelData excelData = new ExcelData(fileInfo.FullName);
			TalkText.modList.Add(excelData);
			talkTexts.Add(excelData);
			list.Add(fileInfo);
		}
		if (list.Count > 0)
		{
			Debug.Log($"TalkText: {talkTexts.Count}");
		}
		return list;
	}

	public IReadOnlyList<FileInfo> ParseMap(DirectoryInfo dir, bool addToList = true)
	{
		List<FileInfo> list = new List<FileInfo>();
		FileInfo[] files = dir.GetFiles("*.z", SearchOption.TopDirectoryOnly);
		foreach (FileInfo fileInfo in files)
		{
			string key = Path.ChangeExtension(fileInfo.Name, null);
			if (addToList)
			{
				MOD.listMaps.Add(fileInfo);
			}
			maps[key] = fileInfo;
			list.Add(fileInfo);
		}
		if (list.Count > 0)
		{
			Debug.Log($"Map: {maps.Count}");
		}
		return list;
	}

	public IReadOnlyList<FileInfo> ParseMapPiece(DirectoryInfo dir, bool addToList = true)
	{
		List<FileInfo> list = new List<FileInfo>();
		FileInfo[] files = dir.GetFiles("*.mp", SearchOption.TopDirectoryOnly);
		foreach (FileInfo fileInfo in files)
		{
			string key = Path.ChangeExtension(fileInfo.Name, null);
			if (addToList)
			{
				MOD.listPartialMaps.Add(fileInfo);
			}
			partialMaps[key] = fileInfo;
			list.Add(fileInfo);
		}
		if (list.Count > 0)
		{
			Debug.Log($"PartialMap: {partialMaps.Count}");
		}
		return list;
	}

	public IReadOnlyList<FileInfo> ParseTextureReplace(DirectoryInfo dir)
	{
		List<FileInfo> list = new List<FileInfo>();
		FileInfo[] files = dir.GetFiles("*.png", SearchOption.TopDirectoryOnly);
		foreach (FileInfo fileInfo in files)
		{
			string key = Path.ChangeExtension(fileInfo.Name, null);
			textureReplaces[key] = fileInfo;
			list.Add(fileInfo);
		}
		if (list.Count > 0)
		{
			Debug.Log($"Texture Replace: {textureReplaces.Count}");
		}
		return list;
	}

	public IReadOnlyList<FileInfo> ParseTexture(DirectoryInfo dir)
	{
		List<FileInfo> list = new List<FileInfo>();
		FileInfo[] files = dir.GetFiles("*.png", SearchOption.AllDirectories);
		foreach (FileInfo fileInfo in files)
		{
			string key = Path.ChangeExtension(Path.GetRelativePath(dir.FullName, fileInfo.FullName).NormalizePath(), null);
			string text = Path.ChangeExtension(fileInfo.FullName, null);
			Dictionary<string, string> dictModItems = SpriteReplacer.dictModItems;
			string value = (textures[key] = text);
			dictModItems[key] = value;
			list.Add(fileInfo);
		}
		if (list.Count > 0)
		{
			Debug.Log($"Texture: {textures.Count}");
		}
		return list;
	}

	public IReadOnlyList<FileInfo> ParsePortrait(DirectoryInfo dir)
	{
		List<FileInfo> list = new List<FileInfo>();
		FileInfo[] files = dir.GetFiles("*.png", SearchOption.TopDirectoryOnly);
		foreach (FileInfo fileInfo in files)
		{
			string name = fileInfo.Name;
			if (name.StartsWith("BG_", StringComparison.Ordinal))
			{
				Portrait.modPortraitBGs.Add(fileInfo);
			}
			else if (name.StartsWith("BGF_", StringComparison.Ordinal))
			{
				Portrait.modPortraitBGFs.Add(fileInfo);
			}
			else if (name.EndsWith("-full.png", StringComparison.Ordinal))
			{
				Portrait.modFull.Add(fileInfo);
			}
			else if (name.EndsWith("-overlay.png", StringComparison.Ordinal))
			{
				Portrait.modOverlays.Add(fileInfo);
			}
			else
			{
				Portrait.modPortraits.Add(fileInfo);
			}
			Portrait.allIds.Add(name);
			portraits[name] = fileInfo;
			list.Add(fileInfo);
		}
		if (list.Count > 0)
		{
			Debug.Log($"Portrait: {portraits.Count}");
		}
		return list;
	}

	public void ParseLangMod(DirectoryInfo dir)
	{
		if (Mapping == null)
		{
			Mapping = new FileMapping(dir.Parent);
		}
		Mapping.RebuildLangModMapping(EClass.core.config?.lang ?? EClass.core.debug.langCode.ToString());
		Lang.excelDialog = null;
		Debug.Log("LangMod: " + string.Join(", ", Mapping.OrderedLangMods));
		if (Mapping.SourceSheets.Count > 0)
		{
			Debug.Log($"Source Sheets: {Mapping.SourceSheets.Count}, {Mapping.SourceLangMod}");
		}
	}

	public IReadOnlyList<FileInfo> ParseSound(DirectoryInfo dir)
	{
		List<FileInfo> list = new List<FileInfo>();
		FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);
		foreach (FileInfo fileInfo in files)
		{
			if (ModUtil.GetAudioType(fileInfo.Extension) != 0)
			{
				string key = Path.ChangeExtension(Path.GetRelativePath(dir.FullName, fileInfo.FullName).NormalizePath(), null);
				Dictionary<string, FileInfo> dictionary = MOD.sounds;
				FileInfo value = (sounds[key] = fileInfo);
				dictionary[key] = value;
				list.Add(fileInfo);
			}
		}
		if (list.Count > 0)
		{
			Debug.Log($"Sound: {sounds.Count}");
		}
		int num = sounds.Keys.Count((string k) => k.StartsWith("BGM/"));
		if (num > 0)
		{
			Debug.Log($"BGM: {num}");
		}
		return list;
	}

	public SortedDictionary<string, string> ExportSourceLocalizations()
	{
		SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
		try
		{
			foreach (SourceData.BaseRow sourceRow in sourceRows)
			{
				foreach (KeyValuePair<string, string> item in sourceRow.ExportTexts(sourceRow.UseAlias ? "alias" : "id"))
				{
					item.Deconstruct(out var key, out var value);
					string key2 = key;
					string value2 = value;
					sortedDictionary[key2] = value2;
				}
			}
		}
		catch (Exception arg)
		{
			Debug.LogError($"#source failed to export localization for {title}/{id}\n{arg}");
		}
		return sortedDictionary;
	}

	public void ImportSourceLocalizations(IReadOnlyDictionary<string, string> texts)
	{
		foreach (SourceData.BaseRow sourceRow in sourceRows)
		{
			sourceRow.ImportTexts(texts, sourceRow.UseAlias ? "alias" : "id");
		}
	}

	public void ClearSourceLocalizations(string lang)
	{
		Mapping.RebuildLangModMapping(lang);
		for (FileInfo fileInfo = Mapping.RelocateFile("SourceLocalization.json"); fileInfo != null; fileInfo = Mapping.RelocateFile("SourceLocalization.json"))
		{
			fileInfo.Delete();
			Debug.Log("#source localization deleted " + fileInfo.ShortPath());
		}
		Mapping.RebuildLangModMapping(Lang.langCode);
	}

	public void AddOrUpdateLang(DirectoryInfo dir)
	{
		Core core = EClass.core;
		SourceManager sources = core.sources;
		DirectoryInfo[] directories = dir.GetDirectories();
		foreach (DirectoryInfo directoryInfo in directories)
		{
			if (!directoryInfo.Name.StartsWith("_") && !TryAddLang(directoryInfo, isNew: false))
			{
				Debug.Log("Generating Language Mod Contents:" + directoryInfo.FullName);
				IO.CopyDir(CorePath.packageCore + "Lang/EN", directoryInfo.FullName);
				Directory.CreateDirectory(directoryInfo.FullName + "/Dialog");
				IO.CopyDir(CorePath.packageCore + "Lang/_Dialog", directoryInfo.FullName + "/Dialog");
				sources.ExportSourceTexts(directoryInfo.FullName + "/Game");
				IO.Copy(CorePath.packageCore + "Lang/lang.ini", directoryInfo.FullName + "/");
				TryAddLang(directoryInfo, isNew: true);
			}
		}
		bool TryAddLang(DirectoryInfo dirLang, bool isNew)
		{
			string name = dirLang.Name;
			FileInfo[] files = dirLang.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				if (fileInfo.Name == "lang.ini")
				{
					LangSetting langSetting = new LangSetting(fileInfo.FullName)
					{
						id = name,
						dir = dirLang.FullName + "/"
					};
					if (isNew)
					{
						langSetting.SetVersion();
					}
					else if ((Application.isEditor || Lang.runUpdate) && !Lang.IsBuiltin(dirLang.Name) && langSetting.GetVersion() < core.version.GetInt())
					{
						sources.Init();
						Log.system = "Updated Language Files:" + Environment.NewLine + Environment.NewLine;
						Debug.Log("Updating Language:" + langSetting.name + "/" + langSetting.GetVersion() + "/" + core.version.GetInt());
						string text = dirLang.FullName + "/Game";
						Directory.Move(text, text + "_temp");
						sources.ExportSourceTexts(text);
						sources.UpdateSourceTexts(text);
						IO.DeleteDirectory(text + "_temp");
						text = dirLang.FullName + "/Dialog";
						Directory.Move(text, text + "_temp");
						IO.CopyDir(CorePath.packageCore + "Lang/_Dialog", text);
						UpdateDialogs(new DirectoryInfo(text), text + "_temp");
						IO.DeleteDirectory(text + "_temp");
						text = dirLang.FullName + "/Data";
						IO.CopyDir(text, text + "_temp");
						IO.Copy(CorePath.packageCore + "Lang/EN/Data/god_talk.xlsx", text);
						IO.Copy(CorePath.packageCore + "Lang/EN/Data/chara_talk.xlsx", text);
						UpdateTalks(new DirectoryInfo(text), text + "_temp");
						IO.DeleteDirectory(text + "_temp");
						langSetting.SetVersion();
						IO.SaveText(dirLang.FullName + "/update.txt", Log.system);
					}
					MOD.langs[name] = langSetting;
					return true;
				}
			}
			return false;
		}
	}

	public void UpdateDialogs(DirectoryInfo dir, string dirTemp)
	{
		DirectoryInfo[] directories = dir.GetDirectories();
		foreach (DirectoryInfo directoryInfo in directories)
		{
			UpdateDialogs(directoryInfo, dirTemp + "/" + directoryInfo.Name);
		}
		FileInfo[] files = dir.GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			if (fileInfo.Name.EndsWith("xlsx"))
			{
				UpdateExcelBook(fileInfo, dirTemp, updateOnlyText: true);
			}
		}
	}

	public void UpdateTalks(DirectoryInfo dir, string dirTemp)
	{
		FileInfo[] files = dir.GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			if (fileInfo.Name == "god_talk.xlsx" || fileInfo.Name == "chara_talk.xlsx")
			{
				UpdateExcelBook(fileInfo, dirTemp, updateOnlyText: false);
			}
		}
	}

	public void UpdateExcelBook(FileInfo f, string dirTemp, bool updateOnlyText)
	{
		string path = dirTemp + "/" + f.Name;
		if (!File.Exists(path))
		{
			return;
		}
		XSSFWorkbook xSSFWorkbook;
		using (FileStream @is = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			xSSFWorkbook = new XSSFWorkbook((Stream)@is);
		}
		XSSFWorkbook xSSFWorkbook2;
		using (FileStream is2 = File.Open(f.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
		{
			xSSFWorkbook2 = new XSSFWorkbook((Stream)is2);
		}
		for (int i = 0; i < xSSFWorkbook2.NumberOfSheets; i++)
		{
			ISheet sheetAt = xSSFWorkbook2.GetSheetAt(i);
			ISheet sheet = xSSFWorkbook.GetSheet(sheetAt.SheetName);
			if (sheet == null)
			{
				Log.system = Log.system + "Old sheet not found:" + sheetAt.SheetName + Environment.NewLine;
				continue;
			}
			int num = UpdateExcelSheet(sheetAt, sheet, updateOnlyText);
			Log.system = Log.system + ((num == 0) ? "(No Changes) " : "(Updated) ") + f.FullName + "(" + sheetAt.SheetName + ")" + Environment.NewLine;
			if (num != 0)
			{
				Log.system = Log.system + num + Environment.NewLine;
			}
			Log.system += Environment.NewLine;
		}
		using FileStream stream = new FileStream(f.FullName, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
		xSSFWorkbook2.Write(stream);
	}

	public int UpdateExcelSheet(ISheet destSheet, ISheet oldSheet, bool updateOnlytext)
	{
		Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
		int num = 0;
		int num2 = 0;
		int num3 = 10;
		IRow row2 = destSheet.GetRow(0);
		IRow row3 = oldSheet.GetRow(0);
		List<SheetIndex> list = new List<SheetIndex>();
		int cellnum = FindField(row2, "id");
		int cellnum2 = FindField(row3, "id");
		for (int i = 0; i < row2.LastCellNum; i++)
		{
			ICell cell = row2.GetCell(i);
			if (cell == null)
			{
				break;
			}
			string stringCellValue = cell.StringCellValue;
			if (stringCellValue == "id" || (updateOnlytext && stringCellValue != "text"))
			{
				continue;
			}
			for (int j = 0; j < row3.LastCellNum; j++)
			{
				cell = row3.GetCell(j);
				if (cell == null)
				{
					break;
				}
				if (cell.StringCellValue == stringCellValue)
				{
					list.Add(new SheetIndex
					{
						dest = i,
						old = j
					});
					Debug.Log(destSheet.SheetName + "/" + stringCellValue + "/" + i + "/" + j);
					break;
				}
			}
		}
		for (int k = 2; k <= oldSheet.LastRowNum; k++)
		{
			IRow row4 = oldSheet.GetRow(k);
			if (row4 == null)
			{
				if (num2 >= num3)
				{
					break;
				}
				num2++;
				continue;
			}
			num2 = 0;
			ICell cell2 = row4.GetCell(cellnum2);
			if (cell2 == null)
			{
				continue;
			}
			string text = ((cell2.CellType == CellType.Numeric) ? cell2.NumericCellValue.ToString() : cell2.StringCellValue);
			if (text.IsEmpty())
			{
				continue;
			}
			string[] array = new string[list.Count];
			for (int l = 0; l < list.Count; l++)
			{
				ICell cell3 = row4.GetCell(list[l].old);
				if (cell3 != null)
				{
					string stringCellValue2 = cell3.StringCellValue;
					if (!stringCellValue2.IsEmpty())
					{
						array[l] = stringCellValue2;
					}
				}
			}
			dictionary.Add(text, array);
		}
		num2 = 0;
		for (int m = 2; m <= destSheet.LastRowNum; m++)
		{
			IRow row5 = destSheet.GetRow(m);
			if (row5 == null)
			{
				if (num2 >= num3)
				{
					break;
				}
				num2++;
				continue;
			}
			num2 = 0;
			ICell cell4 = row5.GetCell(cellnum);
			if (cell4 == null)
			{
				continue;
			}
			string text2 = ((cell4.CellType == CellType.Numeric) ? cell4.NumericCellValue.ToString() : cell4.StringCellValue);
			if (text2.IsEmpty() || !dictionary.ContainsKey(text2))
			{
				continue;
			}
			string[] array2 = dictionary[text2];
			for (int n = 0; n < list.Count; n++)
			{
				ICell cell5 = row5.GetCell(list[n].dest) ?? row5.CreateCell(list[n].dest, CellType.String);
				if (cell5 != null)
				{
					cell5.SetCellValue(array2[n]);
					cell5.SetCellType(CellType.String);
					cell5.SetAsActiveCell();
					num++;
				}
			}
		}
		return num;
		static int FindField(IRow row, string id)
		{
			for (int num4 = 0; num4 < row.LastCellNum; num4++)
			{
				ICell cell6 = row.GetCell(num4);
				if (cell6 == null)
				{
					break;
				}
				if (cell6.StringCellValue == id)
				{
					return num4;
				}
			}
			return -1;
		}
	}
}
