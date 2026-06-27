using System.Windows;
using CryptoViewer.Views;

namespace CryptoViewer.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MainFrame.Navigate(new HomeView());
    }
}