using System;
using UnityEngine;

[Serializable]
public class TooltipData
{
	public bool enable;

	public bool icon;

	public Vector3 offset;

	public string id;

	public string lang;

	public string text;

	public Action<UITooltip> onShowTooltip;
}
