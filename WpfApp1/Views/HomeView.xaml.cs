using System.Windows;
using CryptoViewer.Services;
using CryptoViewer.ViewModels;

namespace CryptoViewer.Views;

public partial class HomeView : Window
{
    public HomeView()
    {
        InitializeComponent();

        DataContext = new HomeViewModel(
            new CoinGeckoService()
        );
    }
}