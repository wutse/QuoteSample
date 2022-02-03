using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuoteClient.ViewModels
{
    public class MainViewModel : ModelBase
    {
        public static MainViewModel Instance { get; set; } = new MainViewModel();

        public MainViewModel()
        {
            startCommand = new RelayCommand(OnStart, CanStart);

            Interval = 10;

            foreach (Quote q in Quote.Quotes.Values)
            {
                QuoteVMs.Add(new QuoteViewModel(q));
            }
        }   

        private ObservableCollection<QuoteViewModel> quoteVMs = new ObservableCollection<QuoteViewModel>();
        public ObservableCollection<QuoteViewModel> QuoteVMs
        {
            get { return quoteVMs; }
            set { SetProperty(ref quoteVMs, value); }
        }


        private int _interval;
        /// <summary>
        /// 
        /// </summary>
        public int Interval
        {
            get { return _interval; }
            set { SetProperty(ref _interval, value); }
        }

        private readonly RelayCommand startCommand;
        public ICommand StartCommand => startCommand;

        private void OnStart(object commandParameter)
        {
            IFeedAdapter adapter = new SimulateAdapter();
            adapter.Start();
        }

        private bool CanStart(object commandParameter)
        {
            return true;
        }
    }
}
