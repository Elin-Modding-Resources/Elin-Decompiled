using System.Collections.Generic;
using System.Linq;

public static class CustomDramaExpansionHelper
{
	public static void AddTempTalk(this DramaManager dm, string text, string actor = "tg", string? jump = null)
	{
		if (jump == null || dm.sequence.steps.ContainsKey(jump))
		{
			DramaEventTalk item = new DramaEventTalk
			{
				idActor = actor,
				idJump = jump,
				text = text,
				temp = true,
				sequence = dm.sequence
			};
			dm.sequence.tempEvents.Add(item);
		}
	}

	public static void Goto(this DramaManager dm, string step)
	{
		if (dm.sequence.steps.ContainsKey(step))
		{
			dm.sequence.Play(step);
		}
	}

	public static void InjectUniqueRumor(this DramaManager dm, Chara chara = null)
	{
		if (chara == null)
		{
			chara = dm.tg.chara;
		}
		if (chara == null)
		{
			return;
		}
		DramaCustomSequence cs = new DramaCustomSequence();
		bool flag = chara.IsHumanSpeak || EClass.pc.HasElement(1640);
		if ((!chara.IsUnique && !cs.HasTopic("unique", chara.id)) || !flag)
		{
			return;
		}
		dm.CustomEvent(dm.sequence.Exit);
		DramaChoice choice = new DramaChoice("letsTalk".lang(), dm.sequence.steps.Last().Key.IsEmpty(dm.setup.step));
		dm.lastTalk.choices.Add(choice);
		dm._choices.Add(choice);
		string rumor = (chara.IsPCParty ? chara.GetTalkText("sup") : cs.GetRumor(chara));
		choice.SetOnClick(delegate
		{
			dm.sequence.firstTalk.funcText = () => rumor;
			List<Hobby> list = chara.ListHobbies();
			Hobby hobby = ((list.Count > 0) ? list[0] : null);
			if (EClass.rnd(20) == 0 || EClass.debug.showFav)
			{
				if (EClass.rnd(2) == 0 || hobby == null)
				{
					GameLang.refDrama1 = chara.GetFavCat().GetName().ToLower();
					GameLang.refDrama2 = chara.GetFavFood().GetName();
					rumor = cs.GetText(chara, "general", "talk_fav");
					chara.knowFav = true;
				}
				else
				{
					GameLang.refDrama1 = hobby.Name.ToLower();
					rumor = cs.GetText(chara, "general", "talk_hobby");
				}
			}
			else
			{
				rumor = cs.GetRumor(chara);
			}
			chara.affinity.OnTalkRumor();
			choice.forceHighlight = true;
		}).SetCondition(() => chara.interest > 0);
	}
}
