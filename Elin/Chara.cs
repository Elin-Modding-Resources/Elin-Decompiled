using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Algorithms;
using Newtonsoft.Json;
using UnityEngine;

public class Chara : Card, IPathfindWalker
{
	private static Point shared = new Point();

	private static List<Hobby> listHobby = new List<Hobby>();

	public static string[] IDTimeTable = new string[2] { "default", "owl" };

	[JsonProperty]
	public int contribution;

	[JsonProperty]
	public Point orgPos;

	[JsonProperty]
	public Quest quest;

	[JsonProperty]
	public Chara ride;

	[JsonProperty]
	public Chara parasite;

	[JsonProperty]
	public Chara host;

	[JsonProperty]
	public ElementContainer tempElements;

	public ElementContainer faithElements;

	public ElementContainer workElements;

	[JsonProperty(PropertyName = "T1")]
	public Party party;

	[JsonProperty(PropertyName = "T2")]
	public FactionMemberType memberType;

	[JsonProperty(PropertyName = "T3")]
	public List<int> _hobbies;

	[JsonProperty(PropertyName = "T4")]
	public List<int> _works;

	[JsonProperty(PropertyName = "T5")]
	public WorkSummary _workSummary;

	[JsonProperty(PropertyName = "T6")]
	public List<int> _cooldowns;

	[JsonProperty(PropertyName = "T7")]
	public List<int> _listAbility;

	[JsonProperty(PropertyName = "T8")]
	public List<List<string>> _historyFood;

	[JsonProperty(PropertyName = "1")]
	public PCCData pccData;

	[JsonProperty(PropertyName = "2")]
	public Card held;

	[JsonProperty(PropertyName = "3")]
	public int[] rawSlots;

	[JsonProperty(PropertyName = "4")]
	public GlobalData global;

	[JsonProperty(PropertyName = "8")]
	public string[] _strs = new string[5];

	[JsonProperty(PropertyName = "9")]
	public int[] _cints = new int[30];

	public BitArray32 _cbits1;

	public Chara enemy;

	public Chara master;

	public Point lastPos = new Point();

	public PathProgress path = new PathProgress();

	public CharaBody body = new CharaBody();

	public CharaAbility _ability;

	public Thing ranged;

	public Thing nextUse;

	public ConSleep conSleep;

	public ConSuspend conSuspend;

	public Emo2 emoIcon;

	public int happiness;

	public int turnLastSeen = -100;

	public int idleActTimer;

	public int combatCount;

	public int calmCheckTurn;

	public int sharedCheckTurn;

	public float idleTimer;

	public bool isBerserk;

	public bool isDrunk;

	public bool isConfused;

	public bool isFainted;

	public bool isBlind;

	public bool isParalyzed;

	public bool _isLevitating;

	public bool isCreated;

	public bool canSeeInvisible;

	public bool hasTelepathy;

	public bool isWet;

	public bool bossText;

	public bool ignoreSPAbsorb;

	public bool wasInWater;

	public SpriteReplacer spriteReplacer;

	private Faction _faction;

	public SourceChara.Row source;

	public SourceRace.Row _race;

	public SourceJob.Row _job;

	public Tactics _tactics;

	public static List<SourceRace.Row> ListAdvRace = new List<SourceRace.Row>();

	public static List<SourceJob.Row> ListAdvJob = new List<SourceJob.Row>();

	public bool dirtySpeed = true;

	private int _Speed;

	private static Point _sharedPos = new Point();

	private bool hasMovedThisTurn;

	public float actTime = 0.3f;

	public static bool consumeTurn;

	public static bool preventRegen;

	public static bool isOnCreate;

	private static List<Chara> _pts = new List<Chara>();

	public int stealthSeen;

	private static List<Thing> _ListItems = new List<Thing>();

	private static List<SourceThing.Row> _listFavFood = new List<SourceThing.Row>();

	private static List<SourceCategory.Row> _listFavCat = new List<SourceCategory.Row>();

	public static NoGoal _NoGoalPC = new NoGoal();

	public static NoGoal _NoGoalRepeat = new NoGoal();

	public GoalList goalList = new GoalList();

	public AIAct ai = new NoGoal();

	public ElementContainer _baseWorkElements;

	private static GoalWork _goalWork = new GoalWork();

	private static GoalHobby _goalHobby = new GoalHobby();

	[JsonProperty]
	public List<Condition> conditions = new List<Condition>();

	[JsonProperty]
	public Dictionary<int, int> resistCon;

	public string Aka => _alias.IsEmpty(source.GetText("aka", returnNull: true) ?? "");

	public string _alias
	{
		get
		{
			return _strs[0];
		}
		set
		{
			_strs[0] = value;
		}
	}

	public string idFaith
	{
		get
		{
			return _strs[1];
		}
		set
		{
			_strs[1] = value;
		}
	}

	public string idFaction
	{
		get
		{
			return _strs[2];
		}
		set
		{
			_strs[2] = value;
		}
	}

	public Zone currentZone
	{
		get
		{
			return RefZone.Get(_cints[1]);
		}
		set
		{
			_cints[1] = RefZone.Set(value);
		}
	}

	public Zone homeZone
	{
		get
		{
			return RefZone.Get(_cints[2]);
		}
		set
		{
			_cints[2] = RefZone.Set(value);
		}
	}

	public GoalListType goalListType
	{
		get
		{
			return _cints[3].ToEnum<GoalListType>();
		}
		set
		{
			_cints[3] = (int)value;
		}
	}

	public Hostility hostility
	{
		get
		{
			return _cints[4].ToEnum<Hostility>();
		}
		set
		{
			_cints[4] = (int)value;
		}
	}

	public int _affinity
	{
		get
		{
			return _cints[5];
		}
		set
		{
			_cints[5] = value;
		}
	}

	public Affinity affinity => Affinity.Get(this);

	public int interest
	{
		get
		{
			return _cints[6];
		}
		set
		{
			_cints[6] = value;
		}
	}

	public int daysStarved
	{
		get
		{
			return _cints[7];
		}
		set
		{
			_cints[7] = value;
		}
	}

	public int _idTimeTable
	{
		get
		{
			return _cints[9];
		}
		set
		{
			_cints[9] = value;
		}
	}

	public int uidEditor
	{
		get
		{
			return _cints[10];
		}
		set
		{
			_cints[10] = value;
		}
	}

	public int _maxStamina
	{
		get
		{
			return _cints[20];
		}
		set
		{
			_cints[20] = value;
		}
	}

	public int corruption
	{
		get
		{
			return _cints[21];
		}
		set
		{
			_cints[21] = value;
		}
	}

	public bool isDead
	{
		get
		{
			return _cbits1[0];
		}
		set
		{
			_cbits1[0] = value;
		}
	}

	public bool isAlawysVisible
	{
		get
		{
			return _cbits1[1];
		}
		set
		{
			_cbits1[1] = value;
		}
	}

	public bool knowFav
	{
		get
		{
			return _cbits1[3];
		}
		set
		{
			_cbits1[3] = value;
		}
	}

	public CharaAbility ability => _ability ?? (_ability = new CharaAbility(this));

	public Faction faction
	{
		get
		{
			return _faction ?? (_faction = EClass.game.factions.dictAll.TryGetValue(idFaction));
		}
		set
		{
			idFaction = value?.uid;
		}
	}

	public Religion faith
	{
		get
		{
			return EClass.game.religions.dictAll.TryGetValue(idFaith) ?? EClass.game.religions.dictAll["eyth"];
		}
		set
		{
			idFaith = value?.id;
		}
	}

	public override CardRow sourceCard => source;

	public override CardRow sourceRenderCard
	{
		get
		{
			if (pccData != null && !(source.renderData is RenderDataPcc))
			{
				return SourceChara.rowDefaultPCC;
			}
			return source;
		}
	}

	public SourceRace.Row race => _race ?? (_race = EClass.sources.races.map.TryGetValue(base.c_idRace.IsEmpty(source.race))) ?? EClass.sources.races.map["norland"];

	public SourceJob.Row job => _job ?? (_job = EClass.sources.jobs.map.TryGetValue(base.c_idJob.IsEmpty(source.job))) ?? EClass.sources.jobs.map["none"];

	public string idTimeTable => IDTimeTable[_idTimeTable];

	public Hostility OriginalHostility
	{
		get
		{
			if (EClass.pc == null || !IsPCFaction)
			{
				if (base.c_originalHostility == (Hostility)0)
				{
					if (!source.hostility.IsEmpty())
					{
						return source.hostility.ToEnum<Hostility>();
					}
					return Hostility.Enemy;
				}
				return base.c_originalHostility;
			}
			return Hostility.Ally;
		}
	}

	public string IDPCCBodySet
	{
		get
		{
			if (source.idActor.Length <= 1)
			{
				return "female";
			}
			return source.idActor[1];
		}
	}

	public new TraitChara trait
	{
		get
		{
			return base.trait as TraitChara;
		}
		set
		{
			base.trait = value;
		}
	}

	public string NameBraced => GetName(NameStyle.Full);

	public string NameTitled => (EClass.game.principal.permadeath ? "★" : "") + NameBraced;

	public override string actorPrefab
	{
		get
		{
			if (IsPCC)
			{
				return "CharaActorPCC";
			}
			return "CharaActor";
		}
	}

	public override int PrefIndex
	{
		get
		{
			if (sourceCard._tiles.Length != 0 || renderer.replacer != null)
			{
				if (base.dir != 1 && base.dir != 2)
				{
					return 0;
				}
				return 1;
			}
			return base.dir;
		}
	}

	public override bool flipX
	{
		get
		{
			if (base.dir != 1)
			{
				return base.dir == 2;
			}
			return true;
		}
	}

	public override string AliasMaterialOnCreate => race.material;

	public override bool IsAliveInCurrentZone
	{
		get
		{
			if (!isDead)
			{
				return base.ExistsOnMap;
			}
			return false;
		}
	}

	public override bool IsDeadOrSleeping
	{
		get
		{
			if (!isDead && conSleep == null && conSuspend == null)
			{
				return isFainted;
			}
			return true;
		}
	}

	public override bool IsDisabled
	{
		get
		{
			if (!isDead && conSleep == null && !isFainted)
			{
				return isParalyzed;
			}
			return true;
		}
	}

	public bool IsLevitating
	{
		get
		{
			if (ride != null)
			{
				return ride._isLevitating;
			}
			return _isLevitating;
		}
	}

	public override bool IsMoving => idleTimer > 0f;

	public override bool IsGlobal => global != null;

	public override bool IsPC => this == EClass.player.chara;

	public override bool IsPCParty
	{
		get
		{
			if (party != null)
			{
				return party.leader == EClass.pc;
			}
			return false;
		}
	}

	public override bool IsMinion
	{
		get
		{
			if (master == null)
			{
				return base.c_uidMaster != 0;
			}
			return true;
		}
	}

	public override bool IsPCPartyMinion
	{
		get
		{
			if (master != null)
			{
				if (!master.IsPCParty)
				{
					return master.IsPCPartyMinion;
				}
				return true;
			}
			return false;
		}
	}

	public override bool IsPCFactionMinion
	{
		get
		{
			if (master != null)
			{
				if (!master.IsPCFaction)
				{
					return master.IsPCFactionMinion;
				}
				return true;
			}
			return false;
		}
	}

	public override bool IsPCFaction
	{
		get
		{
			if (EClass.pc != null)
			{
				return faction == EClass.pc.faction;
			}
			return false;
		}
	}

	public override bool IsPCC => pccData != null;

	public override bool isThing => false;

	public override bool isChara => true;

	public override bool HasHost => host != null;

	public override bool isSynced
	{
		get
		{
			if (!renderer.isSynced)
			{
				if (host != null)
				{
					return host.renderer.isSynced;
				}
				return false;
			}
			return true;
		}
	}

	public override bool IsMultisize => sourceCard.multisize;

	public override int MaxHP => (int)Mathf.Clamp(((long)(base.END * 2 + base.STR + base.WIL / 2) * (long)Mathf.Min(base.LV, 25) / 25 + base.END + 10) * Evalue(60) / 100 * ((IsPCFaction ? 100 : (100 + (int)base.rarity * 300)) + (IsPC ? (EClass.player.lastEmptyAlly * Evalue(1646)) : 0)) / 100, 1f, 100000000f);

	public override int WeightLimit => Mathf.Max((base.STR * 500 + base.END * 250 + Evalue(207) * 2000) * ((!HasElement(1411)) ? 1 : 5) + 45000, 1000);

	public override int SelfWeight => bio.weight * 1000;

	public int MaxSummon => Mathf.Max((int)Mathf.Sqrt(base.CHA), 1) + Evalue(1647) + ((!base.IsPCFactionOrMinion) ? ((int)base.rarity * 5) : 0);

	public Element MainElement
	{
		get
		{
			if (base.c_idMainElement == 0)
			{
				return Element.Void;
			}
			return elements.GetElement(base.c_idMainElement);
		}
	}

	public override int DV
	{
		get
		{
			if (IsPCFaction)
			{
				return WeaknessMod(elements.Value(64));
			}
			int num = base.LV;
			if (num > 50)
			{
				num = 50 + (num - 50) / 10;
			}
			return WeaknessMod(num + elements.Value(64) * (100 + num + race.DV * 5) / 100);
			int WeaknessMod(int a)
			{
				if (HasCondition<ConWeakness>())
				{
					if (a <= 0)
					{
						return a * 2;
					}
					return a / 2;
				}
				return a;
			}
		}
	}

	public override int PV
	{
		get
		{
			if (IsPCFaction)
			{
				return WeaknessMod(elements.Value(65));
			}
			int num = base.LV;
			if (num > 50)
			{
				num = 50 + (num - 50) / 10;
			}
			return WeaknessMod(num + elements.Value(65) * (100 + num + race.PV * 5) / 100);
			int WeaknessMod(int a)
			{
				if (HasCondition<ConWeakness>())
				{
					if (a <= 0)
					{
						return a * 2;
					}
					return a / 2;
				}
				return a;
			}
		}
	}

	public override SourcePref Pref
	{
		get
		{
			if (spriteReplacer != null)
			{
				return EClass.core.refs.prefs.replacer1;
			}
			if (base.idSkin > 0)
			{
				switch (sourceCard.tiles.TryGet(base.idSkin))
				{
				case 2319:
				case 2619:
				case 2621:
				case 2623:
				case 2625:
					return EClass.core.refs.prefs.sonwputit1;
				case 2320:
				case 2611:
				case 2620:
				case 2622:
				case 2624:
				case 2626:
				case 2828:
					return EClass.core.refs.prefs.snowputit2;
				}
			}
			return sourceCard.pref;
		}
	}

	public bool CanOpenDoor
	{
		get
		{
			if (base.INT < 5)
			{
				if (IsPCFaction)
				{
					return memberType == FactionMemberType.Default;
				}
				return false;
			}
			return true;
		}
	}

	public Tactics tactics => _tactics ?? (_tactics = new Tactics(this));

	public TimeTable.Span CurrentSpan => TimeTable.GetSpan(idTimeTable, EClass.world.date.hour);

	public bool IsInActiveZone => currentZone == EClass.game.activeZone;

	public bool IsLocalChara
	{
		get
		{
			if (!IsGlobal && !base.isSubsetCard)
			{
				return homeZone == EClass._zone;
			}
			return false;
		}
	}

	public bool IsIdle
	{
		get
		{
			if (!IsDeadOrSleeping)
			{
				return ai.Current.IsIdle;
			}
			return false;
		}
	}

	public bool IsInCombat => ai is GoalCombat;

	public int DestDist => tactics.DestDist;

	public bool HasNoGoal => ai.IsNoGoal;

	public bool CanWitness
	{
		get
		{
			if (!race.IsHuman && !race.IsFairy && !race.IsGod)
			{
				return race.id == "mutant";
			}
			return true;
		}
	}

	public bool IsHuman => race.tag.Contains("human");

	public bool IsHumanSpeak
	{
		get
		{
			if (!IsHuman)
			{
				return race.tag.Contains("humanSpeak");
			}
			return true;
		}
	}

	public bool IsMaid
	{
		get
		{
			if (EClass.Branch != null)
			{
				return EClass.Branch.uidMaid == base.uid;
			}
			return false;
		}
	}

	public bool IsPrisoner => false;

	public bool IsAdventurer
	{
		get
		{
			if (global != null && faction != EClass.pc.faction)
			{
				return IsPCC;
			}
			return false;
		}
	}

	public bool IsEyth => faith.id == "eyth";

	public bool IsWealthy
	{
		get
		{
			if (!source.works.Contains("Rich"))
			{
				return source.hobbies.Contains("Rich");
			}
			return true;
		}
	}

	public FactionBranch homeBranch => homeZone?.branch;

	public int MaxGeneSlot => race.geneCap - (HasElement(1237) ? 2 : 0);

	public int CurrentGeneSlot
	{
		get
		{
			if (base.c_genes != null)
			{
				return base.c_genes.GetGeneSlot(this);
			}
			return 0;
		}
	}

	public int Speed
	{
		get
		{
			if (dirtySpeed)
			{
				RefreshSpeed();
			}
			return _Speed;
		}
	}

	public bool IsMofuable => race.tag.Contains("mofu");

	public ElementContainer baseWorkElements
	{
		get
		{
			if (_baseWorkElements == null)
			{
				_baseWorkElements = new ElementContainer();
				foreach (Hobby item in ListHobbies())
				{
					Build(item);
				}
				foreach (Hobby item2 in ListWorks())
				{
					Build(item2);
				}
			}
			return _baseWorkElements;
			void Build(Hobby h)
			{
				if (!h.source.elements.IsEmpty())
				{
					for (int i = 0; i < h.source.elements.Length; i += 2)
					{
						int ele = h.source.elements[i];
						int v = h.source.elements[i + 1];
						_baseWorkElements.ModBase(ele, v);
					}
				}
			}
		}
	}

	public Stats hunger => Stats.Hunger.Set(_cints, 10, this);

	public Stats burden => Stats.Burden.Set(_cints, 11, this);

	public Stats stamina => Stats.Stamina.Set(_cints, 12, this);

	public Stats depression => Stats.Depression.Set(_cints, 13, this);

	public Stats bladder => Stats.Bladder.Set(_cints, 14, this);

	public Stats hygiene => Stats.Hygiene.Set(_cints, 15, this);

	public Stats mana => Stats.Mana.Set(_cints, 16, this);

	public Stats sleepiness => Stats.Sleepiness.Set(_cints, 17, this);

	public Stats SAN => Stats.SAN.Set(_cints, 18, this);

	public bool CanGainConResist
	{
		get
		{
			if (base.rarity >= Rarity.Legendary)
			{
				return !IsPCFaction;
			}
			return false;
		}
	}

	public override string ToString()
	{
		return base.Name + "(" + ai?.ToString() + ")" + pos?.ToString() + "/" + base.ExistsOnMap + "/" + isDead;
	}

	public bool IsCriticallyWounded(bool includeRide = false)
	{
		if (host == null || includeRide)
		{
			if (Evalue(1421) <= 0)
			{
				return base.hp < MaxHP / 5;
			}
			return base.hp + mana.value < (MaxHP + mana.max) / 5;
		}
		return false;
	}

	public bool HasHigherGround(Card c)
	{
		if (c == null)
		{
			return false;
		}
		float num = renderer.position.y - pos.Position(0).y + (_isLevitating ? 0.4f : 0f);
		float num2 = c.renderer.position.y - c.pos.Position(0).y + ((c.isChara && c.Chara._isLevitating) ? 0.4f : 0f);
		return num > num2 + 0.1f;
	}

	public bool CanSeeSimple(Point p)
	{
		if (!p.IsValid || p.IsHidden)
		{
			return false;
		}
		if (IsPC)
		{
			if (fov != null)
			{
				return fov.lastPoints.ContainsKey(p.index);
			}
			return false;
		}
		return true;
	}

	public bool CanSee(Card c)
	{
		if (c == this)
		{
			return true;
		}
		if (!c.pos.IsValid)
		{
			return false;
		}
		if (c.isChara)
		{
			if (hasTelepathy && c.Chara.race.visibleWithTelepathy)
			{
				return true;
			}
			if (c.isHidden && c != this && !canSeeInvisible)
			{
				return false;
			}
		}
		if (IsPC)
		{
			if (fov != null)
			{
				return fov.lastPoints.ContainsKey(c.pos.index);
			}
			return false;
		}
		return true;
	}

	public bool CanSeeLos(Card c, int dist = -1)
	{
		if (c.isHidden && !canSeeInvisible && (!hasTelepathy || !c.Chara.race.visibleWithTelepathy))
		{
			return false;
		}
		return CanSeeLos(c.pos, dist);
	}

	public bool CanSeeLos(Point p, int dist = -1)
	{
		if (dist == -1)
		{
			dist = pos.Distance(p);
		}
		if (dist > GetSightRadius())
		{
			return false;
		}
		if (IsPC)
		{
			if (fov != null)
			{
				return fov.lastPoints.ContainsKey(p.index);
			}
			return false;
		}
		return Los.IsVisible(pos, p);
	}

	public bool HasAccess(Card c)
	{
		return HasAccess(c.pos);
	}

	public bool HasAccess(Point p)
	{
		if (!EClass._zone.IsPCFaction || p.cell.room == null || IsPC)
		{
			return true;
		}
		return p.cell.room.data.accessType switch
		{
			BaseArea.AccessType.Resident => memberType == FactionMemberType.Default, 
			BaseArea.AccessType.Private => false, 
			_ => true, 
		};
	}

	public bool CanSleep()
	{
		if (EClass._zone.events.GetEvent<ZoneEventQuest>() != null)
		{
			return false;
		}
		if (!EClass.debug.godMode && sleepiness.GetPhase() == 0)
		{
			return stamina.GetPhase() <= 1;
		}
		return true;
	}

	protected override void OnSerializing()
	{
		if (enemy != null)
		{
			SetInt(55, enemy.uid);
		}
		_cints[0] = (int)_cbits1.Bits;
		List<BodySlot> slots = body.slots;
		rawSlots = new int[slots.Count];
		for (int i = 0; i < slots.Count; i++)
		{
			rawSlots[i] = slots[i].elementId;
		}
	}

	protected override void OnDeserialized()
	{
		isCreated = true;
		_cbits1.Bits = (uint)_cints[0];
		InitStats(onDeserialize: true);
		body.SetOwner(this, deserialized: true);
		elements.ApplyElementMap(base.uid, SourceValueType.Chara, job.elementMap, base.DefaultLV);
		elements.ApplyElementMap(base.uid, SourceValueType.Chara, race.elementMap, base.DefaultLV);
		if (global != null && global.goal != null)
		{
			global.goal.SetOwner(this);
		}
		if (IsPCC)
		{
			pccData.state = (base.isCensored ? PCCState.Naked : PCCState.Normal);
		}
		if (tempElements != null)
		{
			tempElements.SetParent(this);
		}
		UpdateAngle();
		RefreshFaithElement();
		Refresh();
		if (source.tag.Contains("boss"))
		{
			bossText = true;
		}
		sharedCheckTurn = EClass.rnd(200);
	}

	public override string GetName(NameStyle style, int num = -1)
	{
		if (base.isBackerContent && EClass.core.config.backer.Show(base.c_idBacker))
		{
			if (id == "follower" && !IsGlobal)
			{
				return "_follower".lang(EClass.sources.backers.map[base.c_idBacker].Name, faith.Name);
			}
			return EClass.sources.backers.map[base.c_idBacker].Name;
		}
		string text = base.c_altName ?? source.GetName(this);
		text = text.Replace("#ele4", MainElement.source.GetAltname(2)).Replace("#ele3", MainElement.source.GetAltname(1)).Replace("#ele2", MainElement.source.GetAltname(0))
			.Replace("#ele", MainElement.source.GetName().ToLower());
		if (base.c_bossType == BossType.Evolved)
		{
			text = "_evolved".lang(text.ToTitleCase(wholeText: true));
		}
		trait.SetName(ref text);
		if (text.Length > 0 && char.IsLower(text[0]))
		{
			if (base.rarity >= Rarity.Legendary)
			{
				text = text.ToTitleCase();
			}
			else if (num != 0)
			{
				text = text.AddArticle();
			}
		}
		if (style == NameStyle.Simple)
		{
			return text;
		}
		int num2 = ((base.rarity == Rarity.Mythical) ? 3 : ((base.rarity >= Rarity.Legendary) ? 2 : ((!_alias.IsEmpty()) ? 1 : (-1))));
		if (trait is TraitAdventurer)
		{
			num2 = 1;
		}
		text = (Aka.IsEmpty() ? text.Bracket(num2) : (num2 switch
		{
			-1 => "_aka", 
			1 => "_aka3", 
			_ => "_aka2", 
		}).lang((num2 == -1) ? Aka : Aka.ToTitleCase(wholeText: true), text.Bracket(num2)));
		string text2 = (base.isSale ? "forSale".lang(Lang._currency(GetPrice(CurrencyType.Money, sell: true, PriceType.PlayerShop), "money")) : "");
		return text + text2;
	}

	public override void ChangeRarity(Rarity r)
	{
		if (r != base.rarity)
		{
			base.rarity = r;
			if (renderer != null && renderer.isSynced)
			{
				renderer.RefreshExtra();
			}
			base.hp = MaxHP;
		}
	}

	public void SetFaction(Faction f)
	{
		_faction = null;
		faction = f;
		hostility = faction.GetHostility();
	}

	public void SetHomeZone(Zone zone)
	{
		homeZone = zone;
		SetGlobal();
	}

	public void OnBanish()
	{
		if (!IsGlobal)
		{
			return;
		}
		memberType = FactionMemberType.Default;
		foreach (Thing item in things.List((Thing a) => a.HasTag(CTAG.godArtifact)).ToList())
		{
			EClass.pc.PickOrDrop(EClass.pc.pos, item);
		}
		UniqueData uniqueData = base.c_uniqueData;
		if (uniqueData != null)
		{
			Debug.Log(uniqueData.uidZone + "/" + EClass.game.spatials.map.ContainsKey(uniqueData.uidZone) + "/" + uniqueData.x + "/" + uniqueData.y);
		}
		if (uniqueData != null && EClass.game.spatials.map.ContainsKey(uniqueData.uidZone))
		{
			MoveHome(EClass.game.spatials.map[uniqueData.uidZone] as Zone, uniqueData.x, uniqueData.y);
			return;
		}
		Zone z = EClass.game.spatials.Find("somewhere");
		if (trait is TraitAdventurer)
		{
			z = EClass.world.region.ListTowns().RandomItem();
			SetHomeZone(z);
		}
		MoveZone(z, ZoneTransition.EnterState.RandomVisit);
	}

	public Chara SetGlobal(Zone _home, int x, int z)
	{
		SetGlobal();
		homeZone = _home;
		_home.AddCard(this, x, z);
		global.transition = new ZoneTransition
		{
			state = ZoneTransition.EnterState.Exact,
			x = x,
			z = z
		};
		orgPos = new Point(x, z);
		return this;
	}

	public Chara SetGlobal()
	{
		if (!IsGlobal)
		{
			EClass.game.cards.globalCharas.Add(this);
			global = new GlobalData();
			base.isSubsetCard = false;
			enemy = null;
			base.c_uidMaster = 0;
		}
		return this;
	}

	public void RemoveGlobal()
	{
		if (IsGlobal && !(trait is TraitUniqueChara) && !base.IsUnique && !EClass.game.cards.listAdv.Contains(this))
		{
			global = null;
			EClass.game.cards.globalCharas.Remove(this);
		}
	}

	public override void OnBeforeCreate()
	{
		if (source.job == "*r")
		{
			base.c_idJob = EClass.sources.jobs.rows.Where((SourceJob.Row j) => j.playable < 4).RandomItem().id;
		}
		if (bp.idJob != null)
		{
			base.c_idJob = bp.idJob;
		}
		if (bp.idRace != null)
		{
			base.c_idRace = bp.idRace;
		}
	}

	public override void OnCreate(int genLv)
	{
		if (source.tag.Contains("boss"))
		{
			bossText = true;
		}
		int num = ((bp.lv != -999) ? bp.lv : base.LV);
		if (trait.AdvType == TraitChara.Adv_Type.Adv || trait.AdvType == TraitChara.Adv_Type.Adv_Fairy)
		{
			if (ListAdvRace.Count == 0)
			{
				ListAdvRace = EClass.sources.races.rows.Where((SourceRace.Row a) => a.playable <= 1 && a.id != "fairy").ToList();
				ListAdvJob = EClass.sources.jobs.rows.Where((SourceJob.Row a) => a.playable <= 4).ToList();
			}
			if (trait.AdvType == TraitChara.Adv_Type.Adv)
			{
				base.c_idRace = ListAdvRace.RandomItem().id;
			}
			base.c_idJob = ListAdvJob.RandomItem().id;
			_race = null;
			_job = null;
			num = 10 + EClass.rnd(40);
			ChangeMaterial(race.material, ignoreFixedMaterial: true);
		}
		bio = new Biography();
		bio.Generate(this);
		if (source.idActor.Length != 0 && source.idActor[0] == "pcc" && pccData == null)
		{
			pccData = PCCData.Create(IDPCCBodySet);
			if (source.idActor.Length > 2)
			{
				pccData.SetPart("body", IDPCCBodySet, source.idActor[2]);
			}
			else
			{
				pccData.Randomize(IDPCCBodySet);
			}
		}
		if (source.mainElement.Length != 0)
		{
			int _genLv = Mathf.Min(genLv, 100);
			List<Tuple<string, int, int>> list = new List<Tuple<string, int, int>>();
			string[] mainElement = source.mainElement;
			for (int i = 0; i < mainElement.Length; i++)
			{
				string[] array = mainElement[i].Split('/');
				SourceElement.Row row = EClass.sources.elements.alias["ele" + array[0]];
				int num2 = source.LV * row.eleP / 100;
				if (list.Count == 0 || num2 < _genLv || array[0] == bp.idEle)
				{
					list.Add(new Tuple<string, int, int>(array[0], (array.Length > 1) ? int.Parse(array[1]) : 0, num2));
				}
			}
			Tuple<string, int, int> tuple = list.RandomItemWeighted((Tuple<string, int, int> a) => 10000 / (100 + (_genLv - a.Item3) * 25));
			if (!bp.idEle.IsEmpty())
			{
				tuple = list.Where((Tuple<string, int, int> a) => a.Item1 == bp.idEle).FirstOrDefault() ?? tuple;
			}
			SetMainElement(tuple.Item1, (tuple.Item2 == 0) ? 10 : tuple.Item2, elemental: true);
			if (list.Count >= 2)
			{
				num = tuple.Item3 + base.LV - source.LV;
			}
		}
		if (source.name == "*r")
		{
			base.c_altName = NameGen.getRandomName();
		}
		if (source.GetText("aka") == "*r" || trait.UseRandomAlias)
		{
			_alias = AliasGen.GetRandomAlias();
		}
		happiness = EClass.rnd(100);
		contribution = EClass.rnd(100);
		RerollHobby();
		_idTimeTable = ((EClass.rnd(5) == 0) ? 1 : 0);
		ApplyRace();
		ApplyJob();
		if (num != source.LV)
		{
			SetLv(num);
		}
		if (base.LV > 5 && race.id == "mutant")
		{
			for (int j = 0; j < Mathf.Min(1 + base.LV / 5, 22); j++)
			{
				SetFeat(1644, j + 1);
			}
		}
		InitStats();
		body.SetOwner(this);
		hostility = OriginalHostility;
		if (race.EQ.Length != 0 || !source.equip.IsEmpty())
		{
			TryRestock(onCreate: true);
		}
		switch (id)
		{
		case "golem_wood":
			ChangeMaterial("pine");
			break;
		case "golem_steel":
			ChangeMaterial("steel");
			break;
		case "golem_gold":
			ChangeMaterial("gold");
			break;
		case "dodo":
			base.idSkin = EClass.rnd(4);
			break;
		case "putty_snow":
			if (EClass.rnd(100) == 0 || EClass.debug.enable)
			{
				base.idSkin = EClass.rnd(4);
			}
			break;
		case "snail":
			base.idSkin = 5;
			break;
		case "olderyoungersister":
			base.idSkin = 1;
			break;
		case "sister_undead":
		case "dog":
		case "cat":
			base.idSkin = EClass.rnd(sourceCard.tiles.Length);
			if (id == "sister_undead" && EClass.rnd(10) == 0)
			{
				SourceBacker.Row row2 = EClass.sources.backers.listSister.NextItem(ref BackerContent.indexSister);
				if (row2 != null && (!EClass.player.doneBackers.Contains(row2.id) || EClass.core.config.test.ignoreBackerDestoryFlag))
				{
					ApplyBacker(row2.id);
				}
			}
			break;
		case "baby":
		case "ika":
			SetFeat(1232, 3);
			break;
		}
		if (HasTag(CTAG.random_color))
		{
			DyeRandom();
		}
		SetAI(new NoGoal());
		if (!source.faith.IsEmpty())
		{
			SetFaith(source.faith);
		}
		else if (EClass.game.activeZone != null && EClass.game.activeZone.id == "foxtown")
		{
			SetFaith(EClass.game.religions.Trickery);
		}
		else if (EClass.game.activeZone != null && EClass.game.activeZone.id == "foxtown_nefu")
		{
			SetFaith(EClass.game.religions.MoonShadow);
		}
		else
		{
			SetFaith(EClass.game.religions.GetRandomReligion(onlyJoinable: true, EClass.rnd(10) == 0));
		}
		_affinity = 0;
		interest = 100;
		CalculateMaxStamina();
		Refresh();
		stamina.value = stamina.max;
		mana.value = mana.max;
		isCreated = true;
	}

