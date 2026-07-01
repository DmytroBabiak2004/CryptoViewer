using CryptoViewer.Interfaces;
using CryptoViewer.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CryptoViewer.ViewModels;

public class ConverterViewModel : INotifyPropertyChanged
{
    private readonly ICoinGeckoService _coinGecko;

    public ObservableCollection<Coin> Coins { get; } = new();

    private Coin? _fromCoin;
    public Coin? FromCoin
    {
        get => _fromCoin;
        set { _fromCoin = value; OnPropertyChanged(); Calculate(); }
    }

    private Coin? _toCoin;
    public Coin? ToCoin
    {
        get => _toCoin;
        set { _toCoin = value; OnPropertyChanged(); Calculate(); }
    }

    private string _amount = "1";
    public string Amount
    {
        get => _amount;
        set { _amount = value; OnPropertyChanged(); Calculate(); }
    }

    private string _result = "";
    public string Result
    {
        get => _result;
        private set { _result = value; OnPropertyChanged(); }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }

    public ICommand GoBackCommand { get; }

    public ConverterViewModel(ICoinGeckoService service, Action goBack)
    {
        _coinGecko = service;
        GoBackCommand = new RelayCommand(_ => goBack());
        _ = LoadCoinsAsync();
    }

    private async Task LoadCoinsAsync()
    {
        IsLoading = true;
        var coins = await _coinGecko.GetTopCoinsAsync(50);
        foreach (var c in coins)
            Coins.Add(c);
        FromCoin = Coins.FirstOrDefault();
        ToCoin = Coins.Skip(1).FirstOrDefault();
        IsLoading = false;
    }

    private void Calculate()
    {
        if (FromCoin == null || ToCoin == null)
        {
            Result = "";
            return;
        }

        if (!decimal.TryParse(Amount.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var amount))
        {
            Result = "Invalid amount";
            return;
        }

        if (ToCoin.CurrentPrice == 0)
        {
            Result = "N/A";
            return;
        }

        var converted = amount * FromCoin.CurrentPrice / ToCoin.CurrentPrice;
        Result = $"{amount} {FromCoin.Symbol.ToUpper()} = {converted:N6} {ToCoin.Symbol.ToUpper()}";
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}