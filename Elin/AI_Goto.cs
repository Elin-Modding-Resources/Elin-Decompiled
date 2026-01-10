using System.Collections.Generic;
using Algorithms;

public class AI_Goto : AIAct
{
	public Card destCard;

	public Point dest = new Point();

	public int destDist;

	public int waitCount;

	public bool ignoreConnection;

	public bool interaction;

	public bool repath;

	public override bool UseTurbo => false;

	public override bool CancelWhenDamaged => !EClass._zone.IsRegion;

	public override bool CancelWhenMoved => true;

	public override bool InformCancel => false;

	public override bool PushChara => parent?.PushChara ?? true;

	public override bool CanManualCancel()
	{
		return true;
	}

	public override Point GetDestination()
	{
		if (destCard != null)
		{
			return destCard.pos;
		}
		return dest;
	}

	public AI_Goto(Point _dest, int dist, bool _ignoreConnection = false, bool _interaction = false)
	{
		dest.Set(_dest);
		destDist = dist;
		ignoreConnection = _ignoreConnection;
		interaction = _interaction;
	}

	public AI_Goto(Card _card, int dist, bool _ignoreConnection = false, bool _interaction = false)
	{
		destCard = _card;
		destDist = dist;
		ignoreConnection = _ignoreConnection;
		interaction = _interaction;
	}

	public override void OnReset()
	{
		owner.path.state = PathProgress.State.Idle;
	}

	public Status TryGoTo()
	{
		PathProgress path = owner.path;
		Point pos = owner.pos;
		if (owner.IsPC)
		{
			if (dest.x > EClass._map.bounds.maxX + 1)
			{
				dest.x = EClass._map.bounds.maxX + 1;
			}
			else if (dest.x < EClass._map.bounds.x - 1)
			{
				dest.x = EClass._map.bounds.x - 1;
			}
			if (dest.z > EClass._map.bounds.maxZ + 1)
			{
				dest.z = EClass._map.bounds.maxZ + 1;
			}
			else if (dest.z < EClass._map.bounds.z - 1)
			{
				dest.z = EClass._map.bounds.z - 1;
			}
		}
		if ((repath || (owner.IsPC && ActionMode.IsAdv)) && path.state == PathProgress.State.Idle)
		{
			path.RequestPathImmediate(pos, dest, destDist, ignoreConnection);
			repath = false;
		}
		switch (path.state)
		{
		case PathProgress.State.Idle:
			path.RequestPath(pos, dest, destDist, ignoreConnection);
			break;
		case PathProgress.State.Fail:
			return Cancel();
		case PathProgress.State.PathReady:
		{
			if (path.nodeIndex < 0)
			{
				return Cancel();
			}
			PathFinderNode pathFinderNode = path.nodes[path.nodeIndex];
			if (owner.IsPC && ActionMode.IsAdv && path.nodeIndex > 2)
			{
				ActionMode.Adv.SetTurbo();
			}
			Point shared = Point.GetShared(pathFinderNode.X, pathFinderNode.Z);
			if (shared.x == owner.pos.x && shared.z == owner.pos.z && path.nodeIndex > 0 && !shared.cell.HasLadder && !owner.Cell.HasLadder)
			{
				path.nodeIndex--;
				pathFinderNode = path.nodes[path.nodeIndex];
				shared.Set(pathFinderNode.X, pathFinderNode.Z);
			}
			if (shared.HasChara && !owner.IsPC)
			{
				waitCount++;
				if (waitCount < 3 || EClass.rnd(5) != 0)
				{
					return Status.Running;
				}
			}
			waitCount = 0;
			Card.MoveResult moveResult = owner.TryMove(shared);
			if (owner == null)
			{
				return Status.Running;
			}
			if (moveResult == Card.MoveResult.Fail || (uint)(moveResult - 2) <= 1u)
			{
				if (owner.IsPC && ActionMode.IsAdv)
				{
					return Cancel();
				}
				return Restart();
			}
			if (!owner.pos.Equals(shared))
			{
				path.state = PathProgress.State.Idle;
				repath = true;
			}
			path.nodeIndex--;
			if (IsDestinationReached())
			{
				return Success();
			}
			break;
		}
		}
		return Status.Running;
	}

	public bool IsDestinationReached()
	{
		if (destCard != null)
		{
			int num = destCard.pos.Distance(owner.pos);
			if (interaction && num == 1 && owner.CanInteractTo(owner.pos))
			{
				return true;
			}
			return num <= destDist;
		}
		int num2 = Util.Distance(owner.pos.x, owner.pos.z, dest.x, dest.z);
		if (interaction && num2 == 1 && owner.CanInteractTo(dest))
		{
			return true;
		}
		return num2 <= destDist;
	}

	public override IEnumerable<Status> Run()
	{
		if (owner.host != null)
		{
			yield return Cancel();
		}
		while (true)
		{
			if (destCard != null)
			{
				dest.Set(GetDestination());
			}
			while (owner.path.state == PathProgress.State.Searching)
			{
				yield return Status.Running;
			}
			owner.path.state = PathProgress.State.Idle;
			if (IsDestinationReached())
			{
				yield return Success();
			}
			EClass.player.enemySpotted = false;
			while (true)
			{
				if (owner.IsPC)
				{
					if (EClass.player.TooHeavyToMove())
					{
						yield return Cancel();
					}
					if (EClass.player.enemySpotted)
					{
						EClass.player.enemySpotted = false;
						Msg.Say("enemy_spotted");
						yield return Cancel();
					}
				}
				yield return TryGoTo();
				if (destCard != null)
				{
					Point destination = GetDestination();
					int num = dest.Distance(destination);
					if (num > 3 && num < 20)
					{
						break;
					}
				}
			}
			repath = true;
			owner.path.state = PathProgress.State.Idle;
		}
	}
}
