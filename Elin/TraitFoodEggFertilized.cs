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
		chara.SetFeat(1232, (incubator != null) ? 3 : 2, msg: true);
		chara.things.DestroyAll();
		if (chara.id == "putty_snow" && chara.idSkin == 0)
		{
			chara.idSkin = 4 + EClass.rnd(2);
		}
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

	public override bool CanStackTo(Thing to)
	{
		if (to.c_idMainElement != owner.c_idMainElement)
		{
			return false;
		}
		return base.CanStackTo(to);
	}
}
