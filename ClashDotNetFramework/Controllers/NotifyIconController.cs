using Clash.SDK.Models.Enums;
using ClashDotNetFramework.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using WindowsProxy;

using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;

namespace ClashDotNetFramework.Controllers
{
    public class NotifyIconController
    {
        public NotifyIcon Icon;
        public ContextMenu Strip;
        public MenuItem proxyModeMenu;

        #region Public Control Function

        public void Start()
        {
            // 初始化菜单
            Strip = new ContextMenu();
            Strip.MenuItems.Add("Dashboard");                                                   // 0
            Strip.MenuItems.Add("-");                                                           // 1
            proxyModeMenu = new MenuItem("Proxy Mode");
            proxyModeMenu.MenuItems.Add("Direct");
            proxyModeMenu.MenuItems.Add("Rule");
            proxyModeMenu.MenuItems.Add("Global");
            Strip.MenuItems.Add(proxyModeMenu);                                                 // 2
            Strip.MenuItems.Add("System Proxy");                                                // 3
            Strip.MenuItems.Add("Process Proxy");                                               // 4
            Strip.MenuItems.Add("-");                                                           // 5
            MenuItem openTerminalMenu = new MenuItem("Open Terminal With Proxy Set Up");
            openTerminalMenu.MenuItems.Add("Cmd");
            openTerminalMenu.MenuItems.Add("PowerShell");
            openTerminalMenu.MenuItems.Add("Windows Terminal");
            openTerminalMenu.MenuItems.Add("Fluent Terminal");
            Strip.MenuItems.Add(openTerminalMenu);                                              // 6
            MenuItem copyCommandMenu = new MenuItem("Copy Proxy Setting Commands");
            copyCommandMenu.MenuItems.Add("Cmd");
            copyCommandMenu.MenuItems.Add("PowerShell");
            Strip.MenuItems.Add(copyCommandMenu);                                               // 7
            MenuItem setTelegramProxyMenu = new MenuItem("Set Telegram Proxy");
            setTelegramProxyMenu.MenuItems.Add("Set Socks5 Proxy To Telegram");
            Strip.MenuItems.Add(setTelegramProxyMenu);                                          // 8
            Strip.MenuItems.Add("-");                                                           // 9
            MenuItem netFilterMenu = new MenuItem("NetFilter Driver");
            netFilterMenu.MenuItems.Add("Install NetFilter Driver");
            netFilterMenu.MenuItems.Add("Uninstall NetFilter Driver");
            Strip.MenuItems.Add(netFilterMenu);                                                 // 10
            Strip.MenuItems.Add("Enable UWP Loopback");                                         // 11
            Strip.MenuItems.Add("-");                                                           // 12
            Strip.MenuItems.Add("Quit");                                                        // 13

            // 设置属性并分配事件
            Strip.MenuItems[0].Click += OnDashboard_Click;

            proxyModeMenu.MenuItems[0].RadioCheck = true;
            proxyModeMenu.MenuItems[1].RadioCheck = true;
            proxyModeMenu.MenuItems[2].RadioCheck = true;

            proxyModeMenu.MenuItems[0].Click += OnProxyMode_Click;
            proxyModeMenu.MenuItems[1].Click += OnProxyMode_Click;
            proxyModeMenu.MenuItems[2].Click += OnProxyMode_Click;

            Strip.MenuItems[3].Click += OnSystemProxy_Click;

            Strip.MenuItems[4].Click += OnProcessProxy_Click;

            openTerminalMenu.MenuItems[0].Click += OpenCmdWithProxySetUp_Click;
            openTerminalMenu.MenuItems[1].Click += OpenPowerShellWithProxySetUp_Click;
            openTerminalMenu.MenuItems[2].Click += OpenWindowsTerminalWithProxySetUp_Click;
            openTerminalMenu.MenuItems[3].Click += OpenFluentTerminalWithProxySetUp_Click;

            copyCommandMenu.MenuItems[0].Click += CopyCmdProxySettingCommands_Click;
            copyCommandMenu.MenuItems[1].Click += CopyPowerShellProxySettingCommands_Click;

            setTelegramProxyMenu.MenuItems[0].Click += SetSocks5ProxyToTelegram_Click;

            netFilterMenu.MenuItems[0].Click += InstallNetFilterDriver_Click;
            netFilterMenu.MenuItems[1].Click += UninstallNetFilterDriver_Click;

            Strip.MenuItems[11].Click += EnableUWPLoopback_Click;

            Strip.MenuItems[13].Click += QuitItem_Click;

            // 初始化图标
            Icon = new NotifyIcon
            {
                Icon = Resources.Normal,
                Visible = false,
                Text = "Clash .NET Framework",
                ContextMenu = Strip
            };

            Icon.MouseDown += Icon_MouseDown;
            Icon.Visible = true;
        }

