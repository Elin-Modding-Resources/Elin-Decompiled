using UnityEngine;

public class TraitFoodEggFertilized : TraitFoodEgg
{
	public override int DecaySpeed => 1;

	public static Chara Incubate(Thing egg, Point pos, Card incubator = null)
	{
		egg.SetSale(sale: false);
		string str = "";
		CardRow refCard = egg.refCard;
		if (refCard != null)
		{
			str = refCard.id;
			if (refCard.id == "chara" || refCard.quality == 4)
			{
				str = "";
			}
		}
		if (egg.IsDecayed)
		{
			str = "zombie";
		}
		Chara chara = CharaGen.Create(str.IsEmpty("chicken"));
		EClass._zone.AddCard(chara, pos.GetNearestPoint(allowBlock: false, allowChara: false) ?? EClass.pc.pos);
		chara.SetLv(1);
		chara.SetMainElement(egg.c_idMainElement, 10, elemental: true);
		chara.things.DestroyAll();
		MakeBaby(chara, (incubator == null) ? 2 : 3);
		if (chara.Evalue(1644) > 0)
		{
			for (int i = 0; i < chara.Evalue(1644); i++)
			{
				chara.RemoveLastBodyPart();
				Debug.Log(i + "/" + chara.body.slots.Count);
			}
			chara.elements.SetBase(1644, 0);
		}
		foreach (Element value in chara.elements.dict.Values)
		{
			if ((!(value.source.category != "attribute") || !(value.source.category != "skill")) && (!(value.source.category == "attribute") || value.source.tag.Contains("primary")) && value.ValueWithoutLink != 0)
			{
				value.vTempPotential = value.vTempPotential * 2 + 100;
				value.vPotential += 30;
			}
		}
		if (!egg.isNPCProperty)
		{
			FactionBranch factionBranch = EClass.Branch ?? EClass.pc.homeBranch;
			if (factionBranch != null)
			{
				factionBranch.AddMemeber(chara);
				factionBranch.ChangeMemberType(chara, EClass._zone.IsPCFaction ? FactionMemberType.Livestock : FactionMemberType.Default);
				if (!EClass._zone.IsPCFaction)
				{
					EClass.pc.party.AddMemeber(chara);
				}
			}
		}
		Msg.Say("incubate", chara);
		return chara;
	}

	public static void MakeBaby(Chara c, int baby)
	{
		c.SetFeat(1232, baby, msg: true);
		switch (c.id)
		{
		case "putty_snow":
			if (c.idSkin <= 3)
			{
				c.idSkin = 4 + c.idSkin * 2 + EClass.rnd(2);
			}
			break;
		case "putty_snow_gold":
			if (c.idSkin == 0)
			{
				c.idSkin = 1 + EClass.rnd(2);
			}
			break;
		case "shamo":
		case "chicken":
		case "duck":
			c.idSkin = 1;
			break;
		}
	}

	public override bool CanStackTo(Thing to)
	{
		if (to.c_idMainElement != owner.c_idMainElement)
		{
			return false;
		}
		return base.CanStackTo(to);
	}
}
