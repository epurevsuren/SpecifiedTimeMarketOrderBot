using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.EasternStandardTime, AccessRights = AccessRights.None)]
    public class SpecifiedTimeMarketOrderBot : Robot
    {
        [Parameter("Quantity (Lots)", Group = "Volume", DefaultValue = 1, MinValue = 0.01, Step = 0.01)]
        public double Quantity { get; set; }

        [Parameter("Stop Loss (pips)", Group = "Protection", DefaultValue = 3, MinValue = 1)]
        public int StopLossInPips { get; set; }

        [Parameter("Take Profit (pips)", Group = "Protection", DefaultValue = 13, MinValue = 2)]
        public int TakeProfitInPips { get; set; }

        [Parameter("Entry Hour (24h)", Group = "Protection", DefaultValue = 16, MinValue = 0)]
        public int orderHour { get; set; }

        [Parameter("Entry Minute (0-59)", Group = "Protection", DefaultValue = 55, MinValue = 0)]
        public int orderMinute { get; set; }

        [Parameter("Delay Seconds (0-59)", Group = "Protection", DefaultValue = 30, MinValue = 0)]
        public int delaySeconds { get; set; }

        [Parameter("Trailing Stop", Group = "Protection", DefaultValue = true)]
        public bool trailingStop { get; set; }

        [Parameter("Bull Sentiment", Group = "Protection", DefaultValue = true)]
        public bool bullSentiment { get; set; }

        [Parameter("Bear Sentiment", Group = "Protection", DefaultValue = true)]
        public bool bearSentiment { get; set; }

        string label = "Friday Night Celebration cBot";



        protected override void OnStart()
        {
            // Put your initialization logic here
            Print(label + " started...");

            string currentDayOfWeek = Time.DayOfWeek.ToString();
        }

        protected override void OnBar()
        {
            // Put your core logic here
            int currentHour = Time.Hour;
            int currentMinute = Time.Minute;

            if (currentHour == orderHour && currentMinute == orderMinute)
            {
                int milliseconds = 1000 * delaySeconds;
                System.Threading.Thread.Sleep(milliseconds);

                var Entry = TradeType.Buy;

                if (bullSentiment)
                    marketOrder(Entry);

                if (bearSentiment)
                {
                    Entry = TradeType.Sell;
                    marketOrder(Entry);
                }
            }
        }

        void marketOrder(TradeType entryType)
        {
            var volumeInUnits = Symbol.QuantityToVolumeInUnits(Quantity);
            var result = ExecuteMarketOrder(entryType, SymbolName, volumeInUnits, label, StopLossInPips, TakeProfitInPips, "nothing", trailingStop, StopTriggerMethod.Trade);
            if (result.IsSuccessful)
            {
                var position = result.Position;
                Print("Position entry price is {0}", position.EntryPrice);
                Print("Position SL price is {0}", position.StopLoss);
            }
        }

        protected override void OnStop()
        {
            // Put your deinitialization logic here
        }
    }
}
