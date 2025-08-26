public class TraitStrangeGirl : TraitUniqueChara
{
	public override ShopType ShopType
	{
		get
		{
			if (!(EClass._zone is Zone_LittleGarden) && !EClass.game.IsSurvival)
			{
				return ShopType.None;
			}
			return ShopType.StrangeGirl;
		}
	}

	public override CurrencyType CurrencyType => CurrencyType.Influence;
}
