using Newtonsoft.Json;
using UnityEngine;

public class POLICY
{
	public const int license_food = 2818;

	public const int suite_room = 2813;

	public const int mass_exhibition = 2814;

	public const int platinum_ticket = 2815;

	public const int store_ripoff = 2816;

	public const int store_premium = 2817;

	public const int bed_quality = 2812;

	public const int license_furniture = 2819;

	public const int license_slaver = 2828;

	public const int legendary_heirloom = 2821;

	public const int celeb = 2822;

	public const int legendary_exhibition = 2823;

	public const int license_stolen = 2824;

	public const int milk_fan = 2825;

	public const int egg_fan = 2826;

	public const int breed_season = 2827;

	public const int tourist_safety = 2811;

	public const int license_general = 2820;

	public const int open_business = 2810;

	public const int stop_growth = 2515;

	public const int livestock_priv = 2715;

	public const int nocturnal_life = 2508;

	public const int vaccination = 2509;

	public const int ban_radio = 2510;

	public const int self_sufficient = 2511;

	public const int legal_drug = 2505;

	public const int impressment = 2504;

	public const int prohibition = 2503;

	public const int food_for_people = 2502;

	public const int faith_tax = 2501;

	public const int home_discount = 2800;

	public const int wealth_tax = 2500;

	public const int inquisition = 2507;

	public const int resident_tax = 2512;

	public const int resident_wanted = 2513;

	public const int human_right = 2506;

	public const int speed_growth = 2516;

	public const int trash_sort = 2701;

	public const int trash_no = 2702;

	public const int taxfree = 2514;

	public const int border_watch = 2704;

	public const int taxTransfer = 2705;

	public const int demon_invocation = 2706;

	public const int weed_no = 2703;

	public const int noDM = 2708;

	public const int noAnimal = 2709;

	public const int noMother = 2710;

	public const int incomeTransfer = 2711;

	public const int forcePanty = 2712;

	public const int energy_conservation = 2700;

	public const int auto_farm = 2707;

	public static readonly int[] IDS = new int[51]
	{
		2818, 2813, 2814, 2815, 2816, 2817, 2812, 2819, 2828, 2821,
		2822, 2823, 2824, 2825, 2826, 2827, 2811, 2820, 2810, 2515,
		2715, 2508, 2509, 2510, 2511, 2505, 2504, 2503, 2502, 2501,
		2800, 2500, 2507, 2512, 2513, 2506, 2516, 2701, 2702, 2514,
		2704, 2705, 2706, 2703, 2708, 2709, 2710, 2711, 2712, 2700,
		2707
	};
}
public class Policy : EClass
{
	[JsonProperty]
	public int id;

	[JsonProperty]
	public int days;

	[JsonProperty]
	public bool active;

	public FactionBranch branch;

	public Element Ele => branch.elements.GetElement(id);

	public SourceElement.Row source => EClass.sources.elements.map[id];

	public string Name => source.GetName();

	public Sprite Sprite => Resources.Load<Sprite>("Media/Graphics/Image/Policy/" + id);

	public int Next => 100;

	public int Cost => source.cost[0];

	public void SetOwner(FactionBranch _branch)
	{
		branch = _branch;
	}

	public void OnAdvanceHour(VirtualDate date)
	{
		EClass._zone.elements.ModExp(id, 10f);
	}

	public void RefreshEffect(UINote note = null)
	{
		switch (source.alias)
		{
		case "humanRight":
			ModHappiness(20, FactionMemberType.Default, note);
			ModHappiness(-10, FactionMemberType.Livestock, note);
			break;
		case "nocturnalLife":
			ModHappiness(-20, FactionMemberType.Default, note);
			break;
		case "inquisition":
			break;
		case "legalDrug":
			break;
		case "prohibition":
			break;
		}
	}

	public void ModHappiness(int a, FactionMemberType type, UINote note)
	{
		if ((bool)note)
		{
			note.AddText("peHappiness".lang(("member" + type).lang(), a.ToString() ?? "").TagColorGoodBad(() => a >= 0));
		}
		else
		{
			branch.happiness.list[(int)type].modPolicy += a;
		}
	}

	public void WriteNote(UINote n)
	{
		if (Ele == null)
		{
			Debug.Log(id);
			return;
		}
		Ele.WriteNote(n, EClass._zone.elements);
		if (active)
		{
			n.Space();
			n.AddText("activeFor".lang(days.ToString() ?? ""));
		}
	}

	public int GetSortVal(UIList.SortMode m)
	{
		return 0;
	}
}
