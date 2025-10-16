using UnityEngine;

public class Zone_CurryRuinDungeon : Zone_QuestDungeon
{
	public const int LvBoss = -5;

	public bool IsBossLv => base.lv == -5;

	public override int DangerLv => 33 + Mathf.Abs(base.lv);

	public override bool LockExit => base.lv <= -1;

	public override bool CanUnlockExit => EClass.game.quests.IsStarted<QuestCurry>();

	public override string idExport
	{
		get
		{
			if (base.lv != -5)
			{
				return base.idExport;
			}
			return "curry_boss";
		}
	}

	public override bool HasLaw => IsBossLv;

	public override bool SetAlarmOnBreakLaw => IsBossLv;

	public override void OnGenerateMap()
	{
		if (IsBossLv)
		{
			SetAlarm(enable: false);
			FindChara("doga").SetHostility(Hostility.Enemy);
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

	public override void SetAlarm(bool enable)
	{
		base.SetAlarm(enable);
		if (!enable)
		{
			return;
		}
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.IsInstalled && thing.id == "candle3")
			{
				thing.c_lightColor = 30720;
				thing.RecalculateFOV();
			}
		}
		SetBGM(110);
	}
}
