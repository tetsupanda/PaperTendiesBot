using alpacaPaperTendies.Algos;
using System;
using System.Threading.Tasks;


namespace alpacaPaperTendies.AlpacaAPI
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                MeanReversionPaper algo = new MeanReversionPaper("SPY", 200);
                await algo.Run();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }

            Console.Read();
        }
    }
}
