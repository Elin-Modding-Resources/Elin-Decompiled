using System.Collections.Generic;

public class QuestSupplySpecific : QuestSupply
{
	public virtual string idCat => "meal";

	public virtual bool UseWeight => true;

	public override string RewardSuffix => "Supply";

	public override int GetExtraMoney()
	{
		return (int)((float)(base.sourceThing.value * num) * 0.1f) + 5 * num;
	}

	public override int GetBonus(Thing t)
	{
		int num = (int)((float)t.GetPrice(CurrencyType.Money, sell: true, PriceType.Shipping, EClass.pc) * 1.2f) * base.num - rewardMoney;
		if (num <= 0)
		{
			return 0;
		}
		return num;
	}

	public override void SetIdThing()
	{
		List<SourceThing.Row> list = new List<SourceThing.Row>();
		int num = EClass.pc.Evalue(287) * 150 / 100 + 5;
		for (int i = 0; i < 2; i++)
		{
			foreach (SourceThing.Row row in EClass.sources.things.rows)
			{
				if ((i != 0 || !(this is QuestMeal) || row.LV <= num || EClass.rnd(4) == 0) && row.category == idCat && !row.isOrigin && row.chance > 0)
				{
					list.Add(row);
				}
			}
			if (list.Count != 0)
			{
				break;
			}
		}
		idThing = (UseWeight ? list.RandomItemWeighted((SourceThing.Row a) => a.chance).id : list.RandomItem().id);
		if (difficultyType == DifficultyType.Meal)
		{
			difficulty = base.sourceThing.LV / 10 + 1;
		}
	}
}
