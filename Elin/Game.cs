using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using UnityEngine;

public class Game : EClass
{
	public class Config : EClass
	{
		[JsonProperty]
		public bool showRoof;

		[JsonProperty]
		public bool showWall;

		[JsonProperty]
		public bool tilt;

		[JsonProperty]
		public bool tiltRegion;

		[JsonProperty]
		public bool slope;

		[JsonProperty]
		public bool noRoof;

		[JsonProperty]
		public bool freePos;

		[JsonProperty]
		public bool snapFreePos;

		[JsonProperty]
		public bool buildLight;

		[JsonProperty]
		public bool highlightArea;

		[JsonProperty]
		public bool autoWall;

		[JsonProperty]
		public bool showGuideGrid;

		[JsonProperty]
		public bool reverseSnow;

		[JsonProperty]
		public bool autoCollectCard;

		[JsonProperty]
		public bool cheat;

		[JsonProperty]
		public bool showAllFeat;

		[JsonProperty]
		public int slopeMod = 50;

		[JsonProperty]
		public int defaultZoom = 100;

		[JsonProperty]
		public int zoomedZoom = 125;

		[JsonProperty]
		public int regionZoom = 100;

		[JsonProperty]
		public int tiltPower = 80;

		[JsonProperty]
		public int tiltPowerRegion = 50;

		[JsonProperty]
		public int gridIconSize = 100;

		[JsonProperty]
		public int backDrawAlpha = 10;

		[JsonProperty]
		public int animeSpeed = 100;

		[JsonProperty]
		public int lowWallObjAltitude = 2;

		[JsonProperty]
		public ConfigTactics tactics = new ConfigTactics();

		[JsonProperty]
		public ConfigAutoCombat autoCombat = new ConfigAutoCombat();

		[JsonProperty]
		public ConfigPreference preference = new ConfigPreference();

		public bool FreePos
		{
			get
			{
				if (!Input.GetKey(KeyCode.LeftShift))
				{
					return Input.GetKey(KeyCode.RightShift);
				}
				return true;
			}
		}
	}

	public class Flags : EClass
	{
		[JsonProperty]
		public int _b1;

		public BitArray32 b1;

		public bool IsWelcomeMessageShown
		{
			get
			{
				return b1[0];
			}
			set
			{
				b1[0] = value;
			}
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			_b1 = (int)b1.Bits;
		}

		[OnDeserialized]
		private void _OnDeserialized(StreamingContext context)
		{
			b1.Bits = (uint)_b1;
		}
	}

	public static Game Instance;

	public static float waitTimer;

	public static string id;

	public static bool isPaused;

	[JsonProperty(Order = -90)]
	public SpatialManager spatials = new SpatialManager();

	[JsonProperty(Order = -80)]
	public CardManager cards = new CardManager();

	[JsonProperty(Order = -100)]
	public ReligionManager religions = new ReligionManager();

	[JsonProperty(Order = -60)]
	public FactionManager factions = new FactionManager();

	[JsonProperty(Order = -50)]
	public DomainManager domains = new DomainManager();

	[JsonProperty]
	public Version version;

	[JsonProperty]
	public PartyManager parties = new PartyManager();

	[JsonProperty]
	public new World world;

	[JsonProperty]
	public new Player player = new Player();

	[JsonProperty]
	public QuestManager quests = new QuestManager();

	[JsonProperty]
	public RelationManager relations = new RelationManager();

	[JsonProperty]
	public MsgLog log = new MsgLog
	{
		id = "log"
	};

	[JsonProperty]
	public PCCUniformManager uniforms = new PCCUniformManager();

	[JsonProperty]
	public TeleportManager teleports = new TeleportManager();

	[JsonProperty]
	public int seed;

	[JsonProperty]
	public int idPrologue;

	[JsonProperty]
	public int idDifficulty;

	[JsonProperty]
	public int dateScratch;

	[JsonProperty]
	public double backupTime;

	[JsonProperty]
	public Flags flags = new Flags();

	[JsonProperty]
	public Config config = new Config();

	[JsonProperty]
	public List<Thing> lostThings = new List<Thing>();

	[JsonProperty]
	public GamePrincipal principal = new GamePrincipal();

	[JsonProperty]
	public SurvivalManager survival;

