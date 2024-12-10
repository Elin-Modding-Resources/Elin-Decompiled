using UnityEngine;

public class TraitNewZone : Trait
{
	public Zone zone
	{
		get
		{
			return RefZone.Get(owner.c_uidZone);
		}
		set
		{
			owner.c_uidZone = value?.uid ?? 0;
		}
	}

	public virtual bool CanUseInTempDungeon => false;

	public virtual string langOnUse => "";

	public virtual bool IsUpstairs => false;

	public virtual bool IsDownstairs => false;

	public virtual bool IsTeleport => false;

	public virtual bool OnlyInTheSameTopZone => false;

	public virtual bool AutoEnter => true;

	public virtual bool ForceEnter => false;

	public virtual bool CanToggleAutoEnter => false;

	public virtual bool CreateExternalZone
	{
		get
		{
			if (!IsTeleport)
			{
				if (owner.GetStr(30) == null)
				{
					return GetParam(1) != null;
				}
				return true;
			}
			return false;
		}
	}

	public virtual ZoneTransition.EnterState enterState => ZoneTransition.EnterState.Region;

	public virtual bool IsEntrace
	{
		get
		{
			if (!IsUpstairs)
			{
				return !IsDownstairs;
			}
			return false;
		}
	}

	public override bool CanBeHeld => false;

	public override bool CanBeDestroyed => false;

	public override bool CanBeStolen => false;

	public virtual int UseDist => 0;

	public override void OnImportMap()
	{
		owner.c_uidZone = 0;
	}

