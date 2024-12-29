using System;
using UnityEngine;

public class SkinRootStatic : BaseSkinRoot
{
	[Serializable]
	public class BG
	{
		public Sprite sprite;

		public Vector2 size;

		public Color color = Color.white;
	}

	[Serializable]
	public class Assets
	{
		public BG bgWindow;

		public BG bgWindowSlim;

		public BG bgWindowBoard;

		public BG bgInner;

		public BG bgTooltip;

		public BG bgTooltipAlt;

		public BG bgContext;

		public BG bgDialog;

		public BG frame1;

		public SkinAsset_BG BG1;

		public SkinAsset_BG BG2;

		public SkinAsset_Button Button1;
	}

	[Serializable]
	public class SkinColors
	{
		public SkinRootStatic skinTooltip;

		public SkinColorProfile _default;
	}

	[Serializable]
	public class Positions
	{
		public RectOffset paddingWindow;

		public Vector2 bottom;

		public Vector2 tab;

		public Vector2 caption;

		public Vector2 windowCorner;

		public Vector2 innerCorner;

		public Vector2 leftMenu;
	}

	public SkinColors colors;

	public SkinColorProfileEx colorsEx;

	public bool darkSkin;

	public bool textShadow;

	public bool generateWideSprite;

	public bool useDeco;

	public bool useFrame;

	public bool captionOutline;

	public bool captionBG;

	public bool alwaysShadow;

	public bool noShadow;

	public Color textShadowColor;

	public Color shadowColor;

	public Texture2D skinTexture;

	public float bgAlpha;

	public float bgAlpha2;

	public float transparency;

	public Assets assets;

	public Positions positions;

	private Sprite spriteWide;

	public override SkinColorProfile Colors => colors._default;

	public override SkinColorProfileEx ColorsEx => colorsEx;

	public SkinRootStatic GetSkin(SkinType type)
	{
		return type switch
		{
			SkinType.Tooltip => colors.skinTooltip, 
			SkinType.Light => SkinManager.Instance.skinLight, 
			SkinType.Dark => SkinManager.Instance.skinDark, 
			SkinType.Title => SkinManager.Instance.skinTitle, 
			_ => this, 
		};
	}

	public override SkinColorProfile GetColors(SkinType type)
	{
		return GetSkin(type).colors._default;
	}

	public override void ApplySkin(UIImage t)
	{
		switch (t.imageType)
		{
		case ImageType.BG_Window:
			t.sprite = assets.bgWindow.sprite;
			t.color = assets.bgWindow.color;
			if (t.useSkinSize)
			{
				t.Rect().sizeDelta = assets.bgWindow.size;
			}
			break;
		case ImageType.BG_Window_Slim:
			t.sprite = assets.bgWindowSlim.sprite;
			t.color = assets.bgWindowSlim.color;
			if (t.useSkinSize)
			{
				t.Rect().sizeDelta = assets.bgWindowSlim.size;
			}
			break;
		case ImageType.BG_Window_Board:
			t.sprite = assets.bgWindowBoard.sprite;
			t.color = assets.bgWindowBoard.color;
			if (t.useSkinSize)
			{
				t.Rect().sizeDelta = assets.bgWindowBoard.size;
			}
			break;
		case ImageType.BG_Inner:
			t.sprite = assets.bgInner.sprite;
			t.color = assets.bgInner.color;
			if (t.useSkinSize)
			{
				t.Rect().sizeDelta = assets.bgInner.size;
			}
			break;
		case ImageType.BG_Dialog:
			t.sprite = assets.bgDialog.sprite;
			t.color = assets.bgDialog.color;
			if (t.useSkinSize)
			{
				t.Rect().sizeDelta = assets.bgDialog.size;
			}
			break;
		case ImageType.BG_Tooltip:
			t.sprite = assets.bgTooltip.sprite;
			t.color = assets.bgTooltip.color;
			if (t.useSkinSize)
			{
				t.Rect().sizeDelta = assets.bgTooltip.size;
			}
			break;
		case ImageType.BG_TooltipAlt:
			t.sprite = assets.bgTooltipAlt.sprite;
			t.color = assets.bgTooltipAlt.color;
			if (t.useSkinSize)
			{
				t.Rect().sizeDelta = assets.bgTooltipAlt.size;
			}
			break;
		case ImageType.BG_Context:
			t.sprite = assets.bgContext.sprite;
			t.color = assets.bgContext.color;
			if (t.useSkinSize)
			{
				t.Rect().sizeDelta = assets.bgContext.size;
			}
			break;
		}
		if (!t.sprite)
		{
			t.sprite = assets.bgWindow.sprite;
		}
	}
}
