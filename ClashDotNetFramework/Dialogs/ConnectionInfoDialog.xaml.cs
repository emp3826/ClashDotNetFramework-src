using ClashDotNetFramework.Models.Items;
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
using System.Windows.Shapes;

namespace ClashDotNetFramework.Dialogs
{
    /// <summary>
    /// ConnectionInfoDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectionInfoDialog : Window
    {
        public string Host { get; set; }
        
        public string Upload { get; set; }

        public string Download { get; set; }

        public string Source { get; set; }

        public string Destination { get; set; }

        public string Rule { get; set; }

        public string Chains { get; set; }

        public ConnectionInfoDialog()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;

            DataContext = this;
        }

        public ConnectionInfoDialog(ConnectionItem connectionItem) : this()
        {
            Host = connectionItem.Host;
            Upload = connectionItem.Upload;
            Download = connectionItem.Download;
            Source = connectionItem.Source;
            Destination = connectionItem.Destination;
            Rule = connectionItem.Rule;
            Chains = string.Join(" - ", connectionItem.Chains.ToArray().Reverse());
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Hide();
            }
        }
    }
}
