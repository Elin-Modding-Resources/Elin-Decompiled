public class TraitGeneratorHamster : TraitGenerator
{
	public override bool UseExtra => owner.isOn;

	public override bool IsAnimeOn => owner.isOn;

	public override string IdSoundToggleOn => "switch_on_spin";

	public override string IdSoundToggleOff => "switch_off_spin";

	public override ToggleType ToggleType => ToggleType.Custom;

	public override bool IsOn => owner.isOn;

	public override bool Waterproof => true;

	public override bool CanUse(Chara c)
	{
		return owner.IsInstalled;
	}

	public override bool OnUse(Chara c)
	{
		Toggle(!owner.isOn);
		return true;
	}
}
