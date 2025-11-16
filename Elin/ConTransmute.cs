public class ConTransmute : BaseBuff
{
	public override bool IsToggle => true;

	public override bool ShouldRefresh => true;

	public override void Tick()
	{
		if (owner.host == null && owner.conSleep == null && (EClass.pc.conSleep == null || EClass.pc.conSleep.pcSleep == 0))
		{
			base.Tick();
		}
	}

	public override void OnStart()
	{
		owner._CreateRenderer();
		if (owner.IsPCParty)
		{
			WidgetRoster.SetDirty();
		}
	}

	public override void OnRemoved()
	{
		owner._CreateRenderer();
		if (owner.IsPCParty)
		{
			WidgetRoster.SetDirty();
		}
	}
}
