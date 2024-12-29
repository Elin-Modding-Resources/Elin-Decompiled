using System;
using UnityEngine;
using UnityEngine.UI;

public class MsgBlock : MonoBehaviour
{
	public static MsgBlock lastBlock;

	public static string lastText;

	public int countElements;

	public Image bg;

	public LayoutGroup layout;

	public CanvasGroup cg;

	private RectTransform rect;

	[NonSerialized]
	public MsgBox box;

	[NonSerialized]
	public UIText txt;

	[NonSerialized]
	public int repeat;

	private void Awake()
	{
		rect = this.Rect();
	}

	public void Append(string s, Color col)
	{
		txt = PoolManager.Spawn(box.prefabs.text, layout);
		txt.SetText(s, col);
		txt.RebuildLayout();
		AddElement(txt);
		lastBlock = this;
		lastText = s;
		repeat = 0;
	}

	public void Append(Sprite s, bool fitLine = false)
	{
		Transform transform = PoolManager.Spawn(box.prefabs.image, layout);
		Image componentInChildren = transform.GetComponentInChildren<Image>();
		componentInChildren.sprite = s;
		if (fitLine)
		{
			componentInChildren.Rect().sizeDelta = new Vector2(rect.sizeDelta.y, rect.sizeDelta.y);
		}
		else
		{
			componentInChildren.SetNativeSize();
		}
		transform.transform.Rect().sizeDelta = componentInChildren.Rect().sizeDelta;
		AddElement(transform);
	}

	public UIItem Load(string id)
	{
		UIItem uIItem = UnityEngine.Object.Instantiate(ResourceCache.Load<UIItem>("UI/Element/Msg/" + id));
		AddElement(uIItem, autoNewLine: false);
		return uIItem;
	}

	private void AddElement(Component c, bool autoNewLine = true)
	{
		if (rect.rect.width + c.Rect().rect.width > box.maxWidth)
		{
			box.CreateNewBlock();
		}
		AddElement(c, box.currentBlock);
	}

	private void AddElement(Component c, MsgBlock block)
	{
		c.transform.SetParent(block.transform, worldPositionStays: false);
		block.layout.RebuildLayout();
		countElements++;
	}

	public void Reset()
	{
		for (int num = base.transform.childCount - 1; num >= 0; num--)
		{
			Transform child = base.transform.GetChild(num);
			if (!child.tag.Contains("IgnoreDestroy"))
			{
				PoolManager.DespawnOrDestroy(child);
			}
		}
		this.RebuildLayout();
	}
}
