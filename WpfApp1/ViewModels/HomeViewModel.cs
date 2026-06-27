using CryptoViewer.Interfaces;
using CryptoViewer.Models;
using CryptoViewer.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CryptoViewer.ViewModels;

public class HomeViewModel : INotifyPropertyChanged
{
    private readonly ICoinGeckoService _coinGecko;

    public ObservableCollection<Coin> Coins { get; } = new();

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            _isLoading = value;
            OnPropertyChanged();
        }
    }
    public async Task InitializeAsync()
    {
        await LoadCoinsAsync();
    }

    public HomeViewModel(ICoinGeckoService coinGecko)
    {
        _coinGecko = coinGecko;

        _ = LoadCoinsAsync();
    }

    private async Task LoadCoinsAsync()
    {
        try
        {
            IsLoading = true;

            var coins = await _coinGecko.GetTopCoinsAsync();

            Coins.Clear();

            foreach (var coin in coins)
            {
                Coins.Add(coin);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}