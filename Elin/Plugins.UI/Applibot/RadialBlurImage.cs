using UnityEngine;

namespace Applibot;

public class RadialBlurImage : CustomImageBase
{
	public static readonly string SHADER_NAME = "Custom/UI/RadialBlur";

	public float BlurRadius = 30f;

	[Range(0f, 30f)]
	public int SampleCount = 10;

	private Shader _shader;

	private int _BlurRadiusId = Shader.PropertyToID("_BlurRadius");

	private int _SampleCountId = Shader.PropertyToID("_SampleCount");

	protected override void UpdateMaterial(Material baseMaterial)
	{
		if (material == null)
		{
			Shader shader = Shader.Find(SHADER_NAME);
			material = new Material(shader);
		}
		material.SetFloat(_BlurRadiusId, BlurRadius);
		material.SetInt(_SampleCountId, SampleCount);
		float value = 0.0005f;
		if (base.canvasScaler != null)
		{
			Vector2 referenceResolution = base.canvasScaler.referenceResolution;
			value = 1f / Mathf.Max(referenceResolution.x, referenceResolution.y);
		}
		material.SetFloat("_scaleFactor", value);
	}
}
