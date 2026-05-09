using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class ConTransmuteHuman : ConBaseTransmuteMimic
{
	[JsonProperty]
	public Chara chara;

	[JsonProperty]
	public int lastHP;

	public override Card Card => chara;

	public bool IsBaby => chara.HasElement(1232);

	public bool IsNyaru => owner.HasElement(1429);

	public override bool HasDuration => false;

	public override bool ShouldRevealOnContact => false;

	public override bool ShouldRevealOnPush => false;

	public override bool ShouldRevealOnDamage
	{
		get
		{
			if (!IsNyaru)
			{
				return (float)EClass.rnd(50) > (float)owner.hp / (float)owner.MaxHP * 100f;
			}
			return false;
		}
	}

	public override bool ShouldEndMimicry(Act act)
	{
		return false;
	}

	public override RendererReplacer GetRendererReplacer()
	{
		if (chara.IsPCC)
		{
			return RendererReplacer.CreateFromPCC(chara.id, chara.pccData);
		}
		return RendererReplacer.CreateFrom(chara.id);
	}

	public override void OnBeforeStart()
	{
		if (chara == null)
		{
			if (IsNyaru)
			{
				NyaruSpecial();
			}
			else
			{
				List<Thing> list = owner.things.List((Thing t) => t.trait is TraitFigure { source: not null } traitFigure && !traitFigure.source.multisize, onlyAccessible: true);
				if (list.Count > 0)
				{
					chara = CharaGen.Create((list.RandomItem().trait as TraitFigure).source.id);
				}
				else
				{
					List<Chara> list2 = owner.pos.ListCharasInRadius(owner, 6, delegate(Chara c)
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
					if (list2.Count > 0)
					{
						chara = list2.RandomItem().Duplicate();
					}
					else
					{
						chara = CharaGen.CreateFromFilter("c_guest");
					}
				}
			}
		}
		base.OnBeforeStart();
	}

	public void NyaruSpecial()
	{
		if (chara != null && chara.id != owner.id)
		{
			chara = owner;
		}
		else
		{
			IEnumerable<SourceChara.Row> ie = EClass.sources.charas.rows.Where((SourceChara.Row a) => !a.multisize && !a.actCombat.IsEmpty() && !a.HasTag(CTAG.suicide) && !a.HasTag(CTAG.noRandomProduct));
			chara = CharaGen.Create(ie.RandomItem().id);
		}
		lastHP = owner.hp;
		owner.PlayEffect("morph");
		owner.PlaySound("vanish");
	}

	public override void Tick()
	{
		if (IsNyaru && Mathf.Abs(owner.hp - lastHP) >= owner.MaxHP / 10)
		{
			NyaruSpecial();
			Change();
		}
		base.Tick();
	}
}
