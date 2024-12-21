using System;
using System.Collections.Generic;
using System.Text;
using AOT;

namespace B83.Win32;

public static class UnityDragAndDropHook
{
	public delegate void DroppedFilesEvent(List<string> aPathNames, POINT aDropPoint);

	private static uint threadId;

	private static IntPtr mainWindow = IntPtr.Zero;

	private static IntPtr m_Hook;

	private static string m_ClassName = "UnityWndClass";

	public static event DroppedFilesEvent OnDroppedFiles;

	[MonoPInvokeCallback(typeof(EnumThreadDelegate))]
	private static bool EnumCallback(IntPtr W, IntPtr _)
	{
		if (Window.IsWindowVisible(W) && (mainWindow == IntPtr.Zero || (m_ClassName != null && Window.GetClassName(W) == m_ClassName)))
		{
			mainWindow = W;
		}
		return true;
	}

	public static void InstallHook()
	{
		threadId = WinAPI.GetCurrentThreadId();
		if (threadId != 0)
		{
			Window.EnumThreadWindows(threadId, EnumCallback, IntPtr.Zero);
		}
		IntPtr moduleHandle = WinAPI.GetModuleHandle(null);
		m_Hook = WinAPI.SetWindowsHookEx(HookType.WH_GETMESSAGE, Callback, moduleHandle, threadId);
		WinAPI.DragAcceptFiles(mainWindow, fAccept: true);
	}

	public static void UninstallHook()
	{
		WinAPI.UnhookWindowsHookEx(m_Hook);
		WinAPI.DragAcceptFiles(mainWindow, fAccept: false);
		m_Hook = IntPtr.Zero;
	}

	[MonoPInvokeCallback(typeof(HookProc))]
	private static IntPtr Callback(int code, IntPtr wParam, ref MSG lParam)
	{
		if (code == 0 && lParam.message == WM.DROPFILES)
		{
			WinAPI.DragQueryPoint(lParam.wParam, out var pos);
			uint num = WinAPI.DragQueryFile(lParam.wParam, uint.MaxValue, null, 0u);
			StringBuilder stringBuilder = new StringBuilder(1024);
			List<string> list = new List<string>();
			for (uint num2 = 0u; num2 < num; num2++)
			{
				int num3 = (int)WinAPI.DragQueryFile(lParam.wParam, num2, stringBuilder, 1024u);
				list.Add(stringBuilder.ToString(0, num3));
				stringBuilder.Length = 0;
			}
			WinAPI.DragFinish(lParam.wParam);
			if (UnityDragAndDropHook.OnDroppedFiles != null)
			{
				UnityDragAndDropHook.OnDroppedFiles(list, pos);
			}
		}
		return WinAPI.CallNextHookEx(m_Hook, code, wParam, ref lParam);
	}
}
