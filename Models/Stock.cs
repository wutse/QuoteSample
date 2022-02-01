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

        private string _symbol;
        /// <summary>
        /// 代碼
        /// </summary>
        public string Symbol
        {
            get { return _symbol; }
            set { SetProperty(ref _symbol, value); }
        }

        private string _name;
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name 
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

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
        /// 參考價
        /// </summary>
        public decimal LimitUp
        {
            get { return _limitUp; }
            set { SetProperty(ref _limitUp, value); }
        }

        private decimal _limitDown;
        /// <summary>
        /// 參考價
        /// </summary>
        public decimal LimitDown
        {
            get { return _limitDown; }
            set { SetProperty(ref _limitDown, value); }
        }

        #endregion

        public static void LoadStockInfos()
        {
            try
            {
                string[] srcs = null;
                using (StreamReader sr = new StreamReader("twse.csv", System.Text.Encoding.GetEncoding("big5")))
                {
                    srcs = sr.ReadToEnd().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                }

                Stock stock = null;
                string[] datas = null;
                for (int i = 1; i < srcs.Length; i++)
                {
                    datas = srcs[i].Split(',' );

                    stock = new Stock()
                    {
                        Symbol = datas[0],
                        Name = datas[1],
                        LimitUp = Convert.ToDecimal(datas[2]),
                        LimitDown = Convert.ToDecimal(datas[4]),
                        RefPrice = Convert.ToDecimal(datas[6]),
                    };

                    StockInfos.TryAdd(stock.Symbol, stock);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
