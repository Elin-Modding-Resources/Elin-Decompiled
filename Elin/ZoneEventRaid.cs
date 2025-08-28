using UnityEngine;

public class ZoneEventRaid : ZoneEventSiege
{
	public override void OnInit()
	{
		lv = Mathf.Max(1, EClass.game.survival.flags.raidLv);
		max = 5 + lv / 4;
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
		EClass.game.survival.RefreshRewards();
		Point pos = EClass.game.survival.GetRandomPoint() ?? EClass.pc.pos;
		string item = EClass.game.survival.listReward.RandomItem();
		EClass.game.survival.listReward.Remove(item);
		EClass.game.survival.MeteorThing(pos, item, install: true);
	}
}
