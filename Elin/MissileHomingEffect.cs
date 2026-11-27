using System;
using UnityEngine;
using UnityEngine.Rendering;

public class MissileHomingEffect : MonoBehaviour
{
	[Header("Orbit")]
	[SerializeField]
	private float orbitRadiusMin = 2f;

	[SerializeField]
	private float orbitRadiusMax = 4f;

	[SerializeField]
	private float orbitAngleMin = 220f;

	[SerializeField]
	private float orbitAngleMax = 340f;

	[Header("Timing")]
	[SerializeField]
	private float launchDelayMin;

	[SerializeField]
	private float launchDelayMax = 0.25f;

	[SerializeField]
	private bool useFixedDuration = true;

	[SerializeField]
	private float fixedDurationMin = 0.65f;

	[SerializeField]
	private float fixedDurationMax = 0.95f;

	[SerializeField]
	private float speed = 10f;

	[Header("Rotation")]
	[SerializeField]
	private bool rotateToDirection = true;

	[SerializeField]
	private float rotationOffset;

	[Header("Rainbow")]
	[SerializeField]
	private bool rainbow;

	[SerializeField]
	private float rainbowWidth = 0.45f;

	[SerializeField]
	private float rainbowTrailTime = 0.45f;

	[SerializeField]
	private int rainbowSegments = 32;

	[SerializeField]
	private bool rainbowTintMissile = true;

	[SerializeField]
	private float rainbowColorSpeed = 3f;

	[SerializeField]
	private Material rainbowMaterial;

	public GameObject goCat;

	private static readonly Color32[] RainbowColors = new Color32[6]
	{
		new Color32(byte.MaxValue, 0, 0, byte.MaxValue),
		new Color32(byte.MaxValue, 153, 0, byte.MaxValue),
		new Color32(byte.MaxValue, byte.MaxValue, 0, byte.MaxValue),
		new Color32(51, byte.MaxValue, 0, byte.MaxValue),
		new Color32(0, 153, byte.MaxValue, byte.MaxValue),
		new Color32(153, 51, byte.MaxValue, byte.MaxValue)
	};

	private SpriteRenderer spriteRenderer;

	private Mesh rainbowMesh;

	private MeshRenderer rainbowRenderer;

	private Material generatedRainbowMaterial;

	private Vector3[] rainbowVertices;

	private int rainbowSegmentCount;

	private Color originalSpriteColor;

	private bool missileTinted;

	private bool launched;

	private bool startedMoving;

	private float delayElapsed;

	private float launchDelay;

	private float elapsed;

	private float travelDuration;

	private float launchZ;

	private Vector2 startPosition;

	private Vector2 targetPosition;

	private float startRadius;

	private float peakRadius;

	private float startAngle;

	private float totalAngle;

	private float turnSign;

	public bool IsFlying => launched;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Launch(Vector2 from, Vector2 to, bool _rainbow)
	{
		rainbow = _rainbow;
		goCat.SetActive(rainbow);
		ClearRainbow();
		launchZ = base.transform.position.z;
		base.transform.position = new Vector3(from.x, from.y, launchZ);
		startPosition = from;
		targetPosition = to;
		delayElapsed = 0f;
		elapsed = 0f;
		startedMoving = false;
		launched = true;
		launchDelay = UnityEngine.Random.Range(launchDelayMin, launchDelayMax);
		if (spriteRenderer != null)
		{
			spriteRenderer.enabled = launchDelay <= 0f;
		}
		Vector2 vector = from - to;
		startRadius = vector.magnitude;
		if (startRadius < 0.001f)
		{
			startRadius = 0.001f;
			vector = Vector2.right * startRadius;
		}
		startAngle = Mathf.Atan2(vector.y, vector.x);
		float f = UnityEngine.Random.Range(orbitAngleMin, orbitAngleMax);
		totalAngle = Mathf.Max(180f, Mathf.Abs(f)) * (MathF.PI / 180f);
		turnSign = ((UnityEngine.Random.value < 0.5f) ? (-1f) : 1f);
		float a = UnityEngine.Random.Range(orbitRadiusMin, orbitRadiusMax);
		peakRadius = Mathf.Max(a, startRadius * 0.75f);
		if (useFixedDuration)
		{
			travelDuration = Mathf.Max(0.001f, UnityEngine.Random.Range(fixedDurationMin, fixedDurationMax));
		}
		else
		{
			float num = EstimatePathLength();
			travelDuration = Mathf.Max(0.001f, num / Mathf.Max(0.001f, speed));
		}
		if (launchDelay <= 0f)
		{
			StartMoving();
		}
	}

