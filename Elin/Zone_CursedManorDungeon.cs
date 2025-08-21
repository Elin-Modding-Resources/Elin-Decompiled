public class Zone_CursedManorDungeon : Zone_QuestDungeon
{
	public const int LvBoss = -6;

	public bool IsBossLv => base.lv == -6;

	public override bool LockExit => base.lv < -6;

	public override string idExport
	{
		get
		{
			if (base.lv != -6)
			{
				return base.idExport;
			}
			return "cursed_manor_boss";
		}
	}

	public override void OnGenerateMap()
	{
		if (IsBossLv)
		{
			foreach (Thing thing in EClass._map.things)
			{
				if (thing.id == "money2")
				{
					thing.SetNum(20);
				}
				if (thing.id == "rod_wish")
				{
					thing.SetCharge(1);
				}
			}
		}
		base.OnGenerateMap();
	}

	public override string GetDungenID()
	{
		return "Dungeon";
	}
}
