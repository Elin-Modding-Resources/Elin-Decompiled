public class InvOwnerCraft : InvOwnerDraglet
{
	public TraitCrafter crafter;

	public override bool CanTargetAlly => crafter is TraitToolTalisman;

	public override bool ShowFuel => crafter.IsRequireFuel;

	public override string langTransfer => crafter.CrafterTitle;

	public override int numDragGrid => crafter.numIng;

	public override bool DenyImportant => false;

	public override bool AllowStockIngredients
	{
		get
		{
			if (!EClass._zone.IsPCFaction)
			{
				return EClass._zone is Zone_Tent;
			}
			return true;
		}
	}

	public InvOwnerCraft(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.None)
		: base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		return crafter.IsCraftIngredient(t, base.dragGrid.currentIndex);
	}

	public override void _OnProcess(Thing t)
	{
		t.PlaySoundDrop(spatial: false);
		TryStartCraft();
	}

	public override void OnAfterRefuel()
	{
		TryStartCraft();
	}

	public void TryStartCraft()
	{
		for (int i = 0; i < numDragGrid; i++)
		{
			if (base.dragGrid.buttons[i].Card == null)
			{
				return;
			}
		}
		if (!owner.trait.IsFuelEnough(1, base.dragGrid.GetTargets()))
		{
			Msg.Say("notEnoughFuel");
			return;
		}
		EClass.pc.SetAI(new AI_UseCrafter
		{
			crafter = crafter,
			layer = base.dragGrid
		});
	}
}
