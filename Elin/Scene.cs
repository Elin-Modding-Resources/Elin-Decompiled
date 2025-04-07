using System;
using System.Collections.Generic;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine;
using UnityEngine.UI;

public class Scene : EMono
{
	public enum Mode
	{
		None,
		Title,
		StartGame,
		Zone
	}

	public Vector2[] test;

	public static bool isAnnounced;

	public static Point HitPoint = new Point();

	public static Point ClickPoint = new Point();

	public static bool skipAnime;

	public Transform transFocus;

	public Transform transBlizzard;

	public Transform transAudio;

	public Vector3 posAudioListener;

	public AudioListener audioListener;

	public Camera cam;

	public Camera camTreasure;

	public CameraSupport camSupport;

	public VFX firefly;

	public VFX fireflyNight;

	public VFX star;

	public ParticleSystem rain;

	public ParticleSystem snow;

	public ParticleSystem ether;

	public ParticleSystem godray;

	public ParticleSystem godrayDust;

	public ParticleSystem blossom;

	public ParticleSystem etherBlossom;

	public ParticleSystem[] blossoms;

	public ParticleSystem[] blizzards;

	public FlockController flock;

	public MeshRenderer skyBG;

	public MeshRenderer skyPlane;

	public SceneProfile profile;

	public CameraFilterPack_Atmosphere_Rain filterRain;

	public ActionMode actionMode;

	public SoundSource sfxRain;

	public SoundSource sfxWind;

	public SoundSource sfxSea;

	public SoundSource sfxFire;

	public SoundSource sfxEther;

	public GameScreen screenElin;

	public GameScreenElona screenElona;

	public GameScreenNoMap screenNoMap;

	public PopperManager popper;

	public EloMapActor elomapActor;

	public SpriteRenderer srTarget;

	public Tileset tileset;

	public Material matFloorEx;

	public DamageTextRenderer damageTextRenderer = new DamageTextRenderer();

	public ParticleSystem psFoot;

	public ParticleSystem psSmoke;

	public ParticleSystem psRainSplash;

	public ParticleSystem psRainSplashWater;

	public ParticleSystem psFey;

	public MeshPass[] passes;

	[NonSerialized]
	public GameSetting.RenderSetting.MapBGSetting bg;

	[NonSerialized]
	public Mode mode;

	[NonSerialized]
	public float timeRatio;

	[NonSerialized]
	public List<GameObject> loadedPrefabs = new List<GameObject>();

	public PointTarget mouseTarget = new PointTarget();

	public List<ISyncScreen> syncList = new List<ISyncScreen>();

	[NonSerialized]
	public bool paused;

	public long syncFrame;

	public Dictionary<Card, ActorEx> dictActorEx = new Dictionary<Card, ActorEx>();

	[NonSerialized]
	public bool hideBalloon;

	public EloMap elomap => elomapActor.GetEloMap();

	private void Awake()
	{
		InvokeRepeating("RefreshActorEx", 0.5f, 0.5f);
	}

	public void TryWarnMacScreen()
	{
		if (!EMono.core.config.ignoreParallelsWarning && SystemInfo.graphicsDeviceName.Contains("Parallels Display"))
		{
			Dialog.TryWarn("warn_parallels", delegate
			{
				Application.OpenURL(Lang.isJP ? "https://ylvania.org/elin_dev.html" : "https://ylvania.org/elin_dev_e.html");
			}, yes: false);
		}
	}

	public void TryWarnLinuxMod()
	{
		if (!EMono.core.config.ignoreLinuxModWarning)
		{
			Dialog.TryWarn("warn_linuxMod", delegate
			{
				Application.OpenURL(Lang.isJP ? "https://ylvania.org/elin_dev.html" : "https://ylvania.org/elin_dev_e.html");
			}, yes: false);
		}
	}

