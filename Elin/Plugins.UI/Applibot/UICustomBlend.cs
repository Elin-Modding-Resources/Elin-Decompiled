using UnityEngine;
using UnityEngine.Rendering;

namespace Applibot;

public class UICustomBlend : CustomImageBase
{
	public enum MyCustomBlendMode : byte
	{
		Normal,
		LinearDodge,
		Darken,
		Multiply,
		Subtract,
		Lighten
	}

	[SerializeField]
	private MyCustomBlendMode _blendMode;

	private readonly int _SrcFactor = Shader.PropertyToID("_SrcFactor");

	private readonly int _DstFactor = Shader.PropertyToID("_DstFactor");

	private readonly int _BlendOp = Shader.PropertyToID("_BlendOp");

	protected override void UpdateMaterial(Material baseMaterial)
	{
		if (material == null)
		{
			material = new Material(Shader.Find("Applibot/UI/CustomBlend"));
			material.hideFlags = HideFlags.HideAndDontSave;
		}
		material.enabledKeywords = new LocalKeyword[0];
		switch (_blendMode)
		{
		case MyCustomBlendMode.Normal:
			material.SetInt(_SrcFactor, 1);
			material.SetInt(_DstFactor, 10);
			material.SetInt(_BlendOp, 0);
			break;
		case MyCustomBlendMode.LinearDodge:
			material.SetInt(_SrcFactor, 5);
			material.SetInt(_DstFactor, 1);
			material.SetInt(_BlendOp, 0);
			break;
		case MyCustomBlendMode.Multiply:
			material.SetInt(_SrcFactor, 0);
			material.SetInt(_DstFactor, 3);
			material.SetInt(_BlendOp, 0);
			break;
		case MyCustomBlendMode.Darken:
			material.SetInt(_SrcFactor, 1);
			material.SetInt(_DstFactor, 1);
			material.SetInt(_BlendOp, 3);
			break;
		case MyCustomBlendMode.Subtract:
			material.SetInt(_SrcFactor, 1);
			material.SetInt(_DstFactor, 1);
			material.SetInt(_BlendOp, 2);
			break;
		case MyCustomBlendMode.Lighten:
			material.SetInt(_SrcFactor, 1);
			material.SetInt(_DstFactor, 1);
			material.SetInt(_BlendOp, 4);
			break;
		}
		material.EnableKeyword(_blendMode.ToString());
	}
}
