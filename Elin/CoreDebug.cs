using System;
using System.Collections.Generic;
using System.Linq;
using ReflexCLI.Attributes;
using Steamworks;
using UnityEngine;

[ConsoleCommandClassCustomizer("")]
public class CoreDebug : EScriptable
{
	[Serializable]
	public class StartSetting
	{
		public string race;

		public string job;

		public int lv;
	}

	public enum DebugHotkey
	{
		None,
		Block,
		Item,
		Decal,
		Act,
		Test,
		Anime
	}

	public enum StartLoadout
	{
		New,
		AllItem,
		FewItem
	}

	public enum StartParty
	{
		None,
		Farris,
		Full
	}

	public enum StartScene
	{
		Title,
		Zone,
		Home,
		Home_Cave,
		Story_Test,
		MeetFarris,
		NymelleBoss,
		AfterNymelle,
		Melilith,
		Tefra
	}

	[Serializable]
	public class MatColorTest
	{
		public bool enable;

		public Color color;

		public int matColor;

		public ColorProfile profile;

		public void Update()
		{
			foreach (SourceMaterial.Row row in EClass.sources.materials.rows)
			{
				if (row != MATERIAL.sourceWaterSea)
				{
					MatColors matColors = profile.matColors.TryGetValue(row.alias);
					row.matColor = matColors.main;
					row.altColor = matColors.alt;
				}
			}
		}
	}

	public class DebugCommand
	{
		public Action action;

		public int cat;

		public string name;
	}

	[Header("Quick Start Setup")]
	public StartScene startScene;

	public StartLoadout startLoadout;

	public StartParty startParty;

	public int startHour;

	public StartSetting startSetting;

	public string startZone;

	[Header("System(Release)")]
	public bool showSceneSelector;

	[Header("System(Release)")]
	public bool skipModSync;

	public Lang.LangCode langCode;

	[Header("System")]
	public bool ignorePool;

	[Header("System")]
	public bool resetPlayerConfig;

	public bool dontUseThread;

	public bool useNewConfig;

	public bool ignoreAutoSave;

	public bool alwaysResetWindow;

	public bool validateData;

	public bool dontCompressSave;

	public bool skipMod;

	public string command;

	[Header("Input")]
	public bool debugInput;

	public bool keypadDebug;

	public DebugHotkey debugHotkeys;

	[Header("Util")]
	public int advanceMin;

	public bool testLOS;

	public bool testLOS2;

	public bool debugProps;

	public bool revealMap;

	public bool debugHoard;

	public bool revealInner;

	public bool ignorePopup;

	public bool skipEvent;

	public bool skipNerun;

	public bool showTone;

	public bool showExtra;

	public bool test;

	[Header("Game")]
	public bool godMode;

	public bool randomResource;

	public bool debugScatter;

	public bool _godBuild;

	public bool godCraft;

	public bool godFood;

	public bool ignoreBuildRule;

	public bool ignoreWeight;

	public bool autoIdentify;

	public bool allAbility;

	public bool allSkill;

	public bool allHomeSkill;

	public bool allArt;

	public bool allBGM;

	public bool ignoreEncounter;

	public bool returnAnywhere;

	public bool instaReturn;

	public bool travelAnywhere;

	public bool hidePCItemsInBuild;

	public bool autoAdvanceQuest;

	public bool enableMapPieceEditor;

	public bool testThingQuality;

	public int numResource;

	[Header("Game(Specific)")]
	public bool unlimitedInterest;

	public bool inviteAnytime;

	public bool marryAnytime;

	public bool showFav;

	public bool alwaysFavFood;

	public bool maxQuests;

	[Header("Progress")]
	public bool allRecipe;

	public bool allMenu;

	public bool allPolicy;

	public bool allStory;

	[Header("Log")]
	public bool logAdv;

	[Header("Log")]
	public bool logCombat;

	public bool logDice;

	[Header("Once")]
	public bool validatePref;

	[Header("Test")]
	public int param1;

	[Header("Test")]
	public int param2;

	public MatColorTest matColorTest;

	public bool boradcast;

	public bool testFixedColor;

	public Color32 fixedColor;

	private readonly List<int[]> resolutions = new List<int[]>
	{
		new int[2] { 1920, 1080 },
		new int[2] { 1366, 768 },
		new int[2] { 1280, 720 }
	};

	private Card animeDest;

	[NonSerialized]
	private int bidx;

	private bool bilinear;

	[NonSerialized]
	private string[] blines;

	public List<DebugCommand> commands = new List<DebugCommand>();

	[NonSerialized]
	public bool enable;

	public Fov fov = new Fov();

	private int indexResolution;

	private AnimeID lastAnime;

	private Emo lastEmo;

	private bool naked;

	[NonSerialized]
	private bool runtimeGodBuild;

	private int ttt;

	public bool godBuild
	{
		get
		{
			if (!_godBuild)
			{
				return runtimeGodBuild;
			}
			return true;
		}
	}

	public static string EnableCheat => "Enable cheat by typing 'Cheat'";

	private IEnumerable<string> ZoneIDs()
	{
		List<SourceZone.Row> rows = EClass.sources.zones.rows;
		List<string> list = new List<string>();
		foreach (SourceZone.Row item in rows)
		{
			list.Add(item.id);
		}
		return list;
	}

	public void Init()
	{
		InitDebugCommands();
		EInput.disableKeyAxis = Application.isEditor;
	}

