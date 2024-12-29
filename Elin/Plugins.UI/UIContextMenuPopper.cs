using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIContextMenuPopper : UIContextMenuItem, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	[NonSerialized]
	public RectTransform _rect;

	[NonSerialized]
	public bool isPointerOver;

	[NonSerialized]
	public UIContextMenu parent;

	[NonSerialized]
	public string id;

	public UIContextMenu menu;

	public UIContextMenu defaultMenu;

	public Vector2 margin = new Vector2(8f, 10f);

	public float bottomFix;

	private void Awake()
	{
		_rect = base.transform as RectTransform;
	}

	public virtual void OnPointerEnter(PointerEventData data)
	{
		isPointerOver = true;
	}

	public virtual void OnPointerExit(PointerEventData data)
	{
		isPointerOver = false;
	}

	private void Update()
	{
		if (isPointerOver && !menu.gameObject.activeSelf && parent.timeSinceOpen > 0.1f && !Input.GetMouseButton(0))
		{
			Pop();
		}
		if (!isPointerOver && !Input.GetMouseButton(0) && UIContextMenu.Current.hideOnMouseLeave && !menu.isPointerOver)
		{
			Depop();
		}
	}

	public void Pop()
	{
		if (!UIContextMenu.Current.hideOnMouseLeave)
		{
			UIContextMenuPopper[] componentsInChildren = UIContextMenu.Current.GetComponentsInChildren<UIContextMenuPopper>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].Depop();
			}
		}
		if (menu == null)
		{
			CreateMenu();
		}
		menu.Show();
		if (parent != null)
		{
			PositionMenu();
		}
	}

	public void Depop()
	{
		menu.Hide();
	}

	public UIContextMenu CreateMenu()
	{
		menu = UnityEngine.Object.Instantiate(defaultMenu);
		menu.transform.SetParent(base.transform, worldPositionStays: false);
		menu.popper = this;
		menu.depth = parent.depth + 1;
		menu.parent = parent;
		if (parent.system)
		{
			menu.bg.material = parent.matBlur;
		}
		return menu;
	}

	public void PositionMenu()
	{
		RectTransform rect = menu._rect;
		rect.RebuildLayout(recursive: true);
		Vector2 zero = Vector2.zero;
		zero.x = _rect.sizeDelta.x - margin.x;
		zero.y = _rect.sizeDelta.y * -1f - margin.y;
		rect.pivot = new Vector2(0f, 1f);
		rect.anchoredPosition = zero;
		if (parent.transform.position.x + parent._rect.sizeDelta.x * (1f - parent._rect.pivot.x) > (float)Screen.width - rect.sizeDelta.x || menu.alwaysPopLeft)
		{
			zero.x = margin.x;
			rect.pivot = new Vector2(1f, menu._rect.pivot.y);
		}
		zero.x = (int)zero.x;
		zero.y = (int)zero.y;
		rect.anchoredPosition = zero;
		if (rect.position.y - rect.sizeDelta.y * BaseCore.Instance.uiScale < 0f)
		{
			Vector3 position = rect.position;
			position.y = rect.sizeDelta.y * BaseCore.Instance.uiScale;
			rect.position = position;
		}
	}
}
