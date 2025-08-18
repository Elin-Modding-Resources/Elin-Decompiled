using UnityEngine;
using UnityEngine.UI;

public class ButtonChara : UIButton
{
	public enum Mode
	{
		Default,
		Hire,
		Journal,
		Embark,
		Ranking
	}

	public Portrait portrait;

	public UIText textName;

	public UIText textAlias;

	public UIText textBio;

	public UIText textWork;

	public UIText textHobby;

	public UIText textLifeStyle;

	public new UIItem item;

	public Transform transDefeated;

	public LayoutGroup layoutTag;

	public void SetChara(Chara c, Mode m)
	{
		if ((bool)portrait)
		{
			portrait.SetChara(c);
		}
		if (m == Mode.Ranking)
		{
			textBio.text = ((c.currentZone == null) ? "???" : c.currentZone.Name);
		}
		else
		{
			textAlias.text = c.Aka;
			if ((bool)textBio)
			{
				textBio.text = c.race.GetText().ToTitleCase(wholeText: true) + " " + c.bio.TextAge(c) + " " + Lang._gender(c.bio.gender);
			}
		}
		textName.text = c.Name;
		if ((bool)layoutTag)
		{
			layoutTag.DestroyChildren();
			HintIcon hintIcon = Util.Instantiate<HintIcon>("UI/Element/Item/Tag General", layoutTag);
			hintIcon.text.SetText(c.job.GetName().ToTitleCase(wholeText: true));
			hintIcon.RebuildLayout();
			layoutTag.RebuildLayout();
		}
	}
}
