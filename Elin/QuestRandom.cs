public class QuestRandom : Quest
{
	public override bool CanAutoAdvance => false;

	public override bool IsRandomQuest => true;

	public override int RangeDeadLine => 12;

	public override int KarmaOnFail => -5;

	public override int FameOnComplete => 4 + difficulty * 2;

	public override string RefDrama1 => Lang._currency(rewardMoney, showUnit: true, 0);

	public override void OnDropReward()
	{
		long num = bonusMoney * (55 + difficulty * 15) / 100;
		int num2 = MathEx.ClampToInt(num + rewardMoney, 10000000);
		if (num2 > 0)
		{
			if (num > 0)
			{
				Msg.Say("reward_bonus", num.ToString() ?? "");
			}
			DropReward(ThingGen.CreateCurrency(num2));
		}
		Zone zone = EClass._zone.GetTopZone();
		if ((!zone.IsTown || zone.IsPCFaction) && base.ClientZone != null)
		{
			Zone topZone = base.ClientZone.GetTopZone();
			if (topZone.IsTown && !topZone.IsPCFaction)
			{
				zone = topZone;
			}
		}
		Rand.SetSeed(uid);
		string text = Util.EnumToList<SubReward>().RandomItem().ToString();
		if (EClass.rnd(5) == 0)
		{
			text = "gacha_coin_silver";
		}
		if (EClass.rnd(15) == 0)
		{
			text = "gacha_coin_gold";
		}
		Thing thing = ThingGen.Create(text);
		if (thing.id == "ticket_furniture")
		{
			TraitTicketFurniture.SetZone(zone, thing);
		}
		DropReward(thing);
		thing = ThingGen.Create("plat").SetNum(GetRewardPlat(num2));
		DropReward(thing);
		Rand.SetSeed();
		if (zone.IsTown || zone.IsPCFaction)
		{
			zone.GetTopZone().ModInfluence(1);
		}
		if (FameOnComplete > 0)
		{
			EClass.player.ModFame(EClass.rndHalf(FameOnComplete));
		}
	}
}
