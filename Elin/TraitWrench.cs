public class TraitWrench : Trait
{
	public string ID => GetParam(1);

	public override bool IsTool => true;

	public bool IsValidTarget(Thing t)
	{
		if (!t.IsInstalled)
		{
			return false;
		}
		switch (ID)
		{
		case "tent_seabed":
			if (t.trait is TraitTent && (t.trait as TraitTent).zone != null)
			{
				return !(t.trait as TraitTent).zone.IsUnderwater;
			}
			return false;
		case "tent_soil":
		{
			if (!(t.trait is TraitTent))
			{
				return false;
			}
			Zone zone = (t.trait as TraitTent).zone;
			if (zone == null)
			{
				return false;
			}
			return zone.Evalue(2200) < 10;
		}
		case "tent_elec":
			if (t.trait is TraitTent)
			{
				return (t.trait as TraitTent).zone != null;
			}
			return false;
		case "bed":
			return t.trait is TraitBed;
		case "storage":
		case "fridge":
			return t.trait is TraitMagicChest;
		case "extend_v":
		case "extend_h":
			if (t.trait is TraitMagicChest || t.trait is TraitDeliveryChest || t.trait.IsSpecialContainer)
			{
				return false;
			}
			if (t.IsContainer)
			{
				return t.trait is TraitContainer;
			}
			return false;
		default:
			return false;
		}
	}

	public bool Upgrade(Thing t)
	{
		switch (ID)
		{
		case "tent_seabed":
			(t.trait as TraitTent).zone.elements.ModBase(3606, 1);
			break;
		case "tent_elec":
			(t.trait as TraitTent).zone.elements.ModBase(2201, 2);
			break;
		case "tent_soil":
			(t.trait as TraitTent).zone.elements.ModBase(2200, 5);
			break;
		case "bed":
			t.c_containerSize++;
			if (EClass.debug.enable)
			{
				t.c_containerSize += 1000;
			}
			break;
		case "storage":
			t.c_containerUpgrade.cap += 20;
			break;
		case "fridge":
			if (t.c_containerUpgrade.cool != 0)
			{
				return false;
			}
			t.c_containerUpgrade.cool = 1;
			t.elements.SetBase(405, 50);
			break;
		case "extend_v":
		case "extend_h":
		{
			bool flag = ID == "extend_v";
			TraitContainer traitContainer = t.trait as TraitContainer;
			if (t.things.GridSize == 0)
			{
				return false;
			}
			if (flag)
			{
				if (t.things.height != traitContainer.Height)
				{
					return false;
				}
				t.things.SetSize(t.things.width, t.things.height + 1);
			}
			else
			{
				if (t.things.width != traitContainer.Width)
				{
					return false;
				}
				t.things.SetSize(t.things.width + 1, t.things.height);
			}
			break;
		}
		}
		if (EClass.Branch != null)
		{
			EClass.Branch.resources.SetDirty();
		}
		return true;
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.Things.ForeachReverse(delegate(Thing t)
		{
			if (IsValidTarget(t))
			{
				p.TrySetAct("actWrench".lang(t.Name), delegate
				{
					if (Upgrade(t))
					{
						Msg.Say("upgrade", t, owner.GetName(NameStyle.Full, 1));
						SE.Play("build_area");
						t.PlayEffect("buff");
						owner.ModNum(-1);
						EClass._zone.RefreshElectricity();
					}
					else
					{
						Msg.Say("noMoreUpgrade", t, owner.GetName(NameStyle.Full, 1));
					}
					return false;
				}, t);
			}
		});
	}
}
