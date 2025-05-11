using System.Collections.Generic;
using UnityEngine;

public class TraitDrinkMilkMother : TraitDrinkMilk
{
	public override bool HoldAsDefaultInteraction => true;

	public override Emo2 GetHeldEmo(Chara c)
	{
		if (c.Evalue(1232) > 0)
		{
			return Emo2.baby;
		}
		return Emo2.none;
	}

	public override void OnDrink(Chara c)
	{
		if (c.Evalue(1232) > 0)
		{
			c.SetFeat(1232, c.Evalue(1232) - 1);
			int uidNext = EClass.game.cards.uidNext;
			EClass.game.cards.uidNext = 1;
			Rand.SetSeed(1);
			Chara chara = CharaGen.Create(owner.c_idRefCard);
			chara.SetLv(Mathf.Clamp(5 + owner.encLV * 5, 1, 20 + EClass.pc.Evalue(237)));
			Rand.SetSeed();
			EClass.game.cards.uidNext = uidNext;
			Debug.Log(chara.id + "/" + chara.LV);
			List<Element> list = chara.elements.ListBestAttributes();
			List<Element> list2 = chara.elements.ListBestSkills();
			int num = 100;
			int num2 = 0;
			foreach (Element item in list)
			{
				Element element = c.elements.GetElement(item.id);
				int num3 = item.ValueWithoutLink * (element.Potential - element.vTempPotential) / num / 2;
				if (num3 > 0)
				{
					num2 += num3;
					Debug.Log(item.source.alias + "/" + num3 + " org:" + item.ValueWithoutLink);
					c.elements.ModBase(item.id, num3);
				}
				num += 50;
			}
			num = 100;
			foreach (Element item2 in list2)
			{
				Element element2 = c.elements.GetElement(item2.id);
				if (element2 != null && element2.ValueWithoutLink != 0)
				{
					int num4 = item2.ValueWithoutLink * (element2.Potential - element2.vTempPotential) / num / 2;
					if (num4 > 0)
					{
						Debug.Log(item2.source.alias + "/" + num4 + " org:" + item2.ValueWithoutLink);
						c.elements.ModBase(item2.id, num4);
					}
					num += 50;
				}
			}
			c.feat += owner.encLV;
			if (c.Evalue(1232) == 0)
			{
				c.Say("grow_adult", c);
				c.PlaySound("mutation");
				c.PlayEffect("mutation");
				switch (c.id)
				{
				case "putty_snow":
					if (c.idSkin >= 4)
					{
						c.idSkin = (c.idSkin - 4) / 2;
					}
					break;
				case "putty_snow_gold":
					if (c.idSkin >= 1)
					{
						c.idSkin = 0;
					}
					break;
				case "chicken":
				case "duck":
				case "shamo":
				case "penguin":
				case "penguin_emperor":
					c.idSkin = 0;
					break;
				}
			}
			else
			{
				c.Say("grow_baby", c);
				c.PlaySound("ding_potential");
			}
			c.PlayEffect("buff");
			EClass.pc.ModExp(237, 100);
		}
		else
		{
			base.OnDrink(c);
		}
	}
}
