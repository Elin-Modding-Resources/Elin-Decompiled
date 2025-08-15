public class InvOwnerMod : InvOwnerDraglet
{
	public override bool CanTargetAlly => true;

	public override ProcessType processType => ProcessType.None;

	public override string langTransfer => "invMod";

	public static bool IsValidRangedMod(Thing t, SourceElement.Row row)
	{
		if (t.trait is TraitToolRangeCane && !row.tag.Contains("modCane"))
		{
			return false;
		}
		if ((t.trait is TraitToolRangeBow || t.trait is TraitToolRangeCrossbow) && row.id == 601)
		{
			return false;
		}
		return true;
	}

	public InvOwnerMod(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.None)
		: base(owner, container, _currency)
	{
		count = 1;
	}

	public override bool ShouldShowGuide(Thing t)
	{
		TraitMod traitMod = owner.trait as TraitMod;
		if (traitMod is TraitRune)
		{
			if (!t.CanAddRune(traitMod))
			{
				return false;
			}
			return true;
		}
		if (!IsValidRangedMod(t, traitMod.source))
		{
			return false;
		}
		if (t.sockets != null)
		{
			foreach (int socket in t.sockets)
			{
				if (socket == 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void _OnProcess(Thing t)
	{
		Msg.Say("modded", t, owner);
		if (owner.trait is TraitRune)
		{
			SE.Play("intonation");
			EClass.pc.PlayEffect("intonation");
			t.AddRune(owner);
			owner.ModNum(-1);
		}
		else
		{
			SE.Play("reloaded");
			EClass.pc.PlayEffect("identify");
			t.ApplySocket(owner.Thing);
		}
	}
}