	public int gameSpeedIndex = 1;

	public int lastGameSpeedIndex = 1;

	public int sessionMin;

	public int countLoadedMaps;

	public int saveCount;

	public float timeSinceStart;

	public bool isLoading;

	public bool isKilling;

	public bool isCloud;

	public GameBlueprint bp;

	public GameUpdater updater = new GameUpdater();

	public Zone activeZone;

	public Zone lastActiveZone;

	public Dictionary<string, IGlobalValue> referenceMap = new Dictionary<string, IGlobalValue>();

	public HashSet<Texture2D> loadedTextures = new HashSet<Texture2D>();

	private float welcomeTimer;

	public static bool IsWaiting => waitTimer > 0f;

	public Prologue Prologue => EClass.setting.start.prologues[idPrologue];

	public bool UseGrid => EClass.core.config.game.useGrid;

	public bool altUI => EClass.core.config.game.altUI;

	public bool altInv => EClass.core.config.game.altInv;

	public bool altCraft => EClass.core.config.game.altCraft;

	public bool altAbility => EClass.core.config.game.altAbility;

	public bool IsSurvival => Prologue.type == GameType.Survival;

	public Zone StartZone => spatials.Find(EClass.game.Prologue.idStartZone);

	public new static void Wait(float a, Point p)
	{
		_ = p?.IsSync;
	}

	public new static void Wait(float a, Card c)
	{
		if (c != null && c.isSynced && !c.IsPC)
		{
			Wait(a);
		}
	}

	public static void Wait(float a)
	{
		waitTimer = a * (float)(EClass.core.config.game.waiter * 25) / 100f;
	}

	public void OnUpdate()
	{
		backupTime += Time.deltaTime;
		waitTimer -= Time.deltaTime;
		timeSinceStart += Time.deltaTime;
		player.stats.timeElapsed += Time.deltaTime;
		if (!flags.IsWelcomeMessageShown && player.stats.timeElapsed > 2.0)
		{
			flags.IsWelcomeMessageShown = true;
		}
		updater.FixedUpdate();
	}

	public static bool TryLoad(string id, bool cloud, Action onLoad)
	{
		if (GameIO.CanLoad((cloud ? CorePath.RootSaveCloud : CorePath.RootSave) + id))
		{
			onLoad();
			return true;
		}
		EClass.ui.Say("incompatible");
		return false;
	}

	public static void Load(string id, bool cloud)
	{
		string text = (cloud ? CorePath.RootSaveCloud : CorePath.RootSave) + id;
		Debug.Log("Loading: " + id + ": " + text);
		if (EClass.game != null)
		{
			EClass.game.Kill();
		}
		OnBeforeInstantiate();
		EClass.core.game = GameIO.LoadGame(id, text, cloud);
		EClass.game.isCloud = cloud;
		EClass.game.isLoading = true;
		GameIO.ClearTemp();
		EClass.game.OnGameInstantiated();
		EClass.game.OnLoad();
		EClass.scene.Init(Scene.Mode.StartGame);
		if (EClass.game.altInv)
		{
			SoundManager.ignoreSounds = true;
			if (EClass.game.player.pref.layerInventory)
			{
				EClass.ui.OpenFloatInv();
			}
			SoundManager.ignoreSounds = true;
			if (EClass.game.player.pref.layerAbility)
			{
				EClass.ui.ToggleAbility();
			}
			SoundManager.ignoreSounds = false;
			TooltipManager.Instance.HideTooltips(immediate: true);
		}
		if ((bool)WidgetSideScreen.Instance)
		{
			WidgetSideScreen.Instance.OnChangeResolution();
		}
		EClass.game.isLoading = false;
	}

