using System;
using System.Collections.Generic;
using UnityEngine;

public class CardRow : RenderRow
{
	public string id;

	public string idExtra;

	public string tierGroup;

	public string lightData;

	public string _origin;

	public int idShadow;

	public int quality;

	public int[] elements;

	public int[] shadow;

	public int[] size;

	public int[] light;

	public string[] loot;

	public string[] filter;

	public string[] trait;

	public string[] idActor;

	public string[] vals;

	public string[] name2;

	public string[] name2_JP;

	public Dictionary<int, int> elementMap;

	[NonSerialized]
	public Sprite sprite;

	[NonSerialized]
	public CardRow origin;

	[NonSerialized]
	public bool isOrigin;

	[NonSerialized]
	public bool isChara;

	[NonSerialized]
	public Card _model;

	public Card model => _model ?? (_model = (isChara ? ((Card)CharaGen.Create(id)) : ((Card)ThingGen.Create(id))));

	public override string idRenderData => _idRenderData.IsEmpty((isChara ? "Chara/" : "Thing/") + id);

	public override string idSprite => id;

	public override string idString => id;

	public override string pathSprite => pathRenderData + (isChara ? "Chara/" : "Thing/");

	public override SourcePref GetPref()
	{
		if (origin != null && !pref.UsePref)
		{
			return origin.pref;
		}
		return pref;
	}

	public override void OnImportData(SourceData data)
	{
		base.OnImportData(data);
		if (size == null || size.Length == 0)
		{
			W = (H = 1);
			multisize = false;
		}
		else
		{
			W = size[0];
			H = size[1];
			multisize = true;
		}
	}

	public virtual string GetName(int i)
	{
		return GetName() + " x " + i;
	}

	public virtual string GetName(SourceMaterial.Row mat, int sum)
	{
		return "_of2".lang(mat.GetName(), GetName()) + " (" + sum + ")";
	}

	public override string GetEditorListName()
	{
		return this.GetField<string>("id") + "-" + this.GetField<string>("name_JP");
	}

	public override string GetName()
	{
		return GetName(null);
	}

	public string GetName(Card c, bool full = false)
	{
		string text = base.GetName();
		SourceElement.Row source = Element.Void.source;
		if (c != null && c.isChara)
		{
			source = c.Chara.MainElement.source;
		}
		text = ((source == Element.Void.source) ? text.Replace("#ele4", "").Replace("#ele3", "").Replace("#ele2", "")
			.Replace("#ele", "") : text.Replace("#ele4", source.GetAltname(2)).Replace("#ele3", source.GetAltname(1)).Replace("#ele2", source.GetAltname(0))
			.Replace("#ele", source.GetName().ToLower()));
		string text2 = GetText("aka");
		if (text == "*r")
		{
			return text2;
		}
		int type = ((quality == 3) ? 3 : ((quality < 2) ? 1 : 2));
		if (full && !text2.IsEmpty())
		{
			text = text2.ToTitleCase(wholeText: true) + Lang.space + text.Bracket(type);
		}
		return text;
	}
}
