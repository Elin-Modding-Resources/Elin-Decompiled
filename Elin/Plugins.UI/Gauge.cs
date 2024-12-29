using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Gauge : MonoBehaviour
{
	public UIText textNow;

	public Image barCircle;

	public Image bgBar;

	public RawImage bar;

	public SpriteRenderer srBar;

	public float duration;

	public float originalWidth;

	public Gradient gradient;

	public bool useRatio;

	public bool setText = true;

	[NonSerialized]
	public float value;

	[NonSerialized]
	public float lastValue = -1f;

	[NonSerialized]
	public bool first = true;

	[NonSerialized]
	public bool hideBar;

	public Tween tween;

	private int _num;

	private int _lastNum;

	private int max;

	private int Num
	{
		get
		{
			return _num;
		}
		set
		{
			_num = value;
			if (_num != _lastNum)
			{
				if ((bool)textNow && setText)
				{
					textNow.text = (useRatio ? (Mathf.RoundToInt((float)_num / (float)max * 100f) + "%") : (_num.ToString() ?? ""));
				}
				_lastNum = _num;
			}
		}
	}

	public void UpdateValue(float now, float _max)
	{
		UpdateValue((int)(now * 100f), (int)(_max * 100f));
	}

	public void UpdateValue(int now, int _max)
	{
		max = _max;
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		if ((bool)bar)
		{
			bar.SetActive(!hideBar);
		}
		if ((bool)bgBar)
		{
			bgBar.SetActive(!hideBar);
		}
		value = (float)now / (float)max;
		if (value == lastValue)
		{
			return;
		}
		if (value < 0f)
		{
			value = 0f;
		}
		else if (value > 1f)
		{
			value = 1f;
		}
		if ((bool)textNow && setText)
		{
			textNow.color = gradient.Evaluate((float)now / (float)max);
			if (first)
			{
				Num = now;
			}
			else
			{
				DOTween.To(() => Num, delegate(int x)
				{
					Num = x;
				}, now, duration);
			}
		}
		TweenUtil.KillTween(ref tween);
		if ((bool)bar)
		{
			RectTransform rectTransform = bar.Rect();
			Vector2 vector = new Vector2(originalWidth * value, rectTransform.sizeDelta.y);
			if (first)
			{
				rectTransform.sizeDelta = vector;
				first = false;
				return;
			}
			tween = rectTransform.DOSizeDelta(vector, duration);
		}
		else if ((bool)srBar)
		{
			Vector2 vector2 = new Vector2(value * 100f, srBar.size.y);
			if (first)
			{
				srBar.size = vector2;
				first = false;
				return;
			}
			tween = DOTween.To(() => srBar.size, delegate(Vector2 x)
			{
				srBar.size = x;
			}, vector2, duration);
		}
		else
		{
			barCircle.fillAmount = value;
		}
		lastValue = value;
	}
}
