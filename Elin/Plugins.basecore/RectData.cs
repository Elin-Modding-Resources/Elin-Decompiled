using System;
using UnityEngine;

[Serializable]
public class RectData
{
	public Vector2 position = new Vector2(-1f, -1f);

	public Vector2 size = new Vector2(-1f, -1f);

	public Vector2 pivot;

	public Vector2 anchorMin;

	public Vector2 anchorMax;

	public void Apply(RectTransform rect)
	{
		rect.SetRect(position.x, position.y, size.x, size.y, pivot.x, pivot.y, anchorMax.x, anchorMax.y, anchorMax.x, anchorMax.y);
	}
}
