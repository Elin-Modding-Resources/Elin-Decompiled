using UnityEngine;

public class Zone_QuestDungeon : Zone_Dungeon
{
	public bool ShouldScale => EClass.game.principal.scaleQuest;

	public override ZoneScaleType ScaleType => ZoneScaleType.Quest;

	public override int DangerLvBoost
	{
		get
		{
			if (!ShouldScale)
			{
				return 0;
			}
			return Mathf.Max(0, EClass.pc.FameLv * 2 / 3 - DangerLv);
		}
	}

	public override bool ShouldScaleImportedChara(Chara c)
	{
		if (ShouldScale && c.rarity >= Rarity.Legendary)
		{
			return c.IsHostile();
		}
		return false;
	}
}
