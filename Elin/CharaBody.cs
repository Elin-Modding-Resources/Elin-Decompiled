using System.Collections.Generic;
using UnityEngine;

public class CharaBody : EClass
{
	public Chara owner;

	public List<BodySlot> slots = new List<BodySlot>();

	public BodySlot slotMainHand;

	public BodySlot slotOffHand;

	public BodySlot slotRange;

	public int[] rawSlots => owner.rawSlots;

	public void SetOwner(Chara chara, bool deserialized = false)
	{
		owner = chara;
		if (!deserialized)
		{
			return;
		}
		if (rawSlots != null)
		{
			for (int i = 0; i < rawSlots.Length; i++)
			{
				AddBodyPart(rawSlots[i]);
			}
		}
		foreach (Thing thing in owner.things)
		{
			if (thing.isEquipped)
			{
				int num = thing.c_equippedSlot - 1;
				if (num < 0 || num >= slots.Count)
				{
					thing.c_equippedSlot = 0;
					continue;
				}
				slots[num].thing = thing;
				thing.elements.SetParent(owner);
				thing.trait.OnEquip(owner, onSetOwner: true);
			}
		}
	}

	public void Unequip(Thing thing, bool refresh = true)
	{
		if (thing.isEquipped)
		{
			Unequip(slots[thing.c_equippedSlot - 1], refresh);
		}
	}

	public void UnequipAll(int idSlot)
	{
		foreach (BodySlot slot in slots)
		{
			if (slot.elementId == idSlot)
			{
				Unequip(slot);
			}
		}
	}

	public void Unequip(BodySlot slot, bool refresh = true)
	{
		if (slot.thing == null)
		{
			return;
		}
		Thing thing = slot.thing;
		if (EClass.pc != null)
		{
			EClass.pc.faction.charaElements.OnUnequip(owner, thing);
		}
		thing.elements.SetParent();
		thing.trait.OnUnequip(owner);
		thing.c_equippedSlot = 0;
		slot.thing = null;
		if (owner.IsPC)
		{
			if (refresh)
			{
				LayerChara.Refresh();
			}
			WidgetEquip.SetDirty();
		}
		LayerInventory.SetDirty(thing);
		if (slot.elementId == 45 && EClass.core.IsGameStarted)
		{
			owner.RecalculateFOV();
		}
		if (refresh && owner.isCreated && EClass.core.IsGameStarted)
		{
			owner.Refresh();
			if (slot.elementId == 37 && owner.HasElement(1209))
			{
				owner.Say("tail_free", owner);
			}
		}
	}

	public bool IsEquippable(Thing thing, BodySlot slot, bool text = true)
	{
		if (slot == null)
		{
			return false;
		}
		switch (slot.elementId)
		{
		case 31:
			if (owner.HasElement(1555))
			{
				return CannotEquip();
			}
			break;
		case 33:
			if (owner.HasElement(1554))
			{
				return CannotEquip();
			}
			break;
		case 39:
			if (owner.HasElement(1552))
			{
				return CannotEquip();
			}
			break;
		}
		if (slot.thing != null && slot.thing.blessedState <= BlessedState.Cursed)
		{
			return CannotEquip();
		}
		if (IsTooHeavyToEquip(thing))
		{
			if (EClass.core.IsGameStarted && owner.IsPC && text)
			{
				Msg.Say("tooHeavyToEquip", thing);
			}
			return false;
		}
		return true;
		bool CannotEquip()
		{
			if (text)
			{
				Msg.Say("cannnotEquip", slot.element.GetName().ToLower());
			}
			return false;
		}
	}

	public bool IsTooHeavyToEquip(Thing thing)
	{
		if (owner.HasElement(1204) && thing.ChildrenAndSelfWeight > 1000)
		{
			return thing.category.slot != 44;
		}
		return false;
	}

