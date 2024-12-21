using System;
using UnityEngine;

public class AM_Build : AM_Designation<TaskBuild>
{
	public int bridgeHeight;

	public int altitude;

	public Recipe recipe;

	public Recipe _recipe;

	public Func<ButtonGrid> button;

	public TraitHouseBoard houseBoard;

	public override BuildMenu.Mode buildMenuMode => BuildMenu.Mode.Build;

	public override int CostMoney
	{
		get
		{
			if (recipe == null || recipe.UseStock)
			{
				return 0;
			}
			return 10;
		}
	}

	public override bool IsBuildMode => true;

	public override string idSound => null;

	public override bool AllowAutoClick
	{
		get
		{
			if (recipe != null)
			{
				return !(recipe is RecipeCard);
			}
			return false;
		}
	}

	public override bool AllowMiddleClickFunc => false;

	public override int SelectorHeight => bridgeHeight;

	public override BaseTileSelector.HitType hitType => recipe?.tileType.HitType ?? BaseTileSelector.HitType.None;

	public override BaseTileSelector.SelectType selectType => recipe?.tileType.SelectType ?? BaseTileSelector.SelectType.Single;

	public override BaseTileSelector.BoxType boxType => recipe?.GetBoxType() ?? BaseTileSelector.BoxType.Box;

	public override int hitW => recipe?.W ?? 1;

	public override int hitH => recipe?.H ?? 1;

	public int MaxAltitude
	{
		get
		{
			if (!IsRoofEditMode() || !(recipe.renderRow is SourceBlock.Row))
			{
				return recipe.MaxAltitude;
			}
			return 63;
		}
	}

	public bool _IsRoofEditMode(Card c)
	{
		if (Input.GetKey(KeyCode.LeftAlt))
		{
			if (c != null)
			{
				return !c.trait.CanOnlyCarry;
			}
			return true;
		}
		return false;
	}

	public override bool IsRoofEditMode(Card c = null)
	{
		return _IsRoofEditMode(recipe?.Mold);
	}

	public override bool IsFillMode()
	{
		if (!recipe.IsThing && Input.GetKey(KeyCode.LeftControl))
		{
			return EClass.debug.godBuild;
		}
		return false;
	}

	public override int TopHeight(Point p)
	{
		return bridgeHeight + ((!recipe.tileType.AltitudeAsDir) ? altitude : 0);
	}

	public override HitResult HitResultOnDesignation(Point p)
	{
		if (p.detail?.designation is TaskBuild)
		{
			return HitResult.Warning;
		}
		return HitResult.Invalid;
	}

	public override MeshPass GetGuidePass(Point point)
	{
		if (!recipe.IsBlock)
		{
			return EClass.screen.guide.passGuideFloor;
		}
		return EClass.screen.guide.passGuideBlock;
	}

	public override bool ForcedInstaComplete(TaskBuild t)
	{
		return recipe.tileType.CanInstaComplete;
	}

	public override void OnUpdateCursor()
	{
		SetCursorOnMap(CursorSystem.Build);
	}

	public override void OnActivate()
	{
		list = base.Designations.build;
		base.OnActivate();
	}

	public override void OnDeactivate()
	{
		base.Summary.SetRecipe(null);
		recipe = (mold.recipe = null);
		houseBoard = null;
		if ((bool)BuildMenu.Instance)
		{
			BuildMenu.Instance.Unselect();
		}
	}

	public override void OnCancel()
	{
		ActionMode.Inspect.Activate();
	}

	public void StartBuild(Recipe r, Func<ButtonGrid> _button)
	{
		recipe = r;
		button = _button;
		if (!base.IsActive)
		{
			Activate();
		}
		CreateNewMold();
		bridgeHeight = -1;
		SetAltitude(0);
		base.Summary.SetRecipe(r);
		EClass.ui.hud.hint.Refresh();
	}

	public override void OnCreateMold(bool processing = false)
	{
		mold.recipe = (processing ? _recipe : recipe);
		SetAltitude(altitude);
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (start != null)
		{
			mold.bridgeHeight = start.cell.bridgeHeight;
		}
		else
		{
			mold.bridgeHeight = point.cell.bridgeHeight;
		}
		if (recipe == null)
		{
			return HitResult.Default;
		}
		if (IsRoofEditMode())
		{
			return HitResult.Valid;
		}
		if (IsFillMode())
		{
			Cell cell = point.cell;
			if (recipe.IsBridge)
			{
				if (cell._bridge != start.cell._bridge)
				{
					return HitResult.NoTarget;
				}
				return HitResult.Valid;
			}
			if (recipe.IsFloor)
			{
				if (cell._floor != start.cell._floor)
				{
					return HitResult.NoTarget;
				}
				return HitResult.Valid;
			}
			if (recipe.IsBlock)
			{
				if (cell._block != start.cell._block)
				{
					return HitResult.NoTarget;
				}
				return HitResult.Valid;
			}
		}
		if (recipe.IsBridge && recipe.tileType.SelectType == BaseTileSelector.SelectType.Multiple && !point.Equals(start) && point.cell.height > bridgeHeight + altitude)
		{
			return HitResult.Invalid;
		}
		return base.HitTest(point, start);
	}

