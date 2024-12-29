using System;
using System.Collections.Generic;

public class UIButtonLR : UIButton
{
	[NonSerialized]
	public int index;

	public UIButton left;

	public UIButton right;

	public UIText textTitle;

	public bool loopOption;

	[NonSerialized]
	public List<string> options = new List<string>();

	public void SetOptions(int _index, List<string> langs, Action<int> onSelect, bool invoke = true, string langTopic = null)
	{
		left.onClick.RemoveAllListeners();
		right.onClick.RemoveAllListeners();
		options = langs;
		left.onClick.AddListener(delegate
		{
			SelectOption(index - 1);
			onSelect(index);
		});
		right.onClick.AddListener(delegate
		{
			SelectOption(index + 1);
			onSelect(index);
		});
		SelectOption(_index);
		if (invoke)
		{
			onSelect(index);
		}
		if (!langTopic.IsEmpty())
		{
			textTitle.SetLang(langTopic);
		}
	}

	public void SetOptions<T>(int _index, IList<T> list, Func<T, string> funcName, Action<T> onSelect, bool invoke = true, string langTopic = null)
	{
		options.Clear();
		foreach (T item in list)
		{
			options.Add(funcName(item));
		}
		left.onClick.RemoveAllListeners();
		right.onClick.RemoveAllListeners();
		left.onClick.AddListener(delegate
		{
			SelectOption(index - 1);
			onSelect(list[index]);
		});
		right.onClick.AddListener(delegate
		{
			SelectOption(index + 1);
			onSelect(list[index]);
		});
		SelectOption(_index);
		if (invoke)
		{
			onSelect(list[index]);
		}
		if (!langTopic.IsEmpty())
		{
			textTitle.SetLang(langTopic);
		}
	}

	public void SelectOption(int _index)
	{
		if (_index < 0)
		{
			_index = (loopOption ? (options.Count - 1) : 0);
		}
		if (_index >= options.Count)
		{
			_index = ((!loopOption) ? (options.Count - 1) : 0);
		}
		index = _index;
		mainText.SetLang(options[index]);
	}
}
