using System.Collections.Generic;
using UnityEngine;

public class CharaAbility : EClass
{
	public static List<SourceElement.Row> randomAbilities = new List<SourceElement.Row>();

	public static List<SourceElement.Row> randomAbilitiesAdv = new List<SourceElement.Row>();

	public Chara owner;

	public ActList list = new ActList();

	public static List<SourceElement.Row> BuildRandomAbilityList(bool adv)
	{
		List<SourceElement.Row> list = new List<SourceElement.Row>();
		foreach (SourceElement.Row row in EClass.sources.elements.rows)
		{
			if (row.abilityType.Length == 0 || row.aliasRef == "mold")
			{
				continue;
			}
			switch (row.id)
			{
			case 5000:
			case 5001:
			case 5005:
			case 5040:
			case 5048:
			case 6400:
			case 6410:
			case 6800:
			case 8200:
				continue;
			}
			if (row.idMold != 0 && !adv)
			{
				switch (row.aliasRef)
				{
				case "eleEther":
				case "eleAcid":
				case "eleCut":
				case "eleImpact":
					continue;
				}
			}
			if (!row.tag.Contains("noRandomAbility"))
			{
				list.Add(row);
			}
		}
		return list;
	}

	public List<SourceElement.Row> GetRandomAbilityList()
	{
		if (randomAbilities.Count == 0)
		{
			randomAbilities = BuildRandomAbilityList(adv: false);
		}
		if (randomAbilitiesAdv.Count == 0)
		{
			randomAbilitiesAdv = BuildRandomAbilityList(adv: true);
		}
		if (!(owner.trait is TraitAdventurer))
		{
			return randomAbilities;
		}
		return randomAbilitiesAdv;
	}

	public CharaAbility(Chara _owner)
	{
		owner = _owner;
		Refresh();
	}

	public void Refresh()
	{
		list.items.Clear();
		string[] actCombat = owner.source.actCombat;
		for (int i = 0; i < actCombat.Length; i++)
		{
			string[] array = actCombat[i].Split('/');
			list.items.Add(new ActList.Item
			{
				act = ACT.dict[ConvertID(array[0])],
				chance = ((array.Length > 1) ? array[1].ToInt() : 100),
				pt = (array.Length > 2)
			});
		}
		if (owner.trait.MaxRandomAbility > 0 && owner._listAbility == null)
		{
			int num = owner.trait.MaxRandomAbility + EClass.rnd(2) - list.items.Count;
			if (num > 1)
			{
				owner._listAbility = new List<int>();
				for (int j = 0; j < num; j++)
				{
					owner._listAbility.Add(GetRandomAbilityList().RandomItemWeighted((SourceElement.Row e) => e.chance).id);
				}
			}
		}
		if (owner._listAbility == null)
		{
			return;
		}
		foreach (int item in owner._listAbility)
		{
			string alias = EClass.sources.elements.map[Mathf.Abs(item)].alias;
			list.items.Add(new ActList.Item
			{
				act = ACT.dict[alias],
				chance = 50,
				pt = (item < 0)
			});
		}
		string ConvertID(string s)
		{
			if (owner.MainElement == Element.Void)
			{
				return s;
			}
			if (EClass.sources.elements.alias[s].aliasRef == "mold")
			{
				return s + owner.MainElement.source.alias.Replace("ele", "");
			}
			return s;
		}
	}

	public void Add(int id, int chance, bool pt)
	{
		if (owner._listAbility == null)
		{
			owner._listAbility = new List<int>();
		}
		owner._listAbility.Add(id * ((!pt) ? 1 : (-1)));
		Refresh();
	}

	public void AddRandom()
	{
		if (owner._listAbility == null)
		{
			owner._listAbility = new List<int>();
		}
		owner._listAbility.Add(GetRandomAbilityList().RandomItemWeighted((SourceElement.Row e) => e.chance).id);
		Refresh();
	}

	public void Remove(int id)
	{
		owner._listAbility.Remove(id);
		if (owner._listAbility.Count == 0)
		{
			owner._listAbility = null;
		}
		Refresh();
	}

	public bool Has(int id)
	{
		foreach (ActList.Item item in list.items)
		{
			if (item.act.id == id)
			{
				return true;
			}
		}
		return false;
	}
}
