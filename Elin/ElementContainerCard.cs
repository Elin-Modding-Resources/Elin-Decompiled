using UnityEngine;

public class ElementContainerCard : ElementContainer
{
	public Card owner;

	public override Card Card => owner;

	public override Chara Chara => owner.Chara;

	public override bool IsMeleeWeapon
	{
		get
		{
			if (Card.category.slot == 35)
			{
				return Card.category.skill != 0;
			}
			return false;
		}
	}

	public void SetOwner(Card c, bool applyFeat)
	{
		owner = c;
		SourceValueType sourceValueType = ((c.IsEquipmentOrRangedOrAmmo && c.rarity < Rarity.Artifact) ? SourceValueType.EquipmentRandom : SourceValueType.Fixed);
		ApplyElementMap(c.uid, sourceValueType, c.sourceCard.elementMap, (sourceValueType != SourceValueType.EquipmentRandom) ? 1 : c.LV, invert: false, applyFeat);
	}

	public override void OnLearn(int ele)
	{
		if (owner.IsPCFaction)
		{
			SE.DingSkill2();
		}
		Msg.Say("learnSkill", Card, EClass.sources.elements.map[ele].GetName());
		CheckSkillActions();
	}

	public void CheckSkillActions()
	{
		if (owner.IsPC)
		{
			TryLearn(6011, 281, 0);
			TryLearn(6018, 226, 0);
			TryLearn(6019, 227, 0);
			TryLearn(6700, 1649, 0);
			TryLearn(6720, 1657, 0);
			TryLearn(6450, 132, 5);
		}
		void TryLearn(int eleAction, int reqEle, int lqlv)
		{
			if (!HasBase(eleAction) && HasBase(reqEle) && GetElement(reqEle).ValueWithoutLink >= lqlv)
			{
				SetBase(eleAction, 1);
				if (EClass.core.IsGameStarted)
				{
					if (owner.IsPC)
					{
						LayerAbility.Redraw();
					}
					owner.Say("learnSkill", Card, EClass.sources.elements.map[eleAction].GetName());
				}
			}
		}
	}

	public override void OnTrain(int ele)
	{
		Msg.Say("trainSkill", Card, EClass.sources.elements.map[ele].GetName());
	}

	public override void OnModTempPotential(Element e, int v, int threshMsg)
	{
		if (threshMsg == 0 || Mathf.Abs(v) >= threshMsg)
		{
			string lang = ((v > 0) ? "potentialInc" : "potentialDec");
			if (owner.IsPCFaction && v > 0)
			{
				owner.PlaySound("ding_potential");
			}
			Msg.SetColor((v > 0) ? "positive" : "negative");
			owner.Say(lang, owner, e.Name.ToLower());
		}
	}

	public override void OnLevelUp(Element e, int lastValue)
	{
		if (!e.ShowMsgOnValueChanged)
		{
			return;
		}
		if (owner.IsPC)
		{
			if (e.id == 287)
			{
				EClass.player.flags.canComupWithFoodRecipe = true;
			}
			SE.DingSkill2();
		}
		if (owner.isChara)
		{
			if (owner.Chara.IsPCFaction)
			{
				if (!VirtualDate.IsActive)
				{
					if (owner.Chara.IsInActiveZone)
					{
						Msg.SetColor(Msg.colors.Ding);
						string text = e.source.GetText("textInc", returnNull: true);
						if (!text.IsEmpty())
						{
							owner.Say(text, owner);
						}
						else
						{
							owner.Say("ding_skill", owner, e.Name);
						}
						owner.pos.TalkWitnesses(owner.Chara, "ding_other", 4, WitnessType.ally, null, 4 + EClass.pc.party.members.Count);
					}
					if (owner.IsPCParty)
					{
						WidgetPopText.Say("popDing".lang(owner.IsPC ? "" : owner.Name, e.Name, lastValue.ToString() ?? "", e.ValueWithoutLink.ToString() ?? ""), FontColor.Good);
					}
				}
				if (owner.Chara.homeBranch != null)
				{
					owner.Chara.homeBranch.Log("bDing", Card, e.Name, lastValue.ToString() ?? "", e.ValueWithoutLink.ToString() ?? "");
				}
			}
			if (e is Skill)
			{
				owner.AddExp(10 + EClass.rnd(5));
			}
			owner.Chara.CalculateMaxStamina();
		}
		CheckSkillActions();
	}

	public override void OnLevelDown(Element e, int lastValue)
	{
		if (!e.ShowMsgOnValueChanged)
		{
			return;
		}
		_ = owner.IsPC;
		if (!owner.isChara)
		{
			return;
		}
		if (owner.Chara.IsPCFaction)
		{
			Msg.SetColor(Msg.colors.Negative);
			string text = e.source.GetText("textDec", returnNull: true);
			if (!text.IsEmpty())
			{
				Msg.Say(text, owner);
			}
			else
			{
				Msg.Say("dec_skill", owner, e.Name);
			}
		}
		owner.Chara.CalculateMaxStamina();
	}

	public override int ValueBonus(Element e)
	{
		if (EClass.game == null)
		{
			return 0;
		}
		int num = 0;
		if (owner.IsPCFactionOrMinion)
		{
			Element element = EClass.pc.faction.charaElements.GetElement(e.id);
			if (element != null)
			{
				num += element.Value;
			}
			if (owner.IsPCParty)
			{
				int id = e.id;
				if (id == 70 || (uint)(id - 72) <= 1u)
				{
					int num2 = 0;
					foreach (Chara member in EClass.pc.party.members)
					{
						if (member.Evalue(1419) > 0)
						{
							num2 += member.Evalue(1419);
						}
					}
					if (num2 > 0)
					{
						int num3 = 0;
						foreach (Chara chara in EClass._map.charas)
						{
							if (chara.IsHostile(EClass.pc))
							{
								num3++;
							}
						}
						if (num3 > 0)
						{
							num += Mathf.Max(1, (e.ValueWithoutLink + e.vLink) * (int)Mathf.Clamp(4f + Mathf.Sqrt(num3) * (float)num2 * 2f, 5f, 30f) / 100);
						}
					}
				}
			}
			if (e.id == 78)
			{
				num += EClass.player.CountKeyItem("lucky_coin") * 2;
				if (Chara != null && Chara.Evalue(663) > 0)
				{
					num = num * 2 + (e.ValueWithoutLink + e.vLink);
				}
			}
			if (e.id != 664 && (owner.Chara.race.IsMachine || owner.id == "android"))
			{
				int num4 = owner.Evalue(664);
				if (num4 > 0)
				{
					switch (e.id)
					{
					case 64:
					case 65:
						num += (e.ValueWithoutLink + e.vLink) * num4 / 2 / 100;
						break;
					case 79:
						num += (e.ValueWithoutLink + e.vLink) * num4 / 100;
						break;
					}
				}
			}
		}
		if (!e.source.aliasMtp.IsEmpty())
		{
			int num5 = owner.Evalue(e.source.aliasMtp);
			if (num5 != 0)
			{
				num += (e.ValueWithoutLink + e.vLink) * num5 / 100;
			}
		}
		return num;
	}
}
