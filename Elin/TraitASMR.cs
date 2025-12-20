public class TraitASMR : Trait
{
	public int tick;

	public override bool HaveUpdate => true;

	public override void OnCreate(int lv)
	{
		ExcelData.Sheet dialogSheet = Lang.GetDialogSheet("asmr");
		owner.c_idRefCard = GetParam(1) ?? ((EClass.rnd(EClass.debug.enable ? 2 : 10) != 0) ? "jure" : dialogSheet.map.RandomItem()["id"]);
	}

	public override void OnImportMap()
	{
		if (owner.c_idRefCard.IsEmpty())
		{
			owner.c_idRefCard = GetParam(1);
		}
	}

	public override void Update()
	{
		if (!IsOn)
		{
			return;
		}
		tick++;
		if (tick % 5 == 0)
		{
			string[] dialog = Lang.GetDialog("asmr", owner.c_idRefCard.IsEmpty("eyth"));
			if (dialog.IsEmpty())
			{
				dialog = Lang.GetDialog("asmr", "eyth");
			}
			owner.TalkRaw(dialog.RandomItem());
		}
	}
}
