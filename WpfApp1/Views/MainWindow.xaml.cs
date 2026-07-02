using CryptoViewer.Services;
using CryptoViewer.Views;
using System.Windows;

namespace CryptoViewer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Navigate(new HomeView());
    }

    private void NavHome_Click(object sender, RoutedEventArgs e)
        => MainFrame.Navigate(new HomeView());

    private void NavConverter_Click(object sender, RoutedEventArgs e)
        => MainFrame.Navigate(new ConverterView());

    private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        => ThemeService.Toggle();

    private void ToggleLang_Click(object sender, RoutedEventArgs e)
        => LocalizationService.Toggle();
}