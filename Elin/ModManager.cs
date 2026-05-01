using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.API;
using IniParser.Model;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;

[Serializable]
public class ModManager : ModManagerCore
{
	public static List<object> ListPluginObject = new List<object>();

	public static bool disableMod;

	private LoadingScreen _loading;

	public List<FileInfo> replaceFiles = new List<FileInfo>();

	private Action ImportModGodTalks;

	public static List<string> ListChainLoad => BaseModManager.listChainLoad;

	public static DirectoryInfo DirWorkshop => Instance.dirWorkshop;

	public new static ModManager Instance => (ModManager)BaseModManager.Instance;

	public static bool IsInitialized => BaseModManager.isInitialized;

	public override void Init(string path, string defaultPackage = "_Elona")
	{
		base.Init(path, defaultPackage);
		Debug.Log("IsOffline:" + BaseCore.IsOffline);
		IniData elinIni = Core.GetElinIni();
		if (elinIni != null)
		{
			if (BaseCore.IsOffline)
			{
				string key = elinIni.GetKey("path_workshop");
				if (!key.IsEmpty())
				{
					dirWorkshop = new DirectoryInfo(key);
				}
			}
			else
			{
				string path2 = Path.Combine(App.Client.GetAppInstallDirectory(SteamSettings.behaviour.settings.applicationId), "../../workshop/content/2135150");
				dirWorkshop = new DirectoryInfo(path2);
				elinIni.Global["path_workshop"] = dirWorkshop.FullName;
				Core.SaveElinIni(elinIni);
			}
		}
		if (!dirWorkshop.Exists)
		{
			dirWorkshop = null;
		}
		Debug.Log("Workshop:" + dirWorkshop);
		Debug.Log("Packages:" + BaseModManager.rootMod);
		Debug.Log("Core Mod:" + BaseModManager.rootDefaultPacakge);
		BaseModManager.SubscribeEvent("elin.source.lang_set", delegate(object lang)
		{
			if (ModManagerCore.useLocalizations)
			{
				ImportSourceLocalizations(lang as string);
				ModManagerCore.generateLocalizations = false;
			}
			ImportAllModDialogs();
			BookList.dict = null;
		});
	}

	public void SaveLoadOrder()
	{
		if (!disableMod)
		{
			List<string> contents = (from p in packages
				where !p.builtin && p.dirInfo.Exists
				select $"{p.dirInfo.FullName},{(p.willActivate ? 1 : 0)}").ToList();
			File.WriteAllLines(CorePath.rootExe + "loadorder.txt", contents);
		}
	}

