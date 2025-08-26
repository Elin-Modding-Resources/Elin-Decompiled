using UnityEngine;

public class TraitDemitas : TraitUniqueChara
{
	public override int CostRerollShop => 0;

	public bool CanSpellwrite
	{
		get
		{
			if (!EClass.debug.enable && !EClass.game.quests.IsCompleted("demitas_spellwriter"))
			{
				return EClass.game.IsSurvival;
			}
			return true;
		}
	}

	public override bool CanInvest => CanSpellwrite;

	public override CopyShopType CopyShop
	{
		get
		{
			if (!CanSpellwrite)
			{
				return CopyShopType.None;
			}
			return CopyShopType.Spellbook;
		}
	}

	public override ShopType ShopType
	{
		get
		{
			if (!CanSpellwrite)
			{
				return ShopType.None;
			}
			return ShopType.Copy;
		}
	}

	public override int NumCopyItem => 3 + Mathf.Min(base.owner.c_invest / 5, 7);

	public override bool CanJoinParty => false;

	public override bool CanBeBanished => false;

	public override int RestockDay => 28;

	public override bool CanCopy(Thing t)
	{
		if (t.HasElementNoCopy())
		{
			return false;
		}
		return t.trait is TraitSpellbook;
	}
}