	public void Init(Mode newMode)
	{
		Debug.Log("Scene.Init:" + newMode);
		EMono.ui.RemoveLayers();
		BuildMenu.Deactivate();
		ScreenFlash.Reset();
		mode = newMode;
		switch (newMode)
		{
		case Mode.None:
			if (EMono.game != null)
			{
				EMono.game.Kill();
			}
			EMono.ui.RemoveLayers();
			screenNoMap.Activate();
			break;
		case Mode.Title:
			SoundManager.bgmVolumeMod = (SoundManager.bgmDumpMod = 0f);
			if (EMono.game != null)
			{
				EMono.game.Kill();
			}
			EMono.ui.AddLayer<LayerTitle>();
			if (!isAnnounced)
			{
				isAnnounced = true;
				EMono.ui.AddLayer("LayerAnnounce").SetOnKill(TryWarnMacScreen);
			}
			else
			{
				TryWarnMacScreen();
			}
			ActionMode.Title.Activate();
			screenNoMap.Activate();
			break;
		case Mode.StartGame:
			EMono.ui.RemoveLayer<LayerTitle>();
			EMono.ui.ShowCover();
			Init(Mode.Zone);
			break;
		case Mode.Zone:
			EMono.player.target = null;
			UIBook.str_pc = EMono.pc.NameSimple;
			if (EMono.player.zone == null)
			{
				Player obj = EMono.player;
				Zone zone = (EMono.pc.currentZone = EMono.pc.homeZone);
				obj.zone = zone;
			}
			EMono.core.config.game.ignoreWarnCrime = (EMono.core.config.game.ignoreWarnMana = (EMono.core.config.game.ignoreWarnDisassemble = (EMono.core.config.game.ignoreWarnSlaughter = false)));
			EMono.game.updater.Reset();
			CellDetail.count = 0;
			skipAnime = true;
			EMono.player.baseActTime = EMono.setting.defaultActPace;
			EMono.player.Agent.renderer.isSynced = true;
			EMono.player.Agent.currentZone = EMono.player.zone;
			Point._screen = (EMono.player.zone.IsRegion ? ((BaseGameScreen)screenElona) : ((BaseGameScreen)screenElin));
			EMono.player.zone.Activate();
			EMono.player.RefreshEmptyAlly();
			UpdateTimeRatio();
			EMono.world.weather.RefreshWeather();
			EMono.screen.Deactivate();
			Popper.scale = (EMono._zone.IsRegion ? new Vector3(1.7f, 1.7f, 1f) : Vector3.one);
			if (!EMono.pc.IsInActiveZone)
			{
				ActionMode.View.Activate();
			}
			else if (EMono.player.zone is Region)
			{
				ActionMode.Region.Activate();
			}
			else
			{
				ActionMode.Adv.Activate();
			}
			if (EMono.game.altCraft)
			{
				if (EMono._zone.IsRegion)
				{
					if ((bool)EMono.ui.layerFloat.GetLayer<LayerCraftFloat>())
					{
						EMono.ui.layerFloat.RemoveLayer<LayerCraftFloat>();
					}
				}
				else if (!EMono.ui.layerFloat.GetLayer<LayerCraftFloat>())
				{
					EMono.ui.layerFloat.AddLayer<LayerCraftFloat>();
				}
			}
			EMono.screen.tileMap.activeCount = 0;
			EMono.screen.SetZoom(EMono.screen.TargetZoom);
			EMono.player.RefreshCurrentHotItem();
			EMono.screen.FocusPC();
			EMono.game.updater.recipe.Build(EMono.pc.pos);
			WidgetDate.Refresh();
			WidgetMenuPanel.OnChangeMode();
			EMono.player.hotbars.ResetHotbar(2);
			if (EMono.ui.hud.imageCover.gameObject.activeSelf && !EMono.player.simulatingZone)
			{
				EMono.ui.HideCover(4f);
			}
			if (!EMono.core.debug.ignoreAutoSave && EMono.core.config.game.autoSave && EMono.game.countLoadedMaps > 3 && !EMono.player.simulatingZone)
			{
				EMono.game.Save();
			}
			foreach (Thing thing in EMono._map.things)
			{
				if (thing.IsInstalled)
				{
					thing.trait.TryToggle();
				}
			}
			EMono.core.config.ApplyGrading();
			if (EMono.player.onStartZone != null)
			{
				EMono.player.onStartZone();
				EMono.player.onStartZone = null;
			}
			ActionMode.LastBuildMode = null;
			if (ActionMode.Adv.IsActive && (EInput.leftMouse.pressing || EInput.axis != Vector2.zero))
			{
				ActionMode.Adv.SetPressedAction(EInput.leftMouse);
				ActionMode.Adv.RefreshArrow();
				ActionMode.Adv.SetManualMove();
			}
			if (EMono.pc.IsAliveInCurrentZone)
			{
				EMono.pc.RecalculateFOV();
			}
			EMono.player.flags.OnEnterZone();
			flock.SetSpawnType(EMono._zone.FlockType);
			if (!EMono.player.simulatingZone)
			{
				if (EMono._zone == EMono.pc.homeZone)
				{
					EMono.pc.faith.Revelation("welcome");
				}
				if (EMono._zone.Boss != null && EMono._zone.Boss.ExistsOnMap)
				{
					Msg.Say("beware", EMono._zone.Name, EMono._zone.Boss.NameBraced);
				}
				EMono.game.quests.OnEnterZone();
			}
			EInput.keyWait.Consume();
			if (EMono.player.questTracker && !EMono.ui.widgets.GetWidget("QuestTracker"))
			{
				EMono.ui.widgets.Activate("QuestTracker");
			}
			if ((EMono._zone is Zone_Town || EMono._zone is Zone_Tent) && !EMono._zone.isMapSaved && !EMono.debug.ignoreAutoSave)
			{
				EMono.game.Save();
			}
			break;
		}
		etherBlossom.SetActive(mode == Mode.Zone && EMono._zone is Zone_WindRest);
	}

