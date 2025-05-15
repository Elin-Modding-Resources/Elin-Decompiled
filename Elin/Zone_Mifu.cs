public class Zone_Mifu : Zone_SubTown
{
	public override void OnActivate()
	{
		EClass._map.config.blossom = EClass.pc.faith == EClass.game.religions.Trickery;
		base.OnActivate();
	}
}
