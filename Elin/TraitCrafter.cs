using System.Collections.Generic;
using UnityEngine;

public class TraitCrafter : Trait
{
	public enum MixType
	{
		None,
		Food,
		Resource,
		Dye,
		Butcher,
		Grind,
		Sculpture,
		Talisman,
		Scratch,
		Incubator,
		Fortune,
		RuneMold
	}

	public enum AnimeType
	{
		Default,
		Microwave,
		Pot
	}

	public override bool ShowFuelWindow => false;

	public virtual Emo Icon => Emo.none;

	public virtual int numIng => 1;

	public virtual string IdSource => "";

	public virtual AnimeType animeType => AnimeType.Default;

	public virtual AnimeID IdAnimeProgress => AnimeID.HitObj;

	public virtual string idSoundProgress => "";

	public virtual string idSoundComplete => null;

	public virtual bool StopSoundProgress => false;

	public override bool IsNightOnlyLight => false;

	public virtual bool CanUseFromInventory => true;

	public override bool HoldAsDefaultInteraction => true;

	public virtual string idSoundBG => null;

	public virtual string CrafterTitle => "";

	public virtual bool CanTriggerFire => base.IsRequireFuel;

	public virtual bool AutoTurnOff => false;

	public virtual bool IsConsumeIng => true;

	public virtual bool CloseOnComplete => false;

	public virtual int CostSP => 1;

	public virtual string IDReqEle(RecipeSource r)
	{
		return GetParam(1) ?? "handicraft";
	}

