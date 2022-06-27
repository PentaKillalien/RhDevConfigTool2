using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDevConfigTool.ViewModel
{
    /// <summary>
    /// Udp配置
    /// </summary>
    public class UdpConfigViewModel : ViewModelBase
    {
        private string test = "33333";
        /// <summary>
        /// 
        /// </summary>
        public string Test
        {
            get { return test; }
            set
            {
                test = value;
                this.RaisePropertyChanged(nameof(Test));
            }
        }

    }
}
