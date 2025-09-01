using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class Card : BaseCard, IReservable, ICardParent, IRenderSource, IGlobalValue, IInspect
{
	public enum MoveResult
	{
		Fail,
		Success,
		Door
	}

	public enum MoveType
	{
		Walk,
		Force
	}

	public const int MaxWeight = 10000000;

	public const int SocketDiv = 1000;

	[JsonProperty(PropertyName = "A")]
	public int[] _ints = new int[30];

	[JsonProperty(PropertyName = "B")]
	public string id = "";

	[JsonProperty(PropertyName = "C")]
	public ThingContainer things = new ThingContainer();

	[JsonProperty(PropertyName = "D")]
	public ElementContainerCard elements = new ElementContainerCard();

	[JsonProperty(PropertyName = "E")]
	public Biography bio;

	[JsonProperty(PropertyName = "SC")]
	public List<int> sockets;

	public AIAct reservedAct;

	public Props props;

	public Trait trait;

	public ICardParent parent;

	public Fov fov;

	public Point pos = new Point();

	public CardRenderer renderer;

	public CardRow hat;

	public int turn;

	public int _colorInt;

	public float roundTimer;

	public float angle = 180f;

	public float animeCounter;

	public bool isDestroyed;

	public CardBlueprint bp;

	public BitArray32 _bits1;

	public BitArray32 _bits2;

	public PlaceState placeState;

	public bool dirtyWeight = true;

	private int _childrenWeight;

	private SourceCategory.Row _category;

	public SourceMaterial.Row _material;

	private static Color _randColor;

	public Emo lastEmo;

	private LightData _LightData;

	private Sprite _paintSprite;

	public int sortVal;

	public Card parentCard => parent as Card;

	public Thing parentThing => parent as Thing;

	public int colorInt
	{
		get
		{
			if (_colorInt == 0)
			{
				RefreshColor();
			}
			return _colorInt;
		}
	}

	public bool IsHotItem => invY == 1;

	public int uid
	{
		get
		{
			return _ints[1];
		}
		set
		{
			_ints[1] = value;
		}
	}

	public int idMaterial
	{
		get
		{
			return _ints[4];
		}
		set
		{
			_ints[4] = value;
		}
	}

	public int dir
	{
		get
		{
			return _ints[5];
		}
		set
		{
			_ints[5] = value;
		}
	}

	public int Num
	{
		get
		{
			return _ints[6];
		}
		set
		{
			_ints[6] = value;
		}
	}

	public int _x
	{
		get
		{
			return _ints[7];
		}
		set
		{
			_ints[7] = value;
		}
	}

	public int _z
	{
		get
		{
			return _ints[9];
		}
		set
		{
			_ints[9] = value;
		}
	}

	public int refVal
	{
		get
		{
			return _ints[11];
		}
		set
		{
			_ints[11] = value;
		}
	}

	public int decay
	{
		get
		{
			return _ints[12];
		}
		set
		{
			_ints[12] = value;
		}
	}

	public int altitude
	{
		get
		{
			return _ints[13];
		}
		set
		{
			_ints[13] = value;
		}
	}

	public int hp
	{
		get
		{
			return _ints[14];
		}
		set
		{
			_ints[14] = value;
		}
	}

	public float fx
	{
		get
		{
			return 0.001f * (float)_ints[15];
		}
		set
		{
			_ints[15] = (int)(value * 1000f);
		}
	}

	public float fy
	{
		get
		{
			return 0.001f * (float)_ints[16];
		}
		set
		{
			_ints[16] = (int)(value * 1000f);
		}
	}

	public BlessedState blessedState
	{
		get
		{
			return _ints[17].ToEnum<BlessedState>();
		}
		set
		{
			_ints[17] = (int)value;
		}
	}

	public PlaceState _placeState
	{
		get
		{
			return _ints[18].ToEnum<PlaceState>();
		}
		set
		{
			_ints[18] = (int)value;
		}
	}

	public int rarityLv
	{
		get
		{
			return _ints[19];
		}
		set
		{
			_ints[19] = value;
		}
	}

	public Rarity rarity
	{
		get
		{
			return (_ints[19] / 100).ToEnum<Rarity>();
		}
		set
		{
			_ints[19] = (int)value * 100;
		}
	}

	public int encLV
	{
		get
		{
			return _ints[20];
		}
		set
		{
			_ints[20] = value;
		}
	}

	public int posInvX
	{
		get
		{
			return _ints[21];
		}
		set
		{
			_ints[21] = value;
		}
	}

	public int posInvY
	{
		get
		{
			return _ints[22];
		}
		set
		{
			_ints[22] = value;
		}
	}

	public int idSkin
	{
		get
		{
			return _ints[23];
		}
		set
		{
			_ints[23] = value;
		}
	}

	public int feat
	{
		get
		{
			return _ints[24];
		}
		set
		{
			_ints[24] = value;
		}
	}

	public int LV
	{
		get
		{
			return _ints[25];
		}
		set
		{
			_ints[25] = value;
		}
	}

	public int exp
	{
		get
		{
			return _ints[26];
		}
		set
		{
			_ints[26] = value;
		}
	}

	public int tier
	{
		get
		{
			return _ints[27];
		}
		set
		{
			_ints[27] = value;
		}
	}

	public int version
	{
		get
		{
			return _ints[29];
		}
		set
		{
			_ints[29] = value;
		}
	}

	public bool isCensored
	{
		get
		{
			return _bits1[1];
		}
		set
		{
			_bits1[1] = value;
		}
	}

	public bool isDeconstructing
	{
		get
		{
			return _bits1[2];
		}
		set
		{
			_bits1[2] = value;
		}
	}

	public bool isDyed
	{
		get
		{
			return _bits1[3];
		}
		set
		{
			_bits1[3] = value;
		}
	}

	public bool isModified
	{
		get
		{
			return _bits1[4];
		}
		set
		{
			_bits1[4] = value;
		}
	}

	public bool isNew
	{
		get
		{
			return _bits1[5];
		}
		set
		{
			_bits1[5] = value;
		}
	}

	public bool isPlayerCreation
	{
		get
		{
			return _bits1[6];
		}
		set
		{
			_bits1[6] = value;
		}
	}

	public bool ignoreAutoPick
	{
		get
		{
			return _bits1[7];
		}
		set
		{
			_bits1[7] = value;
		}
	}

	public bool freePos
	{
		get
		{
			return _bits1[8];
		}
		set
		{
			_bits1[8] = value;
		}
	}

	public bool isHidden
	{
		get
		{
			return _bits1[9];
		}
		set
		{
			_bits1[9] = value;
		}
	}

	public bool isOn
	{
		get
		{
			return _bits1[10];
		}
		set
		{
			_bits1[10] = value;
		}
	}

	public bool isNPCProperty
	{
		get
		{
			return _bits1[11];
		}
		set
		{
			_bits1[11] = value;
		}
	}

	public bool isRestrained
	{
		get
		{
			return _bits1[12];
		}
		set
		{
			_bits1[12] = value;
		}
	}

	public bool isRoofItem
	{
		get
		{
			return _bits1[13];
		}
		set
		{
			_bits1[13] = value;
		}
	}

	public bool isMasked
	{
		get
		{
			return _bits1[14];
		}
		set
		{
			_bits1[14] = value;
		}
	}

	public bool disableAutoToggle
	{
		get
		{
			return _bits1[15];
		}
		set
		{
			_bits1[15] = value;
		}
	}

	public bool isImported
	{
		get
		{
			return _bits1[16];
		}
		set
		{
			_bits1[16] = value;
		}
	}

	public bool autoRefuel
	{
		get
		{
			return _bits1[17];
		}
		set
		{
			_bits1[17] = value;
		}
	}

	public bool ignoreStackHeight
	{
		get
		{
			return _bits1[18];
		}
		set
		{
			_bits1[18] = value;
		}
	}

	public bool isFloating
	{
		get
		{
			return _bits1[19];
		}
		set
		{
			_bits1[19] = value;
		}
	}

	public bool isWeightChanged
	{
		get
		{
			return _bits1[20];
		}
		set
		{
			_bits1[20] = value;
		}
	}

	public bool isFireproof
	{
		get
		{
			return _bits1[21];
		}
		set
		{
			_bits1[21] = value;
		}
	}

	public bool isAcidproof
	{
		get
		{
			return _bits1[22];
		}
		set
		{
			_bits1[22] = value;
		}
	}

	public bool isReplica
	{
		get
		{
			return _bits1[23];
		}
		set
		{
			_bits1[23] = value;
		}
	}

	public bool isSummon
	{
		get
		{
			return _bits1[24];
		}
		set
		{
			_bits1[24] = value;
		}
	}

	public bool isElemental
	{
		get
		{
			return _bits1[25];
		}
		set
		{
			_bits1[25] = value;
		}
	}

	public bool isBroken
	{
		get
		{
			return _bits1[26];
		}
		set
		{
			_bits1[26] = value;
		}
	}

	public bool isSubsetCard
	{
		get
		{
			return _bits1[27];
		}
		set
		{
			_bits1[27] = value;
		}
	}

	public bool noSnow
	{
		get
		{
			return _bits1[28];
		}
		set
		{
			_bits1[28] = value;
		}
	}

	public bool noMove
	{
		get
		{
			return _bits1[29];
		}
		set
		{
			_bits1[29] = value;
		}
	}

	public bool isGifted
	{
		get
		{
			return _bits1[30];
		}
		set
		{
			_bits1[30] = value;
		}
	}

	public bool isCrafted
	{
		get
		{
			return _bits1[31];
		}
		set
		{
			_bits1[31] = value;
		}
	}

	public bool isLostProperty
	{
		get
		{
			return _bits2[0];
		}
		set
		{
			_bits2[0] = value;
		}
	}

	public bool noShadow
	{
		get
		{
			return _bits2[1];
		}
		set
		{
			_bits2[1] = value;
		}
	}

	public bool noSell
	{
		get
		{
			return _bits2[2];
		}
		set
		{
			_bits2[2] = value;
		}
	}

	public bool isLeashed
	{
		get
		{
			return _bits2[3];
		}
		set
		{
			_bits2[3] = value;
		}
	}

	public bool isStolen
	{
		get
		{
			return _bits2[4];
		}
		set
		{
			_bits2[4] = value;
		}
	}

	public bool isSale
	{
		get
		{
			return _bits2[5];
		}
		set
		{
			_bits2[5] = value;
		}
	}

	public bool isCopy
	{
		get
		{
			return _bits2[6];
		}
		set
		{
			_bits2[6] = value;
		}
	}

	public bool isRestocking
	{
		get
		{
			return _bits2[7];
		}
		set
		{
			_bits2[7] = value;
		}
	}

	public bool hasSpawned
	{
		get
		{
			return _bits2[8];
		}
		set
		{
			_bits2[8] = value;
		}
	}

	public bool isBackerContent => c_idBacker != 0;

	public SourceBacker.Row sourceBacker
	{
		get
		{
			if (!isBackerContent)
			{
				return null;
			}
			return EClass.sources.backers.map.TryGetValue(c_idBacker);
		}
	}

	public BedType c_bedType
	{
		get
		{
			return GetInt(9).ToEnum<BedType>();
		}
		set
		{
			SetInt(9, (int)value);
		}
	}

	public int c_equippedSlot
	{
		get
		{
			return GetInt(6);
		}
		set
		{
			SetInt(6, value);
		}
	}

	public int c_lockLv
	{
		get
		{
			return GetInt(50);
		}
		set
		{
			SetInt(50, value);
		}
	}

	public Hostility c_originalHostility
	{
		get
		{
			return GetInt(12).ToEnum<Hostility>();
		}
		set
		{
			SetInt(12, (int)value);
		}
	}

	public MinionType c_minionType
	{
		get
		{
			return GetInt(61).ToEnum<MinionType>();
		}
		set
		{
			SetInt(61, (int)value);
		}
	}

	public int c_vomit
	{
		get
		{
			return GetInt(13);
		}
		set
		{
			SetInt(13, value);
		}
	}

	public bool c_wasInPcParty
	{
		get
		{
			return GetInt(103) != 0;
		}
		set
		{
			SetInt(103, value ? 1 : 0);
		}
	}

	public bool c_isImportant
	{
		get
		{
			return GetInt(109) != 0;
		}
		set
		{
			SetInt(109, value ? 1 : 0);
		}
	}

	public bool c_lockedHard
	{
		get
		{
			return GetInt(63) != 0;
		}
		set
		{
			SetInt(63, value ? 1 : 0);
		}
	}

	public bool c_revealLock
	{
		get
		{
			return GetInt(64) != 0;
		}
		set
		{
			SetInt(64, value ? 1 : 0);
		}
	}

	public bool c_isTrained
	{
		get
		{
			return GetInt(120) != 0;
		}
		set
		{
			SetInt(120, value ? 1 : 0);
		}
	}

	public bool c_isPrayed
	{
		get
		{
			return GetInt(121) != 0;
		}
		set
		{
			SetInt(121, value ? 1 : 0);
		}
	}

	public bool c_isDisableStockUse
	{
		get
		{
			return GetInt(122) != 0;
		}
		set
		{
			SetInt(122, value ? 1 : 0);
		}
	}

	public int c_lightColor
	{
		get
		{
			return GetInt(5);
		}
		set
		{
			Mod();
			SetInt(5, value);
		}
	}

	public Color LightColor => new Color((float)(c_lightColor / 1024 * 8) / 256f, (float)(c_lightColor % 1024 / 32 * 8) / 256f, (float)(c_lightColor % 32 * 8) / 256f, 1f);

	public int c_uidZone
	{
		get
		{
			return GetInt(10);
		}
		set
		{
			Mod();
			SetInt(10, value);
		}
	}

	public int c_uidRefCard
	{
		get
		{
			return GetInt(11);
		}
		set
		{
			Mod();
			SetInt(11, value);
		}
	}

	public int c_priceFix
	{
		get
		{
			return GetInt(21);
		}
		set
		{
			SetInt(21, value);
		}
	}

	public int c_priceAdd
	{
		get
		{
			return GetInt(29);
		}
		set
		{
			SetInt(29, value);
		}
	}

	public int c_priceCopy
	{
		get
		{
			return GetInt(124);
		}
		set
		{
			SetInt(124, value);
		}
	}

	public int c_fixedValue
	{
		get
		{
			return GetInt(131);
		}
		set
		{
			SetInt(131, value);
		}
	}

	public int c_dyeMat
	{
		get
		{
			return GetInt(3);
		}
		set
		{
			SetInt(3, value);
		}
	}

	public VisitorState visitorState
	{
		get
		{
			return GetInt(4).ToEnum<VisitorState>();
		}
		set
		{
			SetInt(4, (int)value);
		}
	}

	public RescueState c_rescueState
	{
		get
		{
			return GetInt(53).ToEnum<RescueState>();
		}
		set
		{
			SetInt(53, (int)value);
		}
	}

	public BossType c_bossType
	{
		get
		{
			return GetInt(65).ToEnum<BossType>();
		}
		set
		{
			SetInt(65, (int)value);
		}
	}

	public int c_dateStockExpire
	{
		get
		{
			return GetInt(22);
		}
		set
		{
			SetInt(22, value);
		}
	}

	public int c_dateDeathLock
	{
		get
		{
			return GetInt(130);
		}
		set
		{
			SetInt(130, value);
		}
	}

	public int c_IDTState
	{
		get
		{
			return GetInt(2);
		}
		set
		{
			SetInt(2, value);
		}
	}

	public int c_charges
	{
		get
		{
			return GetInt(7);
		}
		set
		{
			SetInt(7, value);
		}
	}

	public int c_bill
	{
		get
		{
			return GetInt(23);
		}
		set
		{
			SetInt(23, value);
		}
	}

	public int c_invest
	{
		get
		{
			return GetInt(28);
		}
		set
		{
			SetInt(28, value);
		}
	}

	public int c_seed
	{
		get
		{
			return GetInt(33);
		}
		set
		{
			SetInt(33, value);
		}
	}

	public int c_allowance
	{
		get
		{
			return GetInt(114);
		}
		set
		{
			SetInt(114, value);
		}
	}

	public int c_fur
	{
		get
		{
			return GetInt(62);
		}
		set
		{
			SetInt(62, value);
		}
	}

	public int c_dateCooked
	{
		get
		{
			return GetInt(66);
		}
		set
		{
			SetInt(66, value);
		}
	}

	public int c_lockedAge
	{
		get
		{
			return GetInt(38);
		}
		set
		{
			SetInt(38, value);
		}
	}

	public int c_containerSize
	{
		get
		{
			return GetInt(8);
		}
		set
		{
			SetInt(8, value);
		}
	}

	public int c_weight
	{
		get
		{
			return GetInt(1);
		}
		set
		{
			SetInt(1, value);
		}
	}

	public int c_diceDim
	{
		get
		{
			return GetInt(14);
		}
		set
		{
			SetInt(14, value);
		}
	}

	public int c_indexContainerIcon
	{
		get
		{
			return GetInt(15);
		}
		set
		{
			SetInt(15, value);
		}
	}

	public int c_idMainElement
	{
		get
		{
			return GetInt(16);
		}
		set
		{
			SetInt(16, value);
		}
	}

	public int c_summonDuration
	{
		get
		{
			return GetInt(17);
		}
		set
		{
			SetInt(17, value);
		}
	}

	public int c_idBacker
	{
		get
		{
			return GetInt(52);
		}
		set
		{
			SetInt(52, value);
		}
	}

	public int c_uidMaster
	{
		get
		{
			return GetInt(54);
		}
		set
		{
			SetInt(54, value);
		}
	}

	public int c_ammo
	{
		get
		{
			return GetInt(27);
		}
		set
		{
			SetInt(27, value);
		}
	}

	public int c_daysWithGod
	{
		get
		{
			return GetInt(57);
		}
		set
		{
			SetInt(57, value);
		}
	}

	public int c_daysWithPC
	{
		get
		{
			return GetInt(67);
		}
		set
		{
			SetInt(67, value);
		}
	}

	public string c_idPortrait
	{
		get
		{
			return GetStr(9);
		}
		set
		{
			SetStr(9, value);
		}
	}

	public string c_idRace
	{
		get
		{
			return GetStr(3);
		}
		set
		{
			SetStr(3, value);
		}
	}

	public string c_idJob
	{
		get
		{
			return GetStr(4);
		}
		set
		{
			SetStr(4, value);
		}
	}

	public string c_idTone
	{
		get
		{
			return GetStr(22);
		}
		set
		{
			SetStr(22, value);
		}
	}

	public string c_color
	{
		get
		{
			return GetStr(8);
		}
		set
		{
			SetStr(8, value);
		}
	}

	public string c_idSpriteReplacer
	{
		get
		{
			return GetStr(13);
		}
		set
		{
			SetStr(13, value);
		}
	}

	public string c_idTalk
	{
		get
		{
			return GetStr(21);
		}
		set
		{
			SetStr(21, value);
		}
	}

	public string c_idDeity
	{
		get
		{
			return GetStr(26);
		}
		set
		{
			SetStr(26, value);
		}
	}

	public string c_altName
	{
		get
		{
			return GetStr(1);
		}
		set
		{
			SetStr(1, value);
		}
	}

	public string c_altName2
	{
		get
		{
			return GetStr(2);
		}
		set
		{
			SetStr(2, value);
		}
	}

	public string c_extraNameRef
	{
		get
		{
			return GetStr(12);
		}
		set
		{
			SetStr(12, value);
		}
	}

	public string c_refText
	{
		get
		{
			return GetStr(10);
		}
		set
		{
			SetStr(10, value);
		}
	}

	public string c_idRefName
	{
		get
		{
			return GetStr(7);
		}
		set
		{
			SetStr(7, value);
		}
	}

	public string c_idRidePCC
	{
		get
		{
			return GetStr(55);
		}
		set
		{
			SetStr(55, value);
		}
	}

	public string c_idAbility
	{
		get
		{
			return GetStr(50);
		}
		set
		{
			SetStr(50, value);
		}
	}

	public string c_context
	{
		get
		{
			return GetStr(20);
		}
		set
		{
			SetStr(20, value);
		}
	}

	public string c_idEditor
	{
		get
		{
			return GetStr(27);
		}
		set
		{
			SetStr(27, value);
		}
	}

	public string c_editorTags
	{
		get
		{
			return GetStr(28);
		}
		set
		{
			SetStr(28, value);
		}
	}

	public string c_editorTraitVal
	{
		get
		{
			return GetStr(25);
		}
		set
		{
			SetStr(25, value);
		}
	}

	public string c_idTrait
	{
		get
		{
			return GetStr(29);
		}
		set
		{
			SetStr(29, value);
		}
	}

	public string c_idRefCard
	{
		get
		{
			return GetStr(5);
		}
		set
		{
			SetStr(5, value);
		}
	}

	public string c_idRefCard2
	{
		get
		{
			return GetStr(6);
		}
		set
		{
			SetStr(6, value);
		}
	}

	public string c_note
	{
		get
		{
			return GetStr(51);
		}
		set
		{
			SetStr(51, value);
		}
	}

	public UniqueData c_uniqueData
	{
		get
		{
			return GetObj<UniqueData>(6);
		}
		set
		{
			SetObj(6, value);
		}
	}

	public Thing ammoData
	{
		get
		{
			return GetObj<Thing>(9);
		}
		set
		{
			SetObj(9, value);
		}
	}

	public List<SocketData> socketList
	{
		get
		{
			return GetObj<List<SocketData>>(17);
		}
		set
		{
			SetObj(17, value);
		}
	}

	public Thing c_copyContainer
	{
		get
		{
			return GetObj<Thing>(10);
		}
		set
		{
			SetObj(10, value);
		}
	}

	public Window.SaveData c_windowSaveData
	{
		get
		{
			return GetObj<Window.SaveData>(2);
		}
		set
		{
			SetObj(2, value);
		}
	}

	public CharaUpgrade c_upgrades
	{
		get
		{
			return GetObj<CharaUpgrade>(11);
		}
		set
		{
			SetObj(11, value);
		}
	}

	public CharaGenes c_genes
	{
		get
		{
			return GetObj<CharaGenes>(15);
		}
		set
		{
			SetObj(15, value);
		}
	}

	public List<int> c_corruptionHistory
	{
		get
		{
			return GetObj<List<int>>(16);
		}
		set
		{
			SetObj(16, value);
		}
	}

	public ContainerUpgrade c_containerUpgrade
	{
		get
		{
			return GetObj<ContainerUpgrade>(12) ?? (c_containerUpgrade = new ContainerUpgrade());
		}
		set
		{
			SetObj(12, value);
		}
	}

	public DNA c_DNA
	{
		get
		{
			return GetObj<DNA>(14);
		}
		set
		{
			SetObj(14, value);
		}
	}

	public CharaList c_charaList
	{
		get
		{
			return GetObj<CharaList>(13);
		}
		set
		{
			SetObj(13, value);
		}
	}

	public MixedFoodData c_mixedFoodData
	{
		get
		{
			return GetObj<MixedFoodData>(18);
		}
		set
		{
			SetObj(18, value);
		}
	}

	public byte[] c_textureData
	{
		get
		{
			return GetObj<byte[]>(4);
		}
		set
		{
			SetObj(4, value);
		}
	}

	public SourceMaterial.Row DyeMat => EClass.sources.materials.rows[c_dyeMat];

	public int invX
	{
		get
		{
			return pos.x;
		}
		set
		{
			pos.x = value;
		}
	}

	public int invY
	{
		get
		{
			return pos.z;
		}
		set
		{
			pos.z = value;
		}
	}

	public CardRow refCard
	{
		get
		{
			object obj;
			if (!c_idRefCard.IsEmpty())
			{
				obj = EClass.sources.cards.map.TryGetValue(c_idRefCard);
				if (obj == null)
				{
					return EClass.sources.cards.map["ash3"];
				}
			}
			else
			{
				obj = null;
			}
			return (CardRow)obj;
		}
	}

	public CardRow refCard2
	{
		get
		{
			object obj;
			if (!c_idRefCard2.IsEmpty())
			{
				obj = EClass.sources.cards.map.TryGetValue(c_idRefCard2);
				if (obj == null)
				{
					return EClass.sources.cards.map["ash3"];
				}
			}
			else
			{
				obj = null;
			}
			return (CardRow)obj;
		}
	}

	public int ExpToNext => (50 + LV * 30) * (100 - Evalue(403)) / 100;

	public int DefaultLV => sourceCard.LV;

	public int ChildrenWeight
	{
		get
		{
			if (dirtyWeight)
			{
				_childrenWeight = 0;
				if (!(trait is TraitMagicChest))
				{
					foreach (Thing thing in things)
					{
						_childrenWeight += thing.ChildrenAndSelfWeight;
					}
					dirtyWeight = false;
					(parent as Card)?.SetDirtyWeight();
					if (isChara && IsPCFaction)
					{
						Chara.CalcBurden();
					}
					if (_childrenWeight < 0 || _childrenWeight > 10000000)
					{
						_childrenWeight = 10000000;
					}
				}
			}
			return _childrenWeight * Mathf.Max(100 - Evalue(404), 0) / 100;
		}
	}

	public int ChildrenAndSelfWeight => ChildrenWeight + SelfWeight * Num;

	public int ChildrenAndSelfWeightSingle => ChildrenWeight + SelfWeight;

	public virtual int SelfWeight => 1000;

	public virtual int WeightLimit => 500000;

	public SourceCategory.Row category => _category ?? (_category = EClass.sources.categories.map[sourceCard.category]);

	public SourceMaterial.Row material => _material ?? (_material = EClass.sources.materials.map.TryGetValue(idMaterial, 3));

	public virtual string AliasMaterialOnCreate => DefaultMaterial.alias;

	public Cell Cell => pos.cell;

	public virtual Thing Thing
	{
		get
		{
			if (!isThing)
			{
				return null;
			}
			return (Thing)this;
		}
	}

	public virtual Chara Chara
	{
		get
		{
			if (!isChara)
			{
				return null;
			}
			return (Chara)this;
		}
	}

	public virtual bool isThing => false;

	public virtual bool isChara => false;

	public bool ExistsOnMap => parent == EClass._zone;

	public virtual bool isSynced => renderer.isSynced;

	public bool IsContainer => trait.IsContainer;

	public bool IsUnique => rarity == Rarity.Artifact;

	public bool IsPowerful
	{
		get
		{
			if (rarity >= Rarity.Legendary || trait is TraitAdventurer)
			{
				return !IsPCFactionOrMinion;
			}
			return false;
		}
	}

	public bool IsImportant => sourceCard.HasTag(CTAG.important);

	public virtual SourcePref Pref => sourceCard.pref;

	public virtual bool IsDeadOrSleeping => false;

	public virtual bool IsDisabled => false;

	public virtual bool IsMoving => false;

	public virtual bool flipX
	{
		get
		{
			if (Tiles.Length == 1)
			{
				return dir % 2 == 1;
			}
			return false;
		}
	}

	public virtual bool IsAliveInCurrentZone => ExistsOnMap;

	public virtual string actorPrefab => "ThingActor";

	public virtual CardRow sourceCard => null;

	public virtual CardRow sourceRenderCard => sourceCard;

	public TileType TileType => sourceCard.tileType;

	public string Name => GetName(NameStyle.Full);

	public string NameSimple => GetName(NameStyle.Simple);

	public string NameOne => GetName(NameStyle.Full, 1);

	public virtual bool IsPC => false;

	public bool _IsPC => GetInt(56) != 0;

	public virtual bool IsPCC => false;

	public virtual bool IsPCParty => false;

	public virtual bool IsMinion => false;

	public virtual bool IsPCPartyMinion => false;

	public virtual bool IsPCFactionMinion => false;

	public virtual bool IsMultisize
	{
		get
		{
			if (sourceCard.multisize)
			{
				return IsInstalled;
			}
			return false;
		}
	}

	public bool IsToolbelt => category.slot == 44;

	public bool IsLightsource => category.slot == 45;

	public bool IsEquipment => category.slot != 0;

	public bool IsFood => category.IsChildOf("food");

	public bool IsInheritFoodTraits
	{
		get
		{
			if (!IsFood && !category.IsChildOf("seed") && !category.IsChildOf("drink") && !(id == "pasture"))
			{
				return id == "grass";
			}
			return true;
		}
	}

	public bool ShowFoodEnc
	{
		get
		{
			if (!IsInheritFoodTraits)
			{
				if (Evalue(10) > 0)
				{
					return !IsEquipmentOrRangedOrAmmo;
				}
				return false;
			}
			return true;
		}
	}

	public bool IsWeapon
	{
		get
		{
			if (!IsMeleeWeapon)
			{
				return IsRangedWeapon;
			}
			return true;
		}
	}

	public bool IsEquipmentOrRanged
	{
		get
		{
			if (category.slot == 0)
			{
				return IsRangedWeapon;
			}
			return true;
		}
	}

	public bool IsEquipmentOrRangedOrAmmo
	{
		get
		{
			if (category.slot == 0 && !IsRangedWeapon)
			{
				return IsAmmo;
			}
			return true;
		}
	}

	public bool IsMeleeWeapon => category.IsChildOf("melee");

	public bool IsRangedWeapon => trait is TraitToolRange;

	public bool IsThrownWeapon => sourceCard.HasTag(CTAG.throwWeapon);

	public bool IsAmmo => trait is TraitAmmo;

	public bool IsAgent => this == EClass.player.Agent;

	public bool IsFurniture => category.IsChildOf("furniture");

	public bool IsBlessed => blessedState >= BlessedState.Blessed;

	public bool IsCursed => blessedState <= BlessedState.Cursed;

	public bool IsRestrainedResident
	{
		get
		{
			if (isRestrained)
			{
				return IsPCFaction;
			}
			return false;
		}
	}

	public virtual bool IsPCFaction => false;

	public bool IsPCFactionOrMinion
	{
		get
		{
			if (!IsPCFaction)
			{
				return IsPCFactionMinion;
			}
			return true;
		}
	}

	public virtual bool IsGlobal => false;

	public virtual int MaxDecay => 1000;

	public bool IsDecayed => decay > MaxDecay;

	public bool IsRotting => decay >= MaxDecay / 4 * 3;

	public bool IsFresn => decay < MaxDecay / 4;

	public virtual int MaxHP => 100;

	public virtual int Power => Mathf.Max(20, EClass.curve(GetTotalQuality() * 10, 400, 100));

	public int FameLv
	{
		get
		{
			if (!IsPC)
			{
				return LV;
			}
			return EClass.player.fame / 100 + 1;
		}
	}

	public virtual int[] Tiles => sourceCard._tiles;

	public virtual int PrefIndex
	{
		get
		{
			if (Tiles.Length < 3)
			{
				return dir % 2;
			}
			return dir;
		}
	}

	public bool IsVariation => sourceCard.origin != null;

	public virtual int DV => elements.Value(64);

	public virtual int PV => elements.Value(65);

	public int HIT => elements.Value(66);

	public int DMG => elements.Value(67);

	public int STR => elements.Value(70);

	public int DEX => elements.Value(72);

	public int END => elements.Value(71);

	public int PER => elements.Value(73);

	public int LER => elements.Value(74);

	public int WIL => elements.Value(75);

	public int MAG => elements.Value(76);

	public int CHA => elements.Value(77);

	public int INT => elements.Value(80);

	public int LUC => elements.Value(78);

	public int W
	{
		get
		{
			if (dir % 2 != 0)
			{
				return sourceCard.H;
			}
			return sourceCard.W;
		}
	}

	public int H
	{
		get
		{
			if (dir % 2 != 0)
			{
				return sourceCard.W;
			}
			return sourceCard.H;
		}
	}

	public bool IsIdentified => c_IDTState == 0;

	public string TextRarity => Lang.GetList("quality")[Mathf.Clamp((int)(rarity + 1), 0, 5)];

	public bool IsInstalled => placeState == PlaceState.installed;

	public bool IsMale
	{
		get
		{
			if (bio != null)
			{
				return bio.gender == 2;
			}
			return false;
		}
	}

	public bool IsNegativeGift
	{
		get
		{
			if (!HasTag(CTAG.neg))
			{
				return blessedState <= BlessedState.Cursed;
			}
			return true;
		}
	}

	public bool HasContainerSize => c_containerSize != 0;

	public Thing Tool
	{
		get
		{
			if (!IsPC)
			{
				return null;
			}
			return EClass.player.currentHotItem.Thing;
		}
	}

	public virtual SourceMaterial.Row DefaultMaterial => sourceCard.DefaultMaterial;

	public virtual bool HasHost => false;

	public int Quality => Evalue(2);

	public int QualityLv => Quality / 10;

	public LightData LightData
	{
		get
		{
			if (_LightData == null)
			{
				return _LightData = ((!sourceCard.lightData.IsEmpty()) ? EClass.Colors.lightColors[sourceCard.lightData] : null);
			}
			return _LightData;
		}
	}

	public CardRenderer HostRenderer
	{
		get
		{
			if (!isChara || Chara.host == null)
			{
				return renderer;
			}
			return Chara.host.renderer;
		}
	}

	public bool ShouldShowMsg
	{
		get
		{
			if (IsPC || parent == EClass.pc || isSynced)
			{
				if (isChara)
				{
					return !Chara.isDead;
				}
				return true;
			}
			return false;
		}
	}

	public bool CanInspect
	{
		get
		{
			if (!isDestroyed)
			{
				return ExistsOnMap;
			}
			return false;
		}
	}

	public string InspectName => Name;

	public Point InspectPoint => pos;

	public Vector3 InspectPosition => renderer.position;

	public override string ToString()
	{
		return Name + "/" + pos?.ToString() + "/" + parent;
	}

	public bool CanReserve(AIAct act)
	{
		if (reservedAct != null && reservedAct != act)
		{
			return !reservedAct.IsRunning;
		}
		return true;
	}

	public bool TryReserve(AIAct act)
	{
		if (CanReserve(act))
		{
			reservedAct = act;
			return true;
		}
		return false;
	}

	public void Mod()
	{
		isModified = true;
	}

	public Window.SaveData GetWindowSaveData()
	{
		if (IsPC)
		{
			return Window.dictData.TryGetValue("LayerInventoryFloatMain0");
		}
		if (trait is TraitChestMerchant)
		{
			return Window.dictData.TryGetValue("ChestMerchant");
		}
		return c_windowSaveData;
	}

	public bool IsExcludeFromCraft(Recipe.Ingredient ing)
	{
		if ((IsUnique && ing.id != id && !ing.idOther.Contains(id)) || c_isImportant)
		{
			return true;
		}
		if (parent is Card card)
		{
			if (card.trait is TraitChestMerchant)
			{
				return true;
			}
			if (card.isSale || !card.trait.CanUseContent)
			{
				return true;
			}
			Window.SaveData windowSaveData = card.GetWindowSaveData();
			if (windowSaveData != null)
			{
				return windowSaveData.excludeCraft;
			}
		}
		return false;
	}

	public void SetDirtyWeight()
	{
		if (EClass.core.IsGameStarted && IsPC)
		{
			EClass.player.wasDirtyWeight = true;
		}
		dirtyWeight = true;
		(parent as Card)?.SetDirtyWeight();
	}

	public void ChangeWeight(int a)
	{
		c_weight = a;
		isWeightChanged = true;
		SetDirtyWeight();
	}

	public int Evalue(int ele)
	{
		return elements.Value(ele);
	}

	public int Evalue(int ele, bool ignoreGlobalElement)
	{
		if (!ignoreGlobalElement || !HasGlobalElement(ele))
		{
			return elements.Value(ele);
		}
		return 0;
	}

	public int EvalueMax(int ele, int min = 0)
	{
		return Mathf.Max(elements.Value(ele), min);
	}

	public int Evalue(string alias)
	{
		return elements.Value(EClass.sources.elements.alias[alias].id);
	}

	public bool HasTag(CTAG tag)
	{
		return sourceCard.tag.Contains(tag.ToString());
	}

	public bool HasEditorTag(EditorTag tag)
	{
		return c_editorTags?.Contains(tag.ToString()) ?? false;
	}

	public void AddEditorTag(EditorTag tag)
	{
		c_editorTags = (c_editorTags.IsEmpty() ? tag.ToString() : (c_editorTags + "," + tag));
	}

	public void RemoveEditorTag(EditorTag tag)
	{
		c_editorTags = (c_editorTags.IsEmpty() ? null : c_editorTags.Replace(tag.ToString(), "").Replace(",,", ","));
	}

	public virtual string GetName(NameStyle style, int num = -1)
	{
		return "Card";
	}

	public virtual string GetExtraName()
	{
		return "";
	}

	public virtual string GetDetail()
	{
		return sourceCard.GetText("detail", returnNull: true);
	}

	public int GetBestAttribute()
	{
		int num = 1;
		foreach (Element item in elements.dict.Values.Where((Element a) => Element.List_MainAttributesMajor.Contains(a.id)))
		{
			int num2 = item.Value;
			if (isChara && Chara.tempElements != null)
			{
				num2 -= Chara.tempElements.Value(item.id);
			}
			if (num2 > num)
			{
				num = num2;
			}
		}
		return num;
	}

	public void ModExp(string alias, int a)
	{
		ModExp(EClass.sources.elements.alias[alias].id, a);
	}

	public void ModExp(int ele, int a)
	{
		if (isChara)
		{
			elements.ModExp(ele, a);
		}
	}

	public bool IsChildOf(Card c)
	{
		return GetRootCard() == c;
	}

	public T FindTool<T>() where T : Trait
	{
		if (IsPC)
		{
			return Tool?.trait as T;
		}
		return things.Find<T>()?.trait as T;
	}

	[OnSerializing]
	private void _OnSerializing(StreamingContext context)
	{
		_x = pos.x;
		_z = pos.z;
		_ints[0] = _bits1.ToInt();
		_ints[2] = _bits2.ToInt();
		_placeState = placeState;
		version = 2;
		OnSerializing();
	}

	protected virtual void OnSerializing()
	{
	}

	[OnDeserialized]
	private void _OnDeserialized(StreamingContext context)
	{
		if (version < 2)
		{
			if (sockets != null)
			{
				for (int i = 0; i < sockets.Count; i++)
				{
					sockets[i] = sockets[i] / 100 * 1000 + sockets[i] % 100;
				}
			}
			version = 2;
		}
		_bits1.SetInt(_ints[0]);
		_bits2.SetInt(_ints[2]);
		placeState = _placeState;
		pos.Set(_x, _z);
		SetSource();
		things.SetOwner(this);
		elements.SetOwner(this, applyFeat: false);
		OnDeserialized();
		ApplyTrait();
		ApplyMaterialElements(remove: false);
		_CreateRenderer();
		foreach (Thing thing in things)
		{
			thing.parent = this;
		}
		if (isThing && Num <= 0)
		{
			isDestroyed = true;
		}
	}

	protected virtual void OnDeserialized()
	{
	}

	public string ReferenceId()
	{
		return "c" + uid;
	}

	public void Create(string _id, int _idMat = -1, int genLv = -1)
	{
		if (CardBlueprint.current != null)
		{
			bp = CardBlueprint.current;
			CardBlueprint.current = null;
		}
		else
		{
			bp = CardBlueprint._Default;
		}
		id = _id;
		Num = 1;
		autoRefuel = true;
		EClass.game.cards.AssignUID(this);
		isNew = true;
		SetSource();
		OnBeforeCreate();
		if (sourceCard.quality != 0)
		{
			rarity = sourceCard.quality.ToEnum<Rarity>();
		}
		else if (bp.rarity != Rarity.Random)
		{
			rarity = bp.rarity;
		}
		else if (category.slot != 0 && category.slot != 45 && category.slot != 44)
		{
			if (EClass.rnd(10) == 0)
			{
				rarity = Rarity.Crude;
			}
			else if (EClass.rnd(10) == 0)
			{
				rarity = Rarity.Superior;
			}
			else if (EClass.rnd(80) == 0)
			{
				rarity = Rarity.Legendary;
			}
			else if (EClass.rnd(250) == 0)
			{
				rarity = Rarity.Mythical;
			}
		}
		if (rarity != 0 && category.tag.Contains("fixedRarity"))
		{
			rarity = Rarity.Normal;
		}
		LV = DefaultLV;
		if (bp.lv != -999)
		{
			LV = bp.lv;
		}
		if (id == "microchip")
		{
			Debug.Log(id + "/" + _idMat + "/" + sourceCard.fixedMaterial);
		}
		if (sourceCard.fixedMaterial)
		{
			_material = EClass.sources.materials.alias[AliasMaterialOnCreate];
		}
		else
		{
			bool flag = (bp != null && bp.fixedMat) || sourceCard.quality == 4 || sourceCard.tierGroup.IsEmpty();
			if (_idMat != -1)
			{
				_material = EClass.sources.materials.rows[_idMat];
			}
			else if (!flag)
			{
				if (sourceCard.tierGroup == "wood")
				{
					Debug.Log(id);
				}
				_material = MATERIAL.GetRandomMaterial(genLv, sourceCard.tierGroup, bp.tryLevelMatTier);
			}
			else
			{
				_material = EClass.sources.materials.alias[AliasMaterialOnCreate];
			}
		}
		idMaterial = _material.id;
		things.SetOwner(this);
		elements.SetOwner(this, applyFeat: true);
		ApplyTrait();
		if (!bp.fixedQuality && trait.LevelAsQuality && (!EClass._zone.IsPCFaction || EClass.Branch.lv != 1) && EClass.debug.testThingQuality && EClass.rnd(2) == 0)
		{
			tier = Mathf.Clamp(EClass.rnd(5) + 1, 1, 3);
			LV = LV + tier * 10 + (LV - 1) * (125 + tier * 25) / 100;
		}
		ApplyMaterial();
		OnCreate(genLv);
		_CreateRenderer();
		trait.OnCreate(genLv);
		hp = MaxHP;
		if (HasTag(CTAG.hidden))
		{
			SetHidden();
		}
		isFloating = Pref.Float;
	}

	public virtual void OnBeforeCreate()
	{
	}

	public virtual void OnCreate(int genLv)
	{
	}

	public virtual void SetSource()
	{
	}

	public virtual void ApplyEditorTags(EditorTag tag)
	{
		switch (tag)
		{
		case EditorTag.Hidden:
			SetHidden();
			break;
		case EditorTag.Empty:
			RemoveThings();
			break;
		case EditorTag.IsOn:
			isOn = true;
			break;
		case EditorTag.IsOff:
			isOn = false;
			break;
		case EditorTag.NoSnow:
			noSnow = true;
			break;
		case EditorTag.Boss:
			EClass._zone.Boss = Chara;
			break;
		}
	}

	public void ApplyTrait()
	{
		string str = c_idTrait;
		if (str.IsEmpty())
		{
			if (sourceCard.trait.IsEmpty())
			{
				trait = (isChara ? new TraitChara() : new Trait());
			}
			else
			{
				trait = ClassCache.Create<Trait>("Trait" + sourceCard.trait[0], "Elin");
			}
		}
		else
		{
			trait = ClassCache.Create<Trait>(str, "Elin");
		}
		trait.SetOwner(this);
	}

	public Card SetLv(int a)
	{
		bool flag = a > LV;
		LV = a;
		if (!isChara)
		{
			return this;
		}
		Rand.SetSeed(uid);
		ElementContainer elementContainer = new ElementContainer();
		elementContainer.ApplyElementMap(uid, SourceValueType.Chara, Chara.job.elementMap, LV);
		elementContainer.ApplyElementMap(uid, SourceValueType.Chara, Chara.race.elementMap, LV);
		elementContainer.ApplyElementMap(uid, SourceValueType.Chara, Chara.source.elementMap, 1, invert: false, applyFeat: true);
		foreach (Element value in elements.dict.Values)
		{
			int num = elementContainer.Value(value.id);
			if (num != 0)
			{
				int num2 = num - value.ValueWithoutLink;
				if (num2 != 0)
				{
					elements.ModBase(value.id, num2);
				}
			}
		}
		if (flag)
		{
			ClampInitialSkill();
		}
		Rand.SetSeed();
		hp = MaxHP;
		Chara.mana.value = Chara.mana.max;
		Chara.CalculateMaxStamina();
		SetDirtyWeight();
		return this;
	}

	public void ClampInitialSkill()
	{
		if (elements.Base(286) > 50)
		{
			elements.SetTo(286, 50 + (int)Mathf.Sqrt(elements.Base(286) - 50));
		}
	}

	public void AddExp(int a)
	{
		a = a * GetExpMtp() / 100;
		exp += a;
		while (exp >= ExpToNext)
		{
			exp -= ExpToNext;
			LevelUp();
		}
	}

	public int GetExpMtp()
	{
		int num = 100;
		if (!IsPC)
		{
			num *= 2;
			if (IsPCFaction)
			{
				num = num * GetAffinityExpBonus() / 100;
				if (EClass.game.principal.petFeatExp)
				{
					num = num * (50 + EClass.game.principal.petFeatExpMtp * 50) / 100;
				}
			}
		}
		return num * (100 + Evalue(1237) * 30) / 100;
	}

	public int GetAffinityExpBonus()
	{
		return Mathf.Clamp(100 + Chara.affinity.value / 10, 50, 200);
	}

	public int GetDaysTogetherBonus()
	{
		if (!IsPCFactionOrMinion)
		{
			return 100;
		}
		return 100 + EClass.curve(c_daysWithPC / 100 * 3, 100, 20, 70);
	}

	public void LevelUp()
	{
		if (IsPC)
		{
			if (EClass.core.version.demo && EClass.player.totalFeat >= 5)
			{
				Msg.Say("demoLimit");
				return;
			}
			EClass.player.totalFeat++;
			Tutorial.Reserve("feat");
		}
		feat++;
		LV++;
		Say("dingExp", this);
		PlaySound("jingle_lvup");
		PlayEffect("aura_heaven");
		if (HasElement(1415) && Evalue(1415) < 9 && LV >= Evalue(1415) * 5 + 10)
		{
			Chara.SetFeat(1415, Evalue(1415) + 1, msg: true);
		}
		if (IsPC)
		{
			return;
		}
		if (Chara.race.id == "mutant")
		{
			int num = Mathf.Min(1 + LV / 5, 20);
			for (int i = 0; i < num; i++)
			{
				if (Evalue(1644) < i + 1)
				{
					Chara.SetFeat(1644, i + 1, msg: true);
				}
			}
		}
		Chara.TryUpgrade();
	}

	public virtual void ApplyMaterialElements(bool remove)
	{
	}

	public virtual void ApplyMaterial(bool remove = false)
	{
		_colorInt = 0;
	}

	public Card ChangeMaterial(int idNew, bool ignoreFixedMaterial = false)
	{
		return ChangeMaterial(EClass.sources.materials.map.TryGetValue(idNew, 1), ignoreFixedMaterial);
	}

	public Card ChangeMaterial(string idNew, bool ignoreFixedMaterial = false)
	{
		return ChangeMaterial(EClass.sources.materials.alias.TryGetValue(idNew, "oak"), ignoreFixedMaterial);
	}

	public Card ChangeMaterial(SourceMaterial.Row row, bool ignoreFixedMaterial = false)
	{
		if (sourceCard.fixedMaterial && !ignoreFixedMaterial)
		{
			return this;
		}
		ApplyMaterial(remove: true);
		_material = row;
		idMaterial = row.id;
		decay = 0;
		dirtyWeight = true;
		Card rootCard = GetRootCard();
		if (rootCard != null && rootCard.IsPC)
		{
			GetRootCard().SetDirtyWeight();
		}
		if (isThing)
		{
			LayerInventory.SetDirty(Thing);
		}
		ApplyMaterial();
		return this;
	}

	public void SetReplica(bool on)
	{
		isReplica = true;
		ChangeMaterial("granite");
	}

	public Thing Add(string id, int num = 1, int lv = 1)
	{
		if (num == 0)
		{
			num = 1;
		}
		return AddCard(ThingGen.Create(id, -1, lv).SetNum(num)) as Thing;
	}

	public Card AddCard(Card c)
	{
		return AddThing(c as Thing);
	}

	public void RemoveCard(Card c)
	{
		RemoveThing(c as Thing);
	}

	public void NotifyAddThing(Thing t, int num)
	{
	}

	public Thing AddThing(string id, int lv = -1)
	{
		return AddThing(ThingGen.Create(id, -1, (lv == -1) ? LV : lv));
	}

	public Thing AddThing(Thing t, bool tryStack = true, int destInvX = -1, int destInvY = -1)
	{
		if (t.Num == 0 || t.isDestroyed)
		{
			Debug.LogWarning("tried to add destroyed thing:" + t.Num + "/" + t.isDestroyed + "/" + t?.ToString() + "/" + this);
			return t;
		}
		if (t.parent == this)
		{
			Debug.LogWarning("already child:" + t);
			return t;
		}
		if (things.Contains(t))
		{
			Debug.Log("already in the list" + t);
			return t;
		}
		_ = t.parent;
		_ = EClass._zone;
		bool flag = IsPC && t.GetRootCard() == EClass.pc;
		if (t.parent != null)
		{
			t.parent.RemoveCard(t);
		}
		if (t.trait.ToggleType == ToggleType.Fire && destInvY == 0)
		{
			t.trait.Toggle(on: false);
		}
		t.isMasked = false;
		t.ignoreAutoPick = false;
		t.parent = this;
		t.noShadow = false;
		t.isSale = false;
		if (t.IsContainer)
		{
			t.RemoveEditorTag(EditorTag.PreciousContainer);
		}
		t.invX = -1;
		if (destInvY == -1)
		{
			t.invY = 0;
		}
		if (t.IsUnique && t.HasTag(CTAG.godArtifact) && t.GetRootCard() is Chara { IsPCFactionOrMinion: not false })
		{
			PurgeDuplicateArtifact(t);
		}
		Thing thing = (tryStack ? things.TryStack(t, destInvX, destInvY) : t);
		if (t == thing)
		{
			things.Add(t);
			things.OnAdd(t);
		}
		if (thing == t && IsPC && EClass.core.IsGameStarted && EClass._map != null && parent == EClass.game.activeZone && pos.IsValid && !flag)
		{
			NotifyAddThing(t, t.Num);
		}
		if (t == thing && isThing && parent == EClass._zone && placeState != 0)
		{
			EClass._map.Stocked.Add(t);
		}
		SetDirtyWeight();
		if (ShouldTrySetDirtyInventory())
		{
			EClass.pc.SetDirtyWeight();
			LayerInventory.SetDirty(thing);
		}
		if (IsPC)
		{
			goto IL_029f;
		}
		if (IsContainer)
		{
			Card rootCard = GetRootCard();
			if (rootCard != null && rootCard.IsPC)
			{
				goto IL_029f;
			}
		}
		goto IL_0345;
		IL_0345:
		return thing;
		IL_029f:
		t.isNPCProperty = false;
		t.isGifted = false;
		int count = 0;
		HashSet<string> ings = EClass.player.recipes.knownIngredients;
		TryAdd(t);
		if (t.CanSearchContents)
		{
			t.things.Foreach(delegate(Thing _t)
			{
				TryAdd(_t);
			});
		}
		if (count > 0 && EClass.core.IsGameStarted)
		{
			Msg.Say((count == 1) ? "newIng" : "newIngs", count.ToString() ?? "");
		}
		goto IL_0345;
		void TryAdd(Thing a)
		{
			if (!ings.Contains(a.id))
			{
				ings.Add(a.id);
				count++;
				if (a.sourceCard.origin != null && !ings.Contains(a.sourceCard.origin.id))
				{
					ings.Add(a.sourceCard.origin.id);
				}
			}
		}
	}

	public void PurgeDuplicateArtifact(Thing af)
	{
		List<Chara> list = new List<Chara>();
		foreach (FactionBranch child in EClass.pc.faction.GetChildren())
		{
			foreach (Chara member in child.members)
			{
				list.Add(member);
			}
		}
		foreach (Chara chara in EClass._map.charas)
		{
			list.Add(chara);
		}
		foreach (Chara item in list)
		{
			if (!item.IsPCFactionOrMinion)
			{
				continue;
			}
			List<Thing> list2 = item.things.List((Thing t) => t.id == af.id && t != af);
			if (list2.Count == 0)
			{
				continue;
			}
			foreach (Thing item2 in list2)
			{
				Msg.Say("destroyed_inv_", item2, item);
				item2.Destroy();
			}
		}
	}

	public void RemoveThings()
	{
		for (int num = things.Count - 1; num >= 0; num--)
		{
			RemoveThing(things[num]);
		}
	}

	public void RemoveThing(Thing thing)
	{
		Card rootCard = GetRootCard();
		if (rootCard != null && rootCard.isChara && (rootCard.Chara.held == thing || (rootCard.IsPC && thing.things.Find((Thing t) => EClass.pc.held == t) != null)))
		{
			rootCard.Chara.held = null;
			if (rootCard.IsPC)
			{
				WidgetCurrentTool instance = WidgetCurrentTool.Instance;
				if ((bool)instance && instance.selected != -1 && instance.selectedButton.card != null && instance.selectedButton.card == thing)
				{
					instance.selectedButton.card = null;
				}
				EClass.player.RefreshCurrentHotItem();
				ActionMode.AdvOrRegion.updatePlans = true;
				LayerInventory.SetDirty(thing);
			}
			RecalculateFOV();
		}
		dirtyWeight = true;
		if (thing.c_equippedSlot != 0 && isChara)
		{
			Chara.body.Unequip(thing);
		}
		things.Remove(thing);
		things.OnRemove(thing);
		if (isSale && things.Count == 0 && IsContainer)
		{
			isSale = false;
			EClass._map.props.sales.Remove(this);
		}
		if (thing.invY == 1)
		{
			WidgetCurrentTool.dirty = true;
		}
		thing.invX = -1;
		thing.invY = 0;
		if (thing.props != null)
		{
			thing.props.Remove(thing);
		}
		SetDirtyWeight();
		if (ShouldTrySetDirtyInventory())
		{
			LayerInventory.SetDirty(thing);
			WidgetHotbar.dirtyCurrentItem = true;
			thing.parent = null;
			if (thing.trait.IsContainer)
			{
				foreach (LayerInventory item in LayerInventory.listInv.Copy())
				{
					if (item.invs[0].owner.Container.GetRootCard() != EClass.pc && item.floatInv)
					{
						EClass.ui.layerFloat.RemoveLayer(item);
					}
				}
			}
		}
		thing.parent = null;
	}

	public bool ShouldTrySetDirtyInventory()
	{
		if (EClass.player.chara != null)
		{
			if (!IsPC && GetRootCard() != EClass.pc)
			{
				return EClass.ui.layers.Count > 0;
			}
			return true;
		}
		return false;
	}

	public virtual bool CanStackTo(Thing to)
	{
		return false;
	}

	public bool TryStackTo(Thing to)
	{
		if (isDestroyed || to.isDestroyed || !CanStackTo(to))
		{
			return false;
		}
		to.decay = (to.decay * to.Num + decay * Num) / (to.Num + Num);
		to.ModNum(Num);
		if (c_isImportant)
		{
			to.c_isImportant = true;
		}
		if (EClass.core.config.game.markStack && to.GetRootCard() == EClass.pc)
		{
			to.isNew = true;
		}
		Destroy();
		return true;
	}

	public ICardParent GetRoot()
	{
		if (parent == null)
		{
			return this;
		}
		return parent.GetRoot();
	}

	public Card GetRootCard()
	{
		if (!(parent is Card card))
		{
			return this;
		}
		return card.GetRootCard();
	}

	public bool IsStackable(Thing tg)
	{
		if (id != tg.id || material != tg.material)
		{
			return false;
		}
		return true;
	}

	public Thing Duplicate(int num)
	{
		Thing thing = ThingGen.Create(id);
		thing.ChangeMaterial(idMaterial, ignoreFixedMaterial: true);
		thing._bits1 = _bits1;
		thing._bits2 = _bits2;
		thing.dir = dir;
		thing.refVal = refVal;
		thing.altitude = altitude;
		thing.idSkin = idSkin;
		thing.blessedState = blessedState;
		thing.rarityLv = rarityLv;
		thing.tier = tier;
		thing.LV = LV;
		thing.exp = exp;
		thing.encLV = encLV;
		thing.decay = decay;
		thing.mapInt.Clear();
		thing.mapStr.Clear();
		foreach (KeyValuePair<int, int> item in mapInt)
		{
			thing.mapInt[item.Key] = item.Value;
		}
		foreach (KeyValuePair<int, string> item2 in mapStr)
		{
			thing.mapStr[item2.Key] = item2.Value;
		}
		elements.CopyTo(thing.elements);
		thing.SetNum(num);
		if (thing.IsRangedWeapon)
		{
			thing.sockets = IO.DeepCopy(sockets);
		}
		if (thing.c_containerSize != 0)
		{
			thing.things.SetOwner(thing);
		}
		return thing;
	}

	public Thing Split(int a)
	{
		if (a == Num)
		{
			return Thing;
		}
		Thing result = Duplicate(a);
		ModNum(-a, notify: false);
		return result;
	}

	public Thing SetNum(int a)
	{
		if (!isThing)
		{
			return null;
		}
		if (a == Num)
		{
			return this as Thing;
		}
		ModNum(a - Num);
		return this as Thing;
	}

	public Thing SetNoSell()
	{
		noSell = true;
		return this as Thing;
	}

	public void ModNum(int a, bool notify = true)
	{
		if (Num + a < 0)
		{
			a = -Num;
		}
		Num += a;
		if (props != null)
		{
			props.OnNumChange(this, a);
		}
		if (parent != null)
		{
			parent.OnChildNumChange(this);
		}
		if (a > 0 && EClass.core.IsGameStarted && GetRootCard() == EClass.pc && notify)
		{
			NotifyAddThing(Thing, a);
		}
		SetDirtyWeight();
		if (Num <= 0)
		{
			Destroy();
		}
	}

	public void AddSocket()
	{
		if (sockets == null)
		{
			sockets = new List<int>();
		}
		sockets.Add(0);
	}

	public void ApplySocket(Thing t)
	{
		if (t.trait is TraitMod traitMod && sockets != null)
		{
			ApplySocket(traitMod.source.id, traitMod.owner.encLV, traitMod.owner);
		}
	}

	public void ApplySocket(int id, int lv, Card mod = null)
	{
		for (int i = 0; i < sockets.Count; i++)
		{
			if (sockets[i] == 0)
			{
				if (lv >= 1000)
				{
					lv = 999;
				}
				sockets[i] = id * 1000 + lv;
				elements.ModBase(id, lv);
				mod?.Destroy();
				break;
			}
		}
	}

	public void EjectSockets()
	{
		for (int i = 0; i < sockets.Count; i++)
		{
			int num = sockets[i];
			if (num != 0)
			{
				Thing thing = ThingGen.Create(isCopy ? "ash3" : "mod_ranged");
				int ele = num / 1000;
				int num2 = num % 1000;
				elements.ModBase(ele, -num2);
				if (!isCopy)
				{
					thing.refVal = ele;
					thing.encLV = num2;
				}
				EClass._map.TrySmoothPick(pos.IsBlocked ? EClass.pc.pos : pos, thing, EClass.pc);
				sockets[i] = 0;
			}
		}
	}

	public SocketData AddRune(Card rune)
	{
		return AddRune(rune.refVal, rune.encLV, rune.trait is TraitRuneFree);
	}

	public SocketData AddRune(int idEle, int v, bool free)
	{
		if (socketList == null)
		{
			socketList = new List<SocketData>();
		}
		SourceElement.Row row = EClass.sources.elements.map[idEle];
		SocketData socketData = new SocketData
		{
			idEle = idEle,
			value = v,
			type = SocketData.Type.Rune,
			isFree = free
		};
		socketList.Add(socketData);
		if (IsWeapon || !row.IsWeaponEnc)
		{
			elements.SetTo(idEle, v);
		}
		return socketData;
	}

	public SocketData GetRuneEnc(int idEle)
	{
		if (socketList != null)
		{
			foreach (SocketData socket in socketList)
			{
				if (socket.type == SocketData.Type.Rune && socket.idEle == idEle)
				{
					return socket;
				}
			}
		}
		return null;
	}

	public int CountRune(bool countFree = true)
	{
		int num = 0;
		if (socketList != null)
		{
			foreach (SocketData socket in socketList)
			{
				if (socket.type == SocketData.Type.Rune && (countFree || !socket.isFree))
				{
					num++;
				}
			}
		}
		return num;
	}

	public int MaxRune()
	{
		return ((!IsUnique) ? 1 : 0) + Evalue(484);
	}

	public bool CanAddRune(TraitMod mod)
	{
		SourceElement.Row source = mod.source;
		if (category.slot == 0)
		{
			return false;
		}
		if (material.HasEnc(source.id))
		{
			return false;
		}
		if (!IsWeapon && source.IsWeaponEnc)
		{
			return false;
		}
		if (source.category == "resist")
		{
			foreach (Element item in elements.ListElements())
			{
				if (item.source.category == "resist" && (item.vBase != 0 || item.vSource != 0))
				{
					return false;
				}
			}
		}
		string encSlot = source.encSlot;
		if (encSlot == null || encSlot.Length != 0)
		{
			switch (encSlot)
			{
			case "shield":
				if (!category.IsChildOf("shield") && !category.IsChildOf("martial"))
				{
					return false;
				}
				break;
			default:
			{
				bool flag = false;
				string[] array = source.encSlot.Split(',');
				foreach (string key in array)
				{
					if (EClass.sources.elements.alias[key].id == category.slot)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
				break;
			}
			case "global":
			case "all":
			case "weapon":
				break;
			}
		}
		if (mod is TraitRuneFree)
		{
			return true;
		}
		return CountRune(countFree: false) < MaxRune();
	}

	public bool HasRune()
	{
		return CountRune() > 0;
	}

	public void OnChildNumChange(Card c)
	{
		if (ShouldTrySetDirtyInventory() && c.isThing)
		{
			LayerInventory.SetDirty(c.Thing);
			WidgetCurrentTool.RefreshCurrentHotItem();
		}
	}

	public Card Install()
	{
		SetPlaceState(PlaceState.installed);
		return this;
	}

	public void SetPlaceState(PlaceState newState, bool byPlayer = false)
	{
		if (this.placeState == newState)
		{
			return;
		}
		if (parent != EClass._zone)
		{
			Debug.Log("tried to change placestate of non-root card:" + this);
			return;
		}
		PlaceState placeState = this.placeState;
		Area area = pos.area;
		if (placeState == PlaceState.installed)
		{
			area?.OnUninstallCard(this);
			if (!isRoofItem)
			{
				altitude = 0;
				freePos = false;
				float num2 = (fy = 0f);
				fx = num2;
			}
			trait.Uninstall();
		}
		if (placeState == PlaceState.installed || newState == PlaceState.installed)
		{
			ForeachPoint(delegate(Point p, bool main)
			{
				p.cell.RemoveCard(this);
			});
			this.placeState = newState;
			ForeachPoint(delegate(Point p, bool main)
			{
				p.cell.AddCard(this);
			});
		}
		else
		{
			this.placeState = newState;
		}
		if (trait is TraitFakeBlock)
		{
			_CreateRenderer();
		}
		if (newState == PlaceState.none)
		{
			this.placeState = PlaceState.roaming;
			if (props != null)
			{
				props.Remove(this);
			}
		}
		else
		{
			EClass._map.props.OnCardAddedToZone(this);
			if (this.placeState == PlaceState.installed)
			{
				if (isThing)
				{
					if (trait.InstallBottomPriority != -1)
					{
						pos.detail.MoveThingToBottom(Thing);
					}
					else
					{
						pos.detail.MoveThingToTop(Thing);
					}
				}
				area?.OnInstallCard(this);
				isRoofItem = false;
				trait.Install(byPlayer);
			}
		}
		if (trait.ShouldRefreshTile)
		{
			pos.RefreshNeighborTiles();
		}
		if (trait.ShouldTryRefreshRoom && (placeState == PlaceState.installed || this.placeState == PlaceState.installed))
		{
			EClass._map.OnSetBlockOrDoor(pos.x, pos.z);
		}
		trait.OnChangePlaceState(newState);
		if (EClass._zone.IsPCFaction)
		{
			EClass.Branch.resources.SetDirty();
		}
	}

	public int GetTotalQuality(bool applyBonus = true)
	{
		int num = 5 + LV + material.quality;
		if (applyBonus)
		{
			num = num * (100 + Quality) / 100;
		}
		return num;
	}

	public void ModEncLv(int a)
	{
		ApplyMaterialElements(remove: true);
		encLV += a;
		ApplyMaterialElements(remove: false);
		if (IsEquipmentOrRangedOrAmmo)
		{
			if (IsWeapon || IsAmmo)
			{
				elements.ModBase(67, a);
			}
			else
			{
				elements.ModBase(65, a * 2);
			}
		}
	}

	public void SetEncLv(int a)
	{
		ModEncLv(a - encLV);
	}

	public void SetTier(int a, bool setTraits = true)
	{
		if (a < 0)
		{
			a = 0;
		}
		tier = a;
		if (setTraits)
		{
			int num = ((a == 1) ? 180 : ((a == 2) ? 400 : ((a >= 3) ? 600 : 0)));
			if (a > 0)
			{
				foreach (Element value in elements.dict.Values)
				{
					if (value.IsFoodTrait || value.IsTrait)
					{
						elements.SetTo(value.id, value.Value * num / 100);
					}
				}
			}
			elements.SetBase(2, a * 30);
			elements.SetBase(759, (a > 1) ? a : 0);
		}
		LayerInventory.SetDirty(Thing);
	}

	public virtual void SetBlessedState(BlessedState s)
	{
		int num = 0;
		if (s == BlessedState.Blessed && blessedState < BlessedState.Blessed)
		{
			num = 1;
		}
		if (s < BlessedState.Blessed && blessedState == BlessedState.Blessed)
		{
			num = -1;
		}
		if (num != 0 && IsEquipmentOrRangedOrAmmo)
		{
			if (IsWeapon || IsAmmo)
			{
				elements.ModBase(67, num);
			}
			else
			{
				elements.ModBase(65, num * 2);
			}
		}
		blessedState = s;
	}

	public virtual void ChangeRarity(Rarity q)
	{
		rarity = q;
	}

	public bool TryPay(int a, string id = "money")
	{
		if (GetCurrency(id) < a)
		{
			if (IsPC)
			{
				SE.Beep();
				Msg.Say((id == "ration") ? "notEnoughFood" : "notEnoughMoney");
			}
			return false;
		}
		if (IsPC && !(id == "ration"))
		{
			SE.Pay();
		}
		ModCurrency(-a, id);
		return true;
	}

	public void SetCharge(int a)
	{
		c_charges = a;
		LayerInventory.SetDirty(Thing);
	}

	public void ModCharge(int a, bool destroy = false)
	{
		c_charges += a;
		LayerInventory.SetDirty(Thing);
		if (c_charges <= 0 && destroy)
		{
			Say("spellbookCrumble", this);
			ModNum(-1);
		}
	}

	public void ModCurrency(int a, string id = "money")
	{
		if (a != 0)
		{
			if (id == "influence")
			{
				EClass._zone.ModInfluence(a);
				return;
			}
			SourceMaterial.Row mat = null;
			things.AddCurrency(this, id, a, mat);
		}
	}

	public int GetCurrency(string id = "money")
	{
		if (id == "influence")
		{
			return EClass._zone.influence;
		}
		int sum = 0;
		SourceMaterial.Row mat = null;
		things.GetCurrency(id, ref sum, mat);
		return sum;
	}

	public virtual void HealHPHost(int a, HealSource origin = HealSource.None)
	{
		if (isChara)
		{
			if (Chara.parasite != null)
			{
				Chara.parasite.HealHP(a);
			}
			if (Chara.ride != null)
			{
				Chara.ride.HealHP(a);
			}
		}
		HealHP(a, origin);
	}

	public virtual void HealHP(int a, HealSource origin = HealSource.None)
	{
		long num = a;
		if (origin == HealSource.Magic)
		{
			num = (long)a * (long)Mathf.Max(100 - Evalue(93), 1) / 100;
		}
		if (num > 100000000)
		{
			num = 100000000L;
		}
		hp += (int)num;
		if (hp > MaxHP)
		{
			hp = MaxHP;
		}
		switch (origin)
		{
		case HealSource.Magic:
		case HealSource.Item:
			PlaySound("heal");
			PlayEffect("heal");
			break;
		case HealSource.HOT:
			PlaySound("heal_tick");
			PlayEffect("heal_tick");
			break;
		}
	}

	public virtual int GetArmorSkill()
	{
		return 0;
	}

	public virtual int ApplyProtection(int dmg, int mod = 100)
	{
		int armorSkill = GetArmorSkill();
		Element orCreateElement = elements.GetOrCreateElement(armorSkill);
		int num = PV + orCreateElement.Value + DEX / 10;
		int num2 = 1;
		int sides = 1;
		int bonus = 0;
		if (num > 0)
		{
			int num3 = num / 4;
			num2 = num3 / 10 + 1;
			sides = num3 / num2 + 1;
			bonus = 0;
			dmg = dmg * 100 / Mathf.Max(100 + num, 1);
		}
		int num4 = Dice.Roll(num2, sides, bonus, this);
		dmg -= num4 * mod / 100;
		if (dmg < 0)
		{
			dmg = 0;
		}
		return dmg;
	}

	public void DamageHP(int dmg, AttackSource attackSource = AttackSource.None, Card origin = null)
	{
		DamageHP(dmg, 0, 0, attackSource, origin);
	}

	public void DamageHP(int dmg, int ele, int eleP = 100, AttackSource attackSource = AttackSource.None, Card origin = null, bool showEffect = true, Thing weapon = null, Chara originalTarget = null)
	{
		if (hp < 0)
		{
			return;
		}
		bool flag = originalTarget != null;
		if (isChara && !isRestrained && !HasElement(1241) && !flag)
		{
			AttackSource attackSource2 = attackSource;
			if ((uint)(attackSource2 - 3) > 2u && (uint)(attackSource2 - 13) > 4u)
			{
				foreach (Chara chara3 in EClass._map.charas)
				{
					if (chara3 != this && !chara3.IsHostile(Chara))
					{
						int num = chara3.Evalue(1241);
						int num2 = chara3.Evalue(438);
						if ((num != 0 || num2 != 0) && !chara3.IsDisabled && !chara3.isRestrained && (!IsPCFactionOrMinion || chara3.IsPCFactionOrMinion) && chara3.Dist(this) <= Mathf.Max(num, (num2 > 0) ? 1 : 0) && (num != 0 || num2 <= 0 || hp * 100 / MaxHP <= chara3.hp * 100 / chara3.MaxHP))
						{
							Say((num2 == 0) ? "wall_flesh" : "wall_knightly", chara3, this);
							chara3.DamageHP(dmg, ele, eleP, attackSource, origin, showEffect, weapon, Chara);
							return;
						}
					}
				}
			}
		}
		Element e;
		if (ele == 0 || ele == 926)
		{
			e = Element.Void;
			if (origin != null)
			{
				dmg = dmg * Mathf.Max(100 + origin.Evalue(93) / 2, 10) / 100;
			}
		}
		else
		{
			e = Element.Create(ele);
			if (attackSource != AttackSource.Condition && showEffect)
			{
				ActEffect.TryDelay(delegate
				{
					PlayEffect(e.id, useRenderPos: true, 0.25f);
					EClass.Sound.Play("atk_" + e.source.alias);
				});
			}
			if (!e.source.aliasRef.IsEmpty() && attackSource != AttackSource.ManaBackfire)
			{
				int num3 = ((origin != null) ? origin.Evalue(1238) : 0);
				if (attackSource == AttackSource.MagicSword)
				{
					num3 += 2;
				}
				dmg = Element.GetResistDamage(dmg, Evalue(e.source.aliasRef), num3);
				dmg = dmg * 100 / (100 + Mathf.Clamp(Evalue(961) * 5, -50, 200));
				dmg = dmg * Mathf.Max(100 - Evalue(93), 10) / 100;
			}
			switch (e.id)
			{
			case 910:
			{
				Chara chara2 = Chara;
				if (chara2 != null && chara2.isWet)
				{
					dmg /= 3;
				}
				break;
			}
			case 912:
			{
				Chara chara = Chara;
				if (chara != null && chara.isWet)
				{
					dmg = dmg * 150 / 100;
				}
				break;
			}
			}
		}
		if (origin != null && origin.isChara && origin.Chara.HasCondition<ConSupress>())
		{
			dmg = dmg * 2 / 3;
		}
		if (attackSource != AttackSource.Finish)
		{
			if (!IsPCFaction && LV > 50)
			{
				dmg = dmg * (100 - (int)Mathf.Min(80f, Mathf.Sqrt(LV - 50) * 2.5f)) / 100;
			}
			if (origin != null && origin.HasCondition<ConBerserk>())
			{
				dmg = dmg * 3 / 2;
			}
			if (EClass.game.principal.enableDamageReduction && IsPCFaction)
			{
				int num4 = ((origin != null) ? origin.LV : EClass._zone.DangerLv);
				if (num4 > 50)
				{
					dmg = dmg * (100 - (int)Mathf.Min(95f, Mathf.Sqrt(num4 - 50))) / 100;
				}
			}
			if (attackSource == AttackSource.Throw || attackSource == AttackSource.Range)
			{
				dmg = dmg * 100 / (100 + Evalue(435) * 2);
			}
			dmg = dmg * Mathf.Max(0, 100 - Mathf.Min(Evalue((e == Element.Void || e.id == 926) ? 55 : 56), 100) / ((!flag) ? 1 : 2)) / 100;
			if (origin != null && origin.IsPC && EClass.player.codex.Has(id))
			{
				dmg = dmg * (100 + Mathf.Min(10, EClass.player.codex.GetOrCreate(id).weakspot)) / 100;
			}
			if (isChara && Chara.body.GetAttackStyle() == AttackStyle.Shield && elements.ValueWithoutLink(123) >= 5 && (e == Element.Void || e.id == 926))
			{
				dmg = dmg * 90 / 100;
			}
			if (Evalue(971) > 0)
			{
				dmg = dmg * 100 / Mathf.Clamp(100 + Evalue(971), 25, 1000);
			}
			if (HasElement(1305))
			{
				dmg = dmg * 90 / 100;
			}
			if (EClass.pc.HasElement(1207) && isChara)
			{
				int num5 = 0;
				int num6 = 0;
				foreach (Condition condition2 in Chara.conditions)
				{
					if (condition2.Type == ConditionType.Buff)
					{
						num5++;
					}
					else if (condition2.Type == ConditionType.Debuff)
					{
						num6++;
					}
				}
				if (IsPCParty)
				{
					dmg = dmg * 100 / Mathf.Min(100 + num5 * 5, 120);
				}
				else
				{
					dmg = dmg * Mathf.Min(100 + num6 * 5, 120) / 100;
				}
			}
			if (IsPCParty && EClass.pc.ai is GoalAutoCombat)
			{
				dmg = dmg * 100 / Mathf.Clamp(105 + EClass.pc.Evalue(135) / 10, 10, 110);
			}
			if (HasElement(1218) && attackSource != AttackSource.ManaBackfire && (hp > 0 || Evalue(1421) <= 0))
			{
				dmg = dmg * (1000 - Mathf.Min(Evalue(1218), 1000) / ((!flag) ? 1 : 2)) / 1000;
				if (dmg <= 0 && EClass.rnd(4) == 0)
				{
					dmg++;
				}
			}
			if (dmg >= MaxHP / 10 && Evalue(68) > 0)
			{
				int num7 = MaxHP / 10;
				int num8 = dmg - num7;
				num8 = num8 * 100 / (200 + Evalue(68) * 10);
				dmg = num7 + num8;
			}
		}
		if (origin != null && origin.IsPC && EClass.pc.Evalue(654) > 0)
		{
			dmg = 0;
		}
		if (dmg < 0)
		{
			dmg = 0;
		}
		int num9 = Mathf.Clamp(dmg * 6 / MaxHP, 0, 4) + ((dmg > 0) ? 1 : 0);
		int num10 = hp;
		if (Evalue(1421) > 0)
		{
			int num11 = 0;
			int num12 = dmg;
			if (hp > 0)
			{
				num12 = dmg - hp;
				hp -= dmg;
				num11 += dmg;
				if (hp < 0 && Chara.mana.value >= 0)
				{
					num11 += hp;
					hp = 0;
				}
			}
			if (hp <= 0)
			{
				if (Evalue(1421) >= 2)
				{
					num12 /= 2;
				}
				dmg = num12;
				if (Chara.mana.value > 0)
				{
					num12 -= Chara.mana.value;
					Chara.mana.value -= dmg;
					num11 += dmg;
				}
				if (Chara.mana.value <= 0)
				{
					hp -= num12;
					num11 += num12;
				}
			}
			dmg = num11;
		}
		else
		{
			hp -= dmg;
		}
		if (isSynced && dmg != 0)
		{
			float ratio = (float)dmg / (float)MaxHP;
			Card c = ((parent is Chara) ? (parent as Chara) : this);
			ActEffect.TryDelay(delegate
			{
				c.PlayEffect("blood").SetParticleColor(EClass.Colors.matColors[material.alias].main).Emit(20 + (int)(30f * ratio));
				if (EClass.core.config.test.showNumbers || isThing)
				{
					EClass.scene.damageTextRenderer.Add(this, c, dmg, e);
				}
			});
		}
		if (EClass.pc.ai is AI_PracticeDummy { IsRunning: not false } aI_PracticeDummy && aI_PracticeDummy.target == this && (origin == null || origin.IsPC))
		{
			aI_PracticeDummy.hit++;
			aI_PracticeDummy.totalDamage += dmg;
		}
		ZoneInstanceBout zoneInstanceBout = EClass._zone.instance as ZoneInstanceBout;
		bool flag2 = false;
		bool flag3 = false;
		if (hp < 0 && Religion.recentWrath == null)
		{
			if (isRestrained && IsPCFaction && EClass._zone.IsPCFaction && (!IsPC || (Chara.ai is AI_Torture && Chara.ai.IsRunning)))
			{
				EvadeDeath();
				if (Chara.stamina.value > 0 && (EClass.rnd(2) == 0 || !IsPC))
				{
					Chara.stamina.Mod(-1);
				}
			}
			else if (IsInstalled && pos.HasBlock && trait.IsDoor)
			{
				EvadeDeath();
			}
			else if (!trait.CanBeDestroyed)
			{
				EvadeDeath();
			}
			else if (HasEditorTag(EditorTag.Invulnerable) || (HasEditorTag(EditorTag.InvulnerableToMobs) && (origin == null || !origin.IsPCParty)))
			{
				EvadeDeath();
			}
			else if (isChara)
			{
				if (Chara.HasCondition<ConInvulnerable>())
				{
					EvadeDeath();
				}
				else if (IsPC && EClass.debug.godMode)
				{
					EvadeDeath();
				}
				else if (Chara.host != null || (weapon != null && weapon.HasElement(485)))
				{
					EvadeDeath();
					flag2 = true;
				}
				else
				{
					if (origin != null && origin != this && Evalue(436) > 0 && !HasCondition<ConFractured>())
					{
						int num13 = (HasElement(1218) ? MaxHP : (MaxHP / 2));
						if (num10 > num13)
						{
							EvadeDeath();
							flag3 = true;
							Chara.AddCondition<ConFractured>((int)Mathf.Max(10f, 30f - Mathf.Sqrt(Evalue(436))));
							hp = Mathf.Min(num13 * (int)Mathf.Sqrt(Evalue(436) * 2) / 100, MaxHP / 3);
							goto IL_0e7f;
						}
					}
					if (zoneInstanceBout != null && (bool)LayerDrama.Instance)
					{
						EvadeDeath();
					}
					else if (LayerDrama.IsActive() && IsPC)
					{
						EvadeDeath();
					}
					else
					{
						if (attackSource != AttackSource.Finish && IsPCParty && Chara.host == null)
						{
							if (EClass.pc.ai is GoalAutoCombat && !EClass.player.invlunerable && (EClass.pc.ai as GoalAutoCombat).listHealthy.Contains(Chara))
							{
								EClass.core.actionsNextFrame.Add(delegate
								{
									Msg.Say(IsPC ? "abort_damage" : "abort_damgeAlly");
								});
								EClass.player.invlunerable = true;
								EClass.player.TryAbortAutoCombat();
								EClass.pc.stamina.Mod(-EClass.pc.stamina.max / 5);
							}
							if (EClass.player.invlunerable)
							{
								EvadeDeath();
								goto IL_0e7f;
							}
						}
						if (IsPC && Evalue(1220) > 0 && Chara.stamina.value >= Chara.stamina.max / 2)
						{
							Chara.stamina.Mod(-Chara.stamina.max / 2);
							Chara.AddCondition<ConInvulnerable>();
							EvadeDeath();
						}
					}
				}
			}
		}
		goto IL_0e7f;
		IL_0e7f:
		if (trait.CanBeAttacked)
		{
			renderer.PlayAnime(AnimeID.HitObj);
			hp = MaxHP;
			if (trait.CanBeSmashedToDeath)
			{
				hp = -1;
			}
		}
		Chara target;
		if (hp < 0)
		{
			if ((attackSource == AttackSource.Melee || attackSource == AttackSource.Range) && origin != null && originalTarget == null && (origin.isSynced || IsPC))
			{
				string text = "";
				if (IsPC && Lang.setting.combatTextStyle == 0)
				{
					if (e != Element.Void && e != null)
					{
						text = "dead_" + e.source.alias;
					}
					if (text == "" || !LangGame.Has(text))
					{
						text = "dead_attack";
					}
					EClass.pc.Say(text, this, "");
				}
				else
				{
					if (e != Element.Void && e != null)
					{
						text = "kill_" + e.source.alias;
					}
					if (text == "" || !LangGame.Has(text))
					{
						text = "kill_attack";
					}
					(IsPC ? EClass.pc : origin).Say(text, origin, this);
				}
			}
			if (isChara && Religion.recentWrath == null)
			{
				if (HasElement(1410) && !Chara.HasCooldown(1410))
				{
					Chara.AddCooldown(1410);
					Say("reboot", this);
					PlaySound("reboot");
					Chara.Cure(CureType.Boss);
					hp = MaxHP / 3;
					Chara.AddCondition<ConInvulnerable>();
					return;
				}
				if (HasCondition<ConRebirth>())
				{
					Say("rebirth", this);
					hp = Mathf.Min(MaxHP * (int)(5f + Mathf.Sqrt(Chara.GetCondition<ConRebirth>().power)) / 100, MaxHP);
					Chara.AddCondition<ConInvulnerable>();
					Chara.RemoveCondition<ConRebirth>();
					PlayEffect("revive");
					PlaySound("revive");
					return;
				}
				foreach (Chara chara4 in EClass._map.charas)
				{
					if (Chara.IsFriendOrAbove(chara4) && chara4.HasElement(1408) && chara4.faith == EClass.game.religions.Healing && EClass.world.date.GetRawDay() != chara4.GetInt(58) && (!chara4.IsPCFaction || IsPCFaction))
					{
						Msg.alwaysVisible = true;
						Msg.Say("layhand", chara4, this);
						Msg.Say("pray_heal", this);
						hp = MaxHP;
						Chara.AddCondition<ConInvulnerable>();
						PlayEffect("revive");
						PlaySound("revive");
						chara4.SetInt(58, EClass.world.date.GetRawDay());
						return;
					}
				}
			}
			if (zoneInstanceBout != null)
			{
				target = EClass._map.FindChara(zoneInstanceBout.uidTarget);
				if (target != null)
				{
					EClass.pc.ai.Cancel();
					if (IsPC)
					{
						EClass.pc.hp = 0;
						Heal();
						EClass.player.ModFame(-10 - (int)((float)EClass.player.fame * 0.05f));
						target.ShowDialog("_chara", "bout_lose");
						return;
					}
					if (target == this)
					{
						hp = 0;
						Heal();
						target.ModAffinity(EClass.pc, 10);
						target.ShowDialog("_chara", "bout_win");
						return;
					}
				}
			}
			if (!isDestroyed)
			{
				if (attackSource == AttackSource.MoonSpear && isChara)
				{
					Rand.SetSeed(uid + EClass.world.date.day);
					ActEffect.LoveMiracle(Chara, origin?.Chara, 100, EffectId.MoonSpear);
					Rand.SetSeed();
				}
				Die(e, origin, attackSource, originalTarget);
				if (trait.CanBeSmashedToDeath && !EClass._zone.IsUserZone)
				{
					Rand.SetSeed(uid);
					if (EClass.rnd(3) == 0 && !isCrafted && !isCopy)
					{
						string text2 = new int[18]
						{
							233, 235, 236, 236, 236, 1170, 1143, 1144, 727, 728,
							237, 869, 1178, 1179, 1180, 1243, 1244, 1245
						}.RandomItem().ToString();
						if (EClass.rnd(10) == 0)
						{
							text2 = "casino_coin";
						}
						if (EClass.rnd(10) == 0)
						{
							text2 = "scratchcard";
						}
						if (EClass.rnd(3) == 0)
						{
							text2 = "money";
						}
						if (EClass.rnd(5) == 0)
						{
							text2 = "plat";
						}
						if (EClass.rnd(10) == 0)
						{
							text2 = "money2";
						}
						if (EClass.rnd(20) == 0 || EClass.debug.enable)
						{
							text2 = "medal";
						}
						EClass._zone.AddCard(ThingGen.Create(text2).SetNum((!(text2 == "money")) ? 1 : EClass.rndHalf(100)).SetHidden(hide: false), pos);
					}
					Rand.SetSeed();
				}
				ProcAbsorb();
				if (EClass.pc.Evalue(1355) > 0 && isChara && (IsPCFactionOrMinion || (origin != null && origin.IsPCParty)))
				{
					((EClass.pc.AddCondition<ConStrife>() as ConStrife) ?? EClass.pc.GetCondition<ConStrife>())?.AddKill(Chara);
				}
			}
			if (origin != null && origin.isChara)
			{
				if (origin.IsPCFactionOrMinion && isChara && !isCopy)
				{
					EClass.player.stats.kills++;
					EClass.game.quests.list.ForeachReverse(delegate(Quest q)
					{
						q.OnKillChara(Chara);
					});
					EClass.player.codex.AddKill(id);
					if (Guild.Fighter.CanGiveContribution(Chara))
					{
						Guild.Fighter.AddContribution(5 + LV / 5);
					}
					if (Guild.Fighter.HasBounty(Chara))
					{
						int a2 = EClass.rndHalf(200 + LV * 20);
						Msg.Say("bounty", Chara, a2.ToString() ?? "");
						EClass.pc.ModCurrency(a2);
						SE.Pay();
					}
				}
				if (origin.GetInt(106) == 0)
				{
					origin.Chara.TalkTopic("kill");
				}
			}
			Msg.SetColor();
		}
		else if ((attackSource == AttackSource.Melee || attackSource == AttackSource.Range) && origin != null && originalTarget == null)
		{
			(IsPC ? EClass.pc : origin).Say("dmgMelee" + num9 + (IsPC ? "pc" : ""), origin, this);
		}
		else if (isChara)
		{
			int num14 = ((attackSource != AttackSource.Condition && attackSource != AttackSource.WeaponEnchant) ? 1 : 2);
			if (num9 >= num14)
			{
				if (e != Element.Void)
				{
					Say("dmg_" + e.source.alias, this);
				}
				if (e == Element.Void || num9 >= 2)
				{
					Say("dmg" + num9, this);
				}
			}
		}
		if (flag3)
		{
			Say("guts", this);
		}
		if (isChara)
		{
			if (flag2)
			{
				if (!Chara.HasCondition<ConFaint>())
				{
					Chara.AddCondition<ConFaint>(200, force: true);
				}
				return;
			}
			if (origin != null && origin.IsAliveInCurrentZone && origin.isChara)
			{
				if (e.id == 916)
				{
					origin.HealHP(Mathf.Clamp(EClass.rnd(dmg * (50 + eleP) / 500 + 5), 1, origin.MaxHP / 5 + EClass.rnd(10)));
				}
				if (attackSource == AttackSource.Melee || attackSource == AttackSource.Range)
				{
					int num15 = origin.Dist(this);
					if (attackSource == AttackSource.Melee && HasElement(1221) && num15 <= Evalue(1221))
					{
						int ele2 = ((Chara.MainElement == Element.Void) ? 924 : Chara.MainElement.id);
						if (id == "hedgehog_ether")
						{
							ele2 = 922;
						}
						Say("reflect_thorne", this, origin);
						origin.DamageHP(Mathf.Clamp(dmg / 10, 1, MaxHP / (origin.IsPowerful ? 200 : 20)), ele2, Power, AttackSource.Condition, this);
					}
					if (HasElement(1223) && num15 <= Evalue(1223))
					{
						int ele3 = ((Chara.MainElement == Element.Void) ? 923 : Chara.MainElement.id);
						Say("reflect_acid", this, origin);
						origin.DamageHP(Mathf.Clamp(dmg / 10, 1, MaxHP / (origin.IsPowerful ? 200 : 20)), ele3, Power * 2, AttackSource.Condition, this);
					}
				}
				ProcAbsorb();
			}
		}
		if (hp < 0 || !isChara)
		{
			return;
		}
		if (dmg > 0)
		{
			int a3 = (int)(100L * (long)(dmg * 100 / MaxHP) / 100) + 1;
			a3 = Mathf.Min(a3, Chara.isRestrained ? 15 : 200);
			if (a3 > 0)
			{
				elements.ModExp(GetArmorSkill(), a3);
				if (Chara.body.GetAttackStyle() == AttackStyle.Shield)
				{
					elements.ModExp(123, a3);
				}
			}
		}
		int num16 = ((EClass.rnd(2) == 0) ? 1 : 0);
		if (attackSource == AttackSource.Condition)
		{
			num16 = 1 + EClass.rnd(2);
		}
		if (num16 > 0)
		{
			bool flag4 = Chara.HasCondition<ConPoison>() || ((e.id == 915 || e.id == 923) && ResistLv(Evalue(955)) < 4);
			AddBlood(num16, flag4 ? 6 : (-1));
		}
		bool flag5 = true;
		switch (e.id)
		{
		case 910:
			if (Chance(30 + eleP / 5, 100))
			{
				Chara.AddCondition<ConBurning>(eleP);
			}
			break;
		case 911:
			if (Chara.isWet)
			{
				if (Chance(30 + eleP / 10, 100))
				{
					Chara.AddCondition<ConFreeze>(eleP);
				}
			}
			else if (Chance(50 + eleP / 10, 100))
			{
				Chara.AddCondition<ConWet>(eleP);
			}
			break;
		case 912:
			if (Chance(75 + eleP / 20, 100) && EClass.rnd(3) == 0)
			{
				Chara.AddCondition<ConParalyze>(1, force: true);
			}
			break;
		case 915:
			if (Chance(30 + eleP / 5, 100))
			{
				Chara.AddCondition<ConPoison>(eleP);
			}
			break;
		case 913:
			if (Chance(30 + eleP / 5, 100))
			{
				Chara.AddCondition<ConBlind>(eleP);
			}
			break;
		case 918:
			flag5 = false;
			if (Chance(30 + eleP / 5, 100))
			{
				Chara.AddCondition<ConParalyze>(eleP);
			}
			break;
		case 914:
			flag5 = false;
			if (EClass.rnd(3) != 0)
			{
				if (Chance(30 + eleP / 5, 100))
				{
					Chara.AddCondition<ConConfuse>(eleP);
				}
			}
			else if (Chance(30 + eleP / 5, 100))
			{
				Chara.AddCondition<ConSleep>(eleP);
			}
			break;
		case 917:
			if (Chance(50 + eleP / 10, 100))
			{
				Chara.AddCondition<ConDim>(eleP);
			}
			break;
		case 925:
			if (EClass.rnd(3) == 0)
			{
				if (Chance(30 + eleP / 5, 100))
				{
					Chara.AddCondition<ConDim>(eleP);
				}
			}
			else if (EClass.rnd(2) == 0)
			{
				if (EClass.rnd(3) == 0)
				{
					Chara.AddCondition<ConParalyze>(1, force: true);
				}
			}
			else if (EClass.rnd(2) == 0)
			{
				Chara.AddCondition<ConConfuse>(1 + EClass.rnd(3), force: true);
			}
			break;
		case 920:
			flag5 = false;
			if (Chance(5 + eleP / 25, 40))
			{
				Chara.AddCondition<ConBlind>(eleP / 2);
			}
			if (Chance(5 + eleP / 25, 40))
			{
				Chara.AddCondition<ConParalyze>(eleP / 2);
			}
			if (Chance(5 + eleP / 25, 40))
			{
				Chara.AddCondition<ConConfuse>(eleP / 2);
			}
			if (Chance(5 + eleP / 25, 40))
			{
				Chara.AddCondition<ConPoison>(eleP / 2);
			}
			if (Chance(5 + eleP / 25, 40))
			{
				Chara.AddCondition<ConSleep>(eleP / 2);
			}
			if (Chance(5 + eleP / 25, 40))
			{
				Chara.AddCondition<ConDim>(eleP / 2);
			}
			if (Chance(30 + eleP / 10, 100))
			{
				Chara.SAN.Mod(EClass.rnd(2));
			}
			break;
		case 924:
			if (Chance(50 + eleP / 10, 100))
			{
				Chara.AddCondition<ConBleed>(eleP);
			}
			break;
		case 923:
			if (Chance(50 + eleP / 10, 100) && EClass.rnd(4) == 0)
			{
				ActEffect.Proc(EffectId.Acid, Chara);
			}
			break;
		case 922:
			Chara.ModCorruption(EClass.rnd(eleP / 50 + 10));
			break;
		}
		if (origin != null && origin.HasElement(1411) && !Chara.HasCondition<ConGravity>())
		{
			Condition.ignoreEffect = true;
			Chara.AddCondition<ConGravity>(2000);
			Condition.ignoreEffect = false;
		}
		if (Chara.conSleep != null && flag5)
		{
			Chara.conSleep.Kill();
		}
		if (IsPC)
		{
			float num17 = (float)hp / (float)MaxHP;
			if (Evalue(1421) > 0)
			{
				num17 = (float)Chara.mana.value / (float)Chara.mana.max;
			}
			if (num17 < 0.3f)
			{
				PlaySound("heartbeat", 1f - num17 * 2f);
			}
		}
		if (!IsPC && hp < MaxHP / 5 && Evalue(423) <= 0 && dmg * 100 / MaxHP + 10 > EClass.rnd(IsPowerful ? 400 : 150) && !HasCondition<ConFear>())
		{
			Chara.AddCondition<ConFear>(100 + EClass.rnd(100));
		}
		if (Chara.ai.Current.CancelWhenDamaged && attackSource != AttackSource.Hunger && attackSource != AttackSource.Fatigue)
		{
			Chara.ai.Current.TryCancel(origin);
		}
		if (Chara.HasCondition<ConWeapon>())
		{
			ConWeapon condition = Chara.GetCondition<ConWeapon>();
			if (e.source.aliasRef == condition.sourceElement.aliasRef)
			{
				condition.Mod(-1);
			}
		}
		if (Chara.HasElement(1222) && (dmg >= MaxHP / 10 || EClass.rnd(20) == 0))
		{
			ActEffect.Proc(EffectId.Duplicate, this);
		}
		if (origin != null)
		{
			AttackProcess.ProcShieldEncs(Chara, origin);
		}
		if (hp < MaxHP / 3 && HasElement(1409) && !Chara.HasCooldown(1409))
		{
			Chara.AddCooldown(1409);
			Chara.AddCondition<ConBoost>(Power);
			Chara.Cure(CureType.Boss);
			Chara.HealHP(MaxHP / 2);
		}
		if (origin != null && origin.isChara && attackSource != AttackSource.Finish)
		{
			if (!AI_PlayMusic.ignoreDamage)
			{
				Chara.TrySetEnemy(origin.Chara);
			}
			if ((weapon == null || !weapon.HasElement(486)) && origin.Evalue(428) > 0 && !IsPCFactionOrMinion && EClass.rnd(dmg) >= EClass.rnd(MaxHP / 10) + MaxHP / 100 + 1)
			{
				origin.Chara.TryNeckHunt(Chara, origin.Evalue(428) * 20, harvest: true);
			}
		}
		bool Chance(int a, int max)
		{
			if (dmg > 0 || (origin != null && origin.HasElement(1345)))
			{
				return Mathf.Min(a, max) > EClass.rnd(100);
			}
			return false;
		}
		void EvadeDeath()
		{
			hp = 0;
			if (Evalue(1421) > 0 && isChara && Chara.mana.value < 0)
			{
				Chara.mana.value = 0;
			}
		}
		void Heal()
		{
			target.Cure(CureType.Death);
			foreach (Chara member in EClass.pc.party.members)
			{
				member.Cure(CureType.Death);
			}
		}
		void ProcAbsorb()
		{
			if (origin != null && origin.isChara && isChara && (weapon == null || !weapon.HasElement(486)))
			{
				int valueOrDefault = (origin.Evalue(662) + weapon?.Evalue(662, ignoreGlobalElement: true)).GetValueOrDefault();
				int valueOrDefault2 = (origin.Evalue(661) + weapon?.Evalue(661, ignoreGlobalElement: true)).GetValueOrDefault();
				if (valueOrDefault > 0 && attackSource == AttackSource.Melee && origin.isChara && !origin.Chara.ignoreSPAbsorb && Chara.IsHostile(origin as Chara))
				{
					int num18 = EClass.rnd(3 + Mathf.Clamp(dmg / 100, 0, valueOrDefault / 10));
					origin.Chara.stamina.Mod(num18);
					if (IsAliveInCurrentZone)
					{
						Chara.stamina.Mod(-num18);
					}
				}
				if (origin.HasElement(1350) && attackSource == AttackSource.Melee)
				{
					int num19 = EClass.rndHalf(2 + Mathf.Clamp(dmg / 10, 0, origin.Chara.GetPietyValue() + 10));
					origin.Chara.mana.Mod(num19);
					if (IsAliveInCurrentZone)
					{
						Chara.mana.Mod(-num19);
					}
				}
				if (valueOrDefault2 > 0 && attackSource == AttackSource.Melee)
				{
					int num20 = EClass.rnd(2 + Mathf.Clamp(dmg / 10, 0, valueOrDefault2 + 10));
					origin.Chara.mana.Mod(num20);
					if (IsAliveInCurrentZone)
					{
						Chara.mana.Mod(-num20);
					}
				}
			}
		}
	}

	public virtual void Die(Element e = null, Card origin = null, AttackSource attackSource = AttackSource.None, Chara originalTarget = null)
	{
		Card rootCard = GetRootCard();
		Point _pos = rootCard?.pos ?? pos;
		if (_pos != null && !_pos.IsValid)
		{
			_pos = null;
		}
		if (trait.EffectDead == EffectDead.Default && _pos != null)
		{
			_pos.PlaySound(material.GetSoundDead(sourceCard));
			_pos.PlayEffect("mine").SetParticleColor(material.GetColor()).Emit(10 + EClass.rnd(10));
			material.AddBlood(_pos, trait.CanBeSmashedToDeath ? (12 + EClass.rnd(8)) : 6);
			if (_pos.IsSync)
			{
				string text = ((rootCard != this) ? "destroyed_inv_" : "destroyed_ground_");
				if (e != null && LangGame.Has(text + e.source.alias))
				{
					text += e.source.alias;
				}
				if (attackSource != AttackSource.Throw)
				{
					Msg.Say(text, this, rootCard);
				}
			}
			else if (attackSource != AttackSource.Throw)
			{
				Msg.Say("destroyed", this);
			}
		}
		if (_pos != null && !EClass._zone.IsUserZone)
		{
			things.ForeachReverse(delegate(Thing t)
			{
				if (!(t.trait is TraitChestMerchant))
				{
					EClass._zone.AddCard(t, _pos);
				}
			});
		}
		Destroy();
		if (e != null && _pos != null && e.id == 21)
		{
			EClass._zone.AddCard(ThingGen.Create((EClass.rnd(2) == 0) ? "ash" : "ash2"), _pos);
		}
		if (trait.ThrowType == ThrowType.Explosive)
		{
			Explode(pos, origin);
		}
	}

	public void Explode(Point p, Card origin)
	{
		ActEffect.ProcAt(EffectId.Explosive, 100, blessedState, this, null, p, isNeg: true, new ActRef
		{
			origin = origin?.Chara,
			refThing = Thing,
			aliasEle = "eleImpact"
		});
	}

	public void Deconstruct()
	{
		PlaySound(material.GetSoundDead(sourceCard));
		Destroy();
	}

	public void Destroy()
	{
		if (isDestroyed)
		{
			Debug.Log(Name + " is already destroyed.");
			return;
		}
		if (isChara)
		{
			if (IsPCFaction && !Chara.isSummon)
			{
				Debug.Log(this);
				return;
			}
			Chara.DropHeld();
			Chara.isDead = true;
			if (IsPCParty)
			{
				EClass.pc.party.RemoveMember(Chara);
			}
			if (IsGlobal)
			{
				EClass.game.cards.globalCharas.Remove(Chara);
			}
		}
		if (renderer.hasActor)
		{
			renderer.KillActor();
		}
		if (parent != null)
		{
			parent.RemoveCard(this);
		}
		for (int num = things.Count - 1; num >= 0; num--)
		{
			things[num].Destroy();
		}
		isDestroyed = true;
	}

	public void SpawnLoot(Card origin)
	{
		if (!isChara || IsPCFactionMinion || (isCopy && EClass.rnd(10) != 0))
		{
			return;
		}
		bool isUserZone = EClass._zone.IsUserZone;
		bool flag = EClass._zone is Zone_Music;
		List<Card> list = new List<Card>();
		if (!IsPCFaction && !isUserZone && sourceCard.idActor.IsEmpty())
		{
			int i2 = 500;
			if (this.rarity >= Rarity.Legendary)
			{
				i2 = ((!EClass.player.codex.DroppedCard(id)) ? 1 : 10);
				EClass.player.codex.MarkCardDrop(id);
			}
			if (trait is TraitAdventurerBacker)
			{
				i2 = 10;
			}
			if (chance(i2))
			{
				Thing thing = ThingGen.Create("figure");
				thing.MakeFigureFrom(id);
				list.Add(thing);
			}
			if (chance(i2))
			{
				Thing thing2 = ThingGen.Create("figure3");
				thing2.MakeFigureFrom(id);
				list.Add(thing2);
			}
		}
		bool flag2 = Chara.race.corpse[1].ToInt() > EClass.rnd(1500) || (Chara.IsPowerful && !IsPCFaction) || EClass.debug.godFood;
		int num = 1;
		if (!IsMinion && Chara.IsAnimal && EClass.rnd(EClass._zone.IsPCFaction ? 3 : 5) == 0)
		{
			flag2 = true;
		}
		if (AI_Slaughter.slaughtering)
		{
			flag2 = true;
			num = EClass.rndHalf(4 + 10 * (50 + Mathf.Max(0, (int)MathF.Sqrt(EClass.pc.Evalue(290) * 10))) / 100);
		}
		else if (origin != null && origin.HasElement(290) && !IsMinion)
		{
			if (!flag2 && Chara.race.corpse[1].ToInt() > EClass.rnd(150000 / (100 + (int)Mathf.Sqrt(origin.Evalue(290)) * 5)))
			{
				flag2 = true;
				origin.elements.ModExp(290, 100f);
			}
			else
			{
				origin.elements.ModExp(290, 5f);
			}
		}
		if (id == "littleOne" && IsPCFactionOrMinion)
		{
			flag2 = false;
		}
		if (flag2 && !isUserZone)
		{
			string text = Chara.race.corpse[0];
			bool num2 = text == "_meat";
			int num3 = 10;
			if (AI_Slaughter.slaughtering)
			{
				num3 += (int)Mathf.Min(Mathf.Sqrt(EClass.pc.Evalue(290)), 20f);
			}
			if (EClass.rnd((Act.CurrentAct is ActMeleeBladeStorm || (origin != null && (origin.HasElement(1556) || origin.HasCondition<ConTransmuteCat>()))) ? 2 : 100) == 0)
			{
				text = "dattamono";
			}
			if (num2 && num3 > EClass.rnd(100))
			{
				text = "meat_marble";
			}
			Thing thing3 = ThingGen.Create(text).SetNum(num);
			if (thing3.source._origin == "meat")
			{
				thing3.MakeFoodFrom(this);
			}
			else
			{
				thing3.ChangeMaterial(Chara.material);
			}
			list.Add(thing3);
		}
		if (!IsPCFaction && (!isUserZone || !EClass.game.principal.disableUsermapBenefit) && chance(200))
		{
			list.Add(Chara.MakeGene());
		}
		if (!IsPCFaction && !isUserZone)
		{
			foreach (string item2 in sourceCard.loot.Concat(Chara.race.loot).ToList())
			{
				string[] array = item2.Split('/');
				int num4 = array[1].ToInt();
				if (num4 >= 1000 || num4 > EClass.rnd(1000) || EClass.debug.godMode)
				{
					list.Add(ThingGen.Create(array[0]).SetNum((num4 < 1000) ? 1 : (num4 / 1000 + ((EClass.rnd(1000) > num4 % 1000) ? 1 : 0))));
				}
			}
			if (Chara.IsMachine)
			{
				if (chance(20))
				{
					list.Add(ThingGen.Create("microchip"));
				}
				if (chance(15))
				{
					list.Add(ThingGen.Create("battery"));
				}
			}
			else
			{
				if (Chara.IsAnimal)
				{
					if (chance(15))
					{
						list.Add(ThingGen.Create("fang"));
					}
					if (chance(10))
					{
						list.Add(ThingGen.Create("skin"));
					}
				}
				if (chance(20))
				{
					list.Add(ThingGen.Create("offal"));
				}
				if (chance(20))
				{
					list.Add(ThingGen.Create("heart"));
				}
			}
			if (!isBackerContent && !flag)
			{
				switch (id)
				{
				case "isca":
					list.Add(ThingGen.Create("blood_angel"));
					break;
				case "golem_wood":
					if (chance(30))
					{
						list.Add(ThingGen.Create("crystal_earth"));
					}
					break;
				case "golem_fish":
				case "golem_stone":
					if (chance(30))
					{
						list.Add(ThingGen.Create("crystal_sun"));
					}
					break;
				case "golem_steel":
					if (chance(30))
					{
						list.Add(ThingGen.Create("crystal_mana"));
					}
					break;
				case "golem_gold":
					list.Add(ThingGen.Create("money2"));
					break;
				}
				int num5 = ((EClass._zone.Boss == this) ? 2 : ((this.rarity >= Rarity.Legendary) ? 1 : 0));
				if (EClass._zone is Zone_Void)
				{
					num5++;
				}
				if (EClass.rnd(5) == 0)
				{
					num5++;
				}
				string text2 = id;
				if (text2 == "big_daddy" || text2 == "santa")
				{
					num5 += 2;
				}
				if (num5 > 0 && EClass.game.principal.dropRate)
				{
					num5 = Mathf.Max(1, num5 * (50 + EClass.game.principal.dropRateMtp * 50) / 100);
				}
				List<Thing> list2 = new List<Thing>();
				foreach (Thing thing4 in things)
				{
					if (thing4.HasTag(CTAG.gift) || thing4.trait is TraitChestMerchant)
					{
						continue;
					}
					if (thing4.isGifted || thing4.rarity >= Rarity.Artifact || thing4.trait.DropChance > EClass.rndf(1f))
					{
						list.Add(thing4);
					}
					else if (thing4.IsEquipmentOrRanged)
					{
						if (thing4.rarity >= Rarity.Legendary)
						{
							list2.Add(thing4);
						}
						else if (EClass.rnd(100) == 0)
						{
							list.Add(thing4);
						}
					}
					else if (EClass.rnd(5) == 0)
					{
						list.Add(thing4);
					}
				}
				if (num5 > 0 && list2.Count > 0)
				{
					list2.Shuffle();
					for (int j = 0; j < list2.Count && j < num5; j++)
					{
						list.Add(list2[j]);
						num5--;
					}
				}
				if (this.rarity >= Rarity.Legendary && !IsUnique && c_bossType != BossType.Evolved)
				{
					int num6 = 0;
					foreach (Card item3 in list)
					{
						if (item3.rarity >= Rarity.Legendary || item3.IsContainer)
						{
							num6++;
						}
					}
					if (num6 == 0)
					{
						int num7 = ((!(EClass._zone is Zone_Void)) ? 1 : 2);
						if (num5 < num7)
						{
							num5 = num7;
						}
						for (int k = 0; k < num5; k++)
						{
							Rand.SetSeed(uid + k);
							if (EClass.rnd((EClass._zone.events.GetEvent<ZoneEventDefenseGame>() != null) ? 3 : 2) == 0)
							{
								Rarity rarity = ((EClass.rnd(20) == 0) ? Rarity.Mythical : Rarity.Legendary);
								CardBlueprint.Set(new CardBlueprint
								{
									rarity = rarity
								});
								Thing item = ThingGen.CreateFromFilter("eq", LV);
								list.Add(item);
							}
							else if (EClass.rnd(3) == 0)
							{
								list.Add(ThingGen.Create("medal"));
							}
							Rand.SetSeed();
						}
					}
				}
			}
		}
		foreach (Thing thing5 in things)
		{
			if (thing5.GetInt(116) != 0)
			{
				list.Add(thing5);
			}
		}
		Point nearestPoint = GetRootCard().pos;
		if (nearestPoint.IsBlocked)
		{
			nearestPoint = nearestPoint.GetNearestPoint();
		}
		foreach (Card item4 in list)
		{
			if (item4.parent == EClass._zone)
			{
				continue;
			}
			item4.isHidden = false;
			item4.SetInt(116);
			EClass._zone.AddCard(item4, nearestPoint);
			if (!item4.IsEquipment || item4.rarity < Rarity.Superior || item4.IsCursed)
			{
				continue;
			}
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara.HasElement(1412) && chara.Dist(nearestPoint) < 3)
				{
					item4.Thing.TryLickEnchant(chara);
					break;
				}
			}
		}
		bool chance(int i)
		{
			i = i * 100 / (100 + EClass.player.codex.GetOrCreate(id).BonusDropLv * 10);
			if (i < 1)
			{
				i = 1;
			}
			if (IsMinion)
			{
				i *= 5;
			}
			if (EClass.rnd(i) == 0)
			{
				return true;
			}
			return false;
		}
	}

	public Thing TryMakeRandomItem(int lv = -1, TryMakeRandomItemSource itemSource = TryMakeRandomItemSource.Default, Chara crafter = null)
	{
		if (lv == -1)
		{
			lv = EClass._zone.DangerLv;
		}
		switch (id)
		{
		case "gene":
			DNA.CopyDNA(DNA.GenerateRandomGene(lv), Thing);
			break;
		case "log":
			ChangeMaterial(EClass.sources.materials.rows.Where((SourceMaterial.Row m) => m.category == "wood").RandomItem());
			break;
		case "ore_gem":
			ChangeMaterial(MATERIAL.GetRandomMaterialFromCategory(lv, "gem", material));
			break;
		case "ore":
			ChangeMaterial(MATERIAL.GetRandomMaterialFromCategory(lv, "ore", material));
			break;
		case "milk":
		case "_egg":
		case "egg_fertilized":
		case "_meat":
		case "meat_marble":
		case "dattamono":
		{
			string text = "c_wilds";
			if (id == "_meat" || id == "meat_marble")
			{
				text = "c_animal";
			}
			if (itemSource == TryMakeRandomItemSource.Cooking && crafter != null && crafter.HasElement(1205))
			{
				text = "c_human";
			}
			for (int i = 0; i < 20; i++)
			{
				CardRow cardRow = SpawnList.Get(text).Select(lv + i);
				if ((cardRow.model.Chara.race.corpse[0] != "_meat" && id != "milk" && id != "_egg" && id != "egg_fertilized") || (itemSource == TryMakeRandomItemSource.Cooking && cardRow.model.HasElement(701)))
				{
					continue;
				}
				if (id == "milk")
				{
					if (c_idRefCard.IsEmpty())
					{
						MakeRefFrom(cardRow.model);
					}
				}
				else
				{
					MakeFoodFrom(cardRow.model);
				}
				return this as Thing;
			}
			MakeFoodFrom(EClass.sources.charas.map.Values.Where((SourceChara.Row r) => r.chance > 0 && r.quality == 0).RandomItem().model);
			break;
		}
		}
		return this as Thing;
	}

	public Card MakeFoodFrom(string _id)
	{
		return MakeFoodFrom(EClass.sources.cards.map[_id].model);
	}

	public Card MakeFoodFrom(Card c)
	{
		MakeRefFrom(c);
		ChangeMaterial(c.material);
		if (!c.isChara)
		{
			return this;
		}
		SourceRace.Row race = c.Chara.race;
		int num = race.food[0].ToInt();
		bool flag = id == "meat_marble";
		int num2 = 1;
		bool flag2 = category.IsChildOf("meat");
		bool flag3 = category.IsChildOf("egg");
		if (flag)
		{
			num += 100;
		}
		if (flag2)
		{
			if (c.IsPCFaction && c.IsUnique)
			{
				num = -100;
			}
			elements.SetBase(70, race.STR * race.STR / 5 * num / 100 - 10 + num / 10);
			if (flag)
			{
				elements.SetBase(440, race.END * race.END / 5 * num / 100 - 10 + num / 10);
			}
			elements.SetBase(71, (int)Mathf.Clamp((float)(num / 10) + Mathf.Sqrt(race.height) - 10f, 1f, 60f));
		}
		else if (flag3)
		{
			elements.SetBase(444, race.LER * race.LER / 5 * num / 100 - 10 + num / 10);
			num2 = 2;
		}
		else
		{
			num2 = 3;
		}
		if (flag2)
		{
			if (c.Chara.IsHuman)
			{
				elements.SetBase(708, 1);
			}
			if (c.Chara.IsUndead)
			{
				elements.SetBase(709, 1);
			}
		}
		foreach (Element value in c.elements.dict.Values)
		{
			if ((!flag3 || value.id != 1229) && (value.source.category == "food" || value.source.tag.Contains("foodEnc") || value.IsTrait))
			{
				elements.SetBase(value.id, value.Value);
			}
		}
		List<Tuple<int, int>> list = new List<Tuple<int, int>>();
		foreach (KeyValuePair<int, int> item in race.elementMap)
		{
			if (EClass.sources.elements.map[item.Key].tag.Contains("primary"))
			{
				list.Add(new Tuple<int, int>(item.Key, item.Value));
			}
		}
		list.Sort((Tuple<int, int> a, Tuple<int, int> b) => b.Item2 - a.Item2);
		for (int i = 0; i < num2 && i < list.Count; i++)
		{
			Tuple<int, int> tuple = list[i];
			elements.SetBase(tuple.Item1, tuple.Item2 * tuple.Item2 / 4);
		}
		if (c.Chara.IsUndead)
		{
			elements.ModBase(73, -20);
		}
		isWeightChanged = true;
		c_weight = race.height * 4 + 100;
		c_idMainElement = c.c_idMainElement;
		SetBlessedState(BlessedState.Normal);
		int num3 = c.LV - c.sourceCard.LV;
		if (num3 < 0)
		{
			num3 = 0;
		}
		num3 = EClass.curve(num3, 10, 10, 80);
		if (c.rarity >= Rarity.Legendary || c.IsUnique)
		{
			num3 += 60;
		}
		if (flag2 && c.IsPCFaction && c.IsUnique)
		{
			num3 = 0;
		}
		if (num3 > 0)
		{
			elements.ModBase(2, num3);
		}
		return this;
	}

	public void MakeFoodRef(Card c1, Card c2 = null)
	{
		Card card = c1;
		Card card2 = c2;
		if (IsIgnoreName(card))
		{
			card = null;
		}
		if (IsIgnoreName(card2))
		{
			card2 = null;
		}
		if (card == null && card2 != null)
		{
			card = card2;
			card2 = null;
		}
		if (card != null)
		{
			MakeRefFrom(card, card2);
			if (card.c_idRefCard != null)
			{
				c_idRefCard = card.c_idRefCard;
				c_altName = TryGetFoodName(card);
			}
			if (card2 != null && card2.c_idRefCard != null)
			{
				c_idRefCard2 = card2.c_idRefCard;
				c_altName2 = TryGetFoodName(card2);
			}
		}
		static bool IsIgnoreName(Card c)
		{
			if (c == null)
			{
				return true;
			}
			switch (c.id)
			{
			case "dough_cake":
			case "dough_bread":
			case "noodle":
			case "flour":
			case "rice":
				return true;
			default:
				return false;
			}
		}
	}

	public string TryGetFoodName(Card c)
	{
		if (c.c_idRefCard.IsEmpty())
		{
			return c.c_altName;
		}
		if (!(c.refCard is SourceChara.Row { isChara: not false } row))
		{
			return c.c_altName;
		}
		if (!row.aka.IsEmpty())
		{
			if (row.name == "*r" && row.aka == "*r")
			{
				return "corpseGeneral".lang();
			}
			if (row.name == "*r")
			{
				return row.GetText("aka");
			}
		}
		return row.GetName();
	}

	public string GetFoodName(string s)
	{
		return s.Replace("_corpseFrom".lang(), "_corpseTo".lang());
	}

	public void MakeFigureFrom(string id)
	{
		MakeRefFrom(id);
	}

	public void MakeRefFrom(string id)
	{
		c_idRefCard = id;
	}

	public void MakeRefFrom(Card c1, Card c2 = null)
	{
		c_idRefCard = c1.id;
		c_altName = (c1.IsPC ? c1.c_altName : c1.GetName(NameStyle.Ref, (!c1.isChara) ? 1 : 0));
		if (c2 != null)
		{
			c_idRefCard2 = c2.id;
			c_altName2 = (c2.IsPC ? c2.c_altName : c2.GetName(NameStyle.Ref, (!c2.isChara) ? 1 : 0));
		}
		c_extraNameRef = (c1.IsPC ? EClass.pc.c_altName : c1.c_extraNameRef);
	}

	public Thing MakeEgg(bool effect = true, int num = 1, bool addToZone = true, int fertChance = 20, BlessedState? state = null)
	{
		Thing thing = ThingGen.Create((EClass.rnd(EClass.debug.enable ? 1 : fertChance) == 0) ? "egg_fertilized" : "_egg").SetNum(num);
		thing.MakeFoodFrom(this);
		thing.c_idMainElement = c_idMainElement;
		if (state.HasValue)
		{
			thing.SetBlessedState(state.Value);
		}
		if (!addToZone)
		{
			return thing;
		}
		return GiveBirth(thing, effect);
	}

	public Thing MakeMilk(bool effect = true, int num = 1, bool addToZone = true, BlessedState? state = null)
	{
		Thing thing = ThingGen.Create("milk").SetNum(num);
		thing.MakeRefFrom(this);
		if (state.HasValue)
		{
			thing.SetBlessedState(state.Value);
		}
		int num2 = LV - sourceCard.LV;
		if (!IsPCFaction && EClass._zone.IsUserZone)
		{
			num2 = 0;
		}
		if (num2 >= 10)
		{
			thing.SetEncLv(num2 / 10);
		}
		if (!addToZone)
		{
			return thing;
		}
		return GiveBirth(thing, effect);
	}

	public Thing GiveBirth(Thing t, bool effect)
	{
		Card card = (ExistsOnMap ? this : (GetRootCard() ?? EClass.pc));
		EClass.player.forceTalk = true;
		card.Talk("giveBirth");
		EClass._zone.TryAddThing(t, card.pos);
		if (effect)
		{
			card.pos.PlayEffect("revive");
			card.pos.PlaySound("egg");
			PlayAnime(AnimeID.Shiver);
			if (isChara)
			{
				Chara.AddCondition<ConDim>(200);
			}
		}
		return t;
	}

	public Card SetHidden(bool hide = true)
	{
		isHidden = hide;
		pos.cell.Refresh();
		return this;
	}

	public virtual MoveResult _Move(Point p, MoveType type = MoveType.Walk)
	{
		EClass._map.MoveCard(p, this);
		if (isChara)
		{
			Chara.SyncRide();
		}
		return MoveResult.Success;
	}

	public void MoveImmediate(Point p, bool focus = true, bool cancelAI = true)
	{
		if (p == null)
		{
			return;
		}
		EClass._map.MoveCard(p, this);
		if (!IsPC || focus)
		{
			renderer.SetFirst(first: true, p.PositionCenter());
		}
		if (isChara)
		{
			if (cancelAI)
			{
				Chara.ai.Cancel();
			}
			Chara.SyncRide();
		}
		if (IsPC && focus)
		{
			EClass.screen.FocusPC();
			EClass.screen.RefreshPosition();
		}
	}

	public void Teleport(Point point, bool silent = false, bool force = false)
	{
		if (EClass._zone.IsRegion)
		{
			SayNothingHappans();
			return;
		}
		PlayEffect("teleport");
		if (!force && (!trait.CanBeTeleported || elements.Has(400) || (isChara && Chara.HasCondition<ConGravity>())))
		{
			Say("antiTeleport", this);
			PlaySound("gravity");
			return;
		}
		if (!silent)
		{
			PlaySound("teleport");
			Say("teleported", this);
		}
		_Move(point);
		renderer.SetFirst(first: true, pos.PositionCenter());
		if (isChara)
		{
			Chara.ai.Cancel();
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara.enemy == this)
				{
					chara.SetEnemy();
				}
			}
			Chara.RemoveCondition<ConEntangle>();
		}
		if (IsPC)
		{
			EClass.screen.FocusPC();
			EClass.screen.RefreshPosition();
			EClass.player.haltMove = true;
		}
		PlayEffect("teleport", useRenderPos: false);
	}

	public virtual void OnLand()
	{
		if (Cell.IsTopWaterAndNoSnow)
		{
			PlayEffect("ripple");
			PlaySound("Footstep/water");
		}
	}

	public int ResistLvFrom(int ele)
	{
		return ResistLv(EClass.sources.elements.alias.TryGetValue(EClass.sources.elements.map[ele].aliasRef)?.id ?? 0);
	}

	public int ResistLv(int res)
	{
		return Element.GetResistLv(Evalue(res));
	}

	public bool HasElement(int ele, int req = 1)
	{
		return elements.Value(ele) >= req;
	}

	public bool HasElement(string id, int req = 1)
	{
		return HasElement(EClass.sources.elements.alias[id].id, req);
	}

	public bool HasGlobalElement(int ele)
	{
		return elements.GetElement(ele)?.IsGlobalElement ?? false;
	}

	public bool HasElementNoCopy()
	{
		if (HasElement(764))
		{
			return true;
		}
		if (HasElement(759))
		{
			return true;
		}
		if (HasElement(703))
		{
			return true;
		}
		return false;
	}

	public virtual CardRenderer _CreateRenderer()
	{
		renderer = new CardRenderer();
		renderer.SetOwner(this);
		return renderer;
	}

	public void AddBlood(int a = 1, int id = -1)
	{
		if (!EClass._zone.IsRegion)
		{
			for (int i = 0; i < a; i++)
			{
				EClass._map.AddDecal(pos.x + ((EClass.rnd(2) != 0) ? (EClass.rnd(3) - 1) : 0), pos.z + ((EClass.rnd(2) != 0) ? (EClass.rnd(3) - 1) : 0), (id == -1) ? (isChara ? Chara.race.blood : material.decal) : id);
			}
			PlaySound("blood");
		}
	}

	public RenderParam GetRenderParam()
	{
		RenderParam shared = RenderParam.shared;
		shared.color = 11010048f;
		shared.liquidLv = 0;
		shared.cell = null;
		SetRenderParam(shared);
		return shared;
	}

	public virtual void SetRenderParam(RenderParam p)
	{
	}

	public void DyeRandom()
	{
		Dye(EClass.sources.materials.rows.Where((SourceMaterial.Row r) => r.matColor.r != r.matColor.g || r.matColor.g != r.matColor.b || r.matColor.b != r.matColor.r).RandomItem().alias);
	}

	public void Dye(string idMat)
	{
		Dye(EClass.sources.materials.alias[idMat]);
	}

	public void Dye(SourceMaterial.Row mat)
	{
		isDyed = mat != null;
		c_dyeMat = mat?.id ?? 0;
		_colorInt = 0;
	}

	public void RefreshColor()
	{
		if (isChara)
		{
			if (isDyed)
			{
				_colorInt = BaseTileMap.GetColorInt(ref DyeMat.matColor, sourceRenderCard.colorMod);
			}
			else if (isElemental)
			{
				_colorInt = BaseTileMap.GetColorInt(ref EClass.setting.elements[Chara.MainElement.source.alias].colorSprite, sourceRenderCard.colorMod);
			}
			else
			{
				_colorInt = 104025;
			}
		}
		else if (isDyed)
		{
			if (sourceRenderCard.useAltColor)
			{
				_colorInt = BaseTileMap.GetColorInt(ref DyeMat.altColor, sourceRenderCard.colorMod);
			}
			else
			{
				_colorInt = BaseTileMap.GetColorInt(ref DyeMat.matColor, sourceRenderCard.colorMod);
			}
		}
		else if (sourceRenderCard.useRandomColor)
		{
			_colorInt = BaseTileMap.GetColorInt(ref GetRandomColor(), sourceRenderCard.colorMod);
		}
		else if (sourceRenderCard.useAltColor)
		{
			_colorInt = BaseTileMap.GetColorInt(ref material.altColor, sourceRenderCard.colorMod);
		}
		else
		{
			_colorInt = BaseTileMap.GetColorInt(ref material.matColor, sourceRenderCard.colorMod);
		}
	}

	public ref Color GetRandomColor()
	{
		int num = EClass.game.seed + refVal;
		num += id[0] % 10000;
		if (id.Length > 1)
		{
			num += id[1] % 1000;
			if (id.Length > 2)
			{
				num += id[2] % 1000;
				if (id.Length > 3)
				{
					num += id[3] % 1000;
					if (id.Length > 4)
					{
						num += id[4] % 1000;
					}
				}
			}
		}
		Rand.UseSeed(num, delegate
		{
			_randColor = EClass.sources.materials.rows[EClass.rnd(90)].matColor;
		});
		return ref _randColor;
	}

	public virtual Sprite GetSprite(int dir = 0)
	{
		if (trait is TraitAbility)
		{
			return (trait as TraitAbility).CreateAct()?.GetSprite() ?? EClass.core.refs.icons.defaultAbility;
		}
		return sourceCard.GetSprite(dir, trait.IdSkin, (IsInstalled && pos != null && pos.IsValid && pos.cell.IsSnowTile) ? true : false);
	}

	public virtual Sprite GetImageSprite()
	{
		return null;
	}

	public void SetImage(Image image, int dir, int idSkin = 0)
	{
		sourceRenderCard.SetImage(image, GetSprite(dir), colorInt, setNativeSize: true, dir, idSkin, this);
	}

	public virtual void SetImage(Image image)
	{
		if (trait is TraitAbility)
		{
			(trait as TraitAbility).act.SetImage(image);
		}
		else
		{
			sourceRenderCard.SetImage(image, GetSprite(), colorInt, setNativeSize: true, 0, 0, this);
		}
	}

	public void ShowEmo(Emo _emo = Emo.none, float duration = 0f, bool skipSame = true)
	{
		if ((!isChara || Chara.host == null) && !(_emo == lastEmo && skipSame))
		{
			if (_emo != 0)
			{
				renderer.ShowEmo(_emo, duration);
			}
			lastEmo = _emo;
		}
	}

	public void PlaySoundHold(bool spatial = true)
	{
		PlaySound(material.GetSoundDrop(sourceCard), 1f, spatial);
	}

	public void PlaySoundDrop(bool spatial = true)
	{
		PlaySound(material.GetSoundDrop(sourceCard), 1f, spatial);
	}

	public void PlaySoundImpact(bool spatial = true)
	{
		PlaySound(material.GetSoundImpact(sourceCard), 1f, spatial);
	}

	public void PlaySoundDead(bool spatial = true)
	{
		PlaySound(material.GetSoundDead(sourceCard), 1f, spatial);
	}

	public SoundSource PlaySound(string id, float v = 1f, bool spatial = true)
	{
		Card rootCard = GetRootCard();
		if (rootCard.IsPC)
		{
			spatial = false;
		}
		if (rootCard.Dist(EClass.pc) < EClass.player.lightRadius + 1 || !spatial)
		{
			return rootCard.pos.PlaySound(id, isSynced || !spatial, v, spatial);
		}
		return null;
	}

	public void KillAnime()
	{
		renderer.KillAnime();
	}

	public void PlayAnime(AnimeID id, bool force = false)
	{
		renderer.PlayAnime(id, force);
	}

	public void PlayAnime(AnimeID id, Point dest, bool force = false)
	{
		renderer.PlayAnime(id, dest);
	}

	public void PlayAnimeLoot()
	{
		renderer.PlayAnime(AnimeID.Loot);
	}

	public Effect PlayEffect(string id, bool useRenderPos = true, float range = 0f, Vector3 fix = default(Vector3))
	{
		if (id.IsEmpty())
		{
			return null;
		}
		Card rootCard = GetRootCard();
		return Effect.Get(id)._Play(rootCard.pos, fix + ((isSynced && useRenderPos) ? rootCard.renderer.position : rootCard.pos.Position()) + new Vector3(Rand.Range(0f - range, range), Rand.Range(0f - range, range), 0f));
	}

	public void PlayEffect(int ele, bool useRenderPos = true, float range = 0f)
	{
		Effect effect = Effect.Get("Element/" + EClass.sources.elements.map[ele].alias);
		if (effect == null)
		{
			Debug.Log(ele);
			return;
		}
		Card rootCard = GetRootCard();
		effect._Play(rootCard.pos, ((isSynced && useRenderPos) ? rootCard.renderer.position : rootCard.pos.Position()) + new Vector3(Rand.Range(0f - range, range), Rand.Range(0f - range, range), 0f));
	}

	public virtual void SetDir(int d)
	{
		dir = d;
		renderer.RefreshSprite();
	}

	public void SetRandomDir()
	{
		SetDir(EClass.rnd(4));
	}

	public virtual void LookAt(Card c)
	{
	}

	public virtual void LookAt(Point p)
	{
	}

	public virtual void Rotate(bool reverse = false)
	{
		int num = 4;
		if (sourceCard.tiles.Length > 4)
		{
			num = sourceCard.tiles.Length;
		}
		if (TileType == TileType.Door)
		{
			num = 2;
		}
		if (reverse)
		{
			dir--;
		}
		else
		{
			dir++;
		}
		if (dir < 0)
		{
			dir = num - 1;
		}
		if (dir == num)
		{
			dir = 0;
		}
		SetDir(dir);
		renderer.RefreshSprite();
	}

	public void ChangeAltitude(int a)
	{
		altitude += a;
		if (altitude < 0)
		{
			altitude = 0;
		}
		if (altitude > TileType.MaxAltitude)
		{
			altitude = TileType.MaxAltitude;
		}
	}

	public virtual SubPassData GetSubPassData()
	{
		return SubPassData.Default;
	}

	public void SetFreePos(Point point)
	{
		freePos = EClass.game.config.FreePos && isThing && TileType.FreeStyle && !sourceCard.multisize && !EClass.scene.actionMode.IsRoofEditMode(this);
		if (freePos)
		{
			Vector3 vector = point.Position();
			Vector3 thingPosition = EClass.screen.tileMap.GetThingPosition(this, point);
			fx = EInput.mposWorld.x + EClass.setting.render.freePosFix.x;
			fy = EInput.mposWorld.y + EClass.setting.render.freePosFix.y;
			if (EClass.game.config.snapFreePos)
			{
				fx -= fx % 0.2f;
				fy -= fy % 0.1f;
			}
			fx = fx - vector.x + thingPosition.x;
			fy = fy - vector.y + thingPosition.y;
		}
		else
		{
			float num2 = (fy = 0f);
			fx = num2;
		}
	}

	public void RenderMarker(Point point, bool active, HitResult result, bool main, int dir, bool useCurrentPosition = false)
	{
		if (dir != -1)
		{
			this.dir = dir;
		}
		Vector3 v = point.Position();
		bool skipRender = point.cell.skipRender;
		if (result != 0 && EClass.screen.guide.isActive && !skipRender)
		{
			EClass.screen.guide.passGuideBlock.Add(ref v, (point.HasObj || point.HasChara) ? 5 : 0);
		}
		if (!main)
		{
			return;
		}
		RenderParam renderParam = GetRenderParam();
		if (EClass.scene.actionMode.IsRoofEditMode(this))
		{
			renderParam.x = v.x;
			renderParam.y = v.y;
			renderParam.z = v.z;
			EClass.screen.tileMap.SetRoofHeight(renderParam, point.cell, point.x, point.z);
			v.x = renderParam.x;
			v.y = renderParam.y;
			v.z = renderParam.z;
		}
		if (TileType.UseMountHeight && !EClass.scene.actionMode.IsRoofEditMode(this))
		{
			TileType.GetMountHeight(ref v, point, this.dir, this);
		}
		v.z += EClass.setting.render.thingZ;
		if (!skipRender)
		{
			Vector3 thingPosition = EClass.screen.tileMap.GetThingPosition(this, point);
			if (freePos)
			{
				v.x += fx;
				v.y += fy;
				v.z += thingPosition.z;
			}
			else
			{
				v += thingPosition;
			}
		}
		if (useCurrentPosition)
		{
			v = renderer.position;
			v.z += -0.01f;
		}
		if (TileType == TileType.Door)
		{
			v.z -= 0.5f;
		}
		renderParam.matColor = (active ? EClass.Colors.blockColors.Active : EClass.Colors.blockColors.Inactive);
		point.ApplyAnime(ref v);
		if (renderer.hasActor)
		{
			renderer.actor.RefreshSprite();
		}
		renderer.Draw(renderParam, ref v, drawShadow: false);
	}

	public void RecalculateFOV()
	{
		if (fov != null)
		{
			ClearFOV();
			fov = null;
			if (IsPC)
			{
				EClass.player.lightRadius = 1;
			}
		}
		CalculateFOV();
	}

	public bool HasLight()
	{
		return GetLightRadius() > 0;
	}

	public float GetLightPower()
	{
		float num = (isChara ? EClass.scene.profile.light.fovCurveChara.Evaluate(EClass.scene.timeRatio) : EClass.scene.profile.global.fovPower);
		if (LightData != null)
		{
			return 0.01f * LightData.color.a * 256f * 1.12f;
		}
		if (IsPCFaction && !IsPC)
		{
			num *= 4f;
		}
		return num;
	}

	public int GetSightRadius()
	{
		if (IsPC)
		{
			return EClass.player.lightRadius;
		}
		return (EClass._map.IsIndoor ? 4 : 5) + (IsPCFaction ? 1 : 0);
	}

	public int GetLightRadius()
	{
		if (isThing)
		{
			if (!IsInstalled && EClass.pc.held != this)
			{
				return 0;
			}
			if (trait is TraitLightSource && Thing.isEquipped)
			{
				return (trait as TraitLightSource).LightRadius;
			}
			if (LightData == null || !trait.IsLightOn)
			{
				return 0;
			}
			return LightData.radius;
		}
		int num = ((LightData != null) ? LightData.radius : 0);
		int num2 = 0;
		if (IsPC)
		{
			if (Chara.isBlind)
			{
				return 1;
			}
			num = ((EClass._map.IsIndoor || EClass.world.date.IsNight) ? 2 : ((EClass.world.date.periodOfDay == PeriodOfDay.Day) ? 6 : 5));
			num2 = 2;
		}
		else
		{
			if (!EClass.core.config.graphic.drawAllyLight)
			{
				return 0;
			}
			if (LightData == null && !EClass._map.IsIndoor && !EClass.world.date.IsNight)
			{
				return 0;
			}
		}
		if (IsPCFaction)
		{
			Thing equippedThing = Chara.body.GetEquippedThing(45);
			if (equippedThing != null && equippedThing.trait is TraitLightSource traitLightSource)
			{
				num2 = traitLightSource.LightRadius;
			}
			if (Chara.held != null && IsPC)
			{
				int lightRadius = Chara.held.GetLightRadius();
				if (lightRadius > 0)
				{
					if (lightRadius > num2)
					{
						num2 = Chara.held.GetLightRadius() - 1;
					}
					if (num2 < 3)
					{
						num2 = 3;
					}
				}
			}
			if (num < num2)
			{
				num = num2;
			}
		}
		return num;
	}

	public void CalculateFOV()
	{
		int radius = GetLightRadius();
		if (radius == 0 || !IsAliveInCurrentZone || !EClass._zone.isStarted)
		{
			return;
		}
		float power = GetLightPower();
		if (IsPC)
		{
			if (Chara.held != null && Chara.held.GetLightRadius() > 0)
			{
				power += Chara.held.GetLightPower();
			}
			if (radius <= 2)
			{
				power = 0f;
			}
			foreach (Condition condition in Chara.conditions)
			{
				condition.OnCalculateFov(fov, ref radius, ref power);
			}
			if (power > EClass.scene.profile.global.playerLightPowerLimit)
			{
				power = EClass.scene.profile.global.playerLightPowerLimit;
			}
			power *= EClass.scene.profile.light.playerLightMod + (float)EClass.player.customLightMod * EClass.scene.profile.light.playerLightCustomMod;
			EClass.player.lightRadius = radius;
			EClass.player.lightPower = power;
		}
		if (fov == null)
		{
			fov = CreateFov();
		}
		fov.Perform(pos.x, pos.z, radius, power * 2f);
	}

	public void SetRandomLightColors()
	{
		c_lightColor = (byte)(EClass.rnd(8) + 1) * 1024 + (byte)(EClass.rnd(8) + 1) * 32 + (byte)(EClass.rnd(8) + 1);
	}

	public Fov CreateFov()
	{
		Fov fov = new Fov();
		int num = (trait.UseLightColor ? c_lightColor : 0);
		if (num != 0)
		{
			fov.r = (byte)(num / 1024);
			fov.g = (byte)(num % 1024 / 32);
			fov.b = (byte)(num % 32);
		}
		else if (LightData != null)
		{
			fov.r = (byte)(LightData.color.r * 16f);
			fov.g = (byte)(LightData.color.g * 16f);
			fov.b = (byte)(LightData.color.b * 16f);
		}
		else if (isChara)
		{
			fov.r = 0;
			fov.g = 0;
			fov.b = 0;
		}
		else
		{
			fov.r = 3;
			fov.g = 2;
			fov.b = 1;
		}
		if (isChara && Chara.held != null && Chara.held.GetLightRadius() > 0)
		{
			Fov fov2 = Chara.held.CreateFov();
			fov.r += fov2.r;
			fov.g += fov2.g;
			fov.b += fov2.b;
		}
		if (IsPC)
		{
			fov.isPC = true;
			foreach (Condition condition in Chara.conditions)
			{
				condition.OnCreateFov(fov);
			}
		}
		fov.limitGradient = IsPCParty && EClass.scene.profile.global.limitGradient;
		return fov;
	}

	public void ClearFOV()
	{
		if (fov != null && fov.lastPoints.Count != 0)
		{
			fov.Perform(pos.x, pos.z, 0);
		}
	}

	public virtual void OnSimulateHour(VirtualDate date)
	{
		trait.OnSimulateHour(date);
		if (date.IsRealTime)
		{
			DecayNatural();
		}
	}

	public void DecayNatural(int hour = 1)
	{
		if (!isNPCProperty)
		{
			things.ForeachReverse(delegate(Thing t)
			{
				t.DecayNatural(hour);
			});
			if (sourceCard._origin == "dish")
			{
				CheckJustCooked();
			}
			if (parent is Card && (parent as Card).trait.CanChildDecay(this))
			{
				Decay(10 * hour);
			}
			else if (!isChara && trait.Decay != 0)
			{
				Decay(trait.Decay * hour);
			}
		}
	}

	public void CheckJustCooked()
	{
		if (HasElement(757) && c_dateCooked <= EClass.world.date.GetRaw() - 120)
		{
			c_dateCooked = 0;
			elements.Remove(757);
		}
	}

	public void Decay(int a = 10)
	{
		Card card = parent as Card;
		int num = 200;
		int num2 = MaxDecay / 4 * 3;
		if (a > 0)
		{
			if (card != null)
			{
				num = card.trait.DecaySpeedChild;
			}
			num = num * trait.DecaySpeed / 100;
			int num3 = Evalue(405);
			if (num3 != 0)
			{
				num = num * (100 - num3 * 2) / 100;
			}
			if (num < 0)
			{
				num = 0;
			}
		}
		else
		{
			num = 100;
		}
		a = a * num / 100;
		if (decay + a > MaxDecay)
		{
			if (card != null && !card.trait.OnChildDecay(this, !IsDecayed))
			{
				return;
			}
			if (!IsDecayed)
			{
				if (EClass.pc.HasElement(1325) && GetRootCard() is Chara && category.IsChildOf("food"))
				{
					Thing thing = TraitSeed.MakeRandomSeed(enc: true).SetNum(Mathf.Min(Num, 3));
					card.AddCard(thing);
					if (!IsParentLocked())
					{
						GetRootCard().Say("seed_rot", GetRootCard(), this, thing.Name);
					}
					Destroy();
					return;
				}
				if (parent == EClass._zone)
				{
					Say("rot", this);
				}
				else if (GetRootCard() == EClass.pc)
				{
					if (!IsParentLocked())
					{
						EClass.pc.Say("rotInv", this, EClass.pc);
					}
					LayerInventory.SetDirty(Thing);
				}
				if (IsFood)
				{
					elements.ModBase(73, -10);
				}
			}
		}
		else if (decay < num2 && decay + a >= num2 && GetRootCard() == EClass.pc)
		{
			if (!IsParentLocked())
			{
				EClass.pc.Say("rottingInv", this, EClass.pc);
			}
			LayerInventory.SetDirty(Thing);
		}
		decay += a;
		bool IsParentLocked()
		{
			if (parent is Thing)
			{
				return (parent as Thing).c_lockLv > 0;
			}
			return false;
		}
	}

	public bool HasTalk(string idTopic)
	{
		return !MOD.listTalk.GetTalk(c_idTalk.IsEmpty(id), idTopic, useDefault: true).IsEmpty();
	}

	public void Talk(string idTopic, string ref1 = null, string ref2 = null, bool forceSync = false)
	{
		if (IsPC && !EClass.player.forceTalk && idTopic != "goodBoy" && idTopic != "insane")
		{
			EClass.player.forceTalk = false;
			Msg.SetColor();
			return;
		}
		EClass.player.forceTalk = false;
		if (!isSynced && !forceSync)
		{
			Msg.SetColor();
			return;
		}
		GameLang.refDrama1 = ref1;
		GameLang.refDrama2 = ref2;
		string text = GetTalkText(idTopic, stripPun: true);
		if (HasElement(1232) && idTopic != "baby")
		{
			BackerContent.GakiConvert(ref text, "babu");
		}
		else
		{
			string text2 = id;
			if (!(text2 == "adv_gaki"))
			{
				if (text2 == "corgon")
				{
					BackerContent.GakiConvert(ref text, "mokyu");
				}
			}
			else
			{
				BackerContent.GakiConvert(ref text);
			}
		}
		TalkRaw(text, ref1, ref2, forceSync);
	}

	public void TalkRaw(string text, string ref1 = null, string ref2 = null, bool forceSync = false)
	{
		if ((!isSynced && !forceSync) || text.IsEmpty())
		{
			Msg.SetColor();
			return;
		}
		if (ref1 != null)
		{
			text = text.Replace("#1", ref1);
		}
		if (ref2 != null)
		{
			text = text.Replace("#2", ref2);
		}
		HostRenderer.Say(ApplyNewLine(text));
		bool flag = text.StartsWith("*");
		Msg.SetColor(text.StartsWith("(") ? Msg.colors.Thinking : (flag ? Msg.colors.Ono : Msg.colors.Talk));
		if (!flag)
		{
			text = text.Bracket();
		}
		Msg.Say(text.Replace("&", ""));
	}

	public string ApplyNewLine(string text)
	{
		if (text.Contains("&"))
		{
			string text2 = "_comma".lang();
			text = text.Replace(text2 + " &", Environment.NewLine ?? "");
			text = text.Replace(text2 + "&", Environment.NewLine ?? "");
			text = text.Replace("&", Environment.NewLine ?? "");
		}
		return text;
	}

	public void SayRaw(string text, string ref1 = null, string ref2 = null)
	{
		if (isSynced && !text.IsEmpty())
		{
			if (ref1 != null)
			{
				text = text.Replace("#1", ref1);
			}
			if (ref2 != null)
			{
				text = text.Replace("#2", ref2);
			}
			HostRenderer.Say(text);
		}
	}

	public void SayNothingHappans()
	{
		Say("nothingHappens");
	}

	public void Say(string lang, string ref1 = null, string ref2 = null)
	{
		if (ShouldShowMsg)
		{
			Msg.Say(IsPC ? Lang.Game.TryGetId(lang + "_pc", lang) : lang, ref1, ref2);
		}
		Msg.SetColor();
	}

	public void Say(string lang, Card c1, Card c2, string ref1 = null, string ref2 = null)
	{
		if (ShouldShowMsg)
		{
			Msg.Say(IsPC ? Lang.Game.TryGetId(lang + "_pc", lang) : lang, c1, c2, ref1, ref2);
		}
		Msg.SetColor();
	}

	public void Say(string lang, Card c1, string ref1 = null, string ref2 = null)
	{
		if (ShouldShowMsg)
		{
			Msg.Say(IsPC ? Lang.Game.TryGetId(lang + "_pc", lang) : lang, c1, ref1, ref2);
		}
		Msg.SetColor();
	}

	public string GetTalkText(string idTopic, bool stripPun = false, bool useDefault = true)
	{
		bool flag = isChara && Chara.IsHumanSpeak;
		string text = MOD.listTalk.GetTalk(c_idTalk.IsEmpty(id), idTopic, useDefault, flag);
		if (!text.IsEmpty())
		{
			text = text.Split('|').RandomItem();
			if (!flag || (IsDeadOrSleeping && IsAliveInCurrentZone))
			{
				if (!text.StartsWith("(") && !text.StartsWith("*"))
				{
					text = "(" + text + ")";
				}
				text = text.Replace("。)", ")");
			}
		}
		return ApplyTone(text, stripPun);
	}

	public string ApplyTone(string text, bool stripPun = false)
	{
		text = GameLang.ConvertDrama(text, Chara);
		return ApplyTone(Chara, ref text, c_idTone, bio?.gender ?? 0, stripPun);
	}

	public static string ApplyTone(Chara c, ref string text, string _tones, int gender, bool stripPun = false)
	{
		if (text.IsEmpty())
		{
			return text;
		}
		string[] array = _tones.IsEmpty("").Split('|');
		string key = array[0];
		string text2 = "";
		MOD.tones.Initialize();
		if (!Lang.setting.useTone || MOD.tones.list.Count == 0)
		{
			text2 = text.Replace("{", "").Replace("}", "");
		}
		else
		{
			if (array[0].IsEmpty())
			{
				key = "default";
			}
			if (MOD.tones.all.ContainsKey(key))
			{
				StringBuilder stringBuilder = MOD.tones.ApplyTone(key, ref text, gender);
				if (Lang.isJP && c != null)
				{
					if (array.Length >= 2)
					{
						stringBuilder.Replace("_toneI".lang(), array[1]);
					}
					if (array.Length >= 3)
					{
						stringBuilder.Replace("_toneYou".lang(), array[2]);
					}
				}
				text2 = stringBuilder.ToString();
			}
			else
			{
				text2 = text.Replace("{", "").Replace("}", "");
			}
		}
		if (c != null)
		{
			text2 = text2.Replace("#me", c.NameSimple);
		}
		if (!stripPun || !Lang.setting.stripPuns)
		{
			return text2;
		}
		return text2.StripLastPun();
	}

	public void SetRandomTalk()
	{
		MOD.listTalk.Initialize();
		if (!MOD.listTalk.list[0].ContainsKey(id))
		{
			c_idTalk = MOD.listTalk.GetRandomID("human");
		}
	}

	public void SetRandomTone()
	{
		MOD.tones.Initialize();
		List<Dictionary<string, string>> list = MOD.tones.list;
		if (list.Count != 0)
		{
			int mtp = EClass.core.config.test.extraToneMTP switch
			{
				4 => 10, 
				3 => 5, 
				2 => 2, 
				1 => 1, 
				0 => 0, 
				_ => 0, 
			};
			if (EClass.debug.enable)
			{
				mtp *= 100;
			}
			string text = list.RandomItem()["id"];
			text = list.RandomItemWeighted((Dictionary<string, string> a) => a["chance"].ToInt() * ((!a["tag"].Contains("meta")) ? 1 : mtp))["id"];
			c_idTone = MOD.tones.GetToneID(text, bio?.gender ?? 0);
		}
	}

	public bool HasCraftBonusTrait()
	{
		return ListCraftBonusTraits().Count > 0;
	}

	public List<Element> ListCraftBonusTraits()
	{
		List<Element> list = new List<Element>();
		string[] tag = sourceCard.tag;
		for (int i = 0; i < tag.Length; i++)
		{
			string[] array = tag[i].Split('/');
			if (!(array[0] != "craft_bonus"))
			{
				Element item = Element.Create(array[1], array[2].ToInt());
				list.Add(item);
			}
		}
		return list;
	}

	public void TryStack(Thing t)
	{
		if (t == this)
		{
			return;
		}
		ThingContainer.DestData dest = things.GetDest(t);
		if (dest.stack != null)
		{
			if (IsPC)
			{
				Say("stack_thing", t, dest.stack);
			}
			t.TryStackTo(dest.stack);
		}
	}

	public void ApplyBacker(int bid)
	{
		ChangeRarity(Rarity.Normal);
		SourceBacker.Row row = EClass.sources.backers.map.TryGetValue(bid);
		if (row == null)
		{
			return;
		}
		c_idBacker = row.id;
		if (row.type == 4)
		{
			Chara.bio.SetGender(row.gender);
			Chara chara = Chara;
			Hostility hostility2 = (Chara.c_originalHostility = Hostility.Neutral);
			chara.hostility = hostility2;
		}
		if (row.type == 6)
		{
			Chara.bio.SetGender(row.gender);
			Chara.bio.SetPortrait(Chara);
			Chara.idFaith = row.deity.ToLower();
		}
		if (row.type == 4 || row.type == 5 || row.type == 7)
		{
			idSkin = ((row.skin == 0) ? EClass.rnd(sourceCard._tiles.Length) : row.skin);
			if (id == "putty_snow")
			{
				idSkin = 0;
			}
		}
		if (bid == 164)
		{
			Chara.EQ_ID("amulet_moonnight");
		}
	}

	public void RemoveBacker()
	{
		if (c_idBacker == 164)
		{
			Chara.things.Find("amulet_moonnight")?.Destroy();
		}
		c_idBacker = 0;
	}

	public void SetPaintData()
	{
		EClass.ui.Hide(0f);
		EClass.core.WaitForEndOfFrame(delegate
		{
			ClearPaintSprite();
			c_textureData = GetPaintData();
			EClass.core.WaitForEndOfFrame(delegate
			{
				EClass.ui.Show(0f);
			});
		});
	}

	public byte[] GetPaintData()
	{
		Sprite sprite = GetSprite();
		Texture2D texture2D = ScreenCapture.CaptureScreenshotAsTexture();
		int num = sprite.texture.width * 2;
		int num2 = sprite.texture.height * 2;
		int x = (int)Mathf.Clamp(Input.mousePosition.x - (float)(num / 2), 1f, texture2D.width - num - 1);
		int y = (int)Mathf.Clamp(Input.mousePosition.y - (float)(num2 / 2), 1f, texture2D.height - num2 - 1);
		Color[] pixels = texture2D.GetPixels(x, y, num, num2);
		Texture2D texture2D2 = new Texture2D(num, num2, TextureFormat.ARGB32, mipChain: false);
		texture2D2.SetPixels(pixels);
		texture2D2.Apply();
		byte[] result = texture2D2.EncodeToJPG();
		UnityEngine.Object.Destroy(texture2D);
		UnityEngine.Object.Destroy(texture2D2);
		return result;
	}

	public void ClearPaintSprite()
	{
		if ((bool)_paintSprite)
		{
			UnityEngine.Object.Destroy(_paintSprite.texture);
			UnityEngine.Object.Destroy(_paintSprite);
			_paintSprite = null;
		}
	}

	public Sprite GetPaintSprite()
	{
		if (!_paintSprite)
		{
			byte[] data = c_textureData;
			Texture2D texture2D = new Texture2D(1, 1);
			texture2D.LoadImage(data);
			_paintSprite = Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 200f);
			EClass.game.loadedTextures.Add(texture2D);
			if (trait is TraitCanvas traitCanvas)
			{
				texture2D.filterMode = ((!traitCanvas.PointFilter) ? FilterMode.Bilinear : FilterMode.Point);
			}
		}
		return _paintSprite;
	}

	public void TryUnrestrain(bool force = false, Chara c = null)
	{
		if (!(GetRestrainer() == null || force))
		{
			return;
		}
		isRestrained = false;
		Say("unrestrained", this);
		if (this != c)
		{
			Talk("thanks");
		}
		MoveImmediate(pos.GetNearestPoint());
		renderer.SetFirst(first: true);
		if (c == null)
		{
			return;
		}
		if (c_rescueState == RescueState.WaitingForRescue)
		{
			c_rescueState = RescueState.Rescued;
			if (c.IsPC)
			{
				EClass.player.ModKarma(2);
			}
		}
		if (EClass._zone.IsPCFaction || EClass._zone is Zone_Tent)
		{
			return;
		}
		foreach (Chara item in c.pos.ListCharasInRadius(c, 5, (Chara _c) => _c.id == "fanatic" && _c.faith != Chara.faith))
		{
			c.DoHostileAction(item, immediate: true);
		}
	}

	public TraitShackle GetRestrainer()
	{
		foreach (Card item in pos.ListCards())
		{
			if (item.trait is TraitShackle && item.c_uidRefCard == uid)
			{
				return item.trait as TraitShackle;
			}
		}
		return null;
	}

	public virtual void Tick()
	{
	}

	public static int GetTilePrice(TileRow row, SourceMaterial.Row mat)
	{
		int result = 0;
		if (row.id == 0)
		{
			return result;
		}
		result = row.value * mat.value / 100;
		if (result < 0)
		{
			result = 1;
		}
		return result;
	}

	public Thing SetPriceFix(int a)
	{
		c_priceFix = a;
		return Thing;
	}

	public int GetEquipValue()
	{
		return GetValue();
	}

	public void SetSale(bool sale)
	{
		if (isSale != sale)
		{
			isSale = sale;
			if (isSale)
			{
				EClass._map.props.sales.Add(this);
			}
			else
			{
				EClass._map.props.sales.Remove(this);
			}
		}
	}

	public int GetValue(PriceType priceType = PriceType.Default, bool sell = false)
	{
		int num = ((c_fixedValue == 0) ? trait.GetValue() : c_fixedValue);
		if (id == "plat" && !sell)
		{
			num = 10000;
		}
		if (num == 0)
		{
			return 0;
		}
		float num2 = num;
		if (priceType == PriceType.CopyShop)
		{
			num2 += (float)c_priceCopy * 0.2f;
			num2 = num2 * (float)Mathf.Max(150 + rarityLv, 150) / 100f;
		}
		else
		{
			num2 = num2 * (float)Mathf.Max(100 + rarityLv + Mathf.Min(QualityLv * 10, 200), 80) / 100f;
		}
		if (IsFood && !material.tag.Contains("food"))
		{
			num2 *= 0.5f;
		}
		float num3;
		if (IsEquipmentOrRangedOrAmmo || trait is TraitMod)
		{
			if (sell)
			{
				num2 *= 0.3f;
			}
			num3 = 2f;
		}
		else
		{
			num3 = 0.5f;
		}
		if (isReplica)
		{
			num2 *= 0.15f;
		}
		if (!IsUnique)
		{
			if (IsEquipmentOrRanged && rarity >= Rarity.Legendary)
			{
				num2 = Mathf.Max(num2, 1800f + num2 / 5f);
			}
			num2 = num2 * (100f + num3 * (float)(material.value - 100)) / 100f;
			if (IsEquipmentOrRanged)
			{
				int num4 = 0;
				foreach (Element value in elements.dict.Values)
				{
					num4 += value.source.value;
				}
				num2 = num2 * (float)(100 + (sell ? ((int)MathF.Sqrt(num4) * 10) : num4)) / 100f;
				if (rarity >= Rarity.Legendary)
				{
					num2 = Mathf.Max(num2, 3600f + num2 / 5f);
				}
			}
		}
		if (trait is TraitRecipe && sell)
		{
			num2 *= 0.1f;
		}
		if (encLV != 0 && !category.tag.Contains("noEnc"))
		{
			num2 = (category.tag.Contains("enc") ? (num2 * (0.7f + (float)(encLV - 1) * 0.2f)) : ((!IsFood) ? (num2 * (1f + (float)encLV * 0.01f)) : ((!(id == "honey")) ? (num2 * Mathf.Min(1f + 0.1f * (float)encLV, 2f) + (float)(encLV * 100)) : (num2 + (float)(encLV * 10)))));
		}
		if (tier > 0)
		{
			num2 *= (float)(tier + 1);
		}
		return (int)num2;
	}

	public virtual int GetPrice(CurrencyType currency = CurrencyType.Money, bool sell = false, PriceType priceType = PriceType.Default, Chara c = null)
	{
		if (priceType == PriceType.CopyShop && sell)
		{
			return 0;
		}
		if (!sell)
		{
			if (id == "littleball")
			{
				return 0;
			}
			switch (currency)
			{
			case CurrencyType.Ecopo:
				switch (id)
				{
				case "plat":
					return 500;
				case "rune_free":
					return 1000;
				case "whip_egg":
					return 3000;
				case "hammer_strip":
					return 5000;
				case "helm_chef":
					return 25000;
				}
				break;
			case CurrencyType.Plat:
			{
				string text = id;
				if (!(text == "lucky_coin"))
				{
					if (!(text == "book_skill"))
					{
						break;
					}
					return 50;
				}
				return 100;
			}
			case CurrencyType.Medal:
				switch (id)
				{
				case "bill_tax":
					return 3;
				case "water":
					return 3;
				case "bill":
					return 3;
				case "1165":
					return 10;
				case "diary_sister":
					return 12;
				case "diary_lady":
					return 25;
				case "diary_catsister":
					return 85;
				case "container_magic":
					return 20;
				case "wrench_tent_elec":
					return 3;
				case "wrench_tent_soil":
					return 3;
				case "wrench_tent_seabed":
					return 12;
				case "wrench_bed":
					return 3;
				case "wrench_storage":
					return 4;
				case "wrench_fridge":
					return 15;
				case "wrench_extend_v":
					return 6;
				case "wrench_extend_h":
					return 6;
				case "monsterball":
					return LV / 8;
				case "spellbook":
					return 6;
				}
				break;
			}
		}
		if (sell && noSell)
		{
			return 0;
		}
		if (!sell && id == "casino_coin")
		{
			return 20;
		}
		int value = GetValue(priceType, sell);
		if (value == 0)
		{
			return 0;
		}
		if (c == null)
		{
			c = EClass.pc;
		}
		double p = value;
		Trait trait = this.trait;
		if (!(trait is TraitBed))
		{
			if (trait is TraitContainer traitContainer)
			{
				p *= 1f + 4f * (float)(things.width - traitContainer.Width) + 4f * (float)(things.height - traitContainer.Height);
			}
		}
		else
		{
			p *= 1f + 0.5f * (float)c_containerSize;
		}
		p += c_priceAdd;
		if (c_priceFix != 0)
		{
			p = (int)((float)p * (float)Mathf.Clamp(100 + c_priceFix, 0, 1000000) / 100f);
			if (p == 0.0)
			{
				return 0;
			}
		}
		if (isStolen)
		{
			if (sell && priceType == PriceType.PlayerShop && EClass.Branch != null && EClass.Branch.policies.IsActive(2824))
			{
				p = p * 100.0 / (double)Mathf.Max(110f, 170f - Mathf.Sqrt(EClass.Branch.Evalue(2824) * 5));
			}
			else if (sell && Guild.Thief.IsMember)
			{
				p = Guild.Thief.SellStolenPrice((int)p);
			}
			else
			{
				p *= 0.5;
			}
		}
		if (!sell && category.id == "spellbook")
		{
			p = Guild.Mage.BuySpellbookPrice((int)p);
		}
		bool flag = priceType == PriceType.CopyShop;
		int num = ((!flag) ? 1 : 5);
		float num2 = Mathf.Min(0.01f * (float)Evalue(752), 1f);
		float num3 = Mathf.Min(0.01f * (float)Evalue(751), 1f);
		float num4 = Mathf.Min(0.02f * (float)Evalue(759), 2f);
		if (num3 > 0f)
		{
			num3 *= (float)num;
		}
		float num5 = Mathf.Clamp(1f + num2 + num3, 0.5f, 5f) + num4;
		p *= num5;
		p *= 0.20000000298023224;
		if (sell)
		{
			p *= 0.20000000298023224;
			if (isCopy)
			{
				p *= 0.20000000298023224;
			}
			if (currency == CurrencyType.Money && (category.IsChildOf("meal") || category.IsChildOf("preserved")))
			{
				p *= 0.5;
			}
			if ((uint)(priceType - 1) <= 1u)
			{
				if (category.IsChildOf("fish"))
				{
					p *= ((EClass.pc.faith == EClass.game.religions.Luck) ? 1.5f : 1f);
				}
				if (category.IsChildOf("vegi") || category.IsChildOf("fruit"))
				{
					p *= ((EClass.pc.faith == EClass.game.religions.Harvest) ? 3f : 2f);
				}
			}
		}
		if (id == "rod_wish")
		{
			p *= (sell ? 0.01f : 50f);
		}
		switch (currency)
		{
		case CurrencyType.Ecopo:
			if (this.trait is TraitSeed)
			{
				p *= 2.0;
			}
			else if (this.trait is TraitEcoMark)
			{
				p *= 1.0;
			}
			else
			{
				p *= 0.20000000298023224;
			}
			break;
		case CurrencyType.Casino_coin:
			p *= 0.10000000149011612;
			break;
		case CurrencyType.Influence:
			p *= 0.0020000000949949026;
			break;
		case CurrencyType.Medal:
			p *= 0.00019999999494757503;
			break;
		case CurrencyType.Money2:
			p *= 0.004999999888241291;
			break;
		default:
			if (IsIdentified || (this.trait is TraitErohon && !sell) || priceType == PriceType.Tourism)
			{
				if (blessedState == BlessedState.Blessed)
				{
					p *= 1.25;
				}
				else if (blessedState <= BlessedState.Cursed)
				{
					p *= (flag ? 1.25f : 0.3f);
				}
				if (this.trait.HasCharges)
				{
					p = p * 0.05000000074505806 + p * (double)(0.5f + Mathf.Clamp(0.1f * (float)c_charges, 0f, 1.5f));
				}
				if (IsDecayed)
				{
					p *= (flag ? 0.9f : 0.5f);
				}
			}
			else
			{
				Rand.UseSeed(uid, delegate
				{
					p = (sell ? (1 + EClass.rnd(15)) : (50 + EClass.rnd(500)));
				});
			}
			if (!sell)
			{
				p *= 1f + 0.2f * (float)c.Evalue(1406);
			}
			break;
		}
		float num6 = Math.Clamp(Mathf.Sqrt(c.EvalueMax(291) + ((!sell && EClass._zone.IsPCFaction) ? (EClass.Branch.Evalue(2800) * 2) : 0)), 0f, 25f);
		switch (priceType)
		{
		case PriceType.Tourism:
			num6 = 0f;
			break;
		case PriceType.Shipping:
			if (sell)
			{
				p *= 1.100000023841858;
			}
			break;
		case PriceType.PlayerShop:
		{
			if (!sell)
			{
				break;
			}
			float num7 = 1.25f;
			if (EClass.Branch != null)
			{
				if (EClass.Branch.policies.IsActive(2817))
				{
					num7 += 0.1f + 0.01f * Mathf.Sqrt(EClass.Branch.Evalue(2817));
				}
				if (EClass.Branch.policies.IsActive(2816))
				{
					num7 += 0.2f + 0.02f * Mathf.Sqrt(EClass.Branch.Evalue(2816));
				}
				if (isChara)
				{
					if (EClass.Branch.policies.IsActive(2828))
					{
						num7 += 0.1f + 0.01f * Mathf.Sqrt(EClass.Branch.Evalue(2828));
					}
				}
				else if (category.IsChildOf("food") || category.IsChildOf("drink"))
				{
					if (EClass.Branch.policies.IsActive(2818))
					{
						num7 += 0.05f + 0.005f * Mathf.Sqrt(EClass.Branch.Evalue(2818));
					}
				}
				else if (category.IsChildOf("furniture"))
				{
					if (EClass.Branch.policies.IsActive(2819))
					{
						num7 += 0.05f + 0.005f * Mathf.Sqrt(EClass.Branch.Evalue(2819));
					}
				}
				else if (EClass.Branch.policies.IsActive(2820))
				{
					num7 += 0.05f + 0.005f * Mathf.Sqrt(EClass.Branch.Evalue(2820));
				}
			}
			p *= num7;
			break;
		}
		}
		if ((uint)currency > 1u)
		{
			num6 = 0f;
		}
		p *= (sell ? (1f + num6 * 0.02f) : (1f - num6 * 0.02f));
		if (sell)
		{
			p = EClass.curve((int)p, 10000, 10000, 80);
		}
		if (p < 1.0)
		{
			p = ((!sell) ? 1 : 0);
		}
		if (!sell)
		{
			if (currency == CurrencyType.Casino_coin)
			{
				if (p > 100000.0)
				{
					p = Mathf.CeilToInt((float)p / 100000f) * 100000;
				}
				else if (p > 10000.0)
				{
					p = Mathf.CeilToInt((float)p / 10000f) * 10000;
				}
				else if (p > 1000.0)
				{
					p = Mathf.CeilToInt((float)p / 1000f) * 1000;
				}
				else if (p > 100.0)
				{
					p = Mathf.CeilToInt((float)p / 100f) * 100;
				}
				else if (p > 10.0)
				{
					p = Mathf.CeilToInt((float)p / 10f) * 10;
				}
			}
			if (this.trait is TraitDeed)
			{
				p *= Mathf.Pow(2f, EClass.player.flags.landDeedBought);
			}
		}
		if (p > (double)(sell ? 500000000 : 1000000000))
		{
			if (!sell)
			{
				return 1000000000;
			}
			return 500000000;
		}
		return (int)p;
	}

	public virtual string GetHoverText()
	{
		return Name + GetExtraName();
	}

	public virtual string GetHoverText2()
	{
		return "";
	}

	public int Dist(Card c)
	{
		if (!IsMultisize && !c.IsMultisize)
		{
			return pos.Distance(c.pos);
		}
		if (IsMultisize)
		{
			int dist = 99;
			ForeachPoint(delegate(Point p, bool main)
			{
				int num2 = DistMulti(p, c);
				if (num2 < dist)
				{
					dist = num2;
				}
			});
			return dist;
		}
		return DistMulti(pos, c);
		static int DistMulti(Point p1, Card c)
		{
			if (!c.IsMultisize)
			{
				return p1.Distance(c.pos);
			}
			int dist2 = 99;
			c.ForeachPoint(delegate(Point p, bool main)
			{
				int num = p1.Distance(p);
				if (num < dist2)
				{
					dist2 = num;
				}
			});
			return dist2;
		}
	}

	public int Dist(Point p)
	{
		return pos.Distance(p);
	}

	public bool IsInMutterDistance(int d = 10)
	{
		return pos.Distance(EClass.pc.pos) < d;
	}

	public void SetCensored(bool enable)
	{
		isCensored = enable;
		if (EClass.core.config.other.noCensor)
		{
			isCensored = false;
		}
		renderer.SetCensored(isCensored);
	}

	public void SetDeconstruct(bool deconstruct)
	{
		if (isDeconstructing != deconstruct)
		{
			if (deconstruct)
			{
				EClass._map.props.deconstructing.Add(this);
			}
			else
			{
				EClass._map.props.deconstructing.Remove(this);
			}
			isDeconstructing = deconstruct;
		}
	}

	public virtual bool MatchEncSearch(string s)
	{
		return false;
	}

	public virtual void SetSortVal(UIList.SortMode m, CurrencyType currency = CurrencyType.Money)
	{
		switch (m)
		{
		case UIList.SortMode.ByEquip:
			sortVal = ((c_equippedSlot == 0) ? (category.sortVal * 1000) : 0);
			break;
		case UIList.SortMode.ByValue:
			sortVal = -GetPrice(currency) * 1000;
			break;
		case UIList.SortMode.ByNumber:
			sortVal = -Num * 1000;
			break;
		case UIList.SortMode.ByCategory:
			sortVal = category.sortVal * 1000;
			break;
		case UIList.SortMode.ByWeight:
			sortVal = -ChildrenAndSelfWeight * 1000;
			break;
		case UIList.SortMode.ByWeightSingle:
			sortVal = -ChildrenAndSelfWeightSingle * 1000;
			break;
		case UIList.SortMode.ByPrice:
			sortVal = -GetPrice(currency) * 1000;
			break;
		default:
			sortVal = sourceCard._index * 1000;
			break;
		}
	}

	public virtual int SecondaryCompare(UIList.SortMode m, Card c)
	{
		int num = 0;
		if (num == 0)
		{
			num = id.CompareTo(c.id);
		}
		if (num == 0)
		{
			num = trait.CompareTo(c);
		}
		if (num == 0)
		{
			num = Lang.comparer.Compare(GetName(NameStyle.Full, 1), c.GetName(NameStyle.Full, 1));
		}
		if (num == 0)
		{
			num = refVal - c.refVal;
		}
		if (num == 0)
		{
			num = c_charges - c.c_charges;
		}
		if (num == 0)
		{
			num = encLV - c.encLV;
		}
		if (num == 0)
		{
			num = Num - c.Num;
		}
		if (num == 0)
		{
			num = uid - c.uid;
		}
		return num;
	}

	public void ForeachFOV(Func<Point, bool> func)
	{
		if (fov == null)
		{
			return;
		}
		foreach (KeyValuePair<int, byte> lastPoint in fov.lastPoints)
		{
			Point arg = new Point().Set(lastPoint.Key);
			if (func(arg))
			{
				break;
			}
		}
	}

	public void ForeachPoint(Action<Point, bool> action)
	{
		if (IsMultisize)
		{
			pos.ForeachMultiSize(W, H, action);
		}
		else
		{
			action(pos, arg2: true);
		}
	}

	public void OnInspect()
	{
	}

	public virtual void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
	}

	public void Inspect()
	{
		SE.Play("pop_paper");
		if (isChara)
		{
			LayerChara layerChara = EClass.ui.AddLayerDontCloseOthers<LayerChara>();
			layerChara.windows[0].SetRect(EClass.core.refs.rects.center);
			layerChara.SetChara(Chara);
		}
		else
		{
			EClass.ui.AddLayerDontCloseOthers<LayerInfo>().SetThing(Thing);
		}
	}

	public virtual bool HasCondition<T>() where T : Condition
	{
		return false;
	}

	public bool HaveFur()
	{
		if (!isChara)
		{
			return false;
		}
		string text = id;
		if (text == "putty_snow" || text == "putty_snow_gold")
		{
			return true;
		}
		return !Chara.race.fur.IsEmpty();
	}

	public bool CanBeSheared()
	{
		if (EClass._zone.IsUserZone)
		{
			return false;
		}
		if (!HaveFur() || c_fur < 0)
		{
			return false;
		}
		return true;
	}
}
