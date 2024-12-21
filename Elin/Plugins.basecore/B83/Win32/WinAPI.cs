using System;
using System.Runtime.InteropServices;
using System.Text;

namespace B83.Win32;

public static class WinAPI
{
	[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
	public static extern IntPtr GetModuleHandle(string lpModuleName);

	[DllImport("kernel32.dll")]
	public static extern uint GetCurrentThreadId();

	[DllImport("user32.dll", SetLastError = true)]
	public static extern IntPtr SetWindowsHookEx(HookType hookType, HookProc lpfn, IntPtr hMod, uint dwThreadId);

	[DllImport("user32.dll", SetLastError = true)]
	public static extern bool UnhookWindowsHookEx(IntPtr hhk);

	[DllImport("user32.dll")]
	public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref MSG lParam);

	[DllImport("shell32.dll")]
	public static extern void DragAcceptFiles(IntPtr hwnd, bool fAccept);

	[DllImport("shell32.dll", CharSet = CharSet.Unicode)]
	public static extern uint DragQueryFile(IntPtr hDrop, uint iFile, StringBuilder lpszFile, uint cch);

	[DllImport("shell32.dll")]
	public static extern void DragFinish(IntPtr hDrop);

	[DllImport("shell32.dll")]
	public static extern void DragQueryPoint(IntPtr hDrop, out POINT pos);
}
