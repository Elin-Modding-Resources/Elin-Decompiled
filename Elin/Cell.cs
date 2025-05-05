using System.Collections.Generic;
using Algorithms;
using FloodSpill;
using UnityEngine;

public class Cell : WeightCell, IFloodCell
{
	public static Map map;

	public static Cell Void = new Cell();

	public static List<SourceMaterial.Row> matList;

	public static List<SourceBlock.Row> blockList;

	public static List<SourceFloor.Row> floorList;

	public static List<SourceCellEffect.Row> effectList;

	public static List<SourceObj.Row> objList;

	public static int Size;

	public static Cell[,] cells;

	public const int DivAutotile = 12;

	public byte _dirs;

	public byte _block;

	public byte _blockMat;

	public byte _floor;

	public byte _floorMat;

	public byte obj;

	public byte objVal;

	public byte objMat;

	public byte decal;

	public byte _bridge;

	public byte _bridgeMat;

	public byte _roofBlock;

	public byte _roofBlockMat;

	public byte _roofBlockDir;

	public byte x;

	public byte z;

	public byte light;

	public byte autotile;

	public byte autotileBridge;

	public byte autotileObj;

	public byte shore;

	public byte shadow;

	public byte height;

	public byte bridgeHeight;

	public byte shadowMod;

	public byte topHeight;

	public byte minHeight;

	public byte bridgePillar;

	public byte highlight;

	public ushort lightR;

	public ushort lightG;

	public ushort lightB;

	public CellDetail detail;

	public CellEffect effect;

	public Critter critter;

	public BitArray32 bits;

	public BitArray32 bits2;

	public Room room;

	public bool pcSync;

	public const int DIV_LIQUID = 10;

	private static bool openSight;

	private static bool openPath;

	public const int DivStage = 30;

	public int index => x + z * Size;

	public byte TopHeight
	{
		get
		{
			if (bridgeHeight != 0)
			{
				return bridgeHeight;
			}
			return height;
		}
	}

	public Cell Front
	{
		get
		{
			if (z <= 0)
			{
				return Void;
			}
			return cells[x, z - 1];
		}
	}

	public Cell Right
	{
		get
		{
			if (x + 1 >= Size)
			{
				return Void;
			}
			return cells[x + 1, z];
		}
	}

	public Cell Back
	{
		get
		{
			if (z + 1 >= Size)
			{
				return Void;
			}
			return cells[x, z + 1];
		}
	}

	public Cell Left
	{
		get
		{
			if (x <= 0)
			{
				return Void;
			}
			return cells[x - 1, z];
		}
	}

	public Cell FrontRight
	{
		get
		{
			if (x + 1 >= Size || z <= 0)
			{
				return Void;
			}
			return cells[x + 1, z - 1];
		}
	}

	public Cell FrontLeft
	{
		get
		{
			if (x <= 0 || z <= 0)
			{
				return Void;
			}
			return cells[x - 1, z - 1];
		}
	}

	public Cell BackRight
	{
		get
		{
			if (x + 1 >= Size || z + 1 >= Size)
			{
				return Void;
			}
			return cells[x + 1, z + 1];
		}
	}

	public Cell BackLeft
	{
		get
		{
			if (x <= 0 || z + 1 >= Size)
			{
				return Void;
			}
			return cells[x - 1, z + 1];
		}
	}

	public GrowSystem growth
	{
		get
		{
			GrowSystem.cell = this;
			return sourceObj.growth;
		}
	}

	public bool isSurrounded
	{
		get
		{
			return bits[0];
		}
		set
		{
			bits[0] = value;
		}
	}

	public bool isShadowed
	{
		get
		{
			return bits[1];
		}
		set
		{
			bits[1] = value;
		}
	}

	public bool hasDoor
	{
		get
		{
			return bits[3];
		}
		set
		{
			bits[3] = value;
		}
	}

	public bool skipRender
	{
		get
		{
			return bits[4];
		}
		set
		{
			bits[4] = value;
		}
	}

	public bool isSeen
	{
		get
		{
			return bits[6];
		}
		set
		{
			bits[6] = value;
		}
	}

	public bool isSurrounded4d
	{
		get
		{
			return bits[7];
		}
		set
		{
			bits[7] = value;
		}
	}

	public bool isForceFloat
	{
		get
		{
			return bits[8];
		}
		set
		{
			bits[8] = value;
		}
	}

	public bool blockSight
	{
		get
		{
			return bits[9];
		}
		set
		{
			bits[9] = value;
		}
	}

	public bool isHarvested
	{
		get
		{
			return bits[10];
		}
		set
		{
			bits[10] = value;
		}
	}

	public bool isWatered
	{
		get
		{
			return bits[11];
		}
		set
		{
			bits[11] = value;
		}
	}

