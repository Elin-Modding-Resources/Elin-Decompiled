using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PopperManager : MonoBehaviour
{
	public List<Popper> list = new List<Popper>();

	public Popper Pop(Vector3 pos, string id, Sprite sprite)
	{
		Popper popper = Pop(pos, null, id);
		popper.sr.sprite = sprite;
		return popper;
	}

	public Popper Pop(Vector3 pos, string id = "Default")
	{
		return Pop(pos, null, id);
	}

	public Popper Pop(Func<Vector3> func, string id = "Default")
	{
		return Pop(func(), func, id);
	}

	public Popper Pop(Vector3 pos, Func<Vector3> func, string id = "Default")
	{
		Popper p = PoolManager.Spawn<Popper>("Popper" + id, "UI/Pop/Popper" + id);
		p.SetActive(enable: true);
		if (p.useLocalPosition)
		{
			p.anime.transform.localPosition = new Vector3(p.posFix.x + p.posRandom.x * UnityEngine.Random.Range(-1f, 1f), p.posFix.y + p.posRandom.y * UnityEngine.Random.Range(-1f, 1f), p.posFix.z - 0.01f * (float)list.Count);
		}
		else
		{
			p.anime.transform.position = new Vector3(pos.x + p.posFix.x + p.posRandom.x * UnityEngine.Random.Range(-1f, 1f), pos.y + p.posFix.y + p.posRandom.y * UnityEngine.Random.Range(-1f, 1f), pos.z + p.posFix.z - 0.01f * (float)list.Count);
		}
		p.anime.onComplete.AddListener(delegate
		{
			Kill(p);
		});
		if (func != null)
		{
			p.anime.onUpdate.AddListener(delegate
			{
				p.transform.position = func();
			});
		}
		DOTweenAnimation[] componentsInChildren = p.GetComponentsInChildren<DOTweenAnimation>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].DORestart(fromHere: true);
		}
		list.Add(p);
		return p;
	}

	public void Kill(Popper p, bool removeFromList = true)
	{
		if (removeFromList)
		{
			list.Remove(p);
		}
		p.anime.onComplete.RemoveAllListeners();
		if (p.anime.onUpdate != null)
		{
			p.anime.onUpdate.RemoveAllListeners();
		}
		if (p.anime.onPlay != null)
		{
			p.anime.onPlay.RemoveAllListeners();
		}
		TweenUtil.KillTween(ref p.tweenDelay);
		PoolManager.Despawn(p);
	}

	public void KillAll()
	{
		foreach (Popper item in list)
		{
			Kill(item, removeFromList: false);
		}
		list.Clear();
	}
}
