/*
* 命名空间: RhDevConfigTool.Model.AtModel
*
* 功 能： N/A
* 类 名：AdbtncfgDto.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/4/20 15:44:37  彭政亮 初版
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
    /// 安灯按钮参数设置
    /// </summary>
    public class AdbtncfgDto
    {
        /// <summary>
        /// 按钮编号： 
        /// 整型，范围 0-6 
        /// </summary>
        public string Num { get; set; }
        /// <summary>
        /// 按钮灯运行模式 
        ///0. 状态锁定模式/手动复位模式
        ///（按钮按下后变更状态，且不
        ///自动恢复） 
        ///1. 点动模式，自动复位模式（按
        ///下后灯亮，松开后灯灭） 
        ///2. 延时模式，延时复位（按钮按
        ///下后延时一定时间后执行灯
        ///状态变更） 
        /// </summary>
        public string LedMode { get; set; }
        /// <summary>
        /// Ledmode 的延时时间: 
        /// 整形，范围 0-65535
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        /// 组成的At指令配置
        /// </summary>
        /// <returns></returns>
        public string GetAtStr()
        {
            return $"AT+ADBTNCFG={Num},{LedMode},{Time}";
        }
        /// <summary>
        /// 组成的At指令查询
        /// </summary>
        /// <returns></returns>
        public string GetAtStrQuery()
        {
            return "AT+ADBTNCFG?\r\n";
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
        /// <summary>
        /// 发送At配置指令
        /// </summary>
        /// <param name="localip"></param>
        /// <param name="remoteip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public Boolean SendAt(string localip, string remoteip, int port)
        {
            if (Num.Equals("0") && LedMode.Equals("0") && Time.Equals("0"))
                return false;
            UdpUtils.SendOneMsg(localip, remoteip, GetAtStr(), port);
                return true;
        }

        public Boolean SendAtSerialPort()
        {
            if (Num.Equals("0") && LedMode.Equals("0") && Time.Equals("0"))
                return false;
            MainViewModel.serialPort.Write(GetAtStr()) ;
                return true;
        }

        public void SendQueryAtSerialPort() {
            MainViewModel.serialPort.WriteLine("AT+ADBTNCFG?");
        }
    }
}
