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

        public event FeedError OnFeedError;

        public void Start()
        {
            //jobTask = Task.Run(() =>
            //{
            //    GenerateData(0, 1761);
            //});

            Task.Factory.StartNew(new Action(() =>
            {
                GenerateData(0, 500);
            }));

            Task.Factory.StartNew(new Action(() =>
            {
                GenerateData(501, 1000);
            }));

            Task.Factory.StartNew(new Action(() =>
            {
                GenerateData(1001, 1500);
            }));

            Task.Factory.StartNew(new Action(() =>
            {
                GenerateData(1501, 1761);
            }));
        }

        private void GenerateData(int from, int to)
        {
            try
            {
                isStop = false;

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                int index = 0;
                Quote quote;
                Random rnd = new Random();
                string[] symbols = Stock.StockInfos.Keys.ToArray();
                while (true)
                {
                    if (isStop)
                    {
                        break;
                    }

                    sw.Restart();
                    index = rnd.Next(from, to);
                    quote = Quote.Quotes[symbols[index]];
                    quote.UpdateValues();
                    sw.Stop();
                    //System.Diagnostics.Debug.WriteLine($"total ms = {sw.ElapsedMilliseconds}");
                    //Thread.Sleep(1); //Thread.Sleep(1) tends to block for somewhere between 12-15ms
                }
            }
            catch (Exception err)
            {
                OnFeedError?.Invoke("Models.SimulateAdapter.Start", err);
            }
        }

        public void Stop()
        {
            isStop = true;
        }
    }
}
