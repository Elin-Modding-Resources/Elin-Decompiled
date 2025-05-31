using System.Collections.Generic;
using UnityEngine;

public class FEAT
{
	public const int featElea = 1213;

	public const int featResCurse = 1641;

	public const int featAnimalLover = 1640;

	public const int featFaith = 1636;

	public const int featMartial = 1635;

	public const int featNegotiate = 1634;

	public const int featSpotting = 1633;

	public const int featEvade = 1632;

	public const int featDefense = 1631;

	public const int featHardy = 1630;

	public const int featSPD = 1629;

	public const int featLuck = 1628;

	public const int featCHA = 1627;

	public const int featWIL = 1626;

	public const int featMAG = 1625;

	public const int featLER = 1624;

	public const int featPER = 1623;

	public const int featEND = 1622;

	public const int featDEX = 1621;

	public const int featSTR = 1620;

	public const int featStamina = 1612;

	public const int featMana = 1611;

	public const int featLife = 1610;

	public const int featHealer = 1422;

	public const int featManaMeat = 1421;

	public const int featExecutioner = 1420;

	public const int featSleeper = 1642;

	public const int featSorter = 1643;

	public const int featBodyParts = 1644;

	public const int featParty = 1645;

	public const int featFairyResist = 1212;

	public const int featSlowFood = 1200;

	public const int featManaBond = 1201;

	public const int featFastLearner = 1202;

	public const int featGrowParts = 1203;

	public const int featFairyWeak = 1204;

	public const int featCannibalism = 1205;

	public const int featMelilithCurse = 1206;

	public const int featFoxBless = 1207;

	public const int featFoxLearn = 1208;

	public const int featFluffyTail = 1209;

	public const int featUndead = 1210;

	public const int featMilitant = 1419;

	public const int featSnail = 1211;

	public const int featManaCost = 1657;

	public const int featScavenger = 1656;

	public const int featModelBeliever = 1655;

	public const int featHeavyCasting = 1654;

	public const int featDreamWaker = 1653;

	public const int featRapidArrow = 1652;

	public const int featMagicManner = 1651;

	public const int featGourmet = 1650;

	public const int featDefender = 1649;

	public const int featRapidMagic = 1648;

	public const int featSummoner = 1647;

	public const int featLonelySoul = 1646;

	public const int featChef = 1658;

	public const int featSwordsage = 1418;

	public const int featFisher = 1659;

	public const int featInquisitor = 1416;

	public const int featElder = 1238;

	public const int featDemon = 1239;

	public const int featMassSummoner = 1240;

	public const int featGod_element1 = 1300;

	public const int featGod_earth1 = 1305;

	public const int featGod_wind1 = 1310;

	public const int featGod_machine1 = 1315;

	public const int featGod_harvest1 = 1325;

	public const int featGod_luck1 = 1330;

	public const int featRoran = 1237;

	public const int featGod_harmony1 = 1335;

	public const int featGod_trickery1 = 1345;

	public const int featGod_moonshadow1 = 1350;

	public const int featGod_strife1 = 1355;

	public const int featWarrior = 1400;

	public const int featThief = 1401;

	public const int featWizard = 1402;

	public const int featFarmer = 1403;

	public const int featArcher = 1404;

	public const int featPianist = 1405;

	public const int featGod_oblivion1 = 1340;

	public const int featNorland = 1236;

	public const int featLightEater = 1235;

	public const int featHeavyEater = 1234;

	public const int featWitch = 1417;

	public const int featDwarf = 1215;

	public const int featSuccubus = 1216;

	public const int featGolem = 1217;

	public const int featMetal = 1218;

	public const int featElderCrab = 1219;

	public const int featFate = 1220;

	public const int featSpike = 1221;

	public const int featSplit = 1222;

	public const int featAcidBody = 1223;

	public const int featShiva = 1224;

	public const int featLoyal = 1225;

	public const int featUnderground = 1226;

	public const int featManaPrecision = 1214;

	public const int featServant = 1227;

	public const int featDemigod = 1228;

	public const int featLittleOne = 1229;

	public const int featAdam = 1230;

	public const int featNirvana = 1231;

	public const int featBaby = 1232;

	public const int featCosmicHorror = 1233;

	public const int featTourist = 1406;

	public const int featPaladin = 1407;

	public const int featGod_healing1 = 1320;

	public const int featBoost = 1409;

	public const int featReboot = 1410;

	public const int featEarthStrength = 1411;

