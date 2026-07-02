using CryptoViewer.Interfaces;
using CryptoViewer.Models;
using CryptoViewer.Services;
using CryptoViewer.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace CryptoViewer.Views;

public partial class HomeView : Page
{
    public HomeView(ICoinGeckoService service)
    {
        InitializeComponent();
        var vm = new HomeViewModel(service);
        DataContext = vm;
        _ = vm.InitializeAsync();
    }

    private void Coin_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListView lv && lv.SelectedItem is Coin coin)
            NavigationService.Navigate(new DetailsView(coin.Id));
    }
}