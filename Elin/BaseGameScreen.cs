using System;
using UnityEngine;

public class BaseGameScreen : EMono
{
	public class FocusOption
	{
		public Point pos;

		public float speed = 2f;

		public bool linear = true;

		public bool player;
	}

	private readonly int _BackDrawPower = Shader.PropertyToID("_BackDrawPower");

	private readonly int _Snow = Shader.PropertyToID("_Snow");

	private readonly int _SkyLevel = Shader.PropertyToID("_SkyLevel");

	private readonly int _NightRate = Shader.PropertyToID("_NightRate");

	private readonly int _ViewHeight = Shader.PropertyToID("_ViewHeight");

	private readonly int _FogStrength = Shader.PropertyToID("_FogStrength");

	private readonly int _FogColor = Shader.PropertyToID("_FogColor");

	private readonly int _FowColor = Shader.PropertyToID("_FowColor");

	private readonly int _SunColor = Shader.PropertyToID("_SunColor");

	private readonly int _SeaColor = Shader.PropertyToID("_SeaColor");

	private readonly int _SkyColor = Shader.PropertyToID("_SkyColor");

	private readonly int _SkyBGColor = Shader.PropertyToID("_SkyBGColor");

	private readonly int _Offset = Shader.PropertyToID("_Offset");

	private readonly int _Zoom = Shader.PropertyToID("_Zoom");

	private readonly int _LightPower = Shader.PropertyToID("_LightPower");

	private readonly int _AnimeSpeedGlobal = Shader.PropertyToID("_AnimeSpeedGlobal");

	private readonly int _ScreenFlash = Shader.PropertyToID("_ScreenFlash");

	public float SharpenAmount;

	public float camSpeed2;

	public float heightLight;

	public float pushbackSpeed;

	public Vector2 snapSize;

	public Vector2 tileSize;

	public Vector2 tileAlign;

	public Vector2 tileWorldSize;

	public Vector2 tileViewSize;

	public Vector3 tileWeight;

	public Vector3 focusFix;

	public Vector3 tileOffsetHeight;

	public Vector3 tileWeightHeight;

	public Vector3 planeAngle;

	public Vector3 planeSpeed;

	public Vector2 paddings;

	public Vector2 paddingOffset;

	public Vector3 lastPos;

	public Rect mapLimit;

	[Space]
	public float Zoom;

	[Space]
	public float nextMove;

	[Space]
	public float focusSpeed;

	[Space]
	public float focusSpeedSlow;

	[Space]
	public float focusSpeedSlow2;

	public int width;

	public int height;

	public int scrollX;

	public int scrollY;

	public int moonLevel;

	public BaseTileMap tileMap;

	public BaseTileSelector tileSelector;

	public ScreenGuide guide;

	public Grid grid;

	public MeshRenderer overlayShadow;

	public PCOrbit pcOrbit;

	[NonSerialized]
	public float forcePrecision;

	[NonSerialized]
	public bool isCameraMoving;

	[NonSerialized]
	public FocusOption focusOption;

	[NonSerialized]
	public Vector3? focusPos;

	[NonSerialized]
	public Vector3 position;

	protected Vector3 camPos;

	protected Vector3 pushBack;

	public float screenFixX;

	public float screenFixX2;

	public float screenFixX3;

	public float screenFixX4;

	public float screenFixY;

	public float screenFixY2;

	public float screenFixY3;

	public float screenFixY4;

	private Vector3 _camPos;

	private Point _randomPoint = new Point();

	[NonSerialized]
	public bool instantFocus;

	public int targetZoomIndex;

	public float targetZoom = 0.5f;

	public float zoomTimer;

	public Vector3 zoomPos;

	public bool fixFocus => EMono.scene.actionMode?.FixFocus ?? false;

	public virtual float TargetZoom => EMono.scene.actionMode.TargetZoom;

	protected CameraSupport camSupport => EMono.scene.camSupport;

	protected Transform transFocus => EMono.scene.transFocus;

	public virtual float SkyRate => Mathf.Clamp((1.2f - Zoom) * 2f - (EMono._zone.IsSnowCovered ? 0.38f : 0f), 0f, 2f);

	public virtual float ViewHeight => SkyRate * 10f + 5f;

	public virtual bool IsGameScreen => true;

