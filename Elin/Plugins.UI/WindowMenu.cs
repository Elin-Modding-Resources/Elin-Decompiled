using System;
using UnityEngine;
using UnityEngine.UI;

public class WindowMenu
{
	public string id;

	public Window window;

	public LayoutGroup layout;

	public Transform header;

	private bool initialized;

	public WindowMenu(LayoutGroup _layout)
	{
		layout = _layout;
		initialized = true;
	}

	public WindowMenu(string _id, Window _window)
	{
		id = _id;
		window = _window;
	}

	public void Init()
	{
		if (!initialized)
		{
			layout = Util.Instantiate<LayoutGroup>("UI/Window/Base/Element/WindowMenu" + id, window);
			header = layout.transform.Find("header");
			initialized = true;
			SkinRootStatic currentSkin = SkinManager.CurrentSkin;
			if (id == "Left")
			{
				layout.Rect().anchoredPosition = currentSkin.positions.leftMenu;
			}
		}
	}

	public void Clear()
	{
		Init();
		layout.DestroyChildren();
		if ((bool)header)
		{
			header.SetActive(enable: false);
		}
	}

	public UIButton AddButton(string idLang, Action<UIButton> onClick, Sprite sprite = null, string idButton = "Default")
	{
		Init();
		UIButton b = _AddButton(idButton, sprite, onClick);
		b.onClick.AddListener(delegate
		{
			onClick(b);
		});
		b.mainText.text = idLang.lang();
		if ((bool)header)
		{
			header.SetActive(enable: true);
		}
		return b;
	}

	public UIButton AddButtonSimple(Func<string> funcText, Action<UIButton> onClick = null, Sprite sprite = null, string idButton = "Simple")
	{
		Init();
		UIButton b = _AddButton(idButton, sprite, onClick);
		b.onClick.AddListener(delegate
		{
			onClick?.Invoke(b);
			SE.Click();
			if ((bool)b)
			{
				b.subText.text = funcText().lang();
			}
		});
		b.subText.text = funcText().lang();
		if ((bool)header)
		{
			header.SetActive(enable: true);
		}
		return b;
	}

	public UIButton AddButton2Line(string idLang, Func<string> funcText, Action<UIButton> onClick = null, Sprite sprite = null, string idButton = "2line")
	{
		Init();
		UIButton b = _AddButton(idButton, sprite, onClick);
		b.onClick.AddListener(delegate
		{
			onClick?.Invoke(b);
			SE.Click();
			if ((bool)b)
			{
				b.subText.text = funcText().lang();
			}
		});
		b.mainText.text = idLang.lang();
		b.subText.text = funcText().lang();
		if ((bool)header)
		{
			header.SetActive(enable: true);
		}
		return b;
	}

	public UIButton _AddButton(string idButton, Sprite sprite, Action<UIButton> onClick)
	{
		Init();
		UIButton uIButton = Util.Instantiate<UIButton>("UI/Window/Base/Element/ButtonWindowMenu_" + idButton, layout);
		if ((bool)sprite)
		{
			uIButton.icon.sprite = sprite;
			uIButton.icon.SetActive(enable: true);
		}
		if ((bool)header)
		{
			header.SetActive(enable: true);
		}
		return uIButton;
	}
}
