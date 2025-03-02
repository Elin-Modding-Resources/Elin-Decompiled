using UnityEngine;

public class GrowSystem : EClass
{
	public class Stage
	{
		public int[] tiles;

		public int idx;

		public int tileEx;

		public RenderData renderData;

		public bool harvest;

		public void SetTile(int _tile)
		{
			tiles = new int[Convert(_tile)];
		}

		public void SetTile(int tileIdx, params int[] _tiles)
		{
			tiles = new int[_tiles.Length];
			for (int i = 0; i < tiles.Length; i++)
			{
				tiles[i] = Convert(_tiles[i] + tileIdx);
			}
		}

		public int Convert(int tile)
		{
			return renderData.ConvertTile(tile);
		}
	}

	public static SourceObj.Row[] sourceSnowTree = new SourceObj.Row[2]
	{
		EClass.sources.objs.map[54],
		EClass.sources.objs.map[55]
	};

	public const int DivStage = 30;

	public static Cell cell;

	public static Stage currentStage;

	public SourceObj.Row source;

	public Stage[] stages;

	public int[] baseTiles;

	public int harvestTile;

	public int afterHarvestTile;

	public string idHarvestThing;

	public virtual RenderData RenderHarvest => source.renderData;

	public virtual int Step => 5;

	public virtual int StageLength => 5;

	public virtual int DefaultStage => Mathf.Clamp(StageLength - 2, 0, StageLength - 1);

	public virtual int HarvestStage => -1;

	public virtual int AutoMineStage => HarvestStage;

	protected virtual int HaltStage => HarvestStage;

	protected virtual bool DrawHarvestOnTop => false;

	protected virtual bool UseGenericFirstStageTile => true;

	protected virtual bool WitherOnLastStage => true;

	protected virtual bool CanRegrow => true;

	public virtual int ShadowStage => 1;

	public virtual bool IsTree => false;

	public virtual bool IsCrimeToHarvest => false;

	public virtual float MtpProgress => 1f;

	public virtual bool NeedSunlight => true;

	public virtual AnimeID AnimeProgress => AnimeID.HitObj;

	public Stage stage => stages[cell.objVal / 30];

	public virtual bool IsMature => false;

	public bool IsLastStage()
	{
		return stage.idx == StageLength - 1;
	}

	protected virtual bool IsHarvestStage(int idx)
	{
		return idx == HarvestStage;
	}

	public virtual bool BlockSight(Cell cell)
	{
		return false;
	}

	public virtual bool BlockPass(Cell cell)
	{
		return false;
	}

	public virtual string GetSoundProgress()
	{
		return "Material/grass";
	}

	public virtual Stage CreateStage()
	{
		return new Stage();
	}

