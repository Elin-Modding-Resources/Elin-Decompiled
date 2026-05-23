namespace Algorithms;

public class WeightCell
{
	public bool blocked;

	public byte[] weights = new byte[4];

	public byte baseWeight;

	public virtual bool IsPathBlocked(IPathfindWalker walker, PathManager.MoveType moveType)
	{
		return blocked;
	}
}
