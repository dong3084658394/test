using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleServer
{
    class Program
    {
        private static byte[] result = new byte[1024];
        private static int myProt = 6001; //端口
        static Socket serverSocket;

        static void Main(String[] args)
        {
            //服务器IP地址
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myProt)); //绑定IP地址：端口
            serverSocket.Listen(10); //设定最多10个排队连接请求
           
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
         
            //通过Clientsoket发送数据
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();

            Console.Read();
        }
        /// <summary>
        /// 监听客户端连接
        /// </summary>
        private static void ListenClientConnect()
        {
            while (true)
            {
                Socket clientSocket = serverSocket.Accept();

                Thread SendThread = new Thread(SendMessage);//发送数据
                SendThread.Start(clientSocket);

                Thread receiveThread = new Thread(ReceiveMessage);//接收数据
                receiveThread.Start(clientSocket);
            }
        }
        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="clientSocket"></param>
        private static void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    //通过clientSocket接收数据
                    int receiveNumber = myClientSocket.Receive(result);
                    Console.WriteLine("接收客户端{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.ASCII.GetString(result, 0, receiveNumber));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="clientSocket"></param>
        public static void SendMessage(object clientSocket) {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                try
                {
                    string strMsg = string.Empty;
                    for (int i = 0; i < 10; i++) {
                        strMsg += "#027009129"+i+ "|05|00|CF4(1," + i + ",3)|" + DateTime.Now.ToString() + "|\r\n";
                    }

                    strMsg ="#0267855818|05|00|CF4(9.7,2.2,0.7)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(11.5,3.3,0.7)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(10.5,3.5,1.0)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(11.7,5.3,1.3)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(10.7,4.9,0.9)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(11.6,5.8,1.5)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(11.9,6.0,1.9)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(11.4,5.9,1.4)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(10.9,4.5,1.0)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.8,3.2,2.3)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.7,3.0,1.1)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.7,3.0,1.1)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.7,3.1,2.8)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.7,3.1,1.1)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.7,3.0,1.1)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.7,3.1,2.4)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.7,3.0,1.1)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.7,3.1,1.1)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.7,3.0,1.1)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.7,3.1,1.1)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.7,3.1,1.1)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.7,3.1,1.1)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.7,3.1,1.1)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.7,3.1,1.1)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.7,3.1,1.1)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.8,3.2,1.0)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.8,3.2,1.0)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.8,3.2,1.0)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.8,3.2,1.0)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.8,3.1,1.0)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.8,3.1,2.3)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.8,3.2,1.0)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.8,3.2,1.0)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.8,3.2,1.0)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.8,3.2,1.0)|" + DateTime.Now.ToString() + "|\r\n"+
                            "#0267855818|05|00|CF4(8.8,3.2,1.0)|" + DateTime.Now.ToString() + "|\r\n";
                    myClientSocket.Send(Encoding.ASCII.GetBytes(strMsg));
                    Console.WriteLine(strMsg);
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }
    }
}
