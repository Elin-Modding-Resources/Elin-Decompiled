using System.Collections.Generic;
using UnityEngine;

public class FoodEffect : EClass
{
	public static bool IsLeftoverable(Thing food)
	{
		return food.trait is TraitLunch;
	}

	public static void Proc(Chara c, Thing food, bool consume = true)
	{
		Chara c2 = c;
		if (food.id == "bloodsample")
		{
			food.ModNum(-1);
			return;
		}
		food.CheckJustCooked();
		bool flag = EClass._zone.IsPCFaction && c2.IsInSpot<TraitSpotDining>();
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
		bool flag4 = c2.HasElement(1205);
		bool flag5 = food.IsDecayed || flag3;
		bool flag6 = IsLeftoverable(food);
		c2.AddFoodHistory(food);
		if (c2.IsPCFaction && !c2.IsPC)
		{
			int num6 = c2.CountNumEaten(food);
			bool flag7 = c2.GetFavFood().id == food.id;
			if (num6 < 2 || flag7)
			{
				if (num6 == 1 || flag7 || EClass.rnd(4) == 0)
				{
					c2.Talk("foodNice");
				}
			}
			else if (num6 > 3 && EClass.rnd(num6) >= 3)
			{
				c2.Talk("foodBored");
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
		if (c2.HasElement(1250))
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
		if (c2.HasElement(1200))
		{
			num2 *= 1f + (float)c2.Evalue(1200) * 0.25f;
		}
		if (!c2.IsPC)
		{
			num2 *= 3f;
		}
		if (flag5 && !c2.HasElement(480))
		{
			if (c2.IsPC)
			{
				if (flag3)
				{
					c2.Say("food_undead");
				}
				c2.Say("food_rot");
			}
			num5 = 0f;
			num3 /= 2;
		}
		else
		{
			switch (food.source._origin)
			{
			case "meat":
				if (c2.IsPC)
				{
					c2.Say("food_raw_meat");
				}
				num2 *= 0.7f;
				num5 = 0.5f;
				break;
			case "fish":
				if (c2.IsHuman)
				{
					if (c2.IsPC)
					{
						c2.Say("food_raw_fish");
					}
					num2 *= 0.9f;
					num5 = 0.5f;
				}
				break;
			case "dough":
				if (c2.IsPC)
				{
					c2.Say("food_raw_powder");
				}
				num2 *= 0.9f;
				num5 = 0.5f;
				break;
			}
		}
		float num7 = Mathf.Min(c2.hunger.value, num3);
		if (c2.hunger.GetPhase() >= 3)
		{
			num7 *= 1.1f;
		}
		if (flag5 && !c2.HasElement(480))
		{
			c2.ModExp(70, -300);
			c2.ModExp(71, -300);
			c2.ModExp(72, -200);
			c2.ModExp(73, -200);
			c2.ModExp(74, -200);
			c2.ModExp(75, 500);
			c2.ModExp(76, -200);
			c2.ModExp(77, -300);
		}
		else
		{
			num2 = num2 * num7 / 10f;
			if (c2.HasCondition<ConAnorexia>())
			{
				num2 = 0.01f;
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
				float num8 = num2 * (float)value.Value;
				if (value.source.category == "food" && c2.IsPC)
				{
					bool flag8 = num8 >= 0f;
					string text = value.source.GetText(flag8 ? "textInc" : "textDec", returnNull: true);
					if (text != null)
					{
						Msg.SetColor(flag8 ? "positive" : "negative");
						c2.Say(text);
					}
				}
				switch (foodEffect[0])
				{
				case "god":
				{
					int int2 = c2.GetInt(117);
					if (int2 < 10)
					{
						foreach (Element value2 in c2.elements.dict.Values)
						{
							if (value2.IsMainAttribute)
							{
								c2.elements.ModPotential(value2.id, 2);
							}
						}
					}
					c2.Say("little_eat", c2);
					c2.PlaySound("ding_potential");
					c2.elements.ModExp(306, -1000f);
					c2.SetInt(117, int2 + 1);
					flag6 = false;
					break;
				}
				case "exp":
				{
					id = ((foodEffect.Length > 1) ? EClass.sources.elements.alias[foodEffect[1]].id : value.id);
					int a = (int)(num8 * (float)((foodEffect.Length > 2) ? foodEffect[2].ToInt() : 4)) * 2 / 3;
					c2.ModExp(id, a);
					break;
				}
				case "pot":
				{
					id = ((foodEffect.Length > 1) ? EClass.sources.elements.alias[foodEffect[1]].id : value.id);
					int vTempPotential = c2.elements.GetElement(id).vTempPotential;
					int num9 = EClass.rndHalf((int)(num8 / 5f) + 1);
					num9 = num9 * 100 / Mathf.Max(100, vTempPotential * 2 / 3);
					c2.elements.ModTempPotential(id, num9, 8);
					break;
				}
				case "karma":
					if (c2.IsPCParty)
					{
						EClass.player.ModKarma(-5);
					}
					break;
				case "poison":
					ActEffect.Poison(c2, EClass.pc, value.Value * 10);
					if (!c2.isDead)
					{
						break;
					}
					goto IL_0f76;
				case "love":
					ActEffect.LoveMiracle(c2, EClass.pc, value.Value * 10);
					break;
				case "loseWeight":
					c2.ModWeight(-EClass.rndHalf(value.Value), ignoreLimit: true);
					break;
				case "gainWeight":
					c2.ModWeight(EClass.rndHalf(value.Value), ignoreLimit: true);
					break;
				case "little":
				{
					int @int = c2.GetInt(112);
					if (@int < 30)
					{
						c2.Say("little_eat", c2);
						c2.PlaySound("ding_potential");
						int v = Mathf.Max(5 - @int / 2, 1);
						Debug.Log("sister eaten:" + @int + "/" + v);
						foreach (Element value3 in c2.elements.dict.Values)
						{
							if (value3.IsMainAttribute)
							{
								c2.elements.ModPotential(value3.id, v);
							}
						}
					}
					if (c2.race.id == "mutant" && c2.elements.Base(1230) < 10)
					{
						c2.Say("little_adam", c2);
						c2.SetFeat(1230, c2.elements.Base(1230) + 1);
					}
					c2.SetInt(112, @int + 1);
					break;
				}
				}
			}
		}
		ProcTrait(c2, food);
		num4 += (float)food.Evalue(70);
		num4 += (float)(food.Evalue(72) / 2);
		num4 += (float)(food.Evalue(73) / 2);
		num4 += (float)(food.Evalue(75) / 2);
		num4 += (float)(food.Evalue(76) * 3 / 2);
		num4 += (float)food.Evalue(440);
		num4 += (float)(food.Evalue(445) / 2);
		num4 -= (float)food.Evalue(71);
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
			if (c2.IsPC)
			{
				c2.Say(idTaste);
				if (flag2)
				{
					c2.Say(flag4 ? "food_human_pos" : "food_human_neg");
				}
				else if (flag4)
				{
					c2.Say("food_human_whine");
				}
			}
		}
		if (LangGame.Has(idTaste + "2"))
		{
			c2.Say(idTaste + "2", c2, food);
		}
		if (!c2.IsPCParty)
		{
			num3 *= 2;
		}
		num3 = num3 * (100 + c2.Evalue(1235) * 10) / (100 + c2.Evalue(1234) * 10 + c2.Evalue(1236) * 15);
		c2.hunger.Mod(-num3);
		if (flag2)
		{
			if (!flag4)
			{
				if (c2.IsHuman)
				{
					c2.AddCondition<ConInsane>(200);
					c2.SAN.Mod(15);
				}
				if (EClass.rnd(c2.IsHuman ? 5 : 20) == 0)
				{
					c2.SetFeat(1205, 1, msg: true);
					flag4 = true;
				}
			}
			if (flag4)
			{
				c2.SetInt(31, EClass.world.date.GetRaw() + 10080);
			}
		}
		else if (flag4 && c2.GetInt(31) < EClass.world.date.GetRaw())
		{
			c2.SetFeat(1205, 0, msg: true);
		}
		if (flag5 && !c2.HasElement(480))
		{
			c2.AddCondition<ConParalyze>();
			c2.AddCondition<ConConfuse>(200);
		}
		if (c2.HasCondition<ConAnorexia>())
		{
			c2.Vomit();
		}
		if (num3 > 20 && c2.HasElement(1413))
		{
			Thing thing = ThingGen.Create("seed");
			if (EClass.rnd(EClass.debug.enable ? 2 : 10) == 0)
			{
				TraitSeed.ApplySeed(thing, (EClass.rnd(4) == 0) ? 118 : ((EClass.rnd(3) == 0) ? 119 : 90));
			}
			thing.SetNum(2 + EClass.rnd(3));
			c2.Talk("vomit");
			c2.Say("fairy_vomit", c2, thing);
			c2.PickOrDrop(c2.pos, thing);
		}
		food.trait.OnEat(c2);
		if (food.trait is TraitDrink)
		{
			food.trait.OnDrink(c2);
		}
		goto IL_0f76;
		IL_0f76:
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
		if (c2.IsCat || !(food.trait is TraitFoodChuryu))
		{
			return;
		}
		foreach (Chara item in c2.pos.ListCharasInRadius(c2, 5, (Chara c) => c.IsCat))
		{
			item.Say("angry", item);
			item.ShowEmo(Emo.angry);
			item.PlaySound("Animal/Cat/cat_angry");
			if (c2.IsPC)
			{
				EClass.player.ModKarma(-3);
			}
		}
	}

	public static void ProcTrait(Chara c, Card t)
	{
		bool flag = false;
		foreach (Element value in t.elements.dict.Values)
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
