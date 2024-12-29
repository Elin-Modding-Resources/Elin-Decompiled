using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HintIcon : MonoBehaviour
{
	public TooltipData tooltip;

	public UIText text;

	public UIText text2;

	public EventTrigger trigger;

	public Action onPointerDown;

	public void ShowTooltip()
	{
		if (tooltip.enable)
		{
			TooltipManager.Instance.ShowTooltip(tooltip, base.transform);
		}
	}

	public void OnPointerDown(BaseEventData e)
	{
		if (onPointerDown != null && Input.GetMouseButton(0))
		{
			onPointerDown();
		}
	}
}
