public class TraitUsuihon : TraitErohon
{
	public override bool UseSourceValue => false;

	public override int Difficulty => 30;

	public override Type BookType => Type.Dojin;

	public override int IdSkin => owner.idSkin;
}
