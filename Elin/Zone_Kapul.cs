public class Zone_Kapul : Zone_Town
{
	public override string IDPlaylistOverwrite
	{
		get
		{
			if (!IsFestival)
			{
				return null;
			}
			return "Festival_Noyel";
		}
	}

	public override bool IsFestival
	{
		get
		{
			if (base.lv == 0)
			{
				return EClass.world.date.month == 6;
			}
			return false;
		}
	}

	public override float VolumeSea => 1f;
}