using System.Text.Json.Serialization;

namespace CryptoViewer.Models;

public class Coin
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("current_price")]
    public decimal CurrentPrice { get; set; }

    [JsonPropertyName("price_change_percentage_24h")]
    public decimal PriceChange24h { get; set; }

    [JsonPropertyName("market_cap")]
    public decimal MarketCap { get; set; }
}