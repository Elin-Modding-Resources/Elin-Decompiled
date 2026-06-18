using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptOut)]
public class CustomReligionContent : CustomSourceContent
{
	public static Dictionary<string, ReligionCustom> managed = new Dictionary<string, ReligionCustom>();

	public List<string> artifacts = new List<string>();

	[DefaultValue(true)]
	public bool canJoin = true;

	public bool isMinorGod;

	public bool noPunish;

	public bool noPunishTakeover;

	public List<string> elements = new List<string>();

	public List<string> godAbilities = new List<string>();

	public Dictionary<string, int> offeringMtp = new Dictionary<string, int>();

	public Dictionary<string, string> offeringValue = new Dictionary<string, string>();

	public override string SourceType => "SourceReligion";

	public string ChunkName => "custom_religion_" + base.SourceId;

	public static CustomReligionContent CreateFromRow(SourceReligion.Row r, ModPackage owner = null)
	{
		string text = r.id.Split('#')[0];
		if (owner == null)
		{
			owner = ModUtil.FindSourceRowPackage(r);
		}
		CustomReligionContent obj = new CustomReligionContent
		{
			ContentId = "Religion/" + text,
			SourceId = text,
			Owner = owner,
			canJoin = (!r.id.Contains("#noJoin") && !r.id.Contains("#cannot")),
			isMinorGod = r.id.Contains("#minor")
		};
		EClass.sources.religions.map[text] = r;
		if (!Portrait.modPortraits.dict.ContainsKey("UN_" + r.id) && SpriteReplacer.dictModItems.TryGetValue(r.id, out var value))
		{
			Portrait.modPortraits.Add("UN_" + r.id, new FileInfo(value + ".png"));
		}
		obj.RegisterCustomReligion();
		return obj;
	}

	public void RegisterCustomReligion(ReligionCustom religionCustom = null)
	{
		Dictionary<string, ReligionCustom> dictionary = managed;
		string sourceId = base.SourceId;
		ReligionCustom obj = religionCustom ?? new ReligionCustom();
		ReligionCustom religionCustom2 = obj;
		dictionary[sourceId] = obj;
		religionCustom2.content = this;
	}

	public static void Init()
	{
		(FileInfo, EMod)[] filesEx = PackageIterator.GetFilesEx("Data/religion_data.json");
		for (int i = 0; i < filesEx.Length; i++)
		{
			var (fileInfo, package) = filesEx[i];
			try
			{
				foreach (var (key, customReligionContent2) in IO.LoadFile<Dictionary<string, CustomReligionContent>>(fileInfo.FullName))
				{
					if (managed.TryGetValue(key, out var value))
					{
						value.content.canJoin = customReligionContent2.canJoin;
						value.content.isMinorGod = customReligionContent2.isMinorGod;
						value.content.artifacts = customReligionContent2.artifacts;
						value.content.offeringMtp = customReligionContent2.offeringMtp;
						value.content.offeringValue = customReligionContent2.offeringValue;
						value.content.godAbilities = customReligionContent2.godAbilities;
					}
				}
			}
			catch (Exception ex)
			{
				ModUtil.LogModError("exception while loading religion data '" + fileInfo.ShortPath() + "'\n" + ex.Message, package);
				Debug.LogException(ex);
			}
		}
		LoadDeprecatedCwlSpec();
		static void LoadDeprecatedCwlSpec()
		{
			foreach (SourceThing.Row row2 in EClass.sources.things.rows)
			{
				if (row2.HasTag(CTAG.godArtifact))
				{
					managed.Values.FirstOrDefault((ReligionCustom r) => row2.tag.Contains(r.id))?.content.artifacts.Add(row2.id);
				}
			}
			foreach (SourceElement.Row row in EClass.sources.elements.rows)
			{
				if (row.tag.Contains("godAbility"))
				{
					managed.Values.FirstOrDefault((ReligionCustom r) => row.tag.Contains(r.id))?.content.godAbilities.Add(row.alias);
				}
			}
			(FileInfo, EMod)[] filesEx2 = PackageIterator.GetFilesEx("Data/religion_elements.json");
			string key2;
			for (int j = 0; j < filesEx2.Length; j++)
			{
				var (fileInfo2, package2) = filesEx2[j];
				try
				{
					foreach (KeyValuePair<string, List<string>> item in IO.LoadFile<Dictionary<string, List<string>>>(fileInfo2.FullName))
					{
						item.Deconstruct(out key2, out var value2);
						string key3 = key2;
						List<string> collection = value2;
						if (managed.TryGetValue(key3, out var value3))
						{
							value3.content.elements.AddRange(collection);
						}
					}
				}
				catch (Exception ex2)
				{
					ModUtil.LogModError("exception while loading '" + fileInfo2.ShortPath() + "'\n" + ex2.Message, package2);
					Debug.LogException(ex2);
				}
			}
			filesEx2 = PackageIterator.GetFilesEx("Data/religion_offerings.json");
			for (int j = 0; j < filesEx2.Length; j++)
			{
				var (fileInfo3, package3) = filesEx2[j];
				try
				{
					foreach (KeyValuePair<string, Dictionary<string, int>> item2 in IO.LoadFile<Dictionary<string, Dictionary<string, int>>>(fileInfo3.FullName))
					{
						item2.Deconstruct(out key2, out var value4);
						string key4 = key2;
						Dictionary<string, int> other = value4;
						if (managed.TryGetValue(key4, out var value5))
						{
							value5.content.offeringMtp.Merge(other);
						}
					}
				}
				catch (Exception ex3)
				{
					ModUtil.LogModError("exception while loading '" + fileInfo3.ShortPath() + "'\n" + ex3.Message, package3);
					Debug.LogException(ex3);
				}
			}
		}
	}

