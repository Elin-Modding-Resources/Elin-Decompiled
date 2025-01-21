using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[JsonObject(MemberSerialization.OptOut)]
public class CoreConfig : EClass
{
	public enum GameFunc
	{
		None,
		ToggleZoom,
		ShowInv,
		ShowChara,
		ShowAbility,
		ToggleBuild,
		ShowJournal,
		EmuShift,
		EmuAlt,
		AllAction,
		ToggleNoRoof,
		OpenAllyInv,
		Talk,
		EmptyHand,
		Fire,
		SwitchHotbar,
		PropertySearch
	}

	public enum GameFuncBuild
	{
		None,
		ExitBuildMode,
		Rotate,
		ToggleFreepos,
		SnapFreepos,
		ToggleRoof,
		ToggleSlope,
		ToggleWall,
		TogglePicker,
		ToggleBuildLight
	}

	public enum ScreenSnapType
	{
		None,
		Default,
		Floor,
		Ceil,
		Hack3,
		Hack4,
		Hack5,
		Hack6,
		Hack7,
		Hack8,
		Hack9,
		Grid
	}

	[Serializable]
	public class CameraConfig
	{
		public bool invertX;

		public bool invertY;

		public bool edgeScroll;

		public bool zoomToMouse;

		public bool extendZoomMin;

		public bool extendZoomMax;

		public bool linearZoom;

		public bool smoothFollow;

		public bool smoothMove;

		public float senseKeyboard;

		public float sensEdge;

		public float sensDrag;

		public float momentum;

		public float zoomSpeed;

		public int defaultZoom;

		public int moveframe;

		public float[] zooms;
	}

	[Serializable]
	public class GameConfig
	{
		public int numBackup;

		public int backupInterval;

		public bool autoBackup;

		public bool sortEach;

		public bool autopause;

		public bool showQuickMenuHint;

		public bool dontRenderOutsideMap;

		public bool freePos;

		public bool showOffhand;

		public bool confirmGive;

		public bool confirmMapExit;

		public bool holdMiddleButtonToHold;

		public bool doubleClickToHold;

		public bool useAbilityOnHotkey;

		public float runDistance;

		public float angleMargin;

		public int newlineCount;

		public int showBorder;

		public int showRide;

		public int waiter;

		public bool altUI;

		public bool altInv;

		public bool altAbility;

		public bool altCraft;

		public bool autoSave;

		public bool alwaysUpdateRecipe;

		public bool showInvBG;

		public bool useGrid;

		public bool rightScroll;

		public bool tutorial;

		public bool advancedMenu;

		public bool warnCrime;

		public bool warnMana;

		public bool warnDisassemble;

		public bool hideWeapons;

		public bool disableAutoStairs;

		public bool smoothPick;

		public bool markStack;

		public bool waitOnRange;

		public bool waitOnDebuff;

		public bool waitOnMelee;

		public bool highlightEnemy;

		public bool showShippingResult;

		public bool shiftToUseNegativeAbilityOnSelf;

		public bool haltOnSpotEnemy;

		public bool haltOnSpotTrap;

		[NonSerialized]
		[JsonIgnore]
		public bool ignoreWarnCrime;

		[NonSerialized]
		[JsonIgnore]
		public bool ignoreWarnMana;

		[NonSerialized]
		[JsonIgnore]
		public bool ignoreWarnDisassemble;

		[NonSerialized]
		[JsonIgnore]
		public bool ignoreWarnSlaughter;
	}

	[Serializable]
	public class InputSetting
	{
		public bool autorun;

		public bool autowalk;

		public bool altKeyAxis;

		public bool keepRunning;

		public bool rightClickExitBuildMode;

		public bool ignoreNPCs;

		public bool altExamine;

		public bool altChangeHeight;

		public GameFunc middleClick;

		public GameFunc middlePressLong;

		public GameFunc mouse3Click;

		public GameFunc mouse3PressLong;

		public GameFunc mouse4Click;

		public GameFunc mouse4PressLong;

		public GameFuncBuild b_middleClick;

		public GameFuncBuild b_middlePressLong;

		public GameFuncBuild b_mouse3Click;

		public GameFuncBuild b_mouse3PressLong;

		public GameFuncBuild b_mouse4Click;

		public GameFuncBuild b_mouse4PressLong;

		public EInput.KeyMapManager keys;
	}

	[Serializable]
	public class SoundSetting
	{
		public float volumeMaster;

		public float volumeBGM;

		public float volumeSFX;

		public float volumeAMB;
	}

	[Serializable]
	public class GraphicSetting
	{
		public string idPostProfile;

