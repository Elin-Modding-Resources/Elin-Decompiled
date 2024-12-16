using UnityEngine;

public class AM_Cinema : AM_BaseTileSelect
{
	public SceneProfile profile;

	public Point destPos;

	public Point center = new Point();

	public override bool IsBuildMode => true;

	public override BaseTileSelector.SelectType selectType => BaseTileSelector.SelectType.None;

	public override bool ShowActionHint => false;

	public override bool ShowMouseoverTarget => false;

	public override bool enableMouseInfo => false;

	public override bool ShowMaskedThings => false;

	public CinemaConfig conf => EClass.player.cinemaConfig;

	public override void OnActivate()
	{
		if (!profile)
		{
			profile = SceneProfile.Load("cinema");
		}
		EClass.ui.widgets.Activate("ArtTool");
		destPos = null;
	}

	public override void OnDeactivate()
	{
		EClass.ui.widgets.DeactivateWidget("ArtTool");
		EClass.scene.camSupport.grading.cinemaBrightness = 0f;
		EClass.core.config.ApplyGrading();
	}

	public override void OnUpdateInput()
	{
		if (EInput.leftMouse.clicked && !EClass.ui.isPointerOverUI)
		{
			SE.ClickGeneral();
			EClass.ui.ToggleCanvas();
		}
		if (EInput.rightMouse.down || conf.speed == 0)
		{
			destPos = null;
			return;
		}
		Vector3Int vector3Int = EClass.screen.grid.WorldToCell(EClass.screen.position);
		center.Set(-vector3Int.y, vector3Int.x - 1);
		center.Clamp();
		if (destPos == null || destPos.Distance(center) <= 2)
		{
			destPos = new Point();
			for (int i = 0; i < 10000; i++)
			{
				destPos.x = EClass._map.bounds.x + EClass.rnd(EClass._map.bounds.Width);
				destPos.z = EClass._map.bounds.z + EClass.rnd(EClass._map.bounds.Height);
				if (center.Distance(destPos) > EClass._map.bounds.Width)
				{
					break;
				}
			}
		}
		Vector3 normalized = (destPos.PositionCenter() - center.PositionCenter()).normalized;
		EClass.screen.position += normalized * Core.delta * conf.speed * 0.5f;
	}
}
