using System.Text.Json.Serialization;

namespace CryptoViewer.Models;

public class CoinDetails
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("market_cap_rank")]
    public int MarketCapRank { get; set; }

    [JsonPropertyName("current_price")]
    public decimal CurrentPrice { get; set; }

    [JsonPropertyName("market_cap")]
    public long MarketCap { get; set; }

    [JsonPropertyName("total_volume")]
    public long TotalVolume { get; set; }

    [JsonPropertyName("high_24h")]
    public decimal High24h { get; set; }

    [JsonPropertyName("low_24h")]
    public decimal Low24h { get; set; }

    [JsonPropertyName("price_change_percentage_24h")]
    public decimal PriceChange24h { get; set; }

    public bool IsPriceChangePositive => PriceChange24h >= 0;
}