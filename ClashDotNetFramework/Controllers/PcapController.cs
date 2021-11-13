using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashDotNetFramework.Controllers
{
    public class PcapController : Guard
    {
        public override string MainFile { get; protected set; } = "Pcap2socks.exe";

        public override string Name { get; } = "Pcap2socks";

        public void Start()
        {

        }

        public override void Stop()
        {
            StopInstance();
        }
    }
}
