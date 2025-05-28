using System;
using System.Collections.Generic;
using UnityEngine;

public class AM_Adv : AM_BaseGameMode
{
	public class PressedAction
	{
		public ButtonState button;

		public bool canTurbo;

		public bool willEndTurn;

		public bool waitForTurn;

		public bool hideRightInfo;

		public bool repeat;

		public bool ignoreCount;

		public Func<bool> action;

		public Act act;

		public ActPlan plan;

		public float timer;

		public float timerRepeat;

		public Vector2 axis;

		public int count;

		public void Init(Vector3 _axis)
		{
			button = null;
			action = null;
			plan = null;
			timer = 0f;
			axis = _axis;
		}

		public void Init(ButtonState _button = null)
		{
			button = _button;
			action = null;
			plan = null;
			timer = 0f;
			axis = Vector2.zero;
		}

		public void SetAction(Func<bool> _action = null, bool _canTurbo = true, bool _willEndTurn = true, bool _waitForTurn = true, bool _canRepeat = false)
		{
			plan = null;
			action = _action;
			canTurbo = _canTurbo;
			willEndTurn = _willEndTurn;
			waitForTurn = _waitForTurn;
			hideRightInfo = false;
			repeat = _canRepeat;
			ignoreCount = false;
			act = null;
		}

		public void SetPlan(ActPlan _plan)
		{
			plan = _plan;
			action = plan.GetAction();
			canTurbo = true;
			willEndTurn = plan.WillEndTurn;
			waitForTurn = true;
			hideRightInfo = plan.HideRightInfo;
			repeat = plan.canRepeat;
			ignoreCount = false;
			act = ((plan.list.Count == 1) ? plan.list[0].act : null);
		}

		public bool IsPressing()
		{
			if (button != null)
			{
				if (!button.down && !button.pressing)
				{
					return button.clicked;
				}
				return true;
			}
			if (axis != Vector2.zero)
			{
				return EInput.axis == axis;
			}
			return false;
		}
	}

	public static float turbo;

	public static int actCount;

	public bool zoomOut;

	public bool zoomOut2;

	public bool movedByKey;

	public int itemLost;

	protected int arrowIndex;

	protected float timerStartRunning;

	protected bool cursorMove;

	protected bool keepWalking;

	protected Vector3 posOrigin;

	protected Vector3 posArrow;

	protected Vector2 vArrow;

	public PressedAction pressedAction = new PressedAction();

	public PointTarget axisTarget = new PointTarget
	{
		mouse = false
	};

	public ActPlan planLeft = new ActPlan
	{
		input = ActInput.LeftMouse
	};

	public ActPlan planRight = new ActPlan
	{
		input = ActInput.RightMouse
	};

	public ActPlan planKeyboard = new ActPlan
	{
		input = ActInput.Key
	};

	public ActPlan planAll = new ActPlan
	{
		input = ActInput.AllAction
	};

	public string textWheel;

	public string textMiddle;

	private float gearAngle;

	public bool updatePlans;

	public bool isMoving;

	private Vector3 lastCamPos;

	public float rightMouseTimer;

	private bool startedRun;

	protected Vector3 clickPos;

	public PointTarget mouseTarget => EClass.scene.mouseTarget;

	public bool autorun => EClass.core.config.input.autorun;

	public bool ShouldHideTile
	{
		get
		{
			if (EClass.ui.layers.Count <= 0 && (!EClass.pc.renderer.IsMoving || EInput.rightMouse.pressing) && !(EClass.pc.ai.Current is AI_Goto) && !cursorMove && !EClass.ui.isPointerOverUI)
			{
				return EInput.axis != Vector2.zero;
			}
			return true;
		}
	}

	public override float gameSpeed
	{
		get
		{
			if (!ShouldPauseGame)
			{
				return ActionMode.GameSpeeds[EClass.game.gameSpeedIndex] * ((turbo != 0f) ? turbo : 1f);
			}
			return 1f;
		}
	}

	public override bool FixFocus
	{
		get
		{
			if (!zoomOut)
			{
				if (!EClass.core.config.test.alwaysFixCamera && !EInput.leftMouse.pressing)
				{
					if (EInput.rightMouse.pressing && actCount > 0)
					{
						if (EClass.pc.ai != Chara._NoGoalRepeat)
						{
							if (EClass.pc.ai is TaskDesignation)
							{
								return !(EClass.pc.ai as TaskDesignation).isRepeated;
							}
							return true;
						}
						return false;
					}
					return true;
				}
				return true;
			}
			return false;
		}
	}

	public override bool ShouldPauseGame
	{
		get
		{
			if (EClass.core.config.game.autopause)
			{
				return EClass.pc.HasNoGoal;
			}
			return false;
		}
	}

	public override bool AllowWheelZoom => false;

	public override float TargetZoom
	{
		get
		{
			if (!zoomOut2)
			{
				return 0.01f * (float)EClass.game.config.defaultZoom;
			}
			return 0.01f * (float)EClass.game.config.zoomedZoom;
		}
	}

	public override bool HighlightWall(Point p)
	{
		if (EClass.pc.held != null)
		{
			return TaskMine.CanMine(p, EClass.pc.held);
		}
		return false;
	}

