using DG.Tweening;
using UnityEngine.Events;

public class UIAnimeSequence : UIAnime
{
	private Sequence sequenceIntro;

	private Sequence sequenceOutro;

	public override void Play(AnimePhase phase, UnityAction onComplete = null)
	{
		Sequence sequence = ((phase == AnimePhase.Intro) ? sequenceIntro : sequenceOutro);
		if (sequence == null)
		{
			onComplete();
			return;
		}
		sequence.Play().OnComplete(delegate
		{
			onComplete();
		});
	}

	public override bool Has(AnimePhase phase)
	{
		if (phase != 0 || sequenceIntro == null)
		{
			if (phase == AnimePhase.Outro)
			{
				return sequenceOutro != null;
			}
			return false;
		}
		return true;
	}

	public UIAnimeSequence SetIntro(Sequence sequence)
	{
		sequenceIntro = sequence;
		sequence.Pause();
		return this;
	}

	public UIAnimeSequence SetOutro(Sequence sequence)
	{
		sequenceOutro = sequence;
		sequence.Pause();
		return this;
	}
}