	public void SetFaith(string id)
	{
		SetFaith(EClass.game.religions.dictAll[id]);
	}

	public void SetFaith(Religion r)
	{
		faith = r;
		RefreshFaithElement();
	}

	public void HealAll()
	{
		Cure(CureType.Death);
		base.hp = MaxHP;
		mana.value = mana.max;
		stamina.value = stamina.max;
		Refresh();
	}

	public void Refresh(bool calledRecursive = false)
	{
		if (ride != null)
		{
			ride.Refresh(calledRecursive: true);
		}
		hasTelepathy = false;
		isWet = false;
		_isLevitating = HasElement(401) || (ride != null && ride._isLevitating);
		canSeeInvisible = HasElement(416);
		base.isHidden = HasElement(415);
		foreach (Condition condition in conditions)
		{
			condition.OnRefresh();
		}
		if (isWet)
		{
			base.isHidden = false;
		}
		SetDirtySpeed();
		if (host != null && !calledRecursive)
		{
			host.Refresh(calledRecursive: true);
		}
	}

	public Chara Duplicate()
	{
		Chara chara = CharaGen.Create(id);
		chara.mana.value = mana.value;
		chara.stamina.value = stamina.value;
		foreach (KeyValuePair<int, Element> item in elements.dict)
		{
			Element orCreateElement = chara.elements.GetOrCreateElement(item.Key);
			orCreateElement.vBase = item.Value.ValueWithoutLink - orCreateElement.vSource;
		}
		chara.SetFaith(faith);
		chara.bio = IO.DeepCopy(bio);
		chara.LV = base.LV;
		chara.c_daysWithGod = base.c_daysWithGod;
		chara.RefreshFaithElement();
		chara.hp = (int)Mathf.Clamp((float)chara.MaxHP * ((float)base.hp / (float)MaxHP) * 0.99f, 0f, chara.MaxHP);
		chara.isCopy = true;
		if (HaveFur())
		{
			chara.c_fur = -1;
		}
		return chara;
	}

	public int GetBurden(Card t = null, int num = -1)
	{
		int num2 = (base.ChildrenWeight + ((t != null) ? ((num == -1) ? t.ChildrenAndSelfWeight : (t.SelfWeight * num)) : 0)) * 100 / WeightLimit;
		if (num2 < 0)
		{
			num2 = 1000;
		}
		if (EClass.debug.ignoreWeight && IsPC)
		{
			num2 = 0;
		}
		int num3 = ((num2 >= 100) ? ((num2 - 100) / 10 + 1) : 0);
		if (num3 > 9)
		{
			num3 = 9;
		}
		return num3;
	}

	public void CalcBurden()
	{
		int num = base.ChildrenWeight * 100 / Mathf.Max(1, WeightLimit);
		if (num < 0)
		{
			num = 1000;
		}
		if (EClass.debug.ignoreWeight && IsPC)
		{
			num = 0;
		}
		burden.Set(num);
		SetDirtySpeed();
	}

	public void Stumble(int mtp = 100)
	{
		bool flag = EClass._map.FindThing((Thing t) => t.IsInstalled && t.pos.Equals(EClass.pc.pos) && t.trait is TraitStairsUp) != null;
		Say(flag ? "dmgBurdenStairs" : "dmgBurdenFallDown", this);
		int num = MaxHP;
		if (Evalue(1421) > 0)
		{
			num = mana.max;
		}
		int num2 = (num * (base.ChildrenWeight * 100 / WeightLimit) / (flag ? 100 : 200) + 1) * mtp / 100;
		if (base.hp <= 0)
		{
			num2 *= 2;
		}
		DamageHP(num2, flag ? AttackSource.BurdenStairs : AttackSource.BurdenFallDown);
	}

	public void SetDirtySpeed()
	{
		dirtySpeed = true;
		if (host != null)
		{
			host.SetDirtySpeed();
		}
	}

	public void RefreshSpeed(Element.BonusInfo info = null)
	{
		if (ride != null && !ride.IsDeadOrSleeping)
		{
			ride.RefreshSpeed();
			_Speed = ride._Speed;
		}
		else if (host != null)
		{
			if (host.ride == this)
			{
				_Speed = Evalue(79);
				_Speed = _Speed * 100 / Mathf.Clamp(100 + _Speed * ((!race.tag.Contains("noRide")) ? 1 : 5) - base.STR - host.EvalueRiding() * 2 - (race.tag.Contains("ride") ? 50 : 0), 100, 1000);
			}
			else
			{
				_Speed = (Evalue(79) + host.Evalue(79)) / 2;
			}
		}
		else
		{
			_Speed = Evalue(79) + Evalue(407) / 2;
		}
		if (body.GetSlot(37, onlyEmpty: false)?.thing != null && HasElement(1209))
		{
			_Speed -= 25;
			info?.AddText(-25, EClass.sources.elements.map[1209].GetName());
		}
		if (parasite != null)
		{
			_Speed = _Speed * 100 / Mathf.Clamp(120 + parasite.LV * 2 - base.STR - Evalue(227) * 2, 100, 1000);
		}
		if (_Speed < elements.ValueWithoutLink(79) / 3)
		{
			_Speed = elements.ValueWithoutLink(79) / 3;
			info?.AddText("minSpeed".lang((elements.ValueWithoutLink(79) / 3).ToString() ?? ""));
		}
		int num = 100;
		if (EClass._zone.map != null && EClass._zone.IsUnderwater)
		{
			int num2 = Evalue(200);
			num = 50 + Mathf.Clamp((int)Mathf.Sqrt(num2) * 5 - EClass._zone.DangerLv / 50, 0, 50) + Mathf.Clamp((int)Mathf.Sqrt(num2), 0, 25);
			if (info != null && num != 100)
			{
				info.AddFix(num - 100, EClass.sources.elements.map[200].GetName().ToTitleCase());
			}
		}
		if (IsPCFaction)
		{
			switch (burden.GetPhase())
			{
			case 1:
				num -= 10;
				info?.AddFix(-10, burden.GetPhaseStr());
				break;
			case 2:
				num -= 20;
				info?.AddFix(-20, burden.GetPhaseStr());
				break;
			case 3:
				num -= 30;
				info?.AddFix(-30, burden.GetPhaseStr());
				break;
			case 4:
				num -= (IsPC ? 50 : 100);
				info?.AddFix(IsPC ? (-50) : (-100), burden.GetPhaseStr());
				break;
			}
			if (IsPC)
			{
				switch (stamina.GetPhase())
				{
				case 1:
					num -= 10;
					info?.AddFix(-10, stamina.GetPhaseStr());
					break;
				case 0:
					num -= 20;
					info?.AddFix(-20, stamina.GetPhaseStr());
					break;
				}
				switch (sleepiness.GetPhase())
				{
				case 2:
					num -= 10;
					info?.AddFix(-10, sleepiness.GetPhaseStr());
					break;
				case 3:
					num -= 20;
					info?.AddFix(-20, sleepiness.GetPhaseStr());
					break;
				}
				switch (hunger.GetPhase())
				{
				case 3:
				case 4:
					num -= 10;
					info?.AddFix(-10, hunger.GetPhaseStr());
					break;
				case 5:
					num -= 30;
					info?.AddFix(-30, hunger.GetPhaseStr());
					break;
				}
				num += EClass.player.lastEmptyAlly * Evalue(1646);
				info?.AddFix(EClass.player.lastEmptyAlly * Evalue(1646), EClass.sources.elements.map[1646].GetName());
			}
			if (IsPCParty && EClass.player.lastEmptyAlly < 0)
			{
				num += EClass.player.lastEmptyAlly * 10 - 10;
				info?.AddFix(EClass.player.lastEmptyAlly * 10 - 10, "exceedParty".lang());
			}
		}
		else if (base.LV >= 1000 && !EClass.game.principal.disableVoidBlessing && !base.IsPCFactionOrMinion)
		{
			num += EClass.curve((base.LV - 900) / 100 * 10, 500, 100);
			info?.AddFix(EClass.curve((base.LV - 900) / 100 * 10, 500, 100), "enemySpeedBuff".lang());
		}
		if (HasCondition<ConGravity>())
		{
			num -= 30;
			info?.AddFix(-30, GetCondition<ConGravity>().Name);
		}
		if (_Speed < 10)
		{
			_Speed = 10;
		}
		_Speed = _Speed * num / 100;
		if (_Speed < 10)
		{
			_Speed = 10;
			info?.AddText("minSpeed".lang(10.ToString() ?? ""));
		}
		dirtySpeed = false;
	}

	public void CalculateMaxStamina()
	{
		int num = base.END;
		int num2 = 0;
		foreach (Element value in elements.dict.Values)
		{
			if (value.source.category == "skill")
			{
				num = ((!IsPC) ? (num + Mathf.Max(value.ValueWithoutLink, 0)) : (num + Mathf.Max(value.vBase, 0)));
			}
		}
		num2 = EClass.curve(num, 30, 10, 60);
		if (num2 < 10)
		{
			num2 = 10;
		}
		_maxStamina = num2 + 15;
	}

	public override void ApplyEditorTags(EditorTag tag)
	{
		switch (tag)
		{
		case EditorTag.HostilityNeutral:
		{
			Hostility hostility2 = (base.c_originalHostility = Hostility.Neutral);
			this.hostility = hostility2;
			break;
		}
		case EditorTag.HostilityEnemy:
		{
			Hostility hostility2 = (base.c_originalHostility = Hostility.Enemy);
			this.hostility = hostility2;
			break;
		}
		case EditorTag.HostilityFriend:
		{
			Hostility hostility2 = (base.c_originalHostility = Hostility.Friend);
			this.hostility = hostility2;
			break;
		}
		case EditorTag.Male:
			bio.SetGender(2);
			base.c_idPortrait = Portrait.GetRandomPortrait(2, GetIdPortraitCat());
			break;
		case EditorTag.Female:
			bio.SetGender(1);
			base.c_idPortrait = Portrait.GetRandomPortrait(1, GetIdPortraitCat());
			break;
		case EditorTag.baby:
			SetFeat(1232);
			break;
		}
		base.ApplyEditorTags(tag);
	}

	public override void SetSource()
	{
		source = EClass.sources.charas.map.TryGetValue(id);
		if (source == null)
		{
			Debug.LogWarning("Chara " + id + " not found");
			id = "chicken";
			source = EClass.sources.charas.map[id];
		}
		path.walker = this;
	}

	public void SetMainElement(string id, int v = 0, bool elemental = false)
	{
		if (!id.StartsWith("ele"))
		{
			id = "ele" + id;
		}
		SetMainElement(EClass.sources.elements.alias[id].id, v, elemental);
	}

	public void SetMainElement(int id, int v = 0, bool elemental = false)
	{
		if (base.c_idMainElement != 0)
		{
			SetElements(base.c_idMainElement, remove: true);
			base.c_idMainElement = 0;
		}
		if (id != 0)
		{
			_ = EClass.sources.elements.map[id];
			SetElements(id, remove: false);
			base.c_idMainElement = id;
			if (elemental)
			{
				base.isElemental = true;
				_colorInt = 0;
				Color colorSprite = EClass.setting.elements[MainElement.source.alias].colorSprite;
				base.c_lightColor = (byte)Mathf.Clamp(colorSprite.r * 3f, 1f, 31f) * 1024 + (byte)Mathf.Clamp(colorSprite.g * 3f, 1f, 31f) * 32 + (byte)Mathf.Clamp(colorSprite.b * 3f, 1f, 31f);
			}
			_ability = null;
		}
		void SetElements(int idEle, bool remove)
		{
			elements.SetBase(idEle, (!remove) ? ((v == 0) ? 10 : v) : 0);
			elements.ModBase(EClass.sources.elements.alias[EClass.sources.elements.map[idEle].aliasRef].id, remove ? (-20) : 20);
			switch (idEle)
			{
			case 910:
				elements.ModBase(951, remove ? 10 : (-10));
				break;
			case 911:
				elements.ModBase(950, remove ? 10 : (-10));
				break;
			case 912:
				elements.ModBase(953, remove ? 10 : (-10));
				break;
			case 913:
				elements.ModBase(952, remove ? 10 : (-10));
				break;
			case 916:
				elements.ModBase(960, remove ? 10 : (-10));
				break;
			case 919:
				elements.ModBase(956, remove ? 10 : (-10));
				break;
			case 925:
				elements.ModBase(962, remove ? 10 : (-10));
				break;
			case 922:
				elements.ModBase(965, remove ? 10 : (-10));
				break;
			case 921:
				elements.ModBase(971, remove ? 10 : (-10));
				break;
			case 926:
				elements.ModBase(961, remove ? 10 : (-10));
				break;
			case 914:
			case 915:
			case 917:
			case 918:
			case 920:
			case 923:
			case 924:
				break;
			}
		}
	}

	public void ApplyJob(bool remove = false)
	{
		elements.ApplyElementMap(base.uid, SourceValueType.Chara, job.elementMap, base.DefaultLV, remove, applyFeat: true);
		if (IsPCC)
		{
			EClass.game.uniforms.Apply(pccData, job.id, base.IsMale, canUseOtherGender: true);
		}
	}

	public void ChangeJob(string idNew)
	{
		ApplyJob(remove: true);
		base.c_idJob = idNew;
		_job = null;
		ApplyJob();
		if (IsPCC)
		{
			PCC.Get(pccData).Build();
		}
	}

	private int ParseBodySlot(string s)
	{
		return s switch
		{
			"頭" => 30, 
			"首" => 31, 
			"体" => 32, 
			"背" => 33, 
			"手" => 35, 
			"指" => 36, 
			"腕" => 34, 
			"腰" => 37, 
			"足" => 39, 
			_ => -1, 
		};
	}

	public void AddRandomBodyPart(bool msg = false)
	{
		int ele = new int[9] { 30, 31, 33, 35, 35, 36, 34, 37, 39 }.RandomItem();
		body.AddBodyPart(ele);
		if (msg)
		{
			Say("gain_bodyparts", this, Element.Get(ele).GetName().ToLower());
			PlaySound("offering");
		}
	}

	public void RemoveLastBodyPart(bool msg = false)
	{
		if (body.slots.Count != 0)
		{
			BodySlot bodySlot = body.slots.LastItem();
			body.RemoveBodyPartAt(body.slots.Count - 1);
			if (msg)
			{
				Say("lose_bodyparts", this, Element.Get(bodySlot.elementId).GetName().ToLower());
				PlaySound("offering");
			}
		}
	}

	public void ResetBody()
	{
		for (int num = body.slots.Count - 1; num >= 0; num--)
		{
			BodySlot bodySlot = body.slots[num];
			if (bodySlot.elementId == 45 || bodySlot.elementId == 40)
			{
				return;
			}
			body.RemoveBodyPart(num);
		}
		string[] array = race.figure.Split('|');
		foreach (string s in array)
		{
			int num2 = ParseBodySlot(s);
			if (num2 != -1)
			{
				body.AddBodyPart(num2);
			}
		}
		body.RefreshBodyParts();
	}

	public void ApplyRace(bool remove = false)
	{
		string[] array = race.figure.Split('|');
		foreach (string s in array)
		{
			int num = ParseBodySlot(s);
			if (num != -1)
			{
				if (remove)
				{
					body.RemoveBodyPart(num);
				}
				else
				{
					body.AddBodyPart(num);
				}
			}
		}
		body.RefreshBodyParts();
		elements.ApplyElementMap(base.uid, SourceValueType.Chara, race.elementMap, base.DefaultLV, remove, applyFeat: true);
	}

	public void ChangeRace(string idNew)
	{
		ApplyRace(remove: true);
		base.c_idRace = idNew;
		_race = null;
		ApplyRace();
		ChangeMaterial(race.material);
	}

	public void MakePartyMemeber()
	{
		_MakeAlly();
		EClass.pc.party.AddMemeber(this);
	}

	public void MakeAlly(bool msg = true)
	{
		if (IsLocalChara && !base.IsUnique)
		{
			Debug.Log("Creating Replacement NPC for:" + this);
			EClass._map.deadCharas.Add(CreateReplacement());
		}
		_MakeAlly();
		if (msg)
		{
			EClass.pc.Say("hire", this);
			EClass.Sound.Play("good");
		}
		EClass.pc.party.AddMemeber(this);
		if (EClass._zone.IsTown)
		{
			EClass._zone.RefreshListCitizen();
		}
	}

	public void _MakeAlly()
	{
		if (EClass.pc.homeBranch != null)
		{
			EClass.pc.homeBranch.AddMemeber(this);
		}
		else
		{
			SetGlobal();
			SetFaction(EClass.Home);
		}
		Hostility hostility2 = (base.c_originalHostility = Hostility.Ally);
		this.hostility = hostility2;
		orgPos = null;
		base.c_summonDuration = 0;
		base.isSummon = false;
		ReleaseMinion();
		SetInt(32);
		Refresh();
	}

	public bool CanBeTempAlly(Chara c)
	{
		if (IsPCFaction || IsGlobal || IsMinion || IsMultisize || EClass._zone.CountMinions(c) > c.MaxSummon || base.rarity >= Rarity.Legendary)
		{
			return false;
		}
		if (HasElement(1222))
		{
			return false;
		}
		return true;
	}

	public void MakeMinion(Chara _master, MinionType type = MinionType.Default)
	{
		ReleaseMinion();
		Hostility hostility2 = (base.c_originalHostility = (_master.IsPCFaction ? Hostility.Ally : _master.hostility));
		this.hostility = hostility2;
		base.c_uidMaster = _master.uid;
		base.c_minionType = type;
		master = _master;
		Refresh();
	}

