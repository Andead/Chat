using System;

namespace Andead.Chat.Client.WinForms
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            new Bootstrapper().Run();
        }
    }
}