	public bool isSlopeEdge
	{
		get
		{
			return bits[12];
		}
		set
		{
			bits[12] = value;
		}
	}

	public bool isBridgeEdge
	{
		get
		{
			return bits[13];
		}
		set
		{
			bits[13] = value;
		}
	}

	public bool ignoreObjShadow
	{
		get
		{
			return bits[14];
		}
		set
		{
			bits[14] = value;
		}
	}

	public bool nonGradient
	{
		get
		{
			return bits[15];
		}
		set
		{
			bits[15] = value;
		}
	}

	public bool impassable
	{
		get
		{
			return bits[16];
		}
		set
		{
			bits[16] = value;
		}
	}

	public bool outOfBounds
	{
		get
		{
			return bits[17];
		}
		set
		{
			bits[17] = value;
		}
	}

	public bool isWallEdge
	{
		get
		{
			return bits[20];
		}
		set
		{
			bits[20] = value;
		}
	}

	public bool isModified
	{
		get
		{
			return bits[21];
		}
		set
		{
			bits[21] = value;
		}
	}

	public bool isClearSnow
	{
		get
		{
			return bits[22];
		}
		set
		{
			bits[22] = value;
		}
	}

	public bool fullWall
	{
		get
		{
			return bits[23];
		}
		set
		{
			bits[23] = value;
		}
	}

	public bool isFloating
	{
		get
		{
			return bits[24];
		}
		set
		{
			bits[24] = value;
		}
	}

	public bool lotWall
	{
		get
		{
			return bits[25];
		}
		set
		{
			bits[25] = value;
		}
	}

	public bool hasDoorBoat
	{
		get
		{
			return bits[26];
		}
		set
		{
			bits[26] = value;
		}
	}

	public bool isDeck
	{
		get
		{
			return bits[27];
		}
		set
		{
			bits[27] = value;
		}
	}

	public bool castFloorShadow
	{
		get
		{
			return bits[28];
		}
		set
		{
			bits[28] = value;
		}
	}

	public bool lotShade
	{
		get
		{
			return bits[29];
		}
		set
		{
			bits[29] = value;
		}
	}

	public bool isShoreSand
	{
		get
		{
			return bits[30];
		}
		set
		{
			bits[30] = value;
		}
	}

	public bool isToggleWallPillar
	{
		get
		{
			return bits[31];
		}
		set
		{
			bits[31] = value;
		}
	}

	public bool hasWindow
	{
		get
		{
			return bits2[0];
		}
		set
		{
			bits2[0] = value;
		}
	}

	public bool isCurtainClosed
	{
		get
		{
			return bits2[1];
		}
		set
		{
			bits2[1] = value;
		}
	}

	public bool isSkyFloor
	{
		get
		{
			return bits2[2];
		}
		set
		{
			bits2[2] = value;
		}
	}

	public bool isClearArea
	{
		get
		{
			return bits2[3];
		}
		set
		{
			bits2[3] = value;
		}
	}

	public bool isObjDyed
	{
		get
		{
			return bits2[4];
		}
		set
		{
			bits2[4] = value;
		}
	}

	public bool crossWall
	{
		get
		{
			return bits2[5];
		}
		set
		{
			bits2[5] = value;
		}
	}

	public bool HasObj => obj != 0;

	public bool HasBlock => _block != 0;

	public bool HasFloor => _floor != 0;

	public bool HasRoof
	{
		get
		{
			if (room != null)
			{
				return room.lot.idRoofStyle != 0;
			}
			return false;
		}
	}

	public bool HasFloorOrBlock
	{
		get
		{
			if (_block == 0)
			{
				return _floor != 0;
			}
			return true;
		}
	}

	public bool HasBridge => _bridge != 0;

	public bool HasLiquid => effect?.IsLiquid ?? false;

	public bool HasFire
	{
		get
		{
			if (effect != null)
			{
				return effect.IsFire;
			}
			return false;
		}
	}

	public bool IsNotWaterEdge
	{
		get
		{
			if (!HasFullBlock && !floorList[_floor].tileType.IsWater)
			{
				return floorList[_floor].edge == 0;
			}
			return true;
		}
	}

	public bool IsTopWater
	{
		get
		{
			if (_bridge != 0)
			{
				return floorList[_bridge].tileType.IsWater;
			}
			return floorList[_floor].tileType.IsWater;
		}
	}

	public bool IsTopWaterAndNoSnow
	{
		get
		{
			if (IsTopWater)
			{
				if (map.zone.IsSnowCovered && !HasRoof)
				{
					return isClearSnow;
				}
				return true;
			}
			return false;
		}
	}

	public bool IsFloorWater => floorList[_floor].tileType.IsWater;

	public bool IsFarmField
	{
		get
		{
			if (_bridge != 0)
			{
				return sourceBridge.alias == "field";
			}
			return sourceFloor.alias == "field";
		}
	}

