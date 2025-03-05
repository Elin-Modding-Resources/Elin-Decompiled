using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

public class Player : EClass
{
	public class ReturnInfo
	{
		public int turns;

		public int uidDest;

		public bool isEvac;

		public bool askDest;
	}

	public class Pref
	{
		[JsonIgnore]
		public string lastBuildCategory;

		[JsonIgnore]
		public string lastBuildRecipe;

		public int lastIdTabAbility;

		public bool layerInventory;

		public bool layerAbility;

		public bool layerCraft;

		public bool sort_ascending;

		public bool sort_ascending_shop;

		public UIList.SortMode sortResources;

		public UIList.SortMode sortPeople;

		public UIList.SortMode sortResearch = UIList.SortMode.ByCategory;

		public UIList.SortMode sortInv = UIList.SortMode.ByCategory;

		public UIList.SortMode sortInvShop = UIList.SortMode.ByValue;

		public UIList.SortMode sortAbility = UIList.SortMode.ByID;

		public LayerPeople.ShowMode modePoeple;

		public UIInventory.InteractMode interactMode;
	}

	public class Stats
	{
		public double timeElapsed;

		public int kumi;

		public int mins;

		public int days;

		public int months;

		public int sieges;

		public int turns;

		public int kills;

		public int taxBills;

		public int taxBillsPaid;

		public int digs;

		public int shipNum;

		public int slept;

		public int death;

		public int allyDeath;

		public int deepest;

		public int gambleChest;

		public int gambleChestOpen;

		public long shipMoney;

		[JsonIgnore]
		public int lastShippingExp;

		[JsonIgnore]
		public int lastShippingExpMax;

		public int GetShippingBonus(long _a)
		{
			long p = 0L;
			bool first = true;
			SetBonus(1000000000, 50000);
			SetBonus(100000000, 20000);
			SetBonus(10000000, 10000);
			SetBonus(500000, 5000);
			SetBonus(250000, 3000);
			SetBonus(100000, 2000);
			SetBonus(20000, 1000);
			SetBonus(6000, 500);
			SetBonus(2000, 200);
			SetBonus(0, 100);
			return (int)p;
			void SetBonus(int threshold, int div)
			{
				if (_a >= threshold)
				{
					p += (_a - threshold) / div;
					if (first)
					{
						lastShippingExp = (int)((_a - threshold) % div);
						lastShippingExpMax = div;
						first = false;
					}
					_a = threshold;
				}
			}
		}
	}

	public class Flags : EClass
	{
		public const int MonoDeparture = 100;

		[JsonProperty]
		public int[] ints = new int[50];

		[JsonProperty]
		public List<int> playedStories = new List<int>();

		[JsonProperty]
		public List<int> availableStories = new List<int>();

		[JsonProperty]
		public HashSet<int> pleaseDontTouch = new HashSet<int>();

		[JsonProperty]
		public List<string> reservedTutorial = new List<string>();

		public BitArray32 bits;

		public Dictionary<string, ExcelData> storyExcel = new Dictionary<string, ExcelData>();

		public bool gotClickReward
		{
			get
			{
				return bits[1];
			}
			set
			{
				bits[1] = value;
			}
		}

		public bool welcome
		{
			get
			{
				return bits[2];
			}
			set
			{
				bits[2] = value;
			}
		}

		public bool fiamaStoryBookGiven
		{
			get
			{
				return bits[3];
			}
			set
			{
				bits[3] = value;
			}
		}

		public bool fiamaFirstDream
		{
			get
			{
				return bits[4];
			}
			set
			{
				bits[4] = value;
			}
		}

		public bool helpHighlightDisabled
		{
			get
			{
				return bits[5];
			}
			set
			{
				bits[5] = value;
			}
		}

		public bool pickedMelilithTreasure
		{
			get
			{
				return bits[6];
			}
			set
			{
				bits[6] = value;
			}
		}

		public bool isShoesOff
		{
			get
			{
				return bits[7];
			}
			set
			{
				bits[7] = value;
			}
		}

		public bool backpackHighlightDisabled
		{
			get
			{
				return bits[8];
			}
			set
			{
				bits[8] = value;
			}
		}

		public bool abilityHighlightDisabled
		{
			get
			{
				return bits[9];
			}
			set
			{
				bits[9] = value;
			}
		}

		public bool elinGift
		{
			get
			{
				return bits[10];
			}
			set
			{
				bits[10] = value;
			}
		}

		public bool gotMelilithCurse
		{
			get
			{
				return bits[11];
			}
			set
			{
				bits[11] = value;
			}
		}

		public bool canComupWithFoodRecipe
		{
			get
			{
				return bits[12];
			}
			set
			{
				bits[12] = value;
			}
		}

		public bool KilledBossInVoid
		{
			get
			{
				return bits[13];
			}
			set
			{
				bits[13] = value;
			}
		}

		public bool statueShipped
		{
			get
			{
				return bits[14];
			}
			set
			{
				bits[14] = value;
			}
		}

		public bool little_saved
		{
			get
			{
				return bits[15];
			}
			set
			{
				bits[15] = value;
			}
		}

		public bool little_killed
		{
			get
			{
				return bits[16];
			}
			set
			{
				bits[16] = value;
			}
		}

		public bool gotEtherDisease
		{
			get
			{
				return bits[17];
			}
			set
			{
				bits[17] = value;
			}
		}

		public bool loytelEscaped
		{
			get
			{
				return bits[18];
			}
			set
			{
				bits[18] = value;
			}
		}

		public bool toggleHotbarHighlightDisabled
		{
			get
			{
				return bits[19];
			}
			set
			{
				bits[19] = value;
			}
		}

		public bool debugEnabled
		{
			get
			{
				return bits[20];
			}
			set
			{
				bits[20] = value;
			}
		}

		public bool magicChestSent
		{
			get
			{
				return bits[21];
			}
			set
			{
				bits[21] = value;
			}
		}

		public bool toggleHotbarHighlightActivated
		{
			get
			{
				return bits[22];
			}
			set
			{
				bits[22] = value;
			}
		}

		public bool gotJurePillow
		{
			get
			{
				return bits[30];
			}
			set
			{
				bits[30] = value;
			}
		}

		public int start
		{
			get
			{
				return ints[10];
			}
			set
			{
				ints[10] = value;
			}
		}

		public int build
		{
			get
			{
				return ints[11];
			}
			set
			{
				ints[11] = value;
			}
		}

		public int main
		{
			get
			{
				return ints[13];
			}
			set
			{
				ints[13] = value;
			}
		}

		public int storyFiama
		{
			get
			{
				return ints[14];
			}
			set
			{
				ints[14] = value;
			}
		}

		public int lutz
		{
			get
			{
				return ints[15];
			}
			set
			{
				ints[15] = value;
			}
		}

		public int daysAfterQuestExploration
		{
			get
			{
				return ints[16];
			}
			set
			{
				ints[16] = value;
			}
		}

