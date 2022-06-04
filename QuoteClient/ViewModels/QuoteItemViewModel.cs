using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteClient.ViewModels
{
    public class QuoteItemViewModel : ModelBase
    {
        public QuoteItemViewModel(Quote quote)
        {
            _quote = quote;
            _quote.PropertyChanged += Quote_PropertyChanged;
        }

        private Quote _quote;
        /// <summary>
        /// 
        /// </summary>
        public Quote Quote
        {
            get { return _quote; }
            set { SetProperty(ref _quote, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int UpDownFlag
        {
            get 
            {
                if (Quote.LastPrice > Quote.Stock.RefPrice)
                {
                    if(Quote.LastPrice == Quote.Stock.LimitUp)
                    {
                        return 2;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else if (Quote.LastPrice < Quote.Stock.RefPrice)
                {
                    if (Quote.LastPrice == Quote.Stock.LimitDown)
                    {
                        return -2;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else 
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CurrentPrice => string.Format("{0:n2}", Quote.LastPrice); 
       
        /// <summary>
        /// 
        /// </summary>
        public string CurrentVolume => string.Format("{0:n0}", Quote.Volume);

        /// <summary>
        /// 
        /// </summary>
        public string Change 
        {
            get 
            {
                string result;
                decimal tempChange = Quote.LastPrice - Quote.Stock.RefPrice;
                switch (UpDownFlag)
                {
                    case 1:
                    case 2:
                        result = string.Format("🔺{0:n2}", tempChange);
                        break;
                    case -1:
                    case -2:
                        result = string.Format("🔻{0:n2}", tempChange);
                        break;
                    default:
                        result = "--";
                        break;
                }

                return result;
            }
        } 

        public DateTime LastUpdateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 
        /// </summary>
        public string ChangeRate => string.Format("({0:p2})", (Quote.Stock.RefPrice == 0 ? 0 : ((Quote.LastPrice - Quote.Stock.RefPrice) / Quote.Stock.RefPrice)));

        private void Quote_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LastPrice" || e.PropertyName == "Volume")
            {
                TimeSpan ts1 = DateTime.Now - LastUpdateTime;
                TimeSpan ts2 = DateTime.Now - Quote.MarketTime;
                if (ts1.TotalMilliseconds > MainViewModel.Instance.Interval || ts2.TotalMilliseconds >= MainViewModel.Instance.Interval)
                {
                    LastUpdateTime = DateTime.Now;
                    OnPropertyChanged("UpDownFlag");
                    OnPropertyChanged("CurrentPrice");
                    OnPropertyChanged("CurrentVolume");
                    OnPropertyChanged("Change");
                    OnPropertyChanged("ChangeRate");
                }
            }            
        }
    }
}
