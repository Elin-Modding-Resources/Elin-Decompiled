namespace B83.Win32;

public struct RECT
{
	public int Left;

	public int Top;

	public int Right;

	public int Bottom;

	public RECT(int left, int top, int right, int bottom)
	{
		Left = left;
		Top = top;
		Right = right;
		Bottom = bottom;
	}

	public override string ToString()
	{
		return "(" + Left + ", " + Top + ", " + Right + ", " + Bottom + ")";
	}
}
