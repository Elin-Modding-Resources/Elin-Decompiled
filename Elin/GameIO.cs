using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class GameIO : EClass
{
	public static JsonSerializerSettings jsReadGame = new JsonSerializerSettings
	{
		NullValueHandling = NullValueHandling.Ignore,
		DefaultValueHandling = DefaultValueHandling.Ignore,
		PreserveReferencesHandling = PreserveReferencesHandling.Objects,
		TypeNameHandling = TypeNameHandling.Auto,
		Error = IO.OnError,
		SerializationBinder = GameSerializationBinder.Instance
	};

	public static JsonSerializerSettings jsWriteGame = new JsonSerializerSettings
	{
		NullValueHandling = NullValueHandling.Ignore,
		DefaultValueHandling = DefaultValueHandling.Ignore,
		PreserveReferencesHandling = PreserveReferencesHandling.Objects,
		TypeNameHandling = TypeNameHandling.Auto,
		ContractResolver = ShouldSerializeContractResolver.Instance,
		Error = IO.OnError
	};

	public static Formatting formatting = Formatting.Indented;

	public static string pathCurrentSave => (EClass.core.game.isCloud ? CorePath.RootSaveCloud : CorePath.RootSave) + Game.id + "/";

	public static string pathTemp => pathCurrentSave + "Temp/";

	public static int NumBackup => (int)MathF.Max(5f, EClass.core.config.game.numBackup);

	public static bool compressSave => EClass.core.config.test.compressSave;

	public static void ResetTemp()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(pathTemp);
		if (directoryInfo.Exists)
		{
			directoryInfo.Delete(recursive: true);
		}
		IO.CreateDirectory(pathTemp);
	}

	public static void ClearTemp()
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(pathTemp);
		if (directoryInfo.Exists)
		{
			DirectoryInfo[] directories = directoryInfo.GetDirectories();
			for (int i = 0; i < directories.Length; i++)
			{
				directories[i].Delete(recursive: true);
			}
			FileInfo[] files = directoryInfo.GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				files[i].Delete();
			}
		}
	}

	public static GameIndex SaveGame()
	{
		string text = JsonConvert.SerializeObject(EClass.core.game, formatting, jsWriteGame);
		string path = pathCurrentSave + "game.txt";
		GameIndex gameIndex = new GameIndex().Create(EClass.core.game);
		gameIndex.id = Game.id;
		gameIndex.cloud = EClass.game.isCloud;
		IO.SaveFile(pathCurrentSave + "index.txt", gameIndex);
		if (compressSave)
		{
			IO.Compress(path, text);
		}
		else
		{
			File.WriteAllText(path, text);
		}
		DirectoryInfo[] directories = new DirectoryInfo(pathCurrentSave).GetDirectories();
		foreach (DirectoryInfo directoryInfo in directories)
		{
			if (int.TryParse(directoryInfo.Name, out var result) && !EClass.game.spatials.map.ContainsKey(result))
			{
				IO.DeleteDirectory(directoryInfo.FullName);
				Debug.Log("Deleting unused map:" + directoryInfo.FullName);
			}
		}
		ClearTemp();
		if (gameIndex.cloud)
		{
			PrepareSteamCloud(gameIndex.id);
		}
		return gameIndex;
	}

	public static void MakeBackup(GameIndex index, string suffix = "")
	{
		Debug.Log("Start backup:" + index.id);
		string id = index.id;
		bool cloud = index.cloud;
		IO.CreateDirectory(cloud ? CorePath.PathBackupCloud : CorePath.PathBackup);
		string text = (cloud ? CorePath.PathBackupCloud : CorePath.PathBackup) + id;
		IO.CreateDirectory(text);
		Debug.Log(text);
		List<DirectoryInfo> dirs = new DirectoryInfo(text).GetDirectories().ToList();
		dirs.ForeachReverse(delegate(DirectoryInfo i)
		{
			if (!int.TryParse(i.Name, out var _))
			{
				dirs.Remove(i);
			}
		});
		dirs.Sort((DirectoryInfo a, DirectoryInfo b) => int.Parse(a.Name) - int.Parse(b.Name));
		int count = dirs.Count;
		Debug.Log("Deleting excess backup:" + dirs.Count + "/" + NumBackup);
		if (count > NumBackup)
		{
			for (int j = 0; j < count - NumBackup; j++)
			{
				IO.DeleteDirectory(dirs[j].FullName);
			}
		}
		Debug.Log("Copying backup:");
		string newId = GetNewId(text + "/", "", (dirs.Count == 0) ? 1 : int.Parse(dirs.LastItem().Name));
		IO.CopyDir((cloud ? CorePath.RootSaveCloud : CorePath.RootSave) + id + "/", text + "/" + newId, (string s) => s == "Temp");
	}

	public static bool CanLoad(string root)
	{
		GameIndex gameIndex = IO.LoadFile<GameIndex>(root + "/index.txt");
		return EClass.core.version.IsSaveCompatible(gameIndex.version);
	}

	public static Game LoadGame(string id, string root, bool cloud)
	{
		Game.id = id;
		GameIndex gameIndex = IO.LoadFile<GameIndex>(root + "/index.txt");
		string path = root + "/game.txt";
		foreach (KeyValuePair<string, string> fallbackType in gameIndex.fallbackTypes)
		{
			ModUtil.fallbackTypes[fallbackType.Key] = fallbackType.Value;
		}
		if (cloud)
		{
			gameIndex.cloud = true;
			Debug.Log(TryLoadSteamCloud(root));
		}
		else if (!File.Exists(path))
		{
			Debug.Log(TryLoadSteamCloud(root));
		}
		return JsonConvert.DeserializeObject<Game>(IO.IsCompressed(path) ? IO.Decompress(path) : File.ReadAllText(path), jsReadGame);
	}

	public static void PrepareSteamCloud(string id, string path = "")
	{
		if (path.IsEmpty())
		{
			path = CorePath.RootSaveCloud + "/" + id;
		}
		Debug.Log("Prepareing Steam Cloud:" + id + ": " + path);
		string text = CorePath.RootSaveCloud + "/cloud.zip";
		string text2 = path + "/cloud.zip";
		try
		{
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			if (File.Exists(text2))
			{
				File.Delete(text2);
			}
			ZipFile.CreateFromDirectory(path, text);
			if (File.Exists(text2))
			{
				File.Delete(text2);
			}
			File.Move(text, text2);
		}
		catch (Exception ex)
		{
			EClass.ui.Say(ex.Message);
		}
	}

	public static bool TryLoadSteamCloud(string pathSave)
	{
		Debug.Log("LoadGame using cloud save");
		string text = pathSave + "/cloud.zip";
		string text2 = CorePath.RootSaveCloud + "/cloud.zip";
		bool flag = false;
		try
		{
			if (!File.Exists(text))
			{
				EClass.ui.Say("Steam Cloud save not found:" + text);
				return true;
			}
			if (File.Exists(text2))
			{
				File.Delete(text2);
			}
			File.Move(text, text2);
			IO.DeleteDirectory(pathSave);
			flag = true;
			Directory.CreateDirectory(pathSave);
			ZipFile.ExtractToDirectory(text2, pathSave);
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			File.Move(text2, text);
		}
		catch (Exception ex)
		{
			EClass.ui.Say(ex.Message);
			if (flag)
			{
				Debug.Log("Try restore backup:");
				if (Directory.Exists(pathSave))
				{
					Directory.Delete(pathSave);
				}
				File.Move(text2, text);
				return true;
			}
			return false;
		}
		return true;
	}

	public static void UpdateGameIndex(GameIndex i)
	{
		IO.SaveFile(i.path + "/index.txt", i);
	}

	public static void SaveFile(string path, object obj)
	{
		IO.SaveFile(path, obj, compressSave, jsWriteGame);
	}

	public static T LoadFile<T>(string path) where T : new()
	{
		return IO.LoadFile<T>(path, compressSave, jsReadGame);
	}

	public static void DeleteGame(string id, bool cloud, bool deleteBackup = true)
	{
		string path = (cloud ? CorePath.RootSaveCloud : CorePath.RootSave) + id;
		if (!Directory.Exists(path))
		{
			return;
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		if (directoryInfo.Exists)
		{
			directoryInfo.Delete(recursive: true);
		}
		if (deleteBackup)
		{
			directoryInfo = new DirectoryInfo((cloud ? CorePath.PathBackupCloud : CorePath.PathBackup) + id);
			if (directoryInfo.Exists)
			{
				directoryInfo.Delete(recursive: true);
			}
		}
	}

	public static List<GameIndex> GetGameList(string path, bool sortByName = false, bool includeEmptyFolder = false)
	{
		List<GameIndex> list = new List<GameIndex>();
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		if (!directoryInfo.Exists)
		{
			return list;
		}
		DirectoryInfo[] directories = directoryInfo.GetDirectories();
		foreach (DirectoryInfo directoryInfo2 in directories)
		{
			if (File.Exists(directoryInfo2?.ToString() + "/index.txt"))
			{
				try
				{
					GameIndex gameIndex = IO.LoadFile<GameIndex>(directoryInfo2?.ToString() + "/index.txt");
					gameIndex.id = directoryInfo2.Name;
					gameIndex.path = directoryInfo2.FullName;
					list.Add(gameIndex);
				}
				catch (Exception message)
				{
					Debug.Log(message);
					goto IL_0097;
				}
				continue;
			}
			goto IL_0097;
			IL_0097:
			if (includeEmptyFolder && Directory.Exists(CorePath.PathBackup + directoryInfo2.Name))
			{
				GameIndex gameIndex2 = new GameIndex();
				gameIndex2.id = directoryInfo2.Name;
				gameIndex2.path = directoryInfo2.FullName;
				gameIndex2.date = (gameIndex2.real = new Date());
				list.Add(gameIndex2);
			}
		}
		if (sortByName)
		{
			list.Sort(delegate(GameIndex a, GameIndex b)
			{
				int.TryParse(a.id, out var result);
				int.TryParse(b.id, out var result2);
				return result2 - result;
			});
		}
		else
		{
			list.Sort((GameIndex a, GameIndex b) => b.real.GetRawReal() - a.real.GetRawReal());
		}
		return list;
	}

	public static void DeleteEmptyGameFolders(string path)
	{
		DirectoryInfo[] directories = new DirectoryInfo(path).GetDirectories();
		foreach (DirectoryInfo directoryInfo in directories)
		{
			if (!File.Exists(directoryInfo?.ToString() + "/game.txt"))
			{
				directoryInfo.Delete(recursive: true);
			}
		}
	}

	public static string GetNewId(string path, string prefix = "", int start = 1)
	{
		string text = "";
		for (int i = start; i < 999999; i++)
		{
			text = prefix + i;
			if (!Directory.Exists(path + text))
			{
				break;
			}
		}
		return text;
	}
}
