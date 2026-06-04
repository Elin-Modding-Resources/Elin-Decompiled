public class ConWet : Condition
{
	public override bool ShouldRefresh => true;

	public override int GetPhase()
	{
		return 0;
	}

	public override void OnRefresh()
	{
		owner.isWet = true;
	}

	public override void Tick()
	{
		if (base.value > 100)
		{
			base.value = 100;
		}
		if ((!owner.Cell.IsTopWaterAndNoSnow && !owner.Cell.HasLiquid) || owner.IsLevitating)
		{
			Mod(-1);
		}
	}

	public override void OnStart()
	{
		if (!EClass._zone.IsRegion && owner.HasElement(1254))
		{
			Point randomPoint = owner.pos.GetRandomPoint(2, requireLos: false, allowChara: false, allowBlocked: false, 200);
			if (randomPoint != null && !randomPoint.Equals(BaseStats.CC.pos) && randomPoint.IsValid)
			{
				Chara t = owner.Duplicate();
				EClass._zone.AddCard(t, randomPoint);
				owner.Say("split", owner);
			}
		}
	}
}
