using Alpaca.Markets;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace alpacaPaperTendies.AlpacaAPI
{
    public interface ITradeDataClient
    {
        AlpacaDataClient DataClient();
        Task<List<IAgg>> GetBarSet(string stockSymbol);
    }
    public class TradeDataClient : ITradeDataClient
    {
        private IApiBase _api;
        public TradeDataClient()
        {
            _api = new AlpacaApiBase();
        }
        
        private AlpacaDataClient _client;
        public AlpacaDataClient DataClient()
        {
            _client = Environments.Paper.GetAlpacaDataClient(new SecretKey(_api.ClientKey, _api.ClientSecret));
            return _client;
        }

        public async Task<List<IAgg>> GetBarSet(string stockSymbol)
        {
            var barSet = await this.DataClient().GetBarSetAsync(
                        new BarSetRequest(stockSymbol, TimeFrame.Minute) { Limit = 20 });
            var bars = barSet[stockSymbol].ToList();
            return bars;
        }
    }
}
