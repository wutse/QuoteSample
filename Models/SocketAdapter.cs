using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Models
{
    public class StateObject
    {
        public Socket WorkSocket { get; set; } = null;

        public const int BufferSize = 1024;

        public byte[] Buffer { get; set; } = new byte[BufferSize];

        public StringBuilder SrcDatas { get; set; } = new StringBuilder();
    }

    public class SocketAdapter : IFeedAdapter
    {
        public SocketAdapter()
        { }

        private ManualResetEvent connectDone = null;
        private ManualResetEvent sendDone = null;
        private ManualResetEvent receiveDone = null;
        private ManualResetEvent parseDone = null;

        private ConcurrentQueue<string> srcQueue = new ConcurrentQueue<string>();

        private Task praseTask = null;
        private Task receiveTask = null;

        private bool isStop = true;

        private Socket client = null;


        private IPEndPoint targetServer = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 12345);

        public event FeedError OnFeedError;

        public void Start()
        {
            try
            {
                connectDone = new ManualResetEvent(false);
                sendDone = new ManualResetEvent(false);
                receiveDone = new ManualResetEvent(false);
                parseDone = new ManualResetEvent(false);

                client = new Socket(targetServer.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                client.BeginConnect(targetServer, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                if (client.Connected)
                {
                    //isStop = false;

                    praseTask = Task.Run(() =>
                    {
                        StringBuilder sb = new StringBuilder();
                        string queueString = null;
                        string srcString = null;
                        string pareseString = null;
                        string[] srcDatas = null;
                        Quote quote = null;
                        parseDone.Set();
                        while (true)
                        {
                            if (isStop)
                            {
                                break;
                            }

                            if (srcQueue.TryDequeue(out queueString))
                            {
                                //System.Diagnostics.Debug.WriteLine($"Queue count : {srcQueue.Count}, Dequeue data : {queueString}");
                                sb.Append(queueString);
                                srcString = sb.ToString();
                                pareseString = srcString.Substring(0, srcString.LastIndexOf('|') + 1);
                                srcDatas = pareseString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (string srcData in srcDatas)
                                {
                                    string[] srcs = srcData.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                    quote = Quote.Quotes[srcs[0]];
                                    quote.LastPrice = decimal.Parse(srcs[1]);
                                    quote.Volume = int.Parse(srcs[2]);
                                    quote.MarketTime = DateTime.Now;
                                }

                                sb.Remove(0, pareseString.Length);
                            }
                            else
                            {
                                parseDone.WaitOne();
                            }
                        }
                    });

                    receiveTask = Task.Run(() =>
                    {
                        receiveDone.Set();

                        while (true)
                        {
                            if (isStop)
                            {
                                break;
                            }

                            if (client.Connected)
                            {
                                Receive(client);
                            }
                            else
                            {
                                isStop = true;
                                break;
                            }

                            receiveDone.WaitOne();
                        }
                    });

                    var timer = new System.Timers.Timer(1000);
                    timer.Elapsed += (obj, e) =>
                    {
                        if (isStop)
                        {
                            timer.Stop();
                        }

                        System.Diagnostics.Debug.WriteLine($"Queue count : {srcQueue.Count}");
                    };
                    timer.AutoReset = true;
                    timer.Enabled = true;
                }
                else
                {
                }
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.ToString());
                OnFeedError?.Invoke("Models.SocketAdapter.ConnectCallback", err);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndConnect(ar);

                System.Diagnostics.Debug.WriteLine($"Socket connected to {socket.RemoteEndPoint.ToString()}");
                isStop = false;
                connectDone.Set();
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.ToString());

                isStop = true;
                connectDone.Set();
                OnFeedError?.Invoke("Models.SocketAdapter.ConnectCallback", err);
            }
        }

        private void Receive(Socket socket)
        {
            try
            {
                StateObject state = new StateObject();
                state.WorkSocket = socket;
                int bytesRec = socket.Receive(state.Buffer);
                srcQueue.Enqueue(Encoding.ASCII.GetString(state.Buffer, 0, bytesRec));

                parseDone.Set();
                receiveDone.Set();
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.ToString());
                OnFeedError?.Invoke("Models.SocketAdapter.ConnectCallback", err);
            }
        }

        private void Send(Socket target, String data)
        {
            try
            {
                byte[] byteData = Encoding.ASCII.GetBytes(data);
                target.Send(Encoding.ASCII.GetBytes(data));
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.ToString());
                OnFeedError?.Invoke("Models.SocketAdapter.ConnectCallback", err);
            }
        }

        public void Stop()
        {
            try
            {
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                client.Dispose();
                isStop = true;
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.ToString());
                OnFeedError?.Invoke("Models.SocketAdapter.ConnectCallback", err);
            }
        }
    }
}
