using System.Collections.Generic;
using UnityEngine;

public class DramaOutcome : EMono
{
	public static string idJump;

	public DramaManager manager;

	public DramaSequence sequence => manager.sequence;

	public Person tg => manager.tg;

	public Chara cc => tg.chara;

	public void StartNewGame()
	{
		if ((bool)LayerTitle.actor)
		{
			EMono.game.StartNewGame();
		}
	}

	public void StartNewGame2()
	{
		if ((bool)LayerTitle.actor)
		{
			EMono.core.actionsNextFrame.Add(LayerTitle.KillActor);
			EMono.pc.MoveZone(EMono.game.Prologue.idStartZone);
			EMono.pc.global.transition = new ZoneTransition
			{
				state = ZoneTransition.EnterState.Exact,
				x = EMono.game.Prologue.startX,
				z = EMono.game.Prologue.startZ
			};
		}
	}

	public void PutOutFire()
	{
		foreach (Card item in EMono._map.props.installed.traits.GetTraitSet<TraitHearth>())
		{
			EMono._zone.AddCard(ThingGen.Create("dish_soup"), item.pos.GetRandomNeighbor());
		}
	}

	public void OnClaimLand()
	{
		Chara c = EMono.game.cards.globalCharas.Find("ashland");
		EMono.game.quests.globalList.Add(Quest.Create("sharedContainer").SetClient(c, assignQuest: false));
		EMono.game.quests.globalList.Add(Quest.Create("crafter").SetClient(c, assignQuest: false));
		EMono.game.quests.globalList.Add(Quest.Create("defense").SetClient(c, assignQuest: false));
		EMono.game.quests.Get<QuestHome>().ChangePhase(2);
		AddMaid();
	}

	public void AddMaid()
	{
	}

	public void convert_Jure()
	{
		EMono.game.religions.Healing.JoinFaith(EMono.pc, Religion.ConvertType.Campaign);
		if (!EMono.player.flags.gotJurePillow)
		{
			EMono.player.flags.gotJurePillow = true;
			EMono.player.DropReward(ThingGen.Create("pillow_jure"));
		}
	}

	public void QuestSharedContainer_Drop1()
	{
		EMono.player.DropReward(ThingGen.Create("chest6"));
	}

	public void QuestShippingChest_Drop1()
	{
		Recipe.DropIngredients("container_shipping", "palm", 6);
	}

	public void QuestExploration_Drop1()
	{
		EMono.player.DropReward(ThingGen.CreateScroll(8220), silent: true).c_IDTState = 0;
		EMono.player.DropReward(ThingGen.CreateScroll(8221)).c_IDTState = 0;
	}

	public void QuestExploration_MeetFarris()
	{
		EMono.game.quests.Get<QuestExploration>().ChangePhase(1);
		cc.RemoveEditorTag(EditorTag.AINoMove);
		cc.RemoveEditorTag(EditorTag.InvulnerableToMobs);
		cc.RemoveEditorTag(EditorTag.Invulnerable);
		cc.homeZone = EMono.game.StartZone;
		cc.MoveZone(EMono.game.StartZone, ZoneTransition.EnterState.Return);
	}

	public void QuestExploration_MeetFarris2()
	{
		EMono.game.quests.Get<QuestExploration>().ChangePhase(2);
		if (EMono.Branch == null)
		{
			EMono._zone.ClaimZone();
		}
		EMono.Branch.Recruit(cc);
		if (EMono.game.quests.Main == null)
		{
			EMono.game.quests.Start("main");
		}
		EMono.game.quests.Main.ChangePhase(300);
	}

	public void QuestExploration_AfterCrystal()
	{
		Quest quest = EMono.game.quests.Get<QuestExploration>();
		if (quest == null)
		{
			quest = EMono.game.quests.Start("exploration", EMono.game.cards.globalCharas.Find("ashland"), assignQuest: false);
		}
		quest.ChangePhase(5);
		Chara chara = EMono.game.cards.globalCharas.Find("fiama");
		EMono._zone.AddCard(ThingGen.CreateScroll(8220).Identify(show: false), chara.pos);
		chara.MoveZone(EMono.game.StartZone);
		chara.RemoveEditorTag(EditorTag.AINoMove);
	}

	public void QuestExploration_AfterComplete()
	{
		Chara chara = EMono.game.cards.globalCharas.Find("ashland");
		chara.MoveHome("lothria", 40, 49);
		EMono.game.quests.RemoveAll(chara);
		chara = EMono.game.cards.globalCharas.Find("fiama");
		chara.MoveHome("lothria", 46, 56);
		EMono.game.quests.RemoveAll(chara);
		if (EMono.game.quests.Main == null)
		{
			EMono.game.quests.Start("main");
		}
		EMono.game.quests.Main.ChangePhase(700);
	}

	public void QuestCraft_Drop1()
	{
		EMono.player.DropReward(ThingGen.CreateRawMaterial(EMono.sources.materials.alias["straw"]));
	}

