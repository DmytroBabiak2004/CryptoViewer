using System.Net.Http;
using System.Text.Json;
using CryptoViewer.Models;

namespace CryptoViewer.Services;

public class CoinGeckoService
{
    private readonly HttpClient _httpClient;

    public CoinGeckoService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.coingecko.com/api/v3/")
        };
    }

    public async Task<List<Coin>> GetTopCoinsAsync(int limit = 10)
    {
        var url =$"coins/markets?vs_currency=usd&order=market_cap_desc&per_page={limit}&page=1&sparkline=false";

        var response = await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<List<Coin>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result ?? new List<Coin>();
    }
}