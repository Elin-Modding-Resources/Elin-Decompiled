using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class CharaUpgrade : EClass
{
	public class Item : EClass
	{
		[JsonProperty]
		public int[] ints = new int[4];

		public int idEle
		{
			get
			{
				return ints[0];
			}
			set
			{
				ints[0] = value;
			}
		}

		public int value
		{
			get
			{
				return ints[1];
			}
			set
			{
				ints[1] = value;
			}
		}

		public int cost
		{
			get
			{
				return ints[2];
			}
			set
			{
				ints[2] = value;
			}
		}

		public DNA.Type type
		{
			get
			{
				return ints[3].ToEnum<DNA.Type>();
			}
			set
			{
				ints[3] = (int)value;
			}
		}
	}

	[JsonProperty]
	public List<Item> items = new List<Item>();

	[JsonProperty]
	public int count;

	[JsonProperty]
	public int spent;

	[JsonProperty]
	public int reset;

	[JsonProperty]
	public bool halt;

	public void Reset(Chara c)
	{
		foreach (Item item in items)
		{
			Element element = c.elements.GetElement(item.idEle);
			if (element != null)
			{
				if (element is Feat)
				{
					c.SetFeat(item.idEle, Mathf.Max(0, element.ValueWithoutLink - item.value));
				}
				else
				{
					c.elements.ModBase(item.idEle, -item.value);
				}
			}
		}
		items.Clear();
		c.feat += spent;
		spent = 0;
		count = 0;
		c.Refresh();
		reset++;
	}
}
