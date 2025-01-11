using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Mosframe;

[RequireComponent(typeof(ScrollRect))]
public abstract class DynamicScrollView : UIBehaviour
{
	public enum Direction
	{
		Vertical,
		Horizontal
	}

	public int totalItemCount;

	public RectTransform itemPrototype;

	public UIDynamicList dynamicList;

	protected Direction direction;

	public LinkedList<DSVRow> containers = new LinkedList<DSVRow>();

	protected float prevAnchoredPosition;

	protected int nextInsertItemNo;

	[NonSerialized]
	public int prevTotalItemCount = -1;

	protected ScrollRect scrollRect;

	protected RectTransform viewportRect;

	public RectTransform contentRect;

	public abstract float contentAnchoredPosition { get; set; }

	public abstract float contentSize { get; }

	public abstract float viewportSize { get; }

	protected abstract float itemSize { get; }

	public void scrollToLastPos()
	{
		contentAnchoredPosition = viewportSize - contentSize;
		refresh();
	}

	public void scrollByItemIndex(int itemIndex)
	{
		float num = contentSize / (float)totalItemCount * (float)itemIndex;
		contentAnchoredPosition = 0f - num;
		Update();
		refresh();
	}

	public void refresh()
	{
		int num = 0;
		if (contentAnchoredPosition != 0f)
		{
			num = (int)((0f - contentAnchoredPosition) / itemSize);
		}
		foreach (DSVRow container in containers)
		{
			if (!(container == null) && !(container.Rect() == null))
			{
				float num2 = itemSize * (float)num;
				container.Rect().anchoredPosition = ((direction == Direction.Vertical) ? new Vector2(0f, 0f - num2) : new Vector2(num2, 0f));
				updateItem(num, container);
				num++;
			}
		}
		nextInsertItemNo = num - containers.Count;
		prevAnchoredPosition = (float)(int)(contentAnchoredPosition / itemSize) * itemSize;
	}

	protected override void Awake()
	{
		base.Awake();
		scrollRect = GetComponent<ScrollRect>();
		viewportRect = scrollRect.viewport;
		contentRect = scrollRect.content;
		contentRect.anchoredPosition = Vector2.zero;
		prevTotalItemCount = totalItemCount;
	}

	public void Build()
	{
		itemPrototype.gameObject.SetActive(value: false);
		int num = (int)(viewportSize / itemSize) + 3;
		for (int i = containers.Count; i < num; i++)
		{
			RectTransform rectTransform = UnityEngine.Object.Instantiate(itemPrototype);
			rectTransform.SetParent(contentRect, worldPositionStays: false);
			rectTransform.name = i.ToString();
			rectTransform.anchoredPosition = ((direction == Direction.Vertical) ? new Vector2(0f, (0f - itemSize) * (float)i) : new Vector2(itemSize * (float)i, 0f));
			DSVRow component = rectTransform.GetComponent<DSVRow>();
			containers.AddLast(component);
			rectTransform.gameObject.SetActive(value: true);
			updateItem(i, component);
		}
		resizeContent();
	}

	public void Update()
	{
		Build();
		if (totalItemCount != prevTotalItemCount)
		{
			prevTotalItemCount = totalItemCount;
			bool flag = false;
			if (viewportSize - contentAnchoredPosition >= contentSize - itemSize * 0.5f && contentAnchoredPosition != 0f)
			{
				flag = true;
			}
			resizeContent();
			if (flag)
			{
				contentAnchoredPosition = viewportSize - contentSize;
			}
			refresh();
		}
		while (contentAnchoredPosition - prevAnchoredPosition < (0f - itemSize) * 2f)
		{
			prevAnchoredPosition -= itemSize;
			LinkedListNode<DSVRow> first = containers.First;
			if (first == null)
			{
				break;
			}
			DSVRow value = first.Value;
			containers.RemoveFirst();
			containers.AddLast(value);
			float num = itemSize * (float)(containers.Count + nextInsertItemNo);
			value.Rect().anchoredPosition = ((direction == Direction.Vertical) ? new Vector2(0f, 0f - num) : new Vector2(num, 0f));
			updateItem(containers.Count + nextInsertItemNo, value);
			nextInsertItemNo++;
		}
		while (contentAnchoredPosition - prevAnchoredPosition > 0f)
		{
			prevAnchoredPosition += itemSize;
			LinkedListNode<DSVRow> last = containers.Last;
			if (last != null)
			{
				DSVRow value2 = last.Value;
				containers.RemoveLast();
				containers.AddFirst(value2);
				nextInsertItemNo--;
				float num2 = itemSize * (float)nextInsertItemNo;
				value2.Rect().anchoredPosition = ((direction == Direction.Vertical) ? new Vector2(0f, 0f - num2) : new Vector2(num2, 0f));
				updateItem(nextInsertItemNo, value2);
				continue;
			}
			break;
		}
	}

