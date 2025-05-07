public class ConSuffocation : Condition
{
	public override bool PreventRegen => true;

	public override int GetPhase()
	{
		if (base.value >= 50)
		{
			if (base.value >= 100)
			{
				return 2;
			}
			return 1;
		}
		return 0;
	}

	public override void OnStart()
	{
		if (owner.IsPC)
		{
			Tutorial.Reserve("water", delegate
			{
				EClass.player.haltMove = true;
				EInput.Consume(consumeAxis: true);
			});
		}
	}

	public override void Tick()
	{
		if (owner.HasElement(429))
		{
			Kill();
			return;
		}
		if (base.value >= 200)
		{
			base.value = 200;
		}
		if (base.value >= 100 && !EClass._zone.IsRegion)
		{
			owner.DamageHP((base.value - 100) / 10 + owner.MaxHP / 20, AttackSource.Condition);
		}
		if (owner != null && owner.IsAliveInCurrentZone)
		{
			if (!owner.IsPC)
			{
				Mod(owner.IsPowerful ? (-15) : (-5));
			}
			else if (!owner.Cell.CanSuffocate())
			{
				Mod(-25);
			}
			else if (!EClass._zone.IsRegion)
			{
				owner.ModExp(200, 4 + phase * 4);
			}
		}
	}
}
