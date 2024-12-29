using UnityEngine;

namespace Mosframe;

[AddComponentMenu("UI/Dynamic V Scroll View")]
public class DynamicVScrollView : DynamicScrollView
{
	public override float contentAnchoredPosition
	{
		get
		{
			return 0f - contentRect.anchoredPosition.y;
		}
		set
		{
			contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, 0f - value);
		}
	}

	public override float contentSize => contentRect.rect.height;

	public override float viewportSize => viewportRect.rect.height;

	protected override float itemSize => itemPrototype.rect.height;

	public override void init()
	{
		direction = Direction.Vertical;
		base.init();
	}

	protected override void Awake()
	{
		base.Awake();
		direction = Direction.Vertical;
	}

	protected override void Start()
	{
		base.Start();
	}
}
