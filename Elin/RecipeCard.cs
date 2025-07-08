using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class RecipeCard : Recipe
{
	public CardRow _sourceCard;

	[JsonProperty]
	public bool freePos;

	[JsonProperty]
	public float fx;

	[JsonProperty]
	public float fy;

	public Card _mold;

	public CardRow sourceCard => _sourceCard ?? (_sourceCard = EClass.sources.cards.map.TryGetValue(base.source.row.idString));

	public override RenderRow renderRow => sourceCard;

	public override TileType tileType => Mold.trait.tileType;

	public string idCard => sourceCard.id;

	public override bool CanRotate => tileType.CanRotate(buildMode: true);

	public override bool IsThing => true;

	public override Card Mold => _mold ?? (_mold = CreateMold());

	public bool isDish => sourceCard._origin == "dish";

	public override IRenderer GetRenderer()
	{
		return Mold.renderer;
	}

	public override string GetDetail()
	{
		return sourceCard.GetDetail();
	}

	public override string GetIdThing()
	{
		return id;
	}

	public override void OnRenderMarker(Point point, bool active, HitResult result, bool main, int dir, int bridgeHeight)
	{
		Mold.ignoreStackHeight = Input.GetKey(KeyCode.LeftControl);
		Mold.SetFreePos(point);
		freePos = Mold.freePos;
		fx = Mold.fx;
		fy = Mold.fy;
		Mold.RenderMarker(point, active, result, main, dir);
		if (!point.cell.skipRender)
		{
			Mold.trait.OnRenderTile(point, result, dir);
		}
	}

	public override void BuildIngredientList()
	{
		if (ingredients.Count != 0)
		{
			return;
		}
		base.BuildIngredientList();
		if (!EClass.core.IsGameStarted || EClass.core.game.isLoading || !isDish || EClass.pc.Evalue(1650) < 2)
		{
			return;
		}
		bool flag = ingredients.Count < 3;
		foreach (Ingredient ingredient in ingredients)
		{
			if (ingredient.id == "seasoning")
			{
				flag = false;
			}
		}
		if (flag)
		{
			ingredients.Add(new Ingredient
			{
				id = "seasoning",
				useCat = true,
				optional = true,
				req = 1
			});
		}
	}

	public override void OnChangeIngredient()
	{
		if (!UseStock)
		{
			Mold.ChangeMaterial(GetMainMaterial());
			if (base.source.colorIng != 0)
			{
				Mold.Dye(GetColorMaterial());
			}
		}
	}

	public override void OnSelected()
	{
		CreateMold();
	}

	public Card CreateMold()
	{
		if (_mold != null)
		{
			_mold.Destroy();
		}
		if (UseStock)
		{
			ingredients[0].RefreshThing();
		}
		if (UseStock && ingredients[0].thing != null)
		{
			_mold = ingredients[0].thing.Duplicate(1);
		}
		else
		{
			if (sourceCard.isChara)
			{
				_mold = CharaGen.Create(idCard);
			}
			else
			{
				_mold = ThingGen.Create(idCard);
			}
			OnChangeIngredient();
		}
		_mold.dir = _dir;
		_mold.altitude = ActionMode.Build.altitude;
		_mold.idSkin = idSkin;
		return _mold;
	}

	public override Thing Craft(BlessedState blessed, bool sound = false, List<Thing> ings = null, TraitCrafter crafter = null, bool model = false)
	{
		string key = idCard;
		int num = GetMainMaterial().id;
		Element reqSkill = base.source.GetReqSkill();
		int num2 = reqSkill.Value - EClass.pc.Evalue(reqSkill.id);
		int num3 = GetQualityBonus();
		int num4 = renderRow.LV + num3;
		bool flag = num3 < 0;
		CardRow cardRow = EClass.sources.cards.map.TryGetValue(key);
		bool flag2 = false;
		if (ings != null)
		{
			foreach (Thing ing in ings)
			{
				if (ing != null && ing.IsDecayed)
				{
					flag2 = true;
				}
			}
		}
		if (EClass.debug.godCraft || model)
		{
			flag = false;
		}
		else if (cardRow != null && cardRow.tag.Contains("no_fail"))
		{
			flag = false;
		}
		else if (isDish && !EClass.scene.actionMode.IsBuildMode)
		{
			if (num2 > 0 && EClass.rnd(num2 * 10) > EClass.rnd(100))
			{
				key = GetIdFailDish();
				flag = true;
			}
			else
			{
				flag = false;
			}
		}
		if (!model)
		{
			switch (id)
			{
			case "weapon_stone":
			case "weapon_wood":
				Rand.SetSeed();
				key = new string[6] { "dagger", "sword", "axe_hand", "blunt_club", "spear", "staff_long" }.RandomItem();
				break;
			case "weapon_anvil":
				Rand.SetSeed();
				key = new string[6] { "dagger", "sword", "axe_hand", "blunt_club", "spear", "staff_long" }.RandomItem();
				num = ings[1].material.id;
				break;
			}
		}
		bool flag3 = EClass.sources.cards.map[key].tag.Contains("static_craft");
		if (!isDish && num4 < 1)
		{
			num4 = 1;
		}
		if (EClass.sources.cards.map[key].tag.Contains("noQuality"))
		{
			num4 = -1;
		}
		if (EClass.sources.cards.map[key].tag.Contains("noMaterialChange"))
		{
			num = -1;
		}
		if (flag3)
		{
			num3 = 0;
			flag = false;
		}
		CardBlueprint.Set(new CardBlueprint
		{
			qualityBonus = num3,
			rarity = (flag ? Rarity.Crude : Rarity.Normal)
		});
		Thing thing = (flag3 ? ThingGen.Create(key) : ThingGen.Create(key, num, num4));
		thing.idSkin = idSkin;
		if (thing.IsEquipment && ings != null)
		{
			foreach (Thing ing2 in ings)
			{
				if (ing2.trait is TraitRune)
				{
					thing.AddRune(ing2);
					if (!thing.HasElement(484))
					{
						thing.elements.SetBase(484, 1);
					}
				}
			}
		}
		thing.Identify(show: false);
		thing.isCrafted = true;
		if (!flag3)
		{
			if (base.source.colorIng != 0)
			{
				thing.Dye(GetColorMaterial());
			}
			if (thing.IsContainer)
			{
				thing.RemoveThings();
				thing.c_lockLv = 0;
			}
		}
		thing.SetBlessedState(blessed);
		if (!flag3)
		{
			if (isDish)
			{
				if (flag)
				{
					thing.ChangeMaterial("meat");
				}
				else
				{
					MakeDish(thing);
				}
			}
			else
			{
				MixIngredients(thing);
			}
			if (isDish && flag2)
			{
				thing.decay = thing.MaxDecay + 1;
			}
		}
		thing.trait.OnCrafted(this, ings);
		if (thing.IsAmmo && num2 < 0)
		{
			thing.SetEncLv(-num2 / 10);
		}
		if (model)
		{
			thing.SetNum(1);
			return thing;
		}
		int num5 = thing.trait.CraftNum;
		if (crafter != null && EClass.pc.Evalue(1417) > 0 && crafter.WitchDoubleCraftChance(thing) > EClass.rnd(100))
		{
			num5 *= 2;
		}
		if (thing.trait is TraitFoodFishSlice && thing.c_idRefCard == thing.id && (thing.SelfWeight == 0 || thing.Evalue(10) == 0))
		{
			Msg.Say("noMoreSlice");
			num5 = 1;
		}
		if (num5 > 1 && (thing.HasElement(703) || thing.HasElement(704) || thing.HasElement(1229)))
		{
			num5 = 1;
		}
		if (num5 > 1)
		{
			thing.SetNum(num5);
		}
		if (EClass.pc.held == null || !thing.TryStackTo(EClass.pc.held.Thing))
		{
			EClass.pc.HoldCard(thing);
		}
		if (sound)
		{
			thing.PlaySoundDrop(spatial: false);
		}
		Msg.Say("crafted", thing);
		if (thing.Num > EClass.rnd(1000) || EClass.debug.enable)
		{
			EClass.player.recipes.ComeUpWithRandomRecipe(thing.category.id);
		}
		if (isDish)
		{
			if (EClass.debug.enable || (EClass.player.flags.canComupWithFoodRecipe && EClass.rnd(30) == 0))
			{
				EClass.player.recipes.ComeUpWithRandomRecipe(thing.category.id);
				EClass.player.flags.canComupWithFoodRecipe = false;
			}
			if (flag && crafter != null && crafter.CanTriggerFire && EClass.rnd(4) == 0)
			{
				Point point = (crafter.ExistsOnMap ? crafter.owner.pos : EClass.pc.pos);
				if (!point.cell.HasFire)
				{
					EClass._map.ModFire(point.x, point.z, 10);
				}
			}
		}
		return thing;
	}

	public void MakeDish(Thing t)
	{
		Rand.SetSeed(EClass.pc.turn);
		List<Thing> list = new List<Thing>();
		foreach (Ingredient ingredient in ingredients)
		{
			list.Add(ingredient.thing);
		}
		CraftUtil.MakeDish(t, list, GetQualityBonus(), EClass.pc);
		Rand.SetSeed();
		if (EClass.pc.HasElement(1658))
		{
			t.elements.SetBase(757, 1);
			t.c_dateCooked = EClass.world.date.GetRaw() + (EClass.pc.Evalue(1658) - 1) * 48 * 60;
		}
	}

	public void MixIngredients(Thing t)
	{
		Rand.SetSeed(EClass.pc.turn);
		List<Thing> list = new List<Thing>();
		foreach (Ingredient ingredient in ingredients)
		{
			list.Add(ingredient.thing);
		}
		CraftUtil.MixIngredients(t, list, CraftUtil.MixType.General, GetQualityBonus());
		Rand.SetSeed();
	}

	public string GetIdFailDish()
	{
		return sourceCard.category switch
		{
			"meal_meat" => "fail_meat", 
			"meal_fish" => "fail_fish", 
			"meal_vegi" => "fail_vegi", 
			"meal_fruit" => "fail_fruit", 
			"meal_cake" => "fail_dough_cake", 
			"meal_bread" => "fail_dough_bread", 
			"meal_noodle" => "fail_noodle", 
			"meal_egg" => "fail_egg", 
			"meal_rice" => "fail_rice", 
			"meal_soup" => "fail_drink", 
			_ => "fail_dish", 
		};
	}

	public override void Build(TaskBuild task)
	{
		Card card = null;
		if (task.target != null)
		{
			card = task.target;
		}
		else if (IngAsProduct)
		{
			if (task.resources.Count == 0)
			{
				Thing thing = ingredients[0].RefreshThing();
				if (thing == null)
				{
					SE.Beep();
					return;
				}
				Thing item = thing.Split(1);
				task.resources.Add(item);
				if (ingredients[0].thing.isDestroyed || ingredients[0].thing.ExistsOnMap)
				{
					ingredients[0].thing = null;
				}
			}
			card = task.resources[0];
		}
		else if (sourceCard.isChara)
		{
			card = CharaGen.Create(idCard, Mathf.Max(EClass._zone.DangerLv, EClass.pc.LV));
		}
		else
		{
			card = ThingGen.Create(idCard, -1, Mathf.Max(EClass._zone.DangerLv, EClass.pc.LV));
			if (!card.IsUnique)
			{
				card.ChangeMaterial(GetMainMaterial());
			}
			if (base.source.colorIng != 0)
			{
				card.Dye(GetColorMaterial());
			}
			if (card.IsContainer)
			{
				card.RemoveThings();
			}
			foreach (Ingredient ingredient in ingredients)
			{
				if (ingredient.thing != null && ingredient.thing.HasElement(759))
				{
					card.elements.SetBase(759, ingredient.thing.Evalue(759));
				}
			}
		}
		Build(task.owner, card, task.pos, ingredients[0].mat, task.dir, task.altitude, task.bridgeHeight);
		card.renderer.PlayAnime(AnimeID.Place);
	}

	public override void Build(Chara chara, Card t, Point pos, int mat, int dir, int altitude, int bridgeHeight)
	{
		if (mat == -1)
		{
			mat = 2;
		}
		EClass.pc.PlaySound(EClass.sources.materials.rows[mat].GetSoundImpact());
		t.SetDir(dir);
		t.idSkin = idSkin;
		EClass._zone.AddCard(t, pos);
		if (t.trait is TraitHouseBoard && ActionMode.Build.houseBoard != null)
		{
			(t.trait as TraitHouseBoard).data = IO.DeepCopy(ActionMode.Build.houseBoard.data);
		}
		t.SetPlaceState(PlaceState.installed, byPlayer: true);
		t.altitude = altitude;
		t.isPlayerCreation = true;
		if (EClass._zone.idCurrentSubset != null)
		{
			t.isSubsetCard = true;
		}
		t.ignoreStackHeight = Input.GetKey(KeyCode.LeftControl);
		if (ActionMode.Build.IsActive && t.trait.MaskOnBuild && EClass.debug.enable)
		{
			t.isMasked = true;
		}
		if (EClass.scene.actionMode.IsRoofEditMode(t) && t.isThing)
		{
			t.isRoofItem = true;
			t.SetPlaceState(PlaceState.roaming);
		}
		try
		{
			t.ForeachPoint(delegate(Point p, bool main)
			{
				CheckBlock(p);
			});
		}
		catch
		{
			Debug.LogWarning("kokodayo");
		}
		t.freePos = freePos;
		if (freePos)
		{
			t.fx = fx;
			t.fy = fy;
		}
		t.renderer.RefreshSprite();
		void CheckBlock(Point _pos)
		{
			if (_pos.cell.IsBlocked && _pos.HasChara)
			{
				foreach (Chara item in _pos.ListCharas().Copy())
				{
					chara.Kick(item, ignoreSelf: false, karmaLoss: false);
				}
			}
		}
	}

	public override void OnChangeAltitude(int a)
	{
		Mold.altitude = a;
	}

	public override void Rotate()
	{
		Mold.Rotate();
		_dir = Mold.dir;
		if ((bool)BuildMenu.Instance)
		{
			BuildMenu.Instance.info1.OnRotate();
		}
	}

	public override void SetDir(int d)
	{
		Mold.dir = d;
		base.SetDir(d);
	}

	public override void WriteNote(UINote n)
	{
		n.Clear();
		if (!Mold.isChara)
		{
			Mold.elements.AddNote(n);
		}
		n.Build();
	}

	public override Recipe Duplicate()
	{
		RecipeCard recipeCard = IO.DeepCopy(this);
		recipeCard._mold = _mold;
		return recipeCard;
	}
}