	private void Update()
	{
		if (!launched)
		{
			return;
		}
		if (!startedMoving)
		{
			delayElapsed += Time.deltaTime;
			if (delayElapsed >= launchDelay)
			{
				StartMoving();
			}
			return;
		}
		elapsed += Time.deltaTime;
		float num = Mathf.Clamp01(elapsed / travelDuration);
		Vector2 vector = EvaluatePosition(num);
		base.transform.position = new Vector3(vector.x, vector.y, launchZ);
		if (rotateToDirection)
		{
			UpdateRotation(num);
		}
		UpdateRainbow(num);
		if (num >= 1f)
		{
			base.transform.position = new Vector3(targetPosition.x, targetPosition.y, launchZ);
			if (spriteRenderer != null)
			{
				spriteRenderer.enabled = false;
			}
			launched = false;
			ClearRainbow();
			OnArrive();
		}
	}

	private void StartMoving()
	{
		startedMoving = true;
		elapsed = 0f;
		base.transform.position = new Vector3(startPosition.x, startPosition.y, launchZ);
		if (spriteRenderer != null)
		{
			spriteRenderer.enabled = true;
		}
		if (rotateToDirection)
		{
			UpdateRotation(0f);
		}
		UpdateRainbow(0f);
	}

	private Vector2 EvaluatePosition(float t)
	{
		float num = EvaluateRadius(t);
		float f = startAngle + totalAngle * turnSign * t;
		Vector2 vector = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
		return targetPosition + vector * num;
	}

	private Vector2 EvaluateDirection(float t)
	{
		float num = EvaluateRadius(t);
		float num2 = EvaluateRadiusDerivative(t);
		float f = startAngle + totalAngle * turnSign * t;
		float num3 = totalAngle * turnSign;
		Vector2 vector = new Vector2(Mathf.Cos(f), Mathf.Sin(f));
		Vector2 vector2 = new Vector2(0f - vector.y, vector.x);
		return vector * num2 + vector2 * num * num3;
	}

	private float EvaluateRadius(float t)
	{
		float num = 1f - t;
		return num * num * num * startRadius + 3f * num * num * t * peakRadius + 3f * num * t * t * peakRadius;
	}

	private float EvaluateRadiusDerivative(float t)
	{
		float num = 1f - t;
		return 3f * num * num * (peakRadius - startRadius) - 3f * t * t * peakRadius;
	}

	private void UpdateRotation(float t)
	{
		Vector2 vector = EvaluateDirection(t);
		if (!(vector.sqrMagnitude < 1E-06f))
		{
			float num = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
			base.transform.rotation = Quaternion.Euler(0f, 0f, num + rotationOffset);
		}
	}

	private float EstimatePathLength()
	{
		float num = 0f;
		Vector2 a = EvaluatePosition(0f);
		for (int i = 1; i <= 32; i++)
		{
			Vector2 vector = EvaluatePosition((float)i / 32f);
			num += Vector2.Distance(a, vector);
			a = vector;
		}
		return num;
	}

