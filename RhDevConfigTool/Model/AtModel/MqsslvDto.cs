﻿/**
* 命名空间: RhDevConfigTool.Model.AtModel
*
* 功 能： N/A
* 类 名：MqsslvDto.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/3/17 11:35:07  彭政亮 初版
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
    /// MQTT SSL 通讯版本
    /// </summary>
    public class MqsslvDto
    {
        /// <summary>
        /// 使能 SSL 功能
        ///0.禁止 SSL 功能
        ///1.使能 SSL 功能
        /// </summary>
        public string En { get; set; }
        /// <summary>
        /// ver::SSL 协议版本:
        /// float 型，最长 8 字节
        /// </summary>
        public string Ver { get; set; }
        /// <summary>
        /// 组成的At指令
        /// </summary>
        /// <returns></returns>
        public string GetAtStr()
        {
            return $"AT+MQSSLV={En},{Ver}";
        }
        //发送At指令
        public Boolean SendAt(string localip, string remoteip, int port)
        {
            if (En.Equals("0") && Ver.Equals("0"))
                return false;
            UdpUtils.SendOneMsg(localip, remoteip, GetAtStr(), port);
            return true;
        }
        public Boolean SendAtSerialPort()
        {
            if (En.Equals("0") && Ver.Equals("0"))
                return false;
            MainViewModel.serialPort.WriteLine(GetAtStr());
            return true;
        }
        public void SendQueryAtSerialPort()
        {
            MainViewModel.serialPort.WriteLine("AT+MQSSLV?");
        }
        /// <summary>
        /// 组成的At指令查询
        /// </summary>
        /// <returns></returns>
        public string GetAtStrQuery()
        {
            return "AT+MQSSLV?\r\n";
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
