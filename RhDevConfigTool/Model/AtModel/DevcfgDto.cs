/*
* 命名空间: RhDevConfigTool.Model.AtModel
*
* 功 能： N/A
* 类 名：DevcfgDto.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/4/20 15:35:55  彭政亮 初版
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
    /// 设备参数设置
    /// </summary>
    public class DevcfgDto
    {

        /// <summary>
        /// 设备 ID:
        ///字符串类型,最长 20 字节
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 激活码
        ///字符串类型，最长 20 字节
        ///该参数具有特殊性。
        ///读：返回芯片唯一 ID，
        ///写：根据唯一 ID 生成的激活码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 组成的At指令
        /// </summary>
        /// <returns></returns>
        public string GetAtStr()
        {
            return $"AT+DEVCFG={Id},{Code}";
        }
        //发送At指令
        public Boolean SendAt(string localip, string remoteip, int port)
        {
            if (Id.Equals("0") && Code.Equals("0"))
                return false;
            UdpUtils.SendOneMsg(localip, remoteip, GetAtStr(), port);
            return true;
        }
        public Boolean SendAtSerialPort()
        {
            if (Id.Equals("0") && Code.Equals("0"))
                return false;
            MainViewModel.serialPort.WriteLine(GetAtStr());
                return true;
        }
        public void SendQueryAtSerialPort()
        {
            MainViewModel.serialPort.WriteLine("AT+DEVCFG?");
        }
        /// <summary>
        /// 组成的At指令查询
        /// </summary>
        /// <returns></returns>
        public string GetAtStrQuery()
        {
            return "AT+DEVCFG?\r\n";
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
