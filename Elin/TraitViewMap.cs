public class TraitViewMap : TraitItem
{
	public override bool CanUseInUserZone => true;

	public override bool OnUse(Chara c)
	{
		ActionMode.ViewMap.Activate();
		return false;
	}
}
