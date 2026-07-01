using CryptoViewer.Interfaces;
using CryptoViewer.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CryptoViewer.ViewModels;

public class RelayCommand(Action<object?> execute) : ICommand
{
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => true;
    public void Execute(object? parameter) => execute(parameter);
}

public class DetailsViewModel : INotifyPropertyChanged
{
    private readonly ICoinGeckoService _coinGecko;

    private CoinDetails? _coin;
    public CoinDetails? Coin
    {
        get => _coin;
        private set { _coin = value; OnPropertyChanged(); }
    }

    public ObservableCollection<Market> Markets { get; } = new();

    private PlotModel _plotModel = new();
    public PlotModel PlotModel
    {
        get => _plotModel;
        private set { _plotModel = value; OnPropertyChanged(); }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
    }

    public ICommand OpenTradeCommand { get; } = new RelayCommand(param =>
    {
        if (param is string url && !string.IsNullOrEmpty(url))
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    });

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

            var coinTask = _coinGecko.GetCoinDetailsAsync(id);
            var marketsTask = _coinGecko.GetMarketsAsync(id);
            var chartTask = _coinGecko.GetMarketChartAsync(id);

            await Task.WhenAll(coinTask, marketsTask, chartTask);

            Coin = coinTask.Result;

            foreach (var m in marketsTask.Result)
                Markets.Add(m);

            PlotModel = BuildPlotModel(chartTask.Result);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private static PlotModel BuildPlotModel(List<(double time, double price)> data)
    {
        var model = new PlotModel
        {
            Background = OxyColor.FromRgb(30, 30, 30),
            PlotAreaBorderColor = OxyColors.Transparent,
            TextColor = OxyColor.FromRgb(160, 160, 160),
        };

        model.Axes.Add(new DateTimeAxis
        {
            Position = AxisPosition.Bottom,
            StringFormat = "dd.MM",
            TextColor = OxyColor.FromRgb(120, 120, 120),
            TicklineColor = OxyColors.Transparent,
            MajorGridlineStyle = LineStyle.Solid,
            MajorGridlineColor = OxyColor.FromRgb(45, 45, 45),
            MinorGridlineStyle = LineStyle.None,
            AxislineStyle = LineStyle.None,
        });

        model.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            StringFormat = "$0",
            TextColor = OxyColor.FromRgb(120, 120, 120),
            TicklineColor = OxyColors.Transparent,
            MajorGridlineStyle = LineStyle.Solid,
            MajorGridlineColor = OxyColor.FromRgb(45, 45, 45),
            MinorGridlineStyle = LineStyle.None,
            AxislineStyle = LineStyle.None,
        });

        var line = new LineSeries
        {
            Color = OxyColor.FromRgb(34, 197, 94),
            StrokeThickness = 2,
            MarkerType = MarkerType.None,
            InterpolationAlgorithm = InterpolationAlgorithms.CatmullRomSpline,
        };

        foreach (var (time, price) in data)
        {
            var dt = DateTimeOffset.FromUnixTimeMilliseconds((long)time).UtcDateTime;
            line.Points.Add(DateTimeAxis.CreateDataPoint(dt, price));
        }

        var area = new AreaSeries
        {
            Color = OxyColor.FromRgb(34, 197, 94),
            Fill = OxyColor.FromArgb(40, 34, 197, 94),
            StrokeThickness = 0,
            MarkerType = MarkerType.None,
        };

        foreach (var (time, price) in data)
        {
            var dt = DateTimeOffset.FromUnixTimeMilliseconds((long)time).UtcDateTime;
            area.Points.Add(DateTimeAxis.CreateDataPoint(dt, price));
            area.Points2.Add(DateTimeAxis.CreateDataPoint(dt, 0));
        }

        model.Series.Add(area);
        model.Series.Add(line);

        return model;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}