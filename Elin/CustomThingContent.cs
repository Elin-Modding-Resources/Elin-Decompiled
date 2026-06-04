using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

[JsonObject(MemberSerialization.OptOut)]
public class CustomThingContent : CustomSourceContent
{
	public enum SpawnType
	{
		Item,
		Block,
		Cassette,
		Currency,
		Category,
		Filter,
		Tag,
		Letter,
		Map,
		Perfume,
		Plan,
		Potion,
		Recipe,
		RedBook,
		Rod,
		Rune,
		RuneFree,
		Scroll,
		Skill,
		Spell,
		Usuihon
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public BlessedState blessedState;

	public string id = "";

	[JsonConverter(typeof(StringEnumConverter))]
	public IDTLevel identifyLevel;

	public int lv = -1;

	public Dictionary<string, int> mapInt = new Dictionary<string, int>();

	public Dictionary<string, string> mapStr = new Dictionary<string, string>();

	public string material = "";

	public bool noCopy;

	public bool noRandomSocket;

	public int num = 1;

	[JsonConverter(typeof(StringEnumConverter))]
	public Rarity rarity;

	[JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
	public bool restock = true;

	public List<string> sockets = new List<string>();

	[JsonConverter(typeof(StringEnumConverter))]
	public SpawnType type;

	public string priceCalc = "";

	public override string SourceType => "SourceThing";

	public static CustomThingContent CreateFromRow(SourceThing.Row r, ModPackage mod = null)
	{
		CustomThingContent customThingContent = new CustomThingContent
		{
			ContentId = "Thing/" + r.id,
			SourceId = r.id,
			Owner = mod,
			id = r.id
		};
		string[] tag = r.tag;
		for (int i = 0; i < tag.Length; i++)
		{
			var (text, item, array) = CustomSourceContent.GetParams(tag[i]);
			switch (text)
			{
			case "forceRarity":
				customThingContent.rarity = r.quality.ToEnum<Rarity>();
				break;
			case "noCopy":
				if (r.elements.Contains(759))
				{
					customThingContent.noCopy = true;
				}
				break;
			case "noRandomSocket":
				customThingContent.noRandomSocket = true;
				break;
			case "noRestock":
				customThingContent.restock = false;
				break;
			case "addSocket":
				customThingContent.sockets.Add(item);
				break;
			case "addFlag":
			case "addInt":
				customThingContent.mapInt[array[0]] = array.TryGet(1, returnNull: true)?.ToInt() ?? 1;
				break;
			case "addFlagValue":
			case "addStr":
				if (!array.TryGet(1, returnNull: true).IsEmpty())
				{
					customThingContent.mapStr[array[0]] = array[1];
				}
				break;
			}
		}
		return customThingContent;
	}

	public void OnThingCreated(Thing thing)
	{
		if (thing.GetBool("custom_content") || thing.id != base.SourceId)
		{
			return;
		}
		thing.SetBool("custom_content", enable: true);
		thing.SetStr("custom_content_id", base.ContentId);
		thing.SetStr("custom_content_package", base.Owner.id);
		string key2;
		foreach (KeyValuePair<string, string> item in mapStr)
		{
			item.Deconstruct(out var key, out key2);
			string text = key;
			string value = key2;
			thing.SetStr(text, value);
		}
		foreach (KeyValuePair<string, int> item2 in mapInt)
		{
			item2.Deconstruct(out key2, out var value2);
			string text2 = key2;
			int value3 = value2;
			thing.SetInt(text2, value3);
		}
		thing.ChangeRarity(rarity);
		if (noCopy)
		{
			EClass.core.actionsNextFrame.Add(delegate
			{
				thing.elements?.SetBase(759, 10);
			});
		}
		if (sockets.Count <= 0)
		{
			return;
		}
		if (noRandomSocket)
		{
			thing.sockets.Clear();
		}
		int num = 0;
		foreach (string socket in sockets)
		{
			thing.AddSocket();
			if (socket.IsEmpty())
			{
				num++;
			}
			else
			{
				ApplyRangedSocket(socket);
			}
		}
		thing.sockets.RemoveAll((int s) => s == 0);
		for (int i = 0; i < num; i++)
		{
			thing.sockets.Add(0);
		}
		void ApplyRangedSocket(string socket)
		{
			int num2 = 3 + Mathf.Min(thing.genLv / 10, 15);
			if (!EClass.sources.elements.alias.TryGetValue(socket, out var value4))
			{
				ModUtil.LogModError("source thing row '" + base.ContentId + "' has invalid addSocket spec '" + socket + "'", base.Owner);
			}
			else
			{
				float num3 = Mathf.Sqrt((float)(thing.genLv * value4.encFactor) / 100f);
				float num4 = (float)num2 + num3;
				int num5 = (value4.mtp + EClass.rnd(value4.mtp + (int)num4)) / value4.mtp;
				if (value4.encFactor == 0 && num5 > 25)
				{
					num5 = 25;
				}
				thing.ApplySocket(value4.id, num5);
			}
		}
	}

	public Thing Create(int createLv = -1)
	{
		if (lv != -1)
		{
			createLv = lv;
		}
		CardBlueprint.SetRarity(rarity);
		int.TryParse(id, out var result);
		if (EClass.sources.elements.fuzzyAlias.TryGetValue(id, out var value))
		{
			result = EClass.sources.elements.alias[value].id;
		}
		Thing thing = null;
		int idMat = EClass.sources.materials.alias.TryGetValue(material)?.id ?? (-1);
		switch (type)
		{
		case SpawnType.Item:
			thing = ThingGen.Create(id, idMat, createLv).SetNum(num);
			break;
		case SpawnType.Block:
		{
			SourceBlock.Row row = EClass.sources.blocks.alias.TryGetValue(id);
			if (row != null)
			{
				thing = ThingGen.CreateBlock(row.id, idMat).SetNum(num);
			}
			break;
		}
		case SpawnType.Cassette:
			if (!EClass.core.refs.dictBGM.ContainsKey(result))
			{
				result = EClass.core.refs.dictBGM.RandomItem().id;
			}
			thing = ThingGen.CreateCassette(result);
			break;
		case SpawnType.Currency:
			thing = ThingGen.CreateCurrency(num, id);
			break;
		case SpawnType.Category:
			thing = ThingGen.CreateFromCategory(id, createLv).SetNum(num);
			break;
		case SpawnType.Filter:
			thing = ThingGen.CreateFromFilter(id, createLv).SetNum(num);
			break;
		case SpawnType.Tag:
			thing = ThingGen.CreateFromTag(id, createLv).SetNum(num);
			break;
		case SpawnType.Letter:
			thing = ThingGen.CreateLetter(id);
			break;
		case SpawnType.Map:
			thing = ThingGen.CreateMap(id, createLv);
			break;
		case SpawnType.Perfume:
			thing = ThingGen.CreatePerfume(result, createLv).SetNum(num);
			break;
		case SpawnType.Plan:
			thing = ThingGen.CreatePlan(result);
			break;
		case SpawnType.Potion:
			thing = ThingGen.CreatePotion(result, num);
			break;
		case SpawnType.Recipe:
			thing = ThingGen.CreateRecipe(id);
			break;
		case SpawnType.RedBook:
			thing = ThingGen.CreateRedBook(id);
			break;
		case SpawnType.Rod:
			thing = ThingGen.CreateRod(result, num);
			break;
		case SpawnType.Rune:
			thing = ThingGen.CreateRune(result, num);
			break;
		case SpawnType.RuneFree:
			thing = ThingGen.CreateRune(result, num, free: true);
			break;
		case SpawnType.Scroll:
			thing = ThingGen.CreateScroll(result, num);
			break;
		case SpawnType.Skill:
			thing = ThingGen.CreateSkillbook(result, num);
			break;
		case SpawnType.Spell:
			thing = ThingGen.CreateSpellbook(result, 1, num);
			break;
		case SpawnType.Usuihon:
			thing = ThingGen.Create("1084");
			thing.c_idRefName = EClass.game.religions.dictAll.TryGetValue(id)?.id;
			break;
		default:
			thing = ThingGen.Create(id);
			break;
		}
		if (thing == null)
		{
			return null;
		}
		thing.c_IDTState = (int)identifyLevel;
		thing.SetBlessedState(blessedState);
		if (!restock)
		{
			thing.SetBool(101, enable: true);
		}
		if (!priceCalc.IsEmpty())
		{
			thing.SetStr("price_calc_override", priceCalc);
		}
		return thing;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"{base.ContentId}/{type}/{base.Owner.id}");
		stringBuilder.AppendLine($" - num({num})/lv({lv})/restock({restock})/noCopy({noCopy})/{material}/{rarity}/{blessedState}/{identifyLevel}");
		if (sockets.Count > 0)
		{
			stringBuilder.AppendLine($" - sockets({sockets.Count})/noRandomSocket({noRandomSocket})/{string.Join(';', sockets)}");
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
