/**
* 命名空间: CptConfigureTool.Utils
*
* 功 能： N/A
* 类 名： NetworkUtils
*
* Ver 变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2020-10-15 9:15:17 彭政亮 初版
*
* Copyright (c) 2019 RH Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：宁波瑞辉科技有限公司 　　　　　　　　　　　　　　│
*└──────────────────────────────────┘
*/
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace RhDevConfigTool.Utils
{
    public class NetworkUtils
    {
        /// <summary>
        /// 获取网卡列表
        /// </summary>
        /// <returns></returns>
        public static List<string> NetworkList()
        {
            try
            {
                List<string> NetworkList = new List<string>();
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var item in adapters)
                {
                    if (item.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        NetworkList.Add(item.Description);
                    }

                }
                return NetworkList;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("获取网卡列表", ex);
                return null;
            }

        }
        /// <summary>
        /// 根据网卡描述获取网卡Ipv4
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static string GetIpAddressFromNetworkDescription(string description)
        {
            try
            {
                string addressIp = "";
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var item in adapters)
                {
                    bool bIsOpen = item.OperationalStatus == OperationalStatus.Up;//判断网络连接状态
                    bool bIsLoopback = item.NetworkInterfaceType == NetworkInterfaceType.Loopback;//是否是 回环
                    if (bIsOpen && !bIsLoopback && item.Description.Equals(description))
                    {
                        //获取ip
                        IPInterfaceProperties ipInterfaceProperties = item.GetIPProperties();//获取ip信息 包含ipv4和ipv6
                        foreach (UnicastIPAddressInformation ipAddressInformation in ipInterfaceProperties.UnicastAddresses)
                        {
                            if (ipAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)//判断是否符合ipv4
                            {
                                addressIp = ipAddressInformation.Address.ToString();
                            }
                        }
                    }
                }
                return addressIp;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("根据网卡描述获取网卡Ipv4", ex);
                return null;
            }

        }
        /// <summary>
        /// 保留项
        /// </summary>
        public struct Network
        {
            public string Name;
            public string Mac;
            public IPAddress Ip;
        };
        /// <summary>
        /// 保留项
        /// </summary>
        /// <returns></returns>
        public List<Network> GetNetworkInformation()
        {
            //存储集合声明
            List<Network> LNetwork = new List<Network>();

            NetworkInterface[] InterfacesInformation = NetworkInterface.GetAllNetworkInterfaces();//获取网络接口所有信息 包含未启用的网卡

            foreach (NetworkInterface networkInterface in InterfacesInformation)//遍历所有网络接口信息
            {
                bool bIsOpen = networkInterface.OperationalStatus == OperationalStatus.Up;//判断网络连接状态
                bool bIsLoopback = networkInterface.NetworkInterfaceType == NetworkInterfaceType.Loopback;//是否是 回环
                //Loopback       回环127.0.0.1
                //Wireless80211  无线网 
                //Ethernet       以太网
                if (bIsOpen && !bIsLoopback)
                {
                    //单个网卡声明
                    Network network = new Network
                    {

                        //获取网卡名
                        Name = networkInterface.Name
                    };

                    //获取ip
                    IPInterfaceProperties ipInterfaceProperties = networkInterface.GetIPProperties();//获取ip信息 包含ipv4和ipv6
                    foreach (UnicastIPAddressInformation ipAddressInformation in ipInterfaceProperties.UnicastAddresses)
                    {
                        if (ipAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)//判断是否符合ipv4
                        {
                            network.Ip = ipAddressInformation.Address;
                        }
                    }

                    //获取MAC
                    PhysicalAddress mac = networkInterface.GetPhysicalAddress();
                    string result = Regex.Replace(mac.ToString(), ".{2}", "$0:");//添加字符 .{插入位置} ${数组位置 默认0}{插入字符}
                    string strMac = result.Remove(result.Length - 1);
                    network.Mac = strMac;

                    LNetwork.Add(network);
                }
            }

            return LNetwork;
        }
        /// <summary>
        /// 检查某个Ip地址是否可以ping通
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static Boolean CheckIpAddressPing(string host)
        {
            try
            {
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(host, 120); //ip地址，超时时间
                if (pingReply.Status == IPStatus.Success)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {

                return false;
            }
        }
    }
}

