using System.Collections.Generic;

public class TraitTeleporter : TraitNewZone
{
	public int teleportedTurn;

	public string id
	{
		get
		{
			return owner.GetStr(31);
		}
		set
		{
			owner.SetStr(31, value);
		}
	}

	public override ZoneTransition.EnterState enterState => ZoneTransition.EnterState.Teleport;

	public override string langOnUse => "actUse";

	public override bool IsTeleport => true;

	public override bool CanBeHeld => true;

	public override bool CanBeOnlyBuiltInHome => true;

	public override void TrySetAct(ActPlan p)
	{
		base.TrySetAct(p);
		if (p.input != ActInput.AllAction || (!EClass.debug.enable && owner.isNPCProperty))
		{
			return;
		}
		p.TrySetAct("actSetTeleporterID", delegate
		{
			Dialog.InputName("dialogTeleportId", id.IsEmpty(""), delegate(bool cancel, string text)
			{
				if (!cancel)
				{
					id = text;
					EClass.game.teleports.SetID(this, EClass._zone.uid);
				}
			});
			return false;
		}, owner);
	}

	public override void OnChangePlaceState(PlaceState state)
	{
		if (state == PlaceState.installed)
		{
			EClass.game.teleports.SetID(this, EClass._zone.uid);
		}
		else
		{
			EClass.game.teleports.Remove(owner.uid);
		}
	}

	public override bool TryTeleport()
	{
		if (id.IsEmpty() && !GetParam(1).IsEmpty())
		{
			return false;
		}
		if (teleportedTurn == EClass.pc.turn)
		{
			teleportedTurn = 0;
			return true;
		}
		List<TraitTeleporter> list = new List<TraitTeleporter>();
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.trait is TraitTeleporter traitTeleporter && traitTeleporter != this && traitTeleporter.owner.IsInstalled && traitTeleporter.owner.pos.IsInBounds && traitTeleporter.IsOn && traitTeleporter.id == id && traitTeleporter != this)
			{
				list.Add(traitTeleporter);
			}
		}
		if (list.Count > 0)
		{
			TraitTeleporter traitTeleporter2 = list.RandomItem();
			traitTeleporter2.teleportedTurn = EClass.pc.turn;
			EClass.pc.Teleport(traitTeleporter2.owner.pos, silent: false, force: true);
			return true;
		}
		Zone zone = EClass.game.teleports.GetTeleportZone(this);
		if (zone is Zone_Tent || EClass._zone is Zone_Tent)
		{
			zone = null;
		}
		if (zone != null)
		{
			if ((EClass._zone.IsPCFaction && !EClass._zone.branch.HasNetwork) || (zone.IsPCFaction && !zone.branch.HasNetwork))
			{
				Msg.Say("noNetwork");
				return false;
			}
			if (EClass._zone.IsPCFaction || zone.IsPCFaction)
			{
				bool flag = true;
				foreach (Quest item in EClass.game.quests.list)
				{
					if (item.ForbidTeleport)
					{
						flag = false;
					}
				}
				if (!flag)
				{
					Msg.Say("hasInvalidQuest");
					return false;
				}
			}
			EClass.pc.MoveZone(zone, new ZoneTransition
			{
				state = enterState,
				idTele = id.IsEmpty(GetParam(3))
			});
			return true;
		}
		return false;
	}

	public override void SetName(ref string s)
	{
		if (!id.IsEmpty())
		{
			s = "_engraved".lang(id, s);
		}
	}

	public override bool CanStackTo(Thing to)
	{
		if (id != to.GetStr(31))
		{
			return false;
		}
		return base.CanStackTo(to);
	}
}
