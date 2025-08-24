public class TraitGuildDoorman : TraitUniqueGuildPersonnel
{
	public override AI_Idle.Behaviour IdleBehaviour => AI_Idle.Behaviour.NoMove;

	public virtual bool IsGuildMember => false;

	public override bool CanBePushed
	{
		get
		{
			if (!IsGuildMember)
			{
				return base.owner.IsPCFaction;
			}
			return true;
		}
	}

	public virtual void GiveTrial()
	{
	}

	public virtual void OnJoinGuild()
	{
	}
}
