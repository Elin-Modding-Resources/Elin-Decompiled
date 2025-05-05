using System.Linq;
using Algorithms;
using UnityEngine;

public class Zone_Dungeon : Zone
{
	public enum RailType
	{
		Mine,
		Factoy
	}

	public override string IDGenerator => "Dungeon";

	public override bool DisableRooms => !base.IsPCFaction;

	public override bool UseFog => true;

	public override bool ShowDangerLv => true;

	public override float PrespawnRate
	{
		get
		{
			if (!idExport.IsEmpty())
			{
				return 0f;
			}
			return 1f;
		}
	}

	public override bool IsReturnLocation => GetDeepestLv() == base.lv;

	public override int StartLV => -1;

	public override bool BlockBorderExit => true;

	public override float EvolvedChance
	{
		get
		{
			if (!EClass.debug.test)
			{
				return 0.08f;
			}
			return 1f;
		}
	}

	public override float BigDaddyChance
	{
		get
		{
			if (!EClass.debug.test)
			{
				return 0.08f;
			}
			return 1f;
		}
	}

	public override float ShrineChance
	{
		get
		{
			if (!EClass.debug.test)
			{
				return 0.35f;
			}
			return 1f;
		}
	}

	public override bool IsSnowCovered => false;

	public override bool CountDeepestLevel => true;

	public override int BaseElectricity => 100;

	public override ZoneTransition.EnterState RegionEnterState => ZoneTransition.EnterState.Down;

	public override string GetDungenID()
	{
		if (EClass.rnd(2) == 0)
		{
			return "DungeonRuins";
		}
		if (EClass.rnd(4) == 0)
		{
			return "Cavern";
		}
		if (EClass.rnd(8) == 0)
		{
			return "RoundRooms";
		}
		return "Dungeon";
	}

	public override void OnGenerateMap()
	{
		if (idExport.IsEmpty())
		{
			TryGenerateOre();
			TryGenerateBigDaddy();
			TryGenerateEvolved();
			TryGenerateShrine();
		}
	}

	public void PlaceRail(RailType railType = RailType.Mine)
	{
		int idRail = 31;
		string idTrolley = "390";
		bool placeStopper = true;
		int num = 8;
		if (railType == RailType.Factoy)
		{
			idRail = 110;
			placeStopper = false;
			num = EClass.rnd(4) + 1;
		}
		PathManager.Instance._pathfinder.PunishChangeDirection = true;
		PathManager.Instance._pathfinder.Diagonals = false;
		TraitStairsDown traitStairsDown = EClass._map.FindThing<TraitStairsDown>();
		TraitStairsUp traitStairsUp = EClass._map.FindThing<TraitStairsUp>();
		if (traitStairsDown != null && traitStairsUp != null)
		{
			TryPlaceRail(traitStairsDown.owner.pos, traitStairsUp.owner.pos);
		}
		int num2 = 0;
		for (int i = 0; i < 200; i++)
		{
			if (TryPlaceRail(EClass._map.bounds.GetRandomSurface(), EClass._map.bounds.GetRandomSurface()))
			{
				num2++;
				if (num2 > num)
				{
					break;
				}
			}
		}
		PathManager.Instance._pathfinder.Diagonals = true;
		bool TryPlaceRail(Point p1, Point p2)
		{
			if (p1.Distance(p2) < 20)
			{
				return false;
			}
			PathProgress pathProgress = PathManager.Instance.RequestPathImmediate(p1, p2, EClass.pc);
			if (!pathProgress.HasPath)
			{
				return false;
			}
			Point point = new Point();
			Point point2 = new Point();
			bool flag = false;
			int num3 = 0;
			foreach (PathFinderNode node in pathProgress.nodes)
			{
				Point shared = Point.GetShared(node.X, node.Z);
				for (int j = -1; j < 2; j++)
				{
					for (int k = -1; k < 2; k++)
					{
						point2.Set(shared.x + k, shared.z + j);
						if ((Mathf.Abs(k) != 1 || Mathf.Abs(j) != 1) && point2.IsValid && point2.sourceObj.id == 31 && !point2.Equals(point))
						{
							flag = true;
							if (num3 != 0)
							{
								break;
							}
							return false;
						}
					}
				}
				if (shared.sourceObj.id == idRail)
				{
					break;
				}
				shared.SetBlock();
				shared.SetObj(idRail);
				shared.Things.ForeachReverse(delegate(Thing t)
				{
					if (t.trait.CanBeDestroyed)
					{
						t.Destroy();
					}
				});
				if (flag)
				{
					break;
				}
				num3++;
				point.Set(shared);
			}
			if (!flag && placeStopper)
			{
				Point shared2 = Point.GetShared(pathProgress.nodes.LastItem().X, pathProgress.nodes.LastItem().Z);
				Cell cell = shared2.cell;
				int dir = ((cell.Front.obj != 31) ? ((cell.Right.obj == 31) ? 1 : ((cell.Back.obj == 31) ? 2 : 3)) : 0);
				if (!shared2.HasThing)
				{
					EClass._zone.AddCard(ThingGen.Create("1208"), shared2).Install().dir = dir;
				}
			}
			if (num3 > 2)
			{
				Point shared3 = Point.GetShared(pathProgress.nodes.First().X, pathProgress.nodes.First().Z);
				if (!shared3.HasThing)
				{
					EClass._zone.AddCard(ThingGen.Create(idTrolley), shared3).Install();
				}
			}
			return true;
		}
	}
}
