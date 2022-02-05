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

        private ManualResetEvent connectDone = new ManualResetEvent(false);
        private ManualResetEvent sendDone = new ManualResetEvent(false);
        private ManualResetEvent receiveDone = new ManualResetEvent(false);
        private ManualResetEvent parseDone = new ManualResetEvent(false);

        private ConcurrentQueue<string> srcQueue = new ConcurrentQueue<string>();

        private Task praseTask = null;
        private Task receiveTask = null;

        private bool isStop = false;

        private Socket client = null;

        public void Start()
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 12345);

                client = new Socket(remoteEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                isStop = false;

                praseTask = Task.Run(() =>
                {
                    StringBuilder sb = new StringBuilder();
                    string queueString = null;
                    string srcString = null;
                    string pareseString = null;
                    string [] srcDatas = null;
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
                            srcDatas = pareseString.Split(new char[]{ '|' }, StringSplitOptions.RemoveEmptyEntries);
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
                    while (true)
                    {
                        if (isStop)
                        {
                            break;
                        }

                        receiveDone.Set();

                        if (client.Connected)
                        {
                            Receive(client);
                        }

                        receiveDone.WaitOne();
                    }
                });

                var timer = new System.Timers.Timer(1000);
                timer.Elapsed += (obj, e) =>
                {
                    System.Diagnostics.Debug.WriteLine($"Queue count : {srcQueue.Count}");
                };
                timer.AutoReset = true;
                timer.Enabled = true;

            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.ToString());
                //throw;
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);

                System.Diagnostics.Debug.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                connectDone.Set();
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.ToString());
                //throw;
            }
        }

        private void Receive(Socket client)
        {
            try
            {
                StateObject state = new StateObject();
                state.WorkSocket = client;
                int bytesRec = client.Receive(state.Buffer);
                srcQueue.Enqueue(Encoding.ASCII.GetString(state.Buffer, 0, bytesRec));

                parseDone.Set();
                receiveDone.Set();
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.ToString());
                //throw;
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
                //throw;
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
                System.Diagnostics.Debug.Write(err.ToString());
                //throw;
            }
        }
    }
}