	public bool CanGrowWeed
	{
		get
		{
			if (obj == 0 && FirstThing == null && !HasBlock && !HasRoof)
			{
				if (!IsFarmField)
				{
					if (_bridge != 0)
					{
						if (matBridge.category == "grass")
						{
							return sourceBridge.tag.Contains("weed");
						}
						return false;
					}
					if (matFloor.category == "grass")
					{
						return sourceFloor.tag.Contains("weed");
					}
					return false;
				}
				return true;
			}
			return false;
		}
	}

	public bool IsBridgeWater
	{
		get
		{
			if (_bridge != 0)
			{
				return floorList[_bridge].tileType.IsWater;
			}
			return false;
		}
	}

	public bool IsSnowTile
	{
		get
		{
			if (map.zone.IsSnowCovered && !HasRoof && !IsTopWater && !isClearSnow && !isFloating)
			{
				return !isDeck;
			}
			return false;
		}
	}

	public bool IsDeepWater
	{
		get
		{
			if (sourceSurface.tileType.IsDeepWater)
			{
				return !IsIceTile;
			}
			return false;
		}
	}

	public bool IsIceTile
	{
		get
		{
			if (map.zone.IsSnowCovered && !HasRoof && IsTopWater)
			{
				return !isClearSnow;
			}
			return false;
		}
	}

	public bool HasFullBlock => blockList[_block].tileType.IsFullBlock;

	public bool HasFullBlockOrWallOrFence
	{
		get
		{
			if (!HasFullBlock)
			{
				return HasWallOrFence;
			}
			return true;
		}
	}

	public bool HasWallOrFence => blockList[_block].tileType.IsWallOrFence;

	public bool HasWall => blockList[_block].tileType.IsWall;

	public bool HasFence => blockList[_block].tileType.IsFence;

	public bool HasRamp => blockList[_block].tileType.IsRamp;

	public bool HasLadder => blockList[_block].tileType.IsLadder;

	public bool HasRampOrLadder
	{
		get
		{
			if (!HasRamp)
			{
				return HasLadder;
			}
			return true;
		}
	}

	public bool HasSlope => blockList[_block].tileType == TileType.Slope;

	public bool HasStairs => blockList[_block].tileType == TileType.Stairs;

	public bool HasHalfBlock => blockList[_block].tileType == TileType.HalfBlock;

	public bool HasBlockOrRamp => blockList[_block].isBlockOrRamp;

	public bool UseLowBlock => blockList[_block].tileType.UseLowBlock;

	public bool CastShadow => blockList[_block].tileType.CastShadowSelf;

	public bool CastAmbientShadow => blockList[_block].tileType.CastAmbientShadow;

	public bool IsRoomEdge
	{
		get
		{
			if (Front.room == room && Right.room == room)
			{
				return FrontRight.room != room;
			}
			return true;
		}
	}

	public bool IsLotEdge
	{
		get
		{
			if (Front.room != null && Right.room != null && FrontRight.room != null && Back.room != null && Left.room != null && (Front.room == null || (!Front.room.data.atrium && Front.HasRoof)))
			{
				if (Right.room != null)
				{
					return Right.room.data.atrium | !Right.HasRoof;
				}
				return false;
			}
			return true;
		}
	}

	public bool IsBlocked => blocked;

	public int blockDir
	{
		get
		{
			return _dirs % 4;
		}
		set
		{
			_dirs = (byte)(_dirs - blockDir + value);
		}
	}

	public int objDir
	{
		get
		{
			return _dirs / 4 % 8;
		}
		set
		{
			_dirs = (byte)(_dirs - objDir * 4 + value % 8 * 4);
		}
	}

	public int floorDir
	{
		get
		{
			return _dirs / 32;
		}
		set
		{
			_dirs = (byte)(_dirs - floorDir * 32 + value * 32);
		}
	}

	public bool IsSky
	{
		get
		{
			if (sourceFloor.tileType == TileType.Sky)
			{
				return !HasBridge;
			}
			return false;
		}
	}

	public int liquidLv
	{
		get
		{
			if (effect != null && effect.LiquidAmount != 0)
			{
				return (effect?.LiquidAmount ?? 0) / 3 + 1;
			}
			return 0;
		}
	}

	public int fireAmount => effect?.FireAmount ?? 0;

	public bool IsVoid => this == Void;

	public Thing FirstThing
	{
		get
		{
			CellDetail cellDetail = detail;
			if (cellDetail == null || cellDetail.things.Count <= 0)
			{
				return null;
			}
			return detail.things[0];
		}
	}

	public Thing Installed
	{
		get
		{
			if (detail == null || detail.things.Count <= 0 || !detail.things[0].IsInstalled)
			{
				return null;
			}
			return detail.things[0];
		}
	}

