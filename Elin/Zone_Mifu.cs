public class Zone_Mifu : Zone_SubTown
{
	public override void OnActivate()
	{
		if (base.lv == 0)
		{
			EClass._map.config.blossom = EClass.pc.faith == EClass.game.religions.Trickery;
		}
		base.OnActivate();
	}
}