	public void OnKillGame()
	{
		actionMode.Deactivate();
		mouseTarget.Clear();
		hideBalloon = false;
		actionMode = null;
		PCC.PurgeCache();
		Clear();
		flock.Reset();
		elomapActor.OnKillGame();
	}

	public void Clear()
	{
		foreach (ISyncScreen sync in syncList)
		{
			sync.OnLeaveScreen();
		}
		syncList.Clear();
		EffectManager.Instance.KillAll();
		ClearActorEx();
		DestroyPrefabs();
	}

	public void OnUpdate()
	{
		SoundManager.speed = (EMono.core.IsGameStarted ? Mathf.Clamp(actionMode.gameSpeed * 0.75f, 1f, 2f) : 1f);
		UIButton.UpdateButtons();
		EMono.ui.RefreshActiveState();
		bool isShiftDown = EInput.isShiftDown;
		if ((bool)UIDropdown.activeInstance)
		{
			EInput.UpdateOnlyAxis();
		}
		else
		{
			EInput.Update();
		}
		CoreConfig.InputSetting input = EMono.core.config.input;
		if (EInput.middleMouse.pressing)
		{
			if (input.middleClick == CoreConfig.GameFunc.EmuShift || input.middlePressLong == CoreConfig.GameFunc.EmuShift)
			{
				EInput.isShiftDown = true;
			}
			if (input.middleClick == CoreConfig.GameFunc.EmuAlt || input.middlePressLong == CoreConfig.GameFunc.EmuAlt)
			{
				EInput.isAltDown = true;
			}
		}
		if (EInput.mouse3.pressing)
		{
			if (input.mouse3Click == CoreConfig.GameFunc.EmuShift || input.mouse3PressLong == CoreConfig.GameFunc.EmuShift)
			{
				EInput.isShiftDown = true;
			}
			if (input.mouse3Click == CoreConfig.GameFunc.EmuAlt || input.mouse3PressLong == CoreConfig.GameFunc.EmuAlt)
			{
				EInput.isAltDown = true;
			}
		}
		if (EInput.mouse4.pressing)
		{
			if (input.mouse4Click == CoreConfig.GameFunc.EmuShift || input.mouse4PressLong == CoreConfig.GameFunc.EmuShift)
			{
				EInput.isShiftDown = true;
			}
			if (input.mouse4Click == CoreConfig.GameFunc.EmuAlt || input.mouse4PressLong == CoreConfig.GameFunc.EmuAlt)
			{
				EInput.isAltDown = true;
			}
		}
		EInput.hasShiftChanged = EInput.isShiftDown != isShiftDown;
		Layer.cancelKeyDown = EInput.isCancel;
		Layer.rightClicked = EInput.rightMouse.down;
		int num = EInput.wheel;
		if (num != 0)
		{
			if (EMono.ui.isPointerOverUI)
			{
				if (!EMono.ui.GetLayer<LayerConfig>())
				{
					UIDropdown componentOf = InputModuleEX.GetComponentOf<UIDropdown>();
					if ((bool)componentOf && !UIDropdown.activeInstance)
					{
						if (num < 0)
						{
							componentOf.Next();
						}
						else if (num > 0)
						{
							componentOf.Prev();
						}
						num = (EInput.wheel = 0);
					}
					Slider componentOf2 = InputModuleEX.GetComponentOf<Slider>();
					if ((bool)componentOf2 && (bool)EMono.ui.GetLayer<LayerEditPCC>())
					{
						SE.Tab();
						componentOf2.value += (componentOf2.wholeNumbers ? ((float)num) : (0.1f * (float)num));
						num = (EInput.wheel = 0);
					}
				}
				if (!DropdownGrid.IsActive)
				{
					DropdownGrid componentOf3 = InputModuleEX.GetComponentOf<DropdownGrid>();
					if ((bool)componentOf3)
					{
						if (num < 0)
						{
							componentOf3.Next();
						}
						else if (num > 0)
						{
							componentOf3.Prev();
						}
						num = (EInput.wheel = 0);
					}
				}
				if (EMono.ui.contextMenu.isActive)
				{
					Slider componentOf4 = InputModuleEX.GetComponentOf<Slider>();
					if ((bool)componentOf4)
					{
						componentOf4.value += (componentOf4.wholeNumbers ? ((float)num) : (0.1f * (float)num));
						num = (EInput.wheel = 0);
						SE.Tab();
					}
				}
				UIScrollView componentOf5 = InputModuleEX.GetComponentOf<UIScrollView>();
				if ((bool)componentOf5)
				{
					num = (EInput.wheel = 0);
					if (componentOf5.verticalScrollbar.isActiveAndEnabled)
					{
						EMono.ui.HideMouseHint();
					}
				}
				UIButton componentOf6 = InputModuleEX.GetComponentOf<UIButton>();
				if ((bool)componentOf6 && componentOf6.onInputWheel != null)
				{
					componentOf6.onInputWheel(num);
					num = (EInput.wheel = 0);
				}
			}
			if (num != 0)
			{
				Layer layer = (EMono.ui.IsActive ? EMono.ui.GetTopLayer() : LayerAbility.Instance);
				if (layer != null && layer.windows.Count > 0)
				{
					Window window = layer.windows[0];
					if (InputModuleEX.IsPointerChildOf(window.rectTab) && !DropdownGrid.IsActive && window.NextTab(num))
					{
						num = (EInput.wheel = 0);
					}
				}
			}
		}
		if ((EInput.leftMouse.down || EInput.rightMouse.down) && (bool)WidgetMainText.Instance && WidgetMainText.Instance.box.isShowingLog && !InputModuleEX.IsPointerChildOf(WidgetMainText.Instance))
		{
			WidgetMainText.Instance._ToggleLog();
			EInput.Consume();
		}
		switch (mode)
		{
		case Mode.Title:
			if (EMono.scene.elomapActor.IsActive)
			{
				timeRatio = 0f;
				EMono.screen.UpdateShaders();
			}
			godray.SetActive(enable: false);
			UpdateCursor();
			EMono.ui.UpdateInput();
			break;
		case Mode.Zone:
		{
			if (!EMono.core.IsGameStarted)
			{
				break;
			}
			if (EMono._zone.dirtyElectricity)
			{
				EMono._zone.RefreshElectricity();
			}
			if (EMono.player.target != null && EMono.player.target.isSynced)
			{
				srTarget.enabled = true;
				Vector3 position = EMono.player.target.pos.PositionCenter();
				position.y += EMono.player.target.renderer.data.size.y;
				position.z -= 10f;
				srTarget.transform.parent.position = position;
			}
			else
			{
				srTarget.enabled = false;
			}
			EMono.player.pickupDelay -= Core.delta;
			if (EMono.player.pickupDelay < 0f)
			{
				EMono.player.pickupDelay = 0f;
			}
			actionMode.OnBeforeUpdate();
			paused = EMono.ui.IsPauseGame || actionMode.ShouldPauseGame || Game.IsWaiting;
			EMono.player.RefreshEmptyAlly();
			if (!EMono.pc.isDead)
			{
				if (AI_PlayMusic.keepPlaying && EInput.skipFrame <= 0 && Input.anyKeyDown && !Input.GetKey(KeyCode.Tab) && !Input.GetKey(KeyCode.LeftAlt))
				{
					AI_PlayMusic.CancelKeepPlaying();
				}
				actionMode.UpdateInput();
			}
			else
			{
				EMono.ui.UpdateInput();
			}
			if (EMono.pc.currentZone != EMono.game.activeZone && EMono.pc.global.transition != null)
			{
				EMono.player.flags.OnLeaveZone();
				if (!LayerDrama.IsActive())
				{
					EMono.player.MoveZone(EMono.pc.currentZone);
				}
			}
			if (EMono.pc.isDead && !EMono.player.deathDialog)
			{
				EMono.player.deathDialog = true;
				EMono.player.returnInfo = null;
				EMono.player.ignoreFallOver = 1;
				Msg.Say("diebye");
				EMono.Sound.Play("dead_pc2");
				EMono.Sound.Play("dead_pc");
				string[] list = Lang.GetList("lastWords");
				string lastWord = list.RandomItem();
				if (EMono.game.principal.permadeath)
				{
					GameIO.DeleteGame(Game.id, EMono.game.isCloud);
				}
				EMono.ui.CloseLayers();
				if ((bool)UIContextMenu.Current)
				{
					UIContextMenu.Current.Hide();
				}
				EInput.haltInput = false;
				Dialog.InputName("dialogLastword", lastWord, delegate(bool cancel, string text)
				{
					if (!cancel)
					{
						lastWord = text;
					}
					Player obj = EMono.player;
					obj.deathMsg = obj.deathMsg + Lang.space + lastWord.Bracket(1);
					if (EMono.core.config.net.enable && EMono.core.config.net.sendEvent)
					{
						Net.SendChat(EMono.pc.NameTitled, EMono.player.deathMsg, ChatCategory.Dead, Lang.langCode);
					}
					Debug.Log(lastWord);
					Msg.SetColor(Msg.colors.Talk);
					Msg.Say(lastWord.Bracket(1));
					List<string> list2 = new List<string>();
					Zone lastTown = EMono.game.spatials.Find(EMono.player.uidLastTown);
					bool addTownRevive = lastTown != null && !EMono._zone.IsInstance;
					if (!EMono.game.principal.permadeath)
					{
						if (addTownRevive)
						{
							list2.Add("pc_revive_town".lang(lastTown.Name));
						}
						list2.Add("pc_revive");
						list2.Add("pc_gameover");
					}
					else
					{
						list2.Add("pc_gameover");
					}
					Debug.Log(list2.Count);
					EMono.ui.AddLayer<LayerCover>().SetDuration(1f, delegate
					{
						EMono.ui.CloseLayers();
						Dialog.List("pc_deathChoice".lang(), list2, (string j) => j, delegate(int c, string d)
						{
							EMono.player.deathDialog = false;
							if (!EMono.game.principal.permadeath && (c == 0 || (addTownRevive && c == 1)))
							{
								EMono.pc.MakeGrave(lastWord);
								EMono.pc.Revive();
								EMono.pc.RecalculateFOV();
								Zone zone = EMono.player.spawnZone ?? EMono.pc.homeZone;
								if (addTownRevive && c == 0)
								{
									zone = lastTown;
								}
								if (EMono.game.activeZone == zone)
								{
									EMono.pc.Teleport(EMono._zone.GetSpawnPos(EMono.pc, ZoneTransition.EnterState.Return), silent: true, force: true);
								}
								else
								{
									EMono.player.deathZoneMove = true;
									EMono.pc.MoveZone(zone, ZoneTransition.EnterState.Return);
								}
								EMono.screen.FocusPC();
								Msg.Say("crawlup");
								if (EMono.player.stats.death == 0)
								{
									Tutorial.Reserve("death");
								}
								EMono.player.stats.death++;
							}
							else
							{
								EMono.game.GotoTitle(showDialog: false);
							}
							return true;
						});
					});
				});
			}
			UpdateCursor();
			UpdateTimeRatio();
			EMono.Sound.UpdateBGM();
			EMono.game.updater.Update();
			if (WidgetHotbar.dirtyCurrentItem)
			{
				EMono.player.RefreshCurrentHotItem();
			}
			if (WidgetEquip.dirty)
			{
				WidgetEquip.Redraw();
			}
			if (EMono.player.wasDirtyWeight)
			{
				foreach (Chara member in EMono.pc.party.members)
				{
					member.CalcBurden();
				}
				EMono.player.wasDirtyWeight = false;
			}
			if (!EMono.ui.IsActive)
			{
				EMono.core.screen.tileSelector.OnUpdate();
			}
			else if (EMono.ui.mouseInfo.gameObject.activeSelf)
			{
				EMono.ui.mouseInfo.SetText();
			}
			EMono.screen.Draw();
			damageTextRenderer.Flush();
			actionMode.OnAfterUpdate();
			if (EMono.player.lastTransition == null || EMono.player.simulatingZone)
			{
				break;
			}
			if (!EMono.ui.GetLayer<LayerDrama>())
			{
				switch (EMono.player.lastTransition.state)
				{
				case ZoneTransition.EnterState.Fall:
					EMono.pc.PlaySound("fall");
					EMono.pc.PlayAnime(AnimeID.FallSky);
					EMono.pc.DamageHP((20 + EMono.rnd(30) + ((EMono.player.lastTransition.lastZone is Zone_Casino) ? 1000 : 0)) / ((!EMono.pc.IsLevitating) ? 1 : 10), AttackSource.Fall);
					break;
				case ZoneTransition.EnterState.Elevator:
					EMono.pc.PlaySound("elevator");
					break;
				case ZoneTransition.EnterState.PortalReturn:
				case ZoneTransition.EnterState.Return:
				case ZoneTransition.EnterState.Teleport:
				case ZoneTransition.EnterState.Moongate:
					EMono.pc.PlayEffect("teleport");
					EMono.pc.PlaySound("return");
					break;
				default:
					SE.MoveZone();
					break;
				case ZoneTransition.EnterState.Encounter:
					break;
				}
			}
			bool flag = (EMono.rnd(5) == 0 && EMono.pc.burden.GetPhase() >= 3) || EMono.pc.burden.GetPhase() >= 4;
			if (EMono.player.lastTransition.lastZone is Region)
			{
				flag = false;
			}
			if (EMono.player.ignoreFallOver > 0)
			{
				flag = flag;
				EMono.player.ignoreFallOver--;
			}
			if (flag)
			{
				EMono.pc.Stumble();
			}
			EMono.player.lastTransition = null;
			break;
		}
		default:
			UpdateCursor();
			EMono.ui.UpdateInput();
			break;
		}
	}

