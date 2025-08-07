using System.Collections.Generic;
using UnityEngine;

public class TaskChopWood : TaskDesignation
{
	public override CursorInfo CursorIcon => CursorSystem.Cut;

	public override int destDist => 1;

	public override bool Loop => GetLog() != null;

	public override bool CanManualCancel()
	{
		return true;
	}

	public Thing GetLog()
	{
		return pos.FindThing((Thing t) => t.id == "log");
	}

	public override HitResult GetHitResult()
	{
		if (GetLog() != null)
		{
			return HitResult.Valid;
		}
		return HitResult.Invalid;
	}

	public override bool CanProgress()
	{
		if (base.CanProgress() && GetLog() != null && owner.Tool != null && owner.Tool.trait is TraitTool)
		{
			return owner.Tool.HasElement(225);
		}
		return false;
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		p.textHint = Name;
		p.maxProgress = Mathf.Max((15 + EClass.rnd(20)) * 100 / (100 + owner.Tool.material.hardness * 3), 2);
		p.onProgressBegin = delegate
		{
			if (owner.Tool != null)
			{
				owner.Say("chopwood_start", owner, GetLog().GetName(NameStyle.Full, 1));
			}
		};
		p.onProgress = delegate
		{
			Thing log2 = GetLog();
			SourceMaterial.Row material2 = log2.material;
			log2.PlaySoundImpact();
			material2.AddBlood(pos);
			log2.PlayAnime(AnimeID.HitObj);
			material2.PlayHitEffect(pos);
			owner.renderer.NextFrame();
		};
		p.onProgressComplete = delegate
		{
			Thing log = GetLog();
			SourceMaterial.Row material = log.material;
			log.PlaySoundDead();
			material.AddBlood(pos, 3 + EClass.rnd(2));
			log.material.PlayHitEffect(pos, 10);
			Thing thing = ThingGen.Create("plank", material.id).SetNum(1 + EClass.rnd(2));
			CraftUtil.MixIngredients(thing, new List<Thing> { log }, CraftUtil.MixType.General, 999);
			log.ModNum(-1);
			owner.elements.ModExp(225, 30f);
			owner.stamina.Mod(-1);
			EClass._map.TrySmoothPick(pos, thing, EClass.pc);
		};
	}
}
