/**
* 命名空间: RhDevConfigTool.Model.AtModel
*
* 功 能： N/A
* 类 名：MbcfgDto.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/3/17 11:16:54  彭政亮 初版
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
    /// Modbus地址Dto
    /// </summary>
    public class MbcfgDto
    {
        /// <summary>
        /// Modbus 编号:
        ///整型,范围:0-5
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// Modbus 使能位
        ///0. 禁能
        ///1. 使能
        /// </summary>
        public string En { get; set; }
        /// <summary>
        /// Modbus 地址
        ///整型,范围:1-254
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Modbus 主从模式
        ///0. Slave
        ///1. Master
        /// </summary>
        public string Ms { get; set; }
        /// <summary>
        /// Modbus 模式
        ///0. ModbusRtu
        ///1. ModbusTcp
        /// </summary>
        public string Mode { get; set; }
        /// <summary>
        /// Modbus 扫描时间。
        ///整型,范围:100-50000
        ///单位 ms
        /// </summary>
        public string Scantime { get; set; }
        /// <summary>
        /// 组成的At指令
        /// </summary>
        /// <returns></returns>
        public string GetAtStr()
        {
            return $"AT+MBCFG={Number},{En},{Id},{Ms},{Mode},{Scantime}";
        }
        //发送At指令
        public Boolean SendAt(string localip, string remoteip, int port)
        {
            if (Number.Equals("0") && En.Equals("0") && Id.Equals("0") && Ms.Equals("0") && Mode.Equals("0") && Scantime.Equals("0"))
                return false;
            UdpUtils.SendOneMsg(localip, remoteip, GetAtStr(), port);
                return true;
        }
        public Boolean SendAtSerialPort()
        {
            if (Number.Equals("0") && En.Equals("0") && Id.Equals("100") && Ms.Equals("0") && Mode.Equals("0") && Scantime.Equals("0"))
                return false;
            MainViewModel.serialPort.WriteLine(GetAtStr());
            return true;
        }
        public void SendQueryAtSerialPort()
        {
            MainViewModel.serialPort.WriteLine("AT+MBCFG?");
        }
        /// <summary>
        /// 组成的At指令查询
        /// </summary>
        /// <returns></returns>
        public string GetAtStrQuery()
        {
            return "AT+MBCFG?\r\n";
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
