/**
* 命名空间: RhDevConfigTool.Model.AtModel
*
* 功 能： N/A
* 类 名：WapcfgDto.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/3/17 10:47:48  彭政亮 初版
*
* Copyright (c) 2020 Lir Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：宁波瑞辉智能科技有限公司 　　　　　 　　　　　　　    　│
*└──────────────────────────────────┘
*/
using RhDevConfigTool.Utils;
using RhDevConfigTool.ViewModel;
using System;

namespace RhDevConfigTool.Model.AtModel
{
    /// <summary>
    /// 无线ApDto
    /// </summary>
    public class WapcfgDto
    {
        /// <summary>
        /// 网卡名称 字符串类型,最长 8 字节
        /// </summary>
        public string EthName { get; set; }
        /// <summary>
        /// 使能位 0 禁能 (default) 1 使能
        /// </summary>
        public string En { get; set; }
        /// <summary>
        /// 网络安全类型  0.WPA2-PSK 1.WAP2
        /// </summary>
        public string Encry { get; set; }
        /// <summary>
        /// 无线 AP 账号：
        ///字符串类型,最长 16Byte
        /// </summary>
        public string Ssid { get; set; }
        /// <summary>
        /// 无线 AP 密码：
        ///字符串类型,最长 16Byte
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// dhcp 起始地址:
        ///整型,范围:1-253
        /// </summary>
        public string DhcpStart { get; set; }
        /// <summary>
        /// dhcp 终止地址:
        ///整型,范围: (2~254)
        /// </summary>
        public string DhcpStop { get; set; }
        /// <summary>
        /// 组成配置的At指令
        /// </summary>
        /// <returns></returns>
        public string GetAtStr()
        {
            return $"AT+WAPCFG={EthName},{En},{Encry},{DhcpStart},{DhcpStop},{Ssid},{Password}";
        }
        //发送At指令
        public Boolean SendAt(string localip, string remoteip, int port)
        {
            if (EthName.Equals("br0lan") && En.Equals("0") && Encry.Equals("0") && DhcpStart.Equals("0") && DhcpStop.Equals("0") && Ssid.Equals("0") && Password.Equals("0"))
                return false;
            UdpUtils.SendOneMsg(localip, remoteip, GetAtStr(), port);
                return true;
        }
        public Boolean SendAtSerialPort()
        {
            if (EthName.Equals("0-lan") && En.Equals("0") && Encry.Equals("0") && DhcpStart.Equals("0") && DhcpStop.Equals("0") && Ssid.Equals("0") && Password.Equals("0"))
                return false;
            MainViewModel.serialPort.WriteLine(GetAtStr());
            return true;
        }
        public void SendQueryAtSerialPort()
        {
            MainViewModel.serialPort.WriteLine("AT+WAPCFG?");
        }
        /// <summary>
        /// 组成的At指令查询
        /// </summary>
        /// <returns></returns>
        public string GetAtStrQuery()
        {
            return "AT+WAPCFG?\r\n";
        }
        /// <summary>
        /// 发送At配置指令
        /// </summary>
        /// <param name="localip"></param>
        /// <param name="remoteip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public Boolean SendAtQuery(string localip, string remoteip, int port)
        {
            UdpUtils.SendOneMsg(localip, remoteip, GetAtStrQuery(), port);
            return true;
        }
    }
}
