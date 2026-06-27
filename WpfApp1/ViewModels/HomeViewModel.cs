using CryptoViewer.Interfaces;
using CryptoViewer.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CryptoViewer.ViewModels;

public class HomeViewModel : INotifyPropertyChanged
{
    private readonly ICoinGeckoService _coinGecko;

    private List<Coin> _allCoins = new();

    public ObservableCollection<Coin> Coins { get; } = new();

    private string _searchText = "";
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
            FilterCoins();
        }
    }

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

    public HomeViewModel(ICoinGeckoService coinGecko)
    {
        _coinGecko = coinGecko;
    }

    public async Task InitializeAsync()
    {
        await LoadCoinsAsync();
    }

    private async Task LoadCoinsAsync()
    {
        try
        {
            IsLoading = true;

            var coins = await _coinGecko.GetTopCoinsAsync();

            _allCoins = coins;

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

    private void FilterCoins()
    {
        var query = _searchText?.Trim().ToLower();

        Coins.Clear();

        foreach (var coin in _allCoins)
        {
            if (string.IsNullOrWhiteSpace(query) ||
                coin.Name.ToLower().Contains(query) ||
                coin.Symbol.ToLower().Contains(query))
            {
                Coins.Add(coin);
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}