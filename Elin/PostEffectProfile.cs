using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class PostEffectProfile : EScriptable
{
	public float Brightness;

	public float Saturation;

	public float Contrast;

	public int sharpen = 20;

	public int sharpen2 = 80;

	public int blur;

	public bool enableAA;

	public bool enableCharaAA;

	public bool disable;

	public bool kuwahara;

	public AAMode aaMode;

	public float offsetScale;

	public float blurRadius;

	private void OnValidate()
	{
		if (Application.isPlaying)
		{
			Apply(EClass.scene.cam);
		}
	}

	public void OnChangeProfile()
	{
		CoreConfig.GraphicSetting graphic = EClass.core.config.graphic;
		graphic.sharpen = sharpen;
		graphic.sharpen2 = sharpen2;
		graphic.blur = blur;
		graphic.kuwahara = kuwahara;
	}

	public void Apply(Camera cam)
	{
		Antialiasing component = cam.transform.GetComponent<Antialiasing>();
		if ((bool)component)
		{
			component.enabled = enableAA;
			component.mode = aaMode;
			component.offsetScale = offsetScale;
			component.blurRadius = blurRadius;
		}
		if (enableCharaAA)
		{
			EClass.scene.screenElin.tileMap.passChara.mat.EnableKeyword("AA_ON");
			EClass.scene.screenElin.tileMap.passCharaL.mat.EnableKeyword("AA_ON");
			EClass.scene.screenElin.tileMap.passCharaLW.mat.EnableKeyword("AA_ON");
		}
		else
		{
			EClass.scene.screenElin.tileMap.passChara.mat.DisableKeyword("AA_ON");
			EClass.scene.screenElin.tileMap.passCharaL.mat.DisableKeyword("AA_ON");
			EClass.scene.screenElin.tileMap.passCharaLW.mat.DisableKeyword("AA_ON");
		}
	}
}
