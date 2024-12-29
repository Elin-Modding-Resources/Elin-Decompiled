using UnityEngine;
using UnityEngine.UI;

namespace Applibot;

[ExecuteAlways]
public class Glitch : CustomImageBase
{
	[SerializeField]
	[Range(0f, 2f)]
	private float _scanLineJitter;

	[SerializeField]
	[Range(1f, 300f)]
	private float _JitterSize = 300f;

	[SerializeField]
	[Range(0f, 1f)]
	private float _verticalJump;

	[SerializeField]
	[Range(0f, 1f)]
	private float _horizontalShake;

	[SerializeField]
	[Range(-2f, 2f)]
	private float _colorDrift;

	[SerializeField]
	private Color _ScanlineColor;

	[SerializeField]
	private float _ScanlineSize = 1.2f;

	[SerializeField]
	[Range(1f, 5f)]
	private float _ColorStrength = 1f;

	private float _verticalJumpTime;

	private RawImage _RawImage;

	protected override void UpdateMaterial(Material baseMaterial)
	{
		if (material == null)
		{
			material = new Material(Shader.Find("Custom/UI/Glitch"));
			material.hideFlags = HideFlags.DontSave;
		}
		_verticalJumpTime += Time.deltaTime * _verticalJump * 11.3f;
		float y = Mathf.Clamp01(1f - _scanLineJitter * 1.2f);
		float x = 0.002f + Mathf.Pow(_scanLineJitter, 3f) * 0.05f;
		material.SetVector("_ScanLineJitter", new Vector2(x, y));
		material.SetFloat("_JitterSize", _JitterSize);
		Vector2 vector = new Vector2(_verticalJump, _verticalJumpTime);
		material.SetVector("_VerticalJump", vector);
		material.SetFloat("_HorizontalShake", _horizontalShake * 0.2f);
		material.SetFloat("_ColorDriftAmount", _colorDrift * 0.04f);
		material.SetColor("_ScanlineColor", _ScanlineColor);
		material.SetFloat("_ScanlineSize", _ScanlineSize);
		material.SetFloat("_ColorStrength", _ColorStrength);
	}
}