	public override void OnActivate()
	{
		if ((bool)WidgetMouseover.Instance)
		{
			WidgetMouseover.Instance.Hide();
		}
		ActionMode.DefaultMode = this;
		pressedAction.Init();
		EClass.pc.ai.Cancel();
		EClass.screen.tileMap.RefreshHeight();
		(EClass.pc.renderer as CharaRenderer).first = true;
		EClass.pc.renderer.SetFirst(first: true, EClass.pc.pos.PositionCenter());
		EClass.screen.FocusPC();
		EClass.screen.RefreshPosition();
	}

	public override void OnDeactivate()
	{
		EndTurbo();
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (ShouldHideTile)
		{
			return;
		}
		EClass.player.currentHotItem.OnRenderTile(point, result, dir);
		base.OnRenderTile(point, result, dir);
		if (EClass.game.config.highlightArea && point.Installed != null && (EClass._zone.IsPCFaction || EClass._zone is Zone_Tent))
		{
			point.Installed.trait.OnRenderTile(point, result, dir);
		}
		if (!EClass.core.config.game.highlightEnemy)
		{
			return;
		}
		foreach (Chara item in point.ListCharas())
		{
			if (!item.IsHostile(EClass.pc))
			{
				continue;
			}
			{
				foreach (Chara chara in EClass._map.charas)
				{
					if (chara.isSynced && chara.IsHostile(EClass.pc) && !chara.IsMultisize)
					{
						Vector3 vector = chara.pos.Position();
						EClass.screen.guide.passGuideFloor.Add(vector.x, vector.y, vector.z - 0.01f, 23f, 0.3f);
					}
				}
				break;
			}
		}
	}

	public override int GetDefaultTile(Point p)
	{
		if (!p.IsSync)
		{
			return 30;
		}
		return 0;
	}

	public override void OnBeforeUpdate()
	{
		if (EClass.pc.renderer.IsMoving)
		{
			isMoving = true;
		}
		else
		{
			isMoving = false;
		}
	}

	public override void OnAfterUpdate()
	{
		Vector3 position = (mouseTarget.pos.IsValid ? mouseTarget.pos.PositionAuto() : EClass.pc.pos.PositionAuto());
		position = Camera.main.WorldToScreenPoint(position);
		position.z = 0f;
		position += EClass.ui.hud.transRightPos * Mathf.Min(0f, EClass.screen.Zoom - 1f);
		EClass.ui.hud.transRight.position = position;
		EClass.player.MarkMapHighlights();
	}

	public override void OnUpdateCursor()
	{
		RefreshArrow();
		CursorInfo info = ((EClass.ui.layers.Count == 0 && !EClass.ui.isPointerOverUI) ? CursorSystem.Instance.arrows[arrowIndex] : null);
		CursorSystem.leftIcon = null;
		bool flag = (EClass.pc.ai.IsRunning || turbo != 0f || pressedAction.action != null) && !EClass.pc.HasNoGoal && !(EClass.pc.ai is GoalManualMove);
		if (flag && !EClass.ui.isPointerOverUI)
		{
			CursorSystem.leftIcon = CursorSystem.IconGear;
			CursorSystem.leftIconAngle = (int)(gearAngle / 45f) * 45;
		}
		if (!cursorMove && (mouseTarget.hasTargetChanged || !mouseTarget.pos.Equals(planLeft.pos) || planLeft.performed || planRight.performed))
		{
			updatePlans = true;
		}
		if (!ShouldHideTile)
		{
			if (planLeft.HasAct)
			{
				info = planLeft.CursorIcon;
			}
			else if (planRight.HasAct)
			{
				info = planRight.CursorIcon;
			}
		}
		gearAngle += Core.gameDelta * 200f;
		CursorSystem.SetCursor(info);
		if ((updatePlans && !flag) || EClass.pc.IsDisabled)
		{
			UpdatePlans();
		}
		if (CursorSystem.ignoreCount < 0)
		{
			UpdateLangWheel();
			EClass.ui.hud.transRight.SetActive(enable: true);
			bool flag2 = !ShouldHideTile && !pressedAction.hideRightInfo && !EClass.ui.contextMenu.isActive;
			EClass.ui.hud.textLeft.SetActive(flag2 && planLeft.ShowAct);
			EClass.ui.hud.textRight.SetActive(flag2 && planRight.ShowAct);
			EClass.ui.hud.textMiddle.SetActive(flag2 && textMiddle != null);
			EClass.ui.hud.textWheel.SetActive(flag2 && textWheel != null);
			if (EClass.ui.hud.textLeft.gameObject.activeSelf)
			{
				EClass.ui.hud.textLeft.rectTransform.anchoredPosition = EClass.ui.hud.leftTextPos * Mathf.Max(1f, EClass.screen.Zoom) / EClass.core.uiScale;
			}
			if (EClass.ui.hud.textRight.gameObject.activeSelf)
			{
				EClass.ui.hud.textRight.rectTransform.anchoredPosition = EClass.ui.hud.rightTextPos * Mathf.Max(1f, EClass.screen.Zoom) / EClass.core.uiScale;
			}
			if (EClass.ui.hud.textMiddle.gameObject.activeSelf)
			{
				EClass.ui.hud.textMiddle.rectTransform.anchoredPosition = EClass.ui.hud.wheelTextPos * Mathf.Max(1f, EClass.screen.Zoom) / EClass.core.uiScale;
			}
			if (EClass.ui.hud.textWheel.gameObject.activeSelf)
			{
				EClass.ui.hud.textWheel.rectTransform.anchoredPosition = (EClass.ui.hud.textMiddle.gameObject.activeSelf ? EClass.ui.hud.wheelTextPos2 : EClass.ui.hud.wheelTextPos) * Mathf.Max(1f, EClass.screen.Zoom) / EClass.core.uiScale;
			}
			EClass.ui.hud.imageRight.SetActive(flag2 && planRight.HasAct);
		}
	}

