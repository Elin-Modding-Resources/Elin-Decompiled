public class TraitBookSecret : TraitBookExp
{
	public override void OnRead(Chara c)
	{
		if (c.IsPC && EClass.player.stats.kumi >= 5)
		{
			c.Say("book_secret2", c);
			return;
		}
		c.Say("book_secret", c);
		c.Say("dingExp", c);
		c.feat += (c.IsPC ? 1 : 4);
		if (c.IsPC)
		{
			EClass.player.stats.kumi++;
		}
		c.PlaySound("godbless");
		c.PlayEffect("aura_heaven");
		c.Say("spellbookCrumble", owner.Duplicate(1));
		owner.ModNum(-1);
	}
}
