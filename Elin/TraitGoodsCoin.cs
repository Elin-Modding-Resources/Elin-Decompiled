public class TraitGoodsCoin : TraitItem
{
	public override bool OnUse(Chara c)
	{
		EClass.player.ModKeyItem("lucky_coin");
		owner.ModNum(-1);
		return true;
	}
}
