using System.Collections.Generic;
using UnityEngine;

public class ZonePreEnterBout : ZonePreEnterEvent
{
	public Chara target;

	public override void Execute()
	{
		Debug.Log(EClass._zone);
		Debug.Log(EClass._map);
		Debug.Log(EClass._map.GetCenterPos());
		Point point = EClass._map.GetCenterPos().GetNearestPoint(allowBlock: false, allowChara: false) ?? EClass._map.GetCenterPos();
		Debug.Log(point?.ToString() + "/" + target);
		if (target == null)
		{
			Debug.LogError("exception: Target is Null");
			return;
		}
		target.MoveZone(EClass._zone, new ZoneTransition
		{
			state = ZoneTransition.EnterState.Exact,
			x = point.x,
			z = point.z
		});
		EClass._zone.SetBGM(102);
		Debug.Log(target.pos);
		List<Chara> list = new List<Chara> { target };
		for (int i = 0; i < EClass.pc.party.members.Count - 1; i++)
		{
			Chara chara = CharaGen.CreateFromFilter("c_neutral", target.LV + 10);
			if (!(chara.trait is TraitMerchantTravel))
			{
				chara.ChangeRarity(Rarity.Superior);
				if (chara.LV < target.LV)
				{
					chara.SetLv(target.LV);
				}
				EClass._zone.AddCard(chara, target.pos.GetNearestPoint(allowBlock: false, allowChara: false) ?? target.pos);
				list.Add(chara);
			}
		}
		Debug.Log(list.Count);
		foreach (Chara item in list)
		{
			Hostility c_originalHostility = (item.hostility = Hostility.Enemy);
			item.c_originalHostility = c_originalHostility;
			item.SetEnemy(EClass.pc);
			item.HealAll();
		}
	}
}
