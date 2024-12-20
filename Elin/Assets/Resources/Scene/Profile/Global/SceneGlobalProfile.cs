using UnityEngine;

namespace Assets.Resources.Scene.Profile.Global;

public class SceneGlobalProfile : ScriptableObject
{
	public float fovPower;

	public float fovModNonGradient;

	public float playerLightPowerLimit;

	public AnimationCurve lightLookupCurve;

	public AnimationCurve snowLimit;

	public AnimationCurve roofLightLimitMod;

	public float modSublight1;

	public float modSublight2;

	public float edgeLight;

	public float snowLight;

	public float snowColor;

	public float snowColor2;

	public float snowBrightness;

	public Vector3Int snowRGB;

	public bool limitGradient;

	[Header("Test")]
	public int testFovRange;

	public float testFovPower;
}
