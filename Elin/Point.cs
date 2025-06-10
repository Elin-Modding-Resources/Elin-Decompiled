using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[Serializable]
public class Point : EClass
{
	public static readonly XY[] Surrounds = new XY[4]
	{
		new XY(0, -1),
		new XY(1, 0),
		new XY(0, 1),
		new XY(-1, 0)
	};

	public static Vector3 fixedPos;

	public static Point shared = new Point();

	public static Point shared2 = new Point();

	public static Point shared3 = new Point();

	public static Map map;

	public static Point Invalid = new Point
	{
		IsValid = false
	};

	public static Point Zero = new Point();

	[JsonProperty]
	public int x;

	[JsonProperty]
	public int z;

	private Vector3 vCache;

	public static BaseGameScreen _screen;

	private static List<Card> listCard = new List<Card>();

	private static List<Chara> listChara = new List<Chara>();

	public int index => x + z * map.Size;

	public Cell cell => map.cells[x, z];

	public SourceMaterial.Row matRoofBlock => cell.matRoofBlock;

	public SourceMaterial.Row matBlock => cell.matBlock;

	public SourceMaterial.Row matFloor => cell.matFloor;

	public SourceMaterial.Row matBridge => cell.matBridge;

	public SourceBlock.Row sourceRoofBlock => cell.sourceRoofBlock;

	public SourceBlock.Row sourceBlock => cell.sourceBlock;

	public SourceFloor.Row sourceFloor => cell.sourceFloor;

	public SourceFloor.Row sourceBridge => cell.sourceBridge;

	public SourceObj.Row sourceObj => cell.sourceObj;

	public CellDetail detail => cell.detail;

	public Area area => detail?.area;

	public Point Front => new Point(x + 1, z - 1);

	public GrowSystem growth => cell.growth;

	public int eloX => EClass.scene.elomap.minX + x;

	public int eloY => EClass.scene.elomap.minY + z;

	public bool HasDesignation => cell.detail?.designation != null;

	public bool HasDirt => cell.decal != 0;

	public bool IsValid
	{
		get
		{
			if (x >= 0 && z >= 0 && x < map.Size)
			{
				return z < map.Size;
			}
			return false;
		}
		set
		{
			x = ((!value) ? (-999) : 0);
		}
	}

	public bool IsInBounds
	{
		get
		{
			if (x >= map.bounds.x && z >= map.bounds.z && x <= map.bounds.maxX)
			{
				return z <= map.bounds.maxZ;
			}
			return false;
		}
	}

	public bool IsInBoundsPlus
	{
		get
		{
			if (x >= 0 && z >= 0 && x <= map.Size - 1)
			{
				return z <= map.Size - 1;
			}
			return false;
		}
	}

	public bool IsFarmField => (HasBridge ? sourceBridge : sourceFloor).alias == "field";

	public bool IsWater => cell.IsTopWater;

	public bool HasRamp => cell.HasRamp;

	public bool HasRail => sourceObj.tag.Contains("rail");

	public bool HasRampOrLadder => cell.HasRampOrLadder;

	public bool HasObj => cell.obj != 0;

	public bool HasDecal => cell.decal != 0;

	public bool HasBlock => cell._block != 0;

	public bool HasMinableBlock
	{
		get
		{
			if (HasObj)
			{
				return sourceBlock.tileType.IsFullBlock;
			}
			return false;
		}
	}

	public bool HasWallOrFence => cell.HasWallOrFence;

	public bool HasWall
	{
		get
		{
			if (cell._block != 0)
			{
				return cell.sourceBlock.tileType.IsWall;
			}
			return false;
		}
	}

	public bool HasFence
	{
		get
		{
			if (cell._block != 0)
			{
				return cell.sourceBlock.tileType.IsFence;
			}
			return false;
		}
	}

	public bool HasNonWallBlock
	{
		get
		{
			if (HasBlock)
			{
				return !cell.sourceBlock.tileType.IsWallOrFence;
			}
			return false;
		}
	}

