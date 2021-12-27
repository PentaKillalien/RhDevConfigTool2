using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDevConfigTool.Model
{
    public class ZjShCpt300Dto
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 上一计数
        /// </summary>
        public int PreCount { get; set; }
        /// <summary>
        /// 当前计数
        /// </summary>
        public int CurrentCount { get; set; }
    }
}
