using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DG.Tweening;
using IniParser;
using IniParser.Model;
using ReflexCLI;
using Steamworks;
using UnityEngine;

public class Core : BaseCore
{
	public new static Core Instance;

	public static float delta;

	public static float avgDelta;

	public static float gameDelta;

	public static float gameDeltaNoPause;

	public static bool spiked;

	private static SourceElement sourceElement;

	public CoreDebug debug;

	public SourceManager sources;

	public PathManager pathManager;

	public EffectManager effects;

	public ModManager mods = new ModManager();

	public PCCManager pccs;

	public SkinManager skins;

	public TextureManager textures = new TextureManager();

	public GameSetting gameSetting;

	public GameData gamedata;

	public CoreRef refs;

	public ColorProfile Colors;

	public Material matUI;

	public Steam steam;

	public bool useUImat;

	public bool trial;

	public UI ui;

	public Scene scene;

	public BaseGameScreen screen;

	[NonSerialized]
	public CoreConfig config;

	public Game game;

	[NonSerialized]
	public bool initialized;

	private bool? lastFullScreen;

	private float nextResolutionUpdate;

	private float skinTimer;

	public bool IsGameStarted
	{
		get
		{
			if (game != null)
			{
				return game.activeZone != null;
			}
			return false;
		}
	}

	public override float uiScale => ui.canvasScaler.scaleFactor;