	public List<Thing> Things => (detail ?? CellDetail.Empty).things;

	public List<Chara> Charas => (detail ?? CellDetail.Empty).charas;

	public int gatherCount
	{
		get
		{
			if (!map.gatherCounts.ContainsKey(index))
			{
				return 0;
			}
			return map.gatherCounts[index];
		}
		set
		{
			if (value == 0)
			{
				if (map.gatherCounts.ContainsKey(index))
				{
					map.gatherCounts.Remove(index);
				}
			}
			else
			{
				map.gatherCounts[index] = value;
			}
		}
	}

	public bool HasFloodBlock
	{
		get
		{
			if (!sourceBlock.tileType.IsFloodBlock)
			{
				return hasDoor;
			}
			return true;
		}
	}

	public SourceMaterial.Row matRoofBlock => matList[_roofBlockMat];

	public SourceMaterial.Row matBlock => matList[_blockMat];

	public SourceMaterial.Row matFloor => matList[_floorMat];

	public SourceMaterial.Row matBridge => matList[_bridgeMat];

	public SourceMaterial.Row matObj => matList[objMat];

	public SourceMaterial.Row matObj_fixed
	{
		get
		{
			if (!isObjDyed)
			{
				return matList[objMat];
			}
			return sourceObj.DefaultMaterial;
		}
	}

	public SourceBlock.Row sourceRoofBlock => blockList[_roofBlock];

	public SourceBlock.Row sourceBlock => blockList[_block];

	public SourceFloor.Row sourceFloor => floorList[_floor];

	public SourceFloor.Row sourceBridge => floorList[_bridge];

	public SourceFloor.Row sourceSurface
	{
		get
		{
			if (_bridge != 0)
			{
				return sourceBridge;
			}
			return sourceFloor;
		}
	}

	public SourceCellEffect.Row sourceEffect => effect?.source ?? effectList[0];

	public SourceObj.Row sourceObj => objList[obj];

	public BiomeProfile biome
	{
		get
		{
			if (!(sourceFloor.biome != null))
			{
				return EClass._zone.biome;
			}
			return sourceFloor.biome;
		}
	}

	public override bool Equals(object obj)
	{
		return false;
	}

	public override string ToString()
	{
		return "Cell(" + x + "," + z + " " + matBlock?.ToString() + "/" + matFloor?.ToString() + "/" + _block + ")";
	}

	public override int GetHashCode()
	{
		return index;
	}

	public Point GetPoint()
	{
		return new Point(x, z);
	}

	public Point GetSharedPoint()
	{
		return Point.shared.Set(x, z);
	}

	public bool CanSuffocate()
	{
		if (EClass._zone.IsUnderwater)
		{
			return true;
		}
		if (!IsDeepWater)
		{
			return false;
		}
		if (detail != null && detail.things.Count > 0)
		{
			foreach (Thing thing in detail.things)
			{
				if (thing.IsInstalled && thing.isFloating)
				{
					return false;
				}
			}
		}
		return true;
	}

	public bool HasObstacle()
	{
		if (!blocked && _block == 0 && obj == 0)
		{
			return hasDoor;
		}
		return true;
	}

	public bool CanBuildRamp(int dir)
	{
		if (HasRamp)
		{
			return blockDir == dir;
		}
		return true;
	}

