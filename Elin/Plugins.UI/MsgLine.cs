using System;
using UnityEngine;
using UnityEngine.UI;

public class MsgLine : MonoBehaviour
{
	public HorizontalLayoutGroup layout;

	public RectTransform rect;

	[NonSerialized]
	public MsgBlock block;

	public MsgBox.Prefabs prefabs => block.box.prefabs;

	private void Awake()
	{
		rect = this.Rect();
	}

	public void Append(string s, FontColor col)
	{
		UIText uIText = PoolManager.Spawn(prefabs.text, layout);
		uIText.SetText(s, col);
		uIText.RebuildLayout();
		AddElement(uIText);
	}

	public void Append(Sprite s, bool fitLine = true)
	{
		Transform transform = PoolManager.Spawn(prefabs.image, layout);
		Image componentInChildren = transform.GetComponentInChildren<Image>();
		componentInChildren.sprite = s;
		if (fitLine)
		{
			RectTransform rectTransform = transform.Rect();
			rectTransform.sizeDelta = new Vector2(rect.sizeDelta.y, rect.sizeDelta.y);
			componentInChildren.transform.Rect().sizeDelta = rectTransform.sizeDelta;
		}
		AddElement(transform);
	}

	private void AddElement(Component c)
	{
		if (rect.sizeDelta.x > block.box.maxWidth / 3f)
		{
			_ = rect.sizeDelta.x + c.Rect().sizeDelta.x;
			_ = block.box.maxWidth;
		}
	}

	private void AddElement(Component c, MsgLine line)
	{
		c.transform.SetParent(line.transform, worldPositionStays: false);
		line.layout.RebuildLayout();
	}

	public void Reset()
	{
		for (int num = base.transform.childCount - 1; num >= 0; num--)
		{
			PoolManager.DespawnOrDestroy(base.transform.GetChild(num));
		}
		this.RebuildLayout();
	}
}
