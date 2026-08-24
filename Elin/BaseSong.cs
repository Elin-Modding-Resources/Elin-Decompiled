public class BaseSong : BaseStance
{
	public virtual int IdAbility => -1;

	public override void OnStart()
	{
		owner.ShowEmo(Emo.happy);
		owner.mana.Validate();
	}

	public override void Tick()
	{
		if (!EClass._zone.IsRegion && !owner.HasCondition<ConSilence>())
		{
			Element element = owner.elements.GetElement(IdAbility);
			if (element != null)
			{
				owner.elements.ModExp(element.id, 20f);
			}
			if (EClass.rnd(100) == 0)
			{
				owner.elements.ModExp(241, owner.IsPC ? 10 : 100);
			}
			TickSong();
		}
	}

	public virtual void TickSong()
	{
	}
}
public class BaseSong<T> : BaseSong where T : Condition
{
	public virtual int BaseDuration => 10;

	public override void TickSong()
	{
		foreach (Chara item in owner.pos.ListCharasInRadius(owner, 4, (Chara c) => !c.IsDeadOrSleeping && ((!owner.IsPCFactionOrMinion) ? (!c.IsHostile(owner)) : c.IsPCFactionOrMinion)))
		{
			T condition = item.GetCondition<T>();
			if (condition == null)
			{
				item.AddCondition<T>(base.power);
				continue;
			}
			if (condition.power != base.power)
			{
				condition.power = base.power;
				condition.RefreshElements();
			}
			condition.SetValue(BaseDuration + owner.Evalue(1294) * 2, owner.Evalue(1294));
		}
	}
}
