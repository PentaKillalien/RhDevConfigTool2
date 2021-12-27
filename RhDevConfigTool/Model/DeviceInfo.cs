/**
* 命名空间: RhDevConfigTool.Model
*
* 功 能： N/A
* 类 名：DeviceInfo.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/3/9 11:17:06  彭政亮 初版
*
* Copyright (c) 2020 Lir Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：宁波瑞辉智能科技有限公司 　　　　　 　　　　　　　    　│
*└──────────────────────────────────┘
*/

namespace RhDevConfigTool.Model
{
    public class DeviceInfo
    {
        public DeviceInfo(string strName, string strIpAddress, string strMacAddress, string strNetMask, string strGateWay, string strDns, string strLanCard)
        {

            this.strName = strName;
            this.strLanCard = strLanCard;
            this.strIpAddress = strIpAddress;
            this.strMacAddress = strMacAddress;
            this.strNetMask = strNetMask;
            this.strGateWay = strGateWay;
            this.strDns = strDns;
        }
        public string strLanCard { get; set; }
        public string strName { get; set; }

        public string strIpAddress { get; set; }

        public string strNetMask { get; set; }

        public string strGateWay { get; set; }
        public string strMacAddress { get; set; }
        public string strDns { get; set; }

        public override string ToString()
        {
            return strName + "--" + strIpAddress + "--" + strNetMask + "--" + strGateWay + "--" + strMacAddress + "--" + strDns;
        }
    }
}
