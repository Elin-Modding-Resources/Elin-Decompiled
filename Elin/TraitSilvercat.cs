public class TraitSilvercat : TraitChara
{
	public override bool CanBeTamed => false;

	public override bool CanInvite => false;

	public override bool IsCountAsResident => true;

	public override bool RemoveGlobalOnBanish => true;
}
