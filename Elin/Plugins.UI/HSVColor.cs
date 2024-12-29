public struct HSVColor
{
	public float h;

	public float s;

	public float v;

	public float a;

	public HSVColor(float h, float s, float v, float a = 1f)
	{
		this.h = h;
		this.s = s;
		this.v = v;
		this.a = a;
	}

	public new string ToString()
	{
		return $"HSVA = ({h}, {s}, {v}, {a})";
	}
}
