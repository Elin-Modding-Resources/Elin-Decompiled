using UnityEngine;

public class CalcMoney : EClass
{
	public static int Negotiate(int a, float mod = 1.5f)
	{
		return Mathf.Max((int)((float)((long)a * 100L) / (100f + (float)Mathf.Max(0, EClass.pc.CHA / 2 + EClass.pc.Evalue(291)) * mod)), 1);
	}

	public static int Invest(long a, float mod = 2f)
	{
		long num = a * 100 / (long)(100f + (float)Mathf.Max(0, EClass.pc.CHA / 2 + EClass.pc.Evalue(292)) * mod);
		if (num >= 0 && num < 100000000)
		{
			return (int)num;
		}
		return 100000000;
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
		long a = Guild.Merchant.InvestPrice(tc.c_invest * 700);
		return Mathf.Max(b: Invest(Guild.Merchant.InvestPrice(tc.c_invest * tc.c_invest * 80 + 200)), a: Invest(a));
	}

	public static int InvestZone(Chara c)
	{
		long a = EClass._zone.development * 50;
		return Mathf.Max(b: Invest((long)EClass._zone.development * (long)EClass._zone.development / 4 + 500), a: Invest(a));
	}
}
