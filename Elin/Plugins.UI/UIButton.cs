using System;
using System.Collections.Generic;
using DG.Tweening;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : Button, IUISkin, IPointerDownHandler, IEventSystemHandler
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Item
	{
		[JsonProperty]
		public bool always;

		public virtual string TextTip => Name;

		public virtual string Name => null;

		public virtual Sprite SpriteHighlight => null;

		public virtual string pathSprite => "";

		public virtual bool IsSelectable => false;

		public virtual bool AdjustImageSize => true;

		public virtual bool UseIconForHighlight => false;

		public virtual bool KeepVisibleWhenHighlighted => false;

		public virtual Color SpriteColor => Color.white;

		public virtual Vector3 SpriteScale => Vector3.one;

		public virtual bool Hidden => false;

		public virtual Transition Transition => Transition.SpriteSwap;

		public virtual Sprite GetSprite()
		{
			return SpriteSheet.Get(pathSprite);
		}

		public virtual Sprite GetSprite(bool highlight)
		{
			return null;
		}

		public virtual bool ShouldHighlight()
		{
			return false;
		}

		public virtual void OnAddedToBar()
		{
		}

		public virtual void OnShowContextMenu(UIContextMenu m)
		{
		}

		public virtual void OnHover(UIButton b)
		{
		}

		public virtual void OnClick(UIButton b)
		{
		}

		public virtual void OnSetItem(UIButton b)
		{
		}

		public virtual void SetSubText(UIText t)
		{
			t.SetActive(enable: false);
		}
	}

	public static bool DoSlide;

	public static bool locked;

	public static UIButton lastButton;

	public static UIButton lastHovered;

	public static UIButton currentHighlight;

	public static UIButton currentPressedButton;

	public static float lastClicked;

	public static float doubleClickTime = 0.4f;

	public static Vector2 buttonPos;

	public static Action onPressed;

	public UIText mainText;

	public UIText subText;

	public UIText subText2;

	public UIText keyText;

	public Image icon;

	public Image imageCheck;

	public Graphic[] targetGraphics;

	public float scaleIcon = 1f;

	public float slideX;

	public float navigationFix;

	public bool isChecked;

	public bool instantClick = true;

	public UISelectableGroup group;

	public TooltipData tooltip;

	public SoundData soundClick;

	public SoundData soundHighlight;

	public Transform animeTarget;

	public Anime animeClick;

	public Anime animeHold;

	public ButtonType buttonType;

	public BaseSkinRoot skinRoot;

	public Action onDoubleClick;

	public Action onRightClick;

	public Action<int> onInputWheel;

	[NonSerialized]
	public object refObj;

	[NonSerialized]
	public int refInt;

	[NonSerialized]
	public string refStr;

	[NonSerialized]
	public bool animating;

	[NonSerialized]
	public bool selected;

	[NonSerialized]
	public Vector3 originalIconScale = Vector3.one;

	[NonSerialized]
	public UIButton highlightTarget;

	[NonSerialized]
	public Item item;

	private List<Graphic> m_graphics;

	private Tween tween;

	private SelectionState lastState;

	private static bool waiting;

	public static List<UIButton> buttons = new List<UIButton>();

	public static Action actionTooltip;

	protected List<Graphic> Graphics
	{
		get
		{
			if (m_graphics == null)
			{
				m_graphics = new List<Graphic>();
				if (base.targetGraphic != null)
				{
					m_graphics.Add(base.targetGraphic);
				}
				if (targetGraphics != null)
				{
					Graphic[] array = targetGraphics;
					foreach (Graphic graphic in array)
					{
						m_graphics.Add(graphic);
					}
				}
			}
			return m_graphics;
		}
	}

	public virtual bool CanDragLeftButton => true;

	public virtual bool CanMiddleClick()
	{
		return false;
	}

	public virtual void OnMiddleClick(bool forceClick)
	{
	}

	protected override void Awake()
	{
		if ((bool)icon)
		{
			originalIconScale = icon.transform.localScale;
		}
		if (Application.isPlaying)
		{
			ApplySkin();
		}
		base.Awake();
	}

	protected override void OnEnable()
	{
		selected = false;
		base.OnEnable();
		if (!EventSystem.current)
		{
			return;
		}
		if ((bool)imageCheck)
		{
			imageCheck.gameObject.SetActive(isChecked);
		}
		if (EventSystem.current.currentSelectedGameObject == base.gameObject)
		{
			if (base.transition == Transition.SpriteSwap && base.spriteState.highlightedSprite != base.image.sprite)
			{
				DoHighlightTransition();
			}
			if ((bool)icon)
			{
				icon.transform.localScale = new Vector3(scaleIcon, scaleIcon, 1f);
			}
		}
	}

	public void SetItem(Item i)
	{
		item = i;
		RefreshItem();
		if (item != null)
		{
			item.OnSetItem(this);
		}
	}

	public virtual void RefreshItem()
	{
		bool flag = item != null;
		base.onClick.RemoveAllListeners();
		icon.enabled = flag;
		base.interactable = flag;
		if (flag && item.Hidden)
		{
			icon.enabled = false;
			base.interactable = false;
		}
		base.transition = item?.Transition ?? Transition.SpriteSwap;
		if (flag)
		{
			icon.sprite = item.GetSprite();
			base.onClick.AddListener(delegate
			{
				buttonPos = base.transform.position;
				item.OnClick(this);
				EInput.Consume(0);
			});
			icon.transform.localScale = item.SpriteScale;
			if (item.AdjustImageSize)
			{
				icon.SetNativeSize();
			}
			else
			{
				icon.Rect().sizeDelta = new Vector2(48f, 48f);
			}
			tooltip.enable = !item.TextTip.IsEmpty();
			tooltip.onShowTooltip = delegate(UITooltip t)
			{
				string textTip = item.TextTip;
				t.textMain.text = textTip;
			};
			if ((bool)subText)
			{
				item.SetSubText(subText);
			}
		}
	}

	public virtual void OnHover()
	{
		if ((!(UIContextMenu.timeSinceClosed < 0.1f) || !(lastHovered == this)) && item != null && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
		{
			buttonPos = base.transform.position;
			item.OnHover(this);
			lastHovered = this;
		}
	}

	public void Toggle()
	{
		group.Select(this, !isChecked);
	}

	public void ToggleCheck()
	{
		SetCheck(!isChecked);
	}

	public void SetCheck(bool check)
	{
		isChecked = check;
		if ((bool)imageCheck)
		{
			imageCheck.gameObject.SetActive(isChecked);
		}
	}

	public void SetToggle(bool isOn, Action<bool> onToggle = null)
	{
		SetCheck(isOn);
		base.onClick.RemoveAllListeners();
		base.onClick.AddListener(delegate
		{
			SetCheck(!isChecked);
			if (onToggle != null)
			{
				onToggle(isChecked);
			}
		});
	}

	public void SetToggleWithScore(bool isOn, Action<bool> onToggle, int? score)
	{
		SetToggle(isOn, onToggle);
		subText.SetActive(score != 0);
		UIText uIText = subText;
		int? num = score;
		uIText.SetText(num.ToString() ?? "", (score >= 0) ? FontColor.Good : FontColor.Bad);
	}

	public void SetTooltip(Action<UITooltip> onShowTooltip = null, bool enable = true)
	{
		SetTooltip("note", onShowTooltip, enable);
	}

	public void SetTooltip(string id, Action<UITooltip> onShowTooltip = null, bool enable = true)
	{
		tooltip.id = id;
		tooltip.enable = enable;
		tooltip.onShowTooltip = onShowTooltip;
	}

	public void SetTooltipLang(string lang = null)
	{
		tooltip.enable = true;
		tooltip.lang = lang;
	}

	public virtual void ShowTooltip()
	{
		if (tooltip.enable && !Input.GetMouseButton(0) && !waiting && !Input.GetMouseButton(1) && (!EInput.isShiftDown || !Input.GetMouseButton(0)))
		{
			TooltipManager.Instance.ShowTooltip(tooltip, base.transform);
		}
	}

	public void ShowTooltipForced(bool ignoreWhenRightClick = true)
	{
		if (tooltip.enable && (!ignoreWhenRightClick || !Input.GetMouseButton(1)) && (!EInput.isShiftDown || !Input.GetMouseButton(0)))
		{
			TooltipManager.Instance.ShowTooltip(tooltip, base.transform);
		}
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		currentPressedButton = this;
		if (!instantClick)
		{
			base.OnPointerDown(eventData);
		}
		else
		{
			if (EInput.IsConsumed)
			{
				return;
			}
			if (eventData.button != 0)
			{
				if (eventData.button == PointerEventData.InputButton.Right && onRightClick != null && !EInput.rightMouse.wasConsumed)
				{
					BaseCore.Instance.WaitForEndOfFrame(delegate
					{
						onRightClick();
					});
				}
				return;
			}
			waiting = true;
			BaseCore.Instance.WaitForEndOfFrame(delegate
			{
				BaseCore.Instance.WaitForEndOfFrame(delegate
				{
					waiting = false;
				});
				if (EInput.skipFrame <= 0)
				{
					if ((bool)animeClick)
					{
						EventSystem.current.enabled = false;
						animeClick.Play(animeTarget ? animeTarget : base.transform).OnComplete(delegate
						{
							OnPress();
							EventSystem.current.enabled = true;
						});
					}
					else
					{
						_OnPress();
					}
				}
			});
		}
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		currentPressedButton = null;
		onPressed = null;
		if (eventData.dragging || instantClick)
		{
			return;
		}
		waiting = true;
		BaseCore.Instance.WaitForEndOfFrame(delegate
		{
			BaseCore.Instance.WaitForEndOfFrame(delegate
			{
				waiting = false;
			});
			if (EInput.skipFrame <= 0)
			{
				if (eventData.button != 0)
				{
					if (eventData.button == PointerEventData.InputButton.Right && onRightClick != null && !EInput.rightMouse.wasConsumed)
					{
						onRightClick();
					}
				}
				else
				{
					_OnPress();
				}
			}
		});
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		currentPressedButton = null;
		onPressed = null;
		if (!instantClick)
		{
			base.OnPointerUp(eventData);
		}
	}

	public void OnPointerUpOnDrag(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
	}

	public void _OnPress()
	{
		if (!locked && IsActive() && IsInteractable())
		{
			if ((bool)soundClick)
			{
				SoundManager.current.Play(soundClick);
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (lastButton == this && onDoubleClick != null && realtimeSinceStartup - lastClicked < doubleClickTime)
			{
				onDoubleClick();
				lastButton = null;
			}
			else
			{
				lastClicked = realtimeSinceStartup;
				lastButton = this;
				base.onClick.Invoke();
			}
		}
	}

	public void OnPress()
	{
		Invoke("_OnPress", 0.01f);
	}

	public void AddHighlight(Func<bool> killCondition)
	{
		base.gameObject.AddComponent<UIButtonHighlighter>().Set(killCondition);
	}

	public void DoHighlightTransition(bool instant = false)
	{
		DoStateTransition(SelectionState.Highlighted, instant);
	}

	public void DoNormalTransition(bool instant = true)
	{
		DoStateTransition(SelectionState.Normal, instant);
	}

	protected override void DoStateTransition(SelectionState state, bool instant)
	{
		if ((waiting && state == SelectionState.Highlighted) || !Application.isPlaying)
		{
			return;
		}
		if (!base.interactable)
		{
			state = SelectionState.Disabled;
		}
		if ((bool)highlightTarget)
		{
			highlightTarget.DoStateTransition(state, instant: false);
		}
		if (selected && !imageCheck)
		{
			if (state == SelectionState.Highlighted)
			{
				ShowTooltip();
			}
			state = SelectionState.Selected;
		}
		switch (state)
		{
		case SelectionState.Normal:
			if (scaleIcon != 1f)
			{
				icon.transform.localScale = originalIconScale;
			}
			break;
		case SelectionState.Highlighted:
			currentHighlight = this;
			if (lastState != SelectionState.Pressed)
			{
				ShowTooltip();
			}
			if (scaleIcon != 1f && base.interactable)
			{
				icon.transform.localScale = new Vector3(scaleIcon, scaleIcon, 1f);
			}
			if (lastState != SelectionState.Pressed && (bool)soundHighlight)
			{
				SoundManager.current.Play(soundHighlight);
			}
			break;
		case SelectionState.Pressed:
			if (base.transition == Transition.SpriteSwap && base.spriteState.pressedSprite == null)
			{
				SpriteState spriteState = base.spriteState;
				spriteState.pressedSprite = spriteState.highlightedSprite;
				base.spriteState = spriteState;
			}
			if (scaleIcon != 1f)
			{
				icon.transform.localScale = new Vector3(scaleIcon, scaleIcon, 1f) * 0.9f;
				buttons.Add(this);
			}
			if ((bool)animeHold)
			{
				if (tween == null)
				{
					tween = animeHold.Play(animeTarget ? animeTarget : base.transform).SetAutoKill(autoKillOnCompletion: false);
				}
				else
				{
					tween.Restart();
				}
				animating = true;
				buttons.Add(this);
			}
			break;
		default:
			if (scaleIcon != 1f)
			{
				icon.transform.localScale = originalIconScale;
			}
			if ((bool)animeHold && tween != null && !Input.GetMouseButton(0))
			{
				tween.PlayBackwards();
				animating = false;
			}
			break;
		case SelectionState.Disabled:
			break;
		}
		lastState = state;
		if (base.transition != Transition.ColorTint)
		{
			base.DoStateTransition(state, instant);
			return;
		}
		ColorTween(state switch
		{
			SelectionState.Normal => base.colors.normalColor, 
			SelectionState.Highlighted => base.colors.highlightedColor, 
			SelectionState.Pressed => base.colors.pressedColor, 
			SelectionState.Disabled => base.colors.disabledColor, 
			_ => Color.white, 
		} * base.colors.colorMultiplier, instant);
	}

	private void ColorTween(Color targetColor, bool instant)
	{
		if (!(base.targetGraphic == null))
		{
			for (int i = 0; i < Graphics.Count; i++)
			{
				Graphics[i].CrossFadeColor(targetColor, (!instant) ? base.colors.fadeDuration : 0f, ignoreTimeScale: true, useAlpha: true);
			}
		}
	}

	public override Selectable FindSelectableOnDown()
	{
		if (base.navigation.selectOnDown != null)
		{
			return base.FindSelectableOnDown();
		}
		return FindSelectable(Quaternion.Euler(0f, 0f, navigationFix) * Vector3.down);
	}

	public override Selectable FindSelectableOnUp()
	{
		if (base.navigation.selectOnUp != null)
		{
			return base.FindSelectableOnUp();
		}
		return FindSelectable(Quaternion.Euler(0f, 0f, navigationFix) * Vector3.up);
	}

	public override Selectable FindSelectableOnLeft()
	{
		if (base.navigation.selectOnLeft != null)
		{
			return base.FindSelectableOnLeft();
		}
		return FindSelectable(Quaternion.Euler(0f, 0f, navigationFix) * Vector3.left);
	}

	public override Selectable FindSelectableOnRight()
	{
		if (base.navigation.selectOnRight != null)
		{
			return base.FindSelectableOnRight();
		}
		return FindSelectable(Quaternion.Euler(0f, 0f, navigationFix) * Vector3.right);
	}

	public void SetNavigation(UIButton up, UIButton down = null, UIButton left = null, UIButton right = null)
	{
		Navigation navigation = base.navigation;
		navigation.mode = Navigation.Mode.Explicit;
		navigation.selectOnUp = up;
		navigation.selectOnDown = down;
		navigation.selectOnLeft = left;
		navigation.selectOnRight = right;
		base.navigation = navigation;
	}

	public override bool IsActive()
	{
		if (IsDestroyed())
		{
			return false;
		}
		return base.IsActive();
	}

	public static void UpdateButtons()
	{
		if ((bool)currentPressedButton && onPressed != null)
		{
			onPressed();
		}
		if (Input.GetMouseButton(0) || buttons.Count == 0)
		{
			return;
		}
		foreach (UIButton button in buttons)
		{
			if ((bool)button && (bool)button.gameObject)
			{
				if (button.animating)
				{
					button.tween.PlayBackwards();
					button.animating = false;
				}
				if ((bool)button.icon && button.scaleIcon != 1f)
				{
					button.icon.transform.localScale = button.originalIconScale;
				}
			}
		}
		buttons.Clear();
	}

	public void SetInteractableWithAlpha(bool enable)
	{
		CanvasGroup canvasGroup = base.gameObject.GetComponent<CanvasGroup>();
		if (!canvasGroup)
		{
			canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
		}
		canvasGroup.alpha = (enable ? 1f : 0.5f);
		base.interactable = enable;
	}

	public virtual void ApplySkin()
	{
		if ((bool)skinRoot)
		{
			SkinAsset_Button button = skinRoot.GetButton();
			base.transition = button.transition;
			SpriteState spriteState = base.spriteState;
			spriteState.highlightedSprite = button.spriteHighlight;
			if (base.transition == Transition.ColorTint)
			{
				base.image.color = Color.white;
				ColorBlock colorBlock = base.colors;
				colorBlock.normalColor = button.color;
				colorBlock.highlightedColor = button.colorHighlight;
				base.colors = colorBlock;
			}
			else
			{
				base.image.color = button.color;
			}
			base.spriteState = spriteState;
			base.image.sprite = button.spriteNormal;
		}
	}

	public static void TryHihlight()
	{
		InputModuleEX.UpdateEventData();
		UIButton componentOf = InputModuleEX.GetComponentOf<UIButton>();
		if ((bool)componentOf)
		{
			componentOf.DoHighlightTransition();
		}
	}

	public static void TryShowTip(Transform root = null, bool highlight = true, bool ignoreWhenRightClick = true)
	{
		TryShowTip<UIButton>(root, highlight, ignoreWhenRightClick);
	}

	public static void TryShowTip<T>(Transform root = null, bool highlight = true, bool ignoreWhenRightClick = true) where T : UIButton
	{
		actionTooltip = delegate
		{
			InputModuleEX.UpdateEventData();
			T componentOf = InputModuleEX.GetComponentOf<T>();
			if ((bool)componentOf && componentOf.tooltip.enable)
			{
				TooltipManager.Instance.HideTooltips();
				if ((bool)componentOf && (root == null || componentOf.transform.IsChildOf(root)))
				{
					if (highlight)
					{
						componentOf.DoHighlightTransition();
					}
					try
					{
						componentOf.ShowTooltipForced(ignoreWhenRightClick);
					}
					catch
					{
					}
				}
			}
		};
	}
}
