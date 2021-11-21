using Clash.SDK;
using Clash.SDK.Models.Response;
using ClashDotNetFramework.Controllers;
using ClashDotNetFramework.Models.Enums;
using ClashDotNetFramework.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WindowsProxy;

namespace ClashDotNetFramework.Pages
{
    /// <summary>
    /// SettingsPage.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsPage : Page
    {
        public ClashConfigsResponse clashConfigsResponse { get; set; }

        public SettingsPage()
        {
            InitializeComponent();
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateChecker.NewVersionFound += UpdateChecker_NewVersionFound;
            Task.Run(() =>
            {
                // 卸下所有Handler
                DisconnectHandlers();
                // 获取Clash配置
                GetClashConfigs().GetAwaiter().GetResult();
                // 装上所有Handler
                ConnectHandlers();
            });
        }

        #region Handlers

        private void ConnectHandlers()
        {
            StartWithWindows.Checked += StartWithWindows_Checked;
            StartWithWindows.Unchecked += StartWithWindows_Unchecked;
            SystemProxy.Checked += SystemProxy_Checked;
            SystemProxy.Unchecked += SystemProxy_Unchecked;
            ProcessProxy.Checked += ProcessProxy_Checked;
            ProcessProxy.Unchecked += ProcessProxy_Unchecked;
            AllowLAN.Checked += AllowLAN_Checked;
            AllowLAN.Unchecked += AllowLAN_Unchecked;
            IPV6.Checked += IPV6_Unchecked;
            IPV6.Unchecked += IPV6_Unchecked;
        }

        private void DisconnectHandlers()
        {
            StartWithWindows.Checked -= StartWithWindows_Checked;
            StartWithWindows.Unchecked -= StartWithWindows_Unchecked;
            SystemProxy.Checked -= SystemProxy_Checked;
            SystemProxy.Unchecked -= SystemProxy_Unchecked;
            ProcessProxy.Checked -= ProcessProxy_Checked;
            ProcessProxy.Unchecked -= ProcessProxy_Unchecked;
            AllowLAN.Checked -= AllowLAN_Checked;
            AllowLAN.Unchecked -= AllowLAN_Unchecked;
            IPV6.Checked -= IPV6_Unchecked;
            IPV6.Unchecked -= IPV6_Unchecked;
        }

        #endregion

        #region 新版本事件

        private void UpdateChecker_NewVersionFound(object sender, EventArgs e)
        {
            UpdateButton.Visibility = Visibility.Visible;
            UpdateButton.Tag = UpdateChecker.LatestVersionUrl;
        }

        #endregion

        #region 内部函数

        private async Task GetClashConfigs()
        {
            try
            {
                clashConfigsResponse = await Global.clashClient.GetClashConfigs();
                Dispatcher.Invoke(new Action(() =>
                {
                    Version.Text = $"v{UpdateChecker.Version}";
                    ControllerPort.Content = Convert.ToString(Global.ClashControllerPort);
                    MixedPort.Content = Convert.ToString(clashConfigsResponse.MixedPort);
                    LogLevel.Text = Convert.ToString(clashConfigsResponse.LogLevel.ToString());
                    AllowLAN.IsChecked = clashConfigsResponse.AllowLan;
                    IPV6.IsChecked = clashConfigsResponse.IPV6;
                    StartWithWindows.IsChecked = Global.Settings.StartAfterSystemBoot;
                    ProcessProxy.IsChecked = Global.Settings.ProcessProxy;
                    SystemProxy.IsChecked = Global.Settings.SystemProxy;
                    switch (Global.Settings.Theme)
                    {
                        case ThemeType.Classic:
                            ClassicTheme.IsChecked = true;
                            break;
                        case ThemeType.Modern:
                            ModernTheme.IsChecked = true;
                            break;
                        case ThemeType.Dark:
                            DarkTheme.IsChecked = true;
                            break;
                        default:
                            break;
                    }
                    switch (Global.Settings.Language)
                    {
                        case LanguageType.English:
                            EnglishLanguage.IsChecked = true;
                            break;
                        case LanguageType.Chinese:
                            ChineseLanguage.IsChecked = true;
                            break;
                        case LanguageType.Japanese:
                            JapaneseLanguage.IsChecked = true;
                            break;
                        default:
                            break;
                    }
                }));
            }
            catch
            {
            }
        }

        private bool ReconnectClash()
        {
            // 重新初始化ClashClient
            Global.clashClient = new ClashClient(Global.ClashControllerPort);
            // 最高可重试七次, 防止启动时间过长
            bool connected = false;
            int tryTimes = 0;
            while (!connected && tryTimes < 7)
            {
                connected = Global.clashClient.GetClashStatus().GetAwaiter().GetResult();
                // 立即退出循环, 减少1秒时间消耗
                if (connected)
                    break;
                tryTimes++;
                Task.Delay(1000);
            }
            if (connected)
            {
                // 找到被选中的配置
                var selected = Global.Settings.Profile.FirstOrDefault(p => p.IsSelected == true);
                // 切换Clash配置
                Global.clashClient.ReloadClashConfig(false, selected.FileName).GetAwaiter().GetResult();
                // 重新初始化WebSocket连接
                Global.clashClient.GetClashTraffic();
                Global.clashClient.GetClashLog();
                Global.clashClient.GetClashConnection();
                // 重新设置主界面流量事件
                Dispatcher.Invoke(new Action(() =>
                {
                    dynamic window = Application.Current.MainWindow;
                    window.InitEvent();
                }));
            }
            return connected;
        }

        #endregion

        #region 点击事件

        private void CheckMMDBUpdate_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            button.Content = "Updating...";

            string tmpPath = Path.Combine(Utils.Utils.GetClashConfigDir(), "Country.mmdb.tmp");
            string mmdbPath = Path.Combine(Utils.Utils.GetClashConfigDir(), "Country.mmdb");
            Task.Run(() =>
            {
                try
                {
                    var req = WebUtil.CreateRequest(Global.Settings.MMDBUpdateUrl);
                    var result = WebUtil.DownloadString(req, out var rep);
                    // 写入Country.mmdb.tmp
                    File.WriteAllText(tmpPath, result);
                    // 设置按钮文字
                    Dispatcher.Invoke(() =>
                    {
                        button.Content = "Stoping Clash...";
                    });
                    // 杀死Clash进程
                    Global.clashController.Stop();
                    // 如果存在就删除Country.mmdb
                    if (File.Exists(mmdbPath))
                    {
                        File.Delete(mmdbPath);
                    }
                    // 重命名Country.mmdb.tmp到Country.mmdb
                    File.Move(tmpPath, mmdbPath);
                    // 设置按钮文字
                    Dispatcher.Invoke(() =>
                    {
                        button.Content = "Starting Clash...";
                    });
                    // 重新启动Clash
                    Global.clashController.Start(Global.ClashControllerPort);
                    // 设置按钮文字
                    Dispatcher.Invoke(() =>
                    {
                        button.Content = "Connecting...";
                    });
                    // 重新连接Clash
                    bool status = ReconnectClash();
                    if (status)
                    {
                        // 设置按钮文字
                        Dispatcher.Invoke(() =>
                        {
                            button.Content = "Done";
                        });
                    }
                    else
                    {
                        // 设置按钮文字
                        Dispatcher.Invoke(() =>
                        {
                            button.Content = "Timeout";
                        });
                    }
                }
                catch
                {
                    Dispatcher.Invoke(() =>
                    {
                        button.Content = "Error";
                    });
                }
            });
        }

