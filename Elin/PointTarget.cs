using System.Collections.Generic;
using UnityEngine;

public class PointTarget : EClass
{
	public Card card;

	public Area area;

	public ObjInfo obj;

	public BlockInfo block;

	public TaskPoint task;

	public IInspect target;

	public IInspect lastTarget;

	public bool hasTargetChanged;

	public bool hasValidTarget;

	public bool drawHighlight;

	public bool mouse = true;

	public bool hasInteraction;

	public bool isValid;

	public Point pos = new Point();

	public Point lastPos = new Point();

	public int index;

	public List<Card> cards = new List<Card>();

	public Chara TargetChara
	{
		get
		{
			if (card != null)
			{
				return card.Chara;
			}
			return null;
		}
	}

	public void Update(Point _pos)
	{
		pos.Set(_pos);
		isValid = pos.IsValid;
		cards.Clear();
		if (!isValid || pos.IsHidden || EClass.ui.BlockMouseOverUpdate || (mouse && (EClass.ui.isPointerOverUI || !EClass.scene.actionMode.ShowMouseoverTarget)))
		{
			Clear();
			return;
		}
		card = null;
		area = null;
		task = null;
		block = (BlockInfo._CanInspect(pos) ? BlockInfo.GetTemp(pos) : null);
		if (pos.sourceBlock.tileType.Invisible && !EClass.scene.actionMode.IsBuildMode)
		{
			block = null;
		}
		obj = (ObjInfo._CanInspect(pos) ? ObjInfo.GetTemp(pos) : null);
		if (obj != null)
		{
			target = obj;
		}
		else if (block != null)
		{
			target = block;
		}
		else
		{
			target = null;
		}
		drawHighlight = target != null;
		CellDetail detail = pos.detail;
		if (detail != null)
		{
			area = detail.area;
			task = detail.designation;
			Thing thing = null;
			Chara chara = null;
			foreach (Chara chara2 in detail.charas)
			{
				if (!ShouldIgnore(chara2))
				{
					cards.Add(chara2);
					if (chara == null || chara2.hostility < chara.hostility)
					{
						chara = chara2;
					}
				}
			}
			foreach (Thing thing2 in detail.things)
			{
				if (!ShouldIgnore(thing2))
				{
					cards.Add(thing2);
					thing = thing2;
				}
			}
			if (chara != null)
			{
				target = (card = chara);
				drawHighlight = true;
			}
			else if (thing != null)
			{
				target = (card = thing);
				drawHighlight = true;
			}
			else if (task != null)
			{
				target = task;
				drawHighlight = true;
			}
			else if (area != null && EClass.scene.actionMode.AreaHihlight != 0)
			{
				target = area;
			}
			if (cards.Count > 0 && EClass.scene.actionMode.IsBuildMode)
			{
				target = (card = cards[Mathf.Abs(index) % cards.Count]);
				drawHighlight = true;
			}
		}
		if ((target == null || target is Chara) && EClass._zone.IsRegion)
		{
			Zone zone = EClass.scene.elomap.GetZone(pos);
			if (zone != null)
			{
				target = zone;
			}
		}
		hasInteraction = target != null;
		CheckLastTarget();
	}

	public bool ShouldIgnore(Card c)
	{
		if (c.isChara)
		{
			if (c.Chara.host != null)
			{
				return true;
			}
			if (EClass.scene.actionMode.IsBuildMode)
			{
				if (!EClass.debug.ignoreBuildRule && (c.IsPC || !c.IsPCFaction))
				{
					return true;
				}
			}
			else
			{
				if ((!EClass.pc.hasTelepathy || !c.Chara.visibleWithTelepathy) && c.isHidden && !EClass.pc.canSeeInvisible)
				{
					return true;
				}
				if (c.IsPC)
				{
					return true;
				}
				if (!c.isSynced || !EClass.player.CanSee(c.Chara))
				{
					return true;
				}
			}
			return false;
		}
		if (c.trait.IsGround && !pos.cell.IsFloorWater && c.Cell.IsFloorWater)
		{
			return true;
		}
		if (EClass.scene.actionMode.IsBuildMode)
		{
			if (EClass.scene.actionMode.IsRoofEditMode())
			{
				if (!c.isRoofItem)
				{
					return true;
				}
			}
			else if (c.isRoofItem)
			{
				return true;
			}
		}
		else if (c.isHidden || c.isRoofItem || c.isMasked)
		{
			return true;
		}
		return false;
	}

	public void CycleTarget(int a)
	{
		SE.Rotate();
		index += a;
		Update(Scene.HitPoint);
		if ((bool)WidgetMouseover.Instance)
		{
			WidgetMouseover.Instance.Refresh();
		}
	}

	public bool CanCycle()
	{
		return cards.Count >= 2;
	}

	public void Clear()
	{
		card = null;
		area = null;
		target = null;
		hasInteraction = false;
		CheckLastTarget();
	}

	public void CheckLastTarget()
	{
		hasTargetChanged = target != lastTarget || !pos.Equals(lastPos);
		hasValidTarget = target != null;
		lastTarget = target;
		lastPos.Set(pos);
	}
}
