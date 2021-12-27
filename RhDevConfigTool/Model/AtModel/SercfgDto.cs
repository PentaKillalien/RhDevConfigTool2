/**
* 命名空间: RhDevConfigTool.Model.AtModel
*
* 功 能： N/A
* 类 名：SercfgDto.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/3/17 11:11:13  彭政亮 初版
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
    /// 串口波特率配置Dto
    /// </summary>
    public class SercfgDto
    {
        /// <summary>
        /// 串口编号：
        ///整型,范围:0-5
        /// </summary>
        public string Comx { get; set; }
        /// <summary>
        /// 串口波特率
        ///整型,范围:1200-230400
        /// </summary>
        public string Baud { get; set; }
        /// <summary>
        /// 串口数据为，目前只支持 8
        /// 整型,范围:8
        /// </summary>
        public string Databit { get; set; }
        /// <summary>
        /// 串口停止位 ，目前只支持 1
        /// 整型,范围:1
        /// </summary>
        public string Stopbit { get; set; }
        /// <summary>
        /// 串口校验位
        ///0：无校验
        ///1：奇校验
        ///2：偶校验
        /// </summary>
        public string Parity { get; set; }
        /// <summary>
        /// 串口硬件流控制位
        ///0：禁能
        ///1：使能
        /// </summary>
        public string Flowctrls { get; set; }
        /// <summary>
        /// 缓冲区长度
        /// </summary>
        public string PackageLen { get; set; }
        /// <summary>
        /// 组成的At指令
        /// </summary>
        /// <returns></returns>
        public string GetAtStr()
        {
            return $"AT+SERCFG={Comx},{Baud},{Databit},{Stopbit},{Parity},{Flowctrls},{PackageLen}";
        }
        //发送At指令
        public Boolean SendAt(string localip, string remoteip, int port)
        {
            if (Comx.Equals("0")&& Baud.Equals("0")&&Databit.Equals("0")&&Stopbit.Equals("0")&&Parity.Equals("0")&&Flowctrls.Equals("0")&&PackageLen.Equals("0"))
                return false;
            UdpUtils.SendOneMsg(localip, remoteip, GetAtStr(), port);
                return true;
        }
        public Boolean SendAtSerialPort()
        {
            if (Comx.Equals("0") && Baud.Equals("0") && Databit.Equals("0") && Stopbit.Equals("0") && Parity.Equals("0") && Flowctrls.Equals("0") && PackageLen.Equals("0"))
                return false;
            MainViewModel.serialPort.WriteLine(GetAtStr());
                return true;
        }
        public void SendQueryAtSerialPort()
        {
            MainViewModel.serialPort.WriteLine("AT+SERCFG?");
        }
        /// <summary>
        /// 组成的At指令查询
        /// </summary>
        /// <returns></returns>
        public string GetAtStrQuery()
        {
            return "AT+SERCFG?\r\n";
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
