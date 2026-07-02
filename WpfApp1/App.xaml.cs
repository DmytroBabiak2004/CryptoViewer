using CryptoViewer.Services;
using System.Windows;

namespace CryptoViewer;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        ThemeService.Apply();
        LocalizationService.Apply();
    }
}