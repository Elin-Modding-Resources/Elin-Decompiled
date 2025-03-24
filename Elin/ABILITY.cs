using UnityEngine;

public class ABILITY
{
	public const int ActChat = 5044;

	public const int AI_Fish = 5039;

	public const int ActRanged = 5040;

	public const int AI_Read = 5041;

	public const int ActKick = 5042;

	public const int AI_TendAnimal = 5043;

	public const int AI_Drink = 5045;

	public const int ActInstall = 5046;

	public const int ActPick = 5047;

	public const int ActItem = 5048;

	public const int AI_OpenLock = 5049;

	public const int AI_Sleep = 5050;

	public const int ActZap = 5051;

	public const int ActBash = 5052;

	public const int TaskHarvest = 5053;

	public const int ActThrow = 5038;

	public const int AI_Bladder = 5054;

	public const int AI_PlayMusic = 6001;

	public const int AI_Meditate = 6003;

	public const int AI_Steal = 6011;

	public const int ActQuickCraft = 6012;

	public const int AI_PassTime = 6013;

	public const int AI_SelfHarm = 6015;

	public const int ActRide = 6018;

	public const int ActParasite = 6019;

	public const int ActDreamBug = 6020;

	public const int ActPray = 6050;

	public const int ActEscape = 6400;

	public const int ActSuicide = 6410;

	public const int ActRush = 6450;

	public const int ActCrabBreathe = 6500;

	public const int ActRestrain = 5055;

	public const int ActCurse = 6600;

	public const int ActNoItem = 5037;

	public const int TaskBuild = 5035;

	public const int Wait = 5005;

	public const int Shoot = 5006;

	public const int Use = 5007;

	public const int General = 5008;

	public const int TaskMine = 5009;

	public const int TaskDig = 5010;

	public const int TaskCut = 5011;

	public const int AI_Goto = 5012;

	public const int ActAttack = 5013;

	public const int TaskPlow = 5014;

	public const int TaskAttack = 5015;

	public const int TaskTame = 5016;

	public const int TaskTalk = 5017;

	public const int TaskPick = 5018;

	public const int TaskMoveInstalled = 5036;

	public const int TaskReadBoard = 5019;

	public const int TaskTrain = 5021;

	public const int TaskSleepOnBed = 5022;

	public const int TaskGoOut = 5023;

	public const int AI_Eat = 5024;

	public const int AI_Grab = 5025;

	public const int AI_Haul = 5026;

	public const int GoalSleep = 5027;

	public const int ActReleaseHeld = 5028;

	public const int AI_Offer = 5029;

	public const int AI_ReleaseHeld = 5030;

	public const int AI_Deconstruct = 5031;

	public const int AI_Equip = 5032;

	public const int ActCancelTask = 5033;

	public const int GoalIdle = 5034;

	public const int TaskFarm = 5020;

	public const int ActEntangle = 6601;

	public const int ActDuplicate = 6420;

	public const int ActNTR = 6603;

	public const int breathe_ = 7000;

	public const int breathe_Fire = 50200;

	public const int breathe_Cold = 50201;

	public const int breathe_Lightning = 50202;

	public const int breathe_Darkness = 50203;

	public const int breathe_Mind = 50204;

	public const int breathe_Poison = 50205;

	public const int breathe_Nether = 50206;

	public const int breathe_Sound = 50207;

	public const int ActDraw = 6602;

	public const int breathe_Holy = 50209;

	public const int breathe_Chaos = 50210;

	public const int breathe_Magic = 50211;

	public const int breathe_Ether = 50212;

	public const int breathe_Acid = 50213;

	public const int breathe_Cut = 50214;

	public const int breathe_Impact = 50215;

	public const int breathe_Void = 50216;

	public const int ActWait = 5000;

	public const int ActMelee = 5001;

	public const int Melee = 5002;

	public const int Ranged = 5003;

	public const int Sleep = 5004;

	public const int ActHeadpat = 6904;

	public const int ActKizuamiTrick = 6903;

	public const int breathe_Nerve = 50208;

	public const int ActJureHeal = 6901;

	public const int ActTouchSleep = 6612;

	public const int ActFear = 6611;

	public const int ActWeaken = 6610;

	public const int ActLulwyTrick = 6902;

	public const int ActTouchDrown = 6613;

	public const int ActGazeDim = 6620;

	public const int ActGazeInsane = 6621;

	public const int ActGazeMutation = 6622;

	public const int ActDrainBlood = 6626;

	public const int ActInsult = 6630;

	public const int ActScream = 6631;

	public const int ActGazeMana = 6623;

	public const int ActStealFood = 6641;

	public const int ActStealMoney = 6642;

	public const int ActNeckHunt = 6650;

	public const int ActDropMine = 6660;

	public const int ActThrowPotion = 6661;

	public const int ActSwarm = 6662;

	public const int ActManaAbsorb = 6900;

	public const int StManaCost = 6720;

	public const int ActSteal = 6640;

	public const int StTaunt = 6700;

	public static readonly int[] IDS = new int[118]
	{
		5044, 5039, 5040, 5041, 5042, 5043, 5045, 5046, 5047, 5048,
		5049, 5050, 5051, 5052, 5053, 5038, 5054, 6001, 6003, 6011,
		6012, 6013, 6015, 6018, 6019, 6020, 6050, 6400, 6410, 6450,
		6500, 5055, 6600, 5037, 5035, 5005, 5006, 5007, 5008, 5009,
		5010, 5011, 5012, 5013, 5014, 5015, 5016, 5017, 5018, 5036,
		5019, 5021, 5022, 5023, 5024, 5025, 5026, 5027, 5028, 5029,
		5030, 5031, 5032, 5033, 5034, 5020, 6601, 6420, 6603, 7000,
		50200, 50201, 50202, 50203, 50204, 50205, 50206, 50207, 6602, 50209,
		50210, 50211, 50212, 50213, 50214, 50215, 50216, 5000, 5001, 5002,
		5003, 5004, 6904, 6903, 50208, 6901, 6612, 6611, 6610, 6902,
		6613, 6620, 6621, 6622, 6626, 6630, 6631, 6623, 6641, 6642,
		6650, 6660, 6661, 6662, 6900, 6720, 6640, 6700
	};
}
public class Ability : Act
{
	public override bool ShowBonuses => false;

	public override bool CanPressRepeat => base.source.tag.Contains("repeat");

	public override bool CanLink(ElementContainer owner)
	{
		if (owner.Card == null)
		{
			return !base.IsGlobalElement;
		}
		return false;
	}

	public override int GetSourceValue(int v, int lv, SourceValueType type)
	{
		if (type != 0)
		{
			return base.GetSourceValue(v, lv, type);
		}
		return 10 * (100 + (lv - 1) * base.source.lvFactor / 10) / 100;
	}

	public override int GetPower(Card c)
	{
		int a = base.Value * 8 + 50;
		if (!c.IsPC)
		{
			a = Mathf.Max(a, c.LV * 6 + 30);
			if (c.IsPCFactionOrMinion && !base.source.aliasParent.IsEmpty())
			{
				a = Mathf.Max(a, c.Evalue(base.source.aliasParent) * 4 + 30);
			}
		}
		a = EClass.curve(a, 400, 100);
		if (this is Spell)
		{
			a = a * Mathf.Max(100 + c.Evalue(411) - c.Evalue(93), 1) / 100;
		}
		return a;
	}

	public override void OnChangeValue()
	{
		Card card = owner.Card;
		if (card != null && card._IsPC)
		{
			LayerAbility.SetDirty(this);
		}
	}
}
