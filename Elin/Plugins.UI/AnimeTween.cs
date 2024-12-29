using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class AnimeTween : Anime
{
	public bool autoPlay;

	public bool targetParent = true;

	private List<DOTweenAnimation> animes = new List<DOTweenAnimation>();

	public Transform target
	{
		get
		{
			if (!targetParent)
			{
				return base.transform;
			}
			return base.transform.parent;
		}
	}

	private void Awake()
	{
		Refresh();
		if (autoPlay)
		{
			Play(target);
		}
	}

	public void Refresh()
	{
		animes = GetComponents<DOTweenAnimation>().ToList();
		foreach (DOTweenAnimation anime in animes)
		{
			anime.target = target;
		}
	}

	public void PlayDebug()
	{
		Refresh();
		Play(target);
	}

	public void Kill()
	{
		foreach (DOTweenAnimation anime in animes)
		{
			if (anime.tween != null)
			{
				anime.tween.Kill();
			}
		}
	}

	public void Rewind()
	{
		Kill();
		target.SetLocalScale(1f, 1f, 1f);
		target.SetEulerAngles(0f, 0f, 0f);
		Play(target);
	}

	public void Play()
	{
		Play(target);
	}

	public override Tween Play(Transform trans, UnityAction onComplete = null, float duration = -1f, float delay = 0f)
	{
		if (animes.Count == 0)
		{
			Refresh();
		}
		bool flag = true;
		Sequence sequence = DOTween.Sequence();
		foreach (DOTweenAnimation anime in animes)
		{
			anime.target = trans;
			if (flag && onComplete != null)
			{
				anime.CreateTween(onComplete, duration);
				flag = false;
			}
			else
			{
				anime.CreateTween(null, duration);
			}
			if (delay != 0f)
			{
				anime.tween.SetDelay(anime.tween.Delay() + delay);
			}
			sequence.Join(anime.tween);
		}
		sequence.Play();
		return sequence;
	}
}
