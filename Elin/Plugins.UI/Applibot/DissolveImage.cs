using UnityEngine;

namespace Applibot;

public class DissolveImage : CustomImageBase
{
	[SerializeField]
	private Texture2D _dissovleTex;

	[SerializeField]
	[ColorUsage(false, true)]
	private Color _glowColor;

	[SerializeField]
	[Range(0f, 1f)]
	private float _yAmount = 0.5f;

	[SerializeField]
	[Range(0f, 1f)]
	private float _yRange = 0.5f;

	[SerializeField]
	[Range(0f, 1f)]
	private float _dissolveRange;

	[SerializeField]
	[Range(0f, 1f)]
	private float _distortion = 0.1f;

	[SerializeField]
	private Vector2 _scroll = new Vector2(0f, 0f);

	private int _dissolveTexId = Shader.PropertyToID("_DissolveTex");

	private int _dissolveRangeId = Shader.PropertyToID("_DissolveRange");

	private int _yAmountId = Shader.PropertyToID("_YAmount");

	private int _yRangeId = Shader.PropertyToID("_YRange");

	private int _scrollId = Shader.PropertyToID("_Scroll");

	private int _glowColorId = Shader.PropertyToID("_GlowColor");

	private int _distortionId = Shader.PropertyToID("_Distortion");

	protected override void UpdateMaterial(Material baseMaterial)
	{
		if (material == null)
		{
			material = new Material(Shader.Find("Applibot/UI/Dissolve"));
			material.hideFlags = HideFlags.HideAndDontSave;
		}
		material.SetTexture(_dissolveTexId, _dissovleTex);
		material.SetFloat(_dissolveRangeId, _dissolveRange);
		material.SetFloat(_yAmountId, _yAmount);
		material.SetFloat(_yRangeId, _yRange);
		material.SetVector(_scrollId, _scroll);
		material.SetColor(_glowColorId, _glowColor);
		material.SetFloat(_distortionId, _distortion);
	}
}
