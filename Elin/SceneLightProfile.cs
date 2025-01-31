using UnityEngine;

public class SceneLightProfile : ScriptableObject
{
	public float baseBrightness;

	public float dynamicBrightnessSpeed;

	public float dynamicBrightnessLightBonus;

	public float lightMod;

	public float lightPowerMod;

	public float playerLightMod = 1f;

	public float playerLightCustomMod;

	public AnimationCurve lightLimit;

	public AnimationCurve lightModCurve;

	public AnimationCurve lightPower;

	public AnimationCurve baseBrightnessCurve;

	public AnimationCurve dynamicBrightnessCurve;

	public AnimationCurve dynamicBrightnessCurve2;

	public AnimationCurve nightRatioCurve;

	public AnimationCurve fovCurveChara;

	public AnimationCurve fogBrightness;

	public AnimationCurve vignetteCurve;

	public AnimationCurve bloomCurve;

	public AnimationCurve bloomCurve2;

	public AnimationCurve orbitAlphaCurve;

	public AnimationCurve shadowCurve;

	public AnimationCurve shadowCurveFloor;

	public bool vignette;

	public Color vignetteColor;
}
