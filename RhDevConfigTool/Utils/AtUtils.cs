/*
* 命名空间: RhDevConfigTool.Utils
*
* 功 能： N/A
* 类 名：AtUtils.cs
*
* Ver  变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2021/4/23 10:42:22  彭政亮 初版
*
* Copyright (c) 2021 Lir Corporation. All rights reserved.
*┌──────────────────────────────────┐
*│　此技术信息为本公司机密信息，未经本公司书面同意禁止向第三方披露．　│
*│　版权所有：宁波瑞辉智能科技有限公司 　　　　　 　　　　　　　    　│
*└──────────────────────────────────┘
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDevConfigTool.Utils
{
    public class AtUtils
    {

        /// <summary>
        /// At指令解析
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> AtAnaysis(string str)
        {
            List<string> temp = new List<string>();
            int start = str.IndexOf(":") + 1;
            string s = str.Substring(start, str.Length - start);
            string[] arr = s.Split(',');
            foreach (var item in arr)
            {
                try
                {
                    temp.Add(item);
                }
                catch (Exception)
                {
                    LogHelper.WriteLog("接收数据格式错误") ;
                }
               
            }
            return temp;
        }
    }
}
