public class ActPray : Act
{
	public override TargetType TargetType => TargetType.Self;

	public override bool LocalAct => false;

	public override bool Perform()
	{
		return TryPray(Act.CC);
	}

	public static bool TryPray(Chara c, bool passive = false)
	{
		if (!c.HasCondition<ConWrath>())
		{
			Thing thing = c.things.Find<TraitPunishBall>();
			if (thing != null)
			{
				thing.Destroy();
				c.PlaySound("pray");
				c.PlayEffect("revive");
				c.Say("piety2", c);
				return true;
			}
		}
		if (c.faith.IsEyth)
		{
			if (passive)
			{
				if (c.Evalue(1655) < 2 || EClass.pc.party.members.Count <= 1)
				{
					return true;
				}
				foreach (Chara member in EClass.pc.party.members)
				{
					if (member != EClass.pc)
					{
						member.Say("pray2", member, member.faith.Name);
						member.ModExp(306, 200);
					}
				}
			}
			else
			{
				c.Say("pray", c);
				c.PlaySound("pray_ignore");
			}
			return true;
		}
		foreach (Chara member2 in EClass.pc.party.members)
		{
			if (member2 == EClass.pc || (passive && c.Evalue(1655) >= 2))
			{
				member2.Say("pray2", member2, member2.faith.Name);
			}
		}
		if (passive || !c.faith.TryGetGift())
		{
			if (c.IsPC && EClass.player.prayed)
			{
				if (!passive)
				{
					c.Say("pray_ignore", c, c.faith.Name);
					c.PlaySound("pray_ignore");
				}
			}
			else
			{
				Pray(c, passive);
			}
		}
		return true;
	}

	public static void Pray(Chara c, bool passive)
	{
		if (c.IsPC)
		{
			EClass.player.prayed = true;
		}
		if (!passive)
		{
			c.Say("pray_answer", c, Act.CC.faith.Name);
			c.faith.Revelation((EClass.rnd(2) == 0) ? "random" : "chat");
		}
		foreach (Chara member in EClass.pc.party.members)
		{
			if (member == EClass.pc || (passive && c.Evalue(1655) >= 2))
			{
				member.ModExp(306, 200);
			}
		}
		if (!passive)
		{
			c.PlaySound("pray");
			c.Say("pray_heal", c);
		}
		if (c.IsPC)
		{
			foreach (Chara member2 in c.party.members)
			{
				Heal(member2);
			}
			return;
		}
		Heal(c);
		static void Heal(Chara _c)
		{
			_c.PlayEffect("revive");
			_c.HealHP(999999);
			_c.mana.Mod(999999);
			_c.Cure(CureType.Prayer, 999999);
		}
	}
}