	public void LoadLoadOrder()
	{
		string path = CorePath.rootExe + "loadorder.txt";
		if (!File.Exists(path))
		{
			return;
		}
		Dictionary<string, BaseModPackage> dictionary = new Dictionary<string, BaseModPackage>();
		foreach (BaseModPackage package in packages)
		{
			if (!package.builtin)
			{
				dictionary[package.dirInfo.FullName] = package;
			}
		}
		int num = 0;
		string[] array = File.ReadAllLines(path);
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(',');
			if (dictionary.TryGetValue(array2[0], out var value))
			{
				value.loadPriority = num;
				value.willActivate = array2[1] == "1";
			}
			num++;
		}
	}

	public void ImportSourceLocalizations(string lang)
	{
		PackageIterator.RebuildAllMappings(lang);
		SortedDictionary<string, string> sortedDictionary = new SortedDictionary<string, string>();
		Dictionary<string, SortedDictionary<string, string>> dictionary = new Dictionary<string, SortedDictionary<string, string>>();
		string value;
		string key;
		foreach (string item in FileMapping.FallbackLut[lang].Append(lang).Distinct())
		{
			(FileInfo, EMod)[] filesEx = PackageIterator.GetFilesEx(item + "/SourceLocalization.json", useCache: false);
			for (int i = 0; i < filesEx.Length; i++)
			{
				var (fileInfo, eMod) = filesEx[i];
				try
				{
					SortedDictionary<string, string> sortedDictionary2 = IO.LoadFile<SortedDictionary<string, string>>(fileInfo.FullName);
					foreach (KeyValuePair<string, string> item2 in sortedDictionary2)
					{
						item2.Deconstruct(out value, out key);
						string key2 = value;
						string value2 = key;
						sortedDictionary[key2] = value2;
					}
					dictionary[eMod.id] = sortedDictionary2;
				}
				catch (Exception arg)
				{
					Debug.LogError($"#source localization failed to load {fileInfo.ShortPath()}\n{arg}");
				}
			}
		}
		JsonSerializerSettings setting = new JsonSerializerSettings
		{
			PreserveReferencesHandling = PreserveReferencesHandling.None,
			NullValueHandling = NullValueHandling.Ignore
		};
		foreach (BaseModPackage package in packages)
		{
			if (!(package is ModPackage modPackage) || package.builtin || !package.activated)
			{
				continue;
			}
			HashSet<SourceData.BaseRow> sourceRows = modPackage.sourceRows;
			if (sourceRows == null || sourceRows.Count <= 0)
			{
				continue;
			}
			modPackage.ImportSourceLocalizations(sortedDictionary);
			if (!ModManagerCore.generateLocalizations || (!modPackage.isInPackages && !Application.isEditor))
			{
				continue;
			}
			try
			{
				SortedDictionary<string, string> sortedDictionary3 = dictionary.GetValueOrDefault(modPackage.id) ?? new SortedDictionary<string, string>();
				SortedDictionary<string, string> sortedDictionary4 = modPackage.ExportSourceLocalizations();
				SortedDictionary<string, string> final = new SortedDictionary<string, string>();
				foreach (KeyValuePair<string, string> item3 in sortedDictionary4)
				{
					item3.Deconstruct(out key, out value);
					string key3 = key;
					string defaultValue = value;
					final[key3] = sortedDictionary3.GetValueOrDefault(key3, defaultValue);
				}
				if (sortedDictionary3.Count != final.Count || sortedDictionary3.Any((KeyValuePair<string, string> kv) => !final.ContainsKey(kv.Key)))
				{
					string path = Path.Combine(modPackage.dirInfo.FullName, "LangMod/" + lang + "/SourceLocalization.json");
					IO.SaveFile(path, final, compress: false, setting);
					Debug.Log($"#source localization updated {path.ShortPath()} / {modPackage}");
				}
			}
			catch (Exception arg2)
			{
				Debug.LogError($"#source localization failed to generate {modPackage}\n{arg2}");
			}
		}
	}

	public void ImportAllModSourceSheets()
	{
		try
		{
			SourceCache.InvalidateCacheVersion();
			SourceImporter.HotInit(new SourceData[2]
			{
				EClass.sources.elements,
				EClass.sources.materials
			});
			List<string> list = new List<string>();
			foreach (ModPackage item in packages.OfType<ModPackage>())
			{
				if (item.builtin || !item.activated)
				{
					continue;
				}
				foreach (FileInfo sourceSheet in item.Mapping.SourceSheets)
				{
					if (!sourceSheet.Name.StartsWith(".") && !sourceSheet.Name.Contains("~"))
					{
						ModUtil.sourceImporter.fileProviders[sourceSheet.FullName] = item;
						list.Add(sourceSheet.FullName);
					}
				}
			}
			ModUtil.sourceImporter.ImportFilesCached(list);
			SourceCache.FinalizeCache();
			SourceCache.InvalidateCacheBlobs();
			SourceCache.ClearDetail();
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
		Debug.Log("#source finished importing workbooks");
	}

	public void ImportAllModDialogs()
	{
		Lang.extraExcelDialogs = new HashSet<string>(PathComparer.Default);
		Lang.excelDialog = null;
		FileInfo[] files = PackageIterator.GetFiles("Dialog/dialog.xlsx");
		foreach (FileInfo fileInfo in files)
		{
			Lang.extraExcelDialogs.Add(fileInfo.FullName);
			Debug.Log("#dialog loaded " + fileInfo.ShortPath());
		}
	}

	public void ImportAllModGodTalks()
	{
		if (Lang.setting?.dir == null)
		{
			return;
		}
		Dictionary<string, Dictionary<string, string>> map = EClass.sources.dataGodTalk.sheets["_default"].map;
		foreach (ExcelData item in from f in PackageIterator.GetFiles("Data/god_talk.xlsx")
			select new ExcelData(f.FullName, 3))
		{
			foreach (KeyValuePair<string, Dictionary<string, string>> item2 in map)
			{
				item2.Deconstruct(out var key, out var _);
				string text = key;
				if (text.IsEmpty())
				{
					continue;
				}
				Dictionary<string, string> valueOrDefault = item.sheets["_default"].map.GetValueOrDefault(text);
				if (valueOrDefault == null)
				{
					continue;
				}
				foreach (KeyValuePair<string, string> item3 in valueOrDefault)
				{
					item3.Deconstruct(out key, out var value2);
					string text2 = key;
					string value3 = value2;
					if (text2 != "id")
					{
						map[text][text2] = value3;
					}
				}
			}
			Debug.Log("#dialog loaded " + item.path.ShortPath());
		}
	}

	public IEnumerator RefreshMods(Action onComplete, bool syncMods)
	{
		bool flag = !BaseCore.IsOffline && syncMods && UserGeneratedContent.Client.GetNumSubscribedItems() != 0;
		_loading = Util.Instantiate<LoadingScreen>("LoadingScreen");
		WaitForEndOfFrame awaiter = new WaitForEndOfFrame();
		packages.Clear();
		disableMod |= Application.isEditor && EClass.debug.skipMod;
		LoadLocalPackages();
		LoadCustomPackage();
		if (!disableMod)
		{
			_loading.Log("Loading workshop contents...");
			if (flag)
			{
				yield return LoadWorkshopPackages();
			}
			else if (dirWorkshop != null)
			{
				DirectoryInfo[] directories = dirWorkshop.GetDirectories();
				foreach (DirectoryInfo dir in directories)
				{
					AddPackage(dir);
				}
			}
		}
		InitPackagesMeta();
		LoadLoadOrder();
		packages.Sort((BaseModPackage a, BaseModPackage b) => a.loadPriority - b.loadPriority);
		foreach (BaseModPackage item in packages.Where((BaseModPackage p) => !p.isInPackages && p.willActivate && !p.id.IsEmpty()))
		{
			if (mappedPackages.TryGetValue(item.id, out var value) && value.isInPackages)
			{
				value.hasPublishedPackage = true;
			}
		}
		_loading.Log($"Total number of mods:{packages.Count}");
		_loading.Log("Activating Mods...");
		yield return awaiter;
		ActivatePackages();
		foreach (BaseModPackage package in packages)
		{
			if (package.activated)
			{
				mappedPackages[package.id] = package as ModPackage;
			}
		}
		BaseModManager.isInitialized = true;
		yield return awaiter;
		onComplete?.Invoke();
		if ((bool)_loading)
		{
			UnityEngine.Object.Destroy(_loading.gameObject);
		}
		yield return null;
	}

	public ModPackage AddPackage(DirectoryInfo dir, bool isInPackages = false)
	{
		ModPackage modPackage = new ModPackage
		{
			dirInfo = dir,
			installed = true,
			isInPackages = isInPackages,
			loadPriority = priorityIndex,
			Mapping = new FileMapping(dir)
		};
		packages.Add(modPackage);
		priorityIndex++;
		return modPackage;
	}

	public ModPackage AddWorkshopPackage(WorkshopItem item, bool isInPackages = false)
	{
		ulong sizeOnDisk;
		string folderPath;
		DateTime timeStamp;
		bool itemInstallInfo = UserGeneratedContent.Client.GetItemInstallInfo(item.FileId, out sizeOnDisk, out folderPath, out timeStamp);
		DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
		if (!directoryInfo.Exists)
		{
			return null;
		}
		ModPackage modPackage = AddPackage(directoryInfo, isInPackages);
		modPackage.installed = itemInstallInfo;
		modPackage.banned = item.IsBanned;
		return modPackage;
	}

	public int CountUserMod()
	{
		return packages.Count((BaseModPackage p) => !p.builtin);
	}

	public void LoadLocalPackages()
	{
		_loading.Log("Loading local Package...");
		DirectoryInfo[] directories = new DirectoryInfo(BaseModManager.rootMod).GetDirectories();
		Array.Reverse(directories);
		DirectoryInfo[] array = directories;
		foreach (DirectoryInfo directoryInfo in array)
		{
			if (!disableMod || !(directoryInfo.Name != "_Elona") || !(directoryInfo.Name != "_Lang_Chinese"))
			{
				AddPackage(directoryInfo, isInPackages: true);
			}
		}
	}

	public void LoadCustomPackage()
	{
		_loading.Log("Loading user Custom...");
		DirectoryInfo[] directories = new DirectoryInfo(CorePath.custom).GetDirectories();
		ModPackage package = new ModPackage();
		DirectoryInfo[] array = directories;
		foreach (DirectoryInfo dir in array)
		{
			ParseExtra(dir, package);
		}
	}

	public IEnumerator LoadWorkshopPackages()
	{
		WaitForEndOfFrame awaiter = new WaitForEndOfFrame();
		UgcQuery activeQuery = UgcQuery.GetSubscribed(withLongDescription: false, withMetadata: false, withKeyValueTags: false, withAdditionalPreviews: false, 0u);
		activeQuery.Execute(HandleWorkshopQuery);
		_loading.Log("Fetching subscriptions...(Hit ESC to cancel)");
		while (activeQuery.handle != UGCQueryHandle_t.Invalid && !UnityEngine.Input.GetKey(KeyCode.Escape))
		{
			yield return awaiter;
		}
		yield return UpdateWorkshopPackages();
		void HandleWorkshopQuery(UgcQuery query)
		{
			foreach (WorkshopItem results in query.ResultsList)
			{
				AddWorkshopPackage(results);
			}
		}
	}

	private IEnumerator UpdateWorkshopPackages()
	{
		_loading?.Log("Updating subscriptions...");
		WaitForEndOfFrame awaiter = new WaitForEndOfFrame();
		while (true)
		{
			bool flag = false;
			foreach (BaseModPackage item in packages.Where((BaseModPackage p) => !p.installed))
			{
				if (!(item.item is WorkshopItem { IsBanned: false } workshopItem))
				{
					continue;
				}
				flag = true;
				string text = "Downloading " + workshopItem.Title + ": ";
				BaseModPackage baseModPackage = item;
				if ((object)baseModPackage.progressText == null)
				{
					baseModPackage.progressText = _loading?.Log(text);
				}
				if (item.downloadStarted && workshopItem.DownloadCompletion >= 1f)
				{
					item.installed = true;
					if ((bool)item.progressText)
					{
						item.progressText.text = text + "Done!";
					}
				}
				else if (workshopItem.IsDownloading || workshopItem.IsDownloadPending)
				{
					int num = Mathf.FloorToInt(workshopItem.DownloadCompletion * 100f);
					if ((bool)item.progressText)
					{
						item.progressText.text = text + num + "%";
					}
				}
				else if (!item.downloadStarted)
				{
					item.downloadStarted = true;
					workshopItem.DownloadItem(highPriority: true);
					Debug.Log("Start downloading: " + workshopItem.Title + " | " + $"Installed={workshopItem.IsInstalled}, " + $"Downloading={workshopItem.IsDownloading}, " + $"Pending={workshopItem.IsDownloadPending}");
				}
			}
			if (!flag)
			{
				yield break;
			}
			if (UnityEngine.Input.GetKey(KeyCode.Escape))
			{
				break;
			}
			yield return awaiter;
		}
		Debug.Log("Workshop updating cancelled");
	}

	public void InitPackagesMeta()
	{
		foreach (BaseModPackage package in packages)
		{
			try
			{
				if (package.Init())
				{
					mappedPackages[package.id] = package as ModPackage;
				}
				_loading?.Log(package.ToString());
			}
			catch (Exception ex)
			{
				package.willActivate = false;
				_loading?.Log("Mod " + package.title + "/" + package.id + " has failed to initialize, reason: " + ex.Message);
			}
		}
	}

	public void ActivatePackages()
	{
		BaseModManager.listChainLoad.Clear();
		ListPluginObject.Clear();
		foreach (ModPackage package in packages)
		{
			if ((disableMod && !package.builtin) || !package.IsValidVersion())
			{
				continue;
			}
			try
			{
				package.Activate();
				if (package.activated)
				{
					BaseModManager.listChainLoad.Add(package.dirInfo.FullName);
				}
			}
			catch (Exception ex)
			{
				_loading.Log("Failed to activate mod: " + package.title + ", reason: " + ex.Message);
			}
		}
		ModUtil.OnModsActivated();
		ModUtil.LoadTypeFallback();
	}

	public override void ParseExtra(DirectoryInfo dir, BaseModPackage package)
	{
		ModPackage modPackage = (ModPackage)package;
		switch (dir.Name)
		{
		case "TalkText":
			modPackage.ParseTalkText(dir);
			break;
		case "Map":
			if (!package.builtin)
			{
				modPackage.ParseMap(dir);
			}
			break;
		case "Map Piece":
			if (!package.builtin)
			{
				modPackage.ParseMapPiece(dir);
			}
			break;
		case "Texture Replace":
			replaceFiles.AddRange(modPackage.ParseTextureReplace(dir));
			break;
		case "Texture":
			modPackage.ParseTexture(dir);
			break;
		case "Portrait":
			modPackage.ParsePortrait(dir);
			break;
		case "LangMod":
			modPackage.ParseLangMod(dir);
			break;
		case "Sound":
			modPackage.ParseSound(dir);
			break;
		case "Lang":
			modPackage.AddOrUpdateLang(dir);
			break;
		}
	}
}