	public void FixBridge(Point point, Recipe recipe)
	{
		bridgeHeight = -1;
		if (recipe.IsBridge)
		{
			bridgeHeight = (byte)((point.cell.bridgeHeight == 0) ? (point.cell.height + recipe.tileType.MinAltitude) : point.cell.bridgeHeight);
		}
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (recipe == null)
		{
			return;
		}
		if (recipe is RecipeBridgePillar)
		{
			base.OnRenderTile(point, result, dir);
			return;
		}
		if (EClass.screen.tileSelector.start == null)
		{
			FixBridge(point, recipe);
		}
		if (recipe.IsFloorOrBridge && !recipe.IsBridge && point.cell.bridgeHeight != 0)
		{
			OnRenderTileFloor(point, result);
			return;
		}
		if (result != HitResult.Valid && result != HitResult.Warning)
		{
			base.OnRenderTile(point, result, dir);
			return;
		}
		if (bridgeHeight != -1)
		{
			Vector3 vector = point.Position();
			EClass.screen.guide.passGuideFloor.Add(vector.x, vector.y, vector.z - 0.01f, (float)result);
		}
		int desiredDir = recipe.tileType.GetDesiredDir(point, recipe._dir);
		if (desiredDir != -1)
		{
			dir = (recipe._dir = desiredDir);
		}
		bool main = !base.tileSelector.multisize || (base.tileSelector.firstInMulti && base.Summary.count == base.Summary.countValid);
		recipe.OnRenderMarker(point, active: false, result, main, dir, bridgeHeight + ((!recipe.tileType.AltitudeAsDir || IsRoofEditMode()) ? altitude : 0));
	}

	public override void OnBeforeProcessTiles()
	{
		_recipe = recipe;
		_recipe._dir = recipe._dir;
		mold.dir = recipe._dir;
		base.OnBeforeProcessTiles();
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		if (dir != -1)
		{
			mold.dir = dir;
		}
		else
		{
			mold.dir = recipe._dir;
		}
		if (point.HasDesignation)
		{
			if (point.detail.designation is TaskBuild taskBuild && recipe.IsWallOrFence && taskBuild.recipe.IsWallOrFence && taskBuild.dir != dir)
			{
				mold.dir = (dir = 2);
			}
			point.detail?.designation.Destroy();
		}
		mold.bridgeHeight = bridgeHeight;
		SetAltitude(altitude);
		base.OnProcessTiles(point, dir);
	}

	public override void OnFinishProcessTiles()
	{
		base.OnFinishProcessTiles();
		ButtonGrid buttonGrid = button();
		if (recipe.UseStock)
		{
			Thing thing = recipe.ingredients[0].thing;
			if (thing == null || thing.isDestroyed || thing.ExistsOnMap || thing.Num <= 0)
			{
				ActionMode.ignoreSound = true;
				if ((bool)buttonGrid)
				{
					buttonGrid.selected = false;
					buttonGrid.DoNormalTransition();
				}
				BuildMenu.Instance.Unselect();
				BuildMenu.Instance.RefreshCategory(EClass.player.pref.lastBuildCategory);
				ActionMode.Inspect.Activate();
				BuildMenu.Instance.Unselect();
				return;
			}
		}
		if ((bool)buttonGrid)
		{
			buttonGrid.SetRecipe();
		}
		if (recipe.IsBridge)
		{
			SetAltitude(0);
		}
	}

	public override void RotateUnderMouse()
	{
		if (recipe != null && recipe.CanRotate)
		{
			SE.Rotate();
			recipe.Rotate();
		}
	}

	public override void InputWheel(int wheel)
	{
		if (!EInput.isAltDown && !EInput.isCtrlDown)
		{
			if (recipe.MaxAltitude > 0)
			{
				ModAltitude(wheel);
				EClass.screen.tileSelector.RefreshMouseInfo(force: true);
			}
			else
			{
				base.InputWheel(wheel);
			}
		}
	}

	public void ModAltitude(int a)
	{
		altitude += a;
		if (altitude < (recipe.IsBridge ? (-10) : 0))
		{
			altitude = MaxAltitude;
		}
		if (altitude > MaxAltitude)
		{
			altitude = (recipe.IsBridge ? (-10) : 0);
		}
		SetAltitude(altitude);
	}

	public void SetAltitude(int a)
	{
		if (mold == null)
		{
			CreateNewMold();
		}
		altitude = (mold.altitude = a);
		recipe.OnChangeAltitude(a);
		if (recipe.tileType.AltitudeAsDir && !IsRoofEditMode())
		{
			mold.dir = (recipe._dir = altitude);
		}
	}
}
