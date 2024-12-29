using System;
using System.Collections.Generic;
using UnityEngine;

public class CircularRadialButton : MonoBehaviour
{
	public UIButton moldButton;

	public float MainButtonMinSize = 0.5f;

	public float MainButtonMaxSize = 1f;

	public AnimationCurve MainButtonImageCurve;

	public bool CloseChildOptionsOnClose;

	[Header("Options Settings")]
	public List<RectTransform> Options;

	public float OptionsMinRadius = 75f;

	public float OptionsMaxRadius = 120f;

	[Tooltip("the scale size the options will have when the menu is collapsed.")]
	public float OptionsMinSize = 0.1f;

	[Tooltip("the scale size the options will have when the menu is expanded")]
	public float OptionsMaxSize = 1f;

	[Tooltip("this defines the options’ angle separation when the menu is collapsed.")]
	public float MinAngleDistanceBetweenOptions = 15f;

	[Tooltip("this defines the options’ angle separation when the menu is expanded.")]
	public float MaxAngleDistanceBetweenOptions = 35f;

	[Tooltip("defines the start angle for the previous options (counterclockwise)")]
	public float StartAngleOffset;

	[Tooltip("sets whether the menu options are shown one by one or all at once.")]
	public bool ShowAllOptionsAtOnce;

	[Tooltip("defines the time is takes from the collapsed menu to be expanded.")]
	public float OpenTransitionTime = 0.5f;

	[Tooltip("defines the time is takes from the expanded menu to be collapsed.")]
	public float CloseTransitionTime = 0.5f;

	private float TransitionTime = 0.5f;

	[Tooltip("is used to define the options’ scale size over Open Transition time.")]
	public AnimationCurve OpenCurve;

	[Tooltip("is used to define the options’ scale size over Close Transition time.")]
	public AnimationCurve CloseCurve;

	private AnimationCurve CurrentCurve;

	private bool Open;

	private float T;

	public void Init()
	{
		moldButton.SetActive(enable: false);
		CurrentCurve = (Open ? OpenCurve : CloseCurve);
		TransitionTime = (Open ? OpenTransitionTime : CloseTransitionTime);
		T = TransitionTime;
		MaxAngleDistanceBetweenOptions = 360 / Options.Count;
		StartAngleOffset = 90f;
		if (Options.Count == 10)
		{
			StartAngleOffset = 108f;
		}
		Animate();
		Toggle();
		int num = 4;
		for (int i = 0; i < Options.Count; i++)
		{
			num++;
			if (num >= Options.Count)
			{
				num = 0;
			}
			Options[num].transform.SetAsFirstSibling();
		}
	}

	public void Toggle()
	{
		if (T < TransitionTime)
		{
			return;
		}
		Open = !Open;
		CurrentCurve = (Open ? OpenCurve : CloseCurve);
		TransitionTime = (Open ? OpenTransitionTime : CloseTransitionTime);
		T = 0f;
		if (!Open && CloseChildOptionsOnClose)
		{
			CircularRadialButton[] componentsInChildren = GetComponentsInChildren<CircularRadialButton>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].CloseIfOpen();
			}
		}
	}

	public void CloseIfOpen()
	{
		if (Open)
		{
			Toggle();
		}
	}

	private void Update()
	{
		if (!(T >= TransitionTime))
		{
			T += Time.deltaTime;
			if (T > TransitionTime)
			{
				T = TransitionTime;
			}
			Animate();
		}
	}

	private void Animate()
	{
		int count = Options.Count;
		float num = TransitionTime / (float)count;
		float time = T * 2f / TransitionTime;
		float num2 = MinAngleDistanceBetweenOptions + (MaxAngleDistanceBetweenOptions - MinAngleDistanceBetweenOptions) * CurrentCurve.Evaluate(time);
		float num3 = num2 * (float)(count - 1) / 2f + StartAngleOffset;
		for (int i = 0; i < count; i++)
		{
			if (!(Options[i] == null))
			{
				float time2 = (T - num * (float)i) / num;
				if (T >= num * (float)i && T < num * (float)(i + 1))
				{
					time2 = (T - num * (float)i) / num;
					float num4 = OptionsMinSize + (OptionsMaxSize - OptionsMinSize) * CurrentCurve.Evaluate(time2);
					Options[i].transform.localScale = new Vector3(num4, num4, num4);
				}
				if (ShowAllOptionsAtOnce || T >= num * (float)(i + 1))
				{
					time2 = T / num;
					float num5 = OptionsMinSize + (OptionsMaxSize - OptionsMinSize) * CurrentCurve.Evaluate(time2);
					Options[i].transform.localScale = new Vector3(num5, num5, num5);
				}
				float num6 = OptionsMinRadius + (OptionsMaxRadius - OptionsMinRadius) * CurrentCurve.Evaluate(time2);
				float x = num6 * Mathf.Cos(MathF.PI / 180f * num3);
				float y = num6 * Mathf.Sin(MathF.PI / 180f * num3);
				Options[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
				num3 -= num2;
			}
		}
	}

	public UIButton AddOption(Sprite sprite, Action action = null)
	{
		UIButton uIButton = Util.Instantiate(moldButton, base.transform);
		uIButton.SetActive(enable: true);
		uIButton.icon.sprite = sprite;
		RectTransform rectTransform = uIButton.Rect();
		if (Options == null)
		{
			Options = new List<RectTransform>();
		}
		Options.Add(rectTransform);
		_ = Options.Count;
		uIButton.onClick.SetListener(action);
		rectTransform.localPosition = new Vector3(0f, 0f, 0f);
		return uIButton;
	}
}
