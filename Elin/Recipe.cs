using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class Recipe : EClass
{
	public enum State
	{
		Valid,
		Invalid
	}

	public class Ingredient : EClass
	{
		[JsonProperty]
		public int uid;

		[JsonProperty]
		public int req;

		[JsonProperty]
		public int mat = -1;

		[JsonProperty]
		public int refVal = -1;

		[JsonProperty]
		public string id;

		[JsonProperty]
		public string tag;

		[JsonProperty]
		public List<string> idOther = new List<string>();

		public Thing thing;

		public bool optional;

		public bool dye;

		public bool useCat;

		public bool IsThingSpecified => uid != 0;

		public string IdThing
		{
			get
			{
				if (!useCat)
				{
					return id;
				}
				return EClass.sources.categories.map[id].GetIdThing();
			}
		}

		public Thing RefreshThing()
		{
			if (thing == null)
			{
				thing = EClass.pc.things.Find(uid);
				if (thing == null)
				{
					thing = EClass._map.Stocked.Find(uid);
					if (thing == null)
					{
						thing = EClass.pc.things.Find(id, mat, refVal);
						if (thing == null)
						{
							thing = EClass._map.Stocked.Find(id, mat, refVal);
						}
					}
				}
			}
			return thing;
		}

		public bool CanSetThing(Thing t)
		{
			if (t.id != id || t.Num < req)
			{
				return false;
			}
			return true;
		}

		public void SetThing(Thing t = null)
		{
			uid = t?.uid ?? 0;
			thing = t;
			mat = t?.material.id ?? (-1);
		}

		public string GetName()
		{
			if (useCat)
			{
				string text = CatName(id);
				if (idOther.Count > 0)
				{
					foreach (string item in idOther)
					{
						text = text + ", " + CatName(item);
					}
				}
				return "ingCat".lang(text);
			}
			string text2 = EClass.sources.cards.map[id].GetName().IsEmpty(("card_" + id).lang());
			string text3 = (tag.IsEmpty() ? "" : ("(" + ("tag_" + tag).lang() + ")"));
			if (idOther.Count > 0)
			{
				foreach (string item2 in idOther)
				{
					text3 = text3 + ", " + EClass.sources.cards.map[item2].GetName();
				}
			}
			return text2 + text3;
			static string CatName(string id)
			{
				return EClass.sources.categories.map[id].GetName();
			}
		}
	}

	public static Dictionary<string, Recipe> recipeCache = new Dictionary<string, Recipe>();

	[JsonProperty]
	public string id;

	[JsonProperty]
	public List<Ingredient> ingredients = new List<Ingredient>();

	[JsonProperty]
	public bool UseStock;

	[JsonProperty]
	public bool IngAsProduct;

	[JsonProperty]
	public bool VirtualBlock;

	public int _dir;

	public int sync;

	public int idMat = -1;

	public int idSkin;

	public State state;

	public RecipeSource _source;

	public RecipeSource source => _source ?? (_source = RecipeManager.Get(id));

	public SourceMaterial.Row DefaultMaterial => renderRow.DefaultMaterial;

	public virtual TileRow tileRow => source.row as TileRow;

	public virtual RenderRow renderRow => source.row;

	public string Name => GetName();

	public virtual TileType tileType
	{
		get
		{
			if (!(source.type == "Bridge") || source.row.tileType is TileTypeBridge)
			{
				return renderRow.tileType;
			}
			return TileType.Bridge;
		}
	}

	public virtual bool IsFloorOrBridge => tileType.IsFloorOrBridge;

	public int MaxAltitude
	{
		get
		{
			if (IsThing || IsBridge || tileType.blockRenderMode == BlockRenderMode.Pillar)
			{
				return tileType.MaxAltitude;
			}
			return 0;
		}
	}

	public int RecipeLv => EClass.player.recipes.knownRecipes.TryGetValue(id, 0);

	public bool IsFloor => tileType.IsFloor;

	public bool IsObj => tileRow is SourceObj.Row;

	public virtual bool IsBlock => !IsFloorOrBridge;

	public bool IsWallOrFence => tileType.IsWallOrFence;

	public bool IsBridge => source.type == "Bridge";

	public virtual bool IsThing => false;

	public virtual Card Mold => null;

	public virtual int W
	{
		get
		{
			if (_dir % 2 != 0)
			{
				return renderRow.H;
			}
			return renderRow.W;
		}
	}

	public virtual int H
	{
		get
		{
			if (_dir % 2 != 0)
			{
				return renderRow.W;
			}
			return renderRow.H;
		}
	}

	public virtual bool RequireIngredients => true;

	public bool MultiSize
	{
		get
		{
			if (W == 1)
			{
				return H != 1;
			}
			return true;
		}
	}

	public BaseTileSelector tileSelector => EClass.screen.tileSelector;

	public virtual int CostMoney => 0;

	public virtual bool CanRotate
	{
		get
		{
			if (tileType.CanRotate(buildMode: true))
			{
				return tileRow._tiles.Length > 1;
			}
			return false;
		}
	}

	public static Recipe GetOrCreate(string id)
	{
		Recipe recipe = recipeCache.TryGetValue(id);
		if (recipe == null)
		{
			RecipeSource recipeSource = RecipeManager.dict.TryGetValue(id);
			if (recipeSource != null)
			{
				recipe = Create(recipeSource);
				recipeCache.Add(id, recipe);
			}
		}
		return recipe;
	}

	public static Recipe Create(RecipeSource _source, int idMat = -1, Thing ing = null)
	{
		string type = _source.type;
		Recipe recipe = (_source.isBridgePillar ? new RecipeBridgePillar() : (type.IsEmpty() ? new RecipeCard() : ((type == "Custom") ? new RecipeCustom() : new Recipe())));
		recipe.id = _source.id;
		recipe._source = _source;
		recipe.idMat = idMat;
		if (ing != null)
		{
			recipe.BuildIngredientList(ing);
			if (ing.trait is TraitTile)
			{
				recipe.VirtualBlock = true;
			}
		}
		return recipe;
	}

	public static Recipe Create(Thing t)
	{
		RecipeSource recipeSource = RecipeManager.dict.TryGetValue(t.id);
		if (recipeSource == null)
		{
			return null;
		}
		RecipeCard recipeCard = new RecipeCard
		{
			id = recipeSource.id,
			_source = recipeSource,
			IngAsProduct = true,
			idSkin = t.idSkin
		};
		recipeCard.BuildIngredientList(t);
		if (t.uid == 0 || recipeCard.ingredients[0].uid == 0 || recipeCard.ingredients[0].thing == null)
		{
			Debug.LogError("expection: invalid ingredient " + t);
		}
		return recipeCard;
	}

	public static void DropIngredients(string id, string idMatMain, int num = 1)
	{
		RecipeManager.BuildList();
		Recipe orCreate = GetOrCreate(EClass.sources.cards.map[id].RecipeID);
		orCreate.BuildIngredientList();
		Msg.Say("dropReward");
		foreach (Ingredient ingredient in orCreate.ingredients)
		{
			Thing thing = ThingGen.Create(ingredient.id).SetNum(ingredient.req);
			if (ingredient == orCreate.ingredients[0])
			{
				thing.ChangeMaterial(idMatMain);
				thing.SetNum(num);
			}
			EClass.player.DropReward(thing, silent: true);
		}
	}

	public bool IsStaticLV()
	{
		switch (source.idFactory)
		{
		case "factory_floor":
		case "factory_block":
		case "factory_wall":
		case "factory_platform":
			return true;
		default:
			return renderRow.Category.IsChildOf("block");
		}
	}

	public int GetQualityBonus()
	{
		if (IsStaticLV())
		{
			return 0;
		}
		Element reqSkill = source.GetReqSkill();
		int num = reqSkill.Value - EClass.pc.Evalue(reqSkill.id);
		int num2 = 0;
		if (num > 0)
		{
			if (num < 5)
			{
				return 0;
			}
			return -(num - 4) * 10;
		}
		num2 += EClass.curve(-num, 10, 20, 80) / 10 * 10 + 10;
		if (RecipeLv > 0)
		{
			num2 += (int)Mathf.Sqrt(RecipeLv - 1) * 10;
		}
		return num2;
	}

	public virtual int GetRefVal()
	{
		if (source.row is SourceBlock.Row)
		{
			return (source.row as SourceBlock.Row).id;
		}
		if (source.row is SourceFloor.Row)
		{
			return (source.row as SourceFloor.Row).id;
		}
		if (source.row is SourceObj.Row)
		{
			return (source.row as SourceObj.Row).id;
		}
		return -1;
	}

	public virtual string GetIdThing()
	{
		if (source.row is SourceBlock.Row)
		{
			return (source.row as SourceBlock.Row).idThing;
		}
		if (source.row is SourceFloor.Row)
		{
			if (!IsBridge)
			{
				return "floor";
			}
			return "platform";
		}
		if (source.row is SourceObj.Row)
		{
			return "obj";
		}
		return null;
	}

	public virtual bool ShouldShowHighlight(HitResult r)
	{
		return r != HitResult.Valid;
	}

	public BaseTileSelector.BoxType GetBoxType()
	{
		if (tileType.BoxType != BaseTileSelector.BoxType.Fence)
		{
			return BaseTileSelector.BoxType.Box;
		}
		return BaseTileSelector.BoxType.Fence;
	}

	public int GetDefaultColor()
	{
		if (UseStock)
		{
			Thing thing = ingredients[0].thing;
			return renderRow.GetColorInt(thing.isDyed ? thing.DyeMat : thing.material);
		}
		return renderRow.GetColorInt(DefaultMaterial);
	}

	public virtual SourceMaterial.Row GetMainMaterial()
	{
		int num = -1;
		if (ingredients.Count > 0)
		{
			num = ((ingredients[0].thing == null) ? ingredients[0].mat : ingredients[0].thing.material.id);
		}
		if (num == -1)
		{
			num = 3;
		}
		return EClass.sources.materials.rows[num];
	}

	public virtual SourceMaterial.Row GetColorMaterial()
	{
		if (idMat != -1)
		{
			return EClass.sources.materials.rows[idMat];
		}
		if (UseStock)
		{
			ingredients[0].RefreshThing();
			Thing thing = ingredients[0].thing;
			if (thing != null)
			{
				if (!thing.isDyed)
				{
					return thing.material;
				}
				return thing.DyeMat;
			}
			return EClass.sources.materials.rows[3];
		}
		int num = ((ingredients.Count > 0) ? ingredients[source.colorIng].mat : 3);
		if (num == -1)
		{
			num = 3;
		}
		return EClass.sources.materials.rows[num];
	}

	public virtual void BuildIngredientList()
	{
		if (!UseStock && RequireIngredients)
		{
			ingredients = source.GetIngredients();
		}
	}

	public void BuildIngredientList(Thing t)
	{
		UseStock = true;
		Ingredient item = new Ingredient
		{
			id = t.id,
			tag = null,
			req = 1,
			uid = t.uid,
			thing = t,
			mat = t.material.id
		};
		ingredients.Add(item);
	}

	public virtual void OnChangeIngredient()
	{
	}

	public virtual Thing Craft(BlessedState blessed, bool sound = false, List<Thing> ings = null, TraitCrafter crafter = null, bool model = false)
	{
		Thing thing = null;
		string type = source.type;
		thing = ((type == "Block") ? ThingGen.CreateBlock(tileRow.id, GetMainMaterial().id) : ((!(type == "Obj")) ? ThingGen.CreateFloor(tileRow.id, GetMainMaterial().id, source.isBridge) : ThingGen.CreateObj(tileRow.id, GetMainMaterial().id)));
		if (thing == null)
		{
			return null;
		}
		thing.trait.OnCrafted(this);
		thing.SetBlessedState(blessed);
		if (model)
		{
			return thing;
		}
		Msg.Say("crafted", thing);
		thing = EClass.pc.AddCard(thing) as Thing;
		if (thing.GetRootCard() == EClass.pc)
		{
			EClass.pc.HoldCard(thing);
		}
		return thing;
	}

	public bool HasSameTile(Point point, int dir, int altitude, int bridgeHeight)
	{
		if (IsObj)
		{
			if (point.growth != null && point.sourceObj == tileRow && point.cell.matObj == GetColorMaterial())
			{
				return true;
			}
			if (dir == point.cell.objDir && point.cell.matObj == GetColorMaterial() && point.sourceObj == tileRow)
			{
				return true;
			}
		}
		else if (IsBridge)
		{
			if (dir == point.cell.floorDir && point.matBridge == GetColorMaterial() && point.sourceBridge == tileRow && Mathf.Clamp(bridgeHeight + altitude, 0, 255) == point.cell.bridgeHeight)
			{
				return true;
			}
		}
		else if (IsFloorOrBridge)
		{
			if (dir == point.cell.floorDir && point.matFloor == GetColorMaterial() && point.sourceFloor == tileRow)
			{
				return true;
			}
		}
		else if (IsWallOrFence)
		{
			if (point.matBlock == GetColorMaterial() && point.sourceBlock == tileRow && dir == point.cell.blockDir)
			{
				return true;
			}
		}
		else if (IsBlock && point.matBlock == GetColorMaterial() && point.sourceBlock == tileRow)
		{
			if (point.sourceBlock.tileType.AltitudeAsDir && dir != point.cell.objDir)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	public virtual void Build(TaskBuild task)
	{
		Build(task.owner, null, task.pos, (ingredients.Count > 0) ? ingredients[0].mat : task.recipe.idMat, task.dir, task.altitude, task.bridgeHeight);
	}

	public virtual void Build(Chara chara, Card t, Point pos, int mat, int dir, int altitude, int bridgeHeight)
	{
		if (mat == -1)
		{
			mat = 2;
		}
		EClass.pc.PlaySound(EClass.sources.materials.rows[mat].GetSoundImpact());
		pos.cell.isModified = true;
		switch (source.type)
		{
		case "Block":
		{
			int ramp = tileRow.id;
			if (ramp == 3)
			{
				ramp = EClass.sources.materials.rows[mat].ramp;
			}
			if (EClass.scene.actionMode.IsRoofEditMode())
			{
				EClass._map.SetRoofBlock(pos.x, pos.z, mat, ramp, dir, altitude);
				break;
			}
			if (pos.HasObj && !pos.sourceObj.tileType.CanBuiltOnBlock)
			{
				EClass._map.MineObjSound(pos);
				EClass._map.SetObj(pos.x, pos.z);
			}
			EClass._map.SetBlock(pos.x, pos.z, mat, ramp, dir);
			if (tileType.IsBlockPass && pos.HasChara)
			{
				foreach (Chara item in pos.ListCharas())
				{
					chara.Kick(item, ignoreSelf: false, karmaLoss: false);
				}
			}
			if (tileType.AltitudeAsDir)
			{
				pos.cell.objDir = dir;
			}
			{
				foreach (Card item2 in pos.ListCards())
				{
					if (item2.isThing && item2.placeState == PlaceState.roaming)
					{
						item2._Move(item2.pos.GetNearestPoint());
					}
				}
				break;
			}
		}
		case "Floor":
			if (pos.sourceObj.tileType.RemoveOnFloorChange && (!BuildMenu.Instance || !EClass.debug.ignoreBuildRule))
			{
				EClass._map.SetObj(pos.x, pos.z);
			}
			EClass._map.SetFloor(pos.x, pos.z, mat, tileRow.id, dir);
			break;
		case "Bridge":
			if (pos.sourceObj.tileType.RemoveOnFloorChange)
			{
				EClass._map.SetObj(pos.x, pos.z);
			}
			EClass._map.SetBridge(pos.x, pos.z, Mathf.Clamp(bridgeHeight + altitude, 0, 255), mat, tileRow.id, dir);
			break;
		case "Obj":
			EClass._map.SetObj(pos.x, pos.z, mat, tileRow.id, 1, dir);
			if (tileType.ChangeBlockDir)
			{
				EClass._map.SetBlockDir(pos.x, pos.z, dir);
			}
			if (pos.growth != null)
			{
				pos.growth.SetDefaultStage();
			}
			break;
		}
	}

	public virtual void OnRenderMarker(Point point, bool active, HitResult result, bool main, int dir, int bridgeHeight)
	{
		if (dir != -1)
		{
			_dir = dir;
		}
		RenderParam renderParam = tileRow.GetRenderParam(DefaultMaterial, _dir, point, bridgeHeight);
		renderParam.matColor = (active ? EClass.Colors.blockColors.Active : EClass.Colors.blockColors.Inactive);
		point.ApplyAnime(renderParam);
		if (tileType.IsWater)
		{
			if (EClass.screen.guide.isActive)
			{
				EClass.screen.guide.passGuideFloor.Add(point, (float)result, 0.3f);
			}
			return;
		}
		if (tileType.IsWallOrFence && point.HasNonWallBlock)
		{
			Vector3 v = point.Position();
			if (EClass.screen.guide.isActive)
			{
				EClass.screen.guide.passGuideBlock.Add(ref v, 5f);
			}
		}
		if (renderRow.renderData.pass == EClass.screen.tileMap.passBlock || renderRow.renderData.pass == EClass.screen.tileMap.passBlockEx || renderRow.renderData.pass == EClass.screen.tileMap.passLiquid)
		{
			if (bridgeHeight > 0 && !point.cell.skipRender && EClass.screen.guide.isActive)
			{
				EClass.screen.guide.passGuideFloor.Add(point, (float)result, 0.3f);
			}
			if (!tileType.IsWallOrFence && (point.HasChara || (point.HasObj && point.cell.blocked)))
			{
				Vector3 v2 = point.Position();
				if (EClass.screen.guide.isActive)
				{
					EClass.screen.guide.passGuideBlock.Add(ref v2, 5f);
				}
				return;
			}
			if (EClass.scene.actionMode.IsRoofEditMode())
			{
				EClass.screen.tileMap.SetRoofHeight(renderParam, point.cell, point.x, point.z, 0, bridgeHeight, tileType.IsWallOrFence ? _dir : (-1), ignoreAltitudeY: true);
			}
			renderParam.y = renderParam.y - EClass.screen.tileMap.rendererBlockMarker.offset.y + renderRow.renderData.offset.y;
			renderParam.z += EClass.setting.render.tileMarkerZ;
			int num = ((tileType.blockRenderMode != BlockRenderMode.Pillar) ? 1 : (_dir + 1 + ((_dir >= 7) ? _dir : 0)));
			if (num == 1)
			{
				EClass.screen.tileMap.rendererBlockMarker.Draw(renderParam);
			}
			else
			{
				EClass.screen.tileMap.rendererBlockMarker.DrawRepeat(renderParam, num, tileType.RepeatSize);
			}
		}
		else if (renderRow.renderData.pass == EClass.screen.tileMap.passFloor || renderRow.renderData.pass == EClass.screen.tileMap.passFloorEx || renderRow.renderData.pass == EClass.screen.tileMap.passFloorWater)
		{
			if (point.HasObj)
			{
				_ = ref point.Position();
			}
			renderParam.z += ((point.cell.liquidLv > 0) ? (-0.01f) : EClass.setting.render.tileMarkerZFloor);
			EClass.screen.tileMap.rendererFloorMarker.Draw(renderParam);
		}
		else
		{
			renderParam.z += EClass.setting.render.tileMarkerZ;
			renderRow.renderData.Draw(renderParam);
		}
	}

	public string GetName()
	{
		if (UseStock && ingredients[0].thing == null)
		{
			return "";
		}
		string text = (UseStock ? ingredients[0].thing.Name : source.Name);
		if (text == "")
		{
			text = ("card_" + source.id).lang();
		}
		if (text == "*r")
		{
			text = source.row.GetText("aka");
		}
		if (text == "*r")
		{
			text = "(" + source.row.GetField<string>("id") + ")";
		}
		return text.ToTitleCase();
	}

	public virtual string GetDetail()
	{
		return renderRow.GetText("detail").IsEmpty(source.GetDetail());
	}

	public virtual void WriteNote(UINote n)
	{
		n.Clear();
		n.Build();
	}

	public void WriteReqFactory(UINote n, bool hasFactory = true)
	{
		if (source.NeedFactory)
		{
			n.AddHeaderTopic("reqFactory".lang(source.NameFactory.TagColor(hasFactory ? FontColor.Default : FontColor.Bad)));
			n.Space(8);
		}
	}

	public void WriteReqSkill(UINote n)
	{
		n.AddHeaderTopic("reqSkill");
		Element reqSkill = source.GetReqSkill();
		int value = EClass.pc.elements.GetOrCreateElement(reqSkill).Value;
		Color textColor = EClass.Colors.Skin.GetTextColor((value >= reqSkill.Value) ? FontColor.Good : FontColor.Warning);
		n.AddText(null, reqSkill.Name + " " + reqSkill.Value + " (" + value + ")", textColor).text1.SetSize(-1);
		n.Space(8);
	}

	public virtual IRenderer GetRenderer()
	{
		return tileRow.renderData;
	}

	public virtual void OnSelected()
	{
	}

	public virtual void OnChangeAltitude(int a)
	{
	}

	public virtual void Rotate()
	{
		if (EInput.isShiftDown || Input.GetMouseButton(1))
		{
			_dir--;
		}
		else
		{
			_dir++;
		}
		if (tileType.AltitudeAsDir)
		{
			if (_dir >= tileType.MaxAltitude)
			{
				_dir = 0;
			}
			if (_dir < 0)
			{
				_dir = tileType.MaxAltitude - 1;
			}
		}
		else
		{
			int num = tileRow._tiles.Length;
			if (tileRow.tileType == TileType.Door)
			{
				num = 2;
			}
			if (_dir < 0)
			{
				_dir = num - 1;
			}
			if (_dir >= num)
			{
				_dir = 0;
			}
		}
		if ((bool)BuildMenu.Instance)
		{
			BuildMenu.Instance.info1.OnRotate();
		}
	}

	public virtual void SetDir(int d)
	{
		_dir = d;
		if ((bool)BuildMenu.Instance)
		{
			BuildMenu.Instance.info1.OnRotate();
		}
	}

	public virtual Recipe Duplicate()
	{
		return IO.DeepCopy(this);
	}

	public virtual void SetImage(Image icon)
	{
		renderRow.SetImage(icon, null, renderRow.GetColorInt((ingredients != null && ingredients.Count > 0 && ingredients[0].thing != null) ? ingredients[0].thing.material : DefaultMaterial), setNativeSize: true, 0, idSkin);
	}

	public bool IsCraftable()
	{
		foreach (Ingredient ingredient in ingredients)
		{
			if (!ingredient.optional && EClass._map.Stocked.ListThingStack(ingredient, StockSearchMode.AroundPC).max < ingredient.req)
			{
				return false;
			}
		}
		return true;
	}

	public int GetMaxCount()
	{
		int num = 999;
		for (int i = 0; i < ingredients.Count; i++)
		{
			Ingredient ingredient = ingredients[i];
			Thing thing = ingredient.thing;
			int num2 = 0;
			if (!ingredient.optional || thing != null)
			{
				if (thing != null && !thing.isDestroyed)
				{
					num2 = thing.Num / ingredient.req;
				}
				if (num2 < num)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	public void SetTextDifficulty(UIText text)
	{
		Element reqSkill = source.GetReqSkill();
		_ = EClass.pc.elements.GetOrCreateElement(reqSkill).Value;
		string text2 = reqSkill.Name + " " + reqSkill.Value;
		text.SetText(text2);
	}

	public int GetSortVal()
	{
		Element reqSkill = source.GetReqSkill();
		return reqSkill.source.id * 10000 - reqSkill.Value;
	}

	public bool HasFirstTimeBonus()
	{
		if (EClass.player.recipes.craftedRecipes.Contains(id))
		{
			return false;
		}
		return true;
	}

	public void TryGetFirstTimeBonus()
	{
		if (HasFirstTimeBonus())
		{
			EClass.player.recipes.craftedRecipes.Add(id);
			Element reqSkill = source.GetReqSkill();
			int num = 1 + ((!IsStaticLV()) ? (reqSkill.Value / 20) : 0);
			Msg.Say("firstTimeCraft", Name);
			EClass.pc.Pick(ThingGen.Create("ticket_fortune").SetNum(num));
		}
	}
}
