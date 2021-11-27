using NetFilterHelper.Utils;
using NetFilterHelper.Enums;
using nfapinet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace NetFilterHelper
{
    class Program
    {
        static ServiceController NFService = new ServiceController("netfilter2");
        static string BinDriver = string.Empty;
        static string SystemDriver = $"{Environment.SystemDirectory}\\drivers\\netfilter2.sys";

        static int Main(string[] args)
        {
            if (!IsAdministrator())
            {
                return (int)ResultCode.NoPermission;
            }

            if (args.Length == 1)
            {
                // 在这里初始化
                Init();

                string command = args[0];
                switch (command)
                {
                    case "install":
                        return InstallDriver();
                    case "uninstall":
                        return UninstallDriver();
                    case "reinstall":
                        UninstallDriver();
                        return InstallDriver();
                    default:
                        return (int)ResultCode.NotSupport;
                }
            }
            else
            {
                return (int)ResultCode.NotSupport;
            }
        }

        static void Init()
        {
            string fileName;
            switch ($"{Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Minor}")
            {
                case "10.0": // Win 10
                case "6.3":  // Win 8
                case "6.2":  // Win 8
                case "6.1":  // Win 7
                case "6.0":  // Win 7
                    if (SystemHelper.Is64BitOperatingSystem())
                        fileName = "netfilter2-amd64.sys";
                    else
                        fileName = "netfilter2-i386.sys";
                    break;
                default:
                    return;
            }

            BinDriver = Path.Combine(Directory.GetCurrentDirectory(), $"{fileName}");
        }

        static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
        }

        static int ValueWithCode(IntPtr wow64Value, ResultCode resultCode)
        {
            // 重新启用重定向
            SystemHelper.Wow64RevertWow64FsRedirection(wow64Value);
            // 返回对应值
            return (int)resultCode;
        }

        static int InstallDriver()
        {
            IntPtr wow64Value = IntPtr.Zero;

            // 禁用重定向
            SystemHelper.Wow64DisableWow64FsRedirection(ref wow64Value);

            try
            {
                File.Copy(BinDriver, SystemDriver);
            }
            catch (Exception)
            {
                return ValueWithCode(wow64Value, ResultCode.CopyFail);
            }

            // 注册驱动文件
            var result = NFAPI.nf_registerDriver("netfilter2");
            if (result != NF_STATUS.NF_STATUS_SUCCESS)
            {
                return ValueWithCode(wow64Value, ResultCode.CopyFail);
            }

            return ValueWithCode(wow64Value, ResultCode.Success);
        }

        static int UninstallDriver()
        {
            IntPtr wow64Value = IntPtr.Zero;

            // 禁用重定向
            SystemHelper.Wow64DisableWow64FsRedirection(ref wow64Value);

            try
            {
                if (NFService.Status == ServiceControllerStatus.Running)
                {
                    NFService.Stop();
                    NFService.WaitForStatus(ServiceControllerStatus.Stopped);
                }
            }
            catch (Exception)
            {
                // ignored
            }

            if (!File.Exists(SystemDriver))
                return ValueWithCode(wow64Value, ResultCode.Success);

            NFAPI.nf_unRegisterDriver("netfilter2");
            File.Delete(SystemDriver);

            return ValueWithCode(wow64Value, ResultCode.Success);
        }
    }
}
