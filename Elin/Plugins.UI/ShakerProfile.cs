using UnityEngine;

public class ShakerProfile : ScriptableObject
{
	public enum Type
	{
		Shake,
		Punch
	}

	public Type type;

	public float duration;

	public float power;

	public int vibrato;

	public float elasticity;

	public void ShakeCam()
	{
		Shaker.ShakeCam(base.name);
	}
}
