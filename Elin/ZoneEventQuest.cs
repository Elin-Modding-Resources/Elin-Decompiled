using System.Collections.Generic;
using Newtonsoft.Json;

public class ZoneEventQuest : ZoneEvent
{
	[JsonProperty]
	public int uidQuest;

	[JsonProperty]
	public List<int> enemies = new List<int>();

	public Quest quest => EClass.game.quests.Get(uidQuest);

	public virtual bool WarnBoss => true;

	public virtual bool CountEnemy => false;

	public virtual int TimeLimit => -1;

	public void Spawn(int num = 1)
	{
		for (int i = 0; i < num; i++)
		{
			Point nearestPoint = EClass._map.bounds.GetRandomEdge().GetNearestPoint(allowBlock: false, allowChara: false);
			Chara chara = EClass._zone.SpawnMob(nearestPoint, SpawnSetting.DefenseEnemy(EClass._zone.DangerLv));
			Hostility hostility = (chara.c_originalHostility = Hostility.Enemy);
			chara.hostility = hostility;
			if (CountEnemy)
			{
				enemies.Add(chara.uid);
			}
		}
	}

	public void SpawnBoss(bool evolve = false)
	{
		Point nearestPoint = EClass._map.bounds.GetRandomEdge().GetNearestPoint(allowBlock: false, allowChara: false);
		Chara chara = (evolve ? EClass._zone.TryGenerateEvolved(force: true, nearestPoint) : EClass._zone.SpawnMob(nearestPoint, SpawnSetting.Boss(EClass._zone.DangerLv)));
		Hostility hostility = (chara.c_originalHostility = Hostility.Enemy);
		chara.hostility = hostility;
		if (CountEnemy)
		{
			enemies.Add(chara.uid);
		}
		if (WarnBoss)
		{
			Msg.Say("defense_boss", chara.Name);
			EClass.game.Pause();
		}
	}

	public void AggroEnemy(int chance = 100)
	{
		foreach (Chara chara in EClass._map.charas)
		{
			if (!chara.IsPCFactionOrMinion && !chara.IsInCombat && EClass.rnd(100) <= chance)
			{
				chara.SetEnemy(EClass.pc.party.members.RandomItem());
				chara.SetAIAggro();
			}
		}
	}

	public sealed override void OnTickRound()
	{
		_OnTickRound();
		if (TimeLimit != -1 && minElapsed >= TimeLimit)
		{
			EClass._zone.instance.status = OnReachTimeLimit();
			EClass.game.Pause(delegate
			{
				EClass.pc.MoveZone(EClass._zone.parent as Zone);
			});
		}
	}

	public virtual void _OnTickRound()
	{
	}

	public virtual ZoneInstance.Status OnReachTimeLimit()
	{
		return ZoneInstance.Status.Running;
	}
}
