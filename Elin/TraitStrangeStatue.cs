public class TraitStrangeStatue : TraitVase
{
	public override void OnDie()
	{
		if (!EClass._zone.IsRegion && EClass.rnd(2) != 0)
		{
			Chara chara = EClass._zone.SpawnMob(owner.pos.GetNearestPoint(allowBlock: false, allowChara: false), new SpawnSetting
			{
				idSpawnList = "c_statue",
				filterLv = EClass._zone.DangerLv + 30
			});
			chara.SetHostility((EClass.rnd(3) != 0) ? Hostility.Enemy : Hostility.Neutral);
			Msg.Say("statue_chara", chara, owner);
		}
	}
}
