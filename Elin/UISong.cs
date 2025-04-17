using UnityEngine;
using UnityEngine.UI;

public class UISong : EMono
{
	public static UISong Instance;

	public Gauge gauge;

	public Image bar;

	public SoundSource source;

	public BGMData.Part part;

	public BGMData bgm;

	public float ratio;

	public Vector3 posFix;

	private float timer;

	private void Awake()
	{
		Instance = this;
	}

	public void SetSong(SoundSource _source, BGMData _bgm, BGMData.Part _part)
	{
		source = _source;
		part = _part;
		bgm = _bgm;
		Vector3 position = Camera.main.WorldToScreenPoint(EMono.pc.pos.Position());
		position += posFix * EMono.screen.Zoom;
		position.z = 0f;
		this.Rect().position = position;
	}

	private void Update()
	{
		if (source.source == null)
		{
			Kill();
			return;
		}
		AudioSource audioSource = source.source;
		ratio = Mathf.Clamp((audioSource.time - part.start) / (part.duration + bgm.song.fadeIn), 0f, 1f);
		if ((EMono.pc.ai is GoalManualMove || EMono.pc.IsMoving) && ratio < 0.1f)
		{
			Kill();
		}
		else if (EMono.game == null || !EMono.core.IsGameStarted || source == null || !source.isPlaying || ratio >= 0.99f || (ratio <= 0f && !(EMono.pc.ai is AI_PlayMusic)))
		{
			timer += Core.delta;
			if (timer > 0.3f)
			{
				Kill();
			}
		}
		else
		{
			timer = 0f;
			RectTransform rectTransform = bar.rectTransform;
			rectTransform.sizeDelta = new Vector2(gauge.originalWidth * ratio, rectTransform.sizeDelta.y);
		}
	}

	public void Kill()
	{
		Object.DestroyImmediate(base.gameObject);
	}
}
