using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFirewallHelper;

namespace ClashDotNetFramework.Controllers
{
    public static class FireWallController
    {
        public static string ClashDotNetFramework = "ClashDotNetFramework";

        public static void AddFireWallRules()
        {
            if (!FirewallWAS.IsSupported)
            {
                return;
            }

            var rule = FirewallManager.Instance.Rules.FirstOrDefault(r => r.Name == ClashDotNetFramework);
            if (rule != null)
            {
                if (rule.ApplicationName.StartsWith(Global.ClashDotNetFrameworkDir))
                    return;
            }

            try
            {
                Process proc = new Process
                {
                    StartInfo =
                    {
                        FileName = Path.GetFullPath($"bin\\FireWallHelper.exe"),
                        WorkingDirectory = $"{Global.ClashDotNetFrameworkDir}\\bin",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = true,
                        Verb = "runas"
                    }
                };
                proc.Start();
            }
            catch
            {
            }
        }
    }
}
