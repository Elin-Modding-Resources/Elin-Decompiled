using UnityEngine;

public class AM_MoveInstalled : AM_Designation<TaskMoveInstalled>
{
	public Card target;

	public Card moldCard;

	public bool onetime;

	public override int hitW
	{
		get
		{
			if (target != null)
			{
				return moldCard.W;
			}
			return 1;
		}
	}

	public override int hitH
	{
		get
		{
			if (target != null)
			{
				return moldCard.H;
			}
			return 1;
		}
	}

	public override int CostMoney
	{
		get
		{
			if (target != null)
			{
				return 0;
			}
			return 0;
		}
	}

	public override BaseTileMap.CardIconMode cardIconMode => BaseTileMap.CardIconMode.Inspect;

	public override bool AllowMiddleClickFunc => target == null;

	public bool FreePos => EClass.game.config.FreePos;

	public override BaseTileSelector.SelectType selectType => BaseTileSelector.SelectType.Single;

	public override bool ShouldHideBuildMenu => target != null;

	public override bool IsRoofEditMode(Card c = null)
	{
		return ActionMode.Build._IsRoofEditMode(target);
	}

	public override HitResult HitResultOnDesignation(Point p)
	{
		return HitResult.Invalid;
	}

	public override bool CanInstaComplete(TaskMoveInstalled t)
	{
		return EClass.player.instaComplete;
	}

	public override MeshPass GetGuidePass(Point point)
	{
		return EClass.screen.guide.passGuideBlock;
	}

	public void Activate(Thing t)
	{
		Activate();
		SetTarget(t);
		onetime = true;
		EClass.ui.hud.hint.UpdateText();
	}

	public override void OnActivate()
	{
		onetime = false;
		list = base.Designations.moveInstalled;
		target = (moldCard = null);
		base.OnActivate();
	}

	public override void OnDeactivate()
	{
		target = (moldCard = (mold.target = null));
	}

	public override void OnCreateMold(bool processing)
	{
		mold.target = target;
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (EClass.debug.enable)
		{
			switch (EInput.hotkey)
			{
			case 0:
				EClass._map.SetDecal(point.x, point.z);
				break;
			case 1:
				EClass._map.AddDecal(point.x, point.z, 2);
				break;
			}
		}
		if (target == null)
		{
			if (EClass.scene.mouseTarget.CanCycle())
			{
				return HitResult.Warning;
			}
			if (GetTarget(point) != null)
			{
				return HitResult.Valid;
			}
			return HitResult.Default;
		}
		if (target.isChara && CheckEnemyNearBy(target, point, msg: false))
		{
			return HitResult.Invalid;
		}
		moldCard.ignoreStackHeight = Input.GetKey(KeyCode.LeftControl);
		if (!EClass.debug.ignoreBuildRule)
		{
			if (target.isChara && (point.IsBlocked || point.HasChara))
			{
				return HitResult.Invalid;
			}
			if (!EClass._map.bounds.Contains(point))
			{
				return HitResult.Invalid;
			}
		}
		return base.HitTest(point, start);
	}

	public void SetTarget(Card _target)
	{
		TaskMoveInstalled[] array = EClass._map.tasks.designations.moveInstalled.items.ToArray();
		foreach (TaskMoveInstalled taskMoveInstalled in array)
		{
			if (taskMoveInstalled.target == _target)
			{
				taskMoveInstalled.Destroy();
			}
		}
		target = (mold.target = _target);
		if (target.isThing)
		{
			moldCard = target.Duplicate(1);
			moldCard.placeState = PlaceState.installed;
		}
		else
		{
			moldCard = CharaGen.Create(_target.id);
			moldCard.bio.SetGender(_target.bio.gender);
			moldCard.idSkin = target.idSkin;
		}
		EClass.ui.hud.hint.UpdateText();
	}

	public Card GetTarget(Point point)
	{
		return EClass.scene.mouseTarget.card;
	}

	public bool CanPutAway()
	{
		if (EClass.debug.ignoreBuildRule)
		{
			return true;
		}
		if (target == null || target.isChara || target.trait.CanOnlyCarry)
		{
			return false;
		}
		return true;
	}

	public bool TryPutAway()
	{
		if (!CanPutAway())
		{
			return false;
		}
		if (!EClass._map.PutAway(target))
		{
			SE.Beep();
			return false;
		}
		SE.Click();
		target = null;
		EClass.ui.hud.hint.UpdateText();
		return true;
	}

