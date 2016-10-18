using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ServiceHost
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class NativeMethods
    {
        internal const int SW_HIDE = 0;

        internal const int SW_SHOW = 5;

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    }
}