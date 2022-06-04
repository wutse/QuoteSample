using System;
using System.Collections.Concurrent;
using System.IO;

namespace Models
{
    public class Stock : ModelBase
    {
        public static ConcurrentDictionary<string, Stock> StockInfos = new ConcurrentDictionary<string, Stock>();

        static Stock()
        {
            LoadStockInfos();
        }
        
        #region fields & properties

        /// <summary>
        /// 代碼
        /// </summary>
        public string Symbol { get; private set; } 

        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }

        private decimal _refPrice;
        /// <summary>
        /// 參考價
        /// </summary>
        public decimal RefPrice
        {
            get { return _refPrice; }
            set { SetProperty(ref _refPrice, value); }
        }

        private decimal _limitUp;
        /// <summary>
        /// 漲停價
        /// </summary>
        public decimal LimitUp
        {
            get { return _limitUp; }
            set { SetProperty(ref _limitUp, value); }
        }

        private decimal _limitDown;
        /// <summary>
        /// 跌停價
        /// </summary>
        public decimal LimitDown
        {
            get { return _limitDown; }
            set { SetProperty(ref _limitDown, value); }
        }

        public decimal Capital { get; set; }
        public decimal EPS { get; set; }
        public decimal MarketVlaue { get; set; }

        #endregion

        public static void LoadStockInfos()
        {
            try
            {
                string[] srcs = null;
                using (StreamReader sr = new StreamReader("Stocks_20220604.csv", System.Text.Encoding.GetEncoding("big5")))
                {
                    srcs = sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                }

                Stock stock = null;
                string[] datas = null;
                for (int i = 1; i < srcs.Length; i++)
                {
                    try
                    {
                        datas = srcs[i].Split(',');

                        stock = new Stock()
                        {
                            Symbol = datas[0],
                            Name = datas[1],
                            
                        };

                        decimal tempValue;
                        decimal.TryParse(datas[13], out tempValue);
                        stock.RefPrice = tempValue;
                        decimal.TryParse(datas[17], out tempValue);
                        stock.Capital = tempValue;
                        decimal.TryParse(datas[19], out tempValue);
                        stock.MarketVlaue = tempValue;
                        stock.LimitUp = stock.RefPrice * 1.1m;
                        stock.LimitDown = stock.RefPrice * 0.9m;

                        StockInfos.TryAdd(stock.Symbol, stock);
                    }
                    catch (Exception dataErr)
                    {
                        string str = dataErr.Message;
                        throw;
                    }
                }
            }
            catch (Exception err)
            {
                string str = err.Message;
                throw;
            }
        }
    }
}