	public void UpdatePlans()
	{
		planLeft.Update(mouseTarget);
		planRight.Update(mouseTarget);
		updatePlans = false;
	}

	public void UpdateLangWheel()
	{
		textWheel = null;
		textMiddle = null;
		if (planRight.HasAct)
		{
			if (EClass.scene.mouseTarget.pos.Distance(EClass.pc.pos) <= 1 && HotItemHeld.CanChangeHeightByWheel())
			{
				textWheel = "textWheel_changeHeight".lang();
			}
			else if (HotItemHeld.CanRotate())
			{
				textMiddle = "textMiddle_rotate".lang(EInput.keys.mouseMiddle.key.ToString() ?? "");
			}
			EClass.ui.hud.textWheel.SetText(textWheel.IsEmpty(""));
			EClass.ui.hud.textMiddle.SetText(textMiddle.IsEmpty(""));
		}
	}

	public void ClearPlans()
	{
		planLeft.Clear();
		planRight.Clear();
		updatePlans = true;
	}

	public void OnBecomeNoGoal()
	{
		EClass.player.renderThing = null;
		if (EClass.player.hotItemToRestore != null)
		{
			if (EClass.player.currentHotItem != EClass.player.hotItemToRestore)
			{
				EClass.player.SetCurrentHotItem(null);
			}
			EClass.player.hotItemToRestore = null;
		}
		if (!EClass.core.config.game.alwaysUpdateRecipe)
		{
			EClass.game.updater.recipe.Build(EClass.pc.pos);
		}
		if (EClass.rnd(30) == 0 && EClass.pc.HasElement(1559))
		{
			List<Thing> list = EClass.pc.things.List((Thing a) => a.trait is TraitPotion, onlyAccessible: true);
			if (list.Count > 0)
			{
				EClass.pc.Drink(list.RandomItem());
				EClass.player.EndTurn();
			}
		}
		if ((int)(Time.timeSinceLevelLoad / 60f / 60f) > Player.realHour)
		{
			EClass.player.OnAdvanceRealHour();
		}
		Tutorial.TryPlayReserve();
	}

