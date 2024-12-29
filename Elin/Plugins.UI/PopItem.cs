using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopItem : MonoBehaviour
{
	public Anime animeIn;

	public Anime animeOut;

	public Image bg;

	public Image arrow;

	public float mtpDuration = 1f;

	[NonSerialized]
	public Vector2 destPos;

	[NonSerialized]
	public Tween killTimer;

	[NonSerialized]
	public bool isDying;

	[NonSerialized]
	public bool important;
}