	public void OnLateUpdate()
	{
		DrawPass();
		CursorSystem.Instance.Draw();
	}

	public void UpdateCursor()
	{
		CursorSystem.position = EInput.mposWorld;
		CursorSystem.position.z = -100f;
		if (EMono.core.IsGameStarted)
		{
			CursorSystem.posOrigin = EMono.screen.position.SetZ(-100f);
			actionMode.OnUpdateCursor();
		}
		else
		{
			CursorSystem.SetCursor();
		}
	}

	public void UpdateTimeRatio()
	{
		float num = ((EMono._map.config.hour != -1) ? ((float)EMono._map.config.hour) : ((float)EMono.world.date.hour + (float)EMono.world.date.min / 60f));
		if (num > 12f)
		{
			num = 24f - num;
		}
		timeRatio = Mathf.Clamp(num * 100f / 24f * 0.01f + EMono.setting.dayRatioMod, 0f, 1f);
	}

	public void OnChangeHour()
	{
		UpdateTimeRatio();
		firefly.OnChangeHour();
		fireflyNight.OnChangeHour();
		star.OnChangeHour();
		if ((bool)EMono.screen.pcOrbit)
		{
			EMono.screen.pcOrbit.OnChangeMin();
		}
		EMono.screen.RefreshSky();
		EMono.screen.OnChangeHour();
		foreach (Transform componentsInDirectChild in EMono.ui.rectDynamic.GetComponentsInDirectChildren<Transform>())
		{
			if (componentsInDirectChild.gameObject.tag != "IgnoreDestroy" && !componentsInDirectChild.GetComponent<TC>())
			{
				UnityEngine.Object.Destroy(componentsInDirectChild.gameObject);
			}
		}
	}

