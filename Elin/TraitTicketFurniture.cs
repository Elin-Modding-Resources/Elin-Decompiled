using UnityEngine;

public class TraitTicketFurniture : Trait
{
	public Zone zone
	{
		get
		{
			return RefZone.Get(owner.refVal);
		}
		set
		{
			owner.refVal = value?.uid ?? 0;
		}
	}

	public override bool IsTool => true;

	public override void TrySetHeldAct(ActPlan p)
	{
		if (EClass._zone.GetTopZone() != zone)
		{
			return;
		}
		p.pos.Things.ForEach(delegate(Thing t)
		{
			if (t.IsInstalled && t.isNPCProperty && t.trait.CanBeHeld && !t.trait.IsDoor && !t.isMasked && t.source.value != 0 && (EClass._zone is Zone_LittleGarden || !(t.trait is TraitNewZone)))
			{
				int price = GetPrice(t) * t.Num;
				p.TrySetAct("actCollectFurniture".lang(price.ToString() ?? "", t.Name) + ((t.category.ticket >= 10) ? "ticketNotIntended".lang() : ""), delegate
				{
					if (owner.Num < price)
					{
						Msg.Say("notEnoughTicket");
						return false;
					}
					SE.Pay();
					owner.ModNum(-price);
					t.isNPCProperty = false;
					if (t.trait is TraitPillow)
					{
						t.noSell = true;
					}
					EClass.pc.Pick(t);
					return false;
				});
			}
		});
	}

	public int GetPrice(Thing t)
	{
		int num = (t.GetPrice() / 500 + 1) * t.category.ticket;
		if (EClass._zone is Zone_LittleGarden)
		{
			if (num >= 10)
			{
				num = 10;
			}
			num = Mathf.Max(num + EClass.player.little_dead - EClass.player.little_saved / 5, num);
		}
		return num;
	}

	public override void SetName(ref string s)
	{
		if (zone != null)
		{
			s = "_of".lang(zone.Name, s);
		}
	}

	public static void SetZone(Zone zone, Thing t)
	{
		int num = 0;
		if (zone != null)
		{
			if (zone.IsTown || zone is Zone_LittleGarden)
			{
				num = zone.GetTopZone().uid;
			}
			if (zone.IsPCFaction)
			{
				num = EClass.game.spatials.Find("mysilia").uid;
			}
		}
		if (num == 0)
		{
			num = EClass.game.spatials.Find("palmia").uid;
		}
		t.refVal = num;
	}
}
