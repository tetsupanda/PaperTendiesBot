using Alpaca.Markets;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using alpacaPaperTendies.AlpacaAPI;

namespace alpacaPaperTendies.Algos
{
    internal sealed class MeanReversionPaper : IDisposable
    {

            ITradeClient _tradingClient;
            ITradeDataClient _dataClient;

            // TODO: Move this GUID into trade client, maybe?
            private Guid lastTradeId = Guid.NewGuid();

            private string _stockSymbol;
            private Decimal _scale;
            public MeanReversionPaper(string symbol, Decimal scale)
            {
                _stockSymbol = symbol;
                _scale = scale;
                _dataClient = new TradeDataClient();
                _tradingClient = new TradeClient();
            }

            public async Task Run()
            {
                await _tradingClient.CancelAllOpenOrders();

                var closingTime = await _tradingClient.GetMarketClose();


                Console.WriteLine("Waiting for market open...");
                await _tradingClient.AwaitMarketOpen();
                Console.WriteLine("Market opened.");

                // Check every minute for price updates.
                TimeSpan timeUntilClose = closingTime - DateTime.UtcNow;
                while (timeUntilClose.TotalMinutes > 15)
                {
                    // Cancel old order if it's not already been filled.
                    await _tradingClient.Client().DeleteOrderAsync(lastTradeId);

                    // Get information about current account value.
                    var account = await _tradingClient.Client().GetAccountAsync();
                    Decimal buyingPower = account.BuyingPower;
                    Decimal portfolioValue = account.Equity;

                    // Get information about our existing position.
                    int positionQuantity = 0;
                    Decimal positionValue = 0;
                    try
                    {
                        var currentPosition = await _tradingClient.Client().GetPositionAsync(_stockSymbol);
                        positionQuantity = currentPosition.Quantity;
                        positionValue = currentPosition.MarketValue;
                    }
                    catch (Exception)
                    {
                        // No position exists. This exception can be safely ignored.
                    }

                    var bars = await _dataClient.GetBarSet(_stockSymbol);

                    Decimal avg = bars.Average(item => item.Close);
                    Decimal currentPrice = bars.Last().Close;
                    Decimal diff = avg - currentPrice;

                    if (diff <= 0)
                    {
                        // Above the 20 minute average - exit any existing long position.
                        if (positionQuantity > 0)
                        {
                            Console.WriteLine("Setting position to zero.");
                            var orderId = await _tradingClient.SubmitMarketOrder(_stockSymbol, positionQuantity, currentPrice, OrderSide.Sell);
                            if (orderId != Guid.Empty)
                            {
                                lastTradeId = orderId;
                            }
                        }
                        else
                        {
                            Console.WriteLine("No position to exit.");
                        }
                    }
                    else
                    {
                        // Allocate a percent of our portfolio to this position.
                        Decimal portfolioShare = diff / currentPrice * _scale;
                        Decimal targetPositionValue = portfolioValue * portfolioShare;
                        Decimal amountToAdd = targetPositionValue - positionValue;

                        if (amountToAdd > 0)
                        {
                            // Buy as many shares as we can without going over amountToAdd.

                            // Make sure we're not trying to buy more than we can.
                            if (amountToAdd > buyingPower)
                            {
                                amountToAdd = buyingPower;
                            }
                            int qtyToBuy = (int)(amountToAdd / currentPrice);

                            var orderId = await _tradingClient.SubmitMarketOrder(_stockSymbol, qtyToBuy, currentPrice, OrderSide.Buy);
                            if (orderId != Guid.Empty)
                            {
                                lastTradeId = orderId;
                            }
                    }
                        else
                        {
                            // Sell as many shares as we can without going under amountToAdd.

                            // Make sure we're not trying to sell more than we have.
                            amountToAdd *= -1;
                            int qtyToSell = (int)(amountToAdd / currentPrice);
                            if (qtyToSell > positionQuantity)
                            {
                                qtyToSell = positionQuantity;
                            }

                            var orderId = await _tradingClient.SubmitMarketOrder(_stockSymbol, qtyToSell, currentPrice, OrderSide.Sell);
                            if (orderId != Guid.Empty)
                            {
                                lastTradeId = orderId;
                            }
                        }
                    }

                    // Wait another minute.
                    Thread.Sleep(60000);
                    timeUntilClose = closingTime - DateTime.UtcNow;
                }

                Console.WriteLine("Market nearing close; closing position.");
                await _tradingClient.ClosePositionAtMarket(_stockSymbol);
            }

            public void Dispose()
            {
                _tradingClient?.Client().Dispose();
                _dataClient?.DataClient().Dispose();
            }
            
    }
}
