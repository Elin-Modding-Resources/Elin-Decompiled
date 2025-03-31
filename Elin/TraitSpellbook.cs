public class TraitSpellbook : TraitBaseSpellbook
{
	public override int Difficulty => 10 + source.LV;

	public static void Create(Card owner, int ele)
	{
		owner.refVal = ele;
		SourceElement.Row row = EClass.sources.elements.map[ele];
		if (row.tag.Contains("noCopy"))
		{
			owner.elements.SetBase(759, 10);
		}
		if (row.charge == 1)
		{
			owner.c_charges = 1;
		}
	}
}
