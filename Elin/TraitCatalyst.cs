public class TraitCatalyst : TraitTool
{
	private Act _act;

	public Act act => _act ?? (_act = CreateAct());

	public virtual bool IsRod => true;

	public virtual Act CreateAct()
	{
		return ACT.Create(owner.Thing.sourceCard.vals[0].IsEmpty("AI_SelfHarm"));
	}

	public override bool CanUse(Chara c)
	{
		if (act.LocalAct && EClass._zone.IsRegion)
		{
			return false;
		}
		return act.TargetType.CanSelectSelf;
	}

	public override bool OnUse(Chara c)
	{
		return ButtonAbility.TryUse(act, null, null, owner);
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		Card tg = null;
		p.pos.ListCards().ForeachReverse(delegate(Card a)
		{
			if (EClass.pc.CanSee(a) && !a.HasHost && act.IsValidTC(a))
			{
				tg = a;
				return true;
			}
			return false;
		});
		if (!act.CanPerform(EClass.pc, tg, p.pos))
		{
			return;
		}
		p.TrySetAct(((EClass.pc.elements.GetElement(act.id) as Act) ?? act).GetText(), delegate
		{
			if (tg != null && !tg.ExistsOnMap)
			{
				return false;
			}
			Point point = ((tg != null && tg.ExistsOnMap) ? tg.pos : p.pos);
			if (!act.CanPerform(EClass.pc, tg, point))
			{
				return false;
			}
			return (p.performed && act.CanPressRepeat) ? EClass.pc.UseAbility(act.source.alias, tg, point) : ButtonAbility.TryUse(act, tg, point, owner);
		}, tg, null, -1, EClass._zone.IsCrime(EClass.pc, act), act.LocalAct, act.CanPressRepeat);
	}
}
