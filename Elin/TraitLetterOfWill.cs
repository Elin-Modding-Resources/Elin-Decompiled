public class TraitLetterOfWill : Trait
{
	public override void WriteNote(UINote n, bool identified)
	{
		base.WriteNote(n, identified);
		n.AddText("NoteText_enc", "isPreventDeathPanalty", FontColor.Good);
		n.AddText("NoteText_enc", "isGraveSkin", FontColor.Good);
	}
}
