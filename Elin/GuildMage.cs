public class GuildMage : Guild
{
	public override QuestGuild Quest => EClass.game.quests.Get<QuestGuildMage>();

	public override bool IsCurrentZone
	{
		get
		{
			if (EClass._zone.id == "lumiest")
			{
				return EClass._zone.lv == -1;
			}
			return false;
		}
	}

	public int BuySpellbookPrice(int a)
	{
		if (!base.IsMember)
		{
			return a;
		}
		return a * 100 / (120 + relation.rank / 2);
	}

	public int DomainBonus()
	{
		if (relation.rank < 6)
		{
			return 0;
		}
		return 1;
	}
}
