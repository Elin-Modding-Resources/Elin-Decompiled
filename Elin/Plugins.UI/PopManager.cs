using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopManager : MonoBehaviour
{
	public static int outlineAlpha;

	public int maxLines;

	public Vector2 dir;

	public Vector2 align;

	public Vector2 spacing;

	public Vector2 startPos;

	public float durationMove;

	public float durationActive;

	public Ease ease;

	public bool startAtDestPos;

	public bool insert;

	public bool ignoreDestPos;

	[NonSerialized]
	public List<PopItem> items = new List<PopItem>();

	public T Get<T>(string id) where T : PopItem
	{
		return Util.Instantiate<T>("UI/Pop/" + id, this);
	}

	public PopItemText PopText(string text, Sprite sprite = null, string id = "PopText", Color c = default(Color), Vector3 destPos = default(Vector3), float duration = 0f)
	{
		PopItemText popItemText = Get<PopItemText>(id);
		popItemText.important = false;
		popItemText.SetText(text, sprite, c);
		Pop(popItemText, destPos, duration);
		return popItemText;
	}

	public PopItem Pop(PopItem item, Vector3 destPos = default(Vector3), float duration = 0f)
	{
		Text[] componentsInChildren = item.GetComponentsInChildren<Text>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].RebuildLayout();
		}
		item.RebuildLayout();
		if (insert)
		{
			items.Insert(0, item);
		}
		else
		{
			items.Add(item);
		}
		RectTransform rectTransform = item.Rect();
		if (!ignoreDestPos)
		{
			UnityEngine.Object.Destroy(item.GetComponent<UIFollow>());
			UpdateDestPos();
			Vector2 anchoredPosition = (startAtDestPos ? item.destPos : new Vector2(rectTransform.rect.width * align.x, rectTransform.rect.height * align.y));
			anchoredPosition += startPos;
			rectTransform.anchoredPosition = anchoredPosition;
			for (int j = 0; j < items.Count; j++)
			{
				items[j].Rect().DOAnchorPos(items[j].destPos, durationMove).SetEase(ease);
			}
		}
		else
		{
			UIFollow uIFollow = item.GetComponent<UIFollow>();
			if (!uIFollow)
			{
				uIFollow = item.gameObject.AddComponent<UIFollow>();
			}
			uIFollow.followPos = true;
			uIFollow.fixPos = destPos;
			rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(destPos) / BaseCore.Instance.uiScale;
		}
		if ((bool)item.animeIn)
		{
			item.animeIn.Play(item.transform);
		}
		item.killTimer = TweenUtil.Tween(((duration == 0f) ? 1f : duration) * durationActive + durationMove, null, delegate
		{
			Kill(item);
		});
		base.enabled = true;
		return item;
	}

	private void Update()
	{
		if (items.Count == 0)
		{
			base.enabled = false;
		}
		if (maxLines != 0 && items.Count > maxLines)
		{
			if (insert)
			{
				Kill(items.LastItem());
			}
			else
			{
				Kill(items[0]);
			}
		}
	}

	public void UpdateDestPos()
	{
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < items.Count; i++)
		{
			Rect rect = items[i].Rect().rect;
			zero.x += rect.width * dir.x + spacing.x;
			zero.y += rect.height * dir.y + spacing.y;
			items[i].destPos = zero + new Vector2(rect.width * align.x, rect.height * align.y);
			if ((bool)items[i].arrow)
			{
				items[i].arrow.enabled = i == 0;
			}
		}
	}

	public void Kill(PopItem item, bool instant = false)
	{
		if (item.important)
		{
			return;
		}
		TweenUtil.KillTween(ref item.killTimer);
		if (!instant && (bool)item.animeOut)
		{
			if (!item.isDying)
			{
				item.isDying = true;
				item.animeOut.Play(item.transform).OnComplete(delegate
				{
					_Kill(item);
				});
			}
		}
		else
		{
			_Kill(item);
		}
	}

	private void _Kill(PopItem item)
	{
		if (item == null || item.gameObject == null)
		{
			return;
		}
		DOTween.Kill(item.Rect());
		UnityEngine.Object.Destroy(item.gameObject);
		items.Remove(item);
		if (!ignoreDestPos)
		{
			UpdateDestPos();
			for (int i = 0; i < items.Count; i++)
			{
				items[i].Rect().DOAnchorPos(items[i].destPos, durationMove).SetEase(ease);
			}
		}
	}

	public void CopyAll(RectTransform rect)
	{
		foreach (PopItem item in items)
		{
			ContentSizeFitter[] componentsInChildren = item.GetComponentsInChildren<ContentSizeFitter>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			LayoutGroup[] componentsInChildren2 = item.GetComponentsInChildren<LayoutGroup>();
			for (int i = 0; i < componentsInChildren2.Length; i++)
			{
				componentsInChildren2[i].enabled = false;
			}
			GameObject go = UnityEngine.Object.Instantiate(item.gameObject);
			go.transform.SetParent(rect, worldPositionStays: false);
			go.transform.position = item.transform.position;
			Vector3 fixPos = Camera.main.ScreenToWorldPoint(go.transform.Rect().anchoredPosition * BaseCore.Instance.uiScale);
			UIFollow uIFollow = go.AddComponent<UIFollow>();
			uIFollow.followPos = true;
			uIFollow.fixPos = fixPos;
			uIFollow.LateUpdate();
			go.AddComponent<CanvasGroup>().DOFade(0f, 1.5f).SetDelay(0.5f)
				.OnComplete(delegate
				{
					if (go != null)
					{
						UnityEngine.Object.Destroy(go);
					}
				});
		}
	}

	public void KillAll(bool instant = false)
	{
		if (instant)
		{
			foreach (PopItem item in items)
			{
				TweenUtil.KillTween(ref item.killTimer);
				DOTween.Kill(item.Rect());
				UnityEngine.Object.Destroy(item.gameObject);
			}
			items.Clear();
		}
		else
		{
			for (int num = items.Count - 1; num >= 0; num--)
			{
				Kill(items[num], instant);
			}
		}
	}
}
