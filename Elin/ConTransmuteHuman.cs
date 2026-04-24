using System.Collections.Generic;
using Newtonsoft.Json;

public class ConTransmuteHuman : ConBaseTransmuteMimic
{
	[JsonProperty]
	public Chara chara;

	public override Card Card => chara;

	public bool IsBaby => chara.HasElement(1232);

	public override bool HasDuration => false;

	public override bool ShouldRevealOnContact => false;

	public override bool ShouldRevealOnPush => false;

	public override bool ShouldRevealOnDamage => (float)EClass.rnd(50) > (float)owner.hp / (float)owner.MaxHP * 100f;

	public override bool ShouldEndMimicry(Act act)
	{
		return false;
	}

	public override RendererReplacer GetRendererReplacer()
	{
		return RendererReplacer.CreateFrom(chara.id);
	}

	public override void OnBeforeStart()
	{
		if (chara == null)
		{
			List<Chara> list = owner.pos.ListCharasInRadius(owner, 5, delegate(Chara c)
			{
				if (!c.IsMultisize && c.IsHumanSpeak)
				{
					CardRenderer renderer = c.renderer;
					if (renderer != null && !renderer.hasActor)
					{
						return !c.HasElement(1427);
					}
				}
				return false;
			});
			if (list.Count > 0)
			{
				chara = list.RandomItem().Duplicate();
			}
			else
			{
				chara = CharaGen.CreateFromFilter("c_guest");
			}
		}
		base.OnBeforeStart();
	}
}