	protected override void Awake()
	{
		base.Awake();
		Debug.Log("Core Awake");
		Debug.Log("Version:" + version.GetText());
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
		IO.CreateDirectory(CorePath.Temp);
		SetCurrent();
		Lang.langCode = "";
		if (Application.isEditor && debug.langCode != 0)
		{
			forceLangCode = debug.langCode.ToString();
		}
		string[] array = (Application.isEditor ? debug.command.Split(',') : Environment.GetCommandLineArgs());
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			text = text.Replace("-", "").ToUpper();
			Debug.Log("Commandline args:" + text);
			if (text.StartsWith("LANG_"))
			{
				forceLangCode = text.Replace("LANG_", "");
				continue;
			}
			switch (text)
			{
			case "DEV":
				releaseMode = ReleaseMode.Debug;
				break;
			case "UPDATE_LANG":
				Lang.runUpdate = true;
				break;
			case "NOMOD":
				ModManager.disableMod = true;
				break;
			}
		}
		NewsList.dict = null;
		InvOwner.Trader = (InvOwner.Main = null);
		SpriteSheet.dict.Clear();
		SpriteSheet.loadedPath.Clear();
		SpriteReplacerAnimation.dict.Clear();
		LayerDrama.haltPlaylist = false;
		LayerDrama.keepBGM = false;
		BaseModManager.isInitialized = false;
		Net.isUploading = false;
		MapPiece.initialized = false;
		ActPlan.warning = false;
		Game.isPaused = false;
		if (Application.isEditor && BaseCore.resetRuntime)
		{
			ResetRuntime();
		}
		Window.dictData.Clear();
		LayerInventory.listInv.Clear();
		debug = Resources.Load<CoreDebug>("_Debug " + releaseMode);
		debug.enable = releaseMode != ReleaseMode.Public;
		if (useUImat)
		{
			Canvas.GetDefaultCanvasMaterial().CopyPropertiesFromMaterial(matUI);
			Canvas.GetDefaultCanvasMaterial().shader = matUI.shader;
		}
		if (!ui)
		{
			ui = Util.Instantiate<UI>("UI/UI");
		}
		if (!scene)
		{
			scene = Util.Instantiate<Scene>("Scene/Scene");
		}
		scene.screenNoMap.Activate();
		ui.ShowCover();
		canvas = ui.canvas;
		ui.OnCoreStart();
		BaseCore.BlockInput = () => ui.IsDragging;
		CommandRegistry.assemblies.Add(typeof(Core).Assembly);
		SoundData.EditorPlaySpatial = delegate(SoundData a)
		{
			game.player.chara.PlaySound(a.name);
		};
		SoundManager.funcCanPlayBGM = () => !LayerDrama.haltPlaylist && !LayerDrama.keepBGM;
		FileDragAndDrop.onDrop = textures.OnDropFile;
		MOD.langs.Clear();
		MOD.OnAddPcc = pccs.Add;
		MOD.listTalk = new TalkDataList();
		MOD.tones = new ToneDataList();
		MOD.listMaps.Clear();
		MOD.listPartialMaps.Clear();
		Portrait.modPortraitBGFs = new ModItemList<Sprite>();
		Portrait.modPortraitBGs = new ModItemList<Sprite>();
		Portrait.modPortraits = new ModItemList<Sprite>();
		Portrait.modOverlays = new ModItemList<Sprite>();
		Portrait.modFull = new ModItemList<Sprite>();
		Portrait.dictList = new Dictionary<string, List<ModItem<Sprite>>>();
		TileType.Init();
		screen.tileMap.rendererObjDummy.Init();
		Debug.Log(steam.steamworks.settings.applicationId);
	}

	private void Start()
	{
		config = CoreConfig.TryLoadConfig();
		Debug.Log("Loading Config:" + CoreConfig.path + " - " + config);
		if (config != null && config.other.disableMods)
		{
			ModManager.disableMod = true;
		}
		try
		{
			steam.Init();
		}
		catch
		{
		}
		StartCase();
	}

	public void StartCase()
	{
		Debug.Log("StartCase:" + ModManager.IsInitialized);
		if (!ModManager.IsInitialized)
		{
			mods.Init(CorePath.rootMod);
			StartCoroutine(mods.RefreshMods(delegate
			{
				MOD.actorSources.Initialize();
				StartCase();
			}, !ModManager.disableMod && !BaseCore.IsOffline && !debug.skipModSync && (config == null || config.other.syncMods)));
			return;
		}
		Debug.Log("Plugins:" + ModManager.ListPluginObject.Count);
		foreach (object item in ModManager.ListPluginObject)
		{
			Component component = item as Component;
			if (!component)
			{
				Debug.Log("no go");
				continue;
			}
			GameObject obj = component.gameObject;
			obj.SendMessage("OnStartCore");
			Component[] componentsInChildren = obj.GetComponentsInChildren<Component>();
			foreach (Component component2 in componentsInChildren)
			{
				Debug.Log(component2.GetType().Assembly.GetName().Name);
				ClassCache.assemblies.Add(component2.GetType().Assembly.GetName().Name);
			}
			break;
		}
		sources.Init();
		if (!forceLangCode.IsEmpty())
		{
			langCode = forceLangCode;
		}
		else if (config == null)
		{
			Debug.Log("Config doesn't exist.");
			try
			{
				if (debug.enable || !SteamAPI.IsSteamRunning())
				{
					BookList.dict = null;
					ui.ShowLang();
					return;
				}
				string currentGameLanguage = SteamApps.GetCurrentGameLanguage();
				if (!(currentGameLanguage == "chinese"))
				{
					if (!(currentGameLanguage == "japanese"))
					{
						BookList.dict = null;
						ui.ShowLang();
						return;
					}
					langCode = "JP";
				}
				else
				{
					langCode = "CN";
				}
			}
			catch (Exception message)
			{
				Debug.Log(message);
			}
		}
		if (debug.showSceneSelector || (Input.GetKey(KeyCode.LeftShift) && debug.enable))
		{
			ui.ShowSceneSelector();
		}
		else
		{
			Init();
		}
	}

	public void Update()
	{
		frame++;
		SoundManager.requestCount = 0;
		InputModuleEX.UpdateEventData();
		EInput.uiMousePosition = Input.mousePosition / ui.canvasScaler.scaleFactor;
		if (!initialized)
		{
			return;
		}
		ui.OnUpdate();
		debug.UpdateAlways();
		PoolManager.ignorePool = debug.ignorePool;
		if ((bool)screen.tileMap && !screen.tileMap.passBlock.mat.mainTexture)
		{
			textures.RefreshTextures();
		}
		if (useUImat)
		{
			Canvas.GetDefaultCanvasMaterial().CopyPropertiesFromMaterial(matUI);
		}
		avgDelta += (Time.smoothDeltaTime - delta) * 0.1f;
		delta = Time.smoothDeltaTime;
		if (delta > 0.1f)
		{
			delta = 0.1f;
		}
		EInput.delta = (ButtonState.delta = delta);
		if (lastFullScreen != Screen.fullScreen)
		{
			int width = Display.main.systemWidth;
			int height = Display.main.systemHeight;
			if (config != null && config.graphic.fixedResolution)
			{
				width = config.graphic.w;
				height = config.graphic.h;
			}
			if (Screen.fullScreen)
			{
				Screen.SetResolution(width, height, fullscreen: true);
			}
			lastFullScreen = Screen.fullScreen;
			nextResolutionUpdate = 0f;
		}
		if (nextResolutionUpdate <= 0f)
		{
			if (config != null && config.graphic.fixedResolution)
			{
				int w = config.graphic.w;
				int h = config.graphic.h;
				if (Screen.width != w || Screen.height != h)
				{
					Screen.SetResolution(w, h, Screen.fullScreen);
				}
			}
			if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
			{
				OnChangeResolution();
			}
			nextResolutionUpdate = 0.1f;
		}
		else
		{
			nextResolutionUpdate -= delta;
		}
		if (IsGameStarted)
		{
			game.OnUpdate();
			gameDeltaNoPause = delta * scene.actionMode.gameSpeed;
			gameDelta = (FlockChild.delta = (ui.IsPauseGame ? 0f : gameDeltaNoPause));
		}
		else
		{
			gameDelta = 0f;
		}
		scene.OnUpdate();
		if (actionsNextFrame.Count > 0)
		{
			for (int num = actionsNextFrame.Count - 1; num >= 0; num--)
			{
				actionsNextFrame[num]();
				actionsNextFrame.RemoveAt(num);
			}
		}
	}

	private void LateUpdate()
	{
		if (!initialized)
		{
			return;
		}
		if (UIButton.actionTooltip != null)
		{
			UIButton.actionTooltip();
			UIButton.actionTooltip = null;
		}
		if (actionsLateUpdate.Count > 0)
		{
			for (int num = actionsLateUpdate.Count - 1; num >= 0; num--)
			{
				actionsLateUpdate[num]();
				actionsLateUpdate.RemoveAt(num);
			}
		}
		scene.OnLateUpdate();
	}

	public void OnApplicationFocus(bool focus)
	{
		if (config == null)
		{
			return;
		}
		if (focus)
		{
			AudioListener.volume = 1f;
		}
		else if (config.other.muteBackground)
		{
			AudioListener.volume = 0f;
		}
		if (!focus || !Instance)
		{
			return;
		}
		if (actionsNextFrame.Count == 0)
		{
			actionsNextFrame.Add(delegate
			{
				textures.RefreshTextures();
				actionsNextFrame.Add(delegate
				{
					if (IsGameStarted)
					{
						if (Application.isEditor && (bool)WidgetMinimap.Instance)
						{
							WidgetMinimap.Instance.Reload();
						}
						foreach (CardRow row in sources.cards.rows)
						{
							if (row.replacer.hasChacked && row.replacer.data != null)
							{
								row.replacer.data.GetSprite();
							}
						}
					}
				});
			});
		}
		if (IsGameStarted && debug.enable)
		{
			foreach (PCC.Part value in pccs.allParts.Values)
			{
				foreach (ModItem<Texture2D> value2 in value.modTextures.Values)
				{
					value2.ClearCache();
				}
			}
			CharaActorPCC[] array = UnityEngine.Object.FindObjectsOfType<CharaActorPCC>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].provider.Rebuild();
			}
		}
		EInput.Consume();
		EInput.dragHack = 0f;
		EInput.DisableIME();
	}

	public void OnApplicationQuit()
	{
		try
		{
			Widget[] componentsInChildren = ui.widgets.GetComponentsInChildren<Widget>(includeInactive: true);
			foreach (Widget widget in componentsInChildren)
			{
				if (widget.gameObject != null)
				{
					UnityEngine.Object.Destroy(widget.gameObject);
				}
			}
			IO.PrintLog();
		}
		catch (Exception ex)
		{
			Debug.Log("Exception:" + ex);
		}
		try
		{
			if (SteamAPI.IsSteamRunning())
			{
				SteamAPI.Shutdown();
			}
		}
		catch (Exception ex2)
		{
			Debug.Log("Exception:" + ex2);
		}
		try
		{
			IO.DeleteDirectory(CorePath.Temp);
			GameIO.DeleteEmptyGameFolders(CorePath.RootSave);
		}
		catch (Exception ex3)
		{
			Debug.Log("Exception:" + ex3);
		}
	}

	public void ResetRuntime()
	{
		BaseCore.resetRuntime = false;
		BiomeProfile.forceInitialize = true;
		WidgetHotbar.registering = false;
		WidgetHotbar.registeringItem = null;
		WordGen.initialized = false;
		RecipeManager.rebuild = true;
		BookList.dict = null;
		SpawnList.allList.Clear();
	}

	public void Init()
	{
		Debug.Log("Initializing:" + langCode + "/" + forceLangCode);
		StartCoroutine("OnEndOfFrame");
		InvokeRepeating("Update100ms", 0.1f, 0.1f);
		UnityEngine.Object.DestroyImmediate(ui.layoutLang.gameObject);
		CoreConfig.Init();
		if (config.lang.IsEmpty())
		{
			config.lang = "EN";
		}
		SetLang(config.lang);
		refs.Init();
		debug.Init();
		SpriteSheet.Add("Media/Graphics/Icon/icons_48");
		Cal.Init();
		Colors.Init();
		gameSetting.Init();
		mods.InitLang();
		if (!Lang.isBuiltin)
		{
			sources.ImportSourceTexts();
		}
		PCCManager.current.Init();
		SpriteVariationManager.current.Init();
		scene.InitPass();
		textures.Init();
		textures.RefreshTextures();
		SoundManager.current.Reset();
		if (debug.validatePref)
		{
			SourcePrefInspector.Instance.ValidatePrefs();
		}
		if (debug.startScene == CoreDebug.StartScene.Title)
		{
			scene.Init(Scene.Mode.Title);
		}
		else
		{
			debug.QuickStart();
		}
		ui.HideCover(2f);
		initialized = true;
	}

	public void Quit()
	{
		Application.Quit();
		Debug.Log("Quit");
	}

	public override void ConsumeInput()
	{
		EInput.Consume();
	}

	public static Core SetCurrent(Core _current = null)
	{
		if ((bool)Instance)
		{
			return Instance;
		}
		Instance = _current ?? UnityEngine.Object.FindObjectOfType<Core>();
		BaseCore.Instance = Instance;
		if ((bool)Instance)
		{
			Instance.SetReferences();
		}
		return Instance;
	}

	public static Core GetCurrent()
	{
		return Instance ?? SetCurrent();
	}

	public void SetReferences()
	{
		CorePath.Init();
		SourceData.dataPath = CorePath.packageCore + "/Data/Source/";
		Instance = (EClass.core = (EMono.core = this));
		SkinManager._Instance = skins;
		PathManager.Instance = pathManager;
		EffectManager.Instance = effects;
		sources.InitLang();
	}

	public void SetReleaseMode(ReleaseMode m)
	{
		if (releaseMode != m)
		{
			releaseMode = m;
			debug = Resources.Load<CoreDebug>("_Debug " + releaseMode);
		}
	}

	private IEnumerator OnEndOfFrame()
	{
		while (true)
		{
			yield return new WaitForEndOfFrame();
			if ((bool)screen.guide)
			{
				screen.guide.OnEndOfFrame();
			}
		}
	}

	private void Update100ms()
	{
		if (IsGameStarted)
		{
			game.updater.Update100ms();
		}
	}

	public void OnChangeResolution()
	{
		if (config != null)
		{
			config.OnChangeResolution();
		}
		screen.RefreshScreenSize();
		lastScreenWidth = Screen.width;
		lastScreenHeight = Screen.height;
		IChangeResolution[] componentsInChildren = ui.GetComponentsInChildren<IChangeResolution>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].OnChangeResolution();
		}
		scene.camSupport.OnChangeResolution();
	}

	public void Halt()
	{
	}

	public override void FreezeScreen(float duration = 0.3f)
	{
		ui.FreezeScreen(duration);
	}

	public override void UnfreezeScreen()
	{
		ui.UnfreezeScreen();
	}

	public override void RebuildBGMList()
	{
		refs.RebuildBGMList();
	}

	public override void StopEventSystem(Component c, Action action, float duration = 0.12f)
	{
		eventSystem.enabled = false;
		c.transform.DOScale(new Vector3(1f, 0f, 1f), duration).OnKill(delegate
		{
			eventSystem.enabled = true;
		}).OnComplete(delegate
		{
			action();
		})
			.SetEase(Ease.Linear);
	}

	public override void StopEventSystem(float duration = 0.2f)
	{
		eventSystem.enabled = false;
		TweenUtil.Tween(duration, null, delegate
		{
			eventSystem.enabled = true;
		});
	}

	public void SetLang(string langCode, bool force = false)
	{
		if (!(Lang.langCode == langCode) || !sources || !sources.langGeneral)
		{
			Lang.Init(langCode);
			AliasGen.list = null;
			NameGen.list = null;
			WordGen.initialized = false;
			sources.OnChangeLang();
			if (game != null && !IsGameStarted)
			{
				game.Kill();
			}
			config.OnSetLang();
		}
	}

	public static int[] ParseElements(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return new int[0];
		}
		string[] array = str.Replace("\n", "").Split(',');
		int[] array2 = new int[array.Length * 2];
		for (int i = 0; i < array.Length; i++)
		{
			string[] array3 = array[i].Split('/');
			array2[i * 2] = GetElement(array3[0]);
			array2[i * 2 + 1] = ((array3.Length == 1) ? 1 : int.Parse(array3[1]));
		}
		return array2;
	}

	public static int GetElement(string id)
	{
		SetCurrent();
		if (sourceElement == null)
		{
			sourceElement = UnityEngine.Object.FindObjectOfType<SourceManager>().GetComponent<SourceManager>().elements;
		}
		if (!sourceElement.initialized)
		{
			sourceElement.Init();
		}
		if (!sourceElement.alias.TryGetValue(id ?? "_void", out var value))
		{
			Debug.LogError("exception:" + id);
			value = sourceElement.rows[0];
		}
		return value.id;
	}

	public void ApplySkins()
	{
		IUISkin[] componentsInChildren = ui.GetComponentsInChildren<IUISkin>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ApplySkin();
		}
	}

	public static IniData GetElinIni()
	{
		string pathIni = CorePath.PathIni;
		string ie = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
		try
		{
			FileIniDataParser fileIniDataParser = new FileIniDataParser();
			if (!File.Exists(pathIni))
			{
				File.CreateText(pathIni).Close();
			}
			IniData iniData = fileIniDataParser.ReadFile(pathIni, Encoding.UTF8);
			if (iniData.GetKey("pass").IsEmpty())
			{
				string text = "";
				for (int i = 0; i < 4; i++)
				{
					text += ie.RandomItem();
				}
				iniData.Global["pass"] = text;
				fileIniDataParser.WriteFile(pathIni, iniData);
			}
			return iniData;
		}
		catch (Exception message)
		{
			Debug.Log(message);
			Debug.Log("exception: Failed to parse:" + pathIni);
			IO.DeleteFile(pathIni);
			File.CreateText(pathIni).Close();
			return null;
		}
	}

	public static void SaveElinIni(IniData ini)
	{
		new FileIniDataParser().WriteFile(CorePath.PathIni, ini);
	}

	public static void TryWarnMod(Action action, bool warn = true)
	{
		if (warn)
		{
			IniData ini = GetElinIni();
			if (ini.Global["agreed_usercontens_usage_terms"] != "yes")
			{
				string[] items = new string[3] { "readTerms", "agree", "disagree" };
				Dialog.List("dialogTermsOfUseUGC".lang(), items, (string j) => j, delegate(int c, string d)
				{
					switch (c)
					{
					case 0:
						LayerHelp.Toggle("custom", "terms2");
						return false;
					case 1:
						ini.Global["agreed_usercontens_usage_terms"] = "yes";
						SaveElinIni(ini);
						action();
						break;
					}
					return true;
				}, canCancel: true);
				return;
			}
		}
		action();
	}

	public static void TryWarnUpload(Action action)
	{
		IniData ini = GetElinIni();
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
					SaveElinIni(ini);
					action();
					break;
				}
				return true;
			}, canCancel: true);
		}
		else
		{
			action();
		}
	}
}
