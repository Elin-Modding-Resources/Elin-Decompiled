using System;
using UnityEngine;
using UnityEngine.UI;

public class SkinRoot : BaseSkinRoot
{
	[NonSerialized]
	public SkinConfig _config;

	public virtual SkinConfig Config => _config ?? (_config = GetComponent<ISkinRoot>()?.GetSkinConfig());

	public override SkinColorProfile Colors => Config.BG.colors;

	public override SkinAsset_Button GetButton()
	{
		return Config.Button;
	}

	public override SkinColorProfile GetColors(SkinType type)
	{
		return Config.BG.colors;
	}

	public override void ApplySkin(UIImage t)
	{
		SkinConfig config = Config;
		if (t.imageType == ImageType.BG_Window)
		{
			SkinAsset_BG bG = config.BG;
			if (SkinManager.Instance.skinSets[0].bgs[config.bg].redirect != 0)
			{
				config.bgColor = bG.color;
			}
			t.sprite = bG.sprite;
			t.color = config.bgColor;
			t.type = Image.Type.Sliced;
			RectTransform rectTransform = t.Rect();
			rectTransform.sizeDelta = new Vector2(bG.size.x, bG.size.y) + t.sizeFix + new Vector2(config.bgSize, config.bgSize);
			rectTransform.anchoredPosition = bG.offset;
		}
	}

	public override void ApplySkin(UIRawImage t)
	{
		SkinConfig config = Config;
		if (t.imageType == ImageType.BG_Grid)
		{
			SpriteAsset grid = config.Grid;
			t.texture = ((grid.sprite != null) ? grid.sprite.texture : null);
			t.color = config.gridColor;
		}
	}

	public override void ApplySkin(UIText t, FontSource f)
	{
		SkinAsset_BG bG = Config.BG;
		Shadow shadow = t.GetComponent<Shadow>();
		if (bG.textShadow)
		{
			if (!shadow)
			{
				shadow = t.gameObject.AddComponent<Shadow>();
			}
			if ((bool)shadow)
			{
				shadow.enabled = true;
				shadow.effectColor = bG.textShadowColor;
			}
		}
		else if ((bool)shadow)
		{
			shadow.enabled = false;
		}
	}

	public void Reset()
	{
		_config = null;
	}
}
