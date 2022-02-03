using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SimulateAdapter : IFeedAdapter
    {
        public SimulateAdapter()
        {

        }

        public void Start()
        {
            Task.Run(() =>
            {
                Random rnd = new Random();
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                decimal step = 0m, tempPrice = 0m;
                while (true)
                {
                    sw.Restart();
                    foreach (Quote quote in Quote.Quotes.Values)
                    {
                        //增加差異
                        if (rnd.Next(0, 1000) > 900)
                        {
                            continue;
                        }

                        step = (rnd.Next(0, 2) * 2 - 1) * PriceStepList.AllPriceSteps["Stock"].Where(c => c.Key > (quote.LastPrice == 0 ? quote.Stock.RefPrice : quote.LastPrice)).Min(c => c.Value);
                        tempPrice = quote.LastPrice + step;
                        if (tempPrice <= quote.Stock.LimitUp && tempPrice >= quote.Stock.LimitDown)
                        {
                            quote.LastPrice = tempPrice;
                        }

                        quote.Volume += rnd.Next(0, 2);

                        quote.MarketTime = DateTime.Now;
                    }
                    sw.Stop();
                    System.Diagnostics.Debug.WriteLine($"total ms = {sw.ElapsedMilliseconds}");
                    Task.Delay(1);
                }
            });
        }
    }
}
