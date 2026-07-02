using System.Globalization;
using System.Windows;

namespace CryptoViewer.Services;

public static class LocalizationService
{
    private static string _current = "en";
    public static string Current => _current;

    public static void Toggle()
    {
        _current = _current == "en" ? "uk" : "en";
        Apply();
    }

    public static void Apply()
    {
        var uri = $"Resources/Localization/Strings.{_current}.xaml";

        var dict = new ResourceDictionary
        {
            Source = new Uri(uri, UriKind.Relative)
        };

        var existing = Application.Current.Resources.MergedDictionaries
            .FirstOrDefault(d => d.Source?.OriginalString.Contains("Localization") == true);

        if (existing != null)
            Application.Current.Resources.MergedDictionaries.Remove(existing);

        Application.Current.Resources.MergedDictionaries.Add(dict);
    }
}