public class TraitStrangePainting : TraitVase
{
	public override void OnDie()
	{
		if (!EClass._zone.IsRegion && EClass.rnd(3) != 0)
		{
			Chara chara = EClass._zone.SpawnMob(owner.pos.GetNearestPoint(allowBlock: false, allowChara: false), new SpawnSetting
			{
				id = "paint_living",
				filterLv = EClass._zone.DangerLv + 20
			});
			chara.SetHostility(Hostility.Enemy);
			chara.SetSummon(9999);
			Msg.Say("statue_chara", chara, owner);
		}
	}
}