	public bool HasTaskBuild
	{
		get
		{
			if (cell.detail != null)
			{
				return cell.detail.designation is TaskBuild;
			}
			return false;
		}
	}

	public bool HasBlockRecipe => (cell.detail?.designation as TaskBuild)?.isBlock ?? false;

	public bool HasFloor => cell._floor != 0;

	public bool HasBridge => cell._bridge != 0;

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

	public bool HasArea => detail?.area != null;

	public bool HasChara
	{
		get
		{
			CellDetail cellDetail = detail;
			if (cellDetail == null)
			{
				return false;
			}
			return cellDetail.charas.Count > 0;
		}
	}

	public bool HasThing
	{
		get
		{
			CellDetail cellDetail = detail;
			if (cellDetail == null)
			{
				return false;
			}
			return cellDetail.things.Count > 0;
		}
	}

	public bool HasMultipleChara
	{
		get
		{
			if (detail != null)
			{
				return detail.charas.Count > 1;
			}
			return false;
		}
	}

	public bool IsDeepWater
	{
		get
		{
			if (cell.IsTopWaterAndNoSnow)
			{
				return sourceFloor.tileType.IsDeepWater;
			}
			return false;
		}
	}

	public Chara FirstChara
	{
		get
		{
			CellDetail cellDetail = detail;
			if (cellDetail == null || cellDetail.charas.Count <= 0)
			{
				return null;
			}
			return detail.charas[0];
		}
	}

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