	public virtual bool IsLocalMap => false;

	public void Activate()
	{
		if (!(EMono.core.screen == this))
		{
			if (EMono.core.screen != null)
			{
				EMono.core.screen.Deactivate();
			}
			SetUnitSize();
			EMono.core.screen = this;
			Point._screen = this;
			base.gameObject.SetActive(value: true);
			EMono.scene.flock.SetActive(IsLocalMap);
			EMono.scene.fireflyNight.SetActive(IsLocalMap);
			EMono.scene.firefly.SetActive(IsLocalMap);
			EMono.scene.star.SetActive(IsLocalMap);
			EMono.scene.sfxRain.SetActive(IsGameScreen);
			EMono.scene.sfxSea.SetActive(IsGameScreen);
			EMono.scene.sfxWind.SetActive(IsGameScreen);
			EMono.scene.sfxFire.SetActive(IsGameScreen);
			OnActivate();
			ParticleSystem.MainModule main = EMono.scene.rain.main;
			ParticleSystem.MainModule main2 = EMono.scene.snow.main;
			ParticleSystem.MainModule main3 = EMono.scene.ether.main;
			bool flag2 = (main3.prewarm = true);
			bool prewarm = (main2.prewarm = flag2);
			main.prewarm = prewarm;
			ParticleSystem[] blossoms = EMono.scene.blossoms;
			for (int i = 0; i < blossoms.Length; i++)
			{
				ParticleSystem.MainModule main4 = blossoms[i].main;
				main4.prewarm = true;
			}
			if ((bool)overlayShadow)
			{
				overlayShadow.SetActive(EMono.scene.profile.matOverlay);
				overlayShadow.sharedMaterial = EMono.scene.profile.matOverlay;
			}
			EMono.scene.camSupport.grading.material = EMono.scene.profile.matGrading;
			if (IsGameScreen)
			{
				EMono.scene.OnChangeHour();
				EMono.scene.ApplyZoneConfig();
				RefreshAll();
				tileMap.OnActivate(this);
				WidgetHotbar.RefreshHighlights();
			}
			else if (EMono.core.config != null)
			{
				EMono.core.config.RefreshUIBrightness();
			}
		}
	}

	public virtual void OnActivate()
	{
	}

	public void Deactivate()
	{
		OnDeactivate();
		base.gameObject.SetActive(value: false);
		EMono.core.screen = null;
	}

	public virtual void OnDeactivate()
	{
	}

	public virtual void SetUnitSize()
	{
	}

	public virtual void OnEndPlayerTurn()
	{
	}

	public void Draw()
	{
		if (!IsGameScreen)
		{
			return;
		}
		if (Zoom != targetZoom)
		{
			bool flag = EMono.scene.actionMode == ActionMode.Bird;
			CoreConfig.CameraConfig camera = EMono.core.config.camera;
			float num = camera.zoomSpeed * (flag ? 0.05f : (camera.linearZoom ? 2f : 1f)) * Core.delta;
			zoomTimer += num * 0.3f;
			float t = ((flag || !camera.linearZoom) ? num : zoomTimer);
			float num2 = Mathf.Lerp(EMono.screen.Zoom, targetZoom, t);
			if (Mathf.Abs(num2 - targetZoom) < (flag ? 0.0005f : 0.003f))
			{
				num2 = targetZoom;
			}
			if (camera.zoomToMouse && zoomPos != Vector3.zero)
			{
				position = Vector3.Lerp(position, zoomPos, t);
			}
			EMono.core.config.ApplyZoom(num2);
		}
		RenderObject.gameSpeed = EMono.scene.actionMode.gameSpeed;
		RenderObject.gameDelta = Core.gameDeltaNoPause;
		if (EMono.pc.currentZone == EMono._zone && !EMono._zone.IsRegion && focusOption == null && !focusPos.HasValue && !transFocus && fixFocus && (instantFocus || !EMono.core.config.camera.smoothFollow))
		{
			if (Zoom != targetZoom)
			{
				EMono.core.config.ApplyZoom(TargetZoom);
			}
			Vector3 vector = (EMono.pc.isSynced ? EMono.player.position : EMono.pc.pos.Position()) + focusFix;
			position = vector;
			scrollX = Mathf.FloorToInt(position.x / tileWorldSize.x) - width / 2 + (int)paddingOffset.x;
			scrollY = Mathf.FloorToInt(position.y / tileWorldSize.y) - height / 4 + (int)paddingOffset.y;
		}
		tileMap.Draw();
		if (instantFocus)
		{
			instantFocus = false;
		}
		RefreshPosition();
		if (EMono.game.activeZone != null)
		{
			for (int num3 = EMono._map.pointAnimes.Count - 1; num3 >= 0; num3--)
			{
				if (EMono._map.pointAnimes[num3].Update())
				{
					EMono._map.pointAnimes.RemoveAt(num3);
				}
			}
		}
		UpdateShaders(EMono.scene.timeRatio);
		if (Scene.skipAnime)
		{
			EffectManager.Instance.KillAll();
			Scene.skipAnime = false;
		}
		EffectManager.Instance.UpdateEffects();
		FlockController flock = EMono.scene.flock;
		int visible;
		if (targetZoom <= 1.25f)
		{
			Room room = EMono.pc.pos.cell.room;
			visible = ((room == null || !room.HasRoof) ? 1 : 0);
		}
		else
		{
			visible = 0;
		}
		flock.UpdateVisible((byte)visible != 0);
		if (EMono.pc.isSynced && (bool)EMono.pc.renderer.orbit)
		{
			EMono.pc.renderer.orbit.Refresh();
		}
	}

