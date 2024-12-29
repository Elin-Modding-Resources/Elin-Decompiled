using System.Collections.Generic;

public class TraitBed : Trait
{
	public override bool CanStack => false;

	public int MaxHolders
	{
		get
		{
			if (owner.c_bedType != BedType.residentOne)
			{
				return ((GetParam(1) == null) ? 1 : GetParam(1).ToInt()) + owner.c_containerSize;
			}
			return 1;
		}
	}

	public override bool IsChangeFloorHeight => true;

	public override void TrySetAct(ActPlan p)
	{
		ValidateOwners();
		p.TrySetAct(new AI_Sleep
		{
			target = owner.Thing
		}, owner);
		p.TrySetAct(new AI_PassTime
		{
			target = owner.Thing
		}, owner);
		if (!EClass._zone.IsPCFaction)
		{
			return;
		}
		p.TrySetAct("bedConfig", delegate
		{
			UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
			if (HasHolder())
			{
				uIContextMenu.AddButton("unassignBed", delegate
				{
					ClearHolders();
					SE.Trash();
				});
			}
			else if (owner.c_bedType == BedType.resident || owner.c_bedType == BedType.residentOne)
			{
				uIContextMenu.AddButton("claimBed", delegate
				{
					ClearHolders();
					AddHolder(EClass.pc);
					Msg.Say("claimBed", EClass.pc);
					SE.Play("jingle_embark");
				});
			}
			if (owner.c_bedType == BedType.resident || owner.c_bedType == BedType.residentOne || owner.c_bedType == BedType.livestock)
			{
				uIContextMenu.AddButton("assignBed", delegate
				{
					LayerPeople.CreateBed(this);
				});
			}
			foreach (BedType t in new List<BedType>
			{
				BedType.resident,
				BedType.residentOne,
				BedType.livestock,
				BedType.guest
			})
			{
				uIContextMenu.AddButton(((t == owner.c_bedType) ? "context_checker".lang() : "") + ("bed_" + t).lang(), delegate
				{
					SetBedType(t);
					SE.ClickOk();
				});
			}
			CursorSystem.ignoreCount = 5;
			uIContextMenu.Show();
			return false;
		}, owner);
	}

	public void ValidateOwners()
	{
		CharaList data = owner.c_charaList;
		if (data == null)
		{
			return;
		}
		data.list.ForeachReverse(delegate(int i)
		{
			Chara chara = EClass._map.FindChara(i) ?? EClass.game.cards.globalCharas.Find(i);
			if (chara == null || (chara.IsPCFaction && !chara.IsPCParty && chara.homeBranch != EClass._zone.branch))
			{
				data.list.Remove(i);
			}
		});
	}

	public void AddHolder(Chara c)
	{
		CharaList charaList = owner.c_charaList;
		if (charaList == null)
		{
			charaList = (owner.c_charaList = new CharaList());
		}
		charaList.Add(c);
	}

	public void RemoveHolder(Chara c)
	{
		CharaList c_charaList = owner.c_charaList;
		if (c_charaList != null)
		{
			c_charaList.Remove(c);
			if (c_charaList.list.Count == 0)
			{
				owner.c_charaList = null;
			}
		}
	}

	public void ClearHolders()
	{
		owner.c_charaList?.list.Clear();
	}

	public bool IsHolder(Chara c)
	{
		return owner.c_charaList?.list.Contains(c.uid) ?? false;
	}

	public bool IsFull()
	{
		CharaList c_charaList = owner.c_charaList;
		if (c_charaList != null)
		{
			return c_charaList.list.Count >= MaxHolders;
		}
		return false;
	}

	public bool HasHolder()
	{
		CharaList c_charaList = owner.c_charaList;
		if (c_charaList == null)
		{
			return false;
		}
		return c_charaList.list.Count > 0;
	}

	public bool CanAssign(Chara c)
	{
		CharaList c_charaList = owner.c_charaList;
		if (c_charaList != null && c_charaList.list.Count >= MaxHolders)
		{
			return false;
		}
		BedType c_bedType = owner.c_bedType;
		FactionMemberType memberType = c.memberType;
		if ((memberType != 0 || (c_bedType != 0 && c_bedType != BedType.residentOne)) && (memberType != FactionMemberType.Livestock || c_bedType != BedType.livestock))
		{
			if (memberType == FactionMemberType.Guest)
			{
				return c_bedType == BedType.guest;
			}
			return false;
		}
		return true;
	}

	public void SetBedType(BedType bedType)
	{
		if (bedType != owner.c_bedType)
		{
			owner.c_bedType = bedType;
			ClearHolders();
		}
	}

	public override void SetName(ref string s)
	{
		CharaList c_charaList = owner.c_charaList;
		if (c_charaList != null)
		{
			List<Chara> list = c_charaList.Get();
			if (list.Count == 1)
			{
				s = "_bed".lang(list[0].NameSimple, s);
				return;
			}
			if (list.Count == 2)
			{
				s = "_bed".lang("_and".lang(list[0].NameSimple, list[1].NameSimple), s);
				return;
			}
		}
		if (owner.c_bedType != 0)
		{
			string @ref = ("bed_" + owner.c_bedType).lang().ToLower();
			s = "_of4".lang(@ref, s);
		}
	}

	public override string GetHoverText()
	{
		CharaList c_charaList = owner.c_charaList;
		if (c_charaList == null || c_charaList.Get().Count < 3)
		{
			return null;
		}
		string text = "";
		foreach (Chara item in c_charaList.Get())
		{
			if (text != "")
			{
				text += ", ";
			}
			text += item.NameSimple;
		}
		return "(" + text + ")";
	}
}
