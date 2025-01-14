public class GuildFighter : Guild
{
	public override QuestGuild Quest => EClass.game.quests.Get<QuestGuildFighter>();

	public override bool IsCurrentZone
	{
		get
		{
			if (EClass._zone.id == "kapul")
			{
				return EClass._zone.lv == -1;
			}
			return false;
		}
	}

	public int ServicePrice(int a)
	{
		if (!base.IsMember)
		{
			return a;
		}
		return a * 100 / (125 + relation.rank * 2);
	}

	public bool CanGiveContribution(Chara c)
	{
		if (c.IsUnique || c.rarity < Rarity.Legendary)
		{
			return false;
		}
		if (c.Chara.OriginalHostility != Hostility.Enemy && c.c_bossType != BossType.Evolved)
		{
			return false;
		}
		return true;
	}

	public bool ShowBounty(Chara c)
	{
		return true;
	}

	public bool HasBounty(Chara c)
	{
		if (!CanGiveContribution(c))
		{
			return false;
		}
		if (relation.rank < 4)
		{
			return false;
		}
		if (c.uid % 2 != 0)
		{
			return false;
		}
		return true;
	}
}