	public void Refresh()
	{
		Cell cell2 = ((x > 0) ? cells[x - 1, z] : Void);
		Cell cell3 = ((x + 1 < Size) ? cells[x + 1, z] : Void);
		Cell cell4 = ((z > 0) ? cells[x, z - 1] : Void);
		Cell cell5 = ((z + 1 < Size) ? cells[x, z + 1] : Void);
		Cell cell6 = ((x > 0 && z > 0) ? cells[x - 1, z - 1] : Void);
		Cell cell7 = ((x + 1 < Size && z > 0) ? cells[x + 1, z - 1] : Void);
		Cell cell8 = ((x > 0 && z + 1 < Size) ? cells[x - 1, z + 1] : Void);
		Cell cell9 = ((x + 1 < Size && z + 1 < Size) ? cells[x + 1, z + 1] : Void);
		Cell cell10 = ((z > 1) ? cells[x, z - 2] : Void);
		Cell cell11 = ((x + 2 < Size) ? cells[x + 2, z] : Void);
		TileType tileType = sourceBlock.tileType;
		TileType tileType2 = sourceFloor.tileType;
		MapBounds bounds = map.bounds;
		bool flag = _bridge != 0;
		outOfBounds = x < bounds.x || z < bounds.z || x > bounds.maxX || z > bounds.maxZ;
		isSurrounded4d = cell2.HasFullBlock && cell3.HasFullBlock && cell4.HasFullBlock && cell5.HasFullBlock;
		isSurrounded = isSurrounded4d && cell6.HasFullBlock && cell7.HasFullBlock && cell8.HasFullBlock && cell9.HasFullBlock && cell4.bridgeHeight == bridgeHeight && cell3.bridgeHeight == bridgeHeight && cell5.bridgeHeight == bridgeHeight && cell2.bridgeHeight == bridgeHeight;
		isFloating = isForceFloat;
		hasDoor = false;
		hasDoorBoat = false;
		isDeck = false;
		isShoreSand = false;
		hasWindow = false;
		isCurtainClosed = false;
		isSkyFloor = tileType2 != TileType.Sky && (cell3.sourceFloor.tileType == TileType.Sky || cell4.sourceFloor.tileType == TileType.Sky);
		openPath = false;
		openSight = tileType.IsOpenSight || (cell4.hasWindow && !cell4.isCurtainClosed) || (cell3.hasWindow && !cell3.isCurtainClosed);
		blockSight = tileType.IsBlockSight || ((objList[obj].growth != null) ? objList[obj].growth.BlockSight(this) : objList[obj].tileType.IsBlockSight);
		blocked = outOfBounds || tileType.IsBlockPass || (tileType2.IsBlockPass && !flag) || ((objList[obj].growth != null) ? objList[obj].growth.BlockPass(this) : objList[obj].tileType.IsBlockPass) || impassable;
		isSlopeEdge = height > cell3.height || height > cell4.height;
		if (flag && sourceBridge.tileType.ShowPillar)
		{
			isBridgeEdge = _bridge != cell3._bridge || _bridge != cell4._bridge || bridgeHeight > cell3.bridgeHeight || bridgeHeight > cell4.bridgeHeight || _bridge != cell2._bridge || _bridge != cell5._bridge || bridgeHeight > cell2.bridgeHeight || bridgeHeight > cell5.bridgeHeight;
		}
		else
		{
			isBridgeEdge = false;
		}
		lotShade = false;
		if (room == null && !EClass._zone.IsSnowCovered)
		{
			if ((cell11.room != null && cell3.sourceBlock.tileType.CastShadowSelf) || (cell10.room != null && cell4.sourceBlock.tileType.CastShadowSelf))
			{
				lotShade = true;
			}
			else if ((cell11.Front.room != null && cell7.sourceBlock.tileType.CastShadowSelf) || (cell10.Right.room != null && cell7.sourceBlock.tileType.CastShadowSelf) || (cell7.FrontRight.room != null && cell7.Right.sourceBlock.tileType.CastShadowSelf))
			{
				lotShade = true;
			}
			else if (cell11.sourceBlock.tileType.CastShadowSelf && cell11.Right.room != null)
			{
				lotShade = true;
			}
		}
		isShadowed = lotShade || sourceBlock.tileType.CastShadowSelf || cell4.sourceBlock.tileType.CastShadowBack || (!HasRoof && _roofBlock != 0);
		castFloorShadow = lotShade || (room == null && sourceBlock.tileType.CastShadowSelf);
		byte b = (flag ? bridgeHeight : height);
		byte b2 = ((cell4.bridgeHeight == 0) ? cell4.height : cell4.bridgeHeight);
		byte b3 = ((cell3.bridgeHeight == 0) ? cell3.height : cell3.bridgeHeight);
		byte b4 = ((cell10.bridgeHeight == 0) ? cell10.height : cell10.bridgeHeight);
		byte b5 = ((cell11.bridgeHeight == 0) ? cell11.height : cell11.bridgeHeight);
		int num = 0;
		if (b3 - b > num)
		{
			num = b3 - b;
		}
		if (b2 - b > num)
		{
			num = b2 - b;
		}
		if (b5 - b - 4 > num)
		{
			num = b5 - b - 4;
		}
		if (b4 - b - 4 > num)
		{
			num = b4 - b - 4;
		}
		num -= 4;
		if (num > 15)
		{
			num = 15;
		}
		if (IsTopWater)
		{
			num /= 3;
		}
		else if (isShadowed)
		{
			num /= 2;
		}
		if (num <= 0)
		{
			num = 0;
		}
		shadowMod = (byte)num;
		baseWeight = 0;
		if (detail != null && detail.things.Count > 0)
		{
			for (int i = 0; i < detail.things.Count; i++)
			{
				Thing thing = detail.things[i];
				if (!thing.IsInstalled)
				{
					continue;
				}
				if (thing.trait.WeightMod > baseWeight)
				{
					baseWeight = thing.trait.WeightMod;
				}
				if (thing.isHidden)
				{
					continue;
				}
				if (thing.trait.IsFloating)
				{
					isFloating = true;
					if (IsFloorWater)
					{
						blocked = true;
					}
					else
					{
						isDeck = true;
					}
				}
				else
				{
					if (thing.trait.IsOpenPath)
					{
						blocked = false;
						hasDoorBoat = true;
						openPath = true;
						openSight = true;
					}
					if (thing.trait.IsBlockPath)
					{
						blocked = true;
					}
				}
				if (thing.trait.IsOpenSight)
				{
					openSight = true;
				}
				if (thing.trait is TraitDoor)
				{
					blocked = false;
					hasDoor = true;
					blockSight = true;
				}
				else if (thing.trait.IsBlockSight)
				{
					blockSight = true;
				}
				Trait trait = thing.trait;
				if (!(trait is TraitWindow))
				{
					if (trait is TraitCurtain)
					{
						isCurtainClosed = thing.isOn;
					}
				}
				else
				{
					hasWindow = true;
				}
			}
		}
		minHeight = (byte)(10 + b + ((IsTopWater && !isFloating) ? sourceFloor.tileType.FloorAltitude : 0));
		topHeight = (byte)(minHeight + tileType.slopeHeight);
		weights[0] = (byte)((Mathf.Abs(cell4.minHeight - topHeight) <= 8 || Mathf.Abs(cell4.topHeight - minHeight) <= 8) ? 1u : 0u);
		weights[1] = (byte)((Mathf.Abs(cell3.minHeight - topHeight) <= 8 || Mathf.Abs(cell3.topHeight - minHeight) <= 8) ? 1u : 0u);
		weights[2] = (byte)((Mathf.Abs(cell5.minHeight - topHeight) <= 8 || Mathf.Abs(cell5.topHeight - minHeight) <= 8) ? 1u : 0u);
		weights[3] = (byte)((Mathf.Abs(cell2.minHeight - topHeight) <= 8 || Mathf.Abs(cell2.topHeight - minHeight) <= 8) ? 1u : 0u);
		if (IsDeepWater)
		{
			baseWeight += 100;
		}
		ignoreObjShadow = (b2 > b && b2 - b > 6) || (b3 > b && b3 - b > 6);
		isWallEdge = !blocked && cell3.sourceBlock.tileType.IsWallOrFence && cell3.blockDir != 1 && cell4.sourceBlock.tileType.IsWallOrFence && cell4.blockDir != 0;
		if (isWallEdge)
		{
			blockSight = cell3.sourceBlock.tileType.IsBlockSight && cell4.sourceBlock.tileType.IsBlockSight;
			blocked = true;
			blockSight = true;
		}
		shadow = 0;
		if (!HasBlock)
		{
			if (cell2.isSeen && cell2.CastAmbientShadow && !cell2.hasDoor && bridgeHeight == cell2.bridgeHeight && !sourceFloor.ignoreTransition && (!cell2.sourceBlock.tileType.IsWallOrFence || cell2.blockDir != 0))
			{
				shadow++;
			}
			if (cell5.isSeen && cell5.CastAmbientShadow && !cell5.hasDoor && bridgeHeight == cell5.bridgeHeight && !sourceFloor.ignoreTransition && (!cell5.sourceBlock.tileType.IsWallOrFence || cell5.blockDir != 1))
			{
				shadow += 2;
			}
		}
		else
		{
			if (isFloating && (room != null || cell4.room != null || cell3.room != null || cell7.room != null))
			{
				isFloating = false;
			}
			if (sourceBlock.tileType.IsFence && !hasDoor && (blockDir != 0 || cell5.topHeight >= b - 3) && (blockDir != 1 || cell2.topHeight >= b - 3))
			{
				shadow = (byte)(blockDir + 4);
			}
		}
		bool isFloorWater = IsFloorWater;
		if (isFloorWater)
		{
			shore = (byte)((((!cell5.IsNotWaterEdge && z != Size - 1 && height == cell5.height) ? 1 : 0) + ((!cell3.IsNotWaterEdge && x != Size - 1 && height == cell3.height) ? 2 : 0) + ((!cell4.IsNotWaterEdge && z != 0 && height == cell4.height) ? 4 : 0) + ((!cell2.IsNotWaterEdge && x != 0 && height == cell2.height) ? 8 : 0)) * 12);
			isShoreSand = cell5.sourceFloor.isBeach || cell3.sourceFloor.isBeach || cell4.sourceFloor.isBeach || cell2.sourceFloor.isBeach;
			if (shore != 0)
			{
				shore += (byte)(isShoreSand ? 2 : 3);
			}
		}
		else
		{
			shore = 0;
		}
		if (sourceFloor.autotile > 0)
		{
			autotile = (byte)(((IsAutoTileEdge(cell5) && z != Size - 1) ? 1 : 0) + ((IsAutoTileEdge(cell3) && x != Size - 1) ? 2 : 0) + ((IsAutoTileEdge(cell4) && z != 0) ? 4 : 0) + ((IsAutoTileEdge(cell2) && x != 0) ? 8 : 0));
		}
		else
		{
			autotile = 0;
		}
		if (_bridge != 0 && sourceBridge.autotile > 0)
		{
			autotileBridge = (byte)(((IsBridgeAutoTileEdge(cell5) && z != Size - 1) ? 1 : 0) + ((IsBridgeAutoTileEdge(cell3) && x != Size - 1) ? 2 : 0) + ((IsBridgeAutoTileEdge(cell4) && z != 0) ? 4 : 0) + ((IsBridgeAutoTileEdge(cell2) && x != 0) ? 8 : 0));
		}
		else
		{
			autotileBridge = 0;
		}
		if (obj != 0 && sourceObj.autoTile)
		{
			autotileObj = (byte)(((IsObjAutoTileEdge(cell5) && z != Size - 1) ? 1 : 0) + ((IsObjAutoTileEdge(cell3) && x != Size - 1) ? 2 : 0) + ((IsObjAutoTileEdge(cell4) && z != 0) ? 4 : 0) + ((IsObjAutoTileEdge(cell2) && x != 0) ? 8 : 0));
		}
		else
		{
			autotileObj = 0;
		}
		if (openSight)
		{
			blockSight = false;
		}
		if (openPath)
		{
			blocked = false;
		}
		bool IsAutoTileEdge(Cell cell)
		{
			if (cell.sourceFloor.autotilePriority <= sourceFloor.autotilePriority && (!sourceFloor.isBeach || !cell.sourceFloor.isBeach) && (cell._floor != _floor || cell._floorMat != _floorMat))
			{
				return true;
			}
			if (!isFloorWater)
			{
				return height != cell.height;
			}
			return false;
		}
		bool IsBridgeAutoTileEdge(Cell cell)
		{
			if (cell.sourceBridge.autotilePriority <= sourceBridge.autotilePriority && (cell._bridge != _bridge || cell._bridgeMat != _bridgeMat))
			{
				return true;
			}
			return bridgeHeight != cell.bridgeHeight;
		}
		bool IsObjAutoTileEdge(Cell cell)
		{
			if (cell.obj != obj && (obj != 31 || (cell.obj != 97 && cell.obj != 98)))
			{
				return true;
			}
			return topHeight != cell.topHeight;
		}
	}

