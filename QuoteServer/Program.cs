using Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuoteServer
{
    internal class Program
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        private static ConcurrentDictionary<string, Socket> clients = new ConcurrentDictionary<string, Socket>();

        public static void StartListening()
        {
            IPEndPoint localEndPoint = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 12345);

            Socket listener = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                string[] symbols = Stock.StockInfos.Keys.ToArray();
                Task.Run(() =>
                {
                    int index = 0;
                    try
                    {
                        Random rnd = new Random();
                        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                        Quote quote;
                        while (true)
                        {
                            sw.Restart();

                            index = rnd.Next(0, 373);
                            quote = Quote.Quotes[symbols[index]];
                            quote.UpdateValues();

                            Console.WriteLine($"{clients.Count} clienst");
                            if (clients.Count > 0)
                            {
                                foreach (var client in clients.Values)
                                {
                                    Send(client, $"{quote.Stock.Symbol},{quote.LastPrice},{quote.Volume}|");
                                }
                            }
                            sw.Stop();
                            System.Diagnostics.Debug.WriteLine($"total ms = {sw.ElapsedMilliseconds}");
                            Thread.Sleep(1);
                        }
                    }
                    catch (Exception taskErr)
                    {
                        System.Diagnostics.Debug.WriteLine(taskErr.ToString());
                    }
                });

                while (true)
                {
                    allDone.Reset();

                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept( new AsyncCallback(AcceptCallback), listener);

                    allDone.WaitOne();
                }
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                allDone.Set();

                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                StateObject state = new StateObject();
                state.WorkSocket = handler;

                handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                clients.TryAdd(handler.RemoteEndPoint.ToString(), handler);
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.ToString());
            }
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            try
            {
                string content = string.Empty;

                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.WorkSocket;

                int bytesRead = handler.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.SrcDatas.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Received : {state.SrcDatas.ToString()}");
                }

                handler.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception err)
            {
                System.Diagnostics.Debug.WriteLine(err.ToString());
            }
        }

        private static void Send(Socket handler, String data)
        {
            string remoteEP = handler.RemoteEndPoint.ToString();
            System.Diagnostics.Debug.WriteLine(remoteEP);
            try
            {
                byte[] byteData = Encoding.ASCII.GetBytes(data);
                handler.Send(byteData);
            }
            catch (Exception err)
            {
                clients.TryRemove(remoteEP, out Socket failedClient);
                System.Diagnostics.Debug.WriteLine(err.ToString());
            }
        }

        static void Main(string[] args)
        {
            StartListening();
        }
    }
}
