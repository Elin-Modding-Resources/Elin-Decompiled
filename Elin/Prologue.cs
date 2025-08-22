using System;
using UnityEngine;

[Serializable]
public class Prologue
{
	public GameType type;

	public string idStartZone;

	public int startX;

	public int startZ;

	public int year;

	public int month;

	public int day;

	public int hour;

	public Vector2Int posAsh;

	public Vector2Int posFiama;

	public Vector2Int posPunk;

	public Weather.Condition weather;
}
