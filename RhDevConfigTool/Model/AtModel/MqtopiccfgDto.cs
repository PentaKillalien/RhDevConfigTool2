/**
* 命名空间: RhDevConfigTool.Model.AtModel
*
* 功 能： N/A
* 类 名：MqtopiccfgDto.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/3/17 11:40:01  彭政亮 初版
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
    /// MQTT 发布消息主题设定 
    /// </summary>
    public class MqtopiccfgDto
    {
        /// <summary>
        /// 消息主题编号 
        ///整型，范围 0-5 
        /// </summary>
        public string Num { get; set; }
        /// <summary>
        /// MQTT 发布主题内容 
        ///字符串，最大长度 100 
        /// </summary>
        public string Pbtopic { get; set; }
        /// <summary>
        /// MQTT 订阅主题内容 
        ///字符串，最大长度 100
        /// </summary>
        public string Sbtopic { get; set; }
        /// <summary>
        /// 返回配置At字符串
        /// </summary>
        /// <returns></returns>
        public string GetAtStr()
        {
            return $"AT+MQTOPICCFG={Num},{Pbtopic},{Sbtopic}";
        }
        //发送At指令
        public Boolean SendAt(string localip, string remoteip, int port)
        {
            if (Num.Equals("0") && Pbtopic.Equals("0") && Sbtopic.Equals("0"))
                return false;
            UdpUtils.SendOneMsg(localip, remoteip, GetAtStr(), port);
                return true;
        }
        public Boolean SendAtSerialPort()
        {
            if (Num.Equals("0") && Pbtopic.Equals("0") && Sbtopic.Equals("0"))
                return false;
            MainViewModel.serialPort.WriteLine(GetAtStr());
            return true;
        }
        public void SendQueryAtSerialPort()
        {
            MainViewModel.serialPort.WriteLine("AT+MQTOPICCFG?");
        }
        /// <summary>
        /// 组成的At指令查询
        /// </summary>
        /// <returns></returns>
        public string GetAtStrQuery()
        {
            return "AT+MQTOPICCFG?\r\n";
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
