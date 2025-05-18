public class Zone_Aquli : Zone_Town
{
	public override void OnActivate()
	{
		if (base.lv == 0)
		{
			EClass._map.config.fixedCondition = ((EClass.pc.faith == EClass.game.religions.Strife) ? Weather.Condition.Ether : Weather.Condition.None);
		}
		base.OnActivate();
	}
}
