using System.Collections.Generic;

public class AI_PassTime : AIAct
{
	public enum Type
	{
		passTime,
		meditate,
		selfHarm
	}

	public Thing target;

	public bool startedFull;

	public virtual Type type => Type.passTime;

	public virtual int exp => 0;

	public virtual int turns => 3000;

	public override bool IsAutoTurn => true;

	public override bool LocalAct => type == Type.selfHarm;

	public override TargetType TargetType
	{
		get
		{
			if (type != Type.passTime)
			{
				return TargetType.Self;
			}
			return TargetType.Any;
		}
	}

	public bool IsFull
	{
		get
		{
			if (owner != null)
			{
				if (owner.hp == owner.MaxHP)
				{
					return owner.mana.value == owner.mana.max;
				}
				return false;
			}
			return true;
		}
	}

	public override bool CanManualCancel()
	{
		return true;
	}

	public override MultiSprite GetStateIcon()
	{
		return type switch
		{
			Type.meditate => EClass.core.refs.stateIcons.meditation, 
			Type.selfHarm => EClass.core.refs.stateIcons.selfharm, 
			_ => base.GetStateIcon(), 
		};
	}

	public override void OnStart()
	{
		startedFull = IsFull;
	}

	public override IEnumerable<Status> Run()
	{
		if (target != null)
		{
			yield return DoGoto(target.pos);
		}
		owner.Say(type.ToString() + "_start", owner);
		for (int i = 0; i < turns; i++)
		{
			if (exp > 0 && i % 5 == 0)
			{
				owner.ModExp(GetType().Name, exp);
			}
			if (type == Type.selfHarm)
			{
				if (EClass.rnd(10) == 0)
				{
					owner.AddCondition<ConBleed>(50);
				}
				else if (EClass.rnd(10) == 0)
				{
					owner.DamageHP(5 + EClass.rnd(5));
					if (owner != null)
					{
						owner.Teleport(ActEffect.GetTeleportPos(EClass.pc.pos));
					}
				}
			}
			else
			{
				foreach (Chara member in EClass.pc.party.members)
				{
					bool flag = false;
					foreach (Condition condition in member.conditions)
					{
						if (condition.PreventRegen)
						{
							flag = true;
						}
					}
					if (!flag)
					{
						int num = 1 + EClass.pc.Evalue(6003) / 5;
						if (EClass.rnd(3) == 0)
						{
							member.HealHP(num * (member.IsPC ? 1 : 2));
						}
						member.mana.Mod(num * (member.IsPC ? 1 : 2));
					}
				}
				if (i == 50 && EClass.pc.pos.IsHotSpring && (!EClass.pc.IsPCC || EClass.pc.pccData.state == PCCState.Undie))
				{
					int p = (EClass._zone.elements.Has(3701) ? 150 : 100);
					foreach (Chara member2 in EClass.pc.party.members)
					{
						member2.AddCondition<ConHotspring>(p)?.SetPerfume();
					}
				}
				if (!startedFull && IsFull)
				{
					yield return Success();
				}
				if (!EClass.debug.enable && owner.CanSleep() && EClass.rnd(10) == 0)
				{
					owner.Sleep();
					yield return Success();
				}
			}
			yield return KeepRunning();
		}
		if (owner != null)
		{
			owner.Say(type.ToString() + "_end", owner);
		}
	}

	public override void OnCancel()
	{
		if (owner != null)
		{
			owner.Say(type.ToString() + "_end", owner);
		}
	}
}
