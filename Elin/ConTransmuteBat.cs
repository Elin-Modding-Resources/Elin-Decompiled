public class ConTransmuteBat : ConTransmute
{
	public override RendererReplacer GetRendererReplacer()
	{
		return RendererReplacer.CreateFrom("bat_vampire");
	}
}