	public void RefreshBG()
	{
		bg = EMono.setting.render.bgs[EMono._map.config.bg];
		if (EMono._map.config.bg == MapBG.Sky && EMono._zone.IsSnowZone)
		{
			bg = EMono.setting.render.bgs[MapBG.Snow];
		}
		skyBG.SetActive(bg.skyBox);
		skyBG.sharedMaterial = bg.mat;
		skyPlane.SetActive(bg.plane);
		skyPlane.sharedMaterial = bg.mat;
		cam.clearFlags = ((!EMono.core.config.graphic.alwaysClearCamera && bg.skyBox) ? CameraClearFlags.Depth : CameraClearFlags.Color);
	}

	public void FocusAndPause(Point pos, int a)
	{
		if (a >= 1)
		{
			EMono.core.screen.Focus(pos);
		}
		if (a >= 2)
		{
			ActionMode.Sim.Pause();
		}
	}

	public void ApplyZoneConfig()
	{
		MapConfig conf;
		if (EMono.core.IsGameStarted)
		{
			conf = EMono._map.config;
			LiquidProfile.Apply(conf.idLiquid);
			RefractionProfile.Apply(conf.idRefraction);
			BaseTileMap tileMap = screenElin.tileMap;
			SetOcean(tileMap.passLiquid.mat);
			SetOcean(tileMap.passFloorWater.mat);
			SetOcean(tileMap.passAutoTileWater.mat);
			ScreenGrading grading = camSupport.grading;
			ScreenGradingProfile.Lut lut = grading.lut;
			grading.profile.funcOverlay = () => EMono.scene.profile.overlay;
			if (conf.idLut == "None")
			{
				lut.tex = null;
			}
			else
			{
				lut.tex = Resources.Load<Texture2D>("Scene/Profile/Lut/" + conf.idLut);
				lut.Brightness = conf.lutBrightness;
				lut.Saturation = conf.lutSaturation;
				lut.Contrast = conf.lutContrast;
				lut.blend = conf.lutBlend;
			}
			grading.SetGrading();
		}
		void SetOcean(Material m)
		{
			m.SetFloat("_GradientWater", EMono.core.config.graphic.gradientWater ? EMono.core.config.graphic.gradientWaterLevel : 0f);
			if (conf.seaDir != 0)
			{
				m.EnableKeyword("WAVE_ON");
				Vector4[] array = new Vector4[4]
				{
					new Vector4(-2f, -1f, -1f, 1f),
					new Vector4(1f, 0f, 1f, 1f),
					new Vector4(2f, 1f, -1f, 1f),
					new Vector4(-1f, 0f, 1f, 1f)
				};
				m.SetVector("_WaveDir", array[conf.seaDir - 1]);
			}
			else
			{
				m.DisableKeyword("WAVE_ON");
			}
		}
	}

