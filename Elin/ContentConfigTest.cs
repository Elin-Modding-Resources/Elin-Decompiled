using System.Collections.Generic;
using UnityEngine.UI;

public class ContentConfigTest : ContentConfig
{
	public UIDropdown ddSkin;

	public UIButtonLR buttonAnimeFrame;

	public UIButtonLR buttonAnimeFramePCC;

	public UIButtonLR buttonExtraToneMTP;

	public UIButton toggleAlwaysRun;

	public UIButton toggleShowNumber;

	public UIButton toggleStackNumber;

	public UIButton toggleAAPortrait;

	public UIButton toggleExTurn;

	public UIButton toggleBloom2;

	public UIButton toggleExtraRace;

	public UIButton toggleSeal;

	public UIButton toggleExtraCancelMove;

	public UIButton toggleBlockOnItem;

	public UIButton toggleAlwaysFixCamera;

	public UIButton toggleIgnoreBackerFlag;

	public UIButton toggleRefIcon;

	public UIButton toggleToolNoPick;

	public Slider sliderBrightness;

	public override void OnInstantiate()
	{
		List<SkinRootStatic> mainSkins = EClass.ui.skins.mainSkins;
		ddSkin.SetList(base.config.test.idSkin, mainSkins, (SkinRootStatic a, int b) => a.Name, delegate(int a, SkinRootStatic b)
		{
			base.config.test.idSkin = a;
			base.config.ApplySkin();
		});
		List<string> langs = new List<string> { "ani0", "ani1", "ani2", "ani3", "ani4" };
		List<string> langs2 = new List<string> { "0x", "1x", "2x", "5x", "10x" };
		toggleRefIcon.SetToggle(base.config.test.showRefIcon, delegate(bool on)
		{
			base.config.test.showRefIcon = on;
			if (EClass.core.IsGameStarted)
			{
				LayerInventory.SetDirtyAll();
			}
		});
		buttonExtraToneMTP.SetOptions(base.config.test.extraToneMTP, langs2, delegate(int i)
		{
			base.config.test.extraToneMTP = i;
			base.config.Apply();
		}, invoke: false, "extraToneMTP");
		buttonAnimeFrame.SetOptions(base.config.test.animeFrame, langs, delegate(int i)
		{
			base.config.test.animeFrame = i;
			base.config.Apply();
		}, invoke: false, "animeFrame");
		buttonAnimeFramePCC.SetOptions(base.config.test.animeFramePCC, langs, delegate(int i)
		{
			base.config.test.animeFramePCC = i;
			base.config.Apply();
		}, invoke: false, "animeFramePCC");
		toggleShowNumber.SetToggle(base.config.test.showNumbers, delegate(bool on)
		{
			base.config.test.showNumbers = on;
		});
		toggleStackNumber.SetToggle(base.config.test.stackNumbers, delegate(bool on)
		{
			base.config.test.stackNumbers = on;
		});
		toggleToolNoPick.SetToggle(base.config.test.toolNoPick, delegate(bool on)
		{
			base.config.test.toolNoPick = on;
		});
		toggleAlwaysRun.SetToggle(base.config.test.alwaysRun, delegate(bool on)
		{
			base.config.test.alwaysRun = on;
		});
		toggleExTurn.SetToggle(base.config.test.extraTurnaround, delegate(bool on)
		{
			base.config.test.extraTurnaround = on;
		});
		toggleExtraRace.SetToggle(base.config.test.extraRace, delegate(bool on)
		{
			base.config.test.extraRace = on;
		});
		toggleExtraCancelMove.SetToggle(base.config.test.extraMoveCancel, delegate(bool on)
		{
			base.config.test.extraMoveCancel = on;
		});
		toggleAAPortrait.SetToggle(base.config.test.aaPortrait, delegate(bool on)
		{
			base.config.test.aaPortrait = on;
		});
		toggleBloom2.SetToggle(base.config.test.bloom2, delegate(bool on)
		{
			base.config.test.bloom2 = on;
			base.config.Apply();
		});
		toggleBlockOnItem.SetToggle(base.config.test.allowBlockOnItem, delegate(bool on)
		{
			base.config.test.allowBlockOnItem = on;
			base.config.Apply();
		});
		toggleAlwaysFixCamera.SetToggle(base.config.test.alwaysFixCamera, delegate(bool on)
		{
			base.config.test.alwaysFixCamera = on;
		});
		toggleIgnoreBackerFlag.SetToggle(base.config.test.ignoreBackerDestoryFlag, delegate(bool on)
		{
			base.config.test.ignoreBackerDestoryFlag = on;
		});
		toggleSeal.SetToggle(base.config.test.unsealWidgets, delegate(bool on)
		{
			base.config.test.unsealWidgets = on;
			base.config.Apply();
			WidgetHotbar.RebuildPages();
		});
		SetSlider(sliderBrightness, base.config.test.brightnessNight, delegate(float a)
		{
			base.config.test.brightnessNight = a;
			base.config.ApplyGrading();
			if (EClass.core.IsGameStarted)
			{
				EClass.screen.RefreshGrading();
			}
			return Lang.Get("brightnessNight") + "(" + (int)(a + 100f) + "%)";
		});
	}
}
