using System;

namespace B83.Win32;

public struct MSG
{
	public IntPtr hwnd;

	public WM message;

	public IntPtr wParam;

	public IntPtr lParam;

	public ushort time;

	public POINT pt;
}
