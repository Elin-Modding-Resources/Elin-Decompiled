using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TraitPotionAlchemy : TraitPotionRandom
{
	public override int Power => owner.Power * (100 + owner.encLV * 100) / 100;

	public override void OnCrafted(Recipe recipe)
	{
		owner.refVal = 0;
		List<Element> list = owner.elements.dict.Values.Where((Element e) => e.IsTrait).ToList();
		list.Sort((Element a, Element b) => Mathf.Abs(b.Value) - Mathf.Abs(a.Value));
		int num = 0;
		foreach (Element item in list)
		{
			int num2 = item.Value / 10;
			switch (item.id)
			{
			case 750:
			case 753:
				num = ((num2 >= 6) ? 8402 : ((num2 >= 4) ? 8401 : 8400));
				break;
			case 754:
				num = 8471;
				break;
			case 755:
				num = 8470;
				break;
			case 751:
				num = 8501;
				break;
			case 752:
				num = 8791;
				break;
			case 760:
				num = 8704;
				break;
			case 756:
				num = 8506;
				break;
			case 763:
				num = 8507;
				break;
			}
			if (num != 0)
			{
				break;
			}
		}
		owner.refVal = num;
		owner.SetEncLv(Mathf.Min(EClass.pc.Evalue(257) / 10, EClass.debug.enable ? 1000 : owner.QualityLv));
	}
}
