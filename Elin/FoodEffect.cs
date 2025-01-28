using System.Collections.Generic;
using UnityEngine;

public class FoodEffect : EClass
{
	public static bool IsHumanFlesh(Thing food)
	{
		if (food.HasTag(CTAG.notHumanMeat))
		{
			return false;
		}
		if (food.id == "deadbody")
		{
			return true;
		}
		if (food.source._origin != "meat" && food.source._origin != "dish")
		{
			return false;
		}
		string[] components = food.source.components;
		for (int i = 0; i < components.Length; i++)
		{
			if (components[i].Contains("egg"))
			{
				return false;
			}
		}
		if (!IsHumanFlesh(food.refCard))
		{
			return IsHumanFlesh(food.refCard2);
		}
		return true;
	}

	public static bool IsUndeadFlesh(Thing food)
	{
		if (food.source._origin != "meat" && food.source._origin != "dish")
		{
			return false;
		}
		if (!IsUndeadFlesh(food.refCard))
		{
			return IsUndeadFlesh(food.refCard2);
		}
		return true;
	}

	public static bool IsHumanFlesh(CardRow r)
	{
		if (r == null)
		{
			return false;
		}
		if (r.id == "chara")
		{
			return EClass.pc.race.tag.Contains("human");
		}
		if (r.isChara)
		{
			return EClass.sources.races.map[EClass.sources.charas.map[r.id].race].tag.Contains("human");
		}
		return false;
	}

	public static bool IsUndeadFlesh(CardRow r)
	{
		if (r == null)
		{
			return false;
		}
		if (r.id == "chara")
		{
			return EClass.pc.race.tag.Contains("undead");
		}
		if (r.isChara)
		{
			return EClass.sources.races.map[EClass.sources.charas.map[r.id].race].tag.Contains("undead");
		}
		return false;
	}

	public static void Proc(Chara c, Thing food)
	{
		food.CheckJustCooked();
		bool flag = EClass._zone.IsPCFaction && c.IsInSpot<TraitSpotDining>();
		int num = (food.isCrafted ? ((EClass.pc.Evalue(1650) >= 3) ? 5 : 0) : 0);
		float num2 = (float)(100 + (food.HasElement(757) ? 10 : 0) + (flag ? 10 : 0) + num + Mathf.Min(food.QualityLv * 10, 100)) / 200f;
		if (num2 < 0.1f)
		{
			num2 = 0.1f;
		}
		int num3 = food.Evalue(10);
		float num4 = 25f;
		float num5 = 1f;
		string idTaste = "";
		bool flag2 = IsHumanFlesh(food);
		bool flag3 = IsUndeadFlesh(food);
		bool flag4 = c.HasElement(1205);
		bool flag5 = food.IsDecayed || flag3;
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
				num2 *= 1.5f;
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
		if (c.HasElement(1200))
		{
			num2 *= 1.25f;
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
		float num6 = Mathf.Min(c.hunger.value, num3);
		if (c.hunger.GetPhase() >= 3)
		{
			num6 *= 1.1f;
		}
		if (flag5 && !c.HasElement(480))
		{
			c.ModExp(70, -300);
			c.ModExp(71, -300);
			c.ModExp(72, -200);
			c.ModExp(73, -200);
			c.ModExp(74, -200);
			c.ModExp(75, 500);
			c.ModExp(76, -200);
			c.ModExp(77, -300);
		}
		else
		{
			num2 = num2 * num6 / 10f;
			if (c.HasCondition<ConAnorexia>())
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
				float num7 = num2 * (float)value.Value;
				if (value.source.category == "food" && c.IsPC)
				{
					bool flag6 = num7 >= 0f;
					string text = value.source.GetText(flag6 ? "textInc" : "textDec", returnNull: true);
					if (text != null)
					{
						Msg.SetColor(flag6 ? "positive" : "negative");
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
					c.elements.ModExp(306, -1000);
					c.SetInt(117, int2 + 1);
					break;
				}
				case "exp":
				{
					id = ((foodEffect.Length > 1) ? EClass.sources.elements.alias[foodEffect[1]].id : value.id);
					int a = (int)(num7 * (float)((foodEffect.Length > 2) ? foodEffect[2].ToInt() : 4)) * 2 / 3;
					c.ModExp(id, a);
					break;
				}
				case "pot":
				{
					id = ((foodEffect.Length > 1) ? EClass.sources.elements.alias[foodEffect[1]].id : value.id);
					int vTempPotential = c.elements.GetElement(id).vTempPotential;
					int num8 = EClass.rndHalf((int)(num7 / 5f) + 1);
					num8 = num8 * 100 / Mathf.Max(100, vTempPotential * 2 / 3);
					c.elements.ModTempPotential(id, num8, 8);
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
					if (c.isDead)
					{
						return;
					}
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
					if (@int >= 30)
					{
						break;
					}
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
		ProcTrait(c, food);
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
		if (food.trait is TraitDrink)
		{
			food.trait.OnDrink(c);
		}
	}

	public static void ProcTrait(Chara c, Card t)
	{
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
