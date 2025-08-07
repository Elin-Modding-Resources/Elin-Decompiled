using UnityEngine;

public class TaskPlow : TaskDesignation
{
	public override CursorInfo CursorIcon => CursorSystem.Dig;

	public override bool CanPressRepeat => true;

	public override int destDist
	{
		get
		{
			if (!pos.cell.blocked)
			{
				return 0;
			}
			return 1;
		}
	}

	public override bool CanProgress()
	{
		if (base.CanProgress())
		{
			return !pos.IsFarmField;
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
				owner.Say("till_start", owner, owner.Tool);
			}
		};
		p.onProgress = delegate(Progress_Custom _p)
		{
			SourceMaterial.Row row = (pos.cell.HasBridge ? pos.cell.matBridge : pos.cell.matFloor);
			row.PlayHitEffect(pos);
			row.AddBlood(pos);
			pos.PlaySound(row.GetSoundImpact());
			owner.renderer.NextFrame();
			if (!(pos.HasBridge ? pos.sourceBridge : pos.sourceFloor).tag.Contains("soil"))
			{
				owner.Say("till_invalid");
				_p.Cancel();
			}
		};
		p.onProgressComplete = delegate
		{
			owner.PlaySound(pos.cell.HasBridge ? pos.cell.matBridge.GetSoundDead(pos.sourceBridge) : pos.cell.matFloor.GetSoundDead(pos.sourceFloor));
			Effect.Get("mine").Play(pos).SetParticleColor(pos.cell.HasBridge ? pos.matBridge.GetColor() : pos.matFloor.GetColor())
				.Emit(10 + EClass.rnd(10));
			pos.Animate(AnimeID.Dig, animeBlock: true);
			if (pos.HasBridge)
			{
				pos.cell._bridge = 4;
			}
			else
			{
				pos.SetFloor(pos.matFloor.id, 4);
			}
			owner.elements.ModExp(286, 30f);
			if (owner.IsPC)
			{
				EClass.player.stats.plow++;
			}
			owner.stamina.Mod(-1);
		};
	}

	public override HitResult GetHitResult()
	{
		if (pos.cell.IsTopWater || pos.HasObj || pos.IsFarmField)
		{
			return HitResult.Invalid;
		}
		return HitResult.Valid;
	}
}