	public void Init(SourceObj.Row _row)
	{
		source = _row;
		stages = new Stage[StageLength];
		string[] growth = _row._growth;
		string[] array = growth[1].Split('/');
		baseTiles = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			baseTiles[i] = array[i].ToInt();
		}
		if (growth.Length > 2)
		{
			harvestTile = RenderHarvest.ConvertTile(growth[2].ToInt());
			idHarvestThing = growth[3];
		}
		for (int j = 0; j < StageLength; j++)
		{
			Stage stage = CreateStage();
			stages[j] = stage;
			stage.idx = j;
			if (j == 0 && UseGenericFirstStageTile)
			{
				SetGenericFirstStageTile(stage);
			}
			else
			{
				SetStageTile(stage);
			}
			if (harvestTile != 0 && IsHarvestStage(j))
			{
				stage.harvest = true;
			}
		}
	}

	public virtual void SetGenericFirstStageTile(Stage s)
	{
		s.renderData = EClass.core.refs.renderers.objS_flat;
		s.SetTile(0, 100, 101, 102);
	}

	public virtual void SetStageTile(Stage s)
	{
		s.renderData = source.renderData;
		s.SetTile(s.idx + (UseGenericFirstStageTile ? (-1) : 0), baseTiles);
	}

	public virtual int GetStageTile()
	{
		return currentStage.tiles[cell.objDir % currentStage.tiles.Length];
	}

	public virtual int GetShadow(int index)
	{
		return -1;
	}

	public virtual void OnRenderTileMap(RenderParam p)
	{
		int num = cell.objVal / 30;
		currentStage = stages[num];
		SourceObj.Row row = source;
		float y = p.y;
		float z = p.z;
		if (num != 0 || !UseGenericFirstStageTile)
		{
			p.y += row.pref.y;
			p.z += row.pref.z;
		}
		if (currentStage.harvest)
		{
			if (cell.isHarvested || harvestTile == 0)
			{
				p.tile = ((afterHarvestTile != 0) ? afterHarvestTile : GetStageTile());
				currentStage.renderData.Draw(p);
			}
			else
			{
				if (DrawHarvestOnTop)
				{
					p.tile = GetStageTile();
					currentStage.renderData.Draw(p);
					p.liquidLv = 0;
				}
				p.tile = harvestTile;
				RenderHarvest.Draw(p);
			}
		}
		else
		{
			p.tile = GetStageTile();
			currentStage.renderData.Draw(p);
		}
		p.y = y;
		p.z = z;
		if (num >= ShadowStage)
		{
			int shadow = GetShadow(num);
			if (shadow == -1)
			{
				shadow = row.pref.shadow;
			}
			if (shadow > 1 && !cell.ignoreObjShadow)
			{
				EClass.screen.tileMap.passShadow.AddShadow(p.x + row.renderData.offsetShadow.x, p.y + row.renderData.offsetShadow.y, p.z + row.renderData.offsetShadow.z, ShadowData.Instance.items[shadow], row.pref, 0, p.snow);
			}
		}
	}

	public void SetRandomStage()
	{
		int num = Rand.rnd(stages.Length);
		if (num == 0 && EClass.rnd(5) != 0 && stages.Length >= 2)
		{
			num++;
		}
		else if (num == stages.Length - 1 && EClass.rnd(5) != 0 && stages.Length >= 2)
		{
			num--;
		}
		cell.objVal = (byte)(num * 30);
	}

	public void SetDefaultStage()
	{
		cell.objVal = (byte)(DefaultStage * 30);
	}

	public virtual void OnSetObj()
	{
	}

	public bool CanGrow(VirtualDate date)
	{
		if (source.id == 0)
		{
			return false;
		}
		if (cell.HasBlock && !cell.sourceBlock.tileType.IsFence)
		{
			return false;
		}
		if (NeedSunlight)
		{
			if (date.sunMap == null)
			{
				date.BuildSunMap();
			}
			if ((cell.HasRoof || date.IsWinter || EClass._map.IsIndoor) && !date.sunMap.Contains(cell.index))
			{
				return false;
			}
		}
		return true;
	}

	public void TryGrow(VirtualDate date)
	{
		if (CanGrow(date))
		{
			Grow();
		}
	}

	public void EqualizePlants(Point pos)
	{
		PlantData p1 = EClass._map.TryGetPlant(pos);
		if (p1 == null || p1.seed == null)
		{
			return;
		}
		pos.ForeachNeighbor(delegate(Point pos2)
		{
			Thing thing = EClass._map.TryGetPlant(pos2)?.seed;
			if (thing == null)
			{
				Thing installed = pos2.Installed;
				if (installed != null && installed.id == p1.seed.id)
				{
					thing = installed;
				}
			}
			if (thing != null && p1.seed.refVal == thing.refVal && thing.encLV >= p1.seed.encLV)
			{
				p1.seed = thing;
			}
		}, diagonal: false);
	}

	public void Grow(int mtp = 1)
	{
		bool flag = cell.isWatered || (cell.IsTopWater && source.tag.Contains("flood"));
		PlantData plantData = EClass._map.TryGetPlant(cell);
		if (plantData != null && flag)
		{
			plantData.water++;
		}
		int num = Step * mtp * ((!flag) ? 1 : 2);
		int num2 = cell.objVal / 30;
		int num3 = (cell.objVal + num) / 30;
		if (num2 != num3)
		{
			if (EClass.player.isAutoFarming && num2 == HarvestStage && CanHarvest())
			{
				PopHarvest(null);
			}
			if (EClass.player.isAutoFarming && num2 >= AutoMineStage)
			{
				Point point = cell.GetPoint();
				EqualizePlants(point);
				Thing thing = TryPopSeed(null);
				PopMineObj();
				if (thing != null)
				{
					point.SetObj();
					EClass._zone.AddCard(thing, point).Install();
				}
			}
			else if (num2 == StageLength - 1)
			{
				if (EClass.player.isAutoFarming)
				{
					Point point2 = cell.GetPoint();
					EqualizePlants(point2);
					Thing thing2 = TryPopSeed(null);
					if (thing2 != null)
					{
						point2.SetObj();
						EClass._zone.AddCard(thing2, point2).Install();
					}
				}
				else
				{
					OnExceedLastStage();
				}
			}
			else
			{
				OnReachNextStage();
			}
		}
		else
		{
			cell.objVal += (byte)num;
		}
		cell.Refresh();
		EClass._map.RefreshFOV(cell.x, cell.z);
	}

	public virtual void OnReachNextStage()
	{
		SetStage(cell.objVal / 30 + 1);
	}

	public virtual void OnExceedLastStage()
	{
		cell.objVal = (byte)(stages.Length * 30 - 1);
		if (!CanRegrow || Rand.rnd(2) != 0)
		{
			return;
		}
		Point point = cell.GetPoint();
		if (point.IsFarmField)
		{
			return;
		}
		if (Rand.rnd(2) == 0)
		{
			Point randomNeighbor = point.GetRandomNeighbor();
			if (randomNeighbor.cell.CanGrowWeed)
			{
				randomNeighbor.SetObj(cell.obj);
				point.SetObj();
				return;
			}
		}
		cell.objVal = 0;
		cell.isHarvested = false;
		cell.isWatered = cell.HasLiquid;
	}

	public void SetStage(int idx, bool renewHarvest = false)
	{
		cell.objVal = (byte)(idx * 30);
		cell.gatherCount = 0;
		if (cell.HasLiquid)
		{
			cell.isWatered = true;
		}
		if (renewHarvest)
		{
			cell.isHarvested = true;
		}
		if (cell.sourceObj.id == 118)
		{
			EClass._zone.dirtyElectricity = true;
		}
		OnSetStage(idx);
	}

	public virtual void OnSetStage(int idx)
	{
	}

	public bool HaltGrowth()
	{
		return stage.idx == HarvestStage;
	}

	public void Perish()
	{
		if (EClass.player.isAutoFarming)
		{
			if (CanHarvest())
			{
				PopHarvest(null);
			}
			Thing thing = TryPopSeed(null);
			if (thing != null)
			{
				EClass._zone.AddCard(thing, cell.GetPoint()).Install();
			}
			EClass._map.SetObj(cell.x, cell.z);
			return;
		}
		if (WitherOnLastStage && stage.idx == StageLength - 2)
		{
			SetStage(StageLength - 1);
			return;
		}
		if (EClass.rnd(3) == 0)
		{
			Thing thing2 = TryPopSeed(null);
			if (thing2 != null)
			{
				EClass._zone.AddCard(thing2, cell.GetPoint()).Install();
			}
		}
		EClass._map.SetObj(cell.x, cell.z);
	}

	public bool IsWithered()
	{
		if (WitherOnLastStage && stage.idx == StageLength - 1)
		{
			return true;
		}
		return false;
	}

	public bool CanHarvest()
	{
		if (!stage.harvest || cell.isHarvested)
		{
			return false;
		}
		return true;
	}

	public virtual bool CanReapSeed()
	{
		return CanHarvest();
	}

	public virtual void OnHit(Chara c)
	{
	}

	public virtual void OnHitFail(Chara c)
	{
	}

	public void OnProgressComplete(Chara c)
	{
		EClass._map.MineObj(cell.GetPoint(), null, c);
	}

	public void PopMineObj(Chara c = null)
	{
		if (!EClass._zone.IsUserZone || !EClass.game.principal.disableUsermapBenefit)
		{
			OnMineObj(c);
			TryPopSeed(c);
		}
	}

	public virtual void OnMineObj(Chara c = null)
	{
		if (!IsWithered())
		{
			int num = cell.sourceObj.components.Length - 1;
			Thing t = ThingGen.Create(cell.sourceObj.components[num].Split('/')[0], cell.matObj_fixed.alias);
			TryPick(cell, t, c);
		}
	}

	public void TryPick(Cell cell, Thing t, Chara c, bool applySeed = false)
	{
		if (applySeed)
		{
			ApplySeed(t);
		}
		if (EClass.player.isAutoFarming)
		{
			if (EClass.rnd(4) != 0)
			{
				if (t.Num > 1)
				{
					t.SetNum(t.Num / 2);
				}
				else if (EClass.rnd(2) == 0)
				{
					return;
				}
				if (!EClass._zone.TryAddThingInSharedContainer(t))
				{
					EClass.game.cards.container_shipping.AddCard(t);
				}
			}
		}
		else
		{
			EClass._map.TrySmoothPick(cell, t, c);
		}
	}

	public void TryPick(Cell cell, string idThing, int idMat = -1, int num = 1, bool applySeed = false)
	{
		if (num > 0)
		{
			TryPick(cell, ThingGen.Create(idThing, idMat).SetNum(num), EClass.pc, applySeed);
		}
	}

	public Thing TryPopSeed(Chara c)
	{
		if (source.HasTag(CTAG.seed))
		{
			int num = (cell.IsFarmField ? 400 : 1000);
			int soilCost = EClass._zone.GetSoilCost();
			int maxSoil = EClass._zone.MaxSoil;
			if (soilCost > maxSoil)
			{
				num += (soilCost - maxSoil) * 10;
			}
			if (IsWithered())
			{
				num /= 5;
			}
			if (EClass._zone.IsUserZone && EClass.game.principal.disableUsermapBenefit)
			{
				return null;
			}
			if (EClass.player.isAutoFarming || EClass.rnd(num) < EClass.rnd(source.chance))
			{
				Thing thing = TraitSeed.MakeSeed(source, EClass._map.TryGetPlant(cell));
				if (EClass.player.isAutoFarming)
				{
					return thing;
				}
				TryPick(cell, thing, c);
				return thing;
			}
		}
		return null;
	}

	public void Harvest(Chara c)
	{
		cell.isHarvested = true;
		cell.gatherCount = 0;
		PopHarvest(c);
	}

	public void PopHarvest(Chara c, string idThing, int num = -1)
	{
		PopHarvest(c, ThingGen.Create(idThing), num);
	}

	public void PopHarvest(Chara c, Thing t = null, int num = -1)
	{
		if (t == null)
		{
			t = ((!idHarvestThing.StartsWith('#')) ? ThingGen.Create(idHarvestThing.IsEmpty("apple")) : ThingGen.CreateFromCategory(idHarvestThing.Replace("#", "")));
		}
		ApplySeed(t);
		PlantData plantData = EClass._map.TryGetPlant(cell);
		if (plantData != null && plantData.size > 0)
		{
			t.c_weight = t.SelfWeight * (80 + plantData.size * plantData.size * 100) / 100;
			t.SetBool(115, enable: true);
			t.isWeightChanged = true;
		}
		t.SetBlessedState(BlessedState.Normal);
		if (source._growth.Length > 4)
		{
			int num2 = EClass.rnd(source._growth[4].ToInt()) + 1;
			int soilCost = EClass._zone.GetSoilCost();
			int maxSoil = EClass._zone.MaxSoil;
			if (soilCost > maxSoil && EClass.player.stats.days >= 5)
			{
				num2 -= EClass.rnd(2 + (soilCost - maxSoil) / 20);
			}
			if (num2 <= 0)
			{
				if (!EClass.player.isAutoFarming)
				{
					c?.Say("cropSpoiled", c, cell.GetObjName());
				}
				return;
			}
			t.SetNum(num2);
		}
		else
		{
			Debug.Log("harvest count not set:" + source.id + "/" + source.alias);
		}
		if (num > 0)
		{
			t.SetNum(num);
		}
		if (c == null || EClass.player.isAutoFarming)
		{
			TryPick(cell, t, c);
		}
		else
		{
			c.Pick(t);
		}
	}

	public void ApplySeed(Thing t)
	{
		Thing thing = EClass._map.TryGetPlant(cell)?.seed;
		if (EClass._zone.IsUserZone)
		{
			thing = null;
		}
		if (thing == null)
		{
			return;
		}
		int encLv = thing.encLV / 10 + ((thing.encLV > 0) ? 1 : 0);
		bool flag = t.IsFood || t.Evalue(10) > 0 || t.id == "grass";
		foreach (Element value in thing.elements.dict.Values)
		{
			if ((!value.IsFoodTrait || flag) && (value.IsFoodTrait || value.id == 2))
			{
				t.elements.ModBase(value.id, value.Value / 10 * 10);
			}
		}
		t.SetEncLv(encLv);
		t.c_refText = thing.c_refText;
		t.isCrafted = true;
	}

	public virtual int GetHp()
	{
		return source.hp;
	}
}
