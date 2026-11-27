using UnityEngine;
using UnityEngine.UI;

public class TraitFigure : Trait
{
	public CardRenderer extraRenderer;

	public override bool CanBeHallucinated => false;

	public CardRow source => EClass.sources.cards.map.TryGetValue(owner.c_idRefCard);

	public virtual bool ShowShadow => true;

	public virtual int GetMatColor()
	{
		return -1;
	}

	public override void OnSetCardGrid(ButtonGrid b)
	{
		if (!owner.c_idRefCard.IsEmpty())
		{
			SourceChara.Row row = EClass.sources.charas.map.TryGetValue(owner.c_idRefCard) ?? EClass.sources.charas.map["putty"];
			Transform transform = b.Attach<Transform>("figure", rightAttach: false);
			int num = ((EClass.core.config.game.antiSpider && row.skinAntiSpider != 0) ? row.skinAntiSpider : owner.refVal);
			Image component = transform.GetChild(0).GetComponent<Image>();
			RectTransform rectTransform = component.rectTransform;
			RectTransform componentInChildren = ResourceCache.Load<Transform>("UI/Element/Grid/Attach/figure").GetComponentInChildren<RectTransform>();
			rectTransform.anchorMin = componentInChildren.anchorMin;
			rectTransform.anchorMax = componentInChildren.anchorMax;
			rectTransform.anchoredPosition = componentInChildren.anchoredPosition;
			rectTransform.sizeDelta = componentInChildren.sizeDelta;
			if (!component.maskable)
			{
				component.maskable = true;
				component.RecalculateClipping();
			}
			row.SetImage(component, null, 0, setNativeSize: false, 0, num);
			if ((bool)component.sprite && row.idRenderData[0] == '@' && row.GetSpriteData(0, num, snow: false) != null)
			{
				SetFigureSprite(rectTransform, row, component.sprite);
			}
		}
	}

	public static void SetFigureSprite(RectTransform r, SourceChara.Row source, Sprite sprite)
	{
		RenderData renderData = ResourceCache.Load<RenderData>(source.pathRenderData + source.idRenderData.Replace("@", "").Split('#')[0]) ?? source.defaultRenderData;
		if (!renderData || !renderData.pass || renderData.pass.pmesh.size.y <= 0f)
		{
			return;
		}
		float y = renderData.pass.pmesh.size.y;
		if (Mathf.Approximately(source.renderData.offset.y, renderData.offset.y + y * 0.5f))
		{
			float num = SpriteData.BaseWidth(sprite) * 0.01f / y;
			float num2 = SpriteData.BaseHeight(sprite) * 0.01f / y;
			if (!(num <= 1f) || !(num2 <= 1f))
			{
				float num3 = 1f / Mathf.Max(num, num2);
				float num4 = 0.5f + (source.replacer.data?.pref ?? source.pref).y / y;
				Vector2 a = new Vector2(0.5f - num * num3 * 0.5f, (num4 - num2 * 0.5f) * num3);
				Vector2 a2 = new Vector2(0.5f + num * num3 * 0.5f, (num4 + num2 * 0.5f) * num3);
				Vector2 anchorMin = r.anchorMin;
				Vector2 b = r.anchorMax - r.anchorMin;
				Vector2 offsetMin = r.offsetMin;
				Vector2 b2 = r.offsetMax - r.offsetMin;
				r.anchorMin = anchorMin + Vector2.Scale(a, b);
				r.anchorMax = anchorMin + Vector2.Scale(a2, b);
				r.offsetMin = offsetMin + Vector2.Scale(a, b2);
				r.offsetMax = offsetMin + Vector2.Scale(a2, b2);
			}
		}
	}

	public override int GetValue()
	{
		if (source == null)
		{
			return base.GetValue();
		}
		return MathEx.ClampToInt((base.GetValue() + Mathf.Min(source.LV, 1000000) * 50) * ((!source.multisize) ? 1 : 2) * ((source.quality < 4) ? 1 : 2));
	}

	public override void TrySetAct(ActPlan p)
	{
		if (p.input != ActInput.AllAction || source == null || source.tiles.Length <= 1)
		{
			return;
		}
		p.TrySetAct("actChangeType", delegate
		{
			UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
			uIContextMenu.AddSlider("actChangeType", (float a) => a.ToString() ?? "", owner.refVal, delegate(float b)
			{
				owner.refVal = (int)b;
			}, 0f, source.tiles.Length - 1, isInt: true);
			uIContextMenu.Show();
			return false;
		}, owner);
	}
}
