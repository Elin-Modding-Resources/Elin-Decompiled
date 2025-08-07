using System;
using System.Collections.Generic;
using UnityEngine;

public class SpriteReplacerAnimation : MonoBehaviour
{
	public enum Type
	{
		Default,
		Boat
	}

	public static Dictionary<string, SpriteData> dict = new Dictionary<string, SpriteData>();

	public Type type;

	public string id;

	public SpriteData data;

	[NonSerialized]
	public SpriteRenderer sr;

	private int index;

	private void Awake()
	{
		if (!id.IsEmpty())
		{
			SetData(id);
		}
	}

	public void SetData(string id)
	{
		this.id = id;
		string text = id.IsEmpty(base.name + "_anime");
		string path = CorePath.packageCore + "Texture/Item/" + text;
		data = dict.TryGetValue(text);
		if (data == null)
		{
			data = new SpriteData
			{
				path = path
			};
			data.Init();
			dict.Add(text, data);
		}
		sr = GetComponent<SpriteRenderer>();
		sr.sprite = data.GetSprite();
		if (type == Type.Default)
		{
			CancelInvoke();
			InvokeRepeating("Refresh", 0f, data.time);
		}
	}

	private void Refresh()
	{
		index++;
		if (index >= data.frame)
		{
			index = 0;
		}
		sr.sprite = data.GetSprites()[index];
	}
}
