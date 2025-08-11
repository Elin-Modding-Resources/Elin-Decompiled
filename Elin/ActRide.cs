using System.Collections.Generic;

public class ActRide : Ability
{
	public virtual bool IsParasite => false;

	public override bool Perform()
	{
		List<Chara> list = Act.TP.ListCharas();
		list.Reverse();
		bool flag = false;
		foreach (Chara item in list)
		{
			if (item.host != null || item.IsMultisize)
			{
				continue;
			}
			if (item == Act.CC)
			{
				if ((IsParasite && Act.CC.parasite != null) || (!IsParasite && Act.CC.ride != null))
				{
					Unride(Act.CC, IsParasite);
					flag = true;
					break;
				}
				continue;
			}
			if (!item.trait.CanJoinPartyResident)
			{
				Msg.Say("ride_req");
				return false;
			}
			if (item.memberType == FactionMemberType.Default && (EClass.debug.enable || (item.IsPCFaction && item.trait.CanJoinParty)))
			{
				Ride(Act.CC, item, IsParasite);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			Msg.Say("noTargetFound");
		}
		return false;
	}

	public static void Ride(Chara host, Chara t, bool parasite = false, bool talk = true)
	{
		if (parasite)
		{
			if (host.parasite != null)
			{
				Unride(host, parasite: true);
			}
			host.parasite = t;
			host.Say("parasite", host, t);
		}
		else
		{
			if (host.ride != null)
			{
				Unride(host);
			}
			host.ride = t;
			host.Say("ride", host, t);
		}
		if (!t.IsPCFaction)
		{
			t.MakeAlly();
		}
		EClass.pc.party.AddMemeber(t);
		if (!parasite)
		{
			if (t.race.tag.Contains("ride"))
			{
				Msg.Say("ride_good");
			}
			if (t.race.tag.Contains("noRide"))
			{
				Msg.Say("ride_bad");
			}
			if (host.HasElement(1417) && t.HasCondition<ConTransmuteBroom>())
			{
				Msg.Say("ride_broom");
			}
		}
		t.host = host;
		t._CreateRenderer();
		if (talk)
		{
			host.PlaySound("ride");
			t.Talk(parasite ? "parasite" : "ride");
		}
		host.SetDirtySpeed();
		t.SetDirtySpeed();
		host.SyncRide();
		t.noMove = false;
		host.Refresh();
		WidgetRoster.SetDirty();
	}

	public static void Unride(Chara host, Chara mount, bool talk = true)
	{
		if (host != null && (host.parasite == mount || host.ride == mount))
		{
			Unride(host, host.parasite == mount, talk);
		}
	}

	public static void Unride(Chara host, bool parasite = false, bool talk = true)
	{
		Chara chara = null;
		if (parasite)
		{
			chara = host.parasite;
			host.parasite = null;
			host.Say("parasite_unride", host, chara);
		}
		else
		{
			chara = host.ride;
			host.ride = null;
			host.Say("ride_unride", host, chara);
		}
		chara.host = null;
		chara._CreateRenderer();
		if (talk)
		{
			chara.Talk(parasite ? "parasite_unride" : "ride_unride", null, null, forceSync: true);
		}
		host.PlaySound("ride");
		host.SetDirtySpeed();
		chara.SetDirtySpeed();
		host.Refresh();
		Point randomPoint = host.pos.GetRandomPoint(1);
		if (randomPoint != null)
		{
			chara.MoveImmediate(randomPoint);
		}
		WidgetRoster.SetDirty();
	}
}
