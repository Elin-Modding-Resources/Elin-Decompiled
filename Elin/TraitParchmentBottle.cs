public class TraitParchmentBottle : TraitParchment
{
	public override bool CanBeSmashedToDeath => true;

	public override bool CanBeAttacked => !EClass._zone.IsPCFactionOrTent;

	public override ThrowType ThrowType => ThrowType.Vase;

	public override BookList.Item Item => BottleMessageList.GetItem(base.IdItem);

	public override void OnCreate(int lv)
	{
		owner.SetStr(53, BottleMessageList.GetRandomItem().id);
		owner.c_lockLv = 1;
		string id;
		int num;
		if (!EClass._zone.IsUserZone && EClass.rnd(4) != 0)
		{
			id = "money";
			num = EClass.rnd(EClass.rnd(EClass.rnd(EClass.rnd(500)))) + 1;
			Rand.SetSeed(owner.uid);
			if (EClass.rnd(5) == 0)
			{
				SetId("scratchcard", 1);
			}
			if (EClass.rnd(10) == 0)
			{
				SetId("234", 1);
			}
			if (EClass.rnd(20) == 0)
			{
				SetId("medal", 1);
			}
			if (EClass.rnd(30) == 0)
			{
				SetId("map_treasure", 1);
			}
			if (EClass.rnd(50) == 0)
			{
				SetId("ticket_fortune", 1);
			}
			if (EClass.rnd(100) == 0)
			{
				SetId("1084", 1);
			}
			if (EClass.rnd(500) == 9)
			{
				SetId("book_exp", 1);
			}
			Rand.SetSeed();
			owner.AddCard(ThingGen.Create(id, -1, EClass.pc.FameLv).SetNum(num));
		}
		void SetId(string _id, int _num)
		{
			id = _id;
			num = _num;
		}
	}

	public override void SetName(ref string s)
	{
		base.SetName(ref s);
		if (owner.c_lockLv != 0)
		{
			s += "_sealed".lang();
		}
	}
}
