using System;
using UnityEngine;

public class ContentQuest : EContent
{
	public UIList list;

	public UIText textClient;

	public UIText textTitle;

	public UIText textDetail;

	public UIText textHours;

	public UIText textNote;

	public UIText textReward;

	public UIText textZone;

	public Portrait portrait;

	public UIButton buttonAbandon;

	public override void OnSwitchContent(int idTab)
	{
		list.sortMode = UIList.SortMode.ByValue;
		list.callbacks = new UIList.Callback<Quest, ItemGeneral>
		{
			onInstantiate = delegate(Quest a, ItemGeneral b)
			{
				b.SetSound();
				string text = a.GetTitle();
				if (EClass.debug.showExtra)
				{
					text = text + "(" + a.phase + ")";
				}
				b.button1.mainText.SetText(a.TitlePrefix + text);
				b.SetSubText(a.TextDeadline, 260, FontColor.Default, TextAnchor.MiddleRight);
				b.button2.SetActive(EClass.debug.enable && a.CanAutoAdvance);
				if (EClass.debug.enable && a.CanAutoAdvance)
				{
					b.button2.SetOnClick(delegate
					{
						if (a.task != null)
						{
							a.CompleteTask();
						}
						else if (a is QuestSequence)
						{
							a.NextPhase();
						}
						else
						{
							a.Complete();
						}
						list.List();
					});
				}
				b.button3.SetOnClick(delegate
				{
					a.track = !a.track;
					if (!WidgetQuestTracker.Instance)
					{
						EClass.player.questTracker = true;
						EClass.ui.widgets.ActivateWidget("QuestTracker");
						WidgetHotbar.RefreshButtons();
					}
					if ((bool)WidgetQuestTracker.Instance)
					{
						WidgetQuestTracker.Instance.Refresh();
					}
					list.Select(a);
					SelectQuest(a);
					RefreshTrackButtons();
				});
			},
			onClick = delegate(Quest a, ItemGeneral b)
			{
				SelectQuest(a);
			},
			onList = delegate
			{
				foreach (Quest item in EClass.game.quests.list)
				{
					list.Add(item);
				}
			},
			onSort = (Quest a, UIList.SortMode m) => -EClass.sources.quests.rows.IndexOf(a.source)
		};
		list.List();
		SelectQuest(list.items[0] as Quest);
		RefreshTrackButtons();
	}

	public void RefreshTrackButtons()
	{
		foreach (UIList.ButtonPair button in list.buttons)
		{
			ClassExtension.SetActive(enable: (button.obj as Quest).track, c: (button.component as ItemGeneral).button3.icon);
		}
	}

	public void SelectQuest(Quest q)
	{
		buttonAbandon.SetActive(q.CanAbandon);
		buttonAbandon.SetOnClick(delegate
		{
			Dialog.YesNo("dialog_abandonQuest", delegate
			{
				Msg.Say("questAbandon", q.GetTitle());
				q.Fail();
				OnSwitchContent(0);
			});
		});
		textClient.text = q.person.NameBraced;
		textReward.SetText(q.GetRewardText());
		string text = q.GetDetail(onJournal: true);
		if (EClass.debug.showExtra && q.person != null)
		{
			text += Environment.NewLine;
			text = text + q.person.id + "/" + q.person.name + "/" + q.person.chara?.ToString() + Environment.NewLine;
			if (q is QuestDeliver questDeliver)
			{
				text = text + questDeliver.uidDest + "/" + questDeliver.DestChara;
			}
		}
		textDetail.SetText(text);
		textHours.text = q.TextDeadline;
		textZone.text = q.ClientZone?.Name ?? "-";
		portrait.SetPerson(q.person);
		this.RebuildLayout(recursive: true);
	}
}