	public override void _OnUpdateInput()
	{
		if (EClass.debug.enable)
		{
			EClass.player.flags.debugEnabled = true;
		}
		if (EClass.player.willAutoSave)
		{
			EClass.game.Save(isAutoSave: true);
			EClass.player.willAutoSave = false;
		}
		if (RecipeUpdater.dirty)
		{
			RecipeUpdater.dirty = false;
			EClass.game.updater.recipe.Build(EClass.pc.pos);
		}
		if (!EInput.rightMouse.pressing)
		{
			actCount = 0;
		}
		if (EInput.rightMouse.pressing && EInput.rightMouse.pressedLong && !EInput.leftMouse.down && EClass.pc.ai is TaskDesignation && (EClass.pc.ai as TaskDesignation).isRepeated)
		{
			if (!EInput.rightMouse.pressing || !EInput.leftMouse.pressing)
			{
				rightMouseTimer = 2.5f;
			}
		}
		else
		{
			rightMouseTimer -= Core.delta;
		}
		EClass.player.waitingInput = false;
		if (EClass.pc.ai is GoalAutoCombat && EClass.pc.ai.IsRunning)
		{
			if (!EClass.debug.enable && (EClass.pc.IsCriticallyWounded() || (EClass.game.config.autoCombat.abortOnHalfHP && ((EClass.pc.Evalue(1421) > 0) ? (EClass.pc.hp + EClass.pc.mana.value) : EClass.pc.hp) < EClass.player.autoCombatStartHP / 2)))
			{
				Msg.Say("abort_lowHP");
				EClass.pc.ai.Cancel();
			}
			else if (!EClass.debug.enable && EClass.game.config.autoCombat.abortOnAllyDying && EClass.pc.party.IsCriticallyWounded())
			{
				Msg.Say("abort_allyDying");
				EClass.pc.ai.Cancel();
			}
			else if (EClass.game.config.autoCombat.abortOnItemLoss && itemLost > 0)
			{
				Msg.Say("abort_itemLoss");
				EClass.pc.ai.Cancel();
			}
		}
		itemLost = 0;
		if (EClass.pc.HasNoGoal)
		{
			if (EClass.pc.WillConsumeTurn())
			{
				EClass.player.EndTurn();
				EClass.player.invlunerable = false;
				return;
			}
			if (EClass.pc.isBerserk && !(EClass.pc.ai is GoalCombat))
			{
				EClass.pc.FindNearestNewEnemy();
				if (EClass.pc.enemy != null)
				{
					EClass.pc.SetAIImmediate(new GoalAutoCombat(EClass.pc.enemy));
				}
				EClass.player.EndTurn();
				return;
			}
			if (EClass.player.lastTurn != EClass.pc.turn)
			{
				EClass.player.lastTurn = EClass.pc.turn;
				EClass.player.invlunerable = false;
				EClass.pc.RemoveCondition<ConInvulnerable>();
				OnBecomeNoGoal();
				if (!EClass.pc.HasNoGoal)
				{
					return;
				}
			}
			EClass.player.waitingInput = true;
			if (AI_PlayMusic.keepPlaying)
			{
				Thing playingTool = AI_PlayMusic.playingTool;
				if (EInput.IsAnyKeyDown() || (playingTool.GetRootCard() != EClass.pc && (!playingTool.ExistsOnMap || playingTool.Dist(EClass.pc) > 1)))
				{
					AI_PlayMusic.CancelKeepPlaying();
					return;
				}
				UISong instance = UISong.Instance;
				if (!instance || instance.ratio > 0.85f)
				{
					EClass.pc.SetAIImmediate(new AI_PlayMusic
					{
						tool = playingTool
					});
					return;
				}
			}
		}
		if (EClass.player.returnInfo != null && EClass.player.returnInfo.askDest)
		{
			EClass.player.returnInfo.askDest = false;
			int uidDest = 0;
			List<Zone> list = EClass.game.spatials.ListReturnLocations();
			if (list == null || list.Count == 0)
			{
				EClass.player.returnInfo = null;
				Msg.Say("returnAbort");
				return;
			}
			EClass.ui.AddLayer<LayerList>().SetList2(list, (Zone a) => a.NameWithDangerLevel, delegate(Zone a, ItemGeneral b)
			{
				uidDest = a.uid;
				if (a is Zone_Dungeon)
				{
					uidDest = a.FindDeepestZone().uid;
				}
				EClass.player.returnInfo = new Player.ReturnInfo
				{
					turns = EClass.rnd(10) + 10,
					uidDest = uidDest
				};
				if (EClass.debug.instaReturn)
				{
					EClass.player.returnInfo.turns = 1;
					EClass.player.EndTurn();
				}
			}, delegate(Zone a, ItemGeneral b)
			{
				string lang = ((a is Zone_Dungeon) ? a.TextDeepestLv : "surface".lang());
				b.SetSubText(lang, 200, FontColor.Default, TextAnchor.MiddleRight);
				b.Build();
				b.button1.mainText.rectTransform.sizeDelta = new Vector2(350f, 20f);
			}).SetSize(500f)
				.SetOnKill(delegate
				{
					if (uidDest == 0)
					{
						EClass.player.returnInfo = null;
						Msg.Say("returnAbort");
					}
				})
				.SetTitles("wReturn");
			return;
		}
		if (EClass.player.haltMove)
		{
			EClass.player.haltMove = false;
			TryCancelInteraction(sound: false);
			EInput.Consume(1);
			return;
		}
		if (keepWalking && (EInput.leftMouse.down || EInput.rightMouse.down))
		{
			keepWalking = false;
			EInput.Consume(1);
			return;
		}
		if (EClass.player.showShippingResult && !EClass.ui.IsActive)
		{
			EClass.player.showShippingResult = false;
			EClass.ui.AddLayer<LayerShippingResult>().Show();
		}
		if (pressedAction.IsPressing() || keepWalking)
		{
			pressedAction.timer += Core.delta;
			if (pressedAction.action != null)
			{
				if (pressedAction.canTurbo && ((autorun && pressedAction.timer > 0.45f) || Input.GetKey(KeyCode.LeftShift)))
				{
					SetTurbo();
				}
				if (pressedAction.waitForTurn && !CanAct())
				{
					return;
				}
				if (pressedAction.count == 1 && !pressedAction.ignoreCount)
				{
					pressedAction.timerRepeat += Core.delta;
					if (pressedAction.timerRepeat < 0.3f)
					{
						return;
					}
				}
				pressedAction.count++;
				if (pressedAction.action())
				{
					if (pressedAction.willEndTurn)
					{
						EClass.player.EndTurn(consume: false);
						if (!pressedAction.repeat)
						{
							pressedAction.action = null;
						}
					}
				}
				else if (!pressedAction.repeat)
				{
					pressedAction.action = null;
				}
			}
			else if ((autorun && pressedAction.timer > 0.5f) || Input.GetKey(KeyCode.LeftShift))
			{
				SetTurbo();
			}
			return;
		}
		if (pressedAction.button != null || EInput.hasShiftChanged)
		{
			updatePlans = true;
		}
		cursorMove = false;
		EClass.player.nextMove = Vector2.zero;
		if (EClass.pc.HasNoGoal)
		{
			EndTurbo();
		}
		pressedAction.button = null;
		pressedAction.axis = Vector2.zero;
		pressedAction.count = 0;
		pressedAction.timerRepeat = 0f;
		if (EClass.player.willEndTurn)
		{
			EClass.player.willEndTurn = false;
			EClass.player.EndTurn();
			return;
		}
		if (EInput.axis != Vector2.zero)
		{
			AxisMove();
			return;
		}
		timerStartRunning = 0f;
		startedRun = false;
		if (movedByKey && EClass.pc.ai is GoalManualMove)
		{
			EClass.pc.ai.Cancel();
		}
		movedByKey = false;
		CoreConfig.InputSetting input = EClass.core.config.input;
		if (EInput.leftMouse.down)
		{
			if (TryCancelInteraction())
			{
				return;
			}
			CursorSystem.ignoreCount = 5;
			SetPressedAction(EInput.leftMouse);
		}
		if (EInput.rightMouse.down)
		{
			if (TryCancelInteraction())
			{
				return;
			}
			CursorSystem.ignoreCount = 5;
			SetPressedAction(EInput.rightMouse);
		}
		if (EInput.middleMouse.pressing || EInput.isShiftDown)
		{
			if (HotItemHeld.CanChangeHeightByWheel())
			{
				if (EInput.wheel != 0)
				{
					ActionMode.Build.ModAltitude(EInput.wheel);
					SE.Tab();
					WidgetCurrentTool.Instance.placer.Refresh();
					EInput.wheel = 0;
				}
				if (EInput.middleMouse.pressedTimer > EInput.middleMouse.clickDuration)
				{
					EInput.middleMouse.pressedTimer = 100f;
				}
			}
			else if (EClass.scene.mouseTarget.CanCycle())
			{
				if (EInput.wheel != 0)
				{
					EClass.scene.mouseTarget.CycleTarget(EInput.wheel);
					EInput.wheel = 0;
				}
				if (EInput.middleMouse.pressedTimer > EInput.middleMouse.clickDuration)
				{
					EInput.middleMouse.pressedTimer = 100f;
				}
			}
		}
		if ((EInput.middleMouse.down || EInput.middleMouse.clicked || EInput.middleMouse.pressedLong) && !EClass.ui.contextMenu.isActive)
		{
			if (EInput.middleMouse.clicked)
			{
				if (HotItemHeld.CanRotate())
				{
					HotItemHeld.taskBuild.recipe.Rotate();
					SE.Rotate();
				}
				else
				{
					DoFunc(input.middleClick);
				}
			}
			else if (EInput.middleMouse.pressedLong)
			{
				DoFunc(input.middlePressLong);
			}
		}
		if (EInput.mouse3.clicked)
		{
			DoFunc(input.mouse3Click);
		}
		else if (EInput.mouse3.pressedLong)
		{
			DoFunc(input.mouse3PressLong);
		}
		if (EInput.mouse4.clicked)
		{
			if (zoomOut)
			{
				ToggleZoom();
			}
			else
			{
				DoFunc(input.mouse4Click);
			}
		}
		else if (EInput.mouse4.pressedLong)
		{
			DoFunc(input.mouse4PressLong);
		}
		if (EInput.wheel != 0)
		{
			if (EClass.scene.mouseTarget.pos.Distance(EClass.pc.pos) <= 1 && HotItemHeld.CanChangeHeightByWheel())
			{
				if (EInput.wheel != 0)
				{
					ActionMode.Build.ModAltitude(EInput.wheel);
					SE.Tab();
					WidgetCurrentTool.Instance.placer.Refresh();
					EInput.wheel = 0;
				}
			}
			else
			{
				WidgetCurrentTool.Instance.ModSelected(-EInput.wheel);
				UpdatePlans();
			}
		}
		if (EClass.pc.HasNoGoal && EClass.player.currentHotItem.LookAtMouse)
		{
			EClass.pc.LookAt(planLeft.pos);
		}
		if (Input.GetKey(KeyCode.LeftControl))
		{
			if (Input.GetKeyDown(KeyCode.F))
			{
				EClass.ui.widgets.Toggle("Search")?.SoundActivate();
			}
			return;
		}
		switch (EInput.action)
		{
		case EAction.Search:
			EClass.ui.widgets.Toggle("Search")?.SoundActivate();
			break;
		case EAction.Meditate:
			if (EClass.pc.HasNoGoal)
			{
				EClass.pc.UseAbility("AI_Meditate", EClass.pc);
			}
			break;
		case EAction.Dump:
			TaskDump.TryPerform();
			break;
		case EAction.EmptyHand:
			if ((bool)WidgetCurrentTool.Instance)
			{
				WidgetCurrentTool.Instance.Select(-1);
			}
			break;
		case EAction.AutoCombat:
		{
			if (!EClass.scene.mouseTarget.isValid || !EClass.player.CanAcceptInput())
			{
				break;
			}
			List<Chara> list2 = EClass.scene.mouseTarget.pos.ListCharas();
			list2.Sort((Chara a, Chara b) => a.hostility - b.hostility);
			foreach (Chara item in list2)
			{
				if (item.hostility < Hostility.Friend)
				{
					EClass.pc.SetAIImmediate(new GoalAutoCombat(item));
					return;
				}
			}
			Msg.Say("noTargetFound");
			break;
		}
		case EAction.QuickSave:
			if (EClass.debug.enable || !EClass.game.principal.disableManualSave)
			{
				if (!EClass.pc.HasNoGoal)
				{
					SE.Beep();
				}
				else
				{
					EClass.game.Save();
				}
			}
			break;
		case EAction.QuickLoad:
		{
			if (!EClass.debug.enable && EClass.game.principal.disableManualSave)
			{
				SE.Beep();
				break;
			}
			string slot = Game.id;
			bool isCloud = EClass.game.isCloud;
			Game.TryLoad(slot, isCloud, delegate
			{
				EClass.core.WaitForEndOfFrame(delegate
				{
					EClass.scene.Init(Scene.Mode.None);
					Game.Load(slot, isCloud);
				});
			});
			break;
		}
		case EAction.Report:
			if (!EClass.debug.enable)
			{
				EClass.ui.ToggleFeedback();
			}
			break;
		case EAction.Wait:
		{
			if (EClass.debug.boradcast)
			{
				EClass.debug.BroadcastNext();
				break;
			}
			if (!EClass.pc.HasNoGoal)
			{
				if (EClass.pc.ai.CanManualCancel())
				{
					EClass.pc.ai.Cancel();
				}
				break;
			}
			if (EClass._zone.IsRegion)
			{
				EClass.player.EnterLocalZone();
				break;
			}
			TraitNewZone traitNewZone = EClass.pc.pos.FindThing<TraitNewZone>();
			if (traitNewZone != null && traitNewZone.CanAutoEnter())
			{
				traitNewZone.MoveZone();
				EInput.WaitReleaseKey();
			}
			else
			{
				ACT.Wait.Perform(EClass.pc);
				EClass.player.EndTurn();
			}
			break;
		}
		case EAction.Fire:
			if (!EClass.player.HasValidRangedTarget() || EInput.isShiftDown)
			{
				EClass.player.TargetRanged();
				if (EClass.player.HasValidRangedTarget())
				{
					SE.Play("lock_on");
				}
				if (EInput.keyFire.IsRepeating)
				{
					EInput.keyFire.Consume();
				}
			}
			else
			{
				if (!EClass.pc.HasNoGoal)
				{
					break;
				}
				Thing thing = EClass.player.currentHotItem.Thing;
				bool reloading = EClass.pc.HasCondition<ConReload>();
				if (thing == null || !thing.CanAutoFire(EClass.pc, EClass.player.target, reloading))
				{
					foreach (UIList.ButtonPair button in WidgetCurrentTool.Instance.list.buttons)
					{
						if (button.obj is Thing thing2 && thing2.CanAutoFire(EClass.pc, EClass.player.target, reloading))
						{
							thing = thing2;
							break;
						}
					}
				}
				if (thing == null || !thing.CanAutoFire(EClass.pc, EClass.player.target, reloading))
				{
					thing = EClass.pc.things.Find((Thing _t) => _t.isEquipped && _t.CanAutoFire(EClass.pc, EClass.player.target, reloading));
				}
				if (thing == null || !thing.CanAutoFire(EClass.pc, EClass.player.target, reloading))
				{
					break;
				}
				if (thing.HasTag(CTAG.throwWeapon))
				{
					_ = EClass.player.target.pos;
					if (ActThrow.CanThrow(EClass.pc, thing, EClass.player.target))
					{
						ACT.Throw.target = thing;
						ACT.Throw.Perform(EClass.pc, EClass.player.target, EClass.player.target.pos);
						EClass.player.EndTurn(consume: false);
						SetTurbo();
					}
				}
				else if (thing.trait is TraitAbility traitAbility)
				{
					if (traitAbility.act.CanPerform(EClass.pc, EClass.player.target, EClass.player.target.pos) && EClass.pc.UseAbility(traitAbility.act.source.alias, EClass.player.target, EClass.player.target.pos))
					{
						EClass.player.EndTurn(consume: false);
					}
				}
				else
				{
					if (!(thing.trait is TraitToolRange))
					{
						break;
					}
					EClass.pc.ranged = thing;
					if (ACT.Ranged.CanPerform(EClass.pc, EClass.player.target))
					{
						EClass.player.renderThing = thing;
						if (ACT.Ranged.Perform(EClass.pc, EClass.player.target))
						{
							EClass.player.EndTurn(consume: false);
							SetTurbo();
						}
					}
				}
			}
			break;
		}
	}

