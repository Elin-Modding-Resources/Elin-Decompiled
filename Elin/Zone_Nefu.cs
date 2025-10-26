public class Zone_Nefu : Zone_SubTown
{
	public override void OnActivate()
	{
		if (EClass._map.version.IsBelow(0, 23, 226))
		{
			SetBGM(119, refresh: false);
		}
		if (base.lv == 0)
		{
			EClass._map.config.blossom = EClass.pc.faith == EClass.game.religions.MoonShadow;
		}
		base.OnActivate();
	}
}
