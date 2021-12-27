/*
* 命名空间: RhDevConfigTool.Model.AtModel
*
* 功 能： N/A
* 类 名：ColmdcfgDto.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/4/20 15:48:14  彭政亮 初版
*
* Copyright (c) 2021 Lir Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：宁波瑞辉智能科技有限公司 　　　　　 　　　　　　　    　│
*└──────────────────────────────────┘
*/
using RhDevConfigTool.Utils;
using RhDevConfigTool.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDevConfigTool.Model.AtModel
{
    /// <summary>
    /// 采集模式设置
    /// </summary>
    public class ColmdcfgDto
    {
        /// <summary>
        /// 采集器数据上传模式：
        ///整型，范围 0-2
        ///0：基于事件触发模式，当采集器
        ///的采集参数发生变化时上传数据
        ///1：基于定时上传模式。即采集器
        ///在定时时间到后，主动上传一批
        ///数据
        ///2 基于主从查询模式。该模式下事
        ///件触发及定时上传取消。
        ///注：主从查询模式在 0,1 模式下
        ///依旧存在
        /// </summary>
        public string mode { get; set; }
        /// <summary>
        /// 定时器数据监控/触发时间：
        ///100~100000ms
        ///当 mode=0 时。该定时值为两次上
        ///传的最小时间间隔。
        ///当 mode = 1 时，该值为每次主动上
        /// 传的时间间隔。
        /// </summary>
        public string Timer { get; set; }
        /// <summary>
        /// 采集器心跳时间 10~1000 秒
        ///作为采集器空闲时的心跳时间，
        ///心跳时间到后，采集器主动上传
        ///数据到服务器
        /// </summary>
        public string heartTimer { get; set; }
        /// <summary>
        /// 心跳数据
        ///字符串，最大长度 50
        /// </summary>
        public string heartString { get; set; }
        /// <summary>
        /// 组成的At指令
        /// </summary>
        /// <returns></returns>
        public string GetAtStr()
        {
            return $"AT+COLMDCFG={mode},{Timer},{heartTimer},{heartString}";
        }
        //发送At指令
        public Boolean SendAt(string localip, string remoteip, int port)
        {
            if (mode.Equals("0") && Timer.Equals("0") && heartTimer.Equals("0") && heartString.Equals("0"))
                return false;
            UdpUtils.SendOneMsg(localip, remoteip, GetAtStr(), port);
                return true;
        }
        //发送At指令
        public Boolean SendAtSerialPort()
        {
            if (mode.Equals("0") && Timer.Equals("0") && heartTimer.Equals("0") && heartString.Equals("0"))
                return false;
            MainViewModel.serialPort.WriteLine(GetAtStr());
                return true;
        }
        public void SendQueryAtSerialPort() {
            MainViewModel.serialPort.WriteLine("AT+COLMDCFG?");
        }
        /// <summary>
        /// 组成的At指令查询
        /// </summary>
        /// <returns></returns>
        public string GetAtStrQuery()
        {
            return "AT+COLMDCFG?\r\n";
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
