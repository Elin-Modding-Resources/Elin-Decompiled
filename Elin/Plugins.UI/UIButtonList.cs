using System;
using UnityEngine;

public class UIButtonList : UIListEx<UIButton>
{
	public override void OnInstantiate(RefObjectEx o, UIButton t)
	{
		t.mainText.text = o.text;
	}

	public void AddButton(object obj, string text, Action onClick, Action<UIButton> onInstantiate = null)
	{
		Add(new RefObjectEx
		{
			obj = obj,
			text = text,
			onClick = onClick,
			onInstantiate = onInstantiate
		});
	}

	public new void Select(object obj, bool invoke = false)
	{
		for (int i = 0; i < items.Count; i++)
		{
			RefObject refObject = (RefObject)items[i];
			if (obj.Equals(refObject.obj))
			{
				base.Select(i, invoke);
				return;
			}
		}
		Debug.Log(obj?.ToString() + obj.GetType());
	}
}
