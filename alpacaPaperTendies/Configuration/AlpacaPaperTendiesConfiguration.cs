using Microsoft.Extensions.Configuration;

namespace alpacaPaperTendies.Configuration
{
    class AlpacaPaperTendiesConfiguration
    {
        private static IConfigurationRoot _configuration;
        private static IConfigurationRoot Configuration
        {
            get
            {
                if (_configuration == null)
                {
                    var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                    _configuration = builder.Build();
                }

                return _configuration;

            }
        }

        private static AlpacaApiConfiguration _alpacaApiConfiguration;
        public static AlpacaApiConfiguration AlpacaApiConfiguration
        {
            get
            {
                if (_alpacaApiConfiguration == null)
                {
                    _alpacaApiConfiguration = new AlpacaApiConfiguration();
                    Configuration.Bind("Aplaca", _alpacaApiConfiguration);
                }

                return _alpacaApiConfiguration;
            }
        }
    }
}
