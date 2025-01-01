using System.Linq;

public class TraitDeliveryChest : TraitContainer
{
	public override int GuidePriotiy => 2;

	public override bool IsSpecialContainer => true;

	public override bool ShowOpenActAsCrime => false;

	public override bool CanOpenContainer
	{
		get
		{
			if (owner.IsInstalled)
			{
				if (!EClass._zone.IsPCFaction && !EClass._zone.IsTown && !(EClass._zone is Zone_MerchantGuild))
				{
					return EClass._zone is Zone_Casino;
				}
				return true;
			}
			return false;
		}
	}

	public override bool ShowChildrenNumber => false;

	public override bool UseAltTiles => EClass.game.cards.container_deliver.things.Count > 0;

	public override bool CanBeShipped => false;

	public override void Prespawn(int lv)
	{
	}

	public override void TrySetAct(ActPlan p)
	{
		base.TrySetAct(p);
		if (owner.things.Count <= 0)
		{
			return;
		}
		p.TrySetAct("Eject", delegate
		{
			foreach (Thing item in owner.things.ToList())
			{
				EClass.pc.PickOrDrop(EClass.pc.pos, item);
			}
			return false;
		});
	}
}