		public bool fullScreen;

		public bool pixelperfectUI;

		public bool alwaysClearCamera;

		public bool vsync;

		public bool kuwahara;

		public bool drawAllyLight;

		public bool hdr;

		public bool fixedResolution;

		public bool floorEx;

		public bool cloud;

		public bool firefly;

		public bool bloom;

		public bool gradientWater;

		public bool godray;

		public bool enhanceRain;

		public bool blizzard;

		public bool disableShake;

		public bool noLightning;

		public int fps;

		public int fireflyCount = 150;

		public int starCount = 200;

		public int sharpen;

		public int sharpen2;

		public int blur;

		public int spriteFrameMode;

		public int w = 1280;

		public int h = 768;

		public float brightness;

		public float contrast;

		public float saturation;

		public float vibrance;

		public float gradientWaterLevel = 0.2f;
	}

	[Serializable]
	public class UISetting
	{
		public string defaultTheme;

		public bool openLastTab;

		public bool rightClickClose;

		public bool blur;

		public bool cornerHoard;

		public bool animeWindow;

		public bool autoscale;

		public bool dynamicBrightness;

		public bool secureMinWidth;

		public bool autoFocusWindow;

		public bool showFloatButtons;

		public bool balloonBG;

		public bool closePopupOnMouseLeave;

		public float scrollSens;

		public float baseScrollSens;

		public float blurSize;

		public float dynamicBrightnessMod;

		public int mouseDragMargin;

		public int scale;

		public int brightness;

		public int contrast;

		public int outlineAlpha;

		public float ScrollSensitivity => baseScrollSens * scrollSens * scrollSens;
	}

	[Serializable]
	public class FontSetting
	{
		public SkinManager.FontSaveData fontUI;

		public SkinManager.FontSaveData fontChatbox;

		public SkinManager.FontSaveData fontBalloon;

		public SkinManager.FontSaveData fontDialog;

		public SkinManager.FontSaveData fontWidget;

		public SkinManager.FontSaveData fontNews;
	}

	[Serializable]
	public class CustomFont
	{
		public string id;

		public string name;
	}

	[Serializable]
	public class NetSetting
	{
		public bool enable;

		public bool sendEvent;

		public bool receiveRealtime;

		public bool password;
	}

	[Serializable]
	public class BackerContentConfig
	{
		public int filter;

		public bool FilterAll => filter == 2;

		public bool FilterLang => filter == 1;

		public bool FilterNone => filter == 0;

		public bool Show(int id)
		{
			return Show(EClass.sources.backers.map.TryGetValue(id));
		}

		public bool Show(SourceBacker.Row row)
		{
			if (row == null)
			{
				return false;
			}
			if (FilterNone)
			{
				return true;
			}
			if (FilterAll)
			{
				return false;
			}
			bool flag = row.lang == "JP" || row.lang == "CN";
			string langCode = Lang.langCode;
			if (langCode == "JP" || langCode == "CN")
			{
				return flag;
			}
			return !flag;
		}

		public bool Show(string s)
		{
			if (FilterNone)
			{
				return true;
			}
			if (FilterAll)
			{
				return false;
			}
			bool flag = IsJapanese(s);
			string langCode = Lang.langCode;
			if (langCode == "JP" || langCode == "CN")
			{
				return flag;
			}
			return !flag;
		}

		private bool IsJapanese(string text)
		{
			return Regex.IsMatch(text, "[\\p{IsHiragana}\\p{IsKatakana}\\p{IsCJKUnifiedIdeographs}]+");
		}
	}

	[Serializable]
	public class OtherSetting
	{
		public int antiMissClick = 2;

		public float bgmInterval = 1f;

		public bool noCensor;

		public bool runBackground;

		public bool muteBackground;

		public bool showTestOptions;

		public bool syncMods;

		public bool disableMods;

		public string idMainWidgetTheme;

		public string idSubWidgetTheme;
	}

	[Serializable]
	public class Test
	{
		public int idSkin;

		public int animeFramePCC;

		public int animeFrame;

		public bool showNumbers;

		public bool aaPortrait;

		public bool extraTurnaround;

		public bool bloom2;

		public bool extraRace;

		public bool unsealWidgets;

		public bool extraMoveCancel;

		public bool showNetError;

		public bool alwaysRun;

		public bool allowBlockOnItem;

		public bool alwaysFixCamera;

		public bool ignoreBackerDestoryFlag;

		public bool showRefIcon;

		public bool showTrait;

		public bool toolNoPick;

		public float brightnessNight;
	}

