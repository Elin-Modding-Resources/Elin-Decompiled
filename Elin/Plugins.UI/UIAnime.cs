using UnityEngine;
using UnityEngine.Events;

public class UIAnime : MonoBehaviour
{
	public bool isDisableInput = true;

	public virtual void Play(AnimePhase phase, UnityAction onComplete = null)
	{
	}

	public virtual bool Has(AnimePhase phase)
	{
		return true;
	}

	public void OnComplete(UnityAction onComplete)
	{
		onComplete?.Invoke();
	}
}
