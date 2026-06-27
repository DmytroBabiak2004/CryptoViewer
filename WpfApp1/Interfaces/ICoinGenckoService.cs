using CryptoViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoViewer.Interfaces
{
    public interface ICoinGeckoService
    {
        Task<List<Coin>> GetTopCoinsAsync(int limit = 10);
        Task<List<Market>> GetMarketsAsync(string coinId);
        Task<CoinDetails> GetCoinDetailsAsync(string id);
        Task<List<(double time, double price)>> GetMarketChartAsync(string id);
    }
}
