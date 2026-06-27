using CryptoViewer.Interfaces;
using CryptoViewer.Models;
using System.Net.Http;
using System.Text.Json;

namespace CryptoViewer.Services;

public class CoinGeckoService : ICoinGeckoService
{
    private static readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://api.coingecko.com/api/v3/")
    };

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
        catch
        {
            return new List<Coin>();
        }
    }
}