using Newtonsoft.Json;

public class ConDeathSentense : BaseDebuff
{
	[JsonProperty]
	public int uidGiver;

	[JsonProperty]
	public bool euthanasia;

	public override Emo2 EmoIcon => Emo2.death;

	public override void Tick()
	{
		if (TryRemove())
		{
			return;
		}
		Mod(-1);
		if (base.value <= 0 && owner.IsAliveInCurrentZone)
		{
			owner.Say("death_sentense", owner);
			if (!euthanasia && owner.IsPowerful)
			{
				owner.DamageHP(owner.MaxHP / (EClass.debug.enable ? 1 : 13) + 1, AttackSource.DeathSentense);
			}
			else
			{
				owner.Die(null, null, euthanasia ? AttackSource.Euthanasia : AttackSource.DeathSentense);
			}
		}
	}

	public bool TryRemove()
	{
		if (uidGiver != 0 && EClass._map.FindChara(uidGiver) == null)
		{
			Kill();
			return true;
		}
		return false;
	}

	public void SetChara(Chara c)
	{
		if (c != null)
		{
			uidGiver = c.uid;
		}
	}

	public override void OnRemoved()
	{
		base.OnRemoved();
	}
}
