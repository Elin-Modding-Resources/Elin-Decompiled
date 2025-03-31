public class TraitPotion : TraitDrink
{
	public static void Create(Card owner, int ele)
	{
		owner.refVal = ele;
		if (EClass.sources.elements.map[ele].tag.Contains("noCopy"))
		{
			owner.elements.SetBase(759, 10);
		}
	}
}