	public void OnLoad()
	{
		domains.OnLoad();
		religions.OnLoad();
		factions.OnLoad();
		world._OnLoad();
		player.OnLoad();
		if (EClass.pc.currentZone == null || EClass.pc.currentZone.destryoed)
		{
			EClass.pc.party.members.ForEach(delegate(Chara c)
			{
				c.currentZone = EClass.pc.homeZone;
				c.global.transition = new ZoneTransition
				{
					state = ZoneTransition.EnterState.Return
				};
			});
		}
		player.zone = EClass.pc.currentZone;
		EClass.pc.currentZone.lastActive = world.date.GetRaw();
		if (spatials.Find("somewhere") == null)
		{
			SpatialGen.Create("somewhere", world.region, register: true);
		}
		EClass.pc.homeZone.isKnown = true;
		EClass.debug.OnLoad();
		EClass.pc.things.RefreshGridRecursive();
		foreach (Thing item in EClass.pc.things.List((Thing t) => t.Num <= 0 || t.isDestroyed))
		{
			item.parent.RemoveCard(item);
		}
		foreach (Thing item2 in EClass.pc.things.List((Thing t) => t.trait is TraitChestMerchant))
		{
			item2.Destroy();
		}
		foreach (Thing item3 in EClass.pc.things.List((Thing t) => t.invY == 1 && t.invX == -1))
		{
			item3.invY = 0;
		}
		foreach (Chara value in EClass.game.cards.globalCharas.Values)
		{
			if (value.id == "demitas" && value.faction.id == "wilds")
			{
				EClass.pc.homeBranch.AddMemeber(value);
			}
			if (value.memberType == FactionMemberType.Livestock && !value.IsPCFaction)
			{
				value.memberType = FactionMemberType.Default;
			}
			if (!value.isDyed && value.HasTag(CTAG.random_color))
			{
				value.DyeRandom();
			}
		}
		foreach (FactionBranch child in EClass.pc.faction.GetChildren())
		{
			foreach (Chara member in child.members)
			{
				if (!member.isDead && (member.currentZone == null || member.currentZone.id == "somewhere"))
				{
					Debug.Log("exception: Moving invalid chara: " + member.Name + "/" + member.currentZone?.ToString() + "/" + member.homeBranch?.ToString() + "/" + member.faction);
					member.MoveZone(child.owner, ZoneTransition.EnterState.RandomVisit);
				}
				member.RefreshWorkElements(child.owner.elements);
				member.Refresh();
			}
		}
		EClass.pc.angle = player.angle;
		EClass.pc.elements.CheckSkillActions();
		if (cards.listAdv.Count == 0)
		{
			AddAdventurers();
		}
		quests.list.ForeachReverse(delegate(Quest q)
		{
			if (!(q is QuestRandom))
			{
				foreach (Quest item4 in quests.list)
				{
					if (q != item4 && item4.id == q.id)
					{
						quests.list.Remove(item4);
						break;
					}
				}
			}
		});
		quests.globalList.ForeachReverse(delegate(Quest q)
		{
			if (!(q is QuestRandom))
			{
				foreach (Quest global in quests.globalList)
				{
					if (q != global && global.id == q.id)
					{
						quests.globalList.Remove(global);
						break;
					}
				}
			}
		});
		TryAddQuest("into_darkness", "exile_kettle");
		if (version.IsBelow(0, 23, 185))
		{
			player.resetPrincipal = true;
		}
		if (version.IsBelow(0, 23, 182))
		{
			foreach (Chara value2 in cards.globalCharas.Values)
			{
				if (!value2.IsUnique && value2.c_lockedAge != 0)
				{
					value2.elements.SetBase(1243, 1);
				}
			}
		}
		if (version.IsBelow(0, 23, 155))
		{
			AddAdventurer("adv_yukiimo");
		}
		if (version.IsBelow(0, 23, 100))
		{
			int num = player.recipes.knownRecipes.TryGetValue("trainingDummy_heavy", 0);
			if (num != 0)
			{
				player.recipes.knownRecipes["trainingDummy_heavy1"] = num;
			}
		}
		if (version.IsBelow(0, 23, 96))
		{
			EClass.game.principal = IO.DeepCopy(EClass.setting.start.principals[0]);
		}
		if (player.resetPrincipal)
		{
			GameDifficultySetting gameDifficultySetting = EClass.setting.start.difficulties[idDifficulty];
			EClass.game.principal.id = -1;
			EClass.game.principal.permadeath = gameDifficultySetting.deleteGameOnDeath;
			EClass.game.principal.disableManualSave = !gameDifficultySetting.allowManualSave;
			EClass.core.actionsNextFrame.Add(delegate
			{
				EClass.ui.AddLayer<LayerWorldSetting>();
			});
		}
		if (version.IsBelow(0, 23, 93))
		{
			RecipeManager.BuildList();
			Debug.Log("■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■feafefaeffaeaffeaaeaefseasfaefaef");
			while (TryAddRecipe())
			{
			}
		}
		if (version.IsBelow(0, 23, 72))
		{
			foreach (Chara value3 in EClass.game.cards.globalCharas.Values)
			{
				if (!value3.isDead)
				{
					value3.c_wasInPcParty = false;
				}
			}
		}
		if (version.IsBelow(0, 23, 59))
		{
			while (TryDestroy())
			{
			}
		}
		if (version.IsBelow(0, 23, 52))
		{
			player.flags.toggleHotbarHighlightActivated = true;
		}
		if (version.IsBelow(0, 23, 51))
		{
			foreach (Chara value4 in EClass.game.cards.globalCharas.Values)
			{
				if (!(value4.id != "adv") && value4.IsPCFaction)
				{
					value4.idSkin = value4.uid % (value4.source._tiles.Length - 4) / 2 * 2 + ((!value4.IsMale) ? 1 : 0);
				}
			}
		}
		if (version.IsBelow(0, 22, 91))
		{
			TryAddQuestIfActive("demitas_spellwriter", "into_darkness");
		}
		if (version.IsBelow(0, 22, 86))
		{
			Chara chara = cards.globalCharas.Find("demitas");
			if (chara != null && chara._works != null && chara._works.Count > 0)
			{
				chara._works[0] = 51;
			}
		}
		if (version.IsBelow(0, 22, 86))
		{
			TryAddQuest("farris_tulip", "greatDebt");
		}
		if (version.IsBelow(0, 22, 86))
		{
			TryAddQuest("exile_whisper", "exile_kettle");
		}
		if (version.IsBelow(0, 22, 85))
		{
			TryAddQuest("exile_meet", "pre_debt");
		}
		if (version.IsBelow(0, 22, 60) && EClass.game.quests.Get<QuestDebt>() != null)
		{
			Chara chara2 = cards.globalCharas.Find("loytel");
			Debug.Log("Loytelfix: " + chara2.homeZone);
			if (chara2.homeZone == null || !chara2.homeZone.IsPCFaction)
			{
				EClass.pc.homeBranch.AddMemeber(chara2);
				chara2.homeZone = EClass.pc.homeBranch.owner;
				chara2.RemoveEditorTag(EditorTag.Invulnerable);
			}
		}
		if (version.IsBelow(0, 22, 52) && quests.completedIDs.Contains("vernis_gold"))
		{
			quests.Add("pre_debt", "farris");
		}
		if (quests.completedIDs.Contains("farris_tulip"))
		{
			quests.globalList.ForeachReverse(delegate(Quest q)
			{
				if (q.id == "farris_tulip")
				{
					quests.globalList.Remove(q);
				}
			});
		}
		quests.list.ForeachReverse(delegate(Quest q)
		{
			if (q is QuestDialog && quests.completedIDs.Contains(q.id))
			{
				quests.list.Remove(q);
			}
		});
		if (version.IsBelow(0, 22, 20))
		{
			foreach (Chara value5 in cards.globalCharas.Values)
			{
				value5.SetBool(18, enable: false);
			}
		}
		if (version.IsBelow(0, 22, 22))
		{
			if (EClass.pc.faithElements != null)
			{
				EClass.pc.faithElements.SetParent();
			}
			foreach (Element item5 in EClass.pc.elements.ListElements((Element e) => e.source.categorySub == "god"))
			{
				EClass.pc.SetFeat(item5.id, 0);
			}
			EClass.pc.RefreshFaithElement();
		}
		if (version.IsBelow(0, 22, 45))
		{
			player.debt = 20000000;
		}
		if (!version.Equals(EClass.core.version))
		{
			player.recipes.OnVersionUpdate();
		}
		if (cards.container_deposit == null)
		{
			cards.container_deposit = ThingGen.Create("container_deposit");
			if (player.bankMoney > 0)
			{
				cards.container_deposit.Add("money", player.bankMoney);
			}
		}
		foreach (Quest item6 in quests.list)
		{
			if (item6 is QuestSequence && !EClass.sources.quests.map.ContainsKey(item6.idSource))
			{
				item6.phase = 0;
			}
		}
		foreach (Quest global2 in quests.globalList)
		{
			if (global2 is QuestSequence && !EClass.sources.quests.map.ContainsKey(global2.idSource))
			{
				global2.phase = 0;
			}
		}
		void TryAddQuest(string idQuest, string idReqQuest)
		{
			if (quests.completedIDs.Contains(idReqQuest) && !quests.completedIDs.Contains(idQuest) && quests.GetGlobal(idQuest) == null && quests.Get(idQuest) == null)
			{
				quests.Add(idQuest);
			}
		}
		void TryAddQuestIfActive(string idQuest, string idReqQuest)
		{
			if (quests.Get(idReqQuest) != null && !quests.completedIDs.Contains(idQuest) && quests.GetGlobal(idQuest) == null && quests.Get(idQuest) == null)
			{
				quests.Add(idQuest);
			}
		}
		bool TryAddRecipe()
		{
			foreach (string key in player.recipes.knownRecipes.Keys)
			{
				if (key.Length > 1 && key[0] == 'b')
				{
					RecipeSource recipeSource = RecipeManager.Get(key + "-p");
					if (recipeSource != null && !player.recipes.knownRecipes.ContainsKey(recipeSource.id))
					{
						Debug.Log(recipeSource.id);
						player.recipes.Add(recipeSource.id, showEffect: false);
						return true;
					}
					recipeSource = RecipeManager.Get(key.Replace("-p", ""));
					if (recipeSource != null && !player.recipes.knownRecipes.ContainsKey(recipeSource.id))
					{
						Debug.Log(recipeSource.id);
						player.recipes.Add(recipeSource.id, showEffect: false);
						return true;
					}
				}
			}
			return false;
		}
		static bool TryDestroy()
		{
			for (int i = 0; i < EClass.pc.party.members.Count; i++)
			{
				Chara chara3 = EClass.pc.party.members[i];
				Debug.Log(chara3.id + "/" + chara3.uid + "/" + chara3._cints[1]);
				for (int j = 0; j < EClass.pc.party.members.Count; j++)
				{
					Chara chara4 = EClass.pc.party.members[j];
					if (i != j && chara3.uid == chara4.uid)
					{
						EClass.pc.party.uidMembers.RemoveAt(j);
						EClass.pc.party.members.RemoveAt(j);
						chara4.Destroy();
						Debug.Log("Destroyed");
						return true;
					}
				}
			}
			return false;
		}
	}

