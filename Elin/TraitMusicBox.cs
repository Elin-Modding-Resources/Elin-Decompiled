public class TraitMusicBox : TraitJukeBox
{
	public override ToggleType ToggleType => ToggleType.Custom;

	public override bool CanUseFromInventory => false;

	public override bool OnUse(Chara c)
	{
		owner.refVal = 115;
		Toggle(!owner.isOn);
		return true;
	}
}