	public void OnToggle()
	{
		SE.ClickGeneral();
		WidgetSystemIndicator.Refresh();
		WidgetHotbar.RefreshHighlights();
	}

	public void ToggleShowRoof()
	{
		EMono.game.config.showRoof = !EMono.game.config.showRoof;
		OnToggle();
	}

	public void ToggleLight()
	{
		EMono.game.config.buildLight = !EMono.game.config.buildLight;
		OnToggle();
	}

	public void ToggleSnapFreePos()
	{
		if (!EMono.core.config.game.freePos)
		{
			SE.Beep();
			return;
		}
		EMono.game.config.snapFreePos = !EMono.game.config.snapFreePos;
		OnToggle();
	}

	public void ToggleFreePos()
	{
		if (!EMono.core.config.game.freePos)
		{
			SE.Beep();
			return;
		}
		EMono.game.config.freePos = !EMono.game.config.freePos;
		OnToggle();
	}

	public void ToggleShowWall()
	{
		EMono.game.config.showWall = !EMono.game.config.showWall;
		OnToggle();
	}

	public void ToggleRoof()
	{
		EMono.game.config.noRoof = !EMono.game.config.noRoof;
		OnToggle();
	}

	public void ToggleSlope()
	{
		EMono.game.config.slope = !EMono.game.config.slope;
		OnToggle();
		skipAnime = true;
	}

