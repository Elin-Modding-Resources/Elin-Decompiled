using System.Collections.Generic;

public class AI_Torture : AI_Practice
{
	public TraitShackle shackle;

	public Chara _owner;

	public override bool CancelWhenDamaged => false;

	public override IEnumerable<Status> Run()
	{
		_owner = owner;
		isFail = () => shackle == null || !shackle.owner.ExistsOnMap || !owner.isRestrained || !shackle.owner.pos.Equals(owner.pos) || owner.stamina.value <= 0;
		while (!owner.IsPC && owner.stamina.value < owner.stamina.max * 4 / 5)
		{
			yield return KeepRunning();
		}
		yield return DoProgress();
	}

	public override AIProgress CreateProgress()
	{
		return new Progress_Custom
		{
			canProgress = () => !isFail(),
			onProgressBegin = delegate
			{
			},
			onProgress = delegate(Progress_Custom p)
			{
				if (p.progress % 8 == 0)
				{
					owner.PlayAnime(shackle.animeId);
				}
				if (EClass.rnd(20) == 0)
				{
					owner.Talk("restrained");
				}
				List<Chara> list = new List<Chara>();
				foreach (Chara chara in EClass._map.charas)
				{
					if (!chara.IsDisabled && chara.IsPCFaction && chara != owner && !chara.IsPC && !chara.isRestrained && chara.host == null && !chara.noMove && !chara.IsInCombat && !(chara.ai.Current is AI_Eat))
					{
						list.Add(chara);
					}
				}
				list.Sort((Chara a, Chara b) => SortVal(a) - SortVal(b));
				int num = 1 + EClass.Branch.members.Count / 5;
				int num2 = 0;
				foreach (Chara item in list)
				{
					if (EClass.rnd(3) == 0 && item.HasAccess(owner.pos) && PathManager.Instance.IsPathClear(owner.pos, item.pos, item, 30))
					{
						item.SetEnemy(owner);
					}
					num2++;
					if (num2 >= num)
					{
						break;
					}
				}
			},
			onProgressComplete = delegate
			{
			},
			cancelWhenDamaged = false
		}.SetDuration(10000);
		int SortVal(Chara c)
		{
			return owner.Dist(c);
		}
	}

	public override void OnCancelOrSuccess()
	{
		base.OnCancelOrSuccess();
		if (_owner == null)
		{
			return;
		}
		_owner.AddCondition<ConInvulnerable>();
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara != _owner && (chara.enemy == _owner || chara.enemy == _owner.parasite || chara.enemy == _owner.ride))
			{
				chara.SetEnemy();
				if (!(chara.ai is AI_Torture))
				{
					chara.ai.Cancel();
				}
			}
		}
	}
}
