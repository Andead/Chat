using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Andead.Chat.Client.WinForms.Utilities
{
    /// <remarks>
    ///     Taken from http://pietschsoft.com/post/2009/01/26/CSharp-Flash-Window-in-Taskbar-via-Win32-FlashWindowEx.
    /// </remarks>
    public static class FlashWindow
    {
        /// <summary>
        ///     A boolean value indicating whether the application is running on Windows 2000 or later.
        /// </summary>
        private static bool Win2000OrLater => Environment.OSVersion.Version.Major >= 5;

        /// <summary>
        ///     Flash the spacified Window (Form) until it recieves focus.
        /// </summary>
        /// <param name="form">The Form (Window) to Flash.</param>
        public static bool Flash(Form form)
        {
            // Make sure we're running under Windows 2000 or later
            if (Win2000OrLater)
            {
                NativeMethods.FLASHWINFO fi = Create_FLASHWINFO(form.Handle,
                    NativeMethods.FLASHW_ALL | NativeMethods.FLASHW_TIMERNOFG, uint.MaxValue, 0);
                return NativeMethods.FlashWindowEx(ref fi);
            }

            return false;
        }

        private static NativeMethods.FLASHWINFO Create_FLASHWINFO(IntPtr handle, uint flags, uint count, uint timeout)
        {
            var fi = new NativeMethods.FLASHWINFO();
            fi.cbSize = Convert.ToUInt32(Marshal.SizeOf(fi));
            fi.hwnd = handle;
            fi.dwFlags = flags;
            fi.uCount = count;
            fi.dwTimeout = timeout;
            return fi;
        }

        /// <summary>
        ///     Flash the specified Window (form) for the specified number of times
        /// </summary>
        /// <param name="form">The Form (Window) to Flash.</param>
        /// <param name="count">The number of times to Flash.</param>
        public static bool Flash(Form form, uint count)
        {
            if (Win2000OrLater)
            {
                NativeMethods.FLASHWINFO fi = Create_FLASHWINFO(form.Handle, NativeMethods.FLASHW_ALL, count, 0);
                return NativeMethods.FlashWindowEx(ref fi);
            }

            return false;
        }

        /// <summary>
        ///     Start Flashing the specified Window (form)
        /// </summary>
        /// <param name="form">The Form (Window) to Flash.</param>
        public static bool Start(Form form)
        {
            if (Win2000OrLater)
            {
                NativeMethods.FLASHWINFO fi = Create_FLASHWINFO(form.Handle, NativeMethods.FLASHW_ALL, uint.MaxValue, 0);
                return NativeMethods.FlashWindowEx(ref fi);
            }

            return false;
        }

        /// <summary>
        ///     Stop Flashing the specified Window (form)
        /// </summary>
        public static bool Stop(Form form)
        {
            if (Win2000OrLater)
            {
                NativeMethods.FLASHWINFO fi = Create_FLASHWINFO(form.Handle, NativeMethods.FLASHW_STOP, uint.MaxValue, 0);
                return NativeMethods.FlashWindowEx(ref fi);
            }

            return false;
        }
    }
}