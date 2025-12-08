using System;
using System.Collections.Generic;

public class AM_Picker : AM_BaseTileSelect
{
	public struct Result
	{
		public RecipeSource source;

		public SourceMaterial.Row mat;

		public Thing thing;

		public bool IsValid => source != null;

		public string category => source.recipeCat;

		public string GetText()
		{
			return source.Name + (EClass.debug.showExtra ? (":" + source.id) : "") + "(" + mat.GetName() + (EClass.debug.showExtra ? (":" + mat.id) : "") + ")" + (EClass.player.recipes.IsKnown(source.id) ? "" : (Environment.NewLine + "noRecipe".lang()));
		}
	}

	public override bool IsBuildMode => true;

	public override BaseTileSelector.HitType hitType => EClass.core.screen.tileSelector.inspectHitType;

	public override BaseTileSelector.SelectType selectType => BaseTileSelector.SelectType.Single;

	public BuildMenu BM => BuildMenu.Instance;

	public bool CanActivate
	{
		get
		{
			if (!base.IsActive)
			{
				if (!EClass.debug.godBuild)
				{
					if (EClass.Branch != null)
					{
						return EClass.Branch.elements.Has(4005);
					}
					return false;
				}
				return true;
			}
			return false;
		}
	}

	public override void OnUpdateCursor()
	{
		SetCursorOnMap(CursorSystem.Picker);
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (!Test(point).IsValid)
		{
			return HitResult.Default;
		}
		return HitResult.Valid;
	}

	public override void OnProcessTiles(Point point, int dir)
	{
		Test(point, select: true);
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (point.cell.HasWallOrFence && (result == HitResult.Valid || result == HitResult.Invalid))
		{
			EClass.screen.guide.DrawWall(point, (result == HitResult.Valid) ? EClass.Colors.blockColors.Valid : EClass.Colors.blockColors.Warning);
		}
		else
		{
			base.OnRenderTile(point, result, dir);
		}
	}

	public Result TestBlock(Point point)
	{
		Result result = default(Result);
		result.mat = point.matBlock;
		result.source = TryGetRecipe(point.sourceBlock.RecipeID);
		return result;
	}

	public Result TestObj(Point point)
	{
		Result result = default(Result);
		result.mat = point.cell.matObj;
		result.source = TryGetRecipe(point.sourceObj.RecipeID);
		return result;
	}

	public Result TestFloor(Point point)
	{
		Result result = default(Result);
		if (point.cell._bridge == 0)
		{
			result.mat = point.matFloor;
			result.source = TryGetRecipe(point.sourceFloor.RecipeID);
		}
		else
		{
			result.mat = point.cell.matBridge;
			result.source = TryGetRecipe(point.cell.sourceBridge.RecipeID + "-b");
		}
		return result;
	}

	public Result TestThing(Thing t)
	{
		Result result = default(Result);
		if (t.trait.IsGround)
		{
			return result;
		}
		result.mat = t.material;
		result.source = TryGetRecipe(t.sourceCard.RecipeID);
		result.thing = t;
		return result;
	}

	public Result Test(Point point, bool select = false)
	{
		List<Card> list = point.ListCards(includeMasked: true);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			Card card = list[num];
			if (card.isThing)
			{
				Result result = TestThing(card.Thing);
				if (result.IsValid)
				{
					if (select)
					{
						Select(result);
					}
					if (card.trait is TraitHouseBoard houseBoard)
					{
						ActionMode.Build.houseBoard = houseBoard;
					}
					return result;
				}
			}
		}
		if (point.HasBlock)
		{
			Result result2 = TestBlock(point);
			if (result2.IsValid)
			{
				if (select)
				{
					Select(result2);
				}
				return result2;
			}
		}
		if (EClass.debug.godBuild && point.HasObj)
		{
			Result result3 = TestObj(point);
			if (result3.IsValid)
			{
				if (select)
				{
					Select(result3);
				}
				return result3;
			}
		}
		if (point.HasFloor)
		{
			Result result4 = TestFloor(point);
			if (result4.IsValid)
			{
				if (select)
				{
					Select(result4);
				}
				return result4;
			}
		}
		return default(Result);
	}

	public RecipeSource TryGetRecipe(string id)
	{
		return EClass.player.recipes.GetSource(id);
	}

	public new bool Select(Result r)
	{
		ActionMode.ignoreSound = true;
		BM.Select(r);
		return true;
	}
}
