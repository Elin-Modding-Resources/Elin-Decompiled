using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class BaseStats : EClass
{
	public static Chara CC;

	[JsonProperty]
	public int id;

	public SourceStat.Row _source;

	public SourceStat.Row source => _source ?? (_source = EClass.sources.stats.map[id]);

	public virtual Emo2 EmoIcon => Emo2.none;

	public virtual ConditionType Type => source.group.ToEnum<ConditionType>();

	public virtual string idSprite => source.element.IsEmpty(source.alias);

	public virtual bool ShowInWidget => true;

	public virtual Chara Owner => CC;

	public virtual Color GetColor(Gradient gradient)
	{
		return Color.white;
	}

	public virtual Color GetColor(SkinColorProfile c)
	{
		return GetColor(c.gradients[source.colors.IsEmpty("default")]);
	}

	public Color GetColor()
	{
		return GetColor(SkinManager.CurrentColors);
	}

	public virtual string GetText()
	{
		return null;
	}

	public virtual string GetPhaseStr()
	{
		return GetText();
	}

	public virtual int GetValue()
	{
		return 0;
	}

	public virtual int GetSortVal()
	{
		return source.id;
	}

	public virtual Sprite GetSprite()
	{
		return SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", idSprite) ?? EClass.core.refs.spriteDefaultCondition;
	}

	public virtual void SetText(UIText t, SkinColorProfile cols = null)
	{
		if (cols == null)
		{
			cols = EClass.ui.skins.currentSkin.colors._default;
		}
		t.SetText(GetText(), GetColor(cols));
	}

	public virtual int GetPhase()
	{
		return 0;
	}

	public void PopText()
	{
		if (EClass.core.IsGameStarted && Owner.ShouldShowMsg)
		{
			string phaseStr = GetPhaseStr();
			if (!phaseStr.IsEmpty() && !(phaseStr == "#"))
			{
				Popper popper = EClass.scene.popper.Pop(Owner.renderer.PositionCenter(), "Condition");
				Color c = GetColor() * 1.3f;
				c.r += 0.3f;
				c.g += 0.3f;
				c.b += 0.3f;
				popper.SetText(phaseStr, c);
			}
		}
	}

	public virtual void WriteNote(UINote n, Action<UINote> onWriteNote = null)
	{
		n.Clear();
		n.AddHeader(source.GetName());
		string text = (Lang.Has("contype_" + Type) ? ("(" + ("contype_" + Type).lang() + ") ") : "");
		n.AddText("NoteText_flavor_element", text + source.GetDetail());
		_WriteNote(n);
		n.Build();
	}

	public virtual void _WriteNote(UINote n, bool asChild = false)
	{
		List<string> list = new List<string>();
		string[] nullify = source.nullify;
		foreach (string key in nullify)
		{
			list.Add("hintNullify".lang(EClass.sources.stats.alias[key].GetName()));
		}
		if (list.Count <= 0)
		{
			return;
		}
		if (!asChild)
		{
			n.Space(8);
		}
		foreach (string item in list)
		{
			n.AddText("_bullet".lang() + item);
		}
	}
}