        private void ResetAllSettings_Click(object sender, RoutedEventArgs e)
        {
            AllowLAN.IsChecked = false;
            IPV6.IsChecked = false;
            StartWithWindows.IsChecked = false;
            ProcessProxy.IsChecked = false;
            SystemProxy.IsChecked = false;
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string url = button.Tag.ToString();
            if (!string.IsNullOrWhiteSpace(url))
            {
                Process.Start(url);
            }
        }

        private void ControllerPort_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText($"http://127.0.0.1:{Global.ClashControllerPort}");
        }

        private void MixedPort_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StartWithWindows_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key.SetValue("Clash .NET Framework", Process.GetCurrentProcess().MainModule.FileName);
            }
            catch
            {

            }
            Global.Settings.StartAfterSystemBoot = true;
        }

        private void StartWithWindows_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                key.DeleteValue("Clash .NET Framework");
            }
            catch
            {

            }
            Global.Settings.StartAfterSystemBoot = false;
        }

        private async void IPV6_Checked(object sender, RoutedEventArgs e)
        {
            var dict = new Dictionary<string, dynamic>();
            dict.Add("ipv6", true);

            await Global.clashClient.ChangeClashConfigs(dict);
        }

        private async void IPV6_Unchecked(object sender, RoutedEventArgs e)
        {
            var dict = new Dictionary<string, dynamic>();
            dict.Add("ipv6", false);

            await Global.clashClient.ChangeClashConfigs(dict);
        }

        private async void AllowLAN_Checked(object sender, RoutedEventArgs e)
        {
            var dict = new Dictionary<string, dynamic>();
            dict.Add("allow-lan", true);

            await Global.clashClient.ChangeClashConfigs(dict);
        }

        private async void AllowLAN_Unchecked(object sender, RoutedEventArgs e)
        {
            var dict = new Dictionary<string, dynamic>();
            dict.Add("allow-lan", false);

            await Global.clashClient.ChangeClashConfigs(dict);
        }

        private void UWPLoopback_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(Path.Combine(Global.ClashDotNetFrameworkDir, "bin\\EnableLoopback.exe"));
            }
            catch
            {
            }
        }

        private void NetFilterSDK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Global.nfController.InstallDriver();
            }
            catch
            {

            }
        }

        private void ProcessProxy_Checked(object sender, RoutedEventArgs e)
        {
            Global.nfController.Start();
            Global.Settings.ProcessProxy = true;
            Global.iconController.SetIcon();
        }

        private void ProcessProxy_Unchecked(object sender, RoutedEventArgs e)
        {
            Global.nfController.Stop();
            Global.Settings.ProcessProxy = false;
            Global.iconController.SetIcon();
        }

        private void ClassicTheme_Click(object sender, RoutedEventArgs e)
        {
            if (Global.Settings.Theme != ThemeType.Classic)
            {
                Global.Settings.Theme = ThemeType.Classic;
                (App.Current as App).ChangeTheme(ThemeType.Classic);
            }
        }

        private void ModernTheme_Click(object sender, RoutedEventArgs e)
        {
            if (Global.Settings.Theme != ThemeType.Modern)
            {
                Global.Settings.Theme = ThemeType.Modern;
                (App.Current as App).ChangeTheme(ThemeType.Modern);
            }
        }

        private void DarkTheme_Click(object sender, RoutedEventArgs e)
        {
            if (Global.Settings.Theme != ThemeType.Dark)
            {
                Global.Settings.Theme = ThemeType.Dark;
                (App.Current as App).ChangeTheme(ThemeType.Dark);
            }
        }

        private void EnglishLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (Global.Settings.Language != LanguageType.English)
            {
                Global.Settings.Language = LanguageType.English;
                (App.Current as App).ChangeLanguage(LanguageType.English);
            }
        }

        private void ChineseLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (Global.Settings.Language != LanguageType.Chinese)
            {
                Global.Settings.Language = LanguageType.Chinese;
                (App.Current as App).ChangeLanguage(LanguageType.Chinese);
            }
        }

        private void JapaneseLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (Global.Settings.Language != LanguageType.Japanese)
            {
                Global.Settings.Language = LanguageType.Japanese;
                (App.Current as App).ChangeLanguage(LanguageType.Japanese);
            }
        }

        private void SystemProxy_Checked(object sender, RoutedEventArgs e)
        {
            using var service = new ProxyService
            {
                Server = $"127.0.0.1:{clashConfigsResponse.MixedPort}",
                Bypass = string.Join(@";", ProxyService.LanIp)
            };
            service.Global();
            Global.Settings.SystemProxy = true;
            Global.iconController.SetIcon();
        }

        private void SystemProxy_Unchecked(object sender, RoutedEventArgs e)
        {
            using var service = new ProxyService();
            service.Direct();
            Global.Settings.SystemProxy = false;
            Global.iconController.SetIcon();
        }

        #endregion
    }
}
