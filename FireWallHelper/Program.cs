using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using WindowsFirewallHelper;
using WindowsFirewallHelper.FirewallRules;

namespace FireWallHelper
{
    class Program
    {
        static string ClashDotNet = "Clash .NET";
        static readonly string[] ProgramPath =
        {
            "ClashDotNet.exe",
            "bin/Clash.exe",
            "bin/Tun2socks.exe",
            "bin/Redirector.exe",
            "bin/Pcap2socks.exe",
        };

        static void Main(string[] args)
        {
            if (!IsAdministrator())
            {
                return;
            }

            RemoveFirewallRules();

            foreach (var p in ProgramPath)
            {
                var path = Path.GetFullPath(p);
                if (File.Exists(path))
                    AddFirewallRule(ClashDotNet, path);
            }
        }

        static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void RemoveFirewallRules()
        {
            if (!FirewallWAS.IsSupported)
                return;

            try
            {
                foreach (var rule in FirewallManager.Instance.Rules.Where(r => r.Name == ClashDotNet))
                    FirewallManager.Instance.Rules.Remove(rule);
            }
            catch (Exception e)
            {

            }
        }

        #region 封装

        static void AddFirewallRule(string ruleName, string exeFullPath)
        {
            var rule = new FirewallWASRule(ruleName,
                exeFullPath,
                FirewallAction.Allow,
                FirewallDirection.Inbound,
                FirewallProfiles.Private | FirewallProfiles.Public | FirewallProfiles.Domain);

            FirewallManager.Instance.Rules.Add(rule);
        }

        #endregion
    }
}
