public class TraitLightSource : TraitTorch
{
	public int LightRadius => GetParam(1).ToInt();

	public override void OnListInteraction(InvOwner.ListInteraction list, ButtonGrid b, bool context)
	{
		base.OnListInteraction(list, b, context);
		if (!context)
		{
			return;
		}
		list.Add("customBrightness", 300, delegate
		{
			UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
			uIContextMenu.AddSlider("brightness", (float a) => a.ToString() ?? "", EClass.player.customLightMod, delegate(float b)
			{
				EClass.player.customLightMod = (int)b;
				EClass.pc.RecalculateFOV();
			}, 1f, 6f, isInt: true, hideOther: false);
			uIContextMenu.Show();
		});
	}
}
