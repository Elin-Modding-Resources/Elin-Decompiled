public class Zone_Void : Zone_Dungeon
{
	public bool IsBossLV
	{
		get
		{
			if (base.lv % 5 == 0)
			{
				return GetTopZone().GetDeepestLv() == base.lv;
			}
			return false;
		}
	}

	public override string TextWidgetDate
	{
		get
		{
			if (base.Boss == null)
			{
				return "";
			}
			return "bossLevel".lang(base.Boss.Name);
		}
	}

	public override int MinLv => -10000;

	public override ZoneScaleType ScaleType => ZoneScaleType.Void;

	public override bool LockExit => IsBossLV;

	public override bool CanUnlockExit
	{
		get
		{
			if (base.Boss != null)
			{
				return base.Boss.isDead;
			}
			return true;
		}
	}

	public override string IDPlayList
	{
		get
		{
			if (!IsBossLV)
			{
				return base.IDPlayList;
			}
			return "Dungeon_Boss";
		}
	}

	public override void OnGenerateMap()
	{
		base._dangerLv = 50;
		if (IsBossLV)
		{
			base.Boss = SpawnMob(null, SpawnSetting.Boss(DangerLv));
			Chara boss = base.Boss;
			Hostility hostility2 = (base.Boss.c_originalHostility = Hostility.Enemy);
			boss.hostility = hostility2;
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara.IsHostile())
				{
					chara.enemy = EClass.pc.party.members.RandomItem();
				}
			}
		}
		base.OnGenerateMap();
	}
}
