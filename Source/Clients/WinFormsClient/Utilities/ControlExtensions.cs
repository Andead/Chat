using System;
using System.Windows.Forms;

namespace Andead.Chat.Client.WinForms.Utilities
{
    public static class ControlExtensions
    {
        /// <summary>
        ///     Executes a method on the UI thread.
        /// </summary>
        public static void InvokeIfRequired(this Control control, Action method)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(method);
                return;
            }

            method();
        }

        /// <summary>
        ///     Executes a method on the UI thread.
        /// </summary>
        public static void InvokeIfRequired<T>(this Control control, Action<T> method, T arg)
        {
            control.InvokeIfRequired(() => method(arg));
        }
    }
}