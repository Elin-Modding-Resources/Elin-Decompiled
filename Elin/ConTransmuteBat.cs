public class ConTransmuteBat : ConTransmute
{
	public override bool HasDuration => false;

	public override RendererReplacer GetRendererReplacer()
	{
		return RendererReplacer.CreateFrom("bat");
	}

	public override void Tick()
	{
	}
}
