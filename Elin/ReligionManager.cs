using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class ReligionManager : EClass
{
	public Dictionary<string, Religion> dictAll = new Dictionary<string, Religion>();

	public List<Religion> list = new List<Religion>();

	[JsonProperty]
	public ReligionEyth Eyth = new ReligionEyth();

	[JsonProperty]
	public ReligionWind Wind = new ReligionWind();

	[JsonProperty]
	public ReligionEarth Earth = new ReligionEarth();

	[JsonProperty]
	public ReligionHealing Healing = new ReligionHealing();

	[JsonProperty]
	public ReligionLuck Luck = new ReligionLuck();

	[JsonProperty]
	public ReligionMachine Machine = new ReligionMachine();

	[JsonProperty]
	public ReligionElement Element = new ReligionElement();

	[JsonProperty]
	public ReligionHarvest Harvest = new ReligionHarvest();

	[JsonProperty]
	public ReligionOblivion Oblivion = new ReligionOblivion();

	[JsonProperty]
	public ReligionHarmony Harmony = new ReligionHarmony();

	[JsonProperty]
	public ReligionTrickery Trickery = new ReligionTrickery();

	[JsonProperty]
	public ReligionMoonShadow MoonShadow = new ReligionMoonShadow();

	[JsonProperty]
	public ReligionStrife Strife = new ReligionStrife();

	public void SetOwner()
	{
		list = new List<Religion>
		{
			Eyth, Wind, Earth, Healing, Luck, Machine, Element, Harvest, Oblivion, Harmony,
			Trickery, MoonShadow, Strife
		};
		BaseModManager.PublishEvent("elin.religion_importing", list);
		foreach (Religion item in list)
		{
			dictAll[item.id] = item;
		}
	}

	public void OnCreateGame()
	{
		SetOwner();
		foreach (Religion item in list)
		{
			item.Init();
		}
	}

	public void OnLoad()
	{
		SetOwner();
		foreach (Religion value in dictAll.Values)
		{
			value.OnLoad();
		}
	}

	public Religion Find(string id)
	{
		return dictAll.TryGetValue(id);
	}

	public Religion GetRandomReligion(bool onlyJoinable = true, bool includeMinor = false)
	{
		return list.Where((Religion a) => (!onlyJoinable || a.CanJoin) && (includeMinor || !a.IsMinorGod)).RandomItem();
	}

	public bool ジュアさまの薄い本をください()
	{
		foreach (Chara item in EClass._map.charas.Concat(EClass.game.cards.globalCharas.Values))
		{
			if (item.faith != Healing)
			{
				Thing thing = ThingGen.Createジュアさまの薄い本();
				item.AddCard(thing);
				if (item.ExistsOnMap)
				{
					item.TryUse(thing);
				}
			}
		}
		Debug.Log("hai");
		return true;
	}

	public Religion GetArtifactDeity(string id)
	{
		return list.LastOrDefault((Religion a) => a.IsValidArtifact(id));
	}

	public Thing Reforge(string id, Point pos = null, bool first = true)
	{
		if (pos == null)
		{
			pos = EClass.pc.pos.Copy();
		}
		pos.Set(pos.GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false, ignoreCenter: true) ?? pos);
		Thing thing = ThingGen.Create(id);
		GetArtifactDeity(id)?.OnReforge(thing);
		EClass._zone.AddCard(thing, pos);
		pos.PlayEffect("aura_heaven");
		if (first)
		{
			pos.PlaySound("godbless");
		}
		return thing;
	}
}
