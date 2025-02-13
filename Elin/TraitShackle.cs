using UnityEngine;

public class TraitShackle : Trait
{
	public virtual Vector3 GetRestrainPos => EClass.setting.render.posShackle;

	public virtual AnimeID animeId => AnimeID.Shiver;

	public override bool CanBeHeld => !IsRestraining();

	public override string LangUse => "ActRestrain";

	public virtual bool AllowTraining => true;

	public override bool CanStackTo(Thing to)
	{
		return false;
	}

	public override bool CanUse(Chara c)
	{
		if (EClass._zone.IsPCFaction && owner.IsInstalled && !EClass.pc.isRestrained)
		{
			if (owner.pos.FirstChara != null)
			{
				return !owner.pos.FirstChara.isRestrained;
			}
			return true;
		}
		return false;
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		p.TrySetAct(new ActRestrain
		{
			shackle = this
		}, p.pos.FirstChara);
	}

	public override bool OnUse(Chara c)
	{
		c.MoveImmediate(owner.pos);
		ActRestrain actRestrain = new ActRestrain();
		actRestrain.shackle = this;
		actRestrain.Perform(c, c, c.pos);
		return true;
	}

	public void Restrain(Card tc, bool msg = false)
	{
		tc.isRestrained = true;
		owner.c_uidRefCard = tc.uid;
		if (msg)
		{
			tc.Say("restrained", tc);
		}
	}

	public bool IsRestraining()
	{
		if (!owner.IsInstalled || owner.pos == null || EClass.player.simulatingZone)
		{
			return false;
		}
		foreach (Chara chara in owner.pos.Charas)
		{
			if (chara.isRestrained && owner.c_uidRefCard == chara.uid)
			{
				return true;
			}
		}
		foreach (Chara chara2 in EClass._map.charas)
		{
			if (chara2.isChara && chara2.isRestrained && chara2.pos.Equals(owner.pos))
			{
				owner.c_uidRefCard = chara2.uid;
				return true;
			}
		}
		return false;
	}
}
