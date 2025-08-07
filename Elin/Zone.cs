using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dungen;
using Ionic.Zip;
using Newtonsoft.Json;
using SFB;
using UnityEngine;

public class Zone : Spatial, ICardParent, IInspect
{
	public class PortalReturnData
	{
		[JsonProperty]
		public int uidZone;

		[JsonProperty]
		public int x;

		[JsonProperty]
		public int z;
	}

	public static bool forceRegenerate;

	public static string forceSubset;

	public static CardRow sourceHat;

	public static int okaerinko = 0;

	[JsonProperty]
	public FactionBranch branch;

	[JsonProperty]
	public HashSet<int> completedQuests = new HashSet<int>();

	[JsonProperty]
	public ZoneEventManager events = new ZoneEventManager();

	[JsonProperty]
	public ElementContainerZone elements = new ElementContainerZone();

	[JsonProperty]
	public MapBounds bounds;

	[JsonProperty]
	public Dictionary<int, string> dictCitizen = new Dictionary<int, string>();

	[JsonProperty]
	public ZoneInstance instance;

	[JsonProperty]
	public List<int> landFeats;

	[JsonProperty]
	private FortuneRollData fortuneRoll;

	public Map map;

	public MapSubset subset;

	public ZoneBlueprint bp;

	public int fileVariation;

	public bool dirtyElectricity;

	public bool isStarted;

	public bool isSimulating;

	public int tempDist;

	public BiomeProfile _biome;

	public static bool ignoreSpawnAnime;

	public static List<Thing> Suckers = new List<Thing>();

	public Chara Boss
	{
		get
		{
			return EClass._map.FindChara(base.uidBoss);
		}
		set
		{
			base.uidBoss = value?.uid ?? 0;
		}
	}

	public override int ContentLv
	{
		get
		{
			if (branch == null)
			{
				return DangerLv;
			}
			return branch.ContentLV;
		}
	}

	public override int DangerLv
	{
		get
		{
			if (GetTopZone() != this)
			{
				return GetTopZone().DangerLv + Mathf.Abs(base.lv) - 1;
			}
			return (int)Mathf.Max(1f, (branch != null) ? ((float)branch.DangerLV) : ((float)base._dangerLv + MathF.Abs(base.lv) + (float)DangerLvFix));
		}
	}

	public virtual bool DisableRooms => false;

	public int HourSinceLastActive => EClass.world.date.GetElapsedHour(base.lastActive);

	public int MinsSinceLastActive => EClass.world.date.GetElapsedMins(base.lastActive);

	public virtual string pathExport => CorePath.ZoneSave + idExport.IsEmpty("_new") + ".z";

	public BiomeProfile biome => _biome ?? (_biome = EClass.core.refs.biomes.dict[IdBiome]);

	public virtual string IdBiome => map.config.idBiome.IsEmpty(base.source.idBiome);

	public virtual string IDGenerator => null;

	public virtual string TextWidgetDate => "";

	public MapGenerator Generator => ResourceCache.Load<MapGenerator>("DunGenProfile/Generator_" + GetDungenID());

	public virtual string IdProfile => idProfile.IsEmpty(base.source.idProfile);

	public virtual string IDPlayList => base.source.idPlaylist.IsEmpty((base.lv != 0) ? "Underground" : null);

	public virtual string IDPlaylistOverwrite => null;

	public virtual string IDHat => null;

	public virtual string IDBaseLandFeat => base.Tile.source.trait[0];

	public virtual string idExport
	{
		get
		{
			if (base.source.idFile.Length != 0)
			{
				return base.source.idFile[fileVariation] + ((base.lv == 0) ? "" : ("_F" + base.lv));
			}
			return "";
		}
	}

	public string pathTemp => GameIO.pathTemp + base.uid + "/";

	public Region Region => (this as Region) ?? (parent as Region);

	public Zone ParentZone => parent as Zone;

	public virtual ActionMode DefaultActionMode => ActionMode.Adv;

	public virtual bool BlockBorderExit => base.lv != 0;

	public virtual int ExpireDays => EClass.setting.balance.dateExpireRandomMap;

	public virtual ZoneTransition.EnterState RegionEnterState => ZoneTransition.EnterState.Dir;

	public virtual ZoneFeatureType FeatureType => ZoneFeatureType.Default;

	public virtual string IDAmbience
	{
		get
		{
			if (!IsRegion && !map.IsIndoor)
			{
				if (!IsTown)
				{
					return "forest";
				}
				return "town";
			}
			return null;
		}
	}

	public virtual string IDSceneTemplate
	{
		get
		{
			if (!EClass._map.IsIndoor)
			{
				if (!IsSnowZone)
				{
					return "Default";
				}
				return "Snow";
			}
			return "Indoor";
		}
	}

	public virtual bool IsFestival => false;

	public virtual string IDSubset
	{
		get
		{
			if (!IsFestival)
			{
				return null;
			}
			return "festival";
		}
	}

	public virtual bool IsTown => false;

	public virtual bool CanBeDeliverDestination => IsTown;

	public virtual bool CountDeepestLevel => false;

	public virtual bool HasLaw => false;

	public virtual bool MakeEnemiesNeutral
	{
		get
		{
			if (!IsTown)
			{
				return HasLaw;
			}
			return true;
		}
	}

	public virtual bool MakeTownProperties => HasLaw;

	public virtual bool AllowCriminal => !HasLaw;

	public virtual bool AllowInvest
	{
		get
		{
			if (!IsTown && !IsPCFaction)
			{
				return Guild.GetCurrentGuild() != null;
			}
			return true;
		}
	}

	public virtual bool AllowNewZone => true;

	public virtual bool WillAutoSave => true;

	public virtual bool RegenerateOnEnter => false;

	public virtual bool IsSkyLevel => base.lv > 0;

	public virtual bool IsUnderwater => false;

	public virtual bool IsUserZone => false;

	public virtual bool CanDigUnderground => false;

	public virtual bool LockExit => true;

	public virtual bool CanUnlockExit => !LockExit;

	public virtual int MaxLv => 99999;

	public virtual int MinLv => -99999;

	public virtual bool AddPrefix => false;

	public virtual bool IsNefia => false;

	public virtual bool PetFollow => true;

	public virtual bool RestrictBuild
	{
		get
		{
			if (this is Zone_Civilized)
			{
				return !IsPCFaction;
			}
			return false;
		}
	}

	public virtual bool KeepAllyDistance
	{
		get
		{
			if (!HasLaw)
			{
				return IsPCFaction;
			}
			return true;
		}
	}

	public virtual int MaxSpawn => map.bounds.Width * map.bounds.Height / 175 + 2;

	public int MaxRespawn => (int)((float)MaxSpawn * RespawnRate) + 1;

	public virtual float RuinChance => 0.1f;

	public virtual float OreChance => 1f;

	public virtual float BigDaddyChance => 0f;

	public virtual float EvolvedChance => 0f;

	public virtual float ShrineChance => 0f;

	public virtual float PrespawnRate => 0f;

	public virtual float RespawnRate => PrespawnRate * 0.5f;

	public bool ShowEnemyOnMinimap
	{
		get
		{
			if (instance != null)
			{
				return instance.ShowEnemyOnMinimap;
			}
			return false;
		}
	}

	public virtual int RespawnPerHour => MaxSpawn / 5 + 1;

	public virtual float ChanceSpawnNeutral => 0.05f;

	public virtual bool GrowPlant => base.mainFaction == EClass.pc.faction;

	public virtual bool GrowWeed => GrowPlant;

	public virtual bool IsExplorable => !base.isRandomSite;

	public virtual bool IsReturnLocation
	{
		get
		{
			if (EClass.pc.homeZone != this)
			{
				return base.source.tag.Contains("return");
			}
			return true;
		}
	}

	public virtual bool ShouldMakeExit
	{
		get
		{
			if (base.lv > MinLv)
			{
				return base.lv < MaxLv;
			}
			return false;
		}
	}

	public virtual bool ShouldRegenerate => false;

	public virtual bool ShouldAutoRevive
	{
		get
		{
			if (!ShouldRegenerate)
			{
				return IsPCFaction;
			}
			return true;
		}
	}

	public virtual bool UseFog => base.lv < 0;

	public virtual bool RevealRoom => false;

	public virtual bool AlwaysLowblock => map.IsIndoor;

	public virtual bool UseLight
	{
		get
		{
			if (base.mainFaction != EClass.pc.faction)
			{
				return base.source.tag.Contains("light");
			}
			return true;
		}
	}

	public virtual int StartLV => 0;

	public virtual bool ScaleMonsterLevel => false;

	public virtual bool HiddenInRegionMap => false;

	public virtual FlockController.SpawnType FlockType => FlockController.SpawnType.Default;

	public override string NameSuffix
	{
		get
		{
			if (!IsNefia || !GetTopZone().isConquered)
			{
				return "";
			}
			return "conquered".lang();
		}
	}

	public string NameWithLevel => Name + TextLevel(base.lv);

	public string TextDeepestLv
	{
		get
		{
			if (GetDeepestLv() != 0)
			{
				return "zoneLevelMax".lang(TextLevel(GetDeepestLv()));
			}
			return "";
		}
	}

	public bool CanEnterBuildMode
	{
		get
		{
			if (!EClass.debug.godBuild && !EClass.debug.ignoreBuildRule)
			{
				return base.mainFaction == EClass.pc.faction;
			}
			return true;
		}
	}

	public bool CanEnterBuildModeAnywhere
	{
		get
		{
			if (!EClass.debug.godBuild && !EClass.debug.ignoreBuildRule)
			{
				if (base.mainFaction == EClass.pc.faction)
				{
					return EClass.Branch.elements.Has(4003);
				}
				return false;
			}
			return true;
		}
	}

	public bool IsPCFaction => base.mainFaction == EClass.pc.faction;

	public bool IsPCFactionOrTent
	{
		get
		{
			if (base.mainFaction != EClass.pc.faction)
			{
				return EClass._zone is Zone_Tent;
			}
			return true;
		}
	}

	public bool IsStartZone => this == EClass.game.StartZone;

	public bool IsInstance => instance != null;

	public bool IsLoaded => map != null;

	public virtual int BaseElectricity => 0;

	public bool IsActiveZone => EClass.game.activeZone == this;

	public bool CanInspect
	{
		get
		{
			if (!IsInstance)
			{
				return !HiddenInRegionMap;
			}
			return false;
		}
	}

	public string InspectName => Name + ((IsTown || IsPCFaction || this is Zone_Civilized) ? "" : "dangerLv".lang(DangerLv.ToString() ?? ""));

	public Point InspectPoint => null;

	public Vector3 InspectPosition => default(Vector3);

	public virtual int MaxSoil
	{
		get
		{
			if (branch == null)
			{
				return 99999;
			}
			return branch.MaxSoil;
		}
	}

	public int Evalue(int ele)
	{
		return elements.Value(ele);
	}

	public int Evalue(string alias)
	{
		return elements.Value(EClass.sources.elements.alias[alias].id);
	}

	public virtual string GetDungenID()
	{
		return null;
	}

	public virtual string GetNewZoneID(int level)
	{
		return base.source.id;
	}

	public override string ToString()
	{
		return Name + "(" + base.uid + ")(" + _regionPos?.ToString() + ") instance?" + IsInstance + "/" + EClass.world.date.GetRemainingHours(base.dateExpire) + "h";
	}

	public string TextLevel(int _lv)
	{
		if (_lv != 0)
		{
			if (base.lv <= 0)
			{
				return " " + "zoneLevelB".lang((_lv * -1).ToString() ?? "");
			}
			return " " + "zoneLevel".lang((_lv + 1).ToString() ?? "");
		}
		return "";
	}

	public override void OnCreate()
	{
		events.zone = this;
		base.lv = StartLV;
	}

	public override void OnAfterCreate()
	{
		if (AddPrefix)
		{
			if (GetTopZone() == this)
			{
				base.idPrefix = EClass.sources.zoneAffixes.rows.RandomItem().id;
			}
			else
			{
				base.idPrefix = GetTopZone().idPrefix;
			}
		}
	}

