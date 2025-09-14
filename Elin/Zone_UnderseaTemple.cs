public class Zone_UnderseaTemple : Zone_QuestDungeon
{
	public const int LvBoss = -5;

	public bool IsBossLv => base.lv == -5;

	public override bool IsUnderwater => true;

	public override bool LockExit => base.lv <= -4;

	public override bool CanUnlockExit => EClass.game.quests.GetPhase<QuestNegotiationDarkness>() >= 2;

	public override float RespawnRate => base.RespawnRate * 3f;

	public override string idExport
	{
		get
		{
			if (base.lv != -5)
			{
				return base.idExport;
			}
			return "lurie_boss";
		}
	}

	public override string GetDungenID()
	{
		if (EClass.rnd(2) == 0)
		{
			return "RoundRooms";
		}
		if (EClass.rnd(3) == 0)
		{
			return "CavernBig";
		}
		return "Cavern";
	}

	public override void OnGenerateMap()
	{
		if (!IsBossLv)
		{
			base.OnGenerateMap();
		}
	}
}
