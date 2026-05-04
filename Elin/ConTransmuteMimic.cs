using System.Collections.Generic;
using Newtonsoft.Json;

public class ConTransmuteMimic : ConBaseTransmuteMimic
{
	[JsonProperty]
	public Thing thing;

	public override Card Card => thing;

	public override RendererReplacer GetRendererReplacer()
	{
		if (thing.trait is TraitFigure traitFigure)
		{
			return RendererReplacer.CreateFrom(traitFigure.source.id);
		}
		return RendererReplacer.CreateFrom(thing.id, 0, thing.material.id);
	}

	public override void OnBeforeStart()
	{
		List<Thing> list = owner.things.List((Thing t) => !t.source.multisize && t.c_isImportant, onlyAccessible: true);
		if (list.Count == 0)
		{
			list = owner.things.List((Thing t) => !t.source.multisize && !t.isEquipped, onlyAccessible: true);
		}
		if (list.Count > 0)
		{
			thing = list.RandomItem().Duplicate(1);
		}
		else if (EClass.rnd(3) == 0)
		{
			thing = ThingGen.Create("chest3", -1, EClass._zone.ContentLv);
			thing.ChangeMaterial(EClass._zone.biome.style.matDoor);
		}
		else
		{
			thing = ThingGen.CreateFromFilter("dungeon", EClass._zone.ContentLv).SetNum(1);
		}
		base.OnBeforeStart();
	}

	public override void TrySetAct(ActPlan p)
	{
		if (thing.IsContainer)
		{
			p.TrySetAct("actContainer", delegate
			{
				Reveal(EClass.pc, surprise: true);
				return true;
			}, owner, CursorSystem.Container);
		}
	}
}