	public void ShowAllAction()
	{
		planAll.Update(mouseTarget);
		if (planAll.HasAct)
		{
			planAll.ShowContextMenu();
			return;
		}
		if (EClass._zone.IsRegion)
		{
			EClass.ui.ToggleLayer<LayerTravel>();
			return;
		}
		if (EClass.debug.godBuild)
		{
			Thing lastThing = planAll.pos.LastThing;
			if (lastThing != null && EClass.pc.pos.Distance(planAll.pos) > 1 && lastThing.W == lastThing.H)
			{
				lastThing.Rotate();
				SE.Rotate();
				return;
			}
		}
		SE.BeepSmall();
	}

	public void SetTurbo(int mtp = -1)
	{
		if (mtp == -1)
		{
			if (turbo <= EClass.setting.defaultTurbo)
			{
				turbo = EClass.setting.defaultTurbo;
			}
		}
		else
		{
			turbo = mtp;
		}
	}

	public void EndTurbo()
	{
		turbo = 0f;
	}

	public void ToggleZoom()
	{
		EClass.screen.focusPos = null;
		zoomOut2 = !zoomOut2;
		SE.Play(zoomOut2 ? "zoomOut" : "zoomIn");
		EClass.ui.widgets.OnChangeActionMode();
		ClearPlans();
	}

