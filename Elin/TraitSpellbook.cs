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
		switch (row.id)
		{
		case 8430:
			owner.elements.SetBase(765, 50);
			break;
		case 9155:
			owner.elements.SetBase(765, 90);
			break;
		case 8390:
		case 8550:
			owner.elements.SetBase(765, 100);
			break;
		}
		if (row.charge == 1)
		{
			owner.c_charges = 1;
		}
	}
}
