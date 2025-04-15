public class AI_Practice : AIAct
{
	public long totalDamage;

	public long turn;

	public long hit;

	public override CursorInfo CursorIcon => CursorSystem.IconMelee;

	public override bool HasProgress => true;

	public override bool CanManualCancel()
	{
		return true;
	}

	public override void OnCancelOrSuccess()
	{
		base.OnCancelOrSuccess();
		if (owner == EClass.pc)
		{
			long a = totalDamage / turn;
			Msg.Say("trainingDPS", turn.ToFormat(), a.ToFormat(), hit.ToFormat() ?? "", totalDamage.ToFormat() ?? "");
		}
	}
}
