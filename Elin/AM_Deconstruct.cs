using System.Collections.Generic;
using UnityEngine;

public class AM_Deconstruct : AM_BaseTileSelect
{
	public bool useRange;

	public bool ignoreInstalled;

	public override bool IsBuildMode => true;

	public override BaseTileMap.CardIconMode cardIconMode => BaseTileMap.CardIconMode.Deconstruct;

	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			if (!useRange)
			{
				return BaseTileSelector.SelectType.Single;
			}
			return BaseTileSelector.SelectType.Multiple;
		}
	}

	public override BaseTileSelector.HitType hitType => BaseTileSelector.HitType.Default;

	public override bool ShowMouseoverTarget => true;

	public override bool UseSubMenu => true;

	public override bool IsRoofEditMode(Card c = null)
	{
		return Input.GetKey(KeyCode.LeftAlt);
	}

	public override MeshPass GetGuidePass(Point point)
	{
		if (Perform(point) <= 0)
		{
			return base.GetGuidePass(point);
		}
		return EClass.screen.guide.passGuideBlock;
	}

	public override void OnUpdateCursor()
	{
		SetCursorOnMap(CursorSystem.Select);
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (Perform(point) > 0)
		{
			return HitResult.Valid;
		}
		return base.HitTest(point, start);
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		if (Perform(point) == 0)
		{
			SE.Beep();
		}
		else
		{
			Perform(point, perform: true);
		}
	}

	public int Perform(Point point, bool perform = false)
	{
		List<Card> list = point.ListCards();
		int num = 0;
		list.Reverse();
		foreach (Card item in list)
		{
			if ((!EClass.debug.ignoreBuildRule && (!item.isThing || !item.trait.CanPutAway)) || (ignoreInstalled && item.IsInstalled) || item.IsPCParty)
			{
				continue;
			}
			if (perform)
			{
				item.PlaySound(item.material.GetSoundDead(item.sourceCard));
				if (item.isThing)
				{
					EClass._map.PutAway(item.Thing);
				}
				else
				{
					item.Destroy();
				}
				BuildMenu.dirtyCat = true;
			}
			num++;
		}
		return num;
	}

	public override void OnClickSubMenu(int a)
	{
		switch (a)
		{
		case 0:
			useRange = !useRange;
			break;
		case 1:
			ignoreInstalled = !ignoreInstalled;
			break;
		}
	}

	public override string OnSetSubMenuButton(int a, UIButton b)
	{
		if (a < 2)
		{
			switch (a)
			{
			case 0:
				b.SetCheck(useRange);
				break;
			case 1:
				b.SetCheck(ignoreInstalled);
				break;
			}
			return "deconstructMenu" + a;
		}
		return null;
	}
}
