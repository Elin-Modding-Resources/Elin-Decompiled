public class StatsStamina : Stats
{
	public const int Exhausted = 0;

	public const int VeryTired = 1;

	public const int Tired = 2;

	public const int Fine = 3;

	public override bool TrackPhaseChange => BaseStats.CC.IsPC;

	public override int max => BaseStats.CC._maxStamina * BaseStats.CC.Evalue(62) / 100;

	public override int min => -9999;

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
		int num = BaseStats.CC.Evalue(1403);
		if (a < 0 && num > 0 && (a != -1 || EClass.rnd(num + 1) != 0))
		{
			a = a * 100 / (100 + EClass.rnd(num + 1) * 20);
		}
		if (a != 0)
		{
			base.Mod(a);
			if (a < 0)
			{
				_ = BaseStats.CC.ShouldShowMsg;
			}
			if (a < 0 && value < 0)
			{
				BaseStats.CC.DamageHP(-value, AttackSource.Fatigue);
			}
		}
	}

	public override int GetPhase()
	{
		if (value < 0)
		{
			return 0;
		}
		return base.GetPhase();
	}
}
