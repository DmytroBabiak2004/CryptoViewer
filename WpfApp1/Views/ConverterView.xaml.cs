using CryptoViewer.Interfaces;
using CryptoViewer.ViewModels;
using System.Windows.Controls;

namespace CryptoViewer.Views;

public partial class ConverterView : Page
{
    public ConverterView(ICoinGeckoService service)
    {
        InitializeComponent();
        DataContext = new ConverterViewModel(service, () => NavigationService?.GoBack());
    }
}