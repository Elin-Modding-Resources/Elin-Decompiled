using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_Trolley : AIAct
{
	public static int[][] DirList = new int[4][]
	{
		new int[4] { 0, 1, 3, 2 },
		new int[4] { 1, 0, 2, 3 },
		new int[4] { 2, 1, 3, 0 },
		new int[4] { 3, 2, 0, 1 }
	};

	public static Vector2Int[] VecList = new Vector2Int[4]
	{
		new Vector2Int(0, -1),
		new Vector2Int(1, 0),
		new Vector2Int(0, 1),
		new Vector2Int(-1, 0)
	};

	public TraitTrolley trolley;

	public bool running;

	public int dir => trolley.owner.dir;

	public override bool CancelWhenDamaged
	{
		get
		{
			if (owner != null)
			{
				if (owner.IsPC)
				{
					return owner.hp < owner.MaxHP / 3;
				}
				return true;
			}
			return false;
		}
	}

	public override bool CancelWhenMoved => true;

	public override bool CancelOnAggro => false;

	public override bool ShowCursor => false;

	public override bool CanManualCancel()
	{
		return true;
	}

	public override IEnumerable<Status> Run()
	{
		owner.Say("ride", owner, trolley.owner);
		if (!trolley.HideChara)
		{
			owner.Talk("ride2");
		}
		while (true)
		{
			int nextDir = GetNextDir();
			if (!trolley.owner.ExistsOnMap || nextDir == -1)
			{
				yield return Stop();
			}
			if (owner.rarity >= Rarity.Legendary && owner.FindNearestNewEnemy())
			{
				yield return Stop();
			}
			trolley.owner.dir = nextDir;
			Point point = GetPoint(dir);
			owner.SetDir(nextDir);
			owner.PlayAnime(AnimeID.Truck);
			string idSound = trolley.GetIdSound();
			if (owner.IsPC)
			{
				owner.PlaySound(idSound);
			}
			else if (!(EClass.pc.ai is AI_Trolley))
			{
				owner.PlaySound(idSound);
				EClass.Sound.Stop(idSound, Mathf.Max(1f, trolley.FadeDuration));
			}
			foreach (Chara item in point.ListCharas())
			{
				owner.Kick(item, ignoreSelf: true, karmaLoss: false);
			}
			EClass._map.MoveCard(point, owner);
			trolley.owner.MoveImmediate(point);
			trolley.owner.PlayAnime(AnimeID.Truck);
			running = true;
			yield return KeepRunning();
		}
	}

	public Point GetPoint(int d, bool onlyValid = true)
	{
		Point point = new Point();
		Point pos = trolley.owner.pos;
		point.Set(pos.x + VecList[d].x, pos.z + VecList[d].y);
		if (!point.IsValid || !point.IsInBounds || !point.HasRail || point.IsBlocked)
		{
			return null;
		}
		foreach (Thing thing in point.Things)
		{
			if (!thing.IsMultisize)
			{
				return null;
			}
		}
		if (EClass.pc.pos.Equals(point))
		{
			return null;
		}
		return point;
	}

	public int GetNextDir()
	{
		int[] array = DirList[dir];
		for (int i = 0; i < 3 + ((!running) ? 1 : 0); i++)
		{
			int num = array[i];
			if (GetPoint(num) != null)
			{
				return num;
			}
		}
		return -1;
	}

	public Status Stop(Action action = null)
	{
		EClass.Sound.Stop(trolley.GetIdSound(), trolley.FadeDuration);
		if (owner != null)
		{
			owner.Say("ride_unride", owner, trolley.owner);
		}
		return Success();
	}

	public override void OnCancel()
	{
		Stop();
	}
}
