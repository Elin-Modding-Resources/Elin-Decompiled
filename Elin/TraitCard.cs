public class TraitCard : TraitFigure
{
	public override bool ShowShadow => false;

	public override int GetMatColor()
	{
		return -3;
	}

	public override void OnListInteraction(InvOwner.ListInteraction list, ButtonGrid b, bool context)
	{
		base.OnListInteraction(list, b, context);
		if (context)
		{
			list.Add("invCollect", 150, delegate
			{
				ContentCodex.Collect(owner as Thing);
			});
		}
	}
}
