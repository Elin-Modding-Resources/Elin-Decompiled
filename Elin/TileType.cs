using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TileType : EClass
{
	public enum RampType
	{
		None,
		Full,
		Half
	}

	public static TileTypeNone None = new TileTypeNone();

	public static TileTypeInvisibleBlock InvisiBlock = new TileTypeInvisibleBlock();

	public static TileTypeBlock Block = new TileTypeBlock();

	public static TileTypeBlockShip BlockShip = new TileTypeBlockShip();

	public static TileTypeSlope Slope = new TileTypeSlope();

	public static TileTypeHalfBlock HalfBlock = new TileTypeHalfBlock();

	public static TileTypeStairs Stairs = new TileTypeStairs();

	public static TileTypeStairs StairsHalf = new TileTypeStairsHalf();

	public static TileTypeRooftop Rooftop = new TileTypeRooftop();

	public static TileTypeScaffold Scaffold = new TileTypeScaffold();

	public static TileTypeLadder Ladder = new TileTypeLadder();

	public static TileTypePillar Pillar = new TileTypePillar();

	public static TileTypeWaterfall Waterfall = new TileTypeWaterfall();

	public static TileTypeWall Wall = new TileTypeWall();

	public static TileTypeWallOpen WallOpen = new TileTypeWallOpen();

	public static TileTypeFence Fence = new TileTypeFence();

	public static TileTypeFenceClosed FenceClosed = new TileTypeFenceClosed();

	public static TileTypeFloor Floor = new TileTypeFloor();

	public static TileTypeFloorScaffold FloorScaffold = new TileTypeFloorScaffold();

	public static TileTypeWater FloorWater = new TileTypeWater();

	public static TileTypeWaterShallow FloorWaterShallow = new TileTypeWaterShallow();

	public static TileTypeWaterDeep FloorWaterDeep = new TileTypeWaterDeep();

	public static TileTypeBridge Bridge = new TileTypeBridge();

	public static TileTypeBridgeDeco BridgeDeco = new TileTypeBridgeDeco();

	public static TileTypeBridgePillar BridgePillar = new TileTypeBridgePillar();

	public static TileTypeSky Sky = new TileTypeSky();

	public static TileTypeObj Obj = new TileTypeObj();

	public static TileTypeObjBig ObjBig = new TileTypeObjBig();

	public static TileTypeObjHuge ObjHuge = new TileTypeObjHuge();

	public static TileTypeObjCeil ObjCeil = new TileTypeObjCeil();

	public static TileTypeObjFloat ObjFloat = new TileTypeObjFloat();

	public static TileTypeObjWater ObjWater = new TileTypeObjWater();

	public static TileTypeObjWaterTop ObjWaterTop = new TileTypeObjWaterTop();

	public static TileTypeIllumination Illumination = new TileTypeIllumination();

	public static TileTypeTent Tent = new TileTypeTent();

	public static TileTypeSeed Seed = new TileTypeSeed();

	public static TileTypeTree Tree = new TileTypeTree();

	public static TileTypeDoor Door = new TileTypeDoor();

	public static TileTypeWallHang WallHang = new TileTypeWallHang();

	public static TileTypeWallHangSign WallHangSign = new TileTypeWallHangSign();

	public static TileTypeVine Vine = new TileTypeVine();

	public static TileTypeWallMount WallMount = new TileTypeWallMount();

	public static TileTypePaint Paint = new TileTypePaint();

	public static TileTypeWindow Window = new TileTypeWindow();

	public static TileTypeRoof Roof = new TileTypeRoof();

	public static TileTypeRoad Road = new TileTypeRoad();

	public static TileTypeChasm Chasm = new TileTypeChasm();

	public static TileTypeBoat Boat = new TileTypeBoat();

	public static TileTypeLiquid Liquid = new TileTypeLiquid();

	public static TileTypeMarker Marker = new TileTypeMarker();

	public static Dictionary<string, TileType> dict = new Dictionary<string, TileType>();

	public virtual string LangPlaceType => "place_Obj";

	public virtual bool CanStack => false;

	public virtual bool ChangeBlockDir => false;

	public virtual bool IsSkipLowBlock => false;

	public virtual bool IsSkipFloor => false;

	public virtual bool IsUseBlockDir => false;

	public virtual bool IsFloorOrBridge => false;

	public virtual bool IsWall => false;

	public virtual bool IsFloor => false;

	public virtual bool IsBridge => false;

	public virtual bool IsWallOrFence => false;

	public virtual bool IsWallOrFullBlock => false;

	public bool IsRamp => Ramp != RampType.None;

	public virtual RampType Ramp => RampType.None;

	public virtual bool IsLadder => false;

	public virtual bool IsBlockPass => false;

	public virtual bool IsOccupyCell => true;

	public virtual bool IsBlockSight => false;

	public virtual bool IsOpenSight => false;

	public virtual bool IsBlockLiquid => false;

	public virtual bool IsWater => false;

	public virtual bool IsDeepWater => false;

	public virtual bool IsBlockMount => false;

	public virtual bool IsFullBlock => false;

	public virtual bool IsFence => false;

	public virtual bool IsFloodBlock => false;

	public virtual bool IsPlayFootSound => false;

	public virtual bool CanSpawnOnWater => false;

	public virtual bool IsWaterTop => false;

	public virtual bool CastShadowSelf => false;

	public virtual bool CastShadowBack => false;

	public virtual bool CastAmbientShadow => false;

	public virtual bool CastAmbientShadowBack => false;

	public virtual bool CanBuiltOnArea => true;

	public virtual bool CanBuiltOnWater => true;

	public virtual bool CanBuiltOnThing => false;

	public virtual bool CanBuiltOnBlock => false;

	public virtual bool IsDoor => false;

	public virtual bool CanBuiltOnFloor => true;

	public virtual bool CanBuiltOnBridge => true;

	public virtual bool CanInstaComplete => false;

	public virtual int MinAltitude => 1;

	public virtual int MaxAltitude => 10;

	public virtual bool AltitudeAsDir => false;

	public virtual bool UseLowWallTiles => false;

	public virtual bool UseMountHeight => false;

	public virtual bool UseHangZFix => false;

	public virtual bool UseLowBlock => false;

	public virtual bool RemoveOnFloorChange => true;

	public virtual bool AllowObj => false;

	public virtual bool AllowMultiInstall => true;

	public virtual bool FreeStyle => false;

	public virtual byte slopeHeight => 0;

	public virtual float MountHeight => 0f;

	public virtual float FloorHeight => 0f;

	public virtual float RepeatSize => 1f;

	public virtual int FloorAltitude => 0;

	public virtual int LiquidLV => 0;

	public virtual bool AllowLitter => true;

	public virtual bool AllowBlood => true;

	public virtual bool ShowPillar => true;

	public virtual bool AlwaysShowShadow => false;

	public virtual bool RepeatBlock => false;

	public virtual bool ForceRpeatBlock => false;

	public virtual bool CanBeHeld => true;

	public virtual bool EditorTile => false;

	public virtual bool IsFloodDoor => false;

	public virtual bool Invisible => false;

	public virtual bool IgnoreBuildRule => false;

	public virtual bool RenderWaterBlock => true;

	public virtual BaseTileSelector.SelectType SelectType => BaseTileSelector.SelectType.Single;

	public virtual BaseTileSelector.BoxType BoxType => BaseTileSelector.BoxType.Box;

	public virtual BaseTileSelector.HitType HitType => BaseTileSelector.HitType.Block;

	public virtual BlockRenderMode blockRenderMode => BlockRenderMode.Default;

	public bool IsMountBlock => MountHeight > 0f;

	public static void Init()
	{
		dict.Clear();
		FieldInfo[] fields = typeof(TileType).GetFields(BindingFlags.Static | BindingFlags.Public);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (typeof(TileType).IsAssignableFrom(fieldInfo.FieldType))
			{
				dict[fieldInfo.Name] = (TileType)fieldInfo.GetValue(null);
			}
		}
	}

	public virtual bool CanRotate(bool buildMode)
	{
		return true;
	}

	public HitResult _HitTest(Point pos, Card target, bool canIgnore = true)
	{
		if ((EClass.debug.ignoreBuildRule || IgnoreBuildRule) && canIgnore)
		{
			return HitResult.Valid;
		}
		if (!CanBuiltOnArea && pos.HasArea)
		{
			return HitResult.Invalid;
		}
		if (pos.cell.IsTopWater)
		{
			if (!CanBuiltOnWater)
			{
				return HitResult.Invalid;
			}
		}
		else if (!CanBuiltOnFloor)
		{
			return HitResult.Invalid;
		}
		if (!CanBuiltOnBridge && pos.cell._bridge != 0)
		{
			return HitResult.Invalid;
		}
		if (target != null)
		{
			if (target.sourceCard.multisize && (pos.Installed != null || pos.cell.blocked || (pos.HasChara && pos.FirstChara.IsHostile(EClass.pc))))
			{
				return HitResult.Invalid;
			}
			if (pos.Installed != null && pos.Installed != target)
			{
				TileType tileType = pos.Installed.TileType;
				TileType tileType2 = target.TileType;
				if (!tileType.AllowMultiInstall || !tileType2.AllowMultiInstall)
				{
					return HitResult.Invalid;
				}
			}
			if (!target.trait.CanBuiltAt(pos))
			{
				return HitResult.Invalid;
			}
		}
		else if (pos.Installed != null && !CanBuiltOnThing && !pos.cell.hasDoor)
		{
			return HitResult.Invalid;
		}
		if (pos.HasBlock)
		{
			if (!CanBuiltOnBlock && pos.sourceBlock.tileType.IsOccupyCell && !EClass.core.config.test.allowBlockOnItem)
			{
				return HitResult.Invalid;
			}
			if (IsDoor && !pos.HasWallOrFence)
			{
			}
		}
		else if (IsDoor)
		{
			return HitResult.Invalid;
		}
		return HitTest(pos);
	}

	protected virtual HitResult HitTest(Point pos)
	{
		if (pos.HasObj)
		{
			return HitResult.Warning;
		}
		return HitResult.Valid;
	}

	public virtual int GetDesiredDir(Point p, int d)
	{
		return -1;
	}

	public virtual void GetMountHeight(ref Vector3 v, Point p, int d, Card target = null)
	{
		v += EClass.screen.tileMap.altitudeFix * target.altitude;
	}

	public Vector3 GetRampFix(int dir)
	{
		int num = (int)Ramp * 2 - 2 + ((dir > 1) ? 1 : 0);
		Vector3 vector = EClass.setting.render.rampFix[num];
		return new Vector3(vector.x * (float)((dir % 2 == 0) ? 1 : (-1)), vector.y, vector.z);
	}
}