	public bool CheckEnemyNearBy(Card t, Point p, bool msg = true)
	{
		if (t.isChara && !EClass.debug.ignoreBuildRule)
		{
			bool flag = true;
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara.hostility <= Hostility.Enemy && chara.Dist(p) <= 6)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				if (msg)
				{
					SE.BeepSmall();
					EClass.ui.Say("enemyInMap".langGame());
				}
				return true;
			}
		}
		return false;
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		if (target == null)
		{
			Card t = GetTarget(point);
			if (CheckEnemyNearBy(t, t.pos))
			{
				return;
			}
			if (t.trait.ShowContextOnPick)
			{
				ActPlan actPlan = new ActPlan
				{
					pos = point.Copy(),
					ignoreAdddCondition = true,
					input = ActInput.AllAction,
					altAction = true
				};
				t.trait.TrySetAct(actPlan);
				if (actPlan.list.Count > 0)
				{
					UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
					uIContextMenu.AddButton("move", delegate
					{
						SetTarget(t);
						SE.Click();
					});
					foreach (ActPlan.Item i in actPlan.list)
					{
						uIContextMenu.AddButton(i.GetTextContext(showName: false), delegate
						{
							i.act.Perform();
						});
					}
					uIContextMenu.Show();
					return;
				}
			}
			SetTarget(t);
			SE.Click();
		}
		else
		{
			if (CheckEnemyNearBy(target, point))
			{
				return;
			}
			mold.dir = moldCard.dir;
			mold.altitude = moldCard.altitude;
			if (target.isChara)
			{
				target.Chara.orgPos?.Set(point);
			}
			base.OnProcessTiles(point, dir);
			target.ignoreStackHeight = moldCard.ignoreStackHeight;
			target.freePos = moldCard.freePos;
			target.fx = (FreePos ? moldCard.fx : 0f);
			target.fy = (FreePos ? moldCard.fy : 0f);
			if (target.isChara && (bool)EClass.debug)
			{
				foreach (Thing thing in point.Things)
				{
					if (thing.trait is TraitShackle traitShackle)
					{
						traitShackle.Restrain(target.Chara);
					}
				}
			}
			if (target.isThing)
			{
				target.isRoofItem = IsRoofEditMode();
				if (target.isRoofItem)
				{
					target.SetPlaceState(PlaceState.roaming);
				}
				if (!target.isRoofItem)
				{
					target.ForeachPoint(delegate(Point p, bool center)
					{
						if (p.IsBlocked && p.HasChara)
						{
							foreach (Chara item in p.ListCharas())
							{
								EClass.pc.Kick(item, ignoreSelf: true, karmaLoss: false, show: false);
							}
						}
					});
				}
			}
			SE.Click();
			if (target.renderer.hasActor)
			{
				target.renderer.RefreshSprite();
			}
			target = null;
		}
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (target == null)
		{
			Card card = GetTarget(point);
			if (card != null && card.isThing)
			{
				card.Thing.RenderMarker(point, active: true, result, main: true, -1, useCurrentPosition: true);
			}
			base.OnRenderTile(point, (result != HitResult.Valid) ? result : (EClass.scene.mouseTarget.CanCycle() ? HitResult.Warning : HitResult.Default), dir);
			return;
		}
		if (moldCard == null)
		{
			base.OnRenderTile(point, result, dir);
			return;
		}
		moldCard.SetFreePos(point);
		if (result != HitResult.Valid && result != HitResult.Warning)
		{
			base.OnRenderTile(point, result, dir);
		}
		else
		{
			int desiredDir = target.TileType.GetDesiredDir(point, moldCard.dir);
			if (desiredDir != -1)
			{
				int num2 = (moldCard.dir = desiredDir);
				dir = num2;
			}
			bool flag = !base.tileSelector.multisize || (base.tileSelector.firstInMulti && base.Summary.count == base.Summary.countValid);
			moldCard.RenderMarker(point, active: false, result, flag, -1);
			if (flag)
			{
				target.trait.OnRenderTile(point, result, dir);
			}
		}
		EClass.screen.guide.DrawLine(target.pos.PositionCenter(), point.PositionCenter());
	}

	public override void RotateUnderMouse()
	{
		if (target != null)
		{
			SE.Rotate();
			moldCard.Rotate();
		}
		else
		{
			base.RotateUnderMouse();
		}
	}

	public override void InputWheel(int wheel)
	{
		if (!EInput.isAltDown && !EInput.isCtrlDown)
		{
			if (target != null && target.TileType.MaxAltitude > 0)
			{
				moldCard.ChangeAltitude(wheel);
			}
			else if (EClass.scene.mouseTarget.CanCycle())
			{
				EClass.scene.mouseTarget.CycleTarget(wheel);
			}
			else
			{
				base.InputWheel(wheel);
			}
		}
	}

	public override void OnCancel()
	{
		if (target != null)
		{
			target = null;
			if (onetime)
			{
				base.OnCancel();
			}
		}
		else
		{
			base.OnCancel();
		}
	}

	public override void OnFinishProcessTiles()
	{
		if (onetime)
		{
			Deactivate();
		}
	}
}