	public static void Create(string _id = null, bool cloud = false)
	{
		id = _id ?? GameIO.GetNewId(cloud ? CorePath.RootSaveCloud : CorePath.RootSave, "world_");
		OnBeforeInstantiate();
		EClass.core.game = (Instance = new Game());
		EClass.core.game.isCloud = cloud;
		GameIO.ResetTemp();
		EClass.core.game.OnGameInstantiated();
		EClass.core.game._Create();
	}

	public static void OnBeforeInstantiate()
	{
		Player.seedHallucination = 0;
	}

	public void OnGameInstantiated()
	{
		HotItemHeld.taskBuild = null;
		InvOwner.Trader = (InvOwner.Main = null);
		LayerDrama.currentQuest = null;
		BookList.Init();
		ContentGallery.lastPage = 0;
		ContentGallery.listMode = false;
		if (!Application.isEditor || !EClass.debug.enable)
		{
			RecipeManager.BuildList();
		}
		foreach (Chara value in cards.globalCharas.Values)
		{
			if (value.uid == player.uidChara)
			{
				player.chara = value;
				break;
			}
		}
		ActionMode.OnGameInstantiated();
		EClass.ui.widgets.OnGameInstantiated();
		EClass.Sound.ResetPlaylist();
		EClass.core.config.ApplyFPS(force: true);
	}

