using UnityEngine;
using UnityEngine.UI;

public class PopItemText : PopItem
{
	public UIText text;

	public Image image;

	public Outline outline2;

	public bool setNativeSize = true;

	public void SetText(string s, Sprite sprite = null, Color c = default(Color))
	{
		if ((bool)text)
		{
			if (c != default(Color))
			{
				text.SetText(s, c);
			}
			else
			{
				text.SetText(s);
			}
			text.SetActive(!s.IsEmpty());
		}
		if ((bool)image)
		{
			image.SetActive(sprite);
			if ((bool)sprite)
			{
				image.sprite = sprite;
				if (setNativeSize)
				{
					image.SetNativeSize();
				}
			}
		}
		if ((bool)outline2)
		{
			outline2.enabled = PopManager.outlineAlpha >= 0;
			Color effectColor = outline2.effectColor;
			effectColor.a = 0.01f * (float)PopManager.outlineAlpha;
			outline2.effectColor = effectColor;
		}
	}
}
