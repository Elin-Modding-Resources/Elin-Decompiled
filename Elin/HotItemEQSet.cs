using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class HotItemEQSet : HotAction
{
	public class Item
	{
		public Card container;

		public int invX;

		public int invY;
	}

	[JsonProperty]
	public List<int> ids = new List<int>();

	public override string Id => "EQSet";

	public override bool CanChangeIconColor => true;

	public override string TextTip
	{
		get
		{
			string text = base.text.IsEmpty("EQSet".lang()) + TextHotkey() + Environment.NewLine;
			foreach (int i in ids)
			{
				Thing thing = EClass.pc.things.Find((Thing t) => t.uid == i);
				text += ((thing == null) ? "missingEQ".lang() : thing.Name);
				text += Environment.NewLine;
			}
			return text.TrimEnd(Environment.NewLine.ToCharArray());
		}
	}

	public HotItemEQSet Register()
	{
		ids.Clear();
		foreach (BodySlot slot in EClass.pc.body.slots)
		{
			if (slot.elementId != 44 && slot.thing != null)
			{
				ids.Add(slot.thing.uid);
			}
		}
		SE.Equip();
		return this;
	}

	public override void Perform()
	{
		for (int i = 0; i < 2; i++)
		{
			Dictionary<int, Thing> dictionary = new Dictionary<int, Thing>();
			foreach (BodySlot slot2 in EClass.pc.body.slots)
			{
				if (slot2.elementId != 44 && slot2.thing != null && slot2.thing.blessedState >= BlessedState.Normal)
				{
					dictionary.Add(slot2.thing.c_equippedSlot - 1, slot2.thing);
					EClass.pc.body.Unequip(slot2.thing, refresh: false);
				}
			}
			Card card = null;
			foreach (int i2 in ids)
			{
				Thing thing = EClass.pc.things.Find((Thing t) => t.uid == i2 && !t.isEquipped);
				if (thing == null)
				{
					continue;
				}
				BodySlot slot = EClass.pc.body.GetSlot(thing, onlyEmpty: true);
				if (slot == null || thing == slot.thing)
				{
					continue;
				}
				Card parentCard = thing.parentCard;
				int invX = thing.invX;
				int invY = thing.invY;
				EClass.pc.body.Equip(thing, slot, msg: false);
				if (dictionary.ContainsKey(slot.index))
				{
					Thing thing2 = dictionary[slot.index];
					if (thing2.uid != thing.uid)
					{
						parentCard.AddThing(thing2);
						card = parentCard;
						thing2.invX = invX;
						thing2.invY = invY;
						dictionary.Remove(slot.index);
					}
				}
			}
			if (card == null)
			{
				continue;
			}
			foreach (Thing value in dictionary.Values)
			{
				if (!value.isEquipped && !card.things.IsFull(value))
				{
					card.AddThing(value);
				}
			}
		}
		SE.Equip();
		LayerChara.Refresh();
	}

	public override void OnShowContextMenu(UIContextMenu m)
	{
		base.OnShowContextMenu(m);
		m.AddButton("updateEQ", delegate
		{
			Register();
		});
	}
}
