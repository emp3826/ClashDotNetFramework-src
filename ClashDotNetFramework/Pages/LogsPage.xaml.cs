using Clash.SDK.Models.Events;
using ClashDotNetFramework.Models.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ClashDotNetFramework.Pages
{
    /// <summary>
    /// LogsPage.xaml 的交互逻辑
    /// </summary>
    public partial class LogsPage : Page
    {
        private bool IsLogEnabled = true;
        private ObservableCollection<LogItem> LogItemCollection = new ObservableCollection<LogItem>();

        public LogsPage()
        {
            InitializeComponent();

            LogItemListView.ItemsSource = LogItemCollection;
        }

        private void LogsPage_Load(object sender, RoutedEventArgs e)
        {
            Global.clashClient.LoggingReceivedEvt += OnLogReceived;
            Task.Run(async () =>
            {
                // 获取配置
                var config = await Global.clashClient.GetClashConfigs();
                Dispatcher.Invoke(new Action(() =>
                {
                    CurrentMode.Text = $"Mode: {config.Mode}";
                }));
            });
        }

        private void LogsPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Global.clashClient.LoggingReceivedEvt -= OnLogReceived;
        }

        private void OnLogReceived(object sender, LoggingEvtArgs e)
        {
            var log = e.Response;
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (LogItemCollection.Count > Global.Settings.MaximumLog)
                        LogItemCollection.Clear();
                    LogItem logItem = new LogItem
                    {
                        Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Level = log.Type.ToString(),
                        PayLoad = log.PayLoad
                    };
                    LogItemCollection.Add(logItem);
                }));
            }
            catch
            {
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            if (LogItemCollection.Count > 0)
                LogItemCollection.Clear();
        }

        private void StatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsLogEnabled)
            {
                Global.clashClient.LoggingReceivedEvt -= OnLogReceived;
                StatusButton.Content = "Start";
                StatusButton.Background = new SolidColorBrush(Color.FromRgb(23, 155, 187));
            }
            else
            {
                Global.clashClient.LoggingReceivedEvt += OnLogReceived;
                StatusButton.Content = "Stop";
                StatusButton.Background = new SolidColorBrush(Color.FromRgb(245, 99, 99));
            }
            IsLogEnabled = !IsLogEnabled;
        }
    }
}
