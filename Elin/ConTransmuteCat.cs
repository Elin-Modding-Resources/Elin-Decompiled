public class ConTransmuteCat : ConTransmute
{
	public override RendererReplacer GetRendererReplacer()
	{
		return RendererReplacer.CreateFrom("cat_black");
	}
}