		public int landDeedBought
		{
			get
			{
				return ints[17];
			}
			set
			{
				ints[17] = value;
			}
		}

		public int loytelMartLv
		{
			get
			{
				return ints[18];
			}
			set
			{
				ints[18] = value;
			}
		}

		[OnSerializing]
		private void _OnSerializing(StreamingContext context)
		{
			ints[0] = (int)bits.Bits;
		}

		[OnDeserialized]
		private void _OnDeserialized(StreamingContext context)
		{
			bits.Bits = (uint)ints[0];
		}

		public void OnCreateGame()
		{
		}

		public void OnEnableDebug()
		{
		}

		public void OnBuild(Recipe r)
		{
			if (build == 0 && r.id == "workbench")
			{
				build = 1;
			}
		}

		public bool IsStoryPlayed(int flag)
		{
			return playedStories.Contains(flag);
		}

		public int GetStoryRowID(string idBook, string idStep)
		{
			foreach (Dictionary<string, string> item in GetStoryExcelData(idBook).sheets["index"].list)
			{
				if (item["step"] == idStep)
				{
					return item["id"].ToInt();
				}
			}
			return 0;
		}

		public Dictionary<string, string> GetStoryRow(string idBook, int id)
		{
			foreach (Dictionary<string, string> item in GetStoryExcelData(idBook).sheets["index"].list)
			{
				if (item["id"].ToInt() == id)
				{
					return item;
				}
			}
			return null;
		}

		public ExcelData GetStoryExcelData(string idBook)
		{
			ExcelData excelData = storyExcel.TryGetValue(idBook);
			if (excelData == null)
			{
				excelData = new ExcelData();
				excelData.path = CorePath.DramaData + idBook + ".xlsx";
				excelData.BuildList("index");
				storyExcel.Add(idBook, excelData);
			}
			return excelData;
		}

		public bool PlayStory(string idBook, int id, bool fromBook = false)
		{
			if (!fromBook && playedStories.Contains(id))
			{
				return false;
			}
			Dictionary<string, string> storyRow = GetStoryRow(idBook, id);
			SoundManager.ForceBGM();
			LayerDrama.Activate(idBook, storyRow["sheet"], storyRow["step"]);
			if (!fromBook && !playedStories.Contains(id))
			{
				playedStories.Add(id);
			}
			return true;
		}

		public bool PlayStory(int id, bool fromBook = false)
		{
			return PlayStory("_main", id, fromBook);
		}

		public void AddStory(int id)
		{
			if (!playedStories.Contains(id) && !availableStories.Contains(id))
			{
				availableStories.Add(id);
				Msg.Say("story_added");
				SE.WriteJournal();
			}
		}

		public void OnEnterZone()
		{
			if (EClass._zone.id == "nymelle")
			{
				if (EClass._zone.lv == -1)
				{
					AddStory(10);
				}
				if (EClass._zone.lv == -2)
				{
					AddStory(20);
				}
				if (EClass._zone.lv == -3)
				{
					AddStory(30);
				}
			}
			if (QuestMain.Phase == 700 && EClass._zone.id == "lothria")
			{
				PlayStory(50);
			}
			if ((EClass._zone.IsPCFaction || EClass._zone.id == EClass.game.Prologue.idStartZone) && availableStories.Count > 0)
			{
				PlayStory(availableStories[0]);
				availableStories.RemoveAt(0);
			}
		}

		public void OnLeaveZone()
		{
			if (QuestMain.Phase == 700)
			{
				PlayStory(40);
			}
		}
	}

	[JsonProperty]
	public ReturnInfo returnInfo;

	[JsonProperty]
	public int uidChara;

	[JsonProperty]
	public int uidSpawnZone;

	[JsonProperty]
	public int tutorialStep;

	[JsonProperty]
	public int karma;

	[JsonProperty]
	public int fame;

	[JsonProperty]
	public int expKnowledge;

	[JsonProperty]
	public int expInfluence;

	[JsonProperty]
	public int dateTravel;

	[JsonProperty]
	public int distanceTravel;

	[JsonProperty]
	public int uidLastTravelZone;

	[JsonProperty]
	public int uidLastZone;

	[JsonProperty]
	public int uidLastShippedZone;

	[JsonProperty]
	public int totalFeat;

	[JsonProperty]
	public int taxBills;

	[JsonProperty]
	public int unpaidBill;

	[JsonProperty]
	public int bankMoney;

	[JsonProperty]
	public int holyWell;

	[JsonProperty]
	public int safeTravel;

	[JsonProperty]
	public int hotbarPage;

	[JsonProperty]
	public int little_dead;

	[JsonProperty]
	public int little_saved;

	[JsonProperty]
	public int extraTax;

	[JsonProperty]
	public int lastFelmeraReward;

	[JsonProperty]
	public int uidLastTown;

	[JsonProperty]
	public int seedShrine;

	[JsonProperty]
	public int seedChest;

	[JsonProperty]
	public int debt;

	[JsonProperty]
	public int seedFortune;

	[JsonProperty]
	public int giftJure;

	[JsonProperty]
	public int luckycoin;

	[JsonProperty]
	public int customLightMod = 3;

	[JsonProperty]
	public int questRerollCost;

	[JsonProperty]
	public float angle;

	[JsonProperty]
	public bool hasNewQuest;

	[JsonProperty]
	public bool dailyGacha;

	[JsonProperty]
	public bool useSubWidgetTheme;

	[JsonProperty]
	public bool wellWished;

	[JsonProperty]
	public bool prayed;

	[JsonProperty]
	public bool questTracker;

	[JsonProperty]
	public bool showShippingResult;

	[JsonProperty]
	public bool isEditor;

	[JsonProperty]
	public bool openContainerCenter;

	[JsonProperty]
	public bool showWorkaround;

	[JsonProperty]
	public string title;

	[JsonProperty]
	public string memo = "";

	[JsonProperty]
	public string memo2 = "";

	[JsonProperty]
	public Pref pref = new Pref();

	[JsonProperty]
	public Stats stats = new Stats();

	[JsonProperty]
	public NumLogManager nums = new NumLogManager();

	[JsonProperty]
	public KnowledgeManager knowledges = new KnowledgeManager();

	[JsonProperty]
	public PopupManager popups = new PopupManager();

	[JsonProperty]
	public WidgetManager.SaveData mainWidgets;

	[JsonProperty]
	public WidgetManager.SaveData subWidgets;

	[JsonProperty]
	public Window.SaveData dataPick = new Window.SaveData();

	[JsonProperty]
	public LayerAbility.Config layerAbilityConfig = new LayerAbility.Config();

	[JsonProperty]
	public Dictionary<string, int> dialogFlags = new Dictionary<string, int>();

	[JsonProperty]
	public Dictionary<string, string> hangIcons = new Dictionary<string, string>();

	[JsonProperty]
	public Dictionary<string, KnownSong> knownSongs = new Dictionary<string, KnownSong>();

	[JsonProperty]
	public Dictionary<string, string> lastRecipes = new Dictionary<string, string>();

