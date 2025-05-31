public class Zone_Field : Zone
{
	public override bool WillAutoSave => false;

	public bool IsBridge
	{
		get
		{
			if (base.lv == 0)
			{
				return base.Tile.IsBridge;
			}
			return false;
		}
	}

	public override bool HasLaw => IsBridge;

	public override float ChanceSpawnNeutral
	{
		get
		{
			if (!IsBridge)
			{
				return base.ChanceSpawnNeutral;
			}
			return 0.5f;
		}
	}

	public override bool UseFog
	{
		get
		{
			if (!base.IsPCFaction || base.lv != 0)
			{
				return base.lv <= 0;
			}
			return false;
		}
	}

	public override ZoneFeatureType FeatureType => ZoneFeatureType.RandomField;

	public override int DangerLvFix => base.Tile.source.dangerLv;

	public override string IdBiome
	{
		get
		{
			if (map != null)
			{
				return map.config.idBiome.IsEmpty(base.Tile.source.idBiome.IsEmpty("Plain"));
			}
			return "Plain";
		}
	}

	public override float PrespawnRate => 1.2f;

	public override bool isClaimable => EClass.pc.homeBranch != null;

	public override bool CanFastTravel => base.IsPCFaction;

	public override string idExport
	{
		get
		{
			if (!IsBridge)
			{
				return base.idExport;
			}
			return "bridge";
		}
	}

	public override float BigDaddyChance => 0.02f;

	public override float EvolvedChance => 0.05f;

	public override float OreChance => 5f;

	public override float ShrineChance
	{
		get
		{
			if (!EClass.debug.test)
			{
				return 0.04f;
			}
			return 1f;
		}
	}

	public override string IDBaseLandFeat
	{
		get
		{
			if (!base.isBeach)
			{
				return base.IDBaseLandFeat;
			}
			return "bfBeach";
		}
	}

	public override bool IsUnderwater => IdBiome == "Undersea";

	public override void OnGenerateMap()
	{
		base.OnGenerateMap();
		if (base.lv == 0)
		{
			base.idPrefix = EClass.sources.zoneAffixes.rows.RandomItem().id;
			base.dateExpire = EClass.world.date.GetRaw() + 10080;
		}
		bool draw = EClass.pc.HasCondition<ConDrawBacker>();
		ApplyBackerPet(draw);
		map.AddBackerTree(draw);
		if (IsBridge)
		{
			EClass._map.ForeachCell(delegate(Cell c)
			{
				c.isSeen = true;
			});
		}
	}
}
