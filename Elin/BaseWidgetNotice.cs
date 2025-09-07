using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseWidgetNotice : Widget
{
	public Sprite[] sprites;

	public List<BaseNotification> list = new List<BaseNotification>();

	public VerticalLayoutGroup layout;

	public VerticalLayoutGroup layout2;

	public ItemNotice mold;

	private bool activating = true;

	protected bool dirty;

	public sealed override void OnActivate()
	{
		if (!mold)
		{
			mold = layout.CreateMold<ItemNotice>();
		}
		_OnActivate();
		RefreshLayout();
		layout.RebuildLayout(recursive: true);
		if ((bool)layout2)
		{
			layout2.RebuildLayout(recursive: true);
		}
		activating = false;
		_RefreshAll();
	}

	public virtual void _OnActivate()
	{
	}

	private void OnEnable()
	{
		InvokeRepeating("_RefreshAll", 0.1f, 0.2f);
	}

	private void OnDisable()
	{
		CancelInvoke();
	}

	public void _RefreshAll()
	{
		OnRefresh();
		foreach (BaseNotification i in list)
		{
			i.Refresh();
			i.item.SetActive(i.Visible, delegate(bool enabled)
			{
				if (enabled)
				{
					i.item.button.RebuildLayout(recursive: true);
				}
				dirty = true;
			});
		}
		Sort();
		if (dirty)
		{
			layout.RebuildLayout();
			if ((bool)layout2)
			{
				layout2.RebuildLayout(recursive: true);
			}
			dirty = false;
		}
	}

	public void Sort()
	{
		list.Sort((BaseNotification a, BaseNotification b) => b.GetSortVal() - a.GetSortVal());
		for (int i = 0; i < list.Count; i++)
		{
			list[i].item.transform.SetSiblingIndex(4 + i);
		}
	}

	public virtual void OnRefresh()
	{
	}

	public ItemNotice Add(BaseNotification n, Transform parent = null)
	{
		list.Add(n);
		LayoutGroup layoutGroup = n.GetLayoutGroup() ?? layout;
		ItemNotice itemNotice = Util.Instantiate(n.GetMold() ?? mold, parent ?? layoutGroup.transform);
		itemNotice.button.onClick.AddListener(delegate
		{
			if (EMono.scene.actionMode == ActionMode.NoMap)
			{
				BaseCore.Instance.WaitForEndOfFrame(n.OnClick);
			}
			else
			{
				n.OnClick();
			}
		});
		Sprite sprite = n.Sprite;
		if (n.idSprite != -1)
		{
			sprite = sprites[n.idSprite];
		}
		if ((bool)sprite)
		{
			itemNotice.button.icon.sprite = sprite;
		}
		itemNotice.button.image.raycastTarget = n.Interactable;
		itemNotice.button.tooltip.enable = n.onShowTooltip != null;
		itemNotice.button.tooltip.onShowTooltip = n.onShowTooltip;
		n.item = itemNotice;
		n.Refresh();
		n.OnInstantiate();
		if (!activating)
		{
			layoutGroup.RebuildLayout(recursive: true);
			Sort();
		}
		return itemNotice;
	}

	public void Remove(BaseNotification n)
	{
		list.Remove(n);
		if ((bool)n.item)
		{
			Object.DestroyImmediate(n.item.gameObject);
		}
	}

	public override void OnFlip()
	{
		layout.childAlignment = (flip ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft);
		this.Rect().pivot = new Vector2(flip ? 1 : 0, this.Rect().pivot.y);
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddChild("style").AddSlider("toggleButtonBG", (float a) => a.ToString() ?? "", base.config.skin.button, delegate(float a)
		{
			base.config.skin.button = (int)a;
			ApplySkin();
		}, 0f, base.config.skin.Skin.buttons.Count - 1, isInt: true);
		SetBaseContextMenu(m);
	}

	public void RefreshLayout()
	{
		SkinAsset_Button button = GetComponent<SkinRoot>().Config.Button;
		int y = button.size.y;
		foreach (BaseNotification item in list)
		{
			RectTransform rectTransform = item.item.Rect();
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, y);
		}
		layout.spacing = button.spacing.y;
	}

	public override void OnApplySkin()
	{
		RefreshLayout();
	}
}
