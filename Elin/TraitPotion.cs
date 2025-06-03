public class TraitPotion : TraitDrink
{
	public override int Power => 200;

	public override int CraftNum
	{
		get
		{
			if (GetParam(1) == "HairGrowth")
			{
				return 3;
			}
			return base.CraftNum;
		}
	}

	public static void Create(Card owner, int ele)
	{
		owner.refVal = ele;
		if (EClass.sources.elements.map[ele].tag.Contains("noCopy"))
		{
			owner.elements.SetBase(759, 10);
		}
	}
}
