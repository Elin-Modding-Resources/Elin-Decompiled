using UnityEngine;

public class DamageTextRenderer : EClass
{
	public Card lastTarget;

	public Card lastAttacker;

	public Element lastElement = Element.Void;

	public int sum;

	public int num;

	public void Add(Card target, Card attacker, int dmg, Element e = null)
	{
		if (e == null)
		{
			e = Element.Void;
		}
		if (lastTarget != target || lastAttacker != attacker || lastElement.id != e.id)
		{
			Flush();
			lastTarget = target;
			lastAttacker = attacker;
			lastElement = e;
		}
		sum += dmg;
		num++;
		if (!EClass.core.config.test.stackNumbers)
		{
			Flush();
		}
	}

	public void Flush()
	{
		if (this.num != 0)
		{
			Card card = lastTarget;
			Card card2 = lastAttacker;
			Element element = lastElement;
			Popper popper = EClass.scene.popper.Pop(card.renderer.PositionCenter(), "DamageNum");
			Color c = EClass.Colors.textColors.damage;
			if (card2 != null)
			{
				c = (card2.IsPC ? EClass.Colors.textColors.damagePC : (card2.IsPCFaction ? EClass.Colors.textColors.damagePCParty : EClass.Colors.textColors.damage));
			}
			if (element != Element.Void)
			{
				c = EClass.Colors.elementColors.TryGetValue(element.source.alias);
				float num = (c.r + c.g + c.b) / 3f;
				num = ((num > 0.5f) ? 0f : (0.6f - num));
				c = new Color(c.r + num, c.g + num, c.b + num, 1f);
			}
			popper.SetText((this.num == 1) ? (sum.ToString() ?? "") : (sum + "<size=18> (x" + this.num + ")</size>"), c);
			sum = (this.num = 0);
		}
	}
}
