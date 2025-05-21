using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemGeneral : UIItem, IPrefImage
{
	public enum Mode
	{
		Default,
		Slave
	}

	private const int IconSize = 40;

	private const int IconPadding = 10;

	private const int ButtonPaddingWhenIcon = 3;

	public LayoutGroup layout;

	public int paddingSubText = 50;

	public Card card;

	private int count;

	public void SetChara(Chara c, Mode mode = Mode.Default)
	{
		card = c;
		c.SetImage(button1.icon);
		string text = c.Name;
		if (EClass.Branch?.uidMaid == c.uid)
		{
			text += ("(" + "maid".lang() + ")").TagSize(12);
		}
		FontColor c2 = FontColor.ButtonGeneral;
		if (c.isDead)
		{
			c2 = FontColor.Bad;
		}
		else if (c.IsPCParty)
		{
			c2 = FontColor.Good;
		}
		else if (c.hp < c.MaxHP / 2 && c.currentZone == EClass._zone)
		{
			c2 = FontColor.Warning;
		}
		if (mode == Mode.Slave)
		{
			text = text + " " + c.bio.TextBioSlave(c);
		}
		button1.mainText.SetText(text, c2);
		_ = button1.icon.rectTransform;
	}

	public RenderRow GetRenderRow()
	{
		return card?.sourceRenderCard;
	}

	public void OnRefreshPref()
	{
		if (card != null && card.isChara)
		{
			SetChara(card.Chara);
		}
	}

	public void Clear()
	{
		if (count <= 0)
		{
			return;
		}
		foreach (UIButton componentsInDirectChild in base.transform.GetComponentsInDirectChildren<UIButton>())
		{
			if (componentsInDirectChild != button1)
			{
				UnityEngine.Object.DestroyImmediate(componentsInDirectChild.gameObject);
			}
		}
		count = 0;
	}

	public UIButton AddSubButton(Sprite sprite, Action action, string lang = null, Action<UITooltip> onTooltip = null)
	{
		UIButton uIButton = Util.Instantiate<UIButton>("UI/Element/Button/SubButton", base.transform);
		uIButton.Rect().anchoredPosition = new Vector2(count * -40 - 20 - 10, 0f);
		uIButton.icon.sprite = sprite;
		uIButton.onClick.AddListener(delegate
		{
			action();
		});
		if (!lang.IsEmpty())
		{
			uIButton.tooltip.enable = true;
			uIButton.tooltip.lang = lang;
		}
		if (onTooltip != null)
		{
			uIButton.tooltip.id = "note";
			uIButton.tooltip.onShowTooltip = onTooltip;
			uIButton.tooltip.enable = true;
		}
		uIButton.highlightTarget = button1;
		count++;
		return uIButton;
	}

	public void SetMainText(string lang, Sprite sprite = null, bool disableMask = true)
	{
		button1.mainText.SetText(lang.lang());
		if ((bool)sprite)
		{
			button1.icon.sprite = sprite;
			button1.icon.SetNativeSize();
			if (disableMask)
			{
				DisableMask();
			}
		}
		else
		{
			DisableIcon();
		}
	}

	public UIButton SetSubText(string lang, int x, FontColor c = FontColor.Default, TextAnchor align = TextAnchor.MiddleLeft)
	{
		button1.subText.SetActive(enable: true);
		button1.subText.SetText(lang.lang(), c);
		button1.subText.alignment = align;
		button1.mainText.rectTransform.sizeDelta = new Vector2(x - paddingSubText, 20f);
		button1.subText.rectTransform.anchoredPosition = new Vector2(x, 0f);
		return button1;
	}

	public UIButton SetSubText2(string lang, FontColor c = FontColor.Default, TextAnchor align = TextAnchor.MiddleRight)
	{
		button1.subText2.SetActive(enable: true);
		button1.subText2.SetText(lang.lang(), c);
		button1.subText2.alignment = align;
		return button1;
	}

	public T AddPrefab<T>(string id) where T : Component
	{
		return Util.Instantiate<T>("UI/Element/Item/Extra/" + id, base.transform);
	}

	public void SetSound(SoundData data = null)
	{
		button1.soundClick = data ?? SE.DataClick;
	}

	public void DisableIcon()
	{
		button1.icon.transform.parent.SetActive(enable: false);
		if (!button1.keyText)
		{
			button1.mainText.rectTransform.anchoredPosition = new Vector2(20f, 0f);
		}
	}

	public void DisableMask()
	{
		image2.enabled = false;
	}

	public void Build()
	{
		RectTransform rectTransform = button1.Rect();
		if (count > 0)
		{
			rectTransform.sizeDelta = new Vector2(count * -40 - 10 - 3, 0f);
		}
	}
}
