public class TraitWell : Trait
{
	public bool IsHoly => this is TraitWellHoly;

	public override bool IsBlendBase => true;

	public override bool HasCharges => true;

	public override bool ShowCharges => false;

	public int pollution => owner.GetInt(26);

	public int Charges
	{
		get
		{
			if (!IsHoly)
			{
				return owner.c_charges;
			}
			return EClass.player.holyWell;
		}
		set
		{
			if (IsHoly)
			{
				EClass.player.holyWell = value;
			}
			else
			{
				owner.c_charges = value;
			}
		}
	}

	public bool polluted => pollution >= 10;

	public override bool CanBlend(Thing t)
	{
		return t.category.IsChildOf("drink");
	}

	public override void OnBlend(Thing t, Chara c)
	{
		bool flag = polluted;
		if (!IsHoly)
		{
			ModCharges(1);
		}
		if ((t.id == "water" || t.id == "bucket") && t.IsBlessed)
		{
			if (IsHoly)
			{
				ModCharges(1);
			}
			if (polluted)
			{
				Msg.Say("unpolluted", owner);
				Charges = 1;
			}
			owner.SetInt(26);
		}
		else
		{
			string name = owner.Name;
			owner.SetInt(26, pollution + EClass.rnd(3) + ((t.blessedState <= BlessedState.Cursed) ? 10 : 0) + (IsHoly ? 10 : 0));
			if (!flag && polluted)
			{
				Msg.Say("polluted", name);
			}
		}
		t.ModNum(-1);
	}

	public override void OnCreate(int lv)
	{
		if (!IsHoly)
		{
			owner.c_charges = EClass.rnd(6) + 2;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		p.TrySetAct("actDrink", delegate
		{
			if (Charges <= 0)
			{
				EClass.pc.Say("drinkWell_empty", EClass.pc, owner);
				return false;
			}
			EClass.pc.Say("drinkWell", EClass.pc, owner);
			EClass.pc.PlaySound("drink");
			EClass.pc.PlayAnime(AnimeID.Shiver);
			if (IsHoly || EClass.rnd(5) == 0)
			{
				ActEffect.Proc(EffectId.ModPotential, EClass.pc, null, (!polluted && (IsHoly || EClass.rnd(2) == 0)) ? 100 : (-100));
			}
			else if (EClass.rnd(5) == 0)
			{
				BadEffect(EClass.pc);
			}
			else if (EClass.rnd(4) == 0)
			{
				ActEffect.Proc(EffectId.Mutation, EClass.pc);
			}
			else if (EClass.rnd(EClass.debug.enable ? 2 : 10) == 0 && !polluted && !EClass.player.wellWished)
			{
				if (EClass.player.CountKeyItem("well_wish") > 0)
				{
					EClass.player.ModKeyItem("well_wish", -1);
					ActEffect.Proc(EffectId.Wish, EClass.pc, null, 50 + EClass.player.CountKeyItem("well_enhance") * 50);
					EClass.player.wellWished = true;
				}
				else
				{
					Msg.SayNothingHappen();
				}
			}
			else if (polluted)
			{
				EClass.pc.Say("drinkWater_dirty");
				BadEffect(EClass.pc);
			}
			else
			{
				EClass.pc.Say("drinkWater_clear");
			}
			ModCharges(-1);
			return true;
		}, owner);
	}

	public void ModCharges(int a)
	{
		Charges += a;
		if (Charges <= 0)
		{
			EClass.pc.Say("drinkWell_dry", EClass.pc, owner);
		}
	}

	public static void BadEffect(Chara c)
	{
		switch (EClass.rnd(7))
		{
		case 0:
			ActEffect.Proc(EffectId.Blind, c);
			break;
		case 1:
			ActEffect.Proc(EffectId.Paralyze, c);
			break;
		case 2:
			ActEffect.Proc(EffectId.Sleep, c);
			break;
		case 3:
			ActEffect.Proc(EffectId.Poison, c);
			break;
		case 4:
			ActEffect.Proc(EffectId.Faint, c);
			break;
		case 5:
			ActEffect.Proc(EffectId.Disease, c);
			break;
		default:
			ActEffect.Proc(EffectId.Confuse, c);
			break;
		}
	}

	public override void SetName(ref string s)
	{
		if (polluted)
		{
			s = "_polluted".lang(s);
		}
	}
}
