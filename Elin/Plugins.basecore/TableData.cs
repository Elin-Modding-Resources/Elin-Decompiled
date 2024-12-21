using System;
using System.Collections.Generic;
using UnityEngine;

public class TableData : ScriptableObject
{
	[Serializable]
	public class Col
	{
		public List<string> rows = new List<string>();
	}

	public List<Col> cols;

	public List<string> index;

	public Dictionary<string, List<string>> map = new Dictionary<string, List<string>>();

	public void Init()
	{
		for (int i = 0; i < index.Count; i++)
		{
			map.Add(index[i], cols[i].rows);
		}
	}
}
