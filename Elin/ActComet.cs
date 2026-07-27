public class ActComet : Spell
{
	public override bool CanAutofire => true;

	public override bool CanPressRepeat => true;

	public override bool CanRapidFire => true;

	public override float RapidDelay => 0.3f;

	public override bool ShowMapHighlight => true;

	public override void OnMarkMapHighlights()
	{
		if (!EClass.scene.mouseTarget.pos.IsValid || !EClass.scene.mouseTarget.pos.IsSync || EClass.scene.mouseTarget.pos.IsBlocked)
		{
			return;
		}
		foreach (Point item in EClass._map.ListPointsInCircle(EClass.scene.mouseTarget.pos, 7f))
		{
			item.SetHighlight(8);
		}
	}

	public override bool CanPerform()
	{
		if (Act.TP.IsBlocked)
		{
			return false;
		}
		return base.CanPerform();
	}
}
