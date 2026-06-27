using CryptoViewer.Interfaces;
using CryptoViewer.Services;
using CryptoViewer.ViewModels;
using System.Windows.Controls;

namespace CryptoViewer.Views;

public partial class DetailsView : Page
{
    public DetailsView(string coinId)
    {
        InitializeComponent();

        ICoinGeckoService coinService = new CoinGeckoService();

        DataContext = new DetailsViewModel(coinService, coinId);
    }
}