	public void QuickStart()
	{
		Game.Create("quick");
		Tutorial.debugSkip = EClass.debug.skipNerun;
		if (!startSetting.race.IsEmpty())
		{
			EClass.pc.ChangeRace(startSetting.race);
		}
		if (!startSetting.job.IsEmpty())
		{
			EClass.pc.ChangeJob(startSetting.job);
		}
		switch (startScene)
		{
		case StartScene.Zone:
		case StartScene.Home:
			EClass.game.world.date.hour = EClass.debug.startHour;
			break;
		case StartScene.Home_Cave:
			EClass.game.idPrologue = 2;
			break;
		}
		EClass.game.StartNewGame();
		EClass.player.flags.OnEnableDebug();
		EClass.player.pref.lastIdTabAbility = 3;
		Zone homeZone = EClass.game.spatials.Find(EClass.game.Prologue.idStartZone);
		switch (startParty)
		{
		case StartParty.Farris:
			AddParty("farris");
			break;
		case StartParty.Full:
			AddParty("farris");
			AddParty("wescott");
			break;
		}
		int lv = startSetting.lv;
		if (allAbility)
		{
			foreach (SourceElement.Row row in EClass.sources.elements.rows)
			{
				if (row.category == "ability" && row.aliasRef != "mold" && !EClass.pc.HasElement(row.id) && (row.aliasRef.IsEmpty() || row.id <= 10000 || EClass.sources.elements.alias[row.aliasRef].tag.Contains(row.alias.Split('_')[0])))
				{
					EClass.pc.elements.ModBase(row.id, lv).vPotential = 30;
				}
			}
		}
		if (allSkill)
		{
			foreach (SourceElement.Row row2 in EClass.sources.elements.rows)
			{
				if (row2.isSkill && !row2.tag.Contains("unused"))
				{
					EClass.pc.elements.ModBase(row2.id, 1);
				}
			}
		}
		EClass.pc.elements.ModBase(60, lv / 2);
		EClass.pc.elements.ModBase(61, lv / 2);
		EClass.pc.elements.ModBase(79, lv / 5);
		EClass.player.totalFeat = lv;
		EClass.pc.SetLv(lv - 1);
		EClass.player.fame = EClass.pc.LV * 100;
		int idx = 0;
		switch (startLoadout)
		{
		case StartLoadout.AllItem:
			EClass.pc.ModCurrency(50, "plat");
			EClass.pc.ModCurrency(50, "money2");
			EClass.pc.EQ_CAT("weapon");
			EClass.pc.EQ_CAT((EScriptable.rnd(2) == 0) ? "weapon" : "shield");
			EClass.pc.EQ_CAT("torso");
			EClass.pc.EQ_CAT("arm");
			EClass.pc.EQ_CAT("ring");
			EClass.pc.AddCard(ThingGen.Create("chest2"));
			EClass.pc.AddCard(ThingGen.Create("torch_held"));
			AddHotbar("axe");
			AddHotbar("pickaxe");
			AddHotbar("wateringCan");
			AddHotbar("bow");
			AddHotbar("deed");
			AddAbility("SpellTeleport");
			AddAbility("SpellCatsEye");
			AddHotbar("map_big");
			break;
		case StartLoadout.FewItem:
		{
			Thing thing = ThingGen.Create("backpack");
			thing.AddCard(ThingGen.Create("rp_food", -1, 10).SetNum(30));
			thing.AddCard(ThingGen.Create("rp_block", -1, 10).SetNum(30));
			for (int i = 0; i < 30; i++)
			{
				thing.AddCard(ThingGen.Create("rp_block", -1, 50));
			}
			EClass.pc.AddCard(thing);
			thing = ThingGen.Create("pouch");
			for (int j = 0; j < 30; j++)
			{
				CardBlueprint.Set(CardBlueprint.DebugEQ);
				Thing thing2 = ThingGen.CreateFromCategory("weapon", EClass.pc.LV);
				thing2.elements.SetBase(653, 1);
				if (!(thing2.trait is TraitAmmo))
				{
					thing.AddCard(thing2);
				}
				thing2 = ThingGen.CreateFromCategory("armor", EClass.pc.LV);
				thing2.elements.SetBase(653, 1);
				thing.AddCard(thing2);
			}
			EClass.pc.AddCard(thing);
			thing = ThingGen.Create("pouch");
			for (int k = 0; k < 30; k++)
			{
				thing.AddCard(ThingGen.Create("mathammer", MATERIAL.GetRandomMaterial(100).alias)).SetNum(10);
			}
			thing.AddCard(ThingGen.Create("mathammer", 102)).SetNum(10);
			thing.AddCard(ThingGen.Create("mathammer", 33)).SetNum(10);
			thing.AddCard(ThingGen.Create("mathammer", 18)).SetNum(10);
			thing.AddCard(ThingGen.Create("mathammer", 10)).SetNum(99);
			EClass.pc.AddCard(thing);
			thing = ThingGen.Create("pouch");
			for (int l = 0; l < 30; l++)
			{
				thing.AddCard(ThingGen.CreateFromCategory("ammo", EClass.pc.LV));
			}
			EClass.pc.AddCard(thing);
			thing = ThingGen.Create("coolerbox");
			for (int m = 0; m < 20; m++)
			{
				thing.AddCard(ThingGen.CreateFromCategory("foodstuff").SetNum(EScriptable.rnd(10) + 1));
			}
			EClass.pc.AddCard(thing);
			List<SourceChara.Row> list = EClass.sources.charas.map.Values.Where((SourceChara.Row a) => a._idRenderData == "chara").ToList();
			thing = ThingGen.Create("pouch");
			list = EClass.sources.charas.map.Values.Where((SourceChara.Row a) => a._idRenderData == "chara_L").ToList();
			for (int n = 0; n < 20; n++)
			{
				string id2 = list.RandomItem().id;
				Thing thing3 = ThingGen.Create("figure");
				thing3.MakeFigureFrom(id2);
				thing.AddCard(thing3);
			}
			EClass.pc.AddCard(thing);
			thing = ThingGen.Create("pouch");
			list = EClass.sources.charas.map.Values.Where((SourceChara.Row a) => a._idRenderData == "chara").ToList();
			for (int num = 0; num < 20; num++)
			{
				string id3 = list.RandomItem().id;
				Thing thing4 = ThingGen.Create("figure3");
				thing4.MakeFigureFrom(id3);
				thing.AddCard(thing4);
			}
			EClass.pc.AddCard(thing);
			thing.Dye("oak");
			thing = ThingGen.Create("pouch");
			list = EClass.sources.charas.map.Values.Where((SourceChara.Row a) => a._idRenderData == "chara_L").ToList();
			for (int num2 = 0; num2 < 20; num2++)
			{
				string id4 = list.RandomItem().id;
				Thing thing5 = ThingGen.Create("figure3");
				thing5.MakeFigureFrom(id4);
				thing.AddCard(thing5);
			}
			EClass.pc.AddCard(thing);
			thing.Dye("pine");
			thing = ThingGen.Create("pouch");
			thing.AddCard(ThingGen.Create("flour").SetNum(10));
			thing.AddCard(ThingGen.Create("wheat").SetNum(200));
			thing.AddCard(ThingGen.Create("rice_plant").SetNum(200));
			thing.AddCard(ThingGen.Create("noodle").SetNum(10));
			thing.AddCard(ThingGen.Create("rice").SetNum(10));
			thing.AddCard(ThingGen.Create("dough_cake").SetNum(10));
			thing.AddCard(ThingGen.Create("dough_bread").SetNum(10));
			thing.AddCard(ThingGen.Create("salt").SetNum(10));
			thing.AddCard(ThingGen.Create("sugar").SetNum(10));
			thing.AddCard(ThingGen.Create("honey").SetNum(10));
			thing.AddCard(ThingGen.Create("yeast").SetNum(10));
			thing.AddCard(ThingGen.Create("ketchup").SetNum(10));
			thing.AddCard(ThingGen.Create("butter").SetNum(10));
			thing.AddCard(ThingGen.Create("potion_empty").SetNum(10));
			thing.AddCard(ThingGen.Create("bucket_empty").SetNum(10));
			thing.AddCard(ThingGen.Create("battery"));
			EClass.pc.AddCard(thing);
			thing.Dye("saphire");
			thing = ThingGen.Create("pouch");
			thing.AddCard(ThingGen.Create("log").SetNum(99));
			thing.AddCard(ThingGen.Create("log").SetNum(99).ChangeMaterial("pine"));
			thing.AddCard(ThingGen.Create("ore").SetNum(99).ChangeMaterial("steel"));
			thing.AddCard(ThingGen.Create("ore").SetNum(99).ChangeMaterial("copper"));
			thing.AddCard(ThingGen.Create("ore_gem").SetNum(99).ChangeMaterial("gold"));
			thing.AddCard(ThingGen.Create("gem").SetNum(99).ChangeMaterial("rubinus"));
			thing.AddCard(ThingGen.Create("flower_white").SetNum(99));
			thing.AddCard(ThingGen.Create("bait").SetNum(10));
			EClass.pc.AddCard(thing);
			thing.Dye("rubinus");
			Thing thing6 = ThingGen.Create("quiver");
			thing6.AddCard(ThingGen.Create("bullet").SetNum(250));
			thing6.AddCard(ThingGen.Create("arrow").SetNum(250));
			thing6.AddCard(ThingGen.Create("bolt").SetNum(250));
			thing6.AddCard(ThingGen.Create("bullet_energy").SetNum(250));
			EClass.pc.body.GetEquippedThing(44).AddCard(thing6);
			thing = ThingGen.Create("backpack");
			for (int num3 = 0; num3 < 10; num3++)
			{
				thing.AddThing("book", lv);
				thing.AddThing("parchment");
				thing.AddThing("book_ancient");
			}
			EClass.pc.AddCard(thing);
			thing.Dye("rubinus");
			thing = ThingGen.Create("backpack");
			foreach (SourceObj.Row row3 in EClass.sources.objs.rows)
			{
				if (row3.tag.Contains("seed"))
				{
					Thing c = TraitSeed.MakeSeed(row3).SetNum(10);
					thing.AddCard(c);
				}
			}
			EClass.pc.AddCard(thing);
			thing.Dye("silver");
			thing = ThingGen.Create("pouch");
			for (int num4 = 0; num4 < 30; num4++)
			{
				thing.AddThing((EScriptable.rnd(10) != 0) ? "potion" : "drink", lv).SetNum(99);
			}
			EClass.pc.AddCard(thing);
			thing.Dye("steel");
			thing = ThingGen.Create("pouch");
			for (int num5 = 0; num5 < 30; num5++)
			{
				thing.AddThing(ThingGen.CreateFromCategory("rod", lv));
			}
			EClass.pc.AddCard(thing);
			thing.Dye("steel");
			thing = ThingGen.Create("pouch");
			for (int num6 = 0; num6 < 30; num6++)
			{
				thing.AddThing("scroll", lv).SetNum(99);
			}
			EClass.pc.AddCard(thing);
			thing.Dye("steel");
			thing = ThingGen.Create("pouch");
			for (int num7 = 0; num7 < 40; num7++)
			{
				thing.AddThing("372", 100);
			}
			EClass.pc.AddCard(thing);
			thing.Dye("steel");
			Thing thing7 = EClass.pc.things.Find("purse");
			thing7.AddThing("casino_coin").SetNum(30000000);
			thing7.AddThing("medal").SetNum(1000);
			thing7.ModCurrency(500, "plat");
			EClass.pc.AddThing("record");
			EClass.pc.AddThing("deed").SetNum(5);
			EClass.pc.AddThing("book_story");
			EClass.pc.AddThing("book_story_home");
			EClass.pc.AddThing("book_tutorial");
			EClass.pc.AddThing("water").SetNum(20).SetBlessedState(BlessedState.Blessed);
			EClass.pc.AddThing("water").SetNum(20).SetBlessedState(BlessedState.Cursed);
			EClass.pc.AddThing("potion_empty").SetNum(20);
			EClass.pc.ModCurrency(10000000);
			EClass.pc.AddCard(ThingGen.CreateBlock(67, 1).SetNum(99));
			EClass.pc.AddCard(ThingGen.CreateFloor(41, 1).SetNum(99));
			Thing thing8 = ThingGen.Create("scroll_random").SetNum(10);
			thing8.refVal = 8220;
			EClass.pc.AddCard(thing8);
			EClass.pc.ModCurrency(50, "money2");
			AddHotbar("hoe");
			AddHotbar("shovel");
			AddHotbar("axe");
			AddHotbar("pickaxe");
			AddHotbar("wateringCan");
			AddHotbar("gun_assault");
			AddHotbar("hammer");
			AddHotbar("bow");
			break;
		}
		}
		EClass.pc.RestockEquip(onCreate: true);
		EClass.pc.stamina.value = EClass.pc.stamina.max;
		EClass.pc.mana.value = EClass.pc.mana.max;
		EClass.pc.hp = EClass.pc.MaxHP;
		if (EClass.debug.startScene == StartScene.Zone)
		{
			EClass._zone.ClaimZone(debug: true);
		}
		SoundManager.ignoreSounds = true;
		EClass.ui.ToggleInventory();
		SoundManager.ignoreSounds = true;
		EClass.ui.ToggleAbility();
		SoundManager.ignoreSounds = false;
		EClass.ui.layerFloat.GetLayer<LayerAbility>().windows[0].SetRect(EClass.core.refs.rects.abilityDebug);
		if (allRecipe)
		{
			AddAllRecipes();
		}
		foreach (Thing thing11 in EClass.pc.things)
		{
			thing11.isNew = false;
		}
		LayerInventory.SetDirtyAll();
		EClass.core.actionsNextFrame.Add(EClass.pc.RecalculateFOV);
		if (startScene == StartScene.Story_Test)
		{
			EClass._zone.ClaimZone(debug: true);
			EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("loytel"), EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false)) as Chara);
			EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("farris"), EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false)) as Chara);
			EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("kettle"), EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false)) as Chara);
			EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("quru"), EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false)) as Chara);
			EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("corgon"), EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false)) as Chara);
			EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("demitas"), EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false)) as Chara);
			EClass.game.quests.Add("greatDebt");
			EClass.game.quests.Add("farris_tulip");
			EClass.game.quests.Add("kettle_join");
			EClass.game.quests.Add("quru_morning");
			EClass.game.quests.Add("vernis_gold");
			EClass.game.quests.Add("quru_sing");
			EClass.game.quests.Add("quru_past1");
			EClass.game.quests.Add("quru_past2");
			EClass.game.quests.Add("pre_debt");
			EClass.game.quests.Add("exile_meet");
			EClass.game.quests.Add("exile_quru");
			EClass.game.quests.Add("exile_kettle");
			EClass.game.quests.Add("exile_whisper");
			EClass.game.quests.Add("exile_voice");
			EClass.game.quests.Add("into_darkness");
			EClass.game.quests.Add("demitas_spellwriter");
		}
		static Thing AddAbility(string id)
		{
			Thing thing9 = EClass.pc.AddThing(ThingGen.Create("catalyst"));
			thing9.c_idAbility = id;
			return thing9;
		}
		Thing AddHotbar(string id)
		{
			Thing thing10 = EClass.pc.AddThing(ThingGen.Create(id));
			thing10.invX = idx;
			thing10.invY = 1;
			idx++;
			return thing10;
		}
		void AddParty(string id)
		{
			Chara chara = CharaGen.Create(id);
			chara.SetFaction(EClass.Home);
			chara.SetHomeZone(homeZone);
			if (chara.currentZone != EClass._zone)
			{
				EClass._zone.AddCard(chara, EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false));
			}
			EClass.pc.party.AddMemeber(chara);
		}
	}

	public void OnLoad()
	{
		if (allRecipe)
		{
			AddAllRecipes();
		}
	}

	public void AddAllRecipes()
	{
		foreach (RecipeSource item in RecipeManager.list)
		{
			if (!EClass.player.recipes.knownRecipes.ContainsKey(item.id))
			{
				EClass.player.recipes.knownRecipes.Add(item.id, 1);
			}
		}
	}

	public void SetStartStockpile(Thing container, int num = 100)
	{
		LittlePopper.skipPop = true;
		foreach (SourceMaterial.Row row in EClass.sources.materials.rows)
		{
			if (!(row.alias == "void") && (!randomResource || EScriptable.rnd(4) == 0))
			{
				row.CreateByProduct(container, num);
			}
		}
		foreach (SourceThing.Row row2 in EClass.sources.things.rows)
		{
			if ((row2.factory.IsEmpty() || !(row2.factory[0] == "x")) && !row2.isOrigin && row2.Category.tag.Contains("debug"))
			{
				Thing thing = ThingGen.Create(row2.id);
				if (thing.trait.CanStack)
				{
					thing.ModNum(num);
				}
				container.AddThing(thing);
			}
		}
		for (int i = 0; i < 10; i++)
		{
			container.AddThing(TraitSeed.MakeRandomSeed());
		}
		LittlePopper.skipPop = false;
	}

	public Thing GetOrCreateDebugContainer()
	{
		foreach (Thing thing2 in EClass._map.things)
		{
			if (thing2.c_altName == "DebugContainer")
			{
				return thing2;
			}
		}
		Thing thing = EClass._zone.AddThing("barrel", EClass.pc.pos).Thing;
		thing.ChangeMaterial("obsidian");
		thing.c_altName = "DebugContainer";
		thing.isNPCProperty = false;
		thing.SetPlaceState(PlaceState.installed);
		return thing;
	}

	public Thing GetDebugContainer()
	{
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.c_altName == "DebugContainer")
			{
				return thing;
			}
		}
		return null;
	}

	public void EnableDebugResource()
	{
		AddAllRecipes();
		EClass.player.flags.OnEnableDebug();
		enable = true;
		if ((bool)WidgetHotbar.HotbarBuild)
		{
			WidgetHotbar.HotbarBuild.RebuildPage();
		}
		WidgetSystemIndicator.Refresh();
		Thing orCreateDebugContainer = GetOrCreateDebugContainer();
		SetStartStockpile(orCreateDebugContainer, numResource);
	}

	public void SpawnCheatContainer()
	{
	}

	public void UpdateAlways()
	{
		if (!EClass.debug.enable || (bool)EClass.ui.GetLayer<LayerConsole>())
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.B))
		{
			UIButton.TryShowTip();
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			Steam.GetAchievement(ID_Achievement.test);
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			Steam.GetAchievement(ID_Achievement.mew);
		}
		if (Input.GetKeyDown(KeyCode.F11))
		{
			if (Application.isEditor)
			{
				EClass.core.SetLang((Lang.langCode == "EN") ? "JP" : "EN");
			}
			else
			{
				EClass.sources.ImportSourceTexts();
			}
			NewsList.dict = null;
			BookList.dict = null;
			BookList.Init();
		}
		if (EClass.core.IsGameStarted)
		{
			if (Input.GetKeyDown(KeyCode.U))
			{
				Vector3 offset = EClass.screen.tileMap.rendererInnerBlock.offset;
				Msg.SayRaw(offset.ToString() ?? "");
				EClass.screen.tileMap.rendererInnerBlock.offset = Vector3.zero;
			}
			if (Input.GetKeyDown(KeyCode.I))
			{
				Vector3 offset = EClass.screen.tileMap.rendererInnerBlock.offset;
				Msg.SayRaw(offset.ToString() ?? "");
				EClass.screen.tileMap.rendererInnerBlock.offset = new Vector3(0f, 0.54f, -0.5f);
			}
		}
	}

	public void ValidateData()
	{
		if (EClass.core.IsGameStarted)
		{
			EClass._map.Stocked.Validate();
			EClass._map.Roaming.Validate();
			EClass._map.Installed.Validate();
		}
	}

	public void UpdateInput()
	{
		if (!debugInput || EInput.isInputFieldActive || (bool)EClass.ui.GetLayer<LayerConsole>())
		{
			return;
		}
		bool key = Input.GetKey(KeyCode.LeftShift);
		bool key2 = Input.GetKey(KeyCode.LeftAlt);
		bool key3 = Input.GetKey(KeyCode.LeftControl);
		TraitStairs traitStairs = null;
		if (Input.GetKeyDown(KeyCode.KeypadPlus))
		{
			traitStairs = EClass._map.FindThing<TraitStairsDown>();
		}
		if (Input.GetKeyDown(KeyCode.KeypadMinus))
		{
			traitStairs = EClass._map.FindThing<TraitStairsUp>();
		}
		traitStairs?.MoveZone();
		if (EInput.action == EAction.ShowGrid)
		{
			EClass.ui.Find("_Guide").ToggleActive();
		}
		if (Input.GetKey(KeyCode.N))
		{
			if (key)
			{
				SoundManager.current.sourceBGM.pitch = 10f;
			}
			if (key3)
			{
				SoundManager.current.sourceBGM.time = SoundManager.current.currentBGM.length - 10f;
			}
		}
		else if (Input.GetKeyUp(KeyCode.N))
		{
			SoundManager.current.sourceBGM.pitch = 1f;
		}
		if (EClass.scene.actionMode.IsBuildMode)
		{
			string text = "festival";
			if (Input.GetKeyDown(KeyCode.F1))
			{
				EClass.ui.AddLayer<LayerDebug>();
			}
			if (Input.GetKeyDown(KeyCode.F2))
			{
				Zone.forceRegenerate = true;
				EClass.scene.actionMode.Deactivate();
				EClass.scene.Init(Scene.Mode.Zone);
				ActionMode.Inspect.Activate();
				SE.MoveZone();
			}
			if (Input.GetKeyDown(KeyCode.F4))
			{
				if (!MapSubset.Exist(text))
				{
					SE.Beep();
					return;
				}
				Zone.forceRegenerate = true;
				Zone.forceSubset = text;
				EClass.scene.actionMode.Deactivate();
				EClass.scene.Init(Scene.Mode.Zone);
				ActionMode.Inspect.Activate();
				SE.MoveZone();
				EClass.ui.Say("Loaded Subset");
			}
			if (Input.GetKeyDown(KeyCode.F5))
			{
				MapSubset.Save(text);
				SE.WriteJournal();
				EClass.ui.Say("Exported Subset");
			}
			return;
		}
		if (Input.GetKeyDown(KeyCode.F1))
		{
			EClass.ui.AddLayer<LayerDebug>();
			Debug.Log(EClass._zone);
			Debug.Log(EClass._zone.id + "/" + EClass._zone.parent);
			Debug.Log(EClass._zone.RegionPos);
		}
		if (Input.GetKeyDown(KeyCode.F2))
		{
			EClass.game.world.date.year = 3000;
			EClass._zone.development += 10;
			EClass._zone.development *= EClass._zone.development;
			EClass.pc.Pick(ThingGen.CreateSpellbook(8550));
			EClass.pc.Pick(ThingGen.CreatePotion(8550));
			EClass.pc.Pick(ThingGen.CreateSpellbook(9004));
			Chara targetChara = EClass.scene.mouseTarget.TargetChara;
			if (targetChara != null)
			{
				EClass.pc.Pick(targetChara.MakeMilk());
				EClass.pc.Pick(targetChara.MakeGene());
				EClass.pc.Pick(targetChara.MakeBraineCell());
				EClass.pc.Pick(targetChara.MakeEgg(effect: true, 10));
			}
			if (EClass.game.quests.Get<QuestDebt>() == null)
			{
				Chara chara = CharaGen.Create("loytel");
				EClass._zone.AddCard(chara, EClass.pc.pos);
				chara.SetGlobal();
				Quest q = EClass.game.quests.Add("debt", "loytel");
				EClass.game.quests.Start(q);
				EClass.pc.party.RemoveMember(chara);
				Hostility hostility2 = (chara.c_originalHostility = Hostility.Ally);
				chara.hostility = hostility2;
			}
			return;
		}
		if (Input.GetKeyDown(KeyCode.F3))
		{
			for (int i = 0; i < 10; i++)
			{
				Thing thing = ThingGen.Create("egg_fertilized");
				thing.TryMakeRandomItem(40);
				thing.SetEncLv(200);
				EClass.pc.Pick(thing);
			}
			foreach (Chara deadChara in EClass._map.deadCharas)
			{
				Debug.Log(deadChara);
			}
			EClass.core.steam.CheckUpdate();
			EClass.player.flags.loytelMartLv++;
			Msg.Say("loytelmart:" + EClass.player.flags.loytelMartLv);
			Guild.Fighter.relation.rank = 20;
			Guild.Mage.relation.rank = 20;
			Guild.Thief.relation.rank = 20;
			Guild.Merchant.relation.rank = 20;
			if (EClass.Branch != null)
			{
				EClass.Branch.ModExp(EClass.Branch.GetNextExp());
			}
			foreach (Chara chara2 in EClass._map.charas)
			{
				chara2.AddExp(chara2.ExpToNext);
			}
			EClass.pc.PlayEffect("boost");
			EClass.pc.PlaySound("boost");
			EClass.pc.elements.SetBase(306, 100);
			EClass.pc.elements.SetBase(85, 100);
			EClass.pc.feat += 10;
			EClass.player.totalFeat += 10;
			return;
		}
		if (Input.GetKeyDown(KeyCode.F4))
		{
			foreach (Chara member in EClass.pc.party.members)
			{
				member.InstantEat();
				if (!member.IsPC)
				{
					string id = "test";
					member.spriteReplacer = new SpriteReplacer();
					member.spriteReplacer.HasSprite(id);
					member._CreateRenderer();
				}
			}
			for (int j = 0; j < 10; j++)
			{
				Thing thing2 = ThingGen.CreateFromCategory("fish", 999);
				thing2.SetTier(EScriptable.rnd(4));
				EClass.pc.Pick(thing2);
				thing2 = ThingGen.Create("89");
				thing2.SetTier(EScriptable.rnd(4));
				EClass.pc.Pick(thing2);
			}
			EClass.pc.PlayEffect("phoenix");
			return;
		}
		if (Input.GetKeyDown(KeyCode.F5))
		{
			string text2 = (Input.GetKey(KeyCode.LeftControl) ? "quick3" : (Input.GetKey(KeyCode.LeftShift) ? "quick2" : "quick"));
			if (text2 != Game.id)
			{
				IO.DeleteDirectory(CorePath.RootSave + text2);
				IO.CopyAll(CorePath.RootSave + Game.id, CorePath.RootSave + text2);
				Game.id = text2;
			}
			EClass.game.isCloud = false;
			EClass.game.Save();
		}
		if (Input.GetKeyDown(KeyCode.F6))
		{
			EClass.core.WaitForEndOfFrame(delegate
			{
				string id2 = (Input.GetKey(KeyCode.LeftControl) ? "quick3" : (Input.GetKey(KeyCode.LeftShift) ? "quick2" : "quick"));
				EClass.scene.Init(Scene.Mode.None);
				Game.Load(id2, cloud: false);
			});
		}
		if (Input.GetKeyDown(KeyCode.F7))
		{
			MiniGame.Activate(MiniGame.Type.Scratch);
			return;
		}
		if (Input.GetKeyDown(KeyCode.F8))
		{
			SE.Click();
			EnableDebugResource();
			runtimeGodBuild = true;
			WidgetMenuPanel.OnChangeMode();
			EClass.player.hotbars.ResetHotbar(2);
		}
		if (key3)
		{
			if (Input.GetKeyDown(KeyCode.F9))
			{
				indexResolution++;
				if (indexResolution >= resolutions.Count)
				{
					indexResolution = 0;
				}
				int[] array = resolutions[indexResolution];
				int width = array[0];
				int height = array[1];
				Screen.SetResolution(width, height, key);
				WidgetFeed.Instance?.Nerun(width + "/" + height);
			}
			if (Input.GetKeyDown(KeyCode.F10))
			{
				int width2 = Screen.width;
				int height2 = width2 / 16 * 9;
				Screen.SetResolution(width2, height2, key);
				WidgetFeed.Instance?.Nerun(width2 + "/" + height2);
			}
		}
		else
		{
			Input.GetKeyDown(KeyCode.F9);
			if (Input.GetKey(KeyCode.F9))
			{
				EClass.scene.paused = false;
				for (int k = 0; k < advanceMin; k++)
				{
					EClass.game.updater.FixedUpdate();
				}
				EClass.game.world.date.AdvanceMin(advanceMin);
			}
			if (Input.GetKeyDown(KeyCode.F10))
			{
				switch (EScriptable.rnd(5))
				{
				case 0:
					EClass.world.weather.SetCondition(Weather.Condition.Rain);
					break;
				case 1:
					EClass.world.weather.SetCondition(Weather.Condition.Snow);
					break;
				case 2:
					EClass.world.weather.SetCondition(Weather.Condition.Ether);
					break;
				default:
					EClass.world.weather.SetCondition(Weather.Condition.Fine);
					break;
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.N) && !EInput.isShiftDown && !EInput.isCtrlDown)
		{
			EClass.Sound.NextBGM();
		}
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (key)
			{
				EClass.ui.AddLayer<LayerConsole>();
			}
			else
			{
				EClass.ui.widgets.Toggle("Debug");
			}
		}
		Point hitPoint = Scene.HitPoint;
		if (!hitPoint.IsValid || EClass.ui.IsActive)
		{
			return;
		}
		if (hitPoint.detail != null)
		{
			_ = hitPoint.area;
		}
		if (hitPoint.detail?.things == null)
		{
			new List<Thing>();
		}
		if (hitPoint.detail?.charas == null)
		{
			new List<Chara>();
		}
		List<Card> list = hitPoint.ListCards();
		if (Input.GetMouseButtonDown(2))
		{
			if (EClass.core.releaseMode != 0)
			{
				if (key)
				{
					EClass._zone.SpawnMob(hitPoint);
				}
				else if (key3)
				{
					EClass._zone.SpawnMob(hitPoint);
				}
				else if (key2)
				{
					EClass._zone.SpawnMob(hitPoint);
				}
			}
			return;
		}
		if (key2)
		{
			if (Input.GetMouseButtonDown(0))
			{
				foreach (Card item in list)
				{
					Debug.Log(item.Name + "/" + item.dir + "/" + item.flipX + "/" + item.angle);
				}
			}
			if (Input.GetMouseButtonDown(1))
			{
				if (key)
				{
					EClass._map.charas.ForeachReverse(delegate(Chara c)
					{
						if (c.IsHostile(EClass.pc))
						{
							c.DamageHP(9999999, AttackSource.Finish, EClass.pc);
						}
					});
				}
				else if (hitPoint.detail != null)
				{
					for (int num = hitPoint.detail.charas.Count - 1; num >= 0; num--)
					{
						hitPoint.detail.charas[num].DamageHP(9999999, AttackSource.Finish, EClass.pc);
					}
				}
				EInput.Consume();
			}
		}
		if (Input.GetKeyDown(KeyCode.Delete))
		{
			if (Input.GetKey(KeyCode.RightControl))
			{
				return;
			}
			if (key)
			{
				foreach (Card item2 in EClass._map.Roaming.all)
				{
					EClass._zone.RemoveCard(item2);
				}
			}
			else if (hitPoint.detail != null)
			{
				for (int num2 = hitPoint.detail.things.Count - 1; num2 >= 0; num2--)
				{
					hitPoint.detail.things[num2].Destroy();
				}
				if (hitPoint.detail != null)
				{
					for (int num3 = hitPoint.detail.charas.Count - 1; num3 >= 0; num3--)
					{
						hitPoint.detail.charas[num3].Destroy();
					}
				}
			}
		}
		if (Input.GetKeyDown(KeyCode.End) && hitPoint.detail != null)
		{
			for (int num4 = hitPoint.detail.things.Count - 1; num4 >= 0; num4--)
			{
				Thing thing3 = hitPoint.detail.things[num4];
				Debug.Log(thing3.id + "/" + thing3.Pref.height + "/" + thing3.trait?.ToString() + "/" + thing3.source.tileType.CanStack + "/" + thing3.source.tileType?.ToString() + "/" + thing3.isSynced + "/" + RenderObject.syncList.Contains(thing3.renderer));
			}
		}
		if (!Application.isEditor)
		{
			return;
		}
		switch (debugHotkeys)
		{
		case DebugHotkey.Anime:
		{
			int num5 = -1;
			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				num5 = 0;
			}
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				num5 = 1;
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				num5 = 2;
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				num5 = 3;
			}
			if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				num5 = 4;
			}
			if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				num5 = 5;
			}
			if (Input.GetKeyDown(KeyCode.Alpha6))
			{
				num5 = 6;
			}
			if (Input.GetKeyDown(KeyCode.Alpha7))
			{
				num5 = 7;
			}
			if (Input.GetKeyDown(KeyCode.Alpha8))
			{
				num5 = 8;
			}
			if (Input.GetKeyDown(KeyCode.Alpha9))
			{
				num5 = 9;
			}
			if (num5 == -1)
			{
				break;
			}
			foreach (Card item3 in hitPoint.ListCards())
			{
				item3.renderer.PlayAnime(num5.ToEnum<AnimeID>());
			}
			Debug.Log(num5.ToEnum<AnimeID>());
			break;
		}
		case DebugHotkey.Block:
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				EClass._map.SetLiquid(hitPoint.x, hitPoint.z, 1, 9);
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				EClass._map.SetLiquid(hitPoint.x, hitPoint.z, 2, 9);
			}
			break;
		case DebugHotkey.Item:
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				Thing thing4 = ThingGen.Create("stairsDown_cave");
				EClass._zone.AddCard(thing4, EClass.pc.pos);
				thing4.SetPlaceState(PlaceState.installed);
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				Thing thing5 = ThingGen.Create("stairs");
				EClass._zone.AddCard(thing5, EClass.pc.pos);
				thing5.SetPlaceState(PlaceState.installed);
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				Thing thing6 = ThingGen.Create("sign");
				EClass._zone.AddCard(thing6, hitPoint);
				thing6.SetPlaceState(PlaceState.installed);
			}
			if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				Thing thing7 = ThingGen.Create("sign2");
				EClass._zone.AddCard(thing7, hitPoint);
				thing7.SetPlaceState(PlaceState.installed);
			}
			if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				Thing thing8 = ThingGen.Create("well");
				EClass._zone.AddCard(thing8, hitPoint);
				thing8.SetPlaceState(PlaceState.installed);
			}
			if (Input.GetKeyDown(KeyCode.Alpha6))
			{
				Thing thing9 = ThingGen.Create("altar");
				EClass._zone.AddCard(thing9, hitPoint);
				thing9.SetPlaceState(PlaceState.installed);
			}
			if (Input.GetKeyDown(KeyCode.Alpha7))
			{
				Thing t = ThingGen.Create("torch");
				EClass._zone.AddCard(t, hitPoint);
			}
			if (Input.GetKeyDown(KeyCode.Alpha8))
			{
				Thing t2 = ThingGen.Create("street_lamp");
				EClass._zone.AddCard(t2, hitPoint);
			}
			if (Input.GetKeyDown(KeyCode.Alpha9))
			{
				Thing t3 = ThingGen.Create("statue_elin");
				EClass._zone.AddCard(t3, hitPoint);
			}
			if (Input.GetKeyDown(KeyCode.Alpha0))
			{
				Thing t4 = ThingGen.TestCreate();
				EClass._zone.AddCard(t4, hitPoint);
			}
			if (key && Input.GetKeyDown(KeyCode.Alpha1))
			{
				Chara t5 = CharaGen.Create("korgon");
				EClass._zone.AddCard(t5, hitPoint);
			}
			break;
		case DebugHotkey.Decal:
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				EClass._map.AddDecal(hitPoint.x, hitPoint.z, EClass.pc.material.decal);
			}
			if (!Input.GetKeyDown(KeyCode.Alpha2))
			{
			}
			break;
		case DebugHotkey.Test:
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				QualitySettings.vSyncCount = 0;
				Application.targetFrameRate = 60;
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				QualitySettings.vSyncCount = 0;
				Application.targetFrameRate = 20;
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				QualitySettings.vSyncCount = 0;
				Application.targetFrameRate = 30;
			}
			if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				QualitySettings.vSyncCount = 0;
				Application.targetFrameRate = 40;
			}
			break;
		}
		Card card = (Card)(((object)hitPoint.FirstChara) ?? ((object)hitPoint.FirstThing));
		if (card == null)
		{
			return;
		}
		CharaActorPCC charaActorPCC = (card.renderer.actor as CharaActor) as CharaActorPCC;
		if ((bool)charaActorPCC)
		{
			if (Input.GetKeyDown(KeyCode.Keypad0))
			{
				charaActorPCC.NextFrame();
				charaActorPCC.RefreshSprite();
			}
			if (Input.GetKeyDown(KeyCode.Keypad1))
			{
				charaActorPCC.NextDir();
				card.angle = charaActorPCC.provider.angle;
				charaActorPCC.RefreshSprite();
			}
			if (Input.GetKeyDown(KeyCode.Keypad2))
			{
				charaActorPCC.pcc.data.Randomize();
				charaActorPCC.provider.Rebuild();
			}
		}
		if (Input.GetKeyDown(KeyCode.Keypad4))
		{
			lastEmo = lastEmo.NextEnum();
			card.ShowEmo();
			card.ShowEmo(lastEmo);
		}
		if (Input.GetKeyDown(KeyCode.Keypad5))
		{
			card.ShowEmo();
			card.ShowEmo(lastEmo);
		}
		if (Input.GetKeyDown(KeyCode.Keypad7))
		{
			lastAnime = lastAnime.NextEnum();
			card.renderer.PlayAnime(lastAnime);
		}
		if (Input.GetKeyDown(KeyCode.Keypad8))
		{
			card.renderer.PlayAnime(lastAnime);
			animeDest = card;
		}
		if (Input.GetKeyDown(KeyCode.Keypad9))
		{
			card.renderer.PlayAnime(AnimeID.Attack, animeDest);
		}
	}

	public void InitDebugCommands()
	{
		commands.Clear();
		int cat2 = 0;
		Add(cat2, "Save Widgets", delegate
		{
			EClass.ui.widgets.Save();
		});
		Add(cat2, "Save Widgets(Dialog)", delegate
		{
			EClass.ui.widgets.DialogSave();
		});
		Add(cat2, "Export Zone", delegate
		{
			EClass._zone.Export();
		});
		Add(cat2, "Export Zone(Dialog)", delegate
		{
			EClass._zone.ExportDialog();
		});
		Add(0, "Import Zone(Dialog)", delegate
		{
			EClass._zone.ImportDialog();
		});
		Add(cat2, "Validate Backer Contents", delegate
		{
			foreach (SourceBacker.Row row2 in EClass.sources.backers.rows)
			{
				if (row2.valid)
				{
					switch (row2.type)
					{
					case 1:
						if (!EClass.sources.things.map.ContainsKey(row2.loot))
						{
							Debug.Log(row2.id + "/remain/" + row2.Name + "/" + row2.loot);
						}
						break;
					case 4:
						if (!EClass.sources.charas.map.ContainsKey(row2.chara))
						{
							Debug.Log(row2.id + "/pet/" + row2.Name + "/" + row2.chara);
						}
						break;
					case 6:
						if (!EClass.sources.religions.map.ContainsKey(row2.deity.ToLower()))
						{
							Debug.Log(row2.id + "/" + row2.Name + "/follower/" + row2.deity);
						}
						break;
					}
				}
			}
		});
		Add(cat2, "Edit PCC", delegate
		{
			EClass.ui.AddLayer<LayerEditPCC>().Activate(EClass.pc, UIPCC.Mode.Full);
		});
		Add(cat2, "COM_Teleport", COM_Teleport);
		Add(cat2, "LOG_Spatials", LOG_Spatials);
		Add(cat2, "Play Start Drama", COM_PlayStartDrama);
		Add(cat2, "Fix Floating Items", delegate
		{
			foreach (Thing thing in EClass._map.things)
			{
				if (thing.Pref.Float)
				{
					thing.isFloating = true;
				}
			}
		});
		cat2 = 1;
		Add(cat2, "Add Conditions", delegate
		{
			if (EScriptable.rnd(2) == 0)
			{
				EClass.pc.AddCondition<ConWet>();
			}
			else
			{
				EClass.pc.AddCondition<ConSuffocation>();
			}
			if (EScriptable.rnd(2) == 0)
			{
				EClass.pc.AddCondition<ConPoison>();
			}
			else
			{
				EClass.pc.AddCondition<ConDisease>();
			}
		});
		cat2 = 2;
		Add(cat2, "Max Construction", COM_MaxConstruction);
		Add(cat2, "Add Reserves", delegate
		{
			EClass.Home.AddReserve(CharaGen.Create("merc_archer"));
			EClass.Home.AddReserve(CharaGen.Create("healer"));
			EClass.Home.AddReserve(CharaGen.Create("bartender"));
			EClass.Home.AddReserve(CharaGen.Create("merchant"));
			EClass.Home.AddReserve(CharaGen.CreateFromFilter("c_wilds"));
			EClass.Home.AddReserve(CharaGen.CreateFromFilter("c_wilds"));
			EClass.Home.AddReserve(CharaGen.CreateFromFilter("c_wilds"));
		});
		Add(cat2, "Add Recruits", delegate
		{
			if (EClass.Branch != null)
			{
				EClass.Branch.AddRecruit(CharaGen.Create("merc_archer"));
				EClass.Branch.AddRecruit(CharaGen.Create("healer"));
				EClass.Branch.AddRecruit(CharaGen.Create("bartender"));
				EClass.Branch.AddRecruit(CharaGen.Create("merchant"));
				EClass.Branch.AddRecruit(CharaGen.CreateFromFilter("c_wilds"));
				EClass.Branch.AddRecruit(CharaGen.CreateFromFilter("c_wilds"));
				EClass.Branch.AddRecruit(CharaGen.CreateFromFilter("c_wilds"));
			}
		});
		Add(cat2, "Add Resources", delegate
		{
			if (EClass.Branch != null)
			{
				EClass.Branch.resources.food.Mod(100);
				EClass.Branch.resources.knowledge.Mod(100);
			}
		});
		Add(cat2, "Add Influence", delegate
		{
			EClass._zone.influence += 100;
		});
		Add(cat2, "Reroll Hobbies", delegate
		{
			foreach (Chara chara in EClass._map.charas)
			{
				chara.RerollHobby();
			}
		});
		Add(cat2, "Test_Siege", Test_Siege);
		Add(cat2, "Test_SiegeGuard", Test_SiegeGuard);
		Add(cat2, "Log_BranchMembers", delegate
		{
			if (EClass._zone.IsPCFaction)
			{
				Debug.Log(EClass.Branch.members.Count);
				foreach (Chara member in EClass.Branch.members)
				{
					EClass.debug.Log(member);
				}
			}
		});
		cat2 = 3;
		Add(cat2, "Weather.Fine", delegate
		{
			EClass.world.weather.SetCondition(Weather.Condition.Fine);
		});
		Add(cat2, "Weather.Blossom", delegate
		{
			EClass.world.weather.SetCondition(Weather.Condition.Blossom);
		});
		Add(cat2, "Weather.Random", delegate
		{
			EClass.world.weather.SetRandomCondition();
		});
		Add(cat2, "Weather.Ether+50", delegate
		{
			EClass.world.ModEther(50);
			Debug.Log(EClass.world.ether + "/" + EClass.world.weather.CurrentCondition);
		});
		Add(cat2, "Season.Next", delegate
		{
			EClass.world.season.Next();
		});
		Add(cat2, "Unreveal Map", delegate
		{
			EClass._map.ForeachCell(delegate(Cell c)
			{
				c.isSeen = false;
			});
			WidgetMinimap.Instance.Reload();
		});
		Add(cat2, "Test_GodTalk", Test_GodTalk);
		Add(cat2, "Test_Filter", Test_Filter);
		Add(cat2, "Test_Grow", Test_Grow);
		Add(cat2, "Turn On All Lights", delegate
		{
			foreach (Thing thing2 in EClass._map.things)
			{
				if (thing2.trait.IsLighting)
				{
					thing2.trait.Toggle(on: true);
				}
			}
		});
		Add(cat2, "Reset All Custom Lights", delegate
		{
			foreach (Thing thing3 in EClass._map.things)
			{
				thing3.c_lightColor = 0;
				thing3.RecalculateFOV();
			}
		});
		Add(cat2, "Reset All obj materials", delegate
		{
			EClass._map.ForeachCell(delegate(Cell c)
			{
				if (c.HasObj)
				{
					c.objMat = (byte)c.sourceObj.DefaultMaterial.id;
				}
			});
		});
		Add(cat2, "Fix Floors under Blocks", delegate
		{
			EClass._map.ForeachCell(delegate(Cell c)
			{
				if (c.HasFullBlock)
				{
					SourceBlock.Row sourceBlock = c.sourceBlock;
					SourceFloor.Row row = EClass.sources.floors.alias[sourceBlock.autoFloor];
					EClass._map.SetFloor(c.x, c.z, row.DefaultMaterial.id, row.id);
				}
			});
		});
		Add(cat2, "Bless Inventory", delegate
		{
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				t.SetBlessedState(BlessedState.Blessed);
			});
		});
		Add(cat2, "Curse Inventory", delegate
		{
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				t.SetBlessedState(BlessedState.Cursed);
			});
		});
		Add(cat2, "List Global Charas", delegate
		{
			foreach (KeyValuePair<int, Chara> globalChara in EClass.game.cards.globalCharas)
			{
				Debug.Log(globalChara.Key + "/" + globalChara.Value.Name + "/" + ((globalChara.Value.currentZone == null) ? "NULL" : globalChara.Value.currentZone.Name) + "/" + globalChara.Value.faction?.ToString() + "/" + globalChara.Value.homeBranch);
			}
		});
		Add(cat2, "List Global Charas In Zone", delegate
		{
			foreach (KeyValuePair<int, Chara> globalChara2 in EClass.game.cards.globalCharas)
			{
				if (globalChara2.Value.currentZone == EClass._zone)
				{
					Debug.Log(globalChara2.Key + "/" + globalChara2.Value);
				}
			}
		});
		Add(cat2, "List Citizen", delegate
		{
			foreach (KeyValuePair<int, string> p in EClass._zone.dictCitizen)
			{
				Debug.Log(p.Value);
				Debug.Log(EClass._map.charas.Find((Chara c) => c.uid == p.Key));
				Debug.Log(EClass._map.deadCharas.Find((Chara c) => c.uid == p.Key));
			}
		});
		void Add(int cat, string id, Action action)
		{
			DebugCommand item = new DebugCommand
			{
				name = id,
				action = action,
				cat = cat
			};
			commands.Add(item);
		}
	}

	public void Test_Grow()
	{
		EClass._map.ForeachCell(delegate(Cell c)
		{
			if (c.sourceObj.HasGrowth)
			{
				c.TryGrow();
			}
		});
		foreach (Thing item in EClass._map.things.Copy())
		{
			if (item.trait is TraitSeed traitSeed)
			{
				traitSeed.TrySprout(force: true);
			}
		}
	}

	public void Test_GodTalk()
	{
		foreach (Religion value in EClass.game.religions.dictAll.Values)
		{
			value.Talk("test");
		}
	}

	public void COM_Teleport()
	{
		for (int i = 0; i < 10000; i++)
		{
			Point point = EClass.pc.pos.Copy();
			point.x += EScriptable.rnd(60) - EScriptable.rnd(60);
			point.z += EScriptable.rnd(60) - EScriptable.rnd(60);
			if (point.IsValid && !point.cell.blocked && point.HasFloor)
			{
				EClass.pc.Teleport(point);
				break;
			}
		}
	}

	public void COM_PlayStartDrama()
	{
		EClass.ui.CloseLayers();
		EClass.game.world.date.hour = 2;
		EClass.scene.screenElin.RefreshAll();
		LayerDrama.ActivateMain("mono", "1-2");
	}

	public void Test_Filter()
	{
		bilinear = !bilinear;
		MeshPass[] passes = EClass.scene.passes;
		for (int i = 0; i < passes.Length; i++)
		{
			passes[i].mat.GetTexture("_MainTex").filterMode = (bilinear ? FilterMode.Bilinear : FilterMode.Point);
		}
	}

	public void LOG_Spatials()
	{
		foreach (Spatial child in EClass.world.region.children)
		{
			Debug.Log(child.uid + "/" + child.Name + "/" + child.mainFaction.name + "/" + (child == EClass.player.zone));
		}
	}

	public void Test_SiegeGuard()
	{
		if (EClass._zone.events.GetEvent<ZoneEventSiegeGuard>() == null)
		{
			EClass._zone.events.Add(new ZoneEventSiegeGuard());
		}
		else
		{
			EClass._zone.events.Remove<ZoneEventSiegeGuard>();
		}
	}

	public void Test_Siege()
	{
		if (EClass._zone.events.GetEvent<ZoneEventSiege>() == null)
		{
			EClass._zone.events.Add(new ZoneEventSiege());
		}
		else
		{
			EClass._zone.events.Remove<ZoneEventSiege>();
		}
	}

	public void COM_MaxConstruction()
	{
		if (EClass.pc.homeBranch != null)
		{
			EClass.pc.homeBranch.owner.elements.ModBase(2003, 50);
		}
	}

	public static bool CheatEnabled()
	{
		if (!EClass.debug.enable)
		{
			return EClass.game.config.cheat;
		}
		return true;
	}

	[ConsoleCommand("")]
	public static string Cheat()
	{
		EClass.game.config.cheat = true;
		return "Cheat Enabled";
	}

	[ConsoleCommand("")]
	public static string Reset_LoytelDebt()
	{
		QuestDebt questDebt = EClass.game.quests.Get<QuestDebt>();
		if (questDebt == null)
		{
			return "Quest Status Not Valid.";
		}
		questDebt.stage = 0;
		questDebt.paid = false;
		questDebt.gaveBill = false;
		EClass.player.debt = 20000000;
		Thing thing = ThingGen.Create("856");
		thing.refVal = 109;
		EClass.pc.Pick(thing);
		return "Quest Reset!";
	}

	[ConsoleCommand("")]
	public static string Fix_RemoveDuplicateUnique()
	{
		if (EClass.Branch == null)
		{
			return "No Branch";
		}
		EClass.Branch.members.ForeachReverse(delegate(Chara c)
		{
			if ((c.id == "kettle" || c.id == "quru") && EClass.Branch.members.Where((Chara c2) => c2.id == c.id).Count() >= 2)
			{
				EClass.Branch.RemoveMemeber(c);
				c.Destroy();
			}
		});
		return "Fixed!";
	}

	[ConsoleCommand("")]
	public static string Fix_EtherDisease()
	{
		EClass.pc.ModCorruption(-100000);
		Chara chara = CharaGen.Create("chara");
		chara.ChangeRace(EClass.pc.race.id);
		chara.ChangeJob(EClass.pc.job.id);
		string text = "";
		foreach (Element value in chara.elements.dict.Values)
		{
			if (!(value.source.category != "attribute"))
			{
				Element orCreateElement = EClass.pc.elements.GetOrCreateElement(value.id);
				if (value.vBase > orCreateElement.vBase)
				{
					text = text + "Fixing Base Value:" + orCreateElement.Name + " Before:" + orCreateElement.vBase + " Now:" + (value.vBase + 1) + Environment.NewLine;
					EClass.pc.elements.ModBase(orCreateElement.id, value.vBase - orCreateElement.vBase + 1);
				}
			}
		}
		return text + "Fixed!";
	}

	[ConsoleCommand("")]
	public static string Fix_LostCore()
	{
		if (!EClass._zone.IsPCFaction)
		{
			return "Not in base.";
		}
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.trait is TraitCoreZone)
			{
				return "Base already has core.";
			}
		}
		if (EClass.pc.things.Find((Thing t) => t.trait is TraitCoreZone) != null)
		{
			return "Player has core.";
		}
		EClass._zone.AddCard(ThingGen.Create("core_zone"), EClass._zone.bounds.GetCenterPos().GetNearestPoint());
		return "Done.";
	}

	[ConsoleCommand("")]
	public static string Fix_RemoveDemitas()
	{
		List<Chara> list = new List<Chara>();
		foreach (Chara value in EClass.game.cards.globalCharas.Values)
		{
			if (value.id == "demitas" && value.currentZone == EClass._zone)
			{
				list.Add(value);
			}
		}
		if (list.Count > 1)
		{
			Chara chara = list[1];
			chara.homeBranch.BanishMember(chara);
			chara.Destroy();
			return "Demitas Removed!";
		}
		return "Not enough Demitas!";
	}

	[ConsoleCommand("")]
	public static string ListChara()
	{
		string text = "";
		foreach (SourceChara.Row row in EClass.sources.charas.rows)
		{
			text = text + row.id + "\n";
		}
		return text;
	}

	[ConsoleCommand("")]
	public static string ListThing()
	{
		string text = "";
		foreach (SourceThing.Row row in EClass.sources.things.rows)
		{
			text = text + row.id + "\n";
		}
		return text;
	}

	[ConsoleCommand("")]
	public static string SetElement(string alias, int value, int potential = 100)
	{
		if (!CheatEnabled())
		{
			return EnableCheat;
		}
		SourceElement.Row row = EClass.sources.elements.alias.TryGetValue(alias);
		if (row == null)
		{
			return "Element not found.";
		}
		if (row.type == "Feat")
		{
			EClass.pc.SetFeat(row.id, value, msg: true);
		}
		else
		{
			EClass.pc.elements.SetBase(row.id, value, potential);
		}
		return "Done.";
	}

	[ConsoleCommand("")]
	public static string SpawnBoss(string id)
	{
		if (!CheatEnabled())
		{
			return EnableCheat;
		}
		if (EClass.sources.charas.map.ContainsKey(id))
		{
			Chara chara = EClass._zone.SpawnMob(EClass.pc.pos.GetNearestPoint(), SpawnSetting.Boss(id));
			return "Spawned " + chara.Name;
		}
		return "'" + id + "' does not exist in the database.";
	}

	[ConsoleCommand("")]
	public static string Spawn(string id, int num = 1, string aliasMat = "", int objLv = -1)
	{
		if (!CheatEnabled())
		{
			return EnableCheat;
		}
		if (EClass.sources.things.map.ContainsKey(id))
		{
			Thing thing = ThingGen.Create(id, -1, objLv).SetNum(num);
			if (!aliasMat.IsEmpty())
			{
				thing.ChangeMaterial(aliasMat);
			}
			EClass._zone.AddCard(thing, EClass.pc.pos);
			return "Spawned " + thing.Name;
		}
		if (EClass.sources.charas.map.ContainsKey(id))
		{
			Chara chara = CharaGen.Create(id);
			if (objLv != -1)
			{
				chara.SetLv(objLv);
			}
			EClass._zone.AddCard(chara, EClass.pc.pos);
			return "Spawned " + chara.Name;
		}
		return "'" + id + "' does not exist in the database.";
	}

	[ConsoleCommand("")]
	public static string TestSpawn(int lv, int num, int lvRange = -1)
	{
		if (!CheatEnabled())
		{
			return EnableCheat;
		}
		if (EClass._zone is Zone_Arena)
		{
			EClass._zone._dangerLv = lv;
		}
		foreach (Chara item in EClass._map.charas.Where((Chara c) => c.HasEditorTag(EditorTag.SpawnTest)).ToList())
		{
			item.Destroy();
		}
		for (int i = 0; i < num; i++)
		{
			EClass._zone.SpawnMob(EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false, ignoreCenter: true), new SpawnSetting
			{
				filterLv = lv,
				levelRange = lvRange
			}).AddEditorTag(EditorTag.SpawnTest);
		}
		return "Spawned.";
	}

	[ConsoleCommand("")]
	public static string ResetPetUpgrades()
	{
		foreach (Chara value in EClass.game.cards.globalCharas.Values)
		{
			if (value.IsPCFaction)
			{
				value.ResetUpgrade();
			}
		}
		return "Not Implemented.";
	}

	[ConsoleCommand("")]
	public static string ResetPrincipalSeals()
	{
		if (!CheatEnabled())
		{
			return EnableCheat;
		}
		EClass.game.principal.modified.Clear();
		return "Done.";
	}

	[ConsoleCommand("")]
	public static string GodMode()
	{
		if (!CheatEnabled())
		{
			return EnableCheat;
		}
		EClass.pc.Revive();
		EClass.pc.hp = EClass.pc.MaxHP;
		return "I'm God!";
	}

	[ConsoleCommand("")]
	public static string ModFame(int amount)
	{
		if (!CheatEnabled())
		{
			return EnableCheat;
		}
		EClass.player.ModFame(amount);
		return "Done.";
	}

	[ConsoleCommand("")]
	public static string ModKarma(int amount)
	{
		if (!CheatEnabled())
		{
			return EnableCheat;
		}
		EClass.player.ModKarma(amount);
		return "Done.";
	}

	[ConsoleCommand("")]
	public static string ModContribution(int amount)
	{
		if (!CheatEnabled())
		{
			return EnableCheat;
		}
		Guild.GetCurrentGuild()?.AddContribution(amount);
		return "Done.";
	}

	[ConsoleCommand("")]
	public static string FlyMode()
	{
		EClass.pc.AddCondition<ConLevitate>();
		return "I can fly!";
	}

	[ConsoleCommand("")]
	public static string ChangeRace(string id = "?")
	{
		if (EClass.sources.races.map.ContainsKey(id))
		{
			Thing thing = null;
			foreach (BodySlot slot in EClass.pc.body.slots)
			{
				if (slot.thing != null)
				{
					if (slot.thing.blessedState <= BlessedState.Cursed)
					{
						slot.thing.blessedState = BlessedState.Normal;
					}
					if (slot.thing.trait is TraitToolBelt)
					{
						thing = slot.thing;
					}
					EClass.pc.body.Unequip(slot);
				}
			}
			EClass.pc.body.RemoveBodyPart(45);
			EClass.pc.body.RemoveBodyPart(44);
			EClass.pc.ChangeRace(id);
			if (EClass.ui.IsInventoryOpen)
			{
				EClass.ui.ToggleInventory();
			}
			EClass.pc.body.AddBodyPart(45);
			EClass.pc.body.AddBodyPart(44);
			EClass.pc.body.Equip(thing);
			return "Done.";
		}
		string text = "";
		foreach (SourceRace.Row row in EClass.sources.races.rows)
		{
			text = text + row.id + " " + row.GetName() + Environment.NewLine;
		}
		return text;
	}

	[ConsoleCommand("")]
	public static string ChangeJob(string id = "?")
	{
		if (EClass.sources.jobs.map.ContainsKey(id))
		{
			EClass.pc.ChangeJob(id);
			EClass.player.RefreshDomain();
			return "Done.";
		}
		string text = "";
		foreach (SourceJob.Row row in EClass.sources.jobs.rows)
		{
			text = text + row.id + " " + row.GetName() + Environment.NewLine;
		}
		return text;
	}

	[ConsoleCommand("")]
	public static string FirstAdventurer()
	{
		string text = "Steam is not running.";
		DateTime dateTime = new DateTime(2024, 11, 3, 6, 0, 0);
		DateTimeOffset dateTimeOffset = (Application.isEditor ? new DateTimeOffset(2024, 11, 6, 1, 9, 0, default(TimeSpan)) : ((!SteamAPI.IsSteamRunning()) ? new DateTimeOffset(9999, 1, 1, 1, 9, 0, default(TimeSpan)) : DateTimeOffset.FromUnixTimeSeconds(SteamApps.GetEarliestPurchaseUnixTime(EClass.core.steam.steamworks.settings.applicationId))));
		dateTimeOffset = dateTimeOffset.ToOffset(new TimeSpan(9, 0, 0));
		text = ((dateTimeOffset < dateTime) ? "Valid: " : "Invalid: ");
		text = text + "Arrived North Tyris on " + dateTimeOffset.Year + " " + dateTimeOffset.Month + "/" + dateTimeOffset.Day + " " + dateTimeOffset.Hour + ":" + dateTimeOffset.Minute;
		return text + "  Eligible by " + dateTime.Year + " " + dateTime.Month + "/" + dateTime.Day + " " + dateTime.Hour + ":" + dateTime.Minute;
	}

	[ConsoleCommand("")]
	public static string RegenerateNames()
	{
		EClass.core.mods.InitLang();
		NameGen.list = null;
		AliasGen.list = null;
		NameGen.Init();
		AliasGen.Init();
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsPC)
			{
				if (chara.source.name == "*r")
				{
					chara.c_altName = NameGen.getRandomName();
				}
				if (!chara._alias.IsEmpty())
				{
					chara._alias = AliasGen.GetRandomAlias();
				}
			}
		}
		return "Done!";
	}

	[ConsoleCommand("")]
	public static string AllRecipe(bool forget = false)
	{
		if (!CheatEnabled())
		{
			return EnableCheat;
		}
		if (forget)
		{
			EClass.player.recipes.knownRecipes.Clear();
		}
		else
		{
			EClass.debug.AddAllRecipes();
		}
		return "Done!";
	}

	[ConsoleCommand("")]
	public static string LastWish(string name)
	{
		string text = "";
		foreach (SourceBacker.Row row in EClass.sources.backers.rows)
		{
			if (row.name.Contains(name) || row.id.ToString() == name)
			{
				text = text + row.id + " valid?:" + row.valid + " lang:" + row.lang + " type:" + row.type + " destroyed:" + EClass.player.doneBackers.Contains(row.id) + " loc:" + (row.loc.IsEmpty() ? "random" : row.loc) + Environment.NewLine;
			}
		}
		if (text == "")
		{
			return "Not Found";
		}
		return text + Resources.Load<TextAsset>("logo2").text;
	}

	[ConsoleCommand("")]
	public static string ClearLastWishFlag()
	{
		if (!CheatEnabled())
		{
			return EnableCheat;
		}
		EClass.player.doneBackers.Clear();
		return Resources.Load<TextAsset>("logo2").text + Environment.NewLine + "Done!";
	}

	[ConsoleCommand("")]
	public static string Resource()
	{
		if (!CheatEnabled())
		{
			return EnableCheat;
		}
		EClass.debug.SpawnCheatContainer();
		return "Resources spawned.";
	}

	public void Log(object o)
	{
		Debug.Log(o);
	}

	public void NextBGM()
	{
		if ((bool)SoundManager.current)
		{
			SoundManager.current.NextBGM();
		}
	}

	public void ToggleRevealMap()
	{
		revealMap = !revealMap;
		if (revealMap)
		{
			EClass._map.RevealAll();
		}
		SE.ClickGeneral();
	}

	public void LoadBroadcast()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Data/Text/broadcast");
		blines = textAsset.text.Split('-');
	}

	public void BroadcastNext()
	{
		LoadBroadcast();
		if (Input.GetKey(KeyCode.LeftControl))
		{
			bidx--;
		}
		bidx = Mathf.Clamp(bidx, 0, blines.Length - 1);
		string text = blines[bidx];
		bidx += ((!Input.GetKey(KeyCode.LeftShift)) ? 1 : (-1));
		EClass.pc.SayRaw(text.TrimNewLines());
	}
}
