using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIAnimeGroup : UIAnime
{
	public List<AnimeItem> animes = new List<AnimeItem>();

	public override void Play(AnimePhase phase, UnityAction onComplete = null)
	{
		bool flag = false;
		foreach (AnimeItem anime in animes)
		{
			if (flag)
			{
				anime.Play(phase);
				continue;
			}
			anime.Play(phase, delegate
			{
				OnComplete(onComplete);
			});
			flag = true;
		}
		base.Play(phase, onComplete);
	}

	public UIAnimeGroup AddAnime(Transform target, Anime intro, Anime outro = null, Anime loop = null)
	{
		animes.Add(new AnimeItem
		{
			target = target,
			intro = intro,
			outro = outro,
			loop = loop
		});
		return this;
	}

	public override bool Has(AnimePhase phase)
	{
		foreach (AnimeItem anime in animes)
		{
			if ((phase == AnimePhase.Intro && (bool)anime.intro) || (phase == AnimePhase.Outro && (bool)anime.outro))
			{
				return true;
			}
		}
		return false;
	}
}