	[JsonProperty]
	public Dictionary<string, List<string>> priorityActions = new Dictionary<string, List<string>>();

	[JsonProperty]
	public HashSet<string> trackedCategories = new HashSet<string>();

	[JsonProperty]
	public HashSet<string> trackedCards = new HashSet<string>();

	[JsonProperty]
	public HashSet<int> trackedElements = new HashSet<int>();

	[JsonProperty]
	public HashSet<int> knownBGMs = new HashSet<int>();

	[JsonProperty]
	public HashSet<int> favAbility = new HashSet<int>();

	[JsonProperty]
	public HashSet<int> sketches = new HashSet<int>();

	[JsonProperty]
	public HashSet<int> doneBackers = new HashSet<int>();

	[JsonProperty]
	public HashSet<int> knownCraft = new HashSet<int>();

	[JsonProperty]
	public List<int> domains = new List<int>();

	[JsonProperty]
	public Zone zone;

	[JsonProperty]
	public HotbarManager hotbars = new HotbarManager();

	[JsonProperty]
	public Chara _agent;

	[JsonProperty]
	public Hoard hoard = new Hoard();

	[JsonProperty]
	public Flags flags = new Flags();

	[JsonProperty]
	public RecipeManager recipes = new RecipeManager();

	[JsonProperty]
	public HotItem currentHotItem = new HotItemNoItem();

	[JsonProperty]
	public Point lastZonePos;

	[JsonProperty]
	public Thing eqBait;

	[JsonProperty]
	public Dictionary<string, Window.SaveData> dataWindow;

	[JsonProperty]
	public CinemaConfig cinemaConfig = new CinemaConfig();

	[JsonProperty]
	public CodexManager codex = new CodexManager();

	[JsonProperty]
	public Dictionary<int, int> keyItems = new Dictionary<int, int>();

	[JsonProperty]
	public List<int> uidPickOnLoad = new List<int>();

	[JsonProperty]
	public List<ShippingResult> shippingResults = new List<ShippingResult>();

	[JsonProperty]
	public Dictionary<string, HashSet<string>> noRestocks = new Dictionary<string, HashSet<string>>();

	[JsonProperty]
	public Window.SaveData windowAllyInv;

	[JsonProperty]
	public List<string> favMoongate = new List<string>();

	public static int seedHallucination;

	public static int realHour;

	public Window.SaveData windowDataCopy;

	public string windowDataName;

	public ZoneTransition lastTransition;

	public List<Cell> lastMarkedHighlights = new List<Cell>();

	public HotItem lastHotItem;

	public HotItem hotItemToRestore;

	public bool forceTalk;

	public bool altHeldPos;

	public bool instaComplete = true;

	public bool regionMoveWarned;

	public bool waitingInput;

	public bool willEndTurn;

	public bool wasDirtyWeight;

	public bool deathDialog;

	public bool preventDeathPenalty;

	public bool deathZoneMove;

	public bool haltMove;

	public bool invlunerable;

	public bool willAutoSave;

	public bool resetPrincipal;

	public bool simulatingZone;

	public bool isAutoFarming;

	public bool enemySpotted;

	public string deathMsg;

	public int countNewline;

	public int lightRadius;

	public int lastTurn;

	public int lastEmptyAlly;

	public int ignoreFallOver;

	public float lightPower;

	public float baseActTime;

	public float pickupDelay = 2f;

	public Chara chara;

	public Chara focusedchara;

	public NoticeManager notices = new NoticeManager();

	public QueueManager queues = new QueueManager();

	public Act lastAct;

	public Vector2 nextMove;

	public Vector3 position;

	public Action onStartZone;

	public PlayingSong playingSong;

	public List<Chara> listSummon = new List<Chara>();

	public int autoCombatStartHP;

	public Zone nextZone;

	public Thing renderThing;

	public HotItemNoItem hotItemNoItem = new HotItemNoItem();

	public Chara target;

	public WidgetManager.SaveData widgets
	{
		get
		{
			if (!useSubWidgetTheme)
			{
				return mainWidgets;
			}
			return subWidgets;
		}
		set
		{
			if (useSubWidgetTheme)
			{
				subWidgets = value;
			}
			else
			{
				mainWidgets = value;
			}
		}
	}

	public Zone spawnZone
	{
		get
		{
			return RefZone.Get(uidSpawnZone);
		}
		set
		{
			uidSpawnZone = RefZone.Set(value);
		}
	}

	public bool EnableDreamStory => false;

	public Zone LastTravelZone => RefZone.Get(uidLastTravelZone);

	public Zone LastZone => RefZone.Get(uidLastZone);

	public Chara Agent => _agent;

	public int ContainerSearchDistance => 1 + EClass.pc.elements.GetFeatRef(1643);

	public int MaxAlly => Mathf.Min(Mathf.Max(EClass.pc.CHA / 10, 1), 5) + EClass.pc.Evalue(1645);

	public int MaxExpKnowledge => 1000;

	public int MaxExpInfluence => 1000;

	public bool IsMageGuildMember
	{
		get
		{
			Quest quest = EClass.game.quests.Get("guild_mage");
			if (quest == null)
			{
				return false;
			}
			return quest.phase >= 10;
		}
	}

	public bool IsFighterGuildMember
	{
		get
		{
			Quest quest = EClass.game.quests.Get("guild_fighter");
			if (quest == null)
			{
				return false;
			}
			return quest.phase >= 10;
		}
	}

	public bool IsThiefGuildMember
	{
		get
		{
			Quest quest = EClass.game.quests.Get("guild_thief");
			if (quest == null)
			{
				return false;
			}
			return quest.phase >= 10;
		}
	}

	public bool IsMerchantGuildMember
	{
		get
		{
			Quest quest = EClass.game.quests.Get("guild_merchant");
			if (quest == null)
			{
				return false;
			}
			return quest.phase >= 10;
		}
	}

	public bool IsCriminal
	{
		get
		{
			if (karma < 0)
			{
				return !EClass.pc.HasCondition<ConIncognito>();
			}
			return false;
		}
	}

	public void OnCreateGame()
	{
		karma = 30;
		debt = 20000000;
		chara = CharaGen.Create("chara");
		chara.SetFaction(EClass.Home);
		chara.things.SetSize(7, 5);
		chara.c_idTone = "default";
		chara.SetInt(56, 1);
		chara.things.DestroyAll();
		chara.faith = EClass.game.religions.Eyth;
		chara.RefreshFaithElement();
		uidChara = chara.uid;
		Fav fav = IO.LoadFile<Fav>(CorePath.user + "PCC/fav" + EClass.rnd(3));
		chara.pccData = IO.Duplicate(fav.data);
		chara._CreateRenderer();
		RefreshDomain();
		_agent = CharaGen.Create("chara");
		_agent.c_altName = "stash".lang();
		EClass.player.title = "master".lang();
		nums.OnCreateGame();
		hotbars.OnCreateGame();
		baseActTime = EClass.setting.defaultActPace;
		flags.OnCreateGame();
		sketches.Add(115);
		pref.sort_ascending_shop = true;
		EClass.game.config.preference.autoEat = true;
		flags.toggleHotbarHighlightDisabled = true;
		layerAbilityConfig.hideDepletedSpell = true;
		layerAbilityConfig.bgAlpha = 70;
		memo = (memo2 = "memo_blank".lang());
	}

