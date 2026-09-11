using System;
using UnityEngine;

public class TileRow : RenderRow
{
	[NonSerialized]
	public bool ignoreSnow;

	public int id;

	public int hp;

	public string alias;

	public string soundFoot;

	public void Init()
	{
		if (!TileType.dict.TryGetValue(_tileType, out tileType))
		{
			Debug.LogError($"#source unknown tile type '{_tileType}' for tile row {id}/{alias}");
			tileType = TileType.None;
		}
		SetRenderData();
		OnInit();
	}

	public virtual void OnInit()
	{
	}
}
