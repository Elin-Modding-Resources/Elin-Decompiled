using System.Collections.Generic;

public class TraitMannequin : TraitItem
{
	public override bool CanStack => false;

	public override bool CanSearchContent => true;

	public override bool CanUseContent => false;

	public override bool UseAltTiles => owner.things.Count > 0;

	public override bool CanUseFromInventory => false;

	public override bool CanName => true;

	public override bool CanUse(Chara c)
	{
		if (base.CanUse(c))
		{
			if (!EClass._zone.IsPCFaction)
			{
				return EClass._zone is Zone_Tent;
			}
			return true;
		}
		return false;
	}

	public override bool OnUse(Chara c)
	{
		List<Thing> list = new List<Thing>();
		foreach (BodySlot slot in EClass.pc.body.slots)
		{
			if (slot.elementId != 44 && slot.elementId != 45 && slot.thing != null && slot.thing.blessedState >= BlessedState.Normal)
			{
				list.Add(slot.thing);
			}
		}
		if (owner.things.Count == 0)
		{
			foreach (Thing item in list)
			{
				owner.AddCard(item);
			}
		}
		else
		{
			List<Thing> list2 = new List<Thing>();
			foreach (Thing thing in owner.things)
			{
				list2.Add(thing);
			}
			foreach (Thing item2 in list)
			{
				owner.AddCard(item2);
			}
			foreach (Thing item3 in list2)
			{
				EClass.pc.PickOrDrop(EClass.pc.pos, item3, msg: false);
				if (item3.GetRootCard().IsPC)
				{
					EClass.pc.body.Equip(item3, null, msg: false);
				}
			}
		}
		owner.Dye((owner.things.Count > 0) ? owner.things[0].material : null);
		SE.Equip();
		return true;
	}
}
