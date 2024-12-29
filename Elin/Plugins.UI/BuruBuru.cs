using System;
using UnityEngine;

public class BuruBuru : MonoBehaviour
{
	private Vector3 _originalPos;

	[SerializeField]
	private float _haba;

	private void Start()
	{
		Application.targetFrameRate = 60;
		_originalPos = base.transform.localPosition;
	}

	private void Update()
	{
		float num = _haba * UnityEngine.Random.Range(0.2f, 1f);
		float num2 = UnityEngine.Random.Range(0f, 360f);
		double num3 = (double)num * Math.Cos(MathF.PI / 180f * num2);
		double num4 = (double)num * Math.Sin(MathF.PI / 180f * num2);
		base.transform.localPosition = _originalPos + new Vector3((float)num3, (float)num4, 0f);
	}
}