	public void RotateBlock(int a)
	{
		int num = blockDir;
		int num2 = (sourceBlock.tileType.IsWallOrFence ? 3 : sourceBlock._tiles.Length);
		num += a;
		if (num < 0)
		{
			num = num2 - 1;
		}
		if (num >= num2)
		{
			num = 0;
			crossWall = !crossWall;
		}
		blockDir = num;
	}

	public void RotateFloor(int a)
	{
		int num = floorDir;
		num += a;
		if (num < 0)
		{
			num = sourceFloor._tiles.Length - 1;
		}
		if (num >= sourceFloor._tiles.Length)
		{
			num = 0;
		}
		floorDir = num;
	}

	public void RotateObj(bool reverse = false, bool useBlockDir = false)
	{
		int num = (useBlockDir ? blockDir : objDir);
		num = ((!reverse) ? (num + 1) : (num - 1));
		if (num < 0)
		{
			num = sourceObj._tiles.Length - 1;
		}
		if (num >= sourceObj._tiles.Length)
		{
			num = 0;
		}
		if (useBlockDir)
		{
			blockDir = num;
		}
		else
		{
			objDir = num;
		}
	}

	public void RotateAll()
	{
		bool flag = EInput.isShiftDown || Input.GetMouseButton(1);
		PointTarget mouseTarget = Core.Instance.scene.mouseTarget;
		if (mouseTarget.card != null)
		{
			mouseTarget.card.Rotate(flag);
			return;
		}
		if (detail == null || detail.things.Count == 0)
		{
			if (HasBlock)
			{
				if (sourceBlock.tileType.CanRotate(buildMode: false))
				{
					RotateBlock((!flag) ? 1 : (-1));
				}
			}
			else if (HasObj)
			{
				if (sourceObj.tileType.CanRotate(buildMode: false))
				{
					RotateObj(flag, sourceObj.tileType.IsUseBlockDir);
				}
			}
			else if (HasFloor)
			{
				RotateFloor((!flag) ? 1 : (-1));
			}
		}
		GetPoint().RefreshNeighborTiles();
	}

