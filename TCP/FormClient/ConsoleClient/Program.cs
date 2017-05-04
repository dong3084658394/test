using LogHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleClient
{
    class Program
    {
        // 远程设备的端口号.     
        private const int port = 6000;
        //172.16.47.0: 6000
        // manualresetevent实例完成信号 .     
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);
        // 来自远程设备的响应.     
        private static String response = String.Empty;

        static void Main(String[] args)
        {
            StartClient();
            Console.Read();
        }
        /// <summary>
        /// 连接服务
        /// </summary>
        private static void StartClient()
        {
            // 连接到远程设备.     
            try
            {
                // 为套接字建立远程终结点。 
                // The name of the     
                // remote device is "host.contoso.com".     
                //IPHostEntry ipHostInfo = Dns.Resolve("user");
                //IPAddress ipAddress = ipHostInfo.AddressList[0];
                IPAddress ipAddress = IPAddress.Parse("172.16.47.0");
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                // 创建一个 TCP/IP socket.     
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                // 连接到远程端点 .     
                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();
                // 将测试数据发送到远程设备 .     
                // Send(client, "This is a test<EOF>");
                // sendDone.WaitOne();
                // 接收数据    
                Receive(client);
                receiveDone.WaitOne();
                // 将响应写入控制台.     
                Console.WriteLine("Response received : {0}", response);
                // 释放 socket.     
                client.Shutdown(SocketShutdown.Both);
                client.Close();

                
            }
            catch (Exception e)
            {
                NLogHelper.Debug(e.ToString());
            }
        }
        /// <summary>
        /// 异步连接服务
        /// </summary>
        /// <param name="ar"></param>
        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // 从object中获取socket.     
                Socket client = (Socket)ar.AsyncState;
                // 完成异步连接.     
                client.EndConnect(ar);
                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());
                // 已取得连接的信号    
                connectDone.Set();
            }
            catch (Exception e)
            {
                NLogHelper.Debug(e.ToString());
            }
        }
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="client"></param>
        private static void Receive(Socket client)
        {
            try
            {
                // 创建stateObject对象.     
                StateObject state = new StateObject();
                state.workSocket = client;
                // 开始接收数据.     
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                NLogHelper.Debug(e.ToString());
            }
        }
        /// <summary>
        /// 接收异步数据
        /// </summary>
        /// <param name="ar"></param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // 获取 stateObject对象 和 socket 对象     
                // 从异步中获取 stateObject.     
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                //读取返回来的数据     
                int bytesRead = client.EndReceive(ar);
                if (bytesRead > 0)
                {
                    //  储存接收到的数据  
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                    // 获取数据.     
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReceiveCallback), state);

                    string data = System.Text.Encoding.UTF8.GetString(state.buffer);

                    NLogHelper.Info(data);


                }
                else
                {
                    
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    receiveDone.Set();
                }
            }
            catch (Exception e)
            {
                NLogHelper.Debug(e.ToString());
            }
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="client"></param>
        /// <param name="data"></param>
        private static void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.     
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            // Begin sending the data to the remote device.     
            client.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), client);
        }
        /// <summary>
        /// 异步发送数据
        /// </summary>
        /// <param name="ar"></param>
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
                NLogHelper.Debug(e.ToString());
            }
        }
      
    }

    public class StateObject
    {
        // Client socket.     
        public Socket workSocket = null;
        // Size of receive buffer.     
        public const int BufferSize = 256;
        // Receive buffer.     
        public byte[] buffer = new byte[BufferSize];
        // Received data string.     
        public StringBuilder sb = new StringBuilder();
    }
}
