using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class FACTION
{
	public const int bfFertile = 3700;

	public const int bfRuin = 3702;

	public const int bfUndersea = 3606;

	public const int bfSea = 3605;

	public const int bfBeach = 3604;

	public const int bfHill = 3603;

	public const int bfSnow = 3602;

	public const int bfForest = 3601;

	public const int bfPlain = 3600;

	public const int bfGeyser = 3701;

	public const int bfCave = 3500;

	public const int fPromo = 2202;

	public const int fAttraction = 2206;

	public const int fSafety = 2205;

	public const int fFood = 2204;

	public const int fMoral = 2203;

	public const int fElec = 2201;

	public const int fSoil = 2200;

	public const int fTaxEvasion = 2119;

	public const int fLuck = 2118;

	public const int bfTranquil = 3703;

	public const int fEducation = 2116;

	public const int fLoyal = 2117;

	public const int fRation = 2207;

	public const int bfVolcano = 3704;

	public const int bfIce = 3804;

	public const int bfFish = 3706;

	public const int fAdmin = 2115;

	public const int actBuildInspect = 4006;

	public const int actBuildRecipe = 4005;

	public const int actBuildCollect = 4004;

	public const int actBuildAnywhere = 4003;

	public const int actBuildTerrain = 4002;

	public const int actBuildMine = 4001;

	public const int actBuildCut = 4000;

	public const int bfStart = 3900;

	public const int bfChitin = 3805;

	public const int bfGum = 3803;

	public const int bfSilica = 3802;

	public const int bfMushroom = 3801;

	public const int bfCoal = 3800;

	public const int bfLandmark5 = 3784;

	public const int bfLandmark4 = 3783;

	public const int bfLandmark3 = 3782;

	public const int bfLandmark2 = 3781;

	public const int bfLandmark1 = 3780;

	public const int bfBreed = 3710;

	public const int bfBasin = 3709;

	public const int bfFreshAir = 3708;

	public const int bfMonster = 3707;

	public const int bfHunt = 3705;

	public const int fConstruction = 2003;

	public const int fHeirloom = 2120;

	public static readonly int[] IDS = new int[53]
	{
		3700, 3702, 3606, 3605, 3604, 3603, 3602, 3601, 3600, 3701,
		3500, 2202, 2206, 2205, 2204, 2203, 2201, 2200, 2119, 2118,
		3703, 2116, 2117, 2207, 3704, 3804, 3706, 2115, 4006, 4005,
		4004, 4003, 4002, 4001, 4000, 3900, 3805, 3803, 3802, 3801,
		3800, 3784, 3783, 3782, 3781, 3780, 3710, 3709, 3708, 3707,
		3705, 2003, 2120
	};
}
public class Faction : EClass
{
	[JsonProperty]
	public FactionRelation relation = new FactionRelation();

	[JsonProperty]
	public string id;

	[JsonProperty]
	public string uid;

	[JsonProperty]
	public string name;

	[JsonProperty]
	public List<HireInfo> listReserve = new List<HireInfo>();

	[JsonProperty]
	public ElementContainerZone elements = new ElementContainerZone();

	[JsonProperty]
	public HashSet<int> globalPolicies = new HashSet<int>();

	public ElementContainerFaction charaElements = new ElementContainerFaction();

	public SourceFaction.Row _source;

	public string Name => name;

	public SourceFaction.Row source => _source ?? (_source = EClass.sources.factions.map[id]);

	public virtual string TextType => "sub_faction".lang();

	public static Faction Create(SourceFaction.Row r)
	{
		Faction faction = ClassCache.Create<Faction>(r.type, "Elin");
		faction.id = r.id;
		faction.Init();
		return faction;
	}

	public void Init()
	{
		EClass.game.factions.AssignUID(this);
		relation.faction = this;
		relation.affinity = source.relation;
		name = source.GetText();
	}

	public void OnLoad()
	{
		relation.faction = this;
	}

	public float GetHappiness()
	{
		return 50f;
	}

	public Sprite GetSprite()
	{
		return ResourceCache.Load<Sprite>("Media/Graphics/Image/Faction/" + source.id);
	}

	public int CountTax()
	{
		return (int)((float)CountWealth() * 0.1f);
	}

	public int GetMaxReserve()
	{
		int num = 2;
		foreach (FactionBranch child in GetChildren())
		{
			num += child.lv;
		}
		return num;
	}

