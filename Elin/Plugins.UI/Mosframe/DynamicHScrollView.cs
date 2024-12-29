using UnityEngine;

namespace Mosframe;

[AddComponentMenu("UI/Dynamic H Scroll View")]
public class DynamicHScrollView : DynamicScrollView
{
	public override float contentAnchoredPosition
	{
		get
		{
			return contentRect.anchoredPosition.x;
		}
		set
		{
			contentRect.anchoredPosition = new Vector2(value, contentRect.anchoredPosition.y);
		}
	}

	public override float contentSize => contentRect.rect.width;

	public override float viewportSize => viewportRect.rect.width;

	protected override float itemSize => itemPrototype.rect.width;

	public override void init()
	{
		direction = Direction.Horizontal;
		base.init();
	}

	protected override void Awake()
	{
		base.Awake();
		direction = Direction.Horizontal;
	}

	protected override void Start()
	{
		base.Start();
	}
}
