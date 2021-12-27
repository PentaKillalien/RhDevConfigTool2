/**
* 命名空间: RhDevConfigTool.Utils
*
* 功 能： N/A
* 类 名：UdpUtils.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/3/9 11:26:15  彭政亮 初版
*
* Copyright (c) 2020 Lir Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：宁波瑞辉智能科技有限公司 　　　　　 　　　　　　　    　│
*└──────────────────────────────────┘
*/
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace RhDevConfigTool.Utils
{
    public class UdpUtils
    {
        private const int LocalPort = 29527;
        /// <summary>
        /// 广播一个信息
        /// </summary>
        /// <param name="sendStr"></param>
        /// <param name="port"></param>
        /// <param name="time">广播次数</param>
        public static void UdpLoopSendMsg(string ip, string sendStr, int port, int time)
        {

            //创建UDPsocket通讯实例
            Socket sock1 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //绑定本地网卡
            sock1.Bind(new IPEndPoint(IPAddress.Parse(ip), 0));
            //创建一个终点实例
            IPEndPoint iep1 = new IPEndPoint(IPAddress.Broadcast, port);
            //准备发送字节数组
            byte[] data = Encoding.ASCII.GetBytes(sendStr);
            // 设置套接字选项

            sock1.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            try
            {
                int count = 0;
                while (count < time)
                {
                    sock1.SendTo(data, iep1);
                    Thread.Sleep(500);
                    count++;
                }
                sock1.Close();//关闭Socket
            }
            catch (Exception ex)
            {
                sock1.Close();
                LogHelper.WriteLog("Udp广播异常", ex);
            }

        }
        /// <summary>
        ///  发送一个消息
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="sendStr"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static void SendOneMsg(string localip, string remoteip, string sendStr, int remoteport)
        {
            //创建UDPsocket通讯实例
            Socket sock1 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //绑定本地网卡
            sock1.Bind(new IPEndPoint(IPAddress.Parse(localip), LocalPort));
            //创建一个终点实例
            IPEndPoint iep1 = new IPEndPoint(IPAddress.Parse(remoteip), remoteport);
            //准备发送字节数组
            byte[] data = Encoding.ASCII.GetBytes(sendStr);
            // 设置套接字选项
            sock1.SendTo(data, iep1);
            sock1.Close();
        }
        /// <summary>
        /// Udp接收方法
        /// </summary>
        /// <param name="StopTime">等待时间</param>
        /// <param name="port">接收端口</param>
        /// <returns></returns>
        public string UdpReceive(int StopTime, int port)
        {

            //创建一个UDPSocket
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, StopTime);
                //创建一个端口为port的终结点接收来自任意端口的信息
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, port);
                sock.Bind(iep);
                EndPoint ep = iep;
                byte[] data = new byte[1024];
                int recv = sock.ReceiveFrom(data, ref ep);
                string stringData = Encoding.ASCII.GetString(data, 0, recv);
                sock.Close();
                return stringData;
            }
            catch (Exception ex)
            {

                sock.Close();
                LogHelper.WriteLog("Udp接收异常", ex);
                return string.Empty;


            }

        }


    }
}
