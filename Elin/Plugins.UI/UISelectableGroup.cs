using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISelectableGroup : MonoBehaviour, IUISkin
{
	public enum ColorType
	{
		General = 0,
		Tab = 5,
		DontChange = 99
	}

	public ColorType colorType;

	public LayoutGroup layoutButtons;

	public int selectOnInitialize;

	public bool colorWhenSelected;

	public bool checkbox;

	public bool loopNavigation;

	public bool autoInitialize = true;

	public bool selectOnClick = true;

	public bool useIcon;

	public bool useTransition = true;

	public bool fixRefreshButtons;

	public bool colorWhenDisabled;

	public Color colorDefault;

	public Color colorSelected;

	public Color colorNoSelection;

	public Color colorDisabled;

	public SkinRootStatic rootSkin;

	private SkinColorProfileEx.GroupColors colors;

	[NonSerialized]
	public UIButton selected;

	[NonSerialized]
	public List<UIButton> list = new List<UIButton>();

	public UnityAction<int> onClick;

	private bool refreshButtons;

	public SkinRootStatic Skin
	{
		get
		{
			if (!(rootSkin != null))
			{
				return SkinManager.Instance.currentSkin;
			}
			return rootSkin;
		}
	}

	private void Awake()
	{
		SetColors();
		if (!layoutButtons)
		{
			layoutButtons = GetComponent<LayoutGroup>();
		}
		if (autoInitialize)
		{
			Init(selectOnInitialize);
		}
		if (fixRefreshButtons)
		{
			refreshButtons = true;
		}
	}

	public void ApplySkin()
	{
		SetColors();
		RefreshButtons();
	}

	private void OnDisable()
	{
		if (fixRefreshButtons)
		{
			refreshButtons = true;
		}
	}

	public UIButton AddButton(UIButton mold, string text)
	{
		UIButton uIButton = Util.Instantiate(mold, layoutButtons);
		uIButton.mainText.text = text;
		return uIButton;
	}

	public void RemoveButton(UIButton button)
	{
		list.Remove(button);
	}

	public void SetButton(UIButton button)
	{
		list.Add(button);
		SetGroup(list.Count - 1);
		if (checkbox)
		{
			button.selected = false;
			button.DoNormalTransition();
		}
	}

	public void SetGroup(int id)
	{
		UIButton uIButton = list[id];
		uIButton.group = this;
		if (onClick != null)
		{
			uIButton.onClick.RemoveAllListeners();
			uIButton.onClick.AddListener(delegate
			{
				onClick(id);
			});
		}
		if (selectOnClick)
		{
			uIButton.onClick.RemoveListener(uIButton.Toggle);
			uIButton.onClick.AddListener(uIButton.Toggle);
		}
	}

	public virtual void Init(int index = 0, UnityAction<int> action = null, bool directChildren = false)
	{
		list.Clear();
		if (action != null)
		{
			onClick = action;
		}
		UIButton[] array = (directChildren ? base.transform.GetComponentsInDirectChildren<UIButton>().ToArray() : base.transform.GetComponentsInChildren<UIButton>(includeInactive: true));
		foreach (UIButton uIButton in array)
		{
			if (!uIButton.tag.Contains("IgnoreSelectable") && uIButton.gameObject.activeSelf)
			{
				SetButton(uIButton);
			}
		}
		if (checkbox)
		{
			selected = null;
			RefreshButtons();
		}
		else
		{
			Select(index);
		}
	}

	public void SetLoop()
	{
		if (loopNavigation)
		{
			for (int i = 0; i < list.Count; i++)
			{
				Navigation navigation = list[i].navigation;
				navigation.mode = Navigation.Mode.Explicit;
				navigation.selectOnLeft = ((i == 0) ? list[list.Count - 1] : list[i - 1]);
				navigation.selectOnRight = ((i + 1 == list.Count) ? list[0] : list[i + 1]);
				list[i].navigation = navigation;
			}
		}
	}

	public void Select(UIButton button, bool check = true)
	{
		if (checkbox)
		{
			selected = null;
			button.SetCheck(check);
		}
		else if (!(selected == button))
		{
			selected = button;
			RefreshButtons();
		}
	}

	public void Select(int id)
	{
		Select((id == -1 || id >= list.Count) ? null : list[id]);
	}

	public void Select(string id)
	{
		foreach (UIButton item in list)
		{
			if (item.name == id)
			{
				Select(item);
			}
		}
	}

	public void Select(Func<UIButton, bool> func)
	{
		foreach (UIButton item in list)
		{
			if (func(item))
			{
				Select(item);
				break;
			}
		}
	}

	public void RefreshButtons()
	{
		foreach (UIButton item in list)
		{
			if (!item || item.gameObject == null)
			{
				continue;
			}
			if (colorWhenDisabled && !item.interactable)
			{
				item.image.color = colorDisabled;
				continue;
			}
			if (checkbox)
			{
				if (useIcon)
				{
					item.icon.color = colorDefault;
				}
				else
				{
					item.image.color = colorDefault;
				}
				if (useTransition)
				{
					item.selected = false;
					item.DoNormalTransition();
				}
				break;
			}
			if (!selected && colorWhenSelected)
			{
				item.SetCheck(check: false);
				if (useIcon)
				{
					item.icon.color = colorNoSelection;
				}
				else
				{
					item.image.color = colorNoSelection;
				}
			}
			else if ((bool)selected && item == selected)
			{
				item.SetCheck(check: true);
				if (useIcon)
				{
					item.icon.color = colorSelected;
				}
				else
				{
					item.image.color = colorSelected;
				}
				if (useTransition)
				{
					item.selected = true;
					item.DoNormalTransition();
				}
			}
			else
			{
				item.SetCheck(check: false);
				if (useIcon)
				{
					item.icon.color = colorDefault;
				}
				else
				{
					item.image.color = colorDefault;
				}
				if (useTransition)
				{
					item.selected = false;
					item.DoNormalTransition();
				}
			}
		}
	}

	public void ForceDefaultColor(bool useNoSelection = false)
	{
		selected = null;
		foreach (UIButton item in list)
		{
			item.SetCheck(check: false);
			if (useNoSelection)
			{
				if (useIcon)
				{
					item.icon.color = colorNoSelection;
				}
				else
				{
					item.image.color = colorNoSelection;
				}
			}
			else if (useIcon)
			{
				item.icon.color = colorDefault;
			}
			else
			{
				item.image.color = colorDefault;
			}
		}
	}

	public void ToggleInteractable(bool enable)
	{
		foreach (UIButton item in list)
		{
			item.interactable = enable;
		}
	}

	public void SetColors()
	{
		if (colorType != ColorType.DontChange)
		{
			SkinColorProfileEx colorsEx = Skin.ColorsEx;
			if (colorType == ColorType.Tab)
			{
				colors = colorsEx.tab;
			}
			else
			{
				colors = colorsEx.general;
			}
			colorDefault = colors.colorDefault;
			colorSelected = colors.colorSelected;
			colorNoSelection = colors.colorNoSelection;
		}
	}

	private void LateUpdate()
	{
		if (refreshButtons)
		{
			refreshButtons = false;
			RefreshButtons();
		}
	}
}
