using Newtonsoft.Json;
using UnityEngine;

public class ZoneEventDefenseGame : ZoneEventQuest
{
	[JsonProperty]
	public int wave;

	[JsonProperty]
	public int turns;

	[JsonProperty]
	public int hornTimer;

	[JsonProperty]
	public int kills;

	[JsonProperty]
	public int allyCall;

	[JsonProperty]
	public int allyTimer;

	[JsonProperty]
	public int bonus;

	[JsonProperty]
	public bool retreated;

	public ZoneInstanceRandomQuest instance => EClass._zone.instance as ZoneInstanceRandomQuest;

	public bool CanRetreat
	{
		get
		{
			if (rounds >= 100)
			{
				return hornTimer <= 0;
			}
			return false;
		}
	}

	public bool CanCallAlly
	{
		get
		{
			if (allyCall == 0)
			{
				return rounds >= 50;
			}
			return false;
		}
	}

	public override string TextWidgetDate => "defenseWave".lang(wave.ToString() ?? "", kills.ToString() ?? "", EClass._zone.DangerLv.ToString() ?? "") + ((instance != null && retreated) ? "defenseRetreating" : (CanRetreat ? "defenseRetreat" : "")).lang() + (CanCallAlly ? "defenseAlly" : "").lang();

	public virtual Chara CreateChara()
	{
		return CharaGen.CreateFromFilter("c_wilds");
	}

	public override void OnVisit()
	{
		if (EClass.game.isLoading)
		{
			QuestDefenseGame.lastWave = wave;
			QuestDefenseGame.bonus = bonus;
			return;
		}
		int num = (base.quest.FameContent ? (EClass.pc.FameLv / 100 * 50) : 0);
		EClass._zone._dangerLv = 5 + num;
		bonus += num / 10;
		Point nearestPoint = EClass._map.GetCenterPos().GetNearestPoint(allowBlock: false, allowChara: false);
		EClass._zone.AddCard(ThingGen.Create("stone_defense"), nearestPoint).Install().isNPCProperty = true;
		EClass._zone.AddCard(ThingGen.Create("core_defense"), nearestPoint).Install().isNPCProperty = false;
		EClass._zone.SetBGM(107);
		Msg.Say("defense_start");
		NextWave();
	}

	public void NextWave(int add = 0)
	{
		wave++;
		turns = 0;
		EClass._zone._dangerLv += ((EClass._zone.DangerLv >= 100) ? (EClass._zone.DangerLv / 100 * 5 + 5) : 5);
		SE.Play("warhorn");
		Msg.Say("defense_wave", wave.ToString() ?? "", EClass._zone.DangerLv.ToString() ?? "");
		Spawn(2 + base.quest.difficulty + add);
		AggroEnemy();
	}

	public override void _OnTickRound()
	{
		QuestDefenseGame.lastWave = wave;
		QuestDefenseGame.bonus = bonus;
		Debug.Log("wave:" + wave + " turns:" + turns + " rounds:" + rounds);
		turns++;
		if (hornTimer > 0)
		{
			hornTimer--;
		}
		if (turns <= 3 + base.quest.difficulty)
		{
			Spawn();
		}
		if (turns == 10 && wave % 5 == 0)
		{
			Rand.SetSeed(wave + base.quest.uid);
			SpawnBoss(((wave >= 10) ? (wave * 2) : 0) > EClass.rnd(100));
			Rand.SetSeed();
		}
		if (turns == 20)
		{
			NextWave();
		}
		AggroEnemy();
		if (allyTimer > 0)
		{
			allyTimer--;
			if (allyTimer == 0)
			{
				allyCall--;
			}
		}
	}

	public void Horn_Next()
	{
		Msg.Say("defense_start");
		int add = 0;
		if (turns <= 3)
		{
			Msg.Say("defense_bonus", bonus.ToString() ?? "");
			bonus++;
			add = 1 + base.quest.difficulty;
		}
		NextWave(add);
		hornTimer += 10;
		if (hornTimer > 100)
		{
			hornTimer = 100;
		}
	}

	public void Horn_Retreat()
	{
		SE.Play("warhorn");
		Msg.Say("defense_retreat");
		retreated = true;
		instance.status = ZoneInstance.Status.Success;
		ActEffect.Proc(EffectId.Evac, EClass.pc);
	}

	public void Horn_Ally()
	{
		SE.Play("warhorn");
		ActEffect.ProcAt(EffectId.Summon, 100, BlessedState.Normal, EClass.pc, EClass.pc, EClass.pc.pos, isNeg: false, new ActRef
		{
			n1 = "special_force"
		});
		allyCall++;
		allyTimer += 100;
	}

	public override void OnCharaDie(Chara c)
	{
		if (!c.IsPCParty && !c.IsPCPartyMinion)
		{
			kills++;
		}
	}
}
