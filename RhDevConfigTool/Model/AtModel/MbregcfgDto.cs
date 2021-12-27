/**
* 命名空间: RhDevConfigTool.Model.AtModel
*
* 功 能： N/A
* 类 名：MbregcfgDto.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/3/17 11:23:28  彭政亮 初版
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
    /// Modbus寄存器配置
    /// </summary>
    public class MbregcfgDto
    {
        /// <summary>
        /// Modbus 编号
        ///整型,范围:0-5
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// Modbus 线圈寄存器地址
        ///整型,范围:0-9999
        /// </summary>
        public string Coiladr { get; set; }
        /// <summary>
        /// Modbus 线圈寄存器数量
        ///整型,范围:0-9999
        /// </summary>
        public string Coilcnt { get; set; }
        /// <summary>
        /// Modbus 输入离散量寄存器地址
        ///整型,范围:10000-19999
        /// </summary>
        public string Indesadr { get; set; }
        /// <summary>
        /// Modbus 输入离散量寄存器数量
        ///整型,范围:0-9999
        /// </summary>
        public string Indescnt { get; set; }
        /// <summary>
        /// Modbus 保持寄存器地址
        ///整型,范围:40000-49999
        /// </summary>
        public string Hdregadr { get; set; }
        /// <summary>
        /// Modbus 保持寄存器数量
        ///整型,范围: 0-9999
        /// </summary>
        public string Hdregcnt { get; set; }
        /// <summary>
        /// Modbus 输入寄存器地址
        ///整型,范围:30000-39999
        /// </summary>
        public string Inregadr { get; set; }
        /// <summary>
        /// Modbus 输入寄存器数量
        /// 整型,范围: 0-9999
        /// </summary>
        public string Inregcnt { get; set; }
        /// <summary>
        /// 组成的At指令
        /// </summary>
        /// <returns></returns>
        public string GetAtStr()
        {
            return $"AT+MBREGCFG={Number},{Coiladr},{Coilcnt},{Indesadr},{Indescnt},{Hdregadr},{Hdregcnt},{Inregadr},{Inregcnt}";
        }
        //发送At指令
        public Boolean SendAt(string localip, string remoteip, int port)
        {
            if (Number.Equals("0") && Coiladr.Equals("0") && Coilcnt.Equals("0") && Indesadr.Equals("0") && Indescnt.Equals("0") && Hdregadr.Equals("0") && Hdregcnt.Equals("16") && Inregadr.Equals("30000") && Inregcnt.Equals("24"))
                return false;
            UdpUtils.SendOneMsg(localip, remoteip, GetAtStr(), port);
            return true;
        }
        public Boolean SendAtSerialPort()
        {
            if (Number.Equals("0") && Coiladr.Equals("0") && Coilcnt.Equals("0") && Indesadr.Equals("0") && Indescnt.Equals("0") && Hdregadr.Equals("0") && Hdregcnt.Equals("16") && Inregadr.Equals("30000") && Inregcnt.Equals("24"))
                return false;
            MainViewModel.serialPort.WriteLine(GetAtStr());
            return true;
        }
        public void SendQueryAtSerialPort()
        {
            MainViewModel.serialPort.WriteLine("AT+MBREGCFG?");
        }
        /// <summary>
        /// 组成的At指令查询
        /// </summary>
        /// <returns></returns>
        public string GetAtStrQuery()
        {
            return "AT+MBREGCFG?\r\n";
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
