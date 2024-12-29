using System;
using UnityEngine;

public class UIAnchor : MonoBehaviour
{
	public Action<RectPosition> onSetPivot;

	public Action<RectPosition> onSetAnchor;

	public void OnClickAnchor(int i)
	{
		OnClickAnchor(i.ToEnum<RectPosition>());
	}

	public void OnClickAnchor(RectPosition p)
	{
		onSetAnchor(p);
	}

	public void OnClickPivot(int i)
	{
		OnClickAnchor(i.ToEnum<RectPosition>());
	}

	public void OnClickPivot(RectPosition p)
	{
		onSetPivot(p);
	}
}
