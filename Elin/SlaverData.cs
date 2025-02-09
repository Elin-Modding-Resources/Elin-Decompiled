using System.Collections.Generic;
using Newtonsoft.Json;

public class SlaverData : EClass
{
	[JsonProperty]
	public List<Chara> list = new List<Chara>();

	[JsonProperty]
	public int dateRefresh;

	public void TryRefresh(Chara owner)
	{
		if (!EClass.world.date.IsExpired(dateRefresh))
		{
			return;
		}
		list.Clear();
		int num = 4 + EClass.rnd(4);
		for (int i = 0; i < num; i++)
		{
			Chara item = ((owner.trait.SlaverType != SlaverType.Animal) ? CharaGen.Create(SpawnListChara.Get("slaver_human", (SourceChara.Row s) => EClass.sources.races.map[s.race].tag.Contains("human") && s.quality == 0).Select().id) : ((!(EClass._zone is Zone_Yowyn) || EClass.rnd(3) == 0) ? CharaGen.Create(SpawnListChara.Get("slaver_animal", (SourceChara.Row s) => EClass.sources.races.map[s.race].tag.Contains("animal") && s.quality == 0).Select().id) : CharaGen.Create(SpawnListChara.Get("slaver_horse", (SourceChara.Row s) => s.id == "horse_yowyn").Select().id)));
			list.Add(item);
		}
		dateRefresh = EClass.world.date.GetRaw(168);
		owner.isRestocking = true;
	}
}
