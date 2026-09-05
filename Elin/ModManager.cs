using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.API;
using IniParser.Model;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ModManager : ModManagerCore
{
	private sealed class WorkshopDownload
	{
		public readonly WorkshopItem item;

		public readonly BaseModPackage package;

		public readonly bool isUpdate;

		public float lastChange;

		public float progress;

		public string Title => package.title.IsEmpty(item.Title).IsEmpty(item.FileId.ToString());

		public WorkshopDownload(BaseModPackage package, float now)
		{
			this.package = package;
			item = (WorkshopItem)package.item;
			isUpdate = package.installed;
			lastChange = now;
		}
	}

	public static List<object> ListPluginObject = new List<object>();

	public static bool disableMod;

	private LoadingScreen _loading;

	public List<FileInfo> replaceFiles = new List<FileInfo>();

	private HashSet<ulong> _subscribedItems;

	private readonly HashSet<ulong> _blockedItems = new HashSet<ulong>();

	private int _missingContent;

	public static List<string> ListChainLoad => BaseModManager.listChainLoad;

	public static DirectoryInfo DirWorkshop => Instance.dirWorkshop;

	public static bool IsInitialized => BaseModManager.isInitialized;

	public new static ModManager Instance => (ModManager)BaseModManager.Instance;

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
		if ((!(dirWorkshop?.Exists)) ?? true)
		{
			dirWorkshop = null;
		}
		Debug.Log("Workshop:" + dirWorkshop);
		Debug.Log("Packages:" + BaseModManager.rootMod);
		Debug.Log("Core Mod:" + BaseModManager.rootDefaultPacakge);
	}

	public static string FormatLoadOrder(BaseModPackage p)
	{
		string arg = p.dirInfo?.FullName;
		int num = (p.willActivate ? 1 : 0);
		if (ModLoadOrderPreset.IsValidId(p.id))
		{
			return $"{arg},{num},{p.id.Trim()}";
		}
		return "";
	}

	public void SaveLoadOrder()
	{
		if (disableMod)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (BaseModPackage package in packages)
		{
			string str = package.dirInfo?.FullName;
			if (!package.builtin && !str.IsEmpty())
			{
				list.Add(FormatLoadOrder(package));
			}
		}
		File.WriteAllLines(CorePath.PathLoadOrder, list);
	}

	public void LoadLoadOrder()
	{
		foreach (BaseModPackage package in packages)
		{
			package.orderIndex = -1;
		}
		string pathLoadOrder = CorePath.PathLoadOrder;
		if (!File.Exists(pathLoadOrder))
		{
			return;
		}
		Dictionary<string, BaseModPackage> dictionary = new Dictionary<string, BaseModPackage>(StringComparer.OrdinalIgnoreCase);
		Dictionary<string, BaseModPackage> dictionary2 = new Dictionary<string, BaseModPackage>();
		foreach (BaseModPackage package2 in packages)
		{
			if (!package2.builtin)
			{
				string text = NormalizeOrderPath(package2.dirInfo?.FullName);
				if (!text.IsEmpty())
				{
					dictionary.TryAdd(text, package2);
				}
				string text2 = BaseModPackage.NormalizeId(package2.id);
				if (!text2.IsEmpty())
				{
					dictionary2.TryAdd(text2, package2);
				}
			}
		}
		int num = 0;
		HashSet<BaseModPackage> hashSet = new HashSet<BaseModPackage>();
		string[] array = File.ReadAllLines(pathLoadOrder);
		for (int i = 0; i < array.Length; i++)
		{
			if (ParseLoadOrderLine(array[i], out var path, out var activate, out var id))
			{
				if (!dictionary.TryGetValue(NormalizeOrderPath(path), out var value))
				{
					dictionary2.TryGetValue(BaseModPackage.NormalizeId(id) ?? "", out value);
				}
				if (value != null && hashSet.Add(value))
				{
					value.orderIndex = num;
					value.willActivate = activate;
					num++;
				}
			}
		}
		static bool ParseLoadOrderLine(string line, out string reference, out bool reference2, out string reference3)
		{
			reference = null;
			reference2 = true;
			reference3 = null;
			if (line.IsEmpty() || line.Trim().IsEmpty())
			{
				return false;
			}
			int num2 = line.LastIndexOf(',');
			if (num2 <= 0)
			{
				return false;
			}
			string text3 = line[(num2 + 1)..].Trim();
			if (text3 == "0" || text3 == "1")
			{
				reference = line[..num2];
				reference2 = text3 == "1";
				return !reference.IsEmpty();
			}
			int num3 = line.LastIndexOf(',', num2 - 1);
			if (num3 <= 0)
			{
				return false;
			}
			string text4 = line[(num3 + 1)..num2].Trim();
			if (text4 != "0" && text4 != "1")
			{
				return false;
			}
			reference = line[..num3];
			reference2 = text4 == "1";
			reference3 = text3;
			return !reference.IsEmpty();
		}
	}

	private static string NormalizeOrderPath(string path)
	{
		if (!path.IsEmpty())
		{
			return path.NormalizePath().TrimEnd('/');
		}
		return "";
	}

	public static string GetPresetPath(string name)
	{
		return CorePath.PathLoadOrderPreset + name.SanitizeFileName() + ".txt";
	}

	public List<FileInfo> ListPresets()
	{
		if (!Directory.Exists(CorePath.PathLoadOrderPreset))
		{
			return new List<FileInfo>();
		}
		return new DirectoryInfo(CorePath.PathLoadOrderPreset).GetFiles("*.txt", SearchOption.TopDirectoryOnly).OrderBy((FileInfo f) => f.Name, StringComparer.OrdinalIgnoreCase).ToList();
	}

	public bool SavePreset(string name, out string path)
	{
		path = GetPresetPath(name);
		ModLoadOrderPreset.Preset preset = ModLoadOrderPreset.Save(packages, name, EClass.core.version.GetText());
		if (disableMod || preset.entries.Count == 0)
		{
			return false;
		}
		Directory.CreateDirectory(CorePath.PathLoadOrderPreset);
		File.WriteAllText(path, ModLoadOrderPreset.Serialize(preset));
		return true;
	}

	public bool TryLoadPresetFile(FileInfo file, out ModLoadOrderPreset.Preset preset, out string error)
	{
		preset = null;
		string text;
		try
		{
			text = File.ReadAllText(file.FullName);
		}
		catch (Exception ex)
		{
			error = ex.Message;
			return false;
		}
		if (!ModLoadOrderPreset.TryParse(text, out preset, out error))
		{
			return false;
		}
		if (preset.name.IsEmpty())
		{
			preset.name = Path.GetFileNameWithoutExtension(file.Name);
		}
		return true;
	}

	public ModLoadOrderPreset.ApplyResult ApplyPreset(ModLoadOrderPreset.Preset preset, bool disableOthers = true)
	{
		ModLoadOrderPreset.ApplyResult result = ModLoadOrderPreset.Apply(packages, preset, disableOthers);
		SaveLoadOrder();
		return result;
	}

	public IEnumerator RefreshMods(Action onComplete, bool syncMods)
	{
		if ((bool)_loading)
		{
			UnityEngine.Object.Destroy(_loading.gameObject);
		}
		_loading = Util.Instantiate<LoadingScreen>("LoadingScreen");
		bool flag = !BaseCore.IsOffline && syncMods && UserGeneratedContent.Client.GetNumSubscribedItems() != 0;
		WaitForEndOfFrame awaiter = new WaitForEndOfFrame();
		packages.Clear();
		_subscribedItems = null;
		_blockedItems.Clear();
		_missingContent = 0;
		disableMod |= Application.isEditor && EClass.debug.skipMod;
		LoadLocalPackages();
		LoadCustomPackage();
		if (!disableMod && dirWorkshop != null)
		{
			_loading.Log("Loading workshop contents...");
			if (flag)
			{
				yield return LoadWorkshopPackages();
			}
			LoadCachedWorkshopPackages();
		}
		InitPackagesMeta();
		SortPackages();
		List<BaseModPackage> list = packages.Where((BaseModPackage p) => p.CanActivate && !p.parseError.IsEmpty()).ToList();
		if (list.Count > 0)
		{
			Halt(list);
			yield break;
		}
		_loading.Log($"Total number of mods:{packages.Count}");
		_loading.Log("Activating Mods...");
		yield return awaiter;
		ActivatePackages();
		MapActivatedPackages();
		BaseModManager.isInitialized = true;
		yield return awaiter;
		onComplete?.Invoke();
		if ((bool)_loading)
		{
			UnityEngine.Object.Destroy(_loading.gameObject);
		}
		yield return null;
	}

	private void Halt(List<BaseModPackage> broken)
	{
		List<string> list = broken.Select((BaseModPackage p) => p.title.IsEmpty(p.dirInfo?.Name) + ": package.xml does not parse - " + p.parseError).ToList();
		list.Add("Fix the package.xml, or unsubscribe / delete the mod, then restart the game.");
		string text = string.Join("\n", list);
		Debug.LogError("#mod game will not start:\n" + text);
		EGui.CreatePopup(() => new EGui.UpdateInfo(text), (EGui _) => false);
	}

	private void SortPackages()
	{
		LoadLoadOrder();
		List<BaseModPackage> list = ModOrder.SolveOrder(packages, useLoadorder: true);
		if (list.Count != packages.Count)
		{
			Debug.LogError($"#mod order {list.Count} of {packages.Count} packages");
			return;
		}
		packages.Clear();
		packages.AddRange(list);
	}

	private void MapActivatedPackages()
	{
		foreach (BaseModPackage item in packages.Where((BaseModPackage p) => p.activated && !p.id.IsEmpty()))
		{
			mappedPackages[item.id] = item as ModPackage;
		}
	}

	public ModPackage AddPackage(DirectoryInfo dir, bool isInPackages = false)
	{
		ModPackage modPackage = new ModPackage
		{
			dirInfo = new DirectoryInfo(dir.FullName.NormalizePath()),
			installed = true,
			isInPackages = isInPackages,
			Mapping = new FileMapping(dir)
		};
		packages.Add(modPackage);
		return modPackage;
	}

	public ModPackage AddWorkshopPackage(WorkshopItem item, bool isInPackages = false)
	{
		bool itemInstallInfo = UserGeneratedContent.Client.GetItemInstallInfo(item.FileId, out var _, out var folderPath, out var _);
		if (folderPath.IsEmpty())
		{
			folderPath = Path.Combine(DirWorkshop.FullName, item.FileId.ToString());
		}
		DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
		itemInstallInfo &= directoryInfo.Exists;
		ModPackage modPackage = AddPackage(directoryInfo, isInPackages);
		modPackage.installed = itemInstallInfo;
		modPackage.banned = item.IsBanned;
		modPackage.workshopId = item.FileId.ToString();
		modPackage.item = item;
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
			if (!disableMod || BaseModPackage.IsBuiltinDir(directoryInfo.Name))
			{
				if (directoryInfo.Name == "Mod_FixedPackageLoader")
				{
					IO.DeleteDirectory(directoryInfo.FullName);
				}
				else
				{
					AddPackage(directoryInfo, isInPackages: true);
				}
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

	public void InitPackagesMeta()
	{
		Text status = _loading.Log("Initializing mods...");
		HashSet<BaseModPackage> hashSet = new HashSet<BaseModPackage>();
		int num = 0;
		foreach (BaseModPackage package in packages)
		{
			num++;
			try
			{
				if (!package.Init())
				{
					hashSet.Add(package);
					Debug.LogWarning("Not a package: " + package.dirInfo.FullName);
					continue;
				}
				if (package.parseError.IsEmpty() && package.id.IsEmpty())
				{
					hashSet.Add(package);
					Debug.LogWarning("No <id> in package.xml, skipped: " + package.dirInfo.FullName);
					continue;
				}
				if (!package.id.IsEmpty())
				{
					mappedPackages[package.id] = package as ModPackage;
				}
				Debug.Log(package.ToString());
				SetStatus(status, $"Initializing mods {num}/{packages.Count}: {package.title}");
			}
			catch (Exception ex)
			{
				_loading.Log("Mod " + package.title + "/" + package.id + " has failed to initialize, " + ex.Message);
				Debug.LogError(ex);
			}
		}
		if (hashSet.Count > 0)
		{
			packages.RemoveAll(hashSet.Contains);
			_loading.Log($"Ignored {hashSet.Count} folder(s) without a usable package.xml");
		}
		SetStatus(status, $"Initialized {packages.Count} mods");
	}

	public void ActivatePackages()
	{
		BaseModManager.listChainLoad.Clear();
		ListPluginObject.Clear();
		ActivatePackageList();
		ModUtil.OnModsActivated();
		ModUtil.LoadTypeFallback();
	}

	public void ActivatePackageList()
	{
		Dictionary<string, BaseModPackage> loaded = new Dictionary<string, BaseModPackage>();
		Dictionary<string, BaseModPackage> claimed = new Dictionary<string, BaseModPackage>();
		int num = 0;
		foreach (BaseModPackage package in packages)
		{
			package.blockedBy = null;
			package.langDepError = null;
		}
		Dictionary<string, BaseModPackage> dictionary = new Dictionary<string, BaseModPackage>();
		foreach (BaseModPackage package2 in packages)
		{
			string text = BaseModPackage.NormalizeId(package2.id);
			if (text != null && !package2.hasPublishedPackage)
			{
				dictionary.TryAdd(text, package2);
			}
		}
		foreach (ModPackage item in from p in packages.OfType<ModPackage>()
			where !disableMod || p.builtin
			select p)
		{
			if (!item.IsValidVersion())
			{
				_loading?.Log("Skipped (made for game " + item.version + "): " + item.title + "/" + item.id);
			}
			else
			{
				if (item.banned)
				{
					continue;
				}
				if (item.CanActivate)
				{
					BaseModPackage baseModPackage = FindIncompatible(item, loaded, claimed);
					if (baseModPackage != null)
					{
						item.blockedBy = baseModPackage;
						num++;
						_loading?.Log("Blocked: " + item.title + "/" + item.id + " <-> " + baseModPackage.title + "/" + baseModPackage.id);
						continue;
					}
					string text2 = UnmetDependency(item, loaded, dictionary);
					if (text2 != null)
					{
						item.langDepError = text2;
						_loading?.Log("Skipped (" + text2 + "): " + item.title + "/" + item.id);
						continue;
					}
				}
				try
				{
					item.Activate();
				}
				catch (Exception ex)
				{
					_loading?.Log("Failed to activate mod " + item.title + ": " + ex.Message);
				}
				finally
				{
					if (item.activated)
					{
						BaseModManager.listChainLoad.Add(item.dirInfo.FullName);
						RegisterLoaded(item, loaded, claimed);
					}
				}
			}
		}
		if (num > 0)
		{
			Debug.LogWarning($"#mod {num} package(s) skipped due to incompatibility");
		}
	}

	private string UnmetDependency(BaseModPackage p, Dictionary<string, BaseModPackage> loaded, Dictionary<string, BaseModPackage> installed)
	{
		if (p.dependency == null)
		{
			return null;
		}
		string[][] dependency = p.dependency;
		foreach (string[] array in dependency)
		{
			if (array == null || array.Length == 0)
			{
				continue;
			}
			BaseModPackage baseModPackage = null;
			BaseModPackage baseModPackage2 = null;
			bool flag = false;
			string[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				string text = BaseModPackage.NormalizeId(array2[j]);
				if (text == null)
				{
					continue;
				}
				if (loaded.ContainsKey(text))
				{
					flag = true;
					break;
				}
				if (!installed.TryGetValue(text, out var value) || value == p)
				{
					continue;
				}
				if (packages.IndexOf(value) > packages.IndexOf(p))
				{
					if (baseModPackage == null)
					{
						baseModPackage = value;
					}
				}
				else if (baseModPackage2 == null)
				{
					baseModPackage2 = value;
				}
			}
			if (!flag)
			{
				if (baseModPackage != null)
				{
					return "requires " + baseModPackage.title.IsEmpty(baseModPackage.dirInfo?.Name) + ", which loads after this mod - move it above";
				}
				if (baseModPackage2 != null)
				{
					return "requires " + baseModPackage2.title.IsEmpty(baseModPackage2.dirInfo?.Name) + ", which is installed but did not load";
				}
				return "requires " + string.Join(" or ", array) + ", which is not installed";
			}
		}
		return null;
	}

	private static BaseModPackage FindIncompatible(BaseModPackage p, Dictionary<string, BaseModPackage> loaded, Dictionary<string, BaseModPackage> claimed)
	{
		if (p.builtin)
		{
			return null;
		}
		string text = BaseModPackage.NormalizeId(p.id);
		if (text == null)
		{
			return null;
		}
		string[][] incompatible = p.incompatible;
		if (incompatible != null)
		{
			string[][] array = incompatible;
			foreach (string[] array2 in array)
			{
				if (array2 == null)
				{
					continue;
				}
				string[] array3 = array2;
				for (int j = 0; j < array3.Length; j++)
				{
					string text2 = BaseModPackage.NormalizeId(array3[j]);
					if (text2 == null)
					{
						continue;
					}
					BaseModPackage value;
					if (text2 == text)
					{
						Debug.Log("#mod incompatible: '" + p.id + "' self decl, ignored");
					}
					else if (loaded.TryGetValue(text2, out value) && value != null)
					{
						if (!value.builtin)
						{
							return value;
						}
						Debug.Log("#mod incompatible: '" + p.id + "' targets builtin '" + value.id + "', ignored");
					}
				}
			}
		}
		if (claimed.TryGetValue(text, out var value2) && value2 != null && !value2.builtin)
		{
			return value2;
		}
		return null;
	}

	private static void RegisterLoaded(BaseModPackage p, Dictionary<string, BaseModPackage> loaded, Dictionary<string, BaseModPackage> claimed)
	{
		if (p.builtin)
		{
			return;
		}
		string text = BaseModPackage.NormalizeId(p.id);
		if (text == null)
		{
			return;
		}
		loaded[text] = p;
		string[][] incompatible = p.incompatible;
		if (incompatible == null)
		{
			return;
		}
		string[][] array = incompatible;
		foreach (string[] array2 in array)
		{
			if (array2 == null)
			{
				continue;
			}
			string[] array3 = array2;
			for (int j = 0; j < array3.Length; j++)
			{
				string text2 = BaseModPackage.NormalizeId(array3[j]);
				if (text2 != null && !(text2 == text))
				{
					claimed.TryAdd(text2, p);
				}
			}
		}
	}

	public IEnumerator LoadWorkshopPackages()
	{
		PublishedFileId_t[] subscribedItems = UserGeneratedContent.Client.GetSubscribedItems();
		if (!subscribedItems.IsEmpty())
		{
			_subscribedItems = subscribedItems.Select((PublishedFileId_t id) => id.m_PublishedFileId).ToHashSet();
			yield return QuerySubscriptions(subscribedItems);
			yield return DownloadSubscriptions();
			if (_missingContent > 0)
			{
				yield return new WaitForSecondsRealtime(3f);
			}
		}
	}

	private IEnumerator QuerySubscriptions(PublishedFileId_t[] subscribed)
	{
		WaitForEndOfFrame awaiter = new WaitForEndOfFrame();
		Text status = _loading.Log("Fetching subscriptions...");
		Dictionary<ulong, WorkshopItem> fetched = new Dictionary<ulong, WorkshopItem>();
		bool aborted = false;
		for (int i = 0; i < subscribed.Length; i += 50)
		{
			PublishedFileId_t[] array = subscribed.Skip(i).Take(50).ToArray();
			UgcQuery query = UgcQuery.Get(array);
			bool completed = false;
			bool discard = false;
			SetStatus(status, $"Fetching subscriptions {i + array.Length}/{subscribed.Length}...(Hit ESC to cancel)");
			if (query == null || !query.Execute(OnQueryCompleted))
			{
				Debug.LogWarning("Workshop: steam rejected the subscription query");
				query?.Dispose();
				aborted = true;
				break;
			}
			float deadline = Time.realtimeSinceStartup + 30f;
			while (!completed)
			{
				if (IsCancelRequested() || Time.realtimeSinceStartup > deadline)
				{
					discard = true;
					aborted = true;
					query.Dispose();
					break;
				}
				yield return awaiter;
			}
			if (aborted)
			{
				Debug.LogWarning("Workshop: subscription query cancelled or timed out");
				break;
			}
			void OnQueryCompleted(UgcQuery result)
			{
				completed = true;
				if (discard)
				{
					return;
				}
				foreach (WorkshopItem results in result.ResultsList)
				{
					SteamUGCDetails_t sourceItemDetails = results.SourceItemDetails;
					if (sourceItemDetails.m_eResult == EResult.k_EResultOK && sourceItemDetails.m_nPublishedFileId.m_PublishedFileId != 0L)
					{
						fetched[sourceItemDetails.m_nPublishedFileId.m_PublishedFileId] = results;
					}
				}
			}
		}
		foreach (WorkshopItem value in fetched.Values)
		{
			if (value.IsBanned)
			{
				_blockedItems.Add(value.FileId.m_PublishedFileId);
				_missingContent++;
			}
			else
			{
				AddWorkshopPackage(value);
			}
		}
		SetStatus(status, $"Fetched {fetched.Count}/{subscribed.Length} subscriptions");
		if (aborted)
		{
			yield break;
		}
		for (int j = 0; j < subscribed.Length; j++)
		{
			PublishedFileId_t publishedFileId_t = subscribed[j];
			if (!fetched.ContainsKey(publishedFileId_t.m_PublishedFileId) && !Directory.Exists(Path.Combine(dirWorkshop.FullName, publishedFileId_t.ToString())))
			{
				_missingContent++;
			}
		}
	}

	private IEnumerator DownloadSubscriptions()
	{
		float now = Time.realtimeSinceStartup;
		List<WorkshopDownload> queue = (from p in packages
			where p.item is WorkshopItem workshopItem && (!p.installed || workshopItem.IsNeedsUpdate)
			select new WorkshopDownload(p, now)).ToList();
		if (queue.Count == 0)
		{
			yield break;
		}
		WaitForEndOfFrame awaiter = new WaitForEndOfFrame();
		Text status = _loading.Log($"Downloading {queue.Count} mods...");
		Dictionary<ulong, EResult> results = new Dictionary<ulong, EResult>();
		int total = queue.Count;
		int completed = 0;
		float lastActivity = now;
		float nextStatus = 0f;
		UserGeneratedContent.Client.EventItemDownloaded.AddListener(OnItemDownloaded);
		try
		{
			while (queue.Count > 0)
			{
				now = Time.realtimeSinceStartup;
				for (int num = queue.Count - 1; num >= 0; num--)
				{
					WorkshopDownload workshopDownload = queue[num];
					WorkshopItem item = workshopDownload.item;
					ulong publishedFileId = item.FileId.m_PublishedFileId;
					string path = null;
					string reason = null;
					bool flag = false;
					EResult value;
					if (!workshopDownload.package.downloadStarted)
					{
						workshopDownload.package.downloadStarted = true;
						workshopDownload.lastChange = now;
						if (item.DownloadItem(highPriority: true))
						{
							Debug.Log("Start downloading: " + workshopDownload.Title + " | " + $"Installed={item.IsInstalled}, " + $"Update={item.IsNeedsUpdate}, " + $"Downloading={item.IsDownloading}, " + $"Pending={item.IsDownloadPending}");
							continue;
						}
						reason = "cannot start the download";
					}
					else if (results.TryGetValue(publishedFileId, out value) && value != EResult.k_EResultOK)
					{
						reason = $"download failed ({value})";
					}
					else if (IsItemReady(item, out path))
					{
						flag = true;
					}
					else
					{
						if (item.IsDownloadPending)
						{
							workshopDownload.lastChange = now;
							continue;
						}
						if (item.IsDownloading)
						{
							float downloadCompletion = item.DownloadCompletion;
							if (downloadCompletion > workshopDownload.progress)
							{
								workshopDownload.progress = downloadCompletion;
								float lastChange;
								lastActivity = (lastChange = now);
								workshopDownload.lastChange = lastChange;
							}
							continue;
						}
						if (!(now - workshopDownload.lastChange > 20f))
						{
							continue;
						}
						reason = "cannot start the download";
					}
					queue.RemoveAt(num);
					lastActivity = now;
					if (flag)
					{
						completed++;
						InstallWorkshopPackage(workshopDownload, path);
					}
					else
					{
						FailWorkshopPackage(workshopDownload, reason);
					}
				}
				if (queue.Count == 0)
				{
					break;
				}
				if (IsCancelRequested())
				{
					AbortDownloads(queue, "cancelled");
					break;
				}
				if (now - lastActivity > 90f)
				{
					AbortDownloads(queue, "not responding");
					break;
				}
				if (now >= nextStatus)
				{
					nextStatus = now + 0.5f;
					WorkshopDownload workshopDownload2 = queue.Find((WorkshopDownload d) => d.item.IsDownloading) ?? queue[0];
					SetStatus(status, $"Downloading mods {completed}/{total}: " + $"{workshopDownload2.Title} {Mathf.FloorToInt(workshopDownload2.progress * 100f)}%...(Hit ESC to cancel)");
				}
				yield return awaiter;
			}
		}
		finally
		{
			UserGeneratedContent.Client.EventItemDownloaded.RemoveListener(OnItemDownloaded);
		}
		SetStatus(status, $"Downloaded {completed}/{total} mods");
		packages.RemoveAll((BaseModPackage p) => p.item is WorkshopItem && !p.installed);
		void OnItemDownloaded(DownloadItemResult_t result)
		{
			results[result.m_nPublishedFileId.m_PublishedFileId] = result.m_eResult;
		}
	}

	public void LoadCachedWorkshopPackages()
	{
		HashSet<string> hashSet = packages.Select((BaseModPackage p) => p.dirInfo.FullName).ToHashSet(StringComparer.OrdinalIgnoreCase);
		DirectoryInfo[] directories = dirWorkshop.GetDirectories();
		foreach (DirectoryInfo directoryInfo in directories)
		{
			ulong result;
			bool flag = ulong.TryParse(directoryInfo.Name, out result);
			if ((!flag || !_blockedItems.Contains(result)) && (_subscribedItems == null || (flag && _subscribedItems.Contains(result))) && !hashSet.Contains(new DirectoryInfo(directoryInfo.FullName.NormalizePath()).FullName))
			{
				ModPackage modPackage = AddPackage(directoryInfo);
				if (flag)
				{
					modPackage.workshopId = directoryInfo.Name;
				}
			}
		}
	}

	private void InstallWorkshopPackage(WorkshopDownload download, string path)
	{
		BaseModPackage package = download.package;
		package.installed = true;
		package.dirInfo = new DirectoryInfo(path.NormalizePath());
		if (package is ModPackage modPackage)
		{
			modPackage.Mapping = new FileMapping(package.dirInfo);
		}
		Debug.Log("Downloaded: " + download.Title + " -> " + package.dirInfo.FullName);
	}

	private void FailWorkshopPackage(WorkshopDownload download, string reason)
	{
		if (download.isUpdate)
		{
			Debug.LogWarning("Workshop update failed: " + download.Title + ", " + reason);
			return;
		}
		_missingContent++;
		Debug.LogWarning("Workshop download failed: " + download.Title + ", " + reason);
	}

	private void AbortDownloads(List<WorkshopDownload> queue, string reason)
	{
		foreach (WorkshopDownload item in queue)
		{
			FailWorkshopPackage(item, reason);
		}
		queue.Clear();
	}

	private static bool IsItemReady(WorkshopItem item, out string path)
	{
		path = null;
		if (!item.IsInstalled || item.IsNeedsUpdate || item.IsDownloading || item.IsDownloadPending)
		{
			return false;
		}
		if (UserGeneratedContent.Client.GetItemInstallInfo(item.FileId, out var _, out path, out var _) && !path.IsEmpty())
		{
			return Directory.Exists(path);
		}
		return false;
	}

	private static bool IsCancelRequested()
	{
		return UnityEngine.Input.GetKey(KeyCode.Escape);
	}

	private static void SetStatus(Text status, string message)
	{
		if ((bool)status)
		{
			status.text = message;
		}
	}

	public override void ParseExtra(DirectoryInfo dir, BaseModPackage package)
	{
		ModPackage modPackage = (ModPackage)package;
		switch (dir.Name.ToLower())
		{
		case "talktext":
			modPackage.ParseTalkText(dir);
			break;
		case "map":
			if (!package.builtin)
			{
				modPackage.ParseMap(dir);
			}
			break;
		case "map piece":
			if (!package.builtin)
			{
				modPackage.ParseMapPiece(dir);
			}
			break;
		case "texture replace":
			replaceFiles.AddRange(modPackage.ParseTextureReplace(dir));
			break;
		case "texture":
			modPackage.ParseTexture(dir);
			break;
		case "portrait":
			modPackage.ParsePortrait(dir);
			break;
		case "langmod":
			modPackage.ParseLangMod(dir);
			break;
		case "sound":
			modPackage.ParseSound(dir);
			break;
		case "lang":
			modPackage.AddOrUpdateLang(dir);
			break;
		}
	}
}
