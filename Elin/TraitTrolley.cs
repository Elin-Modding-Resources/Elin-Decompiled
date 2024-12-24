public class TraitTrolley : TraitFloorSwitch
{
	public virtual bool HideChara => false;

	public override bool CanManucalActivate
	{
		get
		{
			if (EClass.pc.pos.Equals(owner.pos))
			{
				return CanActivate(EClass.pc);
			}
			return false;
		}
	}

	public virtual float FadeDuration
	{
		get
		{
			if (owner.idSkin != 7)
			{
				return 1f;
			}
			return 0.5f;
		}
	}

	public bool CanActivate(Chara c)
	{
		if (c.host != null || c.IsInCombat)
		{
			return false;
		}
		if (!owner.pos.HasRail || !owner.IsInstalled)
		{
			return false;
		}
		if (c.IsPCFaction && !c.IsPC && EClass._zone is Zone_Dungeon)
		{
			return false;
		}
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.ai is AI_Trolley { IsRunning: not false } aI_Trolley && aI_Trolley.trolley == this)
			{
				return false;
			}
		}
		return true;
	}

	public override void OnActivateTrap(Chara c)
	{
		TraitSwitch.haltMove = false;
		if (CanActivate(c))
		{
			c.SetAI(new AI_Trolley
			{
				trolley = this
			});
		}
	}

	public virtual string GetIdSound()
	{
		if (owner.idSkin != 7)
		{
			return "ride_trolley";
		}
		return "ride_bike";
	}
}
