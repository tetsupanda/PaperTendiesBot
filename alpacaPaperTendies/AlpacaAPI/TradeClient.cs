using Alpaca.Markets;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace alpacaPaperTendies.AlpacaAPI
{
    public class TradeClient : AlpacaApiBase
    {

        public TradeClient()
        {
            
        }

        private AlpacaTradingClient _client;
        public AlpacaTradingClient Client()
        {
            _client = Environments.Paper.GetAlpacaTradingClient(new SecretKey(ClientKey, ClientSecret));
            return _client;
        }


        // Figure out when the market will close so we can prepare to sell beforehand.
        public async Task<DateTime> GetMarketClose()
        {
            var calendars = (await this.Client()
                    .ListCalendarAsync(new CalendarRequest().SetInclusiveTimeInterval(DateTime.Today, null)))
                    .ToList();
            var calendarDate = calendars.First().TradingDate;
            var closingTime = calendars.First().TradingCloseTime;

            closingTime = new DateTime(calendarDate.Year, calendarDate.Month, calendarDate.Day, closingTime.Hour, closingTime.Minute, closingTime.Second);
            return closingTime;
        }

        public async Task AwaitMarketOpen()
        {
            while (!(await this.Client().GetClockAsync()).IsOpen)
            {
                await Task.Delay(60000);
            }
        }

        public async Task<Guid> SubmitMarketOrder(string stockSymbol, int quantity, Decimal price, OrderSide side)
        {
            if (quantity == 0)
            {
                Console.WriteLine("No order necessary.");
                return Guid.Empty;
            }
            Console.WriteLine($"Submitting {side} order for {quantity} shares at ${price}.");
            var order = await this.Client().PostOrderAsync(
                // TODO: Maybe revist changing function signature to handle OrderType and TimeInForce
                new NewOrderRequest(stockSymbol, quantity, side, OrderType.Limit, TimeInForce.Day)
                {
                    LimitPrice = price
                });
            return order.OrderId;
        }

        public async Task CancelAllOpenOrders()
        {
            var orders = await this.Client().ListAllOrdersAsync();
            foreach (var order in orders)
            {
                await this.Client().DeleteOrderAsync(order.OrderId);
            }
        }

        public async Task ClosePositionAtMarket(string stockTickerSymbol)
        {
            try
            {
                var positionQuantity = (await this.Client().GetPositionAsync(stockTickerSymbol)).Quantity;
                await this.Client().PostOrderAsync(
                    // TODO: Maybe revist changing function signature to handle OrderSide, OrderType, and TimeInForce
                    new NewOrderRequest(stockTickerSymbol, positionQuantity, OrderSide.Sell, OrderType.Market, TimeInForce.Day));
            }
            catch (Exception)
            {
                // No position to exit.
            }
        }
    }
}
