public class TraitErohon : TraitBaseSpellbook
{
	public override int Difficulty => (EClass.sources.charas.map.TryGetValue(owner.c_idRefName)?.LV ?? 0) / 2 + 10;

	public override Type BookType => Type.Ero;

	public override int eleParent => 75;

	public override int IdSkin => (EClass.sources.charas.map.TryGetValue(owner.c_idRefName)?._index ?? 1) % (owner.sourceCard.skins.Length + 1);
}
