using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;
using UnityEngine.Networking;

public class ModUtil : EClass
{
	public static SourceImporter sourceImporter = new SourceImporter(SourceMapping);

	public static Dictionary<string, string> fallbackTypes = new Dictionary<string, string>();

	public static IReadOnlyDictionary<string, SourceData> SourceMapping => (from f in typeof(SourceManager).GetFields()
		where typeof(SourceData).IsAssignableFrom(f.FieldType)
		select f).ToDictionary((FieldInfo f) => f.FieldType.Name, (FieldInfo f) => f.GetValue(EClass.sources) as SourceData);

	public static void OnModsActivated()
	{
		SoundManager.current.soundLoaders.Add(LoadSoundData);
		BaseModManager.PublishEvent("elin.mods.activated");
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
		UnityEngine.Debug.Log("ImportExcel source:" + source?.ToString() + " Path:" + pathToExcelFile);
		using FileStream @is = File.Open(pathToExcelFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		XSSFWorkbook xSSFWorkbook = new XSSFWorkbook((Stream)@is);
		for (int i = 0; i < xSSFWorkbook.NumberOfSheets; i++)
		{
			ISheet sheetAt = xSSFWorkbook.GetSheetAt(i);
			if (sheetAt.SheetName != sheetName)
			{
				continue;
			}
			UnityEngine.Debug.Log("Importing Sheet:" + sheetName);
			try
			{
				ExcelParser.path = pathToExcelFile;
				if (!source.ImportData(sheetAt, new FileInfo(pathToExcelFile).Name, overwrite: true))
				{
					UnityEngine.Debug.LogError(ERROR.msg);
					break;
				}
				UnityEngine.Debug.Log("Imported " + sheetAt.SheetName);
				source.Reset();
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogError("[Error] Skipping import " + sheetAt.SheetName + " :" + ex.Message + "/" + ex.Source + "/" + ex.StackTrace);
				break;
			}
		}
	}

	public static ModPackage FindSourceRowPackage(SourceData.BaseRow row)
	{
		return ModManagerCore.Instance.MappedPackages.Values.FirstOrDefault((EMod p) => p.sourceRows.Contains(row)) as ModPackage;
	}

	public static ModPackage GetModPackage(string modId)
	{
		return ModManagerCore.Instance.MappedPackages.GetValueOrDefault(modId) as ModPackage;
	}

	public static SerializableSoundData GetSoundMeta(string soundPath)
	{
		string path = Path.ChangeExtension(soundPath, ".json");
		SerializableSoundData serializableSoundData;
		if (File.Exists(path))
		{
			try
			{
				serializableSoundData = IO.LoadFile<SerializableSoundData>(path);
				if (serializableSoundData.dataVersion == SerializableSoundData.SoundDataMetaVersion.V1)
				{
					return serializableSoundData;
				}
			}
			catch
			{
			}
		}
		serializableSoundData = new SerializableSoundData();
		if (soundPath.NormalizePath().Contains("/Sound/BGM/"))
		{
			serializableSoundData.type = SoundData.Type.BGM;
			serializableSoundData.bgmDataOptional = new SerializableBGMData();
		}
		IO.SaveFile(path, serializableSoundData);
		return serializableSoundData;
	}

	public static AudioType GetAudioType(string extension)
	{
		return extension.ToLowerInvariant().Trim() switch
		{
			".acc" => AudioType.ACC, 
			".mp3" => AudioType.MPEG, 
			".ogg" => AudioType.OGGVORBIS, 
			".wav" => AudioType.WAV, 
			_ => AudioType.UNKNOWN, 
		};
	}

	public static SoundData LoadSoundData(string soundId)
	{
		if (!MOD.sounds.TryGetValue(soundId, out var value) || !value.Exists)
		{
			return null;
		}
		return LoadSoundData(value);
	}

	public static SoundData LoadSoundData(FileInfo soundFile)
	{
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(soundFile.FullName);
		string fullName = soundFile.FullName;
		AudioType audioType = GetAudioType(soundFile.Extension);
		bool stream = fullName.NormalizePath().Contains("/BGM/") && audioType == AudioType.OGGVORBIS;
		using UnityWebRequest unityWebRequest = AudioClipStream.GetAudioClip("file://" + fullName, audioType, compressed: false, stream);
		unityWebRequest.SendWebRequest();
		Stopwatch stopwatch = Stopwatch.StartNew();
		while (!unityWebRequest.isDone && stopwatch.ElapsedMilliseconds < 5000)
		{
			Thread.Sleep(1);
		}
		if (unityWebRequest.result != UnityWebRequest.Result.Success)
		{
			UnityEngine.Debug.LogError("#sound '" + fileNameWithoutExtension + "' failed to load: " + unityWebRequest.error.IsEmpty("timeout"));
			return null;
		}
		AudioClip content = DownloadHandlerAudioClip.GetContent(unityWebRequest);
		int? num = content?.samples;
		if (!num.HasValue || num.GetValueOrDefault() <= 0)
		{
			UnityEngine.Debug.LogError($"#sound '{fileNameWithoutExtension}' sample is null: {audioType}");
			return null;
		}
		content.name = fileNameWithoutExtension;
		SoundData soundData = GetSoundMeta(fullName).ToSoundData();
		if (soundData is BGMData bGMData)
		{
			bGMData._name = Path.GetFileNameWithoutExtension(fullName);
			if (bGMData.song == null)
			{
				bGMData.song = new BGMData.SongData();
				bGMData.song.parts.Add(new BGMData.Part());
			}
		}
		soundData.clip = content;
		soundData.name = fileNameWithoutExtension;
		UnityEngine.Debug.Log($"#sound '{fileNameWithoutExtension}' loaded: {audioType}/{content.length}s");
		SoundManager.current.dictData[fileNameWithoutExtension] = soundData;
		return soundData;
	}
}
