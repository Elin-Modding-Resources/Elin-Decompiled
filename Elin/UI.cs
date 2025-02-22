using System;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class UI : ELayer
{
	private static float blurSize;

	public Canvas canvas;

	public HUD hud;

	public ExtraHint extraHint;

	public InputModuleEX inputModule;

	public UIContextMenuManager contextMenu;

	public WidgetManager widgets;

	public UIMouseInfo mouseInfo;

	public RectTransform rectDynamic;

	public RectTransform rectDynamicEssential;

	public RectTransform _rectLayers;

	public ReflexConsole console;

	public GameObject blur;

	public Material matBlur;

	public LayoutGroup layoutLang;

	public CanvasScaler canvasScaler;

	public Image light;

	public CanvasGroup cg;

	public int minWidth;

	public Layer layerFloat;

	public UIAutoTurn autoTurn;

	public PopManager popGame;

	public PopManager popSystem;

	public float blurSpeed;

	public Texture2D texFreeze;

	public RawImage imageFreeze;

	public DragItem currentDrag;

	public DragItem nextDrag;

	private bool hidingCover;

	[NonSerialized]
	public float lightContrast;

	[NonSerialized]
	public float dragDuration;

	private Tween tweenCover;

	[NonSerialized]
	public bool wasActive;

	[NonSerialized]
	public bool isPointerOverUI;

	private float durationHideMouseHint;

	public SkinManager skins => ELayer.core.skins;

	public bool IsActive
	{
		get
		{
			if (layers.Count <= 0)
			{
				return contextMenu.isActive;
			}
			return true;
		}
	}

	public bool AllowInventoryInteractions
	{
		get
		{
			if (!contextMenu.isActive)
			{
				if (layers.Count != 0)
				{
					return base.TopLayer.option.allowInventoryInteraction;
				}
				return true;
			}
			return false;
		}
	}

	public bool BlockActions
	{
		get
		{
			if (!wasActive || ((bool)base.TopLayer && base.TopLayer.option.passive))
			{
				return EInput.isInputFieldActive;
			}
			return true;
		}
	}

	public bool BlockMouseOverUpdate => wasActive;

	public bool BlockInput
	{
		get
		{
			if (!IsDragging && !IsBlockWidgetClick() && !(LayerAbility.hotElement != null) && !EInput.isInputFieldActive)
			{
				return contextMenu.isActive;
			}
			return true;
		}
	}

	public bool IsPauseGame
	{
		get
		{
			if (layers.Count > 0)
			{
				return base.TopLayer.option.pauseGame;
			}
			return false;
		}
	}

	public bool IsInventoryOpen => layerFloat.GetLayer<LayerInventory>();

	public bool IsAbilityOpen => layerFloat.GetLayer<LayerAbility>();

	public override bool blockWidgetClick => false;

	public override RectTransform rectLayers => _rectLayers;

	public bool IsDragging => currentDrag != null;

	protected override void Awake()
	{
		base.Awake();
		InvokeRepeating("CheckWindowOrder", 1f, 0.5f);
	}

	public void OnCoreStart()
	{
		mouseInfo.SetActive(enable: false);
	}

	public void OnActivateZone()
	{
		widgets.OnActivateZone();
		ELayer.player.queues.SetOwner(ELayer.pc);
		if ((bool)hud.hangCorner && hud.hangCorner.isActiveAndEnabled)
		{
			hud.hangCorner.Refresh();
		}
	}

	public void ShowFloats()
	{
		layerFloat.SetActive(enable: true);
	}

	public void HideFloats()
	{
		layerFloat.SetActive(enable: false);
	}

	public void OnKillGame()
	{
		widgets.OnKillGame();
		ShowBalloon(enable: true);
		layerFloat.RemoveLayers(removeImportant: true);
		currentDrag = null;
	}

	public void OnClickAction(string _mode)
	{
		RemoveLayers();
		((ActionMode)typeof(ActionMode).GetField(_mode, BindingFlags.Static | BindingFlags.Public).GetValue(null)).Activate();
	}

	public void RefreshActiveState()
	{
		isPointerOverUI = false;
		foreach (GameObject item in InputModuleEX.GetPointerEventData().hovered)
		{
			if ((bool)item && item.layer == 5)
			{
				isPointerOverUI = true;
				break;
			}
		}
		wasActive = IsActive;
		if (wasActive)
		{
			CursorSystem.SetCursor();
		}
	}

	public override void OnChangeLayer()
	{
		RefreshActiveState();
		CursorSystem.SetCursor();
		hud.HideMouseInfo();
		if (ELayer.core.IsGameStarted)
		{
			ELayer.scene.actionMode.OnUpdateCursor();
		}
		CursorSystem.Instance.Draw();
		Layer topLayer = base.TopLayer;
		if ((object)topLayer != null && topLayer.option.dontShowHint)
		{
			return;
		}
		if (layers.Count == 0)
		{
			hud.hint.Refresh();
			return;
		}
		if (base.TopLayer.option.hideFloatUI)
		{
			HideFloats();
		}
		if (base.TopLayer.option.hideWidgets)
		{
			widgets.Hide();
		}
		(base.TopLayer as ELayer)?.TryShowHint();
	}

	public void FlashCover(float durationOut = 1f, float duration = 1f, float durationIn = 1f, Action onFadeOut = null, Action onComplete = null, Color color = default(Color))
	{
		ShowCover(durationOut, 1f, null, color);
		TweenUtil.Tween(durationOut + duration, null, delegate
		{
			if (onFadeOut != null)
			{
				onFadeOut();
			}
			HideCover(durationIn, onComplete);
		});
	}

	public void ShowCover(float duration = 0f, float dest = 1f, Action onComplete = null, Color color = default(Color))
	{
		TweenUtil.KillTween(ref tweenCover);
		float a = hud.imageCover.color.a;
		hud.imageCover.color = ((color == default(Color)) ? Color.black : color).SetAlpha(a);
		hud.imageCover.SetActive(enable: true);
		hidingCover = false;
		tweenCover = hud.imageCover.DOFade(dest, duration).OnComplete(delegate
		{
			onComplete?.Invoke();
		});
	}

	public void HideCover(float duration = 0f, Action onComplete = null)
	{
		if (!hidingCover)
		{
			hidingCover = true;
			TweenUtil.KillTween(ref tweenCover);
			tweenCover = hud.imageCover.DOFade(0f, duration).OnComplete(delegate
			{
				hud.imageCover.SetActive(enable: false);
				onComplete?.Invoke();
				hidingCover = false;
			});
		}
	}

	public bool IsCovered()
	{
		return !hidingCover;
	}

	public void ShowBalloon(bool enable)
	{
		ELayer.ui.rectDynamic.SetActive(enable);
		WidgetSystemIndicator.Refresh();
	}

	public void Show(float duration = 1f)
	{
		ELayer.scene.elomapActor.selector.srHighlight.SetActive(enable: true);
		cg.DOKill();
		if (duration == 0f)
		{
			cg.alpha = 1f;
		}
		else
		{
			cg.DOFade(1f, duration);
		}
	}

	public void Hide(float duration = 1f)
	{
		ELayer.scene.elomapActor.selector.srHighlight.SetActive(enable: false);
		if (duration == 0f)
		{
			cg.alpha = 0f;
		}
		else
		{
			cg.DOFade(0f, duration);
		}
	}

	public void ToggleCanvas()
	{
		canvas.enabled = !canvas.enabled;
		if ((bool)WidgetSideScreen.Instance)
		{
			WidgetSideScreen.Instance.SetActive(canvas.enabled);
		}
	}

	public void OnUpdate()
	{
		if (hud.imageDrag.gameObject.activeSelf)
		{
			hud.imageDrag.transform.position = EInput.mpos + (ELayer.game.UseGrid ? hud.imageDragFix2 : hud.imageDragFix2);
			Util.ClampToScreen(hud.imageDrag.Rect(), hud.marginImageDrag);
		}
		if (ELayer.config.ui.blur && layers.Count > 0 && IsUseBlur())
		{
			blurSize += Time.unscaledDeltaTime * blurSpeed;
			if (blurSize > ELayer.config.ui.blurSize)
			{
				blurSize = ELayer.config.ui.blurSize;
			}
			if (!blur.activeSelf)
			{
				blur.SetActive(value: true);
			}
			int siblingIndex = blur.transform.GetSiblingIndex();
			int num = 0;
			for (int num2 = layers.Count - 1; num2 >= 0; num2--)
			{
				if (layers[num2].IsUseBlur())
				{
					num = layers[num2].transform.GetSiblingIndex() - 1;
					break;
				}
			}
			if (siblingIndex != num)
			{
				blur.transform.SetSiblingIndex(num);
			}
		}
		else
		{
			blurSize -= Time.unscaledDeltaTime * blurSpeed;
			if (blurSize < 0f)
			{
				blurSize = 0f;
			}
			if (blur.activeSelf && blurSize == 0f)
			{
				blur.SetActive(value: false);
			}
		}
		matBlur.SetFloat("_Size", blurSize);
		if (!EInput.isShiftDown)
		{
			LayerInventory.highlightInv = null;
		}
		ShowMouseHint();
	}

	public void ShowMouseHint()
	{
		if (durationHideMouseHint > 0f)
		{
			durationHideMouseHint -= Core.delta;
			hud.textMouseHintLeft.SetActive(enable: false);
			hud.textMouseHintRight.SetActive(enable: false);
			return;
		}
		IMouseHint mouseHint = UIButton.currentHighlight as IMouseHint;
		bool flag = mouseHint != null && !IsDragging && isPointerOverUI && (bool)UIButton.currentHighlight && InputModuleEX.IsPointerOver(UIButton.currentHighlight) && LayerAbility.hotElement == null;
		bool flag2 = flag;
		if (flag2)
		{
			if (mouseHint.ShowMouseHintLeft())
			{
				hud.textMouseHintLeft.text = mouseHint.GetTextMouseHintLeft();
				if (hud.textMouseHintLeft.text == "")
				{
					flag2 = false;
				}
			}
			else
			{
				flag2 = false;
			}
		}
		if (flag)
		{
			if (mouseHint.ShowMouseHintRight())
			{
				hud.textMouseHintRight.text = mouseHint.GetTextMouseHintRight();
				if (hud.textMouseHintRight.text == "")
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
		}
		hud.textMouseHintLeft.SetActive(flag2);
		hud.textMouseHintRight.SetActive(flag);
		if (flag || flag2)
		{
			hud.textMouseHintLeft.transform.position = UIButton.currentHighlight.transform.position + hud.textMouseHintFixLeft;
			hud.textMouseHintRight.transform.position = UIButton.currentHighlight.transform.position + hud.textMouseHintFix;
			Util.ClampToScreen(hud.textMouseHintRight.Rect(), 10);
		}
	}

	public void HideMouseHint(float duration = 0.2f)
	{
		durationHideMouseHint = duration;
		hud.textMouseHintLeft.SetActive(enable: false);
		hud.textMouseHintRight.SetActive(enable: false);
	}

	public void CheckWindowOrder()
	{
		if (layers.Count > 0)
		{
			return;
		}
		foreach (GameObject item in InputModuleEX.GetPointerEventData().hovered)
		{
			if ((bool)item)
			{
				Widget component = item.GetComponent<Widget>();
				if ((bool)component && !component.AlwaysBottom && widgets.transform.GetChild(widgets.transform.childCount - 1) != item.transform)
				{
					item.transform.SetAsLastSibling();
					break;
				}
			}
		}
		if ((bool)WidgetFeed.Instance)
		{
			WidgetFeed.Instance.transform.SetAsLastSibling();
		}
	}

	public void ShowSceneSelector()
	{
		canvasScaler.scaleFactor = 1f;
		Shader.SetGlobalFloat("_UIBrightness", 1f);
		Shader.SetGlobalFloat("_UIContrast", 1f);
		layoutLang.SetActive(enable: true);
		List<CoreDebug.StartScene> list = Util.EnumToList<CoreDebug.StartScene>();
		Button t = layoutLang.CreateMold<Button>();
		foreach (CoreDebug.StartScene item in list)
		{
			Button button = Util.Instantiate(t, layoutLang);
			CoreDebug.StartScene _l = item;
			button.GetComponentInChildren<Text>().text = item.ToString();
			button.onClick.AddListener(delegate
			{
				layoutLang.SetActive(enable: false);
				ELayer.debug.startScene = _l;
				ELayer.core.Init();
			});
		}
		layoutLang.RebuildLayout();
	}

	public void ShowLang()
	{
		canvasScaler.scaleFactor = 1f;
		Shader.SetGlobalFloat("_UIBrightness", 1f);
		Shader.SetGlobalFloat("_UIContrast", 1f);
		layoutLang.SetActive(enable: true);
		Button t = layoutLang.CreateMold<Button>();
		foreach (LangSetting value in MOD.langs.Values)
		{
			Button button = Util.Instantiate(t, layoutLang);
			LangSetting _l = value;
			button.GetComponentInChildren<Text>().text = value.name + " (" + value.name_en + ")";
			button.onClick.AddListener(delegate
			{
				layoutLang.SetActive(enable: false);
				ELayer.core.langCode = _l.id;
				if (ELayer.debug.showSceneSelector || (Input.GetKey(KeyCode.LeftShift) && ELayer.debug.enable))
				{
					ShowSceneSelector();
				}
				else
				{
					ELayer.core.Init();
				}
			});
		}
		layoutLang.RebuildLayout();
	}

	public void SetLight(bool enable)
	{
	}

	public UIContextMenu CreateContextMenu(string cid = "ContextMenu")
	{
		return contextMenu.Create(cid);
	}

	public UIContextMenu CreateContextMenuInteraction()
	{
		return contextMenu.Create("ContextInteraction");
	}

	public void Say(string text, Sprite sprite = null)
	{
		popSystem.PopText(text.lang(), sprite, "PopAchievement");
		Debug.Log(text);
	}

	public void FreezeScreen(float duration, bool highlight = true)
	{
		if ((bool)texFreeze)
		{
			UnityEngine.Object.Destroy(texFreeze);
		}
		texFreeze = ScreenCapture.CaptureScreenshotAsTexture();
		imageFreeze.SetActive(enable: true);
		imageFreeze.texture = texFreeze;
		if (duration == 0f)
		{
			return;
		}
		TweenUtil.Tween(duration, null, delegate
		{
			UnfreezeScreen();
			if (highlight)
			{
				UIButton.TryHihlight();
			}
		});
	}

	public void UnfreezeScreen()
	{
		if ((bool)texFreeze)
		{
			UnityEngine.Object.DestroyImmediate(texFreeze);
		}
		imageFreeze.SetActive(enable: false);
	}

	public void ToggleAbility(bool delay = false)
	{
		if (ELayer.game.altAbility)
		{
			if ((bool)ELayer.ui.layerFloat.GetLayer<LayerAbility>())
			{
				ELayer.ui.layerFloat.RemoveLayer<LayerAbility>();
				ELayer.player.pref.layerAbility = false;
				SE.Play("pop_ability_deactivate");
			}
			else
			{
				ELayer.ui.layerFloat.AddLayer<LayerAbility>("LayerAbility/LayerAbilityFloat");
				ELayer.player.pref.layerAbility = true;
			}
		}
		else if (!ELayer.ui.RemoveLayer<LayerAbility>())
		{
			LayerAbility layerAbility = ELayer.ui.AddLayer<LayerAbility>();
			if (delay)
			{
				layerAbility.windows[0].SetRect(ELayer.core.refs.rects.center);
				layerAbility.Delay();
			}
		}
	}

	public void ToggleInventory(bool delay = false)
	{
		if (IsInventoryOpen)
		{
			if (InvOwner.HasTrader)
			{
				SE.Beep();
				return;
			}
			List<Card> list = new List<Card>();
			foreach (LayerInventory item in LayerInventory.listInv)
			{
				if (item.IsPlayerContainer())
				{
					list.Add(item.invs[0].owner.Container);
				}
			}
			LayerInventory.listInv.ForeachReverse(delegate(LayerInventory l)
			{
				if (l.IsPlayerContainer(includePlayer: true))
				{
					ELayer.ui.layerFloat.RemoveLayer(l);
				}
			});
			ELayer.ui.widgets.DeactivateWidget("Equip");
			foreach (Card item2 in list)
			{
				item2.c_windowSaveData.open = true;
			}
			SE.Play("pop_inventory_deactivate");
		}
		else
		{
			OpenFloatInv();
		}
	}

	public void OpenFloatInv(bool ignoreSound = false)
	{
		TooltipManager.Instance.disableTimer = 0.1f;
		if (ignoreSound)
		{
			SoundManager.ignoreSounds = true;
		}
		ELayer.ui.layerFloat.AddLayer(LayerInventory.CreatePCBackpack());
		SoundManager.ignoreSounds = true;
		ELayer.ui.widgets.Activate("Equip");
		foreach (Thing item in ELayer.pc.things.List((Thing a) => a.trait.IsContainer))
		{
			Window.SaveData c_windowSaveData = item.c_windowSaveData;
			if (!(item.trait is TraitToolBelt) && c_windowSaveData != null && c_windowSaveData.open)
			{
				LayerInventory.CreateContainer(item);
			}
		}
		SoundManager.ignoreSounds = false;
		TooltipManager.Instance.disableTimer = 0f;
	}

	public void ToggleFeedback()
	{
		string text = "";
		if (Application.isEditor)
		{
			text = "Debug";
		}
		else
		{
			try
			{
				if (SteamAPI.IsSteamRunning())
				{
					text = SteamFriends.GetPersonaName();
					if (text.IsEmpty())
					{
						text = "NULL";
					}
				}
			}
			catch
			{
			}
		}
		if (text.IsEmpty())
		{
			Dialog.Ok("dialog_needToLogOn");
			return;
		}
		string backerId = "";
		try
		{
			if (!ELayer.config.rewardCode.IsEmpty())
			{
				backerId = "backer";
				backerId = ElinEncoder.GetID(ELayer.config.rewardCode);
			}
		}
		catch (Exception message)
		{
			backerId = "";
			Debug.Log(message);
		}
		bool flag = false;
		if (!Application.isEditor)
		{
			string pchName = "public";
			try
			{
				if (!SteamApps.GetCurrentBetaName(out pchName, 128) || !(pchName == "nightly"))
				{
					Dialog.Ok("dialog_feedbackTooMany");
					return;
				}
				flag = true;
				Debug.Log(pchName);
			}
			catch
			{
				Dialog.Ok("dialog_feedbackTooMany");
				return;
			}
		}
		if (ELayer.core.IsGameStarted && !ELayer.ui.GetLayer<LayerFeedback>() && ELayer.pc.HasCondition<ConHallucination>())
		{
			SE.Play("wow");
			Msg.Say("bug_hal");
			return;
		}
		if (!Application.isEditor && (ELayer.debug.enable || ELayer.core.version.demo || (ELayer.core.IsGameStarted && ELayer.player.flags.debugEnabled)))
		{
			Dialog.Ok("dialog_debugFeedback");
			return;
		}
		string userName = "Unknown";
		string[] array = Application.persistentDataPath.Split('/');
		if (array.Length > 2)
		{
			userName = array[2];
		}
		LayerFeedback.header = (flag ? "" : "(!?)");
		LayerFeedback.userName = userName;
		LayerFeedback.playedHours = ELayer.config.maxPlayedHours;
		LayerFeedback.backerId = backerId;
		LayerFeedback.steamName = text;
		ELayer.ui.ToggleLayer<LayerFeedback>();
		SE.Tab();
	}

	public void StartDrag(DragItem item)
	{
		dragDuration = 0f;
		if (currentDrag != null)
		{
			EndDrag(canceled: true);
			if (currentDrag != null)
			{
				return;
			}
		}
		currentDrag = item;
		item.OnStartDrag();
		OnDrag();
		ELayer.core.actionsNextFrame.Add(delegate
		{
			TooltipManager.Instance.HideTooltips(immediate: true);
		});
	}

	public void OnDrag()
	{
		dragDuration += Core.delta;
		currentDrag.OnDrag(execute: false);
	}

	public void OnDragSpecial()
	{
		if (currentDrag != null && !currentDrag.OnDragSpecial())
		{
			EndDrag(canceled: true);
		}
	}

	public void EndDrag(bool canceled = false)
	{
		if (currentDrag == null)
		{
			return;
		}
		bool num = currentDrag.OnDrag(execute: true, canceled);
		EInput.Consume();
		EInput.dragHack = 0f;
		if (num)
		{
			ELayer.ui.RemoveLayer<LayerRegisterHotbar>();
			currentDrag.OnEndDrag();
			currentDrag = null;
			if (nextDrag != null)
			{
				StartDrag(nextDrag);
				nextDrag = null;
			}
			else
			{
				hud.SetDragImage(null);
			}
			UIButton.TryShowTip();
		}
		else
		{
			SE.BeepSmall();
		}
	}
}
