using System.Text.Json.Serialization;

namespace CryptoViewer.Models;

public class SearchCoin
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("market_cap_rank")]
    public int? MarketCapRank { get; set; }
}