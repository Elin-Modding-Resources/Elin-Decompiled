using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EffectMeteor : Effect
{
	public Animator aniExplosion;

	public Vector3 startPos;

	public float time;

	public override void OnPlay()
	{
		sr.enabled = true;
		aniExplosion.SetActive(enable: false);
		destV = fromV;
		fromV += startPos + startPos.Random() * 0.2f;
		base.transform.position = fromV;
		moveTween = base.transform.DOMove(destV, time).SetEase(Ease.Linear).SetDelay(startDelay)
			.OnComplete(delegate
			{
				sr.enabled = false;
				aniExplosion.SetActive(enable: true);
				destPos.Animate(AnimeID.Dig, animeBlock: true);
				onComplete?.Invoke();
				EMono.Sound.Play("explode", destV);
				Shaker.ShakeCam("meteor");
			});
	}

	public static void Create(Point center, int radius, int count, Action<int, Point> onComplete)
	{
		List<Point> list = new List<Point>();
		for (int i = 0; i < count; i++)
		{
			Point p = center.Copy();
			Effect effect = Effect.Get("meteor");
			effect.startDelay = Rand.Range(0f, 0.5f);
			if (radius > 0)
			{
				int num = 0;
				if (num < 1000)
				{
					Point point = ((radius == 0) ? center : EMono._map.GetRandomSurface(center.x, center.z, radius));
					foreach (Point item in list)
					{
						point.Equals(item);
					}
					p.Set(point);
					list.Add(point);
				}
			}
			int _i = i;
			effect.onComplete = delegate
			{
				onComplete(_i, p);
			};
			effect.Play(p);
		}
	}
}
