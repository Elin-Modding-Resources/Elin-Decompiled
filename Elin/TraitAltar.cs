using UnityEngine;

public class TraitAltar : Trait
{
	public override bool CanStack => false;

	public override bool IsAltar => true;

	public bool IsBranchAltar => this is TraitAltarAncient;

	public override bool CanOnlyCarry => IsBranchAltar;

	public virtual string idDeity => owner.c_idDeity.IsEmpty("eyth");

	public virtual Religion Deity => EClass.game.religions.Find(idDeity) ?? EClass.game.religions.Eyth;

	public string StrDeity => Deity.NameDomain;

	public bool IsEyth => idDeity == "eyth";

	public override void OnCreate(int lv)
	{
		SetDeity(GetParam(1) ?? EClass.game.religions.GetRandomReligion().id);
	}

	public override void OnImportMap()
	{
		if (owner.c_idDeity.IsEmpty() || GetParam(1) != null)
		{
			SetDeity(GetParam(1) ?? EClass.game.religions.GetRandomReligion().id);
		}
	}

	public void SetDeity(string id)
	{
		owner.c_idDeity = id;
		if (owner.id == "altar")
		{
			owner.ChangeMaterial(Deity.source.idMaterial);
		}
	}

	public override void SetName(ref string s)
	{
		if (!owner.c_idDeity.IsEmpty())
		{
			s = "_of".lang(StrDeity, s);
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (IsBranchAltar)
		{
			return;
		}
		if ((IsBranchAltar && EClass.Branch.rank != 0) || !IsBranchAltar)
		{
			p.TrySetAct("actOffer", delegate
			{
				LayerDragGrid.CreateOffering(this);
				return false;
			}, owner);
		}
		if (!IsBranchAltar && Deity != EClass.pc.faith && Deity.CanJoin)
		{
			p.TrySetAct("actWorship", delegate
			{
				LayerDrama.currentReligion = Deity;
				LayerDrama.Activate("_adv", "god", "worship");
				return false;
			}, owner);
		}
	}

	public override bool CanOffer(Card c)
	{
		if (c != null && c.HasTag(CTAG.godArtifact) && c.c_idDeity == Deity.id)
		{
			return true;
		}
		if (base.CanOffer(c) && (EClass.pc.faith.GetOfferingValue(c as Thing) > 0 || c.id == "water") && !c.isCopy)
		{
			return !c.HasElement(764);
		}
		return false;
	}

	public void OnOffer(Chara c, Thing t)
	{
		if (t == null)
		{
			return;
		}
		if (t.id == "water")
		{
			if (Deity != EClass.pc.faith)
			{
				if (t.blessedState == BlessedState.Cursed)
				{
					Msg.SayNothingHappen();
					return;
				}
				Msg.Say("waterCurse", t);
				EClass.pc.PlayEffect("curse");
				EClass.pc.PlaySound("curse3");
				t.SetBlessedState(BlessedState.Cursed);
			}
			else if (t.blessedState == BlessedState.Blessed)
			{
				Msg.SayNothingHappen();
			}
			else
			{
				Msg.Say("waterBless", t);
				EClass.pc.PlayEffect("revive");
				EClass.pc.PlaySound("revive");
				t.SetBlessedState(BlessedState.Blessed);
			}
			return;
		}
		if (!IsBranchAltar && EClass.pc.IsEyth)
		{
			EClass.pc.Say("god_offerEyth", owner, t);
			return;
		}
		EClass.pc.Say("god_offer", owner, t, Deity.Name);
		if (!CanOffer(t))
		{
			EClass.pc.Say("nothingHappens", owner, t);
			return;
		}
		Effect.Get("debuff").Play(owner.pos);
		EClass.pc.PlaySound("offering");
		if (IsBranchAltar)
		{
			Msg.Say("nothingHappens");
		}
		else if (IsEyth)
		{
			if (EClass.pc.IsEyth)
			{
				Msg.Say("nothingHappens");
			}
			else
			{
				Msg.Say("takeover_empty", EClass.pc.faith.Name);
				TakeOver();
				_OnOffer(c, t, 2);
			}
		}
		else
		{
			if (t.HasTag(CTAG.godArtifact) && t.c_idDeity == Deity.id)
			{
				_ = t.encLV;
				t.Destroy();
				Thing thing = Religion.Reforge(t.id);
				thing.SetEncLv(t.encLV);
				thing.SetBlessedState(t.blessedState);
				if (t.IsIdentified)
				{
					thing.Identify(show: true, IDTSource.SuperiorIdentify);
				}
				return;
			}
			if (EClass.pc.IsEyth)
			{
				Msg.Say("nothingHappens");
				return;
			}
			if (Deity != EClass.pc.faith)
			{
				bool flag = EClass.rnd(EClass.pc.faith.GetOfferingValue(t, t.Num)) > EClass.rnd(200);
				if (GetParam(1) != null)
				{
					Msg.Say("nothingHappens");
					return;
				}
				Msg.Say("takeover_versus", EClass.pc.faith.Name, Deity.Name);
				if (flag)
				{
					Msg.Say("takeover_success", EClass.pc.faith.TextGodGender);
					Msg.Say("takeover_success2", EClass.pc.faith.Name);
					TakeOver();
					_OnOffer(c, t, 5);
				}
				else
				{
					Msg.Say("takeover_fail", Deity.Name);
					Deity.PunishTakeOver(EClass.pc);
				}
			}
			else
			{
				_OnOffer(c, t);
			}
		}
		t.Destroy();
	}

	public void _OnOffer(Chara c, Thing t, int takeoverMod = 0)
	{
		bool @bool = t.GetBool(115);
		int offeringValue = Deity.GetOfferingValue(t, t.Num);
		offeringValue = offeringValue * (c.HasElement(1228) ? 130 : 100) / 100;
		if (takeoverMod == 0)
		{
			if (offeringValue >= 200)
			{
				Msg.Say("god_offer1", t);
				EClass.pc.faith.Talk("offer");
			}
			else if (offeringValue >= 100)
			{
				Msg.Say("god_offer2", t);
			}
			else if (offeringValue >= 50)
			{
				Msg.Say("god_offer3", t);
			}
			else
			{
				Msg.Say("god_offer4", t);
			}
		}
		else
		{
			Msg.Say("god_offer1", t);
			offeringValue += Deity.GetOfferingValue(t, 1) * takeoverMod;
		}
		int num = Mathf.Max(c.Evalue(306), 1);
		Element orCreateElement = c.elements.GetOrCreateElement(85);
		int value = orCreateElement.Value;
		c.elements.ModExp(orCreateElement.id, offeringValue * 2 / 3);
		int num2 = 4;
		if (orCreateElement.vBase >= num)
		{
			c.elements.SetBase(orCreateElement.id, num);
		}
		else
		{
			num2 = Mathf.Clamp(orCreateElement.vBase * 100 / num / 25, 0, 3);
		}
		if (num2 == 4 || orCreateElement.Value != value)
		{
			Msg.Say("piety" + num2, c, c.faith.TextGodGender);
		}
		Debug.Log(offeringValue + "/" + orCreateElement.Value + "/" + orCreateElement.vExp);
		if (orCreateElement.Value > num * 8 / 10)
		{
			c.elements.ModExp(306, offeringValue / 5);
		}
		c.RefreshFaithElement();
		if (@bool)
		{
			EClass.player.ModKarma(-1);
		}
	}

	public void TakeOver()
	{
		SetDeity(EClass.pc.faith.id);
		EClass.pc.faith.Talk("takeover");
		EClass.pc.PlayEffect("revive");
		owner.PlayEffect("aura_heaven");
	}
}
