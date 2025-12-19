using UnityEngine;

public class TraitASMR : Trait
{
	public int tick;

	public override bool HaveUpdate => true;

	public override void OnCreate(int lv)
	{
		owner.c_idRefCard = GetParam(1) ?? ((EClass.rnd(EClass.debug.enable ? 2 : 10) == 0) ? "opatos" : ((EClass.rnd(2) == 0) ? "jure" : ""));
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
			Debug.Log(owner.c_idRefCard);
			string[] dialog = Lang.GetDialog("asmr", owner.c_idRefCard.IsEmpty("eyth"));
			if (dialog.IsEmpty())
			{
				dialog = Lang.GetDialog("asmr", "eyth");
			}
			owner.TalkRaw(dialog.RandomItem());
		}
	}
}
