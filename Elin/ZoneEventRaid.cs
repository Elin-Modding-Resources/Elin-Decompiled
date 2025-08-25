using UnityEngine;

public class ZoneEventRaid : ZoneEventSiege
{
	public override void OnInit()
	{
		lv = Mathf.Max(1, EClass.game.survival.flags.raidLv);
		max = 5 + lv / 5;
		base.OnInit();
	}

	public override Point GetSpawnPos()
	{
		Trait trait = EClass._map.FindThing<TraitVoidgate>();
		if (trait != null)
		{
			trait.Toggle(on: true, silent: true);
		}
		else
		{
			trait = EClass._map.FindThing<TraitCoreDefense>();
		}
		Point point = ((trait != null) ? trait.owner.pos : EClass.pc.pos);
		return point.GetNearestPoint(allowBlock: false, allowChara: false) ?? point;
	}

	public override void OnKill()
	{
		base.OnKill();
		EClass.game.survival.flags.raidRound++;
		EClass.game.survival.flags.raidLv += 5;
		EClass.game.survival.flags.dateNextRaid = EClass.world.date.GetRaw(168);
		Point pos = EClass.game.survival.GetRandomPoint() ?? EClass.pc.pos;
		string text = ((EClass.game.survival.flags.raidLv >= 50 && EClass.rnd(3) == 0) ? "statue_god" : ((EClass.game.survival.flags.raidLv >= 25 && EClass.rnd(2) == 0) ? "statue_power" : "altar"));
		EClass.game.survival.MeteorThing(pos, text, install: true);
	}
}
