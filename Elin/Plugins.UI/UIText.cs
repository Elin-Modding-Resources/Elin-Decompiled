using System;
using UnityEngine;
using UnityEngine.UI;

public class UIText : Text, IUISkin
{
	public static int globalSizeMod;

	public string lang;

	public BaseSkinRoot skinRoot;

	public FontType fontType;

	public FontColor fontColor;

	public SkinType skinType;

	public int size;

	public HyphenationJpn hyphenation;

	public UIButton button;

	public Typewriter typewriter;

	public float orgSpacing;

	[NonSerialized]
	public Color orgColor = Color.white;

	protected override void Awake()
	{
		ApplySkin();
		base.Awake();
	}

	public void ApplySkin()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		SkinManager instance = SkinManager.Instance;
		int num = size + 19 + globalSizeMod;
		SkinManager.FontSet fontSet = instance.fontSet;
		FontSource fontSource = instance.FontList[0];
		if (fontType != FontType.DontChange)
		{
			int num2 = 0;
			switch (fontType)
			{
			case FontType.UI:
				fontSource = fontSet.ui.source;
				num2 = Mathf.Min(fontSet.ui.sizeMod, SkinManager.LimitFontSizeMod_UI);
				break;
			case FontType.Widget:
				fontSource = fontSet.widget.source;
				num2 = fontSet.widget.sizeMod;
				break;
			case FontType.Chatbox:
				fontSource = fontSet.chatbox.source;
				num2 = fontSet.chatbox.sizeMod;
				break;
			case FontType.Balloon:
				fontSource = fontSet.balloon.source;
				num2 = fontSet.balloon.sizeMod;
				break;
			case FontType.Dialog:
				fontSource = fontSet.dialog.source;
				num2 = fontSet.dialog.sizeMod;
				break;
			case FontType.News:
				fontSource = fontSet.news.source;
				num2 = fontSet.news.sizeMod;
				break;
			}
			if (fontSource == null)
			{
				fontSource = instance.FontList[0];
			}
			base.font = fontSource.font;
			num += fontSource.sizeFix + num2;
		}
		if (base.fontSize != num)
		{
			base.fontSize = num;
		}
		if (base.resizeTextForBestFit)
		{
			base.resizeTextMaxSize = base.fontSize;
		}
		if ((bool)button && (bool)button.skinRoot)
		{
			SkinAsset_Button skinAsset_Button = button.skinRoot.GetButton();
			if (fontColor != 0)
			{
				this.color = skinAsset_Button.textColor;
			}
			Shadow component = GetComponent<Shadow>();
			if (skinAsset_Button.textShadow)
			{
				if ((bool)component)
				{
					component.enabled = true;
					component.effectColor = skinAsset_Button.textShadowColor;
				}
			}
			else if ((bool)component)
			{
				component.enabled = false;
			}
			return;
		}
		if (orgSpacing == 0f)
		{
			orgSpacing = base.lineSpacing;
		}
		if (fontColor == FontColor.DontChange && orgColor == Color.white)
		{
			orgColor = this.color;
		}
		BaseSkinRoot obj = skinRoot ?? SkinManager.CurrentSkin;
		SkinColorProfile colors = obj.GetColors(skinType);
		float num3 = 0.5f;
		bool flag = fontColor == FontColor.DontChange;
		float num4 = 1f + (flag ? 0f : colors.contrast) + fontSource.contrast;
		obj.ApplySkin(this, fontSource);
		Color color = (flag ? orgColor : colors.GetTextColor(fontColor));
		if (base.fontStyle == FontStyle.Bold)
		{
			if (fontSource.alwaysBold)
			{
				base.fontStyle = FontStyle.Normal;
			}
			else
			{
				color.a *= fontSource.boldAlpha;
			}
		}
		num3 = ((!(color.r + color.g + color.b > 1.5f)) ? (0.5f - (flag ? 0f : colors.strength) - (Lang.isJP ? (fontSource.strength + fontSource.strengthFixJP) : fontSource.strength)) : (0.5f + (flag ? 0f : colors.strength) + (Lang.isJP ? (fontSource.strength + fontSource.strengthFixJP) : fontSource.strength)));
		color.r = (color.r - 0.5f) * num4 + num3;
		color.g = (color.g - 0.5f) * num4 + num3;
		color.b = (color.b - 0.5f) * num4 + num3;
		this.color = color;
		base.lineSpacing = orgSpacing * fontSource.lineSpacing;
		_ = (bool)hyphenation;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (!string.IsNullOrEmpty(lang))
		{
			m_Text = Lang.Get(lang);
		}
	}

	public void SetText(string s)
	{
		if (s == null)
		{
			s = "";
		}
		if ((bool)hyphenation)
		{
			if (Lang.setting.hyphenation)
			{
				if (hyphenation.evaluateLate)
				{
					BaseCore.Instance.actionsLateUpdate.Add(delegate
					{
						if (this != null)
						{
							hyphenation._SetText(s, Lang.setting.hyphenation);
							hyphenation.RebuildLayoutTo<Canvas>();
						}
					});
				}
				else
				{
					hyphenation._SetText(s, Lang.setting.hyphenation);
				}
			}
			else
			{
				base.horizontalOverflow = HorizontalWrapMode.Wrap;
				text = s;
			}
		}
		else
		{
			text = s;
		}
		lang = null;
		OnSetText();
	}

	public void SetText(string s, Color c)
	{
		SetText(s);
		fontColor = FontColor.DontChange;
		orgColor = c;
		color = c;
		OnSetText();
	}

	public void SetText(string s, FontColor c)
	{
		SetText(s);
		if (c != FontColor.Ignore)
		{
			fontColor = c;
			ApplySkin();
		}
		OnSetText();
	}

	private void OnSetText()
	{
		if ((bool)typewriter)
		{
			typewriter.OnSetText();
		}
	}

	public UIText SetSize(int a)
	{
		size = a;
		ApplySkin();
		return this;
	}

	public void SetColor(FontColor c)
	{
		fontColor = c;
		ApplySkin();
	}

	public void SetLang(string idLang)
	{
		lang = idLang;
		text = Lang.Get(lang);
	}

	public void OnChangeLanguage()
	{
		if (!string.IsNullOrEmpty(lang))
		{
			text = Lang.Get(lang);
		}
	}
}
