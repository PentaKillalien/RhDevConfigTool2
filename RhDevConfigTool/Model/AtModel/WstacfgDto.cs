/**
* 命名空间: RhDevConfigTool.Model.AtModel
*
* 功 能： N/A
* 类 名：Wstacfg.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/3/17 10:54:12  彭政亮 初版
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
    /// 无线STA设置Dto
    /// </summary>
    public class WstacfgDto
    {

        /// <summary>
        /// 网卡名称 字符串类型,最长 8Byte
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 使能位 0 禁能 (default)  1 使能
        /// </summary>
        public string En { get; set; }
        /// <summary>
        /// 网络安全类型 0.WPA2-PSK 1.WAP2
        /// </summary>
        public string Encry { get; set; }
        /// <summary> 
        ///  Dhcp 功能使能 0 禁能 (default) 1 使能
        /// </summary>
        public string Dhcp { get; set; }
        /// <summary>
        /// 无线 AP 账号 字符串类型,最长 16Byte
        /// </summary>
        public string Ssid { get; set; }
        /// <summary>
        /// 无线 AP 密码 字符串类型,最长 16Byte
        /// </summary>
        public string Passwd { get; set; }
        /// <summary>
        /// 组成的At指令
        /// </summary>
        /// <returns></returns>
        public string GetAtStr()
        {
            return $"AT+WSTACFG={Name},{En},{Encry},{Dhcp},{Ssid},{Passwd}";
        }
        //发送At指令
        public Boolean SendAt(string localip, string remoteip, int port)
        {
            if (Name.Equals("0") && En.Equals("0") && Encry.Equals("0") && Dhcp.Equals("0") && Ssid.Equals("0") && Passwd.Equals("0"))
                return false;
            UdpUtils.SendOneMsg(localip, remoteip, GetAtStr(), port);
            return true;
        }
        public Boolean SendAtSerialPort()
        {
            if (Name.Equals("0") && En.Equals("0") && Encry.Equals("0") && Dhcp.Equals("0") && Ssid.Equals("0") && Passwd.Equals("0"))
                return false;
            MainViewModel.serialPort.WriteLine(GetAtStr());
            return true;
        }
        public void SendQueryAtSerialPort()
        {
            MainViewModel.serialPort.WriteLine("AT+WSTACFG?");
        }
        /// <summary>
        /// 组成的At指令查询
        /// </summary>
        /// <returns></returns>
        public string GetAtStrQuery()
        {
            return "AT+WSTACFG?\r\n";
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