	private bool InitializeRainbow()
	{
		if (rainbowMesh != null)
		{
			return true;
		}
		Material material = rainbowMaterial;
		if (material == null)
		{
			Shader shader = Shader.Find("Sprites/Default");
			if (shader == null)
			{
				return false;
			}
			generatedRainbowMaterial = new Material(shader);
			material = generatedRainbowMaterial;
		}
		GameObject gameObject = new GameObject("RainbowTrail");
		gameObject.transform.SetParent(base.transform, worldPositionStays: false);
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		rainbowRenderer = gameObject.AddComponent<MeshRenderer>();
		rainbowRenderer.sharedMaterial = material;
		rainbowRenderer.enabled = false;
		rainbowRenderer.shadowCastingMode = ShadowCastingMode.Off;
		rainbowRenderer.receiveShadows = false;
		if (spriteRenderer != null)
		{
			rainbowRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
			rainbowRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
		}
		rainbowMesh = new Mesh();
		rainbowMesh.name = "RainbowTrailMesh";
		rainbowMesh.MarkDynamic();
		meshFilter.sharedMesh = rainbowMesh;
		rainbowSegmentCount = Mathf.Clamp(rainbowSegments, 2, 128);
		int num = RainbowColors.Length;
		int num2 = rainbowSegmentCount + 1;
		int num3 = num2 * num * 2;
		rainbowVertices = new Vector3[num3];
		Color32[] array = new Color32[num3];
		Vector2[] array2 = new Vector2[num3];
		int[] array3 = new int[rainbowSegmentCount * num * 6];
		for (int i = 0; i < num2; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num4 = (i * num + j) * 2;
				array[num4] = RainbowColors[j];
				array[num4 + 1] = RainbowColors[j];
				array2[num4] = new Vector2(0f, 0f);
				array2[num4 + 1] = new Vector2(1f, 1f);
			}
		}
		int num5 = 0;
		for (int k = 0; k < rainbowSegmentCount; k++)
		{
			for (int l = 0; l < num; l++)
			{
				int num6 = (k * num + l) * 2;
				int num7 = num6 + 1;
				int num8 = num6 + num * 2;
				int num9 = num8 + 1;
				array3[num5++] = num6;
				array3[num5++] = num8;
				array3[num5++] = num7;
				array3[num5++] = num8;
				array3[num5++] = num9;
				array3[num5++] = num7;
			}
		}
		rainbowMesh.vertices = rainbowVertices;
		rainbowMesh.colors32 = array;
		rainbowMesh.uv = array2;
		rainbowMesh.triangles = array3;
		return true;
	}

	private void UpdateRainbow(float t)
	{
		if (!rainbow)
		{
			ClearRainbow();
			return;
		}
		if (rainbowTintMissile && spriteRenderer != null)
		{
			if (!missileTinted)
			{
				originalSpriteColor = spriteRenderer.color;
				missileTinted = true;
			}
			Color color = Color.HSVToRGB(Mathf.Repeat(elapsed * rainbowColorSpeed, 1f), 1f, 1f);
			color.a = originalSpriteColor.a;
			spriteRenderer.color = color;
		}
		else
		{
			RestoreMissileColor();
		}
		if (!InitializeRainbow())
		{
			return;
		}
		float num = Mathf.Max(0f, rainbowTrailTime) / travelDuration;
		float num2 = Mathf.Max(0f, t - num);
		if (t - num2 < 1E-05f || rainbowWidth <= 0f)
		{
			rainbowRenderer.enabled = false;
			return;
		}
		int num3 = RainbowColors.Length;
		float num4 = rainbowWidth;
		Matrix4x4 worldToLocalMatrix = base.transform.worldToLocalMatrix;
		for (int i = 0; i <= rainbowSegmentCount; i++)
		{
			float t2 = (float)i / (float)rainbowSegmentCount;
			float t3 = Mathf.Lerp(num2, t, t2);
			Vector2 vector = EvaluatePosition(t3);
			Vector2 vector2 = EvaluateDirection(t3);
			if (vector2.sqrMagnitude < 1E-06f)
			{
				vector2 = Vector2.right;
			}
			vector2.Normalize();
			Vector2 vector3 = new Vector2(0f - vector2.y, vector2.x);
			for (int j = 0; j < num3; j++)
			{
				float num5 = num4 * (0.5f - (float)j / (float)num3);
				float num6 = num4 * (0.5f - (float)(j + 1) / (float)num3);
				Vector2 vector4 = vector + vector3 * num5;
				Vector2 vector5 = vector + vector3 * num6;
				int num7 = (i * num3 + j) * 2;
				rainbowVertices[num7] = worldToLocalMatrix.MultiplyPoint3x4(new Vector3(vector4.x, vector4.y, launchZ));
				rainbowVertices[num7 + 1] = worldToLocalMatrix.MultiplyPoint3x4(new Vector3(vector5.x, vector5.y, launchZ));
			}
		}
		rainbowMesh.vertices = rainbowVertices;
		rainbowMesh.RecalculateBounds();
		rainbowRenderer.enabled = true;
	}

	private void RestoreMissileColor()
	{
		if (missileTinted && spriteRenderer != null)
		{
			spriteRenderer.color = originalSpriteColor;
		}
		missileTinted = false;
	}

	private void ClearRainbow()
	{
		if (rainbowRenderer != null)
		{
			rainbowRenderer.enabled = false;
		}
		RestoreMissileColor();
	}

	private void OnDisable()
	{
		ClearRainbow();
	}

	private void OnDestroy()
	{
		if (rainbowMesh != null)
		{
			UnityEngine.Object.Destroy(rainbowMesh);
		}
		if (generatedRainbowMaterial != null)
		{
			UnityEngine.Object.Destroy(generatedRainbowMaterial);
		}
	}

	protected virtual void OnArrive()
	{
	}
}