	public void ToggleTilt()
	{
		if (EMono._zone.IsRegion)
		{
			EMono.game.config.tiltRegion = !EMono.game.config.tiltRegion;
		}
		else
		{
			EMono.game.config.tilt = !EMono.game.config.tilt;
		}
		EMono.screen.RefreshTilt();
		OnToggle();
	}

	public void ToggleHighlightArea()
	{
		EMono.game.config.highlightArea = !EMono.game.config.highlightArea;
		OnToggle();
	}

	public void ToggleBirdView(bool sound = true)
	{
		SE.ClickGeneral();
		if (EMono.scene.actionMode == ActionMode.Bird)
		{
			EMono.scene.actionMode.Deactivate();
		}
		else
		{
			ActionMode.Bird.Activate();
		}
	}

	public void ToggleBalloon()
	{
		hideBalloon = !hideBalloon;
		EMono.ui.ShowBalloon(!hideBalloon);
		SE.ClickGeneral();
		WidgetSystemIndicator.Refresh();
	}

	public void ToggleMuteBGM()
	{
		EMono.Sound.ToggleMute(EMono.core.config.sound.volumeBGM);
		SE.ClickGeneral();
		WidgetSystemIndicator.Refresh();
		if (!EMono.Sound.muteBGM)
		{
			EMono.core.config.ApplyVolume();
		}
	}