	[Serializable]
	public class Fix
	{
		public CameraSupport.Divider divider;

		public ScreenSnapType snapType;
	}

	public Version version;

	public string lang = "JP";

	public string nameReport;

	public string emailReport;

	public string rewardCode;

	public int maxPlayedHours;

	public bool compressSave;

	public bool ignoreParallelsWarning;

	public bool ignoreLinuxModWarning;

	public bool cloud;

	public new UISetting ui;

	public SoundSetting sound;

	public FontSetting font;

	public GraphicSetting graphic;

	public new GameConfig game;

	public NetSetting net;

	public BackerContentConfig backer;

	public InputSetting input;

	public CameraConfig camera;

	public OtherSetting other;

	public Fix fix;

	public Test test;

	public List<FontSource> customFonts;

	public HashSet<string> helpFlags = new HashSet<string>();

	public int[] colors = new int[10];

	[NonSerialized]
	public bool ignoreApply;

	private int[] _framerates = new int[4] { 120, 60, 30, 15 };

	public static string path => CorePath.ConfigFile;

	public static int ZoomStep
	{
		get
		{
			if (!EClass.debug.enable)
			{
				return 5;
			}
			return 5;
		}
	}

	[JsonIgnore]
	public CameraSupport camSupport => EClass.scene.camSupport;

	public static bool Exist()
	{
		if (!File.Exists(path) || (EClass.debug.useNewConfig && Application.isEditor))
		{
			return false;
		}
		return true;
	}

	public static CoreConfig TryLoadConfig()
	{
		if (!File.Exists(path) || (EClass.debug.useNewConfig && Application.isEditor))
		{
			return null;
		}
		if (!File.Exists(CorePath.VersionFile))
		{
			return null;
		}
		CoreConfig coreConfig = IO.LoadFile<CoreConfig>(path);
		if (!IsCompatible(coreConfig.version))
		{
			Debug.Log("Config version is different. deleting:" + coreConfig.version.ToString() + "/" + EClass.core.version.GetText());
			return null;
		}
		if (coreConfig.game.backupInterval == 0)
		{
			coreConfig.game.numBackup = 5;
			coreConfig.game.backupInterval = 8;
			coreConfig.game.autoBackup = true;
		}
		if (coreConfig.version.IsBelow(0, 23, 63))
		{
			coreConfig.graphic.floorEx = true;
		}
		if (coreConfig.version.IsBelow(0, 22, 17))
		{
			coreConfig.game.tutorial = true;
		}
		if (coreConfig.version.IsBelow(0, 22, 24))
		{
			coreConfig.ui.balloonBG = true;
		}
		return coreConfig;
	}

	public static bool IsCompatible(Version v)
	{
		return v.minor >= 22;
	}

	public static void Init()
	{
		if (EClass.core.config == null)
		{
			Debug.Log("Creating new config.");
			CoreConfig coreConfig = (EClass.core.config = IO.DeepCopy(EClass.setting.config));
			coreConfig.SetLang(EClass.core.langCode);
			if (!Application.isEditor || !EClass.debug.useNewConfig)
			{
				coreConfig.Save();
			}
			coreConfig.GetPostEffectProfile().OnChangeProfile();
			if (EClass.debug.enable)
			{
				coreConfig.other.showTestOptions = true;
				coreConfig.test.showNumbers = true;
				coreConfig.game.waiter = 0;
				coreConfig.game.advancedMenu = true;
			}
		}
		EClass.core.config.OnInit();
	}

	public void OnInit()
	{
		EClass.core.ui.skins.SetMainSkin(test.idSkin);
		Apply();
	}

	public bool HasBackerRewardCode()
	{
		return ElinEncoder.IsValid(rewardCode);
	}

	public static void Reset()
	{
		string text = EClass.core.config.lang;
		string text2 = EClass.core.config.rewardCode;
		EClass.core.config = IO.DeepCopy(EClass.setting.config);
		EClass.core.config.SetLang(text);
		EClass.core.config.OnReset();
		EClass.core.config.rewardCode = text2;
	}

	public static void ResetGeneral()
	{
		CoreConfig coreConfig = IO.DeepCopy(EClass.setting.config);
		EClass.core.config.ui = coreConfig.ui;
		EClass.core.config.font = coreConfig.font;
		EClass.core.config.sound = coreConfig.sound;
		EClass.core.config.Apply();
		EClass.core.config.ApplyFont();
	}

	public static void ResetGraphics()
	{
		CoreConfig coreConfig = IO.DeepCopy(EClass.setting.config);
		EClass.core.config.graphic = coreConfig.graphic;
		EClass.core.config.GetPostEffectProfile().OnChangeProfile();
		EClass.core.config.Apply();
	}

