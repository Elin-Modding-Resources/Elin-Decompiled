public class TraitBookExp : TraitScroll
{
	public override bool CanRead(Chara c)
	{
		return !c.isBlind;
	}

	public override int GetActDuration(Chara c)
	{
		return 5;
	}

	public override void OnRead(Chara c)
	{
		EClass.player.forceTalk = true;
		c.Talk((EClass.rnd(2) == 0) ? "nice_statue" : "disgust");
		c.AddExp(c.ExpToNext / ((!c.IsPC) ? 1 : 4) * ((!c.HasElement(1273)) ? 1 : 4), applyMod: false);
		c.PlaySound("ding_potential");
		c.Say("spellbookCrumble", owner.Duplicate(1));
		owner.ModNum(-1);
	}
}
