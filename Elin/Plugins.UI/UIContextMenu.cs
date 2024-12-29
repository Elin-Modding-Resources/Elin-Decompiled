using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIContextMenu : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public static UIContextMenu Current;

	public static float timeSinceClosed;

	public static bool closeOnMouseLeave;

	public UIContextMenuItem defaultButton;

	public UIContextMenuPopper defaultPopper;

	public UIContextMenuItem defaultToggle;

	public UIContextMenuItem defaultSlider;

	public GridLayoutGroup defaultGridLayout;

	public Transform logo;

	public CanvasGroup cg;

	public GameObject[] separators;

	public Action onDestroy;

	public Action onUpdate;

	public UIContextMenuPopper popper;

	public bool destroyOnHide;

	public bool wasCanceled;

	public Material matBlur;

	public bool system;

	public bool alwaysPopLeft;

	public Image bg;

	public SoundData soundPop;

	public Vector2 margin;

	public Vector2 fixPos;

	public UIButton highlightTarget;

	public bool hideOnMouseLeave;

	private Vector2 showPos;

	[NonSerialized]
	public VerticalLayoutGroup layoutGroup;

	[NonSerialized]
	public RectTransform _rect;

	[NonSerialized]
	public bool isPointerOver;

	[NonSerialized]
	public int depth;

	[NonSerialized]
	public UIContextMenu parent;

	[NonSerialized]
	public float timeSinceOpen;

	[NonSerialized]
	public float timeSincePointerLeft;

	[NonSerialized]
	public float updatesSinceOpen;

	[NonSerialized]
	public bool isDestroyed;

	public bool isChild => popper != null;

	public UIContextMenu Root
	{
		get
		{
			if (!parent)
			{
				return this;
			}
			return parent.Root;
		}
	}

	private void Awake()
	{
		layoutGroup = GetComponent<VerticalLayoutGroup>();
		_rect = base.transform as RectTransform;
		base.gameObject.SetActive(value: false);
	}

	private void Update()
	{
		if (onUpdate != null)
		{
			onUpdate();
		}
		if ((bool)highlightTarget)
		{
			highlightTarget.DoHighlightTransition();
		}
		timeSinceOpen += Time.unscaledDeltaTime;
		if (timeSinceOpen < 0.3f)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
		{
			wasCanceled = true;
			Hide();
			return;
		}
		updatesSinceOpen += 1f;
		if (!Current.hideOnMouseLeave && updatesSinceOpen > 10f && !isPointerOver && Input.GetMouseButtonDown(0) && !isChild)
		{
			Hide();
		}
		else if (updatesSinceOpen > 10f && !isPointerOver && !Input.GetMouseButton(0))
		{
			timeSincePointerLeft += Time.unscaledDeltaTime;
			if ((!isChild || !popper.isPointerOver) && timeSincePointerLeft > 0.1f)
			{
				if (Input.GetMouseButtonUp(0))
				{
					Hide();
				}
				if (hideOnMouseLeave && (!isChild || !popper.isPointerOver))
				{
					Hide();
				}
			}
		}
		else
		{
			timeSincePointerLeft = 0f;
		}
	}

	private void LateUpdate()
	{
		if ((bool)highlightTarget)
		{
			highlightTarget.DoHighlightTransition();
		}
	}

	public void OnPointerEnter(PointerEventData data)
	{
		isPointerOver = true;
	}

	public void OnPointerExit(PointerEventData data)
	{
		isPointerOver = false;
	}

	public void Show(UIItem i)
	{
		Show(i.button1);
	}

	public void Show(UIButton b)
	{
		SetHighlightTarget(b);
		Show();
	}

	public void Show(Vector2 pos)
	{
		showPos = pos;
		Show();
	}

	public void Show()
	{
		wasCanceled = false;
		if (isChild)
		{
			hideOnMouseLeave = Current.hideOnMouseLeave;
		}
		else
		{
			hideOnMouseLeave = true;
		}
		if (showPos == Vector2.zero)
		{
			showPos = EInput.uiMousePosition;
		}
		base.gameObject.SetActive(value: true);
		this.RebuildLayout(recursive: true);
		if (!isChild)
		{
			showPos += fixPos;
			if ((bool)soundPop)
			{
				SE.Play(soundPop);
			}
			_rect.anchoredPosition = new Vector2(-10000f, -10000f);
			PositionMenu();
			Current = this;
		}
		if ((bool)TooltipManager.Instance)
		{
			TooltipManager.Instance.HideTooltips();
		}
	}

	public void Hide()
	{
		if (!UIContextMenuManager.Instance.autoClose)
		{
			return;
		}
		isPointerOver = false;
		if (destroyOnHide)
		{
			if (!isDestroyed)
			{
				isDestroyed = true;
				UnityEngine.Object.DestroyImmediate(base.gameObject);
				if (onDestroy != null)
				{
					onDestroy();
				}
				if ((bool)highlightTarget && !InputModuleEX.IsPointerOver(highlightTarget))
				{
					highlightTarget.DoNormalTransition();
				}
			}
		}
		else
		{
			if (!base.gameObject.activeSelf)
			{
				return;
			}
			base.gameObject.SetActive(value: false);
		}
		EInput.DisableIME();
		BaseCore.Instance.eventSystem.SetSelectedGameObject(null);
		BaseCore.Instance.ConsumeInput();
		UIButton.TryShowTip();
	}

	public void ClearItems()
	{
		layoutGroup.DestroyChildren();
	}

	public void AddSeparator(int index = 0)
	{
		UnityEngine.Object.Instantiate(separators[index]).transform.SetParent(layoutGroup.transform, worldPositionStays: false);
	}

	public void AddButton(Func<string> funcText, UnityAction action = null)
	{
		UIContextMenuItem item = UnityEngine.Object.Instantiate(defaultButton);
		item.transform.SetParent(layoutGroup.transform, worldPositionStays: false);
		item.textName.text = funcText();
		Button button = item.button;
		if (action != null)
		{
			button.onClick.AddListener(action);
		}
		button.onClick.AddListener(delegate
		{
			if ((bool)item)
			{
				item.textName.text = funcText();
			}
		});
	}

	public UIButton AddButton(string idLang = "", Action action = null, bool hideAfter = true)
	{
		UIContextMenuItem uIContextMenuItem = UnityEngine.Object.Instantiate(defaultButton);
		uIContextMenuItem.transform.SetParent(layoutGroup.transform, worldPositionStays: false);
		uIContextMenuItem.textName.text = Lang.Get(idLang);
		UIButton uIButton = uIContextMenuItem.button as UIButton;
		if (hideAfter)
		{
			uIButton.onClick.AddListener(Hide);
		}
		if ((bool)popper && hideAfter)
		{
			uIButton.onClick.AddListener(popper.parent.Hide);
		}
		if (action != null)
		{
			uIButton.onClick.AddListener(delegate
			{
				action();
			});
		}
		return uIButton;
	}

	public UIContextMenuItem AddToggle(string idLang = "", bool isOn = false, UnityAction<bool> action = null)
	{
		UIContextMenuItem uIContextMenuItem = UnityEngine.Object.Instantiate(defaultToggle);
		uIContextMenuItem.transform.SetParent(layoutGroup.transform, worldPositionStays: false);
		uIContextMenuItem.textName.text = idLang.lang().ToTitleCase();
		uIContextMenuItem.toggle.isOn = isOn;
		if (action != null)
		{
			uIContextMenuItem.toggle.onValueChanged.AddListener(action);
		}
		return uIContextMenuItem;
	}

	public UIContextMenuItem AddSlider(string text, Func<float, string> textFunc, float value, Action<float> action, float min = 0f, float max = 1f, bool isInt = false, bool hideOther = true, bool useInput = false)
	{
		UIContextMenuItem item = UnityEngine.Object.Instantiate(defaultSlider);
		return AddSlider(item, text, textFunc, value, action, min, max, isInt, hideOther, useInput);
	}

	public UIContextMenuItem AddSlider(string id, string text, Func<float, string> textFunc, float value, Action<float> action, float min = 0f, float max = 1f, bool isInt = false, bool hideOther = true, bool useInput = false)
	{
		UIContextMenuItem item = Util.Instantiate<UIContextMenuItem>("Items/" + id);
		return AddSlider(item, text, textFunc, value, action, min, max, isInt, hideOther, useInput);
	}

	public UIContextMenuItem AddSlider(UIContextMenuItem item, string text, Func<float, string> textFunc, float value, Action<float> action, float min = 0f, float max = 1f, bool isInt = false, bool hideOther = true, bool useInput = false)
	{
		UiInputField input = item.GetComponentInChildren<UiInputField>();
		Slider s = item.slider;
		item.transform.SetParent(layoutGroup.transform, worldPositionStays: false);
		item.textName.text = text.lang();
		s.minValue = min;
		if (max < 0f)
		{
			max = 0f;
		}
		s.maxValue = max;
		s.wholeNumbers = isInt;
		s.SetValueWithoutNotify(value);
		item.textSlider.text = textFunc(value);
		s.onValueChanged.AddListener(delegate(float a)
		{
			item.textSlider.text = textFunc(a);
			action(a);
			if ((bool)input)
			{
				input.text = s.value.ToString() ?? "";
				input.Select();
				input.HideCaret();
			}
		});
		if ((bool)input)
		{
			input.SetActive(useInput);
			input.text = s.value.ToString() ?? "";
			input.onValueChanged.AddListener(delegate(string text)
			{
				if (!text.IsEmpty())
				{
					float num2 = Mathf.Clamp(text.ToInt(), min, max);
					s.SetValueWithoutNotify(num2);
					action(num2);
				}
			});
			input.onUpdate = delegate
			{
				if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
				{
					action(s.value);
					Current?.Hide();
				}
				else if (!input.text.IsEmpty())
				{
					int num = input.text.ToInt();
					if ((float)num > max)
					{
						num = (int)max;
						input.text = num.ToString() ?? "";
						s.SetValueWithoutNotify(num);
					}
				}
			};
			if (useInput)
			{
				input.onDisable = delegate
				{
					action(s.value);
				};
			}
			if (useInput)
			{
				input.Select();
			}
		}
		if (hideOther)
		{
			RectTransform blocker = null;
			EventTrigger component = s.GetComponent<EventTrigger>();
			component.triggers.Clear();
			EventTrigger.Entry entry = new EventTrigger.Entry
			{
				eventID = EventTriggerType.PointerDown
			};
			entry.callback.AddListener(delegate
			{
				blocker = Util.Instantiate<RectTransform>("Items/sliderBlocker", this);
				blocker.SetAsFirstSibling();
				UIContextMenu[] componentsInParent2 = GetComponentsInParent<UIContextMenu>();
				for (int j = 0; j < componentsInParent2.Length; j++)
				{
					componentsInParent2[j].cg.DOFade(0.01f, 1f);
				}
				if ((bool)s && s.gameObject != null)
				{
					s.gameObject.AddComponent<CanvasGroup>().ignoreParentGroups = true;
				}
			});
			component.triggers.Add(entry);
			entry = new EventTrigger.Entry
			{
				eventID = EventTriggerType.PointerUp
			};
			entry.callback.AddListener(delegate
			{
				if (!isPointerOver)
				{
					Root.Hide();
				}
				else
				{
					UIContextMenu[] componentsInParent = GetComponentsInParent<UIContextMenu>();
					foreach (UIContextMenu obj in componentsInParent)
					{
						DOTween.Kill(obj.cg);
						obj.cg.alpha = 1f;
					}
					UnityEngine.Object.Destroy(s.GetComponent<CanvasGroup>());
					UnityEngine.Object.DestroyImmediate(blocker.gameObject);
				}
			});
			component.triggers.Add(entry);
		}
		return item;
	}

	public GridLayoutGroup AddGridLayout()
	{
		GridLayoutGroup gridLayoutGroup = UnityEngine.Object.Instantiate(defaultGridLayout);
		gridLayoutGroup.transform.SetParent(layoutGroup.transform, worldPositionStays: false);
		return gridLayoutGroup;
	}

	public GridLayoutGroup AddGridLayout(string id)
	{
		return Util.Instantiate<GridLayoutGroup>("Items/" + id, layoutGroup);
	}

	public T AddGameObject<T>(T c) where T : Component
	{
		c.transform.SetParent(layoutGroup.transform, worldPositionStays: false);
		return c;
	}

	public T AddGameObject<T>(string id) where T : Component
	{
		return Util.Instantiate<T>("Items/" + id, layoutGroup);
	}

	public UIContextMenu AddOrGetChild(string idLang = "Child")
	{
		foreach (UIContextMenuPopper componentsInDirectChild in base.transform.GetComponentsInDirectChildren<UIContextMenuPopper>())
		{
			if (componentsInDirectChild.id == idLang)
			{
				return componentsInDirectChild.menu;
			}
		}
		return AddChild(idLang);
	}

	public UIContextMenu AddChild(string idLang, TextAnchor anchor)
	{
		UIContextMenu uIContextMenu = AddChild(idLang);
		uIContextMenu.layoutGroup.childAlignment = anchor;
		return uIContextMenu;
	}

	public UIContextMenu AddChild(string idLang = "Child")
	{
		UIContextMenuPopper uIContextMenuPopper = UnityEngine.Object.Instantiate(defaultPopper);
		uIContextMenuPopper.transform.SetParent(layoutGroup.transform, worldPositionStays: false);
		uIContextMenuPopper.textName.text = Lang.Get(idLang);
		uIContextMenuPopper.parent = this;
		uIContextMenuPopper.id = idLang;
		UIContextMenu uIContextMenu = uIContextMenuPopper.CreateMenu();
		uIContextMenu.alwaysPopLeft = alwaysPopLeft;
		uIContextMenu.ClearItems();
		uIContextMenuPopper.button.onClick.RemoveAllListeners();
		return uIContextMenuPopper.menu;
	}

	private void PositionMenu()
	{
		base.gameObject.SetActive(value: true);
		this.RebuildLayout(recursive: true);
		_rect.anchoredPosition = showPos;
		RectTransform obj = _rect.parent as RectTransform;
		Vector3 localPosition = _rect.localPosition;
		Vector3 vector = obj.rect.min - _rect.rect.min;
		Vector3 vector2 = obj.rect.max - _rect.rect.max;
		localPosition.x = Mathf.Clamp(_rect.localPosition.x, vector.x, vector2.x);
		localPosition.y = Mathf.Clamp(_rect.localPosition.y, vector.y, vector2.y + margin.y);
		_rect.localPosition = localPosition;
	}

	public UIContextMenu SetHighlightTarget(UIItem i)
	{
		return SetHighlightTarget(i.button1);
	}

	public UIContextMenu SetHighlightTarget(UIButton t)
	{
		highlightTarget = t;
		return this;
	}
}