	public void Activate()
	{
		if (IsRegion)
		{
			EClass.scene.elomapActor.Initialize(Region.elomap);
		}
		isStarted = (isSimulating = false);
		base.isKnown = true;
		EClass.game.config.reverseSnow = false;
		if (RegenerateOnEnter && EClass.game.activeZone != null && EClass.game.activeZone.GetTopZone() != GetTopZone() && EClass.world.date.IsExpired(base.dateExpire))
		{
			Msg.Say("regenerateZone", Name);
			ClearZones();
			if (EClass.pc.global.transition != null)
			{
				EClass.pc.global.transition.uidLastZone = 0;
			}
		}
		_ = EClass.game.activeZone;
		if (EClass.game.activeZone != null)
		{
			EClass.game.activeZone.Deactivate();
		}
		EClass.game.activeZone = this;
		ZoneExportData zoneExportData = null;
		Debug.Log(NameWithLevel + "/" + id + "/" + base.uid + "/" + base.isGenerated + "/" + IsLoaded + "/" + File.Exists(base.pathSave + "map"));
		if (base.isGenerated && !IsLoaded && !File.Exists(base.pathSave + "map"))
		{
			Debug.Log("(Bug) File does not exist:" + destryoed + "/" + base.pathSave);
			base.isGenerated = false;
		}
		bool flag = false;
		Debug.Log(idCurrentSubset + "/" + IDSubset);
		bool flag2 = idCurrentSubset != IDSubset || forceSubset != null;
		if (flag2 && map != null)
		{
			UnloadMap();
		}
		Debug.Log(idExport + "/" + File.Exists(pathExport) + "/" + pathExport);
		if (!base.isGenerated && (idExport.IsEmpty() || !File.Exists(pathExport)))
		{
			Debug.Log("generating random map");
			flag = true;
			base.dateExpire = EClass.world.date.GetRaw() + 1440 * ExpireDays;
			Generate();
			OnGenerateMap();
			if (instance != null)
			{
				instance.OnGenerateMap();
			}
			if (!UseFog)
			{
				map.ForeachCell(delegate(Cell c)
				{
					c.isSeen = true;
				});
			}
			if (!(bp is GameBlueprint))
			{
				AddGlobalCharasOnActivate();
			}
		}
		else if (IsLoaded)
		{
			Debug.Log("zone is already loaded");
			map.SetZone(this);
			EClass.core.textures.ApplyLocalReplace(base.isMapSaved ? base.pathSave : pathTemp);
			AddGlobalCharasOnActivate();
		}
		else
		{
			subset = null;
			bool flag3 = (base.isGenerated && flag2) || (base.isGenerated && !IsInstance && !IsPCFaction && ShouldRegenerate && EClass.world.date.IsExpired(base.dateRegenerate)) || forceRegenerate;
			if (pathExport.IsEmpty() || !File.Exists(pathExport) || EClass.game.isLoading || EClass.player.simulatingZone)
			{
				flag3 = false;
				flag2 = false;
			}
			Debug.Log(base.isGenerated + "/" + flag3 + "/" + flag2 + "/" + IDSubset);
			if (!base.isGenerated || flag3 || flag2)
			{
				Debug.Log("importing map:" + pathExport);
				flag = true;
				base.dateRegenerate = EClass.world.date.GetRaw() + 1440 * EClass.setting.balance.dateRegenerateZone;
				if (!flag3)
				{
					IO.DeleteDirectory(pathTemp + "Texture Replace");
					Debug.Log(pathTemp);
				}
				try
				{
					zoneExportData = Import(pathExport);
					base.isGenerated = true;
					isImported = true;
					if (flag3)
					{
						zoneExportData.orgMap = GameIO.LoadFile<Map>(base.pathSave + "map");
					}
				}
				catch (Exception message)
				{
					Debug.Log(message);
					zoneExportData = Import(CorePath.ZoneSave + "fail.z");
					base.isGenerated = true;
					isImported = true;
				}
			}
			EClass.game.countLoadedMaps++;
			Debug.Log("loading map: imported? " + isImported + " regenerate? " + flag3);
			map = GameIO.LoadFile<Map>((isImported ? pathTemp : base.pathSave) + "map");
			if (map == null)
			{
				EClass.ui.Say("System.IO.EndOfStreamException: Unexpected end of stream:" + Environment.NewLine + "File may be corrupted. Try replacing the following file if you have a backup:" + Environment.NewLine + (isImported ? pathTemp : base.pathSave) + "map");
				return;
			}
			map.SetZone(this);
			map.Load(isImported ? pathTemp : base.pathSave, isImported);
			map.SetReference();
			EClass.core.textures.ApplyLocalReplace(base.isMapSaved ? base.pathSave : pathTemp);
			if (isImported)
			{
				map.deadCharas.Clear();
				map.OnImport(zoneExportData);
				if (UseFog && !flag3)
				{
					map.ForeachCell(delegate(Cell c)
					{
						c.isSeen = false;
					});
				}
				if (zoneExportData.orgMap != null)
				{
					Map orgMap = zoneExportData.orgMap;
					List<Chara> serializedCharas = map.serializedCharas;
					map.charas = orgMap.charas;
					map.serializedCharas = orgMap.serializedCharas;
					map.deadCharas = orgMap.deadCharas;
					byte[] array = orgMap.TryLoadFile(base.pathSave, "flags", EClass._map.Size * EClass._map.Size);
					if (array != null && array.Length == EClass._map.Size * EClass._map.Size)
					{
						for (int i = 0; i < EClass._map.Size; i++)
						{
							for (int j = 0; j < EClass._map.Size; j++)
							{
								map.cells[i, j].isSeen = array[i * EClass._map.Size + j].GetBit(1);
							}
						}
					}
					foreach (Chara item in serializedCharas)
					{
						Debug.Log("Importing New Chara:" + item.id + "/" + item.Name + "/" + item.orgPos);
						map.serializedCharas.Add(item);
					}
					map.things.ForeachReverse(delegate(Thing t)
					{
						if (t.trait is TraitNewZone || t.trait is TraitPowerStatue)
						{
							foreach (Thing thing in orgMap.things)
							{
								if (t.id == thing.id && t.pos.Equals(thing.pos))
								{
									if (t.trait is TraitNewZone)
									{
										RemoveCard(t);
										map.things.Insert(0, thing);
										thing.stackOrder = 0;
									}
									else
									{
										t.ChangeMaterial(thing.material);
										t.isOn = thing.isOn;
									}
									break;
								}
							}
							return;
						}
						if (t.trait is TraitStairsLocked)
						{
							foreach (Thing thing2 in orgMap.things)
							{
								if (thing2.trait is TraitNewZone && t.pos.Equals(thing2.pos))
								{
									RemoveCard(t);
									map.things.Add(thing2);
									break;
								}
							}
							return;
						}
						if (t.id == "medal" || t.id == "856")
						{
							foreach (Thing thing3 in orgMap.things)
							{
								if (t.id == thing3.id && t.pos.Equals(thing3.pos))
								{
									return;
								}
							}
							RemoveCard(t);
						}
					});
					foreach (KeyValuePair<int, int> item2 in EClass._map.backerObjs.ToList())
					{
						EClass._map.GetCell(item2.Key);
						SourceBacker.Row row = EClass.sources.backers.map[item2.Value];
						if (EClass.player.doneBackers.Contains(row.id) && !EClass.core.config.test.ignoreBackerDestoryFlag)
						{
							map.backerObjs.Remove(item2.Key);
						}
					}
					foreach (Chara serializedChara in map.serializedCharas)
					{
						if (serializedChara.orgPos != null && serializedChara.orgPos.IsValid)
						{
							serializedChara.pos.Set(serializedChara.orgPos);
						}
					}
					foreach (Thing thing4 in orgMap.things)
					{
						if (thing4.trait is TraitTent && !thing4.isNPCProperty)
						{
							thing4.AddEditorTag(EditorTag.NoNpcProperty);
							thing4.isSubsetCard = false;
							map.things.Add(thing4);
							Debug.Log(thing4);
						}
					}
				}
			}
			foreach (Thing thing5 in map.things)
			{
				map.AddCardOnActivate(thing5);
			}
			foreach (Chara c3 in map.serializedCharas)
			{
				if (c3.c_uidMaster == 0 || EClass.player.listSummon.Find((Chara c2) => c2.uid == c3.uid) == null)
				{
					map.charas.Add(c3);
					map.AddCardOnActivate(c3);
				}
			}
			map.serializedCharas.Clear();
			if (isImported && IsTown)
			{
				RefreshListCitizen();
			}
			map.RefreshAllTiles();
			AddGlobalCharasOnActivate();
			map.OnLoad();
			if (flag3)
			{
				foreach (Card item3 in map.Cards.ToList())
				{
					if (item3.isSubsetCard)
					{
						item3.Destroy();
					}
				}
			}
			if (isImported)
			{
				idCurrentSubset = forceSubset ?? IDSubset;
				if (idCurrentSubset != null)
				{
					subset = MapSubset.Load(idCurrentSubset);
					subset.Apply();
				}
			}
			if (isImported)
			{
				if (MakeTownProperties)
				{
					foreach (Thing thing6 in map.things)
					{
						thing6.isNPCProperty = !thing6.isHidden && !thing6.HasEditorTag(EditorTag.NoNpcProperty);
					}
				}
				else
				{
					foreach (Thing thing7 in map.things)
					{
						thing7.isNPCProperty = false;
					}
				}
				OnGenerateMap();
				if (instance != null)
				{
					instance.OnGenerateMap();
				}
			}
			if (isImported && !flag3 && !RevealRoom)
			{
				foreach (Room item4 in map.rooms.listRoom)
				{
					if (!item4.HasRoof || item4.data.atrium)
					{
						continue;
					}
					foreach (Point point2 in item4.points)
					{
						point2.cell.isSeen = false;
					}
				}
			}
			if (flag3)
			{
				OnRegenerate();
			}
		}
		PathManager.Instance._pathfinder.PunishChangeDirection = false;
		isImported = false;
		if (this is Zone_Tent && IsUnderwater)
		{
			EClass._map.config.idSceneProfile = "indoor_water";
			EClass.scene.profile = SceneProfile.Load(EClass._map.config.idSceneProfile);
		}
		if (flag && IsTown && base.lv == 0)
		{
			SpawnLostItems();
		}
		if (base.visitCount == 0)
		{
			base.dateRevive = EClass.world.date.GetRaw() + 1440 * EClass.setting.balance.dateRevive;
		}
		map.ForeachCell(delegate(Cell c)
		{
			if (c.HasFire)
			{
				map.effectManager.GetOrCreate(c.GetSharedPoint());
			}
			if (IsRegion)
			{
				c.decal = 0;
			}
		});
		if (EClass.world.weather.IsRaining)
		{
			RainWater();
		}
		if (EClass.debug.revealMap)
		{
			map.ForeachCell(delegate(Cell c)
			{
				c.isSeen = true;
			});
		}
		isStarted = true;
		Map.isDirtySunMap = true;
		map.RefreshAllTiles();
		if (events.listPreEnter.Count > 0)
		{
			foreach (ZonePreEnterEvent item5 in events.listPreEnter)
			{
				item5.Execute();
			}
			events.listPreEnter.Clear();
		}
		foreach (Card card in map.Cards)
		{
			card.CalculateFOV();
			if (card.isChara)
			{
				Chara chara = card.Chara;
				chara.dirtySpeed = true;
				if (!card.isDyed && card.HasTag(CTAG.random_color))
				{
					card.DyeRandom();
				}
				if (card.IsUnique && !card.IsPCFaction && !card.IsPCParty)
				{
					Point point = chara.orgPos ?? card.pos;
					card.c_uniqueData = new UniqueData
					{
						x = point.x,
						y = point.z,
						uidZone = base.uid
					};
				}
				int @int = card.GetInt(55);
				if (@int != 0)
				{
					foreach (Chara chara2 in map.charas)
					{
						if (chara2.uid == @int)
						{
							if (chara.IsHostile(chara2))
							{
								chara.enemy = chara2;
								chara.SetAI(new GoalCombat());
								chara.calmCheckTurn = 20 + EClass.rnd(30);
							}
							break;
						}
					}
					card.SetInt(55);
				}
				chara.SyncRide();
				if (card.c_uidMaster != 0 && chara.master == null)
				{
					chara.FindMaster();
				}
				if (!EClass.game.isLoading)
				{
					chara.enemy = null;
					if (chara.IsInCombat)
					{
						chara.SetNoGoal();
					}
				}
			}
			else if (card.IsInstalled && card.trait is TraitDoor traitDoor && card.pos.HasChara && !traitDoor.IsOpen())
			{
				traitDoor.ToggleDoor(sound: false, refresh: false);
			}
		}
		RefreshHat();
		forceRegenerate = false;
		forceSubset = null;
		EClass.ui.OnActivateZone();
		EClass.scene.RebuildActorEx();
		EClass.Sound.LoadAmbience(IDAmbience);
		if (EClass.Branch != null)
		{
			EClass.Branch.OnActivateZone();
		}
		OnVisit();
		if (flag)
		{
			OnVisitNewMapOrRegenerate();
		}
		Guild.GetCurrentGuild()?.RefreshDevelopment();
		if (IsPCFaction)
		{
			EClass.player.uidLastTown = 0;
		}
		else if (IsTown && base.lv == 0)
		{
			EClass.player.uidLastTown = base.uid;
		}
		RefreshBGM();
		Rand.InitBytes(map.seed + 1);
		RefreshElectricity();
		okaerinko = 0;
		if (EClass.debug.enable)
		{
			ModInfluence(2000);
		}
		if (this is Zone_TinkerCamp)
		{
			Tutorial.Reserve("tinker");
		}
		else if (this is Zone_Town && !(this is Zone_SubTown))
		{
			Tutorial.Reserve("town");
		}
	}