	public void OnStartNewGame()
	{
		ModKeyItem("old_charm", 1, msg: false);
		ModKeyItem("backpack", 1, msg: false);
		EClass.player.knownBGMs.Add(1);
		EClass.player.knownBGMs.Add(3);
		EClass.player.questTracker = true;
		trackedCategories.Add("food");
		trackedCategories.Add("drink");
		trackedCategories.Add("resource");
		EClass.game.quests.Start("main");
		chara.hp = chara.MaxHP;
		chara.SetFaith(EClass.game.religions.list[0]);
		chara.elements.SetBase(135, 1);
		chara.elements.SetBase(6003, 1);
		chara.elements.SetBase(6012, 1);
		chara.elements.SetBase(6015, 1);
		chara.elements.SetBase(6050, 1);
		List<Element> list = new List<Element>();
		foreach (Element value in EClass.pc.elements.dict.Values)
		{
			list.Add(value);
		}
		foreach (Element item in list)
		{
			if (item.Value == 0)
			{
				EClass.pc.elements.Remove(item.id);
			}
			else if (item.HasTag("primary"))
			{
				item.vTempPotential = Mathf.Max(30, (item.ValueWithoutLink - 8) * 7);
			}
		}
		foreach (BodySlot slot in chara.body.slots)
		{
			chara.body.Unequip(slot);
		}
		chara.things.DestroyAll();
		CreateEquip();
		dateTravel = EClass.world.date.GetRaw();
		uidLastTravelZone = EClass.pc.currentZone.uid;
		GenerateBackgroundText();
		EClass.pc.elements.CheckSkillActions();
		EClass.pc.hunger.value = 30;
		EClass.pc.CalculateMaxStamina();
		EClass.pc.stamina.Set(EClass.pc.stamina.max / 2);
		EClass.pc.Refresh();
		isEditor = Application.isEditor;
	}