	public static void ResetGame()
	{
		CoreConfig coreConfig = IO.DeepCopy(EClass.setting.config);
		EClass.core.config.game = coreConfig.game;
		EClass.core.config.net = coreConfig.net;
		EClass.core.config.backer = coreConfig.backer;
		EClass.core.config.Apply();
	}

	public static void ResetInput()
	{
		CoreConfig coreConfig = IO.DeepCopy(EClass.setting.config);
		EClass.core.config.input = coreConfig.input;
		EClass.core.config.camera = coreConfig.camera;
		EClass.core.config.Apply();
	}

	public static void ResetOther()
	{
		CoreConfig coreConfig = IO.DeepCopy(EClass.setting.config);
		EClass.core.config.fix = coreConfig.fix;
		EClass.core.config.other = coreConfig.other;
		EClass.core.config.Apply();
	}

	public static void ResetTest()
	{
		CoreConfig coreConfig = IO.DeepCopy(EClass.setting.config);
		EClass.core.config.test = coreConfig.test;
		EClass.core.config.Apply();
	}

	public void OnReset()
	{
		EClass.core.config.GetPostEffectProfile().OnChangeProfile();
		Apply();
		ApplyFont();
		ApplySkin();
	}

	public void SetLang(string id)
	{
		lang = id;
	}

	public void TryUpdatePlayedHour()
	{
		if (EClass.core.IsGameStarted)
		{
			int num = (int)(EClass.player.stats.timeElapsed / 3600.0);
			if (num > maxPlayedHours)
			{
				maxPlayedHours = num;
				Save();
			}
		}
	}

	public void Save()
	{
		version = EClass.core.version;
		IO.SaveFile(path, this, compress: false, IO.jsWriteConfig);
		IO.SaveFile(CorePath.VersionFile, version);
		Debug.Log("Config saved to " + path);
	}

	public void OnChangeResolution()
	{
		ApplyScale();
	}

