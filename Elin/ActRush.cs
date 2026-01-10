public class ActRush : ActMelee
{
	public override bool ShowMapHighlight => true;

	public override bool ShouldRollMax => true;

	public override int PerformDistance => 6;

	public override void OnMarkMapHighlights()
	{
		if (!EClass.scene.mouseTarget.pos.IsValid || EClass.scene.mouseTarget.TargetChara == null)
		{
			return;
		}
		Point dest = EClass.scene.mouseTarget.pos;
		Los.IsVisible(EClass.pc.pos, dest, delegate(Point p, bool blocked)
		{
			if (!p.Equals(EClass.pc.pos))
			{
				p.SetHighlight((blocked || p.IsBlocked || (!p.Equals(dest) && p.HasChara)) ? 4 : ((p.Distance(EClass.pc.pos) <= 2) ? 2 : 8));
			}
		});
	}

	public override bool CanPerform()
	{
		bool flag = Act.CC.IsPC && !(Act.CC.ai is GoalAutoCombat);
		if (flag)
		{
			Act.TC = EClass.scene.mouseTarget.card;
		}
		if (Act.TC == null)
		{
			return false;
		}
		if (Act.TC.isThing && !Act.TC.trait.CanBeAttacked)
		{
			return false;
		}
		if (Act.TC.Chara?.mimicry != null)
		{
			return false;
		}
		Act.TP.Set(flag ? EClass.scene.mouseTarget.pos : Act.TC.pos);
		if (Act.CC.isRestrained || Act.CC.HasCondition<ConEntangle>())
		{
			return false;
		}
		if (Act.CC.host != null || Act.CC.Dist(Act.TP) <= 2)
		{
			return false;
		}
		if (Los.GetRushPoint(Act.CC.pos, Act.TP) == null)
		{
			return false;
		}
		return base.CanPerform();
	}

	public override bool Perform()
	{
		bool flag = Act.CC.IsPC && !(Act.CC.ai is GoalAutoCombat);
		if (flag)
		{
			Act.TC = EClass.scene.mouseTarget.card;
		}
		if (Act.TC == null)
		{
			return false;
		}
		Act.TP.Set(flag ? EClass.scene.mouseTarget.pos : Act.TC.pos);
		int num = Act.CC.Dist(Act.TP);
		Point rushPoint = Los.GetRushPoint(Act.CC.pos, Act.TP);
		Act.CC.pos.PlayEffect("vanish");
		Act.CC.MoveImmediate(rushPoint, focus: true, cancelAI: false);
		Act.CC.Say("rush", Act.CC, Act.TC);
		Act.CC.PlaySound("rush");
		Act.CC.pos.PlayEffect("vanish");
		float num2 = 1f + 0.1f * (float)num;
		num2 = num2 * (float)(100 + EClass.curve(Act.CC.Evalue(382), 50, 25, 65)) / 100f;
		Attack(num2);
		if (Act.TC.isChara && Act.TC.ExistsOnMap && Act.CC.HasElement(382))
		{
			if (!Act.TC.IsPowerful || EClass.rnd(4) == 0)
			{
				Act.TC.Chara.AddCondition<ConParalyze>(2, force: true);
			}
			if (!Act.TC.IsPowerful || EClass.rnd(3) == 0)
			{
				Act.TC.Chara.AddCondition<ConDim>(5, force: true);
			}
			if (!Act.TC.IsPowerful || EClass.rnd(2) == 0)
			{
				Act.TC.Chara.AddCondition<ConConfuse>(8, force: true);
			}
		}
		return true;
	}
}
