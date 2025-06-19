using System;
using System.Windows;

namespace BookShelf.Services
{
    public enum ThemeType
    {
        Light,
        Dark,
        Crazy
    }

    public class ThemeManager
    {
        public void SetTheme(ThemeType themeType)
        {
            var themeDictionary = new ResourceDictionary();

            switch (themeType)
            {
                case ThemeType.Light:
                    themeDictionary.Source = new Uri("/Themes/LightTheme.xaml", UriKind.Relative);
                    break;
                case ThemeType.Dark:
                    themeDictionary.Source = new Uri("/Themes/DarkTheme.xaml", UriKind.Relative);
                    break;
                case ThemeType.Crazy:
                    themeDictionary.Source = new Uri("/Themes/CrazyTheme.xaml", UriKind.Relative);
                    break;
                default:
                    themeDictionary.Source = new Uri("/Themes/LightTheme.xaml", UriKind.Relative);
                    break;
            }

            // Uklanjamo staru temu i dodajemo novu
            // Ovo je pojednostavljen pristup. Pretpostavljamo da je tema uvijek prvi resurs.
            Application.Current.Resources.MergedDictionaries[0] = themeDictionary;
        }

        // Za kompatibilnost sa postojećim kodom
        public void SetTheme(bool isDark)
        {
            SetTheme(isDark ? ThemeType.Dark : ThemeType.Light);
        }
    }
}