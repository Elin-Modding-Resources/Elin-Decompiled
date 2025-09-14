public class TraitMonsterBall : Trait
{
	public Chara chara
	{
		get
		{
			return owner.GetObj<Chara>(8);
		}
		set
		{
			owner.SetObj(8, value);
		}
	}

	public override bool IsThrowMainAction => true;

	public override ThrowType ThrowType => ThrowType.MonsterBall;

	public override EffectDead EffectDead => EffectDead.None;

	public virtual bool IsLittleBall => false;

	public virtual bool IsDuponneBall => false;

	public override void OnCreate(int lv)
	{
		if (!IsLittleBall && !IsDuponneBall)
		{
			owner.SetLv(1 + EClass.rnd(lv + 10));
		}
	}

	public override bool CanStackTo(Thing to)
	{
		if (to.GetObj<Chara>(8) == null && chara == null)
		{
			return to.LV == owner.LV;
		}
		return false;
	}

	public override void SetName(ref string s)
	{
		s = ((IsDuponneBall || IsLittleBall) ? "_littleBall" : "_monsterball").lang(s, owner.LV.ToString() ?? "", (chara == null) ? "_monsterball_empty".lang() : chara.Name);
	}

	public override int GetValue()
	{
		if (IsLittleBall || IsDuponneBall)
		{
			return base.GetValue();
		}
		return base.GetValue() * (100 + owner.LV * 15) / 100;
	}
}
