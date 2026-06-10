using System;
using System.IO;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class ContentConfigOther : ContentConfig
{
	public UIButton toggleNoCensor;

	public UIButton toggleRunBackground;

	public UIButton toggleMuteBackground;

	public UIButton toggleTest;

	public UIButton toggleAltInv;

	public UIButton toggleAltAbility;

	public UIButton buttonBackerCode;

	public UIButton buttonWallPaper;

	public UIButton toggleSyncMod;

	public UIButton toggleDisableMods;

	public UIButton toggleExceptionPopup;

	public UIDropdown ddSnap;

	public UIDropdown ddDivider;

	public Slider sliderMissClick;

	public Slider sliderBGMInterval;

	public UIText textBackerCode;

	public override void OnInstantiate()
	{
		ddSnap.SetList((int)base.config.fix.snapType, Util.EnumToList<CoreConfig.ScreenSnapType>(), (CoreConfig.ScreenSnapType a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.ScreenSnapType b)
		{
			base.config.fix.snapType = b;
		});
		ddDivider.SetList((int)base.config.fix.divider, Util.EnumToList<CameraSupport.Divider>(), (CameraSupport.Divider a, int b) => a.ToString().lang(), delegate(int a, CameraSupport.Divider b)
		{
			base.config.fix.divider = b;
			EClass.core.screen.RefreshScreenSize();
		});
		SetSlider(sliderMissClick, base.config.other.antiMissClick, delegate(float a)
		{
			base.config.other.antiMissClick = (int)a;
			EInput.antiMissClick = 0.1f * a;
			return Lang.Get("antiMissClick") + "(" + 0.1f * a + "s)";
		});
		SetSlider(sliderBGMInterval, base.config.other.bgmInterval, delegate(float a)
		{
			base.config.other.bgmInterval = (int)a;
			base.config.ApplyVolume();
			return Lang.Get("bgmInterval") + "(" + a * 5f + "sec)";
		});
		toggleSyncMod.SetToggle(base.config.other.syncMods, delegate(bool on)
		{
			base.config.other.syncMods = on;
		});
		toggleDisableMods.SetToggle(base.config.other.disableMods, delegate(bool on)
		{
			base.config.other.disableMods = on;
		});
		toggleExceptionPopup.SetToggle(base.config.other.exceptionPopup, delegate(bool on)
		{
			base.config.other.exceptionPopup = on;
		});
		toggleNoCensor.SetToggle(base.config.other.noCensor, delegate(bool on)
		{
			base.config.other.noCensor = on;
			base.config.Apply();
		});
		toggleRunBackground.SetToggle(base.config.other.runBackground, delegate(bool on)
		{
			base.config.other.runBackground = on;
			base.config.Apply();
		});
		toggleMuteBackground.SetToggle(base.config.other.muteBackground, delegate(bool on)
		{
			base.config.other.muteBackground = on;
			base.config.Apply();
		});
		toggleAltInv.SetToggle(base.config.game.altInv, delegate(bool on)
		{
			base.config.game.altInv = on;
			EClass.ui.layerFloat.RemoveLayers(removeImportant: true);
		});
		toggleAltAbility.SetToggle(base.config.game.altAbility, delegate(bool on)
		{
			base.config.game.altAbility = on;
			EClass.ui.layerFloat.RemoveLayers(removeImportant: true);
		});
		toggleTest.SetToggle(base.config.other.showTestOptions, delegate(bool on)
		{
			base.config.other.showTestOptions = on;
			EClass.ui.GetLayer<LayerConfig>().Close();
			EClass.ui.AddLayer<LayerConfig>();
		});
		buttonWallPaper.SetActive(base.config.HasBackerRewardCode());
		RefreshRewardCode();
	}

	public void OpenLog()
	{
		Util.ShowExplorer(CorePath.RootSave);
	}

	public void OoenConfigFolder()
	{
		Util.ShowExplorer(CorePath.RootSave + "config.txt");
	}

	public void OpenPackage()
	{
		Util.ShowExplorer(CorePath.rootMod + "_Elona");
	}

	public void OpenUser()
	{
		Util.ShowExplorer(CorePath.user + "PCC");
	}

	public void ResetWindows()
	{
		Dialog.YesNo("dialogResetWindow", delegate
		{
			Window.dictData.Clear();
		});
	}

	public void OpenBackerCodeInput()
	{
		Dialog.InputName("dialogBackerCode", "", delegate(bool cancel, string text)
		{
			if (!cancel)
			{
				if (ElinEncoder.IsValid(text))
				{
					base.config.rewardCode = text;
					SE.Change();
					EClass.ui.GetLayer<LayerConfig>().Close();
					EClass.ui.AddLayer<LayerConfig>();
				}
				else
				{
					Dialog.Ok("invalidRewardCode".lang(text));
				}
			}
		});
	}

	public void OnClickDownloadWallpaper()
	{
		EClass.core.WaitForEndOfFrame(delegate
		{
			string text = StandaloneFileBrowser.SaveFilePanel("Export Wallpaper", CorePath.RootData, "wallpaper", "zip");
			Debug.Log(text);
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					TextAsset textAsset = Resources.Load("Etc/wallpaper") as TextAsset;
					Debug.Log(textAsset);
					File.WriteAllBytes(text, textAsset.bytes);
					EClass.ui.Say("umimyaaThankyou");
					SE.Play("godbless");
				}
				catch (Exception ex)
				{
					EClass.ui.Say(ex.Message);
				}
			}
		});
	}

	public void RefreshRewardCode()
	{
		buttonBackerCode.interactable = !base.config.HasBackerRewardCode() || EClass.debug.enable;
		if (base.config.HasBackerRewardCode())
		{
			buttonBackerCode.mainText.SetText("rewardCodeActive".lang());
		}
	}
}
