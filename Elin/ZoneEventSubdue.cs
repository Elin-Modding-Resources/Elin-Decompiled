using Newtonsoft.Json;
using UnityEngine;

public class ZoneEventSubdue : ZoneEventQuest
{
	[JsonProperty]
	public int max;

	public override bool CountEnemy => true;

	public override bool WarnBoss => true;

	public override void OnVisit()
	{
		if (!EClass.game.isLoading)
		{
			EClass._zone._dangerLv = Mathf.Max(base.quest.DangerLv - 2, 1);
			Spawn(4 + base.quest.difficulty * 2 + EClass.rnd(5));
			AggroEnemy(15);
			EClass._zone.SetBGM(102);
			max = enemies.Count;
		}
	}

	public override void OnCharaDie(Chara c)
	{
		CheckClear();
	}

	public override void _OnTickRound()
	{
		AggroEnemy();
		CheckClear();
	}

	public void CheckClear()
	{
		if (EClass._zone.instance.status == ZoneInstance.Status.Success || EClass._zone.instance.status == ZoneInstance.Status.Fail)
		{
			return;
		}
		enemies.ForeachReverse(delegate(int id)
		{
			Chara chara = EClass._map.FindChara(id);
			if (chara == null || !chara.IsAliveInCurrentZone || !EClass.pc.IsHostile(chara))
			{
				enemies.Remove(id);
			}
		});
		if (enemies.Count == 0)
		{
			EClass._zone.instance.status = ZoneInstance.Status.Success;
			Msg.Say("subdue_complete");
			EClass._zone.SetBGM();
			SE.Play("Jingle/fanfare");
		}
	}
}
