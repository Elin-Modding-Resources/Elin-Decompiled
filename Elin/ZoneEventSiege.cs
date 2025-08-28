using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class ZoneEventSiege : ZoneEvent
{
	[JsonProperty]
	public List<int> uids = new List<int>();

	[JsonProperty]
	public int lv = 10;

	[JsonProperty]
	public int idx;

	[JsonProperty]
	public int max = 10;

	public List<Chara> members = new List<Chara>();

	public override string id => "trial_siege";

	public override float roundInterval => Mathf.Max(0.1f, 1.5f - 0.01f * (float)lv);

	public override Playlist playlist => EClass.Sound.playlistBattle;

	public virtual Chara CreateChara(Point p)
	{
		bool flag = idx == max - 1;
		SpawnSetting spawnSetting = ((lv >= 50 && idx == max - 2) ? SpawnSetting.Evolved(lv) : (flag ? SpawnSetting.Boss(lv) : SpawnSetting.DefenseEnemy(lv)));
		spawnSetting.dangerLv = lv + 1;
		return EClass._zone.SpawnMob(p, spawnSetting);
	}

	public override void OnInit()
	{
		EClass.player.stats.sieges++;
		Msg.Say("startSiege");
		EClass._zone.RefreshBGM();
	}

	public override void OnVisit()
	{
		EClass._zone.RefreshBGM();
		members.Clear();
		foreach (int uid in uids)
		{
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara.uid == uid)
				{
					members.Add(chara);
				}
			}
		}
	}

	public void SpawnMob()
	{
		Point spawnPos = GetSpawnPos();
		Chara chara = CreateChara(spawnPos);
		chara.hostility = Hostility.Enemy;
		members.Add(chara);
		uids.Add(chara.uid);
		chara.PlayEffect("teleport");
		chara.PlaySound("spell_funnel");
		idx++;
	}

	public virtual Point GetSpawnPos()
	{
		return EClass._map.GetRandomEdge();
	}

	public override void OnTickRound()
	{
		if (idx < max)
		{
			SpawnMob();
		}
		if (ShouldEnd())
		{
			Kill();
		}
	}

	public override void OnCharaDie(Chara c)
	{
		if (ShouldEnd())
		{
			Kill();
		}
	}

	public bool ShouldEnd()
	{
		bool result = idx >= max;
		foreach (Chara member in members)
		{
			if (!member.IsPCFactionOrMinion && member.IsAliveInCurrentZone)
			{
				if (member.ai is GoalIdle)
				{
					member.SetAI(new GoalSiege());
				}
				result = false;
			}
		}
		return result;
	}

	public override void OnKill()
	{
		Msg.Say("endSiege");
		SE.Play("kill_boss");
		EClass._zone.RefreshBGM();
		EClass._zone.AddCard(ThingGen.CreateTreasure("chest_boss", lv, TreasureType.SurvivalRaid), GetSpawnPos().GetNearestPoint(allowBlock: false, allowChara: false, allowInstalled: false, ignoreCenter: true) ?? EClass.pc.pos).Install();
	}
}