	public void UpdateShaders(float time = 0f)
	{
		SceneProfile profile = EMono.scene.profile;
		SceneColorProfile color = profile.color;
		SceneLightProfile light = profile.light;
		Color value = color.fog.Evaluate(time);
		float value2 = light.nightRatioCurve.Evaluate(time);
		Shader.SetGlobalFloat(_Snow, EMono._zone.IsSnowCovered ? 1f : 0f);
		Shader.SetGlobalFloat(_BackDrawPower, (!ActionMode.Cinema.IsActive) ? EMono.game.config.backDrawAlpha : 0);
		Shader.SetGlobalFloat(_SkyLevel, SkyRate);
		Shader.SetGlobalFloat(_NightRate, value2);
		Shader.SetGlobalFloat(_ViewHeight, ViewHeight);
		Shader.SetGlobalFloat(_FogStrength, value.a * 0.1f);
		Shader.SetGlobalColor(_FogColor, value);
		FowProfile fowProfile = EMono._zone.biome.fowProfile;
		if ((bool)EMono._map.fowProfile)
		{
			fowProfile = EMono._map.fowProfile;
		}
		if ((bool)fowProfile)
		{
			switch (fowProfile.type)
			{
			case FOWType.Color:
				Shader.SetGlobalColor(_FowColor, fowProfile.color);
				break;
			case FOWType.ColorAdd:
				Shader.SetGlobalColor(_FowColor, fowProfile.color * color.fow.Evaluate(time));
				break;
			default:
				Shader.SetGlobalColor(_FowColor, color.fow.Evaluate(time));
				break;
			}
		}
		else
		{
			Shader.SetGlobalColor(_FowColor, color.fow.Evaluate(time));
		}
		Shader.SetGlobalColor(_SunColor, EMono._zone.IsSnowCovered ? color.sunSnow.Evaluate(time) : color.sun.Evaluate(time));
		Shader.SetGlobalColor(_SeaColor, color.sea.Evaluate(time));
		Shader.SetGlobalColor(_SkyColor, color.sky.Evaluate(time));
		Shader.SetGlobalColor(_SkyBGColor, color.skyBG.Evaluate(time));
		Shader.SetGlobalVector(_Offset, EMono.core.config.camera.extendZoomMin ? (position * 0.3f) : (position * 0.5f));
		Shader.SetGlobalFloat(_Zoom, Zoom);
		Shader.SetGlobalFloat(_LightPower, light.lightPower.Evaluate(time) * light.lightPowerMod);
		Shader.SetGlobalFloat(_AnimeSpeedGlobal, EMono.game.config.animeSpeed);
		Shader.SetGlobalColor(_ScreenFlash, ScreenFlash.currentColor);
		ParticleSystem.MainModule main = EMono.scene.godray.main;
		main.startColor = (EMono._zone.IsSnowCovered ? color.godraySnow.Evaluate(time) : color.godray.Evaluate(time));
		camSupport.grading.vignettePower = light.vignetteCurve.Evaluate(time);
		camSupport.grading.UpdateVignette();
		camSupport.bloom.threshold = light.bloomCurve.Evaluate(time);
		camSupport.beautify.bloomIntensity = light.bloomCurve2.Evaluate(time);
	}

