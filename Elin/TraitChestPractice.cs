public class TraitChestPractice : TraitContainer
{
	public override int ChanceLock => 100;

	public override void Prespawn(int lv)
	{
	}

	public override void OnCreate(int lv)
	{
		base.OnCreate(lv);
		owner.refVal = 0;
		owner.LV = 1;
		owner.c_lockLv = owner.LV;
	}

	public override void OnSimulateHour(VirtualDate date)
	{
		owner.turn++;
		if (owner.turn < 2)
		{
			return;
		}
		owner.turn = 0;
		if (owner.c_lockLv == 0)
		{
			owner.LV++;
		}
		else
		{
			owner.LV--;
			if (owner.LV < 1)
			{
				owner.LV = 1;
			}
		}
		owner.c_lockLv = owner.LV;
		owner.hp = 100;
	}
}
