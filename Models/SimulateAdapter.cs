using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Models
{
    public class SimulateAdapter : IFeedAdapter
    {
        public SimulateAdapter()
        {

        }

        private Task jobTask = null;
        private bool isStop = false;

        public void Start()
        {
            jobTask = Task.Run(() =>
            {
                isStop = false;

                Random rnd = new Random();
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                int index = 0;
                Quote quote;
                string[] symbols = Stock.StockInfos.Keys.ToArray();
                while (true)
                {
                    if (isStop)
                    {
                        break;
                    }

                    sw.Restart();
                    index = rnd.Next(0, 373);
                    quote = Quote.Quotes[symbols[index]];
                    quote.UpdateValues();
                    sw.Stop();
                    System.Diagnostics.Debug.WriteLine($"total ms = {sw.ElapsedMilliseconds}");
                    Thread.Sleep(1);
                }
            });
        }

        public void Stop()
        {
            isStop = true;
        }
    }
}
