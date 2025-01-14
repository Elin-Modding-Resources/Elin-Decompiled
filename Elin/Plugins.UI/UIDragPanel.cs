using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragPanel : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDragHandler, IChangeResolution, IInitializePotentialDragHandler
{
	public static bool dragging;

	private Vector2 originalLocalPointerPosition;

	private Vector3 originalPanelLocalPosition;

	public RectTransform target;

	public RectTransform bound;

	public RectTransform container;

	public bool axisY = true;

	public bool axisX = true;

	public bool enable = true;

	public bool autoAnchor;

	public bool clamp = true;

	public Action onDrag;

	private void Awake()
	{
		if (target == null)
		{
			target = base.transform.parent as RectTransform;
		}
		if (bound == null)
		{
			bound = target;
		}
	}

	public void SetTarget(RectTransform r)
	{
		target = (bound = r);
		container = target.parent as RectTransform;
	}

	public void OnChangeResolution()
	{
	}

	public void OnPointerDown(PointerEventData data)
	{
		if (enable && data.button == PointerEventData.InputButton.Left)
		{
			if (container == null)
			{
				container = target.parent as RectTransform;
			}
			originalPanelLocalPosition = target.localPosition;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(container, data.position, data.pressEventCamera, out originalLocalPointerPosition);
			dragging = true;
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		if (enable && data.button == PointerEventData.InputButton.Left)
		{
			if (axisY && axisX && autoAnchor)
			{
				target.SetAnchor();
			}
			dragging = false;
		}
	}

	public void OnInitializePotentialDrag(PointerEventData ped)
	{
		ped.useDragThreshold = false;
	}

	public void OnDrag(PointerEventData data)
	{
		if (enable && data.button == PointerEventData.InputButton.Left && !(target == null) && !(container == null))
		{
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle(container, data.position, data.pressEventCamera, out var localPoint))
			{
				Vector3 vector = localPoint - originalLocalPointerPosition;
				target.localPosition = originalPanelLocalPosition + vector;
			}
			if (clamp)
			{
				ClampToWindow();
			}
			if (onDrag != null)
			{
				onDrag();
			}
		}
	}

	private void ClampToWindow()
	{
		Vector3 localPosition = target.localPosition;
		Vector3 vector = container.rect.min - bound.rect.min;
		Vector3 vector2 = container.rect.max - bound.rect.max;
		if (axisX)
		{
			localPosition.x = (int)Mathf.Clamp(localPosition.x, vector.x - 20f, vector2.x + 20f);
		}
		if (axisY)
		{
			localPosition.y = (int)Mathf.Clamp(localPosition.y, vector.y - 20f, vector2.y + 20f);
		}
		target.localPosition = localPosition;
	}
}