	public void OnLoad()
	{
		nums.OnLoad();
		codex.OnLoad();
		if (dataWindow != null)
		{
			Window.dictData = dataWindow;
		}
		EClass.pc.Refresh();
		if (Application.isEditor && EClass.debug.resetPlayerConfig && !isEditor)
		{
			EClass.game.config = new Game.Config();
			if (dataWindow != null)
			{
				dataWindow.Clear();
			}
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				t.c_windowSaveData = null;
			}, onlyAccessible: false);
		}
		isEditor = Application.isEditor;
	}

	public void OnBeforeSave()
	{
		foreach (Layer layer in EClass.ui.layerFloat.layers)
		{
			foreach (Window componentsInDirectChild in layer.GetComponentsInDirectChildren<Window>())
			{
				componentsInDirectChild.UpdateSaveData();
			}
		}
		dataWindow = Window.dictData;
	}

	public void SetPriorityAction(string id, Thing t)
	{
		if (t == null)
		{
			return;
		}
		if (id.IsEmpty())
		{
			foreach (List<string> p in priorityActions.Values)
			{
				p.ForeachReverse(delegate(string i)
				{
					if (i == t.id)
					{
						p.Remove(i);
					}
				});
			}
			return;
		}
		if (!priorityActions.ContainsKey(id))
		{
			priorityActions[id] = new List<string>();
		}
		if (!priorityActions[id].Contains(t.id))
		{
			priorityActions[id].Add(t.id);
		}
	}

	public bool IsPriorityAction(string id, Thing t)
	{
		if (!id.IsEmpty() && t != null)
		{
			List<string> list = priorityActions.TryGetValue(id);
			if (list != null && list.Contains(t.id))
			{
				return true;
			}
		}
		return false;
	}

	public void OnAdvanceRealHour()
	{
		realHour++;
		Msg.Say("time_warn", realHour.ToString() ?? "");
		string text = "time_warn_" + realHour;
		if (LangGame.Has(text))
		{
			Msg.Say(text);
		}
	}

	public void RefreshDomain()
	{
		domains.Clear();
		for (int i = 0; i < EClass.pc.job.domain.Length; i += 2)
		{
			domains.Add(EClass.pc.job.domain[i]);
		}
	}

	public ElementContainer GetDomains()
	{
		ElementContainer elementContainer = new ElementContainer();
		foreach (int domain in domains)
		{
			elementContainer.GetOrCreateElement(domain).vSource = 1;
		}
		return elementContainer;
	}

	public Layer SelectDomain(Action onKill = null)
	{
		List<SourceElement.Row> list2 = new List<SourceElement.Row>();
		foreach (SourceElement.Row row2 in EClass.sources.elements.rows)
		{
			if (row2.categorySub == "eleAttack" && !row2.tag.Contains("hidden") && (!row2.tag.Contains("high") || EClass.pc.job.domain.Contains(row2.id)))
			{
				list2.Add(row2);
			}
		}
		return EClass.ui.AddLayer<LayerList>().SetListCheck(list2, (SourceElement.Row a) => a.GetName(), delegate(SourceElement.Row s, ItemGeneral b)
		{
			bool flag4 = false;
			foreach (int domain in EClass.player.domains)
			{
				if (s.id == domain)
				{
					flag4 = true;
				}
			}
			if (flag4)
			{
				EClass.player.domains.Remove(s.id);
			}
			else
			{
				EClass.player.domains.Add(s.id);
			}
		}, delegate(List<UIList.ButtonPair> list)
		{
			bool flag = EClass.player.domains.Count >= 3 + EClass.pc.Evalue(1402);
			foreach (UIList.ButtonPair item in list)
			{
				UIButton button = (item.component as ItemGeneral).button1;
				SourceElement.Row row = item.obj as SourceElement.Row;
				bool flag2 = false;
				bool flag3 = false;
				foreach (int domain2 in EClass.player.domains)
				{
					if (row.id == domain2)
					{
						flag3 = true;
					}
				}
				button.SetCheck(flag3);
				for (int i = 0; i < ((EClass.pc.job.id == "swordsage") ? 5 : 3); i++)
				{
					if (EClass.pc.job.domain[i * 2] == row.id)
					{
						flag2 = true;
					}
				}
				button.interactable = !flag2 && (!flag || flag3);
				button.GetComponent<CanvasGroup>().enabled = !button.interactable;
			}
		}).SetOnKill(delegate
		{
			onKill?.Invoke();
		});
	}

	public void RefreshEmptyAlly()
	{
		int num = MaxAlly - EClass.pc.party.members.Count + 1;
		if (num == lastEmptyAlly)
		{
			return;
		}
		lastEmptyAlly = num;
		foreach (Chara member in EClass.pc.party.members)
		{
			member.RefreshSpeed();
		}
	}

	public void GenerateBackgroundText()
	{
		string text = IO.LoadText(new DirectoryInfo(CorePath.CorePackage.Background).GetFiles("*.txt").RandomItem().FullName);
		IO.SaveText(GameIO.pathCurrentSave + "background.txt", text);
	}

	public string GetBackgroundText()
	{
		StringBuilder stringBuilder = new StringBuilder(IO.LoadText(GameIO.pathCurrentSave + "background.txt"));
		stringBuilder.Replace("#name", EClass.pc.NameSimple);
		stringBuilder.Replace("#aka", EClass.pc.Aka);
		stringBuilder.Replace("#father", EClass.pc.bio.nameDad);
		stringBuilder.Replace("#mother", EClass.pc.bio.nameMom);
		stringBuilder.Replace("#birthloc2", EClass.pc.bio.nameHome);
		stringBuilder.Replace("#birthloc", EClass.pc.bio.nameLoc);
		stringBuilder.Replace("#job", EClass.pc.job.GetName().AddArticle());
		stringBuilder.Replace("#year", EClass.pc.bio.birthYear.ToString() ?? "");
		stringBuilder.Replace("#month", EClass.pc.bio.birthMonth.ToString() ?? "");
		stringBuilder.Replace("#day", EClass.pc.bio.birthDay.ToString() ?? "");
		stringBuilder.Replace("#he", (EClass.pc.IsMale ? "he" : "she").lang());
		stringBuilder.Replace("#his", (EClass.pc.IsMale ? "his" : "her").lang());
		stringBuilder.Replace("#him", (EClass.pc.IsMale ? "him" : "her").lang());
		string text = "_period".lang();
		string[] array = stringBuilder.ToString().Split(text);
		string text2 = "";
		if (array.Length != 0)
		{
			string[] array2 = array;
			foreach (string text3 in array2)
			{
				text2 += text3.ToTitleCase();
				if (text3 != text && text3 != Environment.NewLine && text3.Length > 2 && text3 != array[^1])
				{
					text2 += text;
				}
			}
		}
		return text2.TrimEnd(Environment.NewLine.ToCharArray());
	}

	public void EditBackgroundText()
	{
		Util.Run(GameIO.pathCurrentSave + "background.txt");
	}

	public void CreateEquip()
	{
		Chara chara = EClass.pc;
		chara.body.AddBodyPart(45);
		if (EClass.debug.enable)
		{
			EClass.pc.EQ_ID("lantern_old");
		}
		chara.body.AddBodyPart(44);
		chara.EQ_ID("toolbelt").c_IDTState = 0;
		chara.EQ_ID("shirt").c_IDTState = 0;
		chara.AddCard(ThingGen.CreateCurrency(1 + EClass.rnd(5)));
		switch (EClass.pc.job.id)
		{
		case "paladin":
			chara.AddCard(SetSpellbook(ThingGen.CreateSpellbook(8400), 2));
			break;
		case "inquisitor":
			chara.AddCard(SetSpellbook(ThingGen.CreateSpellbook(8706), 4));
			break;
		case "witch":
			chara.AddCard(SetSpellbook(ThingGen.CreateSpellbook(8790), 4));
			break;
		case "swordsage":
			chara.AddCard(SetSpellbook(ThingGen.CreateSpellbook(50300), 4));
			chara.AddCard(SetSpellbook(ThingGen.CreateSpellbook(50301), 4));
			chara.AddCard(SetSpellbook(ThingGen.CreateSpellbook(50802), 2));
			chara.AddCard(ThingGen.Create("tool_talisman"));
			break;
		}
		switch (EClass.pc.job.id)
		{
		case "wizard":
		case "warmage":
		case "priest":
		case "witch":
		{
			int num = 0;
			foreach (int domain in EClass.player.domains)
			{
				Element element = Element.Create(domain);
				string text = "";
				string[] tag = element.source.tag;
				foreach (string text2 in tag)
				{
					if (text != "")
					{
						break;
					}
					switch (text2)
					{
					case "hand":
					case "arrow":
					case "bolt":
						text = text2 + "_";
						break;
					}
				}
				if (text != "")
				{
					chara.AddCard(SetSpellbook(ThingGen.CreateSpellbook(text + element.source.alias.Replace("ele", "")), 4));
					num++;
					if (num >= 2)
					{
						break;
					}
				}
			}
			if (EClass.pc.job.id == "priest")
			{
				chara.AddCard(SetSpellbook(ThingGen.CreateSpellbook(8400), 2));
			}
			else
			{
				chara.AddCard(SetSpellbook(ThingGen.CreateSpellbook(8200), 2));
			}
			break;
		}
		default:
			chara.AddCard(ThingGen.Create("bandage").SetNum(6 + EClass.rnd(3)));
			break;
		}
		static Thing SetSpellbook(Thing t, int charge)
		{
			t.AddEditorTag(EditorTag.NoReadFail);
			t.c_charges = charge;
			t.SetBlessedState(BlessedState.Normal);
			return t;
		}
	}

	public void DreamSpell()
	{
		int num = EClass.pc.Evalue(1653);
		if (num == 0 && EClass.pc.Evalue(1402) == 0 && EClass.rnd(4) != 0)
		{
			return;
		}
		List<SourceElement.Row> list = new List<SourceElement.Row>();
		foreach (int domain in EClass.player.domains)
		{
			Element element = Element.Create(domain);
			string text = "";
			string[] tag = element.source.tag;
			foreach (string text2 in tag)
			{
				if (text != "")
				{
					break;
				}
				bool flag = false;
				switch (text2)
				{
				case "hand":
				case "arrow":
				case "bolt":
					flag = true;
					break;
				}
				if (num >= 3)
				{
					switch (text2)
					{
					case "ball":
					case "weapon":
					case "funnel":
					case "miasma":
						flag = true;
						break;
					}
				}
				if (flag)
				{
					list.Add(EClass.sources.elements.alias[text2 + "_" + element.source.alias.Replace("ele", "")]);
				}
			}
		}
		if (list.Count != 0)
		{
			SourceElement.Row row = list.RandomItemWeighted((SourceElement.Row a) => a.chance);
			Element element2 = EClass.pc.elements.GetElement(row.id);
			int mtp = ((num == 0) ? 100 : (75 + num * 25));
			if (num > 0 || element2 == null || element2.vPotential == 0)
			{
				Msg.Say("dream_spell", EClass.sources.elements.alias[row.aliasRef].GetText("altname").Split(',')[0].ToLower());
				EClass.pc.GainAbility(row.id, mtp);
			}
		}
	}

	public void SimulateFaction()
	{
		simulatingZone = true;
		Zone currentZone = EClass.pc.currentZone;
		Point point = EClass.pc.pos.Copy();
		foreach (FactionBranch child in EClass.pc.faction.GetChildren())
		{
			if (child.owner != currentZone)
			{
				EClass.pc.MoveZone(child.owner);
				zone = child.owner;
				EClass.scene.Init(Scene.Mode.Zone);
			}
		}
		EClass.pc.MoveZone(currentZone, new ZoneTransition
		{
			state = ZoneTransition.EnterState.Exact,
			x = point.x,
			z = point.z
		});
		zone = currentZone;
		EClass.scene.Init(Scene.Mode.Zone);
		simulatingZone = false;
	}

	public void ExitBorder(ActPlan p = null)
	{
		bool flag = EClass.game.quests.HasFarAwayEscort();
		string textDialog = (flag ? "ExitZoneEscort" : "ExitZone").lang(EClass._zone.Name);
		bool flag2 = EClass.pc.pos.x == EClass._map.bounds.x || EClass.pc.pos.x == EClass._map.bounds.x + EClass._map.bounds.Width - 1 || EClass.pc.pos.z == EClass._map.bounds.z || EClass.pc.pos.z == EClass._map.bounds.z + EClass._map.bounds.Height - 1;
		if (flag || EClass.core.config.game.confirmMapExit || (EClass._zone.instance != null && EClass._zone.instance.WarnExit))
		{
			if (p == null)
			{
				Dialog.YesNo(textDialog, delegate
				{
					EClass.game.quests.HasFarAwayEscort(execute: true);
					EClass.pc.MoveZone(EClass._zone.ParentZone);
				}, delegate
				{
				});
				return;
			}
			p.TrySetAct("actNewZone", delegate
			{
				Dialog.YesNo(textDialog, delegate
				{
					EClass.game.quests.HasFarAwayEscort(execute: true);
					EClass.pc.MoveZone(EClass._zone.ParentZone);
				});
				return false;
			}, CursorSystem.MoveZone, (!flag2) ? 1 : 999);
		}
		else if (p == null)
		{
			EClass.pc.MoveZone(EClass._zone.ParentZone);
		}
		else
		{
			p.TrySetAct("actNewZone", delegate
			{
				EClass.pc.MoveZone(EClass._zone.ParentZone);
				return false;
			}, CursorSystem.MoveZone, (!flag2) ? 1 : 999);
		}
	}

	public bool CanExitBorder(Point p)
	{
		if (EClass._zone == EClass.game.StartZone && EClass.pc.homeBranch == null)
		{
			return false;
		}
		if (EClass._zone.BlockBorderExit)
		{
			return false;
		}
		if (EClass.pc.held != null && EClass.pc.held.trait.CanOnlyCarry)
		{
			return false;
		}
		return true;
	}

	public void OnExitBorder(Point p)
	{
		if (EClass._zone.ParentZone.IsRegion)
		{
			int num = 0;
			int num2 = 0;
			MapBounds bounds = EClass._map.bounds;
			if (p.x < bounds.x)
			{
				num = -1;
			}
			else if (p.x <= bounds.maxX)
			{
				num2 = ((p.z >= bounds.z) ? 1 : (-1));
			}
			else
			{
				num = 1;
			}
			ZoneTransition.EnterState state = num switch
			{
				-1 => ZoneTransition.EnterState.Right, 
				1 => ZoneTransition.EnterState.Left, 
				_ => (num2 == 1) ? ZoneTransition.EnterState.Bottom : ZoneTransition.EnterState.Top, 
			};
			float ratePos = ((num == 1 || num == -1) ? ((float)(p.z - bounds.z) / (float)bounds.Height) : ((float)(p.x - bounds.x) / (float)bounds.Width));
			ZoneTransition trans = new ZoneTransition
			{
				state = state,
				ratePos = ratePos
			};
			Point p2 = new Point(EClass._zone.x + num, EClass._zone.y + num2);
			EnterLocalZone(p2, trans);
		}
		else
		{
			EClass.pc.MoveZone(EClass._zone.ParentZone);
		}
	}

	public void EnterLocalZone(bool encounter = false, Chara mob = null)
	{
		EnterLocalZone(EClass.pc.pos.Copy(), null, encounter, mob);
	}

	public void EnterLocalZone(Point p, ZoneTransition trans = null, bool encounter = false, Chara mob = null)
	{
		if (EClass._zone.IsRegion)
		{
			p.Set(p.x + EClass.scene.elomap.minX, p.z + EClass.scene.elomap.minY);
		}
		Zone zone = EClass._zone.Region.GetZoneAt(p.x, p.z);
		int enemies = 1;
		int roadDist = EClass.scene.elomap.GetRoadDist(p.x, p.z);
		int num = ((EClass.pc.homeBranch != null) ? EClass.pc.pos.Distance(EClass.game.StartZone.mapX, EClass.game.StartZone.mapY) : 0);
		if (zone == null)
		{
			if (!EClass._zone.Region.CanCreateZone(p))
			{
				Msg.Say("zoneBlocked");
				return;
			}
			int num2 = Mathf.Clamp(roadDist - 4, 0, 200) + Mathf.Clamp(num / 4, 0, 10);
			if (roadDist > 20)
			{
				num2 += roadDist - 20;
			}
			if (num2 >= 100)
			{
				num2 = 100;
			}
			Debug.Log("encounter roadiDist:" + roadDist + " homeDist:" + num + " lv:" + num2);
			zone = EClass._zone.Region.CreateZone(p);
			zone._dangerLv = num2;
			enemies = 2 + Mathf.Min(num2 / 10, 4) + EClass.rnd(3 + Mathf.Min(num2 / 10, 4));
			if (num < 5)
			{
				enemies = 2;
			}
		}
		else
		{
			if (zone is Zone_Void && EClass.player.CountKeyItem("license_void") == 0)
			{
				Msg.Say("voidClosed");
				return;
			}
			if (zone.IsClosed)
			{
				Msg.Say("zoneClosed");
				return;
			}
			if (zone is Zone_Casino)
			{
				Msg.Say("zoneNoEntrance");
				return;
			}
		}
		if (trans == null)
		{
			ZoneTransition.EnterState state = (encounter ? ZoneTransition.EnterState.Encounter : ((zone.RegionEnterState == ZoneTransition.EnterState.Dir) ? ZoneTransition.DirToState(EClass.pc.GetCurrentDir()) : zone.RegionEnterState));
			trans = new ZoneTransition
			{
				state = state
			};
		}
		EClass.pc.MoveZone(zone, trans);
		if (encounter)
		{
			zone.events.AddPreEnter(new ZonePreEnterEncounter
			{
				enemies = enemies,
				roadDist = roadDist,
				mob = mob
			});
		}
	}

	public void MoveZone(Zone z)
	{
		CursorSystem.ignoreCount = 15;
		EClass.ui.hud.transRight.SetActive(enable: false);
		zone = z;
		if (zone.IsRegion)
		{
			dateTravel = EClass.world.date.GetRaw();
		}
		EClass.scene.Init(Scene.Mode.Zone);
		if ((bool)WidgetRoster.Instance)
		{
			WidgetRoster.Instance.OnMoveZone();
		}
		if ((bool)UIResourceTrack.Instance)
		{
			UIResourceTrack.Instance.OnMoveZone();
		}
		if ((bool)WidgetMinimap.Instance)
		{
			WidgetMinimap.Instance.OnMoveZone();
		}
		if (EClass.pc.currentZone == zone && !zone.IsRegion && LastTravelZone != zone)
		{
			if (LastTravelZone != null)
			{
				int elapsedHour = EClass.world.date.GetElapsedHour(dateTravel);
				int num = elapsedHour / 24;
				if (distanceTravel > 2)
				{
					elapsedHour %= 24;
					Msg.Say("travel", LastTravelZone.Name, Date.GetText(dateTravel, Date.TextFormat.Travel), num.ToString() ?? "", elapsedHour.ToString() ?? "");
					Msg.Say("travel2", distanceTravel.ToString() ?? "", ((EClass.pc.party.members.Count == 1) ? "you" : "youAndCompanion").lang());
					foreach (Chara member in EClass.pc.party.members)
					{
						member.AddExp(distanceTravel / 3);
						member.elements.ModExp(240, 30 + distanceTravel * 5);
					}
				}
			}
			distanceTravel = 0;
			dateTravel = EClass.world.date.GetRaw();
			uidLastTravelZone = zone.uid;
		}
		regionMoveWarned = false;
	}

	public void AddInventory(Card c)
	{
		EClass.pc.AddCard(c);
	}

	public void EndTurn(bool consume = true)
	{
		if (!EClass.pc.isDead)
		{
			if (consume)
			{
				EInput.Consume();
			}
			EClass.pc.SetAI(new GoalEndTurn());
			EClass.player.baseActTime = EClass.setting.defaultActPace;
		}
	}

	public void ModFame(int a)
	{
		if (a == 0)
		{
			return;
		}
		if (a >= 0 && fame + a >= 5000 && EClass.player.CountKeyItem("license_adv") == 0)
		{
			a = 5000 - fame;
			if (a <= 0)
			{
				a = 0;
				Msg.Say("gainFameLimit");
				return;
			}
		}
		fame += a;
		if (fame < 0)
		{
			fame = 0;
		}
		if (a > 0)
		{
			Msg.Say("gainFame", a.ToString() ?? "");
		}
		else
		{
			Msg.Say("looseFame", (-a).ToString() ?? "");
		}
		if (a > 0)
		{
			Tutorial.Reserve("fame");
		}
	}

	public void ModKeyItem(string alias, int num = 1, bool msg = true)
	{
		ModKeyItem(EClass.sources.keyItems.alias[alias].id, num, msg);
	}

	public void ModKeyItem(int id, int num = 1, bool msg = true)
	{
		if (!keyItems.ContainsKey(id))
		{
			keyItems.Add(id, 0);
		}
		keyItems[id] += num;
		if (msg)
		{
			if (num > 0)
			{
				SE.Play("keyitem");
				Msg.Say("get_keyItem", EClass.sources.keyItems.map[id].GetName());
			}
			else
			{
				SE.Play("keyitem_lose");
				Msg.Say("lose_keyItem", EClass.sources.keyItems.map[id].GetName());
			}
		}
	}

	public bool HasKeyItem(string alias)
	{
		return CountKeyItem(EClass.sources.keyItems.alias[alias].id) > 0;
	}

	public int CountKeyItem(string alias)
	{
		return CountKeyItem(EClass.sources.keyItems.alias[alias].id);
	}

	public int CountKeyItem(int id)
	{
		if (!keyItems.ContainsKey(id))
		{
			return 0;
		}
		return keyItems[id];
	}

	public void EquipTool(Thing a, bool setHotItem = true)
	{
		if (a.GetRootCard() != EClass.pc)
		{
			if (a.parent is Thing)
			{
				Msg.Say("movedToEquip", a, a.parent as Thing);
			}
			a = EClass.pc.AddThing(a);
		}
		if (setHotItem)
		{
			EClass.player.SetCurrentHotItem(a.trait.GetHotItem());
			SE.SelectHotitem();
		}
	}

	public void RefreshCurrentHotItem()
	{
		WidgetCurrentTool instance = WidgetCurrentTool.Instance;
		if (!instance)
		{
			return;
		}
		if (currentHotItem != null)
		{
			if ((bool)instance)
			{
				instance.buttonHotItem.Refresh();
			}
			if (currentHotItem is HotItemHeld && (currentHotItem.Thing != EClass.pc.held || currentHotItem.Thing.GetRootCard() != EClass.pc))
			{
				currentHotItem = null;
			}
			else if (currentHotItem is HotItemThing && (currentHotItem.Thing == null || currentHotItem.Thing.GetRootCard() != EClass.pc))
			{
				currentHotItem = null;
			}
		}
		if (EClass.pc.held != null)
		{
			currentHotItem = new HotItemHeld(EClass.pc.held as Thing);
		}
		if (currentHotItem == null)
		{
			if ((bool)instance && instance.selected != -1 && instance.selectedButton.card != null && instance.selectedButton.card.GetRootCard() == EClass.pc && !instance.selectedButton.card.GetRootCard().isDestroyed)
			{
				currentHotItem = instance.selectedButton.card.trait.GetHotItem();
			}
			else
			{
				currentHotItem = hotItemNoItem;
			}
		}
		if (currentHotItem != lastHotItem)
		{
			if (lastHotItem != null)
			{
				lastHotItem.OnUnsetCurrentItem();
			}
			currentHotItem.OnSetCurrentItem();
			lastHotItem = currentHotItem;
		}
		WidgetCurrentTool.RefreshCurrentHotItem();
		WidgetHotbar.dirtyCurrentItem = false;
		MarkMapHighlights();
		EClass.core.actionsNextFrame.Add(delegate
		{
			EClass.core.actionsNextFrame.Add(delegate
			{
				foreach (LayerInventory item in LayerInventory.listInv)
				{
					item.invs[0].RefreshHighlight();
				}
			});
		});
		EClass.pc.RecalculateFOV();
	}

	public void ResetCurrentHotItem()
	{
		EClass.pc.PickHeld();
		EClass.player.SetCurrentHotItem(null);
		SE.SelectHotitem();
	}

	public void SetCurrentHotItem(HotItem item)
	{
		EClass.pc.PickHeld();
		if (currentHotItem != item)
		{
			currentHotItem = item;
			if (currentHotItem is HotItemHeld hotItemHeld)
			{
				EClass.pc.HoldCard(hotItemHeld.Thing);
			}
		}
		RefreshCurrentHotItem();
	}

	public void TryEquipBait()
	{
		if (eqBait != null && eqBait.GetRootCard() != EClass.pc)
		{
			eqBait = null;
		}
		if (eqBait == null)
		{
			EClass.pc.things.Find<TraitBait>()?.trait.OnUse(EClass.pc);
		}
	}

	public bool HasValidRangedTarget()
	{
		if (target == null || !target.IsAliveInCurrentZone || !target.isSynced || !EClass.pc.CanSee(target))
		{
			return false;
		}
		return true;
	}

	public bool TargetRanged()
	{
		_ = EClass.player.currentHotItem.Thing?.trait;
		int num = 999999999;
		Chara chara = null;
		Point pos = EClass.scene.mouseTarget.pos;
		List<Chara> list = new List<Chara>();
		bool flag = false;
		if (EInput.isShiftDown && pos.IsValid && pos.HasChara)
		{
			foreach (Chara chara2 in EClass._map.charas)
			{
				if (chara2.isSynced && chara2.IsAliveInCurrentZone && !chara2.IsPC && chara2.pos.Equals(pos) && EClass.pc.CanSeeLos(chara2.pos))
				{
					list.Add(chara2);
				}
			}
			if (list.Count > 0)
			{
				chara = list.NextItem(EClass.player.target);
				flag = true;
			}
			list.Clear();
		}
		if (!flag)
		{
			foreach (Chara chara3 in EClass._map.charas)
			{
				int num2 = 0;
				if (chara3.isSynced && chara3.IsAliveInCurrentZone && !chara3.IsPC && EClass.pc.CanSeeLos(chara3.pos) && EClass.pc.CanSee(chara3) && !chara3.IsFriendOrAbove())
				{
					if (chara3.IsNeutralOrAbove())
					{
						num2 += 10000;
					}
					num2 += EClass.pc.Dist(chara3);
					if (num2 < num)
					{
						chara = chara3;
						num = num2;
					}
					list.Add(chara3);
				}
			}
			if (EInput.isShiftDown)
			{
				chara = list.NextItem(EClass.player.target);
			}
		}
		if (chara == null)
		{
			if (target != null)
			{
				Msg.Say("noTargetFound");
			}
			target = null;
			return false;
		}
		if (target != chara)
		{
			Msg.Say("targetSet", chara);
		}
		target = chara;
		return true;
	}

	public void OnAdvanceHour()
	{
		EClass.pc.faith.OnChangeHour();
		if (EClass.pc.Evalue(289) > 0)
		{
			foreach (Thing item in EClass.pc.things.List((Thing t) => t.c_IDTState == 5, onlyAccessible: true))
			{
				EClass.pc.TryIdentify(item);
			}
		}
		if (EClass.pc.IsDeadOrSleeping || EClass.rnd(2) != 0 || EClass._zone.IsRegion)
		{
			return;
		}
		foreach (Chara member in EClass.pc.party.members)
		{
			member.AddExp(1);
		}
	}

	public void OnAdvanceDay()
	{
		nums.OnAdvanceDay();
		EClass.pc.c_daysWithGod++;
		if (EClass.pc.Evalue(85) > 0)
		{
			EClass.pc.ModExp(85, 10);
		}
		EClass.pc.RefreshFaithElement();
		prayed = false;
		if (karma < 0 && EClass.rnd(4) == 0)
		{
			ModKarma(1);
		}
	}

	public bool TryAbortAutoCombat(bool immediate = true)
	{
		if (!(EClass.pc.ai is GoalAutoCombat))
		{
			return false;
		}
		EClass.pc.ai.Cancel();
		if (immediate)
		{
			EClass.pc.SetNoGoal();
		}
		return true;
	}

	public void ClearMapHighlights()
	{
		foreach (Cell lastMarkedHighlight in lastMarkedHighlights)
		{
			lastMarkedHighlight.highlight = 0;
		}
		lastMarkedHighlights.Clear();
	}

	public void MarkMapHighlights()
	{
		ClearMapHighlights();
		currentHotItem.OnMarkMapHighlights();
	}

	public bool CanAcceptInput()
	{
		if (EClass.pc.HasNoGoal)
		{
			return !EClass.pc.WillConsumeTurn();
		}
		return false;
	}

	public bool CanSee(Chara c)
	{
		if (EClass.pc.hasTelepathy && c.race.visibleWithTelepathy)
		{
			return true;
		}
		if (!c.IsPC && (EClass.pc.fov == null || EClass.pc.isBlind))
		{
			return false;
		}
		if (c.IsPCParty || c.isDead)
		{
			return true;
		}
		if (c.IsMultisize)
		{
			bool canSee = false;
			int dist = EClass.pc.Dist(c);
			c.ForeachPoint(delegate(Point p, bool main)
			{
				if (!canSee && (p.cell.light > 0 || dist < 2) && p.cell.pcSync)
				{
					canSee = true;
				}
			});
			return canSee;
		}
		if (c.pos.cell.light > 0 || EClass.pc.Dist(c) < 2)
		{
			return c.pos.cell.pcSync;
		}
		return false;
	}

	public void AddExpKnowledge(int a)
	{
		expKnowledge += a;
		if (expKnowledge >= MaxExpKnowledge)
		{
			for (int i = 0; i < expKnowledge / MaxExpKnowledge; i++)
			{
				LvUp();
			}
			expKnowledge %= MaxExpKnowledge;
		}
		static void LvUp()
		{
			Msg.Say("DingKnowledge");
		}
	}

	public void AddExpInfluence(int a)
	{
		expInfluence += a;
		if (expInfluence >= MaxExpInfluence)
		{
			for (int i = 0; i < expInfluence / MaxExpInfluence; i++)
			{
				LvUp();
			}
			expInfluence %= MaxExpInfluence;
		}
		static void LvUp()
		{
			Msg.Say("DingInfluence");
		}
	}

	public void ModKarma(int a)
	{
		if (a != 0)
		{
			if (a < 0)
			{
				Tutorial.Reserve("karma");
			}
			bool flag = karma < 0;
			karma += a;
			Msg.Say((a > 0) ? "karmaUp" : "karmaDown", a.ToString() ?? "");
			if (karma < 0 && !flag)
			{
				Msg.Say("becomeCriminal");
				EClass.pc.pos.TryWitnessCrime(EClass.pc);
				EClass._zone.RefreshCriminal();
				Tutorial.Reserve("criminal");
			}
			if (karma >= 0 && flag)
			{
				Msg.Say("becomeNonCriminal");
				EClass._zone.RefreshCriminal();
			}
			EClass.game.quests.list.ForeachReverse(delegate(Quest q)
			{
				q.OnModKarma(a);
			});
			karma = Mathf.Clamp(karma, -100, 100);
		}
	}

	public Thing DropReward(Thing t, bool silent = false)
	{
		t.things.DestroyAll();
		EClass._zone.AddCard(t, EClass.pc.pos);
		if (!silent)
		{
			Msg.Say("dropReward");
		}
		return t;
	}

	public bool TooHeavyToMove()
	{
		if (!EClass.debug.ignoreWeight && EClass.pc.burden.GetPhase() == 4)
		{
			Msg.Say("tooHeavyToMove");
			EClass.pc.renderer.NextFrame();
			EInput.Consume(consumeAxis: true);
			return true;
		}
		return false;
	}

	public void ToggleFavMoongate(string id)
	{
		if (favMoongate.Contains(id))
		{
			favMoongate.Remove(id);
		}
		else
		{
			favMoongate.Add(id);
		}
	}
}
