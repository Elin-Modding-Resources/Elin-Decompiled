public class ZoneEventRaid : ZoneEventSiege
{
	public override void OnFirstTick()
	{
		lv = 5 + EClass.game.survival.flags.raidRound * 10;
		max = 5 + lv / 5;
		base.OnFirstTick();
	}

	public override Point GetSpawnPos()
	{
		Trait trait = EClass._map.FindThing<TraitVoidgate>();
		if (trait != null)
		{
			return trait.owner.pos;
		}
		trait = EClass._map.FindThing<TraitCoreDefense>();
		if (trait != null)
		{
			return trait.owner.pos;
		}
		return EClass.pc.pos;
	}

	public override void OnKill()
	{
		base.OnKill();
		EClass.game.survival.flags.raidRound++;
		EClass.game.survival.flags.dateNextRaid = EClass.world.date.GetRaw(168);
	}
}
