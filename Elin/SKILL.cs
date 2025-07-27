public class SKILL
{
	public const int fireproof = 50;

	public const int weaponCrossbow = 109;

	public const int weaponScythe = 110;

	public const int weaponBlunt = 111;

	public const int armorLight = 120;

	public const int armorHeavy = 122;

	public const int shield = 123;

	public const int twohand = 130;

	public const int marksman = 133;

	public const int fishing = 245;

	public const int climbing = 242;

	public const int music = 241;

	public const int travel = 240;

	public const int taming = 237;

	public const int milking = 235;

	public const int digging = 230;

	public const int parasite = 227;

	public const int riding = 226;

	public const int lumberjack = 225;

	public const int mining = 220;

	public const int weightlifting = 207;

	public const int swimming = 200;

	public const int stealth = 152;

	public const int evasionPlus = 151;

	public const int evasion = 150;

	public const int strategy = 135;

	public const int eyeofmind = 134;

	public const int twowield = 131;

	public const int spotting = 210;

	public const int weaponAxe = 102;

	public const int weaponDagger = 107;

	public const int END = 71;

	public const int STR = 70;

	public const int FPV = 68;

	public const int DMG = 67;

	public const int HIT = 66;

	public const int PV = 65;

	public const int DEX = 72;

	public const int DV = 64;

	public const int mana = 61;

	public const int life = 60;

	public const int evasionPerfect = 57;

	public const int EDR = 56;

	public const int PDR = 55;

	public const int acidproof = 51;

	public const int vigor = 62;

	public const int PER = 73;

	public const int LER = 74;

	public const int WIL = 75;

	public const int weaponPolearm = 106;

	public const int weaponGun = 105;

	public const int weaponBow = 104;

	public const int weaponStaff = 103;

	public const int weaponSword = 101;

	public const int martial = 100;

	public const int antiMagic = 93;

	public const int penetration = 92;

	public const int vopal = 91;

	public const int critical = 90;

	public const int INT = 80;

	public const int SPD = 79;

	public const int LUC = 78;

	public const int CHA = 77;

	public const int MAG = 76;

	public const int throwing = 108;

	public const int gathering = 250;

	public const int tactics = 132;

	public const int blacksmith = 256;

	public const int eleNether = 916;

	public const int eleSound = 917;

	public const int eleNerve = 918;

	public const int eleHoly = 919;

	public const int eleChaos = 920;

	public const int eleMagic = 921;

	public const int eleEther = 922;

	public const int eleAcid = 923;

	public const int eleCut = 924;

	public const int eleImpact = 925;

	public const int eleVoid = 926;

	public const int resFire = 950;

	public const int resCold = 951;

	public const int resLightning = 952;

	public const int elePoison = 915;

	public const int resDarkness = 953;

	public const int resNether = 956;

	public const int resSound = 957;

	public const int resNerve = 958;

	public const int resChaos = 959;

	public const int resHoly = 960;

	public const int resMagic = 961;

	public const int resEther = 962;

	public const int resAcid = 963;

	public const int resCut = 964;

	public const int resImpact = 965;

	public const int resDecay = 970;

	public const int resDamage = 971;

	public const int resCurse = 972;

	public const int carpentry = 255;

	public const int resMind = 954;

	public const int eleMind = 914;

	public const int resPoison = 955;

	public const int eleLightning = 912;

	public const int eleDarkness = 913;

	public const int alchemy = 257;

	public const int sculpture = 258;

	public const int jewelry = 259;

	public const int weaving = 260;

	public const int handicraft = 261;

	public const int lockpicking = 280;

	public const int stealing = 281;

	public const int reading = 285;

	public const int farming = 286;

	public const int cooking = 287;

	public const int building = 288;

	public const int anatomy = 290;

	public const int negotiation = 291;

	public const int investing = 292;

	public const int appraising = 289;

	public const int fun = 312;

	public const int eleFire = 910;

	public const int env = 313;

	public const int disarmTrap = 293;

	public const int bladder = 311;

	public const int hygine = 310;

	public const int eleCold = 911;

	public const int faith = 306;

	public const int memorization = 307;

	public const int casting = 304;

	public const int manaCapacity = 303;

	public const int controlmana = 302;

	public const int meditation = 301;

	public const int regeneration = 300;

	public const int magicDevice = 305;

	public static readonly int[] IDS = new int[133]
	{
		50, 109, 110, 111, 120, 122, 123, 130, 133, 245,
		242, 241, 240, 237, 235, 230, 227, 226, 225, 220,
		207, 200, 152, 151, 150, 135, 134, 131, 210, 102,
		107, 71, 70, 68, 67, 66, 65, 72, 64, 61,
		60, 57, 56, 55, 51, 62, 73, 74, 75, 106,
		105, 104, 103, 101, 100, 93, 92, 91, 90, 80,
		79, 78, 77, 76, 108, 250, 132, 256, 916, 917,
		918, 919, 920, 921, 922, 923, 924, 925, 926, 950,
		951, 952, 915, 953, 956, 957, 958, 959, 960, 961,
		962, 963, 964, 965, 970, 971, 972, 255, 954, 914,
		955, 912, 913, 257, 258, 259, 260, 261, 280, 281,
		285, 286, 287, 288, 290, 291, 292, 289, 312, 910,
		313, 293, 311, 310, 911, 306, 307, 304, 303, 302,
		301, 300, 305
	};
}
public class Skill : Element
{
	public override bool ShowEncNumber => false;

	public override bool ShowRelativeAttribute => true;

	public override bool CanLink(ElementContainer owner)
	{
		if (!base.IsGlobalElement)
		{
			return !base.source.IsWeaponEnc;
		}
		return false;
	}

	public override int GetSourcePotential(int v)
	{
		if (v > 1)
		{
			return v * 10;
		}
		return 0;
	}

	public override void OnChangeValue()
	{
		Chara chara = owner.Chara;
		switch (id)
		{
		case 207:
			chara?.SetDirtyWeight();
			break;
		case 306:
			chara?.RefreshFaithElement();
			break;
		}
	}
}
