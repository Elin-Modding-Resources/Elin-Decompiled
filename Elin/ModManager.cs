using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using HeathenEngineering.SteamworksIntegration;
using HeathenEngineering.SteamworksIntegration.API;
using IniParser.Model;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Steamworks;
using UnityEngine;

[Serializable]
public class ModManager : BaseModManager
{
	public struct SheetIndex
	{
		public int dest;

		public int old;
	}

	public static List<object> ListPluginObject = new List<object>();

	public static bool disableMod = false;

	public List<FileInfo> replaceFiles = new List<FileInfo>();

	public static List<string> ListChainLoad => BaseModManager.listChainLoad;

	public static DirectoryInfo DirWorkshop => BaseModManager.Instance.dirWorkshop;

	public static bool IsInitialized => BaseModManager.isInitialized;

	public override void Init(string path, string defaultPackage = "_Elona")
	{
		base.Init(path, defaultPackage);
		IniData elinIni = Core.GetElinIni();
		Debug.Log("IsOffline:" + BaseCore.IsOffline);
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
				DirectoryInfo parent = new DirectoryInfo(App.Client.GetAppInstallDirectory(SteamSettings.behaviour.settings.applicationId)).Parent.Parent;
				dirWorkshop = new DirectoryInfo(parent.FullName + "/workshop/content/2135150");
				elinIni.Global["path_workshop"] = dirWorkshop.FullName ?? "";
				Core.SaveElinIni(elinIni);
			}
		}
		if (dirWorkshop != null && !dirWorkshop.Exists)
		{
			dirWorkshop = null;
		}
		Debug.Log("dirWorkshop:" + dirWorkshop);
		Debug.Log("Mod Init:" + BaseModManager.rootDefaultPacakge);
		packages.Clear();
		DirectoryInfo[] directories = new DirectoryInfo(BaseModManager.rootMod).GetDirectories();
		Array.Reverse(directories);
		DirectoryInfo[] array = directories;
		foreach (DirectoryInfo directoryInfo in array)
		{
			if (!EClass.debug.skipMod || !Application.isEditor || !(directoryInfo.Name != "_Elona"))
			{
				AddPackage(directoryInfo, isInPackages: true);
			}
		}
		DirectoryInfo[] directories2 = new DirectoryInfo(CorePath.custom).GetDirectories();
		ModPackage package = new ModPackage();
		array = directories2;
		foreach (DirectoryInfo dir in array)
		{
			ParseExtra(dir, package);
		}
	}

	private void HandleResults(UgcQuery query)
	{
		foreach (WorkshopItem results in query.ResultsList)
		{
			if (results.IsSubscribed)
			{
				AddWorkshopPackage(results);
			}
		}
	}

	public IEnumerator RefreshMods(Action onComplete, bool syncMods)
	{
		bool sync = !BaseCore.IsOffline && syncMods && UserGeneratedContent.Client.GetNumSubscribedItems() != 0;
		LoadingScreen loading = (sync ? Util.Instantiate<LoadingScreen>("LoadingScreen") : null);
		if (!disableMod && (!EClass.debug.skipMod || !Application.isEditor))
		{
			if (sync)
			{
				UgcQuery activeQuery = UgcQuery.GetSubscribed(withLongDescription: false, withMetadata: false, withKeyValueTags: false, withAdditionalPreviews: false, 0u);
				activeQuery.Execute(HandleResults);
				loading?.Log("Fetching subscribed Mods...(Hit ESC to cancel)");
				while (activeQuery.handle != UGCQueryHandle_t.Invalid && !UnityEngine.Input.GetKey(KeyCode.Escape))
				{
					yield return new WaitForEndOfFrame();
				}
			}
			else
			{
				loading?.Log("Fetching offline Mods.");
				if (dirWorkshop != null)
				{
					DirectoryInfo[] directories = dirWorkshop.GetDirectories();
					foreach (DirectoryInfo dir in directories)
					{
						AddPackage(dir);
					}
				}
			}
		}
		if (sync)
		{
			bool valid = false;
			while (!valid)
			{
				valid = true;
				foreach (BaseModPackage package in packages)
				{
					WorkshopItem workshopItem = package.item as WorkshopItem;
					if (!package.installed && workshopItem != null && !workshopItem.IsBanned)
					{
						valid = false;
						string text = "Downloading " + workshopItem.Title + ": ";
						if (!package.progressText)
						{
							package.progressText = loading.Log(text);
						}
						if (package.downloadStarted && workshopItem.DownloadCompletion >= 1f)
						{
							package.progressText.text = text + "Done!";
							package.installed = true;
						}
						else if (workshopItem.IsDownloading || workshopItem.IsDownloadPending)
						{
							package.progressText.text = text + (int)(workshopItem.DownloadCompletion * 100f) + "%";
						}
						else if (!package.downloadStarted)
						{
							package.downloadStarted = true;
							workshopItem.DownloadItem(highPriority: true);
							Debug.Log("start downloading:" + workshopItem.Title + "/" + workshopItem.IsInstalled + "/" + package.installed + "/" + workshopItem.IsDownloading + "/" + workshopItem.IsDownloadPending + "/" + workshopItem.DownloadCompletion);
						}
					}
				}
				if (!valid && UnityEngine.Input.GetKey(KeyCode.Escape))
				{
					break;
				}
				yield return new WaitForEndOfFrame();
			}
		}
		foreach (BaseModPackage package2 in packages)
		{
			package2.Init();
			Debug.Log(package2.title + "/" + package2.id + "/" + package2.installed + "/" + package2.dirInfo.FullName);
		}
		LoadLoadOrder();
		packages.Sort((BaseModPackage a, BaseModPackage b) => a.loadPriority - b.loadPriority);
		foreach (BaseModPackage package3 in packages)
		{
			if (package3.isInPackages || !package3.willActivate)
			{
				continue;
			}
			foreach (BaseModPackage package4 in packages)
			{
				if (package4.isInPackages && package3.id == package4.id)
				{
					package4.hasPublishedPackage = true;
				}
			}
		}
		loading?.Log("Total number of Mods:" + packages.Count);
		if ((bool)loading)
		{
			loading.Log("Activating Mods...");
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
		}
		BaseModManager.listChainLoad.Clear();
		ListPluginObject.Clear();
		foreach (BaseModPackage package5 in packages)
		{
			if (package5.IsValidVersion() && (!disableMod || package5.builtin))
			{
				package5.Activate();
				if (package5.activated)
				{
					BaseModManager.listChainLoad.Add(package5.dirInfo.FullName);
				}
			}
		}
		ModUtil.OnModsActivated();
		ModUtil.LoadTypeFallback();
		BaseModManager.isInitialized = true;
		yield return new WaitForEndOfFrame();
		onComplete?.Invoke();
		if ((bool)loading)
		{
			UnityEngine.Object.Destroy(loading.gameObject);
		}
		yield return null;
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
			if (!package.builtin && Directory.Exists(package.dirInfo.FullName))
			{
				string item = package.dirInfo.FullName + "," + (package.willActivate ? 1 : 0);
				list.Add(item);
			}
		}
		File.WriteAllLines(CorePath.rootExe + "loadorder.txt", list);
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
			if (dictionary.ContainsKey(array2[0]))
			{
				BaseModPackage baseModPackage = dictionary[array2[0]];
				baseModPackage.loadPriority = num;
				baseModPackage.willActivate = array2[1] == "1";
			}
			num++;
		}
	}

	public ModPackage AddPackage(DirectoryInfo dir, bool isInPackages = false)
	{
		ModPackage modPackage = new ModPackage
		{
			dirInfo = dir,
			installed = true
		};
		packages.Add(modPackage);
		modPackage.isInPackages = isInPackages;
		modPackage.loadPriority = priorityIndex;
		priorityIndex++;
		return modPackage;
	}

	public ModPackage AddWorkshopPackage(WorkshopItem item, bool isInPackages = false)
	{
		UserGeneratedContent.Client.GetItemInstallInfo(item.FileId, out var _, out var folderPath, out var _);
		DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
		ModPackage modPackage = new ModPackage
		{
			item = item,
			dirInfo = directoryInfo,
			installed = directoryInfo.Exists,
			banned = item.IsBanned
		};
		packages.Add(modPackage);
		modPackage.isInPackages = isInPackages;
		modPackage.loadPriority = priorityIndex;
		priorityIndex++;
		return modPackage;
	}

	public int CountUserMod()
	{
		int num = 0;
		foreach (BaseModPackage package in packages)
		{
			if (!package.builtin)
			{
				num++;
			}
		}
		return num;
	}

	public override void ParseExtra(DirectoryInfo dir, BaseModPackage package)
	{
		switch (dir.Name)
		{
		case "TalkText":
		{
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo fileInfo in files)
			{
				if (fileInfo.Name.EndsWith(".xlsx"))
				{
					TalkText.modList.Add(new ExcelData(fileInfo.FullName));
				}
			}
			break;
		}
		case "Map":
		{
			if (package.builtin)
			{
				break;
			}
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo fileInfo5 in files)
			{
				if (fileInfo5.Name.EndsWith(".z"))
				{
					MOD.listMaps.Add(fileInfo5);
				}
			}
			break;
		}
		case "Map Piece":
		{
			if (package.builtin)
			{
				break;
			}
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo fileInfo3 in files)
			{
				if (fileInfo3.Name.EndsWith(".mp"))
				{
					MOD.listPartialMaps.Add(fileInfo3);
				}
			}
			break;
		}
		case "Texture Replace":
		{
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo fileInfo6 in files)
			{
				if (fileInfo6.Name.EndsWith(".png"))
				{
					replaceFiles.Add(fileInfo6);
				}
			}
			break;
		}
		case "Texture":
		{
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo fileInfo4 in files)
			{
				if (fileInfo4.Name.EndsWith(".png"))
				{
					SpriteReplacer.dictModItems[fileInfo4.Name.Replace(".png", "")] = fileInfo4.GetFullFileNameWithoutExtension();
				}
			}
			break;
		}
		case "Portrait":
		{
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo fileInfo2 in files)
			{
				if (fileInfo2.Name.EndsWith(".png"))
				{
					if (fileInfo2.Name.StartsWith("BG_"))
					{
						Portrait.modPortraitBGs.Add(fileInfo2);
					}
					else if (fileInfo2.Name.StartsWith("BGF_"))
					{
						Portrait.modPortraitBGFs.Add(fileInfo2);
					}
					else if (fileInfo2.Name.EndsWith("-full.png"))
					{
						Portrait.modFull.Add(fileInfo2);
					}
					else if (fileInfo2.Name.EndsWith("-overlay.png"))
					{
						Portrait.modOverlays.Add(fileInfo2);
					}
					else
					{
						Portrait.modPortraits.Add(fileInfo2);
					}
					Portrait.allIds.Add(fileInfo2.Name);
				}
			}
			break;
		}
		case "Lang":
		{
			DirectoryInfo[] directories = dir.GetDirectories();
			foreach (DirectoryInfo directoryInfo in directories)
			{
				if (!directoryInfo.Name.StartsWith("_") && !TryAddLang(directoryInfo, isNew: false))
				{
					Debug.Log("Generating Language Mod Contents:" + directoryInfo.FullName);
					IO.CopyDir(CorePath.packageCore + "Lang/EN", directoryInfo.FullName);
					Directory.CreateDirectory(directoryInfo.FullName + "/Dialog");
					IO.CopyDir(CorePath.packageCore + "Lang/_Dialog", directoryInfo.FullName + "/Dialog");
					EClass.sources.ExportSourceTexts(directoryInfo.FullName + "/Game");
					IO.Copy(CorePath.packageCore + "Lang/lang.ini", directoryInfo.FullName + "/");
					TryAddLang(directoryInfo, isNew: true);
				}
			}
			break;
		}
		}
		bool TryAddLang(DirectoryInfo dirLang, bool isNew)
		{
			string name = dirLang.Name;
			FileInfo[] files2 = dirLang.GetFiles();
			foreach (FileInfo fileInfo7 in files2)
			{
				if (fileInfo7.Name == "lang.ini")
				{
					LangSetting langSetting = new LangSetting(fileInfo7.FullName)
					{
						id = name,
						dir = dirLang.FullName + "/"
					};
					if (isNew)
					{
						langSetting.SetVersion();
					}
					else if ((Application.isEditor || Lang.runUpdate) && !Lang.IsBuiltin(dirLang.Name) && langSetting.GetVersion() != EClass.core.version.GetInt())
					{
						EClass.sources.Init();
						Log.system = "Updated Language Files:" + Environment.NewLine + Environment.NewLine;
						Debug.Log("Updating Language:" + langSetting.name + "/" + langSetting.GetVersion() + "/" + EClass.core.version.GetInt());
						string text = dirLang.FullName + "/Game";
						Directory.Move(text, text + "_temp");
						EClass.sources.ExportSourceTexts(text);
						EClass.sources.UpdateSourceTexts(text);
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
