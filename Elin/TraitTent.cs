public class TraitTent : TraitNewZone
{
	public override bool CanBeDropped => !(EClass._zone is Zone_Tent);

	public override bool CanBuildInTown => true;

	public override bool CreateExternalZone => true;

	public override bool CanExtendBuild => true;

	public override bool CanBeHeld => true;

	public override int UseDist => 1;

	public override void OnChangePlaceState(PlaceState state)
	{
		if (base.zone == null)
		{
			return;
		}
		if (state == PlaceState.installed)
		{
			if (!EClass._zone.children.Contains(base.zone))
			{
				EClass._zone.AddChild(base.zone);
			}
		}
		else
		{
			EClass._zone.RemoveChild(base.zone);
			owner.ChangeWeight(owner.Thing.source.weight + base.zone.GetInt(1) * 150 / 100);
		}
	}
}
