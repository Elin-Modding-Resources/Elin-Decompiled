public class Zone_Nefu : Zone_SubTown
{
	public override void OnActivate()
	{
		EClass._map.config.blossom = EClass.pc.faith == EClass.game.religions.MoonShadow;
		base.OnActivate();
	}
}
