public class AM_ViewMap : AM_BaseTileSelect
{
	public override bool AllowBuildModeShortcuts => false;

	public override bool ShowActionHint => false;

	public override bool ShowMouseoverTarget => false;

	public override bool ShowMaskedThings => false;

	public override bool enableMouseInfo => false;

	public override BaseTileSelector.SelectType selectType => BaseTileSelector.SelectType.Single;

	public override bool IsBuildMode => true;

	public override bool ShowBuildWidgets => false;

	public override BuildMenu.Mode buildMenuMode => BuildMenu.Mode.None;

	public override void OnActivate()
	{
		EClass.ui.layerFloat.SetActive(enable: true);
		foreach (Layer layer in EClass.ui.layerFloat.layers)
		{
			if (!(layer is LayerTreasureMap))
			{
				layer.SetActive(enable: false);
			}
		}
	}

	public override void OnUpdateInput()
	{
		base.OnUpdateInput();
		if (EInput.middleMouse.clicked)
		{
			EClass.scene.ToggleRoof();
		}
	}

	public override void OnDeactivate()
	{
		foreach (Layer layer in EClass.ui.layerFloat.layers)
		{
			layer.SetActive(enable: true);
		}
	}

	public override void OnCancel()
	{
		Deactivate();
	}
}
