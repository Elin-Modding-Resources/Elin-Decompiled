public class TraitTent : TraitNewZone
{
	public override bool CanBeDropped => !(EClass._zone is Zone_Tent);

	public override bool CanBuildInTown => true;

	public override bool CreateExternalZone => true;

	public override bool CanExtendBuild => true;

	public override bool CanBeHeld => true;

	public override bool CanBeShipped => false;

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
			return;
		}
		EClass._zone.RemoveChild(base.zone);
		int num = owner.Thing.source.weight + base.zone.GetInt(1) * 150 / 100;
		if (owner.HasElement(652))
		{
			num = num * 100 / 110;
		}
		owner.ChangeWeight(num);
	}

	public override void SetName(ref string s)
	{
		if (base.zone != null && !base.zone.name.IsEmpty())
		{
			s = s + "(" + base.zone.name + ")";
		}
	}
}