	public void QuestDefense_0()
	{
		Prologue prologue = EMono.game.Prologue;
		Chara chara = EMono._zone.AddChara("punk", prologue.posPunk.x, prologue.posPunk.y);
		chara.things.DestroyAll();
		(EMono._zone.AddThing("gallows", prologue.posPunk.x, prologue.posPunk.y).Install().trait as TraitShackle).Restrain(chara);
		CardBlueprint.SetNormalRarity();
	}

	public void QuestVernis_DropRecipe()
	{
		EMono.player.DropReward(ThingGen.CreateRecipe("explosive"));
	}

	public void QuestDefense_1()
	{
		Prologue prologue = EMono.game.Prologue;
		Chara tc = EMono._zone.AddChara("boar", prologue.posPunk.x + 1, prologue.posPunk.y);
		(EMono._zone.AddThing("gallows", prologue.posPunk.x + 1, prologue.posPunk.y).Install().trait as TraitShackle).Restrain(tc);
		EMono.player.DropReward(ThingGen.Create("stone").SetNum(20));
		EMono.player.DropReward(ThingGen.Create("330").SetNum(3), silent: true).Identify(show: false);
		EMono.player.DropReward(ThingGen.Create("331").SetNum(3), silent: true).Identify(show: false);
		EMono.player.DropReward(ThingGen.Create("bandage").SetNum(5));
	}

	public void QuestDefense_2()
	{
	}

	public void QuestDebt_reward()
	{
		EMono.game.quests.Get<QuestDebt>().GiveReward();
	}

	public void Tutorial1()
	{
		Thing t = ThingGen.Create("log");
		Point point = new Point(53, 52);
		EMono._zone.AddCard(t, point).SetPlaceState(PlaceState.installed);
		t = ThingGen.Create("crimAle");
		EMono._zone.AddCard(t, point).SetPlaceState(PlaceState.installed);
	}

	public void WelcomeMsg()
	{
	}

	public void chara_hired()
	{
		if (EMono.Branch.IsRecruit(cc))
		{
			EMono.pc.ModCurrency(-CalcGold.Hire(cc), "money2");
			cc.SetBool(18, enable: true);
		}
		EMono.Sound.Play("good");
		EMono.Branch.Recruit(cc);
	}

	public void chara_hired_ticket()
	{
		EMono.pc.things.Find("ticket_resident").ModNum(-1);
		EMono.Sound.Play("good");
		cc.SetBool(18, enable: true);
		EMono.Branch.Recruit(cc);
	}

	public void nerun_gift()
	{
		Dialog.Gift("", true, ThingGen.Create("rp1"));
	}

	public void nerun_gift2()
	{
		Dialog.Gift("", true, ThingGen.Create("rp1"));
	}

	public void nerun_gift3()
	{
		List<Card> list = new List<Card>();
		list.Add(ThingGen.Create("rp1"));
		list.Add(ThingGen.Create("rp2"));
		list.Add(ThingGen.Create("rp3"));
		list.Add(ThingGen.Create("rp4"));
		list.Add(ThingGen.Create("rp5"));
		Dialog.Gift("", autoAdd: true, list);
	}

	public void fiama_gold()
	{
		EMono.player.DropReward(ThingGen.Create("money2").SetNum(10));
		if (EMono.game.idPrologue == 2)
		{
			EMono.player.DropReward(ThingGen.Create("hammer"));
		}
	}

	public void fiama_pet1()
	{
		Chara c = CharaGen.Create("dog");
		fiama_pet(c);
	}

	public void fiama_pet2()
	{
		Chara c = CharaGen.Create("cat");
		fiama_pet(c);
	}

	public void fiama_pet3()
	{
		Chara c = CharaGen.Create("bearCub");
		fiama_pet(c);
	}

	public void fiama_pet4()
	{
		Chara c = CharaGen.Create("shojo");
		fiama_pet(c);
	}

	private void fiama_pet(Chara c)
	{
		EMono._zone.AddCard(c, EMono.pc.pos);
		c.MakeAlly();
		c.SetInt(100, 1);
	}

	public void fiama_starter_gift()
	{
		DramaChoice lastChoice = DramaChoice.lastChoice;
		Thing thing = null;
		switch (lastChoice.index)
		{
		case 0:
			thing = ThingGen.Create("ring_decorative").SetNoSell();
			thing.elements.SetBase(65, 10);
			EMono.player.DropReward(thing);
			break;
		case 1:
			EMono.player.DropReward(ThingGen.Create("ticket_resident").SetNoSell());
			EMono.player.DropReward(ThingGen.Create("1174").SetNoSell());
			break;
		case 2:
			thing = ThingGen.Create("boots_").SetNoSell();
			thing.elements.SetBase(65, 5);
			thing.elements.SetBase(407, 5);
			EMono.player.DropReward(thing);
			break;
		case 3:
			EMono.player.DropReward(ThingGen.Create("1085").SetNum(3).SetNoSell());
			break;
		case 4:
		{
			for (int i = 0; i < 10; i++)
			{
				EMono.player.DropReward(ThingGen.Create("234").SetNoSell());
			}
			break;
		}
		default:
			cc.DoHostileAction(EMono.pc, immediate: true);
			cc.calmCheckTurn = 100;
			break;
		}
	}

