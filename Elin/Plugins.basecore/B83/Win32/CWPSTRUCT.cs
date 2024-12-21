using System;

namespace B83.Win32;

public struct CWPSTRUCT
{
	public IntPtr lParam;

	public IntPtr wParam;

	public WM message;

	public IntPtr hwnd;
}