	public bool TryCancelInteraction(bool sound = true)
	{
		if (!EClass.pc.HasNoGoal && EClass.pc.ai.IsRunning)
		{
			if (EClass.pc.ai.CanManualCancel())
			{
				EClass.pc.ai.Cancel();
				if (sound)
				{
					SE.CancelAction();
				}
			}
			EInput.Consume(consumeAxis: true);
			return true;
		}
		return false;
	}

	public bool CanAct()
	{
		return EClass.pc.HasNoGoal;
	}

	public void AxisMove()
	{
		Vector2 vector = ((EClass.core.config.input.altKeyAxis && !EClass._zone.IsRegion) ? Util.ConvertAxis(EInput.axis) : EInput.axis);
		if (!startedRun)
		{
			if ((autorun && timerStartRunning >= 0.7f) || Input.GetKey(KeyCode.LeftShift))
			{
				SetTurbo();
				startedRun = true;
			}
		}
		else if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			SetTurbo((turbo == 0f) ? (-1) : 0);
		}
		if (EClass.core.config.test.alwaysRun)
		{
			SetTurbo((!Input.GetKey(KeyCode.LeftShift)) ? (-1) : 0);
		}
		timerStartRunning += Core.delta;
		EClass.player.nextMove.x = 0f;
		EClass.player.nextMove.y = 0f;
		Point point = EClass.pc.pos.Copy();
		point.x += (int)vector.x;
		point.z += (int)vector.y;
		if (EClass.pc.HasNoGoal && !point.IsValid && EClass.player.CanExitBorder(point))
		{
			EClass.player.ExitBorder();
			return;
		}
		axisTarget.Update(point);
		pressedAction.Init(Vector3.zero);
		planKeyboard.Update(axisTarget);
		if (planKeyboard.HasAct)
		{
			if (planKeyboard.list[0].act.ResetAxis && EClass.pc.ai is GoalManualMove)
			{
				EInput.forbidAxis = EInput.axis;
				EClass.pc.ai.Cancel();
			}
			else
			{
				pressedAction.axis = EInput.axis;
				pressedAction.SetAction(planKeyboard.GetAction(), _canTurbo: true, planKeyboard.WillEndTurn, _waitForTurn: true, planKeyboard.canRepeat);
			}
			return;
		}
		EClass.player.nextMove.x = vector.x;
		EClass.player.nextMove.y = vector.y;
		if (!(EClass.pc.ai is GoalManualMove) && EClass.pc.HasNoGoal && GoalManualMove.CanMove())
		{
			SetManualMove();
		}
		movedByKey = true;
	}

	public void SetManualMove()
	{
		if (!EClass.player.TooHeavyToMove())
		{
			SetTurbo(EInput.isShiftDown ? (-1) : 0);
			EClass.pc.SetAIImmediate(new GoalManualMove());
			cursorMove = true;
		}
	}

	public void SetPressedAction(ButtonState button)
	{
		if (updatePlans)
		{
			UpdatePlans();
		}
		bool flag = button == EInput.leftMouse;
		pressedAction.Init(button);
		if (isMouseOnMap)
		{
			if (!flag)
			{
				if (planRight.HasAct)
				{
					pressedAction.SetPlan(planRight);
				}
				else
				{
					SE.Play("noAction");
				}
				return;
			}
			if (planLeft.HasAct)
			{
				pressedAction.SetPlan(planLeft);
				return;
			}
		}
		if (!flag)
		{
			return;
		}
		Point pos = ((!isMouseOnMap) ? null : base.hit.Copy());
		if (pos != null && (EClass.pc.pos.Equals(pos) || (EClass.pc.pos.Distance(pos) == 1 && pos.cell.blocked)))
		{
			return;
		}
		planLeft.Update(mouseTarget);
		if (!planLeft.HasAct)
		{
			clickPos = Input.mousePosition;
			pressedAction.SetAction(() => PressedActionMove(pos), _canTurbo: false, _willEndTurn: false, _waitForTurn: false);
			pressedAction.repeat = zoomOut;
			pressedAction.ignoreCount = true;
		}
	}

	public bool PressedActionMove(Point pos)
	{
		if (EClass.player.TooHeavyToMove())
		{
			return false;
		}
		if (pos != null)
		{
			if (EInput.leftMouse.clicked)
			{
				if (EClass.pc.pos.Equals(pos) || !pos.IsValid)
				{
					EClass.player.nextMove = Vector2.zero;
					EClass.pc.ai.Cancel();
				}
				else if (PathManager.Instance.RequestPathImmediate(EClass.pc.pos, pos, EClass.pc).HasPath && (zoomOut || (!pos.Equals(GoalManualMove.lastPoint) && !pos.Equals(GoalManualMove.lastlastPoint))))
				{
					if (!EClass.pc.IsEnemyOnPath(pos))
					{
						EClass.pc.SetAIImmediate(new AI_Goto(pos, 0));
					}
				}
				else
				{
					EClass.player.nextMove = Vector2.zero;
					EClass.pc.ai.Cancel();
				}
				return false;
			}
			if (EInput.rightMouse.down && EClass.core.config.input.autowalk)
			{
				keepWalking = true;
			}
		}
		if (autorun)
		{
			if (Vector2.Distance(posOrigin, posArrow) > EClass.core.config.game.runDistance)
			{
				SetTurbo();
			}
			else if (turbo != 0f && !Input.GetKey(KeyCode.LeftShift) && !EClass.core.config.input.keepRunning && !zoomOut && Vector2.Distance(posOrigin, posArrow) < 1.2f)
			{
				EndTurbo();
			}
		}
		if (pos != null && !cursorMove)
		{
			Vector2Int vector2Int = new Vector2Int(pos.x - EClass.pc.pos.x, pos.z - EClass.pc.pos.z);
			if (Mathf.Abs(vector2Int.x) > 1 || Mathf.Abs(vector2Int.y) > 1)
			{
				int num = Mathf.Max(Mathf.Abs(vector2Int.x), Mathf.Abs(vector2Int.y));
				vector2Int.x /= num;
				vector2Int.y /= num;
			}
			EClass.player.nextMove = vector2Int;
			Point.shared.Set(EClass.pc.pos.x + vector2Int.x, EClass.pc.pos.z + vector2Int.y);
		}
		else
		{
			EClass.player.nextMove = vArrow;
			Point.shared.Set(EClass.pc.pos.x + (int)vArrow.x, EClass.pc.pos.z + (int)vArrow.y);
		}
		if (EClass.pc.IsEnemyOnPath(Point.shared))
		{
			if (EClass.pc.ai is GoalManualMove)
			{
				EClass.pc.ai.Cancel();
			}
			return true;
		}
		if (!Point.shared.IsInBounds)
		{
			if (EClass.pc.HasNoGoal && EClass.player.CanExitBorder(Point.shared))
			{
				EClass.player.ExitBorder();
				return false;
			}
			if (EClass.pc.ai is GoalManualMove)
			{
				EClass.pc.ai.Cancel();
			}
			return true;
		}
		if (!(EClass.pc.ai is GoalManualMove))
		{
			if (EClass.pc.HasNoGoal)
			{
				if (GoalManualMove.CanMove())
				{
					SetTurbo(EInput.isShiftDown ? (-1) : 0);
					EClass.pc.SetAIImmediate(new GoalManualMove());
				}
				cursorMove = true;
			}
		}
		else
		{
			cursorMove = true;
		}
		return true;
	}

	public virtual void RefreshArrow()
	{
		bool flag = zoomOut || EInput.rightMouse.pressedLong;
		if (flag && mouseTarget.pos.Equals(EClass.pc.pos))
		{
			vArrow = Vector2.zero;
			EClass.player.nextMove = vArrow;
			return;
		}
		posOrigin = (flag ? EClass.pc.pos.PositionCenter() : CursorSystem.posOrigin);
		posArrow = CursorSystem.position;
		float num;
		if (cursorMove || EClass.pc.pos.Distance(base.hit) > 1)
		{
			num = Util.GetAngle(posArrow.x - posOrigin.x, posArrow.y - posOrigin.y) + 90f + 22.5f;
		}
		else
		{
			num = Util.GetAngle(EClass.pc.pos.x - base.hit.x, EClass.pc.pos.z - base.hit.z) - 22.5f;
			if (num < 0f)
			{
				num = 360f + num;
			}
		}
		if ((bool)WidgetUnityChan.Instance)
		{
			WidgetUnityChan.Instance.Refresh(num);
		}
		if (clickPos != Vector3.zero)
		{
			if (Vector3.Distance(Input.mousePosition, clickPos) < EClass.core.config.game.angleMargin)
			{
				return;
			}
			clickPos = Vector3.zero;
		}
		vArrow = Vector2.zero;
		int _angle = 0;
		Action<int, int, int, int> action = delegate(int x, int y, int i, int a)
		{
			vArrow.x = x;
			vArrow.y = y;
			arrowIndex = i;
			_angle = -a;
		};
		if (num < 45f || num >= 360f)
		{
			action(-1, -1, 0, 0);
		}
		else if (num < 90f)
		{
			action(-1, 0, 1, 35);
		}
		else if (num < 135f)
		{
			action(-1, 1, 2, 90);
		}
		else if (num < 180f)
		{
			action(0, 1, 3, 145);
		}
		else if (num < 225f)
		{
			action(1, 1, 4, 180);
		}
		else if (num < 270f)
		{
			action(1, 0, 5, 215);
		}
		else if (num < 315f)
		{
			action(1, -1, 6, 270);
		}
		else
		{
			action(0, -1, 7, 325);
		}
	}
}
