using System.Collections.Generic;
using System.Linq;

public class ListPeopleRevive : BaseListPeople
{
	public override bool IsDisabled(Chara c)
	{
		return false;
	}

	public override void OnInstantiate(Chara a, ItemGeneral b)
	{
		base.OnInstantiate(a, b);
		int money = EClass.pc.GetCurrency();
		b.AddPrefab<UIItem>("costMoney").text1.SetText((CalcMoney.Revive(a).ToString() ?? "").TagColorGoodBad(() => money >= CalcMoney.Revive(a)));
	}

	public override void OnClick(Chara c, ItemGeneral i)
	{
		if (EClass.pc.TryPay(CalcMoney.Revive(c)))
		{
			c.GetRevived();
			list.List();
		}
	}

	public override void OnList()
	{
		foreach (KeyValuePair<int, Chara> item in EClass.game.cards.globalCharas.Where((KeyValuePair<int, Chara> a) => a.Value.CanRevive() && a.Value.isDead && a.Value.faction == EClass.pc.faction && !a.Value.isSummon))
		{
			list.Add(item.Value);
		}
	}
}
