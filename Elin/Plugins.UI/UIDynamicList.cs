using System;
using System.Collections.Generic;
using Mosframe;
using UnityEngine;
using UnityEngine.UI;

public class UIDynamicList : BaseList
{
	public class Row
	{
		public bool isHeader;

		public List<object> objects = new List<object>();

		public Action<UIItem> onSetHeader;
	}

	public DynamicScrollView dsv;

	public Transform moldItem;

	public Transform moldHeader;

	public int columns = 1;

	public bool autoSize = true;

	public bool captureScreen = true;

	public bool refreshHighlight = true;

	public bool rebuildLayout = true;

	public bool useGrid;

	public bool useHighlight;

	public List<Row> rows = new List<Row>();

	public List<object> objects = new List<object>();

	public object selectedObject;

	public float minHeight;

	[NonSerialized]
	public bool first = true;

	private bool autoSized;

	public override int ItemCount => objects.Count;

	public override int RowCount => rows.Count;

	public Row NewRow()
	{
		Row row = new Row();
		rows.Add(row);
		return row;
	}

	public override void Clear()
	{
		rows.Clear();
		objects.Clear();
	}

	public override void Add(object o)
	{
		if (o == null)
		{
			Debug.Log("Tried to add null object");
		}
		else
		{
			objects.Add(o);
		}
	}

	private void _Add(object o)
	{
		if (autoSize && !autoSized)
		{
			autoSized = true;
			this.RebuildLayoutTo<Layer>();
			GridLayoutGroup component = dsv.itemPrototype.GetComponent<GridLayoutGroup>();
			columns = (int)(this.Rect().rect.width / (component.spacing.x + component.cellSize.x));
			if (columns < 1)
			{
				columns = 1;
			}
		}
		if (rows.Count == 0)
		{
			NewRow();
		}
		Row row = rows.LastItem();
		if (row.objects.Count >= columns)
		{
			row = NewRow();
		}
		row.objects.Add(o);
	}

	public void RemoveObj(object o)
	{
	}

	public void AddHeader(Action<UIItem> onSetHeader)
	{
		Row row = NewRow();
		row.isHeader = true;
		row.onSetHeader = onSetHeader;
		NewRow();
	}

	public void Refresh()
	{
		dsv.prevTotalItemCount = -1;
		dsv.totalItemCount = rows.Count;
		this.RebuildLayoutTo<Layer>();
		if (!isBuilt)
		{
			dsv.Build();
			isBuilt = true;
		}
		else
		{
			dsv.Update();
		}
		RefreshNoItem();
		RefreshBGGrid();
	}

	public int GetIndex(object o)
	{
		for (int i = 0; i < rows.Count; i++)
		{
			for (int j = 0; j < rows[i].objects.Count; j++)
			{
				if (rows[i].objects[j] == o)
				{
					return i;
				}
			}
		}
		return -1;
	}

	public void UpdateRow(DSVRow dsvRow, int index)
	{
		Row row = rows[index];
		for (int i = 0; i < columns; i++)
		{
			if (dsvRow.items.Count <= i)
			{
				dsvRow.items.Add(new DSVRow.Item
				{
					comp = callbacks.GetComponent(Util.Instantiate(moldItem, dsvRow.transform).transform)
				});
			}
			Component comp = dsvRow.items[i].comp;
			bool flag = !row.isHeader && row.objects.Count > i;
			comp.SetActive(flag);
			dsvRow.items[i].obj = null;
			if (!flag)
			{
				continue;
			}
			object obj = row.objects[i];
			dsvRow.items[i].obj = obj;
			callbacks.OnRedraw(obj, comp, columns * index + i);
			UIButton uIButton = comp as UIButton;
			if (!uIButton)
			{
				uIButton = (comp as UIItem)?.button1;
			}
			if ((bool)uIButton && callbacks.useOnClick)
			{
				uIButton.onClick.RemoveAllListeners();
				uIButton.onClick.AddListener(delegate
				{
					callbacks.OnClick(obj, comp);
				});
			}
			if (useHighlight)
			{
				uIButton.selected = obj == selectedObject;
				if (uIButton.selected)
				{
					uIButton.DoHighlightTransition(instant: true);
				}
				else
				{
					uIButton.DoNormalTransition();
				}
			}
		}
		if (dsvRow.items.Count > columns)
		{
			for (int j = columns; j < dsvRow.items.Count; j++)
			{
				dsvRow.items[j].comp.SetActive(enable: false);
				dsvRow.items[j].obj = null;
			}
		}
		if ((bool)dsvRow.itemHeader)
		{
			if (row.isHeader)
			{
				dsvRow.itemHeader.SetActive(enable: true);
				row.onSetHeader(dsvRow.itemHeader);
			}
			else
			{
				dsvRow.itemHeader.SetActive(enable: false);
			}
		}
		if ((bool)dsvRow.bgGrid)
		{
			dsvRow.bgGrid.SetActive(useGrid);
		}
		if (useGrid)
		{
			dsvRow.bgGrid.Rect().sizeDelta = new Vector2((float)row.objects.Count * dsvRow.GetComponent<GridLayoutGroup>().cellSize.x, 0f);
			dsvRow.bgGrid.uvRect = new Rect(0f, 0f, row.objects.Count, 1f);
		}
	}

