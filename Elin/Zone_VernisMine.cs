using UnityEngine;

public class Zone_VernisMine : Zone_QuestDungeon
{
	public const int LvBoss = -8;

	public bool IsBossLv => base.lv == -8;

	public override int DangerLv => 5 + Mathf.Abs(base.lv);

	public override bool LockExit
	{
		get
		{
			if (base.lv == -1)
			{
				return EClass.game.quests.GetPhase<QuestVernis>() < 7;
			}
			return false;
		}
	}

	public override float OreChance => 4f;

	public override string idExport
	{
		get
		{
			if (base.lv != -8)
			{
				return base.idExport;
			}
			return "vernis_mine_boss";
		}
	}

	public override string GetDungenID()
	{
		return "DungeonMine";
	}

	public override void OnGenerateMap()
	{
		if (IsBossLv)
		{
			return;
		}
		PlaceRail();
		EClass._map.ForeachCell(delegate(Cell c)
		{
			if (EClass.rnd(5) != 0 && c._block != 0 && !c.HasObj && !c.isSurrounded && !c.hasDoor)
			{
				c.GetSharedPoint().SetObj(24, 1, EClass.rnd(3));
			}
		});
		base.OnGenerateMap();
	}
}
