using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DSVRow : MonoBehaviour
{
	public class Item
	{
		public object obj;

		public Component comp;
	}

	public List<Item> items = new List<Item>();

	public UIItem itemHeader;

	public RawImage bgGrid;

	[NonSerialized]
	public RectTransform _rect;

	private void Awake()
	{
		_rect = this.Rect();
	}
}
