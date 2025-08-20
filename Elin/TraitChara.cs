using UnityEngine;

public class TraitChara : Trait
{
	public enum Adv_Type
	{
		None,
		Adv,
		Adv_Fairy,
		Adv_Backer
	}

	public static string[] ListRank = new string[7] { "E", "D", "C", "B", "A", "S", "S" };

	public new Chara owner => base.owner.Chara;

	public virtual AI_Idle.Behaviour IdleBehaviour => AI_Idle.Behaviour.Default;

	public virtual bool CanAutoRevive
	{
		get
		{
			if (owner.isImported && !owner.isDestroyed)
			{
				return !owner.IsGlobal;
			}
			return false;
		}
	}

	public virtual bool IsCitizen => false;

	public virtual bool IsUnique => false;

	public virtual bool CanRevive => false;

	public virtual bool CanGuide => false;

	public virtual bool CanIdentify => false;

	public virtual bool CanPicklock => false;

	public virtual bool CanInvest => false;

	public virtual string IDTrainer => "";

	public virtual bool CanJoinParty => true;

	public virtual bool CanJoinPartyResident
	{
		get
		{
			if (owner.GetBool(18))
			{
				return EClass.pc.CHA >= owner.GetBestAttribute();
			}
			return true;
		}
	}

	public virtual bool CanSellStolenGoods => false;

	public virtual bool CanBeBanished => true;

	public virtual bool CanInvite => !owner.source.multisize;

	public virtual bool IsCountAsResident => !owner.IsUnique;

	public virtual bool CanInvestTown => false;

	public virtual bool CanSellPlan
	{
		get
		{
			if (owner.IsMaid)
			{
				return EClass.BranchOrHomeBranch != null;
			}
			return false;
		}
	}

	public virtual bool CanHeal => false;

	public virtual bool CanWhore => false;

	public virtual bool CanServeFood => false;

	public virtual bool HaveNews => false;

	public virtual bool CanBout => false;

	public virtual bool UseGlobalGoal => false;

	public virtual bool ShowAdvRank => owner.IsPC;

	public virtual int MaxRandomAbility => 0;

	public virtual Adv_Type AdvType => Adv_Type.None;

	public virtual bool EnableTone
	{
		get
		{
			if (IsUnique)
			{
				return !LayerDrama.Instance;
			}
			return true;
		}
	}

	public virtual bool CanBeTamed
	{
		get
		{
			if (!IsUnique && owner.rarity < Rarity.Legendary && !owner.IsMultisize && !owner.IsGlobal && !owner.IsPCFaction && EClass._zone.Boss != owner && CanInvite)
			{
				return owner.c_bossType == BossType.none;
			}
			return false;
		}
	}

	public virtual bool CanBePushed => !owner.source.multisize;

	public virtual bool CanGiveRandomQuest
	{
		get
		{
			if (owner.IsPCFaction || (IsCitizen && !IsUnique))
			{
				return owner.quest == null;
			}
			return false;
		}
	}

	public virtual bool UseRandomAlias => false;

	public virtual bool IsWearingPanty => EClass.pc.faction.IsWearingPanty(owner);

	public override string IDInvStyle => "backpack";

	public virtual string IDRumor => "";

	public virtual string IdAmbience => null;

	public virtual bool CanFish => owner.job.id == "tourist";

	public virtual bool IdleAct()
	{
		return false;
	}

	public virtual string GetDramaText()
	{
		if (CanInvest)
		{
			return "dramaText_shop".lang((owner.c_invest + 1).ToString() ?? "");
		}
		return "";
	}

	public override int GetValue()
	{
		return owner.LV * 500 + 3000;
	}

	public int GetAdvRank()
	{
		int min = 0;
		switch (owner.id)
		{
		case "adv_gaki":
		case "adv_yukiimo":
		case "adv_kiria":
		case "adv_ivory":
			min = 4;
			break;
		}
		return Mathf.Clamp(owner.LV / 10, min, 6);
	}

	public string GetAdvRankText()
	{
		int advRank = GetAdvRank();
		return ListRank[advRank];
	}
}
