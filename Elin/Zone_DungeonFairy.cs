public class Zone_DungeonFairy : Zone_QuestDungeon
{
	public override int MinLv => LvBoss;

	public override bool CanUnlockExit
	{
		get
		{
			if (EClass.pc.party.Find("fairy_nanasu") != null)
			{
				return EClass.game.quests.GetPhase<QuestNasu>() == 0;
			}
			return false;
		}
	}

	public int LvBoss => -5;

	public bool IsBossLv => base.lv == LvBoss;

	public override string GetDungenID()
	{
		if (IsBossLv)
		{
			return "CavernBig";
		}
		return base.GetDungenID();
	}

	public override void OnGenerateMap()
	{
		if (IsBossLv)
		{
			Chara t = CharaGen.Create("fairy_poina").ScaleByPrincipal();
			Chara t2 = CharaGen.Create("fairy_raina").ScaleByPrincipal();
			Point point = EClass._map.FindThing<TraitStairsUp>().owner.pos.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false, ignoreCenter: true, 5) ?? EClass._map.GetCenterPos();
			AddCard(t, point);
			AddCard(t2, point.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false) ?? point);
			LayerDrama.Activate("fairy_nanasu", "fairy_nanasu", "battle", EClass.pc.party.Find("fairy_nanasu"));
		}
		base.OnGenerateMap();
	}
}
