public class InvOwnerGene : InvOwnerDraglet
{
	public Chara tg;

	public override ProcessType processType => ProcessType.None;

	public override string langTransfer => "invGene";

	public InvOwnerGene(Card owner = null, Chara _tg = null)
		: base(owner, null, CurrencyType.None)
	{
		tg = _tg;
		count = 1;
	}

	public override bool ShouldShowGuide(Thing t)
	{
		if (t.c_DNA != null)
		{
			return tg.feat >= t.c_DNA.cost;
		}
		return false;
	}

	public override void _OnProcess(Thing t)
	{
		DNA.Type type = t.c_DNA.type;
		if (type != 0 && tg.c_genes != null)
		{
			int num = t.c_DNA.slot;
			if (num > 1 && tg.HasElement(1237))
			{
				num--;
			}
			if (tg.CurrentGeneSlot + num > tg.MaxGeneSlot)
			{
				SE.Beep();
				Msg.Say("tooManyGene", tg);
				return;
			}
		}
		if (type == DNA.Type.Brain)
		{
			if (tg.c_genes != null)
			{
				foreach (DNA item in tg.c_genes.items)
				{
					if (item.type == DNA.Type.Brain)
					{
						SE.Beep();
						Msg.Say("invalidGeneBrain", tg);
						return;
					}
				}
			}
		}
		else
		{
			Element invalidFeat = t.c_DNA.GetInvalidFeat(tg);
			if (invalidFeat != null)
			{
				SE.Beep();
				Msg.Say("invalidGeneFeat", tg, invalidFeat.Name.ToTitleCase());
				return;
			}
			Element invalidAction = t.c_DNA.GetInvalidAction(tg);
			if (invalidAction != null)
			{
				SE.Beep();
				Msg.Say("invalidGeneAction", tg, invalidAction.Name.ToTitleCase());
				return;
			}
		}
		SE.Play("mutation");
		tg.PlayEffect("identify");
		Msg.Say("gene_modify", tg, t);
		tg.AddCard(t);
		ConSuspend condition = tg.GetCondition<ConSuspend>();
		condition.gene = t;
		condition.duration = t.c_DNA.GetDurationHour();
		condition.dateFinish = EClass.world.date.GetRaw(condition.duration);
	}

	public override void OnWriteNote(Thing t, UINote n)
	{
		if (ShouldShowGuide(t))
		{
			t.c_DNA.WriteNoteExtra(n, tg);
		}
	}
}
