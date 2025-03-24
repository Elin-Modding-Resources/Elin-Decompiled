using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Trait : EClass
{
	public enum TileMode
	{
		Default,
		Door,
		Illumination,
		DefaultNoAnime,
		SignalAnime
	}

	public enum CopyShopType
	{
		None,
		Item,
		Spellbook
	}

	public static TraitSelfFactory SelfFactory = new TraitSelfFactory();

	public Card owner;

	protected static List<Point> listRadiusPoints = new List<Point>();

	public string[] Params
	{
		get
		{
			if (!owner.c_editorTraitVal.IsEmpty())
			{
				return ("," + owner.c_editorTraitVal).Split(',');
			}
			return owner.sourceCard.trait;
		}
	}

	public virtual byte WeightMod => 0;

	public virtual int IdSkin => owner.idSkin;

	public virtual string Name => owner.NameSimple;

	public virtual TileType tileType => owner.TileType;

	public virtual RefCardName RefCardName => RefCardName.Default;

	public virtual bool IsBlockPath => tileType.IsBlockPass;

	public virtual bool IsBlockSight => tileType.IsBlockSight;

	public virtual bool IsDoor => false;

	public virtual bool IsOpenSight => false;

	public virtual bool IsOpenPath => false;

	public virtual bool IsFloating => false;

	public virtual bool IsGround => false;

	public virtual bool InvertHeldSprite => false;

	public virtual bool IsChangeFloorHeight => owner.Pref.Surface;

	public virtual bool ShouldRefreshTile
	{
		get
		{
			if (!IsBlockPath && !IsOpenSight)
			{
				return IsBlockSight;
			}
			return true;
		}
	}

	public virtual bool ShouldTryRefreshRoom => IsDoor;

	public virtual int InstallBottomPriority => -1;

	public virtual bool CanHarvest => true;

	public virtual int radius => 0;

	public virtual TraitRadiusType radiusType => TraitRadiusType.Default;

	public virtual bool CanUseRoomRadius => true;

	public virtual int GuidePriotiy => 0;

	public virtual int Electricity
	{
		get
		{
			if (!owner.isThing)
			{
				return 0;
			}
			int electricity = owner.Thing.source.electricity;
			if (electricity > 0 || EClass._zone == null || EClass._zone.branch == null)
			{
				return electricity;
			}
			return electricity * 100 / (100 + EClass._zone.branch.Evalue(2700) / 2);
		}
	}

	public virtual bool IgnoreLastStackHeight => false;

	public virtual int Decay => owner.material.decay;

	public virtual int DecaySpeed => 100;

	public virtual int DecaySpeedChild => 100;

	public virtual bool IsFridge => false;

	public virtual int DefaultStock => 1;

	public virtual bool HoldAsDefaultInteraction => false;

	public virtual int CraftNum => 1;

	public virtual bool ShowOrbit => false;

	public virtual bool HaveUpdate => false;

	public virtual bool IsSpot => radius > 0;

	public virtual bool IsFactory => false;

	public virtual bool CanAutofire => false;

	public virtual bool CanName => false;

	public virtual bool CanPutAway => true;

	public virtual bool CanChangeHeight => true;

	public virtual bool CanStack => owner.category.maxStack > 1;

	public virtual bool CanCopyInBlueprint
	{
		get
		{
			if (owner.rarity <= Rarity.Superior && owner.sourceCard.value > 0)
			{
				return CanBeDestroyed;
			}
			return false;
		}
	}

	public virtual bool CanBeAttacked => false;

	public virtual bool CanExtendBuild => false;

	public virtual string langNote => "";

	public virtual string IDInvStyle => "default";

	public virtual string IDActorEx => owner.Thing.source.idActorEx;

	public virtual bool MaskOnBuild => false;

	public virtual bool ShowContextOnPick => false;

	public virtual bool IsThrowMainAction
	{
		get
		{
			if (!owner.HasTag(CTAG.throwWeapon))
			{
				return owner.IsMeleeWeapon;
			}
			return true;
		}
	}

	public virtual bool LevelAsQuality => false;

	public virtual bool UseDummyTile => true;

	public virtual bool RequireFullStackCheck => false;

	public virtual bool DisableAutoCombat => false;

	public virtual InvGridSize InvGridSize
	{
		get
		{
			if (!owner.IsPC)
			{
				return InvGridSize.Default;
			}
			return InvGridSize.Backpack;
		}
	}

	public virtual bool IsContainer => false;

	public virtual bool CanUseContent => CanSearchContent;

	public virtual bool CanSearchContent
	{
		get
		{
			if (!owner.isChara)
			{
				if (IsContainer)
				{
					return owner.c_lockLv == 0;
				}
				return false;
			}
			return true;
		}
	}

	public virtual bool CanOpenContainer
	{
		get
		{
			if (IsContainer)
			{
				return !owner.isNPCProperty;
			}
			return false;
		}
	}

	public virtual bool IsSpecialContainer => false;

	public virtual ContainerType ContainerType => ContainerType.Default;

	public virtual ThrowType ThrowType => ThrowType.Default;

	public virtual EffectDead EffectDead => EffectDead.Default;

	public virtual bool IsHomeItem => false;

	public virtual bool IsAltar => false;

	public virtual bool IsRestSpot => false;

	public virtual bool CanBeMasked => false;

	public virtual bool IsBlendBase => false;

	public virtual bool CanBeOnlyBuiltInHome => false;

	public virtual bool CanBuildInTown
	{
		get
		{
			if (!owner.TileType.IsBlockPass)
			{
				return !owner.TileType.IsBlockSight;
			}
			return false;
		}
	}

	public virtual bool CanBeHeld => ReqHarvest == null;

	public virtual bool CanBeStolen
	{
		get
		{
			if (!CanOnlyCarry)
			{
				return CanBeHeld;
			}
			return false;
		}
	}

	public virtual bool CanOnlyCarry => false;

	public virtual bool CanBeDestroyed => true;

	public virtual bool CanBeHallucinated => true;

	public virtual bool CanBeDropped => true;

	public virtual string ReqHarvest => null;

	public virtual bool CanBeDisassembled
	{
		get
		{
			if (CanBeDestroyed && !(this is TraitTrap) && owner.things.Count == 0)
			{
				return owner.rarity < Rarity.Artifact;
			}
			return false;
		}
	}

	public virtual bool CanBeShipped
	{
		get
		{
			if (!owner.IsImportant)
			{
				return !owner.IsUnique;
			}
			return false;
		}
	}

	public virtual bool HasCharges => false;

	public virtual bool ShowCharges => HasCharges;

	public virtual bool ShowChildrenNumber => IsContainer;

	public virtual bool ShowAsTool => false;

	public virtual bool CanBeHeldAsFurniture
	{
		get
		{
			if (!(this is TraitTool))
			{
				return IsThrowMainAction;
			}
			return true;
		}
	}

	public virtual bool HideInAdv => false;

	public virtual bool NoHeldDir => false;

	public virtual bool AlwaysHideOnLowWall => false;

	public bool ExistsOnMap => owner.ExistsOnMap;

	public virtual bool RenderExtra => false;

	public virtual float DropChance => 0f;

	public virtual string IdNoRestock => owner.id;

	public virtual int IdleUseChance => 3;

	public virtual string RecipeCat => owner.sourceCard.RecipeCat;

	public virtual bool IsTool => false;

	public virtual string LangUse => "actUse";

	public virtual bool IsOn => owner.isOn;

	public virtual bool IsAnimeOn
	{
		get
		{
			if (!IsOn)
			{
				return !IsToggle;
			}
			return true;
		}
	}

	public bool IsToggle => ToggleType != ToggleType.None;

	public virtual bool AutoToggle
	{
		get
		{
			if (IsLighting || ToggleType == ToggleType.Curtain || ToggleType == ToggleType.Electronics)
			{
				return !owner.disableAutoToggle;
			}
			return false;
		}
	}

	public bool IsLighting
	{
		get
		{
			if (ToggleType != ToggleType.Fire)
			{
				return ToggleType == ToggleType.Light;
			}
			return true;
		}
	}

	public virtual bool IsLightOn
	{
		get
		{
			if (!owner.isChara)
			{
				return owner.isOn;
			}
			return true;
		}
	}

	public virtual bool IsNightOnlyLight
	{
		get
		{
			if (ToggleType != ToggleType.Electronics && IsToggle)
			{
				return !owner.isRoofItem;
			}
			return false;
		}
	}

	public virtual TileMode tileMode => TileMode.Default;

	public virtual bool UseAltTiles => owner.isOn;

	public virtual bool UseLowblock => false;

	public virtual bool UseExtra => true;

	public virtual bool UseLightColor => true;

	public virtual Color? ColorExtra => null;

	public virtual int MaxFuel
	{
		get
		{
			if (ToggleType != ToggleType.Fire)
			{
				return 0;
			}
			return 100;
		}
	}

	public virtual int FuelCost => 1;

	public virtual bool ShowFuelWindow => true;

	public bool IsRequireFuel => MaxFuel > 0;

	public string IdToggleExtra => owner.Thing?.source.idToggleExtra;

	public virtual ToggleType ToggleType
	{
		get
		{
			if (Electricity >= 0)
			{
				return ToggleType.None;
			}
			return ToggleType.Electronics;
		}
	}

	public virtual int ShopLv => Mathf.Max(1, EClass._zone.development / 10 + owner.c_invest + 1);

	public virtual CopyShopType CopyShop => CopyShopType.None;

	public virtual int NumCopyItem => 2 + Mathf.Min(owner.c_invest / 10, 3);

	public virtual ShopType ShopType => ShopType.None;

	public virtual CurrencyType CurrencyType => CurrencyType.Money;

	public virtual PriceType PriceType => PriceType.Default;

	public virtual bool AllowSell
	{
		get
		{
			if (CurrencyType != CurrencyType.Money)
			{
				return CurrencyType == CurrencyType.None;
			}
			return true;
		}
	}

	public virtual int CostRerollShop
	{
		get
		{
			if (CurrencyType == CurrencyType.Money || CurrencyType == CurrencyType.Influence)
			{
				return 1;
			}
			return 0;
		}
	}

	public virtual bool AllowCriminal => owner.isThing;

	public virtual int RestockDay => 5;

	public virtual SlaverType SlaverType => SlaverType.None;

	public virtual string LangBarter => "daBuy";

	public string TextNextRestock => GetTextRestock(LangBarter, pet: false);

	public string TextNextRestockPet => GetTextRestock((SlaverType == SlaverType.Slave) ? "daBuySlave" : "daBuyPet", pet: true);

	public string GetParam(int i, string def = null)
	{
		if (i < Params.Length)
		{
			return Params[i];
		}
		return def;
	}

	public int GetParamInt(int i, int def)
	{
		if (i < Params.Length)
		{
			return Params[i].ToInt();
		}
		return def;
	}

	public virtual bool Contains(RecipeSource r)
	{
		return r.idFactory == ((owner.sourceCard.origin != null) ? owner.sourceCard.origin.id : owner.id);
	}

	public virtual int GetValue()
	{
		return owner.sourceCard.value;
	}

	public virtual bool CanStackTo(Thing to)
	{
		return CanStack;
	}

	public virtual string GetHoverText()
	{
		return null;
	}

	public virtual Action GetHealAction(Chara c)
	{
		return null;
	}

	public virtual bool CanBlend(Thing t)
	{
		return false;
	}

	public virtual void OnBlend(Thing t, Chara c)
	{
	}

	public virtual int GetActDuration(Chara c)
	{
		return 0;
	}

	public virtual SourceElement.Row GetRefElement()
	{
		return null;
	}

	public virtual Sprite GetRefSprite()
	{
		return null;
	}

	public virtual void OnRenderExtra(RenderParam p)
	{
	}

	public virtual Emo2 GetHeldEmo(Chara c)
	{
		return Emo2.none;
	}

	public virtual void SetOwner(Card _owner)
	{
		owner = _owner;
		OnSetOwner();
	}

	public virtual bool IdleUse(Chara c, int dist)
	{
		return false;
	}

	public virtual void OnSetOwner()
	{
	}

	public virtual void OnImportMap()
	{
	}

	public virtual void SetParams(params string[] s)
	{
	}

	public virtual void OnCrafted(Recipe recipe)
	{
	}

	public virtual void OnCreate(int lv)
	{
	}

	public virtual void OnEquip(Chara c, bool onSetOwner)
	{
	}

	public virtual void OnUnequip(Chara c)
	{
	}

	public virtual void OnChangePlaceState(PlaceState state)
	{
	}

	public virtual void OnAddedToZone()
	{
	}

	public virtual void OnRemovedFromZone()
	{
	}

	public virtual void OnSimulateHour(VirtualDate date)
	{
	}

	public virtual string GetName()
	{
		return owner.sourceCard.GetText();
	}

	public virtual void SetName(ref string s)
	{
	}

	public virtual void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (radius == 0)
		{
			return;
		}
		Vector3 vector = point.Position();
		vector.z += EClass.setting.render.thingZ;
		foreach (Point item in ListPoints(point))
		{
			Vector3 vector2 = item.Position();
			EClass.screen.guide.passGuideFloor.Add(vector2.x, vector2.y, vector2.z, 10f, 0.3f);
		}
	}

	public virtual int CompareTo(Card b)
	{
		return 0;
	}

	public virtual bool CanBuiltAt(Point p)
	{
		return true;
	}

	public virtual void Update()
	{
	}

	public Point GetPoint()
	{
		return owner.pos;
	}

	public Point GetRandomPoint(Func<Point, bool> func = null, Chara accessChara = null)
	{
		if (radius == 0)
		{
			return owner.pos;
		}
		List<Point> list = ListPoints();
		for (int i = 0; i < 50; i++)
		{
			Point point = list.RandomItem();
			if (point.IsValid && (func == null || func(point)) && (accessChara == null || accessChara.HasAccess(point)))
			{
				return point;
			}
		}
		return list[0];
	}

	public virtual List<Point> ListPoints(Point center = null, bool onlyPassable = true)
	{
		listRadiusPoints.Clear();
		if (center == null)
		{
			center = owner.pos;
		}
		if (radius == 0)
		{
			listRadiusPoints.Add(center.Copy());
			return listRadiusPoints;
		}
		Room room = center.cell.room;
		if (room != null && CanUseRoomRadius)
		{
			foreach (Point point in room.points)
			{
				if (radiusType == TraitRadiusType.Farm)
				{
					listRadiusPoints.Add(point.Copy());
				}
				else if ((!onlyPassable || !point.cell.blocked) && !point.cell.HasBlock && point.cell.HasFloor)
				{
					listRadiusPoints.Add(point.Copy());
				}
			}
		}
		else
		{
			EClass._map.ForeachSphere(center.x, center.z, radius + 1, delegate(Point p)
			{
				if (radiusType == TraitRadiusType.Farm)
				{
					if (!p.cell.HasBlock || p.cell.HasFence)
					{
						listRadiusPoints.Add(p.Copy());
					}
				}
				else if ((!onlyPassable || !p.cell.blocked) && !p.cell.HasBlock && p.cell.HasFloor && (!onlyPassable || Los.IsVisible(center, p)))
				{
					listRadiusPoints.Add(p.Copy());
				}
			});
		}
		if (listRadiusPoints.Count == 0)
		{
			listRadiusPoints.Add(center.Copy());
		}
		return listRadiusPoints;
	}

	public virtual Recipe GetRecipe()
	{
		return Recipe.Create(owner.Thing);
	}

	public virtual Recipe GetBuildModeRecipe()
	{
		return Recipe.Create(owner.Thing);
	}

	public virtual bool CanCook(Card c)
	{
		if (c == null || !ExistsOnMap || !(c.trait is TraitFood))
		{
			return false;
		}
		return true;
	}

	public void CookProgress()
	{
		if (!ExistsOnMap)
		{
			return;
		}
		foreach (Card item in owner.pos.ListCards())
		{
			owner.PlaySound("cook");
			item.renderer.PlayAnime(AnimeID.Jump);
		}
	}

	public virtual bool CanOffer(Card tg)
	{
		if (tg == null || tg.isChara || tg.trait.CanOnlyCarry)
		{
			return false;
		}
		if (tg.rarity == Rarity.Artifact)
		{
			return false;
		}
		return true;
	}

	public void OfferProcess(Chara cc)
	{
		if (!ExistsOnMap)
		{
			return;
		}
		SourceReligion.Row row = EClass.sources.religions.map.TryGetValue(owner.c_idDeity, EClass.sources.religions.map["eyth"]);
		string @ref = row.GetTextArray("name2")[1];
		string ref2 = row.GetTextArray("name2")[0];
		if (EClass.rnd(3) == 0)
		{
			cc.Talk("offer", @ref, ref2);
		}
		foreach (Card item in owner.pos.ListCards())
		{
			if (CanOffer(item))
			{
				item.renderer.PlayAnime(AnimeID.Shiver);
			}
		}
	}

	public void Offer(Chara cc)
	{
		if (!ExistsOnMap)
		{
			return;
		}
		foreach (Card item in owner.pos.ListCards())
		{
			if (CanOffer(item))
			{
				item.Destroy();
				cc.depression.Mod(100);
				owner.PlaySound("offering");
			}
		}
	}

	public virtual bool TryProgress(AIProgress p)
	{
		return true;
	}

	public virtual LockOpenState TryOpenLock(Chara cc, bool msgFail = true)
	{
		Thing thing = cc.things.FindBest<TraitLockpick>((Thing t) => -t.c_charges);
		int num = ((thing == null) ? (cc.Evalue(280) / 2 + 2) : (cc.Evalue(280) + 10));
		int num2 = owner.c_lockLv;
		bool flag = this is TraitChestPractice;
		if (flag)
		{
			num2 = num / 4 * 3 - 1;
		}
		if (num <= num2 && cc.IsPC)
		{
			cc.PlaySound("lock");
			cc.Say("openLockFail2");
			owner.PlayAnime(AnimeID.Shiver);
			return LockOpenState.NotEnoughSkill;
		}
		if (thing != null && !flag)
		{
			thing.ModCharge(-1, destroy: true);
		}
		if (EClass.rnd(num + 6) > num2 + 5 || (!cc.IsPC && EClass.rnd(20) == 0) || EClass.rnd(200) == 0)
		{
			cc.PlaySound("lock_open");
			cc.Say("lockpick_success", cc, owner);
			int num3 = 100 + num2 * 10;
			if (owner.c_lockedHard)
			{
				num3 *= 10;
			}
			cc.ModExp(280, num3);
			owner.c_lockLv = 0;
			if (owner.c_lockedHard)
			{
				owner.c_lockedHard = false;
				owner.c_priceAdd = 0;
			}
			if (cc.IsPC && owner.isLostProperty)
			{
				EClass.player.ModKarma(-8);
			}
			owner.isLostProperty = false;
			return LockOpenState.Success;
		}
		cc.PlaySound("lock");
		if (cc.IsPC)
		{
			cc.Say("openLockFail");
		}
		cc.ModExp(280, (thing != null) ? 50 : 30);
		if ((thing == null) | (EClass.rnd(2) == 0))
		{
			cc.stamina.Mod(-1);
		}
		return LockOpenState.Fail;
	}

	public virtual void WriteNote(UINote n, bool identified)
	{
	}

	public int GetSortVal(UIList.SortMode m)
	{
		_ = 7;
		return owner.sourceCard._index;
	}

	public virtual HotItem GetHotItem()
	{
		return new HotItemHeld(owner as Thing);
	}

	public virtual bool CanRead(Chara c)
	{
		return false;
	}

	public virtual void OnRead(Chara c)
	{
	}

	public virtual bool CanEat(Chara c)
	{
		return owner.HasElement(10);
	}

	public virtual void OnEat(Chara c)
	{
	}

	public virtual bool CanDrink(Chara c)
	{
		return false;
	}

	public virtual void OnDrink(Chara c)
	{
	}

	public virtual void OnThrowGround(Chara c, Point p)
	{
	}

	public virtual bool CanUse(Chara c)
	{
		return false;
	}

	public virtual bool CanUse(Chara c, Card tg)
	{
		return false;
	}

	public virtual bool CanUse(Chara c, Point p)
	{
		return false;
	}

	public virtual bool OnUse(Chara c)
	{
		if (c.held != owner)
		{
			c.TryHoldCard(owner);
		}
		return true;
	}

	public virtual bool OnUse(Chara c, Card tg)
	{
		return true;
	}

	public virtual bool OnUse(Chara c, Point p)
	{
		return true;
	}

	public virtual void TrySetAct(ActPlan p)
	{
		if (CanUse(owner.Chara))
		{
			p.TrySetAct(LangUse, () => OnUse(p.cc), owner);
		}
	}

	public virtual void TrySetHeldAct(ActPlan p)
	{
	}

	public virtual void OnHeld()
	{
	}

	public virtual void OnTickHeld()
	{
	}

	public virtual void OnSetCurrentItem()
	{
	}

	public virtual void OnUnsetCurrentItem()
	{
	}

	public virtual bool OnChildDecay(Card c, bool firstDecay)
	{
		return true;
	}

	public virtual bool CanChildDecay(Card c)
	{
		return false;
	}

	public virtual void OnSetCardGrid(ButtonGrid b)
	{
	}

	public virtual void OnStepped(Chara c)
	{
	}

	public virtual void OnSteppedOut(Chara c)
	{
	}

	public virtual void OnOpenDoor(Chara c)
	{
	}

	public void Install(bool byPlayer)
	{
		if (Electricity != 0)
		{
			EClass._zone.dirtyElectricity = true;
			if (Electricity > 0)
			{
				EClass._zone.electricity += Electricity;
				EClass.pc.PlaySound("electricity_on");
			}
		}
		TryToggle();
		owner.RecalculateFOV();
		if (EClass._zone.isStarted && ToggleType == ToggleType.Fire && owner.isOn)
		{
			owner.PlaySound("fire");
		}
		OnInstall(byPlayer);
	}

	public void Uninstall()
	{
		if (Electricity != 0)
		{
			if (owner.isOn)
			{
				Toggle(on: false, silent: true);
			}
			EClass._zone.dirtyElectricity = true;
			if (Electricity > 0)
			{
				EClass._zone.electricity -= Electricity;
				EClass.pc.PlaySound("electricity_off");
			}
		}
		OnUninstall();
	}

	public virtual void OnInstall(bool byPlayer)
	{
	}

	public virtual void OnUninstall()
	{
	}

	public virtual void TryToggle()
	{
		if (!owner.IsInstalled)
		{
			return;
		}
		if (Electricity < 0 && owner.isOn && EClass._zone.electricity < 0)
		{
			Toggle(on: false, silent: true);
		}
		else if (AutoToggle)
		{
			int hour = EClass.world.date.hour;
			bool on = !IsNightOnlyLight || hour >= 17 || hour <= 5 || EClass._map.IsIndoor;
			if (ToggleType == ToggleType.Fire && EClass.world.weather.IsRaining && !EClass._map.IsIndoor && !owner.Cell.HasRoof)
			{
				on = false;
			}
			Toggle(on, silent: true);
		}
	}

	public virtual void Toggle(bool on, bool silent = false)
	{
		if (owner.isOn == on)
		{
			return;
		}
		if (Electricity < 0)
		{
			if (on)
			{
				if (EClass._zone.electricity + Electricity < 0)
				{
					if (EClass._zone.isStarted)
					{
						if (!silent)
						{
							owner.Say("notEnoughElectricity", owner);
						}
						owner.PlaySound("electricity_insufficient");
					}
					return;
				}
				EClass._zone.electricity += Electricity;
			}
			else
			{
				EClass._zone.electricity -= Electricity;
			}
		}
		owner.isOn = on;
		PlayToggleEffect(silent);
		OnToggle();
	}

	public virtual void PlayToggleEffect(bool silent)
	{
		bool flag = ToggleType == ToggleType.Fire;
		bool isOn = owner.isOn;
		switch (ToggleType)
		{
		case ToggleType.Lever:
			if (!silent)
			{
				owner.Say("lever", EClass.pc, owner);
				owner.PlaySound("switch_lever");
			}
			return;
		case ToggleType.Curtain:
			if (!silent)
			{
				owner.Say(isOn ? "close" : "open", EClass.pc, owner);
				owner.PlaySound("Material/leather_drop");
			}
			owner.pos.RefreshNeighborTiles();
			EClass.pc.RecalculateFOV();
			return;
		case ToggleType.None:
			return;
		}
		if (isOn)
		{
			if (!silent)
			{
				owner.Say(flag ? "toggle_fire" : "toggle_ele", EClass.pc, owner);
				string id = ((Electricity < 0) ? "switch_on_electricity" : (flag ? "torch_lit" : "switch_on"));
				if (this is TraitMusicBox)
				{
					id = "switch_on_musicbox";
				}
				owner.PlaySound(id);
			}
			RefreshRenderer();
			owner.RecalculateFOV();
			return;
		}
		if (!silent)
		{
			string id2 = ((Electricity < 0) ? "switch_off_electricity" : (flag ? "torch_unlit" : "switch_off"));
			if (this is TraitMusicBox)
			{
				id2 = "switch_off_musicbox";
			}
			owner.PlaySound(id2);
		}
		RefreshRenderer();
		owner.RecalculateFOV();
	}

	public virtual void OnToggle()
	{
	}

	public virtual void TrySetToggleAct(ActPlan p)
	{
		if (!p.IsSelfOrNeighbor)
		{
			return;
		}
		switch (ToggleType)
		{
		case ToggleType.Lever:
			p.TrySetAct("ActToggleLever", delegate
			{
				Toggle(!owner.isOn);
				return true;
			}, owner);
			break;
		case ToggleType.Curtain:
			p.TrySetAct(owner.isOn ? "actOpen" : "actClose", delegate
			{
				Toggle(!owner.isOn);
				return true;
			}, owner);
			break;
		case ToggleType.Fire:
		case ToggleType.Light:
		case ToggleType.Electronics:
		{
			bool flag = ToggleType == ToggleType.Fire;
			if (EClass._zone.IsPCFaction || p.altAction || this is TraitCrafter || Electricity < 0)
			{
				if (owner.isOn)
				{
					if (p.altAction)
					{
						p.TrySetAct(flag ? "ActExtinguishTorch" : "ActToggleOff", delegate
						{
							Toggle(on: false);
							return true;
						}, owner);
					}
				}
				else if (!(this is TraitFactory) && !(this is TraitIncubator) && (!IsRequireFuel || owner.c_charges > 0))
				{
					p.TrySetAct(flag ? "ActTorch" : "ActToggleOn", delegate
					{
						Toggle(on: true);
						return true;
					}, owner);
				}
				if (IsRequireFuel && ((p.altAction && owner.c_charges < MaxFuel) || (!owner.isOn && owner.c_charges == 0)) && ShowFuelWindow)
				{
					p.TrySetAct("ActFuel", delegate
					{
						LayerDragGrid.Create(new InvOwnerRefuel(owner));
						return false;
					}, owner);
				}
			}
			if (p.altAction)
			{
				p.TrySetAct("disableAutoToggle".lang((owner.disableAutoToggle ? "off" : "on").lang()), delegate
				{
					owner.disableAutoToggle = !owner.disableAutoToggle;
					SE.Click();
					return true;
				}, owner);
			}
			break;
		}
		}
	}

	public bool IsFuelEnough(int num = 1, List<Thing> excludes = null, bool tryRefuel = true)
	{
		if (!IsRequireFuel)
		{
			return true;
		}
		if (owner.c_charges >= FuelCost * num)
		{
			return true;
		}
		if (owner.autoRefuel)
		{
			TryRefuel(FuelCost * num - owner.c_charges, excludes);
		}
		return owner.c_charges >= FuelCost * num;
	}

	public bool IsFuel(string s)
	{
		return GetFuelValue(s) > 0;
	}

	public bool IsFuel(Thing t)
	{
		return GetFuelValue(t) > 0;
	}

	public int GetFuelValue(Thing t)
	{
		if (t.c_isImportant)
		{
			return 0;
		}
		return GetFuelValue(t.id);
	}

	public int GetFuelValue(string id)
	{
		if (ToggleType == ToggleType.Electronics)
		{
			if (id == "battery")
			{
				return 20;
			}
		}
		else
		{
			if (id == "log")
			{
				return 20;
			}
			if (id == "branch")
			{
				return 5;
			}
		}
		return 0;
	}

	public void Refuel(Thing t)
	{
		t.PlaySoundDrop(spatial: false);
		owner.ModCharge(t.Num * GetFuelValue(t));
		Msg.Say("fueled", t);
		if (!owner.isOn)
		{
			owner.trait.Toggle(on: true);
		}
		t.Destroy();
		owner.renderer.PlayAnime(AnimeID.Shiver);
	}

	public void TryRefuel(int dest, List<Thing> excludes)
	{
		if (FindFuel(refuel: false))
		{
			FindFuel(refuel: true);
		}
		bool FindFuel(bool refuel)
		{
			int num = dest;
			List<Thing> list = EClass._zone.TryListThingsInSpot<TraitSpotFuel>((Thing t) => IsFuel(t));
			EClass.pc.things.Foreach(delegate(Thing t)
			{
				if (IsFuel(t) && t.qualityTier == 0 && (excludes == null || !excludes.Contains(t)))
				{
					list.Add(t);
				}
			});
			foreach (Thing item in list)
			{
				if (num > 0)
				{
					int num2 = Mathf.Min(item.Num, Mathf.CeilToInt((float)num / (float)GetFuelValue(item)));
					num -= GetFuelValue(item) * num2;
					if (refuel)
					{
						Refuel(item.Split(num2));
					}
				}
			}
			return num <= 0;
		}
	}

	public virtual void OnEnterScreen()
	{
		RefreshRenderer();
	}

	public virtual void RefreshRenderer()
	{
		if (owner.renderer.isSynced && !IdToggleExtra.IsEmpty())
		{
			if (owner.isOn)
			{
				owner.renderer.AddExtra(IdToggleExtra);
			}
			else
			{
				owner.renderer.RemoveExtra(IdToggleExtra);
			}
		}
	}

	public virtual void SetMainText(UIText t, bool hotitem)
	{
		if (owner.isThing && !owner.Thing.source.attackType.IsEmpty() && owner.ammoData != null)
		{
			t.SetText(owner.c_ammo.ToString() ?? "", FontColor.Charge);
			t.SetActive(enable: true);
		}
		else if (owner.Num == 1 && ShowCharges && owner.IsIdentified)
		{
			t.SetText(owner.c_charges.ToString() ?? "", FontColor.Charge);
			t.SetActive(enable: true);
		}
		else
		{
			t.SetText(owner.Num.ToShortNumber(), FontColor.ButtonGrid);
			t.SetActive(owner.Num > 1);
		}
	}

	public virtual bool CanCopy(Thing t)
	{
		return false;
	}

	public string GetTextRestock(string lang, bool pet)
	{
		int rawDeadLine = 0;
		if (pet)
		{
			SlaverData obj = owner.GetObj<SlaverData>(5);
			if (obj != null)
			{
				rawDeadLine = obj.dateRefresh;
			}
		}
		else
		{
			rawDeadLine = owner.c_dateStockExpire;
		}
		int remainingHours = EClass.world.date.GetRemainingHours(rawDeadLine);
		if (remainingHours > 0)
		{
			return "nextRestock".lang(lang.lang(), Date.GetText(remainingHours) ?? "");
		}
		return lang.lang();
	}

	public Emo2 GetRestockedIcon()
	{
		if (SlaverType != 0)
		{
			SlaverData obj = owner.GetObj<SlaverData>(5);
			if (obj != null && EClass.world.date.IsExpired(obj.dateRefresh))
			{
				return Emo2.restock;
			}
		}
		int c_dateStockExpire = owner.c_dateStockExpire;
		if (c_dateStockExpire != 0 && EClass.world.date.IsExpired(c_dateStockExpire))
		{
			if (ShopType == ShopType.None)
			{
				return Emo2.blessing;
			}
			return Emo2.restock;
		}
		return Emo2.none;
	}

	public void OnBarter()
	{
		Thing t = owner.things.Find("chest_merchant");
		if (t == null)
		{
			t = ThingGen.Create("chest_merchant");
			owner.AddThing(t);
		}
		t.c_lockLv = 0;
		if (!EClass.world.date.IsExpired(owner.c_dateStockExpire))
		{
			return;
		}
		owner.c_dateStockExpire = EClass.world.date.GetRaw(24 * RestockDay);
		owner.isRestocking = true;
		t.things.DestroyAll((Thing _t) => _t.GetInt(101) != 0);
		foreach (Thing thing7 in t.things)
		{
			thing7.invX = -1;
		}
		switch (ShopType)
		{
		case ShopType.Plat:
			NoRestock(ThingGen.Create("lucky_coin").SetNum(10));
			break;
		case ShopType.Copy:
		{
			Thing c_copyContainer = owner.c_copyContainer;
			if (c_copyContainer == null)
			{
				break;
			}
			int num3 = 0;
			foreach (Thing thing8 in c_copyContainer.things)
			{
				if (!owner.trait.CanCopy(thing8))
				{
					continue;
				}
				Thing thing4 = thing8.Duplicate(1);
				thing4.isStolen = false;
				thing4.isCopy = true;
				thing4.c_priceFix = 0;
				foreach (Element item in thing4.elements.dict.Values.Where((Element e) => e.HasTag("noInherit")).ToList())
				{
					thing4.elements.Remove(item.id);
				}
				int num4 = 1;
				switch (owner.trait.CopyShop)
				{
				case CopyShopType.Item:
				{
					num4 = (1000 + owner.c_invest * 100) / (thing4.GetPrice(CurrencyType.Money, sell: false, PriceType.CopyShop) + 50);
					int[] array = new int[4] { 701, 704, 703, 702 };
					foreach (int ele in array)
					{
						if (thing4.HasElement(ele))
						{
							num4 = 1;
						}
					}
					break;
				}
				case CopyShopType.Spellbook:
					thing4.c_charges = thing8.c_charges;
					break;
				}
				if (num4 > 1 && thing4.trait.CanStack)
				{
					thing4.SetNum(num4);
				}
				AddThing(thing4);
				num3++;
				if (num3 > owner.trait.NumCopyItem)
				{
					break;
				}
			}
			break;
		}
		case ShopType.Specific:
		{
			string id2 = owner.id;
			if (!(id2 == "mogu"))
			{
				if (!(id2 == "felmera"))
				{
					break;
				}
				foreach (Thing item2 in new DramaOutcome().ListFelmeraBarter())
				{
					AddThing(item2);
				}
			}
			else
			{
				AddThing(ThingGen.Create("casino_coin").SetNum(5000));
			}
			break;
		}
		case ShopType.Deed:
			Add("deed", 1, 0);
			Add("deed_move", 2 + EClass.rnd(5), 0);
			Add("license_illumination", 1, 0);
			Add("license_void", 1, 0);
			Add("license_adv", 1, 0);
			break;
		case ShopType.RedBook:
		{
			for (int l = 0; l < 30; l++)
			{
				AddThing(ThingGen.CreateFromCategory("book"));
			}
			break;
		}
		case ShopType.Seed:
		{
			AddThing(TraitSeed.MakeSeed("rice")).SetNum(4 + EClass.rnd(4));
			AddThing(TraitSeed.MakeSeed("cabbage")).SetNum(4 + EClass.rnd(4));
			AddThing(TraitSeed.MakeSeed("carrot")).SetNum(4 + EClass.rnd(4));
			AddThing(TraitSeed.MakeSeed("potato")).SetNum(4 + EClass.rnd(4));
			AddThing(TraitSeed.MakeSeed("corn")).SetNum(4 + EClass.rnd(4));
			for (int n = 0; n < EClass.rnd(3) + 1; n++)
			{
				Add("462", 1, 0);
			}
			for (int num5 = 0; num5 < EClass.rnd(3) + 1; num5++)
			{
				Add("1167", 1, 0);
			}
			break;
		}
		case ShopType.Loytel:
			Add("board_map", 1, 0);
			Add("board_build", 1, 0);
			Add("book_resident", 1, 0);
			Add("3", 1, 0);
			Add("4", 1, 0);
			Add("5", 1, 0);
			AddThing(ThingGen.CreatePlan(2512));
			AddThing(ThingGen.CreatePlan(2810));
			NoRestock(ThingGen.Create("rp_block").SetLv(1).SetNum(10));
			if (EClass.game.quests.GetPhase<QuestVernis>() >= 3)
			{
				NoRestock(ThingGen.CreateRecipe("explosive"));
			}
			break;
		case ShopType.Starter:
		case ShopType.StarterEx:
			Add("board_home", 1, 0);
			Add("board_resident", 1, 0);
			Add("1", 1, 0);
			Add("2", 1, 0);
			if (ShopType == ShopType.StarterEx)
			{
				Add("board_expedition", 1, 0);
				Add("mailpost", 1, 0);
				Add("record", 1, 0);
				Add("tent2", 1, 0);
				Add("tent1", 1, 0);
				Add("wagon1", 1, 0);
				Add("wagon_big", 1, 0);
				Add("wagon_big2", 1, 0);
				Add("wagon_big3", 1, 0);
				Add("wagon_big4", 1, 0);
				Add("wagon_big5", 1, 0);
				Add("teleporter", 1, 0);
				Add("teleporter2", 1, 0);
				Add("recharger", 1, 0);
				Add("machine_gene2", 1, 0);
				NoRestock(ThingGen.CreateRecipe("torch_wall"));
				NoRestock(ThingGen.CreateRecipe("factory_sign"));
				NoRestock(ThingGen.CreateRecipe("beehive"));
				NoRestock(ThingGen.Create("rp_food").SetNum(5).SetLv(10)
					.Thing);
				}
				else
				{
					AddThing(ThingGen.CreatePlan(2119));
					NoRestock(ThingGen.Create("rp_food").SetNum(5).SetLv(5)
						.Thing);
					}
					break;
				case ShopType.Farris:
					AddThing(ThingGen.CreateScroll(8220, 4 + EClass.rnd(6)));
					AddThing(ThingGen.CreateScroll(8221, 4 + EClass.rnd(6)));
					Add("drawing_paper", 10, 0);
					Add("drawing_paper2", 10, 0);
					Add("stethoscope", 1, 0);
					Add("whip_love", 1, 0);
					Add("whip_interest", 1, 0);
					break;
				case ShopType.Guild:
					if (this is TraitClerk_Merchant)
					{
						Add("flyer", 1, 0).SetNum(99);
					}
					break;
				case ShopType.Influence:
				{
					bool num2 = owner.id == "big_sister";
					TraitTicketFurniture.SetZone(num2 ? EClass.game.spatials.Find("little_garden") : EClass._zone, Add("ticket_furniture", 1, 0).SetNum(99));
					if (num2)
					{
						Add("littleball", 10, 0);
						break;
					}
					for (int k = 0; k < 10; k++)
					{
						Thing thing3 = ThingGen.Create(EClass._zone.IsFestival ? "1123" : ((EClass.rnd(3) == 0) ? "1169" : "1160"));
						thing3.DyeRandom();
						AddThing(thing3);
					}
					break;
				}
				case ShopType.Casino:
				{
					Add("chest_tax", 1, 0);
					Add("1165", 1, 0);
					Add("monsterball", 1, 0).SetNum(3).SetLv(10);
					Add("1175", 1, 0);
					Add("1176", 1, 0);
					Add("pillow_ehekatl", 1, 0);
					Add("grave_dagger1", 1, 0);
					Add("grave_dagger2", 1, 0);
					Add("434", 1, 0);
					Add("433", 1, 0);
					Add("714", 1, 0);
					Add("1017", 1, 0);
					Add("1155", 1, 0);
					Add("1011", 1, 0);
					AddThing(ThingGen.CreatePerfume(9500, 5));
					AddThing(ThingGen.CreatePerfume(9501, 5));
					AddThing(ThingGen.CreatePerfume(9502, 5));
					AddThing(ThingGen.CreatePerfume(9503, 5));
					for (int j = 0; j < 5; j++)
					{
						Thing thing2 = ThingGen.CreateFromCategory("seasoning").SetNum(10);
						thing2.elements.SetBase(2, 40);
						thing2.c_priceFix = 1000;
						AddThing(thing2);
					}
					break;
				}
				case ShopType.Medal:
					NoRestock(ThingGen.Create("sword_dragon"));
					Add("sword_dragon", 1, 0).SetReplica(on: true);
					NoRestock(ThingGen.Create("axe_destruction"));
					Add("axe_destruction", 1, 0).SetReplica(on: true);
					NoRestock(ThingGen.Create("blunt_bonehammer"));
					Add("blunt_bonehammer", 1, 0).SetReplica(on: true);
					NoRestock(ThingGen.Create("pole_gunlance"));
					Add("pole_gunlance", 1, 0).SetReplica(on: true);
					NoRestock(ThingGen.Create("sword_muramasa"));
					Add("sword_muramasa", 1, 0).SetReplica(on: true);
					NoRestock(ThingGen.Create("sword_forgetmenot"));
					Add("sword_forgetmenot", 1, 0).SetReplica(on: true);
					NoRestock(ThingGen.Create("dagger_fish"));
					Add("dagger_fish", 1, 0).SetReplica(on: true);
					NoRestock(ThingGen.Create("sword_zephir"));
					Add("sword_zephir", 1, 0).SetReplica(on: true);
					Add("ribbon", 1, 0);
					Add("helm_sage", 1, 0);
					Add("diary_sister", 1, 0);
					Add("diary_catsister", 1, 0);
					Add("diary_lady", 1, 0);
					Add("1165", 1, 0).SetNum(5);
					AddThing(ThingGen.CreateScroll(9160).SetNum(5));
					Add("monsterball", 1, 0).SetNum(3).SetLv(20);
					Add("monsterball", 1, 0).SetNum(3).SetLv(40);
					Add("bill_tax", 1, 0).c_bill = 1;
					Add("bill_tax", 1, 0).c_bill = 1;
					Add("bill_tax", 1, 0).c_bill = 1;
					Add("container_magic", 1, 0);
					Add("container_magic", 1, 0).ChangeMaterial("iron").idSkin = 1;
					Add("container_magic", 1, 0).ChangeMaterial("bamboo").idSkin = 2;
					Add("wrench_tent_elec", 1, 0);
					Add("wrench_tent_soil", 1, 0);
					Add("wrench_bed", 1, 0).SetNum(20);
					Add("wrench_storage", 1, 0).SetNum(10);
					Add("wrench_fridge", 1, 0).SetNum(1);
					Add("wrench_extend_v", 1, 0).SetNum(2);
					Add("wrench_extend_h", 1, 0).SetNum(2);
					break;
				default:
				{
					float num = (float)(3 + Mathf.Min(ShopLv / 5, 10)) + Mathf.Sqrt(ShopLv);
					num = num * (float)(100 + EClass.pc.Evalue(1406) * 5) / 100f;
					num = Mathf.Min(num, 300f);
					for (int i = 0; (float)i < num; i++)
					{
						Thing thing = CreateStock();
						if ((!(thing.trait is TraitRod) || thing.c_charges != 0) && thing.GetPrice() > 0)
						{
							t.AddThing(thing);
						}
					}
					break;
				}
				}
				foreach (RecipeSource item3 in RecipeManager.list)
				{
					if (item3.row.recipeKey.IsEmpty())
					{
						continue;
					}
					string[] recipeKey = item3.row.recipeKey;
					for (int m = 0; m < recipeKey.Length; m++)
					{
						if (recipeKey[m] == ShopType.ToString())
						{
							NoRestock(ThingGen.CreateRecipe(item3.id));
							break;
						}
					}
				}
				switch (ShopType)
				{
				case ShopType.StrangeGirl:
				{
					int num8 = (EClass.debug.enable ? 20 : (EClass._zone.development / 10));
					if (num8 > 0)
					{
						Add("syringe_gene", num8, 0);
						Add("diary_little", 1, 0);
					}
					if (num8 > 10)
					{
						Add("syringe_heaven", num8 / 5, 0);
					}
					break;
				}
				case ShopType.GeneralExotic:
					Add("tool_talisman", 1, 0);
					break;
				case ShopType.Healer:
					AddThing(ThingGen.CreatePotion(8400).SetNum(4 + EClass.rnd(6)));
					AddThing(ThingGen.CreatePotion(8401).SetNum(2 + EClass.rnd(4)));
					AddThing(ThingGen.CreatePotion(8402).SetNum(1 + EClass.rnd(3)));
					break;
				case ShopType.Food:
					Add("ration", 2 + EClass.rnd(4), 0);
					break;
				case ShopType.Ecopo:
					Add("ecomark", 5, 0);
					Add("1165", 1, 0);
					AddThing(ThingGen.CreateScroll(9160).SetNum(5));
					break;
				case ShopType.Gun:
					Add("bullet", 1, 0).SetNum(300 + EClass.rnd(100)).ChangeMaterial("iron");
					Add("bullet_energy", 1, 0).SetNum(100 + EClass.rnd(100)).ChangeMaterial("iron");
					break;
				case ShopType.Magic:
					if ((EClass._zone.id == "lumiest" && EClass._zone.lv == 0) || (EClass._zone.id != "lumiest" && EClass.rnd(4) == 0))
					{
						t.AddThing(ThingGen.Create("letter_trial"));
					}
					break;
				case ShopType.Festival:
					if (EClass._zone.IsFestival)
					{
						Add("1085", 1, 0);
						if (EClass._zone.id == "noyel")
						{
							Add("holyFeather", 1, 0);
						}
					}
					break;
				case ShopType.Junk:
				case ShopType.LoytelMart:
				{
					if (ShopType == ShopType.LoytelMart)
					{
						Add("ticket_massage", 1, 0);
						Add("ticket_armpillow", 1, 0);
						Add("ticket_champagne", 1, 0);
					}
					for (int num6 = 0; num6 < (EClass.debug.enable ? 30 : 3); num6++)
					{
						if (EClass.rnd(5) == 0)
						{
							TreasureType treasureType = ((EClass.rnd(10) == 0) ? TreasureType.BossNefia : ((EClass.rnd(10) == 0) ? TreasureType.Map : TreasureType.RandomChest));
							int num7 = EClass.rnd(EClass.rnd(ShopLv + (EClass.debug.enable ? 200 : 50)) + 1) + 1;
							Thing thing5 = ThingGen.Create(treasureType switch
							{
								TreasureType.Map => "chest_treasure", 
								TreasureType.BossNefia => "chest_boss", 
								_ => "chest3", 
							});
							thing5.c_lockedHard = true;
							thing5.c_lockLv = num7;
							thing5.c_priceAdd = 2000 + num7 * 250 * ((treasureType == TreasureType.RandomChest) ? 1 : 5);
							thing5.c_revealLock = true;
							ThingGen.CreateTreasureContent(thing5, num7, treasureType, clearContent: true);
							AddThing(thing5);
						}
					}
					break;
				}
				}
				ShopType shopType = ShopType;
				if (shopType == ShopType.General || shopType == ShopType.Food)
				{
					for (int num9 = 0; num9 < (EClass.debug.enable ? 3 : 3); num9++)
					{
						if (EClass.rnd(3) == 0)
						{
							int lv = EClass.rnd(EClass.rnd(ShopLv + (EClass.debug.enable ? 200 : 50)) + 1) + 1;
							Thing t2 = ThingGen.Create("chest_gamble", -1, lv).SetNum(1 + EClass.rnd(20));
							AddThing(t2);
						}
					}
				}
				switch (owner.id)
				{
				case "rodwyn":
					AddThing(ThingGen.CreateSpellbook(8790));
					AddThing(ThingGen.CreatePotion(8791).SetNum(3 + EClass.rnd(3)));
					break;
				case "girl_blue":
					Add("779", 1 + EClass.rnd(3), 0);
					break;
				case "nola":
					AddThing(ThingGen.CreateRecipe("ic").SetPriceFix(400));
					AddThing(ThingGen.CreateRecipe("bullet").SetPriceFix(300));
					AddThing(ThingGen.CreateRecipe("break_powder").SetPriceFix(1000));
					AddThing(ThingGen.CreateRecipe("quarrel").SetPriceFix(100));
					AddThing(ThingGen.CreateRecipe("1099").SetPriceFix(400));
					AddThing(ThingGen.CreateRecipe("detector").SetPriceFix(700));
					AddThing(ThingGen.CreatePlan(2710)).SetPriceFix(-100);
					AddThing(ThingGen.CreatePlan(2711)).SetPriceFix(-100);
					AddThing(ThingGen.CreatePlan(2712)).SetPriceFix(200);
					break;
				}
				if (Guild.Thief.IsCurrentZone)
				{
					Add("lockpick", 1, 0);
					if (EClass.rnd(2) == 0)
					{
						Add("lockpick", 1, 0);
					}
					AddThing(ThingGen.CreateScroll(8780, EClass.rndHalf(5)));
				}
				foreach (Thing thing9 in t.things)
				{
					thing9.c_idBacker = 0;
					if (ShopType != ShopType.Copy)
					{
						thing9.TryMakeRandomItem(ShopLv);
						if (thing9.Num == 1)
						{
							thing9.SetNum(thing9.trait.DefaultStock);
						}
						if (thing9.trait is TraitFoodMeal)
						{
							CraftUtil.MakeDish(thing9, ShopLv, owner.Chara);
						}
						if (thing9.IsFood && owner.id == "rodwyn")
						{
							SourceElement.Row row = EClass.sources.elements.rows.Where((SourceElement.Row e) => !e.foodEffect.IsEmpty() && e.category != "feat" && e.chance > 0).RandomItem();
							thing9.elements.SetBase(row.id, 10 + EClass.rnd(10));
						}
					}
					if (CurrencyType == CurrencyType.Casino_coin)
					{
						thing9.noSell = true;
					}
					if (Guild.Thief.IsCurrentZone)
					{
						thing9.isStolen = true;
					}
					if (!(thing9.trait is TraitErohon))
					{
						thing9.c_IDTState = 0;
					}
					if (CurrencyType == CurrencyType.Money && (thing9.category.IsChildOf("meal") || thing9.category.IsChildOf("preserved")) && thing9.id != "ration")
					{
						thing9.c_priceFix = -70;
					}
					if (thing9.trait is TraitErohon)
					{
						thing9.c_IDTState = 5;
					}
					if (thing9.IsContainer && !thing9.c_revealLock)
					{
						thing9.RemoveThings();
						t.c_lockLv = 0;
					}
				}
				if (t.things.Count <= t.things.GridSize)
				{
					return;
				}
				int num10 = t.things.width * 10;
				if (t.things.Count > num10)
				{
					int num11 = t.things.Count - num10;
					for (int num12 = 0; num12 < num11; num12++)
					{
						t.things.LastItem().Destroy();
					}
				}
				t.things.ChangeSize(t.things.width, Mathf.Min(t.things.Count / t.things.width + 1, 10));
				Thing Add(string id, int a, int idSkin)
				{
					CardBlueprint.SetNormalRarity();
					Thing thing6 = ThingGen.Create(id, -1, ShopLv).SetNum(a);
					thing6.idSkin = idSkin;
					return t.AddThing(thing6);
				}
				Thing AddThing(Thing _t)
				{
					return t.AddThing(_t);
				}
				void NoRestock(Thing _t)
				{
					HashSet<string> hashSet = EClass.player.noRestocks.TryGetValue(owner.id);
					if (hashSet == null)
					{
						hashSet = new HashSet<string>();
					}
					if (!hashSet.Contains(_t.trait.IdNoRestock))
					{
						hashSet.Add(_t.trait.IdNoRestock);
						EClass.player.noRestocks[owner.id] = hashSet;
						_t.SetInt(101, 1);
						AddThing(_t);
					}
				}
			}

			public Thing CreateStock()
			{
				switch (ShopType)
				{
				case ShopType.Dye:
				{
					Thing thing4 = ThingGen.Create("dye").SetNum(15 + EClass.rnd(30));
					thing4.ChangeMaterial(EClass.sources.materials.rows.RandomItem().alias);
					return thing4;
				}
				case ShopType.GeneralExotic:
					return FromFilter("shop_generalExotic");
				case ShopType.VMachine:
					if (EClass.rnd(10) == 0)
					{
						return Create("panty");
					}
					if (EClass.rnd(5) == 0)
					{
						return Create("234");
					}
					return FromFilter("shop_drink");
				case ShopType.Furniture:
					return FromFilter("shop_furniture");
				case ShopType.Book:
					return FromFilter("shop_book");
				case ShopType.Magic:
					return FromFilter("shop_magic");
				case ShopType.Ecopo:
				{
					Thing thing = TraitSeed.MakeRandomSeed(enc: true);
					if (EClass.rnd(2) == 0)
					{
						TraitSeed.LevelSeed(thing, (thing.trait as TraitSeed).row, 1);
					}
					return thing;
				}
				case ShopType.Healer:
				{
					Thing thing2 = null;
					for (int i = 0; i < 1000; i++)
					{
						thing2 = FromFilter("shop_healer");
						if (thing2.trait is TraitScroll { source: not null } traitScroll)
						{
							if (!(traitScroll.source.aliasParent != "WIL") && !(traitScroll.source.categorySub == "attack"))
							{
								break;
							}
						}
						else if (thing2.trait is TraitPotionRandom { source: not null } traitPotionRandom)
						{
							if (!(traitPotionRandom.source.aliasParent != "WIL") && !(traitPotionRandom.source.categorySub == "attack"))
							{
								thing2.SetNum(EClass.rnd(5) + 1);
								break;
							}
						}
						else if (thing2.trait is TraitRodRandom { source: not null } traitRodRandom && !(traitRodRandom.source.aliasParent != "WIL") && !(traitRodRandom.source.categorySub == "attack"))
						{
							break;
						}
					}
					return thing2;
				}
				case ShopType.Milk:
					if (EClass._zone is Zone_Nefu && EClass.rnd(2) == 0)
					{
						Thing thing3 = ThingGen.Create("milk");
						thing3.MakeRefFrom(EClass.sources.charas.rows.Where((SourceChara.Row r) => r.race == "mifu" || r.race == "nefu").RandomItem().model);
						Debug.Log(thing3);
						return thing3;
					}
					return Create("milk");
				case ShopType.Map:
					return ThingGen.CreateMap();
				case ShopType.Plan:
					return Create("book_plan");
				case ShopType.Weapon:
					return FromFilter("shop_weapon");
				case ShopType.Gun:
					if (EClass.rnd(8) == 0)
					{
						return Create("mod_ranged");
					}
					return FromFilter("shop_gun");
				case ShopType.Blackmarket:
				case ShopType.Exotic:
				{
					int num = 30;
					if (Guild.Thief.IsCurrentZone)
					{
						num = 25;
					}
					if (Guild.Merchant.IsCurrentZone)
					{
						num = 15;
					}
					if (EClass.debug.enable)
					{
						num = 1;
					}
					CardBlueprint.SetRarity((EClass.rnd(num * 5) == 0) ? Rarity.Mythical : ((EClass.rnd(num) == 0) ? Rarity.Legendary : ((EClass.rnd(5) == 0) ? Rarity.Superior : Rarity.Normal)));
					return FromFilter("shop_blackmarket");
				}
				case ShopType.Drink:
					return FromFilter("shop_drink");
				case ShopType.Booze:
					return FromFilter("shop_booze");
				case ShopType.Fruit:
					return FromFilter("shop_fruit");
				case ShopType.Fish:
					if (EClass.rnd(2) == 0)
					{
						return Create("bait");
					}
					if (EClass.rnd(3) == 0)
					{
						return Create("fishingRod");
					}
					return FromFilter("shop_fish");
				case ShopType.Meat:
					if (EClass.rnd(5) == 0)
					{
						return Create("seasoning");
					}
					return FromFilter("shop_meat");
				case ShopType.Bread:
					if (EClass.rnd(3) == 0)
					{
						return Create("dough");
					}
					return FromFilter("shop_bread");
				case ShopType.Sweet:
					if (EClass.rnd(3) == 0)
					{
						return Create("dough");
					}
					return FromFilter("shop_sweet");
				case ShopType.Food:
					if (EClass.rnd(5) == 0)
					{
						return Create("seasoning");
					}
					return FromFilter("shop_food");
				case ShopType.Drug:
					return FromFilter("shop_drug");
				case ShopType.LoytelMart:
					if (EClass.player.flags.loytelMartLv >= 1)
					{
						if (EClass.rnd(10) == 0)
						{
							return Create("monsterball").SetLv(40 + EClass.rnd(ShopLv)).Thing;
						}
						if (EClass.rnd(30) == 0)
						{
							return ThingGen.Create("rp_random", -1, ShopLv + 10);
						}
						if (EClass.rnd(100) == 0)
						{
							return ThingGen.Create("map_treasure", -1, EClass.rndHalf(ShopLv));
						}
						if (EClass.rnd(40) == 0)
						{
							return Create("water").SetPriceFix(1000);
						}
						if (EClass.rnd(EClass.debug.enable ? 20 : 1000) == 0)
						{
							return Create("1165");
						}
					}
					return FromFilter("shop_junk");
				case ShopType.Junk:
					return FromFilter("shop_junk");
				case ShopType.Souvenir:
					return FromFilter("shop_souvenir");
				case ShopType.StrangeGirl:
					return DNA.GenerateGene(SpawnList.Get("chara").Select(ShopLv + 10), DNA.Type.Brain);
				case ShopType.Fireworks:
					if (EClass.rnd(3) == 0)
					{
						return Create("firework_launcher");
					}
					return Create("firework");
				case ShopType.Festival:
					if (EClass.rnd(3) == 0)
					{
						if (IsFestival("olvina"))
						{
							return Create(new string[4] { "1125", "1126", "pillow_truth", "1230" }.RandomItem());
						}
						if (IsFestival("yowyn"))
						{
							return Create(new string[3] { "hat_mushroom", "hat_witch", "hat_kumiromi" }.RandomItem());
						}
						if (IsFestival("noyel"))
						{
							return Create(new string[9] { "1127", "1128", "xmas_sled", "xmas_bigbag", "xmas_bigbox", "xmas_blackcat", "xmas_blackcat", "xmas_jure", "xmas_crown" }.RandomItem());
						}
					}
					if (EClass.rnd(2) == 0)
					{
						return Create(new string[4] { "1081", "1082", "1083", "1084" }.RandomItem());
					}
					if (EClass.rnd(3) == 0)
					{
						return FromFilter("shop_junk");
					}
					return FromFilter("shop_souvenir");
				case ShopType.Lamp:
					if (EClass.rnd(3) != 0)
					{
						if (IsFestival("kapul"))
						{
							return Create(new string[6] { "999", "1000", "1001", "1002", "1003", "1004" }.RandomItem());
						}
						if (IsFestival("yowyn"))
						{
							return Create(new string[2] { "1072", "1073" }.RandomItem());
						}
						if (IsFestival("noyel"))
						{
							return Create(new string[1] { "1069" }.RandomItem());
						}
						if (IsFestival("olvina"))
						{
							return Create(new string[2] { "1070", "1071" }.RandomItem());
						}
					}
					if (EClass._zone.IsFestival && EClass.rnd(2) == 0)
					{
						return Create(new string[4] { "953", "954", "955", "956" }.RandomItem());
					}
					return FromFilter("shop_lamp");
				default:
					if (EClass.rnd(100) == 0)
					{
						return Create("lockpick");
					}
					return FromFilter("shop_general");
				}
				Thing Create(string s)
				{
					return ThingGen.Create(s, -1, ShopLv);
				}
				Thing FromFilter(string s)
				{
					return ThingGen.CreateFromFilter(s, ShopLv);
				}
				static bool IsFestival(string id)
				{
					if (EClass._zone.id == id)
					{
						return EClass._zone.IsFestival;
					}
					return false;
				}
			}
		}
