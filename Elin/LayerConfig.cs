using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LayerConfig : ELayer
{
	public Slider sliderMaster;

	public Slider sliderBGM;

	public Slider sliderSFX;

	public Slider sliderAMB;

	public Slider sliderBrightness;

	public Slider sliderContrast;

	public Slider sliderSaturation;

	public Slider sliderVibrance;

	public Slider sliderScrollSense;

	public Slider sliderScale;

	public Slider sliderSharpen;

	public Slider sliderSharpen2;

	public Slider sliderBlur;

	public UISelectableGroup groupVSync;

	public UISelectableGroup groupLang;

	public UIButton toggleVsync;

	public UIButton toggleFixedResolution;

	public UIButton toggleOpenLastTab;

	public UIButton toggleClosePopupOnMouseLeave;

	public UIButton toggleRightClickClose;

	public UIButton toggleFullscreen;

	public UIButton togglePixelPerfectUI;

	public UIButton toggleBloom;

	public UIButton toggleHDR;

	public UIButton toggleBlur;

	public UIButton toggleAutoscale;

	public UIButton toggleDynamicBrightness;

	public UIButton toggleKuwahara;

	public UIButton toggleBlizzard;

	public UIButton toggleDisableShake;

	public UIButton toggleFirefly;

	public UIButton toggleSecureWidth;

	public UIButton toggleGradientWater;

	public UIButton toggleGodray;

	public UIButton toggleShowFloatButtons;

	public UIButton toggleAllyLight;

	public UIButton toggleBalloon;

	public UIButton toggleFloorEx;

	public UIButton toggleNoLightning;

	public UIItem fontUI;

	public UIItem fontChatbox;

	public UIItem fontBalloon;

	public UIItem fontDialog;

	public UIItem fontWidget;

	public UIItem fontNews;

	public UIDropdown ddPostEffect;

	public UIButton buttonReset;

	public Slider sliderUIBrightness;

	public Slider sliderUIContrast;

	public Slider sliderBalloon;

	public LayoutGroup layoutLanguage;

	public UIButton moldLanguage;

	public UIButton buttonApplyScreenSize;

	public InputField inputW;

	public InputField inputH;

	public override string GetTextHeader(Window window)
	{
		return "_configOf".lang(base.GetTextHeader(window));
	}

	public override void OnBeforeAddLayer()
	{
		windows[0].setting.tabs[5].disable = !ELayer.core.config.other.showTestOptions;
	}

	public override void OnInit()
	{
		inputW.text = ELayer.config.graphic.w.ToString() ?? "";
		inputH.text = ELayer.config.graphic.h.ToString() ?? "";
		ELayer.config.graphic.fullScreen = Screen.fullScreen;
		windows[0].AddBottomSpace();
		buttonReset = windows[0].AddBottomButton("", delegate
		{
			Dialog.YesNo("dialogResetConfigTab".lang(windows[0].CurrentTab.idLang.lang()), delegate
			{
				Close();
				if (windows[0].idTab == 0)
				{
					CoreConfig.ResetGeneral();
				}
				else if (windows[0].idTab == 1)
				{
					CoreConfig.ResetGraphics();
				}
				else if (windows[0].idTab == 2)
				{
					CoreConfig.ResetGame();
				}
				else if (windows[0].idTab == 3)
				{
					CoreConfig.ResetInput();
				}
				else if (windows[0].idTab == 4)
				{
					CoreConfig.ResetOther();
				}
				else
				{
					CoreConfig.ResetTest();
				}
				ELayer.ui.AddLayer<LayerConfig>();
			});
		});
		windows[0].AddBottomButton("resetConfig", delegate
		{
			Dialog.YesNo("dialogResetConfig", delegate
			{
				Close();
				CoreConfig.Reset();
				ELayer.ui.AddLayer<LayerConfig>();
			});
		});
		List<PostEffectProfile> list = (from p in Resources.LoadAll<PostEffectProfile>("Scene/Profile/PostEffect/")
			where !p.disable
			select p).ToList();
		ddPostEffect.SetList(ELayer.config.graphic.idPostProfile, list, (PostEffectProfile a, int b) => a.name, delegate(int a, PostEffectProfile b)
		{
			ELayer.config.graphic.idPostProfile = b.name;
			b.OnChangeProfile();
			sliderSharpen.value = ELayer.config.graphic.sharpen;
			sliderSharpen2.value = ELayer.config.graphic.sharpen2;
			sliderBlur.value = ELayer.config.graphic.blur;
			toggleKuwahara.SetCheck(ELayer.config.graphic.kuwahara);
			ELayer.config.ApplyGrading();
		}, notify: false);
		Refresh();
	}

	public override void OnSwitchContent(Window window)
	{
		buttonReset.mainText.SetText("resetConfigTab".lang(windows[0].CurrentTab.idLang.lang()));
		windows[0].rectBottom.RebuildLayout(recursive: true);
		windows[0].CurrentContent.RebuildLayout(recursive: true);
	}

	private void Update()
	{
		if (!inputW.isFocused && !inputH.isFocused)
		{
			ValidateResolution();
		}
	}

	public void ValidateResolution()
	{
		int num = Mathf.Clamp(inputW.text.ToInt(), 800, Display.main.systemWidth);
		int num2 = Mathf.Clamp(inputH.text.ToInt(), 600, Display.main.systemHeight);
		inputW.text = num.ToString() ?? "";
		inputH.text = num2.ToString() ?? "";
		ELayer.core.config.graphic.w = inputW.text.ToInt();
		ELayer.core.config.graphic.h = inputH.text.ToInt();
	}

	public override void OnKill()
	{
		ELayer.core.config.graphic.w = inputW.text.ToInt();
		ELayer.core.config.graphic.h = inputH.text.ToInt();
		ELayer.core.config.Save();
		ELayer.core.config.Apply();
	}

	public void Refresh()
	{
		ELayer.config.ignoreApply = true;
		SetSlider(sliderMaster, ELayer.config.sound.volumeMaster, delegate(float a)
		{
			ELayer.config.sound.volumeMaster = a;
			ELayer.config.ApplyVolume();
			return Lang.Get("MASTER") + "(" + (int)(a * 100f) + "%)";
		});
		SetSlider(sliderBGM, ELayer.config.sound.volumeBGM, delegate(float a)
		{
			ELayer.config.sound.volumeBGM = a;
			ELayer.config.ApplyVolume();
			return Lang.Get("BGM") + "(" + (int)(a * 100f) + "%)";
		});
		SetSlider(sliderSFX, ELayer.config.sound.volumeSFX, delegate(float a)
		{
			ELayer.config.sound.volumeSFX = a;
			ELayer.config.ApplyVolume();
			return Lang.Get("SFX") + "(" + (int)(a * 100f) + "%)";
		});
		SetSlider(sliderAMB, ELayer.config.sound.volumeAMB, delegate(float a)
		{
			ELayer.config.sound.volumeAMB = a;
			ELayer.config.ApplyVolume();
			return Lang.Get("AMB") + "(" + (int)(a * 100f) + "%)";
		});
		SetSlider(sliderBrightness, ELayer.config.graphic.brightness, delegate(float a)
		{
			ELayer.config.graphic.brightness = a;
			ELayer.config.ApplyGrading();
			return Lang.Get("brightness") + "(" + (int)(a * 100f) + "%)";
		});
		SetSlider(sliderContrast, ELayer.config.graphic.contrast, delegate(float a)
		{
			ELayer.config.graphic.contrast = a;
			ELayer.config.ApplyGrading();
			return Lang.Get("contrast") + "(" + (int)(a * 100f) + "%)";
		});
		SetSlider(sliderSaturation, ELayer.config.graphic.saturation, delegate(float a)
		{
			ELayer.config.graphic.saturation = a;
			ELayer.config.ApplyGrading();
			return Lang.Get("saturation") + "(" + (int)(a * 100f) + "%)";
		});
		SetSlider(sliderVibrance, ELayer.config.graphic.vibrance, delegate(float a)
		{
			ELayer.config.graphic.vibrance = a;
			ELayer.config.ApplyGrading();
			return Lang.Get("vibrance") + "(" + (int)(a * 100f) + "%)";
		});
		SetSlider(sliderSharpen, ELayer.config.graphic.sharpen, delegate(float a)
		{
			ELayer.config.graphic.sharpen = (int)a;
			ELayer.config.ApplyGrading();
			return Lang.Get("sharpen") + "(" + (int)a + "%)";
		});
		SetSlider(sliderSharpen2, ELayer.config.graphic.sharpen2, delegate(float a)
		{
			ELayer.config.graphic.sharpen2 = (int)a;
			ELayer.config.ApplyGrading();
			return Lang.Get("sharpen2") + "(" + (int)a + "%)";
		});
		SetSlider(sliderBlur, ELayer.config.graphic.blur, delegate(float a)
		{
			ELayer.config.graphic.blur = (int)a;
			ELayer.config.ApplyGrading();
			return Lang.Get("blur") + "(" + (int)a + "%)";
		});
		SetSlider(sliderScrollSense, ELayer.config.ui.scrollSens, delegate(float a)
		{
			ELayer.config.ui.scrollSens = a;
			UIScrollView.sensitivity = ELayer.config.ui.ScrollSensitivity;
			return Lang.Get("scrollSens") + "(" + (int)(a * 100f) + ")";
		});
		SetSlider(sliderScale, ELayer.config.ui.scale, delegate(float a)
		{
			ELayer.config.ui.scale = (int)a;
			ELayer.config.ApplyScale();
			ELayer.core.OnChangeResolution();
			return Lang.Get("uiScale") + "(" + a * 5f + "%)";
		});
		SetSlider(sliderUIBrightness, ELayer.config.ui.brightness, delegate(float a)
		{
			ELayer.config.ui.brightness = (int)a;
			ELayer.config.ApplyGrading();
			return Lang.Get("uiBrightness") + "(" + a + "%)";
		});
		SetSlider(sliderUIContrast, ELayer.config.ui.contrast, delegate(float a)
		{
			ELayer.config.ui.contrast = (int)a;
			ELayer.config.ApplyGrading();
			return Lang.Get("uiContrast") + "(" + a + "%)";
		});
		SetSlider(sliderBalloon, ELayer.config.ui.outlineAlpha, delegate(float a)
		{
			ELayer.config.ui.outlineAlpha = (int)a;
			ELayer.config.Apply();
			return Lang.Get("outlineAlpha") + "(" + a + "%)";
		});
		SetGroup(groupVSync, ELayer.config.graphic.fps, delegate(int a)
		{
			ELayer.config.graphic.fps = a;
			ELayer.config.Apply();
		});
		if (!moldLanguage)
		{
			moldLanguage = layoutLanguage.CreateMold<UIButton>();
		}
		layoutLanguage.DestroyChildren();
		int value = 0;
		List<LangSetting> list = new List<LangSetting>();
		foreach (LangSetting value2 in MOD.langs.Values)
		{
			Util.Instantiate(moldLanguage, layoutLanguage).mainText.text = value2.name + "(" + value2.name_en + ")";
			if (ELayer.config.lang == value2.id)
			{
				value = list.Count;
			}
			list.Add(value2);
		}
		layoutLanguage.RebuildLayout();
		SetGroup(groupLang, value, delegate(int a)
		{
			if (ELayer.config.lang != list[a].id)
			{
				ELayer.config.lang = list[a].id;
				ELayer.core.SetLang(ELayer.config.lang);
				Close();
				IChangeLanguage[] componentsInChildren = ELayer.ui.GetComponentsInChildren<IChangeLanguage>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].OnChangeLanguage();
				}
				ELayer.ui.AddLayer<LayerConfig>();
				Dialog.Ok("dialogChangeLang");
				ELayer.config.Save();
			}
		});
		toggleGradientWater.SetToggle(ELayer.config.graphic.gradientWater, delegate(bool on)
		{
			ELayer.config.graphic.gradientWater = on;
			ELayer.scene.ApplyZoneConfig();
		});
		toggleVsync.SetToggle(ELayer.config.graphic.vsync, delegate(bool on)
		{
			ELayer.config.graphic.vsync = on;
			ELayer.config.Apply();
		});
		toggleAutoscale.SetToggle(ELayer.config.ui.autoscale, delegate(bool on)
		{
			ELayer.config.ui.autoscale = on;
			ELayer.core.OnChangeResolution();
			ELayer.config.ApplyScale();
		});
		toggleOpenLastTab.SetToggle(ELayer.config.ui.openLastTab, delegate(bool on)
		{
			ELayer.config.ui.openLastTab = on;
			ELayer.config.Apply();
		});
		if ((bool)toggleClosePopupOnMouseLeave)
		{
			toggleClosePopupOnMouseLeave.SetToggle(ELayer.config.ui.closePopupOnMouseLeave, delegate(bool on)
			{
				ELayer.config.ui.closePopupOnMouseLeave = on;
				ELayer.config.Apply();
			});
		}
		toggleBalloon.SetToggle(ELayer.config.ui.balloonBG, delegate(bool on)
		{
			ELayer.config.ui.balloonBG = on;
			ELayer.config.Apply();
		});
		toggleRightClickClose.SetToggle(ELayer.config.ui.rightClickClose, delegate(bool on)
		{
			ELayer.config.ui.rightClickClose = on;
			ELayer.config.Apply();
		});
		toggleFullscreen.SetToggle(ELayer.config.graphic.fullScreen, delegate(bool on)
		{
			ELayer.config.graphic.fullScreen = on;
			ELayer.config.Apply();
		});
		togglePixelPerfectUI.SetToggle(ELayer.config.graphic.pixelperfectUI, delegate(bool on)
		{
			ELayer.config.graphic.pixelperfectUI = on;
			ELayer.config.Apply();
		});
		toggleFixedResolution.SetToggle(ELayer.config.graphic.fixedResolution, delegate(bool on)
		{
			ELayer.config.graphic.fixedResolution = on;
			ELayer.config.Apply();
		});
		toggleBloom.SetToggle(ELayer.config.graphic.bloom, delegate(bool on)
		{
			ELayer.config.graphic.bloom = on;
			ELayer.config.Apply();
		});
		toggleHDR.SetToggle(ELayer.config.graphic.hdr, delegate(bool on)
		{
			ELayer.config.graphic.hdr = on;
			ELayer.config.Apply();
		});
		toggleKuwahara.SetToggle(ELayer.config.graphic.kuwahara, delegate(bool on)
		{
			ELayer.config.graphic.kuwahara = on;
			ELayer.config.ApplyGrading();
		});
		toggleDisableShake.SetToggle(ELayer.config.graphic.disableShake, delegate(bool on)
		{
			ELayer.config.graphic.disableShake = on;
		});
		toggleFirefly.SetToggle(ELayer.config.graphic.firefly, delegate(bool on)
		{
			ELayer.config.graphic.firefly = on;
			if (ELayer.core.IsGameStarted)
			{
				ELayer.screen.RefreshSky();
			}
		});
		toggleFloorEx.SetToggle(ELayer.config.graphic.floorEx, delegate(bool on)
		{
			ELayer.config.graphic.floorEx = on;
			if (ELayer.core.IsGameStarted)
			{
				ELayer.screen.RefreshAll();
			}
		});
		toggleGodray.SetToggle(ELayer.config.graphic.godray, delegate(bool on)
		{
			ELayer.config.graphic.godray = on;
			if (ELayer.core.IsGameStarted)
			{
				ELayer.screen.RefreshSky();
			}
		});
		toggleBlizzard.SetToggle(ELayer.config.graphic.blizzard, delegate(bool on)
		{
			ELayer.config.graphic.blizzard = on;
			if (ELayer.core.IsGameStarted)
			{
				ELayer.screen.RefreshSky();
			}
		});
		toggleNoLightning.SetToggle(ELayer.config.graphic.noLightning, delegate(bool on)
		{
			ELayer.config.graphic.noLightning = on;
		});
		toggleAllyLight.SetToggle(ELayer.config.graphic.drawAllyLight, delegate(bool on)
		{
			ELayer.config.graphic.drawAllyLight = on;
			if (ELayer.core.IsGameStarted)
			{
				ELayer._map.RefreshFOVAll();
			}
		});
		toggleBlur.SetToggle(ELayer.config.ui.blur, delegate(bool on)
		{
			ELayer.config.ui.blur = on;
		});
		toggleDynamicBrightness.SetToggle(ELayer.config.ui.dynamicBrightness, delegate(bool on)
		{
			ELayer.config.ui.dynamicBrightness = on;
			ELayer.config.RefreshUIBrightness();
		});
		toggleSecureWidth.SetToggle(ELayer.config.ui.secureMinWidth, delegate(bool on)
		{
			ELayer.config.ui.secureMinWidth = on;
		});
		toggleShowFloatButtons.SetToggle(ELayer.config.ui.showFloatButtons, delegate(bool on)
		{
			ELayer.config.ui.showFloatButtons = on;
		});
		buttonApplyScreenSize.SetOnClick(delegate
		{
			ValidateResolution();
			ELayer.config.ApplyResolution(force: true);
		});
		new List<string> { "none", "focus", "focusPause" };
		new List<string> { "ani0", "ani1", "ani2", "ani3", "ani4" };
		SetFont(ELayer.config.font.fontUI, fontUI, "fontUI");
		SetFont(ELayer.config.font.fontChatbox, fontChatbox, "fontChatbox");
		SetFont(ELayer.config.font.fontBalloon, fontBalloon, "fontBalloon");
		SetFont(ELayer.config.font.fontDialog, fontDialog, "fontDialog");
		SetFont(ELayer.config.font.fontWidget, fontWidget, "widget");
		SetFont(ELayer.config.font.fontNews, fontNews, "fontNews");
		ELayer.config.ignoreApply = false;
	}

	public void SetFont(SkinManager.FontSaveData data, UIItem item, string lang)
	{
		item.text1.SetLang(lang);
		UIDropdown componentInChildren = item.GetComponentInChildren<UIDropdown>();
		UIButtonLR obj = item.button1 as UIButtonLR;
		List<string> langs = new List<string> { "sizeS", "sizeDefault", "sizeL", "sizeLL", "sizeLLL", "sizeLLLL" };
		SkinManager skins = ELayer.ui.skins;
		obj.SetOptions(data.size, langs, delegate(int i)
		{
			data.size = i;
			ApplyFont();
		}, invoke: false);
		componentInChildren.options.Clear();
		for (int j = 0; j < skins.FontList.Count; j++)
		{
			FontSource fontSource = skins.FontList[j];
			componentInChildren.options.Add(new Dropdown.OptionData
			{
				text = fontSource._name
			});
			if (j == data.index)
			{
				componentInChildren.value = j;
			}
		}
		componentInChildren.onValueChanged.RemoveAllListeners();
		componentInChildren.onValueChanged.AddListener(delegate(int i)
		{
			data.index = i;
			ApplyFont();
		});
	}

	public void ApplyFont()
	{
		ELayer.config.ApplyFont();
		this.RebuildLayout(recursive: true);
	}

	public void SetSlider(Slider slider, float value, Func<float, string> action, bool notify = false)
	{
		slider.onValueChanged.RemoveAllListeners();
		slider.onValueChanged.AddListener(delegate(float a)
		{
			slider.GetComponentInChildren<UIText>(includeInactive: true).text = action(a);
		});
		if (notify)
		{
			slider.value = value;
		}
		else
		{
			slider.SetValueWithoutNotify(value);
		}
		slider.GetComponentInChildren<UIText>(includeInactive: true).text = action(value);
	}

	public void SetGroup(UISelectableGroup group, int value, UnityAction<int> action)
	{
		group.Init(value, action);
	}
}
