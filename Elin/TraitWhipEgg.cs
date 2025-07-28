public class TraitWhipEgg : TraitWhipLove
{
	public override void OnCreate(int lv)
	{
		owner.c_charges = 6;
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.ListCards().ForEach(delegate(Card c)
		{
			if (p.IsSelfOrNeighbor && EClass.pc.CanSee(c))
			{
				p.TrySetAct("actWhip", delegate
				{
					EClass.pc.Say("use_whip", c, owner);
					EClass.pc.Say("use_scope2", c);
					EClass.player.forceTalk = true;
					EClass.pc.Talk("egg");
					c.Talk("pervert2");
					EClass.pc.PlaySound("whip");
					c.PlayAnime(AnimeID.Shiver);
					if (c.isChara)
					{
						c.Chara.OnInsulted();
					}
					Thing c2 = c.MakeEgg();
					c.Say("item_drop", c2);
					owner.ModCharge(-1);
					if (owner.c_charges <= 0)
					{
						EClass.pc.Say("spellbookCrumble", owner);
						owner.Destroy();
					}
					EClass.player.ModKarma(-1);
					return true;
				}, c);
			}
		});
	}
}
