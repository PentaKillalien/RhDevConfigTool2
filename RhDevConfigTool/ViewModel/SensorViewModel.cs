using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RhDevConfigTool.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDevConfigTool.ViewModel
{
    public class SensorViewModel : ViewModelBase
    {
        Cpt300Driver driver = null;
        private int sensorValue = 0;
        /// <summary>
        /// 传感器计数
        /// </summary>
        public int SensorValue
        {
            get { return sensorValue; }
            set
            {
                sensorValue = value;
                this.RaisePropertyChanged(nameof(SensorValue));
            }
        }
        private Boolean inputEnabled = true;
        /// <summary>
        /// 输入使能
        /// </summary>
        public Boolean InputEnabled
        {
            get { return inputEnabled; }
            set
            {
                inputEnabled = value;
                this.RaisePropertyChanged(nameof(InputEnabled));
            }
        }
        private string inputIo = "";
        /// <summary>
        /// 输入选择的IO 1 2 3 4 
        /// </summary>
        public string InputIo
        {
            get { return inputIo; }
            set
            {
                inputIo = value;
                this.RaisePropertyChanged(nameof(InputIo));
            }
        }



        /// <summary>
        /// 清零
        /// </summary>
        public RelayCommand ClearValue => new RelayCommand(() =>
        {
            SensorValue = 0;
        });



        /// <summary>
        /// 启动连接
        /// </summary>
        public RelayCommand StartConn => new RelayCommand(() =>
        {
            InputEnabled = false;
            //创建Socket
            driver = new Cpt300Driver();
            driver.Connect();
            driver.Cpt300Gather = new Action<string, bool>(Cpt300Gather);

        });

        private void Cpt300Gather(string ip,bool model) {
            SensorValue++;
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        public RelayCommand CloseConn => new RelayCommand(() =>
        {
            InputEnabled = true;
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                //释放连接
                if (driver != null)
                {
                    driver.DisConnect();
                }
            }));
            
            
        });




    }
}
