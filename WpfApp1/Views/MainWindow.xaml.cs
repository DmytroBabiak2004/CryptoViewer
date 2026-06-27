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
}