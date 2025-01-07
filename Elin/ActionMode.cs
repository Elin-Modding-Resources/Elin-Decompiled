using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActionMode : EClass
{
	public static ActionMode DefaultMode;

	public static AM_Title Title = new AM_Title();

	public static AM_Sim Sim;

	public static AM_ViewZone View;

	public static AM_Adv Adv;

	public static AM_Region Region;

	public static AM_ADV_Target AdvTarget;

	public static AM_EloMap EloMap;

	public static AM_Inspect Inspect;

	public static AM_NoMap NoMap;

	public static AM_MiniGame MiniGame;

	public static AM_NewZone NewZone;

	public static AM_Bird Bird;

	public static AM_Mine Mine;

	public static AM_Dig Dig;

	public static AM_Harvest Harvest;

	public static AM_Cut Cut;

	public static AM_StateEditor StateEditor;

	public static AM_Picker Picker;

	public static AM_Copy Copy;

	public static AM_Blueprint Blueprint;

	public static AM_Build Build;

	public static AM_CreateArea CreateArea;

	public static AM_EditArea EditArea;

	public static AM_ExpandArea ExpandArea;

	public static AM_Deconstruct Deconstruct;

	public static AM_Select Select;

	public static AM_RemoveDesignation RemoveDesignation;

	public static AM_ViewMap ViewMap;

	public static AM_Terrain Terrain;

	public static AM_Populate Populate;

	public static AM_EditMarker EditMarker;

	public static AM_Visibility Visibility;

	public static AM_Cinema Cinema;

	public static AM_Paint Paint;

	public static AM_FlagCell FlagCell;

	public static ActionMode LastBuildMode;

	public static SourceMaterial.Row lastEditorMat;

	public static bool ignoreSound;

	private static float smoothX = 0f;

	private static float smoothY = 0f;

	private static float textTimer = 0f;

	protected static Vector3 mpos;

	public static float[] GameSpeeds = new float[4] { 0f, 1f, 2f, 5f };

	public static List<TCSimpleText> simpleTexts = new List<TCSimpleText>();

	public Layer layer;

	public int brushRadius = 4;

	private static float focusTimer;

	public static float hotElementTimer;

	public static AM_Adv AdvOrRegion
	{
		get
		{
			if (!EClass._zone.IsRegion)
			{
				return Adv;
			}
			return Region;
		}
	}

	public static bool IsAdv => DefaultMode is AM_Adv;

	public virtual float gameSpeed => 1f;

	public bool IsActive => EClass.scene.actionMode == this;

	public virtual BaseTileSelector.HitType hitType => BaseTileSelector.HitType.Floor;

	public virtual BaseTileSelector.SelectType selectType => BaseTileSelector.SelectType.Multiple;

	public virtual BaseTileSelector.BoxType boxType => BaseTileSelector.BoxType.Box;

	public virtual bool ContinuousClick => false;

	public virtual int hitW => 1;

	public virtual int hitH => 1;

	public HitSummary Summary => tileSelector.summary;

	public bool Multisize
	{
		get
		{
			if (hitW == 1)
			{
				return hitW != 1;
			}
			return true;
		}
	}

	public virtual string id => GetType().Name.Split('_')[1];

	public virtual CursorInfo DefaultCursor => null;

	public virtual string idHelpTopic => "4";

	public virtual string idSound => null;

	public virtual bool enableMouseInfo => false;

	public virtual bool hideBalloon => false;

	public virtual string textHintTitle => null;

	public virtual bool AllowAutoClick => false;

	public virtual bool ShowActionHint => true;

	public virtual bool ShowMouseoverTarget => true;

	public virtual AreaHighlightMode AreaHihlight => AreaHighlightMode.None;

	public virtual bool CanSelectTile => false;

	public virtual bool CanTargetOutsideBounds => true;

	public virtual bool ShouldPauseGame => true;

	public virtual bool FixFocus => false;

	public virtual bool HideSubWidgets => true;

	public virtual bool IsBuildMode => false;

	public virtual bool ShowBuildWidgets => IsBuildMode;

	public virtual BuildMenu.Mode buildMenuMode
	{
		get
		{
			if (!IsBuildMode)
			{
				return BuildMenu.Mode.None;
			}
			return BuildMenu.Mode.Hide;
		}
	}

	public virtual bool ShouldHideBuildMenu
	{
		get
		{
			if (IsBuildMode)
			{
				return tileSelector.start != null;
			}
			return false;
		}
	}

	public virtual bool CanTargetFog => false;

	public virtual int CostMoney => 0;

	public virtual bool AllowBuildModeShortcuts => IsBuildMode;

	public virtual bool AllowMiddleClickFunc => false;

	public virtual bool AllowHotbar => true;

	public virtual bool AllowGeneralInput => true;

	public virtual bool ShowMaskedThings => false;

	public virtual int SelectorHeight => -1;

	public virtual bool AllowWheelZoom => true;

	public virtual float TargetZoom => 1f;

	public virtual BaseTileMap.CardIconMode cardIconMode => BaseTileMap.CardIconMode.None;

	public virtual BaseGameScreen TargetGameScreen => null;

	public virtual bool IsNoMap => false;

	public virtual bool UseSubMenu => false;

	public virtual bool UseSubMenuSlider => false;

	public virtual bool SubMenuAsGroup => false;

	public virtual int SubMenuModeIndex => 0;

	public virtual bool ShowExtraHint => Lang.GetList("exhint_" + GetType().Name) != null;

	public BaseTileSelector tileSelector => EClass.screen.tileSelector;

	public static void OnGameInstantiated()
	{
		LastBuildMode = null;
		Sim = new AM_Sim();
		View = new AM_ViewZone();
		Adv = new AM_Adv();
		AdvTarget = new AM_ADV_Target();
		Region = new AM_Region();
		NoMap = new AM_NoMap();
		MiniGame = new AM_MiniGame();
		EloMap = new AM_EloMap();
		NewZone = new AM_NewZone();
		Select = new AM_Select();
		Inspect = new AM_Inspect();
		Bird = new AM_Bird();
		DefaultMode = Adv;
		Mine = new AM_Mine();
		Dig = new AM_Dig();
		Harvest = new AM_Harvest();
		Cut = new AM_Cut();
		Copy = new AM_Copy();
		Blueprint = new AM_Blueprint();
		Build = new AM_Build();
		Picker = new AM_Picker();
		StateEditor = new AM_StateEditor();
		CreateArea = new AM_CreateArea();
		EditArea = new AM_EditArea();
		ExpandArea = new AM_ExpandArea();
		RemoveDesignation = new AM_RemoveDesignation();
		Deconstruct = new AM_Deconstruct();
		ViewMap = new AM_ViewMap();
		Terrain = new AM_Terrain();
		Populate = new AM_Populate();
		EditMarker = new AM_EditMarker();
		Visibility = new AM_Visibility();
		Cinema = new AM_Cinema();
		FlagCell = new AM_FlagCell();
		Paint = new AM_Paint();
	}

	public virtual int TopHeight(Point p)
	{
		return -1;
	}

	public virtual void SEExecuteSummary()
	{
		SE.Play("plan");
	}

	public virtual bool HighlightWall(Point p)
	{
		return false;
	}

	public virtual void OnClickSubMenu(int a)
	{
	}

	public virtual string OnSetSubMenuButton(int a, UIButton b)
	{
		return null;
	}

	public virtual bool IsRoofEditMode(Card c = null)
	{
		return false;
	}

	public virtual bool IsFillMode()
	{
		return false;
	}

	public virtual int GetDefaultTile(Point p)
	{
		return 0;
	}

	public virtual void OnShowExtraHint(UINote n)
	{
		string[] list = Lang.GetList("exhint_" + GetType().Name);
		if (list != null)
		{
			string[] array = list;
			foreach (string text in array)
			{
				n.AddText("NoteText_extrahint", text, Color.white);
			}
		}
	}

	public void Activate(bool toggle = true, bool forceActivate = false)
	{
		if (TargetGameScreen != null)
		{
			TargetGameScreen.Activate();
		}
		if (EClass.scene.actionMode == this && !forceActivate)
		{
			if (toggle && EClass.scene.actionMode != DefaultMode)
			{
				DefaultMode.Activate();
			}
			return;
		}
		if (ignoreSound)
		{
			ignoreSound = false;
		}
		else
		{
			EClass.Sound.Play(idSound);
		}
		EInput.Consume(0);
		ActionMode actionMode = EClass.scene.actionMode;
		EClass.scene.actionMode = this;
		if (actionMode != null)
		{
			if (actionMode.IsBuildMode && !(actionMode is AM_ViewMap))
			{
				LastBuildMode = actionMode;
			}
			actionMode.ClearSimpleTexts();
			actionMode.OnDeactivate();
			if (EClass.core.IsGameStarted)
			{
				foreach (Card item in ((IEnumerable<Card>)EClass._map.things).Concat((IEnumerable<Card>)EClass._map.charas))
				{
					item.renderer.DespawnSimpleText();
				}
			}
			_ = NoMap;
			EClass.ui.RemoveLayers();
		}
		if (IsBuildMode)
		{
			BuildMenu.Activate();
			BuildMenu.Instance.terrainMenu.Show(this);
		}
		else
		{
			BuildMenu.Deactivate();
		}
		if (!(this is AM_Adv))
		{
			EClass.ui.hud.transRight.SetActive(enable: false);
		}
		EClass.ui.hud.frame.SetActive(IsBuildMode && EClass.game.altUI);
		if (hideBalloon)
		{
			EClass.ui.ShowBalloon(enable: false);
		}
		else
		{
			EClass.ui.ShowBalloon(!EClass.scene.hideBalloon);
		}
		OnActivate();
		RefreshTexts();
		ShowLayer();
		EClass.ui.widgets.OnChangeActionMode();
		EClass.ui.extraHint.OnChangeActionMode();
		CursorSystem.leftIcon = null;
		EClass.scene.UpdateCursor();
	}

	public virtual void OnActivate()
	{
	}

	public void RefreshTexts()
	{
		if (!EClass.core.IsGameStarted)
		{
			return;
		}
		ClearSimpleTexts();
		if (EClass._zone.IsRegion)
		{
			foreach (Spatial child in EClass._zone.children)
			{
				if (!child.destryoed)
				{
					Sprite sprite = null;
					if (child.isRandomSite && !child.isConquered && child.visitCount > 0)
					{
						sprite = EClass.core.refs.tcs.spriteVisited;
					}
					else if (child.isConquered)
					{
						sprite = EClass.core.refs.tcs.spriteConquer;
					}
					else if (!child.IsPlayerFaction && child is Zone_Field && child.isDeathLocation)
					{
						sprite = EClass.core.refs.tcs.spriteDeath;
					}
					if ((bool)sprite)
					{
						TCSimpleText tCSimpleText = TCSimpleText.SpawnIcon(sprite);
						simpleTexts.Add(tCSimpleText);
						Cell cell = child.RegionPos.cell;
						tCSimpleText.transform.position = cell.GetPoint().PositionTopdown() + EClass.setting.render.tc.simpleTextPos;
					}
				}
			}
		}
		if (!IsBuildMode || !ShowMaskedThings)
		{
			return;
		}
		foreach (Card item in ((IEnumerable<Card>)EClass._map.things).Concat((IEnumerable<Card>)EClass._map.charas))
		{
			string simpleText = GetSimpleText(item);
			if (!simpleText.IsEmpty())
			{
				TCSimpleText tCSimpleText2 = TCSimpleText.Spawn();
				Popper.SetText(tCSimpleText2.tm, simpleText);
				simpleTexts.Add(tCSimpleText2);
				tCSimpleText2.transform.position = item.pos.Position() + EClass.setting.render.tc.simpleTextPos;
			}
		}
		if (!Application.isEditor)
		{
			return;
		}
		foreach (KeyValuePair<int, int> backerObj in EClass._map.backerObjs)
		{
			SourceBacker.Row row = EClass.sources.backers.map[backerObj.Value];
			string text = "Backer:" + row.id + "/" + row.Name;
			if (row.isStatic != 0)
			{
				text = "★" + text;
			}
			TCSimpleText tCSimpleText3 = TCSimpleText.Spawn();
			Popper.SetText(tCSimpleText3.tm, text);
			simpleTexts.Add(tCSimpleText3);
			Cell cell2 = EClass._map.GetCell(backerObj.Key);
			tCSimpleText3.transform.position = cell2.GetPoint().Position() + EClass.setting.render.tc.simpleTextPos;
		}
		foreach (Card card in EClass._map.Cards)
		{
			if (!card.isBackerContent)
			{
				continue;
			}
			SourceBacker.Row row2 = EClass.sources.backers.map.TryGetValue(card.c_idBacker);
			if (row2 != null)
			{
				string text2 = "Backer:" + row2.id + "/" + row2.Name;
				if (row2.isStatic != 0)
				{
					text2 = "★" + text2;
				}
				TCSimpleText tCSimpleText4 = TCSimpleText.Spawn();
				Popper.SetText(tCSimpleText4.tm, text2);
				simpleTexts.Add(tCSimpleText4);
				tCSimpleText4.transform.position = card.renderer.position + EClass.setting.render.tc.simpleTextPos;
			}
		}
	}

	public virtual string GetSimpleText(Card c)
	{
		if (c.trait is TraitRoomPlate && c.Cell.room != null)
		{
			return "#" + c.Cell.room.data.group;
		}
		return null;
	}

	public void ClearSimpleTexts()
	{
		foreach (TCSimpleText simpleText in simpleTexts)
		{
			if (simpleText != null)
			{
				PoolManager.Despawn(simpleText);
			}
		}
		simpleTexts.Clear();
	}

	public void Deactivate()
	{
		DefaultMode.Activate();
	}

	public virtual void OnDeactivate()
	{
	}

	public virtual void OnCancel()
	{
		if (!IsBuildMode || EClass.core.config.input.rightClickExitBuildMode || Input.GetKey(KeyCode.Escape))
		{
			Deactivate();
		}
	}

	public virtual void OnBeforeUpdate()
	{
	}

	public virtual void OnAfterUpdate()
	{
	}

	public virtual void OnUpdateCursor()
	{
		CursorSystem.leftIcon = null;
		CursorSystem.SetCursor();
	}

	public void OnRotate()
	{
		RotateUnderMouse();
	}

	public void SetCursorOnMap(CursorInfo cursor)
	{
		CursorSystem.SetCursor(EClass.ui.isPointerOverUI ? null : cursor);
	}

	public void UpdateInput()
	{
		mpos = Input.mousePosition;
		if (!IsNoMap)
		{
			EClass.scene.mouseTarget.Update(Scene.HitPoint);
			if ((bool)WidgetMouseover.Instance)
			{
				WidgetMouseover.Instance.Refresh();
			}
		}
		if (LayerAbility.hotElement != null)
		{
			hotElementTimer += Core.delta;
			ButtonAbility hotElement = LayerAbility.hotElement;
			if (EInput.leftMouse.down)
			{
				if (EClass.core.config.game.doubleClickToHold && hotElementTimer < 0.35f)
				{
					EClass.player.SetCurrentHotItem(new HotItemAct(hotElement.source));
					SE.SelectHotitem();
				}
				else
				{
					ButtonGrid componentOf = InputModuleEX.GetComponentOf<ButtonGrid>();
					if ((bool)componentOf && componentOf.invOwner != null && componentOf.invOwner.owner == EClass.pc && (componentOf.card == null || componentOf.card.trait is TraitAbility) && !(componentOf.invOwner is InvOwnerEquip))
					{
						if (componentOf.card != null)
						{
							componentOf.card.Destroy();
						}
						SE.Equip();
						CardBlueprint.SetNormalRarity();
						Thing thing = ThingGen.Create("ability");
						int num = ((componentOf.invOwner is InvOwnerHotbar) ? 1 : 0);
						componentOf.invOwner.Container.AddThing(thing, tryStack: false, componentOf.index, num);
						thing.c_idAbility = hotElement.source.alias;
						thing.invY = num;
						thing.invX = componentOf.index;
						WidgetCurrentTool.dirty = true;
					}
					else
					{
						SE.BeepSmall();
					}
				}
				LayerAbility.ClearHotElement();
			}
			if (EInput.rightMouse.down)
			{
				LayerAbility.ClearHotElement();
			}
		}
		else
		{
			hotElementTimer = 0f;
		}
		focusTimer += Core.delta;
		if (EClass.ui.isPointerOverUI && (EInput.leftMouse.down || EInput.rightMouse.down || (EClass.core.config.ui.autoFocusWindow && !Input.GetMouseButton(0))))
		{
			LayerInventory componentOf2 = InputModuleEX.GetComponentOf<LayerInventory>();
			if (componentOf2 != null)
			{
				LayerInventory layerInventory = LayerInventory.GetLayer(InputModuleEX.GetComponentOf<ButtonGrid>()?.card);
				if ((bool)layerInventory)
				{
					LayerInventory.highlightInv = layerInventory.invs[0].owner;
				}
				if (focusTimer > 0.1f)
				{
					if (EClass.ui.layerFloat.layers.Contains(componentOf2) && !layerInventory)
					{
						componentOf2.transform.SetAsLastSibling();
					}
					LayerAbility componentOf3 = InputModuleEX.GetComponentOf<LayerAbility>();
					if (componentOf3 != null && EClass.ui.layerFloat.layers.Contains(componentOf3))
					{
						componentOf3.transform.SetAsLastSibling();
					}
					focusTimer = 0f;
				}
			}
		}
		if ((EInput.isShiftDown && Input.GetMouseButton(0)) || (EInput.rightMouse.dragging2 && !Input.GetMouseButtonDown(1) && Input.GetMouseButton(1)))
		{
			bool rightMouse = !Input.GetMouseButtonDown(1) && Input.GetMouseButton(1);
			ButtonGrid componentOf4 = InputModuleEX.GetComponentOf<ButtonGrid>();
			if (componentOf4 != null && componentOf4.invOwner != null && componentOf4.card != null && componentOf4.invOwner.CanShiftClick(componentOf4, rightMouse) && EClass.ui.AllowInventoryInteractions)
			{
				componentOf4.invOwner.OnShiftClick(componentOf4, rightMouse);
			}
		}
		if (EInput.rightMouse.down)
		{
			LayerInventory componentOf5 = InputModuleEX.GetComponentOf<LayerInventory>();
			if (componentOf5 != null && EClass.ui.layerFloat.layers.Contains(componentOf5) && EInput.rightMouse.down && !componentOf5.mainInv && !InputModuleEX.GetComponentOf<UIButton>() && !componentOf5.windows[0].saveData.noRightClickClose)
			{
				componentOf5.Close();
			}
			if (EClass.ui.contextMenu.isActive)
			{
				EClass.ui.contextMenu.currentMenu.Hide();
				EInput.rightMouse.Consume();
				Layer.rightClicked = false;
			}
			if (DropdownGrid.IsActive && !EClass.ui.GetLayer<LayerInfo>())
			{
				DropdownGrid.Instance.Deactivate();
				EInput.rightMouse.Consume();
				Layer.rightClicked = false;
			}
		}
		if (EInput.middleMouse.down && TryShowWidgetMenu())
		{
			EInput.Consume();
		}
		CoreConfig.InputSetting input = EClass.core.config.input;
		bool flag = (EInput.mouse3.clicked && input.mouse3Click == CoreConfig.GameFunc.AllAction) || (EInput.mouse4.clicked && input.mouse4Click == CoreConfig.GameFunc.AllAction) || (EInput.mouse3.pressedLong && input.mouse3PressLong == CoreConfig.GameFunc.AllAction) || (EInput.mouse4.pressedLong && input.mouse4PressLong == CoreConfig.GameFunc.AllAction);
		if (flag || EInput.middleMouse.down || EInput.middleMouse.clicked || EInput.middleMouse.pressedLong)
		{
			UIButton componentOf6 = InputModuleEX.GetComponentOf<UIButton>();
			if ((bool)componentOf6 && componentOf6.CanMiddleClick() && (flag || EInput.middleMouse.clicked || EInput.middleMouse.pressedLong))
			{
				componentOf6.OnMiddleClick(flag);
			}
		}
		if ((bool)EClass.ui.contextMenu.currentMenu)
		{
			UIContextMenu.timeSinceClosed = 0f;
		}
		else
		{
			UIContextMenu.timeSinceClosed += Time.deltaTime;
		}
		if (EClass.ui.currentDrag != null)
		{
			EClass.ui.OnDrag();
			if (EInput.leftMouse.down || EInput.rightMouse.down || EInput.isCancel)
			{
				if (Input.GetMouseButton(0) && EInput.rightMouse.down)
				{
					EClass.ui.OnDragSpecial();
				}
				else
				{
					EClass.ui.EndDrag(EInput.rightMouse.down || EInput.isCancel);
				}
			}
		}
		ButtonState middleMouse = EInput.middleMouse;
		if (middleMouse.pressedLongAction != null)
		{
			if (middleMouse.pressedLong)
			{
				middleMouse.pressedLongAction();
				middleMouse.pressedLongAction = null;
			}
			else if (!EInput.middleMouse.pressing)
			{
				middleMouse.pressedLongAction = null;
			}
		}
		EClass.ui.UpdateInput();
		if (EInput.action == EAction.Mute)
		{
			EClass.scene.ToggleMuteBGM();
		}
		if (AllowGeneralInput && !EClass.ui.IsDragging)
		{
			if (Input.GetKeyDown(KeyCode.Tab) && !EClass.debug.debugInput && !EClass.ui.BlockInput && !EInput.waitReleaseAnyKey)
			{
				if (!EClass.ui.IsInventoryOpen)
				{
					SE.PopInventory();
				}
				EClass.ui.ToggleInventory();
			}
			if (!EClass.ui.BlockInput && !IsBuildMode && !EInput.waitReleaseAnyKey)
			{
				switch (EInput.action)
				{
				case EAction.MenuChara:
					EClass.ui.ToggleLayer<LayerChara>()?.SetChara(EClass.pc);
					break;
				case EAction.MenuJournal:
					EClass.ui.ToggleLayer<LayerJournal>();
					break;
				case EAction.MenuAbility:
					if (!EClass.debug.enable)
					{
						EClass.ui.ToggleAbility();
					}
					break;
				case EAction.MenuInventory:
					EClass.ui.ToggleInventory();
					break;
				}
			}
			if (!FixFocus)
			{
				InputMovement();
			}
			if (!EClass.ui.canvas.enabled && EInput.IsAnyKeyDown())
			{
				EClass.ui.ToggleCanvas();
				EInput.Consume();
				return;
			}
		}
		if (EInput.action == EAction.Examine && !IsBuildMode)
		{
			ButtonGrid componentOf7 = InputModuleEX.GetComponentOf<ButtonGrid>();
			Card card = null;
			if ((bool)componentOf7 && componentOf7.card != null)
			{
				card = componentOf7.card;
			}
			if (card == null)
			{
				UIItem componentOf8 = InputModuleEX.GetComponentOf<UIItem>();
				if ((bool)componentOf8 && componentOf8.refObj is Thing)
				{
					card = (Thing)componentOf8.refObj;
				}
			}
			if (card == null)
			{
				LayerCraft layerCraft = EClass.ui.GetLayer<LayerCraft>();
				if ((bool)layerCraft)
				{
					card = layerCraft.product;
				}
			}
			if (card != null)
			{
				if ((bool)EClass.ui.GetLayer<LayerInfo>())
				{
					EClass.ui.RemoveLayer<LayerInfo>();
				}
				EClass.ui.AddLayer<LayerInfo>().Set(card, _examine: true);
			}
		}
		if (EInput.buttonCtrl.clicked && !LayerDrama.Instance && EClass.scene.actionMode is AM_Adv && (bool)WidgetCurrentTool.Instance)
		{
			WidgetCurrentTool.Instance.SwitchPage();
		}
		if (EClass.ui.BlockActions)
		{
			return;
		}
		if (AllowBuildModeShortcuts && Input.GetKeyDown(KeyCode.Q) && Picker.CanActivate)
		{
			Picker.Activate();
		}
		if (EClass.debug.godBuild && EInput.isCtrlDown && EInput.wheel != 0 && Scene.HitPoint.IsValid && HotItemHeld.taskBuild == null)
		{
			Point hitPoint = Scene.HitPoint;
			bool flag2 = EInput.wheel > 0;
			if (EClass.scene.mouseTarget.card != null)
			{
				Card card2 = EClass.scene.mouseTarget.card;
				if (card2.isThing && card2.IsInstalled)
				{
					SourceMaterial.Row currentMat = (card2.isDyed ? card2.DyeMat : card2.material);
					List<SourceMaterial.Row> source = EClass.sources.materials.rows.Where((SourceMaterial.Row m) => m.thing == currentMat.thing).ToList();
					if (EInput.isShiftDown)
					{
						source = EClass.sources.materials.rows;
					}
					SourceMaterial.Row row = (flag2 ? source.NextItem(currentMat) : source.PrevItem(currentMat));
					if (row == null)
					{
						row = card2.DyeMat;
					}
					if (EInput.isAltDown)
					{
						row = lastEditorMat;
					}
					if (card2.isDyed)
					{
						card2.Dye(row);
					}
					else
					{
						card2.ChangeMaterial(row);
					}
					lastEditorMat = row;
					Msg.Say(row.GetName() + "(" + row.alias + ")");
				}
			}
			else if (hitPoint.HasObj)
			{
				SourceMaterial.Row matObj = hitPoint.cell.matObj;
				List<SourceMaterial.Row> rows = EClass.sources.materials.rows;
				SourceMaterial.Row row2 = (flag2 ? rows.NextItem(matObj) : rows.PrevItem(matObj));
				if (EInput.isAltDown)
				{
					row2 = lastEditorMat;
				}
				hitPoint.cell.objMat = (byte)row2.id;
				lastEditorMat = row2;
				Msg.Say(row2.GetName());
			}
			EInput.Consume();
		}
		if (AllowHotbar && !IsBuildMode && EClass.debug.debugHotkeys == CoreDebug.DebugHotkey.None)
		{
			if (EInput.hotkey != -1)
			{
				WidgetCurrentTool.Instance.Select(EInput.hotkey, fromHotkey: true);
				AdvOrRegion.UpdatePlans();
			}
			else if (!EClass.debug.enable && EInput.functionkey != -1)
			{
				WidgetHotbar hotbarExtra = WidgetHotbar.HotbarExtra;
				if ((bool)hotbarExtra)
				{
					hotbarExtra.TryUse(EInput.functionkey);
				}
			}
		}
		OnUpdateInput();
		if (EClass.debug.enable)
		{
			EClass.core.debug.UpdateInput();
		}
		textTimer += Core.delta;
		if (textTimer > 0.1f)
		{
			textTimer = 0f;
			RefreshTexts();
		}
		if (IsBuildMode && AllowBuildModeShortcuts)
		{
			if (EInput.middleMouse.clicked)
			{
				DoFunc(input.b_middleClick);
			}
			else if (EInput.middleMouse.pressedLong)
			{
				DoFunc(input.b_middlePressLong);
				EInput.middleMouse.Consume();
			}
			if (EInput.mouse3.clicked)
			{
				DoFunc(input.b_mouse3Click);
			}
			else if (EInput.mouse3.pressedLong)
			{
				DoFunc(input.b_mouse3PressLong);
				EInput.mouse3.Consume();
			}
			if (EInput.mouse4.clicked)
			{
				DoFunc(input.b_mouse4Click);
			}
			else if (EInput.mouse4.pressedLong)
			{
				DoFunc(input.b_mouse4PressLong);
				EInput.mouse4.Consume();
			}
		}
	}

	public void InputMovement()
	{
		float num = 0f;
		float num2 = 0f;
		Vector2 axis = EInput.axis;
		bool flag = !EClass.core.IsGameStarted;
		if ((!FixFocus && !EInput.rightMouse.pressing) || EClass.scene.actionMode.IsBuildMode)
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				axis *= 3f;
			}
			if (EClass.core.config.camera.edgeScroll)
			{
				Vector2 zero = Vector2.zero;
				if (mpos.x < 16f)
				{
					zero.x -= 1f;
				}
				if (mpos.x > (float)(Screen.width - 16))
				{
					zero.x += 1f;
				}
				if (mpos.y < 8f)
				{
					zero.y -= 1f;
				}
				if (mpos.y > (float)(Screen.height - 8))
				{
					zero.y += 1f;
				}
				if (zero != Vector2.zero)
				{
					axis += zero * (EClass.core.config.camera.sensEdge * 2f);
				}
			}
			EInput.hasAxisMoved = axis != Vector2.zero;
			_ = EInput.buttonScroll;
			if (Adv.IsActive)
			{
				_ = Adv.zoomOut;
			}
			if (EInput.buttonScroll != null && EInput.buttonScroll.pressing && (flag || !EClass.ui.BlockActions))
			{
				num = EInput.dragAmount.x * (0.1f + EClass.core.config.camera.sensDrag);
				num2 = EInput.dragAmount.y * (0.1f + EClass.core.config.camera.sensDrag);
				if (!EClass.core.config.camera.invertX)
				{
					num *= -1f;
				}
				if (!EClass.core.config.camera.invertY)
				{
					num2 *= -1f;
				}
				if (EInput.hasMouseMoved)
				{
					EInput.hasAxisMoved = true;
				}
			}
			if (EInput.hasAxisMoved)
			{
				EClass.screen.zoomPos = Vector3.zero;
			}
			if (EInput.wheel != 0 && !InputModuleEX.GetComponentOf<UIScrollView>())
			{
				InputWheel(EInput.wheel);
			}
		}
		float momentum = EClass.core.config.camera.momentum;
		if (momentum > 0f && EInput.axis == Vector2.zero)
		{
			smoothX = Mathf.Lerp(smoothX, num, Time.smoothDeltaTime * momentum);
			smoothY = Mathf.Lerp(smoothY, num2, Time.smoothDeltaTime * momentum);
		}
		else
		{
			smoothX = num;
			smoothY = num2;
		}
		EClass.screen.ScrollMouse(smoothX * 0.1f, smoothY * 0.1f);
		if (EInput.hasAxisMoved)
		{
			OnScroll();
			EClass.screen.ScrollAxis(axis);
		}
	}

	public virtual void InputWheel(int wheel)
	{
		if (AllowWheelZoom)
		{
			EClass.screen.ModTargetZoomIndex(-wheel);
		}
	}

	public virtual void OnUpdateInput()
	{
	}

	public void DoFunc(CoreConfig.GameFuncBuild func)
	{
		switch (func)
		{
		case CoreConfig.GameFuncBuild.ExitBuildMode:
			DefaultMode.Activate();
			break;
		case CoreConfig.GameFuncBuild.ToggleFreepos:
			EClass.scene.ToggleFreePos();
			break;
		case CoreConfig.GameFuncBuild.ToggleRoof:
			EClass.scene.ToggleShowRoof();
			break;
		case CoreConfig.GameFuncBuild.ToggleSlope:
			EClass.scene.ToggleSlope();
			break;
		case CoreConfig.GameFuncBuild.Rotate:
			OnRotate();
			break;
		case CoreConfig.GameFuncBuild.ToggleWall:
			EClass.scene.ToggleShowWall();
			break;
		case CoreConfig.GameFuncBuild.TogglePicker:
			if (Picker.CanActivate)
			{
				Picker.Activate();
			}
			break;
		case CoreConfig.GameFuncBuild.SnapFreepos:
			EClass.scene.ToggleSnapFreePos();
			break;
		case CoreConfig.GameFuncBuild.ToggleBuildLight:
			EClass.scene.ToggleLight();
			break;
		}
		WidgetHotbar.RefreshHighlights();
	}

	public bool IsFuncPressed(CoreConfig.GameFunc func)
	{
		CoreConfig.InputSetting input = EClass.core.config.input;
		if (EInput.middleMouse.clicked)
		{
			return input.middleClick == func;
		}
		if (EInput.middleMouse.pressedLong)
		{
			return input.middlePressLong == func;
		}
		if (EInput.mouse3.clicked)
		{
			return input.mouse3Click == func;
		}
		if (EInput.mouse3.pressedLong)
		{
			return input.mouse3PressLong == func;
		}
		if (EInput.mouse4.clicked)
		{
			return input.mouse4Click == func;
		}
		if (EInput.mouse4.pressedLong)
		{
			return input.mouse4PressLong == func;
		}
		return false;
	}

	public void DoFunc(CoreConfig.GameFunc func)
	{
		Chara targetChara = EClass.scene.mouseTarget.TargetChara;
		switch (func)
		{
		default:
			return;
		case CoreConfig.GameFunc.OpenAllyInv:
			if (!EClass.pc.HasNoGoal || (targetChara != null && !targetChara.IsPCFaction))
			{
				return;
			}
			if (EClass.scene.mouseTarget.pos.Equals(EClass.pc.pos))
			{
				if (!EClass.ui.IsInventoryOpen)
				{
					SE.PopInventory();
				}
				EClass.ui.ToggleInventory();
			}
			else if (targetChara != null && EClass.pc.Dist(targetChara) <= 1)
			{
				LayerInventory.CreateContainer(targetChara);
			}
			break;
		case CoreConfig.GameFunc.EmptyHand:
			EInput.action = EAction.EmptyHand;
			return;
		case CoreConfig.GameFunc.Fire:
			if (EInput.keyFire.Update(forcePress: true))
			{
				EInput.action = EAction.Fire;
				return;
			}
			break;
		case CoreConfig.GameFunc.SwitchHotbar:
			WidgetCurrentTool.Instance.SwitchPage();
			break;
		case CoreConfig.GameFunc.PropertySearch:
			EClass.ui.widgets.Toggle("Search")?.SoundActivate();
			break;
		case CoreConfig.GameFunc.Talk:
			if (!EClass.pc.HasNoGoal || targetChara == null || targetChara.hostility <= Hostility.Enemy || targetChara.IsPC || EClass.pc.Dist(targetChara) > 2)
			{
				return;
			}
			ACT.Chat.Perform(EClass.pc, targetChara);
			break;
		case CoreConfig.GameFunc.AllAction:
			if (!EClass.ui.isPointerOverUI)
			{
				Adv.ShowAllAction();
			}
			return;
		case CoreConfig.GameFunc.ToggleBuild:
			if (!EClass._zone.CanEnterBuildModeAnywhere)
			{
				SE.Beep();
				Msg.Say("invalidAction");
				return;
			}
			BuildMenu.Toggle();
			break;
		case CoreConfig.GameFunc.ShowJournal:
		{
			LayerJournal layerJournal = EClass.ui.ToggleLayer<LayerJournal>();
			if ((bool)layerJournal)
			{
				layerJournal.windows[0].SetRect(EClass.core.refs.rects.center);
				layerJournal.Delay();
			}
			break;
		}
		case CoreConfig.GameFunc.ShowInv:
			EClass.ui.ToggleInventory(delay: true);
			break;
		case CoreConfig.GameFunc.ShowChara:
		{
			LayerChara layerChara = EClass.ui.ToggleLayer<LayerChara>();
			if ((bool)layerChara)
			{
				layerChara.SetChara(EClass.pc);
				layerChara.windows[0].SetRect(EClass.core.refs.rects.center);
				layerChara.Delay();
			}
			break;
		}
		case CoreConfig.GameFunc.ShowAbility:
			EClass.ui.ToggleAbility(delay: true);
			break;
		case CoreConfig.GameFunc.ToggleNoRoof:
			EClass.game.config.noRoof = !EClass.game.config.noRoof;
			break;
		case CoreConfig.GameFunc.ToggleZoom:
			Adv.ToggleZoom();
			break;
		case CoreConfig.GameFunc.EmuShift:
		case CoreConfig.GameFunc.EmuAlt:
			return;
		}
		EInput.Consume();
	}

	public virtual void OnScroll()
	{
	}

	public void TryRightClickCloseWidget()
	{
		Widget componentOf = InputModuleEX.GetComponentOf<Widget>();
		if ((bool)componentOf && componentOf.RightClickToClose)
		{
			componentOf.Close();
		}
	}

	public bool TryShowWidgetMenu()
	{
		Widget componentOf = InputModuleEX.GetComponentOf<Widget>();
		if ((bool)componentOf && componentOf.CanShowContextMenu())
		{
			if (componentOf.RightClickToClose)
			{
				componentOf.Close();
				return true;
			}
			componentOf.ShowContextMenu();
			return true;
		}
		return false;
	}

	public HitResult _HitTest(Point point, Point start)
	{
		if (!EClass.debug.ignoreBuildRule && !CanTargetOutsideBounds && (!point.IsValid || !point.IsInBoundsPlus))
		{
			return HitResult.NoTarget;
		}
		if (!point.IsValid)
		{
			return HitResult.NoTarget;
		}
		if (!CanTargetFog && !point.cell.isSeen)
		{
			return HitResult.Default;
		}
		return HitTest(point, start);
	}

	public virtual HitResult HitTest(Point point, Point start)
	{
		return HitResult.Default;
	}

	public virtual void OnSelectStart(Point point)
	{
	}

	public virtual void OnSelectEnd(bool cancel)
	{
	}

	public virtual MeshPass GetGuidePass(Point point)
	{
		if ((!point.HasNonWallBlock && !point.HasBlockRecipe && point.IsSeen) || (point.sourceBlock.tileType.Invisible && !IsBuildMode))
		{
			return EClass.screen.guide.passGuideFloor;
		}
		return EClass.screen.guide.passGuideBlock;
	}

	public virtual void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (result != HitResult.NoTarget && point.IsValid && (point.IsSeen || CanTargetFog))
		{
			MeshPass guidePass = GetGuidePass(point);
			int num = (int)result;
			if (tileSelector.start != null && result == HitResult.Default)
			{
				num = 2;
			}
			Vector3 vector = point.Position();
			if (num == 0)
			{
				num = GetDefaultTile(point);
			}
			guidePass.Add(vector.x, vector.y, vector.z - 0.01f, num, 0.3f);
			if (!IsRoofEditMode() && point.HasWallOrFence && !point.cell.hasDoor && HighlightWall(point))
			{
				EClass.screen.guide.DrawWall(point, EClass.Colors.blockColors.Passive, useMarkerPass: true);
			}
			if (IsBuildMode && EClass.screen.tileSelector.start == null && point.Installed != null && Inspect.IsActive)
			{
				point.Installed.trait.OnRenderTile(point, result, dir);
			}
		}
	}

	public void OnRenderTileFloor(Point point, HitResult result)
	{
		if (result != HitResult.NoTarget && point.IsValid && (point.IsSeen || CanTargetFog))
		{
			MeshPass guidePass = GetGuidePass(point);
			int num = (int)result;
			if (tileSelector.start != null && result == HitResult.Default)
			{
				num = 2;
			}
			Vector3 vector = point.Position(point.cell.height);
			if (num == 0)
			{
				num = GetDefaultTile(point);
			}
			guidePass.Add(vector.x, vector.y, vector.z - 0.01f, num, 0.3f);
		}
	}

	public virtual bool CanProcessTiles()
	{
		if (Multisize && Summary.countValid != hitW * hitH)
		{
			return false;
		}
		return true;
	}

	public virtual void OnBeforeProcessTiles()
	{
	}

	public virtual void OnProcessTiles(Point point, int dir)
	{
	}

	public virtual void OnAfterProcessTiles(Point start, Point end)
	{
	}

	public virtual void OnFinishProcessTiles()
	{
	}

	public virtual void OnRefreshSummary(Point point, HitResult result, HitSummary summary)
	{
		if (result == HitResult.Valid || result == HitResult.Warning)
		{
			summary.money += CostMoney;
			summary.countValid++;
		}
	}

	public void ShowLayer()
	{
		layer = OnShowLayer();
		if (layer != null)
		{
			CursorSystem.SetCursor();
		}
		else
		{
			CursorSystem.SetCursor(DefaultCursor);
		}
		if ((bool)EClass.core.screen.tileSelector)
		{
			EClass.core.screen.tileSelector.OnChangeActionMode();
		}
		EClass.ui.hud.hint.Refresh();
	}

	public virtual Layer OnShowLayer()
	{
		return null;
	}

	public void HideLayer()
	{
		EClass.ui.RemoveLayer(layer);
		CursorSystem.SetCursor(CursorSystem.Select);
		EClass.ui.hud.hint.UpdateText();
		OnHideLayer();
	}

	public virtual void OnHideLayer()
	{
	}

	public virtual string GetHintText()
	{
		string text = textHintTitle.IsEmpty(Lang.Get(id)) + "   - ";
		BaseTileSelector baseTileSelector = EClass.screen.tileSelector;
		ActionMode actionMode = EClass.scene.actionMode;
		if (!layer)
		{
			text = ((actionMode == Inspect && Inspect.target != null) ? "hintMoveTarget".lang() : ((selectType == BaseTileSelector.SelectType.Single) ? (text + "hintSelectSingle".lang()) : ((baseTileSelector.start != null) ? (text + "hintSelectEnd".lang()) : (text + "hintSelectStart".lang()))));
		}
		else
		{
			if (actionMode == Build)
			{
				text += "hintBuild".lang();
			}
			if (actionMode == CreateArea)
			{
				text += "hintCreateArea".lang();
			}
		}
		if (actionMode == Build && Build.recipe != null && baseTileSelector.start == null)
		{
			if (Build.recipe.CanRotate)
			{
				text = text + "  " + "hintRotate".lang();
			}
			if (Build.MaxAltitude > 0)
			{
				text = text + "  " + "hintAltitude2".lang();
			}
		}
		return text;
	}

	public virtual void RotateUnderMouse()
	{
		SE.Rotate();
		Point hitPoint = Scene.HitPoint;
		if (!hitPoint.IsValid)
		{
			return;
		}
		hitPoint.cell.RotateAll();
		if (EClass.debug.enable)
		{
			hitPoint.growth?.Grow(100);
			if (hitPoint.Installed?.trait is TraitSeed)
			{
				(hitPoint.Installed.trait as TraitSeed).TrySprout(force: true);
			}
		}
	}

	public virtual ref string SetMouseInfo(ref string s)
	{
		return ref s;
	}

	public void TogglePause()
	{
		if (IsAdv)
		{
			EClass.core.config.game.autopause = !EClass.core.config.game.autopause;
			if (EClass.core.config.game.autopause)
			{
				SE.Play("tick0");
			}
			else
			{
				SE.Play("tick1");
			}
		}
		else if (EClass.game.gameSpeedIndex != 0)
		{
			Pause(sound: true);
		}
		else
		{
			UnPause(sound: true);
		}
	}

	public void Pause(bool sound = false)
	{
		ChangeGameSpeed(1, sound);
	}

	public void UnPause(bool sound = false)
	{
		ChangeGameSpeed(EClass.game.lastGameSpeedIndex, sound);
	}

	public void ChangeGameSpeed(int a, bool sound = false)
	{
		if (EClass.game.gameSpeedIndex != a)
		{
			if (sound)
			{
				SE.Play("tick" + a);
			}
			EClass.game.gameSpeedIndex = a;
			if (a != 0)
			{
				EClass.game.lastGameSpeedIndex = a;
			}
			WidgetHotbar.RefreshHighlights();
		}
	}
}