	public void _Create()
	{
		principal = IO.DeepCopy(EClass.setting.start.principals[0]);
		idDifficulty = 1;
		config.snapFreePos = (config.slope = (config.autoWall = true));
		config.autoCombat.abortOnAllyDead = true;
		config.autoCombat.abortOnHalfHP = true;
		config.autoCombat.bUseHotBar = true;
		config.autoCombat.bUseInventory = true;
		config.autoCombat.bDontChangeTarget = true;
		config.autoCombat.abortOnKill = true;
		config.autoCombat.abortOnItemLoss = true;
		seed = EClass.rnd(10000);
		Debug.Log("creating game: " + id + " seed:" + seed);
		uniforms.Import();
		religions.OnCreateGame();
		factions.OnCreateGame();
		world = SpatialGen.CreateRecursive("world") as World;
		domains.OnCreateGame();
		foreach (Spatial value in spatials.map.Values)
		{
			value.SetMainFaction(factions.Find(value.source.faction.IsEmpty("wilds")));
		}
		player.OnCreateGame();
		cards.container_shipping = ThingGen.Create("container_shipping");
		cards.container_deliver = ThingGen.Create("container_delivery");
		cards.container_deposit = ThingGen.Create("container_deposit");
		bp = new GameBlueprint();
		bp.Create();
	}

