using System;
using System.Collections.Generic;

[Serializable]
public class CardFilter : EClass
{
	public class FilterItem
	{
		public bool exclude;

		public string name;
	}

	public class FilterCategory
	{
		public bool exclude;

		public SourceCategory.Row row;
	}

	public enum FilterMode
	{
		None,
		Exclude,
		Match,
		Equipped
	}

	public string id;

	public string[] strTag;

	public string[] strFilter;

	public string[] filterCategory;

	public string[] idCard;

	public List<FilterItem> tags = new List<FilterItem>();

	public List<FilterItem> filters = new List<FilterItem>();

	public List<FilterCategory> categories = new List<FilterCategory>();

	public List<SourceCategory.Row> categoriesInclude = new List<SourceCategory.Row>();

	private bool isInitialied;

	public bool isChara;

	public override string ToString()
	{
		return id;
	}

	protected virtual bool _ShouldPass(CardRow source)
	{
		return true;
	}

	public virtual bool ContainsTag(CardRow source, string str)
	{
		return source.tag.Contains(str);
	}

	public void Init()
	{
		BuildList(tags, strTag);
		BuildList(filters, strFilter);
		if (!filterCategory.IsEmpty())
		{
			string[] array = filterCategory;
			foreach (string text in array)
			{
				bool flag = text.StartsWith('-');
				SourceCategory.Row row = EClass.sources.categories.map[flag ? text.Remove(0, 1) : text];
				categories.Add(new FilterCategory
				{
					exclude = flag,
					row = row
				});
				if (!flag)
				{
					categoriesInclude.Add(row);
				}
			}
		}
		isInitialied = true;
	}

	public bool Pass(CardRow source)
	{
		if (!isInitialied)
		{
			Init();
		}
		if (source.chance == 0 || source.isOrigin)
		{
			return false;
		}
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < tags.Count; i++)
		{
			FilterItem filterItem = tags[i];
			if (filterItem.exclude)
			{
				if (ContainsTag(source, filterItem.name))
				{
					return false;
				}
				continue;
			}
			flag2 = true;
			if (!flag && ContainsTag(source, filterItem.name))
			{
				flag = true;
			}
		}
		if (flag2 && !flag)
		{
			return false;
		}
		for (int j = 0; j < filters.Count; j++)
		{
			FilterItem filterItem2 = filters[j];
			if (filterItem2.exclude)
			{
				if (source.filter.Contains(filterItem2.name))
				{
					return false;
				}
			}
			else if (!source.filter.Contains(filterItem2.name))
			{
				return false;
			}
		}
		if ((isChara && !source.isChara) || (!isChara && source.isChara))
		{
			return false;
		}
		if (categories.Count > 0)
		{
			bool flag3 = false;
			foreach (FilterCategory category in categories)
			{
				if ((!flag3 || category.exclude) && EClass.sources.categories.map[source.category].IsChildOf(category.row))
				{
					if (category.exclude)
					{
						return false;
					}
					flag3 = true;
				}
			}
			if (!flag3)
			{
				return false;
			}
		}
		if (!_ShouldPass(source))
		{
			return false;
		}
		return true;
	}

	public void BuildList(List<FilterItem> list, string[] strs)
	{
		if (strs != null)
		{
			foreach (string text in strs)
			{
				list.Add(GetFilterItem(text));
			}
		}
	}

	public static FilterItem GetFilterItem(string text)
	{
		bool flag = text.StartsWith("-");
		return new FilterItem
		{
			exclude = flag,
			name = (flag ? text.Substring(1) : text)
		};
	}
}
