public class TraitRoomPlate : TraitBoard
{
	public override bool IsHomeItem => true;

	public override bool CanBeMasked => true;

	public override bool ShouldTryRefreshRoom => true;

	public override bool MaskOnBuild => true;

	public override bool ShowContextOnPick => true;

	public AreaData areaData
	{
		get
		{
			return owner.GetObj<AreaData>(3) ?? owner.SetObj<AreaData>(3, new AreaData());
		}
		set
		{
			owner.SetObj(3, value);
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if ((!EClass.debug.enable && !EClass._zone.IsPCFactionOrTent) || !owner.IsInstalled || owner.pos.cell.room == null)
		{
			return;
		}
		foreach (BaseArea.Interaction a in owner.pos.cell.room.ListInteractions())
		{
			p.TrySetAct(a.text, delegate
			{
				a.action();
				EClass._map.rooms.RefreshAll();
				return false;
			}, owner);
		}
	}
}
