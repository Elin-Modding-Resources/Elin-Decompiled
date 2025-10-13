public class TraitItemToggle : TraitItem
{
	public override bool IsAnimeOn => owner.isOn;

	public override ToggleType ToggleType => ToggleType.Custom;

	public override bool CanUseFromInventory => false;

	public override bool OnUse(Chara c)
	{
		Toggle(!owner.isOn);
		return true;
	}
}
