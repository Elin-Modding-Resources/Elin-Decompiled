using System;

namespace B83.Win32;

public delegate IntPtr HookProc(int code, IntPtr wParam, ref MSG lParam);
