/**
* 命名空间: RhDevConfigTool.Model.AtModel
*
* 功 能： N/A
* 类 名：SockcfgDto.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/3/17 11:04:17  彭政亮 初版
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
    /// 网络套接字设置Dto
    /// </summary>
    public class SockcfgDto
    {
        /// <summary>
        /// 网卡名称:
        ///字符串类型,最长 8 字节
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Socket 编号
        ///整型,范围:0-5
        /// </summary>
        public string Num { get; set; }
        /// <summary>
        /// 使能位
        ///0 禁能(default)
        ///1 使能
        /// </summary>
        public string En { get; set; }
        /// <summary>
        /// Socket 协议类型
        ///0.TCPServer
        ///1.TCPClient
        /// </summary>
        public string Mode { get; set; }
        /// <summary>
        /// TCP/UDP 本地端口
        ///整型,范围:500-65535
        /// </summary>
        public string LocalPort { get; set; }
        /// <summary>
        /// TCP/UDP 远程服务器 IP 地址
        ///字符串类型，最长 15 字节
        /// </summary>
        public string RemoteIp { get; set; }
        /// <summary>
        /// TCP/UDP 远程服务器端口
        ///整型,范围:500-65535
        /// </summary>
        public string RemotePort { get; set; }
        /// <summary>
        /// Server 最长连接数
        ///整型,范围:0-10
        /// </summary>
        public string Maxcon { get; set; }
        /// <summary>
        /// Socket 缓冲区长度
        ///整型,范围:100~65536
        /// </summary>
        public string Packagelen { get; set; }
        /// <summary>
        /// 组成的At指令
        /// </summary>
        /// <returns></returns>
        public string GetAtStr()
        {
            return $"AT+SOCKCFG={Name},{Num},{En},{Mode},{LocalPort},{RemoteIp},{RemotePort},{Maxcon},{Packagelen}";
        }
        //发送At指令
        public Boolean SendAt(string localip, string remoteip, int port)
        {
            if (Name.Equals("0") && Num.Equals("0") && En.Equals("0") && Mode.Equals("0") && LocalPort.Equals("0") && RemoteIp.Equals("0") && RemotePort.Equals("1987") && Maxcon.Equals("1") && Packagelen.Equals("1024"))
                return false;
            UdpUtils.SendOneMsg(localip, remoteip, GetAtStr(), port);
                return true;
        }
        public Boolean SendAtSerialPort()
        {
            if (Name.Equals("0") && Num.Equals("0") && En.Equals("0") && Mode.Equals("0") && LocalPort.Equals("0") && RemoteIp.Equals("0") && RemotePort.Equals("1987") && Maxcon.Equals("1") && Packagelen.Equals("1024"))
                return false;
            MainViewModel.serialPort.WriteLine(GetAtStr());
            return true;
        }
        public void SendQueryAtSerialPort()
        {
            MainViewModel.serialPort.WriteLine("AT+SOCKCFG?");
        }
        /// <summary>
        /// 组成的At指令查询
        /// </summary>
        /// <returns></returns>
        public string GetAtStrQuery()
        {
            return "AT+SOCKCFG?\r\n";
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
