public class Zone_Yowyn : Zone_Town
{
	public override string IDPlaylistOverwrite
	{
		get
		{
			if (!IsFestival)
			{
				return null;
			}
			return "Festival_Yowyn";
		}
	}

	public override bool IsFestival
	{
		get
		{
			if (base.lv == 0)
			{
				return EClass.world.date.month == 9;
			}
			return false;
		}
	}

	public override string IDHat
	{
		get
		{
			if (!IsFestival)
			{
				return base.IDHat;
			}
			return "hat_mushroom";
		}
	}

	public override void OnVisitNewMapOrRegenerate()
	{
		base.OnVisitNewMapOrRegenerate();
		if (base.lv == -1)
		{
			ApplyBackerPet(EClass.pc.HasCondition<ConDrawBacker>());
		}
	}
}
