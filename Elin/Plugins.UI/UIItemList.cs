using System;

public class UIItemList : UIListEx<UIItem>
{
	public override void OnInstantiate(RefObjectEx o, UIItem t)
	{
		if ((bool)t.text1)
		{
			t.text1.text = o.text;
		}
	}

	public void AddItem(object obj, string text, Action<UIItem> onInstantiate = null)
	{
		Add(new RefObjectEx
		{
			obj = obj,
			text = text,
			onInstantiate = onInstantiate
		});
	}
}
