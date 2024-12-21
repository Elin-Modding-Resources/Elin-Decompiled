using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteAnimation : MonoBehaviour
{
	[Serializable]
	public class Item
	{
		public Sprite sprite;

		public Vector3 pos;

		public float angle;

		public float angleRange;
	}

	public Item[] items;

	public float minInterval;

	public float maxInterval;

	public bool randomAngle;

	public Transform link;

	private SpriteRenderer sr;

	private int index;

	private float _baseAngle;

	private float interval;

	private void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
		sr.sprite = items[0].sprite;
		interval = UnityEngine.Random.Range(minInterval, maxInterval);
		if (randomAngle)
		{
			_baseAngle = UnityEngine.Random.Range(0, 360);
		}
		InvokeRepeating("Refresh", 0f, interval);
	}

	private void Refresh()
	{
		index++;
		if (index >= items.Length)
		{
			index = 0;
		}
		Item item = items[index];
		sr.sprite = item.sprite;
		if ((bool)link)
		{
			link.localPosition = item.pos;
			link.localEulerAngles = new Vector3(0f, 0f, _baseAngle + item.angle + UnityEngine.Random.Range(0f - item.angleRange, item.angleRange));
		}
	}
}