	public virtual void OnChangeHour()
	{
	}

	public virtual void RefreshPosition()
	{
		if (EMono.pc.currentZone != EMono._zone)
		{
			return;
		}
		if (focusOption != null)
		{
			Vector3 vector = (focusOption.player ? (EMono.pc.pos.Position() + focusFix) : focusOption.pos.PositionCenter());
			vector.z = position.z;
			if (focusOption.linear)
			{
				position = Vector3.MoveTowards(position, vector, focusOption.speed * Core.delta);
			}
			else
			{
				position = Vector3.Lerp(position, vector, focusOption.speed * Core.delta);
			}
		}
		else if (focusPos.HasValue)
		{
			Vector3 obj = focusPos ?? position;
			float x = obj.x;
			Vector3 obj2 = focusPos ?? position;
			focusPos = new Vector3(x, obj2.y, position.z);
			position = Vector3.Lerp(position, focusPos ?? position, focusSpeedSlow * Core.delta);
			if (Vector3.Distance(position, focusPos ?? position) < 0.1f)
			{
				focusPos = null;
			}
		}
		else if ((bool)transFocus)
		{
			pushBack = Vector3.zero;
			position.x = transFocus.position.x;
			position.y = transFocus.position.y;
		}
		else if (fixFocus)
		{
			float z = position.z;
			targetZoom = TargetZoom;
			for (int i = 0; i < EMono.core.config.camera.zooms.Length; i++)
			{
				if (targetZoom == EMono.core.config.camera.zooms[i])
				{
					targetZoomIndex = i;
				}
			}
			Vector3 b = (EMono.pc.isSynced ? EMono.player.position : EMono.pc.pos.Position()) + focusFix;
			if (instantFocus)
			{
				position = b;
			}
			else if (EMono.core.config.camera.smoothFollow)
			{
				position = Vector3.Lerp(position, b, focusSpeed * Core.delta * Mathf.Lerp(0.2f, 1f, 1f - Mathf.Clamp(ActionMode.Adv.rightMouseTimer, 0f, 1f)));
			}
			else
			{
				position = b;
			}
			position.z = z;
			pcOrbit.transform.position = EMono.pc.renderer.position;
		}
		else
		{
			if (ActionMode.Adv.IsActive)
			{
				targetZoom = ActionMode.Adv.TargetZoom;
			}
			pushBack = Vector3.zero;
			int num = scrollX - scrollY;
			int num2 = scrollY + scrollX;
			if ((float)num <= mapLimit.x)
			{
				pushBack.x = mapLimit.x - (float)num;
			}
			if ((float)num2 <= mapLimit.y)
			{
				pushBack.y = mapLimit.y - (float)num2;
			}
			if ((float)num >= (float)EMono._map.Size + mapLimit.width - (float)(width / 2))
			{
				pushBack.x = (float)EMono._map.Size + mapLimit.width - (float)(width / 2) - (float)num;
			}
			if ((float)num2 >= (float)EMono._map.Size + mapLimit.height - (float)(height / 2))
			{
				pushBack.y = (float)EMono._map.Size + mapLimit.height - (float)(height / 2) - (float)num2;
			}
			position += Quaternion.Euler(planeAngle) * pushBack * Core.delta * pushbackSpeed;
		}
		scrollX = Mathf.FloorToInt(position.x / tileWorldSize.x) - width / 2 + (int)paddingOffset.x;
		scrollY = Mathf.FloorToInt(position.y / tileWorldSize.y) - height / 4 + (int)paddingOffset.y;
		if (lastPos.x != (float)scrollX || lastPos.y != (float)scrollY)
		{
			lastPos.x = scrollX;
			lastPos.y = scrollY;
		}
		camPos.x = position.x;
		camPos.y = position.y;
		SnapScreen(ref camPos, Zoom);
		camPos.z = -500f;
		_camPos.x = camPos.x + screenFixX2 + screenFixX4;
		_camPos.y = camPos.y + screenFixY2 + screenFixY4;
		_camPos.z = camPos.z;
		EMono.scene.cam.transform.localPosition = _camPos;
		camPos.z = 0f;
		EMono.scene.transAudio.position = camPos + EMono.scene.posAudioListener;
		if ((bool)WidgetMinimap.Instance)
		{
			WidgetMinimap.Instance.OnUpdate();
		}
	}

