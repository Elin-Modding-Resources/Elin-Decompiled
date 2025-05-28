using Newtonsoft.Json;
using UnityEngine;

public class QuestDefenseGame : QuestInstance
{
	public static int lastWave;

	public static int bonus;

	[JsonProperty]
	public Thing thing;

	[JsonProperty]
	public bool useFame;

	public override string IdZone => "instance_arena";

	public override string RefDrama1 => thing.NameSimple;

	public override string RewardSuffix => "Defense";

	public override bool FameContent => useFame;

	public override int FameOnComplete => (lastWave * 8 + difficulty * 10) * (100 + bonus * 5) / 100;

	public override ZoneEventQuest CreateEvent()
	{
		return new ZoneEventDefenseGame();
	}

	public override ZoneInstanceRandomQuest CreateInstance()
	{
		return new ZoneInstanceDefense();
	}

	public override void OnInit()
	{
		thing = ThingGen.CreateFromFilter("thing", 30);
		useFame = EClass.rnd(3) != 0;
	}

	public override void OnBeforeComplete()
	{
		Debug.Log("QuestDefenseGame: " + lastWave + "/" + bonus);
		bonusMoney += EClass.rndHalf(lastWave * 400 * (100 + bonus * 5) / 100);
	}

	public override string GetTextProgress()
	{
		return "progressDefenseGame".lang(lastWave.ToString() ?? "");
	}
}
