using System.Collections.Generic;
using Newtonsoft.Json;

public class ConTransmuteMimic : ConTransmute
{
	[JsonProperty]
	public Thing thing;

	public override bool HasDuration => false;

	public override RendererReplacer GetRendererReplacer()
	{
		if (thing.trait is TraitFigure traitFigure)
		{
			return RendererReplacer.CreateFrom(traitFigure.source.id);
		}
		return RendererReplacer.CreateFrom(thing.id, 0, thing.material.id);
	}

	public override void Tick()
	{
	}

	public override void OnBeforeStart()
	{
		List<Thing> list = owner.things.List((Thing t) => !t.IsMultisize && !t.isEquipped, onlyAccessible: true);
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

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner);
		owner.mimicry = this;
	}

	public override void OnRemoved()
	{
		owner.mimicry = null;
		base.OnRemoved();
	}

	public virtual void TrySetAct(ActPlan p)
	{
		if (thing.IsContainer)
		{
			p.TrySetAct("actContainer", delegate
			{
				RevealMimicry(EClass.pc, surprise: true);
				return true;
			}, owner, CursorSystem.Container);
		}
	}

	public virtual void RevealMimicry(Card c, bool surprise)
	{
		if (owner.IsHostile(c.Chara))
		{
			owner.DoHostileAction(c, immediate: true);
		}
		if (surprise)
		{
			owner.AddCondition<ConAmbush>();
		}
		Kill();
	}
}