	public override void Redraw()
	{
		List();
	}

	public override void List()
	{
		List(sortMode);
	}

	public override void List(UIList.SortMode m)
	{
		if (!first && captureScreen)
		{
			BaseCore.Instance.FreezeScreen();
		}
		first = false;
		sortMode = m;
		Clear();
		callbacks.OnList(sortMode);
		foreach (object @object in objects)
		{
			_Add(@object);
		}
		Layer root = base.transform.GetComponentInParent<Layer>();
		root = root.parent?.GetComponent<Layer>() ?? root;
		if (objects.Count != 0)
		{
			if (refreshHighlight)
			{
				UIButton.TryHihlight();
				UIButton.TryShowTip(root.transform);
				BaseCore.Instance.WaitForEndOfFrame(delegate
				{
					UIButton.TryHihlight();
					UIButton.TryShowTip(root.transform);
					if (captureScreen)
					{
						BaseCore.Instance.WaitForEndOfFrame(delegate
						{
							BaseCore.Instance.UnfreezeScreen();
						});
					}
				});
			}
		}
		else
		{
			TooltipManager.Instance.HideTooltips();
		}
		Refresh();
	}

	public void OnResizeWindow()
	{
		if (autoSize)
		{
			this.RebuildLayoutTo<Layer>();
			GridLayoutGroup component = dsv.itemPrototype.GetComponent<GridLayoutGroup>();
			columns = (int)(this.Rect().rect.width / (component.spacing.x + component.cellSize.x));
			if (columns < 1)
			{
				columns = 1;
			}
			List();
			dsv.OnResize();
		}
	}

	public void Select<T>(Func<T, bool> func, bool invoke = false)
	{
		foreach (DSVRow container in dsv.containers)
		{
			foreach (DSVRow.Item item in container.items)
			{
				UIButton uIButton = item.comp as UIButton;
				if (!(uIButton == null) && uIButton.gameObject.activeInHierarchy)
				{
					T arg = (T)item.obj;
					if (func(arg))
					{
						Select(item.obj, invoke);
						return;
					}
				}
			}
		}
	}

	public bool Select(object o, bool invoke = false)
	{
		selectedObject = o;
		return RefreshHighlight(invoke);
	}

	public bool RefreshHighlight(bool invoke = false)
	{
		bool result = false;
		foreach (DSVRow container in dsv.containers)
		{
			foreach (DSVRow.Item item in container.items)
			{
				UIButton b = item.comp as UIButton;
				if (b == null || !b.gameObject.activeInHierarchy)
				{
					continue;
				}
				b.selected = item.obj == selectedObject;
				if (b.selected)
				{
					BaseCore.Instance.actionsNextFrame.Add(delegate
					{
						if (b != null)
						{
							b.DoNormalTransition();
							b.DoHighlightTransition(instant: true);
						}
					});
					result = true;
					b.DoHighlightTransition(instant: true);
					if (invoke)
					{
						callbacks.OnClick(item.obj, item.comp);
					}
				}
				else
				{
					b.DoNormalTransition();
				}
			}
		}
		return result;
	}

	public void Scroll(int index = 0)
	{
		if (index <= 0)
		{
			dsv.contentAnchoredPosition = 0f;
		}
		else
		{
			dsv.scrollByItemIndex(index);
		}
		dsv.refresh();
	}

	public void Scroll(object o)
	{
		int index = objects.IndexOf(o);
		Scroll(index);
	}

	public T GetComp<T>(object o) where T : Component
	{
		foreach (DSVRow container in dsv.containers)
		{
			foreach (DSVRow.Item item in container.items)
			{
				if (item.obj == o)
				{
					return item.comp as T;
				}
			}
		}
		return null;
	}
}
