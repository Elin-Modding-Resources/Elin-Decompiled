using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class HotItemAct : HotItemGameAction
{
	[JsonProperty]
	public int id;

	public Act _act;

	public override Act act => _act ?? (_act = ACT.Create(id));

	public override string Name => source.GetName();

	public SourceElement.Row source => EClass.sources.elements.map.TryGetValue(id) ?? EClass.sources.elements.rows[0];

	public override bool UseUIObjMaterial => true;

	public override Color SpriteColor
	{
		get
		{
			if (!source.aliasRef.IsEmpty())
			{
				return EClass.setting.elements[source.aliasRef].colorSprite;
			}
			return Color.white;
		}
	}

	public override Sprite GetSprite()
	{
		return act.GetSprite();
	}

	public HotItemAct()
	{
	}

	public HotItemAct(string alias)
	{
		id = EClass.sources.elements.alias[alias].id;
	}

	public HotItemAct(SourceElement.Row row)
	{
		id = row.id;
	}

	public override void SetImage(Image icon)
	{
		act.SetImage(icon);
	}

	public override bool TrySetAct(ActPlan p)
	{
		Card tg = null;
		p.pos.ListVisibleCards().ForeachReverse(delegate(Card a)
		{
			if (EClass.pc.CanSee(a) && !a.HasHost && act.IsValidTC(a))
			{
				tg = a;
				return true;
			}
			return false;
		});
		if (!act.CanPerform(EClass.pc, tg, p.pos))
		{
			return false;
		}
		p.TrySetAct(((EClass.pc.elements.GetElement(act.id) as Act) ?? act).GetText(), delegate
		{
			if (tg != null && !tg.ExistsOnMap)
			{
				return false;
			}
			Point point = ((tg != null && tg.ExistsOnMap) ? tg.pos : p.pos);
			if (!act.CanPerform(EClass.pc, tg, point))
			{
				return false;
			}
			Debug.Log("hotitemact:" + this?.ToString() + "/" + act.id + "/" + tg?.ToString() + "/" + p.performed);
			return (p.performed && act.CanPressRepeat) ? EClass.pc.UseAbility(act.source.alias, tg, point) : ButtonAbility.TryUse(act, tg, point);
		}, tg, null, -1, EClass._zone.IsCrime(EClass.pc, act), act.LocalAct, act.CanPressRepeat);
		return true;
	}
}
