using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Algorithms;

[Serializable]
public class PathFinder : IPathfinder
{
	internal struct PathFinderNodeFast
	{
		public int F;

		public int G;

		public ushort PX;

		public ushort PZ;

		public byte Status;
	}

	internal class ComparePFNodeMatrix : IComparer<int>
	{
		private PathFinderNodeFast[] mMatrix;

		public ComparePFNodeMatrix(PathFinderNodeFast[] matrix)
		{
			mMatrix = matrix;
		}

		public int Compare(int a, int b)
		{
			if (mMatrix[a].F > mMatrix[b].F)
			{
				return 1;
			}
			if (mMatrix[a].F < mMatrix[b].F)
			{
				return -1;
			}
			return 0;
		}
	}

	public bool debug;

	public bool Diagonals;

	public bool PunishChangeDirection;

	public float HeavyDiagonals = 1f;

	public bool TieBreaker;

	public HeuristicFormula mFormula = HeuristicFormula.Manhattan;

	private IPathfindGrid grid;

	private IPathfindWalker walker;

	private int mHEstimate = 2;

	private PriorityQueueB<int> mOpen;

	private bool mStop;

	private bool mStopped = true;

	private int mHoriz;

	private bool mReopenCloseNodes = true;

	private PathFinderNodeFast[] mCalcGrid;

	private byte mOpenNodeValue = 1;

	private byte mCloseNodeValue = 2;

	private PathManager.MoveType moveType;

	[NonSerialized]
	public int total;

	private int mH;

	private int mLocation;

	private int mNewLocation;

	private ushort mLocationX;

	private ushort mLocationZ;

	private ushort mNewLocationX;

	private ushort mNewLocationZ;

	private int mGridXZ;

	private int mGridX;

	private int mGridZ;

	private int mCloseNodeCounter;

	private bool mFound;

	private sbyte[,] mDirection = new sbyte[8, 2]
	{
		{ 0, -1 },
		{ 1, 0 },
		{ 0, 1 },
		{ -1, 0 },
		{ -1, -1 },
		{ 1, -1 },
		{ -1, 1 },
		{ 1, 1 }
	};

	private int mEndLocation;

	private int mStartLocation;

	private int mNewG;

	private byte _weight;

	public WeightCell[,] weightMap;

	private int index;

	private int mx;

	private int mz;

	[DllImport("KERNEL32.DLL", EntryPoint = "RtlZeroMemory")]
	public unsafe static extern bool ZeroMemory(byte* destination, int length);

	public void Init(IPathfindGrid _grid, WeightCell[,] _weightMap, int size)
	{
		grid = _grid;
		weightMap = _weightMap;
		mGridX = (ushort)size;
		mGridZ = (ushort)size;
		mGridXZ = mGridX * mGridZ;
		if (mCalcGrid == null || mCalcGrid.Length != mGridXZ)
		{
			mCalcGrid = new PathFinderNodeFast[mGridXZ];
			mOpen = new PriorityQueueB<int>(new ComparePFNodeMatrix(mCalcGrid));
		}
	}

	public void FindPath(PathProgress path)
	{
		moveType = path.moveType;
		walker = path.walker;
		_FindPath(path);
		if (path.nodes.Count > 0)
		{
			PathFinderNode pathFinderNode = path.nodes[path.nodes.Count - 1];
			if (pathFinderNode.X == path.startPoint.x && pathFinderNode.Z == path.startPoint.z)
			{
				path.nodes.RemoveAt(path.nodes.Count - 1);
			}
		}
		if (path.nodes.Count == 0)
		{
			path.state = PathProgress.State.Fail;
			return;
		}
		path.state = PathProgress.State.PathReady;
		path.nodeIndex = path.nodes.Count - 1;
	}

