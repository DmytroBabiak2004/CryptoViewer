using System.ComponentModel;
using System.Runtime.CompilerServices;
using CryptoViewer.Interfaces;
using CryptoViewer.Models;

namespace CryptoViewer.ViewModels;

public class DetailsViewModel : INotifyPropertyChanged
{
    private readonly ICoinGeckoService _coinGecko;

    public CoinDetails? Coin { get; private set; }

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

    public DetailsViewModel(ICoinGeckoService service, string coinId)
    {
        _coinGecko = service;
        _ = LoadAsync(coinId);
    }

    private async Task LoadAsync(string id)
    {
        try
        {
            IsLoading = true;
            Coin = await _coinGecko.GetCoinDetailsAsync(id);
            OnPropertyChanged(nameof(Coin));
        }
        finally
        {
            IsLoading = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}