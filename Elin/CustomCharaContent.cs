using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptOut)]
public class CustomCharaContent : CustomSourceContent
{
	public enum SpawnType
	{
		Common,
		Adventurer,
		Merchant,
		Unique
	}

	private static readonly Dictionary<string, SpawnType> _cachedSpawnTypes = new Dictionary<string, SpawnType>();

	public List<string> equipments = new List<string>();

	public Dictionary<string, int> mapInt = new Dictionary<string, int>();

	public Dictionary<string, string> mapStr = new Dictionary<string, string>();

	public SpawnType spawnType;

	public List<string> spawnZones = new List<string>();

	public List<string> stocks = new List<string>();

	public List<string> things = new List<string>();

	public override string SourceType => "SourceChara";

	public bool IsAdventurer => spawnType == SpawnType.Adventurer;

	public static CustomCharaContent CreateFromRow(SourceChara.Row r, ModPackage owner = null)
	{
		string trait = r.trait.TryGet(0, returnNull: true) ?? "Chara";
		if (owner == null)
		{
			owner = ModUtil.FindSourceRowPackage(r);
		}
		CustomCharaContent customCharaContent = new CustomCharaContent
		{
			ContentId = "Chara/" + r.id,
			SourceId = r.id,
			spawnType = GetSpawnTypeFromTrait(trait),
			Owner = owner
		};
		string[] tag = r.tag;
		for (int i = 0; i < tag.Length; i++)
		{
			var (text, text2, array) = CustomSourceContent.GetParams(tag[i]);
			switch (text)
			{
			case "addZone":
			case "addAdvZone":
				if (!text2.IsEmpty())
				{
					customCharaContent.spawnZones.Add(text2);
				}
				break;
			case "addEq":
			case "addAdvEq":
			case "addEquipment":
				if (!text2.IsEmpty())
				{
					customCharaContent.equipments.Add(text2);
				}
				break;
			case "addThing":
				if (!text2.IsEmpty())
				{
					customCharaContent.things.Add(text2);
				}
				break;
			case "addStock":
			{
				string text3 = text2.IsEmpty(r.id);
				if (!ModUtil.HasContent("MerchantStock/" + text3))
				{
					CustomMerchantStock customMerchantStock = CustomMerchantStock.CreateFromId(text3, owner);
					if (customMerchantStock == null)
					{
						break;
					}
					ModUtil.AddContent(customMerchantStock);
				}
				if (!customCharaContent.stocks.Contains(text3))
				{
					customCharaContent.stocks.Add(text3);
					customCharaContent.mapStr["merchant_override"] = string.Join('|', customCharaContent.stocks);
				}
				break;
			}
			case "addDrama":
				if (!text2.IsEmpty())
				{
					customCharaContent.mapStr["drama_override"] = text2;
				}
				break;
			case "addBio":
			case "addBiography":
			{
				string text4 = text2.IsEmpty(r.id);
				customCharaContent.mapStr["biography_override"] = text4;
				if (!ModUtil.HasContent("Biography/" + text4))
				{
					CustomBiographyContent customBiographyContent = CustomBiographyContent.CreateFromId(text4, owner);
					if (customBiographyContent != null)
					{
						ModUtil.AddContent(customBiographyContent);
					}
				}
				break;
			}
			case "addFlag":
			case "addInt":
				customCharaContent.mapInt[array[0]] = array.TryGet(1, returnNull: true)?.ToInt() ?? 1;
				break;
			case "addFlagValue":
			case "addStr":
				if (!array.TryGet(1, returnNull: true).IsEmpty())
				{
					customCharaContent.mapStr[array[0]] = array[1];
				}
				break;
			}
		}
		return customCharaContent;
	}

	public static SpawnType GetSpawnTypeFromTrait(string trait)
	{
		if (_cachedSpawnTypes.TryGetValue(trait, out var value))
		{
			return value;
		}
		Trait trait2 = ClassCache.Create<Trait>("Trait" + trait, "Elin");
		if (!(trait2 is TraitChara traitChara))
		{
			goto IL_005f;
		}
		SpawnType spawnType;
		if (traitChara.AdvType == TraitChara.Adv_Type.None)
		{
			if (!(trait2 is TraitMerchant))
			{
				if (!(trait2 is TraitUniqueChara))
				{
					goto IL_005f;
				}
				spawnType = SpawnType.Unique;
			}
			else
			{
				spawnType = SpawnType.Merchant;
			}
		}
		else
		{
			spawnType = SpawnType.Adventurer;
		}
		goto IL_0061;
		IL_0061:
		return _cachedSpawnTypes[trait] = spawnType;
		IL_005f:
		spawnType = SpawnType.Common;
		goto IL_0061;
	}

