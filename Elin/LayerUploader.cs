using System;
using System.Collections.Generic;
using IniParser.Model;
using UnityEngine;
using UnityEngine.UI;

public class LayerUploader : ELayer
{
	public static int nextUpload;

	public static char[] InvalidChars = new char[21]
	{
		'*', '&', '|', '#', '\\', '/', '?', '!', '"', '>',
		'<', ':', ';', '.', ',', '~', '@', '^', '$', '%',
		' '
	};

	public InputField inputId;

	public InputField inputPassword;

	public InputField inputWelcome;

	public IniData ini;

	public UIText textInvalidId;

	public UIText textInvalidPass;

	public UIText textNextUpload;

	public UIButton buttonUpload;

	public UIButton buttonSave;

	public UIButton toggleClearLocalCharas;

	public int limitSec;

	public HashSet<string> invalidIds = new HashSet<string>();

	[NonSerialized]
	public bool validId;

	[NonSerialized]
	public bool validPass;

	private string savePath => CorePath.ZoneSaveUser + inputId.text + ".z";

	public override void OnInit()
	{
		ini = Core.GetElinIni();
		string text = ini.GetKey("pass") ?? "password";
		inputId.text = ELayer._map.custom?.id ?? "new_zone";
		inputPassword.text = text;
		if (ELayer._map.exportSetting == null)
		{
			ELayer._map.exportSetting = new MapExportSetting();
		}
		MapExportSetting ex = ELayer._map.exportSetting;
		inputWelcome.text = ex.textWelcome.IsEmpty("");
		inputWelcome.onValueChanged.AddListener(delegate(string s)
		{
			ex.textWelcome = s;
		});
		toggleClearLocalCharas.SetToggle(ex.clearLocalCharas, delegate(bool on)
		{
			ex.clearLocalCharas = on;
		});
	}

	private void Update()
	{
		string text = inputId.text;
		validId = text.Length >= 3 && text.IndexOfAny(InvalidChars) == -1 && !invalidIds.Contains(text);
		text = inputPassword.text;
		validPass = text.Length >= 3 && text.IndexOfAny(InvalidChars) == -1;
		textInvalidId.SetActive(!validId);
		textInvalidPass.SetActive(!validPass);
		bool interactableWithAlpha = validId && validPass;
		buttonSave.SetInteractableWithAlpha(interactableWithAlpha);
		int num = nextUpload - (int)Time.realtimeSinceStartup;
		textNextUpload.SetActive(num > 0);
		if (num > 0)
		{
			textNextUpload.text = "net_nextUpload".lang(num.ToString() ?? "");
			if (!ELayer.debug.enable)
			{
				interactableWithAlpha = false;
			}
		}
		buttonUpload.SetInteractableWithAlpha(interactableWithAlpha);
	}

	public override void OnKill()
	{
		if (validId && validPass)
		{
			SaveID();
		}
	}

	public void SaveID()
	{
		if (ELayer._map.custom != null)
		{
			ELayer._map.custom.id = inputId.text;
		}
		ini.Global["pass"] = inputPassword.text;
		Core.SaveElinIni(ini);
	}

	public void ExportMap()
	{
		ELayer._zone.Export(savePath, null, usermap: true);
		Msg.Say("net_mapSaved".lang(savePath));
		SE.WriteJournal();
	}

	public void Upload()
	{
		if (ini.Global["agreed_usercontents_upload_terms"] != "yes")
		{
			string[] items = new string[3] { "readTerms", "agree", "disagree" };
			Dialog.List("dialogTermsOfUse".lang(), items, (string j) => j, delegate(int c, string d)
			{
				switch (c)
				{
				case 0:
					LayerHelp.Toggle("custom", "terms");
					return false;
				case 1:
					ini.Global["agreed_usercontents_upload_terms"] = "yes";
					Upload();
					break;
				}
				return true;
			}, canCancel: true);
		}
		else
		{
			Debug.Log("Uploading map");
			string text = inputId.text;
			string text2 = inputPassword.text;
			SaveID();
			ExportMap();
			Net.UploadFile(text, text2, ELayer.pc.NameBraced, ELayer._zone.Name, savePath, Lang.langCode);
			nextUpload = (int)Time.realtimeSinceStartup + limitSec;
		}
	}
}