	private void _FindPath(PathProgress path)
	{
		lock (this)
		{
			Point startPoint = path.startPoint;
			Point destPoint = path.destPoint;
			path.nodes.Clear();
			mFound = (mStop = (mStopped = false));
			mCloseNodeCounter = 0;
			mOpenNodeValue += 2;
			mCloseNodeValue += 2;
			mOpen.Clear();
			mLocation = (mStartLocation = startPoint.z * mGridX + startPoint.x);
			mEndLocation = destPoint.z * mGridX + destPoint.x;
			if (mLocation >= mCalcGrid.Length)
			{
				if (debug)
				{
					Debug.Log("length over");
				}
			}
			else
			{
				if (mEndLocation == mLocation)
				{
					return;
				}
				mCalcGrid[mLocation].G = 0;
				mCalcGrid[mLocation].F = mHEstimate;
				mCalcGrid[mLocation].PX = (ushort)startPoint.x;
				mCalcGrid[mLocation].PZ = (ushort)startPoint.z;
				mCalcGrid[mLocation].Status = mOpenNodeValue;
				mOpen.Push(mLocation);
				while (mOpen.Count > 0 && !mStop)
				{
					mLocation = mOpen.Pop();
					if (mCalcGrid[mLocation].Status == mCloseNodeValue)
					{
						continue;
					}
					mLocationX = (ushort)(mLocation % mGridX);
					mLocationZ = (ushort)(mLocation % mGridXZ / mGridX);
					if (mLocation == mEndLocation)
					{
						mCalcGrid[mLocation].Status = mCloseNodeValue;
						mFound = true;
						break;
					}
					if (mCloseNodeCounter > path.searchLimit)
					{
						mStopped = true;
						if (path.searchLimit > 1000)
						{
							Debug.Log("search limit: " + path.startPoint?.ToString() + " " + path.destPoint);
						}
						return;
					}
					if (PunishChangeDirection)
					{
						mHoriz = mLocationX - mCalcGrid[mLocation].PX;
					}
					for (int i = 0; i < (Diagonals ? 8 : 4); i++)
					{
						mNewLocationX = (ushort)(mLocationX + mDirection[i, 0]);
						mNewLocationZ = (ushort)(mLocationZ + mDirection[i, 1]);
						mNewLocation = mNewLocationZ * mGridX + mNewLocationX;
						if (mNewLocationX >= mGridX || mNewLocationZ >= mGridZ || (mCalcGrid[mNewLocation].Status == mCloseNodeValue && !mReopenCloseNodes))
						{
							continue;
						}
						if (path.ignoreConnection && mEndLocation == mNewLocation)
						{
							_weight = 1;
						}
						else
						{
							if (i < 4)
							{
								index = ((mNewLocationZ >= mLocationZ) ? ((mNewLocationX > mLocationX) ? 1 : ((mNewLocationZ > mLocationZ) ? 2 : 3)) : 0);
								_weight = weightMap[mLocationX, mLocationZ].weights[index];
							}
							else
							{
								mx = mLocationX + mDirection[i, 0];
								mz = mLocationZ;
								index = ((mz >= mLocationZ) ? ((mx > mLocationX) ? 1 : ((mz > mLocationZ) ? 2 : 3)) : 0);
								_weight = weightMap[mLocationX, mLocationZ].weights[index];
								if (weightMap[mx, mz].IsPathBlocked(walker, moveType))
								{
									_weight = 0;
								}
								if (_weight > 0)
								{
									index = ((mz >= mNewLocationZ) ? ((mx > mNewLocationX) ? 1 : ((mz > mNewLocationZ) ? 2 : 3)) : 0);
									_weight = weightMap[mNewLocationX, mNewLocationZ].weights[index];
									if (_weight > 0)
									{
										mx = mLocationX;
										mz = mLocationZ + mDirection[i, 1];
										index = ((mz >= mLocationZ) ? ((mx > mLocationX) ? 1 : ((mz > mLocationZ) ? 2 : 3)) : 0);
										_weight = weightMap[mLocationX, mLocationZ].weights[index];
										if (weightMap[mx, mz].IsPathBlocked(walker, moveType))
										{
											_weight = 0;
										}
										if (_weight > 0)
										{
											index = ((mz >= mNewLocationZ) ? ((mx > mNewLocationX) ? 1 : ((mz > mNewLocationZ) ? 2 : 3)) : 0);
											_weight = weightMap[mNewLocationX, mNewLocationZ].weights[index];
										}
									}
								}
							}
							if (_weight == 0)
							{
								continue;
							}
							_weight += weightMap[mLocationX, mLocationZ].baseWeight;
							if (mEndLocation != mNewLocation && weightMap[mNewLocationX, mNewLocationZ].IsPathBlocked(walker, moveType))
							{
								continue;
							}
						}
						mNewG = mCalcGrid[mLocation].G + _weight;
						if (PunishChangeDirection)
						{
							if (mNewLocationX - mLocationX != 0 && mHoriz == 0)
							{
								mNewG += Math.Abs(mNewLocationX - destPoint.x) + Math.Abs(mNewLocationZ - destPoint.z);
							}
							if (mNewLocationZ - mLocationZ != 0 && mHoriz != 0)
							{
								mNewG += Math.Abs(mNewLocationX - destPoint.x) + Math.Abs(mNewLocationZ - destPoint.z);
							}
						}
						if ((mCalcGrid[mNewLocation].Status != mOpenNodeValue && mCalcGrid[mNewLocation].Status != mCloseNodeValue) || mCalcGrid[mNewLocation].G > mNewG)
						{
							mCalcGrid[mNewLocation].PX = mLocationX;
							mCalcGrid[mNewLocation].PZ = mLocationZ;
							mCalcGrid[mNewLocation].G = mNewG;
							switch (mFormula)
							{
							default:
								mH = mHEstimate * (Math.Abs(mNewLocationX - destPoint.x) + Math.Abs(mNewLocationZ - destPoint.z));
								break;
							case HeuristicFormula.MaxDXDY:
								mH = mHEstimate * Math.Max(Math.Abs(mNewLocationX - destPoint.x), Math.Abs(mNewLocationZ - destPoint.z));
								break;
							case HeuristicFormula.DiagonalShortCut:
							{
								int num = Math.Min(Math.Abs(mNewLocationX - destPoint.x), Math.Abs(mNewLocationZ - destPoint.z));
								int num2 = Math.Abs(mNewLocationX - destPoint.x) + Math.Abs(mNewLocationZ - destPoint.z);
								mH = mHEstimate * 2 * num + mHEstimate * (num2 - 2 * num);
								break;
							}
							case HeuristicFormula.Euclidean:
								mH = (int)((double)mHEstimate * Math.Sqrt(Math.Pow(mNewLocationZ - destPoint.x, 2.0) + Math.Pow(mNewLocationZ - destPoint.z, 2.0)));
								break;
							case HeuristicFormula.EuclideanNoSQR:
								mH = (int)((double)mHEstimate * (Math.Pow(mNewLocationX - destPoint.x, 2.0) + Math.Pow(mNewLocationZ - destPoint.z, 2.0)));
								break;
							}
							if (TieBreaker)
							{
								int num3 = mLocationX - destPoint.x;
								int num4 = mLocationZ - destPoint.z;
								int num5 = startPoint.x - destPoint.x;
								int num6 = startPoint.z - destPoint.z;
								int num7 = Math.Abs(num3 * num6 - num5 * num4);
								mH = (int)((double)mH + (double)num7 * 0.001);
							}
							mCalcGrid[mNewLocation].F = mNewG + mH;
							mOpen.Push(mNewLocation);
							mCalcGrid[mNewLocation].Status = mOpenNodeValue;
						}
					}
					mCloseNodeCounter++;
					mCalcGrid[mLocation].Status = mCloseNodeValue;
				}
				if (mFound)
				{
					int x = destPoint.x;
					int z = destPoint.z;
					PathFinderNodeFast pathFinderNodeFast = mCalcGrid[destPoint.z * mGridX + destPoint.x];
					PathFinderNode item = default(PathFinderNode);
					item.G = pathFinderNodeFast.G;
					item.PX = pathFinderNodeFast.PX;
					item.PZ = pathFinderNodeFast.PZ;
					item.X = destPoint.x;
					item.Z = destPoint.z;
					while (item.X != item.PX || item.Z != item.PZ)
					{
						path.nodes.Add(item);
						x = item.PX;
						z = item.PZ;
						pathFinderNodeFast = mCalcGrid[z * mGridX + x];
						item.G = pathFinderNodeFast.G;
						item.PX = pathFinderNodeFast.PX;
						item.PZ = pathFinderNodeFast.PZ;
						item.X = x;
						item.Z = z;
					}
					path.nodes.Add(item);
					mStopped = true;
				}
				else
				{
					mStopped = true;
				}
			}
		}
	}
}