	public void RefreshAll()
	{
		RefreshScreenSize();
		bool indoor = EMono._map.config.indoor;
		ScreenGrading grading = camSupport.grading;
		EMono.scene.flock.SetActive(!indoor);
		if (indoor)
		{
			grading.material.DisableKeyword("CLOUD_ON");
		}
		else
		{
			grading.material.EnableKeyword("CLOUD_ON");
		}
		if (EMono.core.config.graphic.floorEx)
		{
			EMono.scene.matFloorEx.EnableKeyword("EX_ON");
		}
		else
		{
			EMono.scene.matFloorEx.DisableKeyword("EX_ON");
		}
		grading.profile.vignette.enable = EMono.scene.profile.light.vignette;
		grading.profile.vignette.vignetteColor = EMono.scene.profile.light.vignetteColor;
		RefreshSky();
		RefreshTilt();
	}

	public void RefreshScreenSize()
	{
		Zoom = EMono.scene.camSupport.Zoom;
		width = (int)((float)Screen.width / tileSize.x / Zoom) + (int)paddings.x + 2;
		height = (int)((float)Screen.height / (tileSize.y / 2f) / Zoom) + (int)paddings.y + 4;
		camSupport.divier = EMono.core.config.fix.divider;
		camSupport.ResizeCameraToPixelPerfect();
		Vector3 localScale = new Vector3(EMono.scene.cam.orthographicSize * ((float)Screen.width / (float)Screen.height) * 2.5f, EMono.scene.cam.orthographicSize * 2.5f, 1f);
		if (EMono.core.IsGameStarted)
		{
			EMono.scene.skyBG.transform.localScale = localScale;
			float num = ((ActionMode.IsAdv || Zoom >= 1f || EMono._map.IsIndoor) ? 0f : (0.4f / Zoom));
			if (EMono.world.weather.CurrentCondition == Weather.Condition.SnowHeavy)
			{
				num += 0.8f;
			}
			if (!EMono.player.simulatingZone)
			{
				EMono.scene.sfxWind.SetVolume(Mathf.Min(num, 1f));
			}
		}
		Vector3 vector = EMono.scene.cam.transform.position;
		EMono.scene.cam.transform.position = Vector3.zero;
		Vector3 vector2 = EMono.scene.cam.WorldToScreenPoint(Vector3.zero);
		screenFixX2 = vector2.x % screenFixX * screenFixX3;
		screenFixY2 = vector2.y % screenFixY * screenFixY3;
		EMono.scene.cam.transform.position = vector;
	}

	public Point GetRandomPoint()
	{
		int num = EMono.screen.scrollX - EMono.screen.scrollY + width / 2 - (height / 2 + 1) / 2;
		int num2 = EMono.screen.scrollY + EMono.screen.scrollX + width / 2 + height / 2 / 2;
		num = num - 20 + EMono.rnd(40);
		num2 = num2 - 20 + EMono.rnd(40);
		return _randomPoint.Set(num, num2);
	}

	public void RefreshSky()
	{
		CoreConfig.GraphicSetting graphic = EMono.core.config.graphic;
		bool indoor = EMono._map.config.indoor;
		EMono.scene.firefly.baseParticleCount = graphic.fireflyCount;
		EMono.scene.fireflyNight.baseParticleCount = graphic.fireflyCount * 10;
		EMono.scene.star.baseParticleCount = graphic.starCount;
		EMono.scene.star.gameObject.GetComponent<ParticleSystemRenderer>().enabled = EMono.world.date.IsNight;
		EMono.scene.star.SetActive(IsLocalMap && !indoor);
		EMono.scene.firefly.SetActive(IsLocalMap && graphic.firefly && !indoor);
		EMono.scene.fireflyNight.SetActive(IsLocalMap && graphic.firefly && !indoor);
		if ((bool)overlayShadow && (bool)EMono.scene.profile.matOverlay && this is GameScreen)
		{
			overlayShadow.sharedMaterial.SetFloat("_ShadowStrength", EMono._map.config.shadowStrength * (EMono._zone.IsSnowCovered ? 0.5f : 1f) * EMono.setting.render.shadowStrength);
		}
		RefreshWeather();
		RefreshGrading();
	}

