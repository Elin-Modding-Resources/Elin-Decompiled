using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class GameDate : Date
{
	[JsonProperty]
	public bool shaken;

	public const int minPerRound = 5;

	public void AdvanceSec(int a)
	{
		base.sec += a;
		if (base.sec >= 60)
		{
			AdvanceMin(base.sec / 60);
			base.sec %= 60;
		}
	}

	public void AdvanceMin(int a)
	{
		EClass.game.sessionMin += a;
		EClass.player.stats.mins += a;
		base.min += a;
		if (base.min >= 60)
		{
			while (base.min >= 60)
			{
				base.min -= 60;
				AdvanceHour();
			}
			EClass.screen.RefreshGrading();
		}
		else if (base.min % 3 == 2)
		{
			EClass.screen.RefreshGrading();
		}
		if (EClass._map.footmarks.Count > 0)
		{
			for (int num = EClass._map.footmarks.Count - 1; num >= 0; num--)
			{
				Footmark footmark = EClass._map.footmarks[num];
				footmark.remaining--;
				if (footmark.remaining <= 0)
				{
					CellDetail detail = footmark.pos.detail;
					if (detail != null && detail.footmark == footmark)
					{
						detail.footmark = null;
						footmark.pos.cell.TryDespawnDetail();
					}
					EClass._map.footmarks.RemoveAt(num);
				}
			}
		}
		EClass.player.countNewline++;
		if (EClass.player.countNewline > EClass.core.config.game.newlineCount)
		{
			Msg.NewLine();
			EClass.player.countNewline = 0;
		}
		EClass.screen.pcOrbit.OnChangeMin();
		foreach (ZoneEvent item in EClass._zone.events.list)
		{
			item.minElapsed += a;
		}
	}

	public void AdvanceHour()
	{
		VirtualDate virtualDate = new VirtualDate();
		virtualDate.IsRealTime = true;
		virtualDate.SimulateHour();
		base.hour++;
		if (base.hour >= 24)
		{
			base.hour = 0;
			AdvanceDay();
		}
		if (!shaken && EClass.rnd(24) == 0 && !EClass._zone.IsRegion)
		{
			Msg.Say("earthquake");
			if (!EClass.core.config.graphic.disableShake)
			{
				Shaker.ShakeCam("earthquake");
			}
			shaken = true;
		}
		EClass.scene.OnChangeHour();
		EClass.world.weather.OnChangeHour();
		EClass.player.OnAdvanceHour();
		EClass.game.quests.OnAdvanceHour();
		EClass._zone.OnAdvanceHour();
		if (EClass._zone.IsRegion)
		{
			EClass._zone.Region.CheckRandomSites();
		}
		foreach (Chara value in EClass.game.cards.globalCharas.Values)
		{
			if (!value.IsPCParty && value.currentZone != EClass.game.activeZone && value.trait.UseGlobalGoal)
			{
				if (value.global.goal == null && !value.IsPCFaction)
				{
					GlobalGoalAdv globalGoalAdv = new GlobalGoalAdv();
					globalGoalAdv.SetOwner(value);
					globalGoalAdv.Start();
				}
				if (value.global.goal != null)
				{
					value.global.goal.AdvanceHour();
				}
			}
		}
		EClass.pc.RecalculateFOV();
		if (base.hour == 5)
		{
			ShipGoods();
			ShipPackages();
			ShipLetter();
			if (EClass.rnd(30) == 0 && EClass.game.cards.listPackage.Count <= 2)
			{
				ShipRandomPackages();
			}
			if (base.month == 10)
			{
				Tutorial.Reserve("season");
			}
			if (base.month == 11)
			{
				Tutorial.Reserve("season2");
			}
			else
			{
				Tutorial.Remove("season2");
			}
		}
	}

	public void AdvanceDay()
	{
		base.day++;
		base.min = 0;
		EClass.player.stats.days++;
		EClass.player.questRerollCost -= 3;
		if (EClass.player.questRerollCost < 0)
		{
			EClass.player.questRerollCost = 0;
		}
		if (EClass.player.stats.days >= 90)
		{
			Tutorial.Reserve("death_penalty");
		}
		if (base.day > 30)
		{
			base.day = 1;
			AdvanceMonth();
		}
		EClass.world.CreateDayData();
		EClass.Sound.Play("owl");
		if (!EClass.player.prayed && EClass.pc.Evalue(1655) > 0)
		{
			ActPray.TryPray(EClass.pc, passive: true);
		}
		Msg.Say("endDay");
		shaken = false;
		EClass.player.OnAdvanceDay();
		EClass.game.relations.UpdateRelations();
		EClass.pc.faction.OnAdvanceDay();
		foreach (Chara item in EClass.game.cards.listAdv)
		{
			if (!item.IsPCFaction && EClass.rnd(10) == 0 && (item.isDead || item.currentZone == null || item.currentZone.id == "somewhere"))
			{
				item.SetHomeZone(EClass.world.region.ListTowns().RandomItem());
				item.Revive();
				item.MoveZone(item.homeZone);
			}
		}
		if (EClass.pc.homeZone != null && EClass.pc.homeZone.mainFaction == EClass.pc.faction)
		{
			WidgetSticky.Add(new StickyHomeReport());
		}
		if (EClass.player.stats.days >= 7 && EClass.game.cards.globalCharas.Find("fiama").currentZone == EClass.game.StartZone && EClass.game.quests.GetGlobal("fiama_starter_gift") == null && !EClass.game.quests.IsCompleted("fiama_starter_gift"))
		{
			EClass.game.quests.Add("fiama_starter_gift", "fiama");
		}
		if (EClass.game.quests.IsCompleted("exploration"))
		{
			EClass.player.flags.daysAfterQuestExploration++;
			if (EClass.player.flags.daysAfterQuestExploration >= 7 && !EClass.player.flags.magicChestSent)
			{
				EClass.player.flags.magicChestSent = true;
				Thing thing = ThingGen.Create("parchment");
				thing.SetStr(53, "letter_magic_chest");
				Thing p = ThingGen.CreateParcel(null, ThingGen.Create("container_magic"), thing);
				EClass.world.SendPackage(p);
			}
		}
		if (base.day == 25 && base.month == 12)
		{
			EClass.world.SendPackage(ThingGen.Create("box_xmas"));
		}
	}

	public void AdvanceMonth()
	{
		base.month++;
		if (base.month > 12)
		{
			base.month = 1;
			AdvanceYear();
		}
		EClass.player.stats.months++;
		EClass.player.nums.OnAdvanceMonth();
		if (base.month % 2 == 0)
		{
			EClass.player.holyWell++;
		}
		EClass._map.RefreshAllTiles();
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.renderer.hasActor)
			{
				thing.renderer.RefreshSprite();
			}
		}
		EClass.pc.faction.OnAdvanceMonth();
	}

	public void AdvanceYear()
	{
		base.year++;
		EClass.player.wellWished = false;
		EClass.player.nums.OnAdvanceYear();
		EClass.world.SendPackage(ThingGen.Create("gift_newyear"));
	}

	public void ShipGoods()
	{
		Thing container_shipping = EClass.game.cards.container_shipping;
		if (container_shipping.things.Count == 0)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		List<Thing> list = new List<Thing>();
		List<string> list2 = new List<string>();
		Zone zone = EClass.game.spatials.Find(EClass.player.uidLastShippedZone);
		if (zone == null || zone.branch == null)
		{
			zone = EClass.pc.homeZone;
		}
		ShippingResult shippingResult = new ShippingResult();
		shippingResult.rawDate = EClass.world.date.GetRaw();
		shippingResult.uidZone = zone.uid;
		if (EClass.player.stats.shipMoney < 0)
		{
			EClass.player.stats.shipMoney = 2147483647L;
		}
		shippingResult.total = EClass.player.stats.shipMoney;
		shippingResult.hearthLv = zone.branch.lv;
		shippingResult.hearthExp = zone.branch.exp;
		shippingResult.debt = EClass.player.debt;
		foreach (Thing thing3 in container_shipping.things)
		{
			if (thing3.trait.CanBeShipped)
			{
				int price = thing3.GetPrice(CurrencyType.Money, sell: true, PriceType.Shipping);
				int num5 = price * thing3.Num;
				num3 += num5;
				num += thing3.Num;
				num2 += EClass.rndHalf(thing3.Num * Mathf.Min(15 + price, 10000) / 100 + 1);
				list.Add(thing3);
				shippingResult.items.Add(new ShippingResult.Item
				{
					text = thing3.Name,
					income = num5
				});
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		num2 = num2 / 2 + 1;
		if (zone.branch.policies.IsActive(2515))
		{
			num2 = 0;
		}
		shippingResult.hearthExpGained = num2;
		EClass.pc.homeBranch.log.Add(Msg.Say("shipped_collect"));
		foreach (string item in list2)
		{
			EClass.pc.homeBranch.log.Add(item);
		}
		int shippingBonus = EClass.player.stats.GetShippingBonus(EClass.player.stats.shipMoney);
		EClass.player.stats.shipNum += num;
		EClass.player.stats.shipMoney += num3;
		int shippingBonus2 = EClass.player.stats.GetShippingBonus(EClass.player.stats.shipMoney);
		if (shippingBonus2 > shippingBonus)
		{
			num4 = shippingBonus2 - shippingBonus;
		}
		foreach (Thing item2 in list)
		{
			item2.Destroy();
		}
		Thing thing = null;
		Thing thing2 = null;
		string text = "";
		if (num3 != 0)
		{
			thing = ThingGen.Create("money").SetNum(num3);
		}
		if (num4 != 0)
		{
			thing2 = ThingGen.Create("money2").SetNum(num4);
		}
		if (thing != null && thing2 != null)
		{
			text = "_and".lang(thing.Name, thing2.Name);
			SE.Pay();
		}
		else if (thing != null || thing2 != null)
		{
			text = ((thing != null) ? thing : thing2).Name;
		}
		EClass.pc.homeBranch.log.Add(Msg.Say((text == "") ? "shipped_none" : "shipped", num.ToString() ?? "", text), FontColor.Good);
		EClass.player.shippingResults.Add(shippingResult);
		EClass.player.showShippingResult = EClass.core.config.game.showShippingResult;
		for (int i = 0; i < EClass.player.shippingResults.Count - 10; i++)
		{
			EClass.player.shippingResults.RemoveAt(0);
		}
		zone.branch.statistics.ship += num3;
		zone.branch.ModExp(num2);
		if (thing != null)
		{
			EClass.pc.Pick(thing);
		}
		if (thing2 != null)
		{
			EClass.pc.Pick(thing2);
		}
	}

	public void ShipPackages()
	{
		Thing container_deliver = EClass.game.cards.container_deliver;
		if (container_deliver.things.Count == 0)
		{
			return;
		}
		int num = 0;
		while (container_deliver.things.Count > 0)
		{
			num++;
			if (num > 100)
			{
				Debug.Log("too many tries");
				break;
			}
			int uidZone = 0;
			foreach (Thing thing2 in container_deliver.things)
			{
				int @int = thing2.GetInt(102);
				if (@int != 0)
				{
					uidZone = @int;
					thing2.SetInt(102);
					break;
				}
			}
			int num2 = 20;
			Thing thing = ThingGen.CreateCardboardBox(uidZone);
			for (int num3 = container_deliver.things.Count - 1; num3 >= 0; num3--)
			{
				Thing c = container_deliver.things[num3];
				thing.AddCard(c);
				num2 += 5;
				if (thing.things.IsFull())
				{
					break;
				}
			}
			EClass.world.SendPackage(thing);
			Thing bill = ThingGen.CreateBill(num2, tax: false);
			EClass.pc.faction.TryPayBill(bill);
		}
	}

	public void ShipLetter()
	{
		int num = -1;
		int lutz = EClass.player.flags.lutz;
		if (EClass.player.stats.days >= 2 && lutz <= 0)
		{
			num = 1;
		}
		else if (EClass.player.stats.days >= 5 && lutz <= 1)
		{
			num = 2;
		}
		else if (EClass.player.stats.days >= 8 && lutz <= 2)
		{
			num = 3;
		}
		else if (EClass.player.stats.days >= 11 && lutz <= 3)
		{
			num = 4;
		}
		else if (EClass.player.stats.days >= 15 && lutz <= 4)
		{
			num = 5;
		}
		else if (EClass.player.stats.days >= 17 && lutz <= 5)
		{
			num = 6;
		}
		else if (EClass.player.stats.days >= 30 && lutz <= 6)
		{
			num = 7;
		}
		else if (EClass.player.stats.days >= 50 && lutz <= 7)
		{
			num = 8;
		}
		if (num != -1)
		{
			EClass.player.flags.lutz = num;
			Thing thing = ThingGen.Create("letter");
			thing.SetStr(53, "lutz_" + num);
			EClass.world.SendPackage(thing);
		}
	}

	public void ShipRandomPackages()
	{
		Thing box = ThingGen.CreateCardboardBox();
		TraitContainer traitContainer = box.trait as TraitContainer;
		bool flag = EClass.pc.homeBranch.policies.IsActive(2708);
		if (EClass.rnd(EClass.debug.enable ? 1 : 100) == 0 && !EClass.player.flags.statueShipped)
		{
			EClass.player.flags.statueShipped = true;
			Add("statue_weird", 1);
			flag = false;
		}
		else if (EClass.rnd(10) == 0)
		{
			Add("234", 1);
		}
		else if (EClass.rnd(5) == 0)
		{
			AddThing(ThingGen.CreateFromCategory("junk"), 1);
		}
		else if (EClass.rnd(10) == 0)
		{
			AddThing(ThingGen.CreateFromTag("garbage"), 1);
		}
		else if (EClass.rnd(8) == 0)
		{
			CardRow cardRow = SpawnList.Get("chara").Select(EClass.pc.LV + 10);
			traitContainer.PutChara(cardRow.id);
			flag = false;
		}
		else if (EClass.rnd(8) == 0)
		{
			Add("plat", 1 + EClass.rnd(4));
			flag = false;
		}
		else if (EClass.rnd(8) == 0)
		{
			Add("money2", 1 + EClass.rnd(4));
			flag = false;
		}
		else
		{
			string id2 = "trash2";
			if (EClass.rnd(3) == 0)
			{
				id2 = "trash1";
			}
			if (EClass.rnd(3) == 0)
			{
				id2 = ((EClass.rnd(3) == 0) ? "529" : "1170");
			}
			if (EClass.rnd(5) == 0)
			{
				id2 = "_poop";
			}
			if (EClass.rnd(100) == 0)
			{
				id2 = "goodness";
				flag = false;
			}
			Add(id2, 1);
		}
		if (!flag)
		{
			EClass.world.SendPackage(box);
		}
		void Add(string id, int num)
		{
			AddThing(ThingGen.Create(id), num);
		}
		void AddThing(Thing t, int num)
		{
			t.SetNum(num);
			box.AddCard(t);
		}
	}
}
