using System.Collections.Generic;
using UnityEngine;

public class SpawnList : EClass
{
	public static Dictionary<string, SpawnList> allList = new Dictionary<string, SpawnList>();

	public static SpawnList tempList = new SpawnList();

	public string id;

	public List<CardRow> rows = new List<CardRow>();

	public CardFilter filter;

	public int totalChance;

	public static SpawnList Get(string id, string parent = null, CardFilter filter = null)
	{
		SpawnList spawnList = allList.TryGetValue(id);
		if (spawnList != null)
		{
			if (spawnList.filter != filter && filter != null)
			{
				spawnList.CreateMaster(filter, parent);
			}
			return spawnList;
		}
		spawnList = new SpawnList(id);
		if (filter != null)
		{
			spawnList.CreateMaster(filter, parent);
		}
		allList.Add(id, spawnList);
		return spawnList;
	}

	public SpawnList(string id = null)
	{
		if (!id.IsEmpty())
		{
			SourceSpawnList.Row row = EClass.sources.spawnLists.map.TryGetValue(id);
			if (row != null)
			{
				bool flag = row.type == "chara";
				CardFilter cardFilter = (flag ? ((CardFilter)new CharaFilter()) : ((CardFilter)new ThingFilter()));
				cardFilter.isChara = flag;
				cardFilter.strTag = row.tag;
				cardFilter.strFilter = row.filter;
				cardFilter.filterCategory = row.category;
				cardFilter.idCard = row.idCard;
				CreateMaster(cardFilter, row.parent);
			}
			this.id = id;
		}
	}

	public void Add(CardRow row)
	{
		rows.Add(row);
		totalChance += row.chance;
	}

	public void CreateMaster(CardFilter _filter, string parent = null)
	{
		rows.Clear();
		totalChance = 0;
		filter = _filter;
		List<CardRow> list = (parent.IsEmpty() ? EClass.sources.cards.rows : Get(parent).rows);
		for (int i = 0; i < list.Count; i++)
		{
			CardRow cardRow = list[i];
			if (cardRow.chance > 0 && filter.Pass(cardRow))
			{
				Add(cardRow);
			}
		}
		if (!filter.idCard.IsEmpty())
		{
			string[] idCard = filter.idCard;
			foreach (string key in idCard)
			{
				CardRow row = EClass.sources.cards.map[key];
				Add(row);
			}
		}
	}

	public SpawnList Filter(int lv, int levelRange = -1)
	{
		tempList.rows.Clear();
		tempList.totalChance = 0;
		tempList.filter = filter;
		for (int i = 0; i < rows.Count; i++)
		{
			CardRow cardRow = rows[i];
			if (levelRange != -1)
			{
				if (cardRow.isChara && (cardRow as SourceChara.Row).mainElement.Length >= 2)
				{
					SourceChara.Row obj = cardRow as SourceChara.Row;
					bool flag = false;
					string[] mainElement = obj.mainElement;
					for (int j = 0; j < mainElement.Length; j++)
					{
						string[] array = mainElement[j].Split('/');
						SourceElement.Row row = EClass.sources.elements.alias["ele" + array[0]];
						if (Mathf.Abs(cardRow.LV * row.eleP / 100 - lv) < levelRange)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						continue;
					}
				}
				else if (Mathf.Abs(cardRow.LV - lv) >= levelRange)
				{
					continue;
				}
			}
			else if (cardRow.LV > lv)
			{
				continue;
			}
			tempList.rows.Add(cardRow);
			tempList.totalChance += cardRow.chance;
		}
		if (tempList.rows.Count == 0)
		{
			Debug.Log("list contains no item: " + id + "/" + lv);
			return this;
		}
		return tempList;
	}

	public CardRow Select(int lv = -1, int levelRange = -1)
	{
		if (lv != -1)
		{
			if (levelRange != -1)
			{
				for (int i = 0; i < 50; i++)
				{
					SpawnList spawnList = Filter(lv + i - i * i, levelRange + i * i);
					if (spawnList.rows.Count > 5)
					{
						return spawnList.Select();
					}
				}
			}
			lv = lv + 2 + EClass.rnd(EClass.rnd(EClass.rnd(EClass.rnd(Mathf.Min(lv * 2, 20)) + 1) + 1) + 1);
			return Filter(lv).Select();
		}
		if (filter != null && filter.categoriesInclude.Count > 0)
		{
			SourceCategory.Row r = filter.categoriesInclude.RandomItem();
			for (int j = 0; j < 100; j++)
			{
				CardRow cardRow = _Select();
				if (cardRow != null && cardRow.Category.IsChildOf(r))
				{
					return cardRow;
				}
			}
		}
		return _Select();
		CardRow _Select()
		{
			int num = EClass.rnd(totalChance);
			int num2 = 0;
			foreach (CardRow row in rows)
			{
				num2 += row.chance;
				if (num < num2)
				{
					return row;
				}
			}
			if (rows.Count == 0)
			{
				Debug.Log("no item:" + id);
				if (filter != null)
				{
					foreach (CardFilter.FilterItem tag in filter.tags)
					{
						Debug.Log(tag.name + "/" + tag.exclude);
					}
				}
				return null;
			}
			return rows[EClass.rnd(rows.Count)];
		}
	}

	public CardRow GetRandom()
	{
		return rows[EClass.rnd(rows.Count)];
	}

	public CardRow GetFirst()
	{
		return rows[0];
	}
}
