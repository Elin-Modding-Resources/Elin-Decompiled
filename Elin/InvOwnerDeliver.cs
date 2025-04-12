public class InvOwnerDeliver : InvOwnerDraglet
{
	public enum Mode
	{
		Default,
		Tax,
		Bank,
		Crop
	}

	public Mode mode;

	public override bool SingleTarget => mode == Mode.Tax;

	public override string langTransfer
	{
		get
		{
			if (mode != Mode.Bank)
			{
				return "invDeliver";
			}
			return "invBank";
		}
	}

	public override ProcessType processType => ProcessType.Consume;

	public InvOwnerDeliver(Card owner = null, Card container = null, CurrencyType _currency = CurrencyType.Money)
		: base(owner, container, _currency)
	{
	}

	public override bool ShouldShowGuide(Thing t)
	{
		switch (mode)
		{
		case Mode.Tax:
			return t.c_bill != 0;
		case Mode.Bank:
			return t.id == "money";
		case Mode.Crop:
			if (!t.GetBool(115))
			{
				return false;
			}
			if (!(t.category.id == "vegi") && !(t.category.id == "fruit"))
			{
				return t.category.id == "mushroom";
			}
			return true;
		default:
			return false;
		}
	}

	public static void PayBill(Thing t, bool fromBank = false)
	{
		bool flag = t.id == "bill_tax";
		bool flag2 = t.id == "bill_debt";
		QuestDebt questDebt = EClass.game.quests.Get<QuestDebt>();
		bool flag3 = flag2 && questDebt != null && !questDebt.paid && questDebt.IsValidBill(t);
		if ((flag && EClass.player.taxBills <= 0) || (flag2 && !flag3))
		{
			SE.Beep();
			Msg.Say("badidea");
			EClass.pc.Pick(t, msg: false);
			return;
		}
		if (fromBank)
		{
			EClass.game.cards.container_deposit.ModCurrency(-t.c_bill);
		}
		else if (!EClass.pc.TryPay(t.c_bill))
		{
			return;
		}
		if (flag2)
		{
			EClass.player.debt -= t.c_bill;
			if (EClass.player.debt < 0)
			{
				EClass.player.debt = 0;
			}
			questDebt.paid = true;
			questDebt.UpdateJournal();
		}
		else if (flag)
		{
			EClass.player.stats.taxBillsPaid += t.c_bill;
			EClass.player.taxBills--;
			if (EClass.player.taxBills < 0)
			{
				EClass.player.taxBills = 0;
			}
			int num = t.GetInt(35) / 1000;
			if (num > 0)
			{
				Thing thing = ThingGen.Create("money2", "copper").SetNum(num);
				Thing p = ThingGen.CreateParcel("parcel_mysiliaGift", thing);
				Msg.Say("getSalary", thing);
				EClass.world.SendPackage(p);
			}
		}
		else
		{
			EClass.player.unpaidBill -= t.c_bill;
		}
		Msg.Say("payBill", t);
		t.Destroy();
	}

	public override void _OnProcess(Thing t)
	{
		switch (mode)
		{
		case Mode.Tax:
			PayBill(t);
			break;
		case Mode.Bank:
			SE.Pay();
			Msg.Say("depositMoney", t, owner);
			owner.AddThing(t);
			break;
		case Mode.Crop:
		{
			Msg.Say("farm_chest", t, Lang._weight(t.SelfWeight * t.Num));
			QuestHarvest questHarvest = EClass.game.quests.Get<QuestHarvest>();
			if (questHarvest != null)
			{
				questHarvest.weightDelivered += t.SelfWeight * t.Num;
			}
			SE.Pick();
			t.Destroy();
			break;
		}
		}
	}
}
