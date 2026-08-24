using System.Collections.Generic;
using UnityEngine;

public class Zone_WindRest : Zone
{
	public override bool IsSnowCovered => false;

	public override void OnGenerateMap()
	{
		base.OnGenerateMap();
		List<Dictionary<string, string>> list = new ExcelData("Data/Raw/dagger_graves", 1).BuildList();
		list.RemoveAt(0);
		int num = 0;
		foreach (Thing thing in EClass._map.things)
		{
			if (thing.id != "grave_dagger1" && thing.id != "grave_dagger2")
			{
				continue;
			}
			thing.isOn = false;
			if (num < list.Count)
			{
				Dictionary<string, string> dictionary = list[num];
				int num2 = dictionary["id"].ToInt();
				num++;
				if (num2 != 0)
				{
					thing.c_note = dictionary["First_" + (Lang.isJP ? "JP" : "EN")];
					thing.c_context = dictionary["Full"];
					thing.c_idBacker = num2;
					thing.isModified = true;
				}
			}
		}
		Debug.Log(num + "/" + list.Count);
	}

	public override void OnAfterSimulate()
	{
		base.OnAfterSimulate();
		if (base.HourSinceLastActive >= 168)
		{
			TryRevive("eureka", 60, 61);
			TryRevive("billy", 35, 64);
		}
		List<Thing> list = EClass._map.ListThing<TraitDaggerGrave>();
		foreach (Thing item in list)
		{
			if (item.pos.x != 52 || item.pos.z != 46)
			{
				continue;
			}
			{
				foreach (Thing item2 in list)
				{
					if (item2.c_idBacker == 1239)
					{
						item.MoveImmediate(item2.pos);
						item2.MoveImmediate(new Point(52, 46));
						break;
					}
				}
				break;
			}
		}
		static void TryRevive(string id, int x, int y)
		{
			if (EClass.game.cards.globalCharas.Find(id) == null)
			{
				bool flag = false;
				foreach (Chara chara in EClass._map.charas)
				{
					if (chara.id == id)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					EClass._zone.AddCard(CharaGen.Create(id), x, y);
				}
			}
		}
	}
}
