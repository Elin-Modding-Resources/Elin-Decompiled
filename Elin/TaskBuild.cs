using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class TaskBuild : TaskBaseBuild
{
	[JsonProperty]
	public Recipe recipe;

	[JsonProperty]
	public int dir;

	[JsonProperty]
	public int bridgeHeight;

	[JsonProperty]
	public int altitude;

	[JsonProperty]
	public List<Thing> resources = new List<Thing>();

	[JsonProperty]
	public int[] reqs;

	public Card held;

	public float fx;

	public float fy;

	public bool freePos;

	public bool disableRotateBlock;

	public Card target;

	public Point lastPos;

	public override int destDist => 1;

	public override bool CanPressRepeat
	{
		get
		{
			if (useHeld)
			{
				if (EClass.pc.held != null)
				{
					return EClass.pc.held.trait.CanExtendBuild;
				}
				return false;
			}
			return false;
		}
	}

	public override bool HasProgress => false;

	public override bool destIgnoreConnection => recipe.IsFloorOrBridge;

	public bool useHeld => held != null;

	public override int W => recipe.W;

	public override int H => recipe.H;

	public override bool isBlock => recipe.IsBlock;

	public override bool CanManualCancel()
	{
		return true;
	}

	public bool CanRotateBlock()
	{
		if (!EInput.rightMouse.pressing)
		{
			disableRotateBlock = false;
		}
		if (useHeld && EClass.pc.held != null)
		{
			if (EClass._zone.IsRegion)
			{
				return false;
			}
			if (!(EClass._zone is Zone_Tent) && !EClass._zone.IsPCFaction && EClass.pc.held.trait.CanBeOnlyBuiltInHome)
			{
				return false;
			}
			if (EClass._zone.RestrictBuild && !EClass.pc.held.trait.CanBuildInTown)
			{
				return false;
			}
			if (EClass.pc.held.trait is TraitBlock && pos.HasBlock && !EClass.pc.held.trait.IsDoor && !disableRotateBlock)
			{
				return true;
			}
		}
		return false;
	}

	public override string GetText(string str = "")
	{
		if (CanRotateBlock())
		{
			return "actRotateWall".lang();
		}
		if (useHeld && EClass.pc.held != null)
		{
			if (EClass.pc.held.category.id == "seed")
			{
				return "actInstallSeed".lang();
			}
			if (EClass.pc.held.id == "fertilizer")
			{
				return "actInstallFertilizer".lang();
			}
		}
		return base.GetText(str);
	}

	public override bool _CanPerformTask(Chara chara, int radius)
	{
		Recipe.Ingredient ingredient = recipe.ingredients[0];
		if (!ingredient.IsThingSpecified && ingredient.thing == null)
		{
			ingredient.thing = EClass._map.Stocked.Find(ingredient.id, ingredient.mat, ingredient.refVal);
			if (ingredient.thing == null)
			{
				TryLater();
				return false;
			}
		}
		return true;
	}

	public override void OnAdd()
	{
		if (reqs == null)
		{
			reqs = new int[recipe.ingredients.Count];
			for (int i = 0; i < recipe.ingredients.Count; i++)
			{
				reqs[i] = recipe.ingredients[i].req;
			}
		}
	}

	public override HitResult GetHitResult()
	{
		if (!pos.IsValid || ((useHeld ? (!pos.IsInBounds) : (!pos.IsInBoundsPlus)) && !EClass.debug.ignoreBuildRule))
		{
			return HitResult.Invalid;
		}
		if (recipe == null)
		{
			Debug.Log("recipe is null");
			return HitResult.Invalid;
		}
		if (useHeld)
		{
			if (EClass.pc.held == null || EClass.pc.held.GetRootCard() != EClass.pc || EClass.pc.held != held)
			{
				return HitResult.Invalid;
			}
			if (EClass.pc.held.TileType.IsBlockPass && pos.HasChara && pos.FirstChara.IsHostile(EClass.pc))
			{
				return HitResult.Invalid;
			}
			if (CanRotateBlock())
			{
				return HitResult.Valid;
			}
			if (pos.HasBlock && EClass.pc.held.TileType.IsDoor && pos.HasWallOrFence && pos.cell.blockDir != 2)
			{
				recipe.SetDir((pos.cell.blockDir != 0) ? 1 : 0);
			}
			if (!EClass.debug.ignoreBuildRule && !EClass._zone.IsPCFaction)
			{
				if (!(EClass._zone is Zone_Tent) && !EClass._zone.IsPCFaction && EClass.pc.held.trait.CanBeOnlyBuiltInHome)
				{
					return HitResult.Invalid;
				}
				if (EClass._zone.RestrictBuild && !EClass.pc.held.trait.CanBuildInTown)
				{
					return HitResult.Invalid;
				}
			}
		}
		if (lastPos != null)
		{
			if (recipe.IsBlock && lastPos.x != pos.x && lastPos.z != pos.z)
			{
				return HitResult.Invalid;
			}
			if (recipe.IsWallOrFence && ((recipe._dir == 0 && pos.z != lastPos.z) || (recipe._dir == 1 && pos.x != lastPos.x)))
			{
				return HitResult.Invalid;
			}
		}
		if (0 == 0 && recipe.HasSameTile(pos, recipe._dir, altitude, bridgeHeight))
		{
			return HitResult.Default;
		}
		if (recipe.IsThing)
		{
			if (recipe.renderRow is CardRow cardRow && !CanPlaceCard(pos, cardRow.model))
			{
				return HitResult.Invalid;
			}
			if (recipe.MultiSize && useHeld)
			{
				Point point = new Point();
				for (int i = 0; i < H; i++)
				{
					for (int j = 0; j < W; j++)
					{
						point.Set(pos.x - j, pos.z + i);
						if (!point.IsValid)
						{
							return HitResult.Invalid;
						}
						HitResult hitResult = _GetHitResult(point);
						if (hitResult != HitResult.Valid && hitResult != HitResult.Warning)
						{
							return HitResult.Invalid;
						}
					}
				}
				return HitResult.Valid;
			}
		}
		if (recipe.IsWallOrFence && pos.HasWallOrFence && AM_Adv.actCount == 0 && recipe._dir != pos.cell.blockDir)
		{
			return HitResult.Valid;
		}
		if (!useHeld && EClass.debug.ignoreBuildRule)
		{
			return HitResult.Valid;
		}
		if (EClass.scene.actionMode.IsRoofEditMode())
		{
			return HitResult.Valid;
		}
		return _GetHitResult(pos);
	}

	public HitResult _GetHitResult(Point p)
	{
		if (useHeld)
		{
			if (EClass.pc.held == null || EClass.pc.held.isDestroyed)
			{
				return HitResult.Invalid;
			}
			if (p.Installed != null)
			{
				if (p.Installed.trait is TraitSeed && !(EClass.pc.held.trait is TraitFertilizer))
				{
					return HitResult.Invalid;
				}
				if (EClass.pc.held.trait is TraitFertilizer && p.Things.LastItem().trait is TraitFertilizer)
				{
					return HitResult.Invalid;
				}
				if (EClass.pc.held.trait is TraitSeed)
				{
					foreach (Thing thing in p.Things)
					{
						if (thing.trait is TraitSeed)
						{
							return HitResult.Invalid;
						}
					}
				}
			}
		}
		return recipe.tileType._HitTest(p, recipe.Mold);
	}

	public override void OnProgressComplete()
	{
		if (useHeld)
		{
			if (EClass.pc.held == null || EClass.pc.held.GetRootCard() != EClass.pc || pos.Distance(EClass.pc.pos) > 1 || !pos.IsInBounds)
			{
				return;
			}
			if (CanRotateBlock())
			{
				SE.Rotate();
				pos.cell.RotateBlock(1);
				disableRotateBlock = true;
				return;
			}
			disableRotateBlock = true;
			ActionMode.Build.FixBridge(pos, recipe);
			bridgeHeight = ActionMode.Build.bridgeHeight;
			target = (EClass.pc.held.category.installOne ? EClass.pc.held.Split(1) : EClass.pc.held);
			if (target.trait is TraitTile)
			{
				target.ModNum(-1);
			}
			dir = recipe._dir;
			EClass.pc.LookAt(pos);
			EClass.pc.renderer.PlayAnime(AnimeID.Attack_Place, pos);
			if (target.id == "statue_weird")
			{
				EClass.pc.Say("statue_install");
			}
		}
		lastPos = pos.Copy();
		if (ActionMode.Build.IsActive && ActionMode.Build.IsFillMode())
		{
			if (recipe.IsBridge)
			{
				dir = pos.cell.floorDir;
				bridgeHeight = pos.cell.bridgeHeight;
				altitude = 0;
			}
			else if (recipe.IsFloor)
			{
				dir = pos.cell.floorDir;
			}
			else if (recipe.IsBlock)
			{
				dir = pos.cell.blockDir;
			}
		}
		else
		{
			Effect.Get("smoke").Play(pos);
			Effect.Get("mine").Play(pos).SetParticleColor(recipe.GetColorMaterial().GetColor())
				.Emit(10 + EClass.rnd(10));
			if (recipe.IsWallOrFence)
			{
				if (pos.HasWallOrFence && pos.cell.blockDir != 2 && pos.cell.blockDir != recipe._dir)
				{
					pos.cell.blockDir = 2;
					EClass.pc.PlaySound(pos.matBlock.GetSoundImpact());
					pos.RefreshTile();
					return;
				}
				if (pos.sourceRoofBlock.tileType.IsWallOrFence && pos.cell._roofBlockDir % 4 != 2 && pos.cell._roofBlockDir % 4 != recipe._dir)
				{
					pos.cell._roofBlockDir = (byte)(pos.cell._roofBlockDir / 4 * 4 + 2);
					EClass.pc.PlaySound(pos.matBlock.GetSoundImpact());
					pos.RefreshTile();
					return;
				}
			}
		}
		if (bridgeHeight > 150)
		{
			bridgeHeight = 150;
		}
		recipe.Build(this);
		resources.Clear();
		EClass.player.flags.OnBuild(recipe);
		EClass._map.RefreshShadow(pos.x, pos.z);
		EClass._map.RefreshShadow(pos.x, pos.z - 1);
		EClass._map.RefreshFOV(pos.x, pos.z);
		EClass.pc.renderer.SetFirst(first: true);
		if (recipe.IsFloor)
		{
			foreach (Card item in pos.ListThings<TraitNewZone>())
			{
				_ = (item.trait as TraitNewZone).IsDownstairs;
			}
		}
		if (ActionMode.Build.IsActive && ActionMode.Build.IsRoofEditMode())
		{
			return;
		}
		pos.ForeachMultiSize(recipe.W, recipe.H, delegate(Point p, bool center)
		{
			if (p.IsBlocked && p.HasChara)
			{
				foreach (Chara item2 in p.ListCharas())
				{
					EClass.pc.Kick(item2, ignoreSelf: true, karmaLoss: false, show: false);
				}
			}
		});
	}

	public override void OnDestroy()
	{
		foreach (Thing resource in resources)
		{
			EClass._zone.AddCard(resource, pos);
		}
	}

	public override void DrawMarker(int x, int z, RenderParam p)
	{
		recipe.OnRenderMarker(Point.shared.Set(x, z), owner != null, HitResult.Default, x == pos.x && z == pos.z, dir, bridgeHeight + altitude);
	}
}
