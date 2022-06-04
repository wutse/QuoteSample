using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoteClient.ViewModels
{
    public class QuoteViewModel : ModelBase
    {
        public QuoteViewModel()
        { }

        public QuoteViewModel(IEnumerable<Quote> quotes) :
            this()
        {
            foreach (var quote in quotes)
            {
                QuoteVMs.Add(new QuoteItemViewModel(quote));
            }
        }

        private ObservableCollection<QuoteItemViewModel> quoteVMs = new ObservableCollection<QuoteItemViewModel>();
        public ObservableCollection<QuoteItemViewModel> QuoteVMs
        {
            get { return quoteVMs; }
            set { SetProperty(ref quoteVMs, value); }
        }

    }
}
