using System.Windows.Controls;
using CryptoViewer.Services;
using CryptoViewer.ViewModels;

namespace CryptoViewer.Views;

public partial class HomeView : Page
{
    public HomeView()
    {
        InitializeComponent();

        var vm = new HomeViewModel(new CoinGeckoService());

        DataContext = vm;

        _ = vm.InitializeAsync();
    }
}