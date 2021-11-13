using ClashDotNetFramework.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ClashDotNetFramework.Controllers
{
    public class NFController : Guard
    {
        private readonly string BinDriver = string.Empty;
        private readonly string SystemDriver = $"{Environment.SystemDirectory}\\drivers\\netfilter2.sys";

        public NFController()
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
                    Logging.Error($"不支持的系统版本：{Environment.OSVersion.Version}");
                    return;
            }

            BinDriver = Path.Combine(Global.ClashDotNetFrameworkDir, $"bin\\{fileName}");
            RedirectStd = true;
        }

        public override string MainFile { get; protected set; } = "Redirector.exe";

        public override string Name { get; } = "Redirector";

        #region NetFilter Controller

        public void Start()
        {
            if (!CheckDriver())
                return;
            try
            {
                _ = Task.Run(FireWallController.AddFireWallRules);
                string arguments = $"-m {string.Join(",", Global.Settings.RedirectTraffic)} -l {Global.Settings.ProcessMode} -b {string.Join(",", Global.Settings.BypassType)} -p {string.Join(",", Global.Settings.Processes)} -r 127.0.0.1:{Global.ClashMixedPort}";
                StartInstanceAuto(arguments);
            }
            catch
            {
            }
        }

        public override void Stop()
        {
            StopInstance();
        }

        #endregion

        #region NetFilter Driver

        /// <summary>
        /// 检查 NF 驱动
        /// </summary>
        private bool CheckDriver()
        {
            var binFileVersion = Utils.Utils.GetFileVersion(BinDriver);
            var systemFileVersion = Utils.Utils.GetFileVersion(SystemDriver);

            Logging.Info("内置驱动版本: " + binFileVersion);
            Logging.Info("系统驱动版本: " + systemFileVersion);

            if (!File.Exists(BinDriver))
            {
                Logging.Warning("内置驱动不存在");
                if (File.Exists(SystemDriver))
                {
                    Logging.Warning("使用系统驱动");
                    return true;
                }

                Logging.Error("未安装驱动");
                return false;
            }

            if (!File.Exists(SystemDriver))
            {
                return InstallDriver();
            }

            var reinstallFlag = false;
            if (Version.TryParse(binFileVersion, out var binResult) && Version.TryParse(systemFileVersion, out var systemResult))
            {
                if (binResult.CompareTo(systemResult) > 0)
                    // Bin greater than Installed
                    reinstallFlag = true;
                else if (systemResult.Major != binResult.Major)
                    // Installed greater than Bin but Major Version Difference (has breaking changes), do downgrade
                    reinstallFlag = true;
            }
            else
            {
                if (!systemFileVersion.Equals(binFileVersion))
                    reinstallFlag = true;
            }

            if (!reinstallFlag)
                return true;

            return ReinstallDriver();
        }

        /// <summary>
        /// 安装 NF 驱动
        /// </summary>
        /// <returns>驱动是否安装成功</returns>
        public bool InstallDriver()
        {
            Logging.Info("安装 NF 驱动");

            if (!File.Exists(BinDriver))
                throw new Exception("builtin driver files missing, can't install NF driver");

            Process proc = new Process
            {
                StartInfo =
                {
                    FileName = Path.GetFullPath($"bin\\NetFilterHelper.exe"),
                    WorkingDirectory = $"{Global.ClashDotNetFrameworkDir}\\bin",
                    Arguments = "install",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true,
                    Verb = "runas"
                }
            };
            proc.Start();
            proc.WaitForExit();

            return proc.ExitCode == 0;
        }

        // <summary>
        /// 卸载 NF 驱动
        /// </summary>
        /// <returns>是否成功卸载</returns>
        public bool UninstallDriver()
        {
            Logging.Info("卸载 NF 驱动");

            Process proc = new Process
            {
                StartInfo =
                {
                    FileName = Path.GetFullPath($"bin\\NetFilterHelper.exe"),
                    WorkingDirectory = $"{Global.ClashDotNetFrameworkDir}\\bin",
                    Arguments = "uninstall",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true,
                    Verb = "runas"
                }
            };
            proc.Start();
            proc.WaitForExit();

            return proc.ExitCode == 0;
        }

        /// <summary>
        /// 更新 NF 驱动
        /// </summary>
        /// <returns>是否更新成功</returns>
        public bool ReinstallDriver()
        {
            Logging.Info("更新 NF 驱动");

            Process proc = new Process
            {
                StartInfo =
                {
                    FileName = Path.GetFullPath($"bin\\NetFilterHelper.exe"),
                    WorkingDirectory = $"{Global.ClashDotNetFrameworkDir}\\bin",
                    Arguments = "reinstall",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = true,
                    Verb = "runas"
                }
            };
            proc.Start();
            proc.WaitForExit();

            return proc.ExitCode == 0;
        }

        #endregion
    }
}
