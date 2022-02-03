using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Quote : ModelBase
    {
        public static ConcurrentDictionary<string, Quote> Quotes { get; set; } = new ConcurrentDictionary<string, Quote>();
        static Quote()
        {
            foreach (Stock stock in Stock.StockInfos.Values)
            {
                Quotes.TryAdd(stock.Symbol, new Quote() { Stock = stock, LastPrice = stock.RefPrice });
            }
        }

        private Stock _stock;
        /// <summary>
        /// 商品
        /// </summary>
        public Stock Stock 
        {
            get { return _stock; }
            set { SetProperty(ref _stock, value); }
        }

        private decimal _lastPrice;
        /// <summary>
        /// 成交價
        /// </summary>
        public decimal LastPrice
        {
            get { return _lastPrice; }
            set { SetProperty(ref _lastPrice, value); }
        }

        private int _volume;
        /// <summary>
        /// 
        /// </summary>
        public int Volume
        {
            get { return _volume; }
            set { SetProperty(ref _volume, value); }
        }

        private DateTime _marketTime;
        public DateTime MarketTime
        {
            get { return _marketTime; }
            set { SetProperty(ref _marketTime, value); }
        }
    }
}
