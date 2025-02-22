using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkinManager : MonoBehaviour
{
	[Serializable]
	public class FontData
	{
		public int sizeMod;

		public int index;

		public FontSource source => Instance.FontList[index];
	}

	[Serializable]
	public class FontSaveData
	{
		public int size;

		public int index;
	}

	public class FontSet
	{
		public FontData ui = new FontData();

		public FontData chatbox = new FontData();

		public FontData balloon = new FontData();

		public FontData dialog = new FontData();

		public FontData widget = new FontData();

		public FontData news = new FontData();
	}

	public static SkinManager _Instance;

	public static int LimitFontSizeMod_UI = 999;

	public List<FontSource> fontList;

	public List<FontSource> fontListOriginal;

	public List<SkinSet> skinSets;

	public List<SkinRootStatic> mainSkins;

	public SkinRootStatic currentSkin;

	public SkinRootStatic defaultSkin;

	public SkinRootStatic skinLight;

	public SkinRootStatic skinDark;

	public SkinRootStatic skinTitle;

	public static SkinRootStatic tempSkin;

	public Action onSetSkin;

	public TMP_FontAsset fontAsset;

	public TMP_FontAsset defaultFontAsset;

	[NonSerialized]
	public Texture2D originalTexture;

	public FontSet fontSet = new FontSet();

	public Dictionary<string, FontSource> dictFonts;

	public static SkinManager Instance => _Instance ?? (_Instance = UnityEngine.Object.FindObjectOfType<SkinManager>());

	public List<FontSource> FontList => fontList;

	public static SkinRootStatic CurrentSkin => tempSkin ?? Instance.currentSkin;

	public static SkinColorProfile CurrentColors => CurrentSkin.colors._default;

	public static SkinColorProfile DarkColors => Instance.skinDark.colors._default;

	public void SetFonts(FontSaveData dataUI, FontSaveData dataChatbox, FontSaveData dataBalloon, FontSaveData dataDialog, FontSaveData dataWidget, FontSaveData dataNews)
	{
		fontSet.ui.index = dataUI.index;
		fontSet.ui.sizeMod = dataUI.size - 4;
		fontSet.chatbox.index = dataChatbox.index;
		fontSet.chatbox.sizeMod = dataChatbox.size - 3;
		fontSet.balloon.index = dataBalloon.index;
		fontSet.balloon.sizeMod = dataBalloon.size - 1;
		fontSet.dialog.index = dataDialog.index;
		fontSet.dialog.sizeMod = dataDialog.size - 3;
		fontSet.widget.index = dataWidget.index;
		fontSet.widget.sizeMod = dataWidget.size - 2;
		fontSet.news.index = dataNews.index;
		fontSet.news.sizeMod = dataNews.size - 2;
		UIText[] componentsInChildren = BaseCore.Instance.canvas.GetComponentsInChildren<UIText>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ApplySkin();
		}
	}

	public void InitFont()
	{
		fontList.Clear();
		for (int i = 0; i < fontListOriginal.Count; i++)
		{
			fontList.Add(fontListOriginal[i]);
		}
		if (dictFonts == null)
		{
			dictFonts = new Dictionary<string, FontSource>();
			foreach (FontSource item in fontListOriginal)
			{
				dictFonts.Add(item._name, item);
			}
		}
		foreach (LangSetting.FontSetting item2 in Lang.setting.listFont)
		{
			FontSource fontSource = dictFonts.TryGetValue(item2.id);
			if (fontSource == null)
			{
				fontSource = dictFonts["Arial"].Instantiate();
				Font font = Font.CreateDynamicFontFromOSFont(item2.id, item2.importSize);
				if (font != null)
				{
					fontSource.font = font;
				}
				_ = "*" + item2.id;
				fontSource._name = "*" + item2.id + ((font == null) ? "(NotFound)" : "");
			}
			fontList[item2.index] = fontSource;
			Debug.Log("Loaded Font:" + item2.id + "/" + fontSource._name + "/" + fontSource.font?.ToString() + "/" + Lang.langCode + "/" + Lang.setting.id);
		}
	}

	public void SetMainSkin(int id)
	{
		SetMainSkin(mainSkins[id]);
	}

	public void SetMainSkin()
	{
		SetMainSkin((currentSkin == mainSkins[0]) ? mainSkins[1] : mainSkins[0]);
	}

	public void SetMainSkin(SkinRootStatic dest)
	{
		if (dest.skinTexture != currentSkin.skinTexture)
		{
			if (dest.skinTexture == defaultSkin.skinTexture)
			{
				Graphics.CopyTexture(originalTexture, defaultSkin.skinTexture);
			}
			else
			{
				if (!originalTexture)
				{
					Texture2D skinTexture = defaultSkin.skinTexture;
					originalTexture = new Texture2D(skinTexture.width, skinTexture.height, skinTexture.format, mipChain: false);
					Graphics.CopyTexture(skinTexture, originalTexture);
				}
				Graphics.CopyTexture(dest.skinTexture, defaultSkin.skinTexture);
			}
		}
		currentSkin = dest;
		if (onSetSkin != null)
		{
			onSetSkin();
		}
	}
}
