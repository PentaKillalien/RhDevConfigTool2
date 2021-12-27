
using GalaSoft.MvvmLight.Ioc;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhDevConfigTool.ViewModel
{
    public class ViewModelLocator
    {
        SimpleIoc m_SimpleIoc = SimpleIoc.Default;
        public ViewModelLocator()
        {

            m_SimpleIoc.Register<MainViewModel>();
            m_SimpleIoc.Register<UserSelectViewModel>();
            m_SimpleIoc.Register<SensorViewModel>();
        }

        public MainViewModel Main
        {
            get
            {
                return m_SimpleIoc.GetInstance<MainViewModel>();
            }
        }
        public UserSelectViewModel UserSelect
        {
            get
            {
                return m_SimpleIoc.GetInstance<UserSelectViewModel>();
            }
        }
        public SensorViewModel Sensor
        {
            get
            {
                return m_SimpleIoc.GetInstance<SensorViewModel>();
            }
        }
    }
}
