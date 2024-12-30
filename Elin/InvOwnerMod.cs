public class InvOwnerMod : InvOwnerDraglet
{
	public override ProcessType processType => ProcessType.None;

	public override string langTransfer => "invMod";

	public static bool IsValidRuneMod(Thing t, SourceElement.Row row, string idMat)
	{
		if (idMat == "adamantite")
		{
			return t.category.slot == 35;
		}
		return false;
	}

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
			if (!IsValidRuneMod(t, traitMod.source, owner.material.alias))
			{
				return false;
			}
		}
		else if (!IsValidRangedMod(t, traitMod.source))
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
		SE.Play("reloaded");
		EClass.pc.PlayEffect("identify");
		Msg.Say("modded", t, owner);
		t.ApplySocket(owner.Thing);
	}
}
