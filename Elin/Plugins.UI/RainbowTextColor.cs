using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Text))]
public sealed class RainbowTextColor : MonoBehaviour
{
	[Header("Duration")]
	[Tooltip("虹色アニメーションを行う秒数")]
	[Min(0f)]
	public float duration = 3f;

	[Header("Speed")]
	[Tooltip("1秒あたり何周Hueを回すか（1=1周/秒）")]
	[Min(0f)]
	public float cyclesPerSecond = 1f;

	[Header("Look")]
	[Tooltip("彩度")]
	[Range(0f, 1f)]
	public float saturation = 1f;

	[Tooltip("明度")]
	[Range(0f, 1f)]
	public float value = 1f;

	[Tooltip("元のアルファに対して掛ける係数（1=そのまま）")]
	[Range(0f, 1f)]
	public float alphaMultiplier = 1f;

	[Header("Playback")]
	[Tooltip("開始時に自動再生")]
	public bool playOnEnable = true;

	[Tooltip("終了時に元の色へ戻す")]
	public bool restoreOriginalColor = true;

	private Text _text;

	private Coroutine _routine;

	private Color _originalColor;

	private void Awake()
	{
		_text = GetComponent<Text>();
		_originalColor = _text.color;
	}

	private void OnEnable()
	{
		if (playOnEnable)
		{
			Play();
		}
	}

	private void OnDisable()
	{
		Stop();
		if (restoreOriginalColor && _text != null)
		{
			_text.color = _originalColor;
		}
	}

	public void Play()
	{
		if (_text == null)
		{
			_text = GetComponent<Text>();
		}
		if (!(_text == null))
		{
			_originalColor = _text.color;
			if (_routine != null)
			{
				StopCoroutine(_routine);
			}
			_routine = StartCoroutine(Run());
		}
	}

	public void Stop()
	{
		if (_routine != null)
		{
			StopCoroutine(_routine);
			_routine = null;
		}
		if (restoreOriginalColor && _text != null)
		{
			_text.color = _originalColor;
		}
	}

	private IEnumerator Run()
	{
		if (duration <= 0f)
		{
			_routine = null;
			yield break;
		}
		float elapsed = 0f;
		while (elapsed < duration)
		{
			elapsed += Time.unscaledDeltaTime;
			Color color = Color.HSVToRGB(Mathf.Repeat(elapsed * cyclesPerSecond, 1f), saturation, value);
			color.a = _originalColor.a * alphaMultiplier;
			_text.color = color;
			yield return null;
		}
		if (restoreOriginalColor)
		{
			_text.color = _originalColor;
		}
		_routine = null;
	}
}
