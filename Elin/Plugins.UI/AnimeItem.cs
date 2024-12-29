using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class AnimeItem
{
	public Anime intro;

	public Anime outro;

	public Anime loop;

	public Transform target;

	public float duration = -1f;

	public void Play(AnimePhase phase, UnityAction onComplete = null)
	{
		if (phase == AnimePhase.Intro && intro != null)
		{
			intro.Play(target, onComplete, duration);
		}
		if (phase == AnimePhase.Outro && outro != null)
		{
			outro.Play(target, onComplete, duration);
		}
	}
}
