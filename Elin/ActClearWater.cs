public class ActClearWater : Act
{
	public TraitToolWaterPot waterPot;

	public override TargetType TargetType => TargetType.Ground;

	public override CursorInfo CursorIcon => CursorSystem.Hand;

	public override bool CanPerform()
	{
		if (HasWaterSource(Act.TP) && waterPot != null)
		{
			return waterPot.owner.c_charges > 0;
		}
		return false;
	}

	public override bool Perform()
	{
		Act.CC.PlaySound("water_draw");
		waterPot.owner.SetCharge(0);
		Act.CC.Say("water_clear", Act.CC, waterPot.owner);
		return true;
	}

	public static bool HasWaterSource(Point p)
	{
		foreach (Thing thing in p.Things)
		{
			if (thing.trait is TraitWell || thing.trait is TraitBath || thing.id == "387" || thing.id == "486" || thing.id == "876" || thing.id == "867" || thing.id == "1158")
			{
				return true;
			}
		}
		return false;
	}
}