	public void Reset()
	{
		_block = (_blockMat = (_floor = (_floorMat = (obj = (decal = (objVal = (objMat = 0)))))));
		bits.Bits = 0u;
		bits2.Bits = 0u;
	}

	public CellDetail GetOrCreateDetail()
	{
		if (detail == null)
		{
			detail = CellDetail.Spawn();
		}
		return detail;
	}

	public void TryDespawnDetail()
	{
		if (detail != null && detail.TryDespawn())
		{
			detail = null;
		}
	}

	public void AddCard(Card c)
	{
		GetOrCreateDetail();
		if (c.isChara)
		{
			detail.charas.Add(c.Chara);
			return;
		}
		detail.things.Add(c.Thing);
		c.Thing.stackOrder = detail.things.Count - 1;
		if (c.trait.ShouldRefreshTile || c.sourceCard.multisize)
		{
			GetPoint().RefreshNeighborTiles();
		}
	}

	public void RemoveCard(Card c)
	{
		if (c.isChara)
		{
			if (detail != null)
			{
				detail.charas.Remove(c.Chara);
			}
		}
		else
		{
			if (detail != null)
			{
				detail.things.Remove(c.Thing);
			}
			if (c.trait.ShouldRefreshTile || c.sourceCard.multisize)
			{
				GetPoint().RefreshNeighborTiles();
			}
		}
		TryDespawnDetail();
	}

