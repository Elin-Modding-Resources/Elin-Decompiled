public class ConBoost : BaseBuff
{
	public override void OnStart()
	{
		owner._CreateRenderer();
	}

	public override void OnRemoved()
	{
		owner._CreateRenderer();
	}

	public override RendererReplacer GetRendererReplacer()
	{
		return owner.id switch
		{
			"black_angel" => RendererReplacer.CreateFrom("black_angel", -1), 
			"adv_verna" => RendererReplacer.CreateFrom("adv_verna", 2), 
			"griffin" => RendererReplacer.CreateFrom("griffin", 1), 
			"isca" => RendererReplacer.CreateFrom("isca", 1), 
			_ => null, 
		};
	}
}
