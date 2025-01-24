public class ListPeopleBed : ListPeople
{
	public TraitBed bed;

	public BedType bedType => bed.owner.c_bedType;

	public override void OnInstantiate(Chara c, ItemGeneral i)
	{
		UIButton uIButton = i.AddSubButton(EClass.core.refs.icons.bed, delegate
		{
		}, null, delegate
		{
		});
		uIButton.icon.SetAlpha((c.FindBed() != null) ? 0.9f : 0.4f);
		uIButton.SetTooltip(delegate(UITooltip t)
		{
			WriteHobbies(t, c, c.GetRoomWork());
		});
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		if (main)
		{
			if (!bed.CanAssign(c))
			{
				SE.Beep();
				return;
			}
			c.FindBed()?.RemoveHolder(c);
			bed.AddHolder(c);
		}
		else
		{
			bed.RemoveHolder(c);
		}
		MoveToOther(c);
		base.Main.OnRefreshMenu();
	}

	public override void OnList()
	{
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsPCFaction || (chara.memberType != 0 && chara.memberType != FactionMemberType.Livestock))
			{
				continue;
			}
			if (main)
			{
				if (bedType == BedType.livestock)
				{
					if (chara.memberType != FactionMemberType.Livestock)
					{
						continue;
					}
				}
				else if (chara.memberType == FactionMemberType.Livestock)
				{
					continue;
				}
				if (!bed.IsHolder(chara))
				{
					list.Add(chara);
				}
			}
			else if (bed.IsHolder(chara))
			{
				list.Add(chara);
			}
		}
	}

	public override void OnRefreshMenu()
	{
	}
}
