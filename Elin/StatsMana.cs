using UnityEngine;

public class StatsMana : Stats
{
	public override int max => (int)Mathf.Clamp(((long)(BaseStats.CC.MAG * 2 + BaseStats.CC.WIL + BaseStats.CC.LER / 2) * (long)Mathf.Min(BaseStats.CC.LV, 25) / 25 + BaseStats.CC.MAG + 10) * (BaseStats.CC.Evalue(61) - BaseStats.CC.Evalue(93)) / 100 * ((BaseStats.CC.IsPCFaction ? 100 : (100 + (int)BaseStats.CC.rarity * 250)) + (BaseStats.CC.IsPC ? (EClass.player.lastEmptyAlly * BaseStats.CC.Evalue(1646)) : 0)) / 100, 1f, 100000000f);

	public override int min => -999999;

	public override void Mod(int a)
	{
		if (BaseStats.CC.IsPC && EClass.debug.godMode && a < 0)
		{
			return;
		}
		if (a < 0 && BaseStats.CC.HasElement(1330))
		{
			if (a == -1 && EClass.rnd(4) == 0)
			{
				return;
			}
			a = -EClass.rnd(-a * 130 / 100 + 2);
		}
		base.Mod(a);
		if (a >= 0)
		{
			return;
		}
		if (!BaseStats.CC.IsPCFaction)
		{
			a /= 10;
		}
		_ = BaseStats.CC.ShouldShowMsg;
		if (value >= 0)
		{
			return;
		}
		int num = -value * 400 / (100 + BaseStats.CC.EvalueMax(303, -5) * 10);
		if (BaseStats.CC.HasElement(1201))
		{
			num /= 3;
		}
		if (!BaseStats.CC.IsPC)
		{
			num /= 5;
			if (num < 10)
			{
				return;
			}
		}
		BaseStats.CC.Say("mana_overflow", BaseStats.CC);
		BaseStats.CC.DamageHP(num, 921, 100, AttackSource.ManaBackfire);
		Chara cC = BaseStats.CC;
		if (cC.IsAliveInCurrentZone)
		{
			cC.elements.ModExp(303, Mathf.Clamp(-a * 10, 10, 200));
		}
	}
}