	public void UnqeuipIfTooHeavy(Thing t)
	{
		if (t.isEquipped)
		{
			_ = slots[t.c_equippedSlot - 1];
			if (IsTooHeavyToEquip(t))
			{
				owner.Say("tooHeavyToEquip", t);
				Unequip(t);
			}
		}
		if (!owner.IsPC && !t.isEquipped)
		{
			owner.TryEquip(t);
		}
	}

	public bool Equip(Thing thing, BodySlot slot = null, bool msg = true)
	{
		slot = slot ?? GetSlot(thing.category.slot) ?? GetSlot(thing.category.slot, onlyEmpty: false);
		if (slot == slotMainHand || slot == slotOffHand)
		{
			owner.combatCount = 10;
		}
		if (slot == null || slot.elementId != thing.category.slot)
		{
			Debug.LogWarning("could not equip:" + thing?.ToString() + "/" + slot?.ToString() + "/" + owner);
			return false;
		}
		if (!IsEquippable(thing, slot))
		{
			return false;
		}
		if (slot.thing != null)
		{
			if (slot.thing == thing)
			{
				Unequip(slot);
				return false;
			}
			Unequip(slot, refresh: false);
		}
		Unequip(thing, refresh: false);
		if (thing.parent != owner)
		{
			if (msg && owner.IsPC && thing.parent is Thing)
			{
				Msg.Say("movedToEquip", thing, thing.parent as Thing);
			}
			owner.AddCard(thing);
		}
		slot.thing = thing;
		thing.c_equippedSlot = slot.index + 1;
		thing.elements.SetParent(owner);
		thing.trait.OnEquip(owner, onSetOwner: false);
		if (EClass.pc != null)
		{
			EClass.pc.faction.charaElements.OnEquip(owner, thing);
		}
		owner.SetTempHand();
		if (owner.IsPC)
		{
			if (msg)
			{
				LayerChara.Refresh();
			}
			LayerInventory.SetDirty(thing);
			WidgetEquip.SetDirty();
		}
		if (slot.elementId == 45 && EClass.core.IsGameStarted)
		{
			owner.RecalculateFOV();
		}
		if (owner.isCreated && EClass.core.IsGameStarted)
		{
			owner.Refresh();
		}
		if (owner.isCreated)
		{
			if (thing.Evalue(656) > 0)
			{
				thing.blessedState = BlessedState.Cursed;
			}
			if (thing.blessedState <= BlessedState.Cursed)
			{
				owner.Say("equipCursed", owner);
				owner.PlaySound("curse3");
			}
		}
		if (slot.elementId == 37 && owner.HasElement(1209))
		{
			owner.Say("tail_covered", owner);
		}
		return true;
	}

	public void AddBodyPart(int ele, Thing thing = null)
	{
		BodySlot item = new BodySlot
		{
			elementId = ele,
			thing = thing,
			index = slots.Count
		};
		if (ele == 35)
		{
			if (slotMainHand == null)
			{
				slotMainHand = item;
			}
			else if (slotOffHand == null)
			{
				slotOffHand = item;
			}
		}
		if (ele == 41)
		{
			slotRange = item;
		}
		slots.Add(item);
	}

	public void RefreshBodyParts()
	{
		for (int i = 0; i < slots.Count; i++)
		{
			BodySlot bodySlot = slots[i];
			bodySlot.index = i;
			int elementId = bodySlot.elementId;
			if (elementId == 35)
			{
				if (slotMainHand == null)
				{
					slotMainHand = bodySlot;
				}
				else if (slotOffHand == null)
				{
					slotOffHand = bodySlot;
				}
			}
			if (elementId == 41)
			{
				slotRange = bodySlot;
			}
			if (bodySlot.thing != null)
			{
				bodySlot.thing.c_equippedSlot = i + 1;
			}
		}
	}

	public void RemoveBodyPart(int ele)
	{
		int idx = slots.FindIndex((BodySlot a) => a.elementId == ele);
		RemoveBodyPartAt(idx);
	}

