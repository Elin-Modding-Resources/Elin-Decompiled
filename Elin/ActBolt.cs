using System.Collections.Generic;

public class ActBolt : Spell
{
	public override bool CanAutofire => true;

	public override bool CanPressRepeat => true;

	public override bool CanRapidFire => true;

	public override float RapidDelay => 0.25f;

	public override bool ShowMapHighlight => true;

	public override void OnMarkMapHighlights()
	{
		if (!EClass.scene.mouseTarget.pos.IsValid)
		{
			return;
		}
		List<Point> list = EClass._map.ListPointsInLine(EClass.pc.pos, EClass.scene.mouseTarget.pos, 10);
		if (list.Count == 0)
		{
			list.Add(Act.CC.pos.Copy());
		}
		foreach (Point item in list)
		{
			item.SetHighlight(8);
		}
	}
}
