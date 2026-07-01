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

	public override void OnVisitNewMapOrRegenerate()
	{
		base.OnVisitNewMapOrRegenerate();
		if (FindChara("mamani") == null && FindDeadChara("mamani") == null && EClass.game.cards.globalCharas.Find("mamani") == null && EClass.pc.faction.FindChara("namamani") != null && EClass.pc.faction.FindChara("namamani2") != null)
		{
			AddChara("mamani", 48, 56).SetHomeZone(this);
		}
	}
}
