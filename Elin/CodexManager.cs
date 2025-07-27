using System.Collections.Generic;
using Newtonsoft.Json;

public class CodexManager : EClass
{
	[JsonProperty]
	public Dictionary<string, CodexCreature> creatures = new Dictionary<string, CodexCreature>();

	public CodexCreature GetOrCreate(string id)
	{
		CodexCreature codexCreature = creatures.TryGetValue(id);
		if (codexCreature == null)
		{
			codexCreature = new CodexCreature
			{
				id = id
			};
			creatures[id] = codexCreature;
		}
		return codexCreature;
	}

	public void OnLoad()
	{
		foreach (KeyValuePair<string, CodexCreature> creature in creatures)
		{
			creature.Value.id = creature.Key;
		}
	}

	public void AddCard(string id, int num = 1)
	{
		GetOrCreate(id).numCard += num;
	}

	public bool Has(string id)
	{
		return creatures.ContainsKey(id);
	}

	public void MarkCardDrop(string id)
	{
		GetOrCreate(id).droppedCard = true;
	}

	public bool DroppedCard(string id)
	{
		if (creatures.ContainsKey(id))
		{
			return creatures[id].droppedCard;
		}
		return false;
	}

	public void AddKill(string id)
	{
		GetOrCreate(id).kills++;
	}

	public void AddWeakspot(string id)
	{
		GetOrCreate(id).weakspot++;
	}

	public void AddSpawn(string id)
	{
		GetOrCreate(id).spawns++;
	}

	public List<CardRow> ListKills()
	{
		List<CardRow> list = new List<CardRow>();
		foreach (KeyValuePair<string, CodexCreature> creature in creatures)
		{
			if (creature.Value.kills > 0 || creature.Value.droppedCard)
			{
				CardRow cardRow = EClass.sources.cards.map.TryGetValue(creature.Key);
				if (cardRow != null && !cardRow.HasTag(CTAG.noRandomProduct))
				{
					list.Add(cardRow);
				}
			}
		}
		return list;
	}
}
