using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDropdown : Dropdown
{
	[NonSerialized]
	public UIButton bl;

	public static UIDropdown activeInstance;

	public override void OnCancel(BaseEventData eventData)
	{
	}

	private void Update()
	{
		if (activeInstance == this && Input.GetMouseButtonDown(1))
		{
			Hide();
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		base.template.GetComponentInChildren<ScrollRect>().scrollSensitivity = UIScrollView.sensitivity;
	}

	protected override GameObject CreateBlocker(Canvas rootCanvas)
	{
		activeInstance = this;
		return base.CreateBlocker(rootCanvas);
	}

	protected override void DestroyBlocker(GameObject blocker)
	{
		activeInstance = null;
		if ((bool)bl)
		{
			UnityEngine.Object.Destroy(bl.gameObject);
			bl = null;
		}
		base.DestroyBlocker(blocker);
	}

	public void SetList<TValue>(string name, IList<TValue> list, Func<TValue, int, string> getName, Action<int, TValue> onChange, bool notify = true)
	{
		int index = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (getName(list[i], i) == name)
			{
				index = i;
				break;
			}
		}
		SetList(index, list, getName, onChange, notify);
	}

	public void SetList<TValue>(int _index, IList<TValue> list, Func<TValue, int, string> getName, Action<int, TValue> onChange, bool notify = true)
	{
		base.options.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			base.options.Add(new OptionData
			{
				text = getName(list[i], i)
			});
		}
		base.onValueChanged.RemoveAllListeners();
		base.onValueChanged.AddListener(delegate(int a)
		{
			onChange(a, list[a]);
		});
		if (notify)
		{
			base.value = _index;
		}
		else
		{
			SetValueWithoutNotify(_index);
		}
		RefreshShownValue();
	}

	public void Next()
	{
		if (base.value < base.options.Count - 1)
		{
			base.value++;
		}
	}

	public void Prev()
	{
		if (base.value > 0)
		{
			base.value--;
		}
	}
}
