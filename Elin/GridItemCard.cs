public class GridItemCard : GridItem
{
	public Card c;

	public override void SetButton(ButtonGrid b)
	{
		b.mainText.text = c.Num.ToShortNumber();
		c.SetImage(b.icon);
		b.SetTooltip("note", delegate(UITooltip t)
		{
			c.WriteNote(t.note);
		});
	}

	public override void OnClick(ButtonGrid b)
	{
		EClass.ui.AddLayer<LayerInfo>().Set(c);
	}

	public override void AutoAdd()
	{
		EClass.player.AddInventory(c);
	}
}
