using System;
using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

public class ModUtil : EClass
{
	public static Dictionary<string, string> fallbackTypes = new Dictionary<string, string>();

	public static void Test()
	{
		ImportExcel("", "", EClass.sources.charas);
	}

	public static void OnModsActivated()
	{
	}

	public static void LoadTypeFallback()
	{
		string text = "type_resolver.txt";
		string[] array = new string[0];
		if (File.Exists(CorePath.RootData + text))
		{
			array = IO.LoadTextArray(CorePath.RootData + text);
		}
		else
		{
			array = new string[2] { "TrueArena,ArenaWaveEvent,ZoneEvent", "Elin-GeneRecombinator,Elin_GeneRecombinator.IncubationSacrifice,Chara" };
			IO.SaveTextArray(CorePath.RootData + text, array);
		}
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string[] array3 = array2[i].Split(',');
			if (array3.Length >= 2)
			{
				RegisterSerializedTypeFallback(array3[0], array3[1], array3[2]);
			}
		}
	}

	public static void RegisterSerializedTypeFallback(string nameAssembly, string nameType, string nameFallbackType)
	{
		fallbackTypes[nameType] = nameFallbackType;
	}

	public static void ImportExcel(string pathToExcelFile, string sheetName, SourceData source)
	{
		Debug.Log("ImportExcel source:" + source?.ToString() + " Path:" + pathToExcelFile);
		using FileStream @is = File.Open(pathToExcelFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		XSSFWorkbook xSSFWorkbook = new XSSFWorkbook((Stream)@is);
		for (int i = 0; i < xSSFWorkbook.NumberOfSheets; i++)
		{
			ISheet sheetAt = xSSFWorkbook.GetSheetAt(i);
			if (sheetAt.SheetName != sheetName)
			{
				continue;
			}
			Debug.Log("Importing Sheet:" + sheetName);
			try
			{
				if (!source.ImportData(sheetAt, new FileInfo(pathToExcelFile).Name, overwrite: true))
				{
					Debug.LogError(ERROR.msg);
					break;
				}
				Debug.Log("Imported " + sheetAt.SheetName);
				source.Reset();
			}
			catch (Exception ex)
			{
				Debug.LogError("[Error] Skipping import " + sheetAt.SheetName + " :" + ex.Message + "/" + ex.Source + "/" + ex.StackTrace);
				break;
			}
		}
	}
}