	public virtual bool IsCraftIngredient(Card c, int idx)
	{
		foreach (SourceRecipe.Row row in EClass.sources.recipes.rows)
		{
			if (idx == 1)
			{
				Card card = LayerDragGrid.Instance.buttons[0].Card;
				if (!IsIngredient(0, row, card) || (card == c && card.Num < 2))
				{
					continue;
				}
			}
			if (IsIngredient(idx, row, c))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsIngredient(int idx, SourceRecipe.Row r, Card c)
	{
		if (r.factory != IdSource || c == null)
		{
			return false;
		}
		if (c.c_isImportant && ShouldConsumeIng(r, idx))
		{
			return false;
		}
		string[] array = ((idx == 0) ? r.ing1 : r.ing2);
		if (r.type.ToEnum<MixType>() == MixType.Grind && idx == 1)
		{
			if (r.tag.Contains("rust") && c.encLV >= 0)
			{
				return false;
			}
			if (r.tag.Contains("mod_eject"))
			{
				if (c.sockets == null)
				{
					return false;
				}
				bool flag = false;
				foreach (int socket in c.sockets)
				{
					if (socket != 0)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (r.tag.Contains("noCarbone") && c.material.alias == "carbone")
			{
				return false;
			}
			if (text.StartsWith('#'))
			{
				string text2 = text.Replace("#", "");
				if (c.category.IsChildOf(text2) && IsIngredient(text2, c))
				{
					return true;
				}
				continue;
			}
			string[] array3 = text.Split('@');
			if (array3.Length > 1)
			{
				if (c.id != array3[0] && c.sourceCard._origin != array3[0])
				{
					return false;
				}
				if (c.refCard is SourceChara.Row row && row.race_row.tag.Contains(array3[1]))
				{
					return true;
				}
				if (c.material.tag.Contains(array3[1]))
				{
					return true;
				}
				continue;
			}
			if (text == "any")
			{
				if (this is TraitDyeMaker && !c.category.GetRoot().tag.Contains("dye") && !c.category.tag.Contains("dye"))
				{
					return false;
				}
				if (!c.IsUnique && !c.IsImportant && !c.trait.CanOnlyCarry)
				{
					return true;
				}
			}
			if (c.id == text || c.sourceCard._origin == text)
			{
				return true;
			}
		}
		return false;
	}

	public virtual bool IsIngredient(string cat, Card c)
	{
		return true;
	}

	public int GetSortVal(SourceRecipe.Row r)
	{
		int num = r.id;
		string[] ing = r.ing1;
		for (int i = 0; i < ing.Length; i++)
		{
			if (ing[i].Contains('@'))
			{
				num -= 10000;
			}
		}
		return num;
	}

	public virtual int GetDuration(AI_UseCrafter ai, int costSp)
	{
		return Mathf.Max(1, GetSource(ai).time * 100 / (80 + EClass.pc.Evalue(IDReqEle(ai.recipe?.source)) * 5));
	}

	public virtual int GetCostSp(AI_UseCrafter ai)
	{
		return GetSource(ai).sp;
	}

	public SourceRecipe.Row GetSource(AI_UseCrafter ai)
	{
		List<SourceRecipe.Row> list = new List<SourceRecipe.Row>();
		foreach (SourceRecipe.Row row in EClass.sources.recipes.rows)
		{
			if (row.factory == IdSource)
			{
				list.Add(row);
			}
		}
		for (int i = 0; i < numIng; i++)
		{
			foreach (SourceRecipe.Row row2 in EClass.sources.recipes.rows)
			{
				if (i >= ai.ings.Count || !IsIngredient(i, row2, ai.ings[i]))
				{
					list.Remove(row2);
				}
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		list.Sort((SourceRecipe.Row a, SourceRecipe.Row b) => GetSortVal(a) - GetSortVal(b));
		return list[0];
	}

	public virtual bool ShouldConsumeIng(SourceRecipe.Row item, int index)
	{
		if (IsFactory)
		{
			return true;
		}
		if (item == null)
		{
			return false;
		}
		int id = item.id;
		if ((uint)(id - 47) <= 1u)
		{
			return index == 0;
		}
		return true;
	}

	public virtual Thing Craft(AI_UseCrafter ai)
	{
		Thing thing = ai.ings[0];
		Thing thing2 = ((numIng > 1) ? ai.ings[1] : null);
		SourceRecipe.Row source = GetSource(ai);
		if (source == null)
		{
			return null;
		}
		if (!EClass.player.knownCraft.Contains(source.id))
		{
			SE.Play("idea");
			Msg.Say("newKnownCraft");
			EClass.player.knownCraft.Add(source.id);
			if ((bool)LayerDragGrid.Instance)
			{
				LayerDragGrid.Instance.info.Refresh();
			}
		}
		string thing3 = source.thing;
		MixType mixType = source.type.ToEnum<MixType>();
		int num = source.num.Calc();
		Thing t = null;
		bool claimed;
		switch (mixType)
		{
		case MixType.Food:
			t = CraftUtil.MixIngredients(thing3, ai.ings, CraftUtil.MixType.General, 0, EClass.pc);
			break;
		case MixType.Resource:
		{
			string[] array = thing3.Split('%');
			t = CraftUtil.MixIngredients(ThingGen.Create(array[0], (array.Length > 1) ? EClass.sources.materials.alias[array[1]].id : thing.material.id), ai.ings, CraftUtil.MixType.General, 999, EClass.pc).Thing;
			break;
		}
		case MixType.Dye:
			t = ThingGen.Create(thing3, thing2.material.id);
			break;
		case MixType.Butcher:
			thing3 = SpawnListThing.Get("butcher", (SourceThing.Row a) => a.Category.id == "bodyparts").Select().id;
			t = ThingGen.Create(thing3);
			break;
		case MixType.Grind:
			if (source.tag.Contains("rust"))
			{
				EClass.pc.Say("polish", EClass.pc, ai.ings[1]);
				ai.ings[1].ModEncLv(1);
				ai.ings[0].ModNum(-1);
			}
			if (source.tag.Contains("mod_eject"))
			{
				ai.ings[1].EjectSockets();
				ai.ings[0].ModNum(-1);
			}
			break;
		case MixType.Sculpture:
		{
			t = ThingGen.Create(thing3);
			List<CardRow> list2 = EClass.player.codex.ListKills();
			list2.Add(EClass.sources.cards.map["putty"]);
			list2.Add(EClass.sources.cards.map["snail"]);
			CardRow cardRow = list2.RandomItemWeighted((CardRow a) => Mathf.Max(50 - a.LV, Mathf.Clamp(EClass.pc.Evalue(258) / 2, 1, a.LV * 2)));
			t.c_idRefCard = cardRow.id;
			t.ChangeMaterial(thing.material);
			t.SetEncLv(Mathf.Min(EClass.rnd(EClass.rnd(Mathf.Max(5 + EClass.pc.Evalue(258) - cardRow.LV, 1))), 12));
			t = CraftUtil.MixIngredients(t, ai.ings, CraftUtil.MixType.General, 999, EClass.pc).Thing;
			break;
		}
		case MixType.RuneMold:
		{
			Thing eq = ai.ings[0];
			Thing thing7 = eq.Duplicate(1);
			thing7.SetEncLv(0);
			List<Element> list = thing7.elements.ListRune();
			if (list.Count == 0)
			{
				Msg.SayNothingHappen();
				break;
			}
			foreach (Element item in list)
			{
				SocketData runeEnc = eq.GetRuneEnc(item.id);
				item.vLink = 0;
				if (runeEnc != null)
				{
					if (item.vBase + item.vSource != runeEnc.value)
					{
						item.vLink = item.vBase + item.vSource;
					}
					item.vBase = runeEnc.value;
					item.vSource = 0;
				}
			}
			if (eq.material.hardness > owner.material.hardness && !EClass.debug.enable)
			{
				Msg.Say("rune_tooHard", owner);
				break;
			}
			EClass.ui.AddLayer<LayerList>().SetList2(list, (Element a) => a.Name, delegate(Element a, ItemGeneral b)
			{
				owner.ModNum(-1);
				eq.Destroy();
				Thing thing8 = ThingGen.Create("rune");
				thing8.ChangeMaterial(owner.material);
				thing8.refVal = a.id;
				thing8.encLV = a.vBase + a.vSource;
				EClass.pc.Pick(thing8);
				EClass.pc.PlaySound("intonation");
				EClass.pc.PlayEffect("intonation");
			}, delegate(Element a, ItemGeneral b)
			{
				b.SetSubText(a.vBase + a.vSource + ((a.vLink != 0) ? (" (" + a.vLink + ")") : ""), 200, FontColor.Default, TextAnchor.MiddleRight);
				b.Build();
				if (a.HasTag("noRune"))
				{
					b.button1.interactable = false;
					b.button1.mainText.gameObject.AddComponent<CanvasGroup>().alpha = 0.5f;
				}
			}).SetSize(500f)
				.SetOnKill(delegate
				{
				})
				.SetTitles("wRuneMold");
			break;
		}
		case MixType.Talisman:
		{
			int num4 = EClass.pc.Evalue(1418);
			Thing thing5 = ai.ings[1];
			SourceElement.Row source2 = (thing5.trait as TraitSpellbook).source;
			int num5 = thing5.c_charges * source2.charge * (100 + num4 * 50) / 500 + 1;
			int num6 = 100;
			Thing thing6 = ThingGen.Create("talisman").SetNum(num5);
			thing6.refVal = source2.id;
			thing6.encLV = num6 * (100 + num4 * 10) / 100;
			thing.ammoData = thing6;
			thing.c_ammo = num5;
			EClass.pc.Say("talisman", thing, thing6);
			thing5.Destroy();
			break;
		}
		case MixType.Scratch:
			claimed = false;
			Prize(20, "medal", "save", cat: false);
			Prize(10, "plat", "save", cat: false);
			Prize(10, "furniture", "nice", cat: true);
			Prize(4, "plamo_box", "nice", cat: false);
			Prize(4, "food", "", cat: false);
			Prize(1, "casino_coin", "", cat: false);
			break;
		case MixType.Fortune:
		{
			EClass.player.seedFortune++;
			int num2 = 0;
			FortuneRollData orCreateFortuneRollData = EClass._zone.GetOrCreateFortuneRollData(refresh: false);
			int seed = orCreateFortuneRollData.seed + orCreateFortuneRollData.count + EClass.player.seedFortune;
			Rand.SetSeed(seed);
			for (int num3 = 3; num3 > 0; num3--)
			{
				if (EClass.rnd(FortuneRollData.chances[num3]) == 0)
				{
					num2 = num3;
					break;
				}
			}
			Rand.SetSeed();
			if (num2 != 0)
			{
				owner.PlaySound((num2 == 3) ? "fortuneroll_winBig" : "fortuneroll_win");
			}
			Thing thing4 = ThingGen.Create("fortune_ball");
			thing4.ChangeMaterial(FortuneRollData.mats[num2]);
			EClass._zone.AddCard(thing4, owner.pos);
			owner.PlaySound("fortuneroll_ball");
			orCreateFortuneRollData.GetPrize(num2, seed);
			if ((bool)LayerDragGrid.Instance)
			{
				LayerDragGrid.Instance.info.Refresh();
			}
			break;
		}
		case MixType.Incubator:
			TraitFoodEggFertilized.Incubate(ai.ings[0], owner.pos, owner);
			break;
		default:
			t = ThingGen.Create(thing3);
			if (thing3 == "gene")
			{
				if (ai.ings[0].c_DNA != null)
				{
					t.MakeRefFrom(ai.ings[0].c_idRefCard);
					t.c_DNA = ai.ings[0].c_DNA;
					t.c_DNA.GenerateWithGene(DNA.Type.Inferior, t);
				}
			}
			else
			{
				t = CraftUtil.MixIngredients(t, ai.ings, CraftUtil.MixType.General, 999, EClass.pc).Thing;
			}
			break;
		}
		if (t != null)
		{
			if (t.HasElement(1229))
			{
				num = 1;
			}
			if (t.HasElement(704))
			{
				num = 1;
			}
			if (t.HasElement(703))
			{
				num = 1;
			}
			t.SetNum(num);
		}
		return t;
		void Prize(int chance, string s, string col, bool cat)
		{
			if (!claimed && EClass.rnd(chance) == 0)
			{
				t = (cat ? ThingGen.CreateFromCategory(s, EClass.pc.LV) : ThingGen.Create(s, -1, EClass.pc.LV));
				claimed = true;
				if (col != "")
				{
					Msg.SetColor(col);
				}
			}
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (IsFactory)
		{
			Thing _t = owner.Thing;
			p.TrySetAct("craft", delegate
			{
				if (EClass.player.recipes.ListSources(_t).Count > 0)
				{
					EClass.ui.AddLayer<LayerCraft>().SetFactory(_t);
				}
				else
				{
					Msg.Say("noRecipes");
				}
				return false;
			}, _t, CursorSystem.Craft);
		}
		else if (!(this is TraitIncubator) || EClass._zone.IsPCFaction)
		{
			p.TrySetAct(CrafterTitle, delegate
			{
				LayerDragGrid.CreateCraft(this);
				return false;
			}, owner);
		}
	}

	public override bool CanUse(Chara c)
	{
		return CanUseFromInventory;
	}

	public override bool OnUse(Chara c)
	{
		if (EClass._zone.IsRegion)
		{
			Msg.SayCannotUseHere();
			return false;
		}
		if (IsFactory)
		{
			Thing thing = owner.Thing;
			if (EClass.player.recipes.ListSources(thing).Count > 0)
			{
				EClass.ui.AddLayer<LayerCraft>().SetFactory(thing);
			}
			else
			{
				Msg.Say("noRecipes");
			}
			return false;
		}
		LayerDragGrid.CreateCraft(this);
		return false;
	}

	public virtual void OnEndAI(AI_UseCrafter ai)
	{
	}
}
