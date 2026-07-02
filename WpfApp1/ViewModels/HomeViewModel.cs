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
    private CancellationTokenSource? _searchCts;

    public ObservableCollection<Coin> Coins { get; } = new();

    private string _searchText = string.Empty;
    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText == value)
                return;

            _searchText = value;
            OnPropertyChanged();

            FilterCoinsLocally();

            _ = SearchRemoteAsync(value);
        }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set
        {
            if (_isLoading == value)
                return;

            _isLoading = value;
            OnPropertyChanged();
        }
    }

    public HomeViewModel(ICoinGeckoService coinGecko)
    {
        _coinGecko = coinGecko;
    }

    public Task InitializeAsync() => LoadCoinsAsync();

    private async Task LoadCoinsAsync()
    {
        try
        {
            IsLoading = true;

            _allCoins = await _coinGecko.GetTopCoinsAsync();

            UpdateCoins(_allCoins);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void FilterCoinsLocally()
    {
        var query = _searchText.Trim();

        if (string.IsNullOrWhiteSpace(query))
        {
            UpdateCoins(_allCoins);
            return;
        }

        var filteredCoins = _allCoins.Where(c =>
            c.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            c.Symbol.Contains(query, StringComparison.OrdinalIgnoreCase));

        UpdateCoins(filteredCoins);
    }

    private async Task SearchRemoteAsync(string query)
    {
        _searchCts?.Cancel();
        _searchCts?.Dispose();

        _searchCts = new CancellationTokenSource();
        var token = _searchCts.Token;

        if (string.IsNullOrWhiteSpace(query))
            return;

        try
        {
            await Task.Delay(400, token);

            IsLoading = true;

            var results = await _coinGecko.SearchCoinsAsync(query, token);

            if (token.IsCancellationRequested)
                return;

            UpdateCoins(results);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            if (!token.IsCancellationRequested)
            {
                IsLoading = false;
            }
        }
    }

    private void UpdateCoins(IEnumerable<Coin> coins)
    {
        Coins.Clear();

        foreach (var coin in coins)
        {
            Coins.Add(coin);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}