	public void RefreshHat()
	{
		if (idHat != null && EClass.world.date.IsExpired(base.dateHat))
		{
			idHat = null;
			base.dateHat = 0;
		}
		sourceHat = ((idHat != null) ? EClass.sources.cards.map[idHat] : ((IDHat != null) ? EClass.sources.cards.map[IDHat] : null));
	}

	public void OnVisit()
	{
		if (CountDeepestLevel && DangerLv > EClass.player.stats.deepest)
		{
			EClass.player.stats.deepest = DangerLv;
		}
		if (EClass.world.date.IsExpired(base.dateRevive))
		{
			ResetHostility();
			Revive();
			foreach (Chara chara in EClass._map.charas)
			{
				chara.TryRestock(onCreate: false);
				if (!chara.IsPCFaction)
				{
					chara.c_fur = 0;
				}
			}
		}
		RefreshCriminal();
		EClass._map.rooms.AssignCharas();
		events.OnVisit();
		OnActivate();
		UpdateQuests();
		OnBeforeSimulate();
		isSimulating = true;
		Simulate();
		isSimulating = false;
		OnAfterSimulate();
		if (EClass.Branch != null)
		{
			EClass.Branch.OnAfterSimulate();
		}
		base.lastActive = EClass.world.date.GetRaw();
		if (!EClass.game.isLoading)
		{
			base.visitCount++;
		}
		base.version = EClass.core.version.GetInt();
	}

	public void Revive()
	{
		base.dateRevive = EClass.world.date.GetRaw() + 1440 * EClass.setting.balance.dateRevive;
		if (ShouldAutoRevive)
		{
			foreach (Chara deadChara in map.deadCharas)
			{
				if (deadChara.trait.CanAutoRevive && deadChara.CanRevive())
				{
					deadChara.Revive();
					if (deadChara.isBackerContent && EClass.player.doneBackers.Contains(deadChara.c_idBacker) && !EClass.core.config.test.ignoreBackerDestoryFlag)
					{
						deadChara.RemoveBacker();
					}
					EClass._zone.AddCard(deadChara, (deadChara.orgPos != null && deadChara.orgPos.IsInBounds) ? deadChara.orgPos : deadChara.pos);
				}
			}
		}
		foreach (Chara value in EClass.game.cards.globalCharas.Values)
		{
			if (value.isDead && value.CanRevive() && value.homeZone == this)
			{
				value.Revive();
				Point point = GetSpawnPos(value);
				if (value.orgPos != null && value.orgPos.IsInBounds)
				{
					point = value.orgPos.GetNearestPoint() ?? point;
				}
				EClass._zone.AddCard(value, point);
			}
		}
		map.deadCharas.Clear();
	}

	public virtual void OnRegenerate()
	{
	}

	public virtual void OnActivate()
	{
	}

	public virtual void OnBeforeDeactivate()
	{
	}

	public virtual void OnDeactivate()
	{
	}

	public virtual void OnBeforeSimulate()
	{
	}

	public virtual void OnVisitNewMapOrRegenerate()
	{
	}

	public virtual void OnAfterSimulate()
	{
	}

	public virtual void OnAdvanceHour()
	{
	}

	public void Simulate()
	{
		if (!EClass.game.isLoading && base.visitCount > 0)
		{
			if (Boss != null && IsNefia)
			{
				Msg.Say("bossLeave", Boss.Name.ToTitleCase(), EClass._zone.Name);
				EClass._zone.RemoveCard(Boss);
				EClass._zone.GetTopZone().isConquered = true;
			}
			int num = Mathf.Clamp(MinsSinceLastActive, 1, 10000);
			foreach (Chara chara2 in EClass._map.charas)
			{
				if (chara2.hp < 0)
				{
					chara2.hp = 0;
				}
				if (!chara2.IsPCParty)
				{
					if (chara2.mana.value < 0)
					{
						chara2.mana.value = 0;
					}
					if (chara2.stamina.value < 0)
					{
						chara2.stamina.value = 0;
					}
				}
				if (num > 60)
				{
					chara2.resistCon = null;
				}
				if (chara2.rarity >= Rarity.Legendary && !chara2.IsPCFaction && num > 0)
				{
					Msg.ignoreAll = true;
					chara2.Cure(CureType.Boss, 20 + num * 10);
					chara2.HealHP(Mathf.Max(1, chara2.MaxHP) * Mathf.Min(num, 20) / 20);
					chara2.mana.Mod(Mathf.Max(1, chara2.mana.max) * Mathf.Min(num, 20) / 20);
					chara2.stamina.Mod(Mathf.Max(1, chara2.stamina.max) * Mathf.Min(num, 20) / 20);
					Msg.ignoreAll = false;
				}
			}
		}
		Debug.Log("Last Active:" + base.lastActive);
		if (base.lastActive == 0 || HourSinceLastActive <= 1 || IsRegion)
		{
			return;
		}
		Debug.Log(Name + " Simulate:" + HourSinceLastActive + " hours");
		VirtualDate virtualDate = new VirtualDate(HourSinceLastActive);
		List<Chara> list = EClass._map.charas.ToList();
		int num2 = HourSinceLastActive / 24;
		int num3 = 0;
		if (num2 > 0)
		{
			foreach (Chara item in list)
			{
				if (!item.IsPCParty)
				{
					item.OnSleep(200, num2);
					if (item.conSleep != null)
					{
						item.conSleep.Kill();
					}
					if (EClass.rnd(EClass.world.date.IsNight ? 20 : 200) == 0 && !item.IsPCFaction)
					{
						item.AddCondition<ConSleep>(1000 + EClass.rnd(1000), force: true);
					}
					if (item.things.Count > 20)
					{
						item.ClearInventory(ClearInventoryType.Purge);
					}
				}
			}
		}
		VirtualDate.current = virtualDate;
		for (int i = 0; i < HourSinceLastActive; i++)
		{
			virtualDate.SimulateHour();
			if (!virtualDate.IsWinter)
			{
				num3++;
			}
		}
		EClass._map.things.ForeachReverse(delegate(Thing t)
		{
			t.DecayNatural(HourSinceLastActive);
		});
		VirtualDate.current = null;
		if (!IsPCFaction)
		{
			return;
		}
		int num4 = 0;
		foreach (Chara item2 in list)
		{
			if (item2.IsPCParty)
			{
				continue;
			}
			if (!item2.IsHomeMember())
			{
				if (item2.id == "bee")
				{
					num4++;
				}
				if (num2 > 0 && item2.IsGuest())
				{
					item2.ChooseNewGoal();
					item2.ai.SimulateZone(num2);
				}
				continue;
			}
			if (num2 > 0)
			{
				Goal goalWork = item2.GetGoalWork();
				item2.SetAI(goalWork);
				if (goalWork is GoalWork)
				{
					(goalWork as GoalWork).FindWork(item2);
				}
				item2.ai.SimulateZone(num2);
				goalWork = item2.GetGoalHobby();
				item2.SetAI(goalWork);
				if (goalWork is GoalWork)
				{
					(goalWork as GoalWork).FindWork(item2);
				}
				item2.ai.SimulateZone(num2);
			}
			item2.ChooseNewGoal();
			if (item2.conSuspend == null)
			{
				item2.ai.OnSimulatePosition();
			}
		}
		List<Thing> list2 = new List<Thing>();
		foreach (Thing thing in map.things)
		{
			if (thing.IsInstalled && thing.trait is TraitBeekeep)
			{
				list2.Add(thing);
			}
		}
		if (num4 >= list2.Count)
		{
			return;
		}
		for (int j = num4; j < list2.Count; j++)
		{
			if (EClass.rnd(200) <= HourSinceLastActive)
			{
				Chara chara = CharaGen.Create("bee");
				AddCard(chara, list2.RandomItem().pos);
				Hostility c_originalHostility = (chara.hostility = Hostility.Neutral);
				chara.c_originalHostility = c_originalHostility;
			}
		}
	}

	public Point GetSpawnPos(Chara c, ZoneTransition.EnterState destState = ZoneTransition.EnterState.Auto)
	{
		ZoneTransition zoneTransition = c.global?.transition;
		Zone zone = zoneTransition?.lastZone;
		ZoneTransition.EnterState enterState = zoneTransition?.state ?? destState;
		bool flag = zone == null || zone.lv > base.lv;
		if (IsRegion)
		{
			if (zone != null && !EClass.player.simulatingZone)
			{
				Zone topZone = zone.GetTopZone();
				if (GetZoneAt(topZone.x, topZone.y) != null)
				{
					return new Point(topZone.mapX, topZone.mapY);
				}
			}
		}
		else if (c.IsPC && EClass.player.lastZonePos != null && zone != null && zone.IsRegion && (enterState == ZoneTransition.EnterState.Left || enterState == ZoneTransition.EnterState.Right || enterState == ZoneTransition.EnterState.Top || enterState == ZoneTransition.EnterState.Bottom))
		{
			return EClass.player.lastZonePos;
		}
		if (enterState == ZoneTransition.EnterState.Region)
		{
			enterState = RegionEnterState;
		}
		if (this is Zone_Kapul && enterState == ZoneTransition.EnterState.Left)
		{
			enterState = ZoneTransition.EnterState.Right;
		}
		float rate = zoneTransition?.ratePos ?? (-1f);
		if (IsPCFaction)
		{
			if ((uint)(enterState - 3) <= 4u || enterState == ZoneTransition.EnterState.Return)
			{
				Thing spot = EClass._map.props.installed.Find<TraitSpotGuidePC>();
				if (spot != null)
				{
					Point nearestPoint = (spot.trait as TraitSpotGuidePC).GetRandomPoint((Point p) => !p.IsBlocked && !p.Equals(spot.pos)).GetNearestPoint();
					if (nearestPoint != null && nearestPoint.IsValid)
					{
						return nearestPoint;
					}
				}
			}
		}
		while (true)
		{
			switch (enterState)
			{
			case ZoneTransition.EnterState.UndergroundOrSky:
				return new Point(zoneTransition.x, zoneTransition.z);
			case ZoneTransition.EnterState.Teleport:
			{
				foreach (Thing thing2 in map.things)
				{
					if (thing2.IsInstalled)
					{
						TraitTeleporter traitTeleporter = thing2.trait as TraitTeleporter;
						if (traitTeleporter != null)
						{
							Debug.Log(zoneTransition.idTele + "/" + traitTeleporter.id.IsEmpty(traitTeleporter.GetParam(3)) + "/" + traitTeleporter.GetParam(3));
						}
						if (traitTeleporter != null && !zoneTransition.idTele.IsEmpty() && zoneTransition.idTele == traitTeleporter.id.IsEmpty(traitTeleporter.GetParam(3)))
						{
							return thing2.pos.GetNearestPoint();
						}
					}
				}
				foreach (Thing thing3 in map.things)
				{
					if (thing3.IsInstalled && thing3.trait is TraitTeleporter traitTeleporter2 && traitTeleporter2.IsFor(zoneTransition.lastZone))
					{
						return thing3.pos.GetNearestPoint();
					}
				}
				Thing randomThing2 = EClass._map.props.installed.traits.GetRandomThing<TraitTeleporter>();
				if (randomThing2 != null)
				{
					return randomThing2.pos.GetNearestPoint();
				}
				goto case ZoneTransition.EnterState.Return;
			}
			case ZoneTransition.EnterState.Return:
			case ZoneTransition.EnterState.Elevator:
			case ZoneTransition.EnterState.Moongate:
			{
				if (enterState == ZoneTransition.EnterState.Elevator)
				{
					foreach (Thing thing4 in map.things)
					{
						if (thing4.IsInstalled && thing4.trait is TraitElevator traitElevator && traitElevator.IsFor(zoneTransition.lastZone))
						{
							return thing4.pos.GetNearestPoint();
						}
					}
					Thing randomThing = EClass._map.props.installed.traits.GetRandomThing<TraitElevator>();
					if (randomThing != null)
					{
						return randomThing.pos.GetNearestPoint();
					}
				}
				Thing thing = null;
				thing = map.props.installed.traits.GetRandomThing<TraitWaystone>();
				if (thing != null)
				{
					return thing.pos.GetNearestPoint();
				}
				thing = map.props.installed.traits.GetRandomThing<TraitCoreZone>();
				if (thing != null)
				{
					return thing.pos.GetNearestPoint();
				}
				if (base.lv == 0)
				{
					goto IL_0499;
				}
				flag = base.lv <= 0;
				break;
			}
			case ZoneTransition.EnterState.Center:
			case ZoneTransition.EnterState.Encounter:
				if (map.config.embarkX != 0)
				{
					return new Point(map.config.embarkX, map.config.embarkY);
				}
				return map.GetCenterPos().GetNearestPoint(allowBlock: false, allowChara: false);
			case ZoneTransition.EnterState.Top:
				return map.bounds.GetTopPos(rate).GetNearestPoint(allowBlock: false, allowChara: false);
			case ZoneTransition.EnterState.Right:
				return map.bounds.GetRightPos(rate).GetNearestPoint(allowBlock: false, allowChara: false);
			case ZoneTransition.EnterState.Bottom:
				return map.bounds.GetBottomPos(rate).GetNearestPoint(allowBlock: false, allowChara: false);
			case ZoneTransition.EnterState.Left:
				return map.bounds.GetLeftPos(rate).GetNearestPoint(allowBlock: false, allowChara: false);
			case ZoneTransition.EnterState.Dead:
			case ZoneTransition.EnterState.Exact:
			case ZoneTransition.EnterState.PortalReturn:
			case ZoneTransition.EnterState.Fall:
				if (zoneTransition.x == 0 && EClass._map.bounds.x != 0)
				{
					return new Point(map.config.embarkX, map.config.embarkY);
				}
				return new Point(zoneTransition.x, zoneTransition.z);
			case ZoneTransition.EnterState.RandomVisit:
				return GetRandomVisitPos(c);
			case ZoneTransition.EnterState.Down:
				flag = true;
				break;
			case ZoneTransition.EnterState.Up:
				flag = false;
				break;
			}
			break;
			IL_0499:
			enterState = ZoneTransition.EnterState.Center;
		}
		foreach (Thing thing5 in map.things)
		{
			if (thing5.trait is TraitNewZone { zone: not null } traitNewZone && zone != null && traitNewZone.zone.uid == zone.uid)
			{
				if (c != null && enterState != 0)
				{
					c.SetDir(traitNewZone.owner.dir);
				}
				return traitNewZone.GetExitPos();
			}
		}
		foreach (Thing thing6 in map.things)
		{
			if (thing6.trait is TraitNewZone traitNewZone2 && ((flag && traitNewZone2.IsUpstairs) || (!flag && traitNewZone2.IsDownstairs)))
			{
				if (c != null && enterState != 0)
				{
					c.SetDir(traitNewZone2.owner.dir);
				}
				return traitNewZone2.GetExitPos();
			}
		}
		return GetRandomVisitPos(c);
	}

