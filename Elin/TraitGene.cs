public class TraitGene : Trait
{
	public override bool CanStack => false;

	public override bool CanBeStolen => false;

	public override bool CanBeDestroyed => false;

	public override float DropChance => 1f;

	public override string GetName()
	{
		if (owner.c_DNA == null || owner.c_DNA.type == DNA.Type.Default || owner.c_DNA.type == DNA.Type.Brain)
		{
			return base.GetName();
		}
		return ("dna_" + owner.c_DNA.type).lang() + Lang.space + owner.sourceCard.GetText();
	}

	public override void WriteNote(UINote n, bool identified)
	{
		if (owner.c_DNA != null)
		{
			bool flag = EClass.pc.HasElement(1274) && !LayerDragGrid.Instance;
			if (owner.c_DNA.cost > 0)
			{
				n.AddText("NoteText_enc", "isCostFeatPoint".lang((flag ? (owner.c_DNA.cost * EClass.pc.GeneCostMTP / 100 + " (" + owner.c_DNA.cost + ")") : ((object)owner.c_DNA.cost))?.ToString() ?? ""));
			}
			if (EClass.debug.showExtra)
			{
				n.AddText("NoteText_enc", "duration:" + owner.c_DNA.GetDurationHour());
			}
			owner.c_DNA.WriteNote(n, flag ? EClass.pc : null);
			if (flag)
			{
				owner.c_DNA.WriteNoteExtra(n, EClass.pc);
			}
		}
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		if (!p.pos.Equals(EClass.pc.pos) || !EClass.pc.HasElement(1274))
		{
			return;
		}
		Element invalidFeat = owner.c_DNA.GetInvalidFeat(EClass.pc);
		Element invalidAction = owner.c_DNA.GetInvalidAction(EClass.pc);
		if (owner.c_DNA.cost * EClass.pc.GeneCostMTP / 100 > EClass.pc.feat)
		{
			p.TrySetAct("invFood", delegate
			{
				SE.Beep();
				Msg.Say("notEnoughFeatPoint");
				return false;
			});
		}
		else if (invalidFeat != null)
		{
			p.TrySetAct("invFood", delegate
			{
				SE.Beep();
				Msg.Say("invalidGeneFeat", EClass.pc, invalidFeat.Name.ToTitleCase());
				return false;
			});
		}
		else if (invalidAction != null)
		{
			p.TrySetAct("invFood", delegate
			{
				SE.Beep();
				Msg.Say("invalidGeneAction", EClass.pc, invalidAction.Name.ToTitleCase());
				return false;
			});
		}
		else
		{
			p.TrySetAct(new AI_Eat
			{
				target = owner
			});
		}
	}

	public override int GetValue()
	{
		return base.GetValue() * ((owner.c_DNA == null) ? 100 : (100 + owner.c_DNA.cost * 10)) / 100;
	}
}
