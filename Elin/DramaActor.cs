using System;
using System.Collections.Generic;
using System.Text;

public class DramaActor : EMono
{
	public static bool useGlitch;

	public string id;

	public Person owner;

	public DramaSequence sequence;

	private int choiceIdx;

	public DialogDrama dialog => sequence.dialog;

	public void Init(DramaSequence _sequence, string _id, Person _owner)
	{
		sequence = _sequence;
		id = _id;
		owner = _owner;
		base.gameObject.SetActive(value: false);
	}

	public void Talk(string text, List<DramaChoice> choices, bool center, bool unknown)
	{
		Chara chara = owner.chara;
		dialog.gameObject.SetActive(value: true);
		dialog.textName.text = (unknown ? "???" : owner.GetDramaTitle());
		if ((bool)dialog.portrait)
		{
			dialog.portrait.enableFull = sequence.fullPortrait;
			dialog.portrait.SetPerson(owner);
		}
		if (!owner.HumanSpeak)
		{
			if (!text.StartsWith("("))
			{
				text = "(" + text + ")";
			}
			text = text.Replace("。)", ")");
		}
		text = ConvertAdv(text);
		text = GameLang.ConvertDrama(text, owner.chara);
		if (chara != null)
		{
			if (chara.HasElement(1232))
			{
				BackerContent.GakiConvert(ref text, "babu");
			}
			else
			{
				string text2 = chara.id;
				if (!(text2 == "adv_gaki"))
				{
					if (text2 == "corgon")
					{
						BackerContent.GakiConvert(ref text, "mokyu");
					}
				}
				else
				{
					BackerContent.GakiConvert(ref text);
				}
			}
		}
		text = text.Replace("((", "(").Replace("))", ")").Replace("（（", "（")
			.Replace("））", "）");
		if ((bool)dialog.goAffinity)
		{
			dialog.layoutInterest.SetActive(chara != null);
			dialog.textNoInterest.SetActive(chara == null);
			bool flag = chara?.trait.ShowAdvRank ?? false;
			if ((bool)dialog.transFav)
			{
				dialog.transFav.SetActive(chara?.knowFav ?? false);
			}
			if ((bool)dialog.transRank)
			{
				dialog.transRank.SetActive(flag);
				if (flag)
				{
					dialog.textRank.text = "factionRank2".lang() + " " + chara.trait.GetAdvRankText() + " " + ((chara.GetInt(111) > 0) ? "advDefeated".lang(chara.GetInt(111).ToString() ?? "") : "");
				}
			}
			if (chara != null)
			{
				dialog.textAffinity.text = ((chara.c_bossType != 0) ? " - " : (chara.affinity.Name + " (" + chara._affinity + ")"));
				dialog.layoutInterest.DestroyChildren();
				int num = chara.interest / 10 + 1;
				if (chara.interest <= 0)
				{
					num = 0;
				}
				if (num > 10)
				{
					num = 10;
				}
				for (int i = 0; i < num; i++)
				{
					Util.Instantiate(dialog.moldInterest, dialog.layoutInterest);
				}
				dialog.layoutInterest.RebuildLayout();
				dialog.textNoInterest.SetActive(num <= 0);
				if ((bool)dialog.textFav && chara.knowFav)
				{
					dialog.textFav.text = chara.GetFavCat().GetName().ToTitleCase() + Environment.NewLine + chara.GetFavFood().GetName().ToTitleCase();
				}
			}
			else
			{
				dialog.textAffinity.text = "???";
				dialog.textNoInterest.text = "???";
			}
		}
		if ((bool)dialog.glitch)
		{
			dialog.glitch.enabled = useGlitch;
			useGlitch = false;
		}
		dialog.SetText(text.ToTitleCase(), center);
		dialog.ClearChoice();
		choiceIdx = 1;
		for (int j = 0; j < choices.Count; j++)
		{
			SetChoice(choices[j], j);
		}
		dialog.transChoices.RebuildLayout(recursive: true);
		dialog.iconNext.SetActive(choiceIdx == 1);
		UIButton.TryHihlight();
		EMono.core.actionsNextFrame.Add(UIButton.TryHihlight);
		EMono.core.actionsNextFrame.Add(delegate
		{
			EMono.core.actionsNextFrame.Add(UIButton.TryHihlight);
		});
	}

	private void SetChoice(DramaChoice item, int index)
	{
		item.index = index;
		if ((!item.IF.IsEmpty() && !DramaOutcome.If(item, owner.chara)) || (item.activeCondition != null && !item.activeCondition()))
		{
			return;
		}
		DramaOutcome.idJump = null;
		string text = item.text.TrimEnd(Environment.NewLine.ToCharArray());
		text = ConvertAdv(text);
		text = GameLang.ConvertDrama(text, owner.chara);
		if (item.check != null && EMono.debug.logDice)
		{
			text = text + " (" + item.check.GetText(EMono.pc, owner.chara, inDialog: true) + ")";
		}
		Check check = null;
		string idJump = item.idJump;
		string idAction = item.idAction;
		object[] PARAMS = null;
		int? affinity = null;
		if (!item.CHECK.IsEmpty())
		{
			string[] array = item.CHECK.Split('/');
			string text2 = array[0];
			if (!(text2 == "check"))
			{
				if (text2 == "affinity")
				{
				}
			}
			else
			{
				check = Check.Get(array[1]);
			}
		}
		UIButton uIButton = dialog.AddChoice(item, text, delegate
		{
			base.gameObject.SetActive(value: false);
			DramaChoice.lastChoice = item;
			_ = check;
			if (affinity.HasValue)
			{
				bool flag = true;
				if (flag)
				{
					idJump = idJump.Split('/')[0];
				}
				else
				{
					idJump = idJump.Split('/')[1];
				}
				PARAMS = new object[1] { flag };
			}
			if (!idAction.IsEmpty())
			{
				typeof(DramaOutcome).GetMethod(sequence.id + "_" + idAction).Invoke(sequence.manager.outcome, PARAMS);
				if (DramaOutcome.idJump != null)
				{
					sequence.Play(DramaOutcome.idJump);
					return;
				}
			}
			if (item.onClick != null)
			{
				item.onClick();
			}
			sequence.tempEvents.Clear();
			if (item.onJump != null)
			{
				item.onJump();
			}
			else if (idJump.IsEmpty())
			{
				sequence.PlayNext();
			}
			else
			{
				sequence.Play(idJump);
			}
		});
		if ((bool)uIButton.subText)
		{
			uIButton.subText.SetText(choiceIdx + ".");
		}
		if (!item.sound)
		{
			uIButton.soundClick = null;
		}
		if (item.onTooltip != null)
		{
			uIButton.SetTooltip(item.onTooltip);
		}
		if (item.forceHighlight)
		{
			uIButton.DoHighlightTransition();
			item.forceHighlight = false;
		}
		choiceIdx++;
	}

	public string ConvertAdv(string text)
	{
		StringBuilder stringBuilder = new StringBuilder(text);
		Religion currentReligion = LayerDrama.currentReligion;
		if (currentReligion != null)
		{
			stringBuilder.Replace("#god_name", currentReligion.Name);
			stringBuilder.Replace("#god_desc", currentReligion.source.GetDetail());
			stringBuilder.Replace("#god_benefit", currentReligion.GetTextBenefit());
			stringBuilder.Replace("#godtalk_worship", currentReligion.GetGodTalk("worship"));
		}
		return stringBuilder.ToString();
	}
}
