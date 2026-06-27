using System.Windows.Controls;
using CryptoViewer.Services;
using CryptoViewer.ViewModels;

namespace CryptoViewer.Views;

public partial class DetailsView : Page
{
    public DetailsView(string coinId)
    {
        InitializeComponent();

        DataContext = new DetailsViewModel(
            new CoinGeckoService(),
            coinId
        );
    }
}