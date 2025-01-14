using System.Collections.Generic;
using Algorithms;
using UnityEngine;

public class GameUpdater : EClass
{
	public class Updater : EClass
	{
		public int index;

		public int updatesPerFrame;

		public int maxCount;

		public float duration = 1f;

		public float _updatesPerFrame;

		public Point pos = new Point();

		public virtual void FixedUpdate()
		{
		}

		public void SetUpdatesPerFrame(int _maxcount, float _duration)
		{
			duration = _duration;
			maxCount = _maxcount;
			if (maxCount <= 0)
			{
				updatesPerFrame = 0;
				return;
			}
			_updatesPerFrame += EClass.scene.actionMode.gameSpeed * (float)maxCount / (1f / delta) / duration;
			updatesPerFrame = (int)_updatesPerFrame;
			_updatesPerFrame -= updatesPerFrame;
		}
	}

	public class AreaUpdater : Updater
	{
		public override void FixedUpdate()
		{
			List<Area> listArea = EClass._map.rooms.listArea;
			if (listArea.Count == 0)
			{
				return;
			}
			SetUpdatesPerFrame(listArea.Count, 1f);
			for (int i = 0; i < updatesPerFrame; i++)
			{
				index++;
				if (index >= listArea.Count)
				{
					index = 0;
				}
				listArea[index].Update();
			}
		}
	}

	public class SurfaceUpdater : Updater
	{
		public int x;

		public int z;

		public int SizeXZ => EClass._map.SizeXZ;

		public int Size => EClass._map.Size;

		public override void FixedUpdate()
		{
			if (!pos.Set(x, z).IsValid)
			{
				x = (z = 0);
			}
			SetUpdatesPerFrame(SizeXZ, 20f);
			_ = EClass._map.Roaming.raceMap.TryGetValue("fish")?.num;
			bool isRaining = EClass.world.weather.IsRaining;
			for (int i = 0; i < updatesPerFrame; i++)
			{
				pos.Set(x, z);
				_ = pos.sourceObj;
				bool flag = pos.sourceBlock.id != 0;
				Cell cell = pos.cell;
				bool flag2 = EClass._map.IsIndoor || cell.HasRoof;
				if (cell.effect != null)
				{
					if (cell.HasFire)
					{
						if (isRaining)
						{
							pos.ModFire(-5);
						}
						else if (EClass.rnd(2) == 0)
						{
							pos.ModFire(-2);
						}
					}
					else if (cell.HasLiquid && (!isRaining || cell.IsTopWater || cell.HasRoof))
					{
						EClass._map.ModLiquid(pos.x, pos.z, -1);
					}
				}
				if (isRaining)
				{
					if (!flag2)
					{
						if (cell.IsFarmField)
						{
							cell.isWatered = true;
						}
						if (EClass.rnd(500) == 0 && !flag && !cell.blocked && !cell.HasObj && !cell.IsTopWaterAndNoSnow && !EClass._zone.IsRegion)
						{
							EClass._map.SetLiquid(pos.x, pos.z, 1, EClass.rnd(3) + 1);
						}
					}
					if (EClass.rnd(50) == 0 && cell.decal != 0)
					{
						cell.decal = 0;
					}
				}
				if (EClass.rnd(30) == 0 && cell.decal != 0 && !pos.IsInBounds)
				{
					cell.decal = 0;
				}
				x++;
				if (x >= Size)
				{
					x = 0;
					z++;
					if (z >= Size)
					{
						z = 0;
					}
				}
			}
		}
	}

	public class FastSurfaceUpdater : SurfaceUpdater
	{
		public override void FixedUpdate()
		{
			if (!pos.Set(x, z).IsValid)
			{
				x = (z = 0);
			}
			SetUpdatesPerFrame(base.SizeXZ, 1f);
			for (int i = 0; i < updatesPerFrame; i++)
			{
				pos.Set(x, z);
				Cell cell = pos.cell;
				if (cell.effect != null && cell.effect.WillFade)
				{
					cell.effect.amount--;
					if (cell.effect.amount <= 0)
					{
						EClass._map.SetEffect(pos.x, pos.z);
					}
				}
				x++;
				if (x >= base.Size)
				{
					x = 0;
					z++;
					if (z >= base.Size)
					{
						z = 0;
					}
				}
			}
		}
	}

	public class LogicUpdater : Updater
	{
		public virtual LogicalPointManager manager => null;

		public virtual float UpdateDuration => 1f;

		public virtual bool RefreshTile => true;

		public override void FixedUpdate()
		{
			List<LogicalPoint> list = manager.list;
			if (list.Count == 0)
			{
				return;
			}
			SetUpdatesPerFrame(list.Count, UpdateDuration);
			bool flag = false;
			for (int i = 0; i < updatesPerFrame; i++)
			{
				index++;
				if (index >= list.Count)
				{
					index = 0;
					flag = true;
				}
				list[index].Update();
			}
			if (RefreshTile)
			{
				foreach (Point refresh in manager.refreshList)
				{
					EClass._map.RefreshSingleTile(refresh.x, refresh.z);
				}
			}
			manager.refreshList.Clear();
		}
	}

