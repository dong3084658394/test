﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FormClient
{
    public partial class FrmClient : Form
    {
        public FrmClient()
        {
            InitializeComponent();
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }

        Thread threadClient = null; // 创建用于接收服务端消息的 线程；  
        Socket sockClient = null;
        /// <summary>
        /// 连接服务
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse(txtIp.Text.Trim());
            IPEndPoint endPoint = new IPEndPoint(ip, int.Parse(txtPort.Text.Trim()));
            sockClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                ShowMsg("与服务器连接中……");
                sockClient.Connect(endPoint);

            }
            catch (SocketException se)
            {
                btnConnect_Click(null, null);
                return;
                //this.Close();  
            }
            ShowMsg("与服务器连接成功！！！");
            threadClient = new Thread(RecMsg);
            threadClient.IsBackground = true;
            threadClient.Start();
        }
        /// <summary>
        /// 接受数据
        /// </summary>
        void RecMsg()
        {
            while (true)
            {
                // 定义一个2M的缓存区；  
                byte[] arrMsgRec = new byte[1024 * 1024 * 2];
                // 将接受到的数据存入到输入  arrMsgRec中；  
                int length = -1;
                try
                {
                    length = sockClient.Receive(arrMsgRec); // 接收数据，并返回数据的长度；  
                }
                catch (SocketException se)
                {
                    btnConnect_Click(null,null);
                    ShowMsg("异常；" + se.Message);
                    return;
                }
                catch (Exception e)
                {
                    btnConnect_Click(null, null);
                    ShowMsg("异常：" + e.Message);
                    return;
                }
              
                    string strMsg = System.Text.Encoding.UTF8.GetString(arrMsgRec, 1, length - 1);// 将接受到的字节数据转化成字符串；  
                    ShowMsg(strMsg);
              
                if (arrMsgRec[0] == 1) // 表示接收到的是文件数据；  
                {

                    try
                    {
                        SaveFileDialog sfd = new SaveFileDialog();

                        if (sfd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                        {// 在上边的 sfd.ShowDialog（） 的括号里边一定要加上 this 否则就不会弹出 另存为 的对话框，而弹出的是本类的其他窗口，，这个一定要注意！！！【解释：加了this的sfd.ShowDialog(this)，“另存为”窗口的指针才能被SaveFileDialog的对象调用，若不加thisSaveFileDialog 的对象调用的是本类的其他窗口了，当然不弹出“另存为”窗口。】  

                            string fileSavePath = sfd.FileName;// 获得文件保存的路径；  
                            // 创建文件流，然后根据路径创建文件；  
                            using (FileStream fs = new FileStream(fileSavePath, FileMode.Create))
                            {
                                fs.Write(arrMsgRec, 1, length - 1);
                                ShowMsg("文件保存成功：" + fileSavePath);
                            }
                        }
                    }
                    catch (Exception aaa)
                    {
                        MessageBox.Show(aaa.Message);
                    }
                }
            }
        }
        void ShowMsg(string str)
        {
            txtMsg.AppendText(str + "\r\n");
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            string strMsg = txtName.Text.Trim() + "\r\n" + "    -->" + txtSendMsg.Text.Trim() + "\r\n";
            byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(strMsg);
            byte[] arrSendMsg = new byte[arrMsg.Length + 1];
            arrSendMsg[0] = 0; // 用来表示发送的是消息数据  
            Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
            sockClient.Send(arrSendMsg); // 发送消息；  
            ShowMsg(strMsg);
            txtSendMsg.Clear();
        }
        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = "D:\\";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtSelectFile.Text = ofd.FileName;
            }
        }
        /// <summary>
        /// 发送文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSendFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSelectFile.Text))
            {
                MessageBox.Show("请选择要发送的文件！！！");
            }
            else
            {
                // 用文件流打开用户要发送的文件；  
                using (FileStream fs = new FileStream(txtSelectFile.Text, FileMode.Open))
                {
                    //在发送文件以前先给好友发送这个文件的名字+扩展名，方便后面的保存操作；  
                    string fileName = System.IO.Path.GetFileName(txtSelectFile.Text);
                    string fileExtension = System.IO.Path.GetExtension(txtSelectFile.Text);
                    string strMsg = "我给你发送的文件为： " + fileName + "\r\n";
                    byte[] arrMsg = System.Text.Encoding.UTF8.GetBytes(strMsg);
                    byte[] arrSendMsg = new byte[arrMsg.Length + 1];
                    arrSendMsg[0] = 0; // 用来表示发送的是消息数据  
                    Buffer.BlockCopy(arrMsg, 0, arrSendMsg, 1, arrMsg.Length);
                    sockClient.Send(arrSendMsg); // 发送消息；  

                    byte[] arrFile = new byte[1024 * 1024 * 2];
                    int length = fs.Read(arrFile, 0, arrFile.Length);  // 将文件中的数据读到arrFile数组中；  
                    byte[] arrFileSend = new byte[length + 1];
                    arrFileSend[0] = 1; // 用来表示发送的是文件数据；  
                    Buffer.BlockCopy(arrFile, 0, arrFileSend, 1, length);
                    // 还有一个 CopyTo的方法，但是在这里不适合； 当然还可以用for循环自己转化；  
                    sockClient.Send(arrFileSend);// 发送数据到服务端；  
                    txtSelectFile.Clear();
                }
            }
        }


    }
}
