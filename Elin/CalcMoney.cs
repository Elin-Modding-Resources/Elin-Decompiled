using UnityEngine;

public class CalcMoney : EClass
{
	public static int Negotiate(int a, float mod = 1.5f)
	{
		return (int)Mathf.Max((long)a * 100L / (long)(100f + (float)Mathf.Max(0, EClass.pc.CHA / 2 + EClass.pc.Evalue(291)) * mod), 1f);
	}

	public static int Invest(int a, float mod = 2f)
	{
		return (int)Mathf.Max((long)a * 100L / (long)(100f + (float)Mathf.Max(0, EClass.pc.CHA / 2 + EClass.pc.Evalue(292)) * mod), 1f);
	}

	public static int Meal(Chara c)
	{
		return Negotiate(Guild.Fighter.ServicePrice(70));
	}

	public static int Heal(Chara c)
	{
		return Negotiate(Guild.Fighter.ServicePrice(100));
	}

	public static int Picklock(Chara c, Thing t)
	{
		return Negotiate(Guild.Fighter.ServicePrice(t.c_lockLv * 65 + 75));
	}

	public static int Identify(Chara c, bool superior)
	{
		return Negotiate(Guild.Fighter.ServicePrice(superior ? 750 : 50));
	}

	public static int Revive(Chara c)
	{
		return Negotiate((c.LV + 5) * (c.LV + 5) * 3);
	}

	public static int BuySlave(Chara c)
	{
		return Negotiate((c.LV + 5) * (c.LV + 5) * 20 + Rand.rndSeed(c.LV * 20, c.uid));
	}

	public static int SellSlave(Chara c)
	{
		return (c.LV + 5) * (c.LV + 5) * 5;
	}

	public static int Whore(Chara seller, Chara buyer)
	{
		int num = Mathf.Max(seller.CHA * 6, 20) * ((!buyer.IsWealthy) ? 1 : 2);
		int num2 = Mathf.Max(buyer.CHA * 12, 20) * ((!buyer.IsWealthy) ? 1 : 2);
		Debug.Log("seller:" + num + " buyer:" + num2 + " wealthy:" + buyer.IsWealthy);
		if (num > num2)
		{
			num = num2;
		}
		return num;
	}

	public static int InvestShop(Chara c, Chara tc)
	{
		return Invest(Guild.Merchant.InvestPrice(Mathf.Max(tc.c_invest * 700, Mathf.Min(tc.c_invest, 4000) * Mathf.Min(tc.c_invest, 4000) * 80) + 200));
	}

	public static int InvestZone(Chara c)
	{
		return Invest((int)Mathf.Max((long)EClass._zone.development * 50L, Mathf.Min(EClass._zone.development, 80000) * Mathf.Min(EClass._zone.development, 80000) / 4) + 500);
	}
}
