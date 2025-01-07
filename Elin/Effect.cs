using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Effect : SceneObject
{
	public enum Type
	{
		Default,
		Firework
	}

	public Type type;

	public float duration = 1f;

	public float speed;

	public float startDelay;

	public bool lookAtTarget;

	public bool rotate;

	public bool pool;

	public SpriteRenderer sr;

	public ParticleSystem[] systems;

	public Vector3[] dirs;

	public Ease ease;

	public Vector3 posFix;

	public Vector3 randomRange;

	public Tween moveTween;

	public Sprite[] sprites;

	public bool randomFlip;

	public bool randomSpeed;

	public bool test;

	public bool setColor = true;

	public Action onComplete;

	[NonSerialized]
	public int spriteIndex;

	[NonSerialized]
	public float timer;

	[NonSerialized]
	public Vector3 fromV;

	[NonSerialized]
	public Vector3 destV;

	[NonSerialized]
	public bool pooled;

	[NonSerialized]
	public Transform poolParent;

	[NonSerialized]
	public List<Material> materialsToDestroy;

	[NonSerialized]
	public Point destPos;

	protected bool killed;

	[NonSerialized]
	public Tween killTimer;

	public static EffectManager manager => EffectManager.Instance;

	public static Effect Get(Effect original)
	{
		return UnityEngine.Object.Instantiate(original);
	}

	public static Effect Get(string id)
	{
		return manager.effects.Get(id);
	}

	public static T Get<T>(string id) where T : Effect
	{
		return manager.effects.Get(id) as T;
	}

	public void Play(float delay, Point from, float fixY = 0f, Point to = null, Sprite sprite = null)
	{
		Point _from = from.Copy();
		Point _to = to?.Copy() ?? null;
		TweenUtil.Tween(delay, delegate
		{
			Play(_from, fixY, _to, sprite);
		});
	}

	public Effect Play(Point from, float fixY = 0f, Point to = null, Sprite sprite = null)
	{
		return _Play(from, from.Position(), fixY, to, sprite);
	}

	public Effect _Play(Point from, Vector3 fromV, float fixY = 0f, Point to = null, Sprite sprite = null)
	{
		if (!EMono.core.IsGameStarted)
		{
			Kill();
			return null;
		}
		if ((bool)sprite)
		{
			sr.sprite = sprite;
		}
		if (setColor)
		{
			float num = 0.07f * (float)(int)from.cell.light + EMono.core.screen.tileMap._baseBrightness;
			num += ((from.cell.HasRoof || from.cell.isShadowed) ? (-0.2f) : 0f);
			if ((bool)sr)
			{
				sr.material.SetVector("_Color", new Vector4(num, num, num, 1f));
			}
		}
		fromV += posFix;
		fromV.y += fixY;
		if (to == null)
		{
			fromV += randomRange.Random();
		}
		this.fromV = fromV;
		destPos = to ?? from;
		base.transform.position = fromV;
		if (randomFlip)
		{
			sr.flipX = EMono.rnd(2) == 0;
		}
		if (to != null)
		{
			float num2 = Mathf.Min(speed * (float)from.Distance(to), duration);
			destV = to.Position() + posFix + randomRange.Random();
			if (rotate)
			{
				base.transform.rotation = from.GetRotation(to);
			}
			moveTween = base.transform.DOMove(destV, num2).SetEase(Ease.Linear).SetDelay(startDelay);
		}
		Activate();
		OnPlay();
		OnUpdate();
		return this;
	}

	public Effect Play(Vector3 v)
	{
		if (!EMono.core.IsGameStarted)
		{
			Kill();
			return null;
		}
		fromV = v;
		base.transform.position = fromV + posFix;
		Activate();
		OnPlay();
		OnUpdate();
		return this;
	}

	protected void Activate()
	{
		if (sprites.Length != 0 && type != Type.Firework)
		{
			sr.enabled = false;
		}
		manager.Add(this);
		killTimer = TweenUtil.Tween(startDelay + duration + 0.01f + (float)sprites.Length * 0.01f, null, Kill);
	}

	public Effect Flip(bool x = false, bool y = false)
	{
		if ((bool)sr)
		{
			sr.flipX = (sr.flipX ? (!x) : x);
			sr.flipY = (sr.flipY ? (!y) : y);
		}
		return this;
	}

	public Effect SetStartDelay(float a)
	{
		startDelay = a;
		return this;
	}

	public virtual void OnPlay()
	{
	}

	public virtual void OnUpdate()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
		}
		timer += (randomSpeed ? UnityEngine.Random.Range(Time.deltaTime, Time.deltaTime * 1.5f) : Time.deltaTime);
		if (startDelay != 0f)
		{
			if (timer < startDelay)
			{
				return;
			}
			timer = (startDelay = 0f);
		}
		if (sprites.Length != 0 && type != Type.Firework && timer > duration / (float)sprites.Length)
		{
			timer = 0f;
			spriteIndex++;
			if (spriteIndex >= sprites.Length)
			{
				Kill();
				return;
			}
			sr.enabled = true;
			sr.sprite = sprites[spriteIndex];
		}
	}

	public void Kill()
	{
		TweenUtil.KillTween(ref killTimer);
		TweenUtil.KillTween(ref moveTween);
		killed = true;
		OnKill();
		manager.Remove(this);
		if (pool && manager.effects.usePool)
		{
			PoolManager.DespawnOrDestroy(base.transform);
		}
		else if ((bool)base.gameObject)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public virtual void OnKill()
	{
	}

	public void OnDisable()
	{
		if ((bool)base.transform.parent && !test)
		{
			Kill();
		}
	}

	public void OnDestroy()
	{
		if (materialsToDestroy == null)
		{
			return;
		}
		foreach (Material item in materialsToDestroy)
		{
			UnityEngine.Object.Destroy(item);
		}
	}

	public Effect Emit(int num)
	{
		ParticleSystem[] array = systems;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Emit(num);
		}
		return this;
	}

	public Effect SetParticleColor(Color c)
	{
		ParticleSystem[] array = systems;
		for (int i = 0; i < array.Length; i++)
		{
			ParticleSystem.MainModule main = array[i].main;
			main.startColor = c;
		}
		return this;
	}

	public Effect SetParticleColor(Color color, bool changeMaterial = false, string idCol = "_Color")
	{
		if (changeMaterial)
		{
			materialsToDestroy = new List<Material>();
		}
		ParticleSystem[] array = systems;
		foreach (ParticleSystem particleSystem in array)
		{
			if (changeMaterial)
			{
				Material material = particleSystem.GetComponent<ParticleSystemRenderer>().material;
				materialsToDestroy.Add(material);
				material.SetColor(idCol, color);
			}
			else
			{
				ParticleSystem.MainModule main = particleSystem.main;
				main.startColor = color;
			}
		}
		return this;
	}

	public Effect SetScale(float a)
	{
		base.transform.localScale *= a;
		return this;
	}
}
