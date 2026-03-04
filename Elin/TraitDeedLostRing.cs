public class TraitDeedLostRing : TraitTool
{
	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.ListCharas().ForEach(delegate(Chara a)
		{
			if (a.IsMarried)
			{
				p.TrySetAct("actReissue", delegate
				{
					EClass.pc.Say("use_whip", a, owner);
					a.Talk("death_other");
					EClass.pc.PlaySound("whip");
					a.PlayAnime(AnimeID.Shiver);
					a.PlaySound("dropRewardXmas");
					owner.ModNum(-1);
					Thing thing = ThingGen.Create("amulet_engagement");
					thing.Attune(a);
					thing.elements.ModBase(484, 3);
					if (thing.rarity < Rarity.Mythical)
					{
						thing.rarity = Rarity.Mythical;
					}
					EClass.player.DropReward(thing, silent: true);
					thing = ThingGen.Create("ring_engagement");
					thing.Attune(a);
					thing.elements.ModBase(484, 3);
					if (thing.rarity < Rarity.Mythical)
					{
						thing.rarity = Rarity.Mythical;
					}
					EClass.player.DropReward(thing);
					return false;
				}, a);
			}
		});
	}
}