	public virtual void RefreshWeather()
	{
		bool flag = EMono._map.config.indoor || (EMono.pc.IsInActiveZone && (EMono.pc.pos.cell.room?.HasRoof ?? false));
		bool flag2 = EMono.pc.pos.cell.room?.data.atrium ?? false;
		Weather weather = EMono.world.weather;
		Weather.Condition currentCondition = weather.CurrentCondition;
		EMono.scene.filterRain.enabled = !flag && weather.IsRaining && EMono.core.config.graphic.enhanceRain;
		EMono.scene.filterRain.Fade = ((currentCondition == Weather.Condition.RainHeavy) ? 0.4f : 0.3f);
		EMono.scene.rain.enableEmission = !flag && weather.IsRaining;
		ParticleSystem.EmissionModule emission = EMono.scene.rain.emission;
		emission.rateOverTime = ((currentCondition == Weather.Condition.RainHeavy) ? 750 : 200);
		EMono.scene.snow.enableEmission = !flag && currentCondition == Weather.Condition.Snow;
		EMono.scene.ether.enableEmission = !flag && weather.IsEther;
		bool enableEmission = !flag && weather.IsBlossom;
		ParticleSystem[] blossoms = EMono.scene.blossoms;
		for (int i = 0; i < blossoms.Length; i++)
		{
			blossoms[i].enableEmission = enableEmission;
		}
		enableEmission = !flag && currentCondition == Weather.Condition.SnowHeavy && EMono.core.config.graphic.blizzard;
		blossoms = EMono.scene.blizzards;
		for (int i = 0; i < blossoms.Length; i++)
		{
			blossoms[i].enableEmission = enableEmission;
		}
		EMono.scene.transBlizzard.localScale = new Vector3(1f, 1f, 1f);
		bool flag3 = (EMono._map.config.forceGodRay || (EMono.core.config.graphic.godray && !flag && (currentCondition == Weather.Condition.Fine || currentCondition == Weather.Condition.Snow))) && !BuildMenu.Instance;
		EMono.scene.godray.SetActive(flag3, delegate(bool enabled)
		{
			if (!enabled)
			{
				EMono.scene.godray.Clear();
			}
		});
		EMono.scene.godrayDust.SetActive(flag3 && EMono.world.date.IsNight);
		EMono.scene.snow.SetActive(enable: true);
		EMono.scene.rain.SetActive(enable: true);
		EMono.scene.ether.SetActive(enable: true);
		EMono.scene.blossom.SetActive(enable: true);
		Zone zone = EMono._zone;
		if (EMono._zone is Zone_Tent)
		{
			zone = (EMono._zone.parent as Zone) ?? EMono._zone;
		}
		float num = ((zone.lv <= -2) ? 0f : ((zone.lv <= -1) ? 0.3f : ((flag && !flag2) ? 0.6f : 1f)));
		EMono.scene.sfxRain.SetVolume(weather.IsRaining ? num : 0f);
		EMono.scene.sfxSea.SetVolume(EMono._zone.VolumeSea * num);
		EMono.scene.camSupport.grading.profile.fog = EMono.setting.render.fogs[EMono._map.config.fog];
		EMono.scene.camSupport.grading.userFog = ((!EMono._map.config.indoor && flag) ? 0f : 1f);
		float num2 = ((EMono._map.IsIndoor || !EMono.world.weather.IsRaining) ? 0f : ((currentCondition == Weather.Condition.RainHeavy) ? (-0.08f) : (-0.04f)));
		if (EMono._zone.IsSnowCovered)
		{
			num2 += EMono.scene.profile.global.snowBrightness;
		}
		camSupport.grading.sceneBrightness = num2;
		ParticleSystem.MainModule main = EMono.scene.rain.main;
		ParticleSystem.MainModule main2 = EMono.scene.snow.main;
		ParticleSystem.MainModule main3 = EMono.scene.ether.main;
		bool flag5 = (main3.prewarm = false);
		bool prewarm = (main2.prewarm = flag5);
		main.prewarm = prewarm;
		blossoms = EMono.scene.blossoms;
		for (int i = 0; i < blossoms.Length; i++)
		{
			ParticleSystem.MainModule main4 = blossoms[i].main;
			main4.prewarm = false;
		}
		camSupport.grading.SetGrading();
	}

