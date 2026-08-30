using System;
using UnityEngine;

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

	private SpriteRenderer spriteRenderer;

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

	public void Launch(Vector2 from, Vector2 to)
	{
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
			travelDuration = num / Mathf.Max(0.001f, speed);
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
		if (num >= 1f)
		{
			base.transform.position = new Vector3(targetPosition.x, targetPosition.y, launchZ);
			if (spriteRenderer != null)
			{
				spriteRenderer.enabled = false;
			}
			launched = false;
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

	protected virtual void OnArrive()
	{
	}
}
