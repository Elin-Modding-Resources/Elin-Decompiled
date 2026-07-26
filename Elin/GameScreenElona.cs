using CreativeSpore.SuperTilemapEditor;
using UnityEngine;

public class GameScreenElona : BaseGameScreen
{
	public EloMapActor actor;

	public float skyRate;

	public float zoom;

	public Vector3 actorPos;

	public override float SkyRate => skyRate;

	public override float TargetZoom => zoom + 0.01f * (float)EMono.game.config.regionZoom - 1f;

	public override void OnActivate()
	{
		actor.transMap.SetActive(enable: true);
		actor.selector.hasTargetChanged = true;
		EMono.scene.skyBG.SetActive(enable: false);
		EMono.scene.skyPlane.SetActive(enable: false);
		EMono.scene.cam.clearFlags = CameraClearFlags.Color;
		Tutorial.Play("region");
		actor.OnActivate();
	}

	public override void OnDeactivate()
	{
		actor.OnDeactivate();
	}

	public override void OnChangeHour()
	{
		actor.OnChangeHour();
	}

	public override void SetUnitSize()
	{
		tileAlign = new Vector2(tileSize.x * 0.01f, tileSize.y * 0.01f);
		tileWorldSize = new Vector2(tileSize.x * 0.01f, tileSize.y * 0.01f);
		float x = tileSize.x;
		float num = 100f / x;
		float num2 = num * 100f;
		float num3 = num * 100f;
		tileViewSize = new Vector2(num2 * 0.01f, num3 * 0.01f);
	}

	public override void RefreshPosition()
	{
		STETilemap fogmap = actor.elomap.fogmap;
		actor.transMap.position = new Vector3((float)(-fogmap.MinGridX) * tileAlign.x + actorPos.x, (float)(-fogmap.MinGridY) * tileAlign.y + actorPos.y, actorPos.z);
		if (base.fixFocus && EMono.pc.currentZone == EMono._zone)
		{
			float z = position.z;
			targetZoom = TargetZoom;
			Vector3 b = (EMono.pc.isSynced ? EMono.pc.renderer.position : EMono.pc.pos.PositionTopdown()) + focusFix;
			if (instantFocus)
			{
				position = b;
			}
			else
			{
				position = Vector3.Lerp(position, b, focusSpeed * Core.delta);
			}
			position.z = z;
			pcOrbit.transform.position = EMono.pc.renderer.position;
		}
		else
		{
			if (ActionMode.Adv.IsActive)
			{
				targetZoom = 0.01f * (float)EMono.game.config.defaultZoom;
			}
			pushBack = Vector3.zero;
			int num = scrollX;
			int num2 = scrollY;
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
			position += pushBack * Core.delta * pushbackSpeed;
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
		EMono.scene.cam.transform.localPosition = camPos;
		camPos.z = 0f;
		EMono.scene.transAudio.position = camPos + EMono.scene.posAudioListener;
		if ((bool)WidgetMinimap.Instance)
		{
			WidgetMinimap.Instance.OnUpdate();
		}
		RefreshWeather();
	}

	public override void RefreshWeather()
	{
		EMono.world.weather.RefreshWeather();
		Point pos = EMono.pc.pos;
		Weather weather = EMono.world.weather;
		Weather.Condition currentCondition = weather.CurrentCondition;
		bool isRaining = weather.IsRaining;
		bool isEther = weather.IsEther;
		bool flag = currentCondition == Weather.Condition.Snow;
		EMono.scene.filterRain.enabled = isRaining && !flag && EMono.core.config.graphic.enhanceRain;
		EMono.scene.filterRain.Fade = ((currentCondition == Weather.Condition.RainHeavy) ? 0.4f : 0.3f);
		EMono.scene.rain.enableEmission = isRaining && !flag;
		ParticleSystem.EmissionModule emission = EMono.scene.rain.emission;
		emission.rateOverTime = ((currentCondition == Weather.Condition.RainHeavy) ? 700 : 200);
		EMono.scene.snow.enableEmission = flag;
		emission = EMono.scene.snow.emission;
		emission.rateOverTime = ((currentCondition == Weather.Condition.SnowHeavy) ? 120 : 35);
		EMono.scene.ether.enableEmission = isEther;
		EMono.scene.snow.SetActive(enable: true);
		EMono.scene.rain.SetActive(enable: true);
		EMono.scene.ether.SetActive(enable: true);
		EMono.scene.blossom.SetActive(enable: false);
		EMono.scene.godray.SetActive(enable: false);
		EMono.scene.godrayDust.SetActive(enable: false);
		EMono.scene.sfxRain._Volume += Core.delta * ((isRaining && !flag) ? 0.5f : (-0.5f));
		EMono.scene.sfxRain.SetVolume(EMono.scene.sfxRain._Volume);
		EMono.scene.sfxWind.SetVolume(1f);
		EMono.scene.sfxSea.SetVolume(Mathf.Lerp(0f, 1f, -0.05f * (float)(pos.x - 20)));
		ParticleSystem.MainModule main = EMono.scene.rain.main;
		ParticleSystem.MainModule main2 = EMono.scene.snow.main;
		ParticleSystem.MainModule main3 = EMono.scene.ether.main;
		bool flag3 = (main3.prewarm = false);
		bool prewarm = (main2.prewarm = flag3);
		main.prewarm = prewarm;
		ParticleSystem[] blossoms = EMono.scene.blossoms;
		for (int i = 0; i < blossoms.Length; i++)
		{
			ParticleSystem.MainModule main4 = blossoms[i].main;
			main4.prewarm = false;
		}
		bool enableEmission = currentCondition == Weather.Condition.SnowHeavy && EMono.core.config.graphic.blizzard;
		blossoms = EMono.scene.blizzards;
		for (int i = 0; i < blossoms.Length; i++)
		{
			blossoms[i].enableEmission = enableEmission;
		}
		EMono.scene.transBlizzard.localScale = new Vector3(2f, 2f, 2f);
		EMono.scene.camSupport.grading.profile.fog = EMono.setting.render.fogs[EMono._map.config.fog];
		float sceneBrightness = ((EMono._map.IsIndoor || !EMono.world.weather.IsRaining) ? 0f : ((currentCondition == Weather.Condition.RainHeavy) ? (-0.06f) : (-0.03f)));
		base.camSupport.grading.sceneBrightness = sceneBrightness;
	}
}
