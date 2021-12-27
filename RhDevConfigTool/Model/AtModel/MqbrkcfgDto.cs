/**
* 命名空间: RhDevConfigTool.Model.AtModel
*
* 功 能： N/A
* 类 名：MqbrkcfgDto.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/3/17 11:31:01  彭政亮 初版
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
    /// MQTT Broker 远程地址及端口号 
    /// </summary>
    public class MqbrkcfgDto
    {
        /// <summary>
        /// MQTT 客户端标识符。
        ///整型,范围: 0~5。
        /// </summary>
        public string Num { get; set; }
        /// <summary>
        /// MQTT Broker 服务器地址
        ///字符串类型,最大长度:100 字节
        /// </summary>
        public string IpOrUrl { get; set; }
        /// <summary>
        /// MQTT Broker 服务器端口
        ///整型,范围:1~65535
        /// </summary>
        public string Port { get; set; }
        /// <summary>
        /// MQTT 客户端 ID
        ///字符串类型。最大长度:50 字节
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// MQTT Broker 用户名
        ///字符串类型。最大长度:50 字节
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// MQTT Broker 密码
        ///字符串类型。最大长度:50 字节
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 组成的At指令
        /// </summary>
        /// <returns></returns>
        public string GetAtStr()
        {
            return $"AT+MQBRKCFG={Num},{IpOrUrl},{Port},{ClientId},{UserName},{Password}";
        }
        //发送At指令
        public Boolean SendAt(string localip, string remoteip, int port)
        {
            if (Num.Equals("0") && IpOrUrl.Equals("0") && Port.Equals("0") && ClientId.Equals("0") && UserName.Equals("0") && Password.Equals("0"))
                return false;
            UdpUtils.SendOneMsg(localip, remoteip, GetAtStr(), port);
            return true;
        }
        public Boolean SendAtSerialPort()
        {
            if (Num.Equals("0") && IpOrUrl.Equals("0") && Port.Equals("0") && ClientId.Equals("0") && UserName.Equals("0") && Password.Equals("0"))
                return false;
            MainViewModel.serialPort.WriteLine(GetAtStr());
            return true;
        }
        public void SendQueryAtSerialPort()
        {
            MainViewModel.serialPort.WriteLine("AT+MQBRKCFG?");
        }
        /// <summary>
        /// 组成的At指令查询
        /// </summary>
        /// <returns></returns>
        public string GetAtStrQuery()
        {
            return "AT+MQBRKCFG?\r\n";
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
