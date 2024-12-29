using UnityEngine;

public class Typewriter : MonoBehaviour
{
	private UIText text;

	private float timer;

	private bool isStarted;

	private string current;

	private string original;

	private int index;

	public float speed;

	public SoundData sound;

	public bool IsFinished
	{
		get
		{
			if (isStarted)
			{
				return index >= original.Length;
			}
			return false;
		}
	}

	private void Awake()
	{
		text = GetComponent<UIText>();
	}

	public void OnSetText()
	{
		original = text.text;
		current = "";
		text.text = "";
		index = 0;
		timer = 0f;
		isStarted = true;
	}

	public void Skip()
	{
		if (isStarted)
		{
			text.text = original;
			index = original.Length;
		}
	}

	private void Update()
	{
		if (!isStarted)
		{
			return;
		}
		timer -= Time.unscaledDeltaTime;
		if (!(timer > 0f) && index < original.Length)
		{
			string text = original.Substring(index, 1);
			current += text;
			index++;
			this.text.text = current;
			float num = speed;
			if (text == "_comma".lang())
			{
				num *= 3f;
			}
			if (text == "_period".lang())
			{
				num *= 6f;
			}
			timer += num;
			if ((bool)sound)
			{
				sound.Play();
			}
		}
	}
}