	public class FireUpdater : LogicUpdater
	{
		public override LogicalPointManager manager => EClass._map.effectManager;

		public override float UpdateDuration => 5f;
	}

	public class SensorUpdater : Updater
	{
		public override void FixedUpdate()
		{
			List<Chara> charas = EClass._map.charas;
			SetUpdatesPerFrame(charas.Count, 1f);
			for (int i = 0; i < updatesPerFrame; i++)
			{
				index++;
				if (index >= charas.Count)
				{
					index = 0;
				}
				Chara chara = charas[index];
				if (chara.IsAliveInCurrentZone && !chara.IsPC)
				{
					chara.FindNewEnemy();
				}
			}
		}
	}

	public class CharaUpdater : Updater
	{
		public override void FixedUpdate()
		{
			if (Game.isPaused)
			{
				return;
			}
			List<Chara> charas = EClass._map.charas;
			SetUpdatesPerFrame(charas.Count, 0.05f);
			float gameDelta = Core.gameDelta;
			for (int i = 0; i < charas.Count; i++)
			{
				Chara chara = charas[i];
				if (Game.isPaused && !chara.IsPC)
				{
					break;
				}
				chara.roundTimer += gameDelta;
				float actTime = chara.actTime;
				if (chara.roundTimer > actTime)
				{
					chara.Tick();
					chara.roundTimer -= actTime;
					if (chara.ai is GoalEndTurn)
					{
						chara.SetNoGoal();
						break;
					}
					if (i > charas.Count)
					{
						break;
					}
				}
			}
		}
	}

	public class ConditionUpdater : CharaUpdater
	{
		public override void FixedUpdate()
		{
			List<Chara> charas = EClass._map.charas;
			SetUpdatesPerFrame(charas.Count, 0.01f * (float)EClass.pc.Speed * (EClass._zone.IsRegion ? 0.1f : 1f));
			for (int i = 0; i < updatesPerFrame; i++)
			{
				index++;
				if (index >= charas.Count)
				{
					index = 0;
				}
				Chara chara = charas[index];
				for (int num = chara.conditions.Count - 1; num >= 0; num--)
				{
					if (num < chara.conditions.Count)
					{
						Condition condition = chara.conditions[num];
						if (condition.TimeBased)
						{
							condition.Tick();
						}
						if (chara.isDead)
						{
							break;
						}
					}
				}
			}
		}
	}

	public class ThingUpdater : Updater
	{
		public override void FixedUpdate()
		{
			List<Thing> things = EClass._map.things;
			SetUpdatesPerFrame(things.Count, 1f);
			for (int i = 0; i < updatesPerFrame; i++)
			{
				index++;
				if (index >= things.Count)
				{
					index = 0;
				}
				Thing thing = things[index];
				if (thing.fov != null)
				{
					thing.CalculateFOV();
				}
				if (thing.trait.HaveUpdate)
				{
					thing.trait.Update();
				}
				if ((thing.id == "snow" || thing.id == "flyer") && EClass.rnd(30) == 0 && !thing.IsInstalled && !LayerDragGrid.Instance && !LayerCraft.Instance)
				{
					thing.Destroy();
				}
			}
		}
	}

	public static float delta;

	public AreaUpdater area;

	public SurfaceUpdater surface;

	public FastSurfaceUpdater surfaceFast;

	public FireUpdater fire;

	public CharaUpdater chara;

	public ConditionUpdater condition;

	public ThingUpdater thing;

	public SensorUpdater sensor;

	public RecipeUpdater recipe = new RecipeUpdater();

	public Updater[] all;

	private float dateTimer;

	private float counterTimer;

	private int counter;

	private float timerThunder;

	public void Reset()
	{
		area = new AreaUpdater();
		surface = new SurfaceUpdater();
		surfaceFast = new FastSurfaceUpdater();
		fire = new FireUpdater();
		chara = new CharaUpdater();
		condition = new ConditionUpdater();
		thing = new ThingUpdater();
		sensor = new SensorUpdater();
	}

