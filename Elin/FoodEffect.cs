using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FoodEffect : EClass
{
	public static bool IsLeftoverable(Thing food)
	{
		if (food.HasElement(758) || food.HasElement(1229))
		{
			return false;
		}
		return food.trait is TraitLunch;
	}

	public static void Proc(Chara c, Thing food, bool consume = true)
	{
		if (food.id == "bloodsample")
		{
			food.ModNum(-1);
			return;
		}
		food.CheckJustCooked();
		bool flag = EClass._zone.IsPCFactionOrTent && c.IsInSpot<TraitSpotDining>();
		int num = (food.isCrafted ? ((EClass.pc.Evalue(1650) >= 3) ? 5 : 0) : 0);
		float num2 = (float)(100 + (food.HasElement(757) ? 10 : 0) + (flag ? 10 : 0) + num + Mathf.Min(food.QualityLv * 10, 100)) / 200f;
		if (num2 < 0.1f)
		{
			num2 = 0.1f;
		}
		int num3 = Mathf.Clamp(food.Evalue(10), 0, 10000);
		float num4 = 25f;
		float num5 = 1f;
		string idTaste = "";
		bool flag2 = food.HasElement(708);
		bool flag3 = food.HasElement(709);
		bool flag4 = c.HasElement(1205);
		bool flag5 = food.IsDecayed || flag3;
		bool flag6 = IsLeftoverable(food);
		bool flag7 = EClass._zone.HasField(10001) && food.GetBool(128);
		c.AddFoodHistory(food);
		if (c.IsPCFaction && !c.IsPC)
		{
			int num6 = c.CountNumEaten(food);
			bool flag8 = c.GetFavFood().id == food.id;
			if (num6 < 2 || flag8)
			{
				if (num6 == 1 || flag8 || EClass.rnd(4) == 0)
				{
					c.Talk("foodNice");
				}
			}
			else if (num6 > 3 && EClass.rnd(num6) >= 3)
			{
				c.Talk("foodBored");
			}
		}
		if (food.IsBlessed)
		{
			num2 *= 1.5f;
		}
		if (food.IsCursed)
		{
			num2 *= 0.5f;
		}
		if (flag4)
		{
			if (flag2)
			{
				num5 *= 2f;
				num2 *= 1.3f;
			}
			else
			{
				num5 *= 0.5f;
				num2 /= 2f;
				num3 /= 2;
			}
		}
		else if (flag2)
		{
			num5 = 0f;
			num2 *= 0.5f;
		}
		if (c.HasElement(1250))
		{
			if (food.HasElement(710))
			{
				num2 = num2 * 0.1f * (float)(food.Evalue(710) + 10);
			}
			else
			{
				num3 /= 10;
			}
		}
		if (c.HasElement(1200))
		{
			num2 *= 1f + (float)c.Evalue(1200) * 0.3f;
		}
		if (!c.IsPC)
		{
			num2 *= 3f;
		}
		if (flag5 && !c.HasElement(480))
		{
			if (c.IsPC)
			{
				if (flag3)
				{
					c.Say("food_undead");
				}
				c.Say("food_rot");
			}
			num5 = 0f;
			num3 /= 2;
		}
		else
		{
			switch (food.source._origin)
			{
			case "meat":
				if (c.IsPC)
				{
					c.Say("food_raw_meat");
				}
				num2 *= 0.7f;
				num5 = 0.5f;
				break;
			case "fish":
				if (c.IsHuman)
				{
					if (c.IsPC)
					{
						c.Say("food_raw_fish");
					}
					num2 *= 0.9f;
					num5 = 0.5f;
				}
				break;
			case "dough":
				if (c.IsPC)
				{
					c.Say("food_raw_powder");
				}
				num2 *= 0.9f;
				num5 = 0.5f;
				break;
			}
		}
		float num7 = (flag7 ? num3 : Mathf.Min(c.hunger.value, num3));
		if (c.hunger.GetPhase() >= 3)
		{
			num7 *= 1.1f;
		}
		ProcNutrition(c, food, num2, num7);
		if (!c.isDead)
		{
			ProcTrait(c, food);
			if (!c.isDead)
			{
				num4 += (float)food.Evalue(70);
				num4 += (float)(food.Evalue(72) / 2);
				num4 += (float)(food.Evalue(73) / 2);
				num4 += (float)(food.Evalue(75) / 2);
				num4 += (float)(food.Evalue(76) * 3 / 2);
				num4 += (float)food.Evalue(440);
				num4 += (float)(food.Evalue(445) / 2);
				num4 -= (float)food.Evalue(71);
				num4 += (float)food.Evalue(18);
				num4 += (float)(num3 / 2);
				num4 *= num5;
				if (idTaste.IsEmpty())
				{
					if (num4 > 100f)
					{
						idTaste = "food_great";
					}
					else if (num4 > 70f)
					{
						idTaste = "food_good";
					}
					else if (num4 > 50f)
					{
						idTaste = "food_soso";
					}
					else if (num4 > 30f)
					{
						idTaste = "food_average";
					}
					else
					{
						idTaste = "food_bad";
					}
					if (c.IsPC)
					{
						c.Say(idTaste);
						if (flag2)
						{
							c.Say(flag4 ? "food_human_pos" : "food_human_neg");
						}
						else if (flag4)
						{
							c.Say("food_human_whine");
						}
					}
				}
				if (LangGame.Has(idTaste + "2"))
				{
					c.Say(idTaste + "2", c, food);
				}
				if (!c.IsPCParty)
				{
					num3 *= 2;
				}
				num3 = num3 * (100 + c.Evalue(1235) * 10) / (100 + c.Evalue(1234) * 10 + c.Evalue(1236) * 15);
				c.hunger.Mod(-num3);
				if (flag2)
				{
					if (!flag4)
					{
						if (c.IsHuman)
						{
							c.AddCondition<ConInsane>(200);
							c.SAN.Mod(15);
						}
						if (EClass.rnd(c.IsHuman ? 5 : 20) == 0)
						{
							c.SetFeat(1205, 1, msg: true);
							flag4 = true;
						}
					}
					if (flag4)
					{
						c.SetInt(31, EClass.world.date.GetRaw() + 10080);
					}
				}
				else if (flag4 && c.GetInt(31) < EClass.world.date.GetRaw())
				{
					c.SetFeat(1205, 0, msg: true);
				}
				if (flag5 && !c.HasElement(480))
				{
					c.AddCondition<ConParalyze>();
					c.AddCondition<ConConfuse>(200);
				}
				if (c.HasCondition<ConAnorexia>())
				{
					c.Vomit();
				}
				if (num3 > 20 && c.HasElement(1413))
				{
					Thing thing = ThingGen.Create("seed");
					if (EClass.rnd(EClass.debug.enable ? 2 : 10) == 0)
					{
						TraitSeed.ApplySeed(thing, (EClass.rnd(4) == 0) ? 118 : ((EClass.rnd(3) == 0) ? 119 : 90));
					}
					thing.SetNum(2 + EClass.rnd(3));
					c.Talk("vomit");
					c.Say("fairy_vomit", c, thing);
					c.PickOrDrop(c.pos, thing);
				}
				food.trait.OnEat(c);
			}
		}
		if (consume)
		{
			num7 += 5f;
			if (flag6 && (float)food.Evalue(10) > num7 + 10f)
			{
				food.elements.SetTo(10, (int)Mathf.Max((float)food.Evalue(10) - num7, 1f));
				food.SetBool(125, enable: true);
				if (food.HasElement(1229))
				{
					food.elements.Remove(1229);
				}
			}
			else
			{
				food.ModNum(-1);
			}
		}
		if (c.isDead)
		{
			return;
		}
		if (!c.IsCat && food.trait is TraitFoodChuryu)
		{
			int num8 = 0;
			foreach (Chara item in c.pos.ListCharasInRadius(c, 5, (Chara c) => c.IsCat))
			{
				item.Say("angry", item);
				item.ShowEmo(Emo.angry);
				item.PlaySound("Animal/Cat/cat_angry");
				if (c.IsPC)
				{
					EClass.player.ModKarma(-3);
				}
				num8++;
			}
			EClass.player.stats.angryCats += num8;
			Debug.Log(num8 + "/" + EClass.player.stats.angryCats);
			if (num8 >= 10)
			{
				Steam.GetAchievement(ID_Achievement.CHURYU);
			}
		}
		if (c.IsPC && EClass._zone is Zone_Lothria)
		{
			switch (food.id)
			{
			case "681":
			case "pie_meat":
			case "pie_fish":
				Steam.GetAchievement(ID_Achievement.ASHLAND_PIE);
				break;
			}
		}
		if (!(food.trait is TraitGene) || !c.IsSlimeEvolvable)
		{
			return;
		}
		DNA c_DNA = food.c_DNA;
		int slot = c_DNA.slot;
		CharaGenes genes = c.c_genes;
		int excess = c.CurrentGeneSlot + slot - c.MaxGeneSlot;
		switch (c_DNA.type)
		{
		case DNA.Type.Inferior:
			if (genes != null)
			{
				RemoveDNA(fromOldest: false);
			}
			return;
		case DNA.Type.Brain:
			if (genes != null)
			{
				genes.items.Shuffle();
				Msg.Say("reconstruct", c);
				c.Say("food_mind", c);
				c.AddCondition<ConHallucination>();
			}
			return;
		}
		if (excess > 0)
		{
			while (excess > 0 && genes != null && genes.items.Count != 0)
			{
				RemoveDNA(fromOldest: true);
			}
		}
		c_DNA.Apply(c);
		c.Say("little_eat", c);
		c.PlaySound("ding_potential");
		SE.Play("mutation");
		c.PlayEffect("identify");
		void RemoveDNA(bool fromOldest)
		{
			DNA dNA = (fromOldest ? genes.items[0] : genes.items.Last());
			CharaGenes.Remove(c, dNA);
			excess -= dNA.slot;
		}
	}

	public static void ProcDrink(Chara c, Thing food)
	{
		int num = Mathf.Max(1, food.Evalue(10));
		float num2 = (float)(100 + Mathf.Min(food.QualityLv * 10, 100)) / 200f;
		if (num2 < 0.1f)
		{
			num2 = 0.1f;
		}
		if (c.hunger.GetPhase() <= 0)
		{
			num = 0;
		}
		c.hunger.Mod(-num);
		ProcNutrition(c, food, num2, num);
		if (!c.isDead)
		{
			ProcTrait(c, food);
		}
		if (!c.isDead)
		{
			food.trait.OnDrink(c);
		}
	}

	public static void ProcNutrition(Chara c, Thing food, float effP, float validFill)
	{
		bool flag = food.HasElement(709);
		if ((food.IsDecayed || flag) && !c.HasElement(480))
		{
			c.ModExp(70, -300);
			c.ModExp(71, -300);
			c.ModExp(72, -200);
			c.ModExp(73, -200);
			c.ModExp(74, -200);
			c.ModExp(75, 500);
			c.ModExp(76, -200);
			c.ModExp(77, -300);
			return;
		}
		effP = effP * validFill / 10f;
		if (c.HasCondition<ConAnorexia>())
		{
			effP = 0.01f;
		}
		List<Element> list = food.ListValidTraits(isCraft: true, limit: false);
		foreach (Element value in food.elements.dict.Values)
		{
			if (value.source.foodEffect.IsEmpty() || !list.Contains(value))
			{
				continue;
			}
			string[] foodEffect = value.source.foodEffect;
			int id = value.id;
			float num = effP * (float)value.Value;
			if (value.source.category == "food" && c.IsPC)
			{
				bool flag2 = num >= 0f;
				string text = value.source.GetText(flag2 ? "textInc" : "textDec", returnNull: true);
				if (text != null)
				{
					Msg.SetColor(flag2 ? "positive" : "negative");
					c.Say(text);
				}
			}
			switch (foodEffect[0])
			{
			case "god":
			{
				int int2 = c.GetInt(117);
				if (int2 < 10)
				{
					foreach (Element value2 in c.elements.dict.Values)
					{
						if (value2.IsMainAttribute)
						{
							c.elements.ModPotential(value2.id, 2);
						}
					}
				}
				c.Say("little_eat", c);
				c.PlaySound("ding_potential");
				c.elements.ModExp(306, -1000f);
				c.SetInt(117, int2 + 1);
				break;
			}
			case "exp":
			{
				id = ((foodEffect.Length > 1) ? EClass.sources.elements.alias[foodEffect[1]].id : value.id);
				int a = (int)(num * (float)((foodEffect.Length > 2) ? foodEffect[2].ToInt() : 4)) * 2 / 3;
				c.ModExp(id, a);
				break;
			}
			case "pot":
			{
				id = ((foodEffect.Length > 1) ? EClass.sources.elements.alias[foodEffect[1]].id : value.id);
				int vTempPotential = c.elements.GetElement(id).vTempPotential;
				int num2 = EClass.rndHalf((int)(num / 5f) + 1);
				num2 = num2 * 100 / Mathf.Max(100, vTempPotential * 2 / 3);
				c.elements.ModTempPotential(id, num2, 8);
				break;
			}
			case "karma":
				if (c.IsPCParty)
				{
					EClass.player.ModKarma(-5);
				}
				break;
			case "poison":
				ActEffect.Poison(c, EClass.pc, value.Value * 10);
				break;
			case "love":
				ActEffect.LoveMiracle(c, EClass.pc, value.Value * 10);
				break;
			case "loseWeight":
				c.ModWeight(-EClass.rndHalf(value.Value), ignoreLimit: true);
				break;
			case "gainWeight":
				c.ModWeight(EClass.rndHalf(value.Value), ignoreLimit: true);
				break;
			case "little":
			{
				int @int = c.GetInt(112);
				if (@int < 30)
				{
					c.Say("little_eat", c);
					c.PlaySound("ding_potential");
					int v = Mathf.Max(5 - @int / 2, 1);
					Debug.Log("sister eaten:" + @int + "/" + v);
					foreach (Element value3 in c.elements.dict.Values)
					{
						if (value3.IsMainAttribute)
						{
							c.elements.ModPotential(value3.id, v);
						}
					}
				}
				if (c.race.id == "mutant" && c.elements.Base(1230) < 10)
				{
					c.Say("little_adam", c);
					c.SetFeat(1230, c.elements.Base(1230) + 1);
				}
				c.SetInt(112, @int + 1);
				break;
			}
			}
		}
	}

	public static void ProcTrait(Chara c, Card food)
	{
		bool flag = false;
		foreach (Element value in food.elements.dict.Values)
		{
			if (!value.IsTrait)
			{
				continue;
			}
			if (value.Value >= 0)
			{
				switch (value.id)
				{
				case 753:
					c.CureCondition<ConPoison>(value.Value * 2);
					break;
				case 754:
					c.AddCondition<ConPeace>(value.Value * 5);
					break;
				case 755:
					c.CureCondition<ConBleed>(value.Value);
					break;
				case 756:
					c.AddCondition<ConHotspring>(value.Value * 2)?.SetPerfume();
					break;
				case 760:
					if (!c.HasCondition<ConAwakening>())
					{
						flag = true;
					}
					c.AddCondition<ConAwakening>(1000 + value.Value * 20);
					break;
				case 761:
					if (c.HasCondition<ConAwakening>() && !flag)
					{
						if (c.IsPC)
						{
							Msg.Say("recharge_stamina_fail");
						}
					}
					else
					{
						c.Say("recharge_stamina", c);
						c.stamina.Mod(c.stamina.max * (value.Value / 10 + 1) / 100 + value.Value * 2 / 3 + EClass.rnd(5));
					}
					break;
				}
			}
			else
			{
				switch (value.id)
				{
				case 753:
					SayTaste("food_poison");
					c.AddCondition<ConPoison>(-value.Value * 10);
					break;
				case 754:
					SayTaste("food_mind");
					c.AddCondition<ConConfuse>(-value.Value * 10);
					c.AddCondition<ConInsane>(-value.Value * 10);
					c.AddCondition<ConHallucination>(-value.Value * 20);
					break;
				case 755:
					c.AddCondition<ConBleed>(-value.Value * 10);
					break;
				case 756:
					c.hygiene.Mod(-value.Value * 5);
					break;
				case 760:
					c.RemoveCondition<ConAwakening>();
					c.sleepiness.Mod(value.Value);
					break;
				case 761:
					c.Say("recharge_stamina_negative", c);
					c.stamina.Mod(-c.stamina.max * (-value.Value / 10 + 1) / 100 + value.Value);
					break;
				}
			}
		}
		void SayTaste(string _id)
		{
			if (c.IsPC)
			{
				c.Say(_id);
			}
		}
	}
}
