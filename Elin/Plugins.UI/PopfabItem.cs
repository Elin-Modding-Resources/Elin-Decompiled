using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopfabItem : MonoBehaviour
{
	public Transform animeTarget;

	public CanvasGroup animeGroup;

	public Anime intro;

	public Anime outro;

	public Anime outroForce;

	public Anime loop;

	public float duration = 6f;

	public bool pooling;

	public Vector3 fixPos = Vector3.zero;

	private bool isKilled;

	public bool startTransparent;

	public bool clamp;

	[Header("For Canvas")]
	public Text uiText;

	public bool screenSpace = true;

	public ContentSizeFitter fitter;

	public Image icon;

	public bool stopFollow;

	[NonSerialized]
	public float destX;

	[NonSerialized]
	public float destY;

	[NonSerialized]
	public RectTransform rect;

	[NonSerialized]
	public Popfab reciever;

	[NonSerialized]
	public float destTime;

	[NonSerialized]
	public bool isOutro;

	[NonSerialized]
	public Transform followTarget;

	[NonSerialized]
	private Vector3? lastPosition;

	[NonSerialized]
	public bool isUseDuration = true;

	[NonSerialized]
	public bool hasRect;

	private void Awake()
	{
		rect = base.transform as RectTransform;
		hasRect = rect;
		if (animeTarget == null)
		{
			animeTarget = rect;
		}
	}

	public void Set(string text)
	{
		if ((bool)uiText)
		{
			uiText.text = text;
		}
	}

	public void Set(BalloonData data)
	{
		Set(data.text);
		if (data.font != null && (bool)uiText)
		{
			uiText.font = data.font;
		}
		if (data.fontSize != 0 && (bool)uiText)
		{
			uiText.fontSize = data.fontSize;
		}
	}

	public void Init(Transform parent = null)
	{
		if ((bool)parent)
		{
			base.transform.SetParent(parent, worldPositionStays: false);
		}
		animeTarget.localScale = Vector3.one;
		animeTarget.localEulerAngles = Vector3.zero;
		animeTarget.localPosition = new Vector3(0f, 0f, animeTarget.localPosition.z);
		if (stopFollow && (bool)followTarget)
		{
			lastPosition = followTarget.position;
		}
		if (fitter != null)
		{
			fitter.SetLayoutHorizontal();
			fitter.SetLayoutVertical();
		}
		if (startTransparent)
		{
			if ((bool)animeGroup)
			{
				animeGroup.alpha = 0f;
			}
			else if ((bool)uiText)
			{
				Color color = uiText.color;
				color.a = 0f;
				uiText.color = color;
			}
		}
	}

	public void Intro()
	{
		destTime = Time.realtimeSinceStartup + duration;
		LateUpdate();
		this.RebuildLayout();
		if (!(intro == null))
		{
			intro.Play(animeTarget);
		}
	}

	private void LateUpdate()
	{
		if (!isOutro && isUseDuration && Time.realtimeSinceStartup > destTime)
		{
			Outro();
		}
		if ((bool)followTarget)
		{
			if (!followTarget.gameObject.activeInHierarchy && !lastPosition.HasValue)
			{
				Kill();
				return;
			}
			if (!stopFollow && followTarget.gameObject.activeInHierarchy)
			{
				lastPosition = followTarget.position;
			}
			if (hasRect)
			{
				rect.position = Camera.main.WorldToScreenPoint(lastPosition ?? followTarget.position) + fixPos;
			}
			else
			{
				base.transform.position = (lastPosition ?? followTarget.position) + fixPos;
			}
		}
		if (clamp)
		{
			ClampToScreen();
		}
	}

	private void ClampToScreen()
	{
		if (hasRect)
		{
			Vector3 zero = Vector3.zero;
			Vector3 vector = (base.transform.parent as RectTransform).rect.min - rect.rect.min;
			Vector3 vector2 = (base.transform.parent as RectTransform).rect.max - rect.rect.max;
			zero.x = Mathf.Clamp(rect.localPosition.x, vector.x, vector2.x);
			zero.y = Mathf.Clamp(rect.localPosition.y, vector.y, vector2.y);
			rect.localPosition = zero;
		}
		else
		{
			Vector3 position = Camera.main.WorldToViewportPoint(base.transform.position);
			position.x = Mathf.Clamp(position.x, 0.1f, 0.9f);
			position.y = Mathf.Clamp(position.y, 0.1f, 0.9f);
			base.transform.position = Camera.main.ViewportToWorldPoint(position);
		}
	}

	public void ForceOutro()
	{
		if (!isOutro)
		{
			isOutro = true;
			if ((bool)outroForce)
			{
				outroForce.Play(animeTarget, Kill);
			}
			else
			{
				Kill();
			}
		}
	}

	public void Outro()
	{
		isOutro = true;
		if (outro == null)
		{
			destY += 80f;
			animeTarget.DOLocalMoveY(destY, 1.6f).SetEase(Ease.OutQuart).OnComplete(Kill);
		}
		else
		{
			outro.Play(animeTarget, Kill);
		}
	}

	public void Kill()
	{
		if (!isKilled)
		{
			base.transform.DOKill();
			if ((bool)reciever)
			{
				reciever.Remove(this);
			}
			if (pooling)
			{
				stopFollow = false;
				isOutro = false;
				isKilled = false;
				isUseDuration = true;
				lastPosition = null;
				destY = 0f;
			}
			else
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			isKilled = true;
		}
	}

	public void SetDuration(float duration)
	{
		this.duration = duration;
		destTime = Time.realtimeSinceStartup + duration;
	}
}