	public void OnResize()
	{
		Build();
		base.transform.parent.parent.parent.RebuildLayout();
		refresh();
		Update();
		GetComponent<UIScrollView>().Refresh();
	}

	public void resizeContent()
	{
		Vector2 size = contentRect.getSize();
		if (direction == Direction.Vertical)
		{
			size.y = itemSize * (float)totalItemCount;
		}
		else
		{
			size.x = itemSize * (float)totalItemCount;
		}
		contentRect.setSize(size);
	}

	private void updateItem(int index, DSVRow itemObj)
	{
		if (index < 0 || index >= totalItemCount)
		{
			itemObj.SetActive(enable: false);
			return;
		}
		itemObj.SetActive(enable: true);
		dynamicList.UpdateRow(itemObj, index);
	}

	[ContextMenu("Initialize")]
	public virtual void init()
	{
		clear();
		GetComponent<RectTransform>().setFullSize();
		ScrollRect component = GetComponent<ScrollRect>();
		component.horizontal = direction == Direction.Horizontal;
		component.vertical = direction == Direction.Vertical;
		component.scrollSensitivity = 15f;
		RectTransform component2 = new GameObject("Viewport", typeof(RectTransform), typeof(Mask), typeof(Image)).GetComponent<RectTransform>();
		component2.SetParent(component.transform, worldPositionStays: false);
		component2.setFullSize();
		component2.offsetMin = new Vector2(10f, 10f);
		component2.offsetMax = new Vector2(-10f, -10f);
		component2.GetComponent<Image>().type = Image.Type.Sliced;
		component2.GetComponent<Mask>().showMaskGraphic = false;
		component.viewport = component2;
		RectTransform component3 = new GameObject("Content", typeof(RectTransform)).GetComponent<RectTransform>();
		component3.SetParent(component2, worldPositionStays: false);
		if (direction == Direction.Horizontal)
		{
			component3.setSizeFromLeft(1f);
		}
		else
		{
			component3.setSizeFromTop(1f);
		}
		Vector2 size = component3.getSize();
		component3.setSize(size - size * 0.06f);
		component.content = component3;
		RectTransform component4 = new GameObject((direction == Direction.Horizontal) ? "Scrollbar Horizontal" : "Scrollbar Vertical", typeof(Scrollbar), typeof(Image)).GetComponent<RectTransform>();
		component4.SetParent(component2, worldPositionStays: false);
		if (direction == Direction.Horizontal)
		{
			component4.setSizeFromBottom(0.05f);
		}
		else
		{
			component4.setSizeFromRight(0.05f);
		}
		component4.SetParent(component.transform, worldPositionStays: true);
		Scrollbar component5 = component4.GetComponent<Scrollbar>();
		component5.direction = ((direction != Direction.Horizontal) ? Scrollbar.Direction.BottomToTop : Scrollbar.Direction.LeftToRight);
		if (direction == Direction.Horizontal)
		{
			component.horizontalScrollbar = component5;
		}
		else
		{
			component.verticalScrollbar = component5;
		}
		Image component6 = component4.GetComponent<Image>();
		component6.color = new Color(0.1f, 0.1f, 0.1f, 0.5f);
		component6.type = Image.Type.Sliced;
		RectTransform component7 = new GameObject("Sliding Area", typeof(RectTransform)).GetComponent<RectTransform>();
		component7.SetParent(component4, worldPositionStays: false);
		component7.setFullSize();
		RectTransform component8 = new GameObject("Handle", typeof(Image)).GetComponent<RectTransform>();
		component8.SetParent(component7, worldPositionStays: false);
		component8.setFullSize();
		Image component9 = component8.GetComponent<Image>();
		component9.color = new Color(0.5f, 0.5f, 1f, 0.5f);
		component9.type = Image.Type.Sliced;
		component5.handleRect = component8;
		if (component.GetComponent<ScrollbarHandleSize>() == null)
		{
			ScrollbarHandleSize scrollbarHandleSize = component.gameObject.AddComponent<ScrollbarHandleSize>();
			scrollbarHandleSize.maxSize = 1f;
			scrollbarHandleSize.minSize = 0.1f;
		}
		base.gameObject.setLayer(base.transform.parent.gameObject.layer);
	}

	protected virtual void clear()
	{
		while (base.transform.childCount > 0)
		{
			UnityEngine.Object.DestroyImmediate(base.transform.GetChild(0).gameObject);
		}
	}
}
