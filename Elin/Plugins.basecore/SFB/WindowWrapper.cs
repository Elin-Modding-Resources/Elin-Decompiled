using System;
using System.Windows.Forms;

namespace SFB;

public class WindowWrapper : IWin32Window
{
	private IntPtr _hwnd;

	public IntPtr Handle => _hwnd;

	public WindowWrapper(IntPtr handle)
	{
		_hwnd = handle;
	}
}
