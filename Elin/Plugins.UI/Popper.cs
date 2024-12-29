using System;
using DG.Tweening;
using UnityEngine;

public class Popper : MonoBehaviour
{
	public static Vector3 scale = Vector3.one;

	[NonSerialized]
	public bool isDestryoed;

	public DOTweenAnimation anime;

	public SpriteRenderer sr;

	public Vector3 posFix;

	public Vector3 posRandom;

	public bool useLocalPosition;

	public Tween tweenDelay;

	public TextMesh text;

	public TextMesh text2;

	public Popper SetText(string s, Color c = default(Color))
	{
		SetText(text, s);
		base.transform.localScale = scale;
		if (c != default(Color))
		{
			text.color = c;
			text2.color = c;
		}
		return this;
	}

	public static void SetText(TextMesh mesh, string text)
	{
		TextMesh[] componentsInChildren = mesh.GetComponentsInChildren<TextMesh>();
		foreach (TextMesh obj in componentsInChildren)
		{
			obj.font = SkinManager.Instance.fontSet.ui.source.font;
			obj.text = text;
			obj.font.material.renderQueue = 4999;
			obj.GetComponent<MeshRenderer>().sharedMaterial = mesh.font.material;
		}
		mesh.font.material.renderQueue = 5000;
	}
}