	public List<FactionBranch> GetChildren()
	{
		List<FactionBranch> list = new List<FactionBranch>();
		foreach (Spatial value in EClass.game.spatials.map.Values)
		{
			if (value.mainFaction == this)
			{
				list.Add((value as Zone).branch);
			}
		}
		return list;
	}

	public int CountWealth()
	{
		if (EClass._zone.IsPCFaction)
		{
			EClass._zone.branch.resources.Refresh();
		}
		int num = 0;
		foreach (FactionBranch child in GetChildren())
		{
			num += child.Worth;
		}
		return num;
	}

	public int CountTerritories()
	{
		int num = 0;
		foreach (Spatial value in EClass.game.spatials.map.Values)
		{
			if (value.mainFaction == this)
			{
				num++;
			}
		}
		return num;
	}

	public int CountMembers()
	{
		int num = 0;
		foreach (Chara value in EClass.game.cards.globalCharas.Values)
		{
			if (value.Chara.faction == this)
			{
				num++;
			}
		}
		return num;
	}

	public Hostility GetHostility()
	{
		if (this == EClass.Home || relation.affinity >= 200)
		{
			return Hostility.Ally;
		}
		if (relation.affinity >= 100)
		{
			return Hostility.Friend;
		}
		if (relation.affinity <= -100)
		{
			return Hostility.Enemy;
		}
		return Hostility.Neutral;
	}

	public void ModRelation(int a)
	{
		relation.affinity += a;
	}

