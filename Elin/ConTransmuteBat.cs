public class ConTransmuteBat : ConTransmute
{
	public override bool HasDuration => false;

	public override bool CanManualRemove => !owner.HasCooldown(8793);

	public override RendererReplacer GetRendererReplacer()
	{
		return RendererReplacer.CreateFrom("bat_trans");
	}

	public override void Tick()
	{
	}

	public override void OnStart()
	{
		base.OnStart();
		owner.PlaySound("bat");
		if (!EClass._zone.IsPCFactionOrTent && owner.pos.TryWitnessCrime(owner) && owner == EClass.pc)
		{
			EClass.player.ModKarma(-1);
		}
	}

	public override void OnRemoved()
	{
		base.OnRemoved();
		owner.HealHP(owner.MaxHP, HealSource.Item);
		owner.SetCooldown(8793);
		if (!EClass._zone.IsPCFactionOrTent && owner.pos.TryWitnessCrime(owner) && owner == EClass.pc)
		{
			EClass.player.ModKarma(-1);
		}
	}
}
