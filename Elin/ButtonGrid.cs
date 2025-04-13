using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGrid : UIButton, IMouseHint, IPrefImage
{
	public enum Mode
	{
		Default,
		Grid,
		Ingredient,
		Recipe,
		RecipeGrid,
		Search
	}

	public Area area;

	public Recipe recipe;

	public Recipe.Ingredient ing;

	public Card card;

	public int index;

	public InvOwner invOwner;

	public bool keepBg;

	[NonSerialized]
	public List<Transform> listAttach = new List<Transform>();

	private int rightAttachCount;

	public ColorProfile Colors => Core.Instance.Colors;

	public Card Card
	{
		get
		{
			if (card == null || card.isDestroyed)
			{
				return null;
			}
			return card;
		}
	}

	public virtual float extFixY => 0f;

	public override string ToString()
	{
		return card?.ToString();
	}

	public T Attach<T>(string id, bool rightAttach = true) where T : Component
	{
		return Attach(id, rightAttach).GetComponent<T>();
	}

	public Transform Attach(string id, bool rightAttach = true)
	{
		Transform transform = PoolManager.Spawn<Transform>("attach_" + id, "UI/Element/Grid/Attach/" + id, base.transform);
		listAttach.Add(transform);
		if (rightAttach)
		{
			RectTransform rectTransform = transform.Rect();
			rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -8 - 8 * rightAttachCount);
			transform.tag = "RightAttach";
			rightAttachCount++;
		}
		transform.name = id;
		return transform;
	}

	public void Dettach(string id)
	{
		listAttach.ForeachReverse(delegate(Transform t)
		{
			if (t.name == id)
			{
				if (t.tag.Contains("RightAttach"))
				{
					rightAttachCount--;
				}
				listAttach.Remove(t);
				PoolManager.Despawn(t);
			}
		});
	}

	public void Reset()
	{
		base.image.color = Color.white;
		if (listAttach.Count > 0)
		{
			foreach (Transform item in listAttach)
			{
				PoolManager.Despawn(item);
			}
			listAttach.Clear();
		}
		if ((bool)icon)
		{
			icon.enabled = true;
		}
		rightAttachCount = 0;
	}

	protected override void OnDestroy()
	{
		if (listAttach.Count > 0)
		{
			Reset();
		}
		rightAttachCount = 0;
	}

	public void SetBodySlot(BodySlot b, InvOwner owner, bool showIndex = false)
	{
		SetCardGrid(b.thing, owner);
		imageCheck.sprite = SpriteSheet.Get("Media/Graphics/Icon/Element/", "eq_" + b.element.alias);
		imageCheck.SetNativeSize();
		imageCheck.SetAlpha((b.thing == null) ? 1f : 0.4f);
		if ((bool)subText)
		{
			if (showIndex)
			{
				owner.owner.Chara.body.SetBodyIndexText(b, subText);
			}
			else
			{
				subText.SetActive(enable: false);
			}
		}
	}

	public void Redraw()
	{
		SetCardGrid(card, invOwner);
	}

	public void SetCardGrid(Card c, InvOwner owner = null)
	{
		if (owner != null)
		{
			invOwner = owner;
		}
		if (c != null && !c.isDestroyed)
		{
			base.interactable = true;
		}
		SetCard(c, Mode.Grid, delegate(UINote n)
		{
			invOwner?.OnWriteNote(this, n);
		});
		onRightClick = delegate
		{
			if (EClass.ui.AllowInventoryInteractions && invOwner != null)
			{
				InvOwner.clickTimer = 0f;
				invOwner.OnRightClick(this);
				UIButton.onPressed = delegate
				{
					invOwner.OnRightPressed(this);
				};
				EInput.Consume();
				UIButton.TryShowTip();
			}
		};
		this.SetOnClick(delegate
		{
			if (EClass.ui.AllowInventoryInteractions)
			{
				invOwner?.OnClick(this);
				EInput.Consume();
				UIButton.TryShowTip();
			}
		});
	}

	public void SetCard(Card c, Mode mode = Mode.Default, Action<UINote> onWriteNote = null)
	{
		Reset();
		CoreRef.ButtonAssets refs = EClass.core.refs.buttonAssets;
		card = c;
		bool showNew = invOwner?.ShowNew ?? false;
		if (c == null || c.isDestroyed)
		{
			if (mode != Mode.Ingredient)
			{
				RefreshBG();
				icon.sprite = EClass.core.refs.icons.trans;
				mainText.text = "";
				if ((bool)subText)
				{
					subText.text = "";
				}
			}
			else
			{
				icon.sprite = Core.Instance.refs.spriteNoIng;
			}
			icon.SetNativeSize();
			if (invOwner == null || !invOwner.AlwaysShowTooltip || onWriteNote == null)
			{
				SetTooltip(null, enable: false);
				return;
			}
			SetTooltip("note", delegate(UITooltip t)
			{
				onWriteNote(t.note);
			});
			return;
		}
		SetTooltip("note", delegate(UITooltip t)
		{
			if (c.isNew)
			{
				c.isNew = false;
				RefreshBG();
			}
			c.WriteNote(t.note, onWriteNote);
			if (ing != null)
			{
				t.note.Space();
				t.note.AddText("mark_ing".lang() + (ing.optional ? "opIngredient" : "reqIng").lang(ing.GetName()));
			}
		});
		c.SetImage(icon);
		if (c == EClass.pc.held && c.invY != 1)
		{
			Attach("held");
		}
		if (c.trait is TraitAbility && (c.trait as TraitAbility).CanUse(EClass.pc))
		{
			Attach<Image>("target_self").sprite = (c.trait as TraitAbility).act.TargetType.IconType;
		}
		if (EClass.core.config.test.showRefIcon && c.IsIdentified)
		{
			SourceElement.Row refElement = c.trait.GetRefElement();
			if (refElement != null)
			{
				if ((bool)refElement.GetSprite())
				{
					Image image = Attach<Image>("reficon", rightAttach: false);
					refElement.SetImage(image);
				}
			}
			else
			{
				Sprite refSprite = c.trait.GetRefSprite();
				if ((bool)refSprite)
				{
					Image obj = Attach<Image>("reficon", rightAttach: false);
					obj.sprite = refSprite;
					obj.color = Color.white;
				}
			}
		}
		if (c.qualityTier > 0)
		{
			Attach<Image>("quality", rightAttach: false).sprite = EClass.core.refs.icons.quality[Mathf.Clamp(c.qualityTier - 1, 0, EClass.core.refs.icons.quality.Count - 1)];
		}
		if (c.c_equippedSlot != 0 && invOwner != null && invOwner.owner.isChara && !invOwner.owner.IsPC)
		{
			Attach("equip", rightAttach: false);
		}
		if (c.isNPCProperty || c.isGifted)
		{
			Attach("npcProperty");
		}
		if (mode != Mode.Search)
		{
			c.trait.OnSetCardGrid(this);
		}
		switch (mode)
		{
		case Mode.Grid:
		case Mode.Search:
			RefreshBG();
			if (c.IsContainer && c.c_indexContainerIcon != 0)
			{
				Attach("icon_container", rightAttach: false).GetComponent<Image>().sprite = EClass.core.refs.spritesContainerIcon[c.c_indexContainerIcon];
			}
			if (c.IsIdentified)
			{
				BlessedState blessedState2 = c.blessedState;
				if (blessedState2 != 0)
				{
					Attach("status_" + blessedState2);
				}
			}
			else
			{
				Attach("status_unidentified");
			}
			if (c.IsDecayed)
			{
				Attach("rotten");
			}
			else if (c.IsRotting)
			{
				Attach("rotting");
			}
			c.trait.SetMainText(mainText, hotitem: false);
			if (mode == Mode.Search)
			{
				Card rootCard = c.GetRootCard();
				if (rootCard != EClass.pc)
				{
					Attach((rootCard == c) ? "searched" : "searched_container", rightAttach: false);
				}
			}
			break;
		case Mode.Default:
		{
			Color c2 = Colors.Skin.textUnidentified;
			if (c.IsIdentified)
			{
				c2 = c.blessedState switch
				{
					BlessedState.Blessed => Colors.Skin.textBlessed, 
					BlessedState.Doomed => Colors.Skin.textDoomed, 
					BlessedState.Cursed => Colors.Skin.textCursed, 
					_ => Colors.Skin.textIdentified, 
				};
			}
			mainText.SetText(c.Name, c2);
			if ((bool)subText)
			{
				subText.SetText(Lang._weight(c.ChildrenAndSelfWeight));
			}
			break;
		}
		case Mode.Ingredient:
			if (c.IsIdentified)
			{
				BlessedState blessedState = c.blessedState;
				if (blessedState != 0)
				{
					Attach("status_" + blessedState);
				}
			}
			else
			{
				Attach("status_unidentified");
			}
			if (c.IsDecayed)
			{
				Attach("rotten");
			}
			else if (c.IsRotting)
			{
				Attach("rotting");
			}
			break;
		}
		if (c.c_isImportant)
		{
			Attach("important");
		}
		void RefreshBG()
		{
			if (!keepBg)
			{
				if (c == null)
				{
					base.image.sprite = refs.bgDefault;
				}
				else
				{
					Sprite sprite = refs.bgDefault;
					if (c.rarity >= Rarity.Superior && c.IsIdentified && c.IsEquipmentOrRangedOrAmmo)
					{
						sprite = ((c.rarity >= Rarity.Artifact) ? refs.bgArtifact : ((c.rarity >= Rarity.Mythical) ? refs.bgMythical : ((c.rarity < Rarity.Legendary) ? refs.bgSuperior : refs.bgLegendary)));
					}
					else if (showNew && c.isNew && c.invY != 1)
					{
						sprite = refs.bgNew;
					}
					base.image.sprite = sprite;
				}
			}
		}
	}

	public void SetArea(Area a)
	{
		Reset();
		icon.sprite = SpriteSheet.Get("icon_area");
		icon.SetNativeSize();
		mainText.SetActive(enable: false);
	}

	public void SetItem(GridItem i)
	{
		Reset();
		mainText.SetActive(enable: true);
		i.SetButton(this);
	}

	public void SetObject(object o)
	{
		if (o is Area)
		{
			SetArea(o as Area);
		}
		else if (o is Card)
		{
			SetCard(o as Card);
		}
	}

	public void SetDummy()
	{
		Reset();
		icon.enabled = false;
		mainText.SetActive(enable: false);
		base.onClick.RemoveAllListeners();
		base.onClick.AddListener(delegate
		{
			SE.BeepSmall();
		});
		tooltip.enable = false;
		base.image.color = new Color(1f, 1f, 1f, 0.3f);
	}

	public virtual void SetDragParent(IDragParent p)
	{
	}

	public void SetIngredient(Recipe r, Recipe.Ingredient _ing)
	{
		if (_ing.id.IsEmpty())
		{
			SetDummy();
			return;
		}
		mainText.SetActive(enable: true);
		base.image.color = Color.white;
		ing = _ing;
		if (ing.thing != null && (ing.thing.isDestroyed || ing.thing.Num == 0))
		{
			ing.thing = null;
		}
		Thing thing = ing.thing;
		SetCard(thing, Mode.Ingredient);
		if (r != null && r.source.colorIng != 0 && r.ingredients != null && r.ingredients.Count > 0 && r.ingredients.IndexOf(_ing) == r.source.colorIng)
		{
			Attach("ing_color", rightAttach: false);
		}
		if (ing.optional)
		{
			UIButton componentInChildren = Attach("ing_option", rightAttach: false).GetComponentInChildren<UIButton>(includeInactive: true);
			componentInChildren.SetActive(ing.thing != null);
			componentInChildren.SetOnClick(delegate
			{
				SE.Trash();
				ing.thing = null;
				SetIngredient(r, _ing);
				r.SaveLastIngredients();
				r.OnChangeIngredient();
				if ((bool)LayerCraft.Instance)
				{
					LayerCraft.Instance.OnChangeIngredient();
				}
			});
		}
		HitSummary summary = Core.Instance.screen.tileSelector.summary;
		int num = thing?.Num ?? 0;
		int num2 = summary.countValid;
		if (num2 == 0)
		{
			num2 = 1;
		}
		num2 *= ing.req;
		if (ing.optional && num == 0)
		{
			num2 = 0;
		}
		bool flag = num < num2 && !ing.optional;
		string text = (num.ToString() ?? "").TagColor(Colors.colorIngPredict) + (" -" + num2).TagColor(flag ? Colors.colorIngCost : Colors.colorIngReq);
		mainText.text = text;
		if (thing == null)
		{
			SetTooltip("note", delegate(UITooltip tt)
			{
				tt.note.Clear();
				tt.note.Space(16);
				tt.note.AddText("mark_ing".lang() + (ing.optional ? "opIngredient" : "noIngredient").lang(_ing.GetName()), ing.optional ? FontColor.Good : FontColor.Bad);
				tt.note.Build();
			});
			EClass.sources.cards.map[ing.IdThing].SetImage(icon);
			icon.SetAlpha(0.5f);
		}
		else if (thing.GetRootCard() != EClass.pc)
		{
			Attach("pcInv", rightAttach: false);
		}
	}

	public void SetIngredient(Recipe r, Thing t)
	{
		SetCard(t, Mode.Ingredient);
		int num = t.Num;
		mainText.text = num.ToString() ?? "";
		mainText.SetActive(enable: true);
		ing = null;
		if (t.GetRootCard() != EClass.pc)
		{
			Attach("pcInv", rightAttach: false);
		}
	}

	public void SetRecipeVariation(UIRecipeInfo.RecipeVariation a)
	{
		Reset();
		Recipe recipe = a.recipe;
		if (recipe.UseStock && !recipe.VirtualBlock && recipe.ingredients[0].thing != null)
		{
			recipe.ingredients[0].thing.SetImage(icon, a.dir, recipe.ingredients[0].thing.trait.IdSkin);
		}
		else
		{
			recipe.renderRow.SetImage(icon, null, recipe.renderRow.GetColorInt(recipe.GetColorMaterial()), setNativeSize: true, (recipe.renderRow is SourceChara.Row) ? recipe.idSkin : a.dir, (recipe.renderRow is SourceChara.Row) ? a.dir : recipe.idSkin);
		}
		mainText.SetActive(enable: false);
	}

	public void SetRecipe()
	{
		SetRecipe(recipe);
	}

	public void SetRecipe(Area a)
	{
		Reset();
		area = a;
		icon.sprite = Core.Instance.refs.spriteArea;
		icon.SetNativeSize();
		mainText.SetActive(enable: false);
	}

	public void SetRecipe(Recipe r)
	{
		Reset();
		recipe = r;
		if (r.UseStock)
		{
			recipe.ingredients[0].RefreshThing();
			Thing thing = recipe.ingredients[0].thing;
			mainText.SetText(thing.Num.ToString() ?? "", SkinManager.CurrentColors.buttonGrid);
			if (r.VirtualBlock)
			{
				r.renderRow.SetImage(icon, null, r.GetDefaultColor(), setNativeSize: true, 0, thing.trait.IdSkin);
			}
			else
			{
				thing.SetImage(icon);
			}
		}
		else
		{
			r.renderRow.SetImage(icon, null, r.GetDefaultColor(), setNativeSize: true, 0, r.idSkin);
		}
		mainText.SetActive(r.UseStock);
		if (r.UseStock && r.ingredients[0].thing?.GetRootCard() != EClass.pc)
		{
			Attach("pcInv", rightAttach: false);
		}
	}

	public void RefreshCraftable()
	{
		ThingStack thingStack = EClass.pc.things.GetThingStack(recipe.GetIdThing(), recipe.GetRefVal());
		bool flag = recipe.IsCraftable();
		string text = recipe.Name;
		if (recipe.Mold != null && recipe.Mold.trait.CraftNum > 1)
		{
			text = text + " x " + recipe.Mold.trait.CraftNum;
		}
		mainText.SetText(text, flag ? FontColor.Good : FontColor.Bad);
		subText.text = thingStack.count.ToString() ?? "";
		recipe.SetTextDifficulty(subText2);
	}

	public void SetCraftRecipe(Recipe r, Mode mode, bool tooltip = false)
	{
		Reset();
		recipe = r;
		if (mode == Mode.Recipe)
		{
			RefreshCraftable();
		}
		else
		{
			mainText.SetActive(enable: false);
		}
		RecipeCard rCard = r as RecipeCard;
		if (rCard != null)
		{
			r.SetImage(icon);
		}
		else
		{
			r.SetImage(icon);
		}
		if (!tooltip)
		{
			return;
		}
		SetTooltip("note", delegate(UITooltip t)
		{
			if (r.ingredients.Count == 0)
			{
				r.BuildIngredientList();
			}
			EClass.player.recipes.hoveredRecipes.Add(r.id);
			Dettach("recipe_new");
			if (rCard != null)
			{
				if (mode == Mode.RecipeGrid && rCard._mold == null)
				{
					CardBlueprint.SetNormalRarity();
					rCard._mold = ThingGen.Create(r.id);
				}
				rCard.Mold.WriteNote(t.note, delegate(UINote n)
				{
					n.Space();
					WriteReqMat(n, r);
				}, IInspect.NoteMode.Product, r);
			}
			else
			{
				UINote note = t.note;
				note.Clear();
				note.AddHeaderCard(r.GetName(), icon.sprite);
				if (!r.GetDetail().IsEmpty())
				{
					note.AddText(r.GetDetail());
				}
				WriteReqMat(note, r);
				t.note.Build();
			}
		});
	}

	public void WriteReqMat(UINote n, Recipe r)
	{
		r.WriteReqFactory(n);
		r.WriteReqSkill(n);
		n.AddHeaderTopic("reqMat");
		foreach (Recipe.Ingredient ingredient in r.ingredients)
		{
			int count = EClass._map.Stocked.ListThingStack(ingredient, StockSearchMode.AroundPC).count;
			Color c = ((count >= ingredient.req) ? SkinManager.CurrentColors.textGood : SkinManager.CurrentColors.textBad);
			string text = "<color=" + c.ToHex() + ">" + ingredient.GetName() + " x " + ingredient.req + "  (" + count + ")</color>";
			if (ingredient.optional)
			{
				text = "opIngredient".lang(text);
			}
			n.AddText(text).text1.SetSize(-1);
		}
	}

	public bool ShowMouseHintLeft()
	{
		if (invOwner != null && card != null)
		{
			if (!invOwner.CanShiftClick(this) && !invOwner.CanCtrlClick(this))
			{
				return invOwner.CanAltClick(this);
			}
			return true;
		}
		return false;
	}

	public string GetTextMouseHintLeft()
	{
		if (!invOwner.CanShiftClick(this))
		{
			if (!invOwner.CanCtrlClick(this))
			{
				if (!invOwner.CanAltClick(this))
				{
					return "";
				}
				return invOwner.GetTextAltClick(this);
			}
			return invOwner.GetTextCtrlClick(this);
		}
		return invOwner.GetTextShiftClick(this);
	}

	public bool ShowMouseHintRight()
	{
		if (invOwner != null && card != null)
		{
			return invOwner.AllowAutouse;
		}
		return false;
	}

	public string GetTextMouseHintRight()
	{
		return invOwner.GetAutoUseLang(this) ?? "";
	}

	public RenderRow GetRenderRow()
	{
		if (refObj is RenderRow)
		{
			return (RenderRow)refObj;
		}
		if (recipe == null)
		{
			if (card == null)
			{
				return null;
			}
			return card.sourceCard;
		}
		return recipe.renderRow;
	}

	public void OnRefreshPref()
	{
		if (refObj is RenderRow renderRow)
		{
			renderRow.SetImage(icon, null, 0, setNativeSize: false);
		}
		if (recipe != null)
		{
			SetRecipe(recipe);
		}
		else if (card != null)
		{
			SetCard(card);
		}
	}

	public override bool CanMiddleClick()
	{
		if (invOwner != null && card != null)
		{
			return EClass.ui.AllowInventoryInteractions;
		}
		return false;
	}

	public override void OnMiddleClick(bool forceClick)
	{
		Thing t = card.Thing;
		InvOwner o = invOwner;
		if ((forceClick || EInput.middleMouse.clicked) && o.AllowContext)
		{
			o.ShowContextMenu(this);
		}
		EInput.middleMouse.pressedLongAction = delegate
		{
			if (EClass.ui.AllowInventoryInteractions && EClass.core.config.game.holdMiddleButtonToHold && o.TryHold(t))
			{
				if (!(t.trait is TraitAbility) && t.trait.IsTool)
				{
					HotItemHeld.disableTool = true;
				}
				if (EClass.ui.contextMenu.isActive)
				{
					EClass.ui.contextMenu.currentMenu.Hide();
				}
				EInput.Consume();
			}
		};
	}
}