	public Thing LastThing
	{
		get
		{
			CellDetail cellDetail = detail;
			if (cellDetail == null || cellDetail.things.Count <= 0)
			{
				return null;
			}
			return detail.things.LastItem();
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

	public List<Thing> Things => cell.Things;

	public List<Chara> Charas => cell.Charas;

	public bool IsSeen => cell.isSeen;

	public bool IsSync => cell.pcSync;

	public bool IsHidden
	{
		get
		{
			if (cell.isSeen)
			{
				if (cell.room != null && !EClass.screen.tileMap.hideRoomFog && !cell.hasDoor)
				{
					return cell.HasRoof;
				}
				return false;
			}
			return true;
		}
	}

	public bool IsBlocked
	{
		get
		{
			if (IsValid)
			{
				return cell.blocked;
			}
			return true;
		}
	}

	public bool IsHotSpring
	{
		get
		{
			if (cell.IsTopWaterAndNoSnow)
			{
				if (!EClass._zone.elements.Has(3701))
				{
					return IsInSpot<TraitGeyser>();
				}
				return true;
			}
			return false;
		}
	}

	public static Point GetShared(int x, int z)
	{
		shared.x = x;
		shared.z = z;
		return shared;
	}

	public bool Within(int _x, int _z, int _w, int _h)
	{
		if (x >= _x && z >= _z && x < _x + _w)
		{
			return z < _z + _h;
		}
		return false;
	}

	public Chara FirstVisibleChara()
	{
		if (detail != null)
		{
			foreach (Chara chara in detail.charas)
			{
				if (EClass.pc.CanSee(chara))
				{
					return chara;
				}
			}
		}
		return null;
	}

	public bool HasRoomOrArea(BaseArea a)
	{
		if (area != a)
		{
			return cell.room == a;
		}
		return true;
	}

	public bool IsInSpot<T>() where T : Trait
	{
		foreach (Thing thing in EClass._map.things)
		{
			if (!(thing.trait is T) || thing.trait.radius == 0)
			{
				continue;
			}
			foreach (Point item in thing.trait.ListPoints())
			{
				if (Equals(item))
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool IsPublicSpace()
	{
		if (area != null)
		{
			if (area.type.IsPublicArea)
			{
				return true;
			}
			return false;
		}
		if (cell.room != null)
		{
			if (cell.room.type.IsPublicArea)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	public Point()
	{
	}

	public Point(Point p)
	{
		x = p.x;
		z = p.z;
	}

	public Point(int _x, int _z)
	{
		x = _x;
		z = _z;
	}

	public Point(int _index)
	{
		Set(_index);
	}

	public Point Copy()
	{
		return new Point(x, z);
	}

	public T Copy<T>() where T : Point
	{
		T val = Activator.CreateInstance<T>();
		val.Set(this);
		return val;
	}

	public Point Set(int _x, int _z)
	{
		x = _x;
		z = _z;
		return this;
	}

	public Point Set(int index)
	{
		x = index % map.Size;
		z = index % map.SizeXZ / map.Size;
		return this;
	}

	public Point Set(Point point)
	{
		x = point.x;
		z = point.z;
		return this;
	}

	public override string ToString()
	{
		return "(" + x + " / " + z + ")";
	}

	public void Set(Vector3 v)
	{
		v.x -= 0.64f;
		v.y -= 0.64f;
		int num = Mathf.RoundToInt(v.x / _screen.tileAlign.x);
		int num2 = Mathf.RoundToInt(v.y / _screen.tileAlign.y);
		x = (num2 - num) / 2 * -1;
		z = num - x;
	}

	public ref Vector3 PositionAuto()
	{
		if (EClass._zone.IsRegion)
		{
			return ref PositionTopdown();
		}
		return ref Position();
	}

	public ref Vector3 Position(int height)
	{
		if (height == -1)
		{
			return ref Position();
		}
		vCache.x = (float)(x + z) * _screen.tileAlign.x;
		vCache.y = (float)(z - x) * _screen.tileAlign.y + (float)height * _screen.tileMap._heightMod.y;
		vCache.z = 1000f + vCache.x * _screen.tileWeight.x + vCache.y * _screen.tileWeight.z + (float)height * _screen.tileMap._heightMod.z;
		return ref vCache;
	}

	public ref Vector3 Position()
	{
		byte b = ((cell.bridgeHeight == 0) ? cell.height : cell.bridgeHeight);
		vCache.x = (float)(x + z) * _screen.tileAlign.x;
		vCache.y = (float)(z - x) * _screen.tileAlign.y + (float)(int)b * _screen.tileMap._heightMod.y;
		vCache.z = 1000f + vCache.x * _screen.tileWeight.x + vCache.y * _screen.tileWeight.z + (float)(int)b * _screen.tileMap._heightMod.z;
		return ref vCache;
	}

	public ref Vector3 PositionTopdown()
	{
		byte b = ((cell.bridgeHeight == 0) ? cell.height : cell.bridgeHeight);
		vCache.x = (float)x * _screen.tileAlign.x;
		vCache.y = (float)z * _screen.tileAlign.y + (float)(int)b * _screen.tileMap._heightMod.y;
		vCache.z = 1000f + vCache.x * _screen.tileWeight.x + vCache.y * _screen.tileWeight.z + (float)(int)b * _screen.tileMap._heightMod.z;
		return ref vCache;
	}

	public ref Vector3 PositionTopdownTreasure()
	{
		GameScreenElona screenElona = EClass.scene.screenElona;
		vCache.x = (float)(x - EClass.scene.elomap.minX) * screenElona.tileAlign.x;
		vCache.y = (float)(z - EClass.scene.elomap.minY) * screenElona.tileAlign.y;
		vCache.z = 1400f;
		return ref vCache;
	}

	public ref Vector3 PositionCenter()
	{
		if (EClass._zone.IsRegion)
		{
			_ = ref PositionTopdown();
		}
		else
		{
			_ = ref Position();
		}
		vCache.x += EClass.screen.tileWorldSize.x * 0.5f;
		vCache.y += EClass.screen.tileWorldSize.y * 0.75f;
		return ref vCache;
	}

	public Quaternion GetRotation(Point to)
	{
		return Quaternion.Euler(0f, 0f, GetAngle(to));
	}

	public float GetAngle(Point to)
	{
		Vector3 vector = to.Position() - Position();
		return Mathf.Atan2(vector.y, vector.x) * 57.29578f;
	}

	public float GetAngle2(Point to)
	{
		int num = to.x - x;
		return Mathf.Atan2(to.z - z, num) * 57.29578f;
	}

	public Point GetNearestPoint(bool allowBlock = false, bool allowChara = true, bool allowInstalled = true, bool ignoreCenter = false)
	{
		Point p = new Point();
		int num = 1;
		int num2 = x;
		int num3 = z;
		if (IsValid(num2, num3))
		{
			return p;
		}
		if (IsValid(num2, num3 + 1))
		{
			return p;
		}
		for (int i = 0; i < 30; i++)
		{
			for (int j = 0; j < num; j++)
			{
				num2++;
				if (IsValid(num2, num3))
				{
					return p;
				}
			}
			for (int k = 0; k < num; k++)
			{
				num3++;
				if (IsValid(num2, num3))
				{
					return p;
				}
			}
			num++;
			for (int l = 0; l < num; l++)
			{
				num2--;
				if (IsValid(num2, num3))
				{
					return p;
				}
			}
			for (int m = 0; m < num; m++)
			{
				num3--;
				if (IsValid(num2, num3))
				{
					return p;
				}
			}
			num++;
		}
		p.Set(this);
		return p;
		bool IsValid(int dx, int dz)
		{
			p.Set(dx, dz);
			if (!p.IsInBounds || !map.Contains(dx, dz) || (ignoreCenter && dx == x && dz == z))
			{
				return false;
			}
			if ((!allowBlock && (p.cell.blocked || p.cell.hasDoor || (p.cell.growth != null && p.cell.growth.IsTree))) || (!allowChara && p.HasChara) || (!allowInstalled && p.Installed != null))
			{
				return false;
			}
			return true;
		}
	}

	public bool ForeachNearestPoint(Func<Point, bool> endFunc, bool allowBlock = false, bool allowChara = true, bool allowInstalled = true, bool ignoreCenter = false, int maxRange = 30)
	{
		Point p = new Point();
		int num = 1;
		int num2 = x;
		int num3 = z;
		if (IsValid(num2, num3) && endFunc(p))
		{
			return true;
		}
		if (IsValid(num2, num3 + 1) && endFunc(p))
		{
			return true;
		}
		for (int i = 0; i < maxRange; i++)
		{
			for (int j = 0; j < num; j++)
			{
				num2++;
				if (IsValid(num2, num3) && endFunc(p))
				{
					return true;
				}
			}
			for (int k = 0; k < num; k++)
			{
				num3++;
				if (IsValid(num2, num3) && endFunc(p))
				{
					return true;
				}
			}
			num++;
			for (int l = 0; l < num; l++)
			{
				num2--;
				if (IsValid(num2, num3) && endFunc(p))
				{
					return true;
				}
			}
			for (int m = 0; m < num; m++)
			{
				num3--;
				if (IsValid(num2, num3) && endFunc(p))
				{
					return true;
				}
			}
			num++;
		}
		Debug.Log("ForeachNearestPoint Fail:" + this);
		return false;
		bool IsValid(int dx, int dz)
		{
			p.Set(dx, dz);
			if (!p.IsInBounds || !map.Contains(dx, dz) || (ignoreCenter && dx == x && dz == z))
			{
				return false;
			}
			if ((!allowBlock && (p.cell.blocked || p.cell.hasDoor || (p.cell.growth != null && p.cell.growth.IsTree))) || (!allowChara && p.HasChara) || (!allowInstalled && p.Installed != null))
			{
				return false;
			}
			return true;
		}
	}

	public Point GetRandomNeighbor()
	{
		int num = ((EClass.rnd(2) == 0) ? 1 : (-1));
		if (EClass.rnd(2) == 0)
		{
			return new Point(Mathf.Clamp(x + num, 0, map.Size - 1), z);
		}
		return new Point(x, Mathf.Clamp(z + num, 0, map.Size - 1));
	}

	public Point GetRandomPoint(int radius, bool requireLos = true, bool allowChara = true, bool allowBlocked = false, int tries = 100)
	{
		Point point = new Point();
		for (int i = 0; i < tries; i++)
		{
			point.Set(x - radius + EClass.rnd(radius * 2 + 1), z - radius + EClass.rnd(radius * 2 + 1));
			if (point.IsValid && (allowBlocked || !point.IsBlocked) && (!requireLos || Los.IsVisible(this, point)) && (allowChara || !point.HasChara))
			{
				return point;
			}
		}
		return null;
	}

	public Point GetRandomPointInRadius(int minRadius, int maxRadius, bool requireLos = true, bool allowChara = true, bool allowBlocked = false, int tries = 2000)
	{
		Point point = new Point();
		for (int i = 0; i < tries; i++)
		{
			point.Set(x - maxRadius + EClass.rnd(maxRadius * 2 + 1), z - maxRadius + EClass.rnd(maxRadius * 2 + 1));
			if (point.IsValid && (allowBlocked || !point.IsBlocked) && (allowChara || !point.HasChara))
			{
				int num = point.Distance(this);
				if (num >= minRadius && num <= maxRadius && (!requireLos || Los.IsVisible(this, point)))
				{
					return point;
				}
			}
		}
		return null;
	}

	public Point GetPointTowards(Point dest)
	{
		Point point = new Point(this);
		int num = dest.x - point.x;
		int num2 = dest.z - point.z;
		if (Mathf.Abs(num) > 1 || Mathf.Abs(num2) > 1)
		{
			int num3 = Mathf.Max(Mathf.Abs(num), Mathf.Abs(num2));
			point.x += num / num3;
			point.z += num2 / num3;
		}
		return point;
	}

	public void TalkWitnesses(Chara criminal, string idTalk, int radius = 4, WitnessType type = WitnessType.everyone, Func<Chara, bool> talkIf = null, int chance = 3)
	{
		if (talkIf == null)
		{
			talkIf = (Chara c) => EClass.rnd(chance) == 0;
		}
		foreach (Chara item in ListWitnesses(criminal, radius, type))
		{
			if (talkIf(item) && !item.renderer.IsTalking())
			{
				item.Talk(idTalk);
			}
		}
	}

	public List<Chara> ListWitnesses(Chara criminal, int radius = 4, WitnessType type = WitnessType.crime, Chara target = null)
	{
		List<Chara> list = new List<Chara>();
		foreach (Point item in map.ListPointsInCircle(this, radius, mustBeWalkable: false))
		{
			List<Chara> list2 = item.detail?.charas;
			if (list2 == null || list2.Count == 0)
			{
				continue;
			}
			foreach (Chara item2 in list2)
			{
				if (item2 == criminal || item2.IsPC || (item2 != target && !item2.CanWitness) || (item2.HasCondition<ConDim>() && EClass.rnd(2) == 0) || item2.conSuspend != null || item2.isParalyzed || item2.IsDisabled)
				{
					continue;
				}
				switch (type)
				{
				case WitnessType.ally:
					if (!criminal.IsPCFaction || item2.hostility <= Hostility.Neutral)
					{
						continue;
					}
					break;
				case WitnessType.crime:
					if (criminal == null || item2.isBlind || item2.isConfused || (criminal.IsPCParty && (item2.IsPCFaction || item2.IsPCFactionMinion)) || (target == null && item2.OriginalHostility < Hostility.Neutral))
					{
						continue;
					}
					break;
				case WitnessType.music:
					if (item2.hostility <= Hostility.Enemy)
					{
						continue;
					}
					break;
				}
				list.Add(item2);
			}
		}
		return list;
	}

	public bool TryWitnessCrime(Chara criminal, Chara target = null, int radius = 4, Func<Chara, bool> funcWitness = null)
	{
		List<Chara> list = ListWitnesses(criminal, radius, WitnessType.crime, target);
		bool result = false;
		if (funcWitness == null)
		{
			funcWitness = (Chara c) => EClass.rnd(10) == 0;
		}
		foreach (Chara item in list)
		{
			if (funcWitness(item))
			{
				CallGuard(criminal, item);
				result = true;
				target?.DoHostileAction(criminal);
				if (item != target)
				{
					item.DoHostileAction(criminal);
				}
				break;
			}
		}
		return result;
	}

	public void CallGuard(Chara criminal, Chara caller)
	{
		if (caller.HasCondition<ConSleep>())
		{
			caller.RemoveCondition<ConSleep>();
		}
		caller.Talk("callGuards");
		List<Chara> list = EClass._map.charas.Where((Chara c) => c.trait is TraitGuard && !c.IsInCombat).ToList();
		if (list.Count > 0)
		{
			Chara chara = list.RandomItem();
			caller.Say("calledGuard", caller);
			chara.DoHostileAction(criminal);
		}
	}

	public void SetBlock(int idMat = 0, int idBlock = 0)
	{
		map.SetBlock(x, z, idMat, idBlock);
	}

	public void SetFloor(int idMat = 0, int idFloor = 0)
	{
		map.SetFloor(x, z, idMat, idFloor);
	}

	public void SetObj(int id = 0, int value = 1, int dir = 0)
	{
		map.SetObj(x, z, id, value, dir);
	}

	public void ModFire(int value, bool extinguish = false)
	{
		if (extinguish && cell.HasFire)
		{
			PlaySound("extinguish");
			PlayEffect("smoke");
			PlayEffect("vanish");
		}
		map.ModFire(x, z, value);
	}

	public void Plow()
	{
		if (IsFarmField)
		{
			if (cell.Right.HasWallOrFence)
			{
				Set(cell.Right.GetPoint());
			}
			else if (cell.Front.HasWallOrFence)
			{
				Set(cell.Front.GetPoint());
			}
		}
		SetFloor(4, 4);
	}

	public bool Equals(int _x, int _z)
	{
		if (x == _x)
		{
			return z == _z;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		Point point = obj as Point;
		if (point == null)
		{
			Debug.Log(point?.ToString() + ":" + point.GetType()?.ToString() + "is not Point");
			return false;
		}
		if (x == point.x)
		{
			return z == point.z;
		}
		return false;
	}

	public override int GetHashCode()
	{
		return x + z * map.Size;
	}

	public int Distance(Point p)
	{
		return Fov.Distance(p.x, p.z, x, z);
	}

	public int Distance(int tx, int tz)
	{
		return Fov.Distance(tx, tz, x, z);
	}

	public bool IsBlockByHeight(Point p)
	{
		if (Mathf.Abs(p.cell.minHeight - cell.topHeight) > 8)
		{
			return Mathf.Abs(p.cell.topHeight - cell.minHeight) > 8;
		}
		return false;
	}

	public Point Clamp(bool useBounds = false)
	{
		if (useBounds)
		{
			if (x < map.bounds.x)
			{
				x = map.bounds.x;
			}
			else if (x >= map.bounds.maxX)
			{
				x = map.bounds.maxX;
			}
			if (z < map.bounds.z)
			{
				z = map.bounds.z;
			}
			else if (z >= map.bounds.maxZ)
			{
				z = map.bounds.maxZ;
			}
		}
		else
		{
			if (x < 0)
			{
				x = 0;
			}
			else if (x >= map.Size)
			{
				x = map.Size - 1;
			}
			if (z < 0)
			{
				z = 0;
			}
			else if (z >= map.Size)
			{
				z = map.Size - 1;
			}
		}
		return this;
	}

	public List<Card> ListCards(bool includeMasked = false)
	{
		listCard.Clear();
		bool flag = EClass.scene.actionMode != null && EClass.scene.actionMode.IsRoofEditMode();
		if (detail != null)
		{
			foreach (Thing thing in detail.things)
			{
				if (!thing.isHidden && (includeMasked || !thing.isMasked) && thing.isRoofItem == flag)
				{
					listCard.Add(thing);
				}
			}
			if (!flag)
			{
				foreach (Chara chara in detail.charas)
				{
					listCard.Add(chara);
				}
			}
		}
		return listCard;
	}

	public List<Card> ListVisibleCards()
	{
		listCard.Clear();
		if (detail != null)
		{
			foreach (Thing thing in detail.things)
			{
				if (!thing.isHidden && !thing.isMasked && !thing.isRoofItem)
				{
					listCard.Add(thing);
				}
			}
			foreach (Chara chara in detail.charas)
			{
				if (EClass.pc.CanSee(chara))
				{
					listCard.Add(chara);
				}
			}
		}
		return listCard;
	}

	public Card FindAttackTarget()
	{
		foreach (Card item in ListCards())
		{
			if (item.isChara || item.trait.CanBeAttacked)
			{
				return item;
			}
		}
		return null;
	}

	public Chara FindChara(Func<Chara, bool> func)
	{
		if (detail != null)
		{
			foreach (Chara chara in detail.charas)
			{
				if (func(chara))
				{
					return chara;
				}
			}
		}
		return null;
	}

	public T FindThing<T>() where T : Trait
	{
		if (IsValid && detail != null)
		{
			foreach (Thing thing in detail.things)
			{
				if (thing.trait is T)
				{
					return thing.trait as T;
				}
			}
		}
		return null;
	}

	public Thing FindThing(Func<Thing, bool> func)
	{
		if (detail != null)
		{
			foreach (Thing thing in detail.things)
			{
				if (func(thing))
				{
					return thing;
				}
			}
		}
		return null;
	}

	public List<Card> ListThings<T>(bool onlyInstalled = true) where T : Trait
	{
		listCard.Clear();
		if (detail != null)
		{
			foreach (Thing thing in detail.things)
			{
				if (!thing.isHidden && !thing.isRoofItem && !thing.isMasked && (!onlyInstalled || thing.IsInstalled) && thing.trait is T)
				{
					listCard.Add(thing);
				}
			}
		}
		return listCard;
	}

	public List<Chara> ListCharas()
	{
		listChara.Clear();
		if (detail != null)
		{
			foreach (Chara chara in detail.charas)
			{
				listChara.Add(chara);
			}
		}
		return listChara;
	}

	public List<Chara> ListVisibleCharas()
	{
		listChara.Clear();
		if (detail != null)
		{
			foreach (Chara chara in detail.charas)
			{
				if (EClass.pc.CanSee(chara))
				{
					listChara.Add(chara);
				}
			}
		}
		return listChara;
	}

	public List<Chara> ListCharasInNeighbor(Func<Chara, bool> func)
	{
		listChara.Clear();
		ForeachNeighbor(delegate(Point p)
		{
			if (p.detail == null)
			{
				return;
			}
			foreach (Chara chara in p.detail.charas)
			{
				if (func(chara))
				{
					listChara.Add(chara);
				}
			}
		});
		return listChara;
	}

	public List<Chara> ListCharasInRadius(Chara cc, int dist, Func<Chara, bool> func, bool onlyVisible = true)
	{
		listChara.Clear();
		foreach (Chara chara in EClass._map.charas)
		{
			if (func(chara) && chara.Dist(cc) < dist && (!onlyVisible || Los.IsVisible(chara, cc)))
			{
				listChara.Add(chara);
			}
		}
		return listChara;
	}

	public T GetInstalled<T>() where T : Trait
	{
		if (detail != null)
		{
			foreach (Thing thing in detail.things)
			{
				if (thing.IsInstalled && thing.trait is T)
				{
					return thing.trait as T;
				}
			}
		}
		return null;
	}

	public Effect PlayEffect(string id)
	{
		return Effect.Get(id).Play(this);
	}

	public SoundSource PlaySound(string id, bool synced = true, float v = 1f, bool spatial = true)
	{
		Vector3 vector = default(Vector3);
		if (spatial)
		{
			if (!IsValid)
			{
				return null;
			}
			vector = (EClass._zone.IsRegion ? PositionTopdown() : Position());
			vector.z = 0f;
			if (Vector3.Distance(vector, EClass.scene.transAudio.position) > EClass.core.gameSetting.audio.maxRange)
			{
				return null;
			}
			if (!synced)
			{
				v -= 0.4f;
			}
		}
		return EClass.Sound.Play(id, vector, (!spatial) ? 1f : (v * ((EClass.screen.Zoom >= 1f) ? 1f : EClass.screen.Zoom)));
	}

	public void RefreshNeighborTiles()
	{
		map.RefreshNeighborTiles(x, z);
	}

	public void RefreshTile()
	{
		map.RefreshSingleTile(x, z);
	}

	public bool HasNonHomeProperty(Thing exclude = null)
	{
		if (FirstThing == null)
		{
			return false;
		}
		foreach (Thing thing in detail.things)
		{
			if (thing != exclude && thing.isNPCProperty)
			{
				return true;
			}
		}
		return false;
	}

	public void Animate(AnimeID id, bool animeBlock = false)
	{
		if (!IsValid)
		{
			return;
		}
		CellDetail orCreateDetail = cell.GetOrCreateDetail();
		if (orCreateDetail.anime == null)
		{
			TransAnime transAnime = (orCreateDetail.anime = new TransAnime
			{
				data = ResourceCache.Load<TransAnimeData>("Scene/Render/Anime/" + id),
				point = this,
				animeBlock = animeBlock
			}.Init());
			EClass._map.pointAnimes.Add(transAnime);
			if (id == AnimeID.Quake)
			{
				transAnime.drawBlock = true;
			}
		}
	}

	public RenderParam ApplyAnime(RenderParam p)
	{
		if (detail == null || detail.anime == null)
		{
			return p;
		}
		p.x += detail.anime.v.x;
		p.y += detail.anime.v.y;
		p.z += detail.anime.v.z;
		p.v.x += detail.anime.v.x;
		p.v.y += detail.anime.v.y;
		p.v.z += detail.anime.v.z;
		return p;
	}

	public Vector3 ApplyAnime(ref Vector3 p)
	{
		if (detail == null || detail.anime == null)
		{
			return p;
		}
		p.x += detail.anime.v.x;
		p.y += detail.anime.v.y;
		p.z += detail.anime.v.z;
		return p;
	}

	public List<IInspect> ListInspectorTargets()
	{
		List<IInspect> list = new List<IInspect>();
		CellDetail cellDetail = detail;
		if (cellDetail != null)
		{
			foreach (Chara chara in cellDetail.charas)
			{
				if (chara.isSynced)
				{
					list.Add(chara);
				}
			}
			foreach (Thing thing in cellDetail.things)
			{
				list.Add(thing);
			}
			if (cellDetail.designation != null && !(cellDetail.designation is TaskCut) && !(cellDetail.designation is TaskMine))
			{
				list.Add(cellDetail.designation);
			}
		}
		if (ObjInfo._CanInspect(this))
		{
			list.Add(ObjInfo.GetTempList(this));
		}
		if (BlockInfo._CanInspect(this))
		{
			list.Add(BlockInfo.GetTempList(this));
		}
		return list;
	}

	public void ForeachMultiSize(int w, int h, Action<Point, bool> action)
	{
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				shared3.Set(x - j, z + i);
				action(shared3, j == 0 && i == 0);
			}
		}
	}

	public void ForeachNeighbor(Action<Point> action, bool diagonal = true)
	{
		Point point = new Point();
		for (int i = x - 1; i <= x + 1; i++)
		{
			for (int j = z - 1; j <= z + 1; j++)
			{
				if (diagonal || i == x || j == z)
				{
					point.Set(i, j);
					if (point.IsValid)
					{
						action(point);
					}
				}
			}
		}
	}

	public Point ToRegionPos()
	{
		Point point = new Point(x, z);
		point.x += EClass.scene.elomap.minX;
		point.z += EClass.scene.elomap.minY;
		return point;
	}

	public void SetHighlight(int id)
	{
		cell.highlight = (byte)id;
		EClass.player.lastMarkedHighlights.Add(cell);
	}
}
