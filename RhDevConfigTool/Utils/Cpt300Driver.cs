using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SuperSocket;
using RhDevConfigTool.Model;
using SuperSocket.ProtoBase;
using System.Buffers;
using SuperSocket.Server;
using SuperSocket.Channel;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace RhDevConfigTool.Utils
{
    public class Cpt300Driver
    {
        IHost host = null;
        public static List<ZjShCpt300Dto> Cpt300Dtos { get; set; }
        public Action<string, bool> Cpt300Gather;
        private static List<SuperSocket.IAppSession> Sessions { get; set; } = new List<IAppSession>();
        private Timer timer = null;
        public static Action Gather_Action;//声明事件
        public static string Cpt300ReadInputCountStr = "{\"stJsoncmd\":{\"iId\":3,\"strCmd\":\"CmdReadDigitalInput\"},\"stDigitalInputPar\":{\"iOffset\":0,\"iNumber\":4}}##";//读取CPT300 输入计数 In3
        public Cpt300Driver()
        {
           
        }
        public void DisConnect() {
            host.Dispose();
        }
        public void Connect() {
            //定时器
            timer = new Timer(new TimerCallback(Execute), null, 2000, 500);
            Gather_Action += Gather;
            Cpt300Dtos = new List<ZjShCpt300Dto>();
            Task.Factory.StartNew(()=> {
                host = SuperSocketHostBuilder.Create<TextPackageInfo, Cpt300LinePipelineFilter>().UsePackageHandler(async (s, p) =>
                {
                    try
                    {
                        if (p.Text.Contains("stDigitalInputPar"))
                        {
                            try
                            {
                                Cpt300Logic(p.Text + "}}", s);

                            }
                            catch (Exception ex)
                            {

                                Console.WriteLine(ex.Message);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }


                    await new ValueTask();
                }).UseSession<CptAppSession>().ConfigureSuperSocket(options =>//配置服务器如服务器名和监听端口等基本信息
                {
                    options.Name = "Cpt300 Server";
                    options.ReceiveBufferSize = 2048;
                    options.Listeners = new List<ListenOptions>(){
                        new ListenOptions{
                         Ip="Any",
                         Port = 17748
                        }
                          };
                }).Build();

                host.Run();

            });
            
            
        }

        private void Cpt300Logic(string str, IAppSession s)
        {
            foreach (var item in Cpt300Dtos)
            {
                if (s.RemoteEndPoint.ToString().Split(':')[0].Equals(item.Ip))
                {
                    JObject jo = (JObject)JsonConvert.DeserializeObject(str);
                    int countvalue = 0;
                    try
                    {
                        countvalue = int.Parse(jo["stDigitalInputPar"]["ulKeyPushCount"][2].ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"转换异常{ex.Message}");
                    }
                    item.PreCount = item.CurrentCount;
                    item.CurrentCount = countvalue;
                    if (item.CurrentCount - item.PreCount == 1)
                    {
                        Cpt300Gather?.Invoke(s.RemoteEndPoint.ToString().Split(':')[0], true);
                    }
                    else if (item.CurrentCount - item.PreCount > 1)
                    {
                        Cpt300Gather?.Invoke(s.RemoteEndPoint.ToString().Split(':')[0], false);
                    }
                }
            }

        }
        public static void Execute(object o)
        {

            //Console.WriteLine("just run now");
            Gather_Action?.Invoke();
        }

        /// <summary>
        /// 需要定时执行的方法
        /// </summary>
        public static void Gather()
        {
            //Console.WriteLine("定时采集");
            if (Sessions != null)
            {
                try
                {
                    foreach (var item in Sessions)
                    {
                        item.SendAsync(Encoding.ASCII.GetBytes(Cpt300ReadInputCountStr));
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }


            }
        }

        public class Cpt300LinePipelineFilter : TerminatorPipelineFilter<TextPackageInfo>
        {
            protected Encoding Encoding { get; private set; }

            public Cpt300LinePipelineFilter()
                : this(Encoding.ASCII)
            {

            }

            public Cpt300LinePipelineFilter(Encoding encoding)
                : base(new[] { (byte)'}', (byte)'}' })
            {
                Encoding = encoding;
            }

            protected override TextPackageInfo DecodePackage(ref ReadOnlySequence<byte> buffer)
            {
                return new TextPackageInfo { Text = buffer.GetString(Encoding) };
            }
        }

        public class CptAppSession : AppSession
        {
            protected override ValueTask OnSessionConnectedAsync()
            {
                Sessions.Add(this);
                Cpt300Dtos.Add(new ZjShCpt300Dto()
                {
                    Ip = this.RemoteEndPoint.ToString().Split(':')[0],
                    PreCount = 0,
                    CurrentCount = 0
                });
                return base.OnSessionConnectedAsync();
            }

            protected override async ValueTask OnSessionClosedAsync(CloseEventArgs e)
            {

                var ip = this.RemoteEndPoint.ToString().Split(':')[0];
                Sessions.Remove(this);
                await base.OnSessionClosedAsync(e);
                foreach (var item in Cpt300Dtos.ToArray())
                {

                    if (ip.Equals(item.Ip))
                    {
                        Cpt300Dtos.Remove(item);
                    }
                }
            }
        }
    }
}
