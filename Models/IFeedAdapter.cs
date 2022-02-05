using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public delegate void FeedError(object sender, Exception err);
    public interface IFeedAdapter
    {
        void Start();
        void Stop();

        event FeedError OnFeedError;
    }
}
