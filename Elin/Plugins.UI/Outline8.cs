using System.Collections.Generic;
using UnityEngine;

public class Outline8 : ModifiedShadow
{
	public override void ModifyVertices(List<UIVertex> verts)
	{
		if (!IsActive())
		{
			return;
		}
		int num = verts.Count * 9;
		if (verts.Capacity < num)
		{
			verts.Capacity = num;
		}
		int count = verts.Count;
		int num2 = 0;
		for (int i = -1; i <= 1; i++)
		{
			for (int j = -1; j <= 1; j++)
			{
				if (i != 0 || j != 0)
				{
					int num3 = num2 + count;
					ApplyShadow(verts, base.effectColor, num2, num3, base.effectDistance.x * (float)i, base.effectDistance.y * (float)j);
					num2 = num3;
				}
			}
		}
	}
}
