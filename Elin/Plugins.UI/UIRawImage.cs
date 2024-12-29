using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIRawImage : RawImage, IUISkin
{
	public ImageType imageType;

	public BaseSkinRoot skinRoot;

	public bool animate;

	public Vector2 animeSpeed;

	protected override void Awake()
	{
		ApplySkin();
		base.Awake();
	}

	protected override void OnEnable()
	{
		if (animate)
		{
			StartCoroutine("Animate");
		}
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		if (animate)
		{
			StopCoroutine("Animate");
		}
		base.OnDisable();
	}

	public IEnumerator Animate()
	{
		while (true)
		{
			Rect rect = base.uvRect;
			rect.x += animeSpeed.x * Time.smoothDeltaTime;
			rect.y += animeSpeed.y * Time.smoothDeltaTime;
			base.uvRect = rect;
			yield return true;
		}
	}

	public void ApplySkin()
	{
		BaseSkinRoot baseSkinRoot = skinRoot ?? SkinManager.Instance.currentSkin;
		baseSkinRoot.ApplySkin(this);
		if (imageType == ImageType.Line)
		{
			color = baseSkinRoot.Colors.colorLine;
		}
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		if (color.a == 0f || !base.texture)
		{
			vh.Clear();
		}
		else
		{
			base.OnPopulateMesh(vh);
		}
	}
}
