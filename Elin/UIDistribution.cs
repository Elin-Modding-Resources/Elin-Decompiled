using System.Collections.Generic;

public class UIDistribution : EMono
{
	public Card container;

	public UIList list;

	public Window.SaveData data;

	public bool needRefresh;

	private void OnEnable()
	{
		if (needRefresh)
		{
			needRefresh = false;
			Refresh();
		}
	}

	public void SetContainer(Card t, Window.SaveData d)
	{
		container = t;
		data = d;
		needRefresh = true;
		this.SetActive(enable: false);
	}

	public void Refresh()
	{
		HashSet<int> cats = data.cats;
		list.callbacks = new UIList.Callback<SourceCategory.Row, ButtonCategory>
		{
			onClick = delegate(SourceCategory.Row a, ButtonCategory b)
			{
				if (a.children.Count > 0)
				{
					b.buttonFold.onClick.Invoke();
				}
			},
			onInstantiate = delegate(SourceCategory.Row a, ButtonCategory b)
			{
				b.uid = a.uid;
				b.mainText.text = a.GetName().ToTitleCase();
				bool flag3 = false;
				foreach (int item in cats)
				{
					if (a.Contatin(item))
					{
						flag3 = true;
						break;
					}
				}
				b.SetFold(a.children.Count > 0, !flag3, delegate(UIList l)
				{
					foreach (SourceCategory.Row child in a.children)
					{
						l.Add(child);
					}
				});
				b.buttonToggle.icon.SetActive(cats.Contains(a.uid));
				b.buttonToggle.SetOnClick(delegate
				{
					bool flag4 = !cats.Contains(a.uid);
					b.buttonToggle.icon.SetActive(flag4);
					if (flag4)
					{
						cats.Add(a.uid);
					}
					else
					{
						cats.Remove(a.uid);
					}
					SetAll(a.uid, flag4);
					SE.Click();
				});
			},
			onRefresh = null
		};
		foreach (SourceCategory.Row row2 in EMono.sources.categories.rows)
		{
			if (row2.parent == null && row2.id != "new" && row2.id != "none")
			{
				list.Add(row2);
			}
		}
		list.Refresh();
		void SetAll(int uid, bool enable)
		{
			SourceCategory.Row row = EMono.sources.categories.rows.Find((SourceCategory.Row a) => a.uid == uid);
			if (row != null)
			{
				foreach (SourceCategory.Row row3 in EMono.sources.categories.rows)
				{
					if (row.Contatin(row3.uid))
					{
						if (enable)
						{
							cats.Add(row3.uid);
						}
						else
						{
							cats.Remove(row3.uid);
						}
					}
					else if (row.IsChildOf(row3.uid) && enable)
					{
						cats.Add(row3.uid);
					}
				}
				bool flag;
				do
				{
					flag = true;
					foreach (SourceCategory.Row row4 in EMono.sources.categories.rows)
					{
						if (row4.children.Count != 0 && cats.Contains(row4.uid))
						{
							bool flag2 = false;
							foreach (int item2 in cats)
							{
								if (item2 != row4.uid && row4.Contatin(item2))
								{
									flag2 = true;
									break;
								}
							}
							if (!flag2)
							{
								cats.Remove(row4.uid);
								flag = false;
							}
						}
					}
				}
				while (!flag);
				ButtonCategory[] componentsInChildren = list.GetComponentsInChildren<ButtonCategory>();
				foreach (ButtonCategory buttonCategory in componentsInChildren)
				{
					buttonCategory.buttonToggle.icon.SetActive(cats.Contains(buttonCategory.uid));
				}
			}
		}
	}
}