	public void StartNewGame()
	{
		EClass.pc.homeZone = StartZone;
		EClass.pc.SetGlobal();
		parties.Create(EClass.pc);
		Prologue prologue = EClass.game.Prologue;
		if (prologue.type == GameType.Survival)
		{
			survival = new SurvivalManager();
		}
		else
		{
			CharaGen.Create("fiama").SetGlobal(EClass.game.StartZone, prologue.posFiama.x, prologue.posFiama.y);
			CharaGen.Create("ashland").SetGlobal(EClass.game.StartZone, prologue.posAsh.x, prologue.posAsh.y);
		}
		if ((bool)LayerTitle.actor)
		{
			world.date.hour = EClass.game.Prologue.hour;
			EClass.core.actionsNextFrame.Add(LayerTitle.KillActor);
			Player obj = player;
			Chara chara = EClass.pc;
			Zone zone2 = (EClass.pc.homeZone = EClass.game.spatials.Find(EClass.game.Prologue.idStartZone));
			Zone zone4 = (chara.currentZone = zone2);
			obj.zone = zone4;
			EClass.pc.global.transition = new ZoneTransition
			{
				state = ZoneTransition.EnterState.Exact,
				x = EClass.game.Prologue.startX,
				z = EClass.game.Prologue.startZ
			};
			EClass.pc.hp = EClass.pc.MaxHP;
			EClass.pc.mana.Set(EClass.pc.mana.max / 2);
			EClass.pc.stamina.Set(EClass.pc.stamina.max / 2);
			EClass.pc.AddCondition<ConFaint>(200, force: true);
		}
		else
		{
			EClass.pc.homeZone = EClass.game.spatials.Find(EClass.game.Prologue.idStartZone);
			Zone zone5 = null;
			ZoneTransition transition = new ZoneTransition
			{
				state = ZoneTransition.EnterState.Auto
			};
			Chara c = EClass.game.cards.globalCharas.Find("ashland");
			switch (EClass.debug.startScene)
			{
			case CoreDebug.StartScene.Zone:
				zone5 = EClass.game.spatials.Find(EClass.debug.startZone);
				transition = new ZoneTransition
				{
					state = ZoneTransition.EnterState.Exact,
					x = EClass.game.Prologue.startX,
					z = EClass.game.Prologue.startZ
				};
				break;
			case CoreDebug.StartScene.Home:
			case CoreDebug.StartScene.Story_Test:
				zone5 = EClass.game.spatials.Find(EClass.game.Prologue.idStartZone);
				transition = new ZoneTransition
				{
					state = ZoneTransition.EnterState.Exact,
					x = EClass.game.Prologue.startX,
					z = EClass.game.Prologue.startZ
				};
				break;
			case CoreDebug.StartScene.Survival:
				EClass.game.idPrologue = 3;
				zone5 = EClass.game.spatials.Find(EClass.game.Prologue.idStartZone);
				transition = new ZoneTransition
				{
					state = ZoneTransition.EnterState.Exact,
					x = EClass.game.Prologue.startX,
					z = EClass.game.Prologue.startZ
				};
				break;
			case CoreDebug.StartScene.Home_Cave:
				EClass.game.idPrologue = 2;
				zone5 = EClass.game.spatials.Find(EClass.game.Prologue.idStartZone);
				transition = new ZoneTransition
				{
					state = ZoneTransition.EnterState.Exact,
					x = EClass.game.Prologue.startX,
					z = EClass.game.Prologue.startZ
				};
				break;
			case CoreDebug.StartScene.MeetFarris:
			{
				Zone parent4 = EClass.game.spatials.Find("nymelle");
				zone5 = SpatialGen.Create("nymelle", parent4, register: true) as Zone;
				zone5.lv = -2;
				EClass.game.quests.Start("exploration", c, assignQuest: false);
				break;
			}
			case CoreDebug.StartScene.NymelleBoss:
			{
				Zone parent3 = EClass.game.spatials.Find("nymelle");
				zone5 = SpatialGen.Create("nymelle_boss", parent3, register: true) as Zone;
				zone5.lv = -5;
				EClass.game.quests.Start("exploration", c, assignQuest: false).ChangePhase(2);
				break;
			}
			case CoreDebug.StartScene.AfterNymelle:
				zone5 = EClass.game.spatials.Find(EClass.game.Prologue.idStartZone);
				transition = new ZoneTransition
				{
					state = ZoneTransition.EnterState.Exact,
					x = EClass.game.Prologue.startX,
					z = EClass.game.Prologue.startZ
				};
				EClass.game.quests.Start("exploration", c, assignQuest: false).ChangePhase(5);
				EClass.game.quests.globalList.Add(Quest.Create("sharedContainer").SetClient(c, assignQuest: false));
				EClass.game.quests.globalList.Add(Quest.Create("crafter").SetClient(c, assignQuest: false));
				EClass.game.quests.globalList.Add(Quest.Create("defense").SetClient(c, assignQuest: false));
				break;
			case CoreDebug.StartScene.Melilith:
			{
				Zone parent2 = EClass.game.spatials.Find("cursed_manor");
				zone5 = SpatialGen.Create("cursed_manor_dungeon", parent2, register: true) as Zone;
				zone5.lv = -5;
				break;
			}
			case CoreDebug.StartScene.Tefra:
			{
				Zone parent = EClass.game.spatials.Find("vernis");
				zone5 = SpatialGen.Create("vernis_mine", parent, register: true) as Zone;
				zone5.lv = -7;
				break;
			}
			}
			if (zone5 != null)
			{
				Player obj2 = player;
				Chara chara2 = EClass.pc;
				Zone zone2 = (EClass.pc.homeZone = zone5);
				Zone zone4 = (chara2.currentZone = zone2);
				obj2.zone = zone4;
				EClass.pc.global.transition = transition;
			}
			else
			{
				Player obj3 = player;
				Zone zone4 = (EClass.pc.currentZone = EClass.game.spatials.Find(EClass.debug.startZone));
				obj3.zone = zone4;
				EClass.pc.global.transition = new ZoneTransition
				{
					state = ZoneTransition.EnterState.Return
				};
			}
		}
		EClass.pc.homeZone.isKnown = true;
		AddAdventurers();
		player.OnStartNewGame();
		EClass.scene.Init(Scene.Mode.StartGame);
	}

