public class ConBrightnessOfLife : BaseBuff
{
	public override void OnStart()
	{
		(owner.ai as GoalCombat)?.TryAddAbility(6410);
	}

	public override void OnRemoved()
	{
		(owner.ai as GoalCombat)?.TryRemoveAbility(6410);
	}
}