	public void RefreshGrading()
	{
		float timeRatio = (EMono.core.IsGameStarted ? EMono.scene.timeRatio : 0f);
		if ((bool)camSupport.grading.profile.overlay)
		{
			camSupport.grading.profile.overlay.Refresh(timeRatio, EMono._zone.IsSnowCovered);
		}
		ScreenGrading.blind = EMono.pc.isBlind;
		camSupport.grading.profile.sharpen.enable = EMono.core.config.graphic.sharpen > 0;
		camSupport.grading.profile.sharpen.Strength = Mathf.Clamp(0.1f * (float)EMono.core.config.graphic.sharpen, 0f, (this is GameScreen) ? 10f : 1f);
		camSupport.beautify.sharpen = Mathf.Clamp(0.01f * (float)EMono.core.config.graphic.sharpen2, 0f, (this is GameScreen) ? 10f : 0.5f);
		camSupport.grading.SetGrading();
		SceneTemplate sceneTemplate = SceneTemplate.Load(EMono._map.config.idSceneTemplate.IsEmpty(EMono._zone.IDSceneTemplate));
		Color tintColor = EMono._map.config.colorScreen.Get();
		if (tintColor.a == 0f)
		{
			tintColor = sceneTemplate.colorScreen;
		}
		camSupport.beautify.tintColor = tintColor;
		tintColor = EMono._map.config.colorSea.Get();
		MATERIAL.sourceWaterSea.matColor = ((tintColor.a == 0f) ? MATERIAL.sourceWaterSea.GetColor() : tintColor);
		EMono.core.config.RefreshUIBrightness();
		float num = 0f;
		if (!EMono._map.IsIndoor)
		{
			num += 0.01f * EMono.core.config.test.brightnessNight * EMono.scene.profile.light.nightRatioCurve.Evaluate(EMono.scene.timeRatio);
		}
		camSupport.grading.nightBrightness = num;
	}

	public void RefreshTilt()
	{
		camSupport.tiltShift.enabled = ((ActionMode.Bird.IsActive || EMono._zone.IsRegion) ? EMono.game.config.tiltRegion : EMono.game.config.tilt);
		camSupport.tiltShift.blurArea = (ActionMode.Bird.IsActive ? 12f : (0.1f * (float)(EMono._zone.IsRegion ? EMono.game.config.tiltPowerRegion : EMono.game.config.tiltPower)));
	}

	public void ScrollMouse(float x, float y)
	{
		position.x += x / Zoom;
		position.y += y / Zoom;
		if (x > 0.1f || x < -0.1f || y > 0.1f || y < 0.1f)
		{
			RefreshPosition();
		}
	}

	public void ScrollAxis(Vector3 axis, bool direct = false)
	{
		if (direct)
		{
			position += axis;
		}
		else
		{
			position += axis * Core.delta * camSpeed2 * EMono.core.config.camera.senseKeyboard / Zoom;
		}
	}

	public void Focus(Int3 ints)
	{
		position = new Vector3(ints.x, ints.y, ints.z);
	}

	public void Focus(int x, int y)
	{
		if (EMono.AdvMode)
		{
			SE.Beep();
			return;
		}
		position.x = (scrollX = x);
		position.y = (scrollY = y);
	}

	public void Focus(Card c)
	{
		if (c != null)
		{
			Focus(c.GetRootCard().pos);
		}
	}

	public void FocusCenter()
	{
		Point pos = new Point(EMono._map.Size / 2, EMono._map.Size / 2);
		Focus(pos);
	}

	public void Focus(Point pos)
	{
		position = pos.PositionCenter();
	}

	public void FocusPC()
	{
		focusPos = null;
		Focus((EMono.pc.currentZone == EMono.game.activeZone) ? EMono.pc.pos : EMono._map.bounds.GetCenterPos());
		instantFocus = true;
		RefreshPosition();
	}

