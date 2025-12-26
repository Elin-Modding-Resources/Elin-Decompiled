using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIList : BaseList
{
	public class RefObject
	{
		public object obj;
	}

	public enum BGType
	{
		none,
		stripe,
		line,
		dot,
		dot_long,
		grid,
		thin
	}

	public enum SortMode
	{
		ByNone,
		ByNumber,
		ByValue,
		ByCategory,
		ByEquip,
		ByJob,
		ByRace,
		ByLevel,
		ByWeight,
		ByPrice,
		ByElementParent,
		ByName,
		ByID,
		ByWeightSingle,
		ByWorkk,
		ByFeat
	}

	public enum ItemHeight
	{
		DontChange,
		Default
	}

	public class Callback<T1, T2> : ICallback where T2 : Component
	{
		public Action<T1, T2> onInstantiate;

		public Action<T1, T2> onClick;

		public Action<T1, T2, int> onRedraw;

		public Action<SortMode> onList;

		public Func<T1, SortMode, int> onSort;

		public Action onRefresh;

		public T2 mold;

		public UIList list;

		public Action<T1, int> onDragReorder;

		public Func<T1, bool> canDragReorder;

		public bool useSort => onSort != null;

		public bool useOnClick => onClick != null;

		public void SetList(UIList _list)
		{
			list = _list;
		}

		public void CreateMold()
		{
			if (!mold)
			{
				mold = list.GetMold<T2>();
			}
		}

		public Component GetMold()
		{
			return mold;
		}

		public Component Instantiate(object obj, Transform parent)
		{
			T2 val = (list.usePool ? PoolManager.Spawn(mold) : UnityEngine.Object.Instantiate(mold));
			val.SetActive(enable: true);
			val.transform.SetParent(parent, worldPositionStays: false);
			if (list.disableInstaClick)
			{
				UIButton uIButton = (val as UIButton) ?? (val as UIItem)?.button1;
				if ((bool)uIButton)
				{
					uIButton.instantClick = false;
				}
			}
			if (onInstantiate != null)
			{
				onInstantiate((T1)obj, val);
			}
			if (onDragReorder != null)
			{
				UIListDragItem orCreate = val.GetOrCreate<UIListDragItem>();
				orCreate.list = list;
				orCreate.item = obj;
			}
			return val;
		}

		public void OnClick(object obj, Component button)
		{
			if (onClick != null)
			{
				onClick((T1)obj, (T2)button);
			}
		}

		public void OnList(SortMode mode)
		{
			if (onList != null)
			{
				onList(mode);
			}
		}

		public void OnRedraw(object obj, Component c, int index)
		{
			if (onRedraw != null)
			{
				onRedraw((T1)obj, (T2)c, index);
			}
		}

		public int OnSort(object obj, SortMode mode)
		{
			return onSort((T1)obj, mode);
		}

		public void OnRefresh()
		{
			if (onRefresh != null)
			{
				onRefresh();
			}
		}

		public void OnDragReorder(object obj, int a)
		{
			if (onDragReorder != null)
			{
				onDragReorder((T1)obj, a);
			}
		}

		public bool CanDragReorder(object obj)
		{
			if (canDragReorder != null)
			{
				return canDragReorder((T1)obj);
			}
			return false;
		}

		public Component GetComponent(Transform t)
		{
			return t.GetComponent<T2>();
		}
	}

	public interface ICallback
	{
		bool useSort { get; }

		bool useOnClick { get; }

		Component GetComponent(Transform t);

		void SetList(UIList list);

		void CreateMold();

		Component Instantiate(object obj, Transform parent);

		Component GetMold();

		void OnClick(object obj, Component button);

		void OnRedraw(object obj, Component button, int index);

		void OnList(SortMode mode);

		int OnSort(object obj, SortMode mode);

		void OnRefresh();

		void OnDragReorder(object obj, int a);

		bool CanDragReorder(object obj);
	}

	public struct ButtonPair
	{
		public Component component;

		public object obj;
	}

	public string langNoItem;

	public int rows;

	public UISelectableGroup menu;

	public UIListTopbar bar;

	public bool fillEmpty;

	public bool buildLayout;

	public UIItem paginationTop;

	public UIItem paginationBottom;

	public UISelectableGroup group;

	public LayoutGroup _layoutItems;

	public Transform moldItem;

	public bool selectFirst;

	public bool invokeSelected;

	public bool enableGroup = true;

	public bool onlyDirectChildrenButtonForGroup;

	public bool usePage;

	public bool usePool;

	public bool disableInstaClick = true;

	public bool ignoreNullObj = true;

	public ItemHeight itemHeight;

	public int heightFix;

	[NonSerialized]
	public int page;

	[NonSerialized]
	public int maxPage;

	[NonSerialized]
	public int itemsPerPage;

	[NonSerialized]
	public List<object> items = new List<object>();

	[NonSerialized]
	public List<ButtonPair> buttons = new List<ButtonPair>();

	[NonSerialized]
	public string idMold;

	public Action SETab = delegate
	{
	};

	public const int HeightDefault = 34;

	private bool initialized;

	private bool reset;

	public UIList moldList;

	public UIList parent;

	public Dictionary<UIButton, UIList> children = new Dictionary<UIButton, UIList>();

	public static string[] strNumber = new string[26]
	{
		"a", "b", "c", "d", "e", "f", "g", "h", "i", "j",
		"k", "l", "m", "n", "o", "p", "q", "r", "s", "t",
		"u", "v", "w", "x", "y", "z"
	};

	private int highlightIndex;

	private object dragTarget;

	private int dragBeginIndex;

	private int dragHoverIndex;

	[NonSerialized]
	public UIScrollView dragScrollView;

	[NonSerialized]
	public RectTransform dragViewport;

	[NonSerialized]
	public float dragEdgeSize;

	[NonSerialized]
	public float dragScrollSpeed = 2.5f;

	public LayoutGroup layoutItems => _layoutItems ?? (_layoutItems = GetComponent<LayoutGroup>());

	public GridLayoutGroup gridLayout => layoutItems as GridLayoutGroup;

	public override int ItemCount => items.Count;

	public override int RowCount => items.Count;

	public UIList Root
	{
		get
		{
			if (!parent)
			{
				return this;
			}
			return parent.Root;
		}
	}

	public bool IsDragging => dragTarget != null;

	public void AddCollection(ICollection collection)
	{
		foreach (object item in collection)
		{
			Add(item);
		}
	}

	public override void Add(object item)
	{
		if (item != null || !ignoreNullObj)
		{
			items.Add(item);
		}
	}

	public int GetIndexOf(object item)
	{
		return items.IndexOf(item);
	}

	public ButtonPair GetPair(object item)
	{
		foreach (ButtonPair button in buttons)
		{
			if (button.obj == item)
			{
				return button;
			}
		}
		return buttons[0];
	}

	public T GetPair<T>(object item) where T : Component
	{
		return GetPair(item).component as T;
	}

	public void AddDynamic(object item)
	{
	}

	public void RemoveDynamic(object item)
	{
		BaseCore.Instance.StopEventSystem(GetPair(item).component, delegate
		{
			items.Remove(item);
			ButtonPair item2 = buttons.First((ButtonPair a) => a.obj == item);
			buttons.IndexOf(item2);
			buttons.Remove(item2);
			UnityEngine.Object.DestroyImmediate(item2.component.gameObject);
			AfterRefresh();
		});
	}

	public override void Clear()
	{
		items.Clear();
		reset = true;
	}

	private UIList CreateChild(UIButton button)
	{
		UIList uIList = Util.Instantiate(moldList, layoutItems);
		uIList.transform.SetSiblingIndex(button.transform.GetSiblingIndex() + 1);
		uIList.callbacks = callbacks;
		uIList.parent = this;
		children.Add(button, uIList);
		return uIList;
	}

	public bool OnClickFolder(UIButton b, Action<UIList> onFold, bool refresh = true)
	{
		if (children.ContainsKey(b))
		{
			RemoveChild(b);
			return true;
		}
		UIList uIList = CreateChild(b);
		onFold(uIList);
		uIList.Refresh();
		if (refresh)
		{
			GetComponentInParent<ScrollRect>().RebuildLayout(recursive: true);
		}
		return false;
	}

	private void RemoveChild(UIButton button)
	{
		children[button].KillChildren();
		children.Remove(button);
		RebuildLayoutInParent();
		GetComponentInParent<ScrollRect>().RebuildLayout(recursive: true);
	}

	private void KillChildren()
	{
		foreach (UIList value in children.Values)
		{
			value.KillChildren();
		}
		UnityEngine.Object.DestroyImmediate(base.gameObject);
	}

	public void RebuildLayoutInParent()
	{
		this.RebuildLayout();
		if ((bool)parent)
		{
			parent.RebuildLayout();
		}
		else
		{
			base.transform.parent.RebuildLayout();
		}
	}

	private void OnSelect(UIList activeList)
	{
		foreach (UIList value in children.Values)
		{
			value.OnSelect(activeList);
		}
		if (this != activeList && (bool)group)
		{
			group.Select(-1);
		}
	}

	public virtual void Refresh(bool highlightLast = false)
	{
		if (!layoutItems)
		{
			return;
		}
		SkinRootStatic tempSkin = SkinManager.tempSkin;
		if (skinType != 0)
		{
			SkinManager.tempSkin = SkinManager.CurrentSkin.GetSkin(skinType);
		}
		highlightIndex = -1;
		if (highlightLast)
		{
			UIButton componentOf = InputModuleEX.GetComponentOf<UIButton>();
			if ((bool)componentOf)
			{
				Component component = componentOf.transform.parent.GetComponent<UIItem>();
				if (component == null)
				{
					component = componentOf;
				}
				for (int i = 0; i < buttons.Count; i++)
				{
					if (buttons[i].component == component)
					{
						highlightIndex = i;
						break;
					}
				}
			}
		}
		callbacks.SetList(this);
		if (!enableGroup && (bool)group)
		{
			UnityEngine.Object.DestroyImmediate(group);
		}
		if (!usePage)
		{
			maxPage = 9999999;
		}
		callbacks.CreateMold();
		if (itemHeight == ItemHeight.Default)
		{
			RectTransform rectTransform = callbacks.GetMold().Rect();
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 34 + heightFix);
		}
		if (!initialized || reset)
		{
			if (sortMode != 0)
			{
				Sort();
			}
			if (!initialized)
			{
				if ((bool)paginationTop && (bool)paginationTop.button1)
				{
					paginationTop.button1.onClick.AddListener(PrevPage);
					paginationTop.button2.onClick.AddListener(NextPage);
				}
				if ((bool)paginationBottom && (bool)paginationBottom.button1)
				{
					paginationBottom.button1.onClick.AddListener(PrevPage);
					paginationBottom.button2.onClick.AddListener(NextPage);
				}
				_ = (bool)bar;
			}
			initialized = true;
			reset = false;
		}
		children.Clear();
		buttons.Clear();
		GridLayoutGroup gridLayoutGroup = layoutItems as GridLayoutGroup;
		BuildLayout();
		if (usePage)
		{
			if ((bool)gridLayoutGroup)
			{
				itemsPerPage = gridLayoutGroup.constraintCount;
			}
			else
			{
				itemsPerPage = rows;
			}
			this.Rect().RebuildLayout();
			this.Rect().ForceUpdateRectTransforms();
			Canvas.ForceUpdateCanvases();
			float y = RectTransformUtility.PixelAdjustRect(this.Rect(), GetComponentInParent<Canvas>()).size.y;
			float y2 = callbacks.GetMold().Rect().sizeDelta.y;
			Debug.Log(y2);
			Debug.Log(y);
			itemsPerPage = (int)(y / y2);
		}
		else
		{
			itemsPerPage = 99999;
		}
		ClearChildren();
		maxPage = (items.Count - 1) / itemsPerPage + 1;
		if (usePage)
		{
			while (page > 0 && page * itemsPerPage >= items.Count)
			{
				page--;
			}
		}
		for (int j = 0; j < itemsPerPage; j++)
		{
			int num = page * itemsPerPage + j;
			if (num >= items.Count)
			{
				if (fillEmpty)
				{
					new GameObject().AddComponent<RectTransform>().SetParent(layoutItems.transform, worldPositionStays: false);
				}
				continue;
			}
			object item = items[num];
			Component comp = callbacks.Instantiate(item, layoutItems.transform);
			callbacks.OnRedraw(item, comp, j);
			ButtonPair buttonPair = default(ButtonPair);
			buttonPair.obj = item;
			buttonPair.component = comp;
			ButtonPair item2 = buttonPair;
			UIButton uIButton = comp as UIButton;
			if (!uIButton)
			{
				UIItem uIItem = comp as UIItem;
				if ((bool)uIItem)
				{
					uIButton = uIItem.button1;
				}
			}
			if ((bool)uIButton)
			{
				if (numbering && (bool)uIButton.keyText)
				{
					uIButton.keyText.text = strNumber[j % strNumber.Length];
				}
				uIButton.onClick.AddListener(delegate
				{
					Root.OnSelect(this);
					callbacks.OnClick(item, comp);
				});
				if (highlightIndex == j)
				{
					uIButton.DoHighlightTransition();
				}
			}
			buttons.Add(item2);
		}
		AfterRefresh();
		if ((bool)group)
		{
			group.Init((!selectFirst) ? (-1) : 0, null, onlyDirectChildrenButtonForGroup);
		}
		if (selectFirst && invokeSelected)
		{
			UIButton componentInChildren = layoutItems.GetComponentInChildren<UIButton>();
			if ((bool)componentInChildren)
			{
				componentInChildren.onClick.Invoke();
			}
		}
		isBuilt = true;
		SkinManager.tempSkin = tempSkin;
	}

	private void AfterRefresh()
	{
		RefreshNoItem();
		if ((bool)paginationTop)
		{
			paginationTop.text1.text = page + 1 + " / " + maxPage;
		}
		if ((bool)paginationBottom)
		{
			paginationBottom.text1.text = page + 1 + " / " + maxPage;
		}
		callbacks.OnRefresh();
		OnRefresh();
		layoutItems.RebuildLayout();
		RefreshBGGrid();
	}

	protected virtual void OnRefresh()
	{
	}

	public void NextPage()
	{
		SetPage(page + 1);
	}

	public void PrevPage()
	{
		SetPage(page - 1);
	}

	public void SetPage(int i)
	{
		if (i < 0)
		{
			i = maxPage - 1;
		}
		else if (i >= maxPage)
		{
			i = 0;
		}
		page = i;
		Refresh();
	}

	public void OnMove(object o, object select = null)
	{
		List();
		Select(select ?? o);
		SE.Click();
	}

	public void Select(int index = 0, bool invoke = false)
	{
		if (buttons.Count < 0)
		{
			return;
		}
		ButtonPair buttonPair = buttons[Mathf.Clamp(index, 0, buttons.Count - 1)];
		UIButton uIButton = buttonPair.component as UIButton;
		if (!uIButton)
		{
			uIButton = (buttonPair.component as UIItem)?.button1;
		}
		if ((bool)uIButton)
		{
			if (invoke)
			{
				uIButton.onClick.Invoke();
			}
			if ((bool)uIButton.group)
			{
				uIButton.group.Select(uIButton);
			}
			else
			{
				uIButton.DoHighlightTransition();
			}
		}
	}

	public void Select(object obj, bool invoke = false)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (obj == items[i])
			{
				Select(i, invoke);
				break;
			}
		}
	}

	public void Select<T>(Func<T, bool> func, bool invoke = false)
	{
		for (int i = 0; i < items.Count; i++)
		{
			if (func((T)items[i]))
			{
				Select(i, invoke);
				break;
			}
		}
	}

	public void Sort()
	{
		if (callbacks.useSort)
		{
			items.Sort((object a, object b) => callbacks.OnSort(b, sortMode) - callbacks.OnSort(a, sortMode));
		}
	}

	public void ChangeSort(SortMode m)
	{
		sortMode = m;
		Sort();
		Refresh();
	}

	public override void NextSort()
	{
		ChangeSort(sorts.NextItem(sortMode));
	}

	public override void List()
	{
		List(sortMode);
	}

	public override void List(SortMode sort)
	{
		List(sort);
	}

	public void List(bool refreshHighlight = false)
	{
		List(sortMode, refreshHighlight);
	}

	public void List(SortMode m, bool refreshHighlight = false)
	{
		sortMode = m;
		Clear();
		callbacks.OnList(sortMode);
		Refresh(refreshHighlight);
	}

	public override void Redraw()
	{
		if (onBeforeRedraw != null)
		{
			onBeforeRedraw();
		}
		int num = 0;
		foreach (ButtonPair button in buttons)
		{
			callbacks.OnRedraw(button.obj, button.component, num);
			num++;
		}
		if (onAfterRedraw != null)
		{
			onAfterRedraw();
		}
	}

	public void BuildLayout()
	{
		GridLayoutGroup gridLayoutGroup = layoutItems as GridLayoutGroup;
		if ((bool)gridLayoutGroup && buildLayout)
		{
			RectTransform rectTransform = layoutItems.Rect();
			RectTransform rectTransform2 = layoutItems.Rect();
			Vector2 vector2 = (layoutItems.Rect().anchorMax = new Vector2(0f, 1f));
			Vector2 pivot = (rectTransform2.anchorMin = vector2);
			rectTransform.pivot = pivot;
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, (float)rows * gridLayoutGroup.cellSize.y);
		}
	}

	public T GetMold<T>() where T : Component
	{
		if ((bool)moldItem)
		{
			T component = moldItem.GetComponent<T>();
			if ((bool)component)
			{
				layoutItems.DestroyChildren();
				return component;
			}
		}
		return layoutItems.CreateMold<T>(idMold);
	}

	public void OnDestroy()
	{
		ClearChildren();
		if ((bool)transNoItem)
		{
			UnityEngine.Object.DestroyImmediate(transNoItem.gameObject);
		}
	}

	public void ClearChildren()
	{
		if (usePool)
		{
			foreach (ButtonPair button in buttons)
			{
				PoolManager.TryDespawn(button.component);
			}
		}
		layoutItems.DestroyChildren();
	}

	public void BeginItemDrag(UIListDragItem drag)
	{
		if (callbacks.CanDragReorder(drag.item))
		{
			dragTarget = drag.item;
			dragBeginIndex = drag.transform.GetSiblingIndex();
		}
	}

	public void EndItemDrag(UIListDragItem drag)
	{
		if (dragTarget != null)
		{
			int num = dragHoverIndex - dragBeginIndex;
			dragTarget = null;
			if (num != 0)
			{
				callbacks.OnDragReorder(drag.item, num);
			}
		}
	}

	public void UpdateItemDragHover(UIListDragItem drag)
	{
		if (callbacks.CanDragReorder(drag.item) && dragTarget != null && dragTarget != drag.item)
		{
			dragHoverIndex = drag.transform.GetSiblingIndex();
			GetPair(dragTarget).component?.transform.SetSiblingIndex(dragHoverIndex);
		}
	}

	private void Update()
	{
		if (IsDragging)
		{
			AutoScrollWhileDragging();
		}
	}

	private void AutoScrollWhileDragging()
	{
		if ((bool)dragViewport && (bool)dragScrollView)
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(dragViewport, Input.mousePosition, null, out var localPoint);
			float y = localPoint.y;
			float num = dragViewport.rect.height * 0.5f;
			float num2 = 0f;
			if (y < 0f - num + dragEdgeSize)
			{
				float num3 = Mathf.InverseLerp(0f - num + dragEdgeSize, 0f - num, y);
				num2 = (0f - dragScrollSpeed) * num3;
			}
			else if (y > num - dragEdgeSize)
			{
				float num4 = Mathf.InverseLerp(num - dragEdgeSize, num, y);
				num2 = dragScrollSpeed * num4;
			}
			if (num2 != 0f)
			{
				float verticalNormalizedPosition = dragScrollView.verticalNormalizedPosition;
				dragScrollView.verticalNormalizedPosition = Mathf.Clamp01(verticalNormalizedPosition + num2 * Time.unscaledDeltaTime);
			}
		}
	}
}
