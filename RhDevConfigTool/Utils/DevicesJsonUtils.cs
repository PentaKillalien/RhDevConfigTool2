/**
* 命名空间: RhDevConfigTool.Utils
*
* 功 能： N/A
* 类 名：DevicesJsonUtils.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/3/10 16:25:14  彭政亮 初版
*
* Copyright (c) 2020 Lir Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：宁波瑞辉智能科技有限公司 　　　　　 　　　　　　　    　│
*└──────────────────────────────────┘
*/
using Newtonsoft.Json;
using RhDevConfigTool.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RhDevConfigTool.Utils
{
    /// <summary>
    /// 加载DevicesList Json 文件
    /// </summary>
    public class DevicesJsonUtils
    {
        private static readonly string ConfigPath = $"{AppDomain.CurrentDomain.BaseDirectory}" + @"ConfigFile\DevicesList.json";//发布路径;
        /// <summary>
        /// 读取DevicesList.Json启动配置文件,返回list
        /// </summary>
        public static List<DeviceConfigDto> ReadConfig()
        {
            try
            {
                List<DeviceConfigDto> aa = new List<DeviceConfigDto>();
                //获取配置文件路径
                string jsonstr = File.ReadAllText(ConfigPath, Encoding.Default);
                aa = JsonConvert.DeserializeObject<List<DeviceConfigDto>>(jsonstr);
                return aa;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("JsonList.json启动配置文件失败", ex);
                return null;
            }
        }
        /// <summary>
        /// 根据设备名读取DevicesList.Json启动配置文件,返回需要显示的控件list
        /// </summary>
        public static List<string> ReadConfigHide(string DeviceName)
        {
            try
            {
                List<DeviceConfigDto> aa = new List<DeviceConfigDto>();
                List<string> bb = new List<string>();
                //获取配置文件路径
                string jsonstr = File.ReadAllText(ConfigPath, Encoding.Default);
                aa = JsonConvert.DeserializeObject<List<DeviceConfigDto>>(jsonstr);
                foreach (var item in aa)
                {
                    if (item.DeviceName.Equals(DeviceName))
                    {
                        foreach (var item2 in item.DeviceConfigUnits)
                        {
                            foreach (var item3 in item2.NeedConfigParamerter)
                            {
                                bb.Add(item3);
                            }

                        }
                    }

                }
                return bb;
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("JsonList.json启动配置文件失败", ex);
                return null;
            }
        }
    }
}
