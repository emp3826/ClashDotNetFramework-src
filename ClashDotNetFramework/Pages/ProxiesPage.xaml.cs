using Clash.SDK.Extensions;
using Clash.SDK.Models.Enums;
using Clash.SDK.Models.Response;
using ClashDotNetFramework.Models.Items;
using ClashDotNetFramework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ClashDotNetFramework.Pages
{
    /// <summary>
    /// ProxiesPage.xaml 的交互逻辑
    /// </summary>
    public partial class ProxiesPage : Page
    {
        private ModeType Mode = ModeType.Rule;
        private ObservableCollection<ProxyItem> ProxyItemCollection = new ObservableCollection<ProxyItem>();
        public ICollectionView CollectionView { get; }

        public ProxiesPage()
        {
            // 初始化部件
            InitializeComponent();
            // 手动设置Items源
            ProxyListView.ItemsSource = ProxyItemCollection;
            // 设置Collection View
            CollectionView = CollectionViewSource.GetDefaultView(ProxyItemCollection);
            // 设置Group
            CollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Global.Refresh)
            {
                Task.Run(async () =>
                {
                    // 获取当前模式
                    await GetModeType();
                });
                Task.Run(async () =>
                {
                    // 清除所有节点
                    ClearProxies();
                    // 获取所有节点
                    await GetProxies();
                });
                Global.Refresh = false;
            }
            else
            {
                // 随机生成颜色
                ColorUtil.GenerateRandomColor();
                foreach (var proxy in ProxyItemCollection)
                {
                    proxy.NotifyBackgroundChange();
                }
            }
        }

        private async Task ChangeClashMode(ModeType mode)
        {
            try
            {
                var dict = new Dictionary<string, dynamic>();
                dict.Add("mode", mode.ToString());
                await Global.clashClient.ChangeClashConfigs(dict);
            }
            catch
            {
            }
        }

        private void DirectButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Mode = ModeType.Direct;
                ClearProxies();
                ChangeClashMode(ModeType.Direct).GetAwaiter().GetResult();
                GetProxies().GetAwaiter().GetResult();
            });
        }

        private void RuleButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Mode = ModeType.Rule;
                ClearProxies();
                ChangeClashMode(ModeType.Rule).GetAwaiter().GetResult();
                GetProxies().GetAwaiter().GetResult();
            });
        }

        private void GlobalButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Mode = ModeType.Global;
                ClearProxies();
                ChangeClashMode(ModeType.Global).GetAwaiter().GetResult();
                GetProxies().GetAwaiter().GetResult();
            });
        }

        private async Task GetModeType()
        {
            try
            {
                var config = await Global.clashClient.GetClashConfigs();
                switch (config.Mode)
                {
                    case ModeType.Direct:
                        Dispatcher.Invoke(new Action(() => { DirectButton.IsChecked = true; }));
                        break;
                    case ModeType.Rule:
                        Dispatcher.Invoke(new Action(() => { RuleButton.IsChecked = true; }));
                        break;
                    case ModeType.Global:
                        Dispatcher.Invoke(new Action(() => { GlobalButton.IsChecked = true; }));
                        break;
                    default:
                        break;
                }
                Mode = config.Mode;
            }
            catch
            {
            }
        }

        private void ClearProxies(bool regenerateColor = true)
        {
            if (ProxyItemCollection.Count > 0)
                Dispatcher.Invoke(new Action(() =>
                {
                    ProxyItemCollection.Clear();
                }));
            // 随机生成颜色
            if (regenerateColor)
                ColorUtil.GenerateRandomColor();
        }

        private async Task GetProxies()
        {
            try
            {
                var proxies = await Global.clashClient.GetClashProxies();
                if (Mode == ModeType.Direct)
                {
                    return;
                }
                else if (Mode == ModeType.Rule)
                {
                    var groups = proxies.Proxies.Where(p => p.Type.IsPolicyGroup() && p.Name != "GLOBAL");
                    foreach (var group in groups)
                    {
                        foreach (var proxyName in group.All)
                        {
                            var matchedProxy = proxies.Proxies.FirstOrDefault(p => p.Name == proxyName);
                            ProxyItem proxyItem = new ProxyItem
                            {
                                Name = proxyName,
                                Type = matchedProxy.Type.IsPolicyGroup() ? $"{matchedProxy.Type} - {matchedProxy.Now}" : matchedProxy.Type.ToString(),
                                Group = group.Name,
                                Latency = matchedProxy.History.Count > 0 ? matchedProxy.History.First().Delay : -2,
                                IsSelected = matchedProxy.Name == group.Now
                            };
                            Dispatcher.Invoke(new Action(() =>
                            {
                                ProxyItemCollection.Add(proxyItem);
                            }));
                        }
                    }
                } 
                else if (Mode == ModeType.Global)
                {
                    var proxy = proxies.Proxies.FirstOrDefault(p => p.Name == "GLOBAL");
                    foreach (var proxyName in proxy.All)
                    {
                        var matchedProxy = proxies.Proxies.FirstOrDefault(p => p.Name == proxyName);
                        ProxyItem proxyItem = new ProxyItem
                        {
                            Name = proxyName,
                            Type = matchedProxy.Type.IsPolicyGroup() ? $"{matchedProxy.Type} - {matchedProxy.Now}" : matchedProxy.Type.ToString(),
                            Group = proxy.Name,
                            Latency = matchedProxy.History.Count > 0 ? matchedProxy.History.First().Delay : -2,
                            IsSelected = matchedProxy.Name == proxy.Now
                        };
                        Dispatcher.Invoke(new Action(() =>
                        {
                            ProxyItemCollection.Add(proxyItem);
                        }));
                    }
                }
            }
            catch
            {
            }
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            try
            {
                var result = await Global.clashClient.GetClashProxyDelay(button.Tag.ToString());
                if (result.DelayLong <= 0)
                {
                    button.Content = "Timeout";
                    button.Foreground = Brushes.Red;
                }
                else
                {
                    button.Content = $"{result.Delay} ms";
                    button.Foreground = Brushes.White;
                }
            }
            catch
            {
                button.Content = "Timeout";
                button.Foreground = Brushes.Red;
            }
        }

        private async void ProxyListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ProxyItem)ProxyListView.SelectedItem;
            if (item != null)
            {
                try
                {
                    foreach (var p in ProxyItemCollection)
                    {
                        if (p.Group == item.Group)
                        {
                            p.IsSelected = false;
                        }
                    }
                    item.IsSelected = true;
                    await Global.clashClient.SwitchClashProxy(item.Group, item.Name);
                }
                catch
                {
                }
            }
        }
    }
}
