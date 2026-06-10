using System.Collections.Generic;
using System.Linq;
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
		this.list.items.Clear();
		List<string> list = owner.source.actCombat.ToList();
		bool flag = true;
		for (int num = list.Count - 1; num >= 0; num--)
		{
			string text = list[num];
			string text2 = text.Split('/')[0];
			if (text2.IsEmpty())
			{
				list.RemoveAt(num);
				flag = false;
			}
			else if (!EClass.sources.elements.alias.ContainsKey(text2) || !ACT.dict.ContainsKey(text2))
			{
				if (EClass.sources.elements.fuzzyAlias.TryGetValue(text2.Trim(), out var value) && ACT.dict.ContainsKey(value))
				{
					list[num] = text.Replace(text2, value);
				}
				else
				{
					list.RemoveAt(num);
					ModUtil.LogModError("source chara row '" + owner.id + "' has invalid actCombat '" + text + "'", owner.source);
				}
				flag = false;
			}
		}
		if (!flag)
		{
			owner.source.actCombat = list.ToArray();
		}
		foreach (string item in list)
		{
			string[] array = item.Split('/');
			this.list.items.Add(new ActList.Item
			{
				act = ACT.dict[ConvertID(array[0])],
				chance = ((array.Length > 1) ? array[1].ToInt() : 100),
				pt = (array.Length > 2)
			});
		}
		if (owner.trait.MaxRandomAbility > 0 && owner._listAbility == null)
		{
			int num2 = owner.trait.MaxRandomAbility + EClass.rnd(2) - this.list.items.Count;
			if (num2 > 1)
			{
				owner._listAbility = new List<int>();
				for (int i = 0; i < num2; i++)
				{
					owner._listAbility.Add(GetRandomAbilityList().RandomItemWeighted((SourceElement.Row e) => e.chance).id);
				}
			}
		}
		if (owner._listAbility == null)
		{
			return;
		}
		foreach (int item2 in owner._listAbility)
		{
			string text3 = EClass.sources.elements.map.TryGetValue(Mathf.Abs(item2))?.alias;
			if (!text3.IsEmpty())
			{
				this.list.items.Add(new ActList.Item
				{
					act = ACT.dict[text3],
					chance = 50,
					pt = (item2 < 0)
				});
			}
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
		if (owner.IsPC && owner.HasElement(1274))
		{
			Element element = owner.elements.GetElement(id);
			if (element == null)
			{
				owner.elements.ModBase(id, 1);
			}
			else if (!(element is Spell))
			{
				element.vPotential = 0;
			}
		}
		Refresh();
		if (owner.IsPC)
		{
			LayerAbility.Redraw();
		}
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
		if (owner.IsPC && owner.HasElement(1274) && owner.HasElement(id) && owner._listAbility.IndexOf(id) == -1)
		{
			Element element = EClass.pc.elements.GetElement(id);
			if (!(element is Spell))
			{
				element.vPotential = -1;
			}
		}
		if (owner._listAbility.Count == 0)
		{
			owner._listAbility = null;
		}
		Refresh();
		if (owner.IsPC)
		{
			LayerAbility.Redraw();
		}
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
