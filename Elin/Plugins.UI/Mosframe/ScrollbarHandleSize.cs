using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mosframe;

[DisallowMultipleComponent]
[RequireComponent(typeof(ScrollRect))]
public class ScrollbarHandleSize : UIBehaviour
{
	public float maxSize = 1f;

	public float minSize = 0.1f;

	private ScrollRect scrollRect;

	protected override void Awake()
	{
		base.Awake();
		scrollRect = GetComponent<ScrollRect>();
	}

	protected override void OnEnable()
	{
		scrollRect.onValueChanged.AddListener(onValueChanged);
	}

	protected override void OnDisable()
	{
		scrollRect.onValueChanged.RemoveListener(onValueChanged);
	}

	public void onValueChanged(Vector2 value)
	{
		Scrollbar horizontalScrollbar = scrollRect.horizontalScrollbar;
		if (horizontalScrollbar != null)
		{
			if (horizontalScrollbar.size > maxSize)
			{
				horizontalScrollbar.size = maxSize;
			}
			else if (horizontalScrollbar.size < minSize)
			{
				horizontalScrollbar.size = minSize;
			}
		}
		Scrollbar verticalScrollbar = scrollRect.verticalScrollbar;
		if (verticalScrollbar != null)
		{
			if (verticalScrollbar.size > maxSize)
			{
				verticalScrollbar.size = maxSize;
			}
			else if (verticalScrollbar.size < minSize)
			{
				verticalScrollbar.size = minSize;
			}
		}
	}
}
