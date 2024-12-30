using UnityEngine;

public class Resistance : Element
{
	public override bool ShowPotential => false;

	public override bool ShowXP => false;

	public override string ShortName => EClass.sources.elements.alias.TryGetValue(base.source.aliasParent)?.GetName() ?? base.source.aliasParent;

	public override bool CanLink(ElementContainer owner)
	{
		return !base.IsGlobalElement;
	}

	public override Sprite GetIcon(string suffix = "")
	{
		return SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", "ele_" + base.source.alias) ?? SpriteSheet.Get("Media/Graphics/Icon/Element/icon_elements", "ele_resist");
	}

	public override void SetTextValue(UIText text)
	{
		text.text = "なし";
	}
}
