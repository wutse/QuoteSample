using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public interface IFeedAdapter
    {
        ConcurrentDictionary<string, Quote> Quotes { get; set; }

        void Start();


    }
}
