using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[RequireComponent(typeof(RectTransform))]
public class LayoutMaxSize : LayoutElement
{
	public float maxHeight = -1f;

	public float maxWidth = -1f;

	public override void CalculateLayoutInputHorizontal()
	{
		base.CalculateLayoutInputHorizontal();
		UpdateMaxSizes();
	}

	public override void CalculateLayoutInputVertical()
	{
		base.CalculateLayoutInputVertical();
		UpdateMaxSizes();
	}

	protected override void OnRectTransformDimensionsChange()
	{
		base.OnRectTransformDimensionsChange();
		UpdateMaxSizes();
	}

	private void UpdateMaxSizes()
	{
		if (maxWidth != -1f)
		{
			RectTransform rectTransform = this.Rect();
			if (rectTransform.sizeDelta.x > maxWidth)
			{
				rectTransform.sizeDelta = new Vector2(maxWidth, rectTransform.sizeDelta.y);
				preferredWidth = maxWidth;
			}
		}
	}
}
