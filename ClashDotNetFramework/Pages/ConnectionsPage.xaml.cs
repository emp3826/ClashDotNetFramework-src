using Clash.SDK.Models.Events;
using ClashDotNetFramework.Dialogs;
using ClashDotNetFramework.Models.Items;
using ClashDotNetFramework.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// ConnectionsPage.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectionsPage : Page
    {
        private ObservableCollection<ConnectionItem> ConnectionItemCollection = new ObservableCollection<ConnectionItem>();

        public ConnectionsPage()
        {
            InitializeComponent();

            ConnectionItemListView.ItemsSource = ConnectionItemCollection;
        }

        private void ConnectionsPage_Loaded(object sender, RoutedEventArgs e)
        {
            Global.clashClient.ConnectionUpdatedEvt += OnConnectionUpdated;
        }

        private void ConnectionsPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Global.clashClient.ConnectionUpdatedEvt -= OnConnectionUpdated;
        }

        private void OnConnectionUpdated(object sender, ConnectionEvtArgs e)
        {
            var response = e.Response;
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    TotalTraffic.Text = $"Total: ↑{UnitConverter.FormatBytes(response.UploadTotal)} ↓{UnitConverter.FormatBytes(response.DownloadTotal)}";
                    ConnectionItemCollection.Clear();
                    foreach (var connection in response.Connections)
                    {
                        string targetHost = (!string.IsNullOrWhiteSpace(connection.MetaData.Host) ? connection.MetaData.Host : connection.MetaData.DestinationIP) + ":" + connection.MetaData.DestinationPort;
                        string upload = UnitConverter.FormatBytes(connection.Upload);
                        string download = UnitConverter.FormatBytes(connection.Download);
                        ConnectionItem connectionItem = new ConnectionItem
                        {
                            UUID = connection.Id,
                            Network = connection.MetaData.Network.ToUpper(),
                            Type = connection.MetaData.Type,
                            Host = connection.MetaData.Host,
                            Source = $"{connection.MetaData.SourceIP}:{connection.MetaData.SourcePort}",
                            Destination = $"{connection.MetaData.DestinationIP}:{connection.MetaData.DestinationPort}",
                            TargetHost = targetHost,
                            Chains = connection.Chains,
                            FinalNode = connection.Chains.FirstOrDefault(),
                            Rule = connection.Rule,
                            Time = TimeConverter.ParseTime(Convert.ToDateTime(connection.Start)),
                            Upload = upload,
                            Download = download,
                            Speed = $"↑{upload}/s ↓{download}/s"
                        };
                        ConnectionItemCollection.Add(connectionItem);
                    }
                }));
            }
            catch
            {
            }
        }

        private async void CloseAllButton_Click(object sender, RoutedEventArgs e)
        {
            await Global.clashClient.DisconnectAllConnections();
        }

        private void ConnectionItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ConnectionItem)ConnectionItemListView.SelectedItem;
            if (item != null)
            {
                var dlg = new ConnectionInfoDialog(item);
                dlg.ShowDialog();
            }
        }
    }
}
