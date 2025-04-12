public class GuildMerchant : Guild
{
	public override QuestGuild Quest => EClass.game.quests.Get<QuestGuildMerchant>();

	public override bool IsCurrentZone => EClass._zone.id == "guild_merchant";

	public long InvestPrice(long a)
	{
		if (!base.IsMember)
		{
			return a;
		}
		return a * 100 / (110 + relation.rank / 2);
	}
}
