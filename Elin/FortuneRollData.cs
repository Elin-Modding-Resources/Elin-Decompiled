using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class FortuneRollData : EClass
{
	public class Prize : EClass
	{
		[JsonProperty]
		public int grade;

		[JsonProperty]
		public string id;

		[JsonProperty]
		public string idRef;

		[JsonProperty]
		public bool claimed;

		public Card model => EClass.sources.cards.map[id].model;

		public int GetNum()
		{
			if (id == "ration")
			{
				return 10;
			}
			if (id == "medal")
			{
				if (grade != 2)
				{
					if (grade != 3)
					{
						return 1;
					}
					return 10;
				}
				return 3;
			}
			if (id == "plat")
			{
				if (grade != 1)
				{
					if (grade != 2)
					{
						if (grade != 3)
						{
							return 1;
						}
						return 50;
					}
					return 20;
				}
				return 10;
			}
			if (id == "1165" && grade == 3)
			{
				return 3;
			}
			return 1;
		}

		public void AddNote(UINote n)
		{
			string text = "_circle".lang().TagColor(EClass.sources.materials.alias[mats[grade]].GetColor()) + "  " + Lang.GetList("fortuneroll")[grade];
			string text2 = (model.IsUnique ? "★" : "") + EClass.sources.cards.map[id].GetName();
			string text3 = id;
			if (!(text3 == "panty"))
			{
				if (text3 == "mathammer")
				{
					if (idRef.IsEmpty())
					{
						idRef = "iron";
					}
					text2 = "_of".lang(EClass.sources.materials.alias[idRef].GetName(), text2);
				}
			}
			else
			{
				string name = EClass.sources.cards.map[idRef].GetName();
				text2 = "_of".lang(name, text2);
			}
			int num = GetNum();
			if (num > 1)
			{
				text2 = text2 + " x " + num;
			}
			if (claimed)
			{
				text2 = "fortuneroll_claimed".lang();
			}
			n.AddTopic("TopicDomain", text, text2.ToTitleCase().TagColor(model.IsUnique ? FontColor.Great : FontColor.Good));
		}
	}

	public static string[] mats = new string[4] { "plastic", "water", "hide_dragon", "gold" };

	public static int[] chances = new int[4] { 1, 8, 25, 60 };

	[JsonProperty]
	public List<Prize> prizes = new List<Prize>();

	[JsonProperty]
	public int count;

	[JsonProperty]
	public int seed;

	[JsonProperty]
	public int dateNextRefresh;

	public void Refresh()
	{
		if (EClass.world.date.GetRaw() >= dateNextRefresh)
		{
			Date date = EClass.world.date.Copy();
			date.day = 1;
			date.hour = 0;
			date.min = 0;
			date.AddMonth(1);
			dateNextRefresh = date.GetRaw();
			count++;
			RefreshPrize();
		}
	}

	public void RefreshPrize()
	{
		prizes.Clear();
		Rand.SetSeed(EClass.game.seed + seed + count);
		List<List<string>> list = GetPrizeList();
		if (EClass._zone.IsTown && EClass._zone.lv == 0)
		{
			Add(3);
			Add(2);
			Add(2);
			Add(1);
			Add(1);
			Add(1);
		}
		Rand.SetSeed();
		void Add(int grade)
		{
			List<string> list2 = list[grade];
			int index = EClass.rnd(list2.Count);
			Prize prize = new Prize
			{
				id = list2[index],
				grade = grade
			};
			string id = prize.id;
			if (!(id == "panty"))
			{
				if (id == "mathammer")
				{
					SourceMaterial.Row randomMaterial = MATERIAL.GetRandomMaterial(20);
					prize.idRef = randomMaterial.alias;
				}
			}
			else
			{
				IEnumerable<SourceChara.Row> ie = EClass.sources.charas.map.Values.Where((SourceChara.Row r) => (r.model.trait as TraitChara).IsWearingPanty && !r.HasTag(CTAG.noRandomProduct));
				prize.idRef = ie.RandomItem().id;
			}
			prizes.Add(prize);
			list2.RemoveAt(index);
		}
	}

	public List<List<string>> GetPrizeList()
	{
		List<List<string>> list = new List<List<string>>
		{
			new List<string> { "scrubber", "tissue", "plat" },
			new List<string>
			{
				"microchip", "1089", "150", "855", "medal", "water", "goods_charm", "electronicsS", "electronics", "plat",
				"plat", "ration", "backpack2", "sister", "rp_food", "rp_block", "157", "sleepingbag"
			},
			new List<string>
			{
				"computer", "834", "1090", "goods_figure", "goods_canvas", "mb_1", "mb_2", "mb_3", "mb_4", "mb_5",
				"1174", "1085", "toilet", "714", "nobility", "plat", "1165", "mathammer", "medal", "bbq",
				"panty", "beehive", "ticket_resident", "lovepotion", "crystal_sun"
			},
			new List<string>
			{
				(EClass.player.luckycoin < 10) ? "goods_coin" : "plat",
				"plat",
				"1165",
				"boat3",
				"medal"
			}
		};
		if (EClass.pc.faction.IsGlobalPolicyActive(2712))
		{
			for (int i = 0; i < (EClass.debug.enable ? 50 : 5); i++)
			{
				list[2].Add("panty");
			}
		}
		return list;
	}

	public void GetPrize(int grade, int seed)
	{
		Rand.SetSeed(seed);
		Prize prize = null;
		List<Prize> list = prizes.Where((Prize p) => p.grade == grade && !p.claimed).ToList();
		if (list.Count > 0)
		{
			prize = list.RandomItem();
		}
		Card card = null;
		if (prize != null)
		{
			if (prize.id == "sister")
			{
				card = CharaGen.Create("sister");
			}
			else
			{
				card = ThingGen.Create(prize.id).SetNum(prize.GetNum());
				switch (card.id)
				{
				case "rp_food":
				case "rp_block":
					card.SetLv(10);
					break;
				case "goods_coin":
					EClass.player.luckycoin++;
					if (EClass.player.luckycoin >= 10)
					{
						prize.claimed = true;
					}
					break;
				case "mathammer":
					card.ChangeMaterial(prize.idRef);
					card.noSell = true;
					break;
				case "panty":
					card.c_idRefCard = prize.idRef;
					card.rarity = Rarity.Legendary;
					break;
				}
			}
			if (grade != 3)
			{
				prize.claimed = true;
			}
		}
		else
		{
			card = ThingGen.Create(GetPrizeList()[0].RandomItem());
		}
		if (card.isChara)
		{
			EClass._zone.AddCard(card, EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false) ?? EClass.pc.pos);
			card.Chara.MakeMinion(EClass.pc);
		}
		else
		{
			EClass.pc.Pick(card.Thing);
		}
		Rand.SetSeed();
	}

	public void WriteNote(UINote n)
	{
		n.Clear();
		n.AddHeader("fortuneroll_prize");
		n.Space(4);
		foreach (Prize prize in prizes)
		{
			prize.AddNote(n);
		}
		string text = "_circle".lang().TagColor(Color.white) + "  " + Lang.GetList("fortuneroll")[0];
		n.AddTopic("TopicDomain", text, "fortuneroll_lose".lang());
		n.Build();
	}
}
