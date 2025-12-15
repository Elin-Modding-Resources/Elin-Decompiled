public class Tactics : EClass
{
	public SourceTactics.Row source;

	public SourceChara.Row sourceChara;

	public Chara owner;

	public static ConfigAutoCombat at => EClass.game.config.autoCombat;

	public int RandomFacotr
	{
		get
		{
			if (!owner.IsPC)
			{
				return 30;
			}
			return 10;
		}
	}

	public int DestDist
	{
		get
		{
			if (!owner.IsPC)
			{
				if (sourceChara.aiParam.Length < 1)
				{
					return source.dist;
				}
				return sourceChara.aiParam[0];
			}
			return 1;
		}
	}

	public int ChanceMove
	{
		get
		{
			if (!owner.IsPC)
			{
				if (sourceChara.aiParam.Length < 2)
				{
					return source.move;
				}
				return sourceChara.aiParam[1];
			}
			return source.movePC;
		}
	}

	public int ChanceSecondMove
	{
		get
		{
			if (!owner.IsPC)
			{
				if (sourceChara.aiParam.Length < 3)
				{
					return 100;
				}
				return sourceChara.aiParam[2];
			}
			return 100;
		}
	}

	public int P_Party => source.party;

	public int P_Melee => source.melee;

	public int P_Range => source.range;

	public int P_Heal => source.heal;

	public int P_Spell => source.spell;

	public int P_Buff => source.buff;

	public int P_Debuff => source.debuff;

	public int P_Summon => source.summon;

	public bool CastPartyBuff
	{
		get
		{
			if (!owner.IsPC)
			{
				return source.tag.Contains("pt");
			}
			return at.bCastParty;
		}
	}

	public int AbilityChance
	{
		get
		{
			if (!owner.IsPC)
			{
				if (!owner.IsPCParty)
				{
					return 50;
				}
				return 80;
			}
			return 100;
		}
	}

	public int RangedChance
	{
		get
		{
			if (!owner.IsPC)
			{
				if (!owner.IsPCParty)
				{
					return 50;
				}
				return 100;
			}
			return 100;
		}
	}

	public Tactics(Chara c)
	{
		owner = c;
		if (c.c_genes != null)
		{
			foreach (DNA item in c.c_genes.items)
			{
				if (item.type == DNA.Type.Brain && !item.id.IsEmpty())
				{
					SourceChara.Row row = EClass.sources.charas.map[item.id];
					string key = row.tactics.IsEmpty(EClass.sources.tactics.map.TryGetValue(row.id)?.id ?? EClass.sources.tactics.map.TryGetValue(row.job)?.id ?? "predator");
					source = EClass.sources.tactics.map[key];
					sourceChara = row;
					break;
				}
			}
		}
		if (source == null)
		{
			string key2 = (c.IsPC ? at.idType : c.source.tactics.IsEmpty(EClass.sources.tactics.map.TryGetValue(c.id)?.id ?? EClass.sources.tactics.map.TryGetValue(c.job.id)?.id ?? "predator"));
			source = EClass.sources.tactics.map[key2];
			sourceChara = c.source;
		}
	}
}