	public void revive_pet()
	{
		foreach (Chara member in EMono.pc.homeBranch.members)
		{
			if (member.isDead && member.GetInt(100) != 0)
			{
				member.GetRevived();
			}
		}
		cc.ModAffinity(EMono.pc, -2);
	}

	public void nasu_join()
	{
		if (cc.id == "fairy_nanasu")
		{
			cc.MakeAlly();
		}
	}

	public void melilith_friend()
	{
		if (cc.id == "melilith" && EMono.game.quests.completedIDs.Contains("melilith"))
		{
			cc.MakeAlly();
		}
	}

	public void sister_friend()
	{
		if (cc.id == "olderyoungersister")
		{
			cc.MakeAlly();
		}
		EMono.pc.ModCurrency(-10000);
		cc.Say("hug", cc, EMono.pc);
	}

	public void sister_change()
	{
		cc.idSkin = ((cc.idSkin != 1) ? 1 : 2);
	}

	public void get_scratch()
	{
		EMono._map.TrySmoothPick(EMono.pc.pos, ThingGen.Create("scratchcard"), EMono.pc);
		EMono.game.dateScratch = EMono.world.date.GetRaw(24);
	}

	public void poppy_found()
	{
		if (cc.id == "poppy")
		{
			cc.MakeAlly();
		}
		EMono.game.quests.Get("puppy").NextPhase();
		Msg.Say("npc_rescue", cc);
		cc.RemoveEditorTag(EditorTag.InvulnerableToMobs);
		cc.RemoveEditorTag(EditorTag.Invulnerable);
	}

	public void event_swordkeeper()
	{
		EMono._zone.AddChara("swordkeeper", 45, 52);
	}

	public static bool If(DramaChoice item, Chara c)
	{
		_ = item.IF.Split('/')[0] == "costHire";
		return true;
	}

	public void guild_trial()
	{
		if (Guild.CurrentDrama == EMono.game.factions.Merchant)
		{
			EMono.game.quests.Start("guild_merchant", cc, assignQuest: false);
		}
		else
		{
			(cc.trait as TraitGuildDoorman).GiveTrial();
		}
	}

	public void guild_join()
	{
		if (Guild.CurrentDrama != EMono.game.factions.Merchant)
		{
			(cc.trait as TraitGuildDoorman).OnJoinGuild();
		}
		Msg.Say("guild_join", Guild.CurrentDrama.Name);
		SE.Play("questComplete");
		Guild.CurrentDrama.relation.type = FactionRelation.RelationType.Member;
		Guild.CurrentDrama?.Quest.ChangePhase(10);
	}

	public void guild_mageTrial()
	{
		Guild.CurrentQuest.NextPhase();
		EMono.pc.things.Find("letter_trial")?.ModNum(-1);
	}

	public void guild_promote()
	{
		Guild.CurrentDrama.relation.Promote();
		Guild.GetCurrentGuild()?.RefreshDevelopment();
	}

	public bool check_sketch()
	{
		Thing thing = EMono.pc.things.Find("sketch_old");
		if (thing == null)
		{
			return false;
		}
		int num = thing.Num;
		thing.Destroy();
		bool flag = false;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < 5; j++)
			{
				int item = EMono.core.refs.dictSketches.Keys.RandomItem();
				if (!EMono.player.sketches.Contains(item))
				{
					EMono.player.sketches.Add(item);
					Msg.Say("add_sketch", item.ToString() ?? "");
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			SE.WriteJournal();
			return true;
		}
		return false;
	}

	public int GetFelmeraRewardIndex()
	{
		int count = EMono.player.sketches.Count;
		int num = count / 10;
		if (num >= 29)
		{
			num = 29;
		}
		Debug.Log(count + "/" + num + "/" + EMono.player.lastFelmeraReward);
		return num;
	}

	public bool check_sketch2()
	{
		return GetFelmeraRewardIndex() > EMono.player.lastFelmeraReward;
	}

	public List<Thing> ListFelmeraBarter()
	{
		List<Thing> list = new List<Thing>();
		int felmeraRewardIndex = GetFelmeraRewardIndex();
		for (int i = 0; i < felmeraRewardIndex; i++)
		{
			Thing thing = ThingGen.Create("painting_reward");
			thing.idSkin = i;
			list.Add(thing);
		}
		return list;
	}

	public void give_sketch_reward()
	{
		int felmeraRewardIndex = GetFelmeraRewardIndex();
		for (int i = EMono.player.lastFelmeraReward; i < felmeraRewardIndex; i++)
		{
			Thing thing = ThingGen.Create("painting_reward");
			thing.idSkin = i;
			EMono.player.DropReward(thing);
		}
		EMono.player.lastFelmeraReward = felmeraRewardIndex;
	}

	public void give_sketch_special()
	{
		foreach (int key in EMono.core.refs.dictSketches.Keys)
		{
			if (key >= 500 && key < 700)
			{
				EMono.player.sketches.Add(key);
			}
		}
		Msg.Say("add_sketch_special");
		SE.WriteJournal();
		EMono.pc.things.Find("sketch_special").Destroy();
	}
}
