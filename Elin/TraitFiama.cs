public class TraitFiama : TraitUniqueMerchant
{
	public override bool CanInvite => EClass._zone.id == "lothria";

	public override ShopType ShopType => ShopType.Starter;

	public override CurrencyType CurrencyType => CurrencyType.Money2;

	public override string LangBarter => "daBuyStarter";

	public override bool ShouldShowQuestIcon()
	{
		if (EClass.game.IsSurvival)
		{
			return false;
		}
		if (EClass.game.quests.GetPhase<QuestMain>() >= 200 && EClass.player.dialogFlags.TryGetValue("fiama1", 0) == 0)
		{
			return true;
		}
		if (EClass.pc.homeBranch != null)
		{
			foreach (Chara member in EClass.pc.homeBranch.members)
			{
				if (member.isDead && member.GetInt(100) != 0)
				{
					return true;
				}
			}
		}
		return false;
	}
}
