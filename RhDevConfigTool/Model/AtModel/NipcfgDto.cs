/**
* 命名空间: RhDevConfigTool.Model.AtModel
*
* 功 能： N/A
* 类 名：NipcfgDto.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/3/17 11:00:21  彭政亮 初版
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
    /// 网卡IP地址Dto
    /// </summary>
    public class NipcfgDto
    {
        /// <summary>
        /// 网卡名称:
        ///字符串类型,最长 8 字节
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 网络模式：
        ///0:LAN
        ///1:WAN
        /// </summary>
        public string Mode { get; set; }
        /// <summary>
        /// ip 地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// netmask 地址
        /// </summary>
        public string Mask { get; set; }
        /// <summary>
        /// gateway 地址
        /// </summary>
        public string GateWay { get; set; }
        /// <summary>
        /// DNS 地址
        /// </summary>
        public string Dns { get; set; }
        /// <summary>
        /// MAC 地址
        /// </summary>
        public string Mac { get; set; }
        /// <summary>
        /// 组成的At指令
        /// </summary>
        /// <returns></returns>
        public string GetAtStr()
        {
            return $"AT+ENETCFG={Name},{Mode},{Ip},{Mask},{GateWay},{Dns},{Mac}";
        }
        //发送At指令
        public Boolean SendAt(string localip, string remoteip, int port)
        {
            if (Name.Equals("0")&&Mode.Equals("0")  && Ip.Equals("0") && Mask.Equals("0") && GateWay.Equals("0") && Dns.Equals("0"))
                return false;
            UdpUtils.SendOneMsg(localip, remoteip, GetAtStr(), port);
                return true;
        }
        public Boolean SendAtSerialPort()
        {
            if (Name.Equals("0") && Mode.Equals("0") && Ip.Equals("0") && Mask.Equals("0") && GateWay.Equals("0") && Dns.Equals("0"))
                return false;
            MainViewModel.serialPort.WriteLine(GetAtStr());
            return true;
        }
        public void SendQueryAtSerialPort()
        {
            MainViewModel.serialPort.WriteLine("AT+ENETCFG?");
        }
        /// <summary>
        /// 组成的At指令查询
        /// </summary>
        /// <returns></returns>
        public string GetAtStrQuery()
        {
            return "AT+ENETCFG?\r\n";
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
