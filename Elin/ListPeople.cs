public class ListPeople : BaseListPeople
{
	public override string TextTab
	{
		get
		{
			if (textTab.IsEmpty())
			{
				FactionBranch factionBranch = EClass.Branch ?? EClass.pc.homeBranch;
				switch (memberType)
				{
				case FactionMemberType.Default:
					return "residents".lang() + " (" + factionBranch.CountMembers(FactionMemberType.Default) + "/" + factionBranch.MaxPopulation + ")";
				case FactionMemberType.Livestock:
					return "livestock".lang() + " (" + factionBranch.CountMembers(FactionMemberType.Livestock) + ")";
				case FactionMemberType.Guest:
					return "guests".lang() + " (" + factionBranch.CountGuests() + ")";
				}
			}
			return textTab;
		}
	}

	public override string TextHeader => memberType switch
	{
		FactionMemberType.Default => "_listOf".lang("residents".lang()), 
		FactionMemberType.Livestock => "_listOf".lang("livestock".lang()), 
		FactionMemberType.Guest => "_listOf".lang("guests".lang()), 
		_ => textTab, 
	};

	public override bool ShowCharaSheet => true;

	public override bool ShowGoto => true;

	public override bool ShowHome => true;

	public override bool ShowShowMode => true;
}
