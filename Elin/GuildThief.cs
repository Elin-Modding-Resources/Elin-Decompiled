public class GuildThief : Guild
{
	public override QuestGuild Quest => EClass.game.quests.Get<QuestGuildThief>();

	public override bool IsCurrentZone
	{
		get
		{
			if (EClass._zone.id == "derphy")
			{
				return EClass._zone.lv == -1;
			}
			return false;
		}
	}

	public int SellStolenPrice(int a)
	{
		if (!base.IsMember)
		{
			return a;
		}
		return a * 100 / (190 - relation.rank * 2);
	}

	public int FurnitureBonus()
	{
		if (relation.rank < 6)
		{
			return 1;
		}
		return 2;
	}
}
