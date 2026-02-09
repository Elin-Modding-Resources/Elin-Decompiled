public class ConTransmuteShadow : ConTransmute
{
	public override bool HasDuration => false;

	public override RendererReplacer GetRendererReplacer()
	{
		return RendererReplacer.CreateFrom("shade");
	}
}
