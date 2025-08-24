using System.Collections.Generic;
using Newtonsoft.Json;

public class ZoneEventSiege : ZoneEvent
{
	[JsonProperty]
	public List<int> uids = new List<int>();

	public List<Chara> members = new List<Chara>();

	public int lv = 10;

	public int max = 10;

	public override string id => "trial_siege";

	public override Playlist playlist => EClass.Sound.playlistBattle;

	public virtual Chara CreateChara()
	{
		return CharaGen.CreateFromFilter("c_wilds", lv);
	}

	public override void OnFirstTick()
	{
		EClass.player.stats.sieges++;
		Msg.Say("startSiege");
		EClass._zone.RefreshBGM();
		Point spawnPos = GetSpawnPos();
		for (int i = 0; i < 10; i++)
		{
			Chara chara = CreateChara();
			EClass._zone.AddCard(chara, EClass._map.GetRandomSurface(spawnPos.x, spawnPos.z, 6));
			chara.hostility = Hostility.Enemy;
			members.Add(chara);
			uids.Add(chara.uid);
			chara.PlayEffect("teleport");
			chara.PlaySound("spell_funnel");
		}
		Thing t = ThingGen.Create("torch");
		EClass._zone.AddCard(t, spawnPos);
		if (members.Count != 0)
		{
			return;
		}
		foreach (int uid in uids)
		{
			foreach (Chara chara2 in EClass._map.charas)
			{
				if (chara2.uid == uid)
				{
					members.Add(chara2);
				}
			}
		}
	}

	public virtual Point GetSpawnPos()
	{
		return EClass._map.GetRandomEdge();
	}

	public override void OnTickRound()
	{
		bool flag = true;
		foreach (Chara member in members)
		{
			if (member.IsAliveInCurrentZone)
			{
				if (member.ai is GoalIdle)
				{
					member.SetAI(new GoalSiege());
				}
				flag = false;
			}
		}
		if (flag)
		{
			Kill();
		}
	}

	public override void OnKill()
	{
		Msg.Say("endSiege");
		EClass._zone.RefreshBGM();
	}
}
