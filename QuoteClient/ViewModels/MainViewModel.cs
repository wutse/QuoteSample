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
            
            openAllInOneCommand = new RelayCommand(OnOpenAllInOne, CanOpenAllInOne);
            openTenCommand = new RelayCommand(OnOpenTen, CanOpenTen);

            simulateAdapter = new SimulateAdapter();
            simulateAdapter.OnFeedError += SimulateAdapter_OnFeedError;

            socketStartCommand = new RelayCommand(OnSocketStart, CanSocketStart);
            socketStopCommand = new RelayCommand(OnSocketStop, CanSocketStop);

            socketAdapter = new SocketAdapter();
            socketAdapter.OnFeedError += SocketAdapter_OnFeedError;

            Interval = 0;

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

        private IFeedAdapter simulateAdapter;

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

        private void SimulateAdapter_OnFeedError(object sender, Exception err)
        {
            MessageBox.Show(err.Message);
        }

        #endregion

        #region Socket

        private IFeedAdapter socketAdapter;

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

        private void SocketAdapter_OnFeedError(object sender, Exception err)
        {
            MessageBox.Show(err.Message);
        }

        #endregion

        #region Open Win
        private readonly RelayCommand openAllInOneCommand;
        public ICommand OpenAllInOneCommand => openAllInOneCommand;

        private void OnOpenAllInOne(object commandParameter)
        {
            try
            {
                QuoteViewModel qVM = new QuoteViewModel(Quote.Quotes.Values);
                QuoteView qv = new QuoteView
                {
                    DataContext = qVM
                };
                qv.Show();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private bool CanOpenAllInOne(object commandParameter)
        {
            return true;
        }

        private readonly RelayCommand openTenCommand;
        public ICommand OpenTenCommand => openTenCommand;

        private void OnOpenTen(object commandParameter)
        {
            try
            {
                for (int i = 1; i < 10; i++)
                {
                    var qs = Quote.Quotes.Values.Where(s => s.Stock.Symbol.StartsWith(i.ToString()));

                    QuoteViewModel qVM = new QuoteViewModel(qs);
                    QuoteView qv = new QuoteView
                    {
                        DataContext = qVM
                    };
                    qv.Show();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message);
            }
        }

        private bool CanOpenTen(object commandParameter)
        {
            return true;
        }
        #endregion

        #endregion
    }
}
