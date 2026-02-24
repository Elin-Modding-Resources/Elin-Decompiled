public class TraitDeedDivorce : TraitTool
{
	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.ListCharas().ForEach(delegate(Chara a)
		{
			if (a.IsMarried)
			{
				p.TrySetAct("actDivorce", delegate
				{
					Dialog.YesNo("dialogDivorce", delegate
					{
						EClass.pc.Say("use_whip", a, owner);
						a.Talk("death_other");
						EClass.pc.PlaySound("whip");
						a.PlayAnime(AnimeID.Shiver);
						a.PlaySound("laugh_death");
						a.Divorce(EClass.pc);
						owner.ModNum(-1);
						EClass.pc.Say("divorce", EClass.pc, a);
						a.ModAffinity(EClass.pc, -200);
					});
					return false;
				}, a);
			}
		});
	}
}
