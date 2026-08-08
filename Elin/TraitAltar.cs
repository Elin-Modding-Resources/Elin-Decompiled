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

	public bool IsEyth => Deity.IsEyth;

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

	public override bool CanOffer(Chara cc, Card c)
	{
		if (c != null && c.HasTag(CTAG.godArtifact))
		{
			if (cc.IsEyth && cc.HasElement(1228))
			{
				if (IsEyth || Deity.IsValidArtifact(c.id))
				{
					return true;
				}
			}
			else if (c.c_idDeity == Deity.id)
			{
				return true;
			}
		}
		if (base.CanOffer(cc, c) && (cc.faith.GetOfferingValue(c as Thing) > 0 || c.id == "water") && !c.isCopy)
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
			if (Deity != c.faith)
			{
				if (t.blessedState == BlessedState.Cursed)
				{
					Msg.SayNothingHappen();
					return;
				}
				Msg.Say("waterCurse", t);
				c.PlayEffect("curse");
				c.PlaySound("curse3");
				t.SetBlessedState(BlessedState.Cursed);
			}
			else if (t.blessedState == BlessedState.Blessed)
			{
				Msg.SayNothingHappen();
			}
			else
			{
				Msg.Say("waterBless", t);
				c.PlayEffect("revive");
				c.PlaySound("revive");
				t.SetBlessedState(BlessedState.Blessed);
			}
			return;
		}
		if (t.HasElement(766) && !Deity.IsEyth)
		{
			Deity.PunishTakeOver(c);
			return;
		}
		if (!IsBranchAltar && c.IsEyth && !c.HasElement(1228))
		{
			c.Say("god_offerEyth", owner, t);
			return;
		}
		c.Say("god_offer", owner, t, Deity.Name);
		if (!CanOffer(c, t))
		{
			c.Say("nothingHappens", owner, t);
			return;
		}
		Effect.Get("debuff").Play(owner.pos);
		c.PlaySound("offering");
		if (IsBranchAltar)
		{
			Msg.Say("nothingHappens");
		}
		else if (IsEyth && !c.HasElement(1228))
		{
			if (c.IsEyth)
			{
				Msg.Say("nothingHappens");
			}
			else
			{
				Msg.Say("takeover_empty", c.faith.Name);
				TakeOver(c);
				_OnOffer(c, t, 2);
			}
		}
		else
		{
			if (t.HasTag(CTAG.godArtifact) && (t.c_idDeity == Deity.id || (c.IsEyth && c.HasElement(1228))))
			{
				_ = t.encLV;
				t.Destroy();
				Thing thing = EClass.game.religions.Reforge(t.id);
				if (c.IsEyth && c.HasElement(1228) && IsEyth)
				{
					thing.c_idDeity = EClass.game.religions.Eyth.id;
				}
				thing.SetEncLv(t.encLV);
				thing.SetBlessedState(t.blessedState);
				if (t.IsIdentified)
				{
					thing.Identify(show: true, IDTSource.SuperiorIdentify);
				}
				return;
			}
			if (c.IsEyth && !c.HasElement(1228))
			{
				Msg.Say("nothingHappens");
				return;
			}
			if (Deity.id != c.faith.id)
			{
				bool flag = EClass.rnd(c.faith.GetOfferingValue(t, t.Num)) > EClass.rnd(200) || IsEyth;
				if (GetParam(1) != null || c.IsEyth)
				{
					Msg.Say("nothingHappens");
					return;
				}
				Msg.Say("takeover_versus", c.faith.Name, Deity.Name);
				if (flag)
				{
					Msg.Say("takeover_success", c.faith.TextGodGender);
					Msg.Say("takeover_success2", c.faith.Name);
					TakeOver(c);
					_OnOffer(c, t, 5);
				}
				else
				{
					Msg.Say("takeover_fail", Deity.Name);
					Deity.PunishTakeOver(c);
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
		offeringValue = offeringValue * (EClass.debug.enable ? 1000 : (c.HasElement(1228) ? 130 : 100)) / 100;
		if (takeoverMod == 0)
		{
			if (offeringValue >= 200)
			{
				Msg.Say("god_offer1", t);
				c.faith.Talk("offer");
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
		if (orCreateElement.vBase < num)
		{
			c.elements.ModExp(orCreateElement.id, offeringValue * 2 / 3);
			if (orCreateElement.vBase >= num)
			{
				c.elements.SetBase(orCreateElement.id, num);
			}
		}
		int num2 = 4;
		if (orCreateElement.vBase < num)
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
		if (c.faith.GetGiftRank() != -1)
		{
			c.faith.Talk("like");
		}
		if (@bool && c.IsPC)
		{
			EClass.player.ModKarma(-1);
		}
	}

	public void TakeOver(Chara c)
	{
		SetDeity(c.faith.id);
		c.faith.Talk("takeover");
		c.PlayEffect("revive");
		owner.PlayEffect("aura_heaven");
	}
}
