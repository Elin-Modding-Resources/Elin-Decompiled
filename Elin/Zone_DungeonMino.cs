public class Zone_DungeonMino : Zone_QuestDungeon
{
	public const int LvBoss = -5;

	public override int MinLv => -5;

	public override bool LockExit => false;

	public bool IsBossLv => base.lv == -5;

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
		base.OnGenerateMap();
		if (IsBossLv)
		{
			Chara chara = CharaGen.Create("ungaga_pap").ScaleByPrincipal();
			chara.AddThing("minohorn");
			Point point = EClass._map.FindThing<TraitStairsUp>().owner.pos.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false, ignoreCenter: true, 5) ?? EClass._map.GetCenterPos();
			AddCard(chara, point);
			base.Boss = chara;
		}
	}
}
