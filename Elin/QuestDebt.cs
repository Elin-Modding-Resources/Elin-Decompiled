using Newtonsoft.Json;

public class QuestDebt : QuestProgression
{
	[JsonProperty]
	public bool gaveBill;

	[JsonProperty]
	public bool paid;

	[JsonProperty]
	public int stage;

	public override void OnStart()
	{
		Chara chara = EClass.game.cards.globalCharas.Find("loytel");
		if (chara.homeBranch != null)
		{
			chara.homeBranch.RemoveMemeber(chara);
		}
		EClass.pc.homeBranch.AddMemeber(chara);
		EClass.pc.party.AddMemeber(chara);
		chara.homeZone = EClass.pc.homeBranch.owner;
		chara.noMove = false;
		chara.RemoveEditorTag(EditorTag.Invulnerable);
		Thing thing = ThingGen.Create("856");
		thing.refVal = 109;
		EClass.pc.Pick(thing);
	}

	public override bool CanUpdateOnTalk(Chara c)
	{
		_ = phase;
		return false;
	}

	public override string GetTextProgress()
	{
		return "progressDebt".lang(Lang._currency(EClass.player.debt, showUnit: true));
	}

	public bool CanGiveBill()
	{
		return stage < 7;
	}

	public int GetBillAmount()
	{
		return (new int[10] { 1, 3, 6, 10, 30, 50, 100, 300, 500, 1000 })[stage] * 10000;
	}

	public bool IsValidBill(Thing t)
	{
		int billAmount = GetBillAmount();
		return t.c_bill == billAmount;
	}

	public void GiveBill()
	{
		Thing thing = ThingGen.Create("bill_debt");
		thing.c_bill = GetBillAmount();
		thing.c_isImportant = true;
		EClass.player.DropReward(thing);
		gaveBill = true;
	}

	public void GiveReward()
	{
		gaveBill = false;
		paid = false;
		switch (stage)
		{
		case 1:
			EClass.player.DropReward(ThingGen.Create("ticket_massage"));
			break;
		case 2:
			EClass.player.DropReward(ThingGen.Create("ticket_armpillow"));
			break;
		case 3:
			EClass.player.DropReward(ThingGen.Create("ticket_champagne"));
			break;
		case 4:
			EClass.player.DropReward(ThingGen.Create("ticket_resident"));
			break;
		case 5:
			EClass.player.DropReward(ThingGen.Create("loytel_mart"));
			break;
		case 6:
			EClass.player.flags.loytelMartLv = 1;
			Msg.Say("upgradeLoytelMart");
			break;
		case 7:
			EClass.player.flags.loytelMartLv = 2;
			Msg.Say("upgradeLoytelMart");
			break;
		}
	}

	public string GetIdTalk_GiveBill()
	{
		if (stage != 0)
		{
			return "loytel_bill_give2";
		}
		return "loytel_bill_give1";
	}
}
