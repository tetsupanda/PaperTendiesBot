using Alpaca.Markets;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace alpacaPaperTendies.AlpacaAPI
{
    public class TradeDataClient : AlpacaApiBase
    {
        public TradeDataClient()
        {

        }

        private AlpacaDataClient _client;
        public AlpacaDataClient DataClient()
        {
            _client = Environments.Paper.GetAlpacaDataClient(new SecretKey(ClientKey, ClientSecret));
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
