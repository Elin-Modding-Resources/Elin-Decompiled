using UnityEngine;

namespace Applibot;

public class NormalDissolve : CustomImageBase
{
	[SerializeField]
	private Texture2D _dissovleTex;

	[SerializeField]
	[Range(0f, 1f)]
	private float _dissolveAmount;

	[SerializeField]
	[Range(0f, 1f)]
	private float _dissolveRange;

	[SerializeField]
	[ColorUsage(false, true)]
	private Color _glowColor;

	private int _dissolveTexId = Shader.PropertyToID("_DissolveTex");

	private int _dissolveRangeId = Shader.PropertyToID("_DissolveRange");

	private int _dissolveAmountId = Shader.PropertyToID("_DissolveAmount");

	private int _glowColorId = Shader.PropertyToID("_GlowColor");

	protected override void UpdateMaterial(Material baseMaterial)
	{
		if (material == null)
		{
			Shader shader = Shader.Find("Applibot/UI/NormalDissolve");
			material = new Material(shader);
			material.CopyPropertiesFromMaterial(baseMaterial);
			material.hideFlags = HideFlags.HideAndDontSave;
		}
		material.SetTexture(_dissolveTexId, _dissovleTex);
		material.SetFloat(_dissolveAmountId, _dissolveAmount);
		material.SetFloat(_dissolveRangeId, _dissolveRange);
		material.SetColor(_glowColorId, _glowColor);
	}
}
