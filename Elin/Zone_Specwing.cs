public class Zone_Specwing : Zone_SubTown
{
	public override void OnActivate()
	{
		if (base.lv == 1 && (EClass.game.quests.IsCompleted("nasu") || EClass.debug.enable) && EClass.game.cards.globalCharas.Find("fairy_raina") == null)
		{
			EClass._zone.AddChara("fairy_raina", 61, 56).SetHomeZone(this).SetHostility(Hostility.Friend);
			EClass._zone.AddChara("fairy_poina", 61, 59).SetHomeZone(this).SetHostility(Hostility.Friend);
		}
		base.OnActivate();
	}
}
