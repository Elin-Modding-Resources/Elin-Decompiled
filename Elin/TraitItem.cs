public class TraitItem : Trait
{
	public virtual bool CanUseFromInventory => true;

	public virtual bool CanUseInUserZone => !owner.isNPCProperty;

	public override bool CanUse(Chara c)
	{
		if (!CanUseFromInventory && !owner.IsInstalled)
		{
			return false;
		}
		if (!CanUseInUserZone && EClass._zone.IsUserZone && owner.isNPCProperty)
		{
			return false;
		}
		if (Electricity < 0)
		{
			return owner.isOn;
		}
		return true;
	}

	public override void WriteNote(UINote n, bool identified)
	{
		if (!langNote.IsEmpty())
		{
			n.Space(20);
			n.AddText("NoteText_enc", langNote.lang(), FontColor.Good);
		}
	}
}