	public void AddAdventurers()
	{
		List<Zone> source = world.region.ListTowns();
		string[] array = new string[7] { "adv_kiria", "adv_gaki", "adv_wini", "adv_verna", "adv_ivory", "adv_mesherada", "adv_yukiimo" };
		for (int i = 0; i < EClass.setting.balance.numAdv; i++)
		{
			Zone zone = source.RandomItem();
			if (zone is Zone_SubTown && EClass.rnd(5) != 0)
			{
				i--;
				continue;
			}
			if (i >= array.Length && i < array.Length + 2)
			{
				CardBlueprint.Set(CardBlueprint.Chara(i * 15));
			}
			Chara chara = CharaGen.Create((i < array.Length) ? array[i] : ((EClass.rnd(10) == 0) ? "adv_fairy" : "adv"));
			chara.SetHomeZone(zone);
			chara.global.transition = new ZoneTransition
			{
				state = ZoneTransition.EnterState.RandomVisit
			};
			zone.AddCard(chara);
			cards.listAdv.Add(chara);
		}
	}

	public void AddAdventurer(string id, Zone z = null)
	{
		if (z == null)
		{
			List<Zone> source = world.region.ListTowns();
			do
			{
				z = source.RandomItem();
			}
			while ((z is Zone_SubTown && EClass.rnd(5) != 0) || z == EClass.pc.currentZone);
		}
		Chara chara = CharaGen.Create(id);
		chara.SetHomeZone(z);
		chara.global.transition = new ZoneTransition
		{
			state = ZoneTransition.EnterState.RandomVisit
		};
		z.AddCard(chara);
		cards.listAdv.Add(chara);
	}

