using CryptoViewer.Interfaces;
using CryptoViewer.Models;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CryptoViewer.ViewModels;

public class DetailsViewModel : INotifyPropertyChanged
{
    private readonly ICoinGeckoService _coinGecko;

    // --- Coin ---
    private CoinDetails? _coin;
    public CoinDetails? Coin
    {
        get => _coin;
        private set { _coin = value; OnPropertyChanged(); }
    }

    // --- Markets ---
    public ObservableCollection<Market> Markets { get; } = new();

    // --- Chart ---
    private PlotModel _plotModel = new();
    public PlotModel PlotModel
    {
        get => _plotModel;
        private set { _plotModel = value; OnPropertyChanged(); }
    }

    // --- Loading ---
    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set { _isLoading = value; OnPropertyChanged(); }
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

        // X — час
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

        // Y — ціна
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

        // Лінія ціни
        var line = new LineSeries
        {
            Color = OxyColor.FromRgb(34, 197, 94), // #22C55E
            StrokeThickness = 2,
            MarkerType = MarkerType.None,
            InterpolationAlgorithm = InterpolationAlgorithms.CatmullRomSpline,
        };

        foreach (var (time, price) in data)
        {
            var dt = DateTimeOffset
                .FromUnixTimeMilliseconds((long)time)
                .UtcDateTime;

            line.Points.Add(DateTimeAxis.CreateDataPoint(dt, price));
        }

        // Заливка під лінією
        var area = new AreaSeries
        {
            Color = OxyColor.FromRgb(34, 197, 94),
            Fill = OxyColor.FromArgb(40, 34, 197, 94),
            StrokeThickness = 0,
            MarkerType = MarkerType.None,
        };

        foreach (var (time, price) in data)
        {
            var dt = DateTimeOffset
                .FromUnixTimeMilliseconds((long)time)
                .UtcDateTime;

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