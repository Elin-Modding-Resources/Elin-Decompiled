using UnityEngine;

public class CalcPlat : EClass
{
	public static int Learn(Chara c, Element e)
	{
		int num = Mathf.Max(1, e.CostLearn * (c.HasElement(1202) ? 80 : 100) / 100);
		if (e.source.tag.Contains("guild") && Guild.CurrentDrama.relation.rank < 2)
		{
			return num * 2;
		}
		return num;
	}

	public static int Train(Chara c, Element _e)
	{
		Element element = c.elements.GetElement(_e.id);
		if (element.vTempPotential >= 1000)
		{
			return 0;
		}
		int num = Mathf.Max(1, element.CostTrain * (c.HasElement(1202) ? 80 : 100) / 100);
		if (element.source.tag.Contains("guild") && Guild.CurrentDrama.relation.rank < 2)
		{
			return num * 2;
		}
		return num;
	}
}
