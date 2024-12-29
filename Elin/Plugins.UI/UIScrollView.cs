using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIScrollView : ScrollRect
{
	public static float sensitivity;

	public UIItem header;

	public bool enableHeader;

	public bool disableDrag;

	public float sensitivityMulti = 1f;

	private float lastSens;

	protected override void Awake()
	{
		base.Awake();
		base.inertia = true;
		base.decelerationRate = 0.02f;
		Window componentInParent = GetComponentInParent<Window>();
		if ((bool)componentInParent && (bool)componentInParent.rectStats && componentInParent.rectStats.gameObject.activeSelf)
		{
			RectTransform rectTransform = base.verticalScrollbar.Rect();
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y + componentInParent.rectStats.GetComponent<LayoutElement>().minHeight);
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (base.scrollSensitivity != sensitivity * sensitivityMulti)
		{
			base.scrollSensitivity = sensitivity * sensitivityMulti;
		}
	}

	public void Refresh()
	{
		UpdateBounds();
		base.CalculateLayoutInputHorizontal();
		base.CalculateLayoutInputVertical();
		OnRectTransformDimensionsChange();
		base.SetLayoutVertical();
		base.LateUpdate();
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		if (!disableDrag)
		{
			base.OnBeginDrag(eventData);
		}
	}

	public override void OnDrag(PointerEventData eventData)
	{
		if (!disableDrag)
		{
			base.OnDrag(eventData);
		}
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		if (!disableDrag)
		{
			base.OnEndDrag(eventData);
		}
	}
}