	public void ReleaseMinion()
	{
		base.c_uidMaster = 0;
		master = null;
		enemy = null;
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.enemy == this)
			{
				chara.SetEnemy();
				chara.ai.Cancel();
			}
		}
		ai.Cancel();
		Refresh();
	}

	public void SetSummon(int duration)
	{
		base.c_summonDuration = duration;
		base.isSummon = true;
	}

	public Chara FindMaster()
	{
		if (IsMinion)
		{
			master = EClass._map.FindChara(base.c_uidMaster);
		}
		return master;
	}

	public bool IsEscorted()
	{
		if (!IsPCPartyMinion)
		{
			return false;
		}
		foreach (Quest item in EClass.game.quests.list)
		{
			if (item is QuestEscort questEscort && questEscort.uidChara == base.uid)
			{
				return true;
			}
		}
		return false;
	}

	public bool CanDestroyPath()
	{
		if (!IsMultisize)
		{
			if (base.rarity >= Rarity.Superior && !base.IsPCFactionOrMinion && ai is GoalCombat && !EClass._zone.IsPCFaction)
			{
				return !(EClass._zone is Zone_Town);
			}
			return false;
		}
		return true;
	}

	public bool CanMoveTo(Point p, bool allowDestroyPath = true)
	{
		if (!p.IsValid)
		{
			return false;
		}
		int num = ((p.z >= pos.z) ? ((p.x > pos.x) ? 1 : ((p.z > pos.z) ? 2 : 3)) : 0);
		if (allowDestroyPath && CanDestroyPath())
		{
			if (!p.IsInBounds)
			{
				return false;
			}
		}
		else
		{
			if (EClass._map.cells[p.x, p.z].blocked || EClass._map.cells[pos.x, pos.z].weights[num] == 0)
			{
				return false;
			}
			if (p.x != pos.x && p.z != pos.z)
			{
				Cell[,] cells = EClass._map.cells;
				int x = p.x;
				int z = pos.z;
				int num2 = ((z >= pos.z) ? ((x > pos.x) ? 1 : ((z > pos.z) ? 2 : 3)) : 0);
				if (cells[pos.x, pos.z].weights[num2] == 0)
				{
					return false;
				}
				if (cells[x, z].blocked)
				{
					return false;
				}
				num2 = ((z >= p.z) ? ((x > p.x) ? 1 : ((z > p.z) ? 2 : 3)) : 0);
				if (cells[p.x, p.z].weights[num2] == 0)
				{
					return false;
				}
				x = pos.x;
				z = p.z;
				num2 = ((z >= pos.z) ? ((x > pos.x) ? 1 : ((z > pos.z) ? 2 : 3)) : 0);
				if (cells[pos.x, pos.z].weights[num2] == 0)
				{
					return false;
				}
				if (cells[x, z].blocked)
				{
					return false;
				}
				num2 = ((z >= p.z) ? ((x > p.x) ? 1 : ((z > p.z) ? 2 : 3)) : 0);
				if (cells[p.x, p.z].weights[num2] == 0)
				{
					return false;
				}
			}
		}
		if (IsPC)
		{
			if (IsEnemyOnPath(p))
			{
				return false;
			}
		}
		else if (p.HasChara && !IsMultisize && !CanReplace(p.FirstChara))
		{
			return false;
		}
		return true;
	}

	public bool IsEnemyOnPath(Point p, bool cancelAI = true)
	{
		if (!currentZone.IsRegion && p.IsValid)
		{
			CellDetail detail = p.detail;
			if (detail != null && detail.charas.Count > 0)
			{
				foreach (Chara chara in p.detail.charas)
				{
					if (chara.IsHostile(this) || chara.IsHostile() || !chara.trait.CanBePushed)
					{
						if (cancelAI && EClass.pc.ai is GoalManualMove)
						{
							EClass.pc.ai.Cancel();
						}
						return true;
					}
				}
			}
		}
		return false;
	}

	public bool CanInteractTo(Card c)
	{
		return CanInteractTo(c.pos);
	}

	public bool CanInteractTo(Point p)
	{
		if (!p.IsValid)
		{
			return false;
		}
		if (EClass._map.cells[p.x, p.z].blocked)
		{
			return true;
		}
		if (!IsMultisize)
		{
			int num = ((p.z >= pos.z) ? ((p.x > pos.x) ? 1 : ((p.z > pos.z) ? 2 : 3)) : 0);
			if (EClass._map.cells[pos.x, pos.z].weights[num] == 0)
			{
				return false;
			}
			if (p.x != pos.x && p.z != pos.z)
			{
				Cell[,] cells = EClass._map.cells;
				int x = p.x;
				int z = pos.z;
				int num2 = ((z >= pos.z) ? ((x > pos.x) ? 1 : ((z > pos.z) ? 2 : 3)) : 0);
				if (cells[pos.x, pos.z].weights[num2] == 0)
				{
					return false;
				}
				num2 = ((z >= p.z) ? ((x > p.x) ? 1 : ((z > p.z) ? 2 : 3)) : 0);
				if (cells[p.x, p.z].weights[num2] == 0)
				{
					return false;
				}
				x = pos.x;
				z = p.z;
				num2 = ((z >= pos.z) ? ((x > pos.x) ? 1 : ((z > pos.z) ? 2 : 3)) : 0);
				if (cells[pos.x, pos.z].weights[num2] == 0)
				{
					return false;
				}
				num2 = ((z >= p.z) ? ((x > p.x) ? 1 : ((z > p.z) ? 2 : 3)) : 0);
				if (cells[p.x, p.z].weights[num2] == 0)
				{
					return false;
				}
			}
		}
		return true;
	}

	public Point GetFirstStep(Point newPoint, PathManager.MoveType moveType = PathManager.MoveType.Default)
	{
		return PathManager.Instance.GetFirstStep(pos, newPoint, this, IsPC ? 40 : 10, (!IsMultisize) ? moveType : PathManager.MoveType.Default);
	}

	public bool MoveRandom()
	{
		Point randomNeighbor = pos.GetRandomNeighbor();
		if (!randomNeighbor.Equals(pos) && !randomNeighbor.HasChara && HasAccess(randomNeighbor))
		{
			if ((race.tag.Contains("water") || source.tag.Contains("water")) && pos.cell.IsTopWaterAndNoSnow && !randomNeighbor.IsDeepWater)
			{
				return false;
			}
			return TryMove(randomNeighbor) == MoveResult.Success;
		}
		return false;
	}

	public bool MoveNeighborDefinitely()
	{
		List<Point> list = new List<Point>();
		pos.ForeachNeighbor(delegate(Point p)
		{
			list.Add(p.Copy());
		});
		list.Shuffle();
		foreach (Point item in list)
		{
			if (!item.Equals(pos) && !item.HasChara && TryMove(item) == MoveResult.Success)
			{
				return true;
			}
		}
		return false;
	}

	public void MoveByForce(Point newPoint, Card c = null, bool checkWall = false)
	{
		if (!newPoint.sourceBlock.tileType.IsBlockPass && (!checkWall || (Dist(newPoint) <= 1 && CanMoveTo(newPoint, allowDestroyPath: false))) && _Move(newPoint, MoveType.Force) == MoveResult.Success && ai.Current.CancelWhenMoved)
		{
			ai.Current.TryCancel(c);
		}
	}

	public MoveResult TryMoveTowards(Point p)
	{
		if (p.Equals(pos))
		{
			return MoveResult.Success;
		}
		if (IsPC && EClass.player.TooHeavyToMove())
		{
			return MoveResult.Fail;
		}
		bool flag = true;
		Point point = null;
		_sharedPos.Set(p);
		if (CanDestroyPath())
		{
			if (TryMove(pos.GetPointTowards(_sharedPos)) == MoveResult.Success)
			{
				return MoveResult.Success;
			}
			Debug.Log(CanMoveTo(pos));
			Debug.Log(pos.GetPointTowards(_sharedPos));
		}
		int num = pos.Distance(p);
		PathProgress pathProgress = PathManager.Instance.RequestPathImmediate(pos, p, this, PathManager.MoveType.Default, num + 4, 1);
		if (pathProgress.HasPath && pathProgress.nodes.Count < num + 4)
		{
			PathFinderNode pathFinderNode = pathProgress.nodes[pathProgress.nodes.Count - 1];
			if (TryMove(new Point(pathFinderNode.X, pathFinderNode.Z)) == MoveResult.Success)
			{
				return MoveResult.Success;
			}
		}
		pathProgress = PathManager.Instance.RequestPathImmediate(pos, p, this, PathManager.MoveType.Combat, num + 4, 1);
		if (pathProgress.HasPath && pathProgress.nodes.Count < num + 4)
		{
			PathFinderNode pathFinderNode2 = pathProgress.nodes[pathProgress.nodes.Count - 1];
			if (TryMove(new Point(pathFinderNode2.X, pathFinderNode2.Z)) == MoveResult.Success)
			{
				return MoveResult.Success;
			}
		}
		pathProgress = PathManager.Instance.RequestPathImmediate(pos, p, this, PathManager.MoveType.Default, num + 25, 2);
		if (pathProgress.HasPath && pathProgress.nodes.Count < num + 25)
		{
			PathFinderNode pathFinderNode3 = pathProgress.nodes[pathProgress.nodes.Count - 1];
			if (TryMove(new Point(pathFinderNode3.X, pathFinderNode3.Z)) == MoveResult.Success)
			{
				return MoveResult.Success;
			}
		}
		pathProgress = PathManager.Instance.RequestPathImmediate(pos, p, this, PathManager.MoveType.Combat, num + 25, 2);
		if (pathProgress.HasPath && pathProgress.nodes.Count < num + 25)
		{
			PathFinderNode pathFinderNode4 = pathProgress.nodes[pathProgress.nodes.Count - 1];
			if (TryMove(new Point(pathFinderNode4.X, pathFinderNode4.Z)) == MoveResult.Success)
			{
				return MoveResult.Success;
			}
		}
		return MoveResult.Fail;
	}

	public MoveResult TryMoveFrom(Point p)
	{
		if (IsPC && EClass.player.TooHeavyToMove())
		{
			return MoveResult.Fail;
		}
		Point point = p.Copy();
		int num = p.x - pos.x;
		int num2 = p.z - pos.z;
		if (num > 1)
		{
			num = 1;
		}
		else if (num < -1)
		{
			num = -1;
		}
		if (num2 > 1)
		{
			num2 = 1;
		}
		else if (num2 < -1)
		{
			num2 = -1;
		}
		if (num == 0 && num2 == 0)
		{
			num = EClass.rnd(3) - 1;
			num2 = EClass.rnd(3) - 1;
		}
		point.Set(pos);
		point.x -= num;
		point.z -= num2;
		if (point.IsValid && !point.HasChara)
		{
			return TryMove(point, allowDestroyPath: false);
		}
		return MoveResult.Fail;
	}

	public MoveResult TryMove(Point newPoint, bool allowDestroyPath = true)
	{
		foreach (Condition condition in conditions)
		{
			if (!condition.TryMove(newPoint))
			{
				return MoveResult.Fail;
			}
		}
		if (base.isRestrained)
		{
			if (!IsPC)
			{
				return MoveResult.Fail;
			}
			base.isRestrained = false;
		}
		if (!CanMoveTo(newPoint, allowDestroyPath))
		{
			return MoveResult.Fail;
		}
		return _Move(newPoint);
	}

	public override MoveResult _Move(Point newPoint, MoveType type = MoveType.Walk)
	{
		if (isDead)
		{
			return MoveResult.Fail;
		}
		if (IsPC)
		{
			float num = EClass.setting.defaultActPace;
			switch (burden.GetPhase())
			{
			case 3:
				num *= 1.5f;
				break;
			case 4:
				num *= 2f;
				break;
			}
			if (currentZone.IsRegion)
			{
				int num2 = 30;
				if (!EClass.pc.HasElement(408))
				{
					switch (EClass.world.weather.CurrentCondition)
					{
					case Weather.Condition.Rain:
						num2 += 5;
						break;
					case Weather.Condition.RainHeavy:
						num2 += 10;
						num *= 1.5f;
						break;
					case Weather.Condition.Snow:
						num2 += 10;
						break;
					case Weather.Condition.SnowHeavy:
						num2 += 15;
						num *= 1.5f;
						break;
					}
				}
				if (newPoint.matFloor.id == 48)
				{
					num2 += 20;
					num *= 1.8f;
				}
				num2 = num2 * 100 / (100 + EvalueMax(240, -20) + EvalueMax(407, -5) * 5);
				EClass.world.date.AdvanceMin(num2 * 6);
				EClass.player.lastZonePos = null;
				EClass.player.distanceTravel++;
				int num3 = base.hp;
				for (int i = 0; i < num2 * 4; i++)
				{
					EClass.pc.party.members.ForeachReverse(delegate(Chara m)
					{
						if (!m.isDead)
						{
							m.TickConditions();
						}
					});
					if (base.hp < MaxHP / 5 && base.hp < num3 && !EClass.player.regionMoveWarned)
					{
						EClass.player.regionMoveWarned = true;
						Msg.Say("regionAbortMove");
						EInput.Consume(consumeAxis: true);
						SetAI(new NoGoal());
						return MoveResult.Fail;
					}
				}
				if (newPoint.cell.CanSuffocate())
				{
					AddCondition<ConSuffocation>((EClass.pc.Evalue(200) != 0) ? (2000 / (100 + EvalueMax(200, -5) * 10)) : 30, force: true);
					int num4 = GetCondition<ConSuffocation>()?.GetPhase() ?? 0;
					if (num4 >= 2)
					{
						DamageHP(EClass.rndHalf(10 + MaxHP / 5), AttackSource.Condition);
					}
					if (!isDead && !HasElement(429))
					{
						ModExp(200, 1 + num4 * 12);
					}
				}
				EClass.player.regionMoveWarned = false;
				if (isDead)
				{
					return MoveResult.Fail;
				}
			}
			if (num > EClass.setting.defaultActPace * 3f)
			{
				num = EClass.setting.defaultActPace * 3f;
			}
			actTime = num;
		}
		if (IsPCFaction && EClass.rnd(5) == 0 && newPoint.cell.CanSuffocate())
		{
			ModExp(200, EClass._zone.IsRegion ? 50 : 5);
			if (ride != null)
			{
				ride.ModExp(200, EClass._zone.IsRegion ? 50 : 5);
			}
			if (parasite != null)
			{
				parasite.ModExp(200, EClass._zone.IsRegion ? 50 : 5);
			}
		}
		Chara chara = ((ride == null) ? this : ride);
		if (!EClass._zone.IsRegion || chara.IsPC)
		{
			bool flag = (chara.isConfused && EClass.rnd(2) == 0) || (chara.isDrunk && EClass.rnd(IsIdle ? 2 : 8) == 0 && !chara.HasElement(1215));
			if (host != null && host.ride == this && ((host.isConfused && EClass.rnd(2) == 0) || (host.isDrunk && EClass.rnd(IsIdle ? 2 : 8) == 0 && !host.HasElement(1215))))
			{
				flag = true;
			}
			if (flag && newPoint.Distance(pos) <= 1)
			{
				Point randomNeighbor = pos.GetRandomNeighbor();
				if (CanMoveTo(randomNeighbor, allowDestroyPath: false))
				{
					newPoint = randomNeighbor;
					if (isDrunk)
					{
						Talk("drunk");
					}
				}
			}
		}
		if (newPoint.x != pos.x || newPoint.z != pos.z)
		{
			LookAt(newPoint);
		}
		CellEffect effect = base.Cell.effect;
		if (effect != null && effect.id == 7)
		{
			CellEffect effect2 = base.Cell.effect;
			if (race.height < 500 && !race.tag.Contains("webfree") && EClass.rnd(effect2.power + 25) > EClass.rnd(base.STR + base.DEX + 1))
			{
				Say("abWeb_caught", this);
				PlaySound("web");
				effect2.power = effect2.power * 3 / 4;
				renderer.PlayAnime(AnimeID.Shiver);
				return MoveResult.Fail;
			}
			Say("abWeb_pass", this);
			EClass._map.SetEffect(base.Cell.x, base.Cell.z);
		}
		if (IsPC)
		{
			if (EClass._zone.IsRegion)
			{
				actTime *= EClass.setting.render.anime.regionSpeed;
			}
			else if ((newPoint.x > pos.x && newPoint.z > pos.z) || (newPoint.x < pos.x && newPoint.z < pos.z))
			{
				actTime += actTime * EClass.setting.render.anime.diagonalSpeed;
			}
		}
		if (newPoint.cell.hasDoor)
		{
			foreach (Thing thing in pos.Things)
			{
				if (thing.trait is TraitDoor traitDoor && traitDoor.owner.c_lockLv > 0)
				{
					if (base.INT < 10)
					{
						return MoveResult.Fail;
					}
					traitDoor.TryOpenLock(this);
					return MoveResult.Door;
				}
			}
		}
		Cell cell = newPoint.cell;
		Cell cell2 = pos.cell;
		bool flag2 = cell.HasLiquid && !IsLevitating;
		bool hasBridge = cell.HasBridge;
		bool hasRamp = cell.HasRamp;
		bool flag3 = EClass._zone.IsSnowCovered && !cell.HasRoof && !cell.isClearSnow;
		TileRow tileRow = (hasRamp ? ((TileRow)cell.sourceBlock) : ((TileRow)(hasBridge ? cell.sourceBridge : cell.sourceFloor)));
		SourceMaterial.Row row = (hasRamp ? cell.matBlock : (hasBridge ? cell.matBridge : cell.matFloor));
		bool flag4 = cell.IsTopWater && !cell.isFloating;
		if (!EClass._zone.IsRegion)
		{
			if (cell.hasDoorBoat)
			{
				tileRow = FLOOR.sourceWood;
				row = MATERIAL.sourceOak;
				flag4 = false;
			}
			else if (flag3 && !tileRow.ignoreSnow)
			{
				if (tileRow.tileType.IsWater)
				{
					tileRow = FLOOR.sourceIce;
					row = MATERIAL.sourceIce;
				}
				else
				{
					tileRow = FLOOR.sourceSnow;
					row = MATERIAL.sourceSnow;
				}
				flag4 = false;
			}
		}
		if ((pos.sourceFloor.isBeach || cell2.IsSnowTile) && !pos.HasObj)
		{
			EClass._map.SetFoormark(pos, 1, (int)Util.GetAngle(pos.x - newPoint.x, pos.z - newPoint.z), cell2.IsSnowTile ? 312 : 304);
		}
		if (isSynced)
		{
			string text = ((flag2 || flag4) ? "water" : tileRow.soundFoot.IsEmpty(row.soundFoot.IsEmpty("default")));
			if (cell.obj != 0 && cell.sourceObj.tileType.IsPlayFootSound && !cell.matObj.soundFoot.IsEmpty())
			{
				text = cell.matObj.soundFoot;
			}
			if (IsPC)
			{
				foreach (Thing thing2 in newPoint.Things)
				{
					if (thing2.IsInstalled && (thing2.trait.IsChangeFloorHeight || thing2.Pref.Surface))
					{
						text = thing2.material.soundFoot;
					}
				}
			}
			if (!text.IsEmpty())
			{
				SoundManager.altLastData = IsPC;
				PlaySound("Footstep/" + text, IsPC ? 1f : 0.9f);
			}
			if (!flag4)
			{
				Scene scene = EClass.scene;
				PCOrbit pcOrbit = EClass.screen.pcOrbit;
				bool flag5 = scene.actionMode.gameSpeed > 1f;
				scene.psFoot.transform.position = renderer.position + pcOrbit.footPos;
				scene.psFoot.startColor = row.matColor;
				scene.psFoot.Emit(pcOrbit.emitFoot * ((!flag5) ? 1 : 2));
				if (flag5 && IsPC)
				{
					scene.psSmoke.transform.position = renderer.position + pcOrbit.smokePos;
					scene.psSmoke.Emit(pcOrbit.emitSmoke);
				}
			}
			if (flag2 || flag4)
			{
				Effect.Get("ripple").Play(0.4f * actTime * EClass.scene.actionMode.gameSpeed, newPoint);
			}
		}
		lastPos.Set(pos);
		if (type != MoveType.Force)
		{
			if (newPoint.HasChara && ai.Current.PushChara)
			{
				TryPush(newPoint);
			}
			if (newPoint.HasChara && newPoint.Charas.Count == 1)
			{
				Chara chara2 = newPoint.Charas[0];
				if (CanReplace(chara2))
				{
					chara2.MoveByForce(lastPos, this);
					if (chara.IsPC)
					{
						Say("replace_pc", chara, chara2);
					}
				}
			}
		}
		if (cell.hasDoor)
		{
			foreach (Thing thing3 in newPoint.Things)
			{
				if (thing3.trait is TraitDoor traitDoor2)
				{
					traitDoor2.TryOpen(this);
				}
			}
		}
		EClass._zone.map.MoveCard(newPoint, this);
		SyncRide();
		if (IsPC && EClass._zone.PetFollow && (!EClass._zone.KeepAllyDistance || !EClass.game.config.tactics.allyKeepDistance) && !EClass._zone.IsRegion)
		{
			foreach (Chara member in EClass.pc.party.members)
			{
				if (member.isLeashed && !member.IsPC && member.host == null && !member.IsDisabled && !member.HasCondition<ConEntangle>() && !member.IsInCombat && member.Dist(EClass.pc) > 1)
				{
					member.TryMoveTowards(EClass.pc.pos);
				}
			}
		}
		if (EClass.core.config.test.animeFramePCC == 0 && isSynced && renderer.hasActor && renderer.actor.isPCC)
		{
			renderer.NextFrame();
		}
		if (IsPC)
		{
			PlaySound("Footstep/Extra/pcfootstep");
			if (pos.HasThing)
			{
				foreach (Card item in pos.ListCards())
				{
					if (!item.isThing || item.placeState != 0 || item.ignoreAutoPick)
					{
						continue;
					}
					if (EClass.core.config.game.advancedMenu)
					{
						Window.SaveData dataPick = EClass.player.dataPick;
						ContainerFlag containerFlag = item.category.GetRoot().id.ToEnum<ContainerFlag>();
						if (containerFlag == ContainerFlag.none)
						{
							containerFlag = ContainerFlag.other;
						}
						if ((dataPick.noRotten && item.IsDecayed) || (dataPick.onlyRottable && item.trait.Decay == 0))
						{
							continue;
						}
						if (dataPick.userFilter)
						{
							switch (dataPick.IsFilterPass(item.GetName(NameStyle.Full, 1)))
							{
							case Window.SaveData.FilterResult.PassWithoutFurtherTest:
								Pick(item.Thing);
								continue;
							case Window.SaveData.FilterResult.Block:
								continue;
							}
						}
						if (dataPick.advDistribution)
						{
							foreach (int cat in dataPick.cats)
							{
								if (item.category.uid == cat)
								{
									Pick(item.Thing);
								}
							}
						}
						else if (!dataPick.flag.HasFlag(containerFlag))
						{
							Pick(item.Thing);
						}
					}
					else
					{
						Pick(item.Thing);
					}
				}
			}
			if (EClass._zone.IsRegion)
			{
				EloMap.Cell cell3 = EClass.scene.elomap.GetCell(EClass.pc.pos);
				if (cell3?.zone != null && !cell3.zone.HiddenInRegionMap && (!(cell3.zone is Zone_Field) || cell3.zone.children.Count > 0 || cell3.zone.IsPCFaction))
				{
					Msg.Say((!cell3.zone.source.GetText("textFlavor").IsEmpty()) ? cell3.zone.source.GetText("textFlavor") : (cell3.zone.ShowDangerLv ? "seeZoneDanger" : "seeZone"), cell3.zone.Name, cell3.zone.DangerLv.ToString() ?? "");
				}
				if (pos.matFloor.alias == "snow" && EClass.rnd(3) == 0)
				{
					Msg.SetColor(Msg.colors.Ono);
					Msg.Say(Lang.GetList("walk_snow").RandomItem());
				}
				else if (EClass.world.weather.CurrentCondition == Weather.Condition.RainHeavy && EClass.rnd(3) == 0)
				{
					Msg.SetColor(Msg.colors.Ono);
					Msg.Say(Lang.GetList("walk_storm").RandomItem());
				}
			}
			ActWait.Search(EClass.pc);
		}
		if (IsPCC)
		{
			int num5 = Mathf.Abs(cell2.topHeight - cell.topHeight);
			if ((num5 >= 3 && lastPos.sourceBlock.tileType.slopeHeight == 0 && newPoint.sourceBlock.tileType.slopeHeight == 0) || cell2.hasDoorBoat || cell.hasDoorBoat)
			{
				renderer.PlayAnime((cell2.topHeight >= cell.topHeight) ? AnimeID.JumpDown : ((num5 >= 6) ? AnimeID.Jump : AnimeID.JumpSmall));
			}
			else
			{
				float surfaceHeight = cell2.GetSurfaceHeight();
				float surfaceHeight2 = cell.GetSurfaceHeight();
				num5 = (int)Mathf.Abs((surfaceHeight - surfaceHeight2) * 100f);
				if (num5 >= 15)
				{
					renderer.PlayAnime((surfaceHeight >= surfaceHeight2) ? AnimeID.JumpDown : ((num5 >= 40) ? AnimeID.Jump : AnimeID.JumpSmall));
				}
			}
		}
		lastPos.Things.ForeachReverse(delegate(Thing t)
		{
			t.trait.OnSteppedOut(this);
		});
		if (!IsPC)
		{
			pos.Things.ForeachReverse(delegate(Thing t)
			{
				t.trait.OnStepped(this);
			});
		}
		if (CanDestroyPath())
		{
			DestroyPath(pos);
		}
		if (IsPC)
		{
			if (renderer.anime == null && renderer.replacer != null)
			{
				renderer.PlayAnime(AnimeID.Hop);
			}
			if (EClass.player.flags.isShoesOff)
			{
				if (!FLOOR.IsTatami(pos.cell.sourceSurface.id) && pos.cell.room == null)
				{
					EClass.player.flags.isShoesOff = false;
					EClass.pc.Say("shoes_on", EClass.pc);
					EClass.pc.SetPCCState(PCCState.Normal);
				}
			}
			else if (FLOOR.IsTatami(pos.cell.sourceSurface.id) && pos.cell.room != null)
			{
				EClass.player.flags.isShoesOff = true;
				EClass.pc.Say("shoes_off", EClass.pc);
				EClass.pc.SetPCCState(PCCState.ShoesOff);
			}
		}
		hasMovedThisTurn = true;
		return MoveResult.Success;
	}

	public void DestroyPath(Point pos)
	{
		bool broke = false;
		pos.ForeachMultiSize(base.W, base.H, delegate(Point _p, bool main)
		{
			if (_p.IsValid)
			{
				if (_p.HasBlock)
				{
					EClass._map.MineBlock(_p, recoverBlock: false, this);
					if (_p.HasObj)
					{
						EClass._map.MineObj(_p, null, this);
					}
					broke = true;
				}
				if (_p.HasObj && _p.IsBlocked)
				{
					EClass._map.MineObj(_p, null, this);
					broke = true;
				}
				_p.Things.ForeachReverse(delegate(Thing t)
				{
					if (t.IsInstalled && (t.trait.IsBlockPath || t.trait.IsDoor))
					{
						if (t.isNPCProperty && t.trait.CanBeDestroyed)
						{
							t.Destroy();
						}
						else if (!t.IsUnique && !t.trait.CanBeDestroyed)
						{
							t.SetPlaceState(PlaceState.roaming);
						}
					}
				});
			}
		});
		if (broke)
		{
			Msg.Say("stomp");
			Shaker.ShakeCam("stomp");
		}
	}

	public void TryPush(Point point)
	{
		List<Chara> list = point.ListCharas();
		if (list.Count == 0)
		{
			return;
		}
		list.Copy().ForeachReverse(delegate(Chara c)
		{
			if (!c.ai.IsMoveAI && !c.IsPC && c.trait.CanBePushed && c != this && !c.noMove && (!EClass._zone.IsRegion || c.IsPCFactionOrMinion))
			{
				List<Point> list2 = new List<Point>();
				for (int i = point.x - 1; i <= point.x + 1; i++)
				{
					for (int j = point.z - 1; j <= point.z + 1; j++)
					{
						if (i != point.x || j != point.z)
						{
							Point point2 = new Point(i, j);
							if (point2.IsValid && !point2.HasChara && !point2.IsBlocked && !point2.cell.hasDoor && !point2.IsBlockByHeight(point))
							{
								list2.Add(point2);
							}
						}
					}
				}
				if (list2.Count > 0)
				{
					if (list2.Count > 1)
					{
						list2.ForeachReverse(delegate(Point p)
						{
							if (p.Equals(new Point(point.x + point.x - pos.x, point.z + point.z - pos.z)))
							{
								list2.Remove(p);
							}
						});
					}
					Point newPoint = list2.RandomItem();
					if (IsPC)
					{
						Say("displace", this, c);
						PlaySound("push");
					}
					else if (c.isSynced)
					{
						c.PlayEffect("push");
					}
					c.MoveByForce(newPoint, this, checkWall: true);
					if (!c.IsPC && c.trait is TraitRogue && EClass.rnd(3) == 0 && GetCurrency() > 20 && !HasElement(426))
					{
						int num = EClass.rndHalf(10 + Mathf.Min(GetCurrency() / 100, 1000));
						c.Talk("pushed");
						Say("pushed_money", this, c);
						ModCurrency(-num);
						c.ModCurrency(num);
						c.PlaySound("steal_money");
					}
					else if (IsPC && !c.IsPCParty && !c.IsUnique && c.IsHumanSpeak && EClass.rnd(5) == 0)
					{
						c.Talk("pushed");
					}
				}
			}
		});
	}

	public bool CanReplace(Chara c)
	{
		if (c.IsMultisize || !c.trait.CanBePushed || c.isRestrained || c.noMove || IsHostile(c) || IsMinion || (EClass._zone.IsRegion && !c.IsPCFactionOrMinion))
		{
			return false;
		}
		if (IsPC)
		{
			return true;
		}
		if (c.IsPC || c.pos.Equals(EClass.pc.pos) || c.host != null)
		{
			return false;
		}
		if (!IsHostile(c))
		{
			if (c.c_uidMaster != 0 || c.isSummon || base.IsPowerful || IsEscorted())
			{
				return true;
			}
			if (DestDist < c.DestDist)
			{
				return true;
			}
			if (IsPCParty && !c.IsPCParty)
			{
				return true;
			}
			if (IsPCFaction && !c.IsPCParty)
			{
				return true;
			}
		}
		return false;
	}

	public void MoveZone(string alias)
	{
		MoveZone(EClass.game.spatials.Find(alias));
	}

	public void MoveZone(Zone z, ZoneTransition.EnterState state = ZoneTransition.EnterState.Auto)
	{
		MoveZone(z, new ZoneTransition
		{
			state = state
		});
	}

	public void MoveZone(Zone z, ZoneTransition transition)
	{
		if (z == currentZone)
		{
			return;
		}
		if (HasCondition<ConInvulnerable>())
		{
			RemoveCondition<ConInvulnerable>();
		}
		if (IsPC)
		{
			EClass.player.nextZone = z;
			if (IsInActiveZone && !EClass.player.simulatingZone)
			{
				TryDropCarryOnly();
				if (z.instance == null && currentZone.instance == null && !EClass.player.deathZoneMove && !EClass.pc.isDead && (z.IsPCFaction || z.WillAutoSave) && z.GetTopZone() != EClass._zone.GetTopZone())
				{
					if (EClass.player.returnInfo != null)
					{
						EClass.player.returnInfo.turns += 5;
					}
					if (!EClass.debug.ignoreAutoSave)
					{
						EClass.game.Save(isAutoSave: true);
					}
				}
				EClass.player.deathZoneMove = false;
			}
			currentZone.events.OnLeaveZone();
			if (currentZone.instance != null)
			{
				ZoneInstance instance = currentZone.instance;
				z = EClass.game.spatials.Find(instance.uidZone) ?? EClass.pc.homeZone;
				transition = new ZoneTransition
				{
					state = instance.ReturnState,
					x = instance.x,
					z = instance.z
				};
				instance.OnLeaveZone();
				Debug.Log(z);
			}
			EInput.Consume(consumeAxis: true);
			EClass.player.uidLastZone = currentZone.uid;
			if (!EClass.player.simulatingZone)
			{
				if (currentZone.IsRegion)
				{
					Msg.Say("enterZone", z.Name);
				}
				else
				{
					if (z.IsRegion)
					{
						Msg.Say("leaveZone", currentZone.Name);
					}
					else if (z.id != currentZone.id)
					{
						Msg.Say("enterZone", z.Name);
					}
					EClass.player.lastZonePos = pos.Copy();
				}
				EClass.player.lastTransition = transition;
			}
			foreach (Chara item in EClass._map.charas.Where((Chara c) => c.IsPCPartyMinion && c.master != EClass.pc).ToList())
			{
				EClass._zone.RemoveCard(item);
			}
			EClass.player.listSummon = EClass._map.charas.Where((Chara c) => c.c_uidMaster != 0 && c.FindMaster() == EClass.pc && c.c_minionType == MinionType.Default).ToList();
			foreach (Chara item2 in EClass.player.listSummon)
			{
				EClass._zone.RemoveCard(item2);
			}
		}
		if (party != null && party.leader == this)
		{
			foreach (Chara member in party.members)
			{
				if (member != this && !member.isDead && member.parent is Zone)
				{
					member.MoveZone(z);
				}
			}
		}
		if (global == null)
		{
			Debug.Log(base.Name);
			return;
		}
		transition.uidLastZone = currentZone?.uid ?? 0;
		global.transition = transition;
		if (z.IsActiveZone)
		{
			Point spawnPos = z.GetSpawnPos(this);
			global.transition = null;
			if (IsPC)
			{
				EClass.player.lastTransition = null;
			}
			z.AddCard(this, spawnPos);
			if (IsBranchMember())
			{
				EClass._map.rooms.AssignCharas();
			}
		}
		else
		{
			z.AddCard(this);
		}
	}

	public void MoveHome(string id, int x = -1, int z = -1)
	{
		MoveHome(EClass.game.world.FindZone(id), x, z);
	}

	public void MoveHome(Zone zone, int x = -1, int z = -1)
	{
		if (isDead)
		{
			Revive();
		}
		else
		{
			Cure(CureType.Death);
		}
		CureCondition<ConSuspend>();
		if (IsPCParty)
		{
			EClass.pc.party.RemoveMember(this);
		}
		FactionBranch factionBranch = homeBranch;
		if (x == -1)
		{
			x = 50;
		}
		if (z == -1)
		{
			z = 50;
		}
		Point point = new Point(x, z);
		if (zone.IsActiveZone)
		{
			point = point.GetNearestPoint(allowBlock: false, allowChara: false);
		}
		zone.AddCard(this, point);
		SetHomeZone(zone);
		global.transition = new ZoneTransition
		{
			state = ZoneTransition.EnterState.Dead,
			x = point.x,
			z = point.z
		};
		orgPos = new Point(x, z);
		if (factionBranch != null)
		{
			RefreshWorkElements();
			factionBranch.policies.Validate();
		}
	}

	public void FallFromZone()
	{
		Msg.Say("skyFall", EClass.pc, EClass._zone.Name);
		Zone zone = (EClass._zone.isExternalZone ? null : EClass._zone.GetTopZone().FindZone(EClass._zone.lv - 1));
		zone = zone ?? EClass.world.region;
		MoveZone(zone ?? EClass.world.region, new ZoneTransition
		{
			state = ZoneTransition.EnterState.Fall,
			x = pos.x,
			z = pos.z
		});
	}

	public override void SetDir(int d)
	{
		base.dir = d;
		UpdateAngle();
		renderer.RefreshSprite();
	}

	public override void Rotate(bool reverse = false)
	{
		if (renderer.hasActor)
		{
			base.dir = (base.dir + ((!reverse) ? 1 : (-1))).Clamp(0, 3, loop: true);
		}
		else
		{
			base.dir = ((base.dir == 0) ? 1 : 0);
		}
		UpdateAngle();
		renderer.RefreshSprite();
	}

	public override void LookAt(Card c)
	{
		LookAt(c.pos);
	}

	public override void LookAt(Point p)
	{
		angle = Util.GetAngle(p.x - pos.x, p.z - pos.z);
		if (EClass._zone.IsRegion)
		{
			if (angle > 100f && angle < 170f)
			{
				base.dir = 2;
			}
			else if (angle > 170f && angle < 190f)
			{
				base.dir = 0;
			}
			else if (angle > 190f || (angle < -10f && angle > -100f))
			{
				base.dir = 3;
			}
			else
			{
				base.dir = 1;
			}
			angle -= 45f;
		}
		else if (angle > 170f && angle < 235f)
		{
			base.dir = 0;
		}
		else if (angle > 80f && angle < 145f)
		{
			base.dir = 1;
		}
		else if (angle > -100f && angle < -35f)
		{
			base.dir = 3;
		}
		else if (angle > -10f && angle < 55f)
		{
			base.dir = 2;
		}
		renderer.RefreshSprite();
	}

	public void UpdateAngle()
	{
		if (IsPCC)
		{
			if (base.dir == 0)
			{
				angle = 225f;
			}
			else if (base.dir == 1)
			{
				angle = 135f;
			}
			else if (base.dir == 2)
			{
				angle = 45f;
			}
			else if (base.dir == 3)
			{
				angle = -45f;
			}
		}
		else if (base.dir == 0)
		{
			angle = 165f;
		}
		else if (base.dir == 1)
		{
			angle = 300f;
		}
		else if (base.dir == 2)
		{
			angle = 0f;
		}
		else if (base.dir == 3)
		{
			angle = 120f;
		}
	}

	public int GetCurrentDir()
	{
		Debug.Log(angle);
		if (!renderer.hasActor)
		{
			if (angle == 0f || angle == 45f || angle == 90f)
			{
				return 2;
			}
			if (angle == -135f || angle == 180f || angle == -90f)
			{
				return 1;
			}
			if (angle == 135f)
			{
				return 0;
			}
			return 3;
		}
		return renderer.actor.currentDir;
	}

	public void UpdateSight()
	{
		int num = 4;
		for (int i = -num; i < num + 1; i++)
		{
			for (int j = -num; j < num + 1; j++)
			{
				shared.Set(pos.x + i, pos.z + j);
				if (shared.IsValid && !shared.cell.isSeen && i >= -1 && i <= 1 && j >= -1)
				{
					_ = 1;
				}
			}
		}
	}

	public bool WillConsumeTurn()
	{
		for (int num = conditions.Count - 1; num >= 0; num--)
		{
			if (conditions[num].ConsumeTurn)
			{
				return true;
			}
		}
		return false;
	}

	public void TickConditions()
	{
		if (_cooldowns != null)
		{
			TickCooldown();
		}
		turn++;
		consumeTurn = false;
		preventRegen = false;
		emoIcon = Emo2.none;
		if (base.isSummon)
		{
			base.c_summonDuration--;
			if (base.c_summonDuration <= 0)
			{
				Die();
				return;
			}
		}
		if (EClass.world.weather.IsRaining && !EClass._map.IsIndoor && !pos.cell.HasRoof)
		{
			AddCondition<ConWet>(20);
		}
		switch (turn % 50)
		{
		case 0:
			happiness = (hunger.value + stamina.value + depression.value + bladder.value + hygiene.value) / 5;
			break;
		case 1:
			if (!IsPC || !EClass.debug.godMode)
			{
				if (EClass.rnd(2) == 0 && !HasCondition<ConAwakening>())
				{
					sleepiness.Mod(1);
				}
				if (EClass.rnd(3) == 0)
				{
					hunger.Mod(1);
				}
				if (IsPC && (sleepiness.GetPhase() != 0 || stamina.GetPhase() <= 1))
				{
					Tutorial.Play("sleep");
				}
			}
			break;
		case 2:
			if (parasite != null)
			{
				ModExp(227, (EClass._zone.IsRegion ? 5 : 40) * 100 / Mathf.Max(100, 100 + (elements.Base(227) - parasite.LV) * 25));
			}
			if (ride != null)
			{
				ModExp(226, (EClass._zone.IsRegion ? 5 : 40) * 100 / Mathf.Max(100, 100 + (elements.Base(226) - ride.LV) * 25));
			}
			break;
		case 3:
		{
			int phase = hygiene.GetPhase();
			int num = 0;
			num = ((!IsPC) ? ((phase > 3) ? 50 : 0) : ((phase > 3) ? 50 : 0));
			if (num > EClass.rnd(100))
			{
				hygiene.Mod(-1);
			}
			break;
		}
		}
		if (turn % 500 == 0)
		{
			DiminishTempElements();
		}
		if (IsPCParty)
		{
			if (dirtyWeight)
			{
				CalcBurden();
			}
			int phase2 = burden.GetPhase();
			int phase3 = hunger.GetPhase();
			if (phase3 >= 4)
			{
				preventRegen = true;
			}
			if (EClass.rnd(EClass._zone.IsRegion ? 100 : 30) == 0 && phase2 >= 3)
			{
				Say("dmgBurden", this);
				DamageHP(MaxHP * (base.ChildrenWeight * 100 / WeightLimit) / 1000 + 1, AttackSource.Burden);
				if (isDead)
				{
					return;
				}
			}
			if (EClass.rnd(12) == 0)
			{
				if (IsPC)
				{
					if (phase2 > 0)
					{
						ModExp(207, 1 + phase2 * phase2);
					}
				}
				else
				{
					ModExp(207, 4);
				}
			}
			if (IsPC)
			{
				if (phase3 >= 5)
				{
					if (!(ai is AI_Eat) && EClass.rnd(5) == 0)
					{
						DamageHP(1 + EClass.rnd(2) + MaxHP / 50, AttackSource.Hunger);
					}
					if (!isDead && EClass.rnd(3) == 0)
					{
						stamina.Mod(-1);
					}
				}
				if (isDead)
				{
					return;
				}
				phase3 = stamina.GetPhase();
				if (phase3 <= 0)
				{
					preventRegen = true;
				}
				if (currentZone.IsRegion && EClass.world.weather.CurrentCondition == Weather.Condition.RainHeavy && !EClass.pc.HasElement(408))
				{
					if (EClass.rnd(100) == 0 && !isConfused)
					{
						Msg.Say("rain_confuse");
						AddCondition<ConConfuse>(500);
					}
					if (EClass.rnd(300) == 0 && !isBlind)
					{
						Msg.Say("rain_confuse");
						AddCondition<ConBlind>(200);
					}
				}
				if (turn % (200000 / Mathf.Max(100 + Evalue(409) * 10, 50)) == 0)
				{
					ModCorruption(1);
				}
			}
		}
		if (!IsPC)
		{
			int num2 = Evalue(409);
			if (num2 > 0 && turn % (200000 / Mathf.Max(100 + num2 * 10, 50)) == 0)
			{
				ModCorruption(1);
			}
		}
		for (int num3 = conditions.Count - 1; num3 >= 0; num3--)
		{
			if (num3 < conditions.Count)
			{
				Condition condition = conditions[num3];
				if (!condition.TimeBased)
				{
					condition.Tick();
				}
				if (!condition.IsKilled)
				{
					if (condition.ConsumeTurn)
					{
						consumeTurn = true;
					}
					if (condition.PreventRegen)
					{
						preventRegen = true;
					}
					if (condition.EmoIcon != 0 && condition.EmoIcon > emoIcon)
					{
						emoIcon = condition.EmoIcon;
					}
				}
				if (isDead)
				{
					return;
				}
			}
		}
		if (id == "tsunami")
		{
			if (elements.Base(79) < 30)
			{
				Die();
				return;
			}
			if (IsInCombat)
			{
				elements.SetTo(79, elements.Base(79) * 3 / 4);
			}
		}
		if (!preventRegen)
		{
			if (EClass.rnd(25) == 0 && base.hp < MaxHP)
			{
				HealHP(EClass.rnd(Evalue(300) / 3 + 1) + 1);
				elements.ModExp(300, 8);
			}
			if (EClass.rnd(8) == 0 && mana.value < mana.max)
			{
				mana.Mod(EClass.rnd(Evalue(301) / 2 + 1) + 1);
				elements.ModExp(301, 8);
			}
			if (EClass.rnd(20) == 0 && !IsPC && stamina.value < stamina.max)
			{
				stamina.Mod(EClass.rnd(5) + 1);
			}
		}
	}

	public void SyncRide()
	{
		if (host != null)
		{
			host.SyncRide();
		}
		if (ride != null)
		{
			SyncRide(ride);
		}
		if (parasite != null)
		{
			SyncRide(parasite);
		}
	}

	public void SyncRide(Chara c)
	{
		if (!c.pos.Equals(pos))
		{
			if (!pos.IsValid)
			{
				Debug.LogError("exception: pos is not valid:" + pos?.ToString() + "/" + this);
				pos = new Point();
			}
			EClass._map.MoveCard(pos, c);
		}
	}

	public override void Tick()
	{
		SyncRide();
		combatCount--;
		ignoreSPAbsorb = false;
		if (IsPC)
		{
			if (hasMovedThisTurn)
			{
				pos.Things.ForeachReverse(delegate(Thing t)
				{
					t.trait.OnStepped(this);
				});
				if (isDead)
				{
					return;
				}
				hasMovedThisTurn = false;
				if (EClass.player.haltMove)
				{
					EClass.player.haltMove = false;
					ActionMode.Adv.TryCancelInteraction(sound: false);
					EInput.Consume(1);
					return;
				}
				if (EClass._zone.IsRegion)
				{
					if (EClass.game.config.preference.autoEat)
					{
						foreach (Chara member in EClass.pc.party.members)
						{
							if (member.hunger.value > 65)
							{
								member.InstantEat(null, sound: false);
							}
						}
					}
					Chara chara = null;
					EloMap.Cell cell2 = EClass.scene.elomap.GetCell(EClass.pc.pos);
					if (cell2 != null && (cell2.zone == null || (cell2.zone is Zone_Field && !cell2.zone.IsPCFaction)))
					{
						foreach (Chara chara2 in EClass._map.charas)
						{
							if (!chara2.IsPCFactionOrMinion && !chara2.IsGlobal && chara2.pos.Equals(EClass.pc.pos))
							{
								chara = chara2;
								break;
							}
						}
					}
					if (chara != null)
					{
						if (!EClass.pc.HasCondition<ConDrawMonster>())
						{
							EClass.player.safeTravel = 5 + EClass.rnd(5);
						}
						EClass._zone.RemoveCard(chara);
						Msg.Say("encounter");
						EClass.player.EnterLocalZone(encounter: true, chara);
					}
					else if (EClass.player.safeTravel <= 0)
					{
						if (cell2 != null && cell2.zone == null && !EClass.debug.ignoreEncounter)
						{
							EloMap.TileInfo tileInfo = EClass.scene.elomap.GetTileInfo(EClass.pc.pos.eloX, EClass.pc.pos.eloY);
							if (!tileInfo.shore)
							{
								bool num = EClass.pc.HasCondition<ConWardMonster>();
								bool flag = EClass.pc.HasCondition<ConDrawMonster>();
								bool flag2 = EClass.game.quests.Get<QuestEscort>() != null;
								int num2 = (tileInfo.isRoad ? 22 : 12);
								if (flag2)
								{
									num2 = (tileInfo.isRoad ? 16 : 10);
								}
								if (num)
								{
									num2 *= (flag2 ? 2 : 20);
								}
								if (flag)
								{
									num2 /= 2;
								}
								if (EClass.rnd(num2) == 0)
								{
									Msg.Say("encounter");
									if (!flag)
									{
										EClass.player.safeTravel = 5 + EClass.rnd(5);
									}
									EClass.player.EnterLocalZone(encounter: true);
								}
							}
						}
					}
					else
					{
						EClass.player.safeTravel--;
					}
				}
			}
			EClass.player.pickupDelay = 0f;
			if (EClass.player.returnInfo != null)
			{
				EClass.player.returnInfo.turns--;
				if (EClass.player.returnInfo.turns <= 0)
				{
					if (EClass.pc.burden.GetPhase() != 4 || EClass.debug.ignoreWeight)
					{
						int uidDest = EClass.player.returnInfo.uidDest;
						Zone zone = null;
						if (uidDest != 0)
						{
							zone = EClass.game.spatials.map.TryGetValue(uidDest) as Zone;
						}
						if (zone == null || zone.destryoed)
						{
							zone = EClass.world.region;
						}
						if (zone == EClass.game.activeZone || EClass.game.activeZone.IsRegion)
						{
							Msg.Say("returnFail");
						}
						else
						{
							Msg.Say("returnComplete");
							EClass.player.uidLastTravelZone = 0;
							EClass.pc.MoveZone(zone, ZoneTransition.EnterState.Return);
							EClass.player.lastZonePos = null;
						}
						EClass.player.returnInfo = null;
						return;
					}
					EClass.player.returnInfo = null;
					Msg.Say("returnOverweight");
				}
			}
			if ((HasNoGoal || !ai.IsRunning) && !WillConsumeTurn())
			{
				SetAI(_NoGoalPC);
				return;
			}
			EClass.player.stats.turns++;
			if (EClass.core.config.game.alwaysUpdateRecipe)
			{
				RecipeUpdater.dirty = true;
			}
			actTime = EClass.player.baseActTime;
		}
		else
		{
			actTime = EClass.player.baseActTime * Mathf.Max(0.1f, (float)EClass.pc.Speed / (float)Speed);
			hasMovedThisTurn = false;
		}
		TickConditions();
		if (!IsAliveInCurrentZone)
		{
			return;
		}
		renderer.RefreshStateIcon();
		if (host != null && !consumeTurn)
		{
			if (host.ride == this && ((host.hasMovedThisTurn && IsInCombat) || (enemy != null && turn % 3 != 0)))
			{
				consumeTurn = true;
			}
			if (host.parasite == this && enemy != null && EClass.rnd(10) > EClass.rnd(host.Evalue(227) + 10))
			{
				if (Dist(enemy) < 3 && EClass.rnd(2) == 0)
				{
					Say("parasite_fail", this, host);
					if (EClass.rnd(2) == 0 && GetInt(106) == 0)
					{
						Talk("parasite_fail");
					}
				}
				consumeTurn = true;
			}
		}
		if (consumeTurn)
		{
			if (IsPC)
			{
				ActionMode.Adv.SetTurbo();
			}
		}
		else
		{
			if (base.isRestrained)
			{
				TryUnrestrain();
			}
			if (enemy != null)
			{
				if (!enemy.IsAliveInCurrentZone || EClass._zone.IsRegion)
				{
					enemy = null;
				}
				else if (!IsPC && (ai.CancelOnAggro || !ai.IsRunning))
				{
					SetAIAggro();
				}
			}
			if (HasNoGoal || !ai.IsRunning)
			{
				ChooseNewGoal();
			}
			ai.Tick();
		}
		Cell cell = base.Cell;
		if (EClass._zone.IsUnderwater || (cell.IsTopWaterAndNoSnow && !cell.isFloating))
		{
			AddCondition<ConWet>(50);
			if (pos.IsHotSpring)
			{
				hygiene.Mod(2);
			}
		}
		if (IsPC && !EClass._zone.IsRegion && cell.CanSuffocate() && !EClass.debug.godMode)
		{
			AddCondition<ConSuffocation>(800 / (100 + EvalueMax(200, -5) * 10), force: true);
		}
		CellEffect e;
		if (cell.effect != null)
		{
			e = cell.effect;
			switch (e.id)
			{
			case 1:
			case 2:
			case 4:
				if (IsLevitating)
				{
					Say("levitating");
					break;
				}
				AddCondition<ConWet>(50);
				ProcEffect();
				break;
			case 3:
				PlaySound("fire_step");
				AddCondition<ConBurning>(30);
				break;
			case 5:
				if (!isWet)
				{
					PlaySound("bubble");
					AddCondition<ConWet>(30);
					ProcEffect();
				}
				break;
			case 6:
				if (hasMovedThisTurn)
				{
					Say("abMistOfDarkness_step", this);
				}
				break;
			}
		}
		if (IsPC)
		{
			if (EClass.player.currentHotItem.Thing != null)
			{
				EClass.player.currentHotItem.Thing.trait.OnTickHeld();
			}
			EClass.screen.OnEndPlayerTurn();
		}
		void ClearEffect()
		{
			EClass._map.SetLiquid(cell.x, cell.z, 0, 0);
		}
		void ProcEffect()
		{
			if (e.idEffect != 0)
			{
				ActEffect.ProcAt(e.idEffect, e.power, e.isBlessed ? BlessedState.Blessed : (e.isCursed ? BlessedState.Cursed : BlessedState.Normal), this, this, new Point(pos), e.isHostileAct, new ActRef
				{
					aliasEle = EClass.sources.elements.map[e.idEle].alias,
					n1 = e.n1
				});
				ClearEffect();
			}
		}
	}

	public bool CanLift(Card c)
	{
		return true;
	}

	public bool CanAutoPick(Card c)
	{
		return true;
	}

	public bool CanPick(Card c)
	{
		if (c.isDestroyed)
		{
			return false;
		}
		Card rootCard = c.GetRootCard();
		if (rootCard.isDestroyed || (rootCard.ExistsOnMap && rootCard.pos.Distance(pos) > 1))
		{
			return false;
		}
		if (rootCard != this && things.IsFull(c.Thing))
		{
			return false;
		}
		return true;
	}

	public void PickOrDrop(Point p, string idThing, int idMat = -1, int num = 1, bool msg = true)
	{
		if (num != 0)
		{
			PickOrDrop(p, ThingGen.Create(idThing, idMat).SetNum(num), msg);
		}
	}

	public void PickOrDrop(Point p, Thing t, bool msg = true)
	{
		if (things.GetDest(t).IsValid)
		{
			Pick(t, msg);
		}
		else
		{
			EClass._zone.AddCard(t, p);
		}
	}

	public Thing Pick(Thing t, bool msg = true, bool tryStack = true)
	{
		if (t.trait is TraitCard && t.isNew && EClass.game.config.autoCollectCard && !t.c_idRefCard.IsEmpty())
		{
			ContentCodex.Collect(t);
			return t;
		}
		if (t.parent == this)
		{
			return t;
		}
		t = TryPoisonPotion(t);
		ThingContainer.DestData dest = things.GetDest(t, tryStack);
		if (!dest.IsValid)
		{
			if (t.parent != EClass._zone)
			{
				if (IsPC)
				{
					Say("backpack_full_drop", t);
					SE.Drop();
				}
				return EClass._zone.AddCard(t, pos).Thing;
			}
			if (IsPC)
			{
				Say("backpack_full", t);
			}
			return t;
		}
		if (dest.stack != null)
		{
			if (msg)
			{
				PlaySound("pick_thing");
				Say("pick_thing", this, t);
			}
			t.TryStackTo(dest.stack);
			return dest.stack;
		}
		TryAbsorbRod(t);
		if (msg)
		{
			PlaySound("pick_thing");
			Say("pick_thing", this, t);
		}
		TryReservePickupTutorial(t);
		return dest.container.AddThing(t, tryStack);
	}

	public Thing TryPoisonPotion(Thing t)
	{
		if (t.trait is TraitPotion && t.id != "1165" && !t.source.tag.Contains("neg") && EClass.rnd(2) == 0 && HasElement(1565))
		{
			string text = EClass.sources.things.rows.Where((SourceThing.Row a) => a._origin == "potion" && a.tag.Contains("neg") && a.chance > 100).ToList().RandomItemWeighted((SourceThing.Row a) => a.chance)
				.id;
			Say("poisonDrip", this);
			int num = t.Num;
			t.Destroy();
			t = ThingGen.Create(text).SetNum(num);
		}
		return t;
	}

	public void TryAbsorbRod(Thing t)
	{
		if (!IsPC || !(t.trait is TraitRod) || t.c_charges <= 0 || !HasElement(1564))
		{
			return;
		}
		Say("absorbRod", this, t);
		TraitRod rod = t.trait as TraitRod;
		bool flag = false;
		if (rod.source != null)
		{
			using IEnumerator<SourceElement.Row> enumerator = EClass.sources.elements.rows.Where((SourceElement.Row a) => a.id == rod.source.id).GetEnumerator();
			if (enumerator.MoveNext())
			{
				SourceElement.Row current = enumerator.Current;
				if (IsPC)
				{
					GainAbility(current.id, t.c_charges * 100);
					flag = true;
				}
			}
		}
		if (!flag)
		{
			mana.Mod(-50 * t.c_charges);
		}
		t.c_charges = 0;
		LayerInventory.SetDirty(t);
	}

	public void TryReservePickupTutorial(Thing t)
	{
		if (t.id == "axe")
		{
			Tutorial.Reserve("tool");
		}
		if (t.category.id == "mushroom")
		{
			Tutorial.Reserve("food");
		}
		if (t.category.id == "herb")
		{
			Tutorial.Reserve("herb");
		}
		if (t.id == "pasture")
		{
			Tutorial.Reserve("pasture");
		}
		if (t.id == "log")
		{
			Tutorial.Reserve("process");
		}
	}

	public void TryPickGroundItem()
	{
		foreach (Card item in pos.ListCards())
		{
			if ((IsPC || !(EClass.pc.ai is AI_UseCrafter aI_UseCrafter) || !aI_UseCrafter.ings.Contains(item)) && item.isThing && item.placeState == PlaceState.roaming && CanPick(item))
			{
				Thing thing = Pick(item.Thing);
				if (thing != null && !IsPC)
				{
					thing.isNPCProperty = true;
				}
				break;
			}
		}
	}

	public void TryPutShared(Thing t, List<Thing> containers = null, bool dropIfFail = true)
	{
		if (!EClass._zone.TryAddThingInSharedContainer(t, containers) && dropIfFail)
		{
			EClass._zone.AddCard(t, pos);
		}
	}

	public bool TryHoldCard(Card t, int num = -1, bool pickHeld = false)
	{
		if (held == t)
		{
			return true;
		}
		if (t.isDestroyed || t.Num <= 0)
		{
			return false;
		}
		if (!CanPick(t))
		{
			if (t.parent == null)
			{
				EClass._zone.AddCard(t, pos);
			}
			return false;
		}
		HoldCard(t, num);
		return true;
	}

	public void HoldCard(Card t, int num = -1)
	{
		if (held == t || t.isDestroyed || t.Num <= 0)
		{
			return;
		}
		if (IsPC && t.isNPCProperty)
		{
			t.isNPCProperty = false;
			EClass.player.ModKarma(-1);
			pos.TryWitnessCrime(this);
		}
		PickHeld();
		if (t.isChara)
		{
			if (t.IsAliveInCurrentZone)
			{
				t.ShowEmo(Emo.love);
			}
			EClass.player.altHeldPos = t.renderer.data.ForceAltHeldPosition || EClass.rnd(2) == 0;
		}
		else
		{
			if (num == -1 || num > t.Num)
			{
				num = t.Num;
			}
			if (num < t.Num)
			{
				t = t.Split(num);
			}
		}
		if (t.GetRootCard() != this)
		{
			t = Pick(t.Thing, msg: false, tryStack: false);
			if (t.GetRootCard() != this)
			{
				return;
			}
		}
		held = t;
		if (held.GetLightRadius() > 0)
		{
			RecalculateFOV();
		}
		if (IsPC)
		{
			LayerInventory.SetDirty(t.Thing);
			WidgetHotbar.dirtyCurrentItem = true;
		}
	}

	public void PickHeld(bool msg = false)
	{
		if (held == null)
		{
			return;
		}
		Card card = held;
		if (IsPC && held.invY == 1)
		{
			WidgetHotbar.dirtyCurrentItem = true;
			LayerInventory.SetDirty(held.Thing);
			held = null;
			return;
		}
		if (held.isChara)
		{
			DropHeld();
			return;
		}
		if (IsPC && !held.IsHotItem && held.trait.CanOnlyCarry)
		{
			Say("canOnlyCarry", held);
			DropHeld();
			return;
		}
		bool flag = held != things.TryStack(held.Thing);
		if (!flag && things.IsOverflowing())
		{
			if (IsPC)
			{
				Say("backpack_full_drop", held);
				SE.Drop();
			}
			DropHeld();
			return;
		}
		if (msg)
		{
			PlaySound("pick_thing");
			Say("pick_held", this, card);
			if (IsPC && card.id == "statue_weird")
			{
				Say("statue_pick");
			}
		}
		if (IsPC)
		{
			WidgetHotbar.dirtyCurrentItem = true;
			if (!flag)
			{
				LayerInventory.SetDirty(held.Thing);
				if (held.GetRootCard() != EClass.pc)
				{
					Pick(held.Thing, msg: false);
				}
			}
		}
		held = null;
	}

	public Card SplitHeld(int a)
	{
		return held.Split(a);
	}

	public void TryDropCarryOnly()
	{
		if (held != null && held.trait.CanOnlyCarry)
		{
			DropHeld();
		}
		foreach (Thing item in things.List((Thing t) => t.trait.CanOnlyCarry))
		{
			DropThing(item);
		}
	}

	public Card DropHeld(Point dropPos = null)
	{
		if (held == null)
		{
			return null;
		}
		if (IsPC)
		{
			WidgetHotbar.dirtyCurrentItem = true;
			LayerInventory.SetDirty(held.Thing);
		}
		Card card = EClass._zone.AddCard(held, dropPos ?? pos);
		card.OnLand();
		if (card.trait.CanOnlyCarry)
		{
			card.SetPlaceState(PlaceState.installed);
		}
		return card;
	}

	public void DropThing(Thing t, int num = -1)
	{
		if (t.c_isImportant)
		{
			Msg.Say("markedImportant");
			return;
		}
		if (!t.trait.CanBeDropped)
		{
			Msg.Say("cantDrop", t);
			return;
		}
		if (t.trait is TraitAbility)
		{
			SE.Trash();
			t.Destroy();
			return;
		}
		Msg.Say("dropItem", t.Name);
		t.ignoreAutoPick = true;
		PlaySound("drop");
		EClass._zone.AddCard(t, pos);
		if (t.trait.CanOnlyCarry)
		{
			t.Install();
		}
	}

	public AttackStyle GetFavAttackStyle()
	{
		int num = Evalue(131);
		int num2 = Evalue(130);
		int num3 = Evalue(123);
		if (num > num2 && num > num3)
		{
			return AttackStyle.TwoWield;
		}
		if (num2 > num && num2 > num3)
		{
			return AttackStyle.TwoHand;
		}
		if (num3 > num && num3 > num2)
		{
			return AttackStyle.Shield;
		}
		return AttackStyle.Default;
	}

	public Element GetFavWeaponSkill()
	{
		return elements.ListElements((Element e) => e.source.categorySub == "weapon" && !e.HasTag("ranged")).FindMax((Element a) => a.Value);
	}

	public Element GetFavArmorSkill()
	{
		if (elements.Value(122) > elements.Value(120))
		{
			return elements.GetElement(122);
		}
		return elements.GetElement(120);
	}

	public void TryRestock(bool onCreate)
	{
		isOnCreate = onCreate;
		if (onCreate || (!IsPCFaction && (base.IsUnique || trait is TraitAdventurer || trait is TraitGuard)))
		{
			RestockEquip((!EClass.core.IsGameStarted || !(EClass._zone is Zone_Music)) && onCreate);
		}
		RestockInventory(onCreate);
	}

	public void RestockEquip(bool onCreate)
	{
		string equip = source.equip;
		if (equip.IsEmpty())
		{
			equip = job.equip;
		}
		if (equip == "none")
		{
			return;
		}
		switch (id)
		{
		case "kettle":
		case "quru":
		case "loytel":
		case "shojo":
			EQ_ID("staff_long", 1);
			EQ_CAT("head");
			EQ_CAT("torso");
			EQ_CAT("arm");
			return;
		case "adv_kiria":
			if (onCreate)
			{
				EQ_ID("sword_zephir");
			}
			break;
		case "adv_mesherada":
			if (onCreate)
			{
				EQ_ID("dagger_hathaway");
			}
			break;
		case "adv_verna":
			if (onCreate)
			{
				EQ_ID("staff_long", -1, Rarity.Legendary);
			}
			if (onCreate)
			{
				EQ_ID("cloak_wing", -1, Rarity.Mythical);
			}
			break;
		case "big_sister":
			if (onCreate)
			{
				EQ_ID("sword_muramasa");
			}
			break;
		case "adv_gaki":
			if (onCreate)
			{
				EQ_ID("dagger_gaki");
			}
			if (onCreate)
			{
				EQ_ID("dagger_ninto");
			}
			break;
		case "adv_ivory":
			EQ_ID("dagger", -1, Rarity.Legendary);
			AddThing("60");
			break;
		case "adv_wini":
			if (onCreate)
			{
				EQ_ID("staff_Cat", -1, Rarity.Mythical);
			}
			AddCard(ThingGen.CreateSpellbook(9150));
			if (onCreate)
			{
				AddThing("1071");
			}
			break;
		case "seeker":
			if (onCreate)
			{
				EQ_ID("helm_seeker");
			}
			EQ_ID("robe_pope");
			EQ_ID("sword_katana");
			EQ_ID("staff");
			EQ_ID("sword_katana");
			if (onCreate)
			{
				EQ_ID("boots_seven");
			}
			if (onCreate)
			{
				for (int i = 0; i < 20; i++)
				{
					AddThing(ThingGen.CreateFromCategory("book", 50));
				}
				EQ_Item("panty");
				AddThing("plat").SetNum(6);
			}
			break;
		case "ephrond":
			if (onCreate)
			{
				AddThing("guitar_efrond");
			}
			break;
		case "ashland":
			if (onCreate)
			{
				AddThing("guitar_ash");
			}
			break;
		case "swordkeeper":
			if (onCreate)
			{
				EQ_ID("EtherDagger");
			}
			break;
		}
		switch (equip)
		{
		case "archer":
			if (onCreate || !TryEquipRanged())
			{
				EQ_CAT((EClass.rnd(4) == 0) ? "crossbow" : "bow");
			}
			break;
		case "inquisitor":
		case "gunner":
			if (onCreate || !TryEquipRanged())
			{
				EQ_CAT("gun");
			}
			break;
		}
		int num = ((base.rarity >= Rarity.Mythical) ? (base.LV * 3) : ((base.rarity >= Rarity.Legendary) ? (base.LV * 2) : base.LV));
		if (trait is TraitAdventurer)
		{
			num *= 3;
		}
		if (race.id == "asura")
		{
			for (int j = 0; j < 4; j++)
			{
				EQ_CAT(job.weapon.RandomItem());
			}
		}
		for (int k = 0; k < ((!(race.id == "mutant")) ? 1 : (2 + base.LV / 30)); k++)
		{
			if (source.ContainsTag("boxer"))
			{
				EQ_CAT("martial");
			}
			else if (!job.weapon.IsEmpty())
			{
				if (race.id == "mutant" || (body.slotMainHand != null && body.slotMainHand.thing == null))
				{
					EQ_CAT(job.weapon.RandomItem());
				}
				if (race.id == "mutant" || (Evalue(131) > 0 && EClass.rnd(2) == 0))
				{
					EQ_CAT(job.weapon.RandomItem());
				}
			}
			EQ_CAT("torso");
			if (EClass.rnd(num) > 5)
			{
				EQ_CAT("arm");
			}
			if (EClass.rnd(num) > 10)
			{
				EQ_CAT("head");
			}
			if (EClass.rnd(num) > 15)
			{
				EQ_CAT("back");
			}
			if (EClass.rnd(num) > 20)
			{
				EQ_CAT("ring");
			}
			if (EClass.rnd(num) > 25)
			{
				EQ_CAT("amulet");
			}
			if (EClass.rnd(num) > 30)
			{
				EQ_CAT("foot");
			}
			if (EClass.rnd(num) > 35)
			{
				EQ_CAT("waist");
			}
			if (EClass.rnd(num) > 40)
			{
				EQ_CAT("ring");
			}
		}
		if (trait is TraitBard)
		{
			AddThing(ThingGen.Create("lute"));
		}
	}

	public void RestockInventory(bool onCreate)
	{
		switch (id)
		{
		case "fiama":
			Restock("book_story", 1);
			break;
		case "rock_thrower":
			Restock("stone", 10 + EClass.rnd(10));
			break;
		case "giant":
			Restock("rock", 2 + EClass.rnd(10));
			break;
		case "farris":
			Restock("lute", 1);
			Restock("book_story_home", 1);
			break;
		case "begger":
			break;
		}
		void Restock(string id, int num)
		{
			if (things.Find(id) == null)
			{
				AddCard(ThingGen.Create(id).SetNum(num));
			}
		}
	}

	private void SetEQQuality()
	{
		CardBlueprint.Set(CardBlueprint.CharaGenEQ);
		Rarity rarity = Rarity.Normal;
		int num = ((base.LV >= 1000) ? 7 : ((base.LV >= 500) ? 5 : ((base.LV >= 250) ? 3 : ((base.LV >= 100) ? 2 : ((base.LV >= 50) ? 1 : 0)))));
		Rarity rarity2 = base.rarity;
		if (id == "big_sister")
		{
			num = (isOnCreate ? 8 : 4);
		}
		if (!isOnCreate && EClass.rnd(10) != 0)
		{
			num /= 2;
		}
		if (rarity2 == Rarity.Superior && EClass.rnd(10) <= num)
		{
			rarity = Rarity.Superior;
		}
		else if (rarity2 == Rarity.Legendary)
		{
			rarity = ((EClass.rnd(10) <= num) ? Rarity.Legendary : ((EClass.rnd(5) <= num) ? Rarity.Superior : Rarity.Normal));
		}
		else if (rarity2 >= Rarity.Mythical)
		{
			rarity = ((EClass.rnd(30) <= num) ? Rarity.Mythical : ((EClass.rnd(10) > num) ? Rarity.Superior : Rarity.Legendary));
		}
		if (rarity == Rarity.Normal && EClass.rnd(1000) == 0)
		{
			rarity = Rarity.Legendary;
		}
		CardBlueprint.current.rarity = rarity;
	}

	public Thing EQ_ID(string s, int mat = -1, Rarity r = Rarity.Random)
	{
		SetEQQuality();
		if (r != Rarity.Random)
		{
			CardBlueprint.current.rarity = r;
		}
		Thing thing = ThingGen.Create(s, mat, base.LV);
		if (body.GetSlot(thing, onlyEmpty: true) == null)
		{
			return thing;
		}
		AddThing(thing);
		if (!body.Equip(thing))
		{
			thing.Destroy();
		}
		return thing;
	}

	public void EQ_CAT(string s)
	{
		int slot = EClass.sources.categories.map[s].slot;
		BodySlot bodySlot = ((slot == 0) ? null : body.GetSlot(slot));
		if ((slot == 0 || bodySlot != null) && (slot != 37 || !HasElement(1209)))
		{
			SetEQQuality();
			Thing thing = ThingGen.CreateFromCategory(s, base.LV);
			AddThing(thing);
			if (bodySlot != null && !body.Equip(thing, bodySlot))
			{
				thing.Destroy();
			}
		}
	}

	public void EQ_Item(string s, int num = 1)
	{
		SetEQQuality();
		Thing t = ThingGen.Create(s, -1, base.LV).SetNum(num);
		AddThing(t);
	}

	public void Drink(Card t)
	{
		Say("drink", this, t.Duplicate(1));
		Say("quaff");
		PlaySound("drink");
		hunger.Mod(-2);
		t.ModNum(-1);
		t.trait.OnDrink(this);
		_ = IsPC;
	}

	public bool CanRevive()
	{
		return EClass.world.date.IsExpired(base.c_dateDeathLock);
	}

	public void GetRevived()
	{
		Revive(EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: false), msg: true);
		if (!IsPCFaction)
		{
			return;
		}
		if (!IsPC && (!trait.CanJoinPartyResident || !trait.CanJoinParty || memberType != 0))
		{
			if (homeZone != null && EClass._zone != homeZone)
			{
				Msg.Say("returnHome", this, homeZone.Name);
				MoveZone(homeZone);
			}
		}
		else if (!EClass._zone.IsPCFaction || homeBranch != EClass.Branch || base.c_wasInPcParty)
		{
			EClass.pc.party.AddMemeber(this, showMsg: true);
		}
	}

	public void Revive(Point p = null, bool msg = false)
	{
		if (!isDead)
		{
			return;
		}
		isDead = false;
		base.hp = MaxHP / 3;
		mana.value = 0;
		stamina.value = 0;
		if (hunger.value > 30)
		{
			hunger.value = 30;
		}
		sleepiness.value = 0;
		hostility = OriginalHostility;
		RemoveCondition<StanceTaunt>();
		if (IsPC)
		{
			if (EClass.player.preventDeathPenalty)
			{
				Msg.Say("noDeathPenalty2", this);
			}
			else if (EClass.player.stats.days <= 90 && !EClass.game.principal.disableDeathPenaltyProtection)
			{
				Msg.Say("noDeathPenalty", this);
			}
			else
			{
				EClass.pc.ApplyDeathPenalty();
			}
			List<Thing> dropList = new List<Thing>();
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				if (!t.IsContainer && t.SelfWeight > EClass.pc.WeightLimit && t.trait.CanBeDropped && (t.parentCard.c_lockLv == 0 || t.parentCard.trait is TraitChestPractice))
				{
					t.ignoreAutoPick = true;
					dropList.Add(t);
				}
			}, onlyAccessible: false);
			foreach (Thing item in dropList)
			{
				EClass._zone.AddCard(item, EClass.pc.pos);
				Msg.Say("backpack_full_drop", item);
			}
			EClass.player.preventDeathPenalty = false;
		}
		if (IsPCFaction && homeBranch != null)
		{
			homeBranch.Log("bRevive", this);
		}
		if (p != null)
		{
			if (!p.IsInBounds)
			{
				p.Set(EClass._map.GetCenterPos().GetNearestPoint() ?? EClass._map.GetCenterPos());
			}
			EClass._zone.AddCard(this, p);
			if (msg)
			{
				SE.Play("revive");
				Msg.Say("revive", this);
				PlayEffect("revive");
			}
			SetCensored(enable: false);
		}
	}

	public void MakeGrave(string lastword)
	{
		if (!EClass._zone.IsRegion)
		{
			List<string> list = new List<string> { "930", "931", "947", "948", "949", "950", "951", "952" };
			if (IsPC && EClass.rnd(2) == 0 && EClass.pc.things.Find("letter_will") != null)
			{
				list = new List<string> { "944", "946", "backerGrave", "backerGrave2" };
			}
			Thing thing = ThingGen.Create(list.RandomItem());
			thing.MakeRefFrom(this);
			if (!lastword.IsEmpty())
			{
				thing.c_note = lastword;
			}
			thing.isModified = true;
			EClass._zone.AddCard(thing, pos).Install();
		}
	}

	public void ApplyDeathPenalty()
	{
		if (!IsPC)
		{
			return;
		}
		int currency = GetCurrency();
		if (currency > 0)
		{
			int num = currency / 3 + EClass.rnd(currency / 3 + 1);
			if (num <= 0)
			{
				num = 1;
			}
			Msg.Say("panaltyMoney", this, Lang._currency(num));
			ModCurrency(-num);
			EClass._zone.AddCard(ThingGen.CreateCurrency(num), EClass.pc.pos);
		}
		foreach (Element value in EClass.pc.elements.dict.Values)
		{
			if (EClass.rnd(5) == 0 && value.IsMainAttribute)
			{
				EClass.pc.elements.ModExp(value.id, -500);
			}
		}
	}

	public void Vomit()
	{
		base.c_vomit++;
		Say("vomit", this);
		PlaySound("vomit");
		if (!EClass._zone.IsRegion)
		{
			Thing thing = ThingGen.Create("731");
			if (!EClass._zone.IsPCFaction)
			{
				thing.MakeRefFrom(this);
			}
			EClass._zone.AddCard(thing, pos);
		}
		if (HasCondition<ConAnorexia>())
		{
			ModExp(70, -50);
			ModExp(71, -75);
			ModExp(77, -100);
		}
		else if (base.c_vomit > 10)
		{
			AddCondition<ConAnorexia>();
		}
		AddCondition<ConDim>();
		if (HasCondition<ConAnorexia>())
		{
			ModWeight(-1 * (1 + EClass.rnd(5)));
		}
		if (hunger.GetPhase() >= 4)
		{
			DamageHP(9999, AttackSource.Hunger);
		}
		hunger.Mod(30);
	}

	public override void Die(Element e = null, Card origin = null, AttackSource attackSource = AttackSource.None)
	{
		combatCount = 0;
		if (isDead || host != null)
		{
			return;
		}
		bool isInActiveZone = IsInActiveZone;
		if (isInActiveZone)
		{
			if (IsPC)
			{
				EClass._zone.ResetHostility();
			}
			if (base.isSummon)
			{
				if (id == "tsunami")
				{
					pos.PlaySound("water");
				}
				else
				{
					Say("summon_vanish", this);
					pos.PlayEffect("vanish");
					pos.PlaySound("vanish");
				}
				Destroy();
				return;
			}
			if (attackSource == AttackSource.DeathSentense)
			{
				if (trait is TraitLittleOne)
				{
					MakeEgg();
					if (IsPCFaction)
					{
						EClass.Branch.RemoveMemeber(this);
					}
				}
				PlayEffect("revive");
				PlaySound("chime_angel");
			}
			else
			{
				Effect.Get("blood").Play((parent is Chara) ? (parent as Chara).pos : pos).SetParticleColor(EClass.Colors.matColors[base.material.alias].main)
					.Emit(50);
				AddBlood(AI_Slaughter.slaughtering ? 12 : (2 + EClass.rnd(2)));
				PlaySound(base.material.GetSoundDead(source));
			}
			renderer.RefreshSprite();
			renderer.RefreshStateIcon();
			ClearFOV();
		}
		string text = "";
		"dead_in".langGame(EClass._zone.Name);
		string text2 = ((origin == null) ? "" : origin.GetName(NameStyle.Full));
		if (LangGame.Has("dead_" + attackSource))
		{
			text = "dead_" + attackSource;
		}
		else
		{
			if (e != Element.Void && e != null)
			{
				text = "dead_" + e.source.alias;
			}
			if (text == "" || !LangGame.Has(text))
			{
				text = "dead";
			}
		}
		if (IsPC)
		{
			EClass._zone.isDeathLocation = true;
			string s = ((origin == null) ? text : "dead_by");
			Msg.thirdPerson1.Set(EClass.pc);
			if (attackSource == AttackSource.Wrath)
			{
				text2 = Religion.recentWrath.NameShort;
			}
			EClass.player.deathMsg = GameLang.Parse(s.langGame(), thirdPerson: true, EClass.pc.NameBraced, "dead_in".langGame(EClass._zone.Name), text2);
			Debug.Log(EClass.player.deathMsg);
			if (EClass._zone.instance is ZoneInstanceRandomQuest zoneInstanceRandomQuest)
			{
				zoneInstanceRandomQuest.status = ZoneInstance.Status.Fail;
			}
			AI_PlayMusic.keepPlaying = false;
		}
		if (isInActiveZone)
		{
			if (attackSource == AttackSource.DeathSentense)
			{
				Msg.Say("goto_heaven", this);
			}
			else
			{
				if (origin == null || !origin.isSynced || (attackSource != AttackSource.Melee && attackSource != AttackSource.Range))
				{
					Msg.Say(text, this, "", text2);
				}
				string text3 = TalkTopic("dead");
				if (!text3.IsEmpty())
				{
					text3 = text3.StripBrackets();
				}
				bool flag = base.rarity >= Rarity.Legendary && !IsPCFaction;
				if (!IsPC && flag)
				{
					MakeGrave(text3);
				}
				Msg.SetColor();
				SpawnLoot(origin);
			}
			if (IsPC)
			{
				TryDropCarryOnly();
			}
		}
		if (IsPCFaction)
		{
			if (homeBranch != null)
			{
				homeBranch.Log(text, this, "");
			}
			WidgetPopText.Say("popDead".lang(base.Name), FontColor.Bad);
			if (!IsPC)
			{
				if (EClass.player.stats.allyDeath == 0)
				{
					Tutorial.Reserve("death_pet");
				}
				EClass.player.stats.allyDeath++;
			}
		}
		if (id == "mandrake")
		{
			Say("a_scream", this);
			ActEffect.ProcAt(EffectId.Scream, base.LV * 3 + 200, BlessedState.Normal, this, this, pos, isNeg: true);
		}
		daysStarved = 0;
		isDead = true;
		ClearTempElements();
		enemy = null;
		_cooldowns = null;
		base.isSale = false;
		EClass._map.props.sales.Remove(this);
		Cure(CureType.Death);
		SetAI(new NoGoal());
		TryDropBossLoot();
		if (isInActiveZone && EClass._zone.HasLaw && IsHuman && OriginalHostility >= Hostility.Neutral)
		{
			pos.TalkWitnesses(origin?.Chara, "witness", 3, WitnessType.crime, (Chara c) => !c.IsPCParty && !c.IsUnique);
		}
		if (IsPC)
		{
			EClass.player.returnInfo = null;
			EClass.player.uidLastTravelZone = 0;
			foreach (Chara chara2 in EClass._map.charas)
			{
				if (chara2.IsHostile())
				{
					chara2.hostility = chara2.OriginalHostility;
				}
				if (chara2.enemy == EClass.pc)
				{
					chara2.enemy = null;
				}
			}
			if (EClass.pc.things.Find("letter_will") != null && EClass.rnd(10) == 0)
			{
				EClass.player.preventDeathPenalty = true;
			}
		}
		else
		{
			bool flag2 = currentZone.IsActiveZone && IsLocalChara && !currentZone.IsPCFaction;
			if (currentZone is Zone_LittleGarden && id == "littleOne")
			{
				flag2 = false;
			}
			if (flag2)
			{
				EClass._map.deadCharas.Add(this);
			}
			currentZone.RemoveCard(this);
		}
		if ((origin != null && origin.IsPCParty) || IsPCFaction)
		{
			int a = -10;
			if (IsPCFaction && !IsPCParty && (origin == null || !origin.IsPCParty))
			{
				a = -5;
			}
			ModAffinity(EClass.pc, a, show: false);
			string text4 = id;
			if (!(text4 == "quru"))
			{
				if (text4 == "corgon")
				{
					EClass.game.cards.globalCharas.Find("quru")?.ModAffinity(EClass.pc, -20, show: false);
				}
			}
			else
			{
				EClass.game.cards.globalCharas.Find("corgon")?.ModAffinity(EClass.pc, -20, show: false);
			}
		}
		if (origin != null)
		{
			if (origin.IsPCParty || origin.IsPCPartyMinion)
			{
				int num = 0;
				if (OriginalHostility >= Hostility.Friend && IsHuman && !base.IsPCFactionOrMinion)
				{
					num = -5;
				}
				else if (race.id == "cat" && OriginalHostility >= Hostility.Neutral)
				{
					EClass.pc.Say("killcat");
					num = -1;
				}
				if (EClass.pc.party.HasElement(1563) && num < 0)
				{
					num = num * 30 / 100;
				}
				if (num != 0)
				{
					EClass.player.ModKarma(num);
				}
			}
			if (origin == EClass.pc)
			{
				EClass.pc.faith.Revelation("kill", 10);
			}
			else if (origin.IsPCFaction)
			{
				origin.Chara.ModAffinity(EClass.pc, 1, show: false);
				origin.Chara.ShowEmo(Emo.love);
			}
		}
		if (base.sourceBacker != null && origin != null && origin.IsPCParty)
		{
			EClass.player.doneBackers.Add(base.sourceBacker.id);
		}
		if (base.IsPCFactionOrMinion)
		{
			if (!IsPC && !IsMinion && EClass.game.config.autoCombat.abortOnAllyDead && EClass.player.TryAbortAutoCombat())
			{
				Msg.Say("abort_allyDead");
			}
		}
		else if (EClass.game.config.autoCombat.abortOnEnemyDead && EClass.player.TryAbortAutoCombat(immediate: false))
		{
			Msg.Say("abort_enemyDead");
		}
		if (IsPCParty && !IsPC)
		{
			EClass.pc.party.RemoveMember(this);
			base.c_wasInPcParty = true;
			EClass.pc.Say("allyDead");
		}
		switch (id)
		{
		case "littleOne":
			if (attackSource != AttackSource.DeathSentense && !IsPCFaction)
			{
				EClass.player.flags.little_killed = true;
				EClass.player.little_dead++;
			}
			break;
		case "big_daddy":
			if (!IsPCFaction)
			{
				Chara t2 = CharaGen.Create("littleOne");
				EClass._zone.AddCard(t2, pos.Copy());
				Msg.Say("little_pop");
			}
			break;
		case "shark_sister":
			if (!IsPCFaction)
			{
				Chara t = CharaGen.Create("sister_shark");
				EClass._zone.AddCard(t, pos.Copy());
				Msg.Say("shark_pop");
			}
			break;
		}
		if (attackSource == AttackSource.Finish && origin != null && origin.Evalue(665) > 0)
		{
			Chara chara = CharaGen.CreateFromFilter("c_plant", base.LV);
			EClass._zone.AddCard(chara, pos.Copy());
			if (chara.LV < base.LV)
			{
				chara.SetLv(base.LV);
			}
			chara.MakeMinion((origin.IsPCParty || origin.IsPCPartyMinion) ? EClass.pc : origin.Chara, MinionType.Friend);
			Msg.Say("plant_pop", this, chara);
		}
		foreach (ZoneEvent item in EClass._zone.events.list)
		{
			item.OnCharaDie(this);
		}
	}

	public void TryDropBossLoot()
	{
		if (IsPCFaction || IsPCFactionMinion)
		{
			return;
		}
		int num = 0;
		bool flag = false;
		bool flag2 = false;
		Point point = pos.GetNearestPoint(allowBlock: true, allowChara: false, allowInstalled: false, ignoreCenter: true) ?? pos;
		TreasureType type = TreasureType.BossQuest;
		if (EClass._zone.Boss == this)
		{
			type = TreasureType.BossNefia;
			num = 2 + EClass.rnd(2);
			flag = (flag2 = true);
			EClass._zone.Boss = null;
			Msg.Say("boss_win", EClass._zone.Name);
			if (EClass._zone is Zone_Void && (!EClass.player.flags.KilledBossInVoid || EClass.debug.enable))
			{
				Msg.Say("boss_win_void", EClass._zone.Name);
				EClass.player.flags.KilledBossInVoid = true;
			}
			if (EClass._zone.IsNefia)
			{
				EClass._zone.GetTopZone().isConquered = true;
				EClass.Sound.StopBGM(2f);
				SE.Play("Jingle/fanfare2");
				EClass._zone.SetBGM(114);
			}
			EClass.player.ModFame(EClass.rndHalf(30 + EClass._zone.DangerLv * 2));
			EClass.player.ModKarma(5);
		}
		switch (id)
		{
		case "vernis_boss":
			num = 5;
			flag = (flag2 = true);
			EClass.Sound.StopBGM(3f);
			EClass._zone.SetBGM(1, refresh: false);
			if (EClass.game.quests.GetPhase<QuestVernis>() == 8)
			{
				EClass.game.quests.Get<QuestVernis>().UpdateOnTalk();
			}
			break;
		case "melilith_boss":
			num = 5;
			flag = (flag2 = true);
			EClass.Sound.StopBGM(3f);
			EClass._zone.SetBGM(1, refresh: false);
			break;
		case "isygarad":
		{
			num = 5;
			flag = (flag2 = true);
			QuestExploration questExploration = EClass.game.quests.Get<QuestExploration>();
			if (questExploration != null)
			{
				SE.Play("kill_boss");
				questExploration.ChangePhase(3);
				EClass.Sound.StopBGM(3f);
				EClass._zone.SetBGM(1, refresh: false);
			}
			break;
		}
		case "swordkeeper":
			num = 10;
			flag = true;
			SE.Play("kill_boss");
			SoundManager.ForceBGM();
			LayerDrama.Activate("_event", "event", "swordkeeper_defeat");
			break;
		}
		if (flag)
		{
			SE.Play("kill_boss");
		}
		if (num != 0)
		{
			EClass.player.willAutoSave = true;
			Thing thing = ThingGen.CreateTreasure("chest_boss", base.LV, type);
			point.SetBlock();
			point.SetObj();
			EClass._zone.AddCard(thing, point).Install();
			ThingGen.TryLickChest(thing);
		}
		if (flag2)
		{
			EClass._zone.AddCard(ThingGen.CreateScroll(8221), pos);
		}
	}

	public void Kick(Point p, bool ignoreSelf = false)
	{
		foreach (Chara item in p.ListCharas())
		{
			Kick(item, ignoreSelf);
		}
	}

	public void Kick(Chara t, bool ignoreSelf = false, bool karmaLoss = true, bool show = true)
	{
		if (!IsAliveInCurrentZone)
		{
			return;
		}
		if (t.IsPC)
		{
			ActionMode.Adv.ClearPlans();
		}
		if (t.host != null)
		{
			return;
		}
		if (t == this)
		{
			if (!ignoreSelf)
			{
				Debug.Log(t.pos.GetNearestPoint());
				if (TryMove(t.pos.GetNearestPoint()) != MoveResult.Success)
				{
					t.MoveImmediate(pos.GetNearestPoint() ?? t.pos);
				}
			}
			return;
		}
		if (show)
		{
			Say("kick", this, t);
		}
		PlaySound("kick");
		if ((t.conSuspend == null || t.conSuspend.uidMachine != 0) && t.trait.CanBePushed && (!t.IsHostile() || EClass.rnd(2) == 0) && !t.noMove && !t.isRestrained)
		{
			t.MoveByForce(t.pos.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: true, ignoreCenter: true), this, !t.pos.IsBlocked);
		}
		if (t.conSleep != null)
		{
			t.conSleep.Kill();
		}
		if (IsPC && t.IsFriendOrAbove() && !t.IsPCFactionOrMinion && karmaLoss)
		{
			EClass.player.ModKarma(-1);
		}
		t.PlayEffect("kick");
	}

	public bool UseAbility(string idAct, Card tc = null, Point pos = null, bool pt = false)
	{
		return UseAbility(elements.GetElement(idAct)?.act ?? ACT.Create(idAct), tc, pos, pt);
	}

	public bool UseAbility(Act a, Card tc = null, Point pos = null, bool pt = false)
	{
		if (!IsPC && HasCooldown(a.id))
		{
			return false;
		}
		if (a.source.proc.TryGet(0) == "Heal" && HasElement(1422))
		{
			List<int> list = new List<int> { 8400, 8401, 8402, 8403, 8404, 8405 };
			int num = list.IndexOf(a.id);
			if (num != -1)
			{
				int num2 = list.TryGet(num + Evalue(1422));
				a = elements.GetElement(num2)?.act ?? ACT.Create(num2);
			}
		}
		int num3 = 1;
		Act.Cost cost = a.GetCost(this);
		a.GetPower(this);
		int i = 1;
		int num4 = 0;
		bool flag = a.IsTargetHostileParty();
		if (IsPC && HasCondition<StanceManaCost>())
		{
			num4 = Evalue(1657);
		}
		_pts.Clear();
		if (a.TargetType.ForceParty)
		{
			pt = true;
		}
		if (pt)
		{
			i = 0;
			if (flag)
			{
				ForeachEnemy(delegate
				{
					i++;
				});
			}
			else
			{
				ForeachParty(delegate
				{
					i++;
				});
			}
		}
		if (a is Spell && IsPC && a.vPotential < i)
		{
			i = 1;
			_pts.Clear();
			_pts.Add(this);
			pt = false;
		}
		int num5 = 100;
		if (!a.TargetType.ForceParty && i > 1)
		{
			num5 = (IsPC ? (i * 100) : (50 + i * 50));
		}
		int num6 = cost.cost * num5 / 100;
		if (cost.type == Act.CostType.MP && Evalue(483) > 0)
		{
			num6 = num6 * 100 / (100 + (int)Mathf.Sqrt(Evalue(483) * 10) * 3);
		}
		if (i == 0)
		{
			if (IsPC)
			{
				Msg.SayNothingHappen();
			}
			return false;
		}
		if (!IsPC && cost.type == Act.CostType.MP && mana.value < 0 && (EClass.rnd(4) != 0 || IsPCFaction || (base.IsPowerful && mana.value < -20)))
		{
			return false;
		}
		if (IsPC)
		{
			if (!Dialog.warned && cost.type == Act.CostType.MP && cost.cost > 0 && mana.value < num6 && !EClass.debug.godMode)
			{
				ActPlan.warning = true;
				Dialog.TryWarnMana(delegate
				{
					if (UseAbility(a, tc, pos, pt))
					{
						EClass.player.EndTurn();
					}
				});
				return false;
			}
			EClass.ui.CloseLayers();
		}
		if ((isConfused && EClass.rnd(4) == 0) || (isBlind && (pt || (pos != null && !pos.Equals(base.pos)) || (tc != null && tc.pos != null && !tc.pos.Equals(base.pos))) && EClass.rnd(2) == 0))
		{
			Say("shakeHead", this);
			return true;
		}
		if (tc != null && tc != this)
		{
			LookAt(tc.pos);
		}
		if (pos != null && !base.pos.Equals(pos))
		{
			LookAt(pos);
		}
		if (a.CanRapidFire && HasElement(1648))
		{
			num3 = 1 + Evalue(1648);
		}
		if (IsPC && cost.cost > 0 && a.Value == 0)
		{
			Msg.SayNothingHappen();
			return false;
		}
		if (a is Spell)
		{
			string s = (isConfused ? "_cast_confuse" : (HasCondition<ConDim>() ? "_cast_dim" : ""));
			if (!a.source.tag.Contains("useHand"))
			{
				Say(race.castStyle.IsEmpty("cast"), this, a.source.GetName().ToLower(), s.lang());
			}
			if (IsPC)
			{
				_ = (i + 1) / 2;
				if (a.vPotential < i)
				{
					Msg.Say("noSpellStock");
					EInput.Consume();
					return false;
				}
				if (num4 > 0 && a.vPotential >= i * 2)
				{
					a.vPotential -= i * 2;
					num6 = num6 * (100 - num4 * 20) / 100;
				}
				else
				{
					a.vPotential -= i;
				}
				LayerAbility.SetDirty(a);
			}
		}
		else if (a.source.langAct.Length != 0 && tc != null)
		{
			string text = a.source.langAct[0];
			string text2 = ((a.source.langAct.Length >= 2) ? a.source.langAct[1] : "");
			if (text == "spell_hand")
			{
				string[] list2 = Lang.GetList("attack" + race.meleeStyle.IsEmpty("Touch"));
				string @ref = text2.lang(list2[4]);
				Say(tc.IsPCParty ? "cast_hand_ally" : "cast_hand", this, tc, @ref, tc.IsPCParty ? list2[1] : list2[2]);
			}
			else
			{
				Say(text, this, tc, text2.IsEmpty() ? "" : text2.lang());
			}
			if (a.source.id == 6630)
			{
				Talk("insult_" + (base.IsMale ? "m" : "f"));
			}
		}
		switch (cost.type)
		{
		case Act.CostType.MP:
			if (Evalue(1421) >= 2 && base.hp <= MaxHP / (9 - Evalue(1421) * 2))
			{
				num6 /= 2;
			}
			PlayEffect("cast");
			mana.Mod(-num6);
			if (isDead)
			{
				return true;
			}
			elements.ModExp(304, Mathf.Clamp(num6 * 2, 1, 200));
			break;
		case Act.CostType.SP:
			stamina.Mod(-num6);
			ignoreSPAbsorb = true;
			break;
		}
		if (a is Spell && GetCondition<ConSilence>() != null)
		{
			Say("cast_silence", this);
			return true;
		}
		if (isDead)
		{
			return true;
		}
		int spellExp = elements.GetSpellExp(this, a, num5);
		if (EClass.rnd(100) >= CalcCastingChance(a, i) && !EClass.debug.godMode)
		{
			Say("fizzle", this);
			PlayEffect("fizzle");
			PlaySound("fizzle");
			if (cost.cost > 0 && a.source.lvFactor > 0)
			{
				ModExp(a.id, spellExp / 5);
			}
			RemoveCondition<ConInvisibility>();
			return true;
		}
		bool flag2 = true;
		if (HasTalk("phrase_" + a.source.alias))
		{
			EClass.player.forceTalk = true;
			Talk("phrase_" + a.source.alias);
		}
		if (pt)
		{
			Act.forcePt = true;
			if (flag)
			{
				ForeachEnemy(delegate(Chara c)
				{
					a.Perform(this, c, c.pos);
				});
			}
			else
			{
				ForeachParty(delegate(Chara c)
				{
					a.Perform(this, c, c.pos);
				});
			}
			Act.forcePt = false;
		}
		else
		{
			for (int j = 0; j < num3; j++)
			{
				if (a.TargetType != TargetType.SelfParty && tc != null && !tc.IsAliveInCurrentZone)
				{
					break;
				}
				ActEffect.RapidCount = j;
				ActEffect.RapidDelay = a.RapidDelay;
				flag2 = a.Perform(this, tc, pos);
			}
		}
		if (flag2 && !isDead)
		{
			if (cost.cost > 0 && a.source.lvFactor > 0)
			{
				ModExp(a.id, spellExp);
			}
			if (a.source.alias.StartsWith("sword_"))
			{
				ModExp(101, 50);
			}
		}
		ActEffect.RapidCount = 0;
		if (!IsPC && a.source.cooldown > 0)
		{
			AddCooldown(a.id, a.source.cooldown);
		}
		if (flag2 && !a.source.tag.Contains("keepInvisi") && EClass.rnd(2) == 0)
		{
			RemoveCondition<ConInvisibility>();
		}
		return flag2;
		void ForeachEnemy(Action<Chara> action)
		{
			if (_pts.Count == 0)
			{
				for (int num10 = EClass._map.charas.Count - 1; num10 >= 0; num10--)
				{
					Chara chara3 = EClass._map.charas[num10];
					if (chara3 != this && CanSeeLos(chara3) && chara3.IsHostile(this))
					{
						_pts.Add(chara3);
					}
				}
			}
			for (int num11 = _pts.Count - 1; num11 >= 0; num11--)
			{
				action(_pts[num11]);
			}
		}
		void ForeachParty(Action<Chara> action)
		{
			if (_pts.Count == 0)
			{
				if (IsPCParty)
				{
					for (int num7 = EClass.pc.party.members.Count - 1; num7 >= 0; num7--)
					{
						Chara chara = EClass.pc.party.members[num7];
						if (chara == this || chara.host != null || CanSeeLos(chara))
						{
							_pts.Add(chara);
						}
					}
				}
				else
				{
					for (int num8 = EClass._map.charas.Count - 1; num8 >= 0; num8--)
					{
						Chara chara2 = EClass._map.charas[num8];
						if ((chara2 == this || (chara2.IsFriendOrAbove(this) && CanSeeLos(chara2))) && (chara2 == tc || _pts.Count < 3 || EClass.rnd(_pts.Count * _pts.Count) > 6))
						{
							_pts.Add(chara2);
						}
					}
				}
			}
			for (int num9 = _pts.Count - 1; num9 >= 0; num9--)
			{
				action(_pts[num9]);
			}
		}
	}

	public int EvalueRiding()
	{
		if (ride != null && ride.HasCondition<ConTransmuteBroom>() && HasElement(1417))
		{
			return 25 + Evalue(226) * 125 / 100;
		}
		return Evalue(226);
	}

	public int CalcCastingChance(Element e, int num = 1)
	{
		if (!(e is Spell))
		{
			return 100;
		}
		if (!IsPC)
		{
			int num2 = 95;
			if (host != null)
			{
				if (host.ride == this)
				{
					return num2 * 100 / (100 + 300 / Mathf.Max(5, 10 + host.EvalueRiding()));
				}
				if (host.parasite == this)
				{
					return num2 * 100 / (100 + 300 / Mathf.Max(5, 10 + host.Evalue(227)));
				}
			}
			return num2;
		}
		int num3 = Evalue(304);
		if (!IsPCFaction)
		{
			num3 = Mathf.Max(num3, base.LV + 5);
		}
		int num4 = 0;
		int num5 = 0;
		bool num6 = GetArmorSkill() == 122;
		AttackStyle attackStyle = body.GetAttackStyle();
		if (num6)
		{
			num4 = 20 - Evalue(122) / 5;
			num5 += 10 - Evalue(1654) * 4;
		}
		else
		{
			num4 = 10 - Evalue(120) / 5;
		}
		if (num4 < 5)
		{
			num4 = 5;
		}
		if (ride != null)
		{
			num4 += 5;
		}
		if (parasite != null)
		{
			num4 += 10;
		}
		if (attackStyle == AttackStyle.TwoWield)
		{
			num4 += 5;
		}
		if (attackStyle == AttackStyle.Shield)
		{
			num4 += 5;
			num5 += 10 - Evalue(1654) * 4;
		}
		if (isConfused)
		{
			num4 += 10000;
		}
		if (HasCondition<ConDim>())
		{
			num4 += ((Evalue(1654) >= 3) ? 1500 : 2500);
		}
		if (num > 1)
		{
			num4 += 5 * num;
		}
		if (num5 < 0)
		{
			num5 = 0;
		}
		return Mathf.Clamp(100 + e.Value - 10 - e.source.LV * e.source.cost[0] * num4 / (10 + num3 * 10), 0, 100 - num5);
	}

	public void DoAI(int wait, Action onPerform)
	{
		SetAI(new DynamicAIAct("", delegate
		{
			onPerform();
			return true;
		})
		{
			wait = wait
		});
	}

	public void Cuddle(Chara c, bool headpat = false)
	{
		Talk("goodBoy");
		Say(headpat ? "headpat" : "cuddle", this, c);
		c.ShowEmo(Emo.love);
		if (EClass.rnd(IsPC ? 100 : 5000) == 0)
		{
			c.MakeEgg();
		}
		if (headpat && this != c)
		{
			if (c.interest > 0)
			{
				c.ModAffinity(EClass.pc, 1 + EClass.rnd(3));
				c.interest -= 20 + EClass.rnd(10);
			}
			if (faith == EClass.game.religions.MoonShadow && c.IsPCParty)
			{
				foreach (Chara member in party.members)
				{
					if (!member.IsPC && CanSeeLos(member))
					{
						member.AddCondition<ConEuphoric>(100 + Evalue(6904) * 5);
					}
				}
			}
		}
		if (c.Evalue(1221) > 0)
		{
			int ele = ((c.MainElement == Element.Void) ? 924 : c.MainElement.id);
			if (c.id == "hedgehog_ether")
			{
				ele = 922;
			}
			Say("reflect_thorne", c, this);
			DamageHP(10, ele, Power, AttackSource.Condition);
		}
	}

	public Chara SetEnemy(Chara c = null)
	{
		enemy = c;
		if (c == null)
		{
			if (ai is GoalCombat)
			{
				ai.Cancel();
			}
		}
		else
		{
			calmCheckTurn = 10 + EClass.rnd(30);
		}
		return c;
	}

	public void TrySetEnemy(Chara c)
	{
		if ((!IsPC || !EClass.game.config.autoCombat.bDontChangeTarget) && (enemy == null || (EClass.rnd(5) == 0 && Dist(c) <= 1)) && ((!IsPCFaction && !IsPCFactionMinion) || (!c.IsPCFaction && !c.IsPCFactionMinion)) && (hostility != Hostility.Enemy || c.hostility != Hostility.Enemy) && (!c.IsPC || hostility < Hostility.Neutral))
		{
			SetEnemy(c);
		}
	}

	private void GoHostile(Card _tg)
	{
		if (enemy == null && !IsPC)
		{
			if (GetInt(106) == 0)
			{
				TalkTopic("aggro");
			}
			if (OriginalHostility != Hostility.Enemy)
			{
				ShowEmo(Emo.angry);
			}
			SetEnemy(_tg.Chara);
		}
		if (!IsPCFaction && !IsPCFactionMinion && (_tg.IsPCFaction || _tg.IsPCFactionMinion))
		{
			if (hostility >= Hostility.Neutral)
			{
				Say("angry", this);
			}
			hostility = Hostility.Enemy;
		}
	}

	public void DoHostileAction(Card _tg, bool immediate = false)
	{
		if (_tg == null || !_tg.isChara)
		{
			return;
		}
		Chara chara = _tg.Chara;
		if (chara.IsPC)
		{
			EClass.pc.combatCount = 10;
		}
		if (!chara.IsAliveInCurrentZone || !IsAliveInCurrentZone || chara == this)
		{
			return;
		}
		if ((IsPCFaction || IsPCFactionMinion) && (chara.IsPCFaction || chara.IsPCFactionMinion))
		{
			chara.Say("frown", this, chara);
		}
		else
		{
			if (EClass._zone.IsRegion)
			{
				return;
			}
			if (IsPC)
			{
				if (chara.IsFriendOrAbove() && !immediate)
				{
					chara.Say("frown", this, chara);
					chara.ShowEmo(Emo.sad);
					chara.hostility = Hostility.Neutral;
					return;
				}
				if (!chara.IsPCFaction && chara.hostility >= Hostility.Neutral && !EClass._zone.IsPCFaction)
				{
					bool flag = chara.id == "fanatic";
					if (EClass.rnd(4) == 0 || flag)
					{
						chara.Say("callHelp", chara);
						chara.CallHelp(this, flag);
					}
				}
				if (chara.hostility <= Hostility.Enemy)
				{
					foreach (Chara member in EClass.pc.party.members)
					{
						if (member != EClass.pc && member.enemy == null && member.CanSee(chara))
						{
							member.SetEnemy(chara);
						}
					}
				}
			}
			else if (chara.IsPC && hostility <= Hostility.Enemy)
			{
				foreach (Chara member2 in EClass.pc.party.members)
				{
					if (member2 != EClass.pc && member2.enemy == null && member2.CanSee(this))
					{
						member2.SetEnemy(this);
					}
				}
			}
			if (chara.calmCheckTurn <= 0 || IsPC)
			{
				chara.calmCheckTurn = (IsPC ? (20 + EClass.rnd(30)) : (10 + EClass.rnd(10)));
			}
			if (hostility != Hostility.Enemy || chara.hostility != Hostility.Enemy)
			{
				GoHostile(chara);
				chara.GoHostile(this);
				if (base.isHidden && !chara.CanSee(this) && !chara.IsDisabled && !chara.IsPCParty && !chara.IsPCPartyMinion && EClass.rnd(10 + Dist(chara) * 10) == 0)
				{
					Thing t = ThingGen.Create("49");
					ActThrow.Throw(chara, pos, t);
					Act.TC = chara;
				}
			}
		}
	}

	public void CallHelp(Chara tg, bool fanatic = false)
	{
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsPCFaction && chara.OriginalHostility == OriginalHostility && (fanatic || Dist(chara) <= 6) && (EClass.rnd(3) != 0 || fanatic))
			{
				chara.GoHostile(tg);
			}
		}
	}

	public bool FindNewEnemy()
	{
		if (EClass._zone.isPeace && base.IsPCFactionOrMinion && !IsPC)
		{
			return false;
		}
		if (enemy != null && !enemy.IsAliveInCurrentZone)
		{
			enemy = null;
		}
		if (enemy != null)
		{
			return false;
		}
		if (IsPCParty && EClass.pc.isHidden && base.isHidden)
		{
			return false;
		}
		bool flag = enemy != null || ai is GoalCombat;
		int num = (base.PER + Evalue(210) * 2) * ((!flag) ? 1 : 2);
		bool flag2 = IsPCParty && !IsPC && EClass.game.config.tactics.dontWander;
		bool flag3 = !IsPCParty;
		Chara chara = null;
		int num2 = 9999;
		for (int i = 0; i < EClass._map.charas.Count; i++)
		{
			Chara chara2 = EClass._map.charas[i];
			if (chara2 == this || !IsHostile(chara2) || !CanSee(chara2))
			{
				continue;
			}
			int num3 = Dist(chara2);
			int num4 = GetSightRadius() + (flag ? 1 : 0);
			if (num3 > num4)
			{
				continue;
			}
			if (flag3 && EClass.rnd(chara2.Evalue(152) + 5) * (100 + num3 * num3 * 10) / 100 > EClass.rnd(num))
			{
				if (this == pos.FirstChara)
				{
					chara2.ModExp(152, Mathf.Clamp((num - chara2.Evalue(152)) / 2, 1, Mathf.Max(30 - stealthSeen * 2, 1)));
				}
				stealthSeen++;
			}
			else if (CanSeeLos(chara2) && (!flag2 || EClass.pc.isBlind || EClass.pc.CanSeeLos(chara2)) && (!IsPCFaction || EClass.pc.ai.ShouldAllyAttack(chara2)))
			{
				if (!IsPCParty)
				{
					DoHostileAction(chara2);
					enemy = chara2;
					return true;
				}
				if (num3 < num2)
				{
					num2 = num3;
					chara = chara2;
				}
			}
		}
		if (chara != null)
		{
			DoHostileAction(chara);
			enemy = chara;
			return true;
		}
		return false;
	}

	public bool FindNearestNewEnemy()
	{
		for (int i = 0; i < EClass._map.charas.Count; i++)
		{
			Chara chara = EClass._map.charas[i];
			if (chara != this && chara != enemy && IsHostile(chara) && Dist(chara) <= 1 && CanInteractTo(chara.pos))
			{
				DoHostileAction(chara);
				enemy = chara;
				return true;
			}
		}
		return false;
	}

	public bool IsHostile()
	{
		return hostility <= Hostility.Enemy;
	}

	public bool IsHostile(Chara c)
	{
		if (c == null)
		{
			return false;
		}
		if (base.IsPCFactionOrMinion)
		{
			if ((c == EClass.pc.enemy && !c.IsPCFactionOrMinion) || c.hostility <= Hostility.Enemy)
			{
				return true;
			}
		}
		else
		{
			if (trait is TraitGuard && c.IsPCParty && EClass.player.IsCriminal && EClass._zone.instance == null)
			{
				return true;
			}
			if (OriginalHostility >= Hostility.Friend)
			{
				if (c.hostility <= Hostility.Enemy && c.OriginalHostility == Hostility.Enemy)
				{
					return true;
				}
			}
			else if (OriginalHostility <= Hostility.Enemy && (c.IsPCFactionOrMinion || (c.OriginalHostility != Hostility.Enemy && c.hostility >= Hostility.Friend)))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsNeutral()
	{
		return hostility == Hostility.Neutral;
	}

	public bool IsNeutralOrAbove()
	{
		return hostility >= Hostility.Neutral;
	}

	public bool IsBranchMember()
	{
		if (faction == EClass.Home)
		{
			return homeZone == EClass.game.activeZone;
		}
		return false;
	}

	public bool IsHomeMember()
	{
		return faction == EClass.Home;
	}

	public bool IsInHomeZone()
	{
		return EClass.game.activeZone == currentZone;
	}

	public bool IsInSpot<T>() where T : TraitSpot
	{
		foreach (T item in EClass._map.props.installed.traits.List<T>())
		{
			foreach (Point item2 in item.ListPoints())
			{
				if (pos.Equals(item2))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsGuest()
	{
		return memberType == FactionMemberType.Guest;
	}

	public bool IsFriendOrAbove()
	{
		return hostility >= Hostility.Friend;
	}

	public bool IsFriendOrAbove(Chara c)
	{
		if (base.IsPCFactionOrMinion || IsFriendOrAbove())
		{
			if (c.IsPCFactionOrMinion || c.IsFriendOrAbove())
			{
				return true;
			}
		}
		else if (IsHostile() && c.IsHostile())
		{
			return true;
		}
		return false;
	}

	public override CardRenderer _CreateRenderer()
	{
		CardRenderer cardRenderer = renderer;
		CharaRenderer charaRenderer = new CharaRenderer();
		if (race.id == "spider" && source.tiles.Length > 1)
		{
			base.idSkin = (EClass.core.config.game.antiSpider ? 1 : 0);
		}
		if (host != null)
		{
			charaRenderer.pccData = PCCData.Create("ride");
			string idPart = base.c_idRidePCC.IsEmpty(EClass.core.pccs.sets["ride"].map["body"].map.Keys.First());
			charaRenderer.pccData.SetPart("body", "ride", idPart);
			charaRenderer.pccData.ride = EClass.core.pccs.GetRideData(idPart);
		}
		else
		{
			foreach (Condition condition in conditions)
			{
				RendererReplacer rendererReplacer = condition.GetRendererReplacer();
				if (rendererReplacer != null)
				{
					charaRenderer.replacer = rendererReplacer;
					charaRenderer.data = rendererReplacer.data;
					break;
				}
			}
			if (charaRenderer.replacer == null)
			{
				charaRenderer.pccData = pccData;
			}
		}
		renderer = charaRenderer;
		renderer.SetOwner(this);
		if (cardRenderer != null)
		{
			renderer.SetFirst(first: false, cardRenderer.position);
		}
		return charaRenderer;
	}

	public void SetPCCState(PCCState state)
	{
		if (IsPCC)
		{
			PCC.Get(pccData).Build(state);
		}
	}

	public override Sprite GetSprite(int dir = 0)
	{
		if (IsPCC)
		{
			PCC pCC = PCC.Get(pccData);
			pCC.Build();
			return pCC.variation.idle[0, 0];
		}
		if (spriteReplacer != null)
		{
			return spriteReplacer.data.GetSprite();
		}
		return sourceCard.GetSprite(0, (sourceCard._tiles.Length > 1) ? ((base.idSkin != 0 || source.staticSkin) ? base.idSkin : (base.uid % sourceCard._tiles.Length / 2 * 2 + ((!base.IsMale) ? 1 : 0))) : 0);
	}

	public void SetTempHand(int right = 0, int left = 0)
	{
		if (IsPC && !IsPC)
		{
			pccData.tempRight = EClass.scene.screenElin.renderTempEQ.ConvertTile(right);
			pccData.tempLeft = EClass.scene.screenElin.renderTempEQ.ConvertTile(left);
		}
	}

	public override SubPassData GetSubPassData()
	{
		if (IsPCC)
		{
			if (IsDeadOrSleeping || (!EClass.player.altHeldPos && parent is Chara))
			{
				return EClass.setting.pass.subDeadPCC;
			}
		}
		else if (conSleep != null && host == null && pos.Equals(EClass.pc.pos) && IsHuman && GetBool(123))
		{
			return EClass.setting.pass.subDead;
		}
		return SubPassData.Default;
	}

	public override void SetRenderParam(RenderParam p)
	{
		p.mat = base.material;
		p.matColor = base.colorInt;
		if (!renderer.usePass)
		{
			return;
		}
		if (renderer.replacer != null)
		{
			p.tile = renderer.replacer.tile * ((!flipX) ? 1 : (-1));
		}
		else if (source._tiles_snow.Length != 0 && EClass._zone.IsSnowCovered)
		{
			if (source._tiles_snow.Length > 1)
			{
				int num = ((base.idSkin != 0 || source.staticSkin) ? base.idSkin : (base.uid % source._tiles_snow.Length / 2 * 2 + ((!base.IsMale) ? 1 : 0)));
				p.tile = source._tiles_snow[(num < source._tiles_snow.Length) ? num : 0] * ((!flipX) ? 1 : (-1));
			}
			else
			{
				p.tile = source._tiles_snow[0] * ((!flipX) ? 1 : (-1));
			}
		}
		else if (sourceCard._tiles.Length > 1)
		{
			int num2 = ((base.idSkin != 0 || source.staticSkin) ? base.idSkin : (base.uid % sourceCard._tiles.Length / 2 * 2 + ((!base.IsMale) ? 1 : 0)));
			p.tile = sourceCard._tiles[(num2 >= 0 && num2 < sourceCard._tiles.Length) ? num2 : 0] * ((!flipX) ? 1 : (-1));
		}
		else
		{
			p.tile = sourceCard._tiles[0] * ((!flipX) ? 1 : (-1));
		}
		p.dir = base.dir;
	}

	public override string GetHoverText()
	{
		string text = base.Name;
		if (IsFriendOrAbove())
		{
			text = text.TagColor(EClass.Colors.colorFriend);
		}
		else if (IsHostile())
		{
			text = text.TagColor(EClass.Colors.colorHostile);
		}
		int num = 2;
		int lV = EClass.pc.LV;
		if (base.LV >= lV * 5)
		{
			num = 0;
		}
		else if (base.LV >= lV * 2)
		{
			num = 1;
		}
		else if (base.LV <= lV / 4)
		{
			num = 4;
		}
		else if (base.LV <= lV / 2)
		{
			num = 3;
		}
		string text2 = Lang.GetList("lvComparison")[num];
		text2 = (" (" + text2 + ") ").TagSize(14).TagColor(EClass.Colors.gradientLVComparison.Evaluate(0.25f * (float)num));
		string s = (IsFriendOrAbove() ? "HostilityAlly" : (IsNeutral() ? "HostilityNeutral" : "HostilityEnemy"));
		s = (" (" + s.lang() + ") ").TagSize(14);
		s = "";
		if (memberType == FactionMemberType.Guest)
		{
			s += (" (" + "guest".lang() + ") ").TagSize(14);
		}
		else if (memberType == FactionMemberType.Livestock)
		{
			s += (" (" + "livestock".lang() + ") ").TagSize(14);
		}
		if (!EClass.pc.IsMoving)
		{
			if (EClass.pc.HasHigherGround(this))
			{
				text2 += "lowerGround".lang();
			}
			else if (HasHigherGround(EClass.pc))
			{
				text2 += "higherGround".lang();
			}
		}
		if (Evalue(1232) > 0)
		{
			text2 = "milkBaby".lang().TagSize(14) + text2;
		}
		if (Guild.Fighter.ShowBounty(this) && Guild.Fighter.HasBounty(this))
		{
			text2 = "hasBounty".lang().TagSize(14) + text2;
		}
		if (EClass.pc.HasElement(481))
		{
			text2 += ("(" + faith.Name + ")").TagSize(14);
		}
		return text + text2 + s;
	}

	public override string GetHoverText2()
	{
		string text = "";
		if (knowFav)
		{
			text += Environment.NewLine;
			text = text + "<size=14>" + "favgift".lang(GetFavCat().GetName().ToLower(), GetFavFood().GetName()) + "</size>";
		}
		string text2 = "";
		if (EClass.debug.showExtra)
		{
			text2 += Environment.NewLine;
			text2 = text2 + "Lv:" + base.LV + "  HP:" + base.hp + "/" + MaxHP + "  MP:" + mana.value + "/" + mana.max + "  DV:" + DV + "  PV:" + PV + "  Hunger:" + hunger.value;
			text2 += Environment.NewLine;
			text2 = text2 + "Global:" + IsGlobal + "  AI:" + ai?.ToString() + " " + source.tactics.IsEmpty(EClass.sources.tactics.map.TryGetValue(id)?.id ?? EClass.sources.tactics.map.TryGetValue(job.id)?.id ?? "predator");
			text2 += Environment.NewLine;
			text2 = text2 + base.uid + IsMinion + "/" + base.c_uidMaster + "/" + master;
			text2 = text2 + " dir:" + base.dir + " skin:" + base.idSkin;
		}
		if (EClass.pc.held?.trait is TraitWhipLove && IsPCFaction)
		{
			text2 += Environment.NewLine;
			text2 += "<size=14>";
			foreach (Hobby item in ListWorks())
			{
				text2 = text2 + item.Name + ", ";
			}
			foreach (Hobby item2 in ListHobbies())
			{
				text2 = text2 + item2.Name + ", ";
			}
			text2 = text2.TrimEnd(", ".ToCharArray()) + "</size>";
		}
		string text3 = "";
		IEnumerable<BaseStats> enumerable = conditions.Concat((!IsPCFaction) ? new BaseStats[0] : new BaseStats[2] { hunger, stamina });
		if (enumerable.Count() > 0)
		{
			text3 += Environment.NewLine;
			text3 += "<size=14>";
			int num = 0;
			foreach (BaseStats item3 in enumerable)
			{
				string text4 = item3.GetPhaseStr();
				if (text4.IsEmpty() || text4 == "#")
				{
					continue;
				}
				Color c = Color.white;
				switch (item3.source.group)
				{
				case "Bad":
				case "Debuff":
				case "Disease":
					c = EClass.Colors.colorDebuff;
					break;
				case "Buff":
					c = EClass.Colors.colorBuff;
					break;
				}
				if (EClass.debug.showExtra)
				{
					text4 = text4 + "(" + item3.GetValue() + ")";
					if (resistCon != null && resistCon.ContainsKey(item3.id))
					{
						text4 = text4 + "{" + resistCon[item3.id] + "}";
					}
				}
				num++;
				text3 = text3 + text4.TagColor(c) + ", ";
			}
			if (num == 0)
			{
				text3 = "";
			}
			else
			{
				text = "";
				text3 = text3.TrimEnd(", ".ToCharArray()) + "</size>";
			}
		}
		if (EClass.debug.showExtra)
		{
			text3 += Environment.NewLine;
			foreach (ActList.Item item4 in ability.list.items)
			{
				text3 = text3 + item4.act.Name + ", ";
			}
			text3 = text3.TrimEnd(", ".ToCharArray());
		}
		return text + text2 + text3;
	}

	public string GetTopicText(string topic = "calm")
	{
		string key = source.idText.IsEmpty(id);
		if (id == "littleOne" && EClass._zone is Zone_LittleGarden)
		{
			key = "littleOne2";
		}
		SourceCharaText.Row row = EClass.sources.charaText.map.TryGetValue(key);
		if (row == null)
		{
			return null;
		}
		string text = row.GetText(topic, returnNull: true);
		if (text.IsEmpty())
		{
			return null;
		}
		if (text.StartsWith("@"))
		{
			row = EClass.sources.charaText.map.TryGetValue(text.Replace("@", ""));
			if (row == null)
			{
				return null;
			}
			text = row.GetText(topic, returnNull: true);
			if (text.IsEmpty())
			{
				return null;
			}
		}
		return text.Split(Environment.NewLine.ToCharArray()).RandomItem();
	}

	public string TalkTopic(string topic = "calm")
	{
		if (host == null && !IsInActiveZone)
		{
			return null;
		}
		if (!isSynced && (host == null || !host.isSynced) && topic != "dead")
		{
			return null;
		}
		if (IsPCParty)
		{
			int num = EClass.pc.party.members.Count - 1;
			switch (topic)
			{
			case "calm":
				if (EClass.rnd(num * 5) != 0)
				{
					return null;
				}
				break;
			case "aggro":
				if (EClass.rnd(num * 10) != 0)
				{
					return null;
				}
				break;
			case "kill":
				if (EClass.rnd(num * 3) != 0)
				{
					return null;
				}
				break;
			case "fov":
				return null;
			}
		}
		string topicText = GetTopicText(topic);
		if (topicText.IsEmpty())
		{
			return null;
		}
		string text = "_bracketTalk".lang();
		bool flag = topicText.StartsWith("*");
		bool flag2 = topicText.StartsWith("(");
		bool flag3 = topicText.StartsWith(text) || (topicText.Length > 0 && topicText[0] == text[0]) || topicText[0] == '“';
		topicText = ApplyTone(topicText);
		topicText = topicText.Replace("~", "*");
		Msg.SetColor(flag2 ? Msg.colors.Thinking : (flag3 ? Msg.colors.Talk : Msg.colors.Ono));
		Msg.Say(topicText.Replace("&", ""));
		if (topic == "dead")
		{
			EClass.ui.popGame.PopText(ApplyNewLine(topicText.StripBrackets()), null, "PopTextDead", default(Color), pos.Position() + EClass.setting.render.tc.textPosDead);
		}
		else if (flag || flag3 || flag2)
		{
			(host ?? this).renderer.Say(ApplyNewLine(topicText.StripBrackets()), default(Color), IsPCParty ? 0.6f : 0f);
		}
		return topicText;
	}

	public override Sprite GetImageSprite()
	{
		return GetSprite();
	}

	public void ChangeMemberType(FactionMemberType type)
	{
		memberType = type;
	}

	public void ShowDialog()
	{
		Zone_Nymelle zone_Nymelle = EClass._zone as Zone_Nymelle;
		if (conSuspend != null && IsPCParty)
		{
			RemoveCondition<ConSuspend>();
		}
		if (IsDeadOrSleeping)
		{
			ShowDialog("_chara", "sleep");
		}
		else if (base.isRestrained)
		{
			ShowDialog("_chara", "strain");
		}
		else if (EClass.pc.isHidden && !CanSee(EClass.pc))
		{
			ShowDialog("_chara", "invisible");
		}
		else if (IsEscorted())
		{
			ShowDialog("_chara", "escort");
		}
		else if (EClass._zone is Zone_Music)
		{
			ShowDialog("_chara", "party");
		}
		else
		{
			if (LayerDrama.forceJump == null && EClass.game.quests.OnShowDialog(this))
			{
				return;
			}
			switch (id)
			{
			case "loytel":
			{
				if (EClass.player.flags.loytelEscaped)
				{
					EClass.game.quests.Get("pre_debt_runaway").Complete();
					EClass.player.flags.loytelEscaped = false;
					EClass.game.quests.Add("debt", "loytel");
					ShowDialog("loytel", "loytelEscaped");
					return;
				}
				QuestDebt questDebt = EClass.game.quests.Get<QuestDebt>();
				if (questDebt != null && questDebt.paid)
				{
					questDebt.stage++;
					if (questDebt.stage > 7)
					{
						questDebt.stage = 7;
					}
					ShowDialog("loytel", "debt" + questDebt.stage);
					return;
				}
				break;
			}
			case "farris":
				if (EClass._zone.id == "startVillage" || EClass._zone.id == "startVillage3")
				{
					ShowDialog("_chara");
					return;
				}
				switch (EClass.game.quests.GetPhase<QuestExploration>())
				{
				case -1:
					ShowDialog("farris", "nymelle_noQuest");
					break;
				case 0:
					ShowDialog("farris", "nymelle_first");
					break;
				case 1:
					ShowDialog("farris", "home_first");
					break;
				default:
					ShowDialog("_chara");
					break;
				}
				return;
			case "ashland":
				if (zone_Nymelle != null && zone_Nymelle.IsCrystalLv)
				{
					SoundManager.ForceBGM();
					LayerDrama.ActivateMain("mono", "nymelle_crystal");
				}
				else
				{
					ShowDialog("ashland");
				}
				return;
			case "fiama":
				if (zone_Nymelle != null && zone_Nymelle.IsCrystalLv)
				{
					SoundManager.ForceBGM();
					LayerDrama.ActivateMain("mono", "nymelle_crystal");
				}
				else if (EClass.player.EnableDreamStory)
				{
					if (!EClass.player.flags.fiamaFirstDream && EClass.player.flags.storyFiama >= 10)
					{
						EClass.player.flags.fiamaFirstDream = true;
						ShowDialog("fiama", "firstDream");
					}
					else if (!EClass.player.flags.fiamaStoryBookGiven && EClass.player.flags.storyFiama >= 30)
					{
						ShowDialog("fiama", "giveStoryBook").SetOnKill(delegate
						{
							EClass.player.flags.fiamaStoryBookGiven = true;
							EClass.player.DropReward(ThingGen.Create("book_story"));
						});
					}
					else
					{
						ShowDialog("fiama");
					}
				}
				else
				{
					ShowDialog("fiama");
				}
				return;
			case "big_sister":
				if (EClass.player.flags.little_saved)
				{
					ShowDialog("big_sister", "little_saved");
					EClass.player.flags.little_saved = false;
					return;
				}
				if (EClass.player.flags.little_killed)
				{
					ShowDialog("big_sister", "little_dead");
					EClass.player.flags.little_killed = false;
					return;
				}
				break;
			}
			if (trait is TraitGuildDoorman)
			{
				string tag = ((trait is TraitDoorman_Fighter) ? "fighter" : ((trait is TraitDoorman_Mage) ? "mage" : "thief"));
				ShowDialog("guild_doorman", "main", tag);
				return;
			}
			if (trait is TraitGuildClerk)
			{
				string tag2 = ((trait is TraitClerk_Fighter) ? "fighter" : ((trait is TraitClerk_Mage) ? "mage" : "thief"));
				ShowDialog("guild_clerk", "main", tag2);
				return;
			}
			bool flag = true;
			if (id == "parttimer_jure" && (!EClass._zone.IsFestival || !(EClass._zone is Zone_Noyel) || EClass.pc.faith == EClass.game.religions.Healing))
			{
				flag = false;
			}
			if (flag && File.Exists(CorePath.DramaData + id + ".xlsx"))
			{
				ShowDialog(id);
			}
			else
			{
				ShowDialog("_chara");
			}
		}
	}

	public LayerDrama ShowDialog(string book, string step = "main", string tag = "")
	{
		return _ShowDialog(book, null, step, tag);
	}

	private LayerDrama _ShowDialog(string book, string sheet, string step = "main", string tag = "")
	{
		EClass.Sound.Play("pop_drama");
		if (book == "_chara" && IsPC)
		{
			step = "pc";
		}
		return LayerDrama.Activate(book, sheet, step, this, null, tag);
	}

	public Point GetDestination()
	{
		return (ai.IsRunning ? ai.GetDestination() : pos).Copy();
	}

	public int GetHireCost()
	{
		return base.LV / 2 + 4;
	}

	public int GetHappiness()
	{
		int num = 50;
		if (FindBed() != null)
		{
			num += 50;
		}
		return num;
	}

	public string GetTextHappiness()
	{
		return GetHappiness().ToString() ?? "";
	}

	public string GetActionText()
	{
		string result = "?????";
		if (ai != null)
		{
			result = ai.GetCurrentActionText();
		}
		return result;
	}

	public override void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
		n.Clear();
		UIItem uIItem = n.AddHeaderCard(base.Name.ToTitleCase());
		SetImage(uIItem.image2);
		uIItem.text2.SetText(race.GetText().ToTitleCase(wholeText: true) + " " + job.GetText().ToTitleCase(wholeText: true));
		n.AddText("");
		n.Build();
	}

	public override void SetSortVal(UIList.SortMode m, CurrencyType currency = CurrencyType.Money)
	{
		switch (m)
		{
		case UIList.SortMode.ByJob:
			sortVal = job._index * 10000 + sourceCard._index;
			break;
		case UIList.SortMode.ByRace:
			sortVal = race._index * 10000 * ((!IsHuman) ? 1 : (-1)) + sourceCard._index;
			break;
		case UIList.SortMode.ByFeat:
			sortVal = -GetTotalFeat();
			break;
		default:
			sortVal = sourceCard._index * ((!IsHuman) ? 1 : (-1));
			break;
		case UIList.SortMode.ByWorkk:
			break;
		}
	}

	public void ClearBed(Map map = null)
	{
		if (map == null)
		{
			map = EClass._map;
		}
		foreach (Card item in map.props.installed.all)
		{
			if (item.trait is TraitBed traitBed && traitBed.IsHolder(this))
			{
				traitBed.RemoveHolder(this);
			}
		}
	}

	public TraitBed FindBed()
	{
		foreach (Card item in EClass._map.props.installed.all)
		{
			if (item.trait is TraitBed traitBed && traitBed.IsHolder(this))
			{
				return traitBed;
			}
		}
		return null;
	}

	public TraitBed TryAssignBed()
	{
		if (!IsPCFaction && !IsGuest())
		{
			return null;
		}
		foreach (Card item in EClass._map.props.installed.all)
		{
			if (item.trait is TraitBed traitBed && traitBed.CanAssign(this))
			{
				traitBed.AddHolder(this);
				Msg.Say("claimBed", this);
				return traitBed;
			}
		}
		return null;
	}

	public void TryPutSharedItems(IEnumerable<Thing> containers, bool msg = true)
	{
		if (GetInt(113) != 0)
		{
			return;
		}
		_ListItems.Clear();
		Thing bestRangedWeapon = GetBestRangedWeapon();
		foreach (Thing thing in things)
		{
			if (!thing.IsAmmo)
			{
				if (thing.category.slot != 0 && !thing.isEquipped && !thing.HasTag(CTAG.gift))
				{
					_ListItems.Add(thing);
				}
				else if (thing.IsRangedWeapon && thing.category.slot == 0 && bestRangedWeapon != thing)
				{
					_ListItems.Add(thing);
				}
			}
		}
		if (_ListItems.Count == 0)
		{
			return;
		}
		List<Thing> containers2 = containers.ToList();
		foreach (Thing listItem in _ListItems)
		{
			EClass._zone.TryAddThingInSharedContainer(listItem, containers2, add: true, msg: true, this);
		}
	}

	public void TryPutSharedItems(bool msg = true)
	{
		TryPutSharedItems(EClass._map.props.installed.containers);
	}

	public void TryTakeSharedItems(bool msg = true)
	{
		TryTakeSharedItems(EClass._map.props.installed.containers);
	}

	public void TryTakeSharedItems(IEnumerable<Thing> containers, bool msg = true, bool shouldEat = true)
	{
		if (base.isSummon || (memberType == FactionMemberType.Livestock && homeBranch != null && !homeBranch.policies.IsActive(2715)))
		{
			return;
		}
		int num = 2;
		int num2 = 2;
		bool flag = GetInt(113) == 0;
		int num3 = 2;
		int num4 = 2;
		int num5 = 2;
		if (homeBranch != null && homeBranch.HasItemProtection && !IsPCParty)
		{
			num4 = 0;
			num5 = 0;
		}
		foreach (Thing thing3 in things)
		{
			if (CanEat(thing3, shouldEat))
			{
				num -= thing3.Num;
			}
			if (thing3.trait.GetHealAction(this) != null)
			{
				num2 -= thing3.Num;
			}
			if (thing3.id == "polish_powder")
			{
				num3 -= thing3.Num;
			}
			if (thing3.trait is TraitBlanketColdproof)
			{
				num4 -= thing3.Num;
			}
			if (thing3.trait is TraitBlanketFireproof)
			{
				num5 -= thing3.Num;
			}
		}
		_ListItems.Clear();
		foreach (Thing container in containers)
		{
			if (!container.IsSharedContainer)
			{
				continue;
			}
			foreach (Thing thing4 in container.things)
			{
				if (!thing4.c_isImportant && thing4.IsIdentified)
				{
					if (num3 > 0 && thing4.id == "polish_powder")
					{
						_ListItems.Add(thing4);
						num3 -= thing4.Num;
					}
					else if (num4 > 0 && !HasElement(1236) && thing4.trait is TraitBlanketColdproof)
					{
						_ListItems.Add(thing4);
						num4 -= thing4.Num;
					}
					else if (num5 > 0 && thing4.trait is TraitBlanketFireproof)
					{
						_ListItems.Add(thing4);
						num5 -= thing4.Num;
					}
					else if (num > 0 && CanEat(thing4, shouldEat) && !thing4.c_isImportant)
					{
						_ListItems.Add(thing4);
						num -= thing4.Num;
					}
					else if (num2 > 0 && thing4.trait.GetHealAction(this) != null)
					{
						_ListItems.Add(thing4);
						num2 -= thing4.Num;
					}
					else if (flag && thing4.IsEquipmentOrRanged && !thing4.HasTag(CTAG.gift) && ShouldEquip(thing4, useFav: true))
					{
						_ListItems.Add(thing4);
					}
				}
			}
		}
		if (_ListItems.Count == 0)
		{
			return;
		}
		_ListItems.ForeachReverse(delegate(Thing t)
		{
			if (t.IsEquipmentOrRanged)
			{
				bool flag3 = false;
				int slot = t.category.slot;
				int equipValue = t.GetEquipValue();
				foreach (Thing listItem in _ListItems)
				{
					if (listItem.category.slot == slot && listItem.GetEquipValue() > equipValue)
					{
						flag3 = true;
						break;
					}
				}
				if (flag3)
				{
					_ListItems.Remove(t);
				}
			}
		});
		bool flag2 = false;
		foreach (Thing listItem2 in _ListItems)
		{
			Thing thing = listItem2;
			if (things.IsFull(thing))
			{
				break;
			}
			Thing thing2 = listItem2.parent as Thing;
			if (thing.Num > 2)
			{
				thing = thing.Split(2);
			}
			if (msg)
			{
				Say("takeSharedItem", this, thing, thing2.GetName(NameStyle.Full));
			}
			AddCard(thing);
			if (ShouldEquip(thing, useFav: true) && thing.category.slot != 0)
			{
				TryEquip(thing, useFav: true);
				flag2 = true;
			}
		}
		if (flag2 && flag)
		{
			TryPutSharedItems(containers);
		}
	}

	public Thing FindBestFoodToEat()
	{
		Thing result = null;
		int num = -1;
		if (IsPC)
		{
			return null;
		}
		foreach (Thing item in things.List((Thing t) => CanEat(t, shouldEat: true) && !t.c_isImportant, onlyAccessible: true))
		{
			int num2 = CountNumEaten(item);
			int num3 = 100 - num2;
			if (num3 > num)
			{
				result = item;
				num = num3;
			}
		}
		return result;
	}

	public void InstantEat(Thing t = null, bool sound = true)
	{
		if (t == null)
		{
			t = FindBestFoodToEat();
		}
		if (t == null)
		{
			t = things.Find((Thing a) => CanEat(a, shouldEat: true) && !a.c_isImportant);
		}
		if (t == null)
		{
			t = things.Find((Thing a) => CanEat(a) && !a.c_isImportant);
		}
		if (t != null)
		{
			Say("eat_start", this, t.Duplicate(1));
			if (sound)
			{
				PlaySound("eat");
			}
			FoodEffect.Proc(this, t);
			t.ModNum(-1);
		}
	}

	public bool CanEat(Thing t, bool shouldEat = false)
	{
		if (t.IsDecayed && !HasElement(480))
		{
			return false;
		}
		if (shouldEat && (!(t.trait is TraitFoodPrepared) || t.c_isImportant))
		{
			return false;
		}
		if (!t.IsNegativeGift && !t.HasTag(CTAG.ignoreUse) && !t.isEquipped)
		{
			return t.trait.CanEat(this);
		}
		return false;
	}

	public bool ShouldEquip(Thing t, bool useFav = false)
	{
		if (t.IsRangedWeapon && t.category.slot == 0)
		{
			if (!CanEquipRanged(t))
			{
				return false;
			}
			int num = 0;
			foreach (Thing thing in things)
			{
				if (thing.IsRangedWeapon)
				{
					if (thing.category.slot != 0 && thing.isEquipped)
					{
						return false;
					}
					if (CanEquipRanged(thing) && thing.GetEquipValue() > num)
					{
						num = thing.GetEquipValue();
					}
				}
			}
			if (t.GetEquipValue() > num)
			{
				return true;
			}
			return false;
		}
		BodySlot bodySlot = body.GetSlot(t);
		if (bodySlot == null)
		{
			return false;
		}
		if (useFav)
		{
			switch (GetFavAttackStyle())
			{
			case AttackStyle.Default:
			case AttackStyle.TwoHand:
				if (t.IsMeleeWeapon)
				{
					bodySlot = body.slotMainHand;
				}
				else if (bodySlot.elementId == 35)
				{
					return false;
				}
				break;
			case AttackStyle.Shield:
				if (t.IsMeleeWeapon)
				{
					bodySlot = body.slotMainHand;
				}
				else if (bodySlot.elementId == 35 && t.IsMeleeWeapon)
				{
					return false;
				}
				break;
			case AttackStyle.TwoWield:
				if (bodySlot.elementId == 35 && !t.IsMeleeWeapon)
				{
					return false;
				}
				break;
			}
		}
		if (!body.IsEquippable(t, bodySlot, text: false))
		{
			return false;
		}
		if (bodySlot.thing != null && (bodySlot.thing.blessedState <= BlessedState.Cursed || bodySlot.thing.GetEquipValue() >= t.GetEquipValue()))
		{
			return false;
		}
		if (t.HasTag(CTAG.gift))
		{
			return false;
		}
		return true;
	}

	public bool TryEquip(Thing t, bool useFav = false)
	{
		if (!ShouldEquip(t, useFav))
		{
			return false;
		}
		if (t.category.slot == 0)
		{
			return false;
		}
		if (useFav)
		{
			BodySlot slot = body.GetSlot(t);
			switch (GetFavAttackStyle())
			{
			case AttackStyle.Default:
			case AttackStyle.TwoHand:
				if (t.IsMeleeWeapon)
				{
					slot = body.slotMainHand;
				}
				break;
			case AttackStyle.Shield:
				if (t.IsMeleeWeapon)
				{
					slot = body.slotMainHand;
				}
				break;
			}
			body.Equip(t, slot);
		}
		else
		{
			body.Equip(t);
		}
		Say("equip", this, t);
		return true;
	}

	public bool CanEquipRanged(Thing t)
	{
		return !body.IsTooHeavyToEquip(t);
	}

	public Thing TryGetThrowable()
	{
		Thing dest = null;
		if (IsPC)
		{
			if (EClass.game.config.autoCombat.bUseHotBar)
			{
				return FindThrowable(hotbar: true);
			}
			return null;
		}
		things.Foreach(delegate(Thing t)
		{
			if (t.HasTag(CTAG.throwWeapon) || (!base.IsPCFactionOrMinion && t.HasTag(CTAG.throwWeaponEnemy)))
			{
				dest = t;
			}
		});
		return dest;
		Thing FindThrowable(bool hotbar)
		{
			things.Foreach(delegate(Thing t)
			{
				if (dest == null)
				{
					if (t.IsHotItem)
					{
						if (!hotbar)
						{
							return;
						}
					}
					else if (hotbar)
					{
						return;
					}
					if (t.HasTag(CTAG.throwWeapon))
					{
						dest = t;
					}
				}
			});
			return dest;
		}
	}

	public Thing FindAmmo(Thing weapon)
	{
		TraitToolRange ranged = weapon.trait as TraitToolRange;
		Thing thing = (IsPC ? EClass.pc.things.Find<TraitQuiver>() : null);
		if (thing != null)
		{
			Thing thing2 = thing.things.Find((Thing t) => ranged.IsAmmo(t));
			if (thing2 != null)
			{
				return thing2;
			}
		}
		return things.Find((Thing t) => ranged.IsAmmo(t));
	}

	public Thing GetBestRangedWeapon()
	{
		Thing result = null;
		int num = 0;
		foreach (Thing thing in things)
		{
			if (thing.IsRangedWeapon && CanEquipRanged(thing) && thing.GetEquipValue() > num)
			{
				num = thing.GetEquipValue();
				result = thing;
			}
		}
		return result;
	}

	public bool TryEquipRanged()
	{
		if (IsPC)
		{
			Thing thing = EClass.player.currentHotItem.Thing;
			if (thing?.trait is TraitToolRange && CanEquipRanged(thing))
			{
				ranged = thing;
				return true;
			}
			return false;
		}
		if (ranged != null && ranged.parent == this)
		{
			return true;
		}
		ranged = GetBestRangedWeapon();
		return ranged != null;
	}

	public override int GetArmorSkill()
	{
		if (body.GetWeight(armorOnly: true) <= 30000)
		{
			return 120;
		}
		return 122;
	}

	public bool TryUse(Thing t)
	{
		if (t.id == "338")
		{
			Thing thing = things.Find((Thing a) => a.IsEquipmentOrRanged && !a.isAcidproof);
			if (thing != null)
			{
				Say("dip", this, thing, t.GetName(NameStyle.Full, 1));
				SE.Change();
				t.trait.OnBlend(thing, this);
				return true;
			}
			return false;
		}
		if (t.IsNegativeGift || t.source.HasTag(CTAG.ignoreUse))
		{
			return false;
		}
		if (t.id == "cigar")
		{
			(t.trait as TraitItemProc).OnUse(this);
			return true;
		}
		if (t.trait.CanEat(this) && hunger.GetPhase() > ((IsPCFaction || IsPCFactionMinion) ? 2 : 0))
		{
			SetAIImmediate(new AI_Eat
			{
				target = t
			});
			return true;
		}
		if (t.trait.CanDrink(this))
		{
			Drink(t);
			return true;
		}
		if (t.trait.CanRead(this))
		{
			SetAIImmediate(new AI_Read
			{
				target = t
			});
			return true;
		}
		if (base.hp < MaxHP * 90 / 100)
		{
			Action healAction = t.trait.GetHealAction(this);
			if (healAction != null)
			{
				healAction();
				return true;
			}
		}
		return false;
	}

	public Room FindRoom()
	{
		return FindBed()?.owner.pos.cell.room;
	}

	public void ModAffinity(Chara c, int a, bool show = true, bool showOnlyEmo = false)
	{
		if (c == this)
		{
			return;
		}
		if (IsPC)
		{
			c.ModAffinity(EClass.pc, a, show);
			return;
		}
		int num = StatsHygiene.GetAffinityMod(EClass.pc.hygiene.GetPhase()) + (HasElement(417) ? 30 : 0) + (EClass.pc.HasCondition<ConSmoking>() ? (-30) : 0);
		if (IsPCFaction && homeBranch != null)
		{
			num += (int)Mathf.Sqrt(homeBranch.Evalue(2117)) * 5;
		}
		bool flag = a > 0;
		if (flag)
		{
			a = a * num / 100;
		}
		if (show)
		{
			if (a == 0)
			{
				if (!showOnlyEmo)
				{
					Say("affinityNone", this, c);
				}
			}
			else
			{
				ShowEmo((!flag) ? Emo.angry : Emo.love);
				c.ShowEmo(flag ? Emo.love : Emo.sad);
				if (!showOnlyEmo)
				{
					Say(flag ? "affinityPlus" : "affinityMinus", this, c);
				}
			}
		}
		if (c.IsPC)
		{
			a = affinity.Mod(a);
		}
	}

	public bool TryIdentify(Thing t, int count = 1, bool show = true)
	{
		int num = Evalue(289);
		if (num == 0)
		{
			return false;
		}
		int lV = t.LV;
		if (EClass.rnd(num * num + 5) > EClass.rnd(lV * lV) * 20)
		{
			t.Identify(show, (num >= 20) ? IDTSource.SkillHigh : IDTSource.Skill);
			int num2 = 50;
			if (lV > num)
			{
				num2 += (lV - num) * 10;
			}
			elements.ModExp(289, Mathf.Min(num2, 1000));
			return true;
		}
		return false;
	}

	public Chara CreateReplacement()
	{
		Chara chara = CharaGen.Create(id);
		chara.c_originalHostility = base.c_originalHostility;
		if (chara.c_originalHostility != 0)
		{
			chara.hostility = chara.c_originalHostility;
		}
		if (orgPos != null)
		{
			chara.orgPos = orgPos.Copy();
		}
		chara.pos = pos.Copy();
		chara.isImported = true;
		chara.c_editorTags = base.c_editorTags;
		chara.c_editorTraitVal = base.c_editorTraitVal;
		chara.c_idTrait = base.c_idTrait;
		chara.homeZone = homeZone;
		return chara;
	}

	public SourceThing.Row GetFavFood()
	{
		if (_listFavFood.Count == 0)
		{
			foreach (SourceThing.Row row in EClass.sources.things.rows)
			{
				if (row._origin == "dish" && row.value != 0)
				{
					_listFavFood.Add(row);
				}
			}
		}
		SourceThing.Row r = null;
		Rand.UseSeed(base.uid + EClass.game.seed, delegate
		{
			r = _listFavFood.RandomItem();
		});
		return r;
	}

	public SourceCategory.Row GetFavCat()
	{
		SourceCategory.Row r = null;
		if (_listFavCat.Count == 0)
		{
			foreach (SourceCategory.Row row in EClass.sources.categories.rows)
			{
				if (row.gift > 0)
				{
					_listFavCat.Add(row);
				}
			}
		}
		Rand.UseSeed(base.uid + EClass.game.seed, delegate
		{
			r = _listFavCat.RandomItem();
		});
		return r;
	}

	public int GetTotalFeat()
	{
		int num = 0;
		if (base.c_upgrades != null)
		{
			num += base.c_upgrades.spent;
		}
		if (base.c_genes != null)
		{
			num += base.c_genes.GetTotalCost();
		}
		return num + base.feat;
	}

	public bool CanInsult()
	{
		foreach (ActList.Item item in ability.list.items)
		{
			if (item.act.id == 6630)
			{
				return true;
			}
		}
		return false;
	}

	public string GetIdPortraitCat()
	{
		string text = race.id;
		if ((text == "mifu" || text == "nefu") && EClass.rnd(2) == 0)
		{
			return "foxfolk";
		}
		if (trait is TraitGuard)
		{
			return "guard";
		}
		return "";
	}

	public string GetIdPortrait()
	{
		if (id == "olderyoungersister")
		{
			if (base.idSkin != 2)
			{
				return "UN_olderyoungersister";
			}
			return "UN_olderyoungersister_alt";
		}
		if (Portrait.allIds.Contains("UN_" + id + ".png"))
		{
			return "UN_" + id;
		}
		return base.c_idPortrait;
	}

	public Thing MakeGene(DNA.Type? type = null)
	{
		return DNA.GenerateGene(this, type);
	}

	public Thing MakeBraineCell()
	{
		return DNA.GenerateGene(this, DNA.Type.Brain);
	}

	public void OnInsulted()
	{
		if (!isDead)
		{
			if (HasElement(1231))
			{
				Talk("insulted");
				AddCondition<ConEuphoric>();
			}
			else if (EClass.rnd(20) == 0)
			{
				SetFeat(1231, 1, msg: true);
			}
		}
	}

	public bool IsValidGiftWeight(Card t, int num = -1)
	{
		int num2 = ((!HasElement(1411)) ? 1 : 3);
		if (GetBurden(t, num) >= num2)
		{
			return false;
		}
		return true;
	}

	public bool CanAcceptItem(Card t, int num = -1)
	{
		if (EClass.debug.enable)
		{
			return true;
		}
		if (!IsValidGiftWeight(t, num))
		{
			return false;
		}
		if (t.c_isImportant)
		{
			return false;
		}
		if ((t.category.IsChildOf("furniture") || t.category.IsChildOf("junk")) && !HasElement(1411))
		{
			return false;
		}
		return true;
	}

	public bool CanAcceptGift(Chara c, Card t)
	{
		if (things.IsFull())
		{
			return false;
		}
		if (t.c_isImportant)
		{
			return false;
		}
		if (t.id == "1084")
		{
			return true;
		}
		if (t.trait is TraitBookSecret)
		{
			return true;
		}
		if (t.trait.CanOnlyCarry || !t.trait.CanBeDestroyed || t.trait.CanExtendBuild || t.rarity == Rarity.Artifact || t.IsContainer)
		{
			return false;
		}
		return true;
	}

	public void GiveGift(Chara c, Thing t)
	{
		if (c.IsHostile() || c.IsDeadOrSleeping)
		{
			Msg.Say("affinityNone", c, this);
			return;
		}
		if (!t.isCopy && t.IsCursed && t.IsEquipmentOrRanged && !t.IsAmmo && c.HasElement(1414))
		{
			bool num = t.blessedState == BlessedState.Doomed;
			int num2 = 200 + t.LV * 3;
			if (t.rarity == Rarity.Legendary)
			{
				num2 *= 3;
			}
			if (t.rarity >= Rarity.Mythical)
			{
				num2 *= 5;
			}
			if (num)
			{
				num2 *= 2;
			}
			EClass.pc.PlayEffect("identify");
			EClass.pc.PlaySound("identify");
			c.PlayEffect("mutation");
			c.Say("draw_curse", c, t);
			t.Destroy();
			List<Element> list = new List<Element>();
			foreach (Element value in EClass.pc.elements.dict.Values)
			{
				if (value is Spell)
				{
					list.Add(value);
				}
			}
			if (list.Count == 0)
			{
				EClass.pc.SayNothingHappans();
				return;
			}
			Element element = list.RandomItem();
			EClass.pc.ModExp(element.id, num2);
			EClass.pc.Say("draw_curse2", EClass.pc, element.Name);
			c.AddExp(Mathf.Min(num2 / 3, c.ExpToNext));
			return;
		}
		if ((t.id == "lovepotion" || t.id == "dreambug") && !Application.isEditor)
		{
			GiveLovePotion(c, t);
			return;
		}
		if (t.trait is TraitErohon && c.id == t.c_idRefName)
		{
			c.OnGiveErohon(t);
			return;
		}
		if (t.trait is TraitTicketMassage)
		{
			t.ModNum(-1);
			c.Talk("ticket");
			switch (t.id)
			{
			case "ticket_massage":
				c.ModAffinity(EClass.pc, 10);
				EClass.pc.SetAI(new AI_Massage
				{
					target = c
				});
				break;
			case "ticket_armpillow":
				c.ModAffinity(EClass.pc, 20);
				EClass.pc.AddCondition<ConSleep>(300, force: true);
				c.SetAI(new AI_ArmPillow
				{
					target = EClass.pc
				});
				break;
			case "ticket_champagne":
				c.ModAffinity(EClass.pc, 10);
				c.AddCondition<ConChampagne>();
				break;
			}
			return;
		}
		if (t.id == "flyer")
		{
			stamina.Mod(-1);
			if (c.things.Find((Thing a) => a.id == "flyer") != null)
			{
				c.Talk("flyer_miss");
				DoHostileAction(c);
				return;
			}
			if (EClass.rnd(20) != 0 && c.CHA > EClass.rnd(base.CHA + Evalue(291) * 3 + 10))
			{
				Msg.Say("affinityNone", c, this);
				t.Destroy();
				elements.ModExp(291, 10);
				return;
			}
			elements.ModExp(291, 50);
		}
		if (t.id == "statue_weird")
		{
			EClass.pc.Say("statue_sell");
		}
		t.isGifted = true;
		c.nextUse = c.affinity.OnGift(t);
		if (!t.isDestroyed)
		{
			EClass.game.quests.list.ForeachReverse(delegate(Quest q)
			{
				q.OnGiveItem(c, t);
			});
			if (c.TryEquip(t))
			{
				c.nextUse = null;
			}
		}
	}

	public void OnGiveErohon(Thing t)
	{
		Say("give_erohon", this);
		AddCondition<ConParalyze>(50, force: true);
		AddCondition<ConConfuse>(50, force: true);
		AddCondition<ConFear>(1000, force: true);
		ModAffinity(EClass.pc, 100);
		t.Destroy();
		Talk("pervert");
	}

	public void GiveLovePotion(Chara c, Thing t)
	{
		c.Say("give_love", c, t);
		c.PlaySound(t.material.GetSoundDead());
		c.ShowEmo(Emo.angry);
		c.ModAffinity(EClass.pc, -20, show: false);
		c.Talk("pervert");
		t.Destroy();
	}

	public void RequestProtection(Chara attacker, Action<Chara> action)
	{
		if (HasCondition<StanceTaunt>() || base.isRestrained || attacker == this || (host != null && host.isRestrained) || (base.IsPCFactionOrMinion && attacker.IsPCFactionOrMinion))
		{
			return;
		}
		bool flag = false;
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara == attacker || chara.enemy == this || chara == this || chara.host != null || chara.IsDisabled || !chara.IsFriendOrAbove(this) || chara.conSuspend != null || (chara.IsPCParty && !IsPCParty) || (IsPCFaction && !chara.IsPCFaction) || (attacker.IsPCFactionOrMinion && chara.IsPCFactionOrMinion))
			{
				continue;
			}
			bool flag2 = chara.HasElement(1225);
			if ((!flag2 && (flag || EClass.rnd(2) == 0 || !chara.HasCondition<StanceTaunt>())) || chara.HasCooldown(1225))
			{
				continue;
			}
			int num = Mathf.Max(chara.Evalue(1649), (!chara.IsPC) ? ((!flag2) ? 1 : 3) : 0);
			int num2 = Dist(chara);
			if (num2 > 25)
			{
				continue;
			}
			if (num2 > num || !chara.CanSeeLos(pos, num2))
			{
				if (!flag2)
				{
					continue;
				}
				if (Dist(chara) < 5)
				{
					chara.GoHostile(attacker);
					chara.SetEnemy(attacker);
					attacker.SetEnemy(chara);
					continue;
				}
				Point nearestPoint = pos.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: true, ignoreCenter: true);
				if (!nearestPoint.IsValid)
				{
					continue;
				}
				chara.Teleport(nearestPoint);
				chara.AddCooldown(1225, 10);
				num2 = Dist(chara);
				Say("intercept_loyal", chara, this);
				chara.SetEnemy(attacker);
				attacker.SetEnemy(chara);
				if (num2 > num || !chara.CanSeeLos(pos, num2))
				{
					continue;
				}
			}
			if (!flag && !HasElement(1225))
			{
				Say("intercept", chara, this);
				if (EClass.rnd(10) == 0)
				{
					chara.Talk("intercept", base.NameSimple);
				}
				if (attacker.enemy == this)
				{
					attacker.SetEnemy(chara);
				}
				action(chara);
				flag = true;
			}
		}
	}

	public bool ShouldThrowAway(Thing t, ClearInventoryType type)
	{
		if (IsPCFaction)
		{
			if (t.IsFood && t.category.id != "fish" && !t.IsDecayed)
			{
				return false;
			}
			if (t.trait.GetHealAction(this) != null)
			{
				return false;
			}
			if (t.IsThrownWeapon || t.IsRangedWeapon || t.IsAmmo)
			{
				return false;
			}
			if (t.trait is TraitBlanket || t.trait is TraitItemProc || t.trait is TraitBookSkill)
			{
				return false;
			}
		}
		if (t.isEquipped || t.rarity >= Rarity.Legendary || !t.trait.CanBeDestroyed)
		{
			return false;
		}
		if (t.trait is TraitCurrency || t.trait is TraitTool)
		{
			return false;
		}
		switch (type)
		{
		case ClearInventoryType.SellAtTown:
			if (!t.isGifted && !t.isNPCProperty)
			{
				return false;
			}
			break;
		case ClearInventoryType.Purge:
			switch (t.category.id)
			{
			case "fish":
				if (EClass.rnd(3) == 0)
				{
					return true;
				}
				break;
			case "junk":
			case "garbage":
				if (EClass.rnd(3) != 0)
				{
					return true;
				}
				break;
			}
			if (t.id == "flyer")
			{
				return true;
			}
			if (!t.IsDecayed && EClass.rnd(3) == 0)
			{
				return false;
			}
			if (!things.IsFull() && t.IsRangedWeapon)
			{
				return false;
			}
			break;
		}
		return true;
	}

	public void ClearInventory(ClearInventoryType type)
	{
		int num = 0;
		for (int num2 = things.Count - 1; num2 >= 0; num2--)
		{
			Thing thing = things[num2];
			if (ShouldThrowAway(thing, type))
			{
				num += thing.GetPrice(CurrencyType.Money, sell: true, PriceType.Default, this) * thing.Num;
				thing.Destroy();
			}
		}
		if (num > 0)
		{
			ModCurrency(num);
			if (type == ClearInventoryType.SellAtTown)
			{
				Msg.Say("party_sell", this, num.ToString() ?? "");
				PlaySound("pay");
			}
		}
	}

	public void ResetUpgrade()
	{
		_ = base.c_upgrades;
	}

	public void TryUpgrade(bool msg = true)
	{
		if (!EClass.debug.enable || IsPC || !IsGlobal || !IsPCFaction)
		{
			return;
		}
		for (int i = 0; i < 100; i++)
		{
			if (base.feat == 0)
			{
				break;
			}
			if (base.c_upgrades == null)
			{
				base.c_upgrades = new CharaUpgrade();
			}
			if (base.c_upgrades.halt)
			{
				break;
			}
			Rand.SetSeed(base.uid + base.c_upgrades.count);
			int num = EClass.rnd(100);
			int num2 = 0;
			int num3 = 1;
			int num4 = 0;
			bool flag = false;
			IEnumerable<SourceElement.Row> ie = EClass.sources.elements.rows.Where((SourceElement.Row a) => !elements.Has(a) && a.category == "skill" && !a.tag.Contains("noPet"));
			List<Element> list = ListAvailabeFeats(pet: true);
			if (num >= 90 && list.Count > 0)
			{
				Element element = list.RandomItem();
				num2 = element.id;
				num4 = element.CostLearn;
			}
			else if (num >= 60 && ie.Any())
			{
				num2 = ie.RandomItem().id;
				num4 = 3;
			}
			else
			{
				num2 = Element.List_MainAttributesMajor.RandomItem();
				num4 = 1;
				num3 = 2;
				flag = true;
			}
			Rand.SetSeed();
			if (num4 > base.feat)
			{
				break;
			}
			base.feat -= num4;
			base.c_upgrades.count++;
			base.c_upgrades.spent += num4;
			bool flag2 = false;
			if (flag)
			{
				foreach (CharaUpgrade.Item item in base.c_upgrades.items)
				{
					if (item.idEle == num2)
					{
						item.value += num3;
						item.cost += num4;
						flag2 = true;
						break;
					}
				}
			}
			if (!flag2)
			{
				base.c_upgrades.items.Add(new CharaUpgrade.Item
				{
					idEle = num2,
					value = num3,
					cost = num4
				});
			}
			SourceElement.Row row = EClass.sources.elements.map[num2];
			if (row.category == "feat")
			{
				SetFeat(num2, elements.ValueWithoutLink(num2) + 1, msg: true);
			}
			else if (elements.ValueWithoutLink(row.id) == 0)
			{
				elements.Learn(row.id);
			}
			else
			{
				elements.ModBase(num2, num3);
			}
		}
	}

	public void AddCooldown(int idEle, int turns = 0)
	{
		if (_cooldowns == null)
		{
			_cooldowns = new List<int>();
		}
		if (turns != 0)
		{
			_cooldowns.Add(idEle * 1000 + turns);
			return;
		}
		SourceElement.Row row = EClass.sources.elements.map[idEle];
		if (row.cooldown > 0)
		{
			_cooldowns.Add(idEle * 1000 + row.cooldown);
		}
	}

	public bool HasCooldown(int idEle)
	{
		if (_cooldowns != null)
		{
			for (int i = 0; i < _cooldowns.Count; i++)
			{
				if (_cooldowns[i] / 1000 == idEle)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void TickCooldown()
	{
		for (int num = _cooldowns.Count - 1; num >= 0; num--)
		{
			if (_cooldowns[num] % 1000 == 1)
			{
				_cooldowns.RemoveAt(num);
			}
			else
			{
				_cooldowns[num]--;
			}
		}
		if (_cooldowns.Count == 0)
		{
			_cooldowns = null;
		}
	}

	public void ChooseNewGoal()
	{
		if (IsPC && EClass.AdvMode)
		{
			SetNoGoal();
			return;
		}
		if (IsPCParty || base.noMove)
		{
			SetAI(new GoalIdle());
			return;
		}
		if ((IsHomeMember() && IsInHomeZone()) || IsGuest())
		{
			Goal goalFromTimeTable = GetGoalFromTimeTable(EClass.world.date.hour);
			if (goalFromTimeTable != null)
			{
				SetAI(goalFromTimeTable);
				if (goalFromTimeTable is GoalWork)
				{
					goalFromTimeTable.Tick();
				}
				return;
			}
		}
		if (goalList.index == -2)
		{
			goalList.Refresh(this, goalListType);
		}
		SetAI(goalList.Next());
	}

	public Goal GetGoalFromTimeTable(int hour)
	{
		if (IsPC)
		{
			return null;
		}
		switch (TimeTable.GetSpan(idTimeTable, hour))
		{
		case TimeTable.Span.Sleep:
			if (sleepiness.value > 10 || EClass._zone.isSimulating)
			{
				return new GoalSleep();
			}
			break;
		case TimeTable.Span.Eat:
			return new GoalIdle();
		case TimeTable.Span.Work:
			if (IsGuest())
			{
				return new GoalIdle();
			}
			return GetGoalWork();
		case TimeTable.Span.Free:
			if (IsGuest())
			{
				return new GoalIdle();
			}
			return GetGoalHobby();
		}
		return null;
	}

	public Goal GetGoalWork()
	{
		if (IsPrisoner)
		{
			return new GoalIdle();
		}
		if (memberType == FactionMemberType.Livestock)
		{
			return new GoalGraze();
		}
		return new GoalWork();
	}

	public Goal GetGoalHobby()
	{
		if (IsPrisoner)
		{
			return new GoalIdle();
		}
		if (memberType == FactionMemberType.Livestock)
		{
			return new GoalGraze();
		}
		return new GoalHobby();
	}

	public void SetAIIdle()
	{
	}

	public void SetAIAggro()
	{
		SetAI(new GoalCombat());
	}

	public AIAct SetNoGoal()
	{
		return SetAI(_NoGoalPC);
	}

	public AIAct SetAI(AIAct g)
	{
		if (IsPC)
		{
			EClass.player.queues.OnSetGoal(g);
		}
		if (ai.IsRunning)
		{
			if (ai == g && ai.IsNoGoal)
			{
				return g;
			}
			ai.Cancel();
			if (ai == g)
			{
				Debug.Log("goal is g:" + ai?.ToString() + "/" + this);
				return g;
			}
		}
		if (HasCondition<ConWait>())
		{
			RemoveCondition<ConWait>();
		}
		ai = g;
		ai.SetOwner(this);
		if (IsPC)
		{
			renderer.RefreshStateIcon();
		}
		return g;
	}

	public void SetAIImmediate(AIAct g)
	{
		bool hasNoGoal = HasNoGoal;
		SetAI(g);
		if ((EClass.scene.actionMode != ActionMode.Sim || !EClass.scene.paused) && hasNoGoal && !(renderer as CharaRenderer).IsMoving)
		{
			Tick();
		}
	}

	public BaseArea GetRoomWork()
	{
		BaseArea result = null;
		foreach (Hobby item in ListWorks())
		{
			AIWork aI = item.GetAI(this);
			if (aI.SetDestination())
			{
				if (aI.destArea != null)
				{
					result = aI.destArea;
				}
				else if (aI.destThing != null)
				{
					result = aI.destThing.pos.cell.room;
				}
				break;
			}
		}
		return result;
	}

	public List<Hobby> ListWorks(bool useMemberType = true)
	{
		listHobby.Clear();
		if (useMemberType && memberType == FactionMemberType.Livestock)
		{
			listHobby.Add(new Hobby
			{
				id = 45
			});
		}
		else
		{
			if (_works == null)
			{
				RerollHobby();
			}
			foreach (int work in _works)
			{
				listHobby.Add(new Hobby
				{
					id = work
				});
			}
		}
		return listHobby;
	}

	public List<Hobby> ListHobbies(bool useMemberType = true)
	{
		listHobby.Clear();
		if (!useMemberType || memberType != FactionMemberType.Livestock)
		{
			if (_hobbies == null)
			{
				RerollHobby();
			}
			foreach (int hobby in _hobbies)
			{
				listHobby.Add(new Hobby
				{
					id = hobby
				});
			}
		}
		return listHobby;
	}

	public Hobby GetWork(string id)
	{
		foreach (Hobby item in ListWorks())
		{
			if (item.source.alias == id)
			{
				return item;
			}
		}
		foreach (Hobby item2 in ListHobbies())
		{
			if (item2.source.alias == id)
			{
				return item2;
			}
		}
		return null;
	}

	public void RefreshWorkElements(ElementContainer parent = null)
	{
		if (workElements != null)
		{
			workElements.SetParent();
		}
		workElements = null;
		if (IsPCParty || homeBranch == null || homeBranch.owner == null)
		{
			return;
		}
		foreach (Hobby item in ListHobbies())
		{
			TryAdd(item);
		}
		foreach (Hobby item2 in ListWorks())
		{
			TryAdd(item2);
		}
		if (workElements != null)
		{
			workElements.SetParent(parent);
		}
		void TryAdd(Hobby h)
		{
			if (!h.source.elements.IsEmpty())
			{
				if (workElements == null)
				{
					workElements = new ElementContainer();
				}
				for (int i = 0; i < h.source.elements.Length; i += 2)
				{
					int num = h.source.elements[i];
					int num2 = h.source.elements[i + 1];
					int num3 = 100;
					if (num != 2115 && num != 2207)
					{
						num3 = h.GetEfficiency(this) * homeBranch.efficiency / 100;
						if (num3 <= 0)
						{
							continue;
						}
					}
					workElements.ModBase(num, (num2 < 0) ? (num2 / 10) : Mathf.Max(1, h.source.elements[i + 1] * num3 / 1000));
				}
			}
		}
	}

	public string GetTextHobby(bool simple = false)
	{
		string text = (simple ? "" : ("hobby".lang() + ":"));
		foreach (Hobby item in ListHobbies())
		{
			text = text + " " + item.Name.TagColor((item.GetEfficiency(this) > 0) ? FontColor.Good : FontColor.Warning) + ",";
		}
		return text.TrimEnd(',');
	}

	public string GetTextWork(bool simple = false)
	{
		string text = (simple ? "" : ("work".lang() + ":"));
		foreach (Hobby item in ListWorks())
		{
			text = text + " " + item.Name.TagColor((item.GetEfficiency(this) > 0) ? FontColor.Good : FontColor.Warning) + ",";
		}
		return text.TrimEnd(',');
	}

	public void AddHobby(int id)
	{
		foreach (int hobby in _hobbies)
		{
			if (hobby == id)
			{
				return;
			}
		}
		_hobbies.Add(id);
	}

	public void AddWork(int id)
	{
		foreach (int work in _works)
		{
			if (work == id)
			{
				return;
			}
		}
		_works.Add(id);
	}

	public void RerollHobby(bool extraSlotChance = true)
	{
		if (_hobbies != null && _works != null)
		{
			_hobbies.Clear();
			_works.Clear();
		}
		else
		{
			_hobbies = new List<int>();
			_works = new List<int>();
		}
		if (source.hobbies.IsEmpty())
		{
			AddHobby(EClass.sources.hobbies.listHobbies.RandomItem().id);
		}
		else
		{
			string[] hobbies = source.hobbies;
			foreach (string key in hobbies)
			{
				AddHobby(EClass.sources.hobbies.alias[key].id);
			}
		}
		if (source.works.IsEmpty())
		{
			AddWork(EClass.sources.hobbies.listWorks.RandomItem().id);
		}
		else
		{
			string[] hobbies = source.works;
			foreach (string key2 in hobbies)
			{
				AddWork(EClass.sources.hobbies.alias[key2].id);
			}
		}
		GetWorkSummary().Reset();
	}

	public WorkSummary GetWorkSummary()
	{
		if (_workSummary == null)
		{
			_workSummary = new WorkSummary();
		}
		return _workSummary;
	}

	public void TickWork(VirtualDate date)
	{
		TimeTable.Span span = TimeTable.GetSpan(idTimeTable, date.hour);
		if (span != TimeTable.Span.Work && span != 0)
		{
			return;
		}
		WorkSummary workSummary = GetWorkSummary();
		if (span == TimeTable.Span.Work)
		{
			if (workSummary.work != null)
			{
				PerformWork(workSummary.work, isHobby: false, date.IsRealTime);
			}
		}
		else if (workSummary.hobbies.Count > 0)
		{
			PerformWork(workSummary.hobbies.RandomItem(), isHobby: true, date.IsRealTime);
		}
	}

	public bool TryWorkOutside(SourceHobby.Row sourceWork)
	{
		if (EClass.world.date.IsExpired(GetInt(51)))
		{
			Expedition expedition = Expedition.Create(this, sourceWork.expedition.ToEnum<ExpeditionType>());
			SetInt(51, EClass.world.date.GetRaw() + 60 * (expedition.hours + 24));
			homeBranch.expeditions.Add(expedition);
			return true;
		}
		return false;
	}

	public void PerformWork(WorkSession session, bool isHobby = false, bool IsRealTime = false)
	{
		Hobby hobby = new Hobby();
		hobby.id = session.id;
		WorkSummary workSummary = _workSummary;
		hobby.GetAI(this).OnPerformWork(IsRealTime);
		if (!isHobby)
		{
			workSummary.progress += EClass.rnd(5) + 5;
		}
		int num = PerformWork(hobby, 0, isHobby);
		int num2 = PerformWork(hobby, 1, isHobby);
		int num3 = PerformWork(hobby, 2, isHobby);
		int num4 = PerformWork(hobby, 3, isHobby);
		workSummary.money += num;
		workSummary.food += num2;
		workSummary.knowledge += num3;
		workSummary.material += num4;
		int PerformWork(Hobby work, int idx, bool isHobby)
		{
			if (idx >= work.source.resources.Length)
			{
				return 0;
			}
			int num5 = work.source.resources[idx];
			int num6 = num5;
			num5 = Rand.Range(num5 * (100 - work.source.resources[idx]) / 100, num5);
			num5 = num5 * (isHobby ? 50 : 100) * session.efficiency / 10000;
			if (num6 > 0 && num5 <= 0)
			{
				num5 = 1;
			}
			return num5;
		}
	}

	public void ValidateWorks()
	{
		_goalWork.FindWork(this, setAI: false);
		_goalHobby.ValidateHobby(this);
	}

	public void InitStats(bool onDeserialize = false)
	{
		if (!onDeserialize)
		{
			_cints[10] = 20;
			_cints[11] = 70;
			_cints[13] = 70;
			_cints[14] = 70;
			_cints[15] = 70;
			_cints[17] = 0;
			_cints[18] = 0;
		}
		foreach (Condition condition in conditions)
		{
			condition.SetOwner(this, onDeserialize);
		}
	}

	public Condition AddCondition<T>(int p = 100, bool force = false) where T : Condition
	{
		return AddCondition(typeof(T).Name, p, force);
	}

	public Condition AddCondition(string id, int p = 100, bool force = false)
	{
		return AddCondition(Condition.Create(id, p), force);
	}

	public Condition AddCondition(Condition c, bool force = false)
	{
		c.owner = this;
		if (!(c is ConBurning))
		{
			if (c is ConBleed && ResistLv(964) >= 3)
			{
				return null;
			}
		}
		else if (ResistLv(950) >= 3)
		{
			return null;
		}
		if (c.GainResistFactor > 0 && CanGainConResist)
		{
			if (c.GainResistFactor >= 400)
			{
				c.power /= 2;
			}
			ResistCon(c);
			if (c.power <= 0)
			{
				return null;
			}
		}
		if (!force)
		{
			if (c.source.negate.Length != 0)
			{
				string[] negate = c.source.negate;
				foreach (string text in negate)
				{
					if (HasElement(text))
					{
						return null;
					}
				}
			}
			Element defenseAttribute = c.GetDefenseAttribute(this);
			if (defenseAttribute != null)
			{
				c.power = 100 * c.power / Mathf.Max(100 + defenseAttribute.Value, 1);
			}
			if (c.source.resistance.Length != 0)
			{
				int num = ResistLv(EClass.sources.elements.alias[c.source.resistance[0]].id);
				if (num > 0)
				{
					c.power /= num * num + 1;
					if (c.power <= 40)
					{
						return null;
					}
				}
			}
			c.power = c.EvaluatePower(c.power);
			if (c.power <= 0)
			{
				return null;
			}
		}
		for (int j = 0; j < conditions.Count; j++)
		{
			if (conditions[j].id != c.id)
			{
				continue;
			}
			if (c.Type == ConditionType.Stance || c.IsToggle)
			{
				conditions[j].Kill();
				return null;
			}
			if (conditions[j].CanStack(c))
			{
				if (conditions[j].WillOverride)
				{
					conditions[j].Kill(silent: true);
					continue;
				}
				if (CanGainConResist)
				{
					AddResistCon(c);
				}
				conditions[j].OnStacked(c.power);
				conditions[j].OnStartOrStack();
				conditions[j].PlayEffect();
			}
			if (!conditions[j].AllowMultipleInstance)
			{
				return null;
			}
		}
		foreach (Condition condition in conditions)
		{
			if (condition.TryNullify(c))
			{
				return null;
			}
		}
		int num2 = c.EvaluateTurn(c.power);
		if (num2 == 0)
		{
			return null;
		}
		c.value = num2;
		conditions.Add(c);
		if (CanGainConResist)
		{
			AddResistCon(c);
		}
		c.SetOwner(this);
		c.Start();
		SetDirtySpeed();
		if (c.ShouldRefresh)
		{
			Refresh();
		}
		if (c.CancelAI)
		{
			ai.Cancel();
		}
		if (IsPC && c.ConsumeTurn && !EClass.pc.isRestrained)
		{
			EClass.player.EndTurn();
		}
		if (c.SyncRide && (ride != null || parasite != null))
		{
			if (ride != null)
			{
				ride.AddCondition(Condition.Create(c.source.alias, c.power));
			}
			if (parasite != null)
			{
				parasite.AddCondition(Condition.Create(c.source.alias, c.power));
			}
		}
		return c;
	}

	public override bool HasCondition<T>()
	{
		for (int i = 0; i < conditions.Count; i++)
		{
			if (conditions[i] is T)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasCondition(string alias)
	{
		for (int i = 0; i < conditions.Count; i++)
		{
			if (conditions[i].source.alias == alias)
			{
				return true;
			}
		}
		return false;
	}

	public Element GetBuffStats(string alias)
	{
		return GetBuffStats(EClass.sources.elements.alias[alias].id);
	}

	public Element GetBuffStats(int ele)
	{
		for (int i = 0; i < conditions.Count; i++)
		{
			if (conditions[i] is ConBuffStats conBuffStats && conBuffStats.refVal == ele)
			{
				return conBuffStats.elements.GetElement(ele);
			}
		}
		return null;
	}

	public void CureCondition<T>(int v = 99999) where T : Condition
	{
		T condition = GetCondition<T>();
		if (condition != null)
		{
			condition.value -= v;
			if (condition.value <= 0)
			{
				condition.Kill();
			}
		}
	}

	public T GetCondition<T>() where T : Condition
	{
		for (int i = 0; i < conditions.Count; i++)
		{
			if (conditions[i] is T)
			{
				return conditions[i] as T;
			}
		}
		return null;
	}

	public void RemoveCondition<T>() where T : Condition
	{
		for (int num = conditions.Count - 1; num >= 0; num--)
		{
			if (conditions[num] is T)
			{
				conditions[num].Kill();
				break;
			}
		}
	}

	public void CureHost(CureType type, int p = 100, BlessedState state = BlessedState.Normal)
	{
		if (parasite != null)
		{
			parasite.Cure(type, p, state);
		}
		if (ride != null)
		{
			ride.Cure(type, p, state);
		}
		Cure(type, p, state);
	}

	public void Cure(CureType type, int p = 100, BlessedState state = BlessedState.Normal)
	{
		bool flag = state == BlessedState.Blessed;
		switch (type)
		{
		case CureType.Heal:
		case CureType.Prayer:
			CureCondition<ConFear>();
			CureCondition<ConBlind>(2 * p / 100 + 5);
			CureCondition<ConPoison>(5 * p / 100 + 5);
			CureCondition<ConConfuse>(10 * p / 100 + 10);
			CureCondition<ConDim>(p / 100 + 5);
			CureCondition<ConBleed>(2 * p / 100 + 10);
			if (flag)
			{
				SAN.Mod(-15);
			}
			break;
		case CureType.CureBody:
			CureCondition<ConBlind>(5 * p / 100 + 15);
			CureCondition<ConPoison>(10 * p / 100 + 15);
			CureCondition<ConBleed>(5 * p / 100 + 20);
			CureTempElements(p, body: true, mind: false);
			break;
		case CureType.CureMind:
			CureCondition<ConFear>();
			CureCondition<ConDim>(3 * p / 100 + 10);
			CureTempElements(p, body: false, mind: true);
			break;
		case CureType.Sleep:
		{
			for (int num2 = conditions.Count - 1; num2 >= 0; num2--)
			{
				Condition condition2 = conditions[num2];
				if (!(condition2 is ConSleep) && !(condition2 is ConFaint))
				{
					if (condition2.isPerfume)
					{
						condition2.Mod(-1, force: true);
					}
					else if (condition2.Type == ConditionType.Bad || condition2.Type == ConditionType.Debuff)
					{
						condition2.Kill();
					}
				}
			}
			CureCondition<ConWait>();
			CureCondition<ConDisease>((EClass.rnd(20) + 10) * p / 100);
			bool flag2 = HasCondition<ConAnorexia>();
			base.c_vomit -= (flag2 ? 3 : 2) * p / 100;
			if (base.c_vomit < 0)
			{
				base.c_vomit = 0;
				if (flag2)
				{
					RemoveCondition<ConAnorexia>();
				}
			}
			break;
		}
		case CureType.HealComplete:
		case CureType.Death:
		case CureType.Jure:
		case CureType.Boss:
		{
			CureTempElements(p * 100, body: true, mind: true);
			for (int num = conditions.Count - 1; num >= 0; num--)
			{
				Condition condition = conditions[num];
				if (!(condition is ConAnorexia) || type == CureType.Death)
				{
					if (condition.Type == ConditionType.Bad || condition.Type == ConditionType.Debuff || condition.Type == ConditionType.Disease)
					{
						condition.Kill();
					}
					else if (type == CureType.Death && condition.isPerfume)
					{
						condition.Kill();
					}
				}
			}
			CureCondition<ConWait>();
			CureCondition<ConSleep>();
			if (type == CureType.Death || type == CureType.Boss)
			{
				SAN.Mod(-20);
			}
			if (type == CureType.Jure)
			{
				SAN.Mod(-999);
				if (HasElement(1206))
				{
					SetFeat(1206, 0, msg: true);
				}
			}
			break;
		}
		}
	}

	public bool TryNeckHunt(Chara TC, int power, bool harvest = false)
	{
		if (TC == null || TC.HasCondition<ConInvulnerable>() || TC.Evalue(1421) > 0 || !TC.ExistsOnMap)
		{
			return false;
		}
		if (TC.hp > TC.MaxHP * Mathf.Min(5 + (int)Mathf.Sqrt(power), harvest ? 35 : 25) / 100)
		{
			return false;
		}
		if (TC.HasElement(427))
		{
			return false;
		}
		if (TC.IsPC && EClass.player.invlunerable)
		{
			return false;
		}
		PlaySound("hit_finish");
		Say("finish");
		Say("finish2", this, TC);
		TC.DamageHP(TC.MaxHP, AttackSource.Finish, this);
		return false;
	}

	public void AddResistCon(Condition con)
	{
		if (con.power > 0 && con.GainResistFactor > 0)
		{
			int key = con.id;
			if (resistCon == null)
			{
				resistCon = new Dictionary<int, int>();
			}
			if (resistCon.ContainsKey(key))
			{
				resistCon[key] += con.power * con.GainResistFactor / 100;
			}
			else
			{
				resistCon[key] = con.power * con.GainResistFactor / 100;
			}
		}
	}

	public void ResistCon(Condition con)
	{
		if (con.power > 0 && resistCon != null)
		{
			int a = ClassExtension.TryGetValue<int, int>((IDictionary<int, int>)resistCon, con.id, 0);
			if (1000 < EClass.rnd(a))
			{
				con.power = 0;
			}
			else if (500 < EClass.rnd(a))
			{
				con.power /= 5;
			}
			else if (200 < EClass.rnd(a))
			{
				con.power /= 2;
			}
		}
	}

	public void Sleep(Thing bed = null, Thing pillow = null, bool pickup = false, ItemPosition posBed = null, ItemPosition posPillow = null)
	{
		RemoveCondition<ConAwakening>();
		AddCondition(Condition.Create(100, delegate(ConSleep con)
		{
			con.pcSleep = 15;
			con.pcBed = bed;
			con.pcPillow = pillow;
			con.pickup = pickup;
			con.posBed = posBed;
			con.posPillow = posPillow;
		}), force: true);
	}

	public void OnSleep(Thing bed = null, int days = 1)
	{
		TraitPillow traitPillow = pos.FindThing<TraitPillow>();
		int num = bed?.Power ?? 20;
		if (traitPillow != null)
		{
			num += traitPillow.owner.Power / 2;
		}
		if (bed != null)
		{
			num += bed.Evalue(750) * 5;
		}
		OnSleep(num, days);
	}

	public void OnSleep(int power, int days = 1)
	{
		if (days < 1)
		{
			days = 1;
		}
		int num = power * days;
		if (stamina.value < 0)
		{
			stamina.Set(1);
		}
		HealHP(num);
		stamina.Mod(10 + 25 * num / 100 * (100 + elements.GetFeatRef(1642)) / 100);
		mana.Mod(num);
		if (IsPCFaction)
		{
			hunger.Mod(20);
		}
		sleepiness.Set(0);
		if (!IsMinion)
		{
			interest = 100;
		}
		Cure(CureType.Sleep, power);
	}

	public void ModHeight(int a)
	{
		int height = bio.height;
		height = height * (100 + a) / 100 + ((a > 0) ? 1 : (-1));
		if (height < 1)
		{
			height = 1;
		}
		if (height != bio.height)
		{
			bio.height = height;
			Say((a > 0) ? "height_gain" : "height_lose", this);
		}
	}

	public void ModWeight(int a, bool ignoreLimit = false)
	{
		if (a == 0)
		{
			return;
		}
		int weight = bio.weight;
		int height = bio.height;
		int num = height * height * 18 / 25000;
		int num2 = height * height * 24 / 10000;
		if (ignoreLimit || (weight > num && weight < num2))
		{
			weight = weight * (100 + a) / 100 + ((a > 0) ? 1 : (-1));
			if (weight < 1)
			{
				weight = 1;
			}
			if (weight != bio.weight)
			{
				bio.weight = weight;
				Say((a > 0) ? "weight_gain" : "weight_lose", this);
			}
		}
	}

	public void ModCorruption(int a)
	{
		if (a > 0)
		{
			if (ResistLv(962) > 0 && EClass.rnd(ResistLv(962) + 1) != 0)
			{
				return;
			}
			int num = Evalue(412);
			if (num != 0)
			{
				float num2 = (float)a * 100f / (float)Mathf.Max(100 + num * 2, 10);
				a = (int)num2 + ((EClass.rndf(1f) > num2 % 1f) ? 1 : 0);
			}
		}
		if (a == 0)
		{
			return;
		}
		int num3 = (corruption + a) / 100 - corruption / 100;
		for (int i = 0; i < Mathf.Abs(num3); i++)
		{
			if (!MutateRandom((num3 > 0) ? 1 : (-1), 100, ether: true))
			{
				break;
			}
		}
		corruption += a;
		int num4 = 0;
		foreach (Element value in elements.dict.Values)
		{
			if (value.source.category == "ether")
			{
				num4 += value.Value;
			}
		}
		if (num4 > 0 && IsPC)
		{
			Tutorial.Reserve("ether");
		}
		corruption = num4 * 100 + corruption % 100;
	}

	public List<Element> ListAvailabeFeats(bool pet = false, bool showAll = false)
	{
		List<Element> list = new List<Element>();
		foreach (SourceElement.Row item in EClass.sources.elements.rows.Where((SourceElement.Row a) => a.group == "FEAT" && a.cost[0] != -1 && !a.categorySub.IsEmpty()))
		{
			Feat feat = elements.GetOrCreateElement(item.id) as Feat;
			int num = ((feat.ValueWithoutLink <= 0) ? 1 : (feat.ValueWithoutLink + 1));
			if (num <= feat.source.max && !feat.HasTag("class") && !feat.HasTag("hidden") && !feat.HasTag("innate") && (!pet || !feat.HasTag("noPet")) && (showAll || feat.IsPurchaseFeatReqMet(elements)))
			{
				list.Add(Element.Create(feat.id, num) as Feat);
			}
		}
		return list;
	}

	public void SetFeat(int id, int value = 1, bool msg = false)
	{
		Feat feat = elements.GetElement(id) as Feat;
		int num = 0;
		if (feat != null && feat.Value > 0)
		{
			if (value == feat.Value)
			{
				return;
			}
			num = feat.Value;
			feat.Apply(-feat.Value, elements);
		}
		if (value > 0)
		{
			feat = elements.SetBase(id, value - (feat?.vSource ?? 0)) as Feat;
			feat.Apply(feat.Value, elements);
		}
		else
		{
			elements.Remove(id);
		}
		if (EClass.core.IsGameStarted)
		{
			Refresh();
			CalculateMaxStamina();
		}
		if (!msg)
		{
			return;
		}
		if (feat.source.textInc.IsEmpty())
		{
			PlaySound("ding_skill");
			Msg.SetColor(Msg.colors.Ding);
			Say("gainFeat", this, feat.FullName);
		}
		else
		{
			bool flag = value < num;
			if (feat.source.tag.Contains("neg"))
			{
				flag = !flag;
			}
			PlaySound("mutation");
			Msg.SetColor(flag ? Msg.colors.Negative : Msg.colors.Ding);
			Say((value > num) ? feat.source.GetText("textInc") : feat.source.GetText("textDec"), this, feat.FullName);
		}
		elements.CheckSkillActions();
	}

	public void SetMutation(int idEle, int a = 0)
	{
		SourceElement.Row row = EClass.sources.elements.map[idEle];
		SourceElement.Row row2 = EClass.sources.elements.alias[row.aliasParent];
		Element element = elements.GetElement(idEle);
		Element element2 = elements.GetElement(row2.id);
		int num = element?.Value ?? 0;
		if (a < 0)
		{
			a = 0;
		}
		if (a > row.max)
		{
			a = row.max;
		}
		bool flag = (row.tag.Contains("neg") ? (a > num) : (a < num));
		if (a == 0 && (element == null || element.Value == 0))
		{
			SayNothingHappans();
			return;
		}
		if (element != null && element.Value == a)
		{
			SayNothingHappans();
			return;
		}
		if (element2 != null && element2.Value > 0)
		{
			SetFeat(element2.id, 0);
		}
		SetFeat(idEle, a);
		PlaySound("mutation");
		PlayEffect("mutation");
		Msg.SetColor(flag ? Msg.colors.MutateBad : Msg.colors.MutateGood);
		Say(row.GetText(flag ? "textDec" : "textInc", returnNull: true) ?? row.alias, this);
	}

	public bool MutateRandom(int vec = 0, int tries = 100, bool ether = false, BlessedState state = BlessedState.Normal)
	{
		if (!ether && vec >= 0 && HasElement(406) && EClass.rnd(5) != 0)
		{
			Say("resistMutation", this);
			return false;
		}
		IEnumerable<SourceElement.Row> ie = EClass.sources.elements.rows.Where((SourceElement.Row a) => a.category == (ether ? "ether" : "mutation"));
		for (int i = 0; i < tries; i++)
		{
			SourceElement.Row row = ie.RandomItem();
			if (i == 0 && vec < 0 && ether && base.c_corruptionHistory != null && base.c_corruptionHistory.Count > 0)
			{
				row = EClass.sources.elements.map[base.c_corruptionHistory.LastItem()];
				base.c_corruptionHistory.RemoveAt(base.c_corruptionHistory.Count - 1);
				if (base.c_corruptionHistory.Count == 0)
				{
					base.c_corruptionHistory = null;
				}
			}
			Element element = elements.GetElement(row.id);
			int num = 1;
			if ((vec > 0 && ((row.id == 1563 && corruption < 300) || (row.id == 1562 && corruption < 1000 && base.IsPowerful))) || (vec < 0 && (element == null || element.Value <= 0)) || (vec > 0 && element != null && element.Value >= row.max))
			{
				continue;
			}
			if (element == null && !row.aliasParent.IsEmpty() && elements.Has(row.aliasParent))
			{
				row = EClass.sources.elements.alias[row.aliasParent];
				element = elements.GetElement(row.id);
			}
			bool flag = row.tag.Contains("neg");
			if (vec > 0)
			{
				if ((state >= BlessedState.Blessed && flag) || (state <= BlessedState.Cursed && !flag))
				{
					continue;
				}
			}
			else if (vec < 0 && ((state >= BlessedState.Blessed && !flag) || (state <= BlessedState.Cursed && flag)))
			{
				continue;
			}
			bool flag2 = true;
			if (element != null)
			{
				num = element.Value + ((vec != 0) ? vec : ((EClass.rnd(2) == 0) ? 1 : (-1)));
				if (num > element.source.max)
				{
					num = element.source.max - 1;
				}
				flag = (flag && num > element.Value) || (!flag && num < element.Value);
				flag2 = num > element.Value;
				if (vec > 0 && !flag2)
				{
					continue;
				}
			}
			Say(flag2 ? "mutation_gain" : "mutation_loose", this);
			SetFeat(row.id, num);
			if (flag2 && ether)
			{
				if (base.c_corruptionHistory == null)
				{
					base.c_corruptionHistory = new List<int>();
				}
				base.c_corruptionHistory.Add(row.id);
				if (IsPCFaction)
				{
					Element element2 = elements.GetElement(row.id);
					WidgetPopText.Say("popEther".lang(element2.Name, base.Name));
				}
				if (IsPC && !EClass.player.flags.gotEtherDisease)
				{
					EClass.player.flags.gotEtherDisease = true;
					Thing thing = ThingGen.Create("parchment");
					thing.SetStr(53, "letter_ether");
					Thing thing2 = ThingGen.Create("1165");
					thing2.SetBlessedState(BlessedState.Normal);
					Thing p = ThingGen.CreateParcel(null, thing2, thing);
					EClass.world.SendPackage(p);
				}
			}
			if (EClass.core.IsGameStarted && pos != null)
			{
				PlaySound(ether ? "mutation_ether" : "mutation");
				PlayEffect("mutation");
				Msg.SetColor(flag ? Msg.colors.MutateBad : Msg.colors.MutateGood);
				Say(row.GetText(flag ? "textDec" : "textInc", returnNull: true) ?? row.alias, this);
			}
			return true;
		}
		Say("nothingHappens");
		return false;
	}

	public void GainAbility(int ele, int mtp = 100)
	{
		Element orCreateElement = elements.GetOrCreateElement(ele);
		if (orCreateElement.ValueWithoutLink == 0)
		{
			elements.ModBase(orCreateElement.id, 1);
		}
		if (orCreateElement is Spell)
		{
			int num = mtp * orCreateElement.source.charge * (100 + Evalue(307) + (HasElement(307) ? 20 : 0)) / 100 / 100;
			if (orCreateElement.source.charge == 1)
			{
				num = 1;
			}
			orCreateElement.vPotential += Mathf.Max(1, num / 2 + EClass.rnd(num / 2 + 1));
		}
		Say("spell_gain", this, orCreateElement.Name);
		LayerAbility.SetDirty(orCreateElement);
	}

	public bool TryNullifyCurse()
	{
		if (IsPCParty)
		{
			foreach (Chara member in EClass.pc.party.members)
			{
				if (member.HasElement(1641) && EClass.rnd(3) != 0)
				{
					Msg.Say("curse_nullify", member);
					return true;
				}
			}
		}
		else if (HasElement(1641) && EClass.rnd(3) != 0)
		{
			Say("curse_nullify", this);
			return true;
		}
		if (GetCondition<ConHolyVeil>() != null && EClass.rnd(5) != 0)
		{
			Say("curse_nullify", this);
			return true;
		}
		return false;
	}

	public void OnAdvanceDay()
	{
		if (_historyFood != null)
		{
			while (_historyFood.Count > 5)
			{
				_historyFood.RemoveAt(_historyFood.Count - 1);
			}
			if (_historyFood[0].Count > 0)
			{
				_historyFood.Insert(0, new List<string>());
			}
		}
	}

	public void AddFoodHistory(Thing food)
	{
		if (!IsPCFaction || !IsGlobal)
		{
			_historyFood = null;
			return;
		}
		if (_historyFood == null)
		{
			_historyFood = new List<List<string>>
			{
				new List<string>()
			};
		}
		if (_historyFood[0].Count < 5)
		{
			_historyFood[0].Add(food.id);
		}
	}

	public int CountNumEaten(Thing food)
	{
		if (_historyFood == null)
		{
			return -1;
		}
		int num = 0;
		foreach (List<string> item in _historyFood)
		{
			foreach (string item2 in item)
			{
				if (item2 == food.id)
				{
					num++;
				}
			}
		}
		return num;
	}

	public int GetPietyValue()
	{
		if (base._IsPC)
		{
			return 10 + (int)(Mathf.Sqrt(base.c_daysWithGod) * 2f + (float)Evalue(85)) / 2;
		}
		return 10 + (int)(Mathf.Sqrt(base.LV) * 5f + (float)Evalue(306)) / 2;
	}

	public void RefreshFaithElement()
	{
		if (faithElements != null)
		{
			faithElements.SetParent();
		}
		faithElements = new ElementContainer();
		if (idFaith == "eyth" && !HasElement(1228))
		{
			return;
		}
		SourceReligion.Row row = EClass.sources.religions.map.TryGetValue(idFaith);
		if (row == null)
		{
			return;
		}
		faithElements = new ElementContainer();
		SourceElement.Row row2 = EClass.sources.elements.alias.TryGetValue("featGod_" + row.id + "1");
		if (row2 != null)
		{
			faithElements.SetBase(row2.id, 1);
		}
		if (!HasCondition<ConExcommunication>())
		{
			int[] array = row.elements;
			int num = GetPietyValue() * (120 + Evalue(1407) * 15) / 100;
			for (int i = 0; i < array.Length; i += 2)
			{
				int num2 = array[i + 1] * num / 50;
				if (array[i] == 79)
				{
					num2 = EClass.curve(num2, array[i + 1] * 2, 10, 50);
				}
				if (num2 >= 20 && array[i] >= 950 && array[i] < 970)
				{
					num2 = 20;
				}
				faithElements.SetBase(array[i], Mathf.Max(num2, 1));
			}
		}
		faithElements.SetParent(this);
	}

	public void ModTempElement(int ele, int a, bool naturalDecay = false)
	{
		if (a < 0 && !naturalDecay && HasElement(EClass.sources.elements.alias["sustain_" + EClass.sources.elements.map[ele].alias]?.id ?? 0))
		{
			return;
		}
		if (tempElements == null)
		{
			tempElements = new ElementContainer();
			tempElements.SetParent(this);
		}
		if (a > 0 && tempElements.Base(ele) > a)
		{
			a = a * 100 / (200 + (tempElements.Base(ele) - a) * 10);
		}
		int num = Mathf.Abs(elements.ValueWithoutLink(ele)) * 2 + 20;
		int num2 = tempElements.Base(ele) + a;
		if (num2 < -num || num2 > num || (a < 0 && num2 < -100))
		{
			a = 0;
		}
		Element element = tempElements.ModBase(ele, a);
		if (element.vBase == 0)
		{
			tempElements.Remove(element.id);
			if (tempElements.dict.Count == 0)
			{
				tempElements = null;
			}
		}
	}

	public void DamageTempElements(int p, bool body, bool mind)
	{
		if (body)
		{
			DamageTempElement(Element.List_Body.RandomItem(), p);
		}
		if (mind)
		{
			DamageTempElement(Element.List_Mind.RandomItem(), p);
		}
	}

	public void DamageTempElement(int ele, int p)
	{
		ModTempElement(ele, -(p / 100 + EClass.rnd(p / 100 + 1) + 1));
	}

	public void EnhanceTempElements(int p, bool body, bool mind)
	{
		if (body)
		{
			EnhanceTempElement(Element.List_Body.RandomItem(), p);
		}
		if (mind)
		{
			EnhanceTempElement(Element.List_Mind.RandomItem(), p);
		}
	}

	public void EnhanceTempElement(int ele, int p)
	{
		ModTempElement(ele, p / 100 + EClass.rnd(p / 100 + 1));
	}

	public void DiminishTempElements(int a = 1)
	{
		if (tempElements == null)
		{
			return;
		}
		foreach (Element item in tempElements.dict.Values.ToList())
		{
			if (item.vBase > 0)
			{
				ModTempElement(item.id, -Mathf.Min(item.vBase * 5 / 100 + 1, item.vBase), naturalDecay: true);
			}
		}
	}

	public void CureTempElements(int p, bool body, bool mind)
	{
		if (tempElements != null)
		{
			if (body)
			{
				Cure(Element.List_Body);
			}
			if (mind)
			{
				Cure(Element.List_Mind);
			}
		}
		void Cure(int[] eles)
		{
			foreach (int ele in eles)
			{
				if (tempElements == null)
				{
					break;
				}
				Element element = tempElements.GetElement(ele);
				if (element != null && element.vBase < 0)
				{
					ModTempElement(ele, Mathf.Clamp(p / 20 + EClass.rnd(p / 20), 1, -element.vBase));
				}
			}
		}
	}

	public void ClearTempElements()
	{
		if (tempElements != null)
		{
			tempElements.SetParent();
			tempElements = null;
		}
	}
}