	public void Update100ms()
	{
		if (EClass.game.activeZone.IsPCFaction && EClass.Branch.resources.isDirty)
		{
			EClass.Branch.resources.Refresh();
		}
		bool flag = EClass.world.weather.CurrentCondition == Weather.Condition.RainHeavy;
		bool flag2 = EClass._map.config.indoor || (EClass.pc.IsInActiveZone && (EClass.pc.pos.cell.room?.HasRoof ?? false));
		bool flag3 = EClass.pc.pos.cell.room?.data.atrium ?? false;
		float mtpVolume = ((EClass._zone.lv <= -2) ? 0f : ((flag2 && !flag3) ? 0.5f : 1f));
		if (EClass.world.weather.IsRaining && !EClass._map.IsIndoor)
		{
			for (int i = 0; i < EClass.setting.weather.splashCount * ((!flag) ? 1 : 2); i++)
			{
				Point randomPoint = EClass.screen.GetRandomPoint();
				if (!randomPoint.IsValid)
				{
					continue;
				}
				Cell cell = randomPoint.cell;
				if (!cell.HasRoof && cell._block == 0 && !randomPoint.IsSky)
				{
					if (cell.IsTopWaterAndNoSnow)
					{
						EClass.scene.psRainSplash.Emit(randomPoint.PositionCenter(), Vector3.zero, Rand.Range(0.6f, 1.1f), 0.2f, Color.white);
					}
					else
					{
						EClass.scene.psRainSplashWater.Emit(randomPoint.PositionCenter(), Vector3.zero, Rand.Range(0.6f, 1.1f), 0.2f, Color.white);
					}
				}
			}
		}
		if (!flag)
		{
			return;
		}
		timerThunder += 0.1f;
		if (timerThunder > EClass.setting.weather.thunerInterval)
		{
			if (!EClass._map.IsIndoor)
			{
				ScreenFlash.Play("storm" + (EClass.rnd(2) + 1), (flag2 && !flag3) ? 0.3f : 1f);
			}
			EClass.Sound.Play("Ambience/Random/storm", mtpVolume);
			timerThunder = 0f - Rand.Range(0f, EClass.setting.weather.thunerInterval);
		}
	}

	public void FixedUpdate()
	{
		recipe.FixedUpdate();
		if (!EClass.scene.paused)
		{
			if (all == null)
			{
				all = new Updater[8] { area, surface, surfaceFast, fire, chara, condition, sensor, thing };
			}
			delta = Core.delta;
			area.FixedUpdate();
			surface.FixedUpdate();
			surfaceFast.FixedUpdate();
			fire.FixedUpdate();
			chara.FixedUpdate();
			condition.FixedUpdate();
			sensor.FixedUpdate();
			thing.FixedUpdate();
		}
	}

	public void Update()
	{
		if (EClass.game.activeZone == null)
		{
			return;
		}
		EClass._map.RefreshSunMap();
		if (EClass.debug.testLOS)
		{
			if (EClass._zone.IsRegion)
			{
				Zone zone = EClass.game.spatials.Find("palmia");
				PathProgress pathProgress = PathManager.Instance.RequestPathImmediate(new Point(zone.x - EClass.scene.elomap.minX, zone.y - EClass.scene.elomap.minY), EClass.pc.pos, EClass.pc);
				if (pathProgress.HasPath)
				{
					Debug.Log(pathProgress.nodes.Count);
					foreach (PathFinderNode node in pathProgress.nodes)
					{
						Point point2 = new Point(node.X, node.Z);
						EClass.screen.tileMap.passGuideBlock.Add(ref point2.PositionTopdown(), 20f);
					}
				}
			}
			Point hit2 = Scene.HitPoint;
			if (hit2.IsValid)
			{
				for (int i = 0; i < EClass._map.charas.Count; i++)
				{
					Chara tg = EClass._map.charas[i];
					if (!Los.IsVisible(hit2, tg.pos))
					{
						continue;
					}
					Los.IsVisible(hit2, tg.pos, delegate(Point point, bool blocked)
					{
						EClass.screen.tileMap.passGuideFloor.Add(point, 20f);
						if (Input.GetKey(KeyCode.LeftControl))
						{
							Debug.Log(tg.Name + ": Distance:" + hit2.Distance(tg.pos));
						}
					});
				}
			}
		}
		if (EClass.debug.testLOS2)
		{
			Point hit = Scene.HitPoint;
			if (hit.IsValid)
			{
				EClass._map.ForeachSphere(hit.x, hit.z, 6f, delegate(Point p)
				{
					if (p.IsValid && Los.IsVisible(hit, p))
					{
						EClass.screen.tileMap.passGuideFloor.Add(p, 20f);
					}
				});
			}
		}
		if (EClass.scene.paused)
		{
			return;
		}
		delta = Core.gameDelta;
		EClass._zone.events.Tick(delta);
		dateTimer += delta;
		if (dateTimer > EClass.setting.secsPerHour / 60f)
		{
			dateTimer = 0f;
			EClass.world.date.AdvanceMin(1);
		}
		counterTimer += delta;
		if (!(counterTimer > 0.1f))
		{
			return;
		}
		counterTimer = 0f;
		counter++;
		if (counter == 10)
		{
			counter = 0;
		}
		switch (counter)
		{
		case 1:
			UIResourceTrack.Refresh();
			break;
		case 5:
			if ((bool)EClass.Sound.currentAmbience)
			{
				EClass.Sound.currentAmbience.TryEmit(EClass.world.date.IsDay, ref Point.shared.Set(EClass.pc.pos.x + EClass.rnd(5) - EClass.rnd(5), EClass.pc.pos.z).Clamp().Position());
			}
			break;
		}
	}

	public string GetText()
	{
		if (all == null)
		{
			return "";
		}
		string text = "";
		Updater[] array = all;
		foreach (Updater updater in array)
		{
			text = text + updater.GetType().Name + ": " + updater.updatesPerFrame + " / frame     " + updater.maxCount + " in " + updater.duration + "sec \n";
		}
		return text;
	}
}