        public void Stop()
        {
            Icon.MouseDown -= Icon_MouseDown;
            Icon.Visible = false;
        }

        public void SetIcon()
        {
            if (Global.Settings.ProcessProxy || Global.Settings.SystemProxy)
            {
                Icon.Icon = Resources.Proxy;
            }
            else
            {
                Icon.Icon = Resources.Normal;
            }
        }

        #endregion

        #region TrayIcon Event

        private async void Icon_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var window = Application.Current.MainWindow;
                window.Show();
            }
            else if (e.Button == MouseButtons.Right)
            {
                try
                {
                    await InitStatus();
                }
                catch
                {
                }
            }
        }

        #endregion

        #region Internal Function

        private async Task InitStatus()
        {
            var config = await Global.clashClient.GetClashConfigs();
            switch (config.Mode)
            {
                case ModeType.Direct:
                    proxyModeMenu.MenuItems[0].Checked = true;
                    proxyModeMenu.MenuItems[1].Checked = false;
                    proxyModeMenu.MenuItems[2].Checked = false;
                    break;
                case ModeType.Rule:
                    proxyModeMenu.MenuItems[0].Checked = false;
                    proxyModeMenu.MenuItems[1].Checked = true;
                    proxyModeMenu.MenuItems[2].Checked = false;
                    break;
                case ModeType.Global:
                    proxyModeMenu.MenuItems[0].Checked = false;
                    proxyModeMenu.MenuItems[1].Checked = false;
                    proxyModeMenu.MenuItems[2].Checked =true;
                    break;
                default:
                    break;
            }
            Strip.MenuItems[3].Checked = Global.Settings.SystemProxy;
            Strip.MenuItems[4].Checked = Global.Settings.ProcessProxy;
        }

        #endregion

        #region MenuItem Click

        private void OnDashboard_Click(object sender, EventArgs e)
        {
            var window = Application.Current.MainWindow;
            window.Show();
        }

        private async void OnProxyMode_Click(object sender, EventArgs e)
        {
            Dictionary<string, dynamic> dict = new Dictionary<string, dynamic>();
            if (sender == proxyModeMenu.MenuItems[0] && !proxyModeMenu.MenuItems[0].Checked)
            {
                dict.Add("mode", "Direct");
                await Global.clashClient.ChangeClashConfigs(dict);
                proxyModeMenu.MenuItems[0].Checked = true;
                proxyModeMenu.MenuItems[1].Checked = false;
                proxyModeMenu.MenuItems[2].Checked = false;
            }
            else if (sender == proxyModeMenu.MenuItems[1] && !proxyModeMenu.MenuItems[1].Checked)
            {
                dict.Add("mode", "Rule");
                await Global.clashClient.ChangeClashConfigs(dict);
                proxyModeMenu.MenuItems[0].Checked = false;
                proxyModeMenu.MenuItems[1].Checked = true;
                proxyModeMenu.MenuItems[2].Checked = false;
            }
            else if (sender == proxyModeMenu.MenuItems[2] && !proxyModeMenu.MenuItems[2].Checked)
            {
                dict.Add("mode", "Global");
                await Global.clashClient.ChangeClashConfigs(dict);
                proxyModeMenu.MenuItems[0].Checked = false;
                proxyModeMenu.MenuItems[1].Checked = false;
                proxyModeMenu.MenuItems[2].Checked = true;
            }
            else
            {
                // 啥也别干
            }
            Global.Refresh = true;
        }

        private void OnSystemProxy_Click(object sender, EventArgs e)
        {
            if (Strip.MenuItems[3].Checked)
            {
                using var service = new ProxyService();
                service.Direct();
                Global.Settings.SystemProxy = false;
            }
            else
            {
                using var service = new ProxyService
                {
                    Server = $"127.0.0.1:{Global.ClashMixedPort}",
                    Bypass = string.Join(@";", ProxyService.LanIp)
                };
                service.Global();
                Global.Settings.SystemProxy = true;
            }
            Strip.MenuItems[3].Checked = !Strip.MenuItems[3].Checked;
            SetIcon();
        }

        private void OnProcessProxy_Click(object sender, EventArgs e)
        {
            if (Strip.MenuItems[4].Checked)
            {
                Global.nfController.Stop();
                Global.Settings.ProcessProxy = false;
            }
            else
            {
                Global.nfController.Start();
                Global.Settings.ProcessProxy = true;
            }
            Strip.MenuItems[4].Checked = !Strip.MenuItems[4].Checked;
            SetIcon();
        }

        #endregion

        #region Open Terminal With Proxy Set Up

        private void OpenProcessWithProxySetUp(string name)
        {
            try
            {
                var startInfo = new ProcessStartInfo();

                // 设置环境变量
                startInfo.EnvironmentVariables["http_proxy"] = $"http://127.0.0.1:{Global.ClashMixedPort}";
                startInfo.EnvironmentVariables["https_proxy"] = $"http://127.0.0.1:{Global.ClashMixedPort}";

                // 否则环境变量无法被设置
                startInfo.UseShellExecute = false;

                // 设置工作路径
                startInfo.WorkingDirectory = Utils.Utils.GetUserDir();

                // 文件名
                startInfo.FileName = name;

                // 开启进程
                Process.Start(startInfo);
            }
            catch
            {

            }
        }

        private void OpenCmdWithProxySetUp_Click(object sender, EventArgs e)
        {
            OpenProcessWithProxySetUp("cmd.exe");
        }

        private void OpenPowerShellWithProxySetUp_Click(object sender, EventArgs e)
        {
            OpenProcessWithProxySetUp("powershell.exe");
        }

        private void OpenWindowsTerminalWithProxySetUp_Click(object sender, EventArgs e)
        {
            OpenProcessWithProxySetUp("wt.exe");
        }

        private void OpenFluentTerminalWithProxySetUp_Click(object sender, EventArgs e)
        {
            OpenProcessWithProxySetUp("flute.exe");
        }

        #endregion

        #region Copy Proxy Setting Commands

        private void CopyCmdProxySettingCommands_Click(object sender, EventArgs e)
        {
            Clipboard.SetText($"set http_proxy=http://127.0.0.1:{Global.ClashMixedPort} & set https_proxy=http://127.0.0.1:{Global.ClashMixedPort}");
        }

        private void CopyPowerShellProxySettingCommands_Click(object sender, EventArgs e)
        {
            Clipboard.SetText($"$Env:http_proxy=\"http://127.0.0.1:{Global.ClashMixedPort}\";$Env:https_proxy=\"http://127.0.0.1:{Global.ClashMixedPort}\"");
        }

        #endregion

        #region Set Telegram Proxy

        private void SetSocks5ProxyToTelegram_Click(object sender, EventArgs e)
        {
            Process.Start($"tg://socks?server=127.0.0.1&port={Global.ClashMixedPort}");
        }

        #endregion

        #region NetFilter Driver

        private void InstallNetFilterDriver_Click(object sender, EventArgs e)
        {
            try
            {
                Global.nfController.InstallDriver();
            }
            catch
            {

            }
        }

        private void UninstallNetFilterDriver_Click(object sender, EventArgs e)
        {
            try
            {
                Global.nfController.UninstallDriver();
            }
            catch
            {

            }
        }

        #endregion

        #region Enable UWP Loopback

        private void EnableUWPLoopback_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Path.Combine(Global.ClashDotNetFrameworkDir, "bin\\EnableLoopback.exe"));
            }
            catch
            {
            }
        }

        #endregion

        #region Quit

        private void QuitItem_Click(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion
    }
}
