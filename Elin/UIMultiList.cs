using System.Collections.Generic;
using UnityEngine;

public class UIMultiList : MonoBehaviour
{
	public List<ListOwner> owners = new List<ListOwner>();

	public Layer layer;

	public Window[] windows;

	public UIList[] lists;

	public UIHeader[] headers;

	public bool Double;

	public bool addTab;

	public void Clear()
	{
		owners.Clear();
	}

	public void AddOwner(int i, ListOwner o)
	{
		o.index = owners.Count;
		owners.Add(o);
		o.layer = layer;
		o.window = windows[i];
		o.list = lists[i];
		o.multi = this;
		o.main = i == 0;
		if (!Double && addTab)
		{
			o.tab = o.window.AddTab(o.TextTab);
		}
		o.OnCreate();
	}

	public void Build(UIList.SortMode m = UIList.SortMode.ByNone)
	{
		foreach (ListOwner owner in owners)
		{
			owner.list.sortMode = m;
		}
		if (Double)
		{
			owners[0].other = owners[1];
			owners[1].other = owners[0];
		}
	}

	public void Refresh()
	{
		foreach (ListOwner owner in owners)
		{
			owner.OnSwitchContent();
		}
	}
}
