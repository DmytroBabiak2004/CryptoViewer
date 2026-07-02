using System.Windows;

namespace CryptoViewer.Services;

public enum AppTheme { Dark, Light }

public static class ThemeService
{
    private static AppTheme _current = AppTheme.Dark;

    public static void Toggle()
    {
        _current = _current == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
        Apply();
    }

    public static void Apply()
    {
        var uri = _current == AppTheme.Dark
            ? "Resources/Themes/Dark.xaml"
            : "Resources/Themes/Light.xaml";

        var dict = new ResourceDictionary { Source = new Uri(uri, UriKind.Relative) };

        var existing = Application.Current.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source?.OriginalString.Contains("Themes") == true);

        if (existing != null)
            Application.Current.Resources.MergedDictionaries.Remove(existing);

        Application.Current.Resources.MergedDictionaries.Add(dict);
    }
}