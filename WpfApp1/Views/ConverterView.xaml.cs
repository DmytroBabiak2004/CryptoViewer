using CryptoViewer.Services;
using CryptoViewer.ViewModels;
using System.Windows.Controls;

namespace CryptoViewer.Views;

public partial class ConverterView : Page
{
    public ConverterView()
    {
        InitializeComponent();
        DataContext = new ConverterViewModel(new CoinGeckoService(), () =>
        {
            NavigationService?.GoBack();
        });
    }
}