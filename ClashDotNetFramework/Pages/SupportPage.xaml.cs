using ClashDotNetFramework.Models.Responses;
using ClashDotNetFramework.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
    /// SupportPage.xaml 的交互逻辑
    /// </summary>
    public partial class SupportPage : Page
    {
        public SupportPage()
        {
            InitializeComponent();
        }

        private void SupportPage_Loaded(object sender, RoutedEventArgs e)
        {
            // 获取广告
            Task.Run(() =>
            {
                GetAdvertisement();
            });
        }

        private void GetAdvertisement()
        {
            try
            {
                var req = WebUtil.CreateRequest("https://raw.githubusercontent.com/CoelWu/ads/main/ads.json");

                var result = WebUtil.DownloadString(req, out var rep);
                if (rep.StatusCode == HttpStatusCode.OK)
                {
                    var airportResponse = JsonConvert.DeserializeObject<AdvertisementResponse>(result);
                    if (airportResponse.Airports.Count > 1)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            MajorAdvertisement.Source = new BitmapImage(new Uri(airportResponse.Airports[0].Image, UriKind.RelativeOrAbsolute));
                            MajorAdvertisement.Tag = airportResponse.Airports[0].Url;
                            MinorAdvertisement.Source = new BitmapImage(new Uri(airportResponse.Airports[1].Image, UriKind.RelativeOrAbsolute));
                            MinorAdvertisement.Tag = airportResponse.Airports[1].Url;
                        }));
                    }
                    else if (airportResponse.Airports.Count == 1)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            MajorAdvertisement.Source = new BitmapImage(new Uri(airportResponse.Airports[0].Image, UriKind.RelativeOrAbsolute));
                            MajorAdvertisement.Tag = airportResponse.Airports[0].Url;
                        }));
                    }
                }
            }
            catch
            {

            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            string url = e.Uri.ToString();
            if (!string.IsNullOrWhiteSpace(url))
            {
                Process.Start(url);
            }
            e.Handled = true;
        }

        private void Advertisement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = (Image)sender;
            string url = image.Tag.ToString();
            if (!string.IsNullOrWhiteSpace(url))
            {
                Process.Start(url);
            }
        }
    }
}
