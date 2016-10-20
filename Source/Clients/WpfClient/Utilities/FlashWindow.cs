using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Andead.Chat.Clients.Wpf.Utilities
{
    internal static class WindowFlasher
    {
        public static void StartFlashing(this Window win, UInt32 count = UInt32.MaxValue)
        {
            var h = new WindowInteropHelper(win);
            var info = new NativeMethods.FLASHWINFO
            {
                hwnd = h.Handle,
                dwFlags = NativeMethods.FLASHW_ALL | NativeMethods.FLASHW_TIMER,
                uCount = count,
                dwTimeout = 0
            };

            info.cbSize = Convert.ToUInt32(Marshal.SizeOf(info));
            NativeMethods.FlashWindowEx(ref info);
        }

        public static void StopFlashing(this Window win)
        {
            var h = new WindowInteropHelper(win);
            var info = new NativeMethods.FLASHWINFO {hwnd = h.Handle};
            info.cbSize = Convert.ToUInt32(Marshal.SizeOf(info));
            info.dwFlags = NativeMethods.FLASHW_STOP;
            info.uCount = UInt32.MaxValue;
            info.dwTimeout = 0;
            NativeMethods.FlashWindowEx(ref info);
        }
    }
}