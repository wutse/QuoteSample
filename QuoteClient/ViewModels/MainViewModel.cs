using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuoteClient.ViewModels
{
    public class MainViewModel : ModelBase
    {
        public static MainViewModel Instance { get; set; } = new MainViewModel();

        public MainViewModel()
        {
            simulateStartCommand = new RelayCommand(OnSimulateStart, CanSimulateStart);
            simulateStopCommand = new RelayCommand(OnSimulateStop, CanSimulateStop);

            socketStartCommand = new RelayCommand(OnSocketStart, CanSocketStart);
            socketStopCommand = new RelayCommand(OnSocketStop, CanSocketStop);

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

        #region Commands
        #region Simulate

        private IFeedAdapter simulateAdapter = new SimulateAdapter();

        private readonly RelayCommand simulateStartCommand;
        public ICommand SimulateStartCommand => simulateStartCommand;

        private void OnSimulateStart(object commandParameter)
        {
            try
            {
                simulateAdapter.Start();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private bool CanSimulateStart(object commandParameter)
        {
            return true;
        }

        private readonly RelayCommand simulateStopCommand;
        public ICommand SimulateStopCommand => simulateStopCommand;

        private void OnSimulateStop(object commandParameter)
        {
            try
            {
                simulateAdapter.Stop();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private bool CanSimulateStop(object commandParameter)
        {
            return true;
        }

        #endregion

        #region Socket

        private IFeedAdapter socketAdapter = new SocketAdapter();

        private readonly RelayCommand socketStartCommand;
        public ICommand SocketStartCommand => socketStartCommand;

        private void OnSocketStart(object commandParameter)
        {
            try
            {
                socketAdapter.Start();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private bool CanSocketStart(object commandParameter)
        {
            return true;
        }

        private readonly RelayCommand socketStopCommand;
        public ICommand SocketStopCommand => socketStopCommand;

        private void OnSocketStop(object commandParameter)
        {
            try
            {
                socketAdapter.Stop();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private bool CanSocketStop(object commandParameter)
        {
            return true;
        }

        #endregion
        #endregion
    }
}
