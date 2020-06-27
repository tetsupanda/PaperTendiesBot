using alpacaPaperTendies.Configuration;
using System;

namespace alpacaPaperTendies.AlpacaAPI
{    
    public interface IApiBase
    {   
        string ClientKey { get;}
        string ClientSecret { get;}
    }
    
    public class AlpacaApiBase : IApiBase
    {
        private string _clientKey;
        public string ClientKey { get { return _clientKey; } }
     
        private string _clientSecret;
        public string ClientSecret { get { return _clientSecret; }}

        public AlpacaApiBase()
        {   
            this._clientKey = AlpacaPaperTendiesConfiguration.AlpacaApiConfiguration?.API_KEY_ID;
            this._clientSecret = AlpacaPaperTendiesConfiguration.AlpacaApiConfiguration?.SECRET;
        }
    }
}
