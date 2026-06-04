using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MOD
{
	public static Dictionary<string, LangSetting> langs = new Dictionary<string, LangSetting>();

	public static TalkDataList listTalk = new TalkDataList();

	public static GodTalkDataList listGodTalk = new GodTalkDataList();

	public static ToneDataList tones = new ToneDataList();

	public static ExcelDataList actorSources = new ExcelDataList();

	public static ModItemList<Sprite> sprites = new ModItemList<Sprite>();

	public static Action<DirectoryInfo> OnAddPcc;

	public static List<FileInfo> listMaps = new List<FileInfo>();

	public static List<FileInfo> listPartialMaps = new List<FileInfo>();

	public static Dictionary<string, FileInfo> sounds = new Dictionary<string, FileInfo>();

	public static void ResetResources()
	{
		langs.Clear();
		listTalk.Clear();
		listGodTalk.Clear();
		tones.Clear();
		actorSources.Clear();
		sprites = new ModItemList<Sprite>();
		listMaps.Clear();
		listPartialMaps.Clear();
		sounds.Clear();
	}
}
