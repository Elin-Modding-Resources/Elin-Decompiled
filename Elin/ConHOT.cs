using System.Collections.Generic;
using UnityEngine;

public class ConHOT : Timebuff
{
	public override void Tick()
	{
		if (owner.hp < owner.MaxHP)
		{
			long num = Dice.Create("SpHOT", base.power).Roll();
			if (base.Giver != null)
			{
				num = num * Mathf.Max(100 + base.Giver.Evalue(413), 0) / 100;
			}
			owner.HealHP(num, HealSource.HOT);
		}
		Mod(-1);
	}

	public override void OnWriteNote(List<string> list)
	{
		Dice dice = Dice.Create("SpHOT", base.power);
		list.Add("hintHOT".lang(dice.ToString()));
	}
}
