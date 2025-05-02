using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GoalAutoCombat : GoalCombat
{
	public Thing renderThing;

	public List<Chara> listHealthy = new List<Chara>();

	public override bool UseTurbo => EClass.game.config.autoCombat.turbo;

	public override Thing RenderThing => renderThing;

	public ConfigAutoCombat config => EClass.game.config.autoCombat;

	public override Color GetActPlanColor()
	{
		if (!EClass.pc.IsCriticallyWounded())
		{
			if (!EClass.pc.party.IsCriticallyWounded())
			{
				return EClass.Colors.colorAct;
			}
			return EClass.Colors.colorActWarnning;
		}
		return EClass.Colors.colorActCriticalWarning;
	}

	public GoalAutoCombat(Chara e)
	{
		destEnemy = e;
		EClass.player.autoCombatStartHP = ((EClass.pc.Evalue(1421) > 0) ? (EClass.pc.hp + EClass.pc.mana.value) : EClass.pc.hp);
		foreach (Chara member in EClass.pc.party.members)
		{
			if (!member.IsCriticallyWounded())
			{
				listHealthy.Add(member);
			}
		}
	}

	public override bool TryUseRanged(int dist)
	{
		Thing ranged = owner.ranged;
		owner.ranged = null;
		if (config.bUseHotBar)
		{
			FindRanged(hotbar: true);
		}
		if (owner.ranged != null && ACT.Ranged.Perform(owner, tc))
		{
			if (owner == null)
			{
				return true;
			}
			renderThing = owner.ranged;
			if (owner.ranged != ranged)
			{
				owner.ranged = ranged;
			}
			return true;
		}
		return false;
		void FindRanged(bool hotbar)
		{
			owner.things.Foreach(delegate(Thing t)
			{
				if (t.IsHotItem)
				{
					if (!hotbar)
					{
						return false;
					}
				}
				else if (hotbar)
				{
					return false;
				}
				if (t.IsRangedWeapon && (t.trait is TraitToolRangeCane || t.c_ammo > 0 || EClass.pc.FindAmmo(t) != null) && owner.CanEquipRanged(t))
				{
					owner.ranged = t;
					return true;
				}
				return false;
			});
		}
	}

	public void GetAbilities(Func<Element, bool> func)
	{
		if (config.bUseHotBar)
		{
			foreach (Thing item in EClass.pc.things.Where((Thing t) => t.IsHotItem && t.trait is TraitAbility))
			{
				Element element = owner.elements.GetElement(item.c_idAbility);
				if (element != null && func(element) && (element.id != 9150 || EClass.player.favAbility.Contains(element.id)))
				{
					AddAbility(element.act, 15);
				}
			}
		}
		if (!config.bUseInventory)
		{
			return;
		}
		foreach (Element value in owner.elements.dict.Values)
		{
			if ((!config.bUseFav || EClass.player.favAbility.Contains(value.id)) && func(value) && (value.id != 9150 || EClass.player.favAbility.Contains(value.id)))
			{
				AddAbility(value.act);
			}
		}
	}

	public override void BuildAbilityList()
	{
		GetAbilities((Element e) => e.source.abilityType.Length != 0);
		AddAbility(ACT.Ranged);
		AddAbility(ACT.Melee);
		AddAbility(ACT.Item);
		if (owner.HasCondition<ConBrightnessOfLife>())
		{
			TryAddAbility(6410);
		}
	}

	public override bool TryAbortCombat()
	{
		if (idleCount >= 2)
		{
			Msg.Say("abort_idle");
			return true;
		}
		return false;
	}
}
