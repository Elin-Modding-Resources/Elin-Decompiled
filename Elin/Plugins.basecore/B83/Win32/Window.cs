using System;
using System.Runtime.InteropServices;
using System.Text;

namespace B83.Win32;

public static class Window
{
	[DllImport("user32.dll")]
	public static extern bool EnumThreadWindows(uint dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

	[DllImport("user32.dll")]
	public static extern bool IsWindowVisible(IntPtr hWnd);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

	public static string GetClassName(IntPtr hWnd)
	{
		StringBuilder stringBuilder = new StringBuilder(256);
		int className = GetClassName(hWnd, stringBuilder, 256);
		return stringBuilder.ToString(0, className);
	}

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern int GetWindowTextLength(IntPtr hWnd);

	[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
	private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

	public static string GetWindowText(IntPtr hWnd)
	{
		int num = GetWindowTextLength(hWnd) + 2;
		StringBuilder stringBuilder = new StringBuilder(num);
		int windowText = GetWindowText(hWnd, stringBuilder, num);
		return stringBuilder.ToString(0, windowText);
	}
}
