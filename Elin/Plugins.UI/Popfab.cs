using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Popfab : MonoBehaviour
{
	public float fixX;

	public float fixY;

	public float spacing = 10f;

	public bool invert;

	public int maxItems;

	public Ease pushEase = Ease.Linear;

	public float pushDuration = 0.5f;

	private Transform _trans;

	[HideInInspector]
	public List<PopfabItem> list = new List<PopfabItem>();

	protected virtual void Awake()
	{
		_trans = base.transform;
	}

	public T Pop<T>(string id) where T : PopfabItem
	{
		return Util.Instantiate(ResourceCache.Load<T>("UI/Pop/" + id), this);
	}

	public PopfabItem Pop(string id, string text = null)
	{
		PopfabItem popfabItem = ResourceCache.Load<PopfabItem>("UI/Pop/" + id);
		popfabItem.Set(text);
		return Pop(popfabItem);
	}

	public PopfabItem Pop(PopfabItem item)
	{
		item.Init(_trans);
		if (maxItems > 0)
		{
			item.isUseDuration = false;
		}
		if (pushEase != 0 && list.Count > 0)
		{
			PushItems(new Vector3(0f, item.rect.sizeDelta.y + spacing, 0f));
		}
		list.Add(item);
		item.reciever = this;
		if (maxItems != 0 && list.Count >= maxItems)
		{
			for (int i = 0; i < list.Count - maxItems; i++)
			{
				if (!list[i].isOutro)
				{
					list[i].Outro();
				}
			}
		}
		item.Intro();
		return item;
	}

	public void Remove(PopfabItem item)
	{
		list.Remove(item);
	}

	public void Clear()
	{
		for (int num = list.Count - 1; num >= 0; num--)
		{
			list[num].Kill();
		}
	}

	public void ForceOutro()
	{
		foreach (PopfabItem item in list)
		{
			item.ForceOutro();
		}
	}

	public void PushItems(Vector3 pos)
	{
		if (invert)
		{
			pos *= -1f;
		}
		foreach (PopfabItem item in list)
		{
			item.destY += pos.y;
			item.rect.DOLocalMoveY(item.destY, pushDuration).SetEase(pushEase);
		}
	}

	private void OnDestroy()
	{
		for (int num = base.transform.childCount - 1; num >= 0; num--)
		{
			_trans.GetChild(num).GetComponent<PopfabItem>().Kill();
		}
	}
}
