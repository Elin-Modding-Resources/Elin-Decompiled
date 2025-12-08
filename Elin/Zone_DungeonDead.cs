public class Zone_DungeonDead : Zone_QuestDungeon
{
	public const int LvBoss = -6;

	public override int MinLv => -6;

	public override bool LockExit => false;

	public bool IsBossLv => base.lv == -6;

	public override string idExport
	{
		get
		{
			if (base.lv != -6)
			{
				return base.idExport;
			}
			return "boss_dead";
		}
	}

	public override void OnGenerateMap()
	{
		base.OnGenerateMap();
		EClass._map.config.fog = FogType.CaveDeep;
	}
}
