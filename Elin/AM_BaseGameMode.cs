public class AM_BaseGameMode : ActionMode
{
	public bool isMouseOnMap;

	public override AreaHighlightMode AreaHihlight => AreaHighlightMode.None;

	public override bool ShowActionHint => false;

	public override bool HideSubWidgets => false;

	public override BaseTileSelector.HitType hitType => BaseTileSelector.HitType.Default;

	public override BaseTileSelector.SelectType selectType => BaseTileSelector.SelectType.Single;

	public override BaseGameScreen TargetGameScreen
	{
		get
		{
			if (!EClass._zone.IsRegion)
			{
				return EClass.scene.screenElin;
			}
			return EClass.scene.screenElona;
		}
	}

	protected Point hit => Scene.HitPoint;

	public override void OnUpdateInput()
	{
		isMouseOnMap = !EClass.ui.isPointerOverUI && hit.IsValid;
		if (EInput.leftMouse.down && EClass.ui.isPointerOverUI)
		{
			EInput.leftMouse.Consume();
		}
		else
		{
			if (!EClass.core.IsGameStarted)
			{
				return;
			}
			switch (EInput.action)
			{
			case EAction.Cancel:
				if (!WidgetSearch.Instance)
				{
					HotItemContext.Show("system", EInput.uiMousePosition);
				}
				break;
			case EAction.Help:
				LayerHelp.Toggle("general", "1");
				break;
			case EAction.Log:
				WidgetMainText.ToggleLog();
				break;
			}
			if (EClass.pc.currentZone.IsActiveZone)
			{
				_OnUpdateInput();
			}
		}
	}

	public override string GetHintText()
	{
		return null;
	}

	public virtual void _OnUpdateInput()
	{
	}

	public override void OnCancel()
	{
	}

	public override HitResult HitTest(Point point, Point start)
	{
		return HitResult.Default;
	}
}