	public void ApplyFPS(bool force = false)
	{
		if (EClass.core.IsGameStarted || force)
		{
			if (graphic.vsync)
			{
				QualitySettings.vSyncCount = 1;
				Application.targetFrameRate = 60;
			}
			else
			{
				QualitySettings.vSyncCount = 0;
				Application.targetFrameRate = _framerates[graphic.fps];
			}
		}
		else
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 60;
		}
	}

	public void Apply()
	{
		if (!ignoreApply)
		{
			EInput.SetKeyMap(input.keys);
			ApplyFPS();
			Application.runInBackground = Application.isEditor || other.runBackground;
			ApplyVolume();
			ApplyGrading();
			ApplyHUD();
			ApplyResolution();
			Window.openLastTab = ui.openLastTab;
			Layer.closeOnRightClick = ui.rightClickClose;
			RenderObject.animeSetting = EClass.core.gameSetting.render.anime;
			RenderObject.renderSetting = (RenderData.renderSetting = EClass.core.gameSetting.render);
			TC._setting = EClass.core.gameSetting.render.tc;
			UIScrollView.sensitivity = ui.ScrollSensitivity;
			UIContextMenu.closeOnMouseLeave = ui.closePopupOnMouseLeave;
			EClass.core.canvas.pixelPerfect = graphic.pixelperfectUI;
			EClass.screen.RefreshScreenSize();
			CharaRenderer._animeFrame = EClass.setting.render.anime.animeStep[test.animeFrame];
			CharaRenderer._animeFramePCC = EClass.setting.render.anime.animeStep[test.animeFramePCC];
			CharaRenderer.smoothmove = camera.smoothMove;
			CharaRenderer._animeFramePC = (camera.smoothMove ? 6000 : camera.moveframe);
			Scene.skipAnime = true;
			QualitySettings.maxQueuedFrames = 0;
			PopManager.outlineAlpha = ui.outlineAlpha;
			Window.animateWindow = ui.animeWindow;
			EInput.rightScroll = game.rightScroll;
			EInput.buttonScroll = (game.rightScroll ? EInput.rightMouse : EInput.middleMouse);
			EInput.antiMissClick = 0.1f * (float)other.antiMissClick;
			camSupport.bloom.enabled = graphic.bloom;
			camSupport.beautify.bloom = test.bloom2;
			camSupport.cam.allowHDR = graphic.hdr;
			ApplyScale();
		}
	}

	public void ApplyResolution(bool force = false)
	{
		if (graphic.fixedResolution)
		{
			force = true;
		}
		if (force)
		{
			Screen.SetResolution(graphic.w, graphic.h, graphic.fullScreen);
		}
		Screen.fullScreen = graphic.fullScreen;
	}

	public void ApplyScale()
	{
		float num = (float)ui.scale * 0.05f;
		if (ui.autoscale)
		{
			float a = 0.55f;
			float b = num + 0.05f;
			num = 0.01f * (float)Mathf.RoundToInt(Mathf.Lerp(a, b, (float)Screen.height / 1080f) * 100f);
		}
		if (ui.secureMinWidth && Screen.width < EClass.core.ui.minWidth)
		{
			float num2 = (float)Screen.width / (float)EClass.core.ui.minWidth;
			if (num2 < num)
			{
				num = num2;
			}
		}
		Debug.Log("#UI ApplyScale:" + num);
		EClass.core.canvas.GetComponent<CanvasScaler>().scaleFactor = num;
	}

	public void ApplyHUD()
	{
	}

	public void ApplyZoom(float a)
	{
		camSupport.Zoom = a;
		EClass.screen.RefreshScreenSize();
		camSupport.OnChangeResolution();
	}

	public PostEffectProfile GetPostEffectProfile(bool replaceWorld = false)
	{
		string text = graphic.idPostProfile.IsEmpty("None");
		if (replaceWorld && EClass.core.IsGameStarted && EClass.core.game.activeZone.IsRegion && text != "None")
		{
			text = "NFAA";
		}
		return ResourceCache.Load<PostEffectProfile>("Scene/Profile/PostEffect/" + text);
	}

	public void ApplyGrading()
	{
		PostEffectProfile postEffectProfile = GetPostEffectProfile();
		ScreenGrading grading = camSupport.grading;
		grading.userSaturation = graphic.saturation + postEffectProfile.Saturation;
		grading.userBrightness = graphic.brightness + postEffectProfile.Brightness;
		grading.userContrast = graphic.contrast + postEffectProfile.Contrast;
		camSupport.beautify.saturate = graphic.vibrance + grading.profile.Vibrance;
		camSupport.kuwahara.enabled = graphic.kuwahara;
		camSupport.blur.enabled = graphic.blur > 0;
		camSupport.blur.Amount = 0.01f * (float)graphic.blur;
		if (EClass.core.IsGameStarted)
		{
			EClass.screen.RefreshGrading();
		}
		camSupport.OnChangeResolution();
		RefreshUIBrightness();
		postEffectProfile.Apply(EClass.scene.cam);
	}

	public void RefreshUIBrightness()
	{
		ScreenGrading grading = camSupport.grading;
		float num = 0f;
		float num2 = 0f;
		if (EClass.core.IsGameStarted)
		{
			if (ui.dynamicBrightness && (EClass.world.date.IsNight || EClass._map.config.indoor))
			{
				num -= 0.05f * ui.dynamicBrightnessMod;
			}
		}
		else
		{
			num = -0.05f;
		}
		Shader.SetGlobalFloat("_UIBrightness", 0.01f * (float)ui.brightness + grading.profile.uiBrightness + num);
		Shader.SetGlobalFloat("_UIContrast", 0.01f * (float)ui.contrast + grading.profile.uiContrast + EClass.core.ui.lightContrast + num2);
	}

	public void ApplyVolume()
	{
		SetVolume("VolumeMaster", sound.volumeMaster);
		SetVolume("VolumeBGM", EClass.Sound.muteBGM ? 0f : sound.volumeBGM);
		SetVolume("VolumeSpatialBGM", sound.volumeBGM);
		SetVolume("VolumeSFX", sound.volumeSFX);
		SetVolume("VolumeAmbience", sound.volumeAMB);
		SetBGMInterval();
	}

	public void SetBGMInterval()
	{
		if (EClass.core.IsGameStarted)
		{
			EClass._map.plDay.interval = other.bgmInterval * 5f;
		}
	}

	public void SetVolume(string id, float v)
	{
		SoundManager.current.mixer.SetFloat(id, Mathf.Log((v < 0.01f) ? 0.01f : v) * 20f);
	}

	public void OnSetLang()
	{
		SkinManager.Instance.InitFont();
		ApplyFont();
	}

	public void ApplyFont()
	{
		EClass.core.ui.skins.SetFonts(font.fontUI, font.fontChatbox, font.fontBalloon, font.fontDialog, font.fontWidget, font.fontNews);
	}

	public void ApplySkin()
	{
		Core.Instance.ui.skins.SetMainSkin(test.idSkin);
		EClass.core.ApplySkins();
	}
}