	public const int featLuckyCat = 1412;

	public const int featFoxMaid = 1415;

	public const int featPaladin2 = 1408;

	public const int featFairysan = 1413;

	public const int featWhiteVixen = 1414;

	public static readonly int[] IDS = new int[116]
	{
		1213, 1641, 1640, 1636, 1635, 1634, 1633, 1632, 1631, 1630,
		1629, 1628, 1627, 1626, 1625, 1624, 1623, 1622, 1621, 1620,
		1612, 1611, 1610, 1422, 1421, 1420, 1642, 1643, 1644, 1645,
		1212, 1200, 1201, 1202, 1203, 1204, 1205, 1206, 1207, 1208,
		1209, 1210, 1419, 1211, 1657, 1656, 1655, 1654, 1653, 1652,
		1651, 1650, 1649, 1648, 1647, 1646, 1658, 1418, 1659, 1416,
		1238, 1239, 1240, 1300, 1305, 1310, 1315, 1325, 1330, 1237,
		1335, 1345, 1350, 1355, 1400, 1401, 1402, 1403, 1404, 1405,
		1340, 1236, 1235, 1234, 1417, 1215, 1216, 1217, 1218, 1219,
		1220, 1221, 1222, 1223, 1224, 1225, 1226, 1214, 1227, 1228,
		1229, 1230, 1231, 1232, 1233, 1406, 1407, 1320, 1409, 1410,
		1411, 1412, 1415, 1408, 1413, 1414
	};
}
public class Feat : Element
{
	public static List<string> hints = new List<string>();

	public static string[] featRef = new string[5];

	public override bool ShowBonuses => false;

	public override string Name => base.source.GetText().SplitNewline().TryGet(base.Value - 1);

	public override string FullName
	{
		get
		{
			string[] array = base.source.GetText().SplitNewline();
			return array.TryGet(base.Value - 1) + ((base.source.max > 1 && array.Length == 1 && base.Value > 1) ? (" " + base.Value) : "");
		}
	}

	public override bool ShowXP => false;

	public override bool ShowValue => false;

	public override int CostLearn => base.source.cost.TryGet(base.Value - 1);

	public override bool CanLink(ElementContainer owner)
	{
		return !base.IsGlobalElement;
	}

	public override Sprite GetIcon(string suffix = "")
	{
		return SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", "ele_Feat");
	}

	public override void OnWriteNote(UINote n, ElementContainer owner)
	{
		Apply(base.Value, owner, hint: true);
		foreach (string hint in hints)
		{
			n.AddText("_bullet".lang() + hint);
		}
	}

	public string GetHint(ElementContainer owner)
	{
		string text = "";
		Apply(base.Value, owner, hint: true);
		foreach (string hint in hints)
		{
			text = text + hint.StripLastPun() + ", ";
		}
		return text.TrimEnd(' ').TrimEnd(',');
	}

	public override bool IsPurchaseFeatReqMet(ElementContainer owner, int lv = -1)
	{
		if (base.source.req.Length != 0)
		{
			Element element = owner.GetElement(base.source.req[0]);
			if (element == null || element.ValueWithoutLink < ((base.source.req.Length == 1) ? 1 : base.source.req[Mathf.Clamp(((lv == -1 && owner != null) ? owner.Value(id) : lv) + 1, 1, base.source.req.Length - 1)].ToInt()))
			{
				return false;
			}
		}
		return true;
	}

	public override void WritePurchaseReq(UINote n, int lv = -1)
	{
		if (base.source.req.Length != 0)
		{
			Element element = Element.Create(base.source.req[0], (base.source.req.Length == 1) ? 1 : base.source.req[Mathf.Clamp(((lv == -1 && owner != null) ? owner.Value(id) : lv) + 1, 1, base.source.req.Length - 1)].ToInt());
			if (element != null)
			{
				n.AddHeader("featReq");
				n.AddText(element.Name + ": " + element.Value, FontColor.Bad);
			}
		}
	}

