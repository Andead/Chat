using System;
using System.Windows;
using MahApps.Metro;

namespace Andead.Chat.Clients.Wpf
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private readonly string[] _accents =
        {
            "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan",
            "Cobalt", "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel",
            "Mauve", "Taupe", "Sienna"
        };

        private readonly string[] _themes = {"BaseLight", "BaseDark"};

        protected override void OnStartup(StartupEventArgs e)
        {
            int accent = new Random().Next(_accents.Length);
            int theme = accent%2;

            // now set the Green accent and dark theme
            ThemeManager.ChangeAppStyle(Current, ThemeManager.GetAccent(_accents[accent]),
                ThemeManager.GetAppTheme(_themes[theme])); // or appStyle.Item1

            new Container().Run();
        }
    }
}