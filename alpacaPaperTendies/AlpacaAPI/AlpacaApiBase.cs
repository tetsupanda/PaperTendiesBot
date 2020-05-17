using alpacaPaperTendies.Configuration;

namespace alpacaPaperTendies.AlpacaAPI
{
    public class AlpacaApiBase
    {
        protected string ClientKey { get; set; }
        protected string ClientSecret { get; set; }


        public AlpacaApiBase()
        {
            this.ClientKey = AlpacaPaperTendiesConfiguration.AlpacaApiConfiguration?.API_KEY_ID;
            this.ClientSecret = AlpacaPaperTendiesConfiguration.AlpacaApiConfiguration?.SECRET;
        }
    }
}