	public List<string> Apply(int a, ElementContainer owner, bool hint = false)
	{
		if (hint)
		{
			hints.Clear();
		}
		int value = base.Value;
		int A = Mathf.Abs(a);
		int invert = ((a >= 0) ? 1 : (-1));
		if (hint)
		{
			featRef[0] = a.ToString() ?? "";
			featRef[1] = a.ToString() ?? "";
		}
		Chara chara = owner.Chara;
		int num = vPotential;
		if (!hint && a > 0 && chara != null)
		{
			num = (vPotential = chara.LV);
		}
		switch (id)
		{
		case 1206:
			ModBase(78, -a * 10, hide: false);
			break;
		case 1642:
			featRef[0] = ((a == 1) ? 10 : ((a == 2) ? 20 : 30)).ToString() ?? "";
			break;
		case 1643:
			featRef[0] = ((a == 1) ? 1 : ((a == 2) ? 3 : 5)).ToString() ?? "";
			break;
		case 1610:
			ModBase(60, a * 4, hide: false);
			break;
		case 1611:
			ModBase(61, a * 4, hide: false);
			break;
		case 1612:
			ModBase(62, a * 4, hide: false);
			break;
		case 1620:
			ModAttribute(70);
			break;
		case 1621:
			ModAttribute(72);
			break;
		case 1622:
			ModAttribute(71);
			break;
		case 1623:
			ModAttribute(73);
			break;
		case 1626:
			ModAttribute(75);
			break;
		case 1625:
			ModAttribute(76);
			break;
		case 1627:
			ModAttribute(77);
			break;
		case 1624:
			ModAttribute(74);
			break;
		case 1629:
			ModBase(79, a * 5, hide: false);
			break;
		case 1630:
			ModBase(65, ((A == 1) ? 2 : ((A == 2) ? 5 : 10)) * invert, hide: false);
			break;
		case 1631:
			ModBase(64, ((A == 1) ? 2 : ((A == 2) ? 5 : 10)) * invert, hide: false);
			break;
		case 1632:
			ModBase(150, a * 2, hide: false);
			break;
		case 1633:
			ModBase(210, a * 3, hide: false);
			ModBase(402, ((A == 3) ? 1 : 0) * invert, hide: false);
			break;
		case 1634:
			ModBase(291, a * 4, hide: false);
			break;
		case 1635:
			ModBase(100, a * 3, hide: false);
			break;
		case 1636:
			ModBase(306, a * 4, hide: false);
			break;
		case 1628:
			ModBase(78, a * 2, hide: false);
			break;
		case 1640:
			ModBase(77, a * 2, hide: false);
			break;
		case 1400:
			featRef[0] = (a * 10).ToString() ?? "";
			break;
		case 1404:
			featRef[0] = (a * 10).ToString() ?? "";
			break;
		case 1401:
			ModBase(78, a * 15, hide: false);
			break;
		case 1406:
			featRef[0] = (a * 20).ToString() ?? "";
			featRef[1] = (a * 5).ToString() ?? "";
			break;
		case 1644:
			featRef[0] = a.ToString() ?? "";
			if (!hint)
			{
				if (a > 0)
				{
					owner.Chara.AddRandomBodyPart(owner.Chara.IsPC);
				}
				else
				{
					_ = a;
					_ = 0;
				}
				if (owner.Chara.IsPC && (bool)WidgetEquip.Instance)
				{
					WidgetEquip.Instance.Rebuild();
				}
			}
			ModBase(60, (A > 5) ? ((A - 5) * -3 * invert) : 0, hide: false);
			ModBase(79, (A > 5) ? ((A - 5) * -5 * invert) : 0, hide: false);
			ModBase(77, (A > 5) ? ((A - 5) * -3 * invert) : 0, hide: false);
			break;
		case 1230:
			ModBase(60, A * 2 * invert, hide: false);
			ModBase(79, A * 3 * invert, hide: false);
			break;
		case 1204:
			ModBase(64, a * 50, hide: true);
			ModBase(401, a, hide: false);
			break;
		case 1202:
			ModBase(403, a * 20, hide: true);
			break;
		case 1210:
			ModBase(955, a * 5, hide: false);
			ModBase(953, a * 10, hide: false);
			ModBase(958, a * 5, hide: false);
			ModBase(956, a * 10, hide: false);
			ModBase(950, a * -5, hide: false);
			break;
		case 1239:
			ModBase(956, a * 5, hide: false);
			ModBase(959, a * 5, hide: false);
			ModBase(423, a, hide: false);
			ModBase(424, a, hide: false);
			ModBase(421, a, hide: false);
			break;
		case 1233:
			ModBase(954, a * 10, hide: false);
			ModBase(423, a, hide: false);
			ModBase(425, a, hide: false);
			ModBase(424, a, hide: false);
			ModBase(421, a, hide: false);
			break;
		case 1226:
			ModBase(955, a * 5, hide: false);
			ModBase(953, a * 5, hide: false);
			break;
		case 1212:
			ModBase(961, a * 5, hide: false);
			ModBase(953, a * 5, hide: false);
			ModBase(958, a * 5, hide: false);
			ModBase(956, a * 5, hide: false);
			ModBase(954, a * 5, hide: false);
			ModBase(957, a * 5, hide: false);
			ModBase(959, a * 5, hide: false);
			break;
		case 1213:
			ModBase(962, a * 20, hide: false);
			break;
		case 1224:
			ModBase(951, a * 15, hide: false);
			break;
		case 1217:
			ModBase(952, a * -10, hide: false);
			ModBase(955, a * 20, hide: false);
			ModBase(964, a * 20, hide: false);
			break;
		case 1218:
			ModBase(950, a / 40, hide: false);
			ModBase(952, a / 40, hide: false);
			ModBase(955, a / 40, hide: false);
			ModBase(953, a / 40, hide: false);
			ModBase(958, a / 40, hide: false);
			ModBase(956, a / 40, hide: false);
			ModBase(954, a / 40, hide: false);
			ModBase(959, a / 40, hide: false);
			ModBase(964, a / 40, hide: false);
			ModBase(961, a / 40, hide: false);
			break;
		case 1221:
			ModBase(964, a * 20, hide: false);
			break;
		case 1223:
			ModBase(963, a * 20, hide: false);
			break;
		case 1216:
			ModBase(6020, a, hide: false);
			break;
		case 1415:
			ModBase(60, a * 15, hide: false);
			ModBase(79, a * 5, hide: false);
			ModBase(953, a * 3, hide: false);
			ModBase(961, a, hide: false);
			ModBase(960, a, hide: false);
			break;
		case 1419:
			ModPotential(101, a * 50);
			ModPotential(111, a * 50);
			ModPotential(103, a * 50);
			ModPotential(106, a * 50);
			ModPotential(122, a * 50);
			ModPotential(120, a * 50);
			ModPotential(123, a * 50);
			break;
		case 1532:
			ModBase(73, -a * 5, hide: false);
			break;
		case 1533:
			ModBase(73, a * 5, hide: false);
			break;
		case 1510:
			ModBase(65, a * 3, hide: false);
			break;
		case 1511:
			ModBase(65, -a * 3, hide: false);
			break;
		case 1512:
			ModBase(73, a * 5, hide: false);
			break;
		case 1513:
			ModBase(73, -a * 5, hide: false);
			break;
		case 1514:
			ModBase(72, a * 3, hide: false);
			break;
		case 1515:
			ModBase(72, -a * 3, hide: false);
			break;
		case 1516:
			ModBase(300, a * 4, hide: false);
			break;
		case 1517:
			ModBase(300, -a * 4, hide: false);
			break;
		case 1518:
			ModBase(79, a * 5, hide: false);
			break;
		case 1519:
			ModBase(79, -a * 5, hide: false);
			break;
		case 1520:
			ModBase(70, a * 3, hide: false);
			break;
		case 1521:
			ModBase(70, -a * 3, hide: false);
			break;
		case 1522:
			ModBase(77, a * 5, hide: false);
			break;
		case 1523:
			ModBase(77, -a * 5, hide: false);
			break;
		case 1524:
			ModBase(307, a * 4, hide: false);
			break;
		case 1525:
			ModBase(307, -a * 4, hide: false);
			break;
		case 1526:
			ModBase(951, a * 5, hide: false);
			ModBase(950, -a * 5, hide: false);
			break;
		case 1527:
			ModBase(951, -a * 5, hide: false);
			ModBase(950, a * 5, hide: false);
			break;
		case 1528:
			ModBase(952, a * 5, hide: false);
			break;
		case 1529:
			ModBase(952, -a * 5, hide: false);
			break;
		case 1530:
			ModBase(960, a * 5, hide: false);
			ModBase(956, -a * 5, hide: false);
			break;
		case 1531:
			ModBase(960, -a * 5, hide: false);
			ModBase(956, a * 5, hide: false);
			break;
		case 1300:
		case 1305:
		case 1310:
		case 1315:
		case 1320:
		case 1325:
		case 1330:
		case 1335:
		case 1340:
		case 1345:
		case 1350:
		case 1355:
			GodHint();
			break;
		case 1550:
			ModBase(404, -a * 10, hide: false);
			break;
		case 1551:
			ModBase(77, -a * (4 + num / 5), hide: false);
			break;
		case 1552:
			ModBase(79, a * Mathf.Min(30 + num / 5, 100), hide: false);
			if (!hint && a > 0)
			{
				chara?.body.UnequipAll(39);
			}
			break;
		case 1553:
			ModBase(73, a * (5 + num / 3), hide: false);
			ModBase(77, -a * (5 + num / 3), hide: false);
			break;
		case 1554:
			ModBase(404, a * 10, hide: false);
			ModBase(401, a, hide: false);
			ModBase(79, a * (10 + num / 5), hide: false);
			if (!hint && a > 0)
			{
				chara?.body.UnequipAll(33);
			}
			break;
		case 1555:
			ModBase(65, a * (12 + num), hide: false);
			ModBase(77, -a * (5 + num / 5), hide: false);
			if (!hint && a > 0)
			{
				chara?.body.UnequipAll(31);
			}
			break;
		case 1556:
			ModBase(64, -a * (10 + num), hide: false);
			ModBase(67, a * (5 + num / 2), hide: false);
			break;
		case 1557:
			ModBase(71, -a * (5 + num / 3), hide: false);
			ModBase(72, -a * (4 + num / 4), hide: false);
			ModBase(74, a * (6 + num / 2), hide: false);
			ModBase(75, a * (2 + num / 6), hide: false);
			break;
		case 1560:
			ModBase(60, -a * 15, hide: false);
			ModBase(70, -a * (4 + num / 3), hide: false);
			break;
		case 1561:
			ModBase(61, -a * 15, hide: false);
			ModBase(76, -a * (4 + num / 3), hide: false);
			break;
		case 1562:
			ModBase(65, a * (15 + num / 2), hide: false);
			ModBase(79, -a * (10 + num / 5), hide: false);
			break;
		case 1563:
			ModBase(77, -a * (3 + num / 4), hide: false);
			break;
		case 1564:
			ModBase(961, a * 5, hide: false);
			break;
		case 1565:
			ModBase(955, a * 20, hide: false);
			break;
		}
		if (hint)
		{
			string text = base.source.GetText("textExtra");
			if (!text.IsEmpty())
			{
				string text2 = text.SplitNewline().TryGet(value - 1, 99);
				if (!text2.IsEmpty())
				{
					int num2 = 0;
					string[] array = text2.Split(',');
					for (int i = 0; i < array.Length; i++)
					{
						string item = array[i].Replace("#1", featRef[num2]);
						hints.Add(item);
						num2++;
					}
				}
			}
		}
		return hints;
		void GodHint()
		{
			if (!hint)
			{
				return;
			}
			foreach (Element value2 in owner.Card.Chara.faithElements.dict.Values)
			{
				if (value2.source.id != id)
				{
					NoteElement(value2.id, value2.Value);
				}
			}
		}
		void ModAttribute(int ele)
		{
			if (!hint)
			{
				Debug.Log(A);
				Debug.Log(((A == 1) ? 2 : ((A == 2) ? 4 : 5)) * invert);
			}
			ModBase(ele, ((A == 1) ? 2 : ((A == 2) ? 4 : 5)) * invert, hide: false);
			ModPotential(ele, a * 10);
		}
		void ModBase(int ele, int _v, bool hide)
		{
			if (!hint)
			{
				owner.ModBase(ele, _v);
			}
			if (!hide && _v != 0)
			{
				NoteElement(ele, _v);
			}
		}
		void ModPotential(int ele, int _v)
		{
			if (!hint)
			{
				owner.ModPotential(ele, _v);
			}
			Note("modPotential".lang(EClass.sources.elements.map[ele].GetName(), "+" + _v + "%"));
		}
		void Note(string s)
		{
			if (hint)
			{
				hints.Add(s);
			}
		}
		void NoteElement(int ele, int a)
		{
			SourceElement.Row row = EClass.sources.elements.map[ele];
			if (row.category == "ability")
			{
				Note("hintLearnAbility".lang(row.GetName().ToTitleCase()));
			}
			else if (row.tag.Contains("flag"))
			{
				Note(row.GetName());
			}
			else
			{
				string @ref = ((a < 0) ? "" : "+") + a;
				if (row.category == "resist")
				{
					int num3 = 0;
					@ref = ((a > 0) ? "+" : "-").Repeat(Mathf.Clamp(Mathf.Abs(a) / 5 + num3, 1, 5));
					Note("modValueRes".lang(row.GetName(), @ref));
				}
				else
				{
					Note("modValue".lang(row.GetName(), @ref));
				}
			}
		}
	}
}
