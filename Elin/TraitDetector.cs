public class TraitDetector : TraitItem
{
	public int interval;

	public override bool IsTool => true;

	public override bool CanStack => false;

	public string term => owner.c_idRefName;

	public override void TrySetHeldAct(ActPlan p)
	{
		if (p.IsSelf)
		{
			p.TrySetAct(LangUse, () => OnUse(EClass.pc), owner);
		}
	}

	public override bool OnUse(Chara c)
	{
		Dialog.InputName("dialogDetector", term, delegate(bool cancel, string text)
		{
			if (!cancel)
			{
				owner.c_idRefName = text;
				Search();
			}
		});
		return false;
	}

	public override void OnHeld()
	{
		Search();
	}

	public override void OnTickHeld()
	{
		interval--;
		if (interval <= 0)
		{
			Search();
		}
	}

	public void Search()
	{
		if (term.IsEmpty())
		{
			return;
		}
		Card card = null;
		int num = 999;
		foreach (Thing thing in EClass._map.things)
		{
			int num2 = EClass.pc.Dist(thing);
			if ((thing.id.ToLower().Contains(term.ToLower()) || thing.Name.ToLower().Contains(term.ToLower())) && num2 < num)
			{
				num = num2;
				card = thing;
			}
		}
		interval = 10;
		if (card == null)
		{
			EClass.pc.PlaySound("detect_none");
		}
		else
		{
			EClass.pc.PlaySound("detect_" + ((num <= 1) ? "detected" : ((num < 5) ? "near" : ((num < 15) ? "medium" : ((num < 30) ? "far" : ((num < 50) ? "veryFar" : "superFar"))))));
			interval = ((num <= 1) ? 1 : ((num < 5) ? 2 : ((num < 15) ? 4 : ((num < 30) ? 7 : 10))));
			if (EClass.core.config.sound.volumeSFX < 0.1f || EClass.core.config.sound.volumeMaster < 0.1f)
			{
				WidgetMainText.ignoreStack = true;
				Msg.Say("beep".lang());
				WidgetMainText.ignoreStack = false;
			}
		}
		owner.PlayAnime(AnimeID.HitObj);
	}

	public override void SetName(ref string s)
	{
		if (!term.IsEmpty())
		{
			s = "_detect".lang(s, term);
		}
	}
}
