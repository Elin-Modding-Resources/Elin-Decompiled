using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using AeLa.EasyFeedback;
using AeLa.EasyFeedback.FormElements;
using AeLa.EasyFeedback.FormFields;
using Ionic.Zip;
using UnityEngine;
using UnityEngine.UI;

public class LayerFeedback : ELayer
{
	public static string lastSummary = "";

	public static string lastDetail = "";

	public static string steamName = "";

	public static string userName = "";

	public static string backerId = "";

	public static string header = "";

	public static int playedHours;

	public UIText textProgress;

	public InputField inputVersion;

	public InputField inputName;

	public InputField inputEmail;

	public InputField inputLang;

	public InputField inputSummary;

	public InputField inputDetail;

	public FeedbackForm form;

	public DebugLogCollector collector;

	public Transform transUpload;

	public UIDropdown ddSave;

	public Toggle toggleSave;

	public Toggle toggleScreenshot;

	public GameIndex saveIndex;

	public CanvasGroup cgForm;

	public CategoryDropdown category;

	public Button buttonSubmit;

	public UIButton buttonLinkFAQ;

	public UIButton buttonLinkDiscord;

	public override void OnInit()
	{
		inputVersion.text = ELayer.core.version.GetText() + "/" + ELayer.config.lang + " " + steamName + "/" + userName + " hours:" + playedHours + " backer:" + backerId;
		TextField.onAddEmail = delegate
		{
			string s2 = "";
			foreach (BaseModPackage package in ELayer.core.mods.packages)
			{
				if (!package.builtin)
				{
					Append(package.id + "/" + package.activated);
				}
			}
			Append("");
			Append("Save Path: " + CorePath.RootSave);
			Append("EXE Path: " + CorePath.rootExe);
			Append("OS: " + SystemInfo.operatingSystem);
			Append("Processor: " + SystemInfo.processorType);
			Append("Memory: " + SystemInfo.systemMemorySize);
			Append("Graphics API: " + SystemInfo.graphicsDeviceType);
			Append("Graphics Processor: " + SystemInfo.graphicsDeviceName);
			Append("Graphics Memory: " + SystemInfo.graphicsMemorySize);
			Append("Graphics Vendor: " + SystemInfo.graphicsDeviceVendor);
			Append("Quality Level: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
			Append("Resolution: " + Screen.width + "x" + Screen.height);
			Append("Full Screen: " + Screen.fullScreen);
			Append("regionName: " + RegionInfo.CurrentRegion.Name);
			Append("cultureName: " + CultureInfo.CurrentCulture.Name);
			Append("cname: " + Thread.CurrentThread.CurrentCulture.Name);
			Append("uiname: " + Thread.CurrentThread.CurrentUICulture.Name);
			Append("device identifier: " + SystemInfo.deviceUniqueIdentifier);
			return Environment.NewLine + Environment.NewLine + s2;
			void Append(string text)
			{
				if (text != null)
				{
					s2 = s2 + text + Environment.NewLine;
				}
			}
		};
		buttonLinkFAQ.SetOnClick(delegate
		{
			LayerHelp.Toggle("general", "FAQ");
		});
		buttonLinkDiscord.SetOnClick(delegate
		{
			SE.Click();
			Application.OpenURL("https://discord.gg/elona");
		});
		transUpload.SetActive(enable: false);
		buttonSubmit.interactable = false;
		if (!ELayer.config.nameReport.IsEmpty())
		{
			inputName.text = ELayer.config.nameReport;
		}
		else if (ELayer.core.IsGameStarted)
		{
			inputName.text = "nameBraced".lang(ELayer.pc.NameSimple, ELayer.pc.Aka);
		}
		if (!ELayer.config.emailReport.IsEmpty())
		{
			inputEmail.text = ELayer.config.emailReport;
		}
		inputLang.text = ELayer.config.lang;
		inputDetail.text = lastDetail;
		inputSummary.text = lastSummary;
		collector.onSubmit = delegate
		{
			CollectFiles();
		};
		category.Init((string s) => ("form_" + s).lang());
		List<GameIndex> gameList = GameIO.GetGameList((ELayer.core.config.cloud || (ELayer.core.IsGameStarted && ELayer.game.isCloud)) ? CorePath.RootSaveCloud : CorePath.RootSave);
		if (gameList.Count > 0)
		{
			int index = 0;
			saveIndex = gameList[0];
			for (int i = 0; i < gameList.Count; i++)
			{
				if (ELayer.core.IsGameStarted && gameList[i].id == Game.id)
				{
					index = i;
					saveIndex = gameList[i];
				}
			}
			ddSave.SetList(index, gameList, (GameIndex a, int b) => ((ELayer.core.IsGameStarted && a.id == Game.id) ? "currentSave".lang() : "") + a.FormTitle, delegate(int a, GameIndex b)
			{
				saveIndex = b;
			});
		}
		else
		{
			toggleSave.SetActive(enable: false);
		}
		toggleSave.onValueChanged.AddListener(delegate(bool a)
		{
			ddSave.SetActive(a);
		});
		toggleSave.isOn = false;
		ddSave.SetActive(toggleSave.isOn);
		form.Init();
		inputDetail.onValueChanged.AddListener(delegate
		{
			Validate();
		});
		inputSummary.onValueChanged.AddListener(delegate
		{
			Validate();
		});
		Validate();
	}

	public void Validate()
	{
		bool interactable = true;
		if (inputSummary.text.Length < 4)
		{
			interactable = false;
		}
		if (inputDetail.text.Length < 4)
		{
			interactable = false;
		}
		buttonSubmit.interactable = interactable;
	}

	public void CollectFiles()
	{
		ReportTitle.ignore = false;
		ReportTitle.strAdd = "";
		TextField.strAddText = "";
		string text = Application.persistentDataPath + "/";
		string text2 = text + "_temp";
		string text3 = text2 + "/log.zip";
		IO.CreateDirectory(text2);
		IO.Copy(text + "Player.log", text2);
		IO.Copy(text + "Player-prev.log", text2);
		IO.SaveText(text2 + "/System.txt", GetSystemText());
		int errors = 0;
		ParseLog(text2 + "/Player.log");
		ParseLog(text2 + "/Player-prev.log");
		ReportTitle.strAdd = ((errors == 0) ? "" : ("(" + errors + ")"));
		if (ELayer.debug.enable)
		{
			ReportTitle.strAdd += "(debug)";
		}
		string text4 = inputDetail.text;
		int num = 0;
		string[] array = new string[23]
		{
			"シーラカンス", "ナーフ", "つまらない", "面白くない", "不合理", "やめて", "無駄", "弱体化", "不親切", "クソ",
			"糞", "バカ", "馬鹿", "coelacanth", "nerf", "boring", "please don't", "waste", "fuck", "suck",
			"tbh", "shit", "stupid"
		};
		foreach (string c2 in array)
		{
			num += CountString(text4, c2);
		}
		ReportTitle.strAdd = header + ReportTitle.strAdd;
		if (num > 0)
		{
			ReportTitle.strAdd = ReportTitle.strAdd + "[ignore:" + num + "]";
			ReportTitle.strAdd = ReportTitle.strAdd + " " + steamName + "/" + userName + "/" + backerId;
			ReportTitle.ignore = true;
		}
		using (ZipFile zipFile = new ZipFile())
		{
			zipFile.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
			zipFile.AddDirectory(text2);
			zipFile.Save(text3);
		}
		form.CurrentReport.AttachFile("log.zip", File.ReadAllBytes(text3));
		if (toggleSave.isOn)
		{
			bool flag = ELayer.core.config.cloud || (ELayer.core.IsGameStarted && ELayer.game.isCloud);
			string text5 = text2 + "/save.zip";
			using (ZipFile zipFile2 = new ZipFile())
			{
				zipFile2.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
				zipFile2.AddDirectory((flag ? CorePath.RootSaveCloud : CorePath.RootSave) + saveIndex.id);
				zipFile2.Save(text5);
			}
			form.CurrentReport.AttachFile(saveIndex.id + ".zip", File.ReadAllBytes(text5));
		}
		IO.DeleteDirectory(text2);
		static int CountString(string s, string c)
		{
			string newValue = c.Substring(0, c.Length - 1);
			return s.Length - s.Replace(c, newValue).Length;
		}
		void ParseLog(string path)
		{
			if (File.Exists(path))
			{
				string[] array2 = IO.LoadTextArray(path);
				foreach (string text6 in array2)
				{
					switch (text6)
					{
					case "InvalidOperationException: Collection was modified; enumeration operation may not execute.":
					case "NullReferenceException: Object reference not set to an instance of an object":
						errors++;
						break;
					default:
						if (text6.Contains("exception"))
						{
							errors++;
						}
						else if (text6.Contains("Exception"))
						{
							errors++;
						}
						break;
					case "InvalidOperationException: Steamworks is not initialized.":
						break;
					}
				}
			}
		}
	}

	public void OnCancel()
	{
		lastDetail = inputDetail.text;
		lastSummary = inputSummary.text;
		form.Clean();
		Close();
	}

	public void Submit()
	{
		int num = 0;
		foreach (BaseModPackage package in ELayer.core.mods.packages)
		{
			if (!package.builtin && package.activated)
			{
				num++;
			}
		}
		if (num > 0)
		{
			Dialog.YesNo("warn_mod".lang(num.ToString() ?? ""), delegate
			{
			}, delegate
			{
				form.Submit();
			}, "warn_mod_yes", "warn_mod_no");
		}
		else
		{
			form.Submit();
		}
	}

	public void OnSubmit()
	{
		if (!inputEmail.text.IsEmpty())
		{
			ELayer.config.emailReport = inputEmail.text;
		}
		if (!inputName.text.IsEmpty())
		{
			ELayer.config.nameReport = inputName.text;
		}
		transUpload.SetActive(enable: true);
		form.IncludeScreenshot = toggleScreenshot.isOn;
		cgForm.alpha = 0.5f;
		form.transform.SetParent(ELayer.ui.transform, worldPositionStays: false);
		Close();
		lastDetail = (lastSummary = "");
	}

	public string GetSystemText()
	{
		string txt = "";
		Append("OS: " + SystemInfo.operatingSystem);
		Append("Processor: " + SystemInfo.processorType);
		Append("Memory: " + SystemInfo.systemMemorySize);
		Append("Graphics API: " + SystemInfo.graphicsDeviceType);
		Append("Graphics Processor: " + SystemInfo.graphicsDeviceName);
		Append("Graphics Memory: " + SystemInfo.graphicsMemorySize);
		Append("Graphics Vendor: " + SystemInfo.graphicsDeviceVendor);
		Append("Quality Level: " + QualitySettings.names[QualitySettings.GetQualityLevel()]);
		Append("Resolution: " + Screen.width + "x" + Screen.height);
		Append("Full Screen: " + Screen.fullScreen);
		return txt;
		void Append(string s)
		{
			txt = txt + s + Environment.NewLine;
		}
	}
}