	public static List<ReligionCustom> GetCustomReligions()
	{
		List<ReligionCustom> list = new List<ReligionCustom>();
		foreach (ReligionCustom value in managed.Values)
		{
			if (value.content != null && EClass.sources.religions.map.ContainsKey(value.content.SourceId))
			{
				value.relation = value.source.relation;
				list.Add(value);
				Debug.Log($"#mod-content {value.content}");
			}
		}
		return list;
	}

	internal static void LoadReligionData(GameIOContext context)
	{
		if (!context.Load<Dictionary<string, ReligionCustom>>("custom_religion_data", out var data))
		{
			return;
		}
		foreach (var (key, religionCustom2) in data)
		{
			if (EClass.game.religions.dictAll.TryGetValue(key, out var value))
			{
				Debug.Log("#mod-content loading Religion/" + value.id);
				value.mood = religionCustom2.mood;
				value.relation = religionCustom2.relation;
				value.giftRank = religionCustom2.giftRank;
			}
		}
	}

	internal static void SaveReligionData(GameIOContext context)
	{
		if (!context.Load<Dictionary<string, ReligionCustom>>("custom_religion_data", out var data))
		{
			data = new Dictionary<string, ReligionCustom>();
		}
		foreach (ReligionCustom item in EClass.game.religions.list.OfType<ReligionCustom>())
		{
			data[item.id] = item;
		}
		if (data.Count > 0)
		{
			context.SaveUncompressed("custom_religion_data", data);
		}
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine($"{base.ContentId}/canJoin={canJoin}/minorGod={isMinorGod}");
		if (artifacts.Count > 0)
		{
			stringBuilder.AppendLine($" - artifacts({artifacts.Count})/{string.Join(';', artifacts)}");
		}
		if (elements.Count > 0)
		{
			stringBuilder.AppendLine($" - elements({elements.Count})/{string.Join(';', elements)}");
		}
		if (offeringMtp.Count > 0)
		{
			stringBuilder.AppendLine($" - offeringMtp({offeringMtp.Count})/{string.Join(';', offeringMtp.Select((KeyValuePair<string, int> kv) => $"{kv.Key}={kv.Value}"))}");
		}
		if (offeringValue.Count > 0)
		{
			stringBuilder.AppendLine(string.Format(" - offeringValue({0})/{1}", offeringValue.Count, string.Join(';', offeringValue.Select((KeyValuePair<string, string> kv) => kv.Key + "=" + kv.Value))));
		}
		if (godAbilities.Count > 0)
		{
			stringBuilder.AppendLine($" - godAbilities({godAbilities.Count})/{string.Join(';', godAbilities)}");
		}
		return stringBuilder.ToString();
	}
}
