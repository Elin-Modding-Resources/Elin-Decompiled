using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlider : Slider
{
	public UIText textMain;

	public UIText textInfo;

	public UIButton buttonNext;

	public UIButton buttonPrev;

	public void SetList<TValue>(TValue index, IList<TValue> list, Action<int, TValue> onChange, Func<TValue, string> getInfo = null)
	{
		SetList(list.IndexOf(index), list, onChange, getInfo);
	}

	public void SetList<TValue>(int index, IList<TValue> list, Action<int, TValue> onChange, Func<TValue, string> getInfo = null)
	{
		base.minValue = 0f;
		base.maxValue = list.Count - 1;
		index = Mathf.Clamp(index, 0, list.Count - 1);
		base.onValueChanged.RemoveAllListeners();
		base.onValueChanged.AddListener(delegate(float a)
		{
			int num = Mathf.Clamp((int)a, 0, list.Count - 1);
			onChange(num, list[num]);
			if (getInfo != null)
			{
				textInfo.text = getInfo(list[num]);
			}
		});
		if (getInfo != null)
		{
			textInfo.text = getInfo(list[index]);
		}
		value = index;
	}

	public void Prev()
	{
		value--;
	}

	public void Next()
	{
		value++;
	}
}
