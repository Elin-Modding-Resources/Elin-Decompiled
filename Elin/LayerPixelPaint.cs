using System;
using Empyrean.ColorPicker;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class LayerPixelPaint : ELayer
{
	public PixelPaint paint;

	public GridLayoutGroup layoutColors;

	public ColorPicker picker;

	public Action onApply;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Close();
		}
	}

	public void SetCanvas(TraitCanvas c)
	{
		onApply = delegate
		{
			c.owner.PlaySound(c.owner.material.GetSoundImpact());
			c.owner.renderer.PlayAnime(AnimeID.Shiver);
			Thing thing = c.owner.Split(1);
			thing.c_textureData = paint.tex.EncodeToPNG();
			thing.isModified = true;
			thing.ClearPaintSprite();
			thing.GetPaintSprite();
			thing.renderer.RefreshSprite();
			Close();
		};
		paint.scale = c.SizeGrid;
		paint.size = new Vector2Int(c.Width, c.Height);
		InitPaint();
		if (c.owner.c_textureData != null)
		{
			paint.tex.LoadImage(c.owner.c_textureData);
		}
		paint.imageMask.texture = c.owner.GetSprite().texture;
		paint.imageMask.SetNativeSize();
		paint.imagePreview.SetNativeSize();
	}

	public void InitPaint()
	{
		UIItem t = layoutColors.CreateMold<UIItem>();
		for (int i = 0; i < 8; i++)
		{
			UIItem item = Util.Instantiate(t, layoutColors);
			int _i = i;
			item.button1.icon.color = IntColor.FromInt(ELayer.core.config.colors[_i]);
			item.button1.SetOnClick(delegate
			{
				picker.SelectColor(item.button1.icon.color);
			});
			item.button2.SetOnClick(delegate
			{
				item.button1.icon.color = picker.SelectedColor;
				ELayer.core.config.colors[_i] = IntColor.ToInt(picker.SelectedColor);
				SE.Tab();
			});
		}
		layoutColors.RebuildLayout();
		windows[0].AddBottomButton("apply", delegate
		{
			onApply?.Invoke();
		});
		windows[0].AddBottomButton("loadImage", delegate
		{
			DialogLoad(delegate
			{
			});
		});
		windows[0].AddBottomButton("cancel", delegate
		{
			Close();
		});
		paint.Init();
	}

	public void DialogLoad(Action onLoad = null)
	{
		ELayer.core.WaitForEndOfFrame(delegate
		{
			string[] array = StandaloneFileBrowser.OpenFilePanel("Load PNG Image", CorePath.CustomDrawing, "png", multiselect: false);
			if (array.Length != 0)
			{
				Texture2D texture2D = IO.LoadPNG(array[0]);
				if (texture2D != null)
				{
					int num = (paint.size.x - texture2D.width) / 2;
					int num2 = (paint.size.y - texture2D.height) / 2;
					if (num < 0)
					{
						num = 0;
					}
					if (num2 < 0)
					{
						num2 = 0;
					}
					for (int i = 0; i < texture2D.height && i < paint.size.y; i++)
					{
						for (int j = 0; j < texture2D.width && j < paint.size.x; j++)
						{
							paint.tex.SetPixel(num + j, num2 + i, texture2D.GetPixel(j, i));
						}
					}
					paint.tex.Apply();
					SE.Change();
					UnityEngine.Object.Destroy(texture2D);
				}
			}
		});
	}
}
