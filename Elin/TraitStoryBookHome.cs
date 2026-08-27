using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class TraitStoryBookHome : TraitScroll
{
	public override void OnRead(Chara c)
	{
		Dictionary<string, int> dict = new Dictionary<string, int>();
		List<SourceQuest.Row> list = EClass.sources.quests.rows.Where((SourceQuest.Row q) => IsAvailable(q)).ToList();
		EClass.ui.AddLayer<LayerList>().SetSize().SetList2(list, (SourceQuest.Row a) => GetTitle(a), delegate(SourceQuest.Row a, ItemGeneral b)
		{
			EClass.ui.GetLayer<LayerList>().SetActive(enable: false);
			LayerDrama.fromBook = true;
			string text = a.drama[0];
			string idStep = "quest_" + a.id;
			if (a.id == "pre_debt_runaway")
			{
				idStep = "loytelEscaped";
			}
			LayerDrama.Activate(text, text, idStep, GetChara(text)).SetOnKill(delegate
			{
				EClass.ui.GetLayer<LayerList>().SetActive(enable: true);
			});
		}, delegate
		{
		}, autoClose: false);
		static Chara GetChara(string id)
		{
			return EClass.game.cards.globalCharas.Find(id);
		}
		string GetTitle(SourceQuest.Row r)
		{
			string name = r.GetName();
			string text = Regex.Replace(r.id, "([0-9]*$)", "");
			string str = r.id.Replace(text, "");
			if (!str.IsEmpty())
			{
				str.ToInt();
			}
			if (name.IsEmpty())
			{
				r = EClass.sources.quests.map.TryGetValue(text);
				if (r != null)
				{
					name = r.GetName();
				}
			}
			if (!dict.ContainsKey(name))
			{
				dict.Add(name, 0);
			}
			dict[name]++;
			return name + " " + dict[name];
		}
		static bool IsAvailable(SourceQuest.Row r)
		{
			if (r.drama.IsEmpty() || GetChara(r.drama[0]) == null)
			{
				return false;
			}
			if (!EClass.debug.showExtra && r.tags.Contains("debug"))
			{
				return false;
			}
			string text = Regex.Replace(r.id, "([0-9]*$)", "");
			if (EClass.game.quests.completedIDs.Contains(r.id) || EClass.game.quests.completedIDs.Contains(text))
			{
				return true;
			}
			string str = r.id.Replace(text, "");
			int num = 0;
			if (!str.IsEmpty())
			{
				num = str.ToInt();
			}
			Quest quest = EClass.game.quests.Get(text);
			if (quest != null && num <= quest.phase)
			{
				return true;
			}
			return EClass.debug.showExtra;
		}
	}
}