	public void GotoTitle(bool showDialog = true)
	{
		if (showDialog)
		{
			Dialog.YesNo("dialog_gotoTitle", delegate
			{
				if (EClass.game.Save(isAutoSave: true))
				{
					EClass.scene.Init(Scene.Mode.Title);
				}
			});
		}
		else
		{
			EClass.scene.Init(Scene.Mode.Title);
		}
	}

	public void Quit()
	{
		Dialog.YesNo("dialog_quit", delegate
		{
			if (EClass.game.Save())
			{
				EClass.core.Quit();
			}
		});
	}

	public bool Save(bool isAutoSave = false, bool silent = false)
	{
		if (EClass.ui.IsDragging)
		{
			EClass.ui.EndDrag(canceled: true);
		}
		if (isAutoSave && EClass.debug.ignoreAutoSave)
		{
			return true;
		}
		bool flag = EClass.core.config.game.autoBackup && backupTime >= (double)(EClass.core.config.game.backupInterval * 60 * 30);
		if (flag)
		{
			backupTime = 0.0;
		}
		EClass.core.config.TryUpdatePlayedHour();
		countLoadedMaps = 0;
		player.pref.layerInventory = EClass.ui.layerFloat.GetLayer<LayerInventory>();
		if (player.pref.layerInventory)
		{
			player.pref.layerAbility = EClass.ui.layerFloat.GetLayer<LayerAbility>();
		}
		player.pref.layerCraft = EClass.ui.layerFloat.GetLayer<LayerCraftFloat>();
		player.angle = EClass.pc.angle;
		version = EClass.core.version;
		EClass.ui.widgets.UpdateConfigs();
		GameIndex gameIndex = null;
		try
		{
			OnBeforeSave();
			gameIndex = GameIO.SaveGame();
		}
		catch (Exception ex)
		{
			EClass.ui.Say(ex.Message);
			SE.Beep();
			Msg.Say("error_save");
			return false;
		}
		if (flag)
		{
			GameIO.MakeBackup(gameIndex);
			EClass.ui.Say("backupDone");
		}
		if (!silent)
		{
			if (!isAutoSave)
			{
				SE.WriteJournal();
			}
			Msg.Say("saved");
		}
		saveCount++;
		return true;
	}

	public void OnBeforeSave()
	{
		foreach (Zone zone in spatials.Zones)
		{
			zone.OnBeforeSave();
		}
		player.OnBeforeSave();
		foreach (Spatial item in spatials.listDestryoed)
		{
			item.DeleteMapRecursive();
		}
		spatials.listDestryoed.Clear();
	}

	public void Kill()
	{
		isKilling = true;
		Debug.Log("Killing Game: IsStarted? " + EClass.core.IsGameStarted);
		if (EClass.core.IsGameStarted)
		{
			EClass.ui.OnKillGame();
			EClass.scene.OnKillGame();
			activeZone.OnKillGame();
		}
		EClass.core.game = (Instance = null);
		EClass.Sound.currentPlaylist = null;
		Window.dictTab.Clear();
		foreach (Texture2D loadedTexture in loadedTextures)
		{
			if (loadedTexture != null)
			{
				UnityEngine.Object.Destroy(loadedTexture);
			}
		}
	}

	public void Pause(Action onUnpause = null)
	{
		isPaused = true;
		LayerPause layerPause = EClass.ui.AddLayer<LayerPause>();
		Game lastGame = EClass.game;
		if (onUnpause == null)
		{
			return;
		}
		layerPause.SetOnKill(delegate
		{
			if (EClass.core.IsGameStarted && lastGame == EClass.game)
			{
				onUnpause();
			}
		});
	}

	[OnSerializing]
	private void OnSerializing(StreamingContext context)
	{
		Debug.Log("#io game OnSerializing");
	}

	[OnDeserializing]
	private void OnDeserializing(StreamingContext context)
	{
		Debug.Log("#io game OnDeserializing");
		Instance = this;
	}
}
