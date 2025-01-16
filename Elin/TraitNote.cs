public class TraitNote : Trait
{
	public override bool CanStackTo(Thing to)
	{
		return owner.c_note == to.c_note;
	}

	public override void OnCreate(int lv)
	{
		if (GetParam(1) != null)
		{
			owner.c_note = GetParam(1);
		}
	}

	public override void OnImportMap()
	{
		OnCreate(EClass._zone.DangerLv);
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!p.altAction)
		{
			return;
		}
		p.TrySetAct("actWrite", delegate
		{
			Dialog.InputName("dialogWriteNote", owner.c_note, delegate(bool cancel, string text)
			{
				if (!cancel)
				{
					owner.c_note = text;
				}
			});
			return false;
		}, owner);
	}
}