	public virtual Point GetExitPos()
	{
		Point point = new Point(owner.pos);
		if (owner.dir % 2 == 0)
		{
			point.x -= owner.sourceCard.W / 2;
			point.z--;
		}
		else
		{
			point.x++;
			point.z += owner.sourceCard.H / 2;
		}
		return point;
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!EClass._zone.AllowNewZone)
		{
			return;
		}
		bool flag = IsEntrace || p.IsSelf;
		if (owner.sourceRenderCard.multisize)
		{
			int x = p.pos.x;
			int z = p.pos.z;
			int w = owner.W;
			int h = owner.H;
			flag = ((owner.dir % 2 != 0) ? (p.pos.x == owner.pos.x && p.pos.z >= owner.pos.z + 1 && p.pos.z < owner.pos.z + h - 1) : (z == owner.pos.z && x > owner.pos.x - w + 1 && x <= owner.pos.x - 1));
		}
		if (!flag || (EClass.pc.held != null && EClass.pc.held.trait.CanOnlyCarry) || p.dist > UseDist)
		{
			return;
		}
		p.TrySetAct("actNewZone", delegate
		{
			if ((EClass._zone.RegenerateOnEnter || EClass._zone.IsInstance) && !CanUseInTempDungeon)
			{
				Msg.Say("badidea");
				return false;
			}
			MoveZone();
			return false;
		}, owner, CursorSystem.MoveZone);
	}

	public bool CanAutoEnter()
	{
		if (!EClass._zone.AllowNewZone)
		{
			return false;
		}
		if (owner.sourceRenderCard.multisize)
		{
			return false;
		}
		if (EClass.pc.held != null && EClass.pc.held.trait.CanOnlyCarry)
		{
			return false;
		}
		return true;
	}

	public bool MoveZone(bool confirmed = false)
	{
		if (Electricity != 0 && !owner.isOn)
		{
			owner.PlaySound("electricity_insufficient");
			return false;
		}
		if (TryTeleport())
		{
			return true;
		}
		if (!confirmed && EClass._zone.IsNefia && EClass._zone.Boss != null)
		{
			EClass.pc.ai.Cancel();
			EInput.Consume(consumeAxis: true);
			Dialog.YesNo("ExitZoneBoss".lang(EClass._zone.Name), delegate
			{
				MoveZone(confirmed: true);
			});
			return false;
		}
		ZoneTransition transition = new ZoneTransition
		{
			state = enterState,
			idTele = GetParam(3)
		};
		Zone zone = null;
		int num = GetParam(2)?.ToInt() ?? 0;
		if (OnlyInTheSameTopZone && this.zone != null && this.zone.GetTopZone() != EClass._zone.GetTopZone())
		{
			this.zone = null;
		}
		if (this.zone == null)
		{
			if (CreateExternalZone)
			{
				zone = EClass._zone;
			}
			else if (IsTeleport)
			{
				zone = EClass.world.region.FindZone(GetParam(1));
				if (zone != null)
				{
					zone = zone.GetTopZone();
					this.zone = zone.FindZone(num);
				}
			}
			else if (IsDownstairs || IsUpstairs)
			{
				zone = EClass._zone.GetTopZone();
				num = EClass._zone.lv + (IsUpstairs ? 1 : (IsDownstairs ? (-1) : 0));
				this.zone = EClass._zone.GetTopZone().FindZone(num);
				if (this.zone == null && EClass._zone.parent.IsRegion && ((IsUpstairs && EClass._zone.lv == -1) || (IsDownstairs && EClass._zone.lv == 1)))
				{
					EClass.pc.MoveZone(EClass._zone.parent as Zone);
					return false;
				}
			}
			if (OnlyInTheSameTopZone && this.zone != null && this.zone.GetTopZone() != EClass._zone.GetTopZone())
			{
				this.zone = null;
			}
			if (this.zone == null)
			{
				if (OnlyInTheSameTopZone && zone != null && zone.GetTopZone() != EClass._zone.GetTopZone())
				{
					zone = null;
				}
				if (zone == null || EClass._zone.isExternalZone)
				{
					Msg.SayNothingHappen();
					return false;
				}
				CreateZone(zone, num);
			}
		}
		if ((IsDownstairs || IsUpstairs) && this.zone.IDGenerator == null && EClass._zone.IDGenerator == null)
		{
			this.zone.events.AddPreEnter(new ZonePreEnterDigStairs
			{
				pos = owner.pos.Copy(),
				fromAbove = IsDownstairs,
				uidZone = EClass._zone.uid
			});
			transition = new ZoneTransition
			{
				state = ZoneTransition.EnterState.UndergroundOrSky,
				x = owner.pos.x,
				z = owner.pos.z
			};
		}
		Debug.Log(this.zone);
		EClass.pc.MoveZone(this.zone, transition);
		return true;
	}

	public Zone CreateZone(Zone dest, int destLv)
	{
		string text = dest.GetNewZoneID(destLv);
		if (CreateExternalZone)
		{
			text = owner.GetStr(30) ?? GetParam(1);
		}
		Debug.Log("Creating:" + text + "/" + destLv + "/" + EClass.sources.zones.map.ContainsKey(text) + "/" + dest);
		Zone zone = SpatialGen.Create(text, dest, register: true, owner.pos.x, owner.pos.z) as Zone;
		zone.lv = destLv;
		zone.x = dest.x;
		zone.y = dest.y;
		owner.c_uidZone = zone.uid;
		if (CreateExternalZone)
		{
			zone.isExternalZone = true;
		}
		Debug.Log("Created:" + zone.Name + "/" + zone.id);
		return zone;
	}

	public override void OnStepped(Chara c)
	{
		if (AutoEnter && c.IsPC && owner.IsInstalled && c.IsAliveInCurrentZone && (ForceEnter || !EClass.core.config.game.disableAutoStairs))
		{
			Debug.Log("OnStepped:" + EClass.pc.ai);
			AI_Goto aI_Goto = EClass.pc.ai.Current as AI_Goto;
			if ((aI_Goto != null || EClass.pc.ai is GoalManualMove) && (aI_Goto == null || aI_Goto.dest.Equals(owner.pos)))
			{
				MoveZone();
			}
		}
	}

	public virtual bool TryTeleport()
	{
		return false;
	}

	public virtual bool IsFor(Zone z)
	{
		if (z == zone)
		{
			return true;
		}
		string param = GetParam(1);
		if (!param.IsEmpty())
		{
			int num = ((GetParam(2) != null) ? GetParam(2).ToInt() : 0);
			if (z.id == param && z.lv == num)
			{
				return true;
			}
		}
		return false;
	}
}