	public void RebuildActorEx()
	{
		foreach (Thing thing in EMono._map.things)
		{
			if (!thing.trait.IDActorEx.IsEmpty())
			{
				AddActorEx(thing);
			}
		}
	}

	public void RefreshActorEx()
	{
		SoundManager.bgmDumpMod = 0f;
		foreach (ActorEx value in dictActorEx.Values)
		{
			value.Refresh();
		}
	}

	public void ClearActorEx()
	{
		foreach (ActorEx value in dictActorEx.Values)
		{
			value.Kill();
		}
		dictActorEx.Clear();
	}

	public void AddActorEx(Card c, Action<ActorEx> onBeforeSetOwner = null)
	{
		ActorEx actorEx = dictActorEx.TryGetValue(c);
		if (!(actorEx != null))
		{
			actorEx = Util.Instantiate<ActorEx>("Scene/Render/Actor/Ex/" + c.trait.IDActorEx);
			onBeforeSetOwner?.Invoke(actorEx);
			actorEx.SetOwner(c);
			dictActorEx.Add(c, actorEx);
		}
	}

	public void RemoveActorEx(Card c)
	{
		ActorEx actorEx = dictActorEx.TryGetValue(c);
		if (!(actorEx == null))
		{
			actorEx.Kill();
			dictActorEx.Remove(c);
		}
	}

	public void InitPass()
	{
		MeshPass[] array = passes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Init();
		}
		foreach (ModItem<Sprite> item in MOD.sprites.list)
		{
			Sprite sprite = SpriteSheet.Get(item.id);
			if ((bool)sprite)
			{
				Sprite @object = item.GetObject(new SpriteLoadOption
				{
					pivot = new Vector2(sprite.pivot.x / (sprite.bounds.size.x * 100f), sprite.pivot.y / (sprite.bounds.size.y * 100f))
				});
				SpriteSheet.dict[item.id] = @object;
			}
		}
	}

	public void DrawPass()
	{
		if (cam.enabled)
		{
			MeshPass[] array = passes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Draw();
			}
		}
		else
		{
			MeshPass[] array = passes;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].DrawEmpty();
			}
		}
		RefreshSync();
		if ((bool)EMono.screen.guide)
		{
			EMono.screen.guide.OnDrawPass();
		}
	}

	public void RefreshSync()
	{
		for (int num = syncList.Count - 1; num >= 0; num--)
		{
			ISyncScreen syncScreen = syncList[num];
			if (syncScreen.Sync != syncFrame)
			{
				syncScreen.OnLeaveScreen();
				syncList.RemoveAt(num);
			}
		}
		syncFrame++;
		RenderObject.syncFrame = syncFrame;
	}

	public Transform LoadPrefab(string id)
	{
		Transform transform = Util.Instantiate<Transform>("Scene/Prefab/" + id);
		transform.name = id;
		loadedPrefabs.Add(transform.gameObject);
		return transform;
	}

	public void DestroyPrefabs()
	{
		foreach (GameObject loadedPrefab in loadedPrefabs)
		{
			UnityEngine.Object.Destroy(loadedPrefab);
		}
		loadedPrefabs.Clear();
	}

	public void DestroyPrefab(string id)
	{
		GameObject gameObject = GameObject.Find(id);
		if ((bool)gameObject)
		{
			UnityEngine.Object.DestroyImmediate(gameObject);
		}
	}
}
