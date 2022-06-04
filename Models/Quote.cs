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
        private Random rnd = new Random();

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

        private decimal _buyPrice;
        /// <summary>
        /// 委買價
        /// </summary>
        public decimal BuyPrice
        {
            get { return _buyPrice; }
            set { SetProperty(ref _buyPrice, value); }
        }

        private decimal _sellPrice;
        /// <summary>
        /// 委賣價
        /// </summary>
        public decimal SellPrice
        {
            get { return _sellPrice; }
            set { SetProperty(ref _sellPrice, value); }
        }

        private decimal _buyQty;
        /// <summary>
        /// 委買價
        /// </summary>
        public decimal BuyQty
        {
            get { return _buyQty; }
            set { SetProperty(ref _buyQty, value); }
        }

        private decimal _sellQty;
        /// <summary>
        /// 委賣價
        /// </summary>
        public decimal SellQty
        {
            get { return _sellQty; }
            set { SetProperty(ref _sellQty, value); }
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

        private decimal _maxPrice;
        /// <summary>
        /// 委賣價
        /// </summary>
        public decimal MaxPrice
        {
            get { return _maxPrice; }
            set { SetProperty(ref _maxPrice, value); }
        }

        private decimal _minPrice;
        /// <summary>
        /// 委賣價
        /// </summary>
        public decimal MinPrice
        {
            get { return _minPrice; }
            set { SetProperty(ref _minPrice, value); }
        }

        private int _qty;
        /// <summary>
        /// 單量
        /// </summary>
        public int Qty
        {
            get { return _qty; }
            set { SetProperty(ref _qty, value); }
        }

        private int _volume;
        /// <summary>
        /// 總量
        /// </summary>
        public int Volume
        {
            get { return _volume; }
            set { SetProperty(ref _volume, value); }
        }

        private decimal _yesterdayPrice;
        /// <summary>
        /// 成交價
        /// </summary>
        public decimal YesterdayPrice
        {
            get { return _yesterdayPrice; }
            set { SetProperty(ref _yesterdayPrice, value); }
        }

        private int _yesterdayVolume;
        /// <summary>
        /// 成交價
        /// </summary>
        public int YesterdayVolume
        {
            get { return _yesterdayVolume; }
            set { SetProperty(ref _yesterdayVolume, value); }
        }

        /// <summary>
        /// 時間
        /// </summary>
        private DateTime _marketTime;
        public DateTime MarketTime
        {
            get { return _marketTime; }
            set { SetProperty(ref _marketTime, value); }
        }

        private readonly object _lock = new object();

        public void UpdateValues() 
        {
            try
            {
                lock (_lock)
                {
                    decimal direction = rnd.Next(0, 2) * 2 - 1;
                    decimal step = PriceStepList.AllPriceSteps["Stock"].Where(c => c.Key > (this.LastPrice == 0 ? this.Stock.RefPrice : this.LastPrice)).Min(c => c.Value);
                    decimal tempPrice = this.LastPrice + direction * step;
                    if (tempPrice <= this.Stock.LimitUp && tempPrice >= this.Stock.LimitDown)
                    {
                        this.LastPrice = tempPrice;
                    }

                    this.BuyPrice = this.LastPrice - step;
                    this.SellPrice = this.LastPrice + step;
                    this.BuyQty = rnd.Next(0, 100);
                    this.SellQty = rnd.Next(0, 100);

                    if (this.LastPrice > this.MaxPrice)
                    {
                        this.MaxPrice = this.LastPrice;
                    }

                    if (this.LastPrice < this.MinPrice)
                    {
                        this.MinPrice = this.LastPrice;
                    }

                    this.Qty = rnd.Next(0, 50);
                    this.Volume += this.Qty;

                    this.MarketTime = DateTime.Now;
                }
            }
            catch (Exception err)
            {
                string str = err.Message;
            }
        }
    }
}
