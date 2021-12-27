/**
* 命名空间: RhDevConfigTool.Model
*
* 功 能： N/A
* 类 名：DeviceConfigUnit.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/3/10 16:18:06  彭政亮 初版
*
* Copyright (c) 2020 Lir Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：宁波瑞辉智能科技有限公司 　　　　　 　　　　　　　    　│
*└──────────────────────────────────┘
*/
using System.Collections.Generic;

namespace RhDevConfigTool.Model
{
    /// <summary>
    /// 设备配置单元
    /// </summary>
    public class DeviceConfigUnit
    {
        /// <summary>
        /// 配置类型
        /// </summary>
        public string ConfigType { get; set; }
        /// <summary>
        /// 支持配置的参数列表
        /// </summary>
        public List<string> NeedConfigParamerter { get; set; }

    }
}
