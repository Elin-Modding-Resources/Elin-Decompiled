using System;
using UnityEngine;

public class UIButtonHighlighter : MonoBehaviour
{
	public Func<bool> killCondition;

	public UIButton button;

	public Sprite sprite;

	public void Set(Func<bool> _killCondition)
	{
		killCondition = _killCondition;
		button = GetComponent<UIButton>();
		sprite = button.image.sprite;
		button.image.sprite = button.spriteState.highlightedSprite;
	}

	private void Update()
	{
		if (killCondition())
		{
			button.image.sprite = sprite;
			UnityEngine.Object.Destroy(this);
		}
	}
}
