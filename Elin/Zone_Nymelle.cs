public class Zone_Nymelle : Zone_QuestDungeon
{
	public override string idExport
	{
		get
		{
			if (base.lv != LvBoss)
			{
				if (base.lv != LvCrystal)
				{
					return base.source.id;
				}
				return "nymelle_crystal";
			}
			return "nymelle_boss";
		}
	}

	public int LvBoss => -5;

	public int LvCrystal => -6;

	public bool IsBossLv => base.lv == LvBoss;

	public bool IsCrystalLv => base.lv == LvCrystal;

	public override bool LockExit
	{
		get
		{
			if ((base.lv != -2 || EClass.game.quests.GetPhase<QuestExploration>() >= 1) && (base.lv != LvBoss + 1 || EClass.game.quests.GetPhase<QuestExploration>() >= 2))
			{
				if (base.lv == LvBoss)
				{
					return EClass.game.quests.GetPhase<QuestExploration>() < 3;
				}
				return false;
			}
			return true;
		}
	}

	public override bool UseFog
	{
		get
		{
			if (!IsBossLv)
			{
				return !IsCrystalLv;
			}
			return false;
		}
	}

	public override bool RevealRoom
	{
		get
		{
			if (!IsBossLv)
			{
				return IsCrystalLv;
			}
			return true;
		}
	}

	public override float PrespawnRate
	{
		get
		{
			if (!IsBossLv && !IsCrystalLv)
			{
				return base.PrespawnRate;
			}
			return 0f;
		}
	}

	public override string GetNewZoneID(int level)
	{
		if (level == LvBoss)
		{
			return "nymelle_boss";
		}
		if (level == LvCrystal)
		{
			return "nymelle_crystal";
		}
		return base.GetNewZoneID(level);
	}

	public override void OnBeforeSimulate()
	{
		if (base.visitCount != 0)
		{
			return;
		}
		if (IsBossLv)
		{
			EClass._zone.AddChara("isygarad", 40, 37).ScaleByPrincipal();
			SoundManager.ForceBGM();
			LayerDrama.ActivateMain("mono", "nymelle_boss");
		}
		if (IsCrystalLv)
		{
			Chara chara = EClass.game.cards.globalCharas.Find("fiama");
			chara.MoveHome(EClass._zone, 43, 67);
			chara.AddEditorTag(EditorTag.AINoMove);
		}
		else if (base.lv == -2)
		{
			Chara chara2 = EClass.game.cards.globalCharas.Find("farris");
			if (chara2 == null)
			{
				chara2 = CharaGen.Create("farris");
				chara2.SetGlobal();
				Thing thing = EClass._map.props.installed.Find<TraitStairsLocked>();
				EClass._zone.AddCard(chara2, thing.pos.x, thing.pos.z);
				chara2.AddEditorTag(EditorTag.AINoMove);
				chara2.AddEditorTag(EditorTag.Invulnerable);
			}
		}
	}
}
