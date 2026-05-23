using System.Collections.Generic;
using System.Threading;
using Algorithms;
using UnityEngine;

public class PathManager : MonoBehaviour
{
	public enum MoveType
	{
		Default,
		Combat
	}

	public static int requestCount;

	public static PathManager Instance;

	public static PathProgress tempProgress = new PathProgress();

	public PathFinder _pathfinder;

	public int searchLimit = 1000000;

	public IPathfinder pathfinder => _pathfinder;

	private void Awake()
	{
		Instance = this;
	}

	public void RequestPath(PathProgress progress)
	{
		requestCount++;
		progress.state = PathProgress.State.Searching;
		ThreadPool.QueueUserWorkItem(delegate
		{
			new PathThread().Start(progress);
		});
	}

	public void RequestPathImmediate(PathProgress progress)
	{
		requestCount++;
		pathfinder.FindPath(progress);
	}

	public bool IsPathClear(Point origin, Point dest, IPathfindWalker walker, int radius)
	{
		tempProgress.walker = walker;
		tempProgress.moveType = MoveType.Default;
		tempProgress.RequestPathImmediate(origin, dest, 0, _ignoreConnection: false);
		if (tempProgress.nodes.Count > 0)
		{
			return tempProgress.nodes.Count < radius;
		}
		return false;
	}

	public PathProgress RequestPathImmediate(Point origin, Point dest, IPathfindWalker walker, MoveType moveType = MoveType.Default, int searchLimit = -1, int destDist = 0)
	{
		tempProgress.walker = walker;
		tempProgress.moveType = moveType;
		tempProgress.RequestPathImmediate(origin, dest, destDist, _ignoreConnection: false, searchLimit);
		return tempProgress;
	}

	public Point GetFirstStep(Point origin, Point _dest, IPathfindWalker walker, int maxDist = 20, MoveType moveType = MoveType.Default)
	{
		Point dest = _dest.Copy();
		Point point = _GetFirstStep(origin, dest, walker, maxDist, moveType);
		if (point.IsValid)
		{
			return point;
		}
		return Point.Invalid;
	}

	public Point _GetFirstStep(Point origin, Point dest, IPathfindWalker walker, int maxDist = 20, MoveType moveType = MoveType.Default)
	{
		if (!dest.IsValid || (dest.cell.blocked && !walker.IsAstralBody && origin.Distance(dest) <= 1))
		{
			return Point.Invalid;
		}
		tempProgress.walker = walker;
		tempProgress.moveType = moveType;
		tempProgress.RequestPathImmediate(origin, dest, (moveType == MoveType.Combat) ? 1 : 0, _ignoreConnection: false, maxDist);
		if (!tempProgress.HasPath)
		{
			return Point.Invalid;
		}
		List<PathFinderNode> nodes = tempProgress.nodes;
		if (nodes.Count > 0)
		{
			PathFinderNode pathFinderNode = nodes[nodes.Count - 1];
			if (pathFinderNode.X == origin.x && pathFinderNode.Z == origin.z && nodes.Count > 1)
			{
				pathFinderNode = nodes[nodes.Count - 2];
			}
			if (Mathf.Abs(pathFinderNode.X - origin.x) > 1 || Mathf.Abs(pathFinderNode.Z - origin.z) > 1)
			{
				return Point.Invalid;
			}
			Point point = new Point(pathFinderNode.X, pathFinderNode.Z);
			if (point.x == origin.x && point.z == origin.z)
			{
				return Point.Invalid;
			}
			return point;
		}
		return Point.Invalid;
	}

	public void OnGridModified()
	{
	}
}
