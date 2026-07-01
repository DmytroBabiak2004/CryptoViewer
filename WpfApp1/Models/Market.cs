using System.Text.Json.Serialization;

namespace CryptoViewer.Models;

public class ConvertedLast
{
    [JsonPropertyName("usd")]
    public decimal Usd { get; set; }
}

public class Market
{
    [JsonPropertyName("market")]
    public MarketInfo? MarketInfo { get; set; }

    public string Name => MarketInfo?.Name ?? string.Empty;

    [JsonPropertyName("base")]
    public string Base { get; set; } = string.Empty;

    [JsonPropertyName("target")]
    public string Target { get; set; } = string.Empty;

    [JsonPropertyName("converted_last")]
    public ConvertedLast ConvertedLast { get; set; } = new();

    [JsonPropertyName("volume")]
    public decimal Volume { get; set; }

    [JsonPropertyName("trust_score")]
    public string TrustScore { get; set; } = string.Empty;

    [JsonPropertyName("trade_url")]
    public string TradeUrl { get; set; } = string.Empty;
}

public class MarketInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}