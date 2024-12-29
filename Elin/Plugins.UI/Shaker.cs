using DG.Tweening;
using UnityEngine;

public class Shaker : MonoBehaviour
{
	public static void Shake(Transform t, string id, float magnitude)
	{
		DOTween.Kill(t);
		ShakerProfile shakerProfile = ResourceCache.Load<ShakerProfile>("Media/Effect/ScreenEffect/Shaker/" + id);
		int num = ((Random.Range(0, 2) == 0) ? 1 : (-1));
		int num2 = ((Random.Range(0, 2) == 0) ? 1 : (-1));
		_ = t.transform.localPosition;
		t.transform.localPosition = Vector3.zero;
		((shakerProfile.type == ShakerProfile.Type.Shake) ? t.DOShakePosition(shakerProfile.duration, new Vector2((float)num * shakerProfile.power, (float)num2 * shakerProfile.power) * magnitude, shakerProfile.vibrato) : t.DOPunchPosition(new Vector2((float)num * shakerProfile.power, (float)num2 * shakerProfile.power) * magnitude, shakerProfile.duration, shakerProfile.vibrato, shakerProfile.elasticity)).OnComplete(delegate
		{
			t.localPosition = Vector3.zero;
		});
	}

	public static void Shake(Component t, string id, float magnitude = 1f)
	{
		Shake(t.transform, id, magnitude);
	}

	public static void ShakeCam(string id = "default", float magnitude = 1f)
	{
		if ((bool)Camera.main)
		{
			Shake(Camera.main.transform.parent, id, magnitude);
		}
	}
}