	public void RemoveBodyPartAt(int idx)
	{
		if (idx != -1)
		{
			BodySlot bodySlot = slots[idx];
			if (bodySlot.thing != null)
			{
				Unequip(bodySlot);
			}
			if (slotMainHand == bodySlot)
			{
				slotMainHand = null;
			}
			if (slotOffHand == bodySlot)
			{
				slotOffHand = null;
			}
			if (slotRange == bodySlot)
			{
				slotRange = null;
			}
			slots.RemoveAt(idx);
		}
	}

	public BodySlot GetSlot(Thing t, bool onlyEmpty = false, bool secondSlot = false)
	{
		BodySlot slot = GetSlot(t.category.slot, onlyEmpty: true, secondSlot);
		if (slot != null)
		{
			return slot;
		}
		if (!onlyEmpty)
		{
			return GetSlot(t.category.slot, onlyEmpty: false, secondSlot);
		}
		return null;
	}

	public BodySlot GetSlot(int elementId, bool onlyEmpty = true, bool secondSlot = false)
	{
		bool flag = true;
		foreach (BodySlot slot in slots)
		{
			if (elementId == slot.elementId && (!onlyEmpty || slot.thing == null))
			{
				if (!(secondSlot && flag))
				{
					return slot;
				}
				flag = false;
			}
		}
		return null;
	}

	public Thing GetEquippedThing(int elementId)
	{
		foreach (BodySlot slot in slots)
		{
			if (slot.elementId == elementId && slot.thing != null)
			{
				return slot.thing;
			}
		}
		return null;
	}

	public int GetWeight(bool armorOnly = false)
	{
		int num = 0;
		foreach (BodySlot slot in slots)
		{
			if (slot.thing != null && (!armorOnly || (slot.elementId != 44 && slot.elementId != 45)))
			{
				num += slot.thing.ChildrenAndSelfWeight;
			}
		}
		return num;
	}

	public int GetAttackIndex(Thing t)
	{
		int num = 0;
		foreach (BodySlot slot in slots)
		{
			if (slot.thing != null && slot.elementId == 35 && slot.thing.source.offense.Length >= 2)
			{
				if (slot.thing == t)
				{
					return num;
				}
				num++;
			}
		}
		return -1;
	}

	public int GetMeleeDistance()
	{
		int num = owner.Evalue(666);
		foreach (BodySlot slot in slots)
		{
			if (slot.elementId == 35 && slot.thing != null && slot.thing.Evalue(666) > num)
			{
				num = slot.thing.Evalue(666);
			}
		}
		return 1 + num;
	}

	public AttackStyle GetAttackStyle()
	{
		bool flag = false;
		int num = 0;
		foreach (BodySlot slot in slots)
		{
			if (slot.elementId == 35 && slot.thing != null)
			{
				if (slot.thing.IsMeleeWeapon)
				{
					num++;
				}
				else
				{
					flag = true;
				}
			}
		}
		if (num > 1)
		{
			return AttackStyle.TwoWield;
		}
		if (flag)
		{
			return AttackStyle.Shield;
		}
		if (num == 1)
		{
			return AttackStyle.TwoHand;
		}
		return AttackStyle.Default;
	}

	public int GetAttackStyleElement(AttackStyle style)
	{
		return style switch
		{
			AttackStyle.TwoHand => 130, 
			AttackStyle.TwoWield => 131, 
			_ => 0, 
		};
	}

	public int GetSortVal(BodySlot slot)
	{
		return -(slot.element.sort * 100 + owner.body.slots.IndexOf(slot));
	}

	public void SetBodyIndexText(BodySlot b, UIText t)
	{
		if (!t)
		{
			return;
		}
		if (b.indexPart == 0)
		{
			int num = 0;
			foreach (BodySlot slot in slots)
			{
				if (slot.elementId == b.element.id)
				{
					num++;
				}
				if (b == slot)
				{
					break;
				}
			}
			b.indexPart = num;
		}
		if (b.elementId == 35)
		{
			t.SetText(b.indexPart.ToString() ?? "");
			t.SetActive(enable: true);
		}
		else
		{
			t.SetActive(enable: false);
		}
	}
}
