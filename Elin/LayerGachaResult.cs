using System.Collections.Generic;
using UnityEngine.UI;

public class LayerGachaResult : ELayer
{
	public enum Mode
	{
		Chara
	}

	public class Data
	{
		public SourceChara.Row row;

		public int weight;
	}

	public ItemGachaResult moldItem;

	public LayoutGroup layout;

	public UIButton buttonGetAll;

	public UIButton buttonDumpAll;

	public Mode mode;

	public List<ItemGachaResult> items = new List<ItemGachaResult>();

	public void PlayGacha(int n, string id)
	{
		layout.DestroyChildren();
		items.Clear();
		for (int i = 0; i < n; i++)
		{
			Chara c = Draw(id.Split('_').LastItem());
			ItemGachaResult itemGachaResult = Util.Instantiate(moldItem, layout);
			itemGachaResult.SetChara(c, this);
			items.Add(itemGachaResult);
		}
		ELayer.pc.things.AddCurrency(ELayer.pc, id, -n);
		Msg.Say("playedGacha", n.ToString() ?? "");
		buttonGetAll.onClick.AddListener(delegate
		{
			ELayer.Sound.Play("good");
			items.ForeachReverse(delegate(ItemGachaResult itemGachaResult2)
			{
				itemGachaResult2.Confirm(add: true);
			});
		});
		buttonDumpAll.onClick.AddListener(delegate
		{
			ELayer.Sound.Play("pay");
			items.ForeachReverse(delegate(ItemGachaResult itemGachaResult2)
			{
				itemGachaResult2.Confirm(add: false);
			});
		});
		Refresh();
	}

	public static Chara Draw(string id = "citizen")
	{
		List<Data> list = new List<Data>();
		int num = 0;
		foreach (SourceChara.Row row in ELayer.sources.charas.rows)
		{
			switch (id)
			{
			case "citizen":
				if (!row.gachaFilter.Contains("resident"))
				{
					continue;
				}
				break;
			case "livestock":
				if (!row.gachaFilter.Contains("livestock"))
				{
					continue;
				}
				break;
			case "unique":
				if (!row.gachaFilter.Contains("resident") || row.quality != 4)
				{
					continue;
				}
				break;
			default:
				if (!row.gachaFilter.Contains("resident") && !row.gachaFilter.Contains("livestock"))
				{
					continue;
				}
				break;
			}
			int num2 = ((row.chance <= 0) ? 1 : row.chance);
			if (row.LV < 10)
			{
				num2 = num2 * 300 / 100;
			}
			else if (row.LV < 20)
			{
				num2 = num2 * 250 / 100;
			}
			else if (row.LV < 30)
			{
				num2 = num2 * 200 / 100;
			}
			else if (row.LV < 50)
			{
				num2 = num2 * 150 / 100;
			}
			list.Add(new Data
			{
				row = row,
				weight = num2
			});
			num += num2;
		}
		int num3 = ELayer.rnd(num);
		int num4 = 0;
		Data data = list[0];
		foreach (Data item in list)
		{
			num4 += item.weight;
			if (num3 < num4)
			{
				data = item;
				break;
			}
		}
		return CharaGen.Create(data.row.id);
	}

	public void Refresh()
	{
		int num = 0;
		foreach (ItemGachaResult item in items)
		{
			num += item.GetMedal();
		}
		buttonDumpAll.mainText.text = "dumpAll".lang(num.ToString() ?? "");
	}
}