	public void FocusImmediate(Point pos)
	{
		focusPos = null;
		Focus(pos);
		instantFocus = true;
		RefreshPosition();
		RefreshPosition();
	}

	public void SnapScreen(ref Vector3 v, float zoom)
	{
		float num = snapSize.x / zoom;
		float num2 = snapSize.y / zoom;
		switch (EMono.core.config.fix.snapType)
		{
		case CoreConfig.ScreenSnapType.Default:
			v.x = num * (float)Mathf.RoundToInt(v.x / num);
			v.y = num2 * (float)Mathf.RoundToInt(v.y / num2);
			break;
		case CoreConfig.ScreenSnapType.Floor:
			v.x = num * (float)Mathf.FloorToInt(v.x / num);
			v.y = num2 * (float)Mathf.FloorToInt(v.y / num2);
			break;
		case CoreConfig.ScreenSnapType.Ceil:
			v.x = num * (float)Mathf.CeilToInt(v.x / num);
			v.y = num2 * (float)Mathf.CeilToInt(v.y / num2);
			break;
		case CoreConfig.ScreenSnapType.Hack3:
			num = snapSize.x;
			num2 = snapSize.y;
			v.x = num * (float)Mathf.RoundToInt(v.x / num);
			v.y = num2 * (float)Mathf.RoundToInt(v.y / num2);
			break;
		case CoreConfig.ScreenSnapType.Hack4:
			num = snapSize.x;
			num2 = snapSize.y;
			v.x = num * (float)Mathf.FloorToInt(v.x / num);
			v.y = num2 * (float)Mathf.FloorToInt(v.y / num2);
			break;
		case CoreConfig.ScreenSnapType.Hack5:
			num = snapSize.x;
			num2 = snapSize.y;
			v.x = num * (float)Mathf.CeilToInt(v.x / num);
			v.y = num2 * (float)Mathf.CeilToInt(v.y / num2);
			break;
		case CoreConfig.ScreenSnapType.Hack6:
			v.x = 0.01f * (float)Mathf.FloorToInt(v.x * 100f) + 0.001f;
			v.y = 0.01f * (float)Mathf.FloorToInt(v.y * 100f) + 0.001f;
			break;
		case CoreConfig.ScreenSnapType.Hack7:
			v.x = 0.01f * (float)Mathf.FloorToInt(v.x * 100f) - 0.001f;
			v.y = 0.01f * (float)Mathf.FloorToInt(v.y * 100f) - 0.001f;
			break;
		case CoreConfig.ScreenSnapType.Hack8:
			v.x = 0.01f * (float)Mathf.RoundToInt(v.x * 100f) + 0.005f;
			v.y = 0.01f * (float)Mathf.RoundToInt(v.y * 100f) + 0.005f;
			break;
		case CoreConfig.ScreenSnapType.Hack9:
			v.x = 0.01f * (float)Mathf.RoundToInt(v.x * 100f) + 0.025f;
			v.y = 0.01f * (float)Mathf.RoundToInt(v.y * 100f) + 0.025f;
			break;
		}
	}

	public void SetZoom(float z)
	{
		targetZoom = (Zoom = z);
		EMono.core.config.ApplyZoom(targetZoom);
		zoomTimer = 0f;
	}

	public void SetTargetZoomIndex(int index)
	{
		targetZoomIndex = index;
		float num = targetZoom;
		targetZoom = EMono.core.config.camera.zooms[targetZoomIndex];
		if (num != targetZoom)
		{
			zoomTimer = 0f;
		}
	}

	public void ModTargetZoomIndex(int a)
	{
		if (!EMono.scene.elomapActor.IsActive)
		{
			int num = targetZoomIndex;
			targetZoomIndex -= a;
			int num2 = ((!EMono.core.config.camera.extendZoomMin) ? 1 : 0);
			int num3 = (EMono.core.config.camera.extendZoomMax ? 4 : 3);
			if (targetZoomIndex < num2)
			{
				targetZoomIndex = num2;
			}
			else if (targetZoomIndex >= num3)
			{
				targetZoomIndex = num3;
			}
			if (targetZoomIndex > num && EInput.buttonScroll != null && !EInput.buttonScroll.pressing)
			{
				zoomPos = EInput.mposWorld;
			}
			SetTargetZoomIndex(targetZoomIndex);
		}
	}
}
