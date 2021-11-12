using Clash.SDK;
using Clash.SDK.Models.Events;
using ClashDotNetFramework.Controllers;
using ClashDotNetFramework.Pages;
using ClashDotNetFramework.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using WindowsProxy;

namespace ClashDotNetFramework
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer connectionTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(0.5) };
        private DateTime BeginStamp = DateTime.MinValue;

        public MainWindow()
        {
            // 初始化其他界面
            Global.Pages.Init();

            // 显示托盘图标
            Global.iconController.Start();

            // 初始化界面
            InitializeComponent();

            // 设置标题
            Title += $" v{UpdateChecker.Version}";

            // 初始化ClashClient, 系统和进程代理
            InitClash();

            // 导航到Overview页面
            PageFrame.Navigate(Global.Pages.overviewPage);

            // 后台检查更新
            Task.Run(() =>
            {
                UpdateChecker.Check(false);
            });
        }

        #region 初始化函数

        public void InitEvent()
        {
            // 设置流量更新事件
            Global.clashClient.TrafficReceivedEvt -= OnTrafficUpdated;
            Global.clashClient.TrafficReceivedEvt += OnTrafficUpdated;
        }

        private void InitClash()
        {
            // 启动Clash控制器
            Global.clashClient = new ClashClient(Global.ClashControllerPort);
            // 设置Clash计时器和状态
            Task.Run(() =>
            {
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
                    // 获取Clash混合端口
                    GetClashMixedPort();
                    // 找到被选中的配置
                    var selected = Global.Settings.Profile.FirstOrDefault(p => p.IsSelected == true);
                    // 切换Clash配置
                    Global.clashClient.ReloadClashConfig(false, selected.FileName).GetAwaiter().GetResult();
                    // 开启计时器
                    InitTimer();
                    // 设置状态
                    InitStatus();
                    // 设置版本
                    InitVersion();
                    // 初始化WebSocket连接
                    Global.clashClient.GetClashTraffic();
                    Global.clashClient.GetClashLog();
                    Global.clashClient.GetClashConnection();
                    // 设置流量更新事件
                    Global.clashClient.TrafficReceivedEvt += OnTrafficUpdated;
                    // 初始化系统和进程代理
                    InitProxy();
                }
                else
                {
                    InitStatus(false);
                }
            });
        }

        private void InitProxy()
        {
            // 初始化NetFilter控制器
            Global.nfController = new NFController();
            // 在新进程中开启系统代理和进程代理
            Task.Run(() =>
            {
                if (Global.Settings.SystemProxy)
                {
                    using var service = new ProxyService
                    {
                        Server = $"127.0.0.1:{Global.ClashMixedPort}",
                        Bypass = string.Join(@";", ProxyService.LanIp)
                    };
                    service.Global();
                }
                if (Global.Settings.ProcessProxy)
                {
                    Global.nfController.Start();
                }
                Global.iconController.SetIcon();
            });
        }

        private void GetClashMixedPort()
        {
            var resp = Global.clashClient.GetClashConfigs().GetAwaiter().GetResult();
            Global.ClashMixedPort = resp.MixedPort;
        }

        private void InitTimer()
        {
            BeginStamp = DateTime.Now;
            connectionTimer.Tick += ConnectionTimer_Tick;
            connectionTimer.Start();
        }

        private void InitStatus(bool connected = true)
        {
            if (connected)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    StatusLight.Fill = new SolidColorBrush(Color.FromRgb(65, 184, 131));
                    StatusText.Text = "Connected";
                }));
            }
            else
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    StatusLight.Fill = Brushes.Red;
                    StatusText.Text = "Disconnected";
                }));
            }
        }

        private void InitVersion()
        {
            try
            {
                var response = Global.clashClient.GetClashVersion().GetAwaiter().GetResult();
                Dispatcher.Invoke(new Action(() =>
                {
                    CoreVersionText.Text = $"Core Version: {response.Version}";
                }));
            }
            catch
            {
            }
        }

        #endregion

        #region 重写退出函数

        protected override void OnClosing(CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        #endregion

        #region 控件事件

        private void ConnectionTimer_Tick(object sender, object e)
        {
            if (BeginStamp == DateTime.MinValue)
                return;
            var ts = DateTime.Now - BeginStamp;
            ConnectionTime.Text = $"{ts.Hours:00} : {ts.Minutes:00} : {ts.Seconds:00}";
        }

        private void OnTrafficUpdated(object sender, TrafficEvtArgs e)
        {
            var traffic = e.Response;
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    UploadSpeed.Text = $"{UnitConverter.FormatBytes(traffic.Up)}/s";
                    DownloadSpeed.Text = $"{UnitConverter.FormatBytes(traffic.Down)}/s";
                }));
            }
            catch
            {
            }
        }

        #endregion

        #region 键盘按下事件

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Hide();
            }
        }

        #endregion

        #region 按钮事件

        private void OverviewButton_Checked(object sender, RoutedEventArgs e)
        {
            if (PageFrame != null)
            {
                PageFrame.Navigate(Global.Pages.overviewPage);
            }
        }

        private void ProxiesButton_Checked(object sender, RoutedEventArgs e)
        {
            if (PageFrame != null)
            {
                PageFrame.Navigate(Global.Pages.proxiesPage);
            }
        }

        private void ProfilesButton_Checked(object sender, RoutedEventArgs e)
        {
            if (PageFrame != null)
            {
                PageFrame.Navigate(Global.Pages.profilesPage);
            }
        }

        private void RulesButton_Checked(object sender, RoutedEventArgs e)
        {
            if (PageFrame != null)
            {
                PageFrame.Navigate(Global.Pages.rulesPage);
            }
        }

        private void LogsButton_Checked(object sender, RoutedEventArgs e)
        {
            if (PageFrame != null)
            {
                PageFrame.Navigate(Global.Pages.logsPage);
            }
        }

        private void ConnectionsButton_Checked(object sender, RoutedEventArgs e)
        {
            if (PageFrame != null)
            {
                PageFrame.Navigate(Global.Pages.connectionsPage);
            }
        }

        private void SettingsButton_Checked(object sender, RoutedEventArgs e)
        {
            if (PageFrame != null)
            {
                PageFrame.Navigate(Global.Pages.settingsPage);
            }
        }

        private void SupportButton_Checked(object sender, RoutedEventArgs e)
        {
            if (PageFrame != null)
            {
                PageFrame.Navigate(Global.Pages.supportPage);
            }
        }

        #endregion
    }
}
