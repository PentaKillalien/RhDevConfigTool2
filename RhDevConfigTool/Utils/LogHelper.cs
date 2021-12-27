/**
* 命名空间: RhDevConfigTool.Utils
*
* 功 能： N/A
* 类 名：LogHelper.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/3/9 9:43:36  彭政亮 初版
*
* Copyright (c) 2020 Lir Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：宁波瑞辉智能科技有限公司 　　　　　 　　　　　　　    　│
*└──────────────────────────────────┘
*/
using System;

namespace RhDevConfigTool.Utils
{
    public class LogHelper
    {
        public static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("InfoLog");//这里的 InfoLog 和 log4net.config 里的名字要一样
        public static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("ErrorLog");//这里的 InfoLog 和 log4net.config 里的名字要一样
        public static void WriteLog(string info)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(info);
            }
        }

        public static void WriteLog(string info, Exception ex)
        {
            if (logerror.IsErrorEnabled)
            {
                logerror.Error(info, ex);

            }
        }
    }

}
