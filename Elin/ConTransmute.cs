public class ConTransmute : BaseBuff
{
	public override bool IsToggle => true;

	public override bool ShouldRefresh => true;

	public override bool ShouldTryNullify => true;

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
		bool isSynced = owner.isSynced;
		owner._CreateRenderer();
		if (isSynced)
		{
			EClass.scene.syncList.Add(owner.renderer);
			owner.renderer.OnEnterScreen();
		}
		if (owner.IsPCParty)
		{
			WidgetRoster.SetDirty();
		}
	}

	public override bool TryNullify(Condition c)
	{
		if (c != this && c is ConTransmute)
		{
			owner.Say("nullify", owner, Name.ToLower(), c.Name.ToLower());
			return true;
		}
		return false;
	}
}
