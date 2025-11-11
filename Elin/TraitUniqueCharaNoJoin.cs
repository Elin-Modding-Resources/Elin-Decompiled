public class TraitUniqueCharaNoJoin : TraitUniqueChara
{
	public override bool CanInvite => false;

	public override bool CanChangeAffinity => !base.owner.Chara.source.recruitItems.IsEmpty();
}
