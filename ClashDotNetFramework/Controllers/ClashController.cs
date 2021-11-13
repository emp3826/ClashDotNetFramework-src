using ClashDotNetFramework.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashDotNetFramework.Controllers
{
    public class ClashController : Guard
    { 
        public ClashController()
        {
            RedirectStd = true;
        }

        public override string MainFile { get; protected set; } = "Clash.exe";

        public override string Name { get; } = "Clash";

        public void Start()
        {
            int port = PortHelper.GetAvailablePort(PortType.TCP);
            Global.ClashControllerPort = port;
            string arguments = $"-ext-ctl 127.0.0.1:{port}";
            StartInstanceAuto(arguments);
        }

        public void Start(int port)
        {
            PortHelper.CheckPort(Convert.ToUInt16(port), PortType.TCP);
            Global.ClashControllerPort = port;
            string arguments = $"-ext-ctl 127.0.0.1:{port}";
            StartInstanceAuto(arguments);
        }

        public override void Stop()
        {
            StopInstance();
        }
    }
}
