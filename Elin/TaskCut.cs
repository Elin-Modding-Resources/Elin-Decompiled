public class TaskCut : TaskDesignation
{
	public override CursorInfo CursorIcon => CursorSystem.IconCut;

	public override int destDist => 1;

	public override void OnStart()
	{
		if (pos.cell.CanMakeStraw())
		{
			owner.ShowEmo(Emo.straw);
		}
		else
		{
			owner.ShowEmo(Emo.cut);
		}
	}

	public override HitResult GetHitResult()
	{
		if (pos.HasObj && !pos.HasMinableBlock)
		{
			return HitResult.Valid;
		}
		if (pos.HasDecal && (bool)BuildMenu.Instance)
		{
			return HitResult.Valid;
		}
		return HitResult.Default;
	}

	public override void OnCreateProgress(Progress_Custom p)
	{
		p.maxProgress = pos.cell.sourceObj.hp / 10 + 1;
	}

	public override void OnProgress()
	{
		_ = pos.cell.sourceObj;
		if (pos.cell.CanMakeStraw())
		{
			owner.SetTempHand(1006, -1);
		}
		else if (pos.cell.matObj.UseAxe)
		{
			owner.SetTempHand(1100, -1);
		}
		else if (pos.cell.matObj.UsePick)
		{
			owner.SetTempHand(1004, -1);
		}
		else
		{
			owner.SetTempHand(1000, -1);
		}
		owner.LookAt(pos);
		pos.PlaySound(pos.cell.matObj.GetSoundImpact());
		pos.cell.matObj.PlayHitEffect(pos);
		pos.cell.matObj.AddBlood(pos);
		pos.Animate(AnimeID.HitObj);
		if (IsToolValid() && EClass.setting.toolConsumeHP)
		{
			Act.TOOL.DamageHP(1L);
		}
	}

	public override void OnProgressComplete()
	{
		_ = pos.sourceObj.reqHarvest[0];
		EClass._map.MineObj(pos);
		EClass._map.SetDecal(pos.x, pos.z);
	}
}
