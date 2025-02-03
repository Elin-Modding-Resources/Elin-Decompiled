using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TraitBaseSpellbook : TraitScroll
{
	public enum Type
	{
		Ancient,
		Spell,
		RandomSpell,
		Ero,
		Dojin
	}

	public static ElementSelecter selecter = new ElementSelecter
	{
		type = "B",
		useDomain = true
	};

	public virtual Type BookType => Type.Spell;

	public virtual int Difficulty => 10 + owner.LV;

	public override bool CanStack => false;

	public override bool HasCharges => true;

	public override float MTPValue => 10f;

	public override bool CanRead(Chara c)
	{
		return !c.isBlind;
	}

	public override int GetActDuration(Chara c)
	{
		return Mathf.Max(Difficulty, 100) * Mathf.Max(100 - (int)Mathf.Sqrt(c.Evalue(285)) * 7, 10) / 400;
	}

	public override void OnCreate(int lv)
	{
		owner.c_charges = 1 + EClass.rnd(4) + EClass.rnd(EClass.rnd(4) + 1);
		switch (BookType)
		{
		case Type.RandomSpell:
			owner.refVal = selecter.Select(lv);
			break;
		case Type.Ancient:
		{
			int max = Lang.GetList("ancientbook").Length;
			owner.refVal = EClass.rnd(Mathf.Clamp(EClass.rnd(lv / 5 + 5), 1, max));
			break;
		}
		case Type.Ero:
		{
			IEnumerable<SourceChara.Row> ie = EClass.sources.charas.rows.Where((SourceChara.Row a) => !a.name.IsEmpty() && a.name.Length > 2 && a.name[0] != '<' && a.race != "god" && !a.HasTag(CTAG.noRandomProduct));
			owner.c_idRefName = ie.RandomItem().id;
			break;
		}
		case Type.Dojin:
			owner.c_idRefName = EClass.game.religions.dictAll.RandomItem().id;
			break;
		case Type.Spell:
			break;
		}
	}

	public override bool TryProgress(AIProgress p)
	{
		Chara c = p.owner;
		if (BookType == Type.Ancient && owner.isOn)
		{
			if (c.IsPC)
			{
				c.Say("alreadyDecoded");
			}
			return false;
		}
		if (c.isBlind)
		{
			c.Say("blinded", c);
			return false;
		}
		int diff = Mathf.Max(1, Difficulty * ((owner.blessedState >= BlessedState.Blessed) ? 75 : ((owner.blessedState <= BlessedState.Cursed) ? 300 : 100)) / 100);
		int check = Mathf.Max(1, c.Evalue(eleParent) * (100 + c.Evalue(285) * 10) / 100);
		if (ReadCheck() || EClass.debug.godMode || owner.HasEditorTag(EditorTag.NoReadFail))
		{
			return true;
		}
		ReadFailEffect(c);
		ModCharge(c);
		return false;
		bool ReadCheck()
		{
			if (EClass.rnd(4) != 0 && (c.isConfused || c.HasCondition<ConDim>()))
			{
				return false;
			}
			if (check > diff * 3)
			{
				return true;
			}
			if (EClass.rnd(check * 30) < diff)
			{
				return false;
			}
			return true;
		}
	}

	public static void ReadFailEffect(Chara c)
	{
		if (EClass.rnd(2) == 0)
		{
			c.Say("spell_fail_mana", c);
			c.mana.Mod(-c.mana.max / (c.IsPC ? 2 : 5));
		}
		else if (EClass.rnd(3) == 0)
		{
			c.Say("spell_fail_confuse", c);
			c.AddCondition<ConConfuse>();
		}
		else if (((c.IsPCFaction && c.memberType == FactionMemberType.Default) || !EClass._zone.IsPCFaction) && EClass.rnd(3) == 0)
		{
			c.Say("spell_fail_monster", c);
			c.PlaySound("spell_funnel");
			for (int i = 0; i < Mathf.Clamp(1 + EClass._zone.DangerLv / 15 + EClass.rnd(3 + EClass._zone.DangerLv / 15), 1, 8); i++)
			{
				Chara chara = CharaGen.CreateFromFilter("c_readFail", EClass._zone.DangerLv);
				EClass._zone.AddCard(chara, c.pos.GetNearestPoint(allowBlock: false, allowChara: false));
				chara.pos.PlayEffect("teleport");
			}
		}
		else
		{
			c.Say("spell_fail_teleport", c);
			if (c.IsPCFaction && !c.IsPC)
			{
				c.SayNothingHappans();
			}
			else
			{
				ActEffect.Proc(EffectId.Teleport, c);
			}
		}
	}

	public override void OnRead(Chara c)
	{
		bool flag = BookType == Type.Spell || BookType == Type.RandomSpell;
		int a = -1;
		string name = owner.Name;
		if (c.IsPCParty)
		{
			owner.Thing?.Identify(show: true, IDTSource.SuperiorIdentify);
			owner.isOn = true;
		}
		switch (BookType)
		{
		case Type.Ancient:
			c.Say("book_decode", c, name);
			if (!c.IsPC)
			{
				ModCharge(c, -owner.c_charges);
			}
			if (c.IsPC)
			{
				Guild.Mage.AddContribution(5 + owner.refVal * 2);
			}
			break;
		case Type.Spell:
		case Type.RandomSpell:
			if (c.IsPC)
			{
				c.GainAbility(source.id);
			}
			ModCharge(c);
			break;
		case Type.Ero:
		case Type.Dojin:
			c.PlaySound("wow");
			c.Say("book_decode", c, owner);
			if (!c.IsPC)
			{
				c.Talk("wow");
			}
			switch (BookType)
			{
			case Type.Ero:
				if (c.IsPC)
				{
					EClass.pc.SAN.Mod(-(EClass.rnd(5) + 1));
				}
				if (!owner.c_idRefName.IsEmpty())
				{
					CardRow cardRow = EClass.sources.cards.map[owner.c_idRefName];
					c.Say("learn_weakspot", c, cardRow.GetName());
					if (c.IsPC)
					{
						EClass.player.codex.AddWeakspot(cardRow.id);
					}
				}
				if (c.conSleep != null)
				{
					ActEffect.Proc(EffectId.Sleep, c);
				}
				break;
			case Type.Dojin:
			{
				Religion religion = EClass.game.religions.dictAll.TryGetValue(owner.c_idRefName) ?? EClass.game.religions.Eyth;
				if (c.IsPC)
				{
					EClass.player.ModKarma(-1);
				}
				c.AddCondition<ConInsane>(500);
				c.AddCondition<ConFaint>();
				if (!c.IsPC && c.faith != religion)
				{
					if (!c.source.faith.IsEmpty())
					{
						c.Say("faith_stands", c);
						break;
					}
					religion.JoinFaith(c);
					a = -owner.c_charges;
				}
				break;
			}
			}
			ModCharge(c, a);
			break;
		}
		c.ModExp(285, 180);
		if (flag)
		{
			c.ModExp(307, 200);
		}
		if (c.IsPC)
		{
			LayerAbility.Redraw();
		}
	}

	public void ModCharge(Chara c, int a = -1)
	{
		owner.ModCharge(a);
		LayerInventory.SetDirtyAll();
		if (owner.c_charges <= 0)
		{
			c.Say("spellbookCrumble", owner);
			owner.ModNum(-1);
		}
	}

	public override void SetName(ref string s)
	{
		if (!owner.IsIdentified)
		{
			return;
		}
		switch (BookType)
		{
		case Type.Ancient:
			s = "_titled".lang(Lang.GetList("ancientbook")[owner.refVal], s);
			if (owner.isOn)
			{
				s = "_deciphered ".lang(s);
			}
			break;
		case Type.Ero:
		case Type.Dojin:
		{
			string c_idRefName = owner.c_idRefName;
			if (!c_idRefName.IsEmpty())
			{
				string @ref = ((BookType == Type.Dojin) ? EClass.game.religions.dictAll[c_idRefName].Name : EClass.sources.charas.map.TryGetValue(c_idRefName, "putty").GetName());
				s = "_'s".lang(@ref, s);
			}
			break;
		}
		}
	}
}