	public string GetBlockName()
	{
		return Lang.Parse("blockName", matBlock.GetName(), sourceBlock.GetName());
	}

	public string GetFloorName()
	{
		return Lang.Parse("blockName", matFloor.GetName(), sourceFloor.GetName());
	}

	public string GetBridgeName()
	{
		return Lang.Parse("blockName", matBridge.GetName(), sourceBridge.GetName());
	}

	public string GetObjName()
	{
		PlantData plantData = EClass._map.TryGetPlant(this);
		string text = sourceObj.GetName();
		if (sourceObj.tag.Contains("mat_name") && matObj.alias != "granite" && (sourceObj.id != 10 || !(matObj.alias == "crystal")))
		{
			text = "_of2".lang(matObj.GetName(), text);
		}
		if (plantData != null && plantData.size > 0)
		{
			text = Lang.GetList("plant_size")[plantData.size - 1] + text;
		}
		text = text.AddArticle();
		if (plantData != null)
		{
			if (plantData.seed != null)
			{
				if (!plantData.seed.c_refText.IsEmpty())
				{
					text = plantData.seed.c_refText;
				}
				if (plantData.seed.encLV > 0)
				{
					text = text + "+" + plantData.seed.encLV;
				}
			}
			if (plantData.fert != 0)
			{
				text += ((plantData.fert > 0) ? "fertilized" : "defertilized").lang();
			}
		}
		if (map.backerObjs.ContainsKey(index))
		{
			int key = map.backerObjs[index];
			SourceBacker.Row row = EClass.sources.backers.map.TryGetValue(key);
			if (row != null && EClass.core.config.backer.Show(row))
			{
				string name = row.Name;
				text = ((sourceObj.id == 82) ? "backerRemain".lang(text, name) : "backerTree".lang(text, name));
			}
		}
		return text;
	}

	public string GetLiquidName()
	{
		return Lang.Parse("liquidName", sourceEffect.GetName());
	}

	public bool CanGrow(SourceObj.Row obj, VirtualDate date)
	{
		GrowSystem.cell = this;
		return obj.growth?.CanGrow(date) ?? false;
	}

	public void TryGrow(VirtualDate date = null)
	{
		growth?.TryGrow(date);
	}

	public bool CanHarvest()
	{
		return growth?.CanHarvest() ?? false;
	}

	public bool CanReapSeed()
	{
		return growth?.CanReapSeed() ?? false;
	}

	public bool CanMakeStraw()
	{
		if (sourceObj.alias == "wheat")
		{
			return growth.IsLastStage();
		}
		return false;
	}

	public PlantData TryGetPlant()
	{
		return EClass._map.TryGetPlant(this);
	}

	public float GetSurfaceHeight()
	{
		if (detail == null)
		{
			return 0f;
		}
		bool isTopWater = IsTopWater;
		for (int i = 0; i < detail.things.Count; i++)
		{
			Thing thing = detail.things[i];
			if (!thing.IsInstalled)
			{
				continue;
			}
			if (isTopWater)
			{
				if (thing.isFloating)
				{
					return thing.Pref.height + 0.1f;
				}
			}
			else if (thing.Pref.Surface)
			{
				return 0.1f * (float)thing.altitude + thing.Pref.height;
			}
		}
		return 0f;
	}

	public bool HasZoneStairs(bool includeLocked = true)
	{
		if (detail == null)
		{
			return false;
		}
		foreach (Thing thing in detail.things)
		{
			if (thing.IsInstalled && (thing.trait is TraitStairs || (includeLocked && thing.trait is TraitStairsLocked)))
			{
				return true;
			}
		}
		return false;
	}

	public override bool IsPathBlocked(PathManager.MoveType moveType)
	{
		if (moveType == PathManager.MoveType.Default)
		{
			return blocked;
		}
		if (!blocked)
		{
			if (detail != null)
			{
				return detail.charas.Count > 0;
			}
			return false;
		}
		return true;
	}
}
