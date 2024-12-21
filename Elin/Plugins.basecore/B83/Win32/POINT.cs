namespace B83.Win32;

public struct POINT
{
	public int x;

	public int y;

	public POINT(int aX, int aY)
	{
		x = aX;
		y = aY;
	}

	public override string ToString()
	{
		return "(" + x + ", " + y + ")";
	}
}