	public bool HasMember(string id, bool includeReserve = true)
	{
		foreach (Chara value in EClass.game.cards.globalCharas.Values)
		{
			if (value.id == id && value.IsHomeMember())
			{
				return true;
			}
		}
		if (includeReserve)
		{
			foreach (HireInfo item in listReserve)
			{
				if (item.chara.id == id)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsWearingPanty(Chara c)
	{
		if ((!c.IsUnique || c.bio.gender != 2) && (c.IsHuman || c.IsFairy) && !(c.trait is TraitMerchant))
		{
			return true;
		}
		if (IsGlobalPolicyActive(2712))
		{
			foreach (Chara value in EClass.game.cards.globalCharas.Values)
			{
				if (value.id == c.id && value.IsHomeMember())
				{
					return true;
				}
			}
		}
		return false;
	}

	public void AddContribution(int a)
	{
		if (a != 0 && relation.type == FactionRelation.RelationType.Member)
		{
			relation.exp += a;
			Msg.Say("contribute", a.ToString() ?? "", Name);
		}
	}

	public void AddReserve(Chara c)
	{
		if (c.IsPCParty)
		{
			EClass.pc.party.RemoveMember(c);
		}
		if (c.memberType == FactionMemberType.Livestock)
		{
			c.SetInt(36, EClass.world.date.GetRaw() + 14400);
		}
		if (c.IsHomeMember())
		{
			c.homeBranch.RemoveMemeber(c);
		}
		if (c.currentZone != null)
		{
			c.currentZone.RemoveCard(c);
		}
		if (EClass.Branch.uidMaid == c.uid)
		{
			EClass.Branch.uidMaid = 0;
		}
		HireInfo item = new HireInfo
		{
			chara = c,
			isNew = true
		};
		listReserve.Add(item);
	}

	public void RemoveReserve(Chara c)
	{
		listReserve.ForeachReverse(delegate(HireInfo i)
		{
			if (i.chara == c || i.chara.uid == c.uid)
			{
				listReserve.Remove(i);
			}
		});
	}

	public void OnAdvanceDay()
	{
		foreach (FactionBranch child in GetChildren())
		{
			child.OnAdvanceDay();
		}
	}

	public void OnAdvanceMonth()
	{
		if (GetChildren().Count == 0)
		{
			return;
		}
		int num = 0;
		num += GetResidentTax();
		num += GetRankIncome();
		num += GetFactionSalary();
		if (num > 0)
		{
			Thing container_deposit = EClass.game.cards.container_deposit;
			if (EClass.pc.homeBranch.policies.IsActive(2711) && (container_deposit.GetCurrency() > 0 || !container_deposit.things.IsFull()))
			{
				container_deposit.ModCurrency(num);
				Msg.Say("bankIncome", Lang._currency(num, "money"));
			}
			else
			{
				Thing thing = ThingGen.Create("money").SetNum(num);
				Thing p = ThingGen.CreateParcel("parcel_salary", thing);
				EClass.world.SendPackage(p);
			}
		}
		num = GetTotalTax(evasion: true);
		Thing thing2 = ThingGen.CreateBill(num, tax: true);
		thing2.SetInt(35, EClass.player.extraTax);
		Msg.Say("getBill", Lang._currency(num, "money"));
		TryPayBill(thing2);
		Msg.Say("bills", EClass.player.taxBills.ToString() ?? "");
		if (EClass.player.taxBills >= 4 && EClass.game.principal.tax && !EClass.debug.godMode)
		{
			EClass.player.ModKarma(-50);
		}
	}

	public void TryPayBill(Thing bill)
	{
		if (IsGlobalPolicyActive(2705) && EClass.game.cards.container_deposit.GetCurrency() > bill.c_bill)
		{
			InvOwnerDeliver.PayBill(bill, fromBank: true);
		}
		else
		{
			EClass.world.SendPackage(bill);
		}
	}

	public FactionBranch FindBranch(Chara c)
	{
		foreach (FactionBranch child in GetChildren())
		{
			if (child.members.Contains(c))
			{
				return child;
			}
		}
		return null;
	}

	public void AddGlobalPolicy(int id)
	{
		globalPolicies.Add(id);
	}

	public bool IsGlobalPolicyActive(int id)
	{
		bool result = false;
		foreach (FactionBranch child in GetChildren())
		{
			if (child.policies.IsActive(id))
			{
				result = true;
			}
		}
		return result;
	}

	public void SetGlobalPolicyActive(int id, bool active)
	{
		foreach (FactionBranch child in GetChildren())
		{
			child.policies.SetActive(id, active);
		}
	}

	public int GetResidentTax()
	{
		int num = 0;
		foreach (FactionBranch child in GetChildren())
		{
			num += child.GetResidentTax();
		}
		if (num > 0)
		{
			Msg.Say("getResidentTax", Lang._currency(num, "money"));
		}
		return num;
	}

	public int GetRankIncome()
	{
		int num = 0;
		foreach (FactionBranch child in GetChildren())
		{
			num += EClass.game.spatials.ranks.GetIncome(child.owner);
		}
		if (num > 0)
		{
			Msg.Say("getRankIncome", Lang._currency(num, "money"));
		}
		return num;
	}

	public int GetFactionSalary()
	{
		int num = 0;
		foreach (Faction value in EClass.game.factions.dictAll.Values)
		{
			num += value.relation.GetSalary();
		}
		if (num > 0)
		{
			Msg.Say("getFactionSalary", Lang._currency(num, "money"));
		}
		return num;
	}

	public int GetTotalTax(bool evasion)
	{
		return GetBaseTax(evasion) + GetFameTax(evasion) + EClass.player.extraTax;
	}

	public int GetBaseTax(bool evasion)
	{
		int a = EClass.world.date.year - EClass.game.Prologue.year;
		int v = 500 + Mathf.Min(a, 10) * 500;
		return EvadeTax(v, evasion);
	}

	public int GetFameTax(bool evasion)
	{
		int v = EClass.curve(EClass.player.fame * 2, 10000, 2000, 80);
		v = EvadeTax(v, evasion);
		if (v < 0)
		{
			v = 50000;
		}
		return v;
	}

	public int EvadeTax(int v, bool evasion)
	{
		if (!evasion)
		{
			return v;
		}
		int num = 0;
		foreach (FactionBranch child in GetChildren())
		{
			num += child.Evalue(2119);
		}
		return (int)(long)((float)(v * 100) / (100f + Mathf.Sqrt(num * 5)));
	}

	public void SetTaxTooltip(UINote n)
	{
		n.AddHeader("tax");
		n.AddTopic("tax_base", Lang._currency(GetBaseTax(evasion: true), showUnit: true) + " (" + Lang._currency(GetBaseTax(evasion: false)) + ")");
		n.AddTopic("tax_fame", Lang._currency(GetFameTax(evasion: true), showUnit: true) + " (" + Lang._currency(GetFameTax(evasion: false)) + ")");
		n.AddTopic("tax_extra", Lang._currency(EClass.player.extraTax, showUnit: true));
	}

	public int CountTaxFreeLand()
	{
		int num = 0;
		foreach (FactionBranch child in GetChildren())
		{
			if (child.policies.IsActive(2514))
			{
				num++;
			}
		}
		return num;
	}
}
