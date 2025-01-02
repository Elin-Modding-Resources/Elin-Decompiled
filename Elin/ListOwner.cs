public class ListOwner<T1, T2> : ListOwner
{
	public virtual void OnClick(T1 a, T2 b)
	{
	}

	public virtual void OnInstantiate(T1 a, T2 b)
	{
	}

	public virtual void OnList()
	{
	}
}
public class ListOwner : EClass
{
	public Layer layer;

	public Window window;

	public UIMultiList multi;

	public ListOwner other;

	public UIList list;

	public WindowMenu menu;

	public Window.Setting.Tab tab;

	public string textTab;

	public string textHeader;

	public bool main;

	public int index;

	public ListOwner Main
	{
		get
		{
			if (!main)
			{
				return other;
			}
			return this;
		}
	}

	public virtual string IdTitle => GetType().Name;

	public virtual string IdHeaderRow => null;

	public virtual string TextTab => textTab.lang();

	public virtual string TextHeader
	{
		get
		{
			if (!textHeader.IsEmpty())
			{
				return textHeader.lang();
			}
			return "";
		}
	}

	public virtual void List()
	{
	}

	public virtual void OnCreate()
	{
	}

	public virtual void OnSwitchContent()
	{
		List();
		OnRefreshMenu();
		if (Lang.GetList(IdTitle) != null)
		{
			window.SetTitles(IdTitle, IdHeaderRow);
		}
		if (!textHeader.IsEmpty())
		{
			window.SetCaption(TextHeader);
		}
	}

	public virtual void OnRefreshMenu()
	{
		window.menuLeft.Clear();
		window.menuRight.Clear();
	}

	public void RefreshTab()
	{
		tab.button.mainText.SetText(TextTab);
	}

	public void MoveToOther(object c)
	{
		list.RemoveDynamic(c);
		other.List();
		list.RebuildLayoutTo(layer);
		SE.Resource();
	}
}
