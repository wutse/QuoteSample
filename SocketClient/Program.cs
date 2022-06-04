using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketClient
{
    public class StateObject
    {
        public Socket workSocket { get; set; } = null;

        public const int BufferSize = 256;

        public byte[] Buffer { get; set; } = new byte[BufferSize];

    }

    internal class Program
    {
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        private static ManualResetEvent parseDone = new ManualResetEvent(false);

        private static ConcurrentQueue<string> srcQueue = new ConcurrentQueue<string>();
        private static Task praseTask = null;

        // The response from the remote device.  
        private static string response = string.Empty;

        private static void StartClient()
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1}), 12345);

                Socket client = new Socket(remoteEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                //Send(client, "This is a test<EOF>");
                //sendDone.WaitOne();

                praseTask = Task.Run(() => 
                {
                    string srcData = null;
                    while (true)
                    {
                        parseDone.Set();

                        if (srcQueue.TryDequeue(out srcData))
                        {
                            Console.WriteLine($"Queue count : {srcQueue.Count}, Dequeue data : {srcData}");
                        }
                        else
                        {
                            parseDone.WaitOne();
                        }
                    }
                });

                while (true)
                {
                    receiveDone.Set();

                    Receive(client);

                    receiveDone.WaitOne();
                }


                //client.Shutdown(SocketShutdown.Both);
                //client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());

                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.  
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.  
                client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.  
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.  
                int bytesRead = client.EndReceive(ar);
                StringBuilder sb = new StringBuilder();
                string srcData = Encoding.ASCII.GetString(state.Buffer, 0, bytesRead);
                srcQueue.Enqueue(srcData);

                Console.WriteLine("Enqueue received : {0}", srcData);

                client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);

                parseDone.Set();
                receiveDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.  
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.  
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.  
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            StartClient();
        }
    }
}
