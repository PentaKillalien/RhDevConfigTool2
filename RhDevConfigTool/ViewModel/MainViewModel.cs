using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Panuon.UI.Silver;
using RhDevConfigTool.Model;
using RhDevConfigTool.Model.AtModel;
using RhDevConfigTool.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RhDevConfigTool.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// 串口信息
        /// </summary>
        public ObservableCollection<string> SerialPortsName { get; set; }
        /// <summary>
        /// 网卡List
        /// </summary>
        public ObservableCollection<string> NetAdapterList { get; set; }
        /// <summary>
        /// 设备信息List
        /// </summary>
        public ObservableCollection<DeviceInfo> DeviceInfos { get; set; }
        /// <summary>
        /// 瑞辉配置文件标识字段
        /// </summary>
        private const string RHCONFIGFLAG = "RHCONFIGFLAG";
        private const int UdpBoradCastPort = 3691;//向3691进行广播
        private const int UpdReceivePort = 3690;//用3690进行接收
        private const string UdpSendHead = "RH_CPT300_READ_INFO";
        private const string UdpReadHead = "RH_CPT300_READ_INFO";
        private const string UdpReadTail = "RH_CPT300_READ_INFO_BACK##";
        public string LocalIp = string.Empty;
        public string RemoteIp = string.Empty;
        public string DeviceNameCurrent = string.Empty;
        public int ConfigMode = 0;//配置模式,0代表串口,1代表Udp
        #region 待初始化的At配置Dto们
        public MbcfgDto M_MbcfgDto;
        public MbregcfgDto M_Mbregcfg;
        public MqbrkcfgDto M_MqbrkcfgDto;
        public MqheartenDto M_MqheartenDto;
        public MqsslvDto M_MqsslvDto;
        public MqtopiccfgDto M_MqtopiccfgDto;
        public NipcfgDto M_NipcfgDto;
        public SercfgDto M_SercfgDto;
        public SockcfgDto M_SockcfgDto;
        public WapcfgDto M_WapcfgDto;
        public WstacfgDto M_WstacfgDto;
        public DevcfgDto M_DevcfgDto;
        public ColmdcfgDto M_ColmdcfgDto;
        public AdbtncfgDto M_AdbtncfgDto;
        #endregion
        #region 与前端值的绑定
        private string gatherPatternConfig_HeratJumpData = "0";
        /// <summary>
        /// 采集模式设置 心跳数据
        /// </summary>
        public string GatherPatternConfig_HeratJumpData
        {
            get {
                M_ColmdcfgDto.heartString = gatherPatternConfig_HeratJumpData;
                return gatherPatternConfig_HeratJumpData; }
            set
            {
                gatherPatternConfig_HeratJumpData = value;
                M_ColmdcfgDto.heartString = value;
                this.RaisePropertyChanged(nameof(GatherPatternConfig_HeratJumpData));
            }
        }

        private string gatherPatternConfig_HeartTriggerTime = "0";
        /// <summary>
        /// 采集模式设置 心跳触发时间
        /// </summary>
        public string GatherPatternConfig_HeartTriggerTime
        {
            get {
                M_ColmdcfgDto.heartTimer = gatherPatternConfig_HeartTriggerTime;
                return gatherPatternConfig_HeartTriggerTime; }
            set
            {
                gatherPatternConfig_HeartTriggerTime = value;
                M_ColmdcfgDto.heartTimer = value;
                this.RaisePropertyChanged(nameof(GatherPatternConfig_HeartTriggerTime));
            }
        }

        private string gatherPatternConfig_TimerControllerTriggerTime = "0";
        /// <summary>
        /// 采集模式设置 定时器触发时间
        /// </summary>
        public string GatherPatternConfig_TimerControllerTriggerTime
        {
            get {
                M_ColmdcfgDto.Timer = gatherPatternConfig_TimerControllerTriggerTime;
                return gatherPatternConfig_TimerControllerTriggerTime; }
            set
            {
                gatherPatternConfig_TimerControllerTriggerTime = value;
                M_ColmdcfgDto.Timer = value;
                this.RaisePropertyChanged(nameof(GatherPatternConfig_TimerControllerTriggerTime));
            }
        }
        public ObservableCollection<string> GatherPatternConfig_DataUpLoadPattern_Arr { get; set; } = new ObservableCollection<string>() { "0","1","2"};
        private string gatherPatternConfig_DataUpLoadPattern = "0";
        /// <summary>
        /// 采集模式设置 采集数据上传模式
        /// </summary>
        public string GatherPatternConfig_DataUpLoadPattern
        {
            get {
                return gatherPatternConfig_DataUpLoadPattern; }
            set
            {
                gatherPatternConfig_DataUpLoadPattern = value;
                M_ColmdcfgDto.mode = value;
                this.RaisePropertyChanged(nameof(GatherPatternConfig_DataUpLoadPattern));
            }
        }

        private string adButtonParaConfig_LedmodeTimeout = "0";
        /// <summary>
        /// 延时时间
        /// </summary>
        public string AdButtonParaConfig_LedmodeTimeout
        {
            get {
                M_AdbtncfgDto.Time = adButtonParaConfig_LedmodeTimeout;
                return adButtonParaConfig_LedmodeTimeout; }
            set
            {
                adButtonParaConfig_LedmodeTimeout = value;
                M_AdbtncfgDto.Time = value;
                this.RaisePropertyChanged(nameof(AdButtonParaConfig_LedmodeTimeout));
            }
        }
        public ObservableCollection<string> AdButtonParaConfig_ButtonRunPattern_Arr { get; set; } = new ObservableCollection<string>() { "0","1","2"};
        private string adButtonParaConfig_ButtonRunPattern = "0";
        /// <summary>
        /// 安灯按钮参数设置 运行模式
        /// </summary>
        public string AdButtonParaConfig_ButtonRunPattern
        {
            get {
                return adButtonParaConfig_ButtonRunPattern; }
            set
            {
                adButtonParaConfig_ButtonRunPattern = value;
                M_AdbtncfgDto.LedMode = value;
                this.RaisePropertyChanged(nameof(AdButtonParaConfig_ButtonRunPattern));
            }
        }
        public ObservableCollection<string> AdButtonParaConfig_ButtonNum_Arr { get; set; } = new ObservableCollection<string>() { "0", "1", "2", "3", "4", "5", "6" };
        private string adButtonParaConfig_ButtonNum = "0";
        /// <summary>
        /// 安灯按钮参数设置 按钮编号
        /// </summary>
        public string AdButtonParaConfig_ButtonNum
        {
            get {
                return adButtonParaConfig_ButtonNum; }
            set
            {
                adButtonParaConfig_ButtonNum = value;
                M_AdbtncfgDto.Num = value;
                this.RaisePropertyChanged(nameof(AdButtonParaConfig_ButtonNum));
            }
        }

        private string devConfig_ActiveCode = "0";
        /// <summary>
        /// 通用设备参数配置-激活码
        /// </summary>
        public string DevConfig_ActiveCode
        {
            get {
                M_DevcfgDto.Code = devConfig_ActiveCode;
                return devConfig_ActiveCode; }
            set
            {
                devConfig_ActiveCode = value;
                M_DevcfgDto.Code = value;
                this.RaisePropertyChanged(nameof(DevConfig_ActiveCode));
            }
        }

        private string devConfig_DeviceId = "0";
        /// <summary>
        /// 通用设备参数配置-设备ID
        /// </summary>
        public string DevConfig_DeviceId
        {
            get {
                M_DevcfgDto.Id = devConfig_DeviceId;
                return devConfig_DeviceId; }
            set
            {
                devConfig_DeviceId = value;
                M_DevcfgDto.Id = value;
                
                this.RaisePropertyChanged(nameof(DevConfig_DeviceId));
            }
        }

        private string deviceCanConfigParameters = "可配置选项";
        /// <summary>
        /// 设备可配置选项
        /// </summary>
        public string DeviceCanConfigParameters
        {
            get { return deviceCanConfigParameters; }
            set
            {
                deviceCanConfigParameters = value;
                this.RaisePropertyChanged(nameof(DeviceCanConfigParameters));
            }
        }

        private Boolean serialPortIsChecked = false;
        /// <summary>
        /// 串口开关是或否
        /// </summary>
        public Boolean SerialPortIsChecked
        {
            get { return serialPortIsChecked; }
            set
            {
                serialPortIsChecked = value;
                LocalSerialPortConfigEnable = !value;
                if (LocalSerialPortConfigEnable)
                {
                    if (serialPort != null && serialPort.IsOpen)
                    {
                        serialPort.Close();
                    }

                }
                else
                {
                    try
                    {
                        var a = localSerialPortConfig_SerialPortOrder;
                        var b = Convert.ToInt32(localSerialPortConfig_Baud.Content.ToString());
                        if (localSerialPortConfig_SerialPortOrder == string.Empty || localSerialPortConfig_Baud.Content.ToString() == string.Empty)
                        {
                            MessageBoxX.Show("请先设置串口和波特率", "Tips",Application.Current.MainWindow);
                            
                        }
                        else
                        {
                            serialPort = new SerialPort(a, b, Parity.None, 8, StopBits.One);
                            //serialPort.ReadTimeout = 100;
                            serialPort.DataReceived += new SerialDataReceivedEventHandler(Post_DataReceived);
                            serialPort.Open();
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBoxX.Show("请选择串口!", "Tips", Application.Current.MainWindow);
                        SerialPortIsChecked = false;
                        LogHelper.WriteLog("打开串口异常", ex);
                    }

                }
                this.RaisePropertyChanged(nameof(SerialPortIsChecked));
            }
        }
        private ComboBoxItem localSerialPortConfig_Baud = null;
        /// <summary>
        /// 本地串口设置
        /// </summary>
        public ComboBoxItem LocalSerialPortConfig_Baud
        {
            get { return localSerialPortConfig_Baud; }
            set
            {
                localSerialPortConfig_Baud = value;
                this.RaisePropertyChanged(nameof(LocalSerialPortConfig_Baud));
            }
        }

        private string localSerialPortConfig_SerialPortOrder = null;
        /// <summary>
        /// 本地串口设置
        /// </summary>
        public string LocalSerialPortConfig_SerialPortOrder
        {
            get { return localSerialPortConfig_SerialPortOrder; }
            set
            {
                localSerialPortConfig_SerialPortOrder = value;

                this.RaisePropertyChanged(nameof(LocalSerialPortConfig_SerialPortOrder));
            }
        }

        private string modbusConfig_REG_InputNumber = "0";
        /// <summary>
        /// Modbus配置-寄存器配置-输入数量
        /// </summary>
        public string ModbusConfig_REG_InputNumber
        {
            get {
                M_Mbregcfg.Inregcnt = modbusConfig_REG_InputNumber;
                return modbusConfig_REG_InputNumber;
                }
            set
            {
                modbusConfig_REG_InputNumber = value;
                M_Mbregcfg.Inregcnt = value;
                this.RaisePropertyChanged(nameof(ModbusConfig_REG_InputNumber));
            }
        }

        private string modbusConfig_REG_InputAddr = "0";
        /// <summary>
        /// Modbus配置-寄存器配置-输入地址
        /// </summary>
        public string ModbusConfig_REG_InputAddr
        {
            get {

                M_Mbregcfg.Inregadr = modbusConfig_REG_InputAddr;
                return modbusConfig_REG_InputAddr;
            }
            set
            {
                modbusConfig_REG_InputAddr = value;
                M_Mbregcfg.Inregadr = value;
                this.RaisePropertyChanged(nameof(ModbusConfig_REG_InputAddr));
            }
        }

        private string mQTTConfig_Dev_SubscribeInfoTopic = "0";
        /// <summary>
        /// MQTT终端配置-发布订阅-订阅消息主题
        /// </summary>
        public string MQTTConfig_Dev_SubscribeInfoTopic
        {
            get {
                M_MqtopiccfgDto.Sbtopic = mQTTConfig_Dev_SubscribeInfoTopic;
                return mQTTConfig_Dev_SubscribeInfoTopic;
                }
            set
            {
                mQTTConfig_Dev_SubscribeInfoTopic = value;
                M_MqtopiccfgDto.Sbtopic = value;
                this.RaisePropertyChanged(nameof(MQTTConfig_Dev_SubscribeInfoTopic));
            }
        }

        private string mQTTConfig_Dev_PublicInfoTopic = "0";
        /// <summary>
        /// MQTT终端配置-发布订阅-发布消息主题
        /// </summary>
        public string MQTTConfig_Dev_PublicInfoTopic
        {
            get {
                M_MqtopiccfgDto.Pbtopic = mQTTConfig_Dev_PublicInfoTopic;
                return mQTTConfig_Dev_PublicInfoTopic; 
            }
            set
            {
                mQTTConfig_Dev_PublicInfoTopic = value;
                M_MqtopiccfgDto.Pbtopic = value;
                this.RaisePropertyChanged(nameof(MQTTConfig_Dev_PublicInfoTopic));
            }
        }
        public ObservableCollection<string> MQTTConfig_Dev_InfoTopicNumber_No_Arr { get; set; } = new ObservableCollection<string>() {"0","1","2","3","4","5" };
        private string  mQTTConfig_Dev_InfoTopicNumber_No = "0";
        /// <summary>
        ///  MQTT终端配置-发布订阅-主题编号
        /// </summary>
        public string MQTTConfig_Dev_InfoTopicNumber_No
        {
            get { return mQTTConfig_Dev_InfoTopicNumber_No; }
            set
            {
                mQTTConfig_Dev_InfoTopicNumber_No = value;
                M_MqtopiccfgDto.Num = mQTTConfig_Dev_InfoTopicNumber_No;
                this.RaisePropertyChanged(nameof(MQTTConfig_Dev_InfoTopicNumber_No));
            }
        }

        private string mQTTConfig_RunHeartJumpPackage_SendData = "0";
        /// <summary>
        ///  MQTT终端配置-心跳包-发送数据
        /// </summary>
        public string MQTTConfig_RunHeartJumpPackage_SendData
        {
            get {
                M_MqheartenDto.Message = mQTTConfig_RunHeartJumpPackage_SendData;
                return mQTTConfig_RunHeartJumpPackage_SendData; }
            set
            {
                mQTTConfig_RunHeartJumpPackage_SendData = value;
                M_MqheartenDto.Message = value;
                this.RaisePropertyChanged(nameof(MQTTConfig_RunHeartJumpPackage_SendData));
            }
        }
        public ObservableCollection<string> MQTTConfig_RunHeartJumpPackage_BanStation_Arr { get; set; } = new ObservableCollection<string>() { "0","1"};
        private string mQTTConfig_RunHeartJumpPackage_BanStation = "0";
        /// <summary>
        /// MQTT终端配置-心跳包-使能位
        /// </summary>
        public string MQTTConfig_RunHeartJumpPackage_BanStation
        {
            get { return mQTTConfig_RunHeartJumpPackage_BanStation; }
            set
            {
                mQTTConfig_RunHeartJumpPackage_BanStation = value;
                M_MqheartenDto.En = mQTTConfig_RunHeartJumpPackage_BanStation;
                this.RaisePropertyChanged(nameof(MQTTConfig_RunHeartJumpPackage_BanStation));
            }
        }

        private string mQTTConfig_SSL_VersionNumber = "0";
        /// <summary>
        /// MQTT终端配置-SSL-版本号
        /// </summary>
        public string MQTTConfig_SSL_VersionNumber
        {
            get {
                M_MqsslvDto.Ver = mQTTConfig_SSL_VersionNumber;
                return mQTTConfig_SSL_VersionNumber;
            }
            set
            {
                mQTTConfig_SSL_VersionNumber = value;
                M_MqsslvDto.Ver = value;
                this.RaisePropertyChanged(nameof(MQTTConfig_SSL_VersionNumber));
            }
        }
        public ObservableCollection<string> MQTTConfig_SSL_BanStaiton_Arr { get; set; } = new ObservableCollection<string>() {"0","1" };
        private string mQTTConfig_SSL_BanStaitonmyVar = null;
        /// <summary>
        /// MQTT终端配置-SSL-使能位
        /// </summary>
        public string MQTTConfig_SSL_BanStaiton
        {
            get { return mQTTConfig_SSL_BanStaitonmyVar; }
            set
            {
                mQTTConfig_SSL_BanStaitonmyVar = value;
                M_MqsslvDto.En = mQTTConfig_SSL_BanStaitonmyVar;
                this.RaisePropertyChanged(nameof(MQTTConfig_SSL_BanStaiton));
            }
        }

        private string mQTTConfig_AddrAndPort_BrokerPassword = "0";
        /// <summary>
        /// MQTT终端配置-远程地址及端口号-Broker密码
        /// </summary>
        public string MQTTConfig_AddrAndPort_BrokerPassword
        {
            get {
                M_MqbrkcfgDto.Password = mQTTConfig_AddrAndPort_BrokerPassword;
                return mQTTConfig_AddrAndPort_BrokerPassword; }
            set
            {
                mQTTConfig_AddrAndPort_BrokerPassword = value;
                M_MqbrkcfgDto.Password = value;
                this.RaisePropertyChanged(nameof(MQTTConfig_AddrAndPort_BrokerPassword));
            }
        }

        private string mQTTConfig_AddrAndPort_BrokerUserName = "0";
        /// <summary>
        /// MQTT终端配置-远程地址及端口号-Broker用户名
        /// </summary>
        public string MQTTConfig_AddrAndPort_BrokerUserName
        {
            get {
                M_MqbrkcfgDto.UserName = mQTTConfig_AddrAndPort_BrokerUserName;
                return mQTTConfig_AddrAndPort_BrokerUserName; }
            set
            {
                mQTTConfig_AddrAndPort_BrokerUserName = value;
                M_MqbrkcfgDto.UserName = value;
                this.RaisePropertyChanged(nameof(MQTTConfig_AddrAndPort_BrokerUserName));
            }
        }

        private string mQTTConfig_AddrAndPort_ClientId = "0";
        /// <summary>
        /// MQTT终端配置-远程地址及端口号-客户端Id
        /// </summary>
        public string MQTTConfig_AddrAndPort_ClientId
        {
            get {
                M_MqbrkcfgDto.ClientId = mQTTConfig_AddrAndPort_ClientId;
                return mQTTConfig_AddrAndPort_ClientId; }
            set
            {
                mQTTConfig_AddrAndPort_ClientId = value;
                M_MqbrkcfgDto.ClientId = value;
                this.RaisePropertyChanged(nameof(MQTTConfig_AddrAndPort_ClientId));
            }
        }

        private string mQTTConfig_AddrAndPort_RemotePort = "0";
        /// <summary>
        /// MQTT终端配置-远程地址及端口号-远程端口
        /// </summary>
        public string MQTTConfig_AddrAndPort_RemotePort
        {
            get {
                M_MqbrkcfgDto.Port = mQTTConfig_AddrAndPort_RemotePort;
                return mQTTConfig_AddrAndPort_RemotePort; }
            set
            {
                mQTTConfig_AddrAndPort_RemotePort = value;
                M_MqbrkcfgDto.Port = value;
                this.RaisePropertyChanged(nameof(MQTTConfig_AddrAndPort_RemotePort));
            }
        }

        private string mQTTConfig_AddrAndPort_RemoteAddr = "0";
        /// <summary>
        /// MQTT终端配置-远程地址及端口号-远程地址
        /// </summary>
        public string MQTTConfig_AddrAndPort_RemoteAddr
        {
            get {
                M_MqbrkcfgDto.IpOrUrl = mQTTConfig_AddrAndPort_RemoteAddr;
                return mQTTConfig_AddrAndPort_RemoteAddr; }
            set
            {
                mQTTConfig_AddrAndPort_RemoteAddr = value;
                M_MqbrkcfgDto.IpOrUrl = value;
                this.RaisePropertyChanged(nameof(MQTTConfig_AddrAndPort_RemoteAddr));
            }
        }
        public ObservableCollection<string> MQTTConfig_AddrAndPort_Identifier_Arr { get; set; } = new ObservableCollection<string>() {"0","1","2","3","4","5" };
        private string mQTTConfig_AddrAndPort_Identifier = null;
        /// <summary>
        /// MQTT终端配置-远程地址及端口号-客户端标识符
        /// </summary>
        public string MQTTConfig_AddrAndPort_Identifier
        {
            get { return mQTTConfig_AddrAndPort_Identifier; }
            set
            {
                mQTTConfig_AddrAndPort_Identifier = value;
                M_MqbrkcfgDto.Num = mQTTConfig_AddrAndPort_Identifier;
                this.RaisePropertyChanged(nameof(MQTTConfig_AddrAndPort_Identifier));
            }
        }

        private string modbusConfig_REG_KeepNumber = "0";
        /// <summary>
        ///  modus配置-寄存器配置-保持数量
        /// </summary>
        public string ModbusConfig_REG_KeepNumber
        {
            get {
                M_Mbregcfg.Hdregcnt = modbusConfig_REG_KeepNumber;
                return modbusConfig_REG_KeepNumber; }
            set
            {
                modbusConfig_REG_KeepNumber = value;
                M_Mbregcfg.Hdregcnt = value;
                this.RaisePropertyChanged(nameof(ModbusConfig_REG_KeepNumber));
            }
        }

        private string modbusConfig_REG_KeepAddr = "0";
        /// <summary>
        /// modus配置-寄存器配置-保持地址
        /// </summary>
        public string ModbusConfig_REG_KeepAddr
        {
            get {
                M_Mbregcfg.Hdregadr = modbusConfig_REG_KeepAddr;
                return modbusConfig_REG_KeepAddr; }
            set
            {
                modbusConfig_REG_KeepAddr = value;
                M_Mbregcfg.Hdregadr = value;
                this.RaisePropertyChanged(nameof(ModbusConfig_REG_KeepAddr));
            }
        }

        private string modbusConfig_REG_DiscNumber = "0";
        /// <summary>
        /// modus配置-寄存器配置-离散数量
        /// </summary>
        public string ModbusConfig_REG_DiscNumber
        {
            get {
                M_Mbregcfg.Indescnt = modbusConfig_REG_DiscNumber;
                return modbusConfig_REG_DiscNumber; }
            set
            {
                modbusConfig_REG_DiscNumber = value;
                M_Mbregcfg.Indescnt = value;
                this.RaisePropertyChanged(nameof(ModbusConfig_REG_DiscNumber));
            }
        }

        private string modbusConfig_REG_DiscAddr = "0";
        /// <summary>
        ///  modus配置-寄存器配置-离散地址
        /// </summary>
        public string ModbusConfig_REG_DiscAddr
        {
            get {
                M_Mbregcfg.Indesadr = modbusConfig_REG_DiscAddr;
                return modbusConfig_REG_DiscAddr; }
            set
            {
                modbusConfig_REG_DiscAddr = value;
                M_Mbregcfg.Indesadr = value;
                this.RaisePropertyChanged(nameof(ModbusConfig_REG_DiscAddr));
            }
        }

        private string modbusConfig_REG_CoilNumber = "0";
        /// <summary>
        /// modus配置-寄存器配置-线圈数量
        /// </summary>
        public string ModbusConfig_REG_CoilNumber
        {
            get {
                M_Mbregcfg.Coilcnt = modbusConfig_REG_CoilNumber;
                return modbusConfig_REG_CoilNumber; }
            set
            {
                modbusConfig_REG_CoilNumber = value;
                M_Mbregcfg.Coilcnt = value;
                this.RaisePropertyChanged(nameof(ModbusConfig_REG_CoilNumber));
            }
        }

        private string modbusConfig_REG_CoilAddr = "0";
        /// <summary>
        /// modus配置-寄存器配置-线圈地址
        /// </summary>
        public string ModbusConfig_REG_CoilAddr
        {
            get {
                M_Mbregcfg.Coiladr = modbusConfig_REG_CoilAddr;
                return modbusConfig_REG_CoilAddr; }
            set
            {
                modbusConfig_REG_CoilAddr = value;
                M_Mbregcfg.Coiladr = value;
                this.RaisePropertyChanged(nameof(ModbusConfig_REG_CoilAddr));
            }
        }
        public ObservableCollection<string> ModbusConfig_REG_Number_Arr { get; set; } = new ObservableCollection<string>() { "0","1","2","3","4","5"};
        private string modbusConfig_REG_Number = "0";
        /// <summary>
        ///  modus配置-寄存器配置-Modbus编号
        /// </summary>
        public string ModbusConfig_REG_Number
        {
            get { return modbusConfig_REG_Number; }
            set
            {
                modbusConfig_REG_Number = value;
                M_Mbregcfg.Number = modbusConfig_REG_Number;
                this.RaisePropertyChanged(nameof(ModbusConfig_REG_Number));
            }
        }

        private string modbusConfig_Addr_Scantime = "0";
        /// <summary>
        /// modus配置-地址-扫描间隔(ms)
        /// </summary>
        public string ModbusConfig_Addr_Scantime
        {
            get {
                M_MbcfgDto.Scantime = modbusConfig_Addr_Scantime;
                return modbusConfig_Addr_Scantime; }
            set
            {
                modbusConfig_Addr_Scantime = value;
                M_MbcfgDto.Scantime = value;
                this.RaisePropertyChanged(nameof(ModbusConfig_Addr_Scantime));
            }
        }

        public ObservableCollection<string> ModbusConfig_Addr_CommunicationMode_Arr { get; set; } = new ObservableCollection<string>() { "0", "1" };
        private string modbusConfig_Addr_CommunicationMode = "0";
        /// <summary>
        /// modus配置-地址-交流模式
        /// </summary>
        public string ModbusConfig_Addr_CommunicationMode
        {
            get { return modbusConfig_Addr_CommunicationMode; }
            set
            {
                modbusConfig_Addr_CommunicationMode = value;
                M_MbcfgDto.Ms = modbusConfig_Addr_CommunicationMode;
                this.RaisePropertyChanged(nameof(ModbusConfig_Addr_CommunicationMode));
            }
        }
        public ObservableCollection<string> ModbusConfig_Addr_SlaveOrMaster_Arr { get; set; } = new ObservableCollection<string>() { "0","1"};
        private string modbusConfig_Addr_SlaveOrMaster = "0";
        /// <summary>
        /// modus配置-地址-主从模式
        /// </summary>
        public string ModbusConfig_Addr_SlaveOrMaster
        {
            get { return modbusConfig_Addr_SlaveOrMaster; }
            set
            {
                modbusConfig_Addr_SlaveOrMaster = value;
                M_MbcfgDto.Mode = modbusConfig_Addr_SlaveOrMaster;
                this.RaisePropertyChanged(nameof(ModbusConfig_Addr_SlaveOrMaster));
            }
        }

        private string modbusConfig_Addr_Addr = "0";
        /// <summary>
        /// modus配置-地址-地址
        /// </summary>
        public string ModbusConfig_Addr_Addr
        {
            get {
                M_MbcfgDto.Id = modbusConfig_Addr_Addr;
                return modbusConfig_Addr_Addr; }
            set
            {
                modbusConfig_Addr_Addr = value;
                M_MbcfgDto.Id = value;
                this.RaisePropertyChanged(nameof(ModbusConfig_Addr_Addr));
            }
        }
        
        private Boolean modbusConfig_Addr_BanStation_Switch = true;
        /// <summary>
        /// modus配置-地址-使能位-开关
        /// </summary>
        public Boolean ModbusConfig_Addr_BanStation_Switch
        {
            get { return modbusConfig_Addr_BanStation_Switch; }
            set
            {
                modbusConfig_Addr_BanStation_Switch = value;
                M_MbcfgDto.En =EnConvert(modbusConfig_Addr_BanStation_Switch);
                if (modbusConfig_Addr_BanStation_Switch)
                    NetConfigModbusEnabled = true;
                else NetConfigModbusEnabled = false;
                this.RaisePropertyChanged(nameof(ModbusConfig_Addr_BanStation_Switch));
            }
        }
        private Boolean netConfigModbusEnabled = true;
        /// <summary>
        /// modus配置-地址-使能
        /// </summary>
        public Boolean NetConfigModbusEnabled
        {
            get { return netConfigModbusEnabled; }
            set
            {
                netConfigModbusEnabled = value;
                this.RaisePropertyChanged(nameof(NetConfigModbusEnabled));
            }
        }

        public ObservableCollection<string> ModbusConfig_Addr_Number_Arr { get; set; } = new ObservableCollection<string>() { "0", "1", "2", "3", "4", "5" };
        private string modbusConfig_Addr_Number = "0";
        /// <summary>
        /// modus配置-地址-编号
        /// </summary>
        public string ModbusConfig_Addr_Number
        {
            get { return modbusConfig_Addr_Number; }
            set
            {
                modbusConfig_Addr_Number = value;
                M_MbcfgDto.Number = modbusConfig_Addr_Number;
                this.RaisePropertyChanged(nameof(ModbusConfig_Addr_Number));
            }
        }
        public ObservableCollection<string> SerialPortConfig_HardControlStation_Arr { get; set; } = new ObservableCollection<string>() { "0", "1" };
        private string serialPortConfig_HardControlStation = "0";
        /// <summary>
        ///  串口配置-窗口波特率-硬件控制流位
        /// </summary>
        public string SerialPortConfig_HardControlStation
        {
            get { return serialPortConfig_HardControlStation; }
            set
            {
                serialPortConfig_HardControlStation = value;
                M_SercfgDto.Flowctrls = serialPortConfig_HardControlStation;
                this.RaisePropertyChanged(nameof(SerialPortConfig_HardControlStation));
            }
        }
        public ObservableCollection<string> SerialPortConfig_CheckStation_Arr { get; set; } = new ObservableCollection<string>() { "0","1","2"};
        private string serialPortConfig_CheckStation = "0";
        /// <summary>
        /// 串口配置-窗口波特率-校验位
        /// </summary>
        public string SerialPortConfig_CheckStation
        {
            get { return serialPortConfig_CheckStation; }
            set
            {
                serialPortConfig_CheckStation = value;
                M_SercfgDto.Parity = serialPortConfig_CheckStation;
                this.RaisePropertyChanged(nameof(SerialPortConfig_CheckStation));
            }
        }
        public ObservableCollection<string> SerialPortConfig_StopStation_Arr { get; set; } = new ObservableCollection<string>() { "0", "1" };
        private string serialPortConfig_StopStation = "0";
        /// <summary>
        /// 串口配置-窗口波特率-停止位
        /// </summary>
        public string  SerialPortConfig_StopStation
        {
            get { return serialPortConfig_StopStation; }
            set
            {
                serialPortConfig_StopStation = value;
                M_SercfgDto.Stopbit = serialPortConfig_StopStation;
                this.RaisePropertyChanged(nameof(SerialPortConfig_StopStation));
            }
        }
        public ObservableCollection<string> SerialPortConfig_DataStation_Arr { get; set; } = new ObservableCollection<string>() { "0","1","2","3","4","5","6","7","8"};
        private string  serialPortConfig_DataStation = "0";
        /// <summary>
        ///  串口配置-窗口波特率-数据位
        /// </summary>
        public string SerialPortConfig_DataStation
        {
            get { return serialPortConfig_DataStation; }
            set
            {
                serialPortConfig_DataStation = value;
                M_SercfgDto.Databit = serialPortConfig_DataStation;
                this.RaisePropertyChanged(nameof(SerialPortConfig_DataStation));
            }
        }
        public ObservableCollection<string> SerialPortConfig_BaudRate_Arr { get; set; } = new ObservableCollection<string>() {"0","1200","2400","4800","9600","19200","38400","57600","115200", "230400" };
        private string serialPortConfig_BaudRate = "0";
        /// <summary>
        /// 串口配置-窗口波特率-波特率
        /// </summary>
        public string SerialPortConfig_BaudRate
        {
            get { return serialPortConfig_BaudRate; }
            set
            {
                serialPortConfig_BaudRate = value;
                M_SercfgDto.Baud = serialPortConfig_BaudRate;
                this.RaisePropertyChanged(nameof(SerialPortConfig_BaudRate));
            }
        }
        public ObservableCollection<string> SerialPortConfig_SerialPortNumber_Arr { get; set; } = new ObservableCollection<string>() { "0", "1", "2", "3", "4", "5" };
        private string serialPortConfig_SerialPortNumber = "0";
        /// <summary>
        /// 串口配置-窗口波特率-串口号
        /// </summary>
        public string SerialPortConfig_SerialPortNumber
        {
            get { return serialPortConfig_SerialPortNumber; }
            set
            {
                serialPortConfig_SerialPortNumber = value;
                M_SercfgDto.Comx = serialPortConfig_SerialPortNumber;
                this.RaisePropertyChanged(nameof(SerialPortConfig_SerialPortNumber));
            }
        }

        private string netConfig_Socket_BufferSize = "0";
        /// <summary>
        /// 网络配置-Socket-缓冲区大小
        /// </summary>
        public string NetConfig_Socket_BufferSize
        {
            get {
                M_SockcfgDto.Packagelen = netConfig_Socket_BufferSize;
                return netConfig_Socket_BufferSize; }
            set
            {
                netConfig_Socket_BufferSize = value;
                M_SockcfgDto.Packagelen = value;
                this.RaisePropertyChanged(nameof(NetConfig_Socket_BufferSize));
            }
        }

        private string netConfig_Socket_ServerConnections = "0";
        /// <summary>
        /// 网络配置-Socket-服务器最长连接数
        /// </summary>
        public string NetConfig_Socket_ServerConnections
        {
            get {
                M_SockcfgDto.Maxcon = netConfig_Socket_ServerConnections;
                return netConfig_Socket_ServerConnections; }
            set
            {
                netConfig_Socket_ServerConnections = value;
                M_SockcfgDto.Maxcon = value;
                this.RaisePropertyChanged(nameof(NetConfig_Socket_ServerConnections));
            }
        }

        private string netConfig_Socket_RemotePort = "0";
        /// <summary>
        /// 网络配置-Socket-远程端口
        /// </summary>
        public string NetConfig_Socket_RemotePort
        {
            get {
                M_SockcfgDto.RemotePort = netConfig_Socket_RemotePort;
                return netConfig_Socket_RemotePort; }
            set
            {
                netConfig_Socket_RemotePort = value;
                M_SockcfgDto.RemotePort = value;
                this.RaisePropertyChanged(nameof(NetConfig_Socket_RemotePort));
            }
        }

        private string netConfig_Socket_RemoteIpAddr = "0";
        /// <summary>
        /// 网络配置-Socket-远程IP地址
        /// </summary>
        public string NetConfig_Socket_RemoteIpAddr
        {
            get {
                M_SockcfgDto.RemoteIp = netConfig_Socket_RemoteIpAddr;
                return netConfig_Socket_RemoteIpAddr; }
            set
            {
                netConfig_Socket_RemoteIpAddr = value;
                M_SockcfgDto.RemoteIp = value;
                this.RaisePropertyChanged(nameof(NetConfig_Socket_RemoteIpAddr));
            }
        }

        private string netConfig_Socket_LocalPort = "0";
        /// <summary>
        /// 网络配置-Socket-本地端口
        /// </summary>
        public string NetConfig_Socket_LocalPort
        {
            get {
                M_SockcfgDto.LocalPort = netConfig_Socket_LocalPort;
                return netConfig_Socket_LocalPort; }
            set
            {
                netConfig_Socket_LocalPort = value;
                M_SockcfgDto.LocalPort = value;
                this.RaisePropertyChanged(nameof(NetConfig_Socket_LocalPort));
            }
        }
        public ObservableCollection<string> NetConfig_Socket_AgreementType_Arr { get; set; } = new ObservableCollection<string>() { "0", "1" };
        private string netConfig_Socket_AgreementType = "0";
        /// <summary>
        /// 网络配置-Socket-协议类型
        /// </summary>
        public string  NetConfig_Socket_AgreementType
        {
            get { return netConfig_Socket_AgreementType; }
            set
            {
                netConfig_Socket_AgreementType = value;
                M_SockcfgDto.Mode = netConfig_Socket_AgreementType;
                this.RaisePropertyChanged(nameof(NetConfig_Socket_AgreementType));
            }
        }
        
        private Boolean netConfig_Socket_BanStation_Switch = true;
        /// <summary>
        /// 网络配置-套接字配置-使能位-开关
        /// </summary>
        public Boolean NetConfig_Socket_BanStation_Switch
        {
            get { return netConfig_Socket_BanStation_Switch; }
            set
            {
                netConfig_Socket_BanStation_Switch = value;
                M_SockcfgDto.En =EnConvert(netConfig_Socket_BanStation_Switch) ;
                if (netConfig_Socket_BanStation_Switch) {
                    NetConfigSocketEnabled = true;
                } else {
                    NetConfigSocketEnabled = false;
                }
                this.RaisePropertyChanged(nameof(NetConfig_Socket_BanStation_Switch));
            }
        }
        private Boolean netConfigSocketEnabled = true;
        /// <summary>
        /// 网络配置-套接字配置-使能
        /// </summary>
        public Boolean NetConfigSocketEnabled
        {
            get { return netConfigSocketEnabled; }
            set
            {
                netConfigSocketEnabled = value;
                this.RaisePropertyChanged(nameof(NetConfigSocketEnabled));
            }
        }

        private string netConfig_Socket_Number = "0";
        /// <summary>
        /// 网络配置-Socket-网卡号
        /// </summary>
        public string NetConfig_Socket_Number
        {
            get {
                M_SockcfgDto.Num = netConfig_Socket_Number;
                return netConfig_Socket_Number; }
            set
            {
                netConfig_Socket_Number = value;
                M_SockcfgDto.Num = value;
                this.RaisePropertyChanged(nameof(NetConfig_Socket_Number));
            }
        }

        private string netConfig_Socket_NetCardName = "0";
        /// <summary>
        /// 网络配置-Socket-网卡名
        /// </summary>
        public string NetConfig_Socket_NetCardName
        {
            get {
                M_SockcfgDto.Name = netConfig_Socket_NetCardName;
                return netConfig_Socket_NetCardName; }
            set
            {
                netConfig_Socket_NetCardName = value;
                M_SockcfgDto.Name = value;
                this.RaisePropertyChanged(nameof(NetConfig_Socket_NetCardName));
            }
        }

        private string macAddrStr = "0";
        /// <summary>
        /// mac地址
        /// </summary>
        public string MacAddrStr
        {
            get {
                M_NipcfgDto.Mac = macAddrStr;
                return macAddrStr; }
            set
            {
                macAddrStr = value;
                M_NipcfgDto.Mac = value;
                this.RaisePropertyChanged(nameof(MacAddrStr));
            }
        }
        private string netConfig_IP_Dns = "0";
        /// <summary>
        /// 网络配置-IP-DNs
        /// </summary>
        public string NetConfig_IP_Dns
        {
            get {
                M_NipcfgDto.Dns = netConfig_IP_Dns;
                return netConfig_IP_Dns; }
            set
            {
                netConfig_IP_Dns = value;
                M_NipcfgDto.Dns = value;
                this.RaisePropertyChanged(nameof(NetConfig_IP_Dns));
            }
        }

        private string netConfig_IP_GateWay = "0";
        /// <summary>
        /// 网络配置-IP-网关
        /// </summary>
        public string NetConfig_IP_GateWay
        {
            get {
                M_NipcfgDto.GateWay = netConfig_IP_GateWay;
                return netConfig_IP_GateWay; }
            set
            {
                netConfig_IP_GateWay = value;
                M_NipcfgDto.GateWay = value;
                this.RaisePropertyChanged(nameof(NetConfig_IP_GateWay));
            }
        }

        private string netConfig_IP_NetMask = "0";
        /// <summary>
        /// 网络配置-IP-子网掩码
        /// </summary>
        public string NetConfig_IP_NetMask
        {
            get {
                M_NipcfgDto.Mask = netConfig_IP_NetMask;
                return netConfig_IP_NetMask; }
            set
            {
                netConfig_IP_NetMask = value;
                M_NipcfgDto.Mask = value;
                this.RaisePropertyChanged(nameof(NetConfig_IP_NetMask));
            }
        }

        private string netConfig_IP_Addr = "0";
        /// <summary>
        /// 网络配置-IP-IP地址
        /// </summary>
        public string NetConfig_IP_Addr
        {
            get {
                M_NipcfgDto.Ip = netConfig_IP_Addr;
                return netConfig_IP_Addr; }
            set
            {
                netConfig_IP_Addr = value;
                M_NipcfgDto.Ip = value;
                this.RaisePropertyChanged(nameof(NetConfig_IP_Addr));
            }
        }
        public ObservableCollection<string> NetConfig_IP_NetType_Arr { get; set; } = new ObservableCollection<string>() { "0", "1" };
        private string netConfig_IP_NetType = "0";
        /// <summary>
        /// 网络配置-IP-网络类型
        /// </summary>
        public string NetConfig_IP_NetType
        {
            get { return netConfig_IP_NetType; }
            set
            {
                netConfig_IP_NetType = value;
                M_NipcfgDto.Mode = netConfig_IP_NetType;
                this.RaisePropertyChanged(nameof(NetConfig_IP_NetType));
            }
        }

        private string netConfig_IP_NetCardName = "0";
        /// <summary>
        /// 网络配置-IP-网卡名
        /// </summary>
        public string NetConfig_IP_NetCardName
        {
            get {
                M_NipcfgDto.Name = netConfig_IP_NetCardName;
                return netConfig_IP_NetCardName; }
            set
            {
                netConfig_IP_NetCardName = value;
                M_NipcfgDto.Name = value;
                this.RaisePropertyChanged(nameof(NetConfig_IP_NetCardName));
            }
        }

        private string netConfig_STA_Password = "0";
        /// <summary>
        /// 网络配置-STA-密码
        /// </summary>
        public string NetConfig_STA_Password
        {
            get {
                M_WstacfgDto.Passwd = netConfig_STA_Password;
                return netConfig_STA_Password; }
            set
            {
                netConfig_STA_Password = value;
                M_WstacfgDto.Passwd = value;
                this.RaisePropertyChanged(nameof(NetConfig_STA_Password));
            }
        }

        private string netConfig_STA_UserName = "0";
        /// <summary>
        /// 网络配置-STA-用户名
        /// </summary>
        public string NetConfig_STA_UserName
        {
            get {
                M_WstacfgDto.Ssid = netConfig_STA_UserName;
                return netConfig_STA_UserName; }
            set
            {
                netConfig_STA_UserName = value;
                M_WstacfgDto.Ssid = value;
                this.RaisePropertyChanged(nameof(NetConfig_STA_UserName));
            }
        }
        
        private Boolean netConfig_STA_DhcpBanStation_Switch = true;
        /// <summary>
        /// 网络配置-STA-DHCP功能使能-开关
        /// </summary>
        public Boolean NetConfig_STA_DhcpBanStation_Switch
        {
            get { return netConfig_STA_DhcpBanStation_Switch; }
            set
            {
                netConfig_STA_DhcpBanStation_Switch = value;
                M_WstacfgDto.Dhcp =EnConvert(netConfig_STA_DhcpBanStation_Switch) ;
                this.RaisePropertyChanged(nameof( NetConfig_STA_DhcpBanStation_Switch));
            }
        }

        public ObservableCollection<string> NetConfig_STA_SafeType_Arr { get; set; } = new ObservableCollection<string>() { "0", "1" };
        private string netConfig_STA_SafeType = "0";
        /// <summary>
        /// 网络配置-STA-安全类型
        /// </summary>
        public string NetConfig_STA_SafeType
        {
            get { return netConfig_STA_SafeType; }
            set
            {
                netConfig_STA_SafeType = value;
                M_WstacfgDto.Encry = netConfig_STA_SafeType;
                this.RaisePropertyChanged(nameof(NetConfig_STA_SafeType));
            }
        }
        
        private Boolean netConfig_STA_BanStation_Switch = true;
        /// <summary>
        /// 网络配置-STA-使能位-开关
        /// </summary>
        public Boolean NetConfig_STA_BanStation_Switch
        {
            get { return netConfig_STA_BanStation_Switch; }
            set
            {
                netConfig_STA_BanStation_Switch = value;
                M_WstacfgDto.En = EnConvert(netConfig_STA_BanStation_Switch);
                if (netConfig_STA_BanStation_Switch) {
                    NetConfigStaEnabled = true;
                }
                else {
                    NetConfigStaEnabled = false;
                }
                this.RaisePropertyChanged(nameof(NetConfig_STA_BanStation_Switch));
            }
        }
        private Boolean netConfigStaEnabled = true;
        /// <summary>
        /// 网络配置-STA-使能位
        /// </summary>
        public Boolean NetConfigStaEnabled
        {
            get { return netConfigStaEnabled; }
            set
            {
                netConfigStaEnabled = value;
                this.RaisePropertyChanged(nameof(NetConfigStaEnabled));
            }
        }

        private string netConfig_STA_NetCardName = "0";
        /// <summary>
        /// 网络配置-STA-网卡名
        /// </summary>
        public string NetConfig_STA_NetCardName
        {
            get {
                M_WstacfgDto.Name = netConfig_STA_NetCardName;
                return netConfig_STA_NetCardName; }
            set
            {
                netConfig_STA_NetCardName = value;
                M_WstacfgDto.Name = value;
                this.RaisePropertyChanged(nameof(NetConfig_STA_NetCardName));
            }
        }

        private string netConfig_AP_DhcpEndAddr = "0";
        /// <summary>
        /// 网络配置-AP-Dhcp结束地址
        /// </summary>
        public string NetConfig_AP_DhcpEndAddr
        {
            get {
                M_WapcfgDto.DhcpStop = netConfig_AP_DhcpEndAddr;
                return netConfig_AP_DhcpEndAddr; }
            set
            {
                netConfig_AP_DhcpEndAddr = value;
                M_WapcfgDto.DhcpStop = value;
                this.RaisePropertyChanged(nameof(NetConfig_AP_DhcpEndAddr));
            }
        }

        private string netConfig_AP_DhcpStartAddr = "0";
        /// <summary>
        /// 网络配置-AP-Dhcp起始地址
        /// </summary>
        public string NetConfig_AP_DhcpStartAddr
        {
            get {
                M_WapcfgDto.DhcpStart = netConfig_AP_DhcpStartAddr;
                return netConfig_AP_DhcpStartAddr; }
            set
            {
                netConfig_AP_DhcpStartAddr = value;
                M_WapcfgDto.DhcpStart = value;
                this.RaisePropertyChanged(nameof(NetConfig_AP_DhcpStartAddr));
            }
        }

        private string netConfig_AP_Password = "0";
        /// <summary>
        /// 网络配置-AP-密码
        /// </summary>
        public string NetConfig_AP_Password
        {
            get {
                M_WapcfgDto.Password = netConfig_AP_Password;
                return netConfig_AP_Password; }
            set
            {
                netConfig_AP_Password = value;
                M_WapcfgDto.Password = value;
                this.RaisePropertyChanged(nameof(NetConfig_AP_Password));
            }
        }

        private string netConfig_AP_UserName = "0";
        /// <summary>
        /// 网络配置-AP-用户名 
        /// </summary>
        public string NetConfig_AP_UserName
        {
            get {
                M_WapcfgDto.Ssid = netConfig_AP_UserName;
                return netConfig_AP_UserName; }
            set
            {
                netConfig_AP_UserName = value;
                M_WapcfgDto.Ssid = value;
                this.RaisePropertyChanged(nameof(NetConfig_AP_UserName));
            }
        }
        public ObservableCollection<string> NetConfig_AP_SafeType_Arr { get; set; } = new ObservableCollection<string>{ "0", "1" };
        private string netConfig_AP_SafeType = "0";
        /// <summary>
        /// 网络配置-AP-安全类型
        /// </summary>
        public string NetConfig_AP_SafeType
        {
            get { return netConfig_AP_SafeType; }
            set
            {
                netConfig_AP_SafeType = value;
                M_WapcfgDto.Encry = netConfig_AP_SafeType;
                this.RaisePropertyChanged(nameof(NetConfig_AP_SafeType));
            }
        }

        
        private Boolean netConfig_AP_BanStation_Switch = true;
        /// <summary>
        /// 网络配置-AP-使能位-开关转换尝试
        /// </summary>
        public Boolean NetConfig_AP_BanStation_Switch
        {
            get { return netConfig_AP_BanStation_Switch; }
            set
            {
                netConfig_AP_BanStation_Switch = value;
                M_WapcfgDto.En = EnConvert(NetConfig_AP_BanStation_Switch);
                if (netConfig_AP_BanStation_Switch)
                {
                    //不禁止
                    NetConfigEnabled = true;
                }
                else {
                    //禁止
                    NetConfigEnabled = false;
                }
                this.RaisePropertyChanged(nameof(NetConfig_AP_BanStation_Switch));
            }
        }
        private Boolean netConfigEnabled = true;
        /// <summary>
        /// 网络配置区使能
        /// </summary>
        public Boolean NetConfigEnabled
        {
            get { return netConfigEnabled; }
            set
            {
                netConfigEnabled = value;
                this.RaisePropertyChanged(nameof(NetConfigEnabled));
            }
        }

        
        private string netConfig_AP_NetCardName = "0";
        /// <summary>
        /// 网络配置-AP-网卡名
        /// </summary>
        public string NetConfig_AP_NetCardName
        {
            get {
                    M_WapcfgDto.EthName = netConfig_AP_NetCardName;
                    return netConfig_AP_NetCardName; }
            set
            {
                netConfig_AP_NetCardName = value;
                M_WapcfgDto.EthName = value;
                this.RaisePropertyChanged(nameof(NetConfig_AP_NetCardName));
            }
        }

        #endregion
        #region 写一些非绑定的方法
        /// <summary>
        /// 使能系列boolean转string
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        private string EnConvert(Boolean para)
        {
            if (para) return "1";
            else return "0";
        }
        /// <summary>
        /// 使能系列string转boolean
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        private Boolean EnConvertStr(string para)
        {
            if (para.Equals("0"))
                return true;
            else return false;
        }
        /// <summary>
        /// 把每个Dto初始化一下
        /// </summary>
        public void InitConfigDto()
        {
            M_MbcfgDto = new MbcfgDto();
            M_Mbregcfg = new MbregcfgDto();
            M_MqbrkcfgDto = new MqbrkcfgDto();
            M_MqheartenDto = new MqheartenDto();
            M_MqsslvDto = new MqsslvDto();
            M_MqtopiccfgDto = new MqtopiccfgDto();
            M_NipcfgDto = new NipcfgDto();
            M_SercfgDto = new SercfgDto();
            M_SockcfgDto = new SockcfgDto();
            M_WapcfgDto = new WapcfgDto();
            M_WstacfgDto = new WstacfgDto();
            M_DevcfgDto = new DevcfgDto();
            M_ColmdcfgDto = new ColmdcfgDto();
            M_AdbtncfgDto = new AdbtncfgDto();
        }
        /// <summary>
        /// 求两个列表的差集
        /// </summary>
        /// <param name="array0"></param>
        /// <param name="array1"></param>
        /// <returns></returns>
        public static List<string> GetInterSect(List<string> array0, List<string> array1)
        {
            return array0.Except(array1).ToList();
        }
        /// <summary>
        /// 显示或隐藏控制
        /// </summary>
        /// <param name="DeviceName"></param>
        public void ShowOrHideControl(string DeviceName)
        {
            //加载所有可配置控件
            List<string> cc = DevicesJsonUtils.ReadConfigHide("Total");
            //获取到所有需要显示的控件
            List<string> bb = DevicesJsonUtils.ReadConfigHide(DeviceName);
            //求差集，将不需要的隐藏
            var dd = GetInterSect(cc, bb);
            //将dd系列隐藏
            foreach (var item in dd)
            {
                this.GetType().GetProperty(item).SetValue(this, "Collapsed");

            }
        }
        /// <summary>
        /// 自动生成一个不重复的符合我司规则的mac地址
        /// </summary>
        /// <returns></returns>
        public string AutoProduceLanMacAddress()
        {
            try
            {
                System.DateTime currentTime = new System.DateTime();
                currentTime = System.DateTime.Now;
                string str = currentTime.ToString("yyMMddHHmmss");
                long hex = long.Parse(str);
                string result = "0c" + Convert.ToString(hex, 16);
                if (result.Length == 12)
                {
                    result = result.Insert(2, ":");
                    result = result.Insert(5, ":");
                    result = result.Insert(8, ":");
                    result = result.Insert(11, ":");
                    result = result.Insert(14, ":");
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {

                return null;
            }


        }
      
        
        /// <summary>
        /// 移除协议头尾
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        public static string RemoveHeadAndTail(string msg, string head, string tail)
        {
            return msg.Substring(head.Length, msg.Length - head.Length - tail.Length); ;
        }
        /// <summary>
        /// 移除多余字符
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        public static string RemoveOthers(string msg, string head, string tail)
        {
            try
            {
                // MessageBox.Show(RemoveHeadAndTail(msg, head, tail).Remove(1, 34));
                return RemoveHeadAndTail(msg, head, tail);
            }
            catch (Exception)
            {
                Console.WriteLine("字符串不符合格式");
                return null;
            }

        }
        /// <summary>
        /// 解析好的Json字符串转DeviceInfo
        /// </summary>
        /// <param name="jsonstr"></param>
        /// <returns></returns>
        public static DeviceInfo JsonStrToDeviceInfo(string jsonstr)
        {
            try
            {

                JObject jo = (JObject)JsonConvert.DeserializeObject(jsonstr);
                DeviceInfo ds = jo.ToObject<DeviceInfo>();
                return ds;
            }
            catch (Exception)
            {
                Console.WriteLine("字符串无法转换");
                return null;
            }

        }
        /// <summary>
        /// 判断设备对象是否存在
        /// </summary>
        /// <param name="di"></param>
        /// <returns></returns>
        public Boolean IsInDeviceList(DeviceInfo di)
        {
            foreach (var item in DeviceInfos)
            {
                if (item.strName.Equals(di.strName) && item.strIpAddress.Equals(di.strIpAddress) && item.strMacAddress.Equals(di.strMacAddress))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 检查字段是否符合规则
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        public static Boolean CheckRecStr(string msg, string head, string tail)
        {
            return msg.Substring(0, head.Length).Equals(head) && msg.Substring(msg.Length - tail.Length).Equals(tail);
        }
        /// <summary>
        /// udp接收方法
        /// </summary>
        /// <param name="StopTime"></param>
        /// <param name="port"></param>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        public void UdpReceive(string ip, int StopTime, int port, string head, string tail)
        {

            //创建一个UDPSocket
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, StopTime);
                //创建一个端口为port的终结点接收来自任意端口的信息
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, port);
                //绑定本地网卡
                sock.Bind(new IPEndPoint(IPAddress.Parse(ip), 3690));
                //sock.Bind(iep);
                EndPoint ep = iep;
                byte[] data = new byte[1024];
                while (true)
                {
                    int recv = sock.ReceiveFrom(data, ref ep);
                    string stringData = Encoding.ASCII.GetString(data, 0, recv);
                    //MessageBox.Show(stringData);
                    if (CheckRecStr(stringData, head, tail))//判断该模型是否存在，去重操作
                    {

                        //转为设备信息模型存入设备list,如果在list已经存在则不加入
                        System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                        {
                            if (IsInDeviceList(JsonStrToDeviceInfo(RemoveOthers(stringData, head, tail))))
                            {
                                DeviceInfos.Add(JsonStrToDeviceInfo(RemoveOthers(stringData, head, tail)));
                                //MessageBox.Show(stringData);
                            }
                        }));


                    }

                }

            }
            catch (Exception)
            {
                sock.Close();

            }

        }
        /// <summary>
        /// udp接收方法
        /// </summary>
        /// <param name="StopTime"></param>
        /// <param name="port"></param>
        public void UdpReceiveConfigParameters(string ip, int StopTime, int port)
        {

            //创建一个UDPSocket
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, StopTime);
                //创建一个端口为port的终结点接收来自任意端口的信息
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, port);
                //绑定本地网卡
                sock.Bind(new IPEndPoint(IPAddress.Parse(ip), 3690));
                //sock.Bind(iep);
                EndPoint ep = iep;
                byte[] data = new byte[1024];
                while (true)
                {
                    int recv = sock.ReceiveFrom(data, ref ep);
                    string str = Encoding.ASCII.GetString(data, 0, recv);
                    //MessageBox.Show(stringData);
                    ShowTheParameters(str);
                }

            }
            catch (Exception)
            {
                sock.Close();

            }

        }
        /// <summary>
        /// 展示网卡列表
        /// </summary>
        public void ShowNetAdapterList()
        {
            NetAdapterList.Clear();
            if (NetworkUtils.NetworkList() != null)
            {
                foreach (var item in NetworkUtils.NetworkList())
                {
                    NetAdapterList.Add(item);
                }
            }
            else
            {
                MessageBoxX.Show("网卡列表为空.", "Tips", Application.Current.MainWindow);
            }

        }
        #endregion
        /// <summary>
        /// 入口.
        /// </summary>
        public MainViewModel()
        {

            Messenger.Default.Register<string>(this, "DeviceName", ShowUserView);
            NetAdapterList = new ObservableCollection<string>();
            ShowNetAdapterList();
            DeviceInfos = new ObservableCollection<DeviceInfo>();
            SerialPortsName = new ObservableCollection<string>();
            InitConfigDto();
            InitSerialPortsName();
            
            Task.Factory.StartNew(async()=> {
                Boolean flag = true;
                while (flag) {

                    await Task.Delay(100);
                    if (!string.IsNullOrEmpty(DeviceNameCurrent)) {
                        ShowOrHideControl(DeviceNameCurrent);
                        DeviceCanConfigParameters = $"{DeviceNameCurrent}可配置选项";
                        flag = false;
                        if (DeviceNameCurrent.Equals("ADbutton") || DeviceNameCurrent.Equals("Cat1-Link")) {
                            ConfigMode = 0;
                        } else if (DeviceNameCurrent.Equals("Cpt300")) {
                            ConfigMode = 1;
                        }
                    }
                }
            });
            
            LogHelper.WriteLog("启动");
            
        }
        /// <summary>
        /// 接收UserSelect传来消息 
        /// </summary>
        /// <param name="deviceName"></param>
        void ShowUserView(string deviceName)
        {
            DeviceNameCurrent = deviceName;
        }
        #region 关于控制和隐藏需要配置项小项
        private string gatherPatternConfigHide_HeratJumpData = "Visible";
        /// <summary>
        /// 
        /// </summary>
        public string GatherPatternConfigHide_HeratJumpData
        {
            get { return gatherPatternConfigHide_HeratJumpData; }
            set
            {
                gatherPatternConfigHide_HeratJumpData = value;
                this.RaisePropertyChanged(nameof(GatherPatternConfigHide_HeratJumpData));
            }
        }

        private string gatherPatternConfigHide_HeartTriggerTime = "Visible";
        /// <summary>
        /// 采集模式配置_采集器触发时间
        /// </summary>
        public string GatherPatternConfigHide_HeartTriggerTime
        {
            get { return gatherPatternConfigHide_HeartTriggerTime; }
            set
            {
                gatherPatternConfigHide_HeartTriggerTime = value;
                this.RaisePropertyChanged(nameof(GatherPatternConfigHide_HeartTriggerTime));
            }
        }

        private string gatherPatternConfigHide_TimerControllerTriggerTime = "Visible";
        /// <summary>
        /// 采集模式配置_定时器触发时间
        /// </summary>
        public string GatherPatternConfigHide_TimerControllerTriggerTime
        {
            get { return gatherPatternConfigHide_TimerControllerTriggerTime; }
            set
            {
                gatherPatternConfigHide_TimerControllerTriggerTime = value;
                this.RaisePropertyChanged(nameof(GatherPatternConfigHide_TimerControllerTriggerTime));
            }
        }

        private string gatherPatternConfigHide_DataUpLoadPattern = "Visible";
        /// <summary>
        /// 采集模式配置_数据上传模式
        /// </summary>
        public string GatherPatternConfigHide_DataUpLoadPattern
        {
            get { return gatherPatternConfigHide_DataUpLoadPattern; }
            set
            {
                gatherPatternConfigHide_DataUpLoadPattern = value;
                this.RaisePropertyChanged(nameof(GatherPatternConfigHide_DataUpLoadPattern));
            }
        }

        private string gatherPatternConfigHide = "Visible";
        /// <summary>
        /// 采集模式配置 大项
        /// </summary>
        public string GatherPatternConfigHide
        {
            get { return gatherPatternConfigHide; }
            set
            {
                gatherPatternConfigHide = value;
                this.RaisePropertyChanged(nameof(GatherPatternConfigHide));
            }
        }

        private string adButtonParaConfigHide_LedmodeTimeout = "Visible";
        /// <summary>
        /// 安灯按钮参数配置_按钮运行模式
        /// </summary>
        public string AdButtonParaConfigHide_LedmodeTimeout
        {
            get { return adButtonParaConfigHide_LedmodeTimeout; }
            set
            {
                adButtonParaConfigHide_LedmodeTimeout = value;
                this.RaisePropertyChanged(nameof(AdButtonParaConfigHide_LedmodeTimeout));
            }
        }

        private string adButtonParaConfigHide_ButtonRunPattern = "Visible";
        /// <summary>
        /// 安灯按钮参数配置_按钮运行模式
        /// </summary>
        public string AdButtonParaConfigHide_ButtonRunPattern
        {
            get { return adButtonParaConfigHide_ButtonRunPattern; }
            set
            {
                adButtonParaConfigHide_ButtonRunPattern = value;
                this.RaisePropertyChanged(nameof(AdButtonParaConfigHide_ButtonRunPattern));
            }
        }

        private string adButtonParaConfigHide_ButtonNum = "Visible";
        /// <summary>
        /// 安灯按钮参数配置_按钮编号
        /// </summary>
        public string AdButtonParaConfigHide_ButtonNum
        {
            get { return adButtonParaConfigHide_ButtonNum; }
            set
            {
                adButtonParaConfigHide_ButtonNum = value;
                this.RaisePropertyChanged(nameof(AdButtonParaConfigHide_ButtonNum));
            }
        }

        private string adButtonParaConfigHide = "Visible";
        /// <summary>
        /// 安灯按钮参数配置大项
        /// </summary>
        public string AdButtonParaConfigHide
        {
            get { return adButtonParaConfigHide; }
            set
            {
                adButtonParaConfigHide = value;
                this.RaisePropertyChanged(nameof(AdButtonParaConfigHide));
            }
        }

        private string devConfigHide_ActiveCode = "Visible";
        /// <summary>
        /// 通用配置-激活码
        /// </summary>
        public string DevConfigHide_ActiveCode
        {
            get { return devConfigHide_ActiveCode; }
            set
            {
                devConfigHide_ActiveCode = value;
                this.RaisePropertyChanged(nameof(DevConfigHide_ActiveCode));
            }
        }

        private string devConfigHide_DeviceId = "Visible";
        /// <summary>
        /// 通用配置-设备ID
        /// </summary>
        public string DevConfigHide_DeviceId
        {
            get { return devConfigHide_DeviceId; }
            set
            {
                devConfigHide_DeviceId = value;
                this.RaisePropertyChanged(nameof(DevConfigHide_DeviceId));
            }
        }

        private string devConfigHide = "Visible";
        /// <summary>
        /// 通用配置大项
        /// </summary>
        public string DevConfigHide
        {
            get { return devConfigHide; }
            set
            {
                devConfigHide = value;
                this.RaisePropertyChanged(nameof(DevConfigHide));
            }
        }

        private string configParametersUdpHide = "Visible";
        /// <summary>
        /// 配置参数按钮 Udp
        /// </summary>
        public string ConfigParametersUdpHide
        {
            get { return configParametersUdpHide; }
            set
            {
                configParametersUdpHide = value;
                this.RaisePropertyChanged(nameof(ConfigParametersUdpHide));
            }
        }

        private string configParametersSerialPortHide = "Visible";
        /// <summary>
        /// 配置参数按钮 串口
        /// </summary>
        public string ConfigParametersSerialPortHide
        {
            get { return configParametersSerialPortHide; }
            set
            {
                configParametersSerialPortHide = value;
                this.RaisePropertyChanged(nameof(ConfigParametersSerialPortHide));
            }
        }

        private string getParametersUdpHide = "Visible";
        /// <summary>
        /// 获取参数按钮Udp
        /// </summary>
        public string GetParametersUdpHide
        {
            get { return getParametersUdpHide; }
            set
            {
                getParametersUdpHide = value;
                this.RaisePropertyChanged(nameof(GetParametersUdpHide));
            }
        }

        private string getParametersSerialPortHide = "Visible";
        /// <summary>
        /// 获取参数按钮串口
        /// </summary>
        public string GetParametersSerialPortHide
        {
            get { return getParametersSerialPortHide; }
            set
            {
                getParametersSerialPortHide = value;
                this.RaisePropertyChanged(nameof(GetParametersSerialPortHide));
            }
        }

        private string localConnConfigSerialPort = "Visible";
        /// <summary>
        /// 本地通讯设置 串口
        /// </summary>
        public string LocalConnConfigSerialPort
        {
            get { return localConnConfigSerialPort; }
            set
            {
                localConnConfigSerialPort = value;
                this.RaisePropertyChanged(nameof(LocalConnConfigSerialPort));
            }
        }

        private string localConnConfigUdp = "Visible";
        /// <summary>
        /// 本地通讯设置Udp
        /// </summary>
        public string LocalConnConfigUdp
        {
            get { return localConnConfigUdp; }
            set
            {
                localConnConfigUdp = value;
                this.RaisePropertyChanged(nameof(LocalConnConfigUdp));
            }
        }

        private string serialPortConfigHide_BufferSize = "Visible";
        /// <summary>
        /// 串口配置 缓冲区大小
        /// </summary>
        public string SerialPortConfigHide_BufferSize
        {
            get { return serialPortConfigHide_BufferSize; }
            set
            {
                serialPortConfigHide_BufferSize = value;
                this.RaisePropertyChanged(nameof(SerialPortConfigHide_BufferSize));
            }
        }

        private string modbusConfigHide_REG_InputNumber = "Visible";
        /// <summary>
        /// Modbus配置-寄存器配置-输入数量
        /// </summary>
        public string ModbusConfigHide_REG_InputNumber
        {
            get { return modbusConfigHide_REG_InputNumber; }
            set
            {
                modbusConfigHide_REG_InputNumber = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide_REG_InputNumber));
            }
        }

        private string modbusConfigHide_REG_InputAddr = "Visible";
        /// <summary>
        /// Modbus配置-寄存器配置-输入地址
        /// </summary>
        public string ModbusConfigHide_REG_InputAddr
        {
            get { return modbusConfigHide_REG_InputAddr; }
            set
            {
                modbusConfigHide_REG_InputAddr = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide_REG_InputAddr));
            }
        }

        private string mQTTConfigHide_Dev_SubscribeInfoTopic = "Visible";
        /// <summary>
        ///  MQTT终端配置-发布消息主题设定-订阅消息主题
        /// </summary>
        public string MQTTConfigHide_Dev_SubscribeInfoTopic
        {
            get { return mQTTConfigHide_Dev_SubscribeInfoTopic; }
            set
            {
                mQTTConfigHide_Dev_SubscribeInfoTopic = value;
                this.RaisePropertyChanged(nameof(MQTTConfigHide_Dev_SubscribeInfoTopic));
            }
        }

        private string mQTTConfigHide_Dev_PublicInfoTopic = "Visible";
        /// <summary>
        /// MQTT终端配置-发布消息主题设定-发布消息主题
        /// </summary>
        public string MQTTConfigHide_Dev_PublicInfoTopic
        {
            get { return mQTTConfigHide_Dev_PublicInfoTopic; }
            set
            {
                mQTTConfigHide_Dev_PublicInfoTopic = value;
                this.RaisePropertyChanged(nameof(MQTTConfigHide_Dev_PublicInfoTopic));
            }
        }

        private string mQTTConfigHide_Dev_InfoTopicNumber_No = "Visible";
        /// <summary>
        /// MQTT终端配置-发布消息主题设定-消息主题编号
        /// </summary>
        public string MQTTConfigHide_Dev_InfoTopicNumber_No
        {
            get { return mQTTConfigHide_Dev_InfoTopicNumber_No; }
            set
            {
                mQTTConfigHide_Dev_InfoTopicNumber_No = value;
                this.RaisePropertyChanged(nameof(MQTTConfigHide_Dev_InfoTopicNumber_No));
            }
        }



        private string mQTTConfigHide_RunHeartJumpPackage_SendData = "Visible";
        /// <summary>
        /// MQTT配置-运行心跳包-心跳包发送数据
        /// </summary>
        public string MQTTConfigHide_RunHeartJumpPackage_SendData
        {
            get { return mQTTConfigHide_RunHeartJumpPackage_SendData; }
            set
            {
                mQTTConfigHide_RunHeartJumpPackage_SendData = value;
                this.RaisePropertyChanged(nameof(MQTTConfigHide_RunHeartJumpPackage_SendData));
            }
        }

        private string mQTTConfigHide_RunHeartJumpPackage_BanStation = "Visible";
        /// <summary>
        /// MQTT配置-运行心跳包-使能位
        /// </summary>
        public string MQTTConfigHide_RunHeartJumpPackage_BanStation
        {
            get { return mQTTConfigHide_RunHeartJumpPackage_BanStation; }
            set
            {
                mQTTConfigHide_RunHeartJumpPackage_BanStation = value;
                this.RaisePropertyChanged(nameof(MQTTConfigHide_RunHeartJumpPackage_BanStation));
            }
        }

        private string mQTTConfigHide_SSL_VersionNumber = "Visible";
        /// <summary>
        /// MQTT配置-SSL配置-版本号
        /// </summary>
        public string MQTTConfigHide_SSL_VersionNumber
        {
            get { return mQTTConfigHide_SSL_VersionNumber; }
            set
            {
                mQTTConfigHide_SSL_VersionNumber = value;
                this.RaisePropertyChanged(nameof(MQTTConfigHide_SSL_VersionNumber));
            }
        }

        private string mQTTConfigHide_SSL_BanStaiton = "Visible";
        /// <summary>
        /// MQTT配置-SSL配置-使能位
        /// </summary>
        public string MQTTConfigHide_SSL_BanStaiton
        {
            get { return mQTTConfigHide_SSL_BanStaiton; }
            set
            {
                mQTTConfigHide_SSL_BanStaiton = value;
                this.RaisePropertyChanged(nameof(MQTTConfigHide_SSL_BanStaiton));
            }
        }

        private string mQTTConfigHide_AddrAndPort_BrokerPassword = "Visible";
        /// <summary>
        /// MQTT配置-地址和端口号-Broker密码
        /// </summary>
        public string MQTTConfigHide_AddrAndPort_BrokerPassword
        {
            get { return mQTTConfigHide_AddrAndPort_BrokerPassword; }
            set
            {
                mQTTConfigHide_AddrAndPort_BrokerPassword = value;
                this.RaisePropertyChanged(nameof(MQTTConfigHide_AddrAndPort_BrokerPassword));
            }
        }

        private string mQTTConfigHide_AddrAndPort_BrokerUserNameyVar = "Visible";
        /// <summary>
        /// MQTT配置-地址和端口号-Broker账号
        /// </summary>
        public string MQTTConfigHide_AddrAndPort_BrokerUserName
        {
            get { return mQTTConfigHide_AddrAndPort_BrokerUserNameyVar; }
            set
            {
                mQTTConfigHide_AddrAndPort_BrokerUserNameyVar = value;
                this.RaisePropertyChanged(nameof(MQTTConfigHide_AddrAndPort_BrokerUserName));
            }
        }

        private string mQTTConfigHide_AddrAndPort_ClientId = "Visible";
        /// <summary>
        /// MQTT配置-地址和端口号-客户端ID
        /// </summary>
        public string MQTTConfigHide_AddrAndPort_ClientId
        {
            get { return mQTTConfigHide_AddrAndPort_ClientId; }
            set
            {
                mQTTConfigHide_AddrAndPort_ClientId = value;
                this.RaisePropertyChanged(nameof(MQTTConfigHide_AddrAndPort_ClientId));
            }
        }

        private string mQTTConfigHide_AddrAndPort_RemotePort = "Visible";
        /// <summary>
        /// MQTT配置-地址和端口号-远程端口
        /// </summary>
        public string MQTTConfigHide_AddrAndPort_RemotePort
        {
            get { return mQTTConfigHide_AddrAndPort_RemotePort; }
            set
            {
                mQTTConfigHide_AddrAndPort_RemotePort = value;
                this.RaisePropertyChanged(nameof(MQTTConfigHide_AddrAndPort_RemotePort));
            }
        }

        private string mQTTConfigHide_AddrAndPort_RemoteAddr = "Visible";
        /// <summary>
        /// MQTT配置-地址和端口号-远程地址
        /// </summary>
        public string MQTTConfigHide_AddrAndPort_RemoteAddr
        {
            get { return mQTTConfigHide_AddrAndPort_RemoteAddr; }
            set
            {
                mQTTConfigHide_AddrAndPort_RemoteAddr = value;
                this.RaisePropertyChanged(nameof(MQTTConfigHide_AddrAndPort_RemoteAddr));
            }
        }

        private string mQTTConfigHide_AddrAndPort_Identifier = "Visible";
        /// <summary>
        /// MQTT配置-地址和端口号-客户端标识符
        /// </summary>
        public string MQTTConfigHide_AddrAndPort_Identifier
        {
            get { return mQTTConfigHide_AddrAndPort_Identifier; }
            set
            {
                mQTTConfigHide_AddrAndPort_Identifier = value;
                this.RaisePropertyChanged(nameof(MQTTConfigHide_AddrAndPort_Identifier));
            }
        }

        private string modbusConfigHide_REG_KeepNumber = "Visible";
        /// <summary>
        /// modbus配置-寄存器配置—保持寄存器数量
        /// </summary>
        public string ModbusConfigHide_REG_KeepNumber
        {
            get { return modbusConfigHide_REG_KeepNumber; }
            set
            {
                modbusConfigHide_REG_KeepNumber = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide_REG_KeepNumber));
            }
        }

        private string modbusConfigHide_REG_KeepAddr = "Visible";
        /// <summary>
        /// modbus配置-寄存器配置—保持寄存器地址
        /// </summary>
        public string ModbusConfigHide_REG_KeepAddr
        {
            get { return modbusConfigHide_REG_KeepAddr; }
            set
            {
                modbusConfigHide_REG_KeepAddr = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide_REG_KeepAddr));
            }
        }

        private string modbusConfigHide_REG_DiscNumber = "Visible";
        /// <summary>
        /// modbus配置-寄存器配置—离散寄存器数量
        /// </summary>
        public string ModbusConfigHide_REG_DiscNumber
        {
            get { return modbusConfigHide_REG_DiscNumber; }
            set
            {
                modbusConfigHide_REG_DiscNumber = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide_REG_DiscNumber));
            }
        }

        private string modbusConfigHide_REG_DiscAddr = "Visible";
        /// <summary>
        /// modbus配置-寄存器配置—离散寄存器地址
        /// </summary>
        public string ModbusConfigHide_REG_DiscAddr
        {
            get { return modbusConfigHide_REG_DiscAddr; }
            set
            {
                modbusConfigHide_REG_DiscAddr = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide_REG_DiscAddr));
            }
        }

        private string modbusConfigHide_REG_CoilNumber = "Visible";
        /// <summary>
        /// modbus配置-寄存器配置—线圈数量
        /// </summary>
        public string ModbusConfigHide_REG_CoilNumber
        {
            get { return modbusConfigHide_REG_CoilNumber; }
            set
            {
                modbusConfigHide_REG_CoilNumber = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide_REG_CoilNumber));
            }
        }

        private string modbusConfigHide_REG_CoilAddr = "Visible";
        /// <summary>
        /// modbus配置-寄存器配置—线圈地址
        /// </summary>
        public string ModbusConfigHide_REG_CoilAddr
        {
            get { return modbusConfigHide_REG_CoilAddr; }
            set
            {
                modbusConfigHide_REG_CoilAddr = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide_REG_CoilAddr));
            }
        }

        private string modbusConfigHide_REG_Number = "Visible";
        /// <summary>
        /// modbus配置-寄存器配置—编号
        /// </summary>
        public string ModbusConfigHide_REG_Number
        {
            get { return modbusConfigHide_REG_Number; }
            set
            {
                modbusConfigHide_REG_Number = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide_REG_Number));
            }
        }

        private string modbusConfigHide_Addr_Scantime = "Visible";
        /// <summary>
        /// Modbus配置-地址配置-扫描间隔
        /// </summary>
        public string ModbusConfigHide_Addr_Scantime
        {
            get { return modbusConfigHide_Addr_Scantime; }
            set
            {
                modbusConfigHide_Addr_Scantime = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide_Addr_Scantime));
            }
        }



        private string modbusConfigHide_Addr_CommunicationMode = "Visible";
        /// <summary>
        /// Modbus配置-地址配置-通讯模式
        /// </summary>
        public string ModbusConfigHide_Addr_CommunicationMode
        {
            get { return modbusConfigHide_Addr_CommunicationMode; }
            set
            {
                modbusConfigHide_Addr_CommunicationMode = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide_Addr_CommunicationMode));
            }
        }

        private string modbusConfigHide_Addr_SlaveOrMaster = "Visible";
        /// <summary>
        /// Modbus配置-地址配置-主从模式
        /// </summary>
        public string ModbusConfigHide_Addr_SlaveOrMaster
        {
            get { return modbusConfigHide_Addr_SlaveOrMaster; }
            set
            {
                modbusConfigHide_Addr_SlaveOrMaster = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide_Addr_SlaveOrMaster));
            }
        }

        private string modbusConfigHide_Addr_Addr = "Visible";
        /// <summary>
        ///  Modbus配置-地址配置-MOdbus地址
        /// </summary>
        public string ModbusConfigHide_Addr_Addr
        {
            get { return modbusConfigHide_Addr_Addr; }
            set
            {
                modbusConfigHide_Addr_Addr = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide_Addr_Addr));
            }
        }

        private string modbusConfigHide_Addr_BanStation = "Visible";
        /// <summary>
        /// Modbus配置-地址配置-使能位
        /// </summary>
        public string ModbusConfigHide_Addr_BanStation
        {
            get { return modbusConfigHide_Addr_BanStation; }
            set
            {
                modbusConfigHide_Addr_BanStation = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide_Addr_BanStation));
            }
        }

        private string modbusConfigHide_Addr_Number = "Visible";
        /// <summary>
        /// Modbus配置-地址配置-Modbus编号
        /// </summary>
        public string ModbusConfigHide_Addr_Number
        {
            get { return modbusConfigHide_Addr_Number; }
            set
            {
                modbusConfigHide_Addr_Number = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide_Addr_Number));
            }
        }

        private string serialPortConfigHide_HardControlStation = "Visible";
        /// <summary>
        /// 串口配置-硬件流控制位
        /// </summary>
        public string SerialPortConfigHide_HardControlStation
        {
            get { return serialPortConfigHide_HardControlStation; }
            set
            {
                serialPortConfigHide_HardControlStation = value;
                this.RaisePropertyChanged(nameof(SerialPortConfigHide_HardControlStation));
            }
        }

        private string serialPortConfigHide_CheckStation = "Visible";
        /// <summary>
        /// 串口配置-校验位
        /// </summary>
        public string SerialPortConfigHide_CheckStation
        {
            get { return serialPortConfigHide_CheckStation; }
            set
            {
                serialPortConfigHide_CheckStation = value;
                this.RaisePropertyChanged(nameof(SerialPortConfigHide_CheckStation));
            }
        }

        private string serialPortConfigHide_StopStation = "Visible";
        /// <summary>
        /// 串口配置-停止位
        /// </summary>
        public string SerialPortConfigHide_StopStation
        {
            get { return serialPortConfigHide_StopStation; }
            set
            {
                serialPortConfigHide_StopStation = value;
                this.RaisePropertyChanged(nameof(SerialPortConfigHide_StopStation));
            }
        }

        private string serialPortConfigHide_DataStation = "Visible";
        /// <summary>
        /// 串口配置-数据位
        /// </summary>
        public string SerialPortConfigHide_DataStation
        {
            get { return serialPortConfigHide_DataStation; }
            set
            {
                serialPortConfigHide_DataStation = value;
                this.RaisePropertyChanged(nameof(SerialPortConfigHide_DataStation));
            }
        }

        private string serialPortConfigHide_BaudRate = "Visible";
        /// <summary>
        /// 串口配置-波特率
        /// </summary>
        public string SerialPortConfigHide_BaudRate
        {
            get { return serialPortConfigHide_BaudRate; }
            set
            {
                serialPortConfigHide_BaudRate = value;
                this.RaisePropertyChanged(nameof(SerialPortConfigHide_BaudRate));
            }
        }

        private string serialPortConfigHide_SerialPortNumber = "Visible";
        /// <summary>
        /// 串口配置-串口编号
        /// </summary>
        public string SerialPortConfigHide_SerialPortNumber
        {
            get { return serialPortConfigHide_SerialPortNumber; }
            set
            {
                serialPortConfigHide_SerialPortNumber = value;
                this.RaisePropertyChanged(nameof(SerialPortConfigHide_SerialPortNumber));
            }
        }

        private string netConfigHide_Socket_BufferSize = "Visible";
        /// <summary>
        /// 网络配置-Socket配置-缓冲区大小
        /// </summary>
        public string NetConfigHide_Socket_BufferSize
        {
            get { return netConfigHide_Socket_BufferSize; }
            set
            {
                netConfigHide_Socket_BufferSize = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_Socket_BufferSize));
            }
        }

        private string netConfigHide_Socket_ServerConnections = "Visible";
        /// <summary>
        /// 网络配置-Socket配置-最长连接数
        /// </summary>
        public string NetConfigHide_Socket_ServerConnections
        {
            get { return netConfigHide_Socket_ServerConnections; }
            set
            {
                netConfigHide_Socket_ServerConnections = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_Socket_ServerConnections));
            }
        }

        private string netConfigHide_Socket_RemotePort = "Visible";
        /// <summary>
        /// 网络配置-Socket配置-远程端口配置
        /// </summary>
        public string NetConfigHide_Socket_RemotePort
        {
            get { return netConfigHide_Socket_RemotePort; }
            set
            {
                netConfigHide_Socket_RemotePort = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_Socket_RemotePort));
            }
        }

        private string netConfigHide_Socket_RemoteIpAddr = "Visible";
        /// <summary>
        /// 网络配置-Socket配置-远程Ip地址
        /// </summary>
        public string NetConfigHide_Socket_RemoteIpAddr
        {
            get { return netConfigHide_Socket_RemoteIpAddr; }
            set
            {
                netConfigHide_Socket_RemoteIpAddr = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_Socket_RemoteIpAddr));
            }
        }

        private string netConfigHide_Socket_LocalPort = "Visible";
        /// <summary>
        /// 网络配置-Socket配置-协议本地端口
        /// </summary>
        public string NetConfigHide_Socket_LocalPort
        {
            get { return netConfigHide_Socket_LocalPort; }
            set
            {
                netConfigHide_Socket_LocalPort = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_Socket_LocalPort));
            }
        }

        private string netConfigHide_Socket_AgreementType = "Visible";
        /// <summary>
        /// 网络配置-Socket配置-协议类型
        /// </summary>
        public string NetConfigHide_Socket_AgreementType
        {
            get { return netConfigHide_Socket_AgreementType; }
            set
            {
                netConfigHide_Socket_AgreementType = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_Socket_AgreementType));
            }
        }

        private string netConfigHide_Socket_BanStation = "Visible";
        /// <summary>
        /// 网络配置-Socket配置-使能位
        /// </summary>
        public string NetConfigHide_Socket_BanStation
        {
            get { return netConfigHide_Socket_BanStation; }
            set
            {
                netConfigHide_Socket_BanStation = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_Socket_BanStation));
            }
        }

        private string netConfigHide_Socket_Number = "Visible";
        /// <summary>
        /// 网络配置-Socket配置-Socket编号
        /// </summary>
        public string NetConfigHide_Socket_Number
        {
            get { return netConfigHide_Socket_Number; }
            set
            {
                netConfigHide_Socket_Number = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_Socket_Number));
            }
        }

        private string netConfigHide_Socket_NetCardName = "Visible";
        /// <summary>
        /// 网络配置-Socket配置-网卡名
        /// </summary>
        public string NetConfigHide_Socket_NetCardName
        {
            get { return netConfigHide_Socket_NetCardName; }
            set
            {
                netConfigHide_Socket_NetCardName = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_Socket_NetCardName));
            }
        }

        private string netConfigHide_IP_MacAddr = "Visible";
        /// <summary>
        /// 网络配置-IP配置-Mac地址
        /// </summary>
        public string NetConfigHide_IP_MacAddr
        {
            get { return netConfigHide_IP_MacAddr; }
            set
            {
                netConfigHide_IP_MacAddr = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_IP_MacAddr));
            }
        }

        private string netConfigHide_IP_Dns = "Visible";
        /// <summary>
        /// 网络配置-IP配置-Dns
        /// </summary>
        public string NetConfigHide_IP_Dns
        {
            get { return netConfigHide_IP_Dns; }
            set
            {
                netConfigHide_IP_Dns = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_IP_Dns));
            }
        }

        private string netConfigHide_IP_GateWay = "Visible";
        /// <summary>
        /// 网络配置-IP配置-网关
        /// </summary>
        public string NetConfigHide_IP_GateWay
        {
            get { return netConfigHide_IP_GateWay; }
            set
            {
                netConfigHide_IP_GateWay = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_IP_GateWay));
            }
        }

        private string netConfigHide_IP_NetMask = "Visible";
        /// <summary>
        /// 网络配置-Ip配置-子网掩码
        /// </summary>
        public string NetConfigHide_IP_NetMask
        {
            get { return netConfigHide_IP_NetMask; }
            set
            {
                netConfigHide_IP_NetMask = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_IP_NetMask));
            }
        }

        private string netConfigHide_IP_Addr = "Visible";
        /// <summary>
        /// 网络配置-IP配置-Ip地址
        /// </summary>
        public string NetConfigHide_IP_Addr
        {
            get { return netConfigHide_IP_Addr; }
            set
            {
                netConfigHide_IP_Addr = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_IP_Addr));
            }
        }

        private string netConfigHide_IP_NetType = "Visible";
        /// <summary>
        /// 网络配置-IP配置-网络类型
        /// </summary>
        public string NetConfigHide_IP_NetType
        {
            get { return netConfigHide_IP_NetType; }
            set
            {
                netConfigHide_IP_NetType = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_IP_NetType));
            }
        }

        private string netConfigHide_IP_NetCardName = "Visible";
        /// <summary>
        /// 网络配置-IP配置-网卡名
        /// </summary>
        public string NetConfigHide_IP_NetCardName
        {
            get { return netConfigHide_IP_NetCardName; }
            set
            {
                netConfigHide_IP_NetCardName = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_IP_NetCardName));
            }
        }

        private string netConfigHide_STA_Password = "Visible";
        /// <summary>
        /// 网络配置-无线STA-密码
        /// </summary>
        public string NetConfigHide_STA_Password
        {
            get { return netConfigHide_STA_Password; }
            set
            {
                netConfigHide_STA_Password = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_STA_Password));
            }
        }

        private string netConfigHide_STA_UserName = "Visible";
        /// <summary>
        /// 网络配置-无线STA-账号
        /// </summary>
        public string NetConfigHide_STA_UserName
        {
            get { return netConfigHide_STA_UserName; }
            set
            {
                netConfigHide_STA_UserName = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_STA_UserName));
            }
        }

        private string netConfigHide_STA_DhcpBanStation = "Visible";
        /// <summary>
        /// 网络配置-无线STA-Dhcp使能位
        /// </summary>
        public string NetConfigHide_STA_DhcpBanStation
        {
            get { return netConfigHide_STA_DhcpBanStation; }
            set
            {
                netConfigHide_STA_DhcpBanStation = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_STA_DhcpBanStation));
            }
        }

        private string netConfigHide_STA_SafeType = "Visible";
        /// <summary>
        /// 网络配置-无线STA-安全类型
        /// </summary>
        public string NetConfigHide_STA_SafeType
        {
            get { return netConfigHide_STA_SafeType; }
            set
            {
                netConfigHide_STA_SafeType = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_STA_SafeType));
            }
        }

        private string netConfigHide_STA_BanStation = "Visible";
        /// <summary>
        /// 网络配置-无线STA-使能位
        /// </summary>
        public string NetConfigHide_STA_BanStation
        {
            get { return netConfigHide_STA_BanStation; }
            set
            {
                netConfigHide_STA_BanStation = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_STA_BanStation));
            }
        }

        private string netConfigHide_STA_NetCardName = "Visible";
        /// <summary>
        /// 网络配置-无线STA-网卡名
        /// </summary>
        public string NetConfigHide_STA_NetCardName
        {
            get { return netConfigHide_STA_NetCardName; }
            set
            {
                netConfigHide_STA_NetCardName = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_STA_NetCardName));
            }
        }

        private string netConfigHide_AP_DhcpEndAddr = "Visible";
        /// <summary>
        /// 网络配置-无线Ap-dhcp结束地址
        /// </summary>
        public string NetConfigHide_AP_DhcpEndAddr
        {
            get { return netConfigHide_AP_DhcpEndAddr; }
            set
            {
                netConfigHide_AP_DhcpEndAddr = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_AP_DhcpEndAddr));
            }
        }

        private string netConfigHide_AP_DhcpStartAddr = "Visible";
        /// <summary>
        /// 网络配置-无线AP-dhcp起始地址
        /// </summary>
        public string NetConfigHide_AP_DhcpStartAddr
        {
            get { return netConfigHide_AP_DhcpStartAddr; }
            set
            {
                netConfigHide_AP_DhcpStartAddr = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_AP_DhcpStartAddr));
            }
        }

        private string netConfigHide_AP_Password = "Visible";
        /// <summary>
        /// 网络配置-无线AP-密码
        /// </summary>
        public string NetConfigHide_AP_Password
        {
            get { return netConfigHide_AP_Password; }
            set
            {
                netConfigHide_AP_Password = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_AP_Password));
            }
        }

        private string netConfigHide_AP_UserName = "Visible";
        /// <summary>
        /// 网络配置-无线AP-用户名
        /// </summary>
        public string NetConfigHide_AP_UserName
        {
            get { return netConfigHide_AP_UserName; }
            set
            {
                netConfigHide_AP_UserName = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_AP_UserName));
            }
        }

        private string netConfigHide_AP_SafeType = "Visible";
        /// <summary>
        /// 网络配置-无线AP-安全类型
        /// </summary>
        public string NetConfigHide_AP_SafeType
        {
            get { return netConfigHide_AP_SafeType; }
            set
            {
                netConfigHide_AP_SafeType = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_AP_SafeType));
            }
        }

        private string netConfigHide_AP_NetCardName = "Visible";
        /// <summary>
        /// 网络配置-无线AP-网卡名
        /// </summary>
        public string NetConfigHide_AP_NetCardName
        {
            get { return netConfigHide_AP_NetCardName; }
            set
            {
                netConfigHide_AP_NetCardName = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_AP_NetCardName));
            }
        }
        private string netConfigHide_AP_BanStation = "Visible";
        /// <summary>
        /// 网络配置-无线AP-使能位
        /// </summary>
        public string NetConfigHide_AP_BanStation
        {
            get { return netConfigHide_AP_BanStation; }
            set
            {
                netConfigHide_AP_BanStation = value;
                this.RaisePropertyChanged(nameof(NetConfigHide_AP_BanStation));
            }
        }
        private string configFilesHide = "Visible";
        /// <summary>
        /// 加载和保存配置文件
        /// </summary>
        public string ConfigFilesHide
        {
            get { return configFilesHide; }
            set
            {
                configFilesHide = value;
                this.RaisePropertyChanged(nameof(ConfigFilesHide));
            }
        }

        #endregion
        #region props
        private string localSerialPortTextBoxStr = string.Empty;
        /// <summary>
        /// 本地串口配置接收数据展示
        /// </summary>
        public string LocalSerialPortTextBoxStr
        {
            get { return localSerialPortTextBoxStr; }
            set
            {
                localSerialPortTextBoxStr = value;
                this.RaisePropertyChanged(nameof(LocalSerialPortTextBoxStr));
            }
        }

        private Boolean localSerialPortConfigEnable = true;
        /// <summary>
        /// 本地端口是否禁用
        /// </summary>
        public Boolean LocalSerialPortConfigEnable
        {
            get { return localSerialPortConfigEnable; }
            set
            {
                localSerialPortConfigEnable = value;
                this.RaisePropertyChanged(nameof(LocalSerialPortConfigEnable));
            }
        }



        private string serialPortConfig_BufferSize = "0";
        /// <summary>
        /// 串口配置-缓冲区
        /// </summary>
        public string SerialPortConfig_BufferSize
        {
            get {
                M_SercfgDto.PackageLen = serialPortConfig_BufferSize;
                return serialPortConfig_BufferSize; }
            set
            {
                serialPortConfig_BufferSize = value;
                M_SercfgDto.PackageLen = value;
                this.RaisePropertyChanged(nameof(SerialPortConfig_BufferSize));
            }
        }


        private string mQTTConfigHide = "Visible";
        /// <summary>
        /// MQTT配置列表隐藏
        /// </summary>
        public string MQTTConfigHide
        {
            get { return mQTTConfigHide; }
            set
            {
                mQTTConfigHide = value;
                this.RaisePropertyChanged(nameof(MQTTConfigHide));
            }
        }

        private string modbusConfigHide = "Visible";
        /// <summary>
        /// ModelBus配置隐藏
        /// </summary>
        public string ModbusConfigHide
        {
            get { return modbusConfigHide; }
            set
            {
                modbusConfigHide = value;
                this.RaisePropertyChanged(nameof(ModbusConfigHide));
            }
        }

        private string serialPortConfigHide = "Visible";
        /// <summary>
        /// 串口配置隐藏
        /// </summary>
        public string SerialPortConfigHide
        {
            get { return serialPortConfigHide; }
            set
            {
                serialPortConfigHide = value;
                this.RaisePropertyChanged(nameof(SerialPortConfigHide));
            }
        }



        private string netConfigHide = "Visible";
        /// <summary>
        /// 网络配置隐藏
        /// </summary>
        public string NetConfigHide
        {
            get { return netConfigHide; }
            set
            {
                netConfigHide = value;
                this.RaisePropertyChanged(nameof(NetConfigHide));
            }
        }

        



        private string showState = string.Empty;
        /// <summary>
        /// 显示状态
        /// </summary>
        public string ShowState
        {
            get { return showState; }
            set
            {
                showState = value;
                this.RaisePropertyChanged(nameof(ShowState));
            }
        }
        //private string NetCardIP = string.Empty;
        private string adapterDescript = string.Empty;
        /// <summary>
        /// 绑定适配器
        /// </summary>
        public string AdapterDescript
        {
            get { return adapterDescript; }
            set
            {
                adapterDescript = value;
                LocalIp = NetworkUtils.GetIpAddressFromNetworkDescription(adapterDescript);
                this.RaisePropertyChanged(nameof(AdapterDescript));
            }
        }
        #endregion
        #region Functions
        /// <summary>
        /// 获取能使用的串口列表
        /// </summary>
        public RelayCommand GetCanUseSerialPorts => new RelayCommand(() =>
        {
            InitSerialPortsName();
            if (SerialPortsName.Count==0) {
                MessageBoxX.Show("请检查串口是否有设备！","Tips",Application.Current.MainWindow);
            }
        });


        /// <summary>
        /// 保存配置文件按钮
        /// </summary>
        public RelayCommand SaveConfigBtn => new RelayCommand(() =>
        {
            try
            {
                Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "文本文件|*.txt"
                };
                if (dialog.ShowDialog() == true)
                {
                    string path = dialog.FileName;
                    List<string> ss = new List<string>();
                    if (File.Exists(path))
                    {
                        ss.Add(RHCONFIGFLAG);
                        ss.Add(GetSaveStr(M_MbcfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_Mbregcfg.GetAtStr()));
                        ss.Add(GetSaveStr(M_MqbrkcfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_MqheartenDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_MqsslvDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_MqtopiccfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_NipcfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_SercfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_SockcfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_WapcfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_WstacfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_DevcfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_ColmdcfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_AdbtncfgDto.GetAtStr()));
                        var ssarr = ss.ToArray();
                        File.WriteAllLines(path, ssarr);
                        MessageBoxX.Show("写入配置文件成功!", "Tips",Application.Current.MainWindow);
                    }
                    else {
                        ss.Add(RHCONFIGFLAG);
                        ss.Add(GetSaveStr(M_MbcfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_Mbregcfg.GetAtStr()));
                        ss.Add(GetSaveStr(M_MqbrkcfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_MqheartenDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_MqsslvDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_MqtopiccfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_NipcfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_SercfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_SockcfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_WapcfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_WstacfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_DevcfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_ColmdcfgDto.GetAtStr()));
                        ss.Add(GetSaveStr(M_AdbtncfgDto.GetAtStr()));
                        var ssarr = ss.ToArray();
                        File.WriteAllLines(path, ssarr);
                        ShowState = "创建且写入配置文件成功!";
                    }
                    
                    LogHelper.WriteLog("配置文件保存成功" + path);

                }
            }
            catch (Exception)
            {

                LogHelper.WriteLog("配置文件保存失败");
            }

        });
        /// <summary>
        /// 获取存储的配置字段
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string GetSaveStr(string str) {
            return str.Substring(2, str.Length - 2).Replace("=", ":");
        }
        /// <summary>
        /// 加载配置文件按钮
        /// </summary>
        public RelayCommand LoadConfigBtn => new RelayCommand(() =>
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "文本文件|*.txt"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    string path = dialog.FileName;
                    var str = File.ReadAllText(path);
                    if (str.Contains(RHCONFIGFLAG))
                    {
                        var strs = File.ReadAllLines(path);
                        foreach (var item in strs)
                        {
                            ShowTheParameters(item);
                        }
                        ShowState = "解析文件成功!";
                    }
                    else {
                        MessageBoxX.Show("文件格式不正确!", "Tips", Application.Current.MainWindow);
                    }
                    
                }
                catch (Exception ex)
                {
                    MessageBoxX.Show("解析文件异常!", "Tips", Application.Current.MainWindow);
                    LogHelper.WriteLog("解析文件异常", ex);
                }
                
              

            }
        });


        /// <summary>
        /// 生成一个激活码
        /// </summary>
        public RelayCommand CreateOneActiveCode => new RelayCommand(() =>
        {
            MessageBoxX.Show("待实现", "Tips", Application.Current.MainWindow);
        });


        /// <summary>
        /// 输入错误提示
        /// </summary>
        public void InputErrorTips() {
            MessageBoxX.Show("格式错误，请点击右方提示按钮!","Tips",Application.Current.MainWindow);
        }
        


        /// <summary>
        /// 生成Mac地址 Oc开头
        /// </summary>
        public RelayCommand CreateOneMacAddr => new RelayCommand(() =>
        {
            MacAddrStr = AutoProduceLanMacAddress();

        });


        


        /// <summary>
        /// 刷新网卡操作
        /// </summary>
        public RelayCommand FlushNetworkCard => new RelayCommand(() =>
        {
            ShowNetAdapterList();
            SerialPortsName.Clear();
            InitSerialPortsName();
        });
        /// <summary>
        /// 搜索设备依赖命令
        /// </summary>
        public RelayCommand SearchDevices => new RelayCommand(() =>
        {
            DeviceInfos.Clear();
            //获取选择网卡的Ip
            string ip = NetworkUtils.GetIpAddressFromNetworkDescription(adapterDescript);
            LocalIp = ip;
            //设置协议字符串
            string sendstr = UdpSendHead;

            if (string.IsNullOrEmpty(ip))
            {
                MessageBoxX.Show("请选择网卡", "Tips", Application.Current.MainWindow);
            }
            else
            {
                var handler = PendingBox.Show("Please wait ...", "Processing", false, Application.Current.MainWindow);
                handler.UpdateMessage("搜索设备中...");
                //开启线程同步接收1秒，3秒未收到返回数据则退出
                new Task(() =>
                {
                    //LockSomeTime = false;
                    ShowState = "开始搜索!";
                    LogHelper.WriteLog("开始搜索!");

                    UdpReceive(ip, 3000, UpdReceivePort, UdpReadHead, UdpReadTail);
                    ShowState = "搜索完成!共" + DeviceInfos.Count + "台设备(双击设备列表里面的设备即可进行配置)";
                    LogHelper.WriteLog("搜索完成!共" + DeviceInfos.Count + "台设备(双击设备列表里面的设备即可进行配置)");
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        handler.Close();
                    }));
                }).Start();
                new Task(() =>
                {
                    Utils.UdpUtils.UdpLoopSendMsg(ip, sendstr, UdpBoradCastPort, 2);
                    
                }).Start();
            }

        });
        /// <summary>
        /// 双击datagrid某一项
        /// </summary>
        public RelayCommand<DeviceInfo> DoubleClickItem => new RelayCommand<DeviceInfo>((deviceInfo) =>
        {
            if (deviceInfo != null)
            {
                RemoteIp = deviceInfo.strIpAddress;
                ShowState = $"当前配置设备IP{RemoteIp}";

            }


        });

        /// <summary>
        /// 获取参数Udp
        /// </summary>
        public RelayCommand GetParametersUdp => new RelayCommand(() =>
        {
            if (!string.IsNullOrEmpty(LocalIp) && !string.IsNullOrEmpty(RemoteIp))
            {
                var handler = PendingBox.Show("Please wait ...", "Processing", false, Application.Current.MainWindow);
                handler.UpdateMessage("获取参数中 ...");
                Task.Factory.StartNew(() =>
                {
                    UdpReceiveConfigParameters(LocalIp, 2000, UpdReceivePort);
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        handler.Close();
                    }));
                });
                //开启线程同步接收1秒，3秒未收到返回数据则退出
                new Task(async () =>
                {

                    ShowState = "获取参数中 ...";
                    M_Mbregcfg.SendAtQuery(LocalIp, RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_MbcfgDto.SendAtQuery(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_MqbrkcfgDto.SendAtQuery(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_MqheartenDto.SendAtQuery(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_MqsslvDto.SendAtQuery(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_MqtopiccfgDto.SendAtQuery(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_NipcfgDto.SendAtQuery(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_SercfgDto.SendAtQuery(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_WapcfgDto.SendAtQuery(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_SockcfgDto.SendAtQuery(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_WstacfgDto.SendAtQuery(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_DevcfgDto.SendAtQuery(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_ColmdcfgDto.SendAtQuery(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_AdbtncfgDto.SendAtQuery(LocalIp,RemoteIp, UdpBoradCastPort);
                    ShowState = "配置通道(Udp):获取数据完成！";
                }).Start();
            }
            else {
                MessageBoxX.Show("请选择网卡!", "Tips", Application.Current.MainWindow);
            }
        });


        /// <summary>
        /// 获取参数串口
        /// </summary>
        public RelayCommand GetParametersSerialPort => new RelayCommand(() =>
        {
            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    var handler = PendingBox.Show("Please wait ...", "Processing", false, Application.Current.MainWindow);
                    handler.UpdateMessage("Almost complete ...");
                    Task.Factory.StartNew(async () =>
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            ShowState = "正在接收数据..";
                            M_DevcfgDto.SendQueryAtSerialPort();
                            await Task.Delay(100);
                            M_SercfgDto.SendQueryAtSerialPort();
                            await Task.Delay(100);
                            M_Mbregcfg.SendQueryAtSerialPort();
                            await Task.Delay(100);
                            M_MbcfgDto.SendQueryAtSerialPort();
                            await Task.Delay(100);
                            M_MqbrkcfgDto.SendQueryAtSerialPort();
                            await Task.Delay(100);
                            M_MqheartenDto.SendQueryAtSerialPort();
                            await Task.Delay(100);
                            M_MqsslvDto.SendQueryAtSerialPort();
                            await Task.Delay(100);
                            M_MqtopiccfgDto.SendQueryAtSerialPort();
                            await Task.Delay(100);
                            M_NipcfgDto.SendQueryAtSerialPort();
                            await Task.Delay(100);
                            M_WapcfgDto.SendQueryAtSerialPort();
                            await Task.Delay(100);
                            M_SockcfgDto.SendQueryAtSerialPort();
                            await Task.Delay(100);
                            M_WstacfgDto.SendQueryAtSerialPort();
                            await Task.Delay(100);
                            M_ColmdcfgDto.SendQueryAtSerialPort();
                            await Task.Delay(100);
                            M_AdbtncfgDto.SendQueryAtSerialPort();
                            System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                            {
                                handler.Close();
                            }));
                        }
                        
                        ShowState = "数据接收完成!";
                        if (string.IsNullOrEmpty(LocalSerialPortTextBoxStr))
                        {
                            MessageBoxX.Show("串口未返回信息,请检查波特率是否正确！", "Tips", Application.Current.MainWindow);

                        }

                    });


                }
                else
                {
                    MessageBoxX.Show("串口未打开，请先打开串口再执行配置操作！", "Tips", Application.Current.MainWindow);
                }


            }
            catch (Exception ex)
            {

                LogHelper.WriteLog("串口未打开", ex);
            }


        });


        /// <summary>
        /// 配置设备参数Udp
        /// </summary>
        public RelayCommand ConfigParametersUdp => new RelayCommand(() =>
        {
            if (!string.IsNullOrEmpty(LocalIp) && !string.IsNullOrEmpty(RemoteIp))
            {
                var handler = PendingBox.Show("Please wait ...", "Processing", false, Application.Current.MainWindow);
                handler.UpdateMessage("配置中...");
                //M_Mbregcfg.SendAt(LocalIp, RemoteIp, UdpSendPort);
                //M_MqbrkcfgDto.SendAt(LocalIp, RemoteIp, UdpSendPort);
                //M_MqheartenDto.SendAt(LocalIp, RemoteIp, UdpSendPort);
                //M_MqsslvDto.SendAt(LocalIp, RemoteIp, UdpSendPort);
                //M_MqtopiccfgDto.SendAt(LocalIp, RemoteIp, UdpSendPort);
                //M_NipcfgDto.SendAt(LocalIp, RemoteIp, UdpSendPort);
                //M_SercfgDto.SendAt(LocalIp, RemoteIp, UdpSendPort);
                //M_WapcfgDto.SendAt(LocalIp, RemoteIp, UdpSendPort);
                //M_SockcfgDto.SendAt(LocalIp, RemoteIp, UdpSendPort);
                //M_WstacfgDto.SendAt(LocalIp, RemoteIp, UdpSendPort);
                //ShowState = "配置完成!";
                Task.Factory.StartNew(async()=> {

                    ShowState = "配置中...";
                    M_Mbregcfg.SendAt(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_MbcfgDto.SendAt(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_MqbrkcfgDto.SendAt(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_MqheartenDto.SendAt(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_MqsslvDto.SendAt(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_MqtopiccfgDto.SendAt(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_NipcfgDto.SendAt(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_SercfgDto.SendAt(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_WapcfgDto.SendAt(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_SockcfgDto.SendAt(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_WstacfgDto.SendAt(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_DevcfgDto.SendAt(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_ColmdcfgDto.SendAt(LocalIp,RemoteIp, UdpBoradCastPort);
                    await Task.Delay(100);
                    M_AdbtncfgDto.SendAt(LocalIp,RemoteIp, UdpBoradCastPort);
                    ShowState = "配置完成!";
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        handler.Close();
                    }));
                });
                
            }
            else
            {
                MessageBoxX.Show("请选择网卡且双击设备列表某一设备!", "Tips",Application.Current.MainWindow);
            }


        });
        /// <summary>
        /// 初始化串口列表
        /// </summary>
        private void InitSerialPortsName()
        {
            string[] aa = SerialPort.GetPortNames();
            foreach (var item in aa)
            {
                SerialPortsName.Add(item);
            }
        }
        public static SerialPort serialPort;
        /// <summary>
        /// 使用串口进行配置
        /// </summary>
        public RelayCommand ConfigParametersSerialPort => new RelayCommand(() =>
        {

            try
            {
                if (serialPort != null && serialPort.IsOpen)
                {

                    Task.Factory.StartNew(async () =>
                    {
                        //数据检查
                        var handler = PendingBox.Show("Please wait ...", "Processing", false, Application.Current.MainWindow);
                        handler.UpdateMessage("Almost complete ...");
                        ShowState = "配置中...";
                        M_Mbregcfg.SendAtSerialPort();
                        await Task.Delay(1000);
                        M_MbcfgDto.SendAtSerialPort();
                        await Task.Delay(1000);
                        M_MqbrkcfgDto.SendAtSerialPort();
                        await Task.Delay(1000);
                        M_MqheartenDto.SendAtSerialPort();
                        await Task.Delay(1000);
                        M_MqsslvDto.SendAtSerialPort();
                        await Task.Delay(1000);
                        M_MqtopiccfgDto.SendAtSerialPort();
                        await Task.Delay(1000);
                        M_NipcfgDto.SendAtSerialPort();
                        await Task.Delay(1000);
                        M_SercfgDto.SendAtSerialPort();
                        await Task.Delay(1000);
                        M_WapcfgDto.SendAtSerialPort();
                        await Task.Delay(1000);
                        M_SockcfgDto.SendAtSerialPort();
                        await Task.Delay(1000);
                        M_WstacfgDto.SendAtSerialPort();
                        await Task.Delay(1000);
                        M_DevcfgDto.SendAtSerialPort();
                        await Task.Delay(1000);
                        M_ColmdcfgDto.SendAtSerialPort();
                        await Task.Delay(1000);
                        M_AdbtncfgDto.SendAtSerialPort();
                        ShowState = "配置完成!";
                        if (string.IsNullOrEmpty(LocalSerialPortTextBoxStr)) {
                            ShowState = "串口未返回信息,请检查波特率是否正确！";
                        }
                        handler.Close();
                    });


                }
                else
                {
                    ShowState = "串口未打开，请先打开串口再执行配置操作！";
                }


            }
            catch (Exception ex)
            {

                LogHelper.WriteLog("串口未打开", ex);
            }



            //serialPort.Close();
        });
        /// <summary>
        /// 串口接收数据处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Post_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string str = serialPort.ReadLine();//字符串方式读
            LocalSerialPortTextBoxStr = "Rec:" + DateTime.Now + ":" + str + "\r\n" + LocalSerialPortTextBoxStr;
            ShowTheParameters(str);
        }
        /// <summary>
        /// 展示参数
        /// </summary>
        /// <param name="str"></param>
        private void ShowTheParameters(string str) {
            if (str.Contains("+DEVCFG"))
            {

                var a = AtUtils.AtAnaysis(str);
                if (a.Count == 2)
                {
                    DevConfig_DeviceId = a[0];
                    DevConfig_ActiveCode = a[1];
                }

            }
            else if (str.Contains("+WAPCFG"))
            {
                var a = AtUtils.AtAnaysis(str);
                if (a.Count == 7)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        NetConfig_AP_BanStation_Switch = EnConvertStr(a[1]);
                        NetConfig_AP_SafeType = a[2];
                    }));
                    NetConfig_AP_NetCardName = a[0];

                    NetConfig_AP_DhcpStartAddr = a[3];
                    NetConfig_AP_DhcpEndAddr = a[4];
                    NetConfig_AP_UserName = a[5];
                    NetConfig_AP_Password = a[6];
                }

            }
            else if (str.Contains("+WSTACFG"))
            {
                var a = AtUtils.AtAnaysis(str);
                if (a.Count == 6)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        NetConfig_STA_BanStation_Switch = EnConvertStr(a[1]);
                        NetConfig_STA_SafeType = a[2];
                        NetConfig_STA_DhcpBanStation_Switch =EnConvertStr(a[3]);
                    }));
                    NetConfig_STA_NetCardName = a[0];

                    NetConfig_STA_UserName = a[4];
                    NetConfig_STA_Password = a[5];
                }

            }
            else if (str.Contains("+ENETCFG"))
            {
                var a = AtUtils.AtAnaysis(str);
                if (a.Count == 7)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        NetConfig_IP_NetType = a[1];
                    }));
                    NetConfig_IP_NetCardName = a[0];

                    NetConfig_IP_Addr = a[2];
                    NetConfig_IP_NetMask = a[3];
                    NetConfig_IP_GateWay = a[4];
                    NetConfig_IP_Dns = a[5];
                    MacAddrStr = a[6];
                }

            }
            else if (str.Contains("+SOCKCFG"))
            {
                var a = AtUtils.AtAnaysis(str);
                if (a.Count == 9)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        NetConfig_Socket_BanStation_Switch =EnConvertStr(a[2]);
                        NetConfig_Socket_AgreementType = a[3];
                    }));
                    NetConfig_Socket_NetCardName = a[0];
                    NetConfig_Socket_Number = a[1];

                    NetConfig_Socket_LocalPort = a[4];
                    NetConfig_Socket_RemoteIpAddr = a[5];
                    NetConfig_Socket_RemotePort = a[6];
                    NetConfig_Socket_ServerConnections = a[7];
                    NetConfig_Socket_BufferSize = a[8];
                }

            }
            else if (str.Contains("+SERCFG"))
            {

                var a = AtUtils.AtAnaysis(str);
                if (a.Count == 7)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        SerialPortConfig_SerialPortNumber = a[0];
                        SerialPortConfig_BaudRate = a[1];
                        SerialPortConfig_DataStation = a[2];
                        SerialPortConfig_StopStation = a[3];
                        SerialPortConfig_CheckStation = a[4];
                        SerialPortConfig_HardControlStation = a[5];
                    }));

                    SerialPortConfig_BufferSize = a[6];
                }

            }
            else if (str.Contains("+MBCFG"))
            {
                var a = AtUtils.AtAnaysis(str);
                if (a.Count == 6)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        ModbusConfig_Addr_Number = a[0];
                        ModbusConfig_Addr_BanStation_Switch =EnConvertStr(a[1]);
                        ModbusConfig_Addr_SlaveOrMaster = a[3];
                        ModbusConfig_Addr_CommunicationMode = a[4];
                    }));

                    ModbusConfig_Addr_Addr = a[2];

                    ModbusConfig_Addr_Scantime = a[5];
                }

            }
            else if (str.Contains("+MBREGCFG"))
            {

                var a = AtUtils.AtAnaysis(str);
                if (a.Count == 9)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        ModbusConfig_REG_Number = a[0];
                    }));

                    ModbusConfig_REG_CoilAddr = a[1];
                    ModbusConfig_REG_CoilNumber = a[2];
                    ModbusConfig_REG_DiscAddr = a[3];
                    ModbusConfig_REG_DiscNumber = a[4];
                    ModbusConfig_REG_KeepAddr = a[5];
                    ModbusConfig_REG_KeepNumber = a[6];
                    ModbusConfig_REG_InputAddr = a[7];
                    ModbusConfig_REG_InputNumber = a[8];
                }

            }
            else if (str.Contains("+MQBRKCFG"))
            {
                var a = AtUtils.AtAnaysis(str);
                if (a.Count == 6)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MQTTConfig_AddrAndPort_Identifier = a[0];
                    }));

                    MQTTConfig_AddrAndPort_RemoteAddr = a[1];
                    MQTTConfig_AddrAndPort_RemotePort = a[2];
                    MQTTConfig_AddrAndPort_ClientId = a[3];
                    MQTTConfig_AddrAndPort_BrokerUserName = a[4];
                    MQTTConfig_AddrAndPort_BrokerPassword = a[5];
                }

            }
            else if (str.Contains("+MQSSLV"))
            {
                var a = AtUtils.AtAnaysis(str);
                if (a.Count == 2)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MQTTConfig_SSL_BanStaiton = a[0];
                    }));

                    MQTTConfig_SSL_VersionNumber = a[1];
                }

            }
            else if (str.Contains("+MQHEARTCFG"))
            {

                var a = AtUtils.AtAnaysis(str);
                if (a.Count == 2)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MQTTConfig_RunHeartJumpPackage_BanStation = a[0];
                    }));

                    MQTTConfig_RunHeartJumpPackage_SendData = a[1];
                }

            }
            else if (str.Contains("+MQTOPICCFG"))
            {
                var a = AtUtils.AtAnaysis(str);
                if (a.Count == 3)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        MQTTConfig_Dev_InfoTopicNumber_No = a[0];
                    }));

                    MQTTConfig_Dev_PublicInfoTopic = a[1];
                    MQTTConfig_Dev_SubscribeInfoTopic = a[2];
                }

            }
            else if (str.Contains("+ADBTNCFG"))
            {
                var a = AtUtils.AtAnaysis(str);
                if (a.Count == 3)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        AdButtonParaConfig_ButtonNum = a[0];
                        AdButtonParaConfig_ButtonRunPattern = a[1];
                    }));

                    AdButtonParaConfig_LedmodeTimeout = a[2];
                }

            }
            else if (str.Contains("+COLMDCFG"))
            {
                var a = AtUtils.AtAnaysis(str);
                if (a.Count == 4)
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        GatherPatternConfig_DataUpLoadPattern = a[0];
                    }));

                    GatherPatternConfig_TimerControllerTriggerTime = a[1];
                    GatherPatternConfig_HeartTriggerTime = a[2];
                    GatherPatternConfig_HeratJumpData = a[3];
                }

            }
        }


        #endregion

        #region 关于提示按钮的代码绑定

        /// <summary>
        /// 配置 提示
        /// </summary>
        public RelayCommand Tips_GatherPatternConfig_HeratJumpData => new RelayCommand(() =>
        {
            MessageBoxX.Show("心跳数据\r\n字符串，最大长度 50", "Tips", Application.Current.MainWindow);
        });


        /// <summary>
        /// 配置 提示
        /// </summary>
        public RelayCommand Tips_GatherPatternConfig_HeartTriggerTime => new RelayCommand(() =>
        {
            MessageBoxX.Show("采集器心跳时间 10~1000 秒\r\n作为采集器空闲时的心跳时间，\r\n心跳时间到后，采集器主动上传\r\n数据到服务器", "Tips", Application.Current.MainWindow);
        });


        /// <summary>
        /// 配置 提示
        /// </summary>
        public RelayCommand Tips_GatherPatternConfig_TimerControllerTriggerTime => new RelayCommand(() =>
        {
            MessageBoxX.Show(" 定时器数据监控/触发时间：\r\n100~100000ms\r\n当 mode = 0 时。该定时值为两次上\r\n传的最小时间间隔。\r\n当 mode = 1 时，该值为每次主动上\r\n传的时间间隔。", "Tips", Application.Current.MainWindow);
        });


        /// <summary>
        /// 配置 提示
        /// </summary>
        public RelayCommand Tips_GatherPatternConfig_DataUpLoadPattern => new RelayCommand(() =>
        {
            MessageBoxX.Show("采集器数据上传模式：\r\n整型，范围 0 - 2\r\n0：基于事件触发模式，当采集器\r\n的采集参数发生变化时上传数据\r\n1：基于定时上传模式。即采集器\r\n在定时时间到后，主动上传一批\r\n数据\r\n2 基于主从查询模式。该模式下事\r\n件触发及定时上传取消。\r\n注：主从查询模式在 0, 1 模式下\r\n依旧存在 ", "Tips", Application.Current.MainWindow);
        });


        /// <summary>
        /// 配置 提示
        /// </summary>
        public RelayCommand Tips_AdButtonParaConfig_LedmodeTimeout => new RelayCommand(() =>
        {
            MessageBoxX.Show("Ledmode 的延时时间: \r\n整形，范围 0 - 65535", "Tips", Application.Current.MainWindow);
        });


        /// <summary>
        /// 配置 提示
        /// </summary>
        public RelayCommand Tips_AdButtonParaConfig_ButtonRunPattern => new RelayCommand(() =>
        {
            MessageBoxX.Show("按钮灯运行模式 \r\n0.状态锁定模式 / 手动复位模式\r\n（按钮按下后变更状态，且不\r\n自动恢复）\r\n 1.点动模式，自动复位模式（按\r\n下后灯亮，松开后灯灭）\r\n 2.延时模式，延时复位（按钮按\r\n下后延时一定时间后执行灯\r\n状态变更）\r\n ", "Tips", Application.Current.MainWindow);
        });


        /// <summary>
        /// 配置 提示
        /// </summary>
        public RelayCommand Tips_AdButtonParaConfig_ButtonNum => new RelayCommand(() =>
        {
            MessageBoxX.Show("按钮编号： \r\n整型，范围 0 - 6 ", "Tips", Application.Current.MainWindow);
        });


        /// <summary>
        /// 配置 提示
        /// </summary>
        public RelayCommand Tips_DevConfig_ActiveCode => new RelayCommand(() =>
        {
            MessageBoxX.Show("激活码\r\n字符串类型，最长 20 字节\r\n该参数具有特殊性。\r\n读：返回芯片唯一 ID，\r\n写：根据唯一 ID 生成的激活码", "Tips", Application.Current.MainWindow);
        });


        /// <summary>
        /// 配置 提示
        /// </summary>
        public RelayCommand Tips_DevConfig_DeviceId => new RelayCommand(() =>
        {
            MessageBoxX.Show("设备 ID:\r\n字符串类型, 最长 20 字节", "Tips", Application.Current.MainWindow);
        });


        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_SerialPortConfig_BufferSize => new RelayCommand(() =>
        {
            MessageBoxX.Show("缓冲区长度\r\n整型，范围 64 - 1096", "Tips", Application.Current.MainWindow);
        });


        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_AP_NetCardName => new RelayCommand(() =>
        {
            MessageBoxX.Show("网卡名称\r\n字符串类型，最长八个字节", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_AP_BanStation => new RelayCommand(() =>
        {
            MessageBoxX.Show("使能位\r\n0  禁能\r\n1 使能", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_AP_SafeType => new RelayCommand(() =>
        {
            MessageBoxX.Show("网络安全类型\r\n0 WPA2-PSK \r\n1 WPA2", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_AP_UserName => new RelayCommand(() =>
        {
            MessageBoxX.Show("无线AP账号\r\n字符串类型，最长16byte", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_AP_Password => new RelayCommand(() =>
        {
            MessageBoxX.Show("无线AP账号\r\n字符串类型，最长16byte", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_AP_DhcpStartAddr => new RelayCommand(() =>
        {
            MessageBoxX.Show("DHCP起始地址\r\n整型，范围（2-254）", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_AP_DhcpEndAddr => new RelayCommand(() =>
        {
            MessageBoxX.Show("DHCP终止地址\r\n整型，范围（2-254）", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_STA_NetCardName => new RelayCommand(() =>
        {
            MessageBoxX.Show("网卡名称\r\n字符串类型，最长8byte", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_STA_BanStation => new RelayCommand(() =>
        {
            MessageBoxX.Show("使能位\r\n0 禁能\r\n1 使能", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_STA_SafeType => new RelayCommand(() =>
        {
            MessageBoxX.Show("网络安全类型\r\n0 WPA2-PSK\r\n1 WPA2", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_STA_DhcpBanStation => new RelayCommand(() =>
        {
            MessageBoxX.Show("dhcp 功能使能\r\n0 禁能 \r\n1 使能", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_STA_UserName => new RelayCommand(() =>
        {
            MessageBoxX.Show("无线AP账号\r\n字符串类型,最长 16Byte", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_STA_Password => new RelayCommand(() =>
        {
            MessageBoxX.Show("无线AP密码\r\n字符串类型,最长 16Byte", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_IP_NetCardName => new RelayCommand(() =>
        {
            MessageBoxX.Show("网卡名称\r\n字符串类型, 最长 8 字节", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_IP_NetType => new RelayCommand(() =>
        {
            MessageBoxX.Show("网络模式\r\n0 LAN\r\n1 WAN", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_IP_Addr => new RelayCommand(() =>
        {
            MessageBoxX.Show("IP地址\r\n例如:192.168.0.2", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_IP_NetMask => new RelayCommand(() =>
        {
            MessageBoxX.Show("子网掩码\r\n例如:255.255.255.0", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_IP_GateWay => new RelayCommand(() =>
        {
            MessageBoxX.Show("网关\r\n例如:192.168.0.1", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_IP_Dns => new RelayCommand(() =>
        {
            MessageBoxX.Show("DNS", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_MacAddrStr => new RelayCommand(() =>
        {
            MessageBoxX.Show("MAC地址,这里是由按钮自动生成", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Socket_NetCardName => new RelayCommand(() =>
        {
            MessageBoxX.Show("网卡名称\r\n字符串类型, 最长 8 字节", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Socket_Number => new RelayCommand(() =>
        {
            MessageBoxX.Show("Socket 编号整型\r\n范围: 0 - 5", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Socket_BanStation => new RelayCommand(() =>
        {
            MessageBoxX.Show("使能位\r\n0 禁能(default)\r\n1 使能", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Socket_AgreementType => new RelayCommand(() =>
        {
            MessageBoxX.Show("Socket 协议类型\r\n0.TCPServer\r\n1.TCPClient", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Socket_LocalPort => new RelayCommand(() =>
        {
            MessageBoxX.Show("TCP/UDP 本地端口\r\n整型, 范围: 500 - 65535", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Socket_RemoteIpAddr => new RelayCommand(() =>
        {
            MessageBoxX.Show("TCP/UDP 远程服务器 IP 地址\r\n字符串类型，最长 15 字节", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Socket_RemotePort => new RelayCommand(() =>
        {
            MessageBoxX.Show("TCP/UDP 远程服务器端口\r\n整型, 范围: 500 - 65535", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Socket_ServerConnections => new RelayCommand(() =>
        {
            MessageBoxX.Show("Server 最长连接数\r\n整型, 范围: 0 - 10", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Socket_BufferSize => new RelayCommand(() =>
        {
            MessageBoxX.Show("Socket 缓冲区长度\r\n整型, 范围: 100~65536", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_SerialPortNumber => new RelayCommand(() =>
        {
            MessageBoxX.Show("串口编号\r\n整型, 范围: 0 - 5", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_BaudRate => new RelayCommand(() =>
        {
            MessageBoxX.Show("串口波特率\r\n整型, 范围: 1200 - 230400", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_DataStation => new RelayCommand(() =>
        {
            MessageBoxX.Show("串口数据为，目前只支持 8\r\n整型, 范围: 8", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_StopStation => new RelayCommand(() =>
        {
            MessageBoxX.Show("串口停止位 ，目前只支持 1\r\n整型, 范围: 1", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_CheckStation => new RelayCommand(() =>
        {
            MessageBoxX.Show("串口校验位\r\n0：无校验\r\n1：奇校验\r\n2：偶校验", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_HardControlStation => new RelayCommand(() =>
        {
            MessageBoxX.Show("串口硬件流控制位\r\n0：禁能\r\n1：使能", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Addr_Number => new RelayCommand(() =>
        {
            MessageBoxX.Show("Modbus 编号\r\n整型, 范围: 0 - 5", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Addr_BanStation => new RelayCommand(() =>
        {
            MessageBoxX.Show("Modbus 使能位\r\n0.禁能\r\n1.使能", "Tips", Application.Current.MainWindow);
        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Addr_Addr => new RelayCommand(() =>
        {
            MessageBoxX.Show("Modbus 地址\r\n整型, 范围: 1 - 254", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Addr_SlaveOrMaster => new RelayCommand(() =>
        {
            MessageBoxX.Show("Modbus 主从模式\r\n0.Slave\r\n1.Master", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Addr_CommunicationMode => new RelayCommand(() =>
        {
            MessageBoxX.Show("Modbus 模式\r\n0.ModbusRtu\r\n1.ModbusTcp", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Addr_Scantime => new RelayCommand(() =>
        {
            MessageBoxX.Show("Modbus 扫描时间\r\n整型, 范围: 100 - 50000单位 ms", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_REG_Number => new RelayCommand(() =>
        {
            MessageBoxX.Show("Modbus 编号\r\n整型, 范围: 0 - 5", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_REG_CoilAddr => new RelayCommand(() =>
        {
            MessageBoxX.Show("Modbus 线圈寄存器地址\r\n整型, 范围: 0 - 9999", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_REG_CoilNumber => new RelayCommand(() =>
        {
            MessageBoxX.Show("Modbus 线圈寄存器数量\r\n整型, 范围: 0 - 9999", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_REG_DiscAddr => new RelayCommand(() =>
        {
            MessageBoxX.Show("Modbus 输入离散量寄存器地址\r\n整型, 范围: 10000 - 19999", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_REG_DiscNumber => new RelayCommand(() =>
        {
            MessageBoxX.Show("Modbus 输入离散量寄存器数量\r\n整型, 范围: 0 - 9999", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_REG_KeepAddr => new RelayCommand(() =>
        {
            MessageBoxX.Show("Modbus 保持寄存器地址\r\n整型, 范围: 40000 - 49999", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_REG_KeepNumber => new RelayCommand(() =>
        {
            MessageBoxX.Show("Modbus 保持寄存器数量\r\n整型, 范围: 0 - 9999", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_REG_InputAddr => new RelayCommand(() =>
        {
            MessageBoxX.Show("Modbus 输入寄存器地址\r\n整型, 范围: 30000 - 39999", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_REG_InputNumber => new RelayCommand(() =>
        {
            MessageBoxX.Show("Modbus 输入寄存器数量\r\n整型, 范围: 0 - 9999", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_AddrAndPort_Identifier => new RelayCommand(() =>
        {
            MessageBoxX.Show("MQTT 客户端标识符。\r\n整型, 范围: 0~5。", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_AddrAndPort_RemoteAddr => new RelayCommand(() =>
        {
            MessageBoxX.Show("MQTT Broker 服务器地址\r\n字符串类型, 最大长度: 100 字节", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_AddrAndPort_RemotePort => new RelayCommand(() =>
        {
            MessageBoxX.Show("MQTT Broker 服务器端口\r\n整型, 范围: 1~65535", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_AddrAndPort_ClientId => new RelayCommand(() =>
        {
            MessageBoxX.Show("MQTT 客户端 ID\r\n字符串类型。最大长度: 50 字节", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_AddrAndPort_BrokerUserName => new RelayCommand(() =>
        {
            MessageBoxX.Show("MQTT Broker 用户名\r\n字符串类型。最大长度: 50 字节", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_AddrAndPort_BrokerPassword => new RelayCommand(() =>
        {
            MessageBoxX.Show("MQTT Broker 密码\r\n字符串类型。最大长度: 50 字节", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_SSL_BanStaiton => new RelayCommand(() =>
        {
            MessageBoxX.Show("使能 SSL 功能\r\n0.禁止 SSL 功能\r\n1.使能 SSL 功能", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_SSL_VersionNumber => new RelayCommand(() =>
        {
            MessageBoxX.Show("ver::SSL 协议版本:\r\nfloat 型，最长 8 字节", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_RunHeartJumpPackage_BanStation => new RelayCommand(() =>
        {
            MessageBoxX.Show("使能心跳包功能\r\n0.禁止心跳包功能\r\n1.使能心跳包功能", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        ///配置提示
        /// </summary>
        public RelayCommand Tips_RunHeartJumpPackage_SendData => new RelayCommand(() =>
        {
            MessageBoxX.Show("心跳包发送数据\r\n字符串类型，最长 100\r\n单位: 秒", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Dev_InfoTopicNumber_No => new RelayCommand(() =>
        {
            MessageBoxX.Show("消息主题编号 \r\n整型，范围 0 - 5 ", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Dev_PublicInfoTopic => new RelayCommand(() =>
        {
            MessageBoxX.Show("MQTT 发布主题内容 \r\n字符串，最大长度 50", "Tips", Application.Current.MainWindow);

        });
        /// <summary>
        /// 配置提示
        /// </summary>
        public RelayCommand Tips_Dev_SubscribeInfoTopic => new RelayCommand(() =>
        {
            MessageBoxX.Show("MQTT 订阅主题内容 \r\n字符串，最大长度 50", "Tips", Application.Current.MainWindow);

        });
        #endregion
        #region 校验


        /// <summary>
        /// 
        /// </summary>
        public RelayCommand DevConfig_DeviceId_Check => new RelayCommand(() =>
        {
            if (DevConfig_DeviceId.Equals("123456"))
            {
                
            }
            else {
                //MessageBoxX.Show("长度应该小于八", "Tips", Application.Current.MainWindow);
            }
            
        });


        /// <summary>
        /// 
        /// </summary>
        public RelayCommand NetConfig_AP_NetCardName_xName_TextChanged => new RelayCommand(() =>
        {
            if (netConfig_AP_NetCardName.Length>8) {
                //MessageBoxX.Show("长度应该小于八", "Tips", Application.Current.MainWindow);
            }
            
        });





        #endregion
        #region 单个配置功能

        /// <summary>
        /// 网络配置-无线AP配置-网卡名称配置
        /// </summary>
        public RelayCommand Tips_AP_NetCardName_SingleBtn => new RelayCommand(() =>
        {
            if (ConfigMode == 0) {

            } else if(ConfigMode==1){

            }
        });


        #endregion
    }
}