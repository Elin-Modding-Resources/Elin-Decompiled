using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HoardActor : EMono
{
	[Serializable]
	public class BG
	{
		public Sprite sprite;

		public float reflection;

		public float shadow;
	}

	public Camera cam;

	public Vector3 posCenter;

	public float radiusX;

	public float radiusY;

	public float sizeMod;

	public float sizeModCircle;

	public float reflectMod;

	public new bool debug;

	public List<CollectibleActor> actors;

	public List<RigidUpdate> updates;

	public MeshPass passShadow;

	public Material matReflection;

	public Material matMani;

	public AnimationCurve curveMani;

	public SpriteRenderer srBG;

	public SpriteRenderer srReflection;

	public ParticleSystem psExplode;

	public BG[] bgs;

	private CollectibleActor renton;

	private CollectibleActor ehe;

	private bool angryEhe;

	private float eheTimer = -5f;

	private int eheCount;

	public float groundY = -4f;

	public float shdowLimitY = 1f;

	public float alphaFix1 = 0.3f;

	public float alphaFix2 = 0.5f;

	public float alphaFix4 = 0.15f;

	public float maxAplha = 0.6f;

	public float sizeFixY = -0.1f;

	public Hoard hoard => EMono.player.hoard;

	public BG bg => bgs[hoard.bg];

	public void Activate()
	{
		passShadow.Init();
		hoard.score = 0;
		RefreshBG();
		RefreshZoom();
		RigidUpdate.leftX = -5f;
		RigidUpdate.rightX = 5f;
		if (actors.Count > 0)
		{
			Clear();
		}
		angryEhe = EMono.rnd(2) == 0;
		Vector3 zero = Vector3.zero;
		int num = 0;
		foreach (Hoard.Item value in hoard.items.Values)
		{
			num += value.show;
		}
		IList<Hoard.Item> list = new List<Hoard.Item>(hoard.items.Values).Shuffle();
		string text = hoard.mode.ToString();
		foreach (Hoard.Item item in list)
		{
			string filter = item.Source.filter;
			if (hoard.mode != Hoard.Mode.all && !filter.IsEmpty() && filter != text)
			{
				continue;
			}
			bool flag = item.Source.tag.Contains("god");
			int num2 = item.show;
			if (item.random)
			{
				num2 = Rand.rnd(num2 + 1);
				if (num2 > 2 && num2 < 50)
				{
					num2 = Rand.rnd(num2 + 1) + 1;
				}
			}
			if (num2 == 0)
			{
				continue;
			}
			GameObject actor = hoard.GetActor(item.id);
			CollectibleActor component = actor.GetComponent<CollectibleActor>();
			SpriteRenderer component2 = actor.GetComponent<SpriteRenderer>();
			Vector3 extents = component2.bounds.extents;
			int num3 = (int)(extents.x * extents.y * 10f);
			if (num3 > 5)
			{
				num3 = 5;
			}
			float y = extents.y;
			float z = (flag ? (-0.1f) : 0f);
			bool hasSound = !item.Source.sound.IsEmpty();
			if (debug)
			{
				string id = item.id;
				Vector3 vector = extents;
				Debug.Log(id + "/" + vector.ToString());
			}
			float num4 = Mathf.Sqrt(actor.GetComponent<Rigidbody2D>().mass);
			if (num4 < 0.7f)
			{
				num4 = 0.7f;
			}
			BoxCollider2D component3 = actor.GetComponent<BoxCollider2D>();
			if ((bool)component3)
			{
				component3.size = extents * sizeMod;
			}
			else
			{
				actor.GetComponent<CircleCollider2D>().radius = (extents.x + extents.y) * sizeModCircle * sizeModCircle;
			}
			switch (item.id)
			{
			case "renton":
				renton = component;
				if (!angryEhe)
				{
					Util.Instantiate<SpriteRenderer>("UI/Layer/Hoard/Effect/sister", component);
					component.paired = true;
					MakeSteady(component);
				}
				break;
			case "ehe":
				ehe = component;
				break;
			case "sis":
				if (!hoard.voice)
				{
					hasSound = false;
				}
				break;
			case "chicken":
				actor.AddComponent<RigidUpdateSound>();
				break;
			case "mine":
			case "mine_dog":
			case "mine_rock":
				actor.AddComponent<RigidExplode>().particle = psExplode;
				break;
			case "mani":
			{
				if (EMono.rnd(2) == 0)
				{
					actor.AddComponent<RigidAngle>();
				}
				if (EMono.rnd(2) == 0)
				{
					component2.material = matMani;
					UnityEngine.Object.DestroyImmediate(actor.GetComponent<RigidExplode>());
					break;
				}
				RigidExplode rigidExplode2 = actor.AddComponent<RigidExplode>();
				rigidExplode2.particle = psExplode;
				rigidExplode2.radius *= 10f;
				rigidExplode2.force *= 10f;
				rigidExplode2.chance = 0.1f;
				rigidExplode2.intervalMin *= 2f;
				break;
			}
			case "pet_mani":
			{
				RigidExplode rigidExplode = actor.AddComponent<RigidExplode>();
				rigidExplode.particle = psExplode;
				rigidExplode.chance = 0.1f;
				rigidExplode.intervalMin *= 2f;
				break;
			}
			case "tentacle":
			case "tentacle2":
			case "tentacle3":
			case "tentacle4":
				z = -0.05f;
				break;
			case "hat_feather":
			case "hat_magic":
			case "bat":
				actor.AddComponent<RigidAngle>();
				break;
			}
			if (item.floating)
			{
				actor.AddComponent<RigidFloat>();
			}
			component.updates = actor.GetComponents<RigidUpdate>();
			for (int i = 0; i < num2; i++)
			{
				GameObject obj = ((i == 0) ? actor : UnityEngine.Object.Instantiate(actor));
				float num5 = radiusX;
				zero.x = posCenter.x + UnityEngine.Random.Range(0f - num5, num5);
				zero.y = posCenter.y + UnityEngine.Random.Range(0f, radiusY + 0.1f * (float)num) / num4;
				zero.z = z;
				obj.transform.position = zero;
				CollectibleActor component4 = obj.GetComponent<CollectibleActor>();
				Rigidbody2D component5 = obj.GetComponent<Rigidbody2D>();
				component4.item = item;
				component4.hasSound = hasSound;
				component4.rb = component5;
				actors.Add(component4);
				if (component4.updates.Length != 0)
				{
					RigidUpdate[] array = component4.updates;
					foreach (RigidUpdate rigidUpdate in array)
					{
						rigidUpdate.rb = component5;
						updates.Add(rigidUpdate);
					}
				}
				component4.shadow = num3;
				component4.shadowY = y;
			}
		}
		actors.Shuffle();
		IEnumerable<CollectibleActor> enumerable = actors.Where((CollectibleActor a) => a.item.Source.tag.Contains("god"));
		IEnumerable<CollectibleActor> enumerable2 = actors.Where((CollectibleActor a) => a.item.Source.tag.Contains("chara"));
		foreach (CollectibleActor actor2 in actors)
		{
			if (actor2.paired)
			{
				continue;
			}
			switch (actor2.item.id)
			{
			case "jure":
				if (FindPair(actor2, "odina", 0.3f))
				{
					MakeSteady(actor2);
					continue;
				}
				break;
			case "itz":
				if (EMono.rnd(2) == 0)
				{
					foreach (CollectibleActor item2 in enumerable2)
					{
						if (!item2.paired && !(item2 == actor2) && item2.item.Source.tag.Contains("fem"))
						{
							MakePair(actor2, item2, new Vector3(0.02f, 0f, -0.01f));
							item2.transform.localEulerAngles = new Vector3(0f, 0f, -90f);
							item2.GetComponent<SpriteRenderer>().flipX = true;
							MakeSteady(actor2);
							break;
						}
					}
					if (actor2.paired)
					{
						continue;
					}
				}
				actor2.GetComponentInChildren<ParticleSystem>().SetActive(enable: false);
				break;
			case "qulu":
				if (FindPair(actor2, "dragon", 0.3f))
				{
					MakeSteady(actor2);
					continue;
				}
				break;
			case "dragon":
				if (FindPair(actor2, "qulu", 0.3f))
				{
					MakeSteady(actor2);
					continue;
				}
				break;
			case "tentacle":
			case "tentacle2":
			case "tentacle3":
			case "tentacle4":
				foreach (CollectibleActor item3 in enumerable2)
				{
					if (!item3.paired && !(item3 == actor2))
					{
						MakePair(actor2, item3, new Vector3(0f, 0.25f, -0.01f));
						actor2.GetComponent<SpriteAnimation>().link = item3.transform;
						if (EMono.rnd(2) == 0)
						{
							item3.GetComponent<SpriteRenderer>().flipX = true;
						}
						if (EMono.rnd(2) == 0)
						{
							actor2.transform.SetLocalScaleX(actor2.transform.localScale.x * -1f);
						}
						break;
					}
				}
				continue;
			case "lar":
				if (FindPair(actor2, "lomi", 0.15f))
				{
					MakeSteady(actor2);
				}
				continue;
			case "lomi":
				if (FindPair(actor2, "lar", 0.15f))
				{
					MakeSteady(actor2);
				}
				continue;
			case "vesda":
				DisableParticle(actor2, 50);
				foreach (CollectibleActor item4 in enumerable2)
				{
					if (!item4.paired && !(item4 == actor2))
					{
						MakePair(actor2, item4, new Vector3(0f, -0.25f, -0.01f));
						item4.transform.localEulerAngles = new Vector3(0f, 0f, 90f);
						item4.GetComponent<SpriteRenderer>().flipX = false;
						break;
					}
				}
				continue;
			case "lulu":
				DisableParticle(actor2, 50);
				break;
			}
			if (EMono.rnd(2) == 0 || !actor2.item.Source.tag.Contains("pair"))
			{
				continue;
			}
			foreach (CollectibleActor item5 in enumerable)
			{
				if (!item5.paired && !(item5 == actor2))
				{
					MakePair(actor2, item5);
					break;
				}
			}
		}
		foreach (CollectibleActor actor3 in actors)
		{
			if (!actor3.paired && EMono.rnd(3) != 0)
			{
				actor3.GetComponent<SpriteRenderer>().flipX = true;
			}
		}
		static void DisableParticle(CollectibleActor a, int chance)
		{
			if (EMono.rnd(100) < chance)
			{
				ParticleSystem[] componentsInChildren = a.GetComponentsInChildren<ParticleSystem>();
				for (int k = 0; k < componentsInChildren.Length; k++)
				{
					componentsInChildren[k].SetActive(enable: false);
				}
			}
		}
		bool FindPair(CollectibleActor taker, string text2, float dist)
		{
			foreach (CollectibleActor actor4 in actors)
			{
				if (actor4.item.id == text2 && !actor4.paired)
				{
					MakePair(taker, actor4, dist);
					return true;
				}
			}
			return false;
		}
	}

	public void RefreshZoom()
	{
		if (hoard.pixelPerfect)
		{
			cam.orthographicSize = (float)Screen.height * 0.5f * 0.01f * 2f;
		}
		else
		{
			cam.orthographicSize = 5f;
		}
	}

	public void MakeSteady(CollectibleActor a)
	{
		(a.rb ?? a.GetComponent<Rigidbody2D>()).freezeRotation = true;
	}

	public void MakePair(CollectibleActor a, CollectibleActor b, float dist = 0.26f)
	{
		float z = 0.01f;
		if (a.item.id != "lulu")
		{
			z = ((Rand.rnd(2) == 0) ? 0.01f : (-0.01f));
		}
		MakePair(a, b, new Vector3(dist, 0f, z));
	}

	public void MakePair(CollectibleActor a, CollectibleActor b, Vector3 v)
	{
		b.transform.SetParent(a.transform, worldPositionStays: true);
		b.transform.localPosition = v;
		b.GetComponent<SpriteRenderer>().flipX = true;
		b.rb.isKinematic = true;
		b.GetComponent<Collider2D>().enabled = false;
		a.paired = (b.paired = true);
	}

	public void RefreshBG()
	{
		SpriteRenderer spriteRenderer = srBG;
		Sprite sprite = (srReflection.sprite = bg.sprite);
		spriteRenderer.sprite = sprite;
		matReflection.SetColor("_GrabColor", new Color(1f, 1f, 1f, bg.reflection * (0.01f * (float)hoard.reflection)));
		srReflection.SetActive(hoard.reflection > 0);
	}

	public void Clear()
	{
		foreach (CollectibleActor actor in actors)
		{
			if ((bool)actor && (bool)actor.gameObject)
			{
				UnityEngine.Object.DestroyImmediate(actor.gameObject);
			}
		}
		actors.Clear();
		updates.Clear();
	}

	private void Update()
	{
		RefreshZoom();
		matMani.SetFloat("_Space", curveMani.Evaluate(Time.timeSinceLevelLoad % 10f / 10f));
		float value = ((cam.orthographicSize <= 5f) ? 0f : ((cam.orthographicSize - 5f) * reflectMod));
		matReflection.SetFloat("_Offset", value);
		EMono.scene.transAudio.position = new Vector3(0f, groundY, 0f);
		if (!hoard.shadow)
		{
			return;
		}
		float shadow = bg.shadow;
		foreach (CollectibleActor actor in actors)
		{
			if (!actor.active)
			{
				continue;
			}
			float num = (actor.transform.position.y - alphaFix1) * alphaFix2;
			num = 1f - num * num * alphaFix4;
			if (!(num < 0f))
			{
				if (num > maxAplha)
				{
					num = maxAplha;
				}
				num *= shadow;
				float num2 = (actor.transform.position.y + groundY) * 0.05f;
				if (num2 > shdowLimitY)
				{
					num2 = shdowLimitY;
				}
				passShadow.Add(actor.transform.position.x, actor.shadowY * sizeFixY + groundY + num2, 0.8f, (int)(num * 1000f) * 1000 + actor.shadow);
			}
		}
		passShadow.Draw();
	}

	private void FixedUpdate()
	{
		float num = (RigidUpdate.delta = Time.fixedDeltaTime);
		if (angryEhe && (bool)renton && (bool)ehe && !ehe.rb.isKinematic && (Mathf.Abs(renton.transform.position.x - ehe.transform.position.x) > 0.6f || renton.transform.position.y > ehe.transform.position.y))
		{
			eheTimer += num;
			if (eheTimer > 2f)
			{
				ehe.rb.position = new Vector3(renton.transform.position.x, renton.transform.position.y + 4f, ehe.transform.position.z);
				ehe.PlaySound("teleport");
				eheTimer = 0f;
				eheCount++;
				if (eheCount > 5)
				{
					renton.rb.constraints = RigidbodyConstraints2D.FreezePositionX;
					renton.rb.gravityScale = 3f;
				}
			}
		}
		else
		{
			eheTimer = 0f;
		}
		foreach (RigidUpdate update in updates)
		{
			if (update.active)
			{
				update.OnFixedUpdate();
			}
		}
	}
}
