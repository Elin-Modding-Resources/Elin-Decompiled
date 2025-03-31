using System;

public class TraitDrink : Trait
{
	public virtual EffectId IdEffect => (GetParam(1) ?? "Drink").ToEnum<EffectId>();

	public virtual int Power
	{
		get
		{
			if (GetParam(2) == null)
			{
				return 100;
			}
			return GetParam(2).ToInt();
		}
	}

	public virtual int IdEle
	{
		get
		{
			if (!AliasEle.IsEmpty())
			{
				return EClass.sources.elements.alias[AliasEle].id;
			}
			return 0;
		}
	}

	public virtual SourceElement.Row source => null;

	public override bool IsNoShop
	{
		get
		{
			if (source != null)
			{
				return source.tag.Contains("noShop");
			}
			return false;
		}
	}

	public virtual string AliasEle => N1;

	public virtual string N1 => GetParam(3);

	public virtual bool IsNeg => owner.HasTag(CTAG.neg);

	public override bool IsThrowMainAction => true;

	public override ThrowType ThrowType => ThrowType.Potion;

	public override bool IsBlendBase => IsBlend();

	public override bool CanDrink(Chara c)
	{
		return true;
	}

	public bool IsBlend()
	{
		switch (IdEffect)
		{
		case EffectId.DrinkWater:
		case EffectId.Poison:
		case EffectId.Love:
		case EffectId.Acidproof:
			return true;
		default:
			return false;
		}
	}

	public override bool CanBlend(Thing t)
	{
		switch (IdEffect)
		{
		case EffectId.Poison:
		case EffectId.Love:
			return t.IsFood;
		case EffectId.Acidproof:
			if (!t.isAcidproof)
			{
				return t.IsEquipmentOrRangedOrAmmo;
			}
			return false;
		case EffectId.DrinkWater:
			if (t.category.ignoreBless != 1 && t.id != "water")
			{
				return t.id != "bucket";
			}
			return false;
		default:
			return false;
		}
	}

	public override void OnBlend(Thing t, Chara c)
	{
		switch (IdEffect)
		{
		case EffectId.Love:
			BlendLove(c, t);
			break;
		case EffectId.Poison:
			BlendPoison(c, t);
			break;
		case EffectId.Acidproof:
			c.Say("coated", t, c);
			if (t.IsEquipment && owner.blessedState <= BlessedState.Cursed)
			{
				c.Say("blend_curse", t);
				t.ModEncLv(-1);
			}
			else
			{
				t.isAcidproof = true;
			}
			break;
		case EffectId.DrinkWater:
		{
			BlessedState blessedState = owner.blessedState;
			t = t.Split(1);
			if (blessedState >= BlessedState.Blessed)
			{
				Msg.Say("waterBless", t);
				t.SetBlessedState(BlessedState.Blessed);
				c.Pick(t);
			}
			else if (blessedState <= BlessedState.Cursed)
			{
				Msg.Say("waterCurse", t);
				t.SetBlessedState(BlessedState.Cursed);
				c.Pick(t);
			}
			else
			{
				Msg.SayNothingHappen();
			}
			break;
		}
		}
		owner.ModNum(-1);
	}

	public static void BlendLove(Chara c, Thing t, bool dream = false)
	{
		Msg.Say("blend_love", t);
		Thing thing = t.Split(1);
		thing.elements.ModBase(dream ? 704 : 703, t.IsBlessed ? 15 : (t.IsCursed ? 5 : 10));
		c.Pick(thing);
	}

	public static void BlendPoison(Chara c, Thing t)
	{
		Msg.Say("blend_poison", t);
		Thing thing = t.Split(1);
		thing.elements.ModBase(702, t.IsBlessed ? 5 : (t.IsCursed ? 15 : 10));
		c.Pick(thing);
	}

	public override void OnDrink(Chara c)
	{
		if (owner.id == "334" && c.HasCondition<ConAwakening>())
		{
			c.RemoveCondition<ConAwakening>();
			return;
		}
		ActEffect.Proc(IdEffect, Power, owner.blessedState, c, null, new ActRef
		{
			n1 = N1,
			isPerfume = (this is TraitPerfume),
			refThing = owner.Thing,
			act = ((source != null) ? ACT.Create(source) : null)
		});
		FoodEffect.ProcTrait(c, owner);
	}

	public override void OnThrowGround(Chara c, Point p)
	{
		if (IdEffect != EffectId.Drink)
		{
			bool isHostileAct = c.IsPCParty && IsNeg;
			EClass._map.SetLiquid(p.x, p.z, new CellEffect
			{
				id = 4,
				idEffect = IdEffect,
				power = Power,
				amount = 5,
				isHostileAct = isHostileAct,
				color = BaseTileMap.GetColorInt(ref owner.GetRandomColor(), owner.sourceRenderCard.colorMod),
				idEle = IdEle,
				n1 = N1,
				isBlessed = (owner.blessedState >= BlessedState.Blessed),
				isCursed = (owner.blessedState <= BlessedState.Cursed)
			});
		}
		else
		{
			EClass._map.SetLiquid(p.x, p.z, 1, 3);
		}
	}

	public override Action GetHealAction(Chara c)
	{
		if (IdEffect == EffectId.Heal)
		{
			return delegate
			{
				c.Drink(owner);
			};
		}
		return null;
	}
}