	public void OnCharaCreated(Chara chara)
	{
		if (chara.GetBool("custom_content") || chara.GetBool("cwl_tags_applied") || chara.id != base.SourceId)
		{
			return;
		}
		chara.SetBool("custom_content", enable: true);
		chara.SetStr("custom_content_id", base.ContentId);
		chara.SetStr("custom_content_package", base.Owner.id);
		string key2;
		foreach (KeyValuePair<string, string> item in mapStr)
		{
			item.Deconstruct(out var key, out key2);
			string id2 = key;
			string value = key2;
			chara.SetStr(id2, value);
		}
		foreach (KeyValuePair<string, int> item2 in mapInt)
		{
			item2.Deconstruct(out key2, out var value2);
			string id3 = key2;
			int value3 = value2;
			chara.SetInt(id3, value3);
		}
		if (things.Count > 0 || equipments.Count > 0)
		{
			chara.RemoveThings();
		}
		foreach (string[] item3 in equipments.Select((string t) => t.Split('#')))
		{
			if (!Enum.TryParse<Rarity>(item3.TryGet(1, returnNull: true), out var result))
			{
				result = Rarity.Random;
			}
			if (HasValidContentId(item3[0]))
			{
				chara.EQ_ID(item3[0], -1, result);
			}
		}
		foreach (string[] item4 in things.Select((string t) => t.Split('#')))
		{
			if (!int.TryParse(item4.TryGet(1, returnNull: true), out var result2))
			{
				result2 = 1;
			}
			if (HasValidContentId(item4[0]))
			{
				chara.AddThing(item4[0]).SetNum(result2);
			}
		}
		bool HasValidContentId(string id)
		{
			if (EClass.sources.cards.map.ContainsKey(id))
			{
				return true;
			}
			ModUtil.LogModError("source chara row '" + base.ContentId + "' has invalid addEq/addThing spec '" + id + "'", base.Owner);
			return false;
		}
	}

	public override void OnGameLoad(GameIOContext context)
	{
		if (spawnZones.Count == 0)
		{
			return;
		}
		Debug.Log($"#mod-content loading {this}");
		ILookup<string, Chara> lookup = EClass.game.cards.globalCharas.Values.ToLookup((Chara c) => c.id);
		List<Zone> list = new List<Zone>();
		foreach (string spawnZone in spawnZones)
		{
			Zone zone = ModUtil.FindZoneByFullName(spawnZone);
			if (zone != null)
			{
				list.Add(zone);
				continue;
			}
			ModUtil.LogModError("source chara row '" + base.SourceId + "' has invalid addZone spec '" + spawnZone + "'", base.Owner);
		}
		List<Chara> list2 = lookup[base.SourceId].ToList();
		int count = list2.Count;
		if (list.Count == 0 && count == 0)
		{
			ModUtil.LogModError("source chara row '" + base.SourceId + "' has invalid addZone spec", base.Owner);
			return;
		}
		int num = Math.Max(0, list.Count - count);
		int num2 = count;
		if (num2 <= 1)
		{
			if (num2 != 1)
			{
				goto IL_01d5;
			}
			if (IsAdventurer)
			{
				goto IL_017e;
			}
		}
		else if (IsAdventurer)
		{
			ModUtil.LogModError("adventurer '" + base.SourceId + "' has spawned more than once", base.Owner);
			return;
		}
		if (num == 0)
		{
			goto IL_017e;
		}
		goto IL_01d5;
		IL_017e:
		Debug.Log("#mod-content skipping existing character '" + base.SourceId + "', " + $"{count} at {string.Join('/', list2.Select((Chara c) => c.currentZone.ZoneFullName))}");
		return;
		IL_01d5:
		for (int i = 0; i < num; i++)
		{
			Zone z2 = list[i];
			Chara chara = SpawnToZone(z2);
			if (chara == null)
			{
				ModUtil.LogModError("can't spawn character '" + base.SourceId + "'", base.Owner);
				break;
			}
			if (IsAdventurer)
			{
				EClass.game.cards.listAdv.Add(chara);
				break;
			}
		}
		Chara SpawnToZone(Zone z)
		{
			Chara chara2 = CharaGen.Create(base.SourceId);
			if (chara2.id == "chicken")
			{
				return null;
			}
			chara2.SetHomeZone(z);
			chara2.MoveZone(z, ZoneTransition.EnterState.RandomVisit);
			Debug.Log("#mod-content spawned character '" + base.SourceId + "' to " + z.ZoneFullName);
			return chara2;
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"{base.ContentId}/{spawnType}/{base.Owner.id}");
		stringBuilder.AppendLine($" - zones({spawnZones.Count})/{string.Join(';', spawnZones)}");
		if (things.Count > 0)
		{
			stringBuilder.AppendLine($" - things({things.Count})/{string.Join(';', things)}");
		}
		if (equipments.Count > 0)
		{
			stringBuilder.AppendLine($" - equipments({equipments.Count})/{string.Join(';', equipments)}");
		}
		if (stocks.Count > 0)
		{
			stringBuilder.AppendLine(" - stocks/" + string.Join(';', stocks));
		}
		if (mapStr.Count > 0)
		{
			stringBuilder.AppendLine(" - str/" + string.Join(';', mapStr.Select((KeyValuePair<string, string> kv) => kv.Key + "=" + kv.Value)));
		}
		if (mapInt.Count > 0)
		{
			stringBuilder.AppendLine(" - int/" + string.Join(';', mapInt.Select((KeyValuePair<string, int> kv) => $"{kv.Key}={kv.Value}")));
		}
		return stringBuilder.ToString();
	}
}