	public Point GetRandomVisitPos(Chara c)
	{
		Point point = null;
		if (EClass.rnd(3) == 0 && map.rooms.listRoom.Count > 0)
		{
			point = map.rooms.listRoom.RandomItem().points.RandomItem();
		}
		if (point == null && EClass.rnd(4) != 0)
		{
			IEnumerable<Chara> ie = map.charas.Where((Chara t) => t.trait.ShopType != 0 && t.pos != null && t.pos.IsValid);
			if (ie.Count() > 0)
			{
				point = ie.RandomItem().pos.GetRandomPoint(3);
			}
		}
		if (point == null)
		{
			point = map.bounds.GetRandomSurface(centered: false, walkable: true, !IsPCFaction && !(this is Zone_Civilized)) ?? map.bounds.GetRandomPoint();
		}
		return point.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false) ?? EClass._map.GetCenterPos();
	}

	public void AddGlobalCharasOnActivate()
	{
		Point spawnPosPC = null;
		if (EClass.pc.currentZone == this)
		{
			spawnPosPC = GetSpawnPos(EClass.pc);
		}
		if (spawnPosPC == null)
		{
			spawnPosPC = map.GetCenterPos();
		}
		if (spawnPosPC.IsValid && EClass.pc.global.transition != null && spawnPosPC.HasBlock)
		{
			spawnPosPC = spawnPosPC.GetNearestPoint();
		}
		spawnPosPC = spawnPosPC.Clamp(useBounds: true).GetNearestPoint();
		foreach (Chara c in EClass.game.cards.globalCharas.Values)
		{
			if (c.currentZone != this)
			{
				continue;
			}
			if (c.parent is Chara)
			{
				Chara chara = c.parent as Chara;
				c.currentZone = chara.currentZone;
			}
			else
			{
				if (c.isDead)
				{
					continue;
				}
				if (c.isRestrained && c.currentZone == EClass.pc.currentZone && (!c.pos.IsValid || c.pos.FindThing<TraitShackle>() == null))
				{
					c.isRestrained = false;
				}
				if (c.global.transition != null)
				{
					Point pos = (c.IsPC ? spawnPosPC : (c.IsPCParty ? spawnPosPC.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: true, ignoreCenter: true) : GetSpawnPos(c)));
					if (c.IsPCParty && !c.IsPC)
					{
						if (c.host == EClass.pc)
						{
							pos.Set(spawnPosPC);
						}
						else if (pos.Equals(spawnPosPC) || !PathManager.Instance.IsPathClear(spawnPosPC, pos, c, 5))
						{
							c.pos.Set(spawnPosPC);
							if (!spawnPosPC.ForeachNearestPoint(delegate(Point p)
							{
								if (PathManager.Instance.IsPathClear(spawnPosPC, p, c, 10) && !p.Equals(spawnPosPC))
								{
									pos.Set(p);
									return true;
								}
								return false;
							}, allowBlock: false, EClass.pc.party.members.Count >= 12, allowInstalled: true, ignoreCenter: true, EClass._zone.IsRegion ? 2 : 6))
							{
								pos.Set(spawnPosPC);
							}
						}
					}
					c.pos.Set(pos);
					c.global.transition = null;
				}
				map.charas.Add(c);
				map.AddCardOnActivate(c);
			}
		}
		foreach (Chara item in EClass.player.listSummon)
		{
			Point nearestPoint = spawnPosPC.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: true, ignoreCenter: true);
			item.enemy = null;
			item.pos.Set(nearestPoint);
			map.charas.Add(item);
			map.AddCardOnActivate(item);
		}
		EClass.player.listSummon.Clear();
	}

	public void Deactivate()
	{
		if (!IsUserZone && !IsPCFaction && map != null)
		{
			List<Thing> list = new List<Thing>();
			foreach (Thing thing in map.things)
			{
				if (thing.rarity >= Rarity.Artifact && thing.HasTag(CTAG.godArtifact))
				{
					list.Add(thing);
				}
			}
			if (this is Zone_Tent)
			{
				foreach (Thing item in map.props.stocked.Things.Concat(map.props.roaming.Things))
				{
					if (item.IsContainer)
					{
						foreach (Thing thing2 in item.things)
						{
							if (thing2.trait is TraitTent)
							{
								list.Add(thing2);
							}
						}
					}
					if (item.trait is TraitTent)
					{
						list.Add(item);
					}
				}
			}
			if (list.Count > 0)
			{
				Msg.Say("pick_valuable");
				foreach (Thing item2 in list)
				{
					Msg.Say("pick_valuable2", item2);
					EClass.pc.AddCard(item2);
				}
			}
		}
		base.isPeace = false;
		OnBeforeDeactivate();
		if (IsPCFaction)
		{
			foreach (Chara member in branch.members)
			{
				member.ValidateWorks();
			}
		}
		EClass.game.lastActiveZone = EClass.game.activeZone;
		base.lastActive = EClass.world.date.GetRaw();
		map.OnDeactivate();
		EClass.scene.Clear();
		EClass.game.activeZone = null;
		if (IsInstance)
		{
			UnloadMap();
			base.isGenerated = false;
		}
		if (forceRegenerate)
		{
			UnloadMap();
		}
		OnDeactivate();
	}

	public void OnKillGame()
	{
		foreach (Thing thing in map.things)
		{
			if (thing.renderer.hasActor)
			{
				thing.renderer.KillActor();
			}
		}
	}

	public void UnloadMap()
	{
		map = null;
		if (bp != null)
		{
			bp.map = null;
		}
		if (branch != null)
		{
			branch.OnUnloadMap();
		}
		Debug.Log("Unloaded Map:" + this);
	}

	public void ClaimZone(bool debug = false)
	{
		EClass._map.RevealAll();
		SetMainFaction(EClass.pc.faction);
		branch = new FactionBranch();
		branch.OnCreate(this);
		if (base.icon == 0)
		{
			base.icon = 332;
		}
		instance = null;
		base.dateExpire = 0;
		SetInt(2, EClass.world.date.GetRaw());
		Register();
		foreach (Thing thing in map.things)
		{
			thing.isNPCProperty = false;
		}
		EClass.Branch.OnClaimZone();
		EClass.scene.elomap.SetZone(EClass._zone.x, EClass._zone.y, EClass._zone, updateMesh: true);
		if (debug)
		{
			for (int i = 0; i < 7; i++)
			{
				Chara chara = CharaGen.CreateFromFilter("c_neutral");
				EClass._zone.AddCard(chara, EClass._map.bounds.GetRandomPoint().GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false));
				branch.AddMemeber(chara);
			}
		}
		else
		{
			Msg.Say("claimedZone");
			EClass.Branch.Log("claimedZone");
			EClass.Sound.Play("jingle_embark");
			EClass.pc.PlaySound("build");
			Effect.Get("aura_heaven").Play(EClass.pc.pos);
			Point nearestPoint = EClass.pc.pos.GetNearestPoint(allowBlock: false, allowChara: true, allowInstalled: false);
			EClass._zone.AddCard(ThingGen.Create("core_zone"), nearestPoint).SetPlaceState(PlaceState.installed);
		}
		base.idPrefix = 0;
		if (EClass._zone == EClass.game.StartZone)
		{
			EClass.player.spawnZone = EClass._zone;
		}
		if (EClass._zone != EClass.game.StartZone && !(EClass._zone is Zone_Vernis))
		{
			EClass._zone.SetBGM(new List<int> { 41, 90, 44, 43 });
			EClass._zone.RefreshPlaylist();
		}
	}

	public void AbandonZone()
	{
		FactionBranch destBranch = ((EClass.pc.homeBranch == branch) ? EClass.game.StartZone.branch : EClass.pc.homeBranch);
		branch.members.ForeachReverse(delegate(Chara m)
		{
			destBranch.AddMemeber(m);
			if (!m.IsPCParty)
			{
				m.MoveZone(destBranch.owner);
			}
		});
		EClass.Branch.OnUnclaimZone();
		SetMainFaction(null);
		base.dateExpire = EClass.world.date.GetRaw() + 10080;
	}

	public bool CanDestroy()
	{
		if (IsActiveZone || IsPCFaction || EClass.player.simulatingZone)
		{
			return false;
		}
		if (IsInstance)
		{
			return true;
		}
		bool flag = false;
		if (id == "cave_yeek" && !(this is Zone_DungeonYeek))
		{
			flag = true;
		}
		Zone topZone = GetTopZone();
		if (!flag)
		{
			if (base.dateExpire == 0 || !EClass.world.date.IsExpired(base.dateExpire))
			{
				return false;
			}
			if (!base.isRandomSite && !(this is Zone_Field) && (topZone == null || topZone == this || topZone.FindDeepestZone() == this))
			{
				return false;
			}
		}
		if (EClass.pc.currentZone == null || EClass.pc.currentZone is Zone_Tent || topZone == EClass.pc.currentZone.GetTopZone() || (EClass.player.nextZone != null && topZone == EClass.player.nextZone.GetTopZone()))
		{
			return false;
		}
		if (EClass.pc.currentZone.IsInstance)
		{
			Zone zone = EClass.game.spatials.Find(EClass.pc.currentZone.instance.uidZone);
			if (zone != null && zone.GetTopZone() == this)
			{
				return false;
			}
		}
		return true;
	}

	public override void _OnBeforeSave()
	{
		if (CanDestroy())
		{
			Debug.Log("Deleting Zone:" + this);
			if (!destryoed)
			{
				Destroy();
			}
		}
		else if (map != null)
		{
			Debug.Log("Saving Zone:" + this);
			map.Save(base.pathSave);
			if (!base.isMapSaved)
			{
				base.isMapSaved = true;
				IO.DeleteDirectory(base.pathSave + "Texture Replace");
				IO.CopyDir(pathTemp + "Texture Replace", base.pathSave + "/Texture Replace");
			}
			if (!IsActiveZone)
			{
				UnloadMap();
			}
		}
	}

	public override void OnLoad()
	{
		if (branch != null)
		{
			branch.SetOwner(this);
		}
		events.OnLoad(this);
	}

	public override void Destroy()
	{
		children.ForeachReverse(delegate(Spatial z)
		{
			z.Destroy();
		});
		if (map != null)
		{
			UnloadMap();
		}
		if (parent != null && parent.IsRegion && instance == null)
		{
			EClass.scene.elomap.SetZone(base.x, base.y, null, updateMesh: true);
		}
		EClass.game.spatials.Remove(this);
		destryoed = true;
		base.isGenerated = false;
	}

	public void ClearZones(Zone current = null)
	{
		if (map != null)
		{
			UnloadMap();
		}
		base.isGenerated = false;
		Zone topZone = GetTopZone();
		if (topZone != this)
		{
			topZone.ClearZones(this);
			return;
		}
		children.ForeachReverse(delegate(Spatial c)
		{
			if (c != current)
			{
				c.Destroy();
			}
		});
	}

	public void OnChildNumChange(Card c)
	{
	}

	public ICardParent GetRoot()
	{
		return this;
	}

	public Zone GetTopZone()
	{
		if (parent == null || parent.IsRegion)
		{
			return this;
		}
		return parent as Zone;
	}

	public Card AddCardSplinkle(Card t, Point center, int radius = 4)
	{
		Point point = new Point(center);
		for (int i = 0; i < 1000; i++)
		{
			point.x = center.x + EClass.rnd(radius) - EClass.rnd(radius);
			point.z = center.z + EClass.rnd(radius) - EClass.rnd(radius);
			if (point.IsValid && !point.IsBlocked && !point.HasChara)
			{
				return AddCard(t, point);
			}
		}
		return AddCard(t, center);
	}

	public Card AddChara(string id, int x, int z)
	{
		return AddCard(CharaGen.Create(id), x, z);
	}

	public Card AddThing(string id, int x, int z)
	{
		return AddCard(ThingGen.Create(id), x, z);
	}

	public Card AddThing(string id, Point p)
	{
		return AddThing(id, p.x, p.z);
	}

	public Card AddCard(Card t, Point point)
	{
		return AddCard(t, point.x, point.z);
	}

	public Card AddCard(Card t)
	{
		return AddCard(t, 0, 0);
	}

	public Card AddCard(Card t, int x, int z)
	{
		if (t.parent != null)
		{
			t.parent.RemoveCard(t);
		}
		t.parent = this;
		Chara chara = t.Chara;
		if (chara != null)
		{
			chara.currentZone = this;
			chara.SetAI(new NoGoal());
		}
		if (IsActiveZone)
		{
			map.OnCardAddedToZone(t, x, z);
			if (isStarted && t.isThing && t.placeState == PlaceState.roaming && !ignoreSpawnAnime)
			{
				t.PlayAnimeLoot();
			}
			ignoreSpawnAnime = false;
		}
		if (chara != null && EClass.player != null && !chara.hasSpawned)
		{
			EClass.player.codex.AddSpawn(chara.id);
			chara.hasSpawned = true;
		}
		return t;
	}

	public void RemoveCard(Card t)
	{
		if (IsActiveZone)
		{
			map.OnCardRemovedFromZone(t);
		}
		t.parent = null;
		if (t.isChara)
		{
			t.Chara.currentZone = null;
		}
		if (t.isThing && !t.trait.IDActorEx.IsEmpty())
		{
			EClass.scene.RemoveActorEx(t);
		}
		if (t.renderer.hasActor)
		{
			t.renderer.OnLeaveScreen();
		}
	}

	public T GetRandomSpot<T>() where T : Trait
	{
		return EClass._map.props.installed.traits.GetRandomThing<T>() as T;
	}

	public bool TryAddThingInSpot<T>(Thing t, bool useContainer = true) where T : Trait
	{
		Thing randomThing = EClass._map.props.installed.traits.GetRandomThing<T>();
		if (randomThing == null)
		{
			AddCard(t, EClass._map.bounds.GetRandomSurface());
			return false;
		}
		if (useContainer && (!t.IsContainer || t.things.Count == 0))
		{
			List<Thing> list = new List<Thing>();
			foreach (Point item in randomThing.trait.ListPoints(null, onlyPassable: false))
			{
				foreach (Card item2 in item.ListCards())
				{
					if (item2.IsContainer && !(item2.trait is TraitDeliveryChest))
					{
						list.Add(item2.Thing);
					}
				}
			}
			if (TryAddThingInSharedContainer(t, list, add: true, msg: false, null, sharedOnly: false))
			{
				return true;
			}
		}
		AddCard(t, randomThing.trait.GetRandomPoint());
		return true;
	}

	public List<Thing> TryListThingsInSpot<T>(Func<Thing, bool> func = null) where T : TraitSpot
	{
		List<T> list = new List<T>();
		List<Thing> list2 = new List<Thing>();
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.IsInstalled && thing.trait is T)
			{
				list.Add(thing.trait as T);
			}
		}
		foreach (T item in list)
		{
			foreach (Point item2 in item.ListPoints(null, onlyPassable: false))
			{
				foreach (Thing thing2 in item2.Things)
				{
					if (!thing2.IsInstalled)
					{
						continue;
					}
					if (thing2.things.Count == 0)
					{
						if (IsValid(thing2))
						{
							list2.Add(thing2);
						}
						continue;
					}
					foreach (Thing item3 in thing2.things.List((Thing _t) => IsValid(_t)))
					{
						list2.Add(item3);
					}
				}
			}
		}
		return list2;
		bool IsValid(Thing t)
		{
			if (func != null && !func(t))
			{
				return false;
			}
			return true;
		}
	}

	public bool TryAddThingInSharedContainer(Thing t, List<Thing> containers = null, bool add = true, bool msg = false, Chara chara = null, bool sharedOnly = true)
	{
		Thing dest = null;
		int priority = -1;
		ContainerFlag flag = t.category.GetRoot().id.ToEnum<ContainerFlag>();
		if (flag == ContainerFlag.none)
		{
			flag = ContainerFlag.other;
		}
		if (containers == null)
		{
			containers = EClass._map.props.installed.containers;
		}
		if (SearchDest() != null)
		{
			return true;
		}
		if (dest == null)
		{
			return false;
		}
		if (add)
		{
			if (msg)
			{
				chara.Say("putSharedItem", chara, t, dest.GetName(NameStyle.Full));
			}
			dest.AddThing(t);
		}
		return true;
		Thing SearchDest()
		{
			foreach (Thing container in containers)
			{
				Thing thing = container;
				if (thing.trait is TraitShippingChest)
				{
					thing = EClass.game.cards.container_shipping;
				}
				if ((!sharedOnly || thing.IsSharedContainer) && thing.c_lockLv <= 0 && (thing.things.Count < thing.things.MaxCapacity || thing.things.CanStack(t) != t))
				{
					Window.SaveData windowSaveData = thing.GetWindowSaveData();
					if (windowSaveData != null)
					{
						if (windowSaveData.priority <= priority || (windowSaveData.noRotten && t.IsDecayed) || (windowSaveData.onlyRottable && t.trait.Decay == 0))
						{
							continue;
						}
						Window.SaveData.FilterResult filterResult = Window.SaveData.FilterResult.Pass;
						if (windowSaveData.userFilter)
						{
							filterResult = windowSaveData.IsFilterPass(t.GetName(NameStyle.Full, 1));
							if (filterResult == Window.SaveData.FilterResult.Block)
							{
								continue;
							}
						}
						if (filterResult != Window.SaveData.FilterResult.PassWithoutFurtherTest)
						{
							if (windowSaveData.advDistribution)
							{
								bool flag2 = false;
								foreach (int cat in windowSaveData.cats)
								{
									if (t.category.uid == cat)
									{
										flag2 = true;
										break;
									}
								}
								if (!flag2)
								{
									continue;
								}
							}
							else if (windowSaveData.flag.HasFlag(flag))
							{
								continue;
							}
						}
						priority = windowSaveData.priority;
					}
					else
					{
						if (priority != -1)
						{
							continue;
						}
						priority = 0;
					}
					dest = thing;
				}
			}
			return null;
		}
	}

	public bool TryAddThing(Thing target, Point p, bool destroyIfFail = false)
	{
		int num = 0;
		if (p.cell.detail != null)
		{
			foreach (Thing thing in p.cell.detail.things)
			{
				if (thing.placeState == PlaceState.roaming)
				{
					if (target.TryStackTo(thing))
					{
						return true;
					}
					num++;
				}
			}
		}
		if (num == 0 || !destroyIfFail)
		{
			EClass._zone.AddCard(target, p);
			return true;
		}
		target.Destroy();
		return false;
	}

	public Thing TryGetThingFromSharedContainer(Func<Thing, bool> func)
	{
		foreach (Thing container in EClass._map.props.installed.containers)
		{
			if (container.IsSharedContainer)
			{
				Thing thing = container.things.Find(func);
				if (thing != null)
				{
					return thing;
				}
			}
		}
		return null;
	}

	public Thing TryGetRestock<T>(string idCat) where T : TraitSpot
	{
		List<T> list = new List<T>();
		foreach (Thing thing2 in EClass._map.things)
		{
			if (thing2.IsInstalled && thing2.trait is T)
			{
				list.Add(thing2.trait as T);
			}
		}
		foreach (T item in list)
		{
			foreach (Point item2 in item.ListPoints(null, onlyPassable: false))
			{
				foreach (Thing thing3 in item2.Things)
				{
					if (!thing3.IsInstalled || thing3.isSale)
					{
						continue;
					}
					if (thing3.things.Count == 0)
					{
						if (IsValid(thing3, insideContainer: false))
						{
							return thing3;
						}
						continue;
					}
					Thing thing = thing3.things.Find((Thing _t) => IsValid(_t, insideContainer: true));
					if (thing != null)
					{
						return thing;
					}
				}
			}
		}
		return null;
		bool IsValid(Thing t, bool insideContainer)
		{
			if (t.category.id != idCat || !TraitSalesTag.CanTagSale(t, insideContainer))
			{
				return false;
			}
			return true;
		}
	}

	public ZoneProfile GetProfile()
	{
		string text = IdProfile;
		if (text.IsEmpty())
		{
			Region region = parent as Region;
			if (base.lv != 0)
			{
				text = ((base.lv < 0) ? "Underground" : "Sky");
			}
			else if (region != null)
			{
				EClass.scene.elomapActor.Initialize(region.elomap);
				text = EClass.scene.elomapActor.elomap.GetTileInfo(base.x, base.y).idZoneProfile;
				if (bp != null)
				{
					name = Lang.GetList("zone_" + text.Split('/')[1]).RandomItem();
					bp.surrounding = new EloMap.TileInfo[3, 3];
					for (int i = 0; i < 3; i++)
					{
						for (int j = 0; j < 3; j++)
						{
							bp.surrounding[j, i] = EClass.scene.elomapActor.elomap.GetTileInfo(base.x - 1 + j, base.y - 1 + i);
						}
					}
					if (text == "Random/R_Shore")
					{
						base.isBeach = true;
					}
				}
			}
			else
			{
				text = "Random";
			}
			idProfile = text;
		}
		return ZoneProfile.Load(text);
	}

	public void CreateBP()
	{
		bp = new ZoneBlueprint();
		bp.Create();
		bp.genSetting.seed = base.Seed;
		OnCreateBP();
	}

	public virtual void OnCreateBP()
	{
	}

	public void Generate()
	{
		base.isGenerated = true;
		if (bp == null)
		{
			CreateBP();
		}
		if (bp.map == null)
		{
			bp.GenerateMap(this);
		}
		map.SetZone(this);
		Zone_Field zone_Field = this as Zone_Field;
		if (IdBiome == "Sand" || IdBiome == "Water" || IsUnderwater)
		{
			int num = 1 + EClass.rnd((IdBiome == "Water") ? 3 : 2);
			for (int i = 0; i < num; i++)
			{
				Point randomSurface = EClass._map.bounds.GetRandomSurface(centered: false, walkable: true, allowWater: true);
				if (!randomSurface.HasObj && !randomSurface.HasThing)
				{
					Thing t = ThingGen.Create("pearl_oyster", new string[3] { "wood_birch", "poplar", "coralwood" }.RandomItem(), ContentLv);
					EClass._zone.AddCard(t, randomSurface).Install();
				}
			}
			num = 4 + EClass.rnd(5);
			for (int j = 0; j < num; j++)
			{
				Point randomSurface2 = EClass._map.bounds.GetRandomSurface(centered: false, walkable: true, allowWater: true);
				if (!randomSurface2.HasObj && !randomSurface2.HasThing && (IsUnderwater || randomSurface2.cell.IsTopWaterAndNoSnow || EClass.rnd(6) == 0))
				{
					EClass._zone.AddCard(ThingGen.Create("70", -1, ContentLv), randomSurface2);
				}
			}
		}
		if (IsUnderwater)
		{
			for (int k = 0; k < 30 + EClass.rnd(30); k++)
			{
				SpawnMob(null, SpawnSetting.Fish());
			}
			Crawler crawler = Crawler.Create("pasture");
			int num2 = (EClass.debug.enable ? 3 : EClass.rnd(EClass.rnd(EClass.rnd(EClass.rnd(5) + 1) + 1) + 1));
			bool flag = this is Zone_Field;
			Thing seed = null;
			if (IsNefia)
			{
				int num3 = Mathf.Min(EClass._zone.DangerLv, EClass.pc.Evalue(286) * 2 / 3);
				if (num3 > 0)
				{
					seed = TraitSeed.MakeSeed(EClass.sources.objs.map[137]);
					Rand.SetSeed(EClass._zone.uid * 10 + num3);
					TraitSeed.LevelSeed(seed, (seed.trait as TraitSeed).row, num3);
					Rand.SetSeed();
					seed.elements.SetBase(2, EClass.curve(seed.encLV, 50, 10, 80));
				}
			}
			crawler.CrawlUntil(EClass._map, () => EClass._map.GetRandomPoint(), num2 + (flag ? 4 : 0), delegate(Crawler.Result r)
			{
				int num6 = 137;
				foreach (Point point in r.points)
				{
					if (!point.cell.isModified && !point.HasThing && !point.HasBlock && !point.HasObj)
					{
						map.SetObj(point.x, point.z, num6);
						int idx = 3 + ((EClass.rnd(3) == 0) ? 1 : 0) + ((EClass.rnd(3) == 0) ? (-1) : 0) + ((EClass.rnd(3) == 0) ? (-1) : 0);
						point.growth.SetStage(idx);
						if (seed != null)
						{
							EClass._map.AddPlant(point, seed);
						}
					}
				}
				return false;
			});
			crawler.CrawlUntil(tries: EClass.rnd(EClass.rnd(5) + 1) + 1 + (flag ? 20 : 0), map: EClass._map, onStart: () => EClass._map.GetRandomPoint(), canComplete: delegate(Crawler.Result r)
			{
				int num5 = 136;
				foreach (Point point2 in r.points)
				{
					if (!point2.cell.isModified && !point2.HasThing && !point2.HasBlock && !point2.HasObj)
					{
						map.SetObj(point2.x, point2.z, num5, 1, EClass.rnd(4));
					}
				}
				return false;
			});
			crawler.CrawlUntil(tries: EClass.rnd(EClass.rnd(10) + 1) + 3 + (flag ? 40 : 0), map: EClass._map, onStart: () => EClass._map.GetRandomPoint(), canComplete: delegate(Crawler.Result r)
			{
				int idFloor = 121;
				foreach (Point point3 in r.points)
				{
					map.SetFloor(point3.x, point3.z, 97, idFloor, Mathf.Clamp(4 - r.startPos.Distance(point3) + EClass.rnd(3) - EClass.rnd(3), 0, 3));
				}
				return false;
			});
		}
		if (zone_Field != null)
		{
			if (EClass.rnd(3) == 0)
			{
				int num4 = EClass.rnd(2);
				for (int l = 0; l < num4; l++)
				{
					Point randomSurface3 = EClass._map.bounds.GetRandomSurface();
					if (!randomSurface3.HasObj && !randomSurface3.HasThing)
					{
						Card t2 = ThingGen.Create("chest3").ChangeMaterial(biome.style.matDoor);
						EClass._zone.AddCard(t2, randomSurface3).Install();
					}
				}
			}
			if (EClass.rnd(8) == 0)
			{
				SpawnAltar();
			}
			TrySpawnFollower();
		}
		map.plDay = CreatePlaylist(ref map._plDay, EClass.Sound.GetPlaylist(IDPlayList) ?? EClass.Sound.GetPlaylist("Day"));
	}

	public void TrySpawnFollower()
	{
		bool flag = EClass.pc.HasCondition<ConDrawBacker>();
		if (!EClass.debug.enable && EClass.rnd(flag ? 3 : 20) != 0)
		{
			return;
		}
		Point randomSurface = EClass._map.bounds.GetRandomSurface();
		if (!randomSurface.IsValid)
		{
			return;
		}
		Chara c = CharaGen.Create("follower");
		EClass._zone.AddCard(c, randomSurface);
		(EClass._zone.AddThing("gallows", randomSurface).Install().trait as TraitShackle).Restrain(c);
		c.c_rescueState = RescueState.WaitingForRescue;
		if (EClass.rnd(flag ? 1 : 2) == 0 || EClass.debug.enable)
		{
			SourceBacker.Row row = EClass.sources.backers.listFollower.NextItem(ref BackerContent.indexFollower);
			if (row != null)
			{
				c.ApplyBacker(row.id);
			}
		}
		Religion faith = EClass.game.religions.dictAll.Values.Where((Religion a) => a != c.faith).RandomItem();
		for (int i = 0; i < 3 + EClass.rnd(4); i++)
		{
			Chara chara = CharaGen.Create("fanatic");
			chara.SetFaith(faith);
			Point point = randomSurface.GetRandomPoint(4) ?? randomSurface.GetNearestPoint();
			EClass._zone.AddCard(chara, point);
		}
	}

	public void SpawnAltar()
	{
		EClass.core.refs.crawlers.start.CrawlUntil(map, () => map.poiMap.GetCenterCell().GetCenter(), 1, delegate(Crawler.Result r)
		{
			if (r.points.Count <= 4)
			{
				return false;
			}
			map.poiMap.OccyupyPOI(r.points[0]);
			List<Point> points = r.points;
			Religion randomReligion = EClass.game.religions.GetRandomReligion();
			"altarPoint".lang(randomReligion.NameDomain.lang());
			Thing thing = ThingGen.Create("altar");
			(thing.trait as TraitAltar).SetDeity(randomReligion.id);
			Chara t = CharaGen.Create("twintail");
			EClass._zone.AddCard(t, points.RandomItem());
			for (int i = 0; i < 2 + EClass.rnd(2); i++)
			{
				Chara t2 = CharaGen.Create("twintail");
				EClass._zone.AddCard(t2, points.RandomItem());
			}
			if (points[0].Installed == null)
			{
				EClass._zone.AddCard(thing, points[0]).Install();
			}
			foreach (Point item in points)
			{
				if (item.x % 3 == 0 && item.z % 2 == 0 && item != points[0] && !item.Equals(points[0].Front) && item.Installed == null)
				{
					thing = ThingGen.Create("pillar1");
					EClass._zone.AddCard(thing, item).Install();
				}
				item.SetObj();
				item.SetFloor(3, 6);
			}
			return true;
		});
	}

	public virtual void OnGenerateMap()
	{
		if (MakeEnemiesNeutral)
		{
			foreach (Chara chara in EClass._map.charas)
			{
				if (!chara.IsGlobal && chara.hostility < Hostility.Neutral && chara.OriginalHostility < Hostility.Friend)
				{
					Hostility hostility2 = (chara.c_originalHostility = Hostility.Neutral);
					chara.hostility = hostility2;
				}
			}
		}
		if (PrespawnRate != 0f && !IsSkyLevel)
		{
			for (int i = 0; i < (int)((float)MaxSpawn * PrespawnRate); i++)
			{
				SpawnMob();
			}
		}
		TryGenerateOre();
		TryGenerateBigDaddy();
		TryGenerateEvolved();
		TryGenerateShrine();
	}

	public void TryGenerateOre()
	{
		if (OreChance <= 0f)
		{
			return;
		}
		Crawler.Create("ore").CrawlUntil(tries: EClass.rnd((int)((float)(map.bounds.Width * map.bounds.Height / 200 + 1) * OreChance + 2f)), map: EClass._map, onStart: () => EClass._map.bounds.GetRandomPoint(), canComplete: delegate(Crawler.Result r)
		{
			byte b = 18;
			string group = "ore";
			if (EClass.rnd(5) == 0)
			{
				b++;
				group = "gem";
			}
			SourceMaterial.Row randomMaterial = MATERIAL.GetRandomMaterial(DangerLv, group);
			foreach (Point point in r.points)
			{
				if (point.sourceBlock.ContainsTag("ore"))
				{
					map.SetObj(point.x, point.z, randomMaterial.id, b, 1, 0);
				}
			}
			return false;
		});
	}

	public Chara TryGenerateEvolved(bool force = false, Point p = null)
	{
		if (!force && EvolvedChance <= EClass.rndf(1f))
		{
			return null;
		}
		Chara chara = SpawnMob(p, SpawnSetting.Evolved());
		for (int i = 0; i < 2 + EClass.rnd(2); i++)
		{
			chara.ability.AddRandom();
		}
		chara.AddThing(chara.MakeGene(DNA.Type.Default));
		if (EClass.rnd(2) == 0)
		{
			chara.AddThing(chara.MakeGene(DNA.Type.Superior));
		}
		return chara;
	}

	public void TryGenerateBigDaddy()
	{
		if (!(BigDaddyChance <= EClass.rndf(1f)))
		{
			int num = DangerLv * 125 / 100;
			if (num >= 30)
			{
				CardBlueprint.Set(new CardBlueprint
				{
					lv = num
				});
			}
			Chara t = CharaGen.Create("big_daddy");
			EClass._zone.AddCard(t, GetSpawnPos(SpawnPosition.Random, 10000));
			Msg.Say("sign_bigdaddy");
		}
	}

	public void TryGenerateShrine()
	{
		for (int i = 0; i < 3; i++)
		{
			Rand.SetSeed(base.uid + i);
			if (ShrineChance <= EClass.rndf(1f))
			{
				continue;
			}
			Point randomSpace = EClass._map.GetRandomSpace(3, 3);
			if (randomSpace == null)
			{
				continue;
			}
			randomSpace.x++;
			randomSpace.z++;
			if (randomSpace.HasThing || randomSpace.HasChara)
			{
				continue;
			}
			randomSpace.SetObj();
			Rand.SetSeed(EClass.player.seedShrine);
			EClass.player.seedShrine++;
			if (EClass.rnd(EClass.debug.test ? 2 : 15) == 0)
			{
				EClass._zone.AddCard(ThingGen.Create("pedestal_power"), randomSpace).Install();
				EClass._zone.AddCard(ThingGen.Create(EClass.gamedata.godStatues.RandomItemWeighted((GodStatueData a) => a.chance).idThing, -1, DangerLv), randomSpace).Install();
			}
			else
			{
				EClass._zone.AddCard(ThingGen.Create("statue_power", -1, DangerLv), randomSpace).Install().SetRandomDir();
			}
		}
		Rand.SetSeed();
	}

	public void ResetHostility()
	{
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.source.hostility.IsEmpty() && chara.source.hostility.ToEnum<Hostility>() >= Hostility.Friend && !chara.IsPCFactionOrMinion)
			{
				chara.c_originalHostility = (Hostility)0;
			}
			chara.hostility = chara.OriginalHostility;
			if (chara.enemy != null && (chara.enemy.IsPCFaction || chara.IsPCFaction))
			{
				chara.SetEnemy();
			}
		}
	}

	public virtual void OnGenerateRooms(BaseMapGen gen)
	{
	}

	public Point GetSpawnPos(SpawnPosition type, int tries = 100)
	{
		Point point = new Point();
		for (int i = 0; i < tries; i++)
		{
			point = EClass._map.bounds.GetRandomSurface(centered: false, walkable: true, allowWater: true);
			if (point.IsValid && !point.cell.hasDoor && !point.IsSync && (type != SpawnPosition.Outside || (!point.cell.HasRoof && point.cell.light <= 0)))
			{
				return point;
			}
		}
		return null;
	}

	public Chara SpawnMob(Point pos = null, SpawnSetting setting = null)
	{
		if (setting == null)
		{
			setting = SpawnSetting.Default;
		}
		if (pos == null)
		{
			pos = GetSpawnPos(setting.position, setting.tries);
			if (pos == null)
			{
				pos = GetSpawnPos(SpawnPosition.Random, setting.tries);
				if (pos == null)
				{
					return null;
				}
			}
		}
		BiomeProfile biome = pos.cell.biome;
		if (IsUnderwater && EClass.rnd(15) != 0)
		{
			biome = ((EClass.rnd(4) != 0) ? EClass.core.refs.biomes.Water : EClass.core.refs.biomes.Sand);
		}
		SpawnList spawnList = null;
		spawnList = ((setting.idSpawnList != null) ? SpawnList.Get(setting.idSpawnList) : ((EClass._zone is Zone_DungeonYeek) ? SpawnListChara.Get("dungeon_yeek", (SourceChara.Row r) => r.race == "yeek") : ((setting.hostility == SpawnHostility.Neutral || (setting.hostility != SpawnHostility.Enemy && Rand.Range(0f, 1f) < ChanceSpawnNeutral)) ? SpawnList.Get("c_neutral") : ((biome.spawn.chara.Count <= 0) ? SpawnList.Get(biome.name, "chara", new CharaFilter
		{
			ShouldPass = delegate(SourceChara.Row s)
			{
				if (s.hostility != "")
				{
					return false;
				}
				return s.biome == biome.name || s.biome.IsEmpty();
			}
		}) : SpawnList.Get(biome.spawn.GetRandomCharaId())))));
		int dangerLv = DangerLv;
		CardBlueprint cardBlueprint = new CardBlueprint
		{
			rarity = Rarity.Normal,
			idEle = setting.idEle
		};
		int num = ((setting.filterLv == -1) ? dangerLv : setting.filterLv);
		if (ScaleMonsterLevel)
		{
			num = ((dangerLv - 1) % 50 + 5) * 150 / 100;
			if (num >= 20 && EClass.rnd(100) < num)
			{
				num = dangerLv;
			}
		}
		CardRow cardRow = (setting.id.IsEmpty() ? spawnList.Select(num, setting.levelRange) : EClass.sources.cards.map[setting.id]);
		int num2 = ((setting.fixedLv == -1) ? cardRow.LV : setting.fixedLv);
		if (ScaleMonsterLevel)
		{
			num2 = (50 + cardRow.LV) * Mathf.Max(1, (dangerLv - 1) / 50);
		}
		if (setting.rarity == Rarity.Random)
		{
			if (EClass.rnd(100) == 0)
			{
				cardBlueprint.rarity = Rarity.Legendary;
				num2 = num2 * 125 / 100;
			}
		}
		else
		{
			cardBlueprint.rarity = setting.rarity;
		}
		if (setting.isBoss)
		{
			num2 = num2 * 150 / 100;
		}
		if (setting.isEvolved)
		{
			num2 = num2 * 2 + 20;
		}
		if (num2 != cardRow.LV)
		{
			cardBlueprint.lv = num2;
		}
		CardBlueprint.Set(cardBlueprint);
		Chara chara = CharaGen.Create(cardRow.id, num);
		AddCard(chara, pos);
		if (setting.forcedHostility.HasValue)
		{
			Hostility c_originalHostility = (chara.hostility = setting.forcedHostility.Value);
			chara.c_originalHostility = c_originalHostility;
		}
		if (setting.isBoss)
		{
			chara.c_bossType = BossType.Boss;
		}
		if (setting.isEvolved)
		{
			chara.c_bossType = BossType.Evolved;
		}
		return chara;
	}

	public void RefreshElectricity()
	{
		dirtyElectricity = false;
		bool flag = GetSoilCost() > MaxSoil;
		base.electricity = elements.Value(2201) * 10 + BaseElectricity;
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.IsInstalled && thing.trait.Electricity != 0 && (thing.isOn || thing.trait.Electricity > 0))
			{
				base.electricity += thing.trait.Electricity;
			}
		}
		if (!flag)
		{
			EClass._map.bounds.ForeachCell(delegate(Cell c)
			{
				if (c.sourceObj.id == 118 && c.growth.IsMature)
				{
					base.electricity += 20;
				}
			});
		}
		foreach (Thing thing2 in EClass._map.things)
		{
			if (thing2.IsInstalled)
			{
				thing2.trait.TryToggle();
			}
		}
	}

	public int GetElectricity(bool cost = false)
	{
		bool flag = GetSoilCost() > MaxSoil;
		int sum = 0;
		foreach (Thing thing in EClass._map.things)
		{
			if (!thing.IsInstalled || thing.trait.Electricity == 0)
			{
				continue;
			}
			if (cost)
			{
				if (thing.trait.Electricity < 0)
				{
					sum += -thing.trait.Electricity;
				}
			}
			else if (thing.trait.Electricity > 0)
			{
				sum += thing.trait.Electricity;
			}
		}
		if (!cost)
		{
			sum += elements.Value(2201) * 10 + BaseElectricity;
			if (!flag)
			{
				EClass._map.bounds.ForeachCell(delegate(Cell c)
				{
					if (c.sourceObj.id == 118 && c.growth.IsMature)
					{
						sum += 20;
					}
				});
			}
		}
		return sum;
	}

	public void SetBGM(List<int> ids, bool refresh = true)
	{
		map._plDay.Clear();
		if (ids.Count > 0)
		{
			foreach (int id in ids)
			{
				if (id != -1)
				{
					map._plDay.Add(id);
				}
			}
		}
		UnityEngine.Object.DestroyImmediate(map.plDay);
		map.plDay = null;
		RefreshPlaylist();
		if (refresh)
		{
			EClass.Sound.StopBGM();
			RefreshBGM();
		}
	}

	public void SetBGM(int id = -1, bool refresh = true)
	{
		SetBGM(new List<int> { id }, refresh);
	}

	public void RefreshPlaylist()
	{
		if (map.plDay == null)
		{
			map.plDay = CreatePlaylist(ref map._plDay, EClass.Sound.GetPlaylist(IDPlayList));
		}
	}

	public void RefreshBGM()
	{
		if (!EClass.pc.IsInActiveZone || EClass.player.simulatingZone)
		{
			return;
		}
		RefreshPlaylist();
		Playlist playlist = map.plDay;
		foreach (ZoneEvent item in events.list)
		{
			if (item.playlist != null)
			{
				playlist = item.playlist;
			}
		}
		if (IDPlaylistOverwrite != null)
		{
			playlist = EClass.Sound.GetPlaylist(IDPlaylistOverwrite);
		}
		if (EClass.pc.IsInActiveZone)
		{
			Room room = EClass.pc.pos.cell.room;
			if (room != null && room.lot != null && room.lot.idBGM != 0)
			{
				playlist = EClass.Sound.plLot;
				if (playlist.list[0].data?.id != room.lot.idBGM)
				{
					playlist.list[0].data = EClass.core.refs.dictBGM.TryGetValue(room.lot.idBGM);
					playlist.Reset();
					if (!LayerDrama.keepBGM)
					{
						EClass.Sound.StopBGM(1f);
					}
				}
			}
		}
		EClass.core.config.SetBGMInterval();
		EClass.Sound.SwitchPlaylist(playlist, !LayerDrama.keepBGM);
	}

	public Playlist CreatePlaylist(ref List<int> list, Playlist mold = null)
	{
		Playlist playlist = EClass.Sound.plBlank.Instantiate();
		if (list.Count == 0 && (bool)mold)
		{
			list = mold.ToInts();
			playlist.shuffle = mold.shuffle;
			playlist.minSwitchTime = mold.minSwitchTime;
			playlist.nextBGMOnSwitch = mold.nextBGMOnSwitch;
			playlist.ignoreLoop = mold.ignoreLoop;
			playlist.keepBGMifSamePlaylist = mold.keepBGMifSamePlaylist;
			playlist.name = mold.name;
		}
		foreach (int item in list)
		{
			playlist.list.Add(new Playlist.Item
			{
				data = EClass.core.refs.dictBGM[item]
			});
		}
		return playlist;
	}

	public Chara FindChara(string id)
	{
		return map.charas.Find((Chara c) => c.id == id);
	}

	public Chara FindChara(int uid)
	{
		return map.charas.Find((Chara c) => c.uid == uid);
	}

	public int GetDeepestLv()
	{
		int max = base.lv;
		return GetDeepestLv(ref max);
	}

	public int GetDeepestLv(ref int max)
	{
		if (Mathf.Abs(base.lv) > Mathf.Abs(max))
		{
			max = base.lv;
		}
		foreach (Spatial child in children)
		{
			(child as Zone).GetDeepestLv(ref max);
		}
		return max;
	}

	public List<Element> ListLandFeats()
	{
		if (landFeats == null)
		{
			landFeats = new List<int>();
			Rand.SetSeed(EClass._zone.uid);
			string[] listBase = IDBaseLandFeat.Split(',');
			string[] array = listBase;
			foreach (string text in array)
			{
				if (!text.IsEmpty())
				{
					landFeats.Add(EClass.sources.elements.alias[text].id);
				}
			}
			if (listBase.Length == 1)
			{
				List<SourceElement.Row> list = EClass.sources.elements.rows.Where(delegate(SourceElement.Row e)
				{
					if (e.category != "landfeat" || e.chance == 0)
					{
						return false;
					}
					bool flag = true;
					string[] tag = e.tag;
					foreach (string text2 in tag)
					{
						if (text2.StartsWith("bf"))
						{
							flag = false;
							if (listBase[0] == text2)
							{
								flag = true;
								break;
							}
						}
					}
					return flag ? true : false;
				}).ToList();
				SourceElement.Row row = list.RandomItemWeighted((SourceElement.Row e) => e.chance);
				landFeats.Add(row.id);
				list.Remove(row);
				row = list.RandomItemWeighted((SourceElement.Row e) => e.chance);
				landFeats.Add(row.id);
			}
			Rand.SetSeed();
		}
		List<Element> list2 = new List<Element>();
		foreach (int landFeat in landFeats)
		{
			list2.Add(Element.Create(landFeat, 1));
		}
		return list2;
	}

	public ZoneExportData Import(string path)
	{
		ZipFile zipFile = ZipFile.Read(path);
		zipFile.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
		zipFile.ExtractAll(pathTemp);
		zipFile.Dispose();
		return IO.LoadFile<ZoneExportData>(pathTemp + "export") ?? new ZoneExportData();
	}

	public void Export(string path, PartialMap partial = null, bool usermap = false)
	{
		if (subset != null)
		{
			SE.Beep();
			return;
		}
		try
		{
			ZoneExportData zoneExportData = new ZoneExportData
			{
				name = name,
				usermap = usermap
			};
			IO.CreateTempDirectory();
			if (!map.config.retainDecal)
			{
				map.ClearRainAndDecal();
			}
			map.Save(IO.TempPath + "/", zoneExportData, partial);
			map.ExportMetaData(IO.TempPath + "/", Path.GetFileNameWithoutExtension(path), partial);
			if (partial == null)
			{
				IO.CopyDir(base.pathSave + "Texture Replace", IO.TempPath + "/Texture Replace");
			}
			IO.SaveFile(IO.TempPath + "/export", zoneExportData, compress: true);
			using (ZipFile zipFile = new ZipFile())
			{
				zipFile.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
				zipFile.AddDirectory(IO.TempPath);
				zipFile.Save(path);
				zipFile.Dispose();
			}
			IO.DeleteTempDirectory();
		}
		catch (Exception ex)
		{
			EClass.ui.Say(ex.Message + ":" + path);
		}
	}

	public void ExportDialog(string dir = null)
	{
		EClass.core.WaitForEndOfFrame(delegate
		{
			string text = StandaloneFileBrowser.SaveFilePanel("Export Zone", dir ?? CorePath.ZoneSave, "new zone", "z");
			if (!string.IsNullOrEmpty(text))
			{
				Export(text);
				Msg.SayRaw("Exported Zone");
			}
		});
	}

	public void ImportDialog(string dir = null)
	{
		EClass.core.WaitForEndOfFrame(delegate
		{
			string[] array = StandaloneFileBrowser.OpenFilePanel("Import Zone", dir ?? CorePath.ZoneSave, "z", multiselect: false);
			if (array.Length != 0)
			{
				Zone_User zone_User = SpatialGen.Create("user", EClass.world.region, register: true) as Zone_User;
				zone_User.path = array[0];
				Thing thing = ThingGen.Create("teleporter");
				thing.c_uidZone = zone_User.uid;
				EClass._zone.AddCard(thing, EClass.pc.pos);
			}
		});
	}

	public static bool IsImportValid(string path)
	{
		try
		{
			return Map.GetMetaData(path)?.IsValidVersion() ?? false;
		}
		catch (Exception ex)
		{
			EClass.ui.Say(ex.Message);
			return false;
		}
	}

	public void Export()
	{
		EClass._map.ResetEditorPos();
		string text = pathExport;
		IO.Copy(text, CorePath.ZoneSave + "Backup/");
		Export(text);
		Msg.Say("Exported Map:" + text);
	}

	public void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
	}

	public void OnInspect()
	{
	}

	public int GetSortVal()
	{
		if (IsPCFaction)
		{
			return -100000 + base.uid;
		}
		if (this is Zone_Civilized)
		{
			return -90000 + base.uid;
		}
		return base.uid;
	}

	public Chara AddRandomVisitor(bool guest = false)
	{
		Trait random = map.Installed.traits.GetTraitSet<TraitSpotExit>().GetRandom();
		if (random == null)
		{
			return null;
		}
		Point point = random.GetPoint();
		Chara chara = null;
		if (guest)
		{
			Zone z = EClass.world.FindZone("wilds");
			chara = EClass.game.cards.ListGlobalChara(z).RandomItem();
			if (chara != null)
			{
				AddCard(chara, point);
				Msg.Say("guestArrive", chara);
				chara.visitorState = VisitorState.Arrived;
			}
		}
		else
		{
			chara = CharaGen.CreateFromFilter("c_wilds");
			AddCard(chara, point);
			chara.goalListType = GoalListType.Enemy;
		}
		return chara;
	}

	public void OnSimulateHour(VirtualDate date)
	{
		if (base.IsPlayerFaction)
		{
			branch.OnSimulateHour(date);
		}
		events.OnSimulateHour();
		if (date.IsRealTime)
		{
			foreach (Thing thing in EClass._map.things)
			{
				if (thing.IsInstalled)
				{
					thing.trait.TryToggle();
				}
			}
			EClass.pc.DecayNatural();
		}
		EClass._map.things.ForeachReverse(delegate(Thing t)
		{
			t.OnSimulateHour(date);
		});
		foreach (Thing sucker in Suckers)
		{
			sucker.Destroy();
		}
		Suckers.Clear();
		if (RespawnRate != 0f)
		{
			int num = 0;
			foreach (Chara chara in map.charas)
			{
				if (!chara.IsGlobal)
				{
					num++;
				}
			}
			if (num < MaxRespawn)
			{
				for (int i = 0; i < RespawnPerHour; i++)
				{
					SpawnMob();
				}
			}
		}
		if (!date.IsRealTime && EClass.rnd(24) == 0)
		{
			RainWater();
		}
		if (date.hour == 6)
		{
			GrowPlants(date);
		}
	}

	public void OnSimulateDay(VirtualDate date)
	{
		if (base.IsPlayerFaction)
		{
			branch.OnSimulateDay(date);
		}
	}

	public void OnSimulateMonth(VirtualDate date)
	{
		if (base.IsPlayerFaction)
		{
			branch.OnSimulateMonth(date);
		}
		if (date.IsRealTime)
		{
			EClass._map.RefreshAllTiles();
		}
	}

	public void RainWater()
	{
		if (EClass._map.IsIndoor || !IsPCFaction)
		{
			return;
		}
		EClass._map.bounds.ForeachCell(delegate(Cell c)
		{
			if (c.IsFarmField && !c.HasRoof)
			{
				c.isWatered = true;
			}
		});
	}

	public void GrowPlants(VirtualDate date)
	{
		bool num = (EClass.player.isAutoFarming = IsPCFaction && EClass.Branch.policies.IsActive(2707));
		int weedChance = 1;
		if (IsPCFaction && EClass.Branch.policies.IsActive(2703))
		{
			weedChance += (EClass.debug.enable ? 100000 : 20) + EClass.Branch.Evalue(2703) * 10;
		}
		if (date.sunMap == null)
		{
			date.BuildSunMap();
		}
		if (num)
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (Thing thing in EClass._map.things)
			{
				if (!thing.IsInstalled || !(thing.trait is TraitSpotFarm traitSpotFarm))
				{
					continue;
				}
				foreach (Point item in traitSpotFarm.ListPoints(null, onlyPassable: false))
				{
					hashSet.Add(item.index);
				}
			}
			Perform(hashSet);
			EClass.player.isAutoFarming = false;
			Perform(hashSet);
		}
		else
		{
			Perform(null);
		}
		void Perform(HashSet<int> farmMap)
		{
			bool isWinter = date.IsWinter;
			EClass._map.bounds.ForeachCell(delegate(Cell c)
			{
				if (farmMap != null)
				{
					if (EClass.player.isAutoFarming)
					{
						if (!farmMap.Contains(c.index))
						{
							return;
						}
					}
					else if (farmMap.Contains(c.index))
					{
						return;
					}
				}
				if (c.decal != 0 && EClass.rnd(3) == 0)
				{
					c.decal = 0;
				}
				if (GrowPlant)
				{
					if (c.growth != null)
					{
						bool flag = false;
						if (!EClass.player.isAutoFarming && c.growth.HaltGrowth() && (c.IsFarmField || c.IsTopWater) && (!isWinter || !date.IsRealTime))
						{
							flag = true;
						}
						PlantData plantData = map.TryGetPlant(c);
						if (!flag && (plantData == null || plantData.fert >= 0))
						{
							c.TryGrow(date);
						}
						if (isWinter && plantData != null && c.growth != null && c.growth.NeedSunlight && plantData.fert >= 0 && (EClass.rnd(4) == 0 || c.growth.stage.idx == 0))
						{
							if (date.sunMap == null)
							{
								date.BuildSunMap();
							}
							if (!date.sunMap.Contains(c.index) && !c.growth.CanGrow(date))
							{
								c.growth.Perish();
							}
						}
					}
					else if (c.detail != null)
					{
						c.Things.ForeachReverse(delegate(Thing t)
						{
							if (t.IsInstalled && t.trait is TraitSeed && !t.isSale)
							{
								(t.trait as TraitSeed).TrySprout(force: false, sucker: false, date);
							}
						});
					}
					else if (EClass.rnd(20) == 0 && GrowWeed && c.CanGrowWeed && EClass.rnd(weedChance) == 0)
					{
						biome.Populate(c.GetPoint());
						if (c.growth != null)
						{
							c.growth.SetStage(0);
						}
					}
				}
				c.isWatered = false;
			});
		}
	}

	public Zone GetZoneAt(int _x, int _y)
	{
		if (IsRegion)
		{
			foreach (Spatial child in children)
			{
				if (!(child is Zone_Field) && _x == child.x && _y == child.y)
				{
					return child as Zone;
				}
			}
		}
		foreach (Spatial child2 in children)
		{
			if (_x == child2.x && _y == child2.y)
			{
				return child2 as Zone;
			}
		}
		return null;
	}

	public bool IsCrime(Chara c, Act act)
	{
		if (act.IsHostileAct && HasLaw && !IsPCFaction && c.IsPC)
		{
			return true;
		}
		return false;
	}

	public void RefreshCriminal()
	{
		bool flag = EClass.player.IsCriminal && HasLaw && !AllowCriminal && !IsPCFaction;
		Hostility hostility = (flag ? Hostility.Neutral : Hostility.Friend);
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.trait is TraitGuard)
			{
				chara.hostility = hostility;
				if (!flag && chara.enemy != null && chara.enemy.IsPCParty)
				{
					chara.SetEnemy();
				}
			}
		}
	}

	public void RefreshListCitizen()
	{
		if (base.lv != 0)
		{
			return;
		}
		dictCitizen.Clear();
		foreach (Chara item in map.charas.Concat(map.deadCharas))
		{
			if (item.trait.IsCitizen && !item.IsGlobal && !item.isSubsetCard)
			{
				dictCitizen[item.uid] = item.Name;
			}
		}
	}

	public void ModInfluence(int a)
	{
		base.influence += a;
		if (a > 0)
		{
			Msg.Say("gainInfluence", Name, a.ToString() ?? "");
		}
		Tutorial.Reserve("influence");
	}

	public void ModDevelopment(int a)
	{
		base.development += a;
		if (a > 0)
		{
			Msg.Say("gainDevelopment", Name, a.ToString() ?? "");
		}
	}

	public void UpdateQuests(bool force = false)
	{
		if (!IsPCFaction && (!(this is Zone_Town) || base.lv != 0))
		{
			return;
		}
		Debug.Log("Updating Quest:" + force);
		List<SourceQuest.Row> list = EClass.sources.quests.rows.Where((SourceQuest.Row a) => a.group == "random").ToList();
		int num = 0;
		foreach (Chara item in map.charas.Concat(map.deadCharas))
		{
			if (item.quest != null && !EClass.game.quests.list.Contains(item.quest))
			{
				if (item.quest.IsExpired || completedQuests.Contains(item.quest.uid) || force)
				{
					item.quest = null;
				}
				else
				{
					num++;
				}
			}
		}
		if (EClass._zone.dateQuest > EClass.world.date.GetRaw() && !force)
		{
			return;
		}
		EClass._zone.dateQuest = EClass.world.date.GetRaw() + 1440;
		int maxQuest = 3;
		Rand.UseSeed(base.uid + EClass.player.stats.days / 7 % 100, delegate
		{
			maxQuest = 4 + EClass.rnd(4);
		});
		completedQuests.Clear();
		List<Zone> list2 = Quest.ListDeliver();
		List<Tuple<string, int>> listTag = new List<Tuple<string, int>>();
		string[] array = EClass._zone.source.questTag;
		if (EClass._zone.IsPCFaction)
		{
			array = new string[9] { "supply/8", "deliver/7", "food/8", "escort/4", "deliver/4", "monster/0", "war/0", "farm/0", "music/0" };
		}
		string[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			string[] array3 = array2[i].Split('/');
			listTag.Add(new Tuple<string, int>(array3[0], array3[1].ToInt()));
		}
		for (int j = 0; j < map.charas.Count * 2; j++)
		{
			if (num > maxQuest)
			{
				break;
			}
			if (num > 15)
			{
				break;
			}
			Chara chara = map.charas.RandomItem();
			if (!chara.trait.CanGiveRandomQuest || chara.isSubsetCard || chara.homeZone != EClass._zone || chara.IsGuest() || chara.memberType == FactionMemberType.Livestock || (chara.quest != null && !force))
			{
				continue;
			}
			SourceQuest.Row row = list.RandomItemWeighted(delegate(SourceQuest.Row a)
			{
				int num2 = 1;
				foreach (Tuple<string, int> item2 in listTag)
				{
					if (a.tags.Contains(item2.Item1))
					{
						num2 = item2.Item2;
						break;
					}
				}
				if (!EClass._zone.IsPCFaction && a.tags.Contains("bulk"))
				{
					num2 = 0;
				}
				return a.chance * num2;
			});
			if ((!row.tags.Contains("needDestZone") || list2.Count >= 2) && (row.minFame <= 0 || row.minFame < EClass.player.fame || EClass.debug.enable))
			{
				Quest.Create(row.id, null, chara);
				num++;
			}
		}
	}

	public List<Chara> ListMinions(Chara c)
	{
		List<Chara> list = new List<Chara>();
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.c_uidMaster == c.uid && chara.c_minionType == MinionType.Default)
			{
				list.Add(chara);
			}
		}
		return list;
	}

	public int CountMinions(Chara c)
	{
		int num = 0;
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.c_uidMaster == c.uid && chara.c_minionType == MinionType.Default)
			{
				num++;
			}
		}
		return num;
	}

	public int GetSoilCost()
	{
		int i = 0;
		EClass._map.bounds.ForeachCell(delegate(Cell c)
		{
			i += c.sourceObj.costSoil;
		});
		return i / 10;
	}

	public void SpawnLostItems()
	{
		for (int i = 0; i < 2 + EClass.rnd(4); i++)
		{
			Point point = GetPos();
			if (point == null)
			{
				continue;
			}
			if (EClass.rnd(30) == 0)
			{
				Thing thing = ThingGen.Create("purse");
				thing.isLostProperty = true;
				thing.things.DestroyAll();
				int num2 = (thing.c_lockLv = EClass.rndHalf(Mathf.Min(base.development / 10 + 10, 50)));
				thing.Add("money", EClass.rndHalf(num2 * 60 + 1000));
				if (EClass.rnd(2) == 0)
				{
					thing.Add("plat", EClass.rnd(4));
				}
				else
				{
					thing.Add("medal", EClass.rnd(2));
				}
				EClass._zone.AddCard(thing, point);
			}
			else
			{
				EClass._zone.AddCard(ThingGen.CreateFromCategory("junk"), point);
			}
		}
		static Point GetPos()
		{
			for (int j = 0; j < 10; j++)
			{
				Point randomPoint = EClass._zone.bounds.GetRandomPoint();
				if (!randomPoint.IsBlocked && !randomPoint.HasThing && !randomPoint.HasObj && !randomPoint.HasBlock)
				{
					return randomPoint;
				}
			}
			return null;
		}
	}

	public void ApplyBackerPet(bool draw)
	{
		bool flag = this is Zone_Yowyn && base.lv == -1;
		IList<SourceBacker.Row> list = EClass.sources.backers.listPet.Copy();
		list.Shuffle();
		if (!EClass.core.config.test.ignoreBackerDestoryFlag)
		{
			list.ForeachReverse(delegate(SourceBacker.Row a)
			{
				if (EClass.player.doneBackers.Contains(a.id))
				{
					list.Remove(a);
				}
			});
		}
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.IsGlobal || chara.IsMinion)
			{
				continue;
			}
			if (chara.isBackerContent)
			{
				if (chara.sourceBacker.isStatic != 0)
				{
					continue;
				}
				if (chara.id != "follower")
				{
					chara.RemoveBacker();
				}
			}
			if ((flag && chara.race.id != "cat") || EClass.rnd((!flag) ? (draw ? 3 : 10) : (draw ? 1 : 2)) != 0)
			{
				continue;
			}
			foreach (SourceBacker.Row item in list)
			{
				if (item.chara == chara.id)
				{
					chara.ApplyBacker(item.id);
					list.Remove(item);
					break;
				}
			}
		}
	}

	public FortuneRollData GetOrCreateFortuneRollData(bool refresh = true)
	{
		if (fortuneRoll == null)
		{
			fortuneRoll = new FortuneRollData();
			fortuneRoll.seed = EClass._zone.uid * 100 + EClass.game.seed;
		}
		if (refresh || fortuneRoll.count == 0)
		{
			fortuneRoll.Refresh();
		}
		return fortuneRoll;
	}
}
