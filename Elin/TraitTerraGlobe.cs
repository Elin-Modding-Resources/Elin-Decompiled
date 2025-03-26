public class TraitTerraGlobe : TraitItem
{
	public override bool UseExtra => owner.isOn;

	public override bool IsAnimeOn => owner.isOn;

	public override string IdSoundToggleOn => "switch_on_spin";

	public override string IdSoundToggleOff => "switch_off_spin";

	public override ToggleType ToggleType => ToggleType.Custom;

	public override bool CanUseFromInventory => false;

	public override bool OnUse(Chara c)
	{
		Toggle(!owner.isOn);
		return true;
	}
}
