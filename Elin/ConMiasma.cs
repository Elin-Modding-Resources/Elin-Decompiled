using System;
using System.Collections.Generic;
using UnityEngine;

public class ConMiasma : TimeDebuff
{
	public override bool IsElemental => true;

	public void SetResistPenetrationLevel(Chara tc, Chara cc)
	{
		base.refVal2 = tc.GetResistPenetrationLevel(base.refVal, AttackSource.Condition, cc);
	}

	public override void Tick()
	{
		Dice dice = Dice.Create("miasma_", base.power);
		try
		{
			owner.DamageHP(dice.Roll(), base.refVal, EClass.rnd(base.power / 2) + base.power / 4, AttackSource.Condition, null, showEffect: true, null, null, base.refVal2);
			if (owner.IsAliveInCurrentZone && base.value > 1)
			{
				for (int i = 0; i < 6; i++)
				{
					foreach (Chara chara in owner.pos.GetRandomPoint(2).Charas)
					{
						if ((!owner.IsPCFaction || chara.IsPCFaction) && (owner.IsPCFaction || !chara.IsPCFaction) && chara.IsFriendOrAbove(owner) && !chara.HasCondition<ConMiasma>())
						{
							Condition condition = chara.AddCondition(Condition.Create(base.power / 2, delegate(ConMiasma con)
							{
								con.givenByPcParty = base.givenByPcParty;
								con.SetElement(base.refVal);
								con.refVal2 = base.refVal2;
							}));
							if (condition != null)
							{
								condition.value = base.value - 1;
							}
							if (EClass.rnd(2) == 0)
							{
								break;
							}
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
			Debug.Log(owner);
			Debug.Log(base.refVal);
		}
		Mod(-1);
	}

	public override void OnWriteNote(List<string> list)
	{
		Dice dice = Dice.Create("miasma_", base.power);
		list.Add("hintDOT".lang(dice.ToString(), base.sourceElement.GetName().ToLower()));
	}
}
