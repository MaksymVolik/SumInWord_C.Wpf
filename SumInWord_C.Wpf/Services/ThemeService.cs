using SumInWord_C.Wpf.Interfaces;
using System.Windows;

namespace SumInWord_C.Wpf.Services
{
    public enum Theme
    {
        Dark,
        Light,
        DarkOrange
    }

    // Убедитесь, что класс реализует интерфейс IThemeService
    public class ThemeService : IThemeService
    {
        private const string ThemeSettingKey = "SelectedTheme";

        public void ApplyTheme(Theme theme)
        {
            var themeSource = theme switch
            {
                Theme.Dark => "Styles/Themes/DarkTheme.xaml",
                Theme.Light => "Styles/Themes/LightTheme.xaml",
                Theme.DarkOrange => "Styles/Themes/DarkOrangeTheme.xaml",
                _ => "Styles/Themes/DarkTheme.xaml"
            };

            var resourceDict = new ResourceDictionary
            {
                Source = new Uri(themeSource, UriKind.Relative)
            };

            // Видаляємо стару тему
            var oldTheme = Application.Current.Resources.MergedDictionaries
                .FirstOrDefault(d => d.Source?.OriginalString?.Contains("Themes/") == true);
            
            if (oldTheme != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(oldTheme);
            }

            // Додаємо нову тему
            Application.Current.Resources.MergedDictionaries.Insert(0, resourceDict);

            // Зберігаємо вибір теми
            SaveThemePreference(theme);
        }

        public Theme GetCurrentTheme()
        {
            var savedTheme = Properties.Settings.Default[ThemeSettingKey]?.ToString();
            return Enum.TryParse<Theme>(savedTheme, out var theme) ? theme : Theme.Dark;
        }

        private static void SaveThemePreference(Theme theme)
        {
            Properties.Settings.Default[ThemeSettingKey] = theme.ToString();
            Properties.Settings.Default.Save();
        }

        public void LoadSavedTheme()
        {
            var theme = GetCurrentTheme();
            ApplyTheme(theme);
        }
    }
}