using CryptoViewer.Interfaces;
using CryptoViewer.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;

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

    public async Task<List<Coin>> GetTopCoinsAsync(int limit = 10)
    {
        var endpoint =
            $"coins/markets?vs_currency=usd&order=market_cap_desc&per_page={limit}&page=1&sparkline=false";

        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var coins = JsonSerializer.Deserialize<List<Coin>>(json, _jsonOptions);

            return coins ?? new List<Coin>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex.ToString());
            return new List<Coin>();
        }
    }

    public async Task<CoinDetails> GetCoinDetailsAsync(string id)
    {
        var endpoint = $"coins/{id}?localization=false&tickers=false&market_data=true&community_data=false&developer_data=false";

        var response = await _httpClient.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);

        var marketData = doc.RootElement.GetProperty("market_data");

        return new CoinDetails
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
    }
    public async Task<List<(double time, double price)>> GetMarketChartAsync(string coinId)
    {
        var url = $"https://api.coingecko.com/api/v3/coins/{coinId}/market_chart?vs_currency=usd&days=7";

        var json = await _httpClient.GetStringAsync(url);

        using var doc = JsonDocument.Parse(json);

        var prices = doc.RootElement
            .GetProperty("prices")
            .EnumerateArray()
            .Select(x =>
            {
                double time = x[0].GetDouble();
                double price = x[1].GetDouble();
                return (time, price);
            })
            .ToList();

        return prices;
    }
    public async Task<List<Market>> GetMarketsAsync(string coinId)
    {
        var url = $"coins/{coinId}/tickers";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        var root = JsonSerializer.Deserialize<TickersRoot>(json, _jsonOptions);
        return root?.Tickers.Take(8).ToList() ?? new();
    }

    private record TickersRoot(
        [property: JsonPropertyName("tickers")] List<Market> Tickers
    );
}