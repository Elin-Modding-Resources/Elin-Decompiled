using System;
using UnityEngine;
using UnityEngine.UI;

public class BaseList : MonoBehaviour
{
	public UIList.BGType bgType;

	public Vector2 bgFix;

	public Vector2 bgUvFix = Vector2.one;

	public UIList.ICallback callbacks;

	public UIList.SortMode[] sorts;

	public Action onBeforeRedraw;

	public Action onAfterRedraw;

	public bool useDefaultNoItem = true;

	public bool numbering;

	protected Transform transNoItem;

	public SkinType skinType;

	[NonSerialized]
	public RawImage bgGrid;

	[NonSerialized]
	public UIList.SortMode sortMode;

	[NonSerialized]
	public bool isBuilt;

	public virtual int ItemCount => 0;

	public virtual int RowCount => 0;

	public SkinRootStatic Skin => SkinManager.CurrentSkin;

	public virtual void NextSort()
	{
		List(sorts.NextItem(sortMode));
	}

	public virtual void List()
	{
	}

	public virtual void List(UIList.SortMode sort)
	{
	}

	public virtual void Redraw()
	{
	}

	public virtual void Clear()
	{
	}

	public virtual void Add(object o)
	{
	}

	public void RefreshNoItem()
	{
		if (!transNoItem)
		{
			if (useDefaultNoItem)
			{
				UIScrollView[] componentsInParent = base.transform.GetComponentsInParent<UIScrollView>(includeInactive: true);
				transNoItem = Util.Instantiate(ResourceCache.Load<Transform>("NoItem"), (componentsInParent.Length != 0) ? componentsInParent[0].transform : base.transform.parent);
			}
			else
			{
				transNoItem = base.transform.Find("NoItem");
			}
		}
		if ((bool)transNoItem)
		{
			transNoItem.SetActive(ItemCount == 0);
		}
	}

	public void RefreshBGGrid()
	{
		if (bgType == UIList.BGType.none)
		{
			return;
		}
		int num = RowCount;
		LayoutGroup component = base.transform.GetComponent<LayoutGroup>();
		GridLayoutGroup gridLayoutGroup = component as GridLayoutGroup;
		bool flag = bgType == UIList.BGType.stripe;
		if (!bgGrid)
		{
			bgGrid = Util.Instantiate<RawImage>("UI/Element/List/BGList " + bgType, base.transform);
		}
		bgGrid.transform.SetAsFirstSibling();
		if ((bool)gridLayoutGroup && gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
		{
			num = Mathf.CeilToInt((float)num / (float)gridLayoutGroup.constraintCount);
		}
		float height = num;
		if (flag)
		{
			height = (float)num * 0.5f * (float)((num % 2 != 0) ? 1 : (-1));
		}
		float num2 = component?.padding.top ?? 0;
		float num3 = (flag ? 0f : 1f) + bgFix.y;
		float num4 = 0f;
		num4 = (component as VerticalLayoutGroup)?.spacing ?? gridLayoutGroup?.spacing.y ?? 0f;
		if (bgType == UIList.BGType.grid)
		{
			bgGrid.Rect().sizeDelta = new Vector2(0f, 0f);
			bgGrid.Rect().anchoredPosition = new Vector2(0f, 0f);
		}
		else
		{
			bgGrid.Rect().sizeDelta = new Vector2(-8f, 0f - num2 + num4);
			bgGrid.Rect().anchoredPosition = new Vector2(0f, (0f - num2) * 0.5f - num3);
		}
		float width = 1f;
		float num5 = base.transform.Rect().rect.width;
		if ((bool)gridLayoutGroup && gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
		{
			num5 = (float)gridLayoutGroup.constraintCount * (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x);
		}
		if (!flag)
		{
			int num6 = 64;
			if (bgType == UIList.BGType.dot)
			{
				num6 = 60;
			}
			width = num5 / (float)num6 * bgUvFix.x;
		}
		if (bgType == UIList.BGType.grid && gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
		{
			width = gridLayoutGroup.constraintCount;
		}
		bgGrid.uvRect = new Rect(0f, 0f, width, height);
		bgGrid.SetActive(ItemCount > 0);
	}
}
