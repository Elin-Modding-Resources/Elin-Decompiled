using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MOD
{
	public static Dictionary<string, LangSetting> langs = new Dictionary<string, LangSetting>();

	public static TalkDataList listTalk = new TalkDataList();

	public static ToneDataList tones = new ToneDataList();

	public static ExcelDataList actorSources = new ExcelDataList();

	public static ModItemList<Sprite> sprites = new ModItemList<Sprite>();

	public static Action<DirectoryInfo> OnAddPcc;

	public static List<FileInfo> listMaps = new List<FileInfo>();

	public static List<FileInfo> listPartialMaps = new List<FileInfo>();
}
