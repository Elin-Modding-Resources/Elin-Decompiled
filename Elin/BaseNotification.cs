using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseNotification : EClass
{
	public ItemNotice item;

	public string text;

	public string lastText;

	public virtual Sprite Sprite => null;

	public virtual int idSprite => -1;

	public virtual bool Visible => true;

	public virtual bool Interactable => true;

	public virtual Action<UITooltip> onShowTooltip => null;

	public virtual ItemNotice GetMold()
	{
		return null;
	}

	public virtual LayoutGroup GetLayoutGroup()
	{
		return null;
	}

	public virtual int GetSortVal()
	{
		return 0;
	}

	public void Refresh()
	{
		OnRefresh();
		if (text != lastText)
		{
			item.button.mainText.text = text;
			lastText = text;
			if (item.gameObject.activeInHierarchy)
			{
				item.button.RebuildLayout(recursive: true);
			}
		}
	}

	public virtual void OnClick()
	{
		EClass.ui.AddLayer<LayerJournal>();
	}

	public virtual void OnRefresh()
	{
	}

	public virtual bool ShouldRemove()
	{
		return false;
	}

	public virtual void OnInstantiate()
	{
	}
}
