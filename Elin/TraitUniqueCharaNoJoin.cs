public class TraitUniqueCharaNoJoin : TraitUniqueChara
{
	public override bool CanInvite => false;

	public override bool CanChangeAffinity
	{
		get
		{
			if (base.owner.Chara.source.recruitItems.IsEmpty())
			{
				return base.owner.IsPCFaction;
			}
			return true;
		}
	}
}
