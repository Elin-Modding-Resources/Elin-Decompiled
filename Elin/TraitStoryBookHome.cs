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
			string text3 = a.drama[0];
			string idStep = "quest_" + a.id;
			if (a.id == "pre_debt_runaway")
			{
				idStep = "loytelEscaped";
			}
			LayerDrama.Activate(text3, text3, idStep, GetChara(text3)).SetOnKill(delegate
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
			string text2 = Regex.Replace(r.id, "([0-9]*$)", "");
			if (EClass.game.quests.completedIDs.Contains(r.id) || EClass.game.quests.completedIDs.Contains(text2))
			{
				return true;
			}
			string str2 = r.id.Replace(text2, "");
			int num = 0;
			if (!str2.IsEmpty())
			{
				num = str2.ToInt();
			}
			Quest quest = EClass.game.quests.Get(text2);
			if (quest != null && num <= quest.phase)
			{
				return true;
			}
			return EClass.debug.showExtra;
		}
	}
}
