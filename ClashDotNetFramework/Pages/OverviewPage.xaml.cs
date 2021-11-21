using Clash.SDK.Models.Events;
using ClashDotNetFramework.Utils;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
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
    /// OverviewPage.xaml 的交互逻辑
    /// </summary>
    public partial class OverviewPage : Page
    {
        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> YFormatter { get; set; }

        public OverviewPage()
        {
            InitializeComponent();
            DataContext = this;
            YFormatter = value => $"{UnitConverter.FormatBytes(value)}/s";
            SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Upload",
                    Values = new ChartValues<double> { 0.00 },
                    LineSmoothness = 1
                },
                new LineSeries
                {
                    Title = "Download",
                    Values = new ChartValues<double> { 0.00 },
                    LineSmoothness = 1
                }
            };
        }

        private void OverviewPage_Loaded(object sender, RoutedEventArgs e)
        {
            Global.clashClient.TrafficReceivedEvt += OnTrafficUpdated;
        }

        private void OverviewPage_Unloaded(object sender, RoutedEventArgs e)
        {
            Global.clashClient.TrafficReceivedEvt -= OnTrafficUpdated;
        }

        private void OnTrafficUpdated(object sender, TrafficEvtArgs e)
        {
            Task.Run(() =>
            {
                var traffic = e.Response;
                Dispatcher.Invoke(new Action(() =>
                {
                    if (SeriesCollection[0].Values.Count > 30 && SeriesCollection[1].Values.Count > 30)
                    {
                        SeriesCollection[0].Values.Clear();
                        SeriesCollection[1].Values.Clear();
                    }
                    SeriesCollection[0].Values.Add(Convert.ToDouble(traffic.Up));
                    SeriesCollection[1].Values.Add(Convert.ToDouble(traffic.Down));
                }));
            });
        }
    }
}
