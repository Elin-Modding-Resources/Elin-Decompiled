public class TraitMusicBox : TraitJukeBox
{
	public override ToggleType ToggleType => ToggleType.Custom;

	public override string IdSoundToggleOn => "switch_on_musicbox";

	public override string IdSoundToggleOff => "switch_off_musicbox";

	public override bool CanUseFromInventory => false;

	public override bool OnUse(Chara c)
	{
		owner.refVal = GetParam(1).ToInt();
		Toggle(!owner.isOn);
		return true;
	}
}
