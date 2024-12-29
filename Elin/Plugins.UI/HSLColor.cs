public struct HSLColor
{
	public float h;

	public float s;

	public float l;

	public float a;

	public HSLColor(float h, float s, float l, float a = 1f)
	{
		this.h = h;
		this.s = s;
		this.l = l;
		this.a = a;
	}

	public new string ToString()
	{
		return $"HSLA = ({h}, {s}, {l}, {a})";
	}
}
