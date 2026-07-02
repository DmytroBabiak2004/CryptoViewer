using CryptoViewer.Interfaces;
using CryptoViewer.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CryptoViewer.Services;

public class CoinGeckoService : ICoinGeckoService
{
    private static readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://api.coingecko.com/api/v3/")
    };

    static CoinGeckoService()
    {
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "CryptoViewer/1.0");
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // --- Cache ---
    private static (List<Coin> data, DateTime fetchedAt)? _coinsCache;
    private static readonly Dictionary<string, (CoinDetails data, DateTime fetchedAt)> _detailsCache = new();
    private static readonly Dictionary<string, (List<(double, double)> data, DateTime fetchedAt)> _chartCache = new();
    private static readonly Dictionary<string, (List<Market> data, DateTime fetchedAt)> _marketsCache = new();

    private static readonly TimeSpan _coinsTtl = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan _detailsTtl = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan _chartTtl = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan _marketsTtl = TimeSpan.FromMinutes(5);

    public async Task<List<Coin>> GetTopCoinsAsync(int limit = 10)
    {
        if (_coinsCache != null && DateTime.Now - _coinsCache.Value.fetchedAt < _coinsTtl)
            return _coinsCache.Value.data;

        var endpoint = $"coins/markets?vs_currency=usd&order=market_cap_desc&per_page={limit}&page=1&sparkline=false";
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var coins = JsonSerializer.Deserialize<List<Coin>>(json, _jsonOptions) ?? new();
            _coinsCache = (coins, DateTime.Now);
            return coins;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
            return _coinsCache?.data ?? new();
        }
    }

    public async Task<CoinDetails> GetCoinDetailsAsync(string id)
    {
        if (_detailsCache.TryGetValue(id, out var cached) && DateTime.Now - cached.fetchedAt < _detailsTtl)
            return cached.data;

        var endpoint = $"coins/{id}?localization=false&tickers=false&market_data=true&community_data=false&developer_data=false";
        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var marketData = doc.RootElement.GetProperty("market_data");

        var details = new CoinDetails
        {
            Id = id,
            Name = doc.RootElement.GetProperty("name").GetString() ?? "",
            Symbol = doc.RootElement.GetProperty("symbol").GetString() ?? "",
            CurrentPrice = marketData.GetProperty("current_price").GetProperty("usd").GetDecimal(),
            MarketCap = marketData.GetProperty("market_cap").GetProperty("usd").GetInt64(),
            TotalVolume = marketData.GetProperty("total_volume").GetProperty("usd").GetInt64(),
            High24h = marketData.GetProperty("high_24h").GetProperty("usd").GetDecimal(),
            Low24h = marketData.GetProperty("low_24h").GetProperty("usd").GetDecimal(),
            PriceChange24h = marketData.GetProperty("price_change_percentage_24h").GetDecimal()
        };

        _detailsCache[id] = (details, DateTime.Now);
        return details;
    }

    public async Task<List<(double time, double price)>> GetMarketChartAsync(string coinId)
    {
        if (_chartCache.TryGetValue(coinId, out var cached) && DateTime.Now - cached.fetchedAt < _chartTtl)
            return cached.data;

        var url = $"https://api.coingecko.com/api/v3/coins/{coinId}/market_chart?vs_currency=usd&days=7";
        var json = await _httpClient.GetStringAsync(url);
        using var doc = JsonDocument.Parse(json);

        var prices = doc.RootElement
            .GetProperty("prices")
            .EnumerateArray()
            .Select(x => (x[0].GetDouble(), x[1].GetDouble()))
            .ToList();

        _chartCache[coinId] = (prices, DateTime.Now);
        return prices;
    }

    public async Task<List<Market>> GetMarketsAsync(string coinId)
    {
        if (_marketsCache.TryGetValue(coinId, out var cached) && DateTime.Now - cached.fetchedAt < _marketsTtl)
            return cached.data;

        var url = $"coins/{coinId}/tickers";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var root = JsonSerializer.Deserialize<TickersRoot>(json, _jsonOptions);
        var markets = root?.Tickers.Take(8).ToList() ?? new();

        _marketsCache[coinId] = (markets, DateTime.Now);
        return markets;
    }

    private record TickersRoot(
        [property: JsonPropertyName("tickers")] List<Market> Tickers
    );
}