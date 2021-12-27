using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RhDevConfigTool.ViewModel
{
    /// <summary>
    /// 用户选择设备界面
    /// </summary>
    public class UserSelectViewModel : ViewModelBase
    {
        public UserSelectViewModel()
        {
        }
        
        /// <summary>
        /// 单机Cpt300按钮
        /// </summary>
        public RelayCommand<Window> TopDeviceCpt300 => new RelayCommand<Window>((win) =>
        {
            if (win is System.Windows.Window)
            {
                new MainWindow().Show();
                (win as System.Windows.Window).Close();
            }
            Messenger.Default.Send<string>("Cpt300", "DeviceName");
        });

        /// <summary>
        /// 单机安灯按钮
        /// </summary>
        public RelayCommand<Window> TopDeviceADbutton => new RelayCommand<Window>((win) =>
        {
            if (win is System.Windows.Window)
            {
                new MainWindow().Show();
                (win as System.Windows.Window).Close();
            }
            Messenger.Default.Send<string>("ADbutton", "DeviceName");
        });

        /// <summary>
        /// 单机NbIot按钮
        /// </summary>
        public RelayCommand<Window> TopDeviceNbIot => new RelayCommand<Window>((win) =>
        {
            if (win is System.Windows.Window)
            {
                new MainWindow().Show();
                (win as System.Windows.Window).Close();
            }
            Messenger.Default.Send<string>("Cat1-Link", "DeviceName");
        });

        /// <summary>
        /// 打开sensor 计数
        /// </summary>
        public RelayCommand<Window> OpenSensorWindow => new RelayCommand<Window>((win) =>
        {
            if (win is System.Windows.Window)
            {
                new SensorView().Show();
                (win as System.Windows.Window).Close();
            }
        });



    }